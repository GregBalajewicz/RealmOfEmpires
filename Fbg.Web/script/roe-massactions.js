/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.6.4-vsdoc.js"/>
/// <reference path="bda-ui-scrolling.js"/>
/// <reference path="bda-ui-transition.js"/>
/// <reference path="roe-api.js"/>
/// <reference path="roe-player.js" />
/// <reference path="countdown.js"/>
/// <reference path="roe-vov.js" />
/// <reference path="roe-ui.sounds.js" />
(function (ROE) {
} (window.ROE = window.ROE || {}));

(function (obj) {
    

    var _showPopup = function () {

        var content = $('<div class="massActionsContent">' +
            '<div class="header fontSilverFrLClrg">Mass Actions</div>' +
            '<div class="toolkit">' +
                '<div class="massActionBtn massChestBuy"></div>' +
                '<div class="massActionBtn massRecruit" data-title="[Beta] Mass Recruit" data-href="villagemassrecruit.aspx"></div>' +
                '<div class="massActionBtn massBuild" data-title="[Beta] Mass Upgrade" data-href="villagemassupgradeb.aspx"></div>' +
                '<div class="massActionBtn massDisband" data-title="[Beta] Mass Disband" data-href="VillageMassDisband.aspx"></div>' +
                '<div class="massActionBtn massDowngrade" data-title="[Beta] Mass Downgrade" data-href="VillageMassDowngrade.aspx"></div>' +
            '</div>'
        + '</div>');

        ROE.Frame.popDisposable({
            ID: 'massActionsDialog',
            title: 'Mass Actions',
            content: content,
            width: 420,
            height: 500,
            modal: false,
        });

        /*
        content.find('.massRecruit').click(function () {
            _openMassRecruit();
        });
        */


        content.find('.massActionBtn').click(function () {


            //_openMassRecruit();

            if($(this).hasClass('massChestBuy')){
                _openMassChestBuy();
            }else{
                ROE.Frame.popGeneric($(this).attr('data-title'), "", 600, 500);
                ROE.Frame.showIframeOpenDialog('#genericDialog', $(this).attr('data-href'));
            }


        });



    }

    function _openMassChestBuy() {

        var container = $('#massActionsDialog');
        if (!container.dialog('isOpen')) {
            return;
        }

        var panel = $('<div class="panel massChestBuyPanel" ></div>');
        $('.massActionsContent', container).append(panel);

        panel.append(

            '<div class="subPanel head">' +
                '<div>Mass Chest Buy</div>' +
                '<div>Fill up chests from all vills in your empire.</div>' +
            '</div>' +

            '<div class="subPanel optionsPanel">' +
                '<div class="note fontGoldFrLCmed">Input an amount of silver to keep in each village,</br>or leave 0 to buy max chests in every village.</div>' +
                '<div><input type="text" class="keepCoinsInput"></div>' +
                '<div class="goBtn BtnBLg2 fontSilverFrSClrg">Start Mass Buy</div>' +
                '<div class="stopBtn BtnBLg2 fontSilverFrSClrg">Stop Mass Buy</div>' +
            '</div>' +

            '<div class="subPanel progressPanel">' +
                '<div class="vProgressBar">' +
                    '<div class="vProgress" style="width:0%;"></div>' +
                    '<div class="vProgressMessage fontGoldFrLCmed"></div>' +
                '</div>' +
                '<div class="progressInfo fontSilverNumbersMed"></div>' +
            '</div>' +

            '<div class="subPanel logPanel fontSilverFrSCmed">' +
            '</div>' +

            '<div class="close"></div>'

        );

        panel.find('.close').click(function () {
            panel.remove();
        });

        var goBtn = panel.find('.goBtn').click(function () {
            _startMassBuy();
        });

        var stopMassBuyBtn = panel.find('.stopBtn').click(function () {
            _stopMassBuy();
        });


        //first we get your village list
        ROE.Frame.busy('Getting village list...',10000, container);
        ROE.Villages.getVillages(_massChestVillageListReady);

        var _villageList = [];
        function _massChestVillageListReady(listOfVillages) {
            _villageList = listOfVillages;
            ROE.Frame.free(container);
        }

        var _keepCoinsInput = panel.find('.keepCoinsInput').val(0);
        var _silverToLeave = 0;
        var _villtoMassBuyIndex = 0;
        var _totalChestsBought = 0;

        function _startMassBuy() {

            var silverToLeave = parseInt(_keepCoinsInput.val());
            if (isNaN(silverToLeave) || silverToLeave < 0) {
                _keepCoinsInput.val(0);
                return;
            }
            if (silverToLeave > 999999999) {
                _keepCoinsInput.val(999999999);
                return;
            }

            _silverToLeave = silverToLeave;
            _villtoMassBuyIndex = 0;
            _totalChestsBought = 0;

            panel.find('.logPanel').empty();
            panel.addClass('active');
            _massBuyInNextVillage();

        }

        function _massBuyInNextVillage() {

            //var popupContent = $(CONST.Selector.popupContentMassChestBuy);
            var progressPercent = Math.ceil(_villtoMassBuyIndex / _villageList.length * 100);
            panel.find('.vProgress').css('width', progressPercent + '%');

            ///this means the popup was closed
            if ($('#massActionsDialog').length < 1) {
                //ROE.Villages.getVillage(_village.id, _sync, ROE.Villages.Enum.ExtendedDataRetrieveOptions.ensureExists);
                return;
            }

            ///this means user hit stop button
            if (!panel.hasClass('active')) {
                panel.find('.vProgressMessage').html('<div>Mass Buy Stopped</div>');
                //ROE.Villages.getVillage(_village.id, _sync, ROE.Villages.Enum.ExtendedDataRetrieveOptions.ensureExists);
                return;
            }

            ///this means all villages were visited
            if (_villtoMassBuyIndex >= _villageList.length) {
                _massBuyFinished();
                return;
            }

            var nextVillObj = _villageList[_villtoMassBuyIndex];
            var vId = nextVillObj.id;
            var vName = nextVillObj.name;

            panel.find('.vProgressMessage').html('<div>Visiting: ' + vName + '</div>');

            if (_silverToLeave == 0) {
                ROE.Api.call_gov_buychest2(vId, undefined, 1, _massBuySuccessReturn);
            } else {
                ROE.Api.call_gov_buychest_leavesilver(vId, _silverToLeave, 1, _massBuySuccessReturn);
            }

        }

        function _massBuySuccessReturn(data) {
            isIgnoreNextExtendedInfoUpdatedOrPopulatedEvent = true;
            var visitedVillageObj = _villageList[_villtoMassBuyIndex];
            ROE.Villages.__populateExtendedData(data.id, visitedVillageObj, data);
            var vId = visitedVillageObj.id;
            var vName = visitedVillageObj.name;
            var chestsBefore = ROE.Player.chestCount;
            var chestsAfter = data.recruit.recruitInfo.govInfo.availableChests;
            var dif = chestsAfter - chestsBefore;
            _totalChestsBought += dif;
            panel.find('.progressInfo').html('<div>Vills visited: ' + (_villtoMassBuyIndex+1) + ' / ' + _villageList.length + ' Chests Bought: ' + _totalChestsBought + '</div>');
            panel.find('.logPanel').prepend('<div class="result">' + vName + ' <span class="fontSilverNumbersMed ' + (dif ? 'had':'' ) + '">+' + dif + '</span></div>');
            ROE.Player.chestCount = chestsAfter;
            _villtoMassBuyIndex++;
            _massBuyInNextVillage();
        }

        function _stopMassBuy() {
            panel.removeClass('active').addClass('completed');
            panel.find('.vProgressMessage').html('<div>Stopping . . .</div>');
        }

        function _massBuyFinished() {
            panel.removeClass('active').addClass('completed');
            panel.find('.vProgressMessage').html('<div>Mass Buy Completed</div>');
            //ROE.Villages.getVillage(_village.id, _sync, ROE.Villages.Enum.ExtendedDataRetrieveOptions.ensureExists);
        }

    }

    function _openMassRecruit() {

        var container = $('#massActionsDialog');
        if (!container.dialog('isOpen')) {
            return;
        }

        var panel = $('<div class="panel massRecruitPanel" ></div>');
        $('.massActionsContent', container).append(panel);

        panel.append(

            '<div class="subPanel head">' +
                '<div>Mass Recruit</div>' +
                '<div>Recruit troops from all villages in your empire.</div>' +
            '</div>' +

            '<div class="subPanel optionsPanel">' +
               '<div>Do in each village:</div>' +
            '</div>' +

            '<div class="subPanel logPanel">' +
                'stuff log, because reading is cool.' +
            '</div>' +

            '<div class="close"></div>'

        );

        panel.find('.close').click(function () {
            panel.remove();
        });


        addRecruitOptionSet(1);
        //addRecruitOptionSet(2);
        //addRecruitOptionSet(3);


        function addRecruitOptionSet(orderNumber) {

            var optionsPanel = panel.find('.optionsPanel');

            if (orderNumber > 1) {
                optionsPanel.append('<div>then do this in the village: </div>');
            }

            var optionsSet = $('<div class="optionSet" data-order="' + orderNumber + '"></div>');

            var methodContainer = $('<div class="section methodContainer"></div>');
            optionsSet.append(methodContainer);

            var methodRecruitThis = $('<div class="method methodRecruitThis selected">Recruit This</div>').click(function () {
                methodContainer.find('.selected').removeClass('selected');
                $(this).addClass('selected');
            }).appendTo(methodContainer);

            var methodRecruitUpto = $('<div class="method methodRecruitUpto">Recruit Upto</div>').click(function () {
                methodContainer.find('.selected').removeClass('selected');
                $(this).addClass('selected');
            }).appendTo(methodContainer);

            var unitContainer = $('<div class="section unitContainer"></div>');
            optionsSet.append(unitContainer);

            var unitElement;
            var unitEntity;
            for (var i = 0; i < ROE.Entities.UnitTypes.SortedList.length; i++) {
                unitEntity = ROE.Entities.UnitTypes[ROE.Entities.UnitTypes.SortedList[i]];
                unitElement = $('<div class="unit" data-utid="' + unitEntity.ID + '" style="background-image:url(' + unitEntity.IconUrl_ThemeM + ')"></div>');
                unitElement.click(function () {
                    unitContainer.find('.selected').removeClass('selected');
                    $(this).addClass('selected');
                })
                unitContainer.append(unitElement);
            }

            var inputArea = $('<div class="section inputArea"><div class="label">Amount: </div></div>');
            optionsSet.append(inputArea);

            var amountInput = $('<input type="number" class="amountInput" />');
            inputArea.append(amountInput);

            optionsPanel.append(optionsSet);

            optionsPanel.find('.addMore').remove();
            var addMore = $('<div class="addMore">+ additional action</div>');
            addMore.click(function () {
                addRecruitOptionSet(orderNumber + 1);
            });
            optionsPanel.append(addMore);

            optionsPanel.find('.goBtn').remove();
            var goBtn = $('<div class="goBtn BtnBLg2 fontSilverFrSClrg">Start Mass Recruit</div>');
            goBtn.click(_initiateMassRecruit);
            optionsPanel.append(goBtn);
        
        }

        function _initiateMassRecruit() {

            console.log('_initiateMassRecruit');
        }


    }


    obj.showPopup = _showPopup;

    
}(window.ROE.MassActions = window.ROE.MassActions || {}));



