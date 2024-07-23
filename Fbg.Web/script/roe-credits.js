/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.6.4-vsdoc.js"/>
/// <reference path="bda-ui-gesture-swipe.js"/>
/// <reference path="bda-ui-scrolling.js"/>
/// <reference path="bda-ui-transition.js"/>
/// <reference path="roe-api.js"/>
/// <reference path="BDA.js" />
/// <reference path="BDA-exception.js" />
/// <reference path="BDA-utils.js" />
/// <reference path="roe-player.js" />
/// <reference path="roe-frame.js" />
/// <reference path="roe-frame_m.js" />
/// <reference path="countdown.js"/>

(function (ROE) {
}(window.ROE = window.ROE || {}));

(function (obj) {
    var Utils = window.BDA.Utils;

    var Api = window.ROE.Api,
        Entities = window.ROE.Entities,
        Frame = window.ROE.Frame,
        Player = window.ROE.Player;

    var CONST = {},
        CACHE = {};
    CONST.Enum = {};

    var CONST = {
        popupName: "Credits",
        popupTransferName: "CreditTransfer",
        transferLookupThreshold: 500,
    },
    CACHE = {};

    CONST.Selector = {
        panelDescription: ".themeM-panel.description",
        panelCreditPackages: ".themeM-panel.creditpackages",
        panelCreditPackageRow: ".creditpackagerow",
        panelCreditPackage: ["#creditpackage1", "#creditpackage2", "#creditpackage3"],
        credits: ".credits",
        price: ".price",
        foreground: ".fg",
        background: ".bg",
        hidden: ".hidden",
        offer2Message: '.offer2Msg',
        otherPayment: ".otherPayment"
    };

    CONST.CssClass = {
        template: "template",
        hidden: "hidden"
    };

    var _template_Page; // jquery object
    var _popupContent;

    // Transfer creds
    var _maxTransferServants = 0;
    var _matchingPlayerFound = false;
    var _transferAmount = 0;
    var _transferToPlayer = "";

    function _showPopup() {
        _template_Page = $(BDA.Templates.getRaw("Credits", ROE.realmID));

        popupModalPopup(CONST.popupName, "SERVANTS", undefined, undefined, undefined, undefined, closeModalPopupAndReloadHeaderAndView, "https://static.realmofempires.com/images/icons/M_Servants.png");

        _sync();



    }

    function _sync() {
        Frame.busy();
        Api.call_getCreditPackages(_sync_onDataSuccess);

    }

    function _sync_onDataSuccess(data) {
        _populate(data);
        Frame.free();

        $(CONST.Selector.otherPayment).click(function () {

            var content = $("#popup_Credits .otherPaymentContent").html();

            ROE.Frame.simplePopopOverlay("https://static.realmofempires.com/images/icons/M_servants.png", "Payment Methods", content);

            $(".otherPaymentContentClick").click(function () {

                ROE.Api.call("MobilePayment", {}, _mobilePaymentcallBack);
            });
        });
    }

    function _mobilePaymentcallBack(response) {

        console.log(response)
    }


    function _populate(data) {

        _popupContent = $("#" + ROE.Frame.CONSTS.popupNameIDPrefix + CONST.popupName + ' .popupBody');

        var content = BDA.Templates.populate(_template_Page[0].outerHTML, data);
        content = $(content);

        if (!ROE.Player.hasOffer2) {
            content.find(CONST.Selector.offer2Message).remove();
        }

        if (ROE.Player.recEmail_isKLogin) {
            content.find('#typicalPurchase').remove();
        } else {
            content.find('#kongPurchase').remove();

            var creditPackageRow;
            var j = 0;
            var saleType = data.st; // 0 regular, 2 sale 

            for (var i = 0; i < data.CreditPackages.length; i++) {

                //public string ProductID;
                //public int Credits;
                //public int SaleType;
                //public double Price;

                //com.realmofempires.roedroid.100servants	100	0	5
                //com.realmofempires.roedroid.10servants	10	0	1
                //com.realmofempires.roedroid.30servants	30	0	3

                var productID = data.CreditPackages[i].ProductID;
                var credits = data.CreditPackages[i].Credits;

                var price = data.CreditPackages[i].Price;
                var icon = data.CreditPackages[i].Icon;
                var template = content.find(CONST.Selector.panelCreditPackageRow)
                        .filter("." + CONST.CssClass.template);

                j = i % 3;

                //  new row needed
                if (j == 0) {
                    creditPackageRow = template.clone()
                        .removeClass(CONST.CssClass.template);
                    creditPackageRow.insertAfter(template.parent().children(CONST.Selector.panelCreditPackageRow).last());
                }

                var creditPackage = creditPackageRow.find(CONST.Selector.panelCreditPackage[j])

                creditPackage
                    .find(CONST.Selector.hidden)
                        .removeClass(CONST.CssClass.hidden)

                creditPackage
                    .children(CONST.Selector.background)
                        .show();

                creditPackage
                    .children(CONST.Selector.foreground)
                        .children("img")
                            .attr("src", icon);

                creditPackage
                    .attr("onclick", "ROE.Frame.purchase('" + productID + "')");

                creditPackage
                    .children(CONST.Selector.foreground)
                        .children(CONST.Selector.credits)
                            .html(credits + "<BR> Servants")

                if (saleType == 1 && credits !== data.CreditPackagesRegular[i].Credits) {
                    creditPackage
                        .children(CONST.Selector.foreground)
                            .children(CONST.Selector.credits).addClass('saleServants');
                }

                creditPackage
                    .children(CONST.Selector.foreground)
                        .children(CONST.Selector.price)
                            .text("$" + price)

                if (saleType == 2 && price !== data.CreditPackagesRegular[i].Price) {
                    //
                    // sale
                    //
                    creditPackage.find(CONST.Selector.price).addClass('sale');
                    creditPackage.find('.salePrice').text("$" + price)

                    creditPackage
                        .children(CONST.Selector.foreground)
                            .children(CONST.Selector.price)
                                .text("$" + data.CreditPackagesRegular[i].Price)
                }
                else if (saleType == 2 && price === data.CreditPackagesRegular[i].Price) {
                    //
                    // sale, nut this item is not on sale 
                    //

                    //dont hide this on purpose
                    // creditPackage.find('.salePrice').hide(); 

                    creditPackage
                        .children(CONST.Selector.foreground)
                            .children(CONST.Selector.price)
                                .text("$" + price)
                } else {
                    //
                    // no sales
                    //
                    creditPackage.find('.salePrice').hide();

                    creditPackage
                        .children(CONST.Selector.foreground)
                            .children(CONST.Selector.price)
                                .text("$" + price)
                }
            }


            if (saleType == 0) {
                //
                // no sale
                //
                content.find('.headerMessageSale').hide();
            } else {
                //
                // some sale
                //
                content.find('.headerMessage').hide();
            }
        }

        _popupContent.html(content);
    }


   
    function _showTransferPopup() {
        _transferAmount = 0;
        _transferToPlayer = "";

        var credTrans_template = $(BDA.Templates.getRaw("CreditTransfer", ROE.realmID));
     
        popupModalPopup(CONST.popupTransferName, "TRANSFER SERVANTS", undefined, undefined, undefined, undefined, closeModalPopupAndReloadHeaderAndView, "https://static.realmofempires.com/images/icons/M_Servants.png");

        var popupTransferContent = $("#" + ROE.Frame.CONSTS.popupNameIDPrefix + CONST.popupTransferName + ' .popupBody');

        var content = BDA.Templates.populate(credTrans_template[0].outerHTML, {});
        content = $(content);

        popupTransferContent.html(content);
     
        $('#transferToPlayer', content).keyup(_handleTransferToKeyUp);
        $('.loadingSearchList', content).hide();
        $('#transfercredits .searchResultsArea').hide();
        $('#transfercredits .transferFailMsg').hide();
        $('#transfercredits .transferSuccessMsg').hide();


        $('#transfercredits .sendingToPlayer').click(_handlePlayerTap);
        $('#transfercredits .transferYES').click(_transferCreditsConfirm);
        $('#transfercredits .transferNO').click(_resetTransferCreditsPage);

        _sync_maxTransferCredits();
    }

  

    function _transferCredits() {
       
        // Can't transfer less than 5
        if (_maxTransferServants < 5)
            return;

        _transferToPlayer = $('#transferToPlayer').val();
        if (!_transferToPlayer || _transferToPlayer == "" || !_matchingPlayerFound) {
            $('#transfercredits .transferFailMsg').text(ROE.Utils.phrase('CreditTransferPhrases', 'ErrorToPlayer')).show();
            return;
        } else {
            $('#transfercredits .sendingToPlayer').html(_transferToPlayer);            
        }

        _transferAmount = $('#transferQuantity').val();
        if (isNaN(_transferAmount) || _transferAmount < 5) {
            $('#transfercredits .transferFailMsg').text(ROE.Utils.phrase('CreditTransferPhrases', 'ErrorAmount')).show();
            return;
        }
        // Make sure int.
        _transferAmount = parseInt(_transferAmount);
        $('#transfercredits .numServantsToTransfer').html(_transferAmount);
       
        $('#transfercredits .confirmTransferBlock').show();
        $('#transfercredits .hideDuringConfirm').hide();

    }

    function _transferCreditsConfirm() {
        ROE.Frame.busy();

        // Hide any previous results...
        _resetTransferCreditsPage();

        
        Api.call_transfer_credits(_transferAmount, _transferToPlayer, _transfer_onDataSuccess);
    }

    function _handlePlayerTap(e) {        
        ROE.UI.Sounds.click();
        e.preventDefault();
        ROE.Frame.popupPlayerProfile($(e.currentTarget).text());
    }


    function _transfer_onDataSuccess(data) {
        var r = data.resultcode;
        var errorMessage = "";
        
        if (r == 0) {           
            // Awesome, do nothing except let them know things went well.
            //$('#transfercredits .hideAfterSuccess').hide();
            $('#transfercredits .transferSuccessMsg').show();
        } else if (r == 1) {
            errorMessage = ROE.Utils.phrase('CreditTransferPhrases', 'ErrorSteward');// "Transfer failed, you cannot transfer while logged in as steward.";
        } else if (r == 2 || r == 3) {          
            errorMessage = ROE.Utils.phrase('CreditTransferPhrases', 'ErrorAmount');// "Transfer failed, the amount must between 5 and max available.";          
        } else if (r == 4) {            
            errorMessage = ROE.Utils.phrase('CreditTransferPhrases', 'ErrorToPlayer');// "Transfer failed, there is an error transferring to the chosen player.";
        } else if (r == 5) {           
            errorMessage = ROE.Utils.phrase('CreditTransferPhrases', 'ErrorYourself');// "Transfer failed, you cannot transfer to yourself, you already have the servants!";
        } else { // >= 6           
            errorMessage = ROE.Utils.phrase('CreditTransferPhrases', 'ErrorCatchAll');// "We were unable to process your request at this time, please try again later. If this problem continues to persist, contact support.";            
        }

        if (errorMessage != "") {
            $('#transfercredits .transferFailMsg').html(errorMessage).show();
        }        

        _maxTransferServants = data.max;
        _refreshMaxTransferCredits();

        Frame.free();
    }

   

    function _handleTransferToKeyUp(e) {
        _waitToSearchPlayerNames($(e.currentTarget).val(), _handleGetPlayerNamesSuccessful, _clearTransferSearchList);
    }

    function _handleGetPlayerNamesSuccessful(r) {

        var result = $('#transfercredits .transferSearchList').empty();
        
        $.each(r, function (i, n) {
            var sp = $('<li>' + n.value + '</li>').click(_clearTransferSearchList, _selectPlayerNameFromTransferSearchList);
            result.append(sp);
        });

        if (r.length == 0) {
            result.append('<li><em>'+ROE.Utils.phrase('CreditTransferPhrases', 'NoMatching')+'</em></li>');
            _matchingPlayerFound = false;
        } else {
            _matchingPlayerFound = true;
        }

        $('#transfercredits .loadingSearchList').hide();
        $('#transfercredits .searchResultsArea').show();
    }

    function _selectPlayerNameFromTransferSearchList(e) {
        _clearTransferSearchList();
        var playerName = $(e.currentTarget).html();
        $('#transferToPlayer').val(playerName);
    }

    function _clearTransferTo() {
        $('#transferToPlayer').val("");
    }

    function _clearTransferSearchList() {
        $('#transfercredits .transferSearchList').empty()
        $('#transfercredits .searchResultsArea').hide();
    
    }

    // Same as _searchPlayerNames except has a timer before it makes
    // the call.
    var _transferLookupTimer;
    var _nameToLookup;
    function _waitToSearchPlayerNames(partialName, cbSuccess, cbFail) {
        _nameToLookup = partialName;
        if (_transferLookupTimer)
            clearTimeout(_transferLookupTimer);

        // Time out look up.
        if (_nameToLookup.length > 2) {
            _transferLookupTimer = setTimeout(_handleSearchPlayerNamesOnTimeout, CONST.transferLookupThreshold);
        } else {
            _clearTransferSearchList();
        }
    }

    function _handleSearchPlayerNamesOnTimeout() {
        clearTimeout(_transferLookupTimer);
        $('#transfercredits .loadingSearchList').show();
        _clearTransferSearchList();
        ROE.Api.call('player_search', {
            term: _nameToLookup
        }, _handleGetPlayerNamesSuccessful);
    }
   
    function _sync_maxTransferCredits() {
       
        Api.call_get_max_transfer_credits(_sync_maxTransferCredits_onDataSuccess);
    }

    function _sync_maxTransferCredits_onDataSuccess(r) {
        _maxTransferServants = parseInt(r.allowed);
        _refreshMaxTransferCredits();        
    }

    function _refreshMaxTransferCredits() {
       
        $('#transfercredits .maxTransferAvail').html(_maxTransferServants);

        if (_maxTransferServants < 5) {
            $('#transfercredits .transferFailMsg').text(ROE.Utils.phrase('CreditTransferPhrases', 'NotEnoughServants')).show();
            $('#transfercredits .transferBtn').toggleClass('grayout', true);
        } else {
            $('#transfercredits .transferFailMsg').hide();
            $('#transfercredits .transferBtn').toggleClass('grayout', false);
        }
    }

    function _resetTransferCreditsPage() {
        $('#transfercredits .transferFailMsg').hide();
        $('#transfercredits .transferSuccessMsg').hide();
        $('#transfercredits .confirmTransferBlock').hide();
        $('#transfercredits .hideDuringConfirm').show();
        _clearTransferSearchList();
    }



    obj.showPopup = _showPopup;
    obj.showTransferPopup = _showTransferPopup;
    obj.transferCredits = _transferCredits;

}(window.ROE.Credits = window.ROE.Credits || {}));
