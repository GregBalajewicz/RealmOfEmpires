


$(function () {
    if (!ROE.isMobile) {
        $(document).on("click", ".jsV", RoeObjectClick.VillageNameClick);
    } else {
        $(document).on("click", ".jsV", RoeObjectClick.VillageNameClickMobile);
    }
});


RoeObjectClick = {
    VillageNameClick: function (eventObj) {        
        return !RoeObjectClick.FillVillagePopup(eventObj);           
    },

    FillVillagePopup: function FillVillagePopup(eventObj) {
        link = $(eventObj.currentTarget);
        var hm = $('#ClickMenu');

        //
        // create the menu if needed
        //
        if (hm.length <= 0) {
            // item does not exist, create it 
            $('body').append('<div id="ClickMenu"></div>');

            hm = $('#ClickMenu');
        }
        hm.hover(
                function () { },
                function () {
                    $(this).unbind().fadeOut('fast');
                }
            );
        hm.stop().fadeIn('fast');

        //
        // populate the menu
        //
        hm.empty();
        hm.append('<div class="hoverMenu"></div>');

        var a = hm.find('.hoverMenu');
        var ownerPID = parseInt(link.attr('opid') ? link.attr('opid') : link.attr('data-opid'), 10);
        var cordX = parseInt(link.attr('x') ? link.attr('x') : link.attr('data-x'), 10);
        var cordY = parseInt(link.attr('y') ? link.attr('y') : link.attr('data-y'), 10);
        var vID = parseInt(link.attr('vid') ? link.attr('vid') : link.attr('data-vid'), 10);
        
        var vName = link.attr('data-vname');
        var customActionFunct = link.attr('data-addCustomActions');
        var customDefaultActionFunct = link.attr('data-addCustomDefaultActions');        
        var isMine = ownerPID === ROE.playerID; // true if this is my village, false otherwise
        var helpArea = $('<div>');
        var customActions;
        var i;
        var oneCustomActionAnchorJquery;
        var villnameSelector = link.attr('data-villnameSelector'); // alternative way of getting at the village name; must be inside the clicked element
        
        // comma deliminated list of option to exclude
        var excludedIconsRaw = link.attr('data-vsJexcludedOptions');
        var excludedIcons = [];
        if (excludedIconsRaw) {
            excludedIcons = excludedIconsRaw.split(',')
        }

        helpArea.addClass('help');

        ROE.getAnchorMode_IconOnly = true;
        getAnchorMode_IconOnly = true;
        if (ROE.isD2) {
            if (customDefaultActionFunct) {
                customActions = eval(customDefaultActionFunct);
                /*
                Expecting: 

                customActions = {
                    actions : []
                    helpTexts = [];
                    }

                both arrays must be the same length, and of only strings.
                */


                for (i = 0; i <= customActions.actions.length; i++) {
                    var oneCustomActionAnchorJquery = $(customActions.actions[i]);
                    a.append(oneCustomActionAnchorJquery);
                    ROE.helper_AddHelpTextToElement(oneCustomActionAnchorJquery, helpArea, customActions.helpTexts[i]);
                }

            }

            if (excludedIcons.indexOf("mapit") == -1) {
                a.append(ROE.getMapItAnchor(cordX, cordY, helpArea));
            }
            a.append(ROE.getVOVAnchor(link, isMine, vID, helpArea));
            a.append(ROE.getProperAttackAnchor(vID, cordX, cordY, 2, helpArea));
            //a.append(ROE.getAttackAnchor_OneClick(vID, cordX, cordY, 2, helpArea));
            a.append(ROE.getProperSupportAnchor(vID, cordX, cordY, ownerPID, true, helpArea));
            //a.append(ROE.getSupportAnchor_OneClick(vID, cordX, cordY, ownerPID, true, helpArea));
            a.append(ROE.getSupportLookupAnchor(vID, isMine, true, helpArea, ownerPID));
            a.append(ROE.getIncomingToVillageAnchor(vID, vName, cordX, cordY, ownerPID, helpArea));
            a.append('<div>');
            a.append(ROE.getSendSilverAnchor(vID, isMine, cordX, cordY, ownerPID, helpArea, true));
            a.append(RoeObjectClick.getRenameVillageLink(vID, ownerPID, link, helpArea, villnameSelector));
            a.append(ROE.getHQAnchor(vID, isMine, true, helpArea));
            a.append(ROE.getRecruitAnchor(vID, isMine, true, helpArea));
            a.append(ROE.getGiftsAnchor(vID, isMine, true, helpArea));            
            a.append(ROE.getOutgoingFromVillageAnchor(vID, vName, cordX, cordY, ownerPID, helpArea));

            if (customActionFunct) {
                customActions = eval(customActionFunct);
                /*
                Expecting: 

                customActions = {
                    actions : []
                    helpTexts = [];
                    }

                both arrays must be the same length, and of only strings.
                */
                

                for (i = 0; i <= customActions.actions.length; i++) {
                    var oneCustomActionAnchorJquery = $(customActions.actions[i]);
                    a.append(oneCustomActionAnchorJquery);
                    ROE.helper_AddHelpTextToElement(oneCustomActionAnchorJquery, helpArea, customActions.helpTexts[i]);
                }                
            }
        } else {
            a.append(ROE.getVOVAnchor(link, helpArea));
            a.append(ROE.getAttackAnchor(vID, cordX, cordY, 2, helpArea));
            a.append(ROE.getAttackAnchor_OneClick(vID, cordX, cordY, 2, helpArea));
            a.append(ROE.getSupportAnchor(vID, cordX, cordY, ownerPID, true, helpArea));
            a.append(ROE.getSupportAnchor_OneClick(vID, cordX, cordY, ownerPID, true, helpArea));
            a.append(ROE.getSupportLookupAnchor(vID, isMine, true, helpArea, ownerPID));
            a.append('<div>');
            a.append(ROE.getSendSilverAnchor(vID, isMine, cordX, cordY, ownerPID, helpArea, true));
            a.append(ROE.getMapItAnchor(cordX, cordY, helpArea));
            a.append(RoeObjectClick.getRenameVillageLink(vID, ownerPID, link, helpArea));
            a.append(ROE.getHQAnchor(vID, isMine, true, helpArea));
            a.append(ROE.getRecruitAnchor(vID, isMine, true, helpArea));
        }

        a.append(helpArea);

        //        var pos = link.position();
        //        pos.width = link.width();
        //        pos.height = link.height();

        //        hm.css({
        //            'top': parseInt(pos.top, 10) + 'px',
        //            'left': parseInt(pos.left, 10) + 'px'
        //        });        
        var locX = eventObj.pageX;
        var locY = eventObj.pageY;

        hm.css({
            'top': (locY - 10) + 'px',
            'left': (locX - 10) + 'px'
        });

        return true; // says its successful
    },

    //M version - assumed only from village list - bit of a hack -farhad
    VillageNameClickMobile: function VillageNameClickMobile(eventObj) {
        link = $(eventObj.currentTarget);
        
        //clean previous and make new
        $('#ClickMenu').remove();
        var hm = $('<div id="ClickMenu"><div class="hoverMenu"></div></div>').appendTo('body').show();
        hm.click(function () {
            $(this).remove();
        });

        var hoverMenuContent = hm.find('.hoverMenu');

        var ownerPID = parseInt(link.attr('opid') ? link.attr('opid') : link.attr('data-opid'), 10);
        var cordX = parseInt(link.attr('x') ? link.attr('x') : link.attr('data-x'), 10);
        var cordY = parseInt(link.attr('y') ? link.attr('y') : link.attr('data-y'), 10);
        var vID = parseInt(link.attr('vid') ? link.attr('vid') : link.attr('data-vid'), 10);
        var vName = link.attr('data-vname');
        var isMine = ownerPID === ROE.playerID;
        var helpArea = $('<div class="help">');

        ROE.getAnchorMode_IconOnly = true;
        getAnchorMode_IconOnly = true;

        hoverMenuContent.append('<div class="villageName fontGoldFrLCmed">' + vName + '(' + cordX + ',' + cordY + ')</div>');
        hoverMenuContent.append(ROE.getVOVAnchor('#', isMine, vID));
        //hoverMenuContent.append(ROE.getAttackAnchor(vID, cordX, cordY, 2, helpArea));
        hoverMenuContent.append(ROE.getAttackAnchor_OneClick(vID, cordX, cordY, 2, helpArea));
        //hoverMenuContent.append(ROE.getSupportAnchor(vID, cordX, cordY, ownerPID, true, helpArea));
        hoverMenuContent.append(ROE.getSupportAnchor_OneClick(vID, cordX, cordY, ownerPID, true, helpArea));
        hoverMenuContent.append(ROE.getSupportLookupAnchor(vID, isMine, true, helpArea, ownerPID));
        hoverMenuContent.append(ROE.getSendSilverAnchor(vID, isMine, cordX, cordY, ownerPID, helpArea, true));
        hoverMenuContent.append(ROE.getMapItAnchor(cordX, cordY, helpArea));
        hoverMenuContent.append('<a href="#" class="renV sfx2" onclick="ROE.Utils.PopupRenameVillage(' + vID + ');"></a>');
        hoverMenuContent.append(ROE.getHQAnchor(vID, isMine, true, helpArea));
        hoverMenuContent.append(ROE.getRecruitAnchor(vID, isMine, true, helpArea));
        hoverMenuContent.append('<a href="#" class="bazaar sfx2" onclick="ROE.Frame.popupGifts(' + vID + ');"></a>');
        hoverMenuContent.append('<a href="#" class="items2 sfx2" onclick="ROE.Items2.showPopup(' + vID + ');"></a>');

        //click most things should close the village list dialog
        $('a', hoverMenuContent).click(function () {
            if ($(this).hasClass('myvov') || $(this).hasClass('othervov') || $(this).hasClass('map') ) {
                $('#villageListDialog').dialog('close'); 
            }
        });
    
        var locX = eventObj.pageX;
        var locY = eventObj.pageY;

        var top = locY - 10;
        if (top + hm.innerHeight() > $(window).height()) {
            top = $(window).height() - hm.innerHeight();
        }

        hm.append('<div class="close"></div>');
        hm.css({
            'top': top + 'px',
            'left': '30px',
            'right': '30px'
        });


        return true; // says its successful
    },


    getAnchorMode_IconOnly: false,
    //
    // get proper rename village anchor
    //
    getRenameVillageLink: function getRenameVillageLink(vID, ownerPlayerID, link, helpArea, villnameSelector) {
        if (ROE.isMe(ownerPlayerID)) {
            var a = $('<a href="#" class="renV sfx2">' + (getAnchorMode_IconOnly ? '' : 'Rename Village') + '</a>');
            a.click(
                function ()
                {
                    ROE.Utils.RenameVillage(vID
                        , villnameSelector ? link.find(villnameSelector) : link
                        , RoeObjectClick.hideClickMenuPopup);
                    return false;
                }
            );
            ROE.helper_AddHelpTextToElement(a, helpArea, 'Rename this village');
            return a;
        } else {
            return $('<div>').addClass('na').addClass('renV_na');
        }
    },

    hideClickMenuPopup: function hideClickMenuPopup() {
        $('#ClickMenu').unbind().fadeOut('fast');
    },


    handleInOutFilteredPopupClick: function handleInOutFilteredPopupClick(clickedElement) {
        // for summary and some explanation, see case 25039

        var element = $(clickedElement);
        var direction = element.attr('data-direction');
        var village;
        var player;

        var toFromFilter;
        if (element.attr('data-VID')) {
            village = new ROE.Class.Village(element.attr('data-VID'), element.attr('data-VName'), element.attr('data-Vx'), element.attr('data-Vy'))
        } else if (element.attr('data-PID')) {
            player = new ROE.Class.Player(element.attr('data-PID'), element.attr('data-pname'));
        }

        if (direction === 'in') {
            if (element.attr('data-VID')) {
                if (element.attr('data-pid') == ROE.playerID) {
                    toFromFilter = new ROE.Troops.InOutWidget.toFromFilter(undefined, undefined, village); // incoming to this village
                } else {
                    toFromFilter = new ROE.Troops.InOutWidget.toFromFilter(village); // incoming from this village
                }
            } else if (element.attr('data-PID')) {
                toFromFilter = new ROE.Troops.InOutWidget.toFromFilter(undefined, player);
            }

        } else { //direction === 'out') 
            if (element.attr('data-VID')) {
                if (element.attr('data-pid') == ROE.playerID) {
                    toFromFilter = new ROE.Troops.InOutWidget.toFromFilter(village); // outgoing, from this village
                } else {
                    toFromFilter = new ROE.Troops.InOutWidget.toFromFilter(undefined, undefined, village); // outgoing,  To this village
                }
            } else if (element.attr('data-PID')) {
                toFromFilter = new ROE.Troops.InOutWidget.toFromFilter(undefined, undefined, undefined, player);
            }
        }

        ROE.Frame.popupInOut(direction, toFromFilter);

    }

}
