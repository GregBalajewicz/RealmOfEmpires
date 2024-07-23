(function (ROE) {
}(window.ROE = window.ROE || {}));


(function (obj) {
    BDA.Console.setCategoryDefaultView('ROE.QuickCommand', false); // by default, do not display the performance category. this assumes BDA.Console is defined BEFORE this file



    var CONST = {};
    CONST.Enum = {
        AttackType : {support : 0 , attack : 1}
    };


    CONST.CssClass = {
        troopCount: "troopCount" // represents the cell that holds the number of troops in the village for one type
        , troopEntryTextBox: "troopCountEntry" // represents the input element that allows player to enter actual troop amount 
        , hasNoLandByTroops: "hasNoLandByTroops"

    };

    CONST.Selectors = {
        highlightedTroopList: function () { return ROE.isMobile ? ".attackIconSheet .highlight .troopPick": ".attackIconSheet.highlight .troopPick" } 
        , attackSheetHighlighted: function () { return ROE.isMobile ? ".attackIconSheet .highlight" : ".attackIconSheet.highlight" }
        , attackSheetCover: function () { return ROE.isMobile ? ".attackIconSheet .cover" : ".attackIconSheet.cover" }
        , attackSheetPanel: function () { return ROE.isMobile ? ".attackIconSheet .panel" : ".attackIconSheet.panel" }
        , landTimeTimeElement: function () { return ROE.isMobile ? ".QuickCommandPopup .highlight .landtime .landtimeNum" : ".QuickCommandPopup .highlight .landtime .landtimeNum" }
        , travelTimeElement: function () { return ROE.isMobile ? ".QuickCommandPopup .sendPanel .travelTime .travelTimeNum" : ".QuickCommandPopup .highlight .travelTime .travelTimeNum" }
        , listOfAllVillageRows: ".QuickCommandPopup .oneVillage"
        , oneVillageRowByID: function (villageID) { return ".QuickCommandPopup .oneVillage[data-villID=%id%]".format({ id: villageID }) }
        , troopPickSellInOneVillageRow: function (unitTypeID) { return ".troopPick[data-troopID=%id%]".format({ id: unitTypeID }) } // USE on an object returned by CONST.Selectors.oneVillageRowByID()
        , troopCount : "." + CONST.CssClass.troopCount // represents the cell that holds the number of troops in the village for one type
        , troopEntryTextBox: "." + CONST.CssClass.troopEntryTextBox // represents the input element that allows player to enter actual troop amount 
    }


    CONST.Attr = {
    };




    var _isFrozen = false;
    var _rowTemp;
    var _targetX;
    var _targetY;
    var _targetVid;
    var _targetPlayerID;
    var pageOffset;
    var pageCount; // set in init
    var panelHeight; // set in init
    var idx;
    var _attackUnitList = [];
    var _areAllTroopCountsInRange; // will be set to true if troop counts for each troop type entered are / appear to be correct, that is >= 0 and <= max. 
    var _totalTroops;    
    var _currentVillID;
    var _currentVillX;
    var _currentVillY;
    var govCount;
    var _attackType; //Support: 0, Attack: 1
    var mode_allowTroopNumEntry = false;
    var _container;
    var _mustRepaint = false;
    var _justSentAttack = false;
    var _targetDistance = null;
    // var _landTimeByThisTroopType = undefined;
    // var _showOnlyVillagesWithLandTimeByTroops = false
    var _oneClickCommandJustExecutedDoNotSelectVillage = false;

    var _filter = {
        showOnlyVillagesWithLandTimeByTroops: false,
        showOnlyVillagesWithMinimumTroops: 0
    };
    var _settings = {
        landTimeByThisTroopType: undefined,
        askAreYouSure : true,
        isOneClickMode : false
    };

    var _init = function (container, attackType, TargetVID, TargetX, TargetY) {
        _targetVid = TargetVID;
        _targetX = TargetX;
        _targetY = TargetY;

        //get saved Settings/Filter variables, or set defaults
        _settings.landTimeByThisTroopType = ROE.Entities.UnitTypes[localStorage.getItem('warroom_landTimeByThisTroopType') || 5]; //defaults to '5' (LC troop id)
        _filter.showOnlyVillagesWithLandTimeByTroops = localStorage.getItem('warroom_showOnlyVillagesWithLandTimeByTroops') === "true"; //defaults to false
        _filter.showOnlyVillagesWithMinimumTroops = localStorage.getItem('warroom_showOnlyVillagesWithMinimumTroops') == undefined ? 0 : parseInt(localStorage.getItem('warroom_showOnlyVillagesWithMinimumTroops'));
        _settings.isOneClickMode = localStorage.getItem('warroom_isOneClickMode') === "true"; //defaults to false

        //if default is true, need to do it this way, as localstorage only saves strings not bool
        if (localStorage.getItem('warroom_askAreYouSure')) {
            _settings.askAreYouSure = localStorage.getItem('warroom_askAreYouSure') === "true";
        } else {
            _settings.askAreYouSure = true;
        }
        
        //
        //
        //  IF the popup is currently frozen, and IF we got the same attackType
        //      THEN do not rebuild the popup, but treat it as if the target had changed
        //  ELSE 
        //      rebuild / build the popup from scratch
        if (_isFrozen && _attackType === attackType) {
            _updateOnTargetChangedWhenFrozen();
        } else {
            _container = container;
            _attackType = attackType;
            _populate();

        }
    }

    
    var _populate = function () {

        panelHeight = ROE.isMobile ? 46 : 17;
        pageCount = ROE.isMobile ? 10: 30;
        pageOffset = 0;

        //pageCount = 10; //FINISH - remove this temp code!!

        // althought we are passed container, and we inject into it, we do so in a global, non-widget kind of a way. its crude but will do for this popup. 
        _container.empty().append('<div class="QuickCommandPopup"></div>');

        if (ROE.isMobile) {
            $(".QuickCommandPopup").append(BDA.Templates.getRawJQObj("QuickCommandPopup", ROE.realmID));
        } else {
            $(".QuickCommandPopup").append(BDA.Templates.getRawJQObj("QuickCommandPopup_d2", ROE.realmID));
        }

        //hide rebel rush pf suggestion initially
        $(".QuickCommandPopup .premiumPanel.pfrr").hide();

        //get the Target info API call out as easrly as possible.
        _updateTargetInfo();

        BDA.Broadcast.subscribe($(".QuickCommandPopup"), "VillageExtendedInfoUpdated", _handleTroopCountInVillageChanged);
        BDA.Broadcast.subscribe($(".QuickCommandPopup"), "VillageExtendedInfoInitiallyPopulated", _handleTroopCountInVillageChanged);
        BDA.Broadcast.subscribe($(".QuickCommandPopup"), "MapSelectedVillageChanged", _handleMapSelectedVillageChangedEvent);
        BDA.Broadcast.subscribe($(".QuickCommandPopup"), "PlayerMoraleChanged", _updateMorale);
        

        // if we got text boxes in the template, then we know we allow exact troop counts
        mode_allowTroopNumEntry = $(".QuickCommandPopup " + CONST.Selectors.troopEntryTextBox).length !== 0;

        idx = 0;
        _rowTemp = $(".QuickCommandPopup .attackIconSheet").remove();

        var innerBox = ($(".QuickCommandPopup #background").height() - 122);
        $(".QuickCommandPopup .villageList").css("height", innerBox + "px");
        $(".QuickCommandPopup .attackBtn").text(_phrases("Button" + _attackType));

        //create village list from API
        _getVillList(pageOffset);

        $(".QuickCommandPopup .villageListMainTable").toggleClass("showOnlyVillagesWithTroopsByLT", _filter.showOnlyVillagesWithLandTimeByTroops);
        BDA.UI.CheckBox.init($(".QuickCommandPopup .showOnlyVillagesWithLandByTroops"), _showLandTimeBy_ShowOnlyVillagesWithTroopsClicked, $(".QuickCommandPopup .showOnlyVillagesWithLandByTroops").attr("data-name"), _filter.showOnlyVillagesWithLandTimeByTroops);
        BDA.UI.CheckBox.init($(".QuickCommandPopup .askAreYouSureOnCommand"), _settings_AskAreYouSureClicked, $(".QuickCommandPopup .askAreYouSureOnCommand").attr("data-name").format({ cmdName: _phrases("Button" + _attackType) }), _settings.askAreYouSure);
        BDA.UI.CheckBox.init($(".QuickCommandPopup .oneClickCommand"), _commandModeChanged, $(".QuickCommandPopup .oneClickCommand").attr("data-name").format({ cmdName: _phrases("Button" + _attackType) }), _settings.isOneClickMode);


        $(".QuickCommandPopup").delegate(".villFrom", "click", _handleVillageNameClick);
        $(".QuickCommandPopup .savePreset").click(_savePresetClicked);
        $(".QuickCommandPopup .closeSendPanel").click(_deSelect);
        $(".QuickCommandPopup .attackBtn").click(_commandButtonClicked);
        $(".QuickCommandPopup .refreshBtn").click(_refresh);
        $(".QuickCommandPopup").click(_clickedOff);
        $(".QuickCommandPopup").on("click", ".attackIconSheet", _selectVillageToCommand);
        $(".QuickCommandPopup").on("click", ".attackIconSheet .troopPick", _troopPickClicked);

        // handle changed in the text boxes
        $(".QuickCommandPopup").delegate(CONST.Selectors.troopEntryTextBox, "keyup", _troopSendCountChanged);
        $(".QuickCommandPopup").delegate(CONST.Selectors.troopEntryTextBox, "change", _troopSendCountChanged);
        $(".QuickCommandPopup .getMoreVill").click(_nextOffset);
        //$(CONST.Selectors.freezeButton).click(_handleFreezeButtonClick);
        $(".QuickCommandPopup .showLandTimeSelection").delegate(".landTimeByTroopSelector", "click", _showLandTimeBySelectionChanged);
        $(".QuickCommandPopup .filtersCmdBtn").click(_showFilterPopup);
        $(".QuickCommandPopup .moraleBtn").click(_toggleMoraleBar);
     

        $(".QuickCommandPopup .TravelTimeColumn").click(_showFilterPopup);
        $(".QuickCommandPopup .oneClickCommand").click(_showFilterPopup);

      
        //based on attackType, choose a proper PF
        var typePFID;
        if (_attackType == 0) {
            typePFID = 23;
        } else {
            typePFID = 24;
        }

        if (ROE.isVPRealm) {
            //setup and instantiate a Attack / Defence premium feature
            var settings = {
                mode: "inline", // popup || inline
                fill: $(".QuickCommandPopup .premiumPanel.pflust"), //for popup mode, popup content must be passed, for inline mode any element can be passed
                featureId: typePFID //for inline mode only, display one PF
            }
            ROE.UI.PFs.init(settings, ROE.Frame.refreshPFHeader);
        }
        // since we know we "painted", we certainly do not want this to happen again!
        _mustRepaint = false;

        $(".QuickCommandPopup .villageListMainTable").toggleClass("mode_oneClick", _settings.isOneClickMode);
        _selectTheProperLandTimeTroopSelectionInSettings();

        _updateMorale();

        if (_attackType == 0) {
            $('.QCMoraleContainer',_container).remove();
        }

    }

    var _updateOnTargetChangedWhenFrozen = function () {
        var villageRowsList;
        var oneVillageRow;
        var i, x, y;

        villageRowsList = $(CONST.Selectors.listOfAllVillageRows);

        _updateTargetInfo();

        for (i = 0; i < villageRowsList.length; i++) {
            oneVillageRow = $(villageRowsList[i])
            x = parseInt(oneVillageRow.attr("data-VX"), 10);
            y = parseInt(oneVillageRow.attr("data-VY"), 10);
            oneVillageRow.find(".dist").text(
                ROE.Utils.CalculateDistanceBetweenVillages(x, y, _targetX, _targetY).toFixed(2) )
        }
        _reselectTheCurrentlySelectedRowIfAny();

        _updateMorale();
    }

    function _reselectTheCurrentlySelectedRowIfAny() {
        var selectedRow = $(CONST.Selectors.attackSheetHighlighted());
        _deSelect();
        selectedRow.click();
    }

    function _handleFreezeButtonClick(event) {
        event.stopPropagation();
        
        ROE.UI.Sounds.click();
        if (_isFrozen) {
            _isFrozen = false;
            ROE.Frame.infoBar("Unpinned - Village list will refresh each time you change targets. <a href='http://realmofempires.blogspot.ca/2017/01/pin-unpin-war-room.html'>help...<a>");//   
            $(event.target).removeClass('pinned');
        } else {
            _isFrozen = true;
            ROE.Frame.infoBar("Pinned - Village list will remain static. Only distance from target will update. <a href='http://realmofempires.blogspot.ca/2017/01/pin-unpin-war-room.html'>help...<a>");//   
            $(event.target).addClass('pinned');
        }

    }

    function _handleTroopCountInVillageChanged(village) {
        var i;
        var villageTroops;
        var oneUnitContainer;
        var oneUnitCountContainer;
        var oldTroopCount; // can be NaN
        var newTroopCount;
        var troopCountrTextBox;
        var villageRow = $(CONST.Selectors.oneVillageRowByID(village.id));
        var villageHasTroopsToShowLandTimeBy = undefined;


        if (villageRow.length > 0) {

            // if the row was "loading" then remove that
            if (villageRow.hasClass("attackIconSheet-loading")) {
                villageRow.removeClass("attackIconSheet-loading").addClass("attackIconSheet");
            }

            // update troop counts, etc
            villageTroops = village._TroopsList;
            for (i = 0; i < villageTroops.length; i++) {                
                newTroopCount = villageTroops[i].YourUnitsCurrentlyInVillageCount;
                oneUnitContainer = villageRow.find(CONST.Selectors.troopPickSellInOneVillageRow(villageTroops[i].id));
                oneUnitCountContainer = oneUnitContainer.find(CONST.Selectors.troopCount);
                oldTroopCount = parseInt(oneUnitCountContainer.text(), 10); // NOTE ! this CAN return NaN
                if (oldTroopCount !== villageTroops[i].YourUnitsCurrentlyInVillageCount) {
                    oneUnitContainer.toggleClass("zero", newTroopCount === 0);
                    oneUnitContainer.attr("data-originaltroopcount", newTroopCount);
                    oneUnitCountContainer.text(newTroopCount);

                    // update text entry box if it is there 
                    troopCountrTextBox = oneUnitContainer.find(CONST.Selectors.troopEntryTextBox);
                    if (troopCountrTextBox.length > 0) {
                        if (newTroopCount === 0) {
                            troopCountrTextBox.val('');
                            troopCountrTextBox.hide();
                            oneUnitCountContainer.show();
                        } else {
                            troopCountrTextBox.attr("max", newTroopCount);
                            if (troopCountrTextBox.val() > newTroopCount) {
                                troopCountrTextBox.val(newTroopCount);
                            }
                        }
                    }
                    // check if village has troops of type selected for "show landtime by"
                    if (villageTroops[i].id == _settings.landTimeByThisTroopType.ID && newTroopCount > 0 && newTroopCount >= _filter.showOnlyVillagesWithMinimumTroops) { // are there troops to show landtime by in this village ? 
                        villageHasTroopsToShowLandTimeBy = true;
                    }
                }
            }

            if (villageHasTroopsToShowLandTimeBy != undefined) {
                oneUnitContainer.parents(".oneVillage").toggleClass(CONST.CssClass.hasNoLandByTroops, !villageHasTroopsToShowLandTimeBy);
            }
        }
    }

    function _handleMapSelectedVillageChangedEvent(villageData) {
        // hanlding the event that target village has changed. 
        // if this popup is showing, update it with this target. 
        // if the popup is not showing, just note the new target, note that we must repaint, andonly repaint on repaint call 

        /// expecting: 
        /// villageData =  {villageID , ownerPlayerID , Cord = {x , y}}
        if (_targetVid != villageData.villageID) {
            BDA.Console.log('ROE.QuickCommand', "handling selected village changed event");
            _targetVid = villageData.villageID;
            _targetX = villageData.Cord.x;
            _targetY = villageData.Cord.y;

            BDA.Console.log('ROE.QuickCommand', "handling selected village changed event - about to check for visibility");
            if ($(".QuickCommandPopup:visible").length > 0) {
                BDA.Console.log('ROE.QuickCommand', "handling selected village changed event - visible, so refresh");
                // if this popup is showing, update it with this target. 
                _refreshTheUI();
            }else {
                // if the popup is not showing, just note the new target, note that we must repaint [, and only repaint on repaint call ]
                BDA.Console.log('ROE.QuickCommand', "handling selected village changed event - not visible, so just that we must change on repaint");
                _mustRepaint = true;
            }
        }
    }
    
    function _paint() {
        //
        // event that gets (SHOULD GET!) called everytime the window is show from hidden state, or opened, first time or subsequent. ie, consumers should ALWAYS call paint after showing the popup
        //
        if (_mustRepaint) {
            BDA.Console.log('ROE.QuickCommand', "_mustRepaint - repaiting");
            _mustRepaint = false;
            _refreshTheUI();
        }
    }


    function _refreshTheUI() {

        //
        // rebuild the UI
        //
        if (_isFrozen) {
            _updateOnTargetChangedWhenFrozen();
        } else {
            _populate();
        }

        
    }


    function _handleVillageNameClick(event) {
        if (ROE.isMobile) {
            _villageAttackPopup(event);
        }
        else {
            RoeObjectClick.FillVillagePopup(event);
        }

        return false;
    }

    function _villageAttackPopup(event) {
        var originVillageID;

        if (ROE.isMobile) {
            originVillageID = _currentVillID;
        } else {
            originVillageID = $($(event.target).parents('.attackIconSheet')[0]).attr('data-villid');
        }


        ROE.Frame.popupCommandTroops(_attackType, _targetVid, _targetX, _targetY, originVillageID);

        return false;
    }

    function _refresh() {

        ROE.UI.Sounds.click();
        ROE.Frame.infoBar('refreshing troop numbers in villages...');

        var villageRowsList;
        var oneVillageRow;
        var i;

        villageRowsList = $(CONST.Selectors.listOfAllVillageRows);

        for (i = 0; i < villageRowsList.length; i++) {
            oneVillageRow = $(villageRowsList[i])
            ROE.Villages.ExtendedInfo_loadLatest(parseInt(oneVillageRow.attr("data-villid"), 10));
        }
    }

    function _updateTargetInfo() {
        //destory previous warning container.
        $('.combatWarnningContainer', _container).remove();

        //construct new one, in loading mode
        _container.append('<div class="combatWarnningContainer loading" data-targetvid="' + _targetVid + '"><div class="loadGif">Loading target information...</div></div>');

        var TargetVillage = ROE.Landmark.villages[_targetX + "_" + _targetY];
        if (typeof TargetVillage != "undefined") {
            _targetPlayerId = TargetVillage.pid;

            //Rebel rush
            if (_attackType == 1 && ROE.isSpecialPlayer(_targetPlayerId) && ROE.PFPckgs[32]) {
                var settings = {
                    mode: "inline", // popup || inline
                    fill: $(".QuickCommandPopup .premiumPanel.pfrr"), //for popup mode, popup content must be passed, for inline mode any element can be passed
                    featureId: 32 //rebel rush
                }
                ROE.UI.PFs.init(settings, ROE.Frame.refreshPFHeader);
                $(".QuickCommandPopup .premiumPanel.pfrr").show();
            } else {
                $(".QuickCommandPopup .premiumPanel.pfrr").hide();
            }

        } else {
            _targetPlayerId = null;
        }

        ROE.Api.call("othervillageinfo", { vid: _targetVid }, _callBack_villageOtherAPI);
    }

    function _callBack_villageOtherAPI(response) {

        var warningC = $('.combatWarnningContainer', _container).empty();
        if (parseInt(warningC.attr('data-targetvid')) != response.targetId) { return; }
        
        warningC.removeClass('loading').empty();
        warningC.find('.loadGif').remove();
        
        //var AttackError = 0;
        var targetMaxRebelDistance = 22;
        var VillagePoint = response.Points;
        var imageUrl = BDA.Utils.GetMapVillageIconUrl(VillagePoint, response.ID);
        var TargetPoints = response.OwnerPoints;
        var BeginnerProtectionEndsDate = response.BeginnerProtectionEndsDate;
        var IsCapitalVillage = response.IsCapitalVillage;
        var isCapitalVillage_ProtectionEndsInDays = response.IsCapitalVillage_ProtectionEndsInDays;
        var TargetPlayer = response.OwnerName;
        var InSleepModeUntil = response.InSleepModeUntil;
        var IsInVacationMode = response.IsInVacationMode;
        var IsInVacationModeUntill = response.IsInVacationModeUntill;
        var IsInWeekendMode = response.IsInWeekendMode;
        var IsInWeekendModeUntill = response.IsInWeekendModeUntill;
        var IsUnderBeginnerProtection = response.IsUnderBeginnerProtection;
        var TargetVName = response.VillageName;
        var TargetVClanID = response.clanID;
        var TargetIsAlly = response.clanDiplomacy_isAlly;
        var TargetIsNAP = response.clanDiplomacy_isNap;
        var PlayerPoints = ROE.Player.Ranking.points;
        _targetPlayerID = response.OwnerPlayerID;
        var handiCap = _calcHandicap(PlayerPoints, TargetPoints, _targetPlayerID);
        var PlayerClanID = -1;
        if (ROE.Player.Clan != null) { PlayerClanID = ROE.Player.Clan.id; }


        if(_attackType == 1){

            //selfharm warning
            if (_targetPlayerID == ROE.playerID) {
                _addCombatWarnning('Target is your own village!');
            }

            //clan member warning
            if (_targetPlayerID != ROE.playerID && TargetVClanID == PlayerClanID) {
                _addCombatWarnning('Target is your clan member!');
                //AttackError = 1;
            }

            //sleepmode warning
            if (InSleepModeUntil > 0) {
                _addCombatWarnning('Target is in sleep mode until: ' + (new Date(InSleepModeUntil).toUTCString()));
                //AttackError = 1;
            }

            if (IsInVacationMode) {
                _addCombatWarnning('Target is in vacation mode until: ' + (new Date(IsInVacationModeUntill).toUTCString()));
            }

            if (IsInWeekendMode) {
                _addCombatWarnning('Target is in weekend mode until: ' + (new Date(IsInWeekendModeUntill).toUTCString()));
            }


            //beginer protection warning
            if (IsUnderBeginnerProtection) {
                _addCombatWarnning('Target is under beginner protection until: ' + BeginnerProtectionEndsDate);
                //AttackError = 1;
            } else {
                //capital village warning
                if (IsCapitalVillage) {
                    _addCombatWarnning('Target is a Capital Village - it cannot be captured with governors for the next %d% day%s%.'.format({ d: isCapitalVillage_ProtectionEndsInDays, s: isCapitalVillage_ProtectionEndsInDays > 1 ? 's' : '' }));
                }
            }

            //claim warning
            if (ROE.Landmark.villages_byid != undefined) { //parts of landmark are available only when map is initialized
                var targetVillage = ROE.Landmark.villages_byid[response.targetId];
                if (targetVillage && targetVillage.claimedStatus == 2) {
                    _addCombatWarnning('A clan member has claimed this village.');
                }
            }
            
            //ally warning
            if (TargetIsAlly) {
                _addCombatWarnning('Target is your clan ally!');
            }

            //NAP warning
            if (TargetIsNAP) {
                _addCombatWarnning('Target is in a Non Agression Pact with your clan!');
            }

            //handicap warning
            if (handiCap.amount > 0) {
                _addCombatWarnning('Target has a ' + handiCap.txt + ' handicap defense bonus.');
            }

            //Low Morale warning
            if (ROE.PlayerMoraleSettings.isActiveOnThisRealm && ROE.Player.morale < ROE.PlayerMoraleSettings.minMorale_Normal) {
                _addCombatWarnning('Your troops have low morale');
            }

        }
        
        warningC.append('<div class="close"></div>');
        $(".QuickCommandPopup .targetVillage .targetImg").css("background-image", "url(" + imageUrl + ")");
        $(".QuickCommandPopup .targetName SPAN").html(TargetVName + " (" + _targetX + "," + _targetY + ") " + response.OwnerName);
    }

    function _addCombatWarnning(msg) {
        var warningDiv = $('<div class="combatWarnning">' + msg + '</div>').hide().click(_warningDivClick);        
        $('.combatWarnningContainer', _container).append(warningDiv);
        warningDiv.fadeIn();
    }

    function _warningDivClick() {
        //$(this).fadeOut();
        $('.combatWarnningContainer', _container).remove();
    }


    function _nextOffset() {

        $(".QuickCommandPopup .attackIconSheet .troopPick").removeClass("selected");
        $(".QuickCommandPopup " + CONST.Selectors.attackSheetPanel()).removeClass("active");
        $(".QuickCommandPopup " + CONST.Selectors.attackSheetPanel()).removeClass("highlight");
        $(".QuickCommandPopup .sendPanel").hide();

        pageOffset = pageOffset + pageCount;

        _getVillList(pageOffset);
    }


    function _getVillList(pageOffset) {      
        ROE.Frame.busy('Gathering village information.',undefined, $('#quickCommandDialog'));
        //get village list sorted by distance
        //ROE.Api.call("quickcmd_get", { targetx: targetX, targety: _targetY, pager_offset: pageOffset, pager_count: pageCount }, _fnShowVillList);

        ROE.Villages.getVillages(_fnShowVillList2);
    }

    function _sortVillageList(villageList) {
        var i;
        var sortedVillageList = new Array();
        var fromVillageObj;
        for (i = 0; i < villageList.length; i++) {
            fromVillageObj = { village: villageList[i], distanceToTarget: ROE.Utils.CalculateDistanceBetweenVillages(villageList[i].x, villageList[i].y, _targetX, _targetY) };
            if (fromVillageObj.distanceToTarget == 0) { continue;} // this means that this village is the same as target village; 
            sortedVillageList.push(fromVillageObj);
        }

        sortedVillageList.sort(function sorter(a, b) {
            return (a.distanceToTarget - b.distanceToTarget);
        });

        return sortedVillageList;
    }
    
        
    function _fnShowVillList2(vills) {

        $(".QuickCommandPopup .getMoreVill").hide();
        attackItems = {};
        var content = "";
        var i;
        var village;
        var troopsTable;
        var troopList;
        var troops;
        var villageTroops; // this may be undefined
        var rowloadingTemplate;
        var sortedVillageList = _sortVillageList(vills);
        var numVillagesListedCount = 0;
        var villageHasTroopsToShowLandTimeBy = false;

        for (i = pageOffset; i < sortedVillageList.length; i++) {
            numVillagesListedCount++;
            villageHasTroopsToShowLandTimeBy = false;
            if (numVillagesListedCount > pageCount) { break };
            village = sortedVillageList[i].village;
            attackItems.dist = sortedVillageList[i].distanceToTarget.toFixed(2);
            attackItems.distExact = sortedVillageList[i].distanceToTarget;
            attackItems.villID = village.id;
            attackItems.villX = village.x;
            attackItems.villY = village.y;
            attackItems.villName = village.name;
            attackItems.idx = idx;
            attackItems.pid = ROE.playerID;
            attackItems.incomingImages = _getImcomingIndicators(village.id);

            villageTroops = village._TroopsList;
            idx++;
            
            //triger get-troops if not found any troops
            if (villageTroops === undefined) {
                ROE.Villages.ExtendedInfo_loadLatest(village.id)
            }

            if (ROE.isMobile) {
                //
                // Mobile UI
                //
                troopsTable = "";
                troopList = new Array;
                troops = villageTroops;
                //collect troops array
                for (var id in troops) {
                    troopList[troops[id].id] = troops[id].YourUnitsCurrentlyInVillageCount;

                    if (troops[id].id == _settings.landTimeByThisTroopType.ID &&
                        troops[id].YourUnitsCurrentlyInVillageCount > 0 &&
                        troops[id].YourUnitsCurrentlyInVillageCount >= _filter.showOnlyVillagesWithMinimumTroops) { // are there troops to show landtime by in this village ? 
                        villageHasTroopsToShowLandTimeBy = true;
                    }
                }

                //make list in 2 lines
                for (var j = 0; j < ROE.Entities.UnitTypes.SortedList.length; j = j + 2) {
                    troopsTable += _troopContent(j, troopList);
                }
                for (var j = 1; j <= ROE.Entities.UnitTypes.SortedList.length; j = j + 2) {
                    troopsTable += _troopContent(j, troopList);
                }

                attackItems.troops = troopsTable;
            } else {
                //
                // Desktop UI
                //
                attackItems.troops = {};
                troops = villageTroops;
                //
                // create entries like this:
                // attackItems.troops[id2] = {}
                // attackItems.troops[id11] = {}
                // ...
                for (var j = 0; j < ROE.Entities.UnitTypes.SortedList.length; j++) {
                    attackItems.troops['id' + ROE.Entities.UnitTypes.SortedList[j]] = { zeroClass: 'zero' };
                    attackItems.troops['id' + ROE.Entities.UnitTypes.SortedList[j]].count = (villageTroops === undefined ? "?" : 0);
                }

                if (villageTroops !== undefined) {
                    // now update "attackItems.troops[id11].count" property. this list should only have non-zero troop counts
                    for (var j = 0; j < troops.length; j++) {
                        attackItems.troops['id' + troops[j].id].count = troops[j].YourUnitsCurrentlyInVillageCount;
                        if (troops[j].YourUnitsCurrentlyInVillageCount !== 0) {
                            attackItems.troops['id' + troops[j].id].zeroClass = "";
                        }

                        if (troops[j].id == _settings.landTimeByThisTroopType.ID &&
                            troops[j].YourUnitsCurrentlyInVillageCount > 0 &&
                            troops[j].YourUnitsCurrentlyInVillageCount >= _filter.showOnlyVillagesWithMinimumTroops ) { // are there troops to show landtime by in this village ? 
                            villageHasTroopsToShowLandTimeBy = true;
                        }
                    }
                }
            }

            // land time and travel time. 
            var hasGov = _settings.landTimeByThisTroopType.ID == 10;
            var spyOnly = _settings.landTimeByThisTroopType.ID == 12;
            attackItems.landingOn_offset = _getTravelTimeInSec(attackItems.distExact, _settings.landTimeByThisTroopType, hasGov, spyOnly) * 1000;
            attackItems.travelTime = ROE.Utils.msToTime(attackItems.landingOn_offset);
            attackItems.villageHasNoLandByTroopsClass = villageHasTroopsToShowLandTimeBy ? "" : CONST.CssClass.hasNoLandByTroops;


            if (villageTroops === undefined) {
                rowloadingTemplate = $(BDA.Templates.populate(_rowTemp.prop('outerHTML'), attackItems));
                rowloadingTemplate.removeClass("attackIconSheet").addClass("attackIconSheet-loading");
                content += rowloadingTemplate[0].outerHTML;
            } else {
                content += BDA.Templates.populate(_rowTemp.prop('outerHTML'), attackItems);
            }
        }

        $(".QuickCommandPopup .villageList").append(content);

        //add show more vills
        if ((pageCount + pageOffset) < ROE.Player.numberOfVillages) {
            $(".QuickCommandPopup .getMoreVill").appendTo($(".QuickCommandPopup .villageList")).show();
        }
        ROE.Clock.reClock();
        ROE.Frame.free($('#quickCommandDialog'));
        _selectProperLandByTroopIcon(); 

    }

    function _getImcomingIndicators(vid) {
        var list = ROE.Troops.InOut2.getIncomingMiniSummary(vid);
        var indicators = ROE.isMobile ? ROE.Utils.getImcomingMiniIndicatorsHTML(list, 2) : ROE.Utils.getImcomingMiniIndicatorsHTML(list, 5);
        //var indicatorsTooltip = ROE.isMobile ? "" : ROE.Utils.getImcomingMiniIndicatorsHTML(list, 10); //disabled for now as this seems to be unused... -farhad
        return indicators;

        //$('#villageListDialog .incomingMiniIndicatorsTooltips').append('<div class="ID_%id%">%content%</div>'.format({ content: indicatorsTooltip, id: v.id }))
    }
    function _troopContent(j, troopList) {
        var popData = {}
        popData.unitID = ROE.Entities.UnitTypes.SortedList[j];
        popData.IconUrl = ROE.Entities.UnitTypes[popData.unitID].IconUrl;
        popData.troopcount = 0;
        if (troopList[popData.unitID]) { popData.troopcount = troopList[popData.unitID]; }
        popData.zeroClass = "";
        if (popData.troopcount == 0) { popData.zeroClass = "zero" }
        if (!_isOneClickSendable(popData.unitID)) { popData.no1ClickClass = "not1ClickSend" }


        return "<DIV class='troopPick %popData.zeroClass% %popData.no1ClickClass%' data-troopid='%unitID%' data-originalTroopCount='%troopcount%'><img src='%IconUrl%' ><span class='troopCount'>%troopcount%</span></DIV>".format(popData);
    }

    function _troopPickClicked(e) {

        var troopCell = $(e.currentTarget);

        //
        // if this cell already has the text box showing, then do nothing
        if (mode_allowTroopNumEntry) {
            if (troopCell.find(CONST.Selectors.troopEntryTextBox).is(':visible')) {
                return;
            }
        }

        if (_settings.isOneClickMode
            && !troopCell.parents(".panel").hasClass("highlight")
            && _isOneClickSendable(troopCell.attr("data-troopid"))) {
            // if in one click mode and clicked a call that does not have a highlight, then proceed 

            // check if clicked on cell with more then 0 troops
            if (!troopCell.hasClass("zero")) {
                ROE.UI.Sounds.click();
                _oneTroopSelection_OneClickSent(troopCell)
                e.stopPropagation();
            }
        } else {

        // if village to command from is not yet selected, then make sure we do, case 31318
        _selectVillageToCommand2(troopCell.parents(".attackIconSheet"));

        //verify its clicked on the highlighted bar
        if (troopCell.parents(".panel").hasClass("highlight")) {
            // check if clicked on cell with more then 0 troops
            if (!troopCell.hasClass("zero")) {
                ROE.UI.Sounds.click();
                troopCell.toggleClass("selected");
                _addInputElementToSelectTroops(troopCell);
               // $(".QuickCommandPopup .landtime .landtimeNum").text("00:00:00"); // reset landtime just in case nothig is selected -- //FINISH shoudl be set to 0 only if trully nothign is selected
                _oneTroopSelectionChangedOrSelected(troopCell)

            }
        }
    }

    }

    function _troopSendCountChanged(event) {
        var tr = $(event.eventTarget);
        _oneTroopSelectionChangedOrSelected(tr)
        
    }

    function _showLandTimeBySelectionChanged(event) {
        var oneSelectionItem = $(event.currentTarget);
        _settings.landTimeByThisTroopType = ROE.Entities.UnitTypes[oneSelectionItem.attr("data-uid")];
        localStorage.setItem('warroom_landTimeByThisTroopType', oneSelectionItem.attr("data-uid"));
        _refreshTheUI();
        _selectProperLandByTroopIcon();
        _selectTheProperLandTimeTroopSelectionInSettings();
    }
    function _selectTheProperLandTimeTroopSelectionInSettings() {
        $(".QuickCommandPopup .showLandTimeSelection .landTimeByTroopSelector").removeClass("selected");
        $(".QuickCommandPopup .showLandTimeSelection .landTimeByTroopSelector[data-uid=%id%]".format({ id: _settings.landTimeByThisTroopType.ID })).addClass("selected");

    }

    function _showLandTimeBy_ShowOnlyVillagesWithTroopsClicked(currentValue) {
        _filter.showOnlyVillagesWithLandTimeByTroops = currentValue == 1;
        localStorage.setItem('warroom_showOnlyVillagesWithLandTimeByTroops', _filter.showOnlyVillagesWithLandTimeByTroops);
        $(".QuickCommandPopup .villageListMainTable").toggleClass("showOnlyVillagesWithTroopsByLT", _filter.showOnlyVillagesWithLandTimeByTroops);

        if (_filter.showOnlyVillagesWithLandTimeByTroops) {
            $('.QuickCommandPopup .showMinWrapper').show();
        } else {
            $('.QuickCommandPopup .showMinWrapper').hide();
        }

    }

    function _settings_AskAreYouSureClicked(currentValue) {
        _settings.askAreYouSure = currentValue == 1;
        localStorage.setItem('warroom_askAreYouSure', _settings.askAreYouSure);
            }

            function _filter_minimumTroopValueChanged() {
                var minimumTroops = parseInt($('.showOnlyHavingMinimum').val());
                if (isNaN(minimumTroops)){ minimumTroops = 0; }
                if (minimumTroops != _filter.showOnlyVillagesWithMinimumTroops) {
                    _filter.showOnlyVillagesWithMinimumTroops = minimumTroops;
                    localStorage.setItem('warroom_showOnlyVillagesWithMinimumTroops', minimumTroops);
            _refreshTheUI();
        }
    }

    
    
    var _addInputElementToSelectTroops = function _addInputElementToSelectTroops(element)
    {
        var troopCountBox;
        //
        // here we add the text boxes allowing player to specify the troop counts they want to send
        //
        if (mode_allowTroopNumEntry) {
            if (!element.find(CONST.Selectors.troopEntryTextBox).is(':visible')) { 
                element.find(CONST.Selectors.troopCount).hide();
                troopCountBox = $(element.find(CONST.Selectors.troopEntryTextBox)[0]);
                troopCountBox.show();
                if (element.attr("data-troopid") == ROE.CONST.GovUnitTypeID) {
                    troopCountBox.val(1);
                    troopCountBox.attr("max", 1);
                } else {
                    troopCountBox.val(element.find(CONST.Selectors.troopCount).text());
                }
                troopCountBox.attr("max", element.find(CONST.Selectors.troopCount).text());
                troopCountBox.select();
            }
        }
    }



    function _oneTroopSelection_OneClickSent(troopPickCell) {


        _areAllTroopCountsInRange = true;
        var villageRow = troopPickCell.parents('.oneVillage');
        _currentVillID = parseInt(villageRow.attr("data-villid"), 10);
        _currentVillX = parseInt(villageRow.attr("data-vx"), 10);
        _currentVillY = parseInt(villageRow.attr("data-vy"), 10);

        _attackUnitList = [];

        troopID = parseInt((troopPickCell).attr("data-troopid"), 10);

        troopCount = parseInt(troopPickCell.find(".troopCount").text(), 10);
        if (troopCount > 0) {
            _totalTroops = troopCount;

            _attackUnitList.push({ utID: troopID, sendCount: troopCount, targetBuildingTypeID: 1 });
            _commandTroops(undefined, true);
            _oneClickCommandJustExecutedDoNotSelectVillage = true;
        }
    }
    function _oneTroopSelectionChangedOrSelected(tr) {

        var troopList = $(CONST.Selectors.highlightedTroopList());
        _targetDistance = parseFloat($(CONST.Selectors.attackSheetHighlighted()).find(".dist").attr('data-distExact'));
        var slowestUnitSelected = undefined;
        var troopCount;
        var troopID;
        var troopSpeed;
        var totalPop = 0; 
        var attackAble = false;
        var spyAble = false;
        var isGovPresent = false;
        var desertionBox = $('.QuickCommandPopup .sendPanel .set .desertion').hide().empty();
        var warnsBox = $(".QuickCommandPopup .sendPanel .set .warns").hide().empty();
        _totalTroops = 0;
        _attackUnitList = [];
        _areAllTroopCountsInRange = true;

        var spyOnly = 2; //2 means hasnt been looped, 1 means Spy only, 0 means false.
        
        for (var i = 0; i < troopList.length; i++) {
            troopID = parseInt($(troopList[i]).attr("data-troopid"));
            troopSpeed = ROE.Entities.UnitTypes[troopID].Speed;

            // get troop count selected
            if (mode_allowTroopNumEntry) {
                //
                // get the troop number from the text box, and validate it. 
                $(troopList[i]).find(CONST.Selectors.troopEntryTextBox).css("background-color", '');
                troopCount = parseInt($(troopList[i]).find(CONST.Selectors.troopEntryTextBox).val());
                troopCount = isNaN(troopCount) ? 0 : troopCount; // in case the text box was "" or something non-number 

                // if number entered too high, tell the player
                if (troopCount > parseInt($(troopList[i]).find(CONST.Selectors.troopEntryTextBox).attr("max")) || troopCount < 0) {
                    $(troopList[i]).find(CONST.Selectors.troopEntryTextBox).css("background-color", 'red');
                    _areAllTroopCountsInRange = false;
                }
            } else {
                troopCount = parseInt($(troopList[i]).find("SPAN").text());
            }

           
            if ($(troopList[i]).hasClass("selected")) {

                if (troopCount > 0) 
                {

                    _totalTroops = _totalTroops + troopCount;
                    totalPop += (troopCount * ROE.Entities.UnitTypes[troopID].Pop);

                    var unitTarget = 1;
                    //if Ram then default selected is Walls
                    if (troopID == 7) { unitTarget = 4; }
                    //if trebuche then default selected is Towers
                    if (troopID == 8) { unitTarget = 7; }
                    if (troopID == 10) { isGovPresent = true; }
                    if (ROE.isMobile) {
                        // if gov selected, just send 1 only
                        if (troopID == 10) {
                            troopCount = 1;
                            $(troopList[i]).find("SPAN").text(troopCount);
                        }
                    }
    
                    if (spyOnly) {
                        if (troopID != 12) {
                            spyOnly = 0;
                        } else {
                            spyOnly = 1;
                        }
                    }


                    _attackUnitList.push({ utID: troopID, sendCount: troopCount, targetBuildingTypeID: unitTarget });

                    // find the slowest unit in the command
                    if (slowestUnitSelected == undefined || troopSpeed < slowestUnitSelected.Speed) {                        
                        slowestUnitSelected = ROE.Entities.UnitTypes[troopID];
                    }
                    if (ROE.Entities.UnitTypes[troopID].AttackStrength > 0) { attackAble = true; }
                    if (ROE.Entities.UnitTypes[troopID].SpyAbility > 0) { spyAble = true; }
                    
                }
            }
            else {
                // if Gov unselected, write back the original govs number
                if (troopID == 10) {
                    $(troopList[i]).find("SPAN").text(govCount);
                }
            }
        }

        
        //
        // travel time and land time
        //
        var travelTimeSecs = 0;
        if (slowestUnitSelected) { // see case 32020
            travelTimeSecs = _getTravelTimeInSec(_targetDistance, slowestUnitSelected, isGovPresent, spyOnly);
        }

        var travelHour = ("0" + parseInt(travelTimeSecs / 3600)).slice(-2);
        var travelMin = ("0" + parseInt((travelTimeSecs - (3600 * travelHour)) / 60)).slice(-2);
        var travelSec = ("0" + parseInt(travelTimeSecs - (3600 * travelHour) - (60 * travelMin))).slice(-2);


        if (travelTimeSecs > 0) {         
            $(CONST.Selectors.landTimeTimeElement()).addClass("Time").attr('data-offset', travelTimeSecs * 1000);
            $(CONST.Selectors.travelTimeElement()).text(travelHour + ":" + travelMin + ":" + travelSec);
        } else {
            $(CONST.Selectors.landTimeTimeElement()).removeClass("Time").html("00:00:00");
            $(CONST.Selectors.travelTimeElement()).text("00:00:00");
        }


        if (_attackType == 1) {

            //warn about perishing armies (gov / siege only attacks)
            if (_totalTroops > 0 && !(attackAble || spyAble)) {
                warnsBox.append('<div>Your army will perish without additional offensive units</div>').show();
            }

            //calculate and display deserting units        
            var desertionFactor = _calcDesertion(totalPop, isGovPresent);
            var desertersPresent = false;
            if (desertionFactor > 0) {
                for (var i = 0; i < _attackUnitList.length; i++) {                    
                    var troopCount = _attackUnitList[i].sendCount;
                    var troopID = _attackUnitList[i].utID;
                    var troopEntity = ROE.Entities.UnitTypes[troopID];
                    var troopDeserters = Math.round(desertionFactor * troopCount);
                    if (troopDeserters > 0) {
                        desertersPresent = true;
                        desertionBox.append('<div class="deserter" style="background-image:url(' + troopEntity.IconUrl_ThemeM + ')">' + troopDeserters + '</div>');
                    }
                }
                if (desertersPresent) {
                    desertionBox.prepend('<div class="label">Deserters:</div>').show();
                }
            }
            
        }

        //disable button if no troops selected and troops counts arent in range, /*or if while attacking, no solo attackable or spyable units are being sent*/
        //prevents players from sending gov only or siege only attacks by accident or by noobishness
        $(".QuickCommandPopup .attackBtn").toggleClass("grayout", !(_totalTroops > 0 && _areAllTroopCountsInRange) /*|| (_attackType == 1 && !(attackAble || spyAble))*/);

        $(".QuickCommandPopup .savePreset ").toggleClass("grayout", !(_totalTroops > 0));

        ROE.Clock.reClock();

        if (slowestUnitSelected) { // see case 32020
            // M only - change the travel by troop icon
            tr.parents(".attackIconSheet").find(".landTimeByTroopIcons .landTimeByTroopIcon").removeClass("selected");
            tr.parents(".attackIconSheet").find(".landTimeByTroopIcons .landTimeByTroopIcon[data-uid=%id%]".format({ id: slowestUnitSelected.ID })).addClass("selected");
        }

    }

    function _calcDesertion(totalPop, isGovPresent) {

        var Desertion = 0;
        var UnitDesertionScalingFactor = ROE.CONST.UnitDesertionScalingFactor;
        var DesertionMinDistance = ROE.CONST.UnitDesertionMinDistance;
        var UnitDesertionMaxPopulation = ROE.CONST.UnitDesertionMaxPopulation;
        var isSpecialPlayer = ROE.isSpecialPlayer(_targetPlayerId);

        if (!isGovPresent 
                   && _targetDistance > DesertionMinDistance
                   && totalPop <= UnitDesertionMaxPopulation
                   && !isSpecialPlayer
                   && UnitDesertionScalingFactor > 0) {

            var a = Math.pow(_targetDistance - DesertionMinDistance, 2) * UnitDesertionScalingFactor;
            var b = Math.pow(totalPop, 2);

            Desertion = (1 - (1 / (a / b + 1)));

        }

        return Desertion;
    }
    
    var _getCoverObj = function _getCoverObj(attackIconSheetElement) {
        if (ROE.isMobile) {
            return attackIconSheetElement.find(".cover");
        } else {
            return attackIconSheetElement;
        }
    }

    var _getPanelObj = function _getPanelObj(attackIconSheetElement) {
        if (ROE.isMobile) {
            return attackIconSheetElement.find(".panel");
        } else {
            return attackIconSheetElement;
        }
    }

    function _selectVillageToCommand(event) {
        event.stopPropagation();

        if (!_oneClickCommandJustExecutedDoNotSelectVillage) {
        _selectVillageToCommand2($(event.currentTarget))
    }
        _oneClickCommandJustExecutedDoNotSelectVillage = false;
    }

    function _selectVillageToCommand2(tr) {
      
        var top;
        var isClickedOnNonSelectedRow;

        //
        // if again clicked on an already selected row, then do nothing. 
        // 
        if (_getPanelObj(tr).hasClass("highlight")) {
            return;
        }

        //clicked for get back to list, so remove covers
        if (_getCoverObj(tr).hasClass("active")) {
            isClickedOnNonSelectedRow = true;
            _deSelect();
        }

        //
        // in mobile, if we clicked on a different, non-secected row, when de-select it and that's all 
        //  on desktop, immediatlely select the newly clicked row.
        //
        if (!isClickedOnNonSelectedRow || !ROE.isMobile){ 
            
            if (!_getPanelObj(tr).hasClass("highlight")) {

                if (!_justSentAttack) {
                ROE.UI.Sounds.click();                
                } 

                _currentVillID = tr.attr("data-villid");
                _currentVillX = parseInt(tr.attr("data-vx"), 10);
                _currentVillY = parseInt(tr.attr("data-vy"), 10);

                ROE.Villages.ExtendedInfo_loadLatest(_currentVillID);
                _totalTroops = 0;

                $(".QuickCommandPopup " + CONST.Selectors.attackSheetCover()).addClass("active");
                _getPanelObj(tr).addClass("highlight");
                _getCoverObj(tr).removeClass("active");


                // THE FOLLOWING CODE, should have been not needed anymore however, removing it, on mobile, still malformats the display. this should be fixed
                //positioning the Attack Button panel
                if (ROE.isMobile) {
                    //top = parseInt(tr.attr("data-idx")) * panelHeight + panelHeight + 7;
                    
                    $(".QuickCommandPopup .sendPanel").css("margin-top", panelHeight + "px");
                } else {
                    top = tr.position().top + tr.height() - 2; // -2 is to account for cell spacing
                }
                // END -- THE FOLLOWING CODE, should have been not needed anymore however, removing it, on mobile, still malformats the display. this should be fixed
              
                $(tr.find('td')[0]).append($(".QuickCommandPopup .sendPanel").show());

                var travelTimeElement = $(".QuickCommandPopup .highlight .travelTime");
                travelTimeElement.find(".travelTimeNum").attr("data-byTroopType", travelTimeElement.find(".travelTimeNum").text()).text("00:00:00");
                travelTimeElement.show();
                var landTimeTimeElement = $(CONST.Selectors.landTimeTimeElement());
                landTimeTimeElement.attr("data-byTroopType", landTimeTimeElement.attr("data-offset"));

                $(".QuickCommandPopup .attackBtn").addClass("grayout");
                
                //keep original gov number
                govCount = $(CONST.Selectors.attackSheetHighlighted() + " .troopPick[data-troopid=10] SPAN").text();
            }
        }

        // Clear the just sent attack flag (if it was anything other than false)
        _justSentAttack = false;
    }

    function _clickedOff(event) {
        if ($(".panel.highlight").length > 0) {
            var src = $(event.srcElement);
            //for clicking off the standard stuff and deselecting
            if (src && src.parents('.attackIconSheet').length == 0 &&
                !src.hasClass('sendPanel') && src.parents('.sendPanel').length == 0) {
                _deSelect();
            }
        }
    }

    function _deSelect(event) {
        var travelTimeElement;

        //because FireFox doesnt have events most of the time...
        if (event && typeof(event) != undefined) {
            event.stopPropagation();
        }
        
        // travel and land time handled differently depending on platform
        landTimeTimeElement = $(CONST.Selectors.landTimeTimeElement());
        landTimeTimeElement.attr("data-offset", landTimeTimeElement.attr('data-byTroopType'));
        if (ROE.isMobile) {
            $(CONST.Selectors.travelTimeElement()).text("");
            _selectProperLandByTroopIcon(); // need this in M since icon is displayed per each row, so must reset the selected row
        } else {
            travelTimeElement = $(".QuickCommandPopup .highlight .travelTime .travelTimeNum");
            travelTimeElement.text(travelTimeElement.attr('data-byTroopType'));
        }

        $(".QuickCommandPopup .attackIconSheet .troopPick").removeClass("selected");
        $(".QuickCommandPopup " + CONST.Selectors.attackSheetCover()).removeClass("active");// specific for mobile UI
        $(".QuickCommandPopup " + CONST.Selectors.attackSheetPanel()).removeClass("highlight"); // specific for mobile UI
        $(".QuickCommandPopup .sendPanel").hide();
        $('.QuickCommandPopup .sendPanel .set .desertion').hide().empty();
        $(".QuickCommandPopup .sendPanel .set .warns").hide().empty();

       
        

        if (mode_allowTroopNumEntry) {
            $(".QuickCommandPopup " + CONST.Selectors.troopEntryTextBox).hide();
            $(".QuickCommandPopup " + CONST.Selectors.troopCount).show();            
        }

        $(".QuickCommandPopup .sendPanel .landtime .landtimeNum").removeClass("Time").html("00:00:00"); //mobile only
        ROE.Clock.reClock();
    }

    function _commandButtonClicked() {

        var btn = $(".QuickCommandPopup .attackBtn");
        if (!(btn.hasClass("grayout"))) {

            ROE.UI.Sounds.click();

            if (_settings.askAreYouSure) {
                if (!btn.hasClass('areyousure')) {
                    btn.attr('data-label', btn.html());
                    btn.addClass('areyousure').html("Sure?");
                    window.setTimeout(function () {
                        btn.removeClass('areyousure').html(btn.attr('data-label'));
                    }, 2500);
                    return;
                }
            }

            _commandTroops(btn);
        }
    }

    function _savePresetClicked() {

        var btn = $(".QuickCommandPopup .savePreset");
        if (!(btn.hasClass("grayout"))) {

            ROE.UI.Sounds.click();

            /*
            if (!btn.hasClass('areyousure')) {
                btn.attr('data-label', btn.html());
                btn.addClass('areyousure').html("Sure?");
                window.setTimeout(function () {
                    btn.removeClass('areyousure').html(btn.attr('data-label'));
                }, 2500);
                return;
            }
            */

            var newPreset = {
                type: 'csv',
                customText: 'New Preset',
                troops: _attackUnitList,
                commandType: _attackType
            }

            ROE.PresetCommands.externalNewPreset(newPreset);

            btn.addClass("grayout"); //so they cant accidentally spam it
        }


    }


    function _commandTroops(btn, isOneClickCommand) {
        // btn param not required

        if (!_areAllTroopCountsInRange) {
                // this (MAY!) kick in when users enters < 0 or more than max number off troops.
                // in reality, the attack button should be disabled, but this is just in case
                ROE.Frame.errorBar(_phrases("wrongTroopCounts"));
            } else {
            if (_totalTroops > 0) {

                //put a placeholder animation
                //temp ANIMATIONS D2 only
                if (!ROE.isMobile) {
                    ROE.Animation.awaitingTroopCommand({
                        ovid: _currentVillID,
                        ovx: _currentVillX,
                        ovy: _currentVillY,
                        dvid: _targetVid,
                        dvx: _targetX,
                        dvy: _targetY,
                        cmdtype: _attackType
                    });
                }

                ROE.Frame.busy('Marching...', undefined, $('#quickCommandDialog'));
                ROE.Api.call_cmd_execute(_currentVillID, _targetVid, _attackType, $.toJSON(_attackUnitList) 
                    , function (replyObj) {
                        _commandTroopsReply(replyObj, isOneClickCommand)
                    });

                if (btn) {
                    btn.removeClass('areyousure').html(btn.attr('data-label'));
                }
            }
                else {
                    ROE.Frame.errorBar(_phrases("noTroops"));
                }
            }

    }


    function _commandTroopsReply(reply, wasOneClickCommand) {
        var troopID;
        var troopCount;
        var troopLeft;
        var oneUnitContainer;
        var oneUnitCountContainer;
        var oldTroopCount; // can be NaN
        var villageRow;
        var newTroopCount;
        var replycode = reply.canCommand;

        _justSentAttack = true;

        //reset selections
        $(CONST.Selectors.landTimeTimeElement()).text("00:00:00");
        $(".QuickCommandPopup .attackIconSheet .troopPick").removeClass("selected");
        $(".QuickCommandPopup .attackBtn").addClass("grayout");

        if (replycode != 0) { //error report

            if (replycode == 1 && mode_allowTroopNumEntry) {
                ROE.Frame.errorBar(_phrases("1a"));
            } else if (replycode == 14) {
                ROE.Frame.errorBar("Target is in WeekendMode. Can not attack.");
            }else if ( replycode  == 15) {
                ROE.Frame.errorBar("Cannot command troops without subscription. Enable Subscription to be able to command your troops.");
            } else if (replycode == 16) {
                ROE.Frame.errorBar("Cannot support other players on this realm");
            } else {
                ROE.Frame.errorBar(_phrases(replycode));
            }
            _justSentAttack = false;
            _deSelect();
        }
        else { //success, update troops
            ROE.Player.refresh(); //triggers reload of incoming/outgoing immediatelly 

            ROE.Frame.infoBar(_phrases("troopsSent" + _attackType));

            villageRow = $(CONST.Selectors.oneVillageRowByID(_currentVillID));

            for (var i = 0; i < _attackUnitList.length; i++) {
                troopID = _attackUnitList[i].utID;
                troopCount = _attackUnitList[i].sendCount;

                oneUnitContainer = villageRow.find(CONST.Selectors.troopPickSellInOneVillageRow(troopID));
                oneUnitCountContainer = oneUnitContainer.find(CONST.Selectors.troopCount);
                oldTroopCount = parseInt(oneUnitContainer.attr("data-originalTroopCount"), 10);
                newTroopCount = oldTroopCount - troopCount;
                oneUnitCountContainer.text(newTroopCount);
                oneUnitCountContainer.parents(".troopPick").toggleClass("zero", newTroopCount== 0);
                oneUnitContainer.attr("data-originalTroopCount", newTroopCount);
            }

            if (!wasOneClickCommand) {
            // now we deselect and immediatelly reselect the row
            _deSelect();
            villageRow.click();
            }

        }

        //
        // update morale?
        if (ROE.Player.morale != reply.playerMoraleAfterCmd) {
            ROE.Player.morale = reply.playerMoraleAfterCmd;
            BDA.Broadcast.publish("PlayerMoraleChanged");
        }

        //Low Morale warning
        if (ROE.PlayerMoraleSettings.isActiveOnThisRealm && ROE.Player.morale < ROE.PlayerMoraleSettings.minMorale_Normal) {
            if (!$('.combatWarnningContainer').length) {
                _container.append('<div class="combatWarnningContainer" data-targetvid="' + _targetVid + '">');
                _addCombatWarnning('Your troops have low morale');
            }
        }


        ROE.Frame.free($('#quickCommandDialog'));

    }

    function _calcHandicap(attackersPoints, defendersPoints, TargetPlayerId) {

        var actualHandicap = 0;
        var handicapText = "";
        var Param_MaxHandicap = ROE.CONST.Handicap_MaxHandicap;
        var Param_StartRatio = ROE.CONST.Handicap_StartRatio;
        var Param_Steepness = ROE.CONST.Handicap_Steepness;
        var isSpecialPlayer = ROE.isSpecialPlayer(TargetPlayerId);

        if (ROE.CONST.IsBattleHandicapActive) {

            if (isSpecialPlayer) {
                defendersPoints = 100000000; //hack, no handicap on those, so setting to ridiculously large number
            }

            if (attackersPoints <= 0 || defendersPoints <= 0) {
                actualHandicap = 0;
            } else {
                var ratio = (attackersPoints / defendersPoints);
                if (ratio <= Param_StartRatio) {
                    actualHandicap = 0;
                } else {
                    var logValB = 2 * Math.log(ratio - Param_StartRatio + 1) / Math.LN10;
                    actualHandicap = Param_MaxHandicap - Param_MaxHandicap * Math.pow(Param_StartRatio, -0.25 * Param_Steepness * ((logValB * logValB)));
                }
            }
        }

        handicapText = (actualHandicap * 100).toFixed(1) + "%";

        return {
            txt: handicapText,
            amount: actualHandicap
        };
    }
    
    function _phrases(r) {
        return $('.QuickCommandPopup .phrases [ph=' + r + ']').html();
    }

    function _buildObjectClickCustomIcon(originVillageID) {
        ///<param name="fromToColumn">tell you from which column this is from. either 'from' or 'to'</param>

        /*
            from column
                filter on the village from (origin village)            
              to column 
                filter on the village TO (destination village)
        */

        var returnObj = {}

        returnObj.actions = ["<a class='sfx2 pundButton fontSilverNumbersLrg' onclick=\"ROE.Frame.popupCommandTroops(%at%, %tvid%, %x%, %y%, %ovid%);return false;\" >#</a>".format(
            { ovid: originVillageID, at : _attackType, tvid : _targetVid, x : _targetX, y : _targetY }
            )];

        returnObj.helpTexts = [_attackType == CONST.Enum.AttackType.attack ? _phrases("attackThisVillage") : _phrases("supportThisVillage")];

        return returnObj;
    }

    function _getTravelTimeInSec(distance, unitEntity, hasGov, spyOnly) {

        var speed = unitEntity.Speed;
        var travelTimeInSec = (distance / speed) * 3600;

        if (_attackType == 1) {

            //if noGovPresent, is special player, and rebel rush active, then 20x less travel
            if (!hasGov && ROE.isSpecialPlayer(_targetPlayerId) && ROE.Player.PFPckgs.isActive(32)) {
                travelTimeInSec = travelTimeInSec / 20;
            }

            //if a morale realm, affect morale onto speed, unless its spyOnly, or has a gov, and NOT vs Players
            if (ROE.PlayerMoraleSettings.isActiveOnThisRealm && !spyOnly && !hasGov && ROE.isSpecialPlayer(_targetPlayerId)) {
                travelTimeInSec = travelTimeInSec / ROE.PlayerMoraleSettings.Effects[ROE.Player.morale].moveSpeed;
            }

        }

        return travelTimeInSec;
    }

    function _selectProperLandByTroopIcon() {
        $(".QuickCommandPopup .landTimeByTroopIcons .landTimeByTroopIcon").removeClass("selected");
        $(".QuickCommandPopup .landTimeByTroopIcons .landTimeByTroopIcon[data-uid=%id%]".format({ id: _settings.landTimeByThisTroopType.ID })).addClass("selected");
    }

    function _showFilterPopup() {

        /*
            only need the content passed at minimum, the rest are optional -farhad

            !!!NOTE: if no 'appendTo' is passed, the quickPopup is constructued and appened to body
            which means the content will then reside in it, in the body, from then on
            for best practices use an ID for the content selector
            example: instead of $(".QuickCommandPopup .filterandsettings"), itd be best to just do $("#quickCommandSettings")
            -farhad
        */

        ROE.Frame.quickPopup({
            content: $(".QuickCommandPopup .filterandsettings"),
            appendTo: $(".QuickCommandPopup"),
            title: 'War Room Filters',
            icon: 'https://static.realmofempires.com/images/icons/M_FilterB.png',
            customQuickPopupContainerClass: 'quickCommandFilters',
            closeFunction: function () {
                _filter_minimumTroopValueChanged();
            }
        });

        $(".QuickCommandPopup .filterandsettings .showOnlyHavingMinimum").val(_filter.showOnlyVillagesWithMinimumTroops);
        
        if (_filter.showOnlyVillagesWithLandTimeByTroops) {
            $('.QuickCommandPopup .showMinWrapper').show();
        } else {
            $('.QuickCommandPopup .showMinWrapper').hide();
        }
    }

    function _toggleMoraleBar() {
        $('.QCMoraleContainer').toggle();
    }

    function _updateMorale() {
        ROE.PlayerMorale.display(_container.find('.QCMoraleContainer'));
    }
    
    function _commandModeChanged(checked) {
        _settings.isOneClickMode = checked == 1;
        $(".QuickCommandPopup .villageListMainTable").toggleClass("mode_oneClick", _settings.isOneClickMode);
        localStorage.setItem('warroom_isOneClickMode', _settings.isOneClickMode);
        _refreshTheUI();
    }

    function _isOneClickSendable (unitID){
        if (unitID !=  ROE.CONST.ramUnitTypeID && unitID != ROE.CONST.trebUnitTypeID && unitID != ROE.CONST.GovUnitTypeID)
        {
            return true;
        }
        return false;
    }

    obj.init = _init;
    obj.buildObjectClickCustomIcon = _buildObjectClickCustomIcon;
    obj.handlePinButtonClick = _handleFreezeButtonClick;
    obj.paint = _paint;

}(window.ROE.QuickCommand = window.ROE.QuickCommand || {}));