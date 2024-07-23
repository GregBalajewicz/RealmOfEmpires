/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.6.4-vsdoc.js" />
/// <reference path="interfaces.js" />
/// <reference path="ROE_c.js" />

// ensure ROE object exists
(function( obj, $, undefined ) {
} (window.ROE = window.ROE || {}, jQuery));

// ensure ROE.UI object exists
(function (obj, $, undefined) {
} (window.ROE.UI = window.ROE.UI || {}, jQuery));

// ensure ROE.UI.PFs object exists
(function (obj, $, undefined) {

    obj.init = function (settings, clickedCallBack) {
        /*
        settings: is an object that has all the properties to dictate how to display the PFs.
        mode: can be "popup" or "inline"
            "popup" is passed when the PFs are used in the full screen popup mode. The entire template is used, and all PFs are shown.
            "inline" is passed when only on PF is to be used, in a small stripped down version
        fill: is the element to populate. If mode is popup, fill element has be to e the poup content, if mode is inline, any element passed will house the one pf
        featureId: chose the one PF to display      
        */


        ///settings
        var mode = settings.mode;
        var fillElement = settings.fill;
        var featureId = settings.featureId;

        ///load PremiumFeatures.aspx template
        var template = $(BDA.Templates.getRaw("PremiumFeatures", ROE.realmID)).clone();
        var rowTemplateLarge = template.find('.onePF').remove().clone(); //large format row template
        var rowTemplateInline = template.find('.PFInlineRow').remove().clone(); //inline formate row template

        if (mode == "popup") {
            loadFullPopupPackages();
        } else if (mode == "inline") {
            loadInlinePackage();
        }

        ///INLINE MODE FUNCTIONS
        function loadInlinePackage() {
            var rowPopulated = BDA.Templates.populate(rowTemplateInline[0].outerHTML
            , {
                //name: ROE.PFPckgs[featureId].name
                img: ROE.Player.PFPckgs.isActive(featureId) ? ROE.PFPckgs[featureId].icon_ActiveL : ROE.PFPckgs[featureId].icon_NotActiveL
                , state: ROE.Player.PFPckgs.isActive(featureId) ? "active" : "inactive"
                , stateText: ROE.Player.PFPckgs.isActive(featureId) ? "ACTIVE" : "INACTIVE"
                , stateCountdown: ROE.Player.PFPckgs.isActive(featureId) ? "countdown" : ""
                , countdown: ROE.Player.PFPckgs.isActive(featureId) ?
                    ((BDA.Utils.timeLeft(new Date(ROE.Player.PFPckgs.list[featureId].ExpiresOn))).h
                    + ":" + (BDA.Utils.timeLeft(new Date(ROE.Player.PFPckgs.list[featureId].ExpiresOn))).m
                    + ":" + (BDA.Utils.timeLeft(new Date(ROE.Player.PFPckgs.list[featureId].ExpiresOn))).s) : "00:00:00"
                , buttonTxt: ROE.Player.PFPckgs.isActive(featureId) ? "Extend" : "Activate"
                , btnid: (ROE.Player.credits >= ROE.PFPckgs[featureId].cost) ? ROE.PFPckgs[featureId].Id : 'buy'
                , servants: ROE.PFPckgs[featureId].cost
                , period: ROE.PFPckgs[featureId].duration >= 1 ? ROE.PFPckgs[featureId].duration + " day" : Math.round(ROE.PFPckgs[featureId].duration * 24) + " hour"
                , descr: ROE.PFPckgs[featureId].desc
                , pfpid: ROE.PFPckgs[featureId].Id
            });
            fillElement.empty().append(rowPopulated);
            fillElement.find(".castSpell").click(function (event) {
                var btn = $(event.currentTarget);
                if (btn.hasClass('busy')) { return; }
                if (btn.attr('data-pfpid') == 'buy') {
                    ROE.Frame.showBuyCredits();
                } else {
                    
                    if (!btn.hasClass('areyousure')) {
                        btn.addClass('areyousure').html("Sure?");
                        window.setTimeout(function () {                       
                            btn.removeClass('areyousure').html(ROE.Player.PFPckgs.isActive(featureId) ? "Extend" : "Activate");
                        }, 2500);
                        return;
                    }
                    btn.removeClass('areyousure').addClass('busy').html("");
                    ROE.Api.call_activatePFPackage((btn.attr('data-pfpid')), function (data) { packageCallbackInline(btn.attr('data-pfpid'), data); });
                }
            });
            initTimers();
        }

        function packageCallbackInline(pfpid, data) {

            if (data.status == 0) {
                ROE.Player.credits = data.servants;//updates qty of servants available
                ROE.Player.PFPckgs.list = data.PFPckgs; //updates the list of packages
                fillElement.fadeOut('fast').fadeIn('fast');
                loadInlinePackage();
                showSuccess();
                
                if (clickedCallBack) {
                    clickedCallBack(ROE.Player.credits);
                }

                if (data.activatedPackageID == 22) {
                    // if silver spell enabled, clear the cache so that we get the right coins/h numbers
                    ROE.Villages.clearCache();
                }
            } else {
                alert('You do not have enough servants');
            }
        }


        ///FULL POPUP MODE FUNCTIONS
        function loadFullPopupPackages() {//function loads the table with features
            fillElement.empty().append(template); //puts the template into the popup
            fillElement.find('#playerCredits').text(BDA.Utils.formatNum(ROE.Player.credits)); //updates the credits on the popup with current servant count

            var len = ROE.PFPckgs.Order.length;
            var tempTable = $('<div id=pftable>');
            var elmtDrop = fillElement.find('#elmtDrop');
            var rowPopulated;

            for (var i = 0; i < len; i++)//populates contents of rows
            {
                rowPopulated = BDA.Templates.populate(rowTemplateLarge[0].outerHTML
                   , {
                       name: ROE.PFPckgs[ROE.PFPckgs.Order[i]].name
                       , img: ROE.Player.PFPckgs.isActive(ROE.PFPckgs.Order[i]) ? ROE.PFPckgs[ROE.PFPckgs.Order[i]].icon_ActiveL : ROE.PFPckgs[ROE.PFPckgs.Order[i]].icon_NotActiveL/* IF ACTIONVE etc */
                       , state: ROE.Player.PFPckgs.isActive(ROE.PFPckgs.Order[i]) ? "Active. Expires in " : ""
                       , countdown: ROE.Player.PFPckgs.isActive(ROE.PFPckgs.Order[i]) ? ((BDA.Utils.timeLeft(new Date(ROE.Player.PFPckgs.list[ROE.PFPckgs.Order[i]].ExpiresOn))).h + ":" + (BDA.Utils.timeLeft(new Date(ROE.Player.PFPckgs.list[ROE.PFPckgs.Order[i]].ExpiresOn))).m + ":" + (BDA.Utils.timeLeft(new Date(ROE.Player.PFPckgs.list[ROE.PFPckgs.Order[i]].ExpiresOn))).s) : ""
                       , button: ((ROE.Player.credits >= ROE.PFPckgs[ROE.PFPckgs.Order[i]].cost) ? 'https://static.realmofempires.com/images/icons/PF_Plus2.png' : 'https://static.realmofempires.com/images/misc/buy.png')
                       , btnid: (ROE.Player.credits >= ROE.PFPckgs[ROE.PFPckgs.Order[i]].cost) ? ROE.PFPckgs[ROE.PFPckgs.Order[i]].Id : 'buy'
                       , servants: ROE.PFPckgs[ROE.PFPckgs.Order[i]].cost
                       //, period: ROE.PFPckgs[ROE.PFPckgs.Order[i]].duration
                       , period: ROE.PFPckgs[ROE.PFPckgs.Order[i]].duration >= 1 ? ROE.PFPckgs[ROE.PFPckgs.Order[i]].duration + " day" : Math.round(ROE.PFPckgs[ROE.PFPckgs.Order[i]].duration * 24) + " hour"
                       , descr: ROE.PFPckgs[ROE.PFPckgs.Order[i]].desc
                       , pfpid: ROE.PFPckgs[ROE.PFPckgs.Order[i]].Id
                   });
                tempTable.append(rowPopulated);
            }

            elmtDrop.append(tempTable);
            elmtDrop.find("#pftable .button").click(function (event) {
                var toggle = $(event.currentTarget);                              
                if (toggle.attr('id') == 'buy') {
                     
                        ROE.Frame.showBuyCredits();
                    
                } else {
                    ROE.Api.call_activatePFPackage((toggle.attr('id')), function (data) { packageCallback(toggle.attr('id'), data); });
                    toggle.addClass('busy');
                }
            });

            initTimers();//initializes the timers
        }

        function packageCallback(pfpid, data) {

            if (data.status == 0){
                ROE.Player.credits = data.servants;//updates qty of servants available
                ROE.Player.PFPckgs.list = data.PFPckgs; //updates the list of packages
                fillElement.find('#playerCredits').text(ROE.Player.credits).fadeOut('fast').fadeIn('fast');
                showSuccess();
                fillElement.find('#pftable').remove();
                loadFullPopupPackages();
                fillElement.find('.onePF[data-pfid=' + pfpid + '] .state').fadeOut('fast').fadeIn('fast');
                if (clickedCallBack) {
                    clickedCallBack(ROE.Player.credits);
                }

                if (data.activatedPackageID == 22) {
                    // if silver spell enabled, clear the cache so that we get the right coins/h numbers
                    ROE.Villages.clearCache();
                }
            }else{
                alert('You do not have enough servants');
            }
        }


        ///COMMON FUNCTION
        function showSuccess() {
            ROE.UI.Sounds.clickSpell();
            ROE.Frame.infoBar('Success!');
        }



    }

} (window.ROE.UI.PFs = window.ROE.UI.PFs || {}, jQuery));