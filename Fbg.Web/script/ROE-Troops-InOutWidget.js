

(function (obj) {

    var CONST = {
        IDs: {unknownPlayerID : -999, unknownVillageID : -999}
    };
    CONST.Enum = {};

    CONST.Selectors = {
        cmdRow : '.cmd'
        , tbody: {  // selectors that you can execute on a variable called 'tbody' which holds the tbody reference to the table with list of commands
            listOfCmds: '.cmd'  // list of all commands
            , rowCounter: '.rowCounter'
            , listOfCmds_deleted: '.cmd.activeState-deleted' // list of all commands that were deleted
            , cmdInListByEventID: function (eventID) { return '.cmd[data-eid=' + eventID + ']' } // command by event id
            , cmdsInListThatWereHidden : ".hidden" // commands that are OK but were just hidden from the list. 
        }
        , listOfCmds: { activeStatusOK: ".activeState-ok", activeStatusDeleted: ".activeState-deleted" }
        , countDown: '.countdown'
        , arriveOn: '.arriveOn'
        , sfxOpen: 'sfxOpen'
        , anyFilterButtonInCmdDetailsPanel: ".cmd .troopMoveDetails2 .filterButton"
        , anyFilterButtonInFilterPopup: ".filterPopup .filterButton"
        , cmdRowfilterPopupBtn: ".cmd .troopMoveActions .filtersCmdBtn"
        , cmdRowProfilePopupBtn: ".cmd .troopMoveActions .profilesCmdBtn"
        , CmdDetails: { cancelTroopMovementIcon: ".cancel"
            , toggleHideIcon: ".toggleHide"
            , filterButtons : {
                destinationPlayer : ".filterButton.dp"
                ,destinationVillage : ".filterButton.dv"
                ,originPlayer : ".filterButton.op"
                ,originVillage: ".filterButton.ov"

            }

        }
        , template: { // selectors that apply to the template variable which holds a reference to the entire widget's template
            cmdDetails : '.troopMoveDetails2' // selects the comand details panel with troop list display etc
        }
        , filterContainer: { // selectors typically applied to the containerForFilter variable in init function
            toFromContainer: '.byFromOrToVillOrPlayer'
        }
    }


    CONST.CssClass = {
        cmdActiveStatus: { ok: "activeState-ok", newCmd: "activeState-new", deleted: 'activeState-deleted' }
        , sfxOpen: 'sfxOpen'
    };

    CONST.Attr = {
        cmdRow: { // attributes that exist on command row, a row in the list of commands
            eventID: 'data-eid'
            ,originVillageID: 'data-ovid'
            , originVillageName: 'data-ovname'
            , originPlayerID: 'data-opid'
            ,destVillageID: 'data-dvid'
            ,destPlayerID: 'data-dpid'
            , destVillageName: 'data-dvname'

        }
    };

    var _rowTemplates = {};

    var _storedInOutEvents = {};

    var _init = function (direction, containerForTable, containerForFilter, refreshButton, widgetID, options) {
        ///<param name="direction">one of ROE.Troops.InOut2.Enum.Directions</param>)
        ///<param name="containerForTable">jquery object where the table of commands should be located</param>
        ///<param name="containerForFilter">jquery object where the filter should be located. can pass in undefined. 
        //      if undefined, and if options say to not display filter, then it is not displayed
        ///     if undefined and options say to display filter, the default location for filter is used, right above the list of commands 
        ///</param>
        ///<param name="widgetID">ID of this widget if running in multiple widget mode; note that this MAY BE undefined or null; otherwise, it is an intt</param>
        ///<param name="options">options. 
        /// defaults
        ///   displayFilter: true 
        ///   initPageSize: 20 
        ///   hideToColumn : false
        //    hideArrivalInColumn : false
        ///</param>

        BDA.Val.validate(direction, "unrecognized direction", false, ROE.Troops.InOut2.Enum.Directions.validate, obj.getContextInfo);

        var direction = direction;
        var template;
        var rowTemplate;
        var tbody = null; // table body of the command list table - this table holds the list of commands 
        var filter;
        var filter_toFromContainer; // the area of the filter where we display the TO / FROM filter
        var filterByToFrom = {}; // of type ROE.Troops.InOutWidget.toFromFilter
        var destFilter;
        var footer;
        var actionsTemplate;
        var cmdDetailsTemplate;
        var cmdDetailsTemplate2;
        var settings = $.extend({
            'displayFilter': true
            , 'initPageSize': 20
            , 'hideToColumn' : false
            , 'hideArrivalInColumn' : false
        }, options);
        var currentMaxRowsToDisplay
        var currentMoreRowsIncrament;


        // WAS var _loadIncoming_loaded = function (data) {
        var _populate = function () {
            ROE.Frame.free();


            if (ROE.isMobile) {
                template = $(BDA.Templates.getRaw("InOutTroopsWidget2", ROE.realmID));
            } else {
                template = $(BDA.Templates.getRaw("InOutTroopsWidget2_d2", ROE.realmID));
            }
            if (!_rowTemplates[direction]) {
                _rowTemplates[direction] = template.find('.incomingTroops tbody tr');
                _rowTemplates[direction].find(direction == ROE.Troops.InOut2.Enum.Directions.incoming ? '.mode-outgoing' : '.mode-incoming').remove();
            }
            rowTemplate = _rowTemplates[direction].clone();
            actionsTemplate = template.find('.troopMoveActions');
            cmdDetailsTemplate = template.find('.troopMoveDetails').clone();
            cmdDetailsTemplate2 = template.find(CONST.Selectors.template.cmdDetails).clone();

            template.find('.incomingTroops tbody tr').remove();
            template.find('.destinationFilter').remove();
            footer = template.find('.utilFooter');

            containerForTable.append(template);
            tbody = containerForTable.find(".incomingTroops tbody");

            if (settings.hideToColumn) {
                rowTemplate.find('.dv').remove();
                rowTemplate.find('.dpname').remove();
                rowTemplate.find('.dvcords').remove();
                
                template.find('.dv').remove();
            }
            if (settings.hideArrivalInColumn) {
                rowTemplate.find('.arrivalOnColumn').remove();
                template.find('.arrivalOnColumn').remove();
            }


            BDA.Broadcast.subscribe(containerForTable, "InOutgoingDataChanged", _handleInOutChanged);

            // bind event for when new data is available
            /*containerForTable.find(".getInOutDataChangedEvent").bind("InOutgoingDataChanged", function (e, dir) {
                if (dir === direction) {
                    if (containerForTable.is(':visible')
                        && refreshButton.length > 0) //if we got no refreshbutton, that means we refresh right away HACK FOR NOW
                    {
                        BDA.Console.verbose('ROE.InOutW', 'got new data, ui not hidden');
                        _reloadList();
                    } else {
                        BDA.Console.verbose('ROE.InOutW', 'got new data, ui hidden, applying now');
                        _reloadList();
                        _reloadList_ShowChanges();
                        //containerForTable.find('.refresh-NewData').hide();
                    }
                }
            });*/


            //TODO - i dont think this is used at all anynmore ... 
            containerForTable.find('.refresh-NewData').click(function () {
                _reloadList();
                containerForTable.find('.refresh-NewData').fadeOut();
            })
            // END OF JUNK CODE


            //HACK - what if we were not giving a refresh button? for now we just do this stupid thing
            if (!refreshButton) {
                refreshButton = $('');
            }
            refreshButton.bind("click", _reloadList_ShowChanges);
            //
            // display filter in the right location
            //
            if (settings.displayFilter) {

                // if no container for filter was specified, lets use the default filter localtion.
                //  if specified, remove the default filter location
                if (!containerForFilter) {
                    containerForFilter = containerForTable.find('.filter .byType');

                    //destFilter = BDA.UI.Radio.init(containerForFilter.find('.bydest')
                    //    , _filterChanged
                    //    , [BDA.Dict.showAll.format({ Dir: ROE.Troops.InOut2.CONST.directionNameByDirection[direction] })
                    //        , BDA.Dict.limit]
                    //    , 0
                    //    , { 'orientation': 'vertical' });

                    // show/hide the filter area 
                    var filterMainContainer = containerForFilter.parent();
                    filterMainContainer.find(".filterHeader").click(
                        function expandOrColapseFilterHeader(event) {
                            filterMainContainer.toggleClass('expanded');
                        }
                    );

                    //
                    // area for the TO/FROM filter
                    //
                    filter_toFromContainer = containerForFilter.find(CONST.Selectors.filterContainer.toFromContainer);
                    filter_toFromContainer.find('.clearFilter').click(
                          function clearFilter() {
                              ROE.UI.Sounds.click();
                              filter_toFromContainer.addClass("hidden");
                              filterByToFrom = {};
                              _filterChanged();
                          }
                     );

                } else {
                    containerForTable.addClass('noFilter').find('.filter').remove();
                }

                //
                // insert the filter
                //
                if (direction === ROE.Troops.InOut2.Enum.Directions.incoming) {
                    filter = ROE.Troops.InOutTroopsFilter.init(containerForFilter, _filterChanged);
                } else {
                    filter = ROE.Troops.InOutTroopsFilter.init(containerForFilter, _filterChanged, { showReturning: false });
                }
            } else {   //(!settings.displayFilter) {
                containerForTable.addClass('noFilter').find('.filter').remove();
            }

            //
            // populate all rows
            //
            _populateRows2(tbody, ROE.Troops.InOut2.getData(direction), (filter ? filter.getState() : null)
                , (destFilter ? destFilter.getCheckedIndex() : null), filterByToFrom, currentMaxRowsToDisplay, footer, direction, undefined, widgetID, rowTemplate)

            //
            // handle some events
            //
            containerForTable.find('.loadMany').click(_loadManyDetails);
            containerForTable.find('.summaryButton').click(_handleSummaryLinkClick);
            containerForTable.delegate(".utilFooter", "click", _loadMoreRows);

            //condensed row clicking to cause refresh
            containerForTable.delegate(CONST.Selectors.tbody.rowCounter, "click", _reloadList_ShowChanges);
            // cancel troop movement on details pane
            containerForTable.delegate(CONST.Selectors.CmdDetails.cancelTroopMovementIcon, "click", _cancelComand);
            containerForTable.delegate(CONST.Selectors.CmdDetails.toggleHideIcon, "click", _toggleHide);

            if (ROE.isMobile) {
                containerForTable.delegate(".jsV", "click", _handleVillageNameClick);
                containerForTable.delegate("tr.cmd", "click", _handleCmdRowClick);
                // toggle hide on details pane 
                containerForTable.delegate(CONST.Selectors.anyFilterButtonInCmdDetailsPanel, "click", _handleFilterOfCmdDetailsDisplay);
                containerForTable.delegate(CONST.Selectors.anyFilterButtonInFilterPopup, "click", _handleFilterOfFilterPopup);
                containerForTable.delegate(CONST.Selectors.cmdRowfilterPopupBtn, "click", _cmdRowFilterPopup);
                containerForTable.delegate(CONST.Selectors.cmdRowProfilePopupBtn, "click", _cmdRowProfilePopup);

            } else {
                containerForTable.delegate(".cmd .loadDetails", "click", _doDetails_d2);
            }

            _setSomeUIBasedOnFilterAndSummary();
        };

        function _loadManyDetails() {
            $(".loadDetails:lt(10)", containerForTable).click();
        }


        function _handleSummaryLinkClick(event) {
            ROE.UI.Sounds.click();
            event.stopPropagation();
            ROE.Frame.popupInOutSummary(direction);
        }

        var _handleInOutChanged = function _handleInOutChanged(dir) {
            if (dir === direction) {
                if (containerForTable.is(':visible')
                    && refreshButton.length > 0) //if we got no refreshbutton, that means we refresh right away HACK FOR NOW
                {
                    BDA.Console.verbose('ROE.InOutW', 'got new data, ui not hidden');
                    _reloadList();
                } else {
                    BDA.Console.verbose('ROE.InOutW', 'got new data, ui hidden, applying now');
                    _reloadList();
                    _reloadList_ShowChanges();
                    //containerForTable.find('.refresh-NewData').hide();
                }
            }
        }


        var _filterChanged = function _filterChanged() {
            tbody.find(CONST.Selectors.tbody.listOfCmds).remove();
            _populateRows2(tbody, ROE.Troops.InOut2.getData(direction), (filter ? filter.getState() : null)
                , (destFilter ? destFilter.getCheckedIndex() : null), currentMaxRowsToDisplay, filterByToFrom, footer, direction, refreshButton, widgetID, rowTemplate)
            _setSomeUIBasedOnFilterAndSummary();
        }



        function _setSomeUIBasedOnFilterAndSummary () {
            if (ROE.isMobile && ROE.Player.numberOfVillages > 1) {
                if ((filterByToFrom.originPlayer)
                        || (filterByToFrom.originVillage)
                        || (filterByToFrom.destinationPlayer)
                        || (filterByToFrom.destinationVillage))
                {
                    containerForTable.find('.summary').hide();
                    containerForTable.find(".incomingTroops").show();
                    containerForTable.find(".filter").show();
                    if (direction == ROE.Troops.InOut2.Enum.Directions.incoming) {
                        ROE.Tutorial.startIfNotAlreadyRan(1, 'incomingMobileFilter');
                    }
                } else {
                    containerForTable.find(".summary").show();
                    containerForTable.find(".incomingTroops").hide();
                    containerForTable.find(".filter").hide();
                }
            }
        }

        var _reloadList = function _reloadList() {
            //containerForTable.find('.incomingTroops tbody tr').remove();

            _populateRows2(tbody, ROE.Troops.InOut2.getData(direction), (filter ? filter.getState() : null)
                , (destFilter ? destFilter.getCheckedIndex() : null), currentMaxRowsToDisplay, filterByToFrom, footer, direction, refreshButton, widgetID, rowTemplate)

            //containerForTable.find(".incomingTroops tbody").fadeIn('fast');
        }

        var _reloadList_ShowChanges = function _reloadList_ShowChanges() {

            tbody.find(CONST.Selectors.tbody.listOfCmds_deleted).remove();
            tbody.find(CONST.Selectors.tbody.listOfCmds).removeClass(CONST.CssClass.cmdActiveStatus.newCmd);

            //
            // special case to handle commands that were hidden from the list.If the filter does not allow displaying 
            //  those commands, then we hide them 
            //
            if (!filter || filter.getState().hidden == 0) {
                tbody.find(CONST.Selectors.tbody.cmdsInListThatWereHidden).remove();
            }

            refreshButton.fadeOut();
            _condenseActiveNewRows(tbody);
            // Init some JS
            initTimers(); // make sure the countdown timers are taken care of
            InitStripeTable(); // make sure the hover over on tables works, if in desktop
        }

        var _signalNewDataAvailable = function () {
            containerForTable.find('.refresh-NewData').fadeIn();
        }

        var _loadMoreRows = function () {
            currentMaxRowsToDisplay += currentMoreRowsIncrament;
            _reloadList();
        }

        var _popCmdDetailsPanel = function (eventID) {
            if (actionsTemplate.length > 0) {
                var cmd = ROE.Troops.InOut2.findCommandByEventIDAnyDir(eventID);
                var eventTime = new Date(cmd.etime);
                var thePanel;
                var thePanel2;
                var dataForTemplate;
                var data = ROE.Troops.InOut2.getData(direction);
                var commandTypeTextSelector;
                var filterButtonContainer;

                if (!cmd) { return; }

                cmdRow = tbody.find(CONST.Selectors.tbody.cmdInListByEventID(eventID)); /// TODO - make better please - just pass in the row



                dataForTemplate = {
                    eid: eventID
                    , op: data.players[cmd.opid]    //origin player id - can be nothing is origin of attack is unknow
                    , dp: data.players[cmd.dpid]    //destination player id
                    , dv: data.villages[cmd.dvid]   //destination village id
                    , ov: data.villages[cmd.ovid]   //origin village id - can be nothing is origin of attack is unknow
                    , cmd: cmd
                    , timeleft: BDA.Utils.timeLeft(cmd.etime)
                    , arrivalTime: BDA.Utils.formatEventTime(new Date(cmd.etime), true)
                    , type: getTypeShortStr(cmd.type)
                    , hidden: cmd.hidden ? "hidden" : "vis"
                    , cmdType_className: getCommandClassName(cmd.type)
                };


                thePanel = $(BDA.Templates.populate(cmdDetailsTemplate2[0].outerHTML, dataForTemplate));

                thePanel.show(); /// TODO - make better please 




                //
                // display proper type message
                commandTypeTextSelector = direction == ROE.Troops.InOut2.Enum.Directions.incoming ? ".in" : ".out";
                commandTypeTextSelector += getCommandClassName2(cmd.type);
                thePanel.find('.cmdType:not(' + commandTypeTextSelector + ')').remove();

                //
                // we dont display rebel or abandoned player names, so remove it
                if (ROE.isSpecialPlayer(cmd.opid)) { thePanel.find('.opname').remove() }
                if (ROE.isSpecialPlayer(cmd.dpid)) { thePanel.find('.dpname').remove() }
                //
                // if this player is the logged in player, hide the player name
                if (cmd.opid == ROE.playerID) { thePanel.find('.opname').remove() }
                if (cmd.dpid == ROE.playerID) { thePanel.find('.dpname').remove() }
                //
                // if unknown origin (ie, visible to target ==1 ) then remove the player name etc. if not, then remove the "Unknown..." text
                if (cmd.visibleToTarget < 2 && direction == ROE.Troops.InOut2.Enum.Directions.incoming) {
                    thePanel.find('.opname').remove();
                    thePanel.find('.ov').remove();
                    thePanel.find('.ovcords').remove();
                } else {
                    thePanel.find('.ovunknown').remove();
                }

                //
                // figure out what action icons to show
                //
                if (!_canCancel(cmd.type, cmd.opid, eventTime, ROE.playerID)) {
                    thePanel.find(".cancel.clickable").remove();
                } else {
                    thePanel.find(".cancel.notAvail").remove();
                }
                if (!_canSeeDetails(cmd.type, cmd.opid, ROE.playerID)) {
                    thePanel.find(".rep.clickable").remove();
                } else {
                    thePanel.find(".rep.notAvail").remove();
                }

                //containerForTable.find(".troopMoveDetails").html(thePanel.html())


                //
                // if we can see troop movement, then get it now
                //
                if (_canSeeDetails(cmd.type, cmd.opid, ROE.playerID)) {
                    _doDetails_mobile(eventID, thePanel);
                } else {
                    thePanel.find(".cmdTroopList").remove();
                }

                initTimers(); // not very efficient - reinit all timers when only 1 timer has been adeded

                cmdRow.find('td').append(thePanel);
            }
        }

        var _handleCmdRowClick = function (event) {
            ROE.UI.Sounds.click();
            event.stopPropagation();
            var row = $(event.currentTarget);
            var eventID = row.attr('data-eid');
            var hiddenState = row.attr('data-hidestate');
            if (row.hasClass(CONST.CssClass.cmdActiveStatus.deleted) || row.find(CONST.Selectors.tbody.rowCounter).length) {
                return;
            }
            if (row.hasClass("selected")) {
                row.removeClass("selected");
            } else {
                containerForTable.find(".incomingTroops > tbody > tr").removeClass("selected");
                row.addClass("selected");

                if (row.find(CONST.Selectors.template.cmdDetails).length === 0) {
                    _popCmdDetailsPanel(eventID, hiddenState);
                }
            }
        }

        var _toggleHide = function (event, eventID) {
            ROE.UI.Sounds.click();
            event.stopPropagation();
            var hideIcon = $(event.target);
            var eventID = hideIcon.attr('data-eid');
            var isShowing = hideIcon.parents('.cmd').attr('data-hidestate') === "hidden";

            hideIcon.html('<img id=togglehide_eid' + eventID + ' src="https://static.realmofempires.com/images/misc/busy_tinyred.gif" />');
            $(event.target).removeClass('clickable')
                .removeClass('hide-hidden')
                .removeClass('hide-vis')
                .css('background-position', '-1136px 0px');

            ROE.Api.call_troopMove_toggleHide(_toggleHide_CallBack, eventID);
            ROE.Frame.infoBar((isShowing ? "Showing command in list" : "Hiding command from list") + "...");
        }

        var _toggleHide_CallBack = function (data) {
            ROE.Frame.free();
            if (data.eventID == 0) {
                ROE.Frame.infoBar("Cannot complete action. Perhaps command no longer active?");
            } else {
                //slide back 
                //BDA.UI.Transition.slideRight(containerForTable.find('.cmdList'), containerForTable.find('.troopMoveDetails'));
                //
                // update the commmand row to look like it is hidden / not hidden
                //
                var row = tbody.find(CONST.Selectors.tbody.cmdInListByEventID(data.eventID));
                if (data.curHiddenState !== 0) {
                    row.addClass('hidden');
                } else {
                    row.removeClass('hidden');
                }
                if (row.attr('data-hidestate') === 'hidden') {

                    row.attr('data-hidestate', "vis");
                }
                else {

                    row.attr('data-hidestate', "hidden");
                }
                //
                // update the cached cmd data 
                ROE.Troops.InOut2.toggleHiddenStatusByEventID(data.eventID);
                refreshButton.fadeIn();

                $('#togglehide_eid' + data.eventID).remove();
            }
        }

        var _cancelComand = function _cancelComand(event) {
            event.stopPropagation();
            if ($(event.target).hasClass("notAvail")) { return } // means grayed out cancel button was clicked
            var eventID = $(event.target).attr('data-eid');
            ROE.Api.call_troopMove_cancel(_cancelComand_CallBack, eventID)

            if (ROE.isMobile) {
                ROE.Frame.busy("Cancelling command...");
            } else {
                // put a busy animation, and remove the cancel button
                $(event.target).html('<img id=loadcancel_eid' + eventID + ' src="https://static.realmofempires.com/images/misc/busy_tinyred.gif" />');
                $(event.target).removeClass('clickable')
                    .removeClass('cancel')
                    .css('background-position', '-1136px 0px');
            }
        }

        var _cancelComand_CallBack = function (data) {
            ROE.Frame.free();
            if (data.eventID == 0) {
                ROE.Frame.infoBar("Command NOT cancelled. Perhaps it already expired.");
            } else {
                ROE.Player.refresh(); //triggers reload of incoming/outgoing immediatelly 

                //slide back 
                //BDA.UI.Transition.slideRight(containerForTable.find('.cmdList'), containerForTable.find('.troopMoveDetails'));
                // remove the command from the cached data - not sure this is necessary but better not leave inconsistent states
                ROE.Troops.InOut2.removeCommandByEventID(data.eventID);
                //
                // update row that presents the deleted command to look like it was deleted but do not remove. remove will be handled by the refreshbutton
                //
                var cancelledCmdRow = tbody.find(CONST.Selectors.tbody.cmdInListByEventID(data.eventID));

                cancelledCmdRow.addClass(CONST.CssClass.cmdActiveStatus.deleted).find(CONST.Selectors.countDown).remove();
                cancelledCmdRow.find(CONST.Selectors.arriveOn).remove();
                cancelledCmdRow.find(CONST.Selectors.sfxOpen).removeClass(CONST.CssClass.sfxOpen);
                cancelledCmdRow.find(CONST.Selectors.template.cmdDetails).remove();

                // remove the BUSY animation if present
                $('#loadcancel_eid' + data.eventID).remove();

                refreshButton.fadeIn();
            }
        }

        var _handleFilterOfCmdDetailsDisplay = function _handleFilterOfCmdDetailsDisplay(event) {
            ROE.UI.Sounds.click();
            event.stopPropagation();
            var filterButton = $(event.target);

            var cmdRow = filterButton.parents(CONST.Selectors.cmdRow);
            var FilterObj = {};
            var cmd;

            //TODO:
            // here we reset the filter, like this:
            //      filterByToFrom = { toPID:X  };
            // but maybe we shoudl add to it, like this :
            //      filterByToFrom.toPID = X;
            //
            // ????

            cmd = ROE.Troops.InOut2.findCommandByEventID_Rich(direction, cmdRow.attr(CONST.Attr.cmdRow.eventID))
            if (filterButton.is(CONST.Selectors.CmdDetails.filterButtons.destinationPlayer)) {
                FilterObj = new ROE.Troops.InOutWidget.toFromFilter(undefined, undefined, undefined, cmd.destinationPlayer)
            } else if (filterButton.is(CONST.Selectors.CmdDetails.filterButtons.destinationVillage)) {
                FilterObj = new ROE.Troops.InOutWidget.toFromFilter(undefined, undefined, cmd.destinationVillage)
            } else if (filterButton.is(CONST.Selectors.CmdDetails.filterButtons.originPlayer)) {
                FilterObj = new ROE.Troops.InOutWidget.toFromFilter(undefined, cmd.originPlayer, undefined, undefined)
            } else if (filterButton.is(CONST.Selectors.CmdDetails.filterButtons.originVillage)) {
                FilterObj = new ROE.Troops.InOutWidget.toFromFilter(cmd.originVillage)
                //{ fromVID: cmdRow.attr('data-ovid') };
            }
            _applyToFromFilter(FilterObj);
        }

        var _handleFilterOfFilterPopup = function _handleFilterOfFilterPopup(event) {
            ROE.UI.Sounds.click();
            event.stopPropagation();
            var filterButton = $(event.target);
            var FilterObj = {};
            var cmd = ROE.Troops.InOut2.findCommandByEventID_Rich(direction, filterButton.attr(CONST.Attr.cmdRow.eventID))
            if (filterButton.is(CONST.Selectors.CmdDetails.filterButtons.destinationPlayer)) {
                FilterObj = new ROE.Troops.InOutWidget.toFromFilter(undefined, undefined, undefined, cmd.destinationPlayer)
            } else if (filterButton.is(CONST.Selectors.CmdDetails.filterButtons.destinationVillage)) {
                FilterObj = new ROE.Troops.InOutWidget.toFromFilter(undefined, undefined, cmd.destinationVillage)
            } else if (filterButton.is(CONST.Selectors.CmdDetails.filterButtons.originPlayer)) {
                FilterObj = new ROE.Troops.InOutWidget.toFromFilter(undefined, cmd.originPlayer, undefined, undefined)
            } else if (filterButton.is(CONST.Selectors.CmdDetails.filterButtons.originVillage)) {
                FilterObj = new ROE.Troops.InOutWidget.toFromFilter(cmd.originVillage)
            }
            _applyToFromFilter(FilterObj);
            $(this).parents('.filterPopup').remove();
        }

        //this builds a popup based on the old code / elements in the command row, perhaps there is a better way to do it
        var _cmdRowFilterPopup = function _cmdRowFilterPopup(event) {
            ROE.UI.Sounds.click();
            event.stopPropagation();
            var cmdRow = $(event.target).parents(CONST.Selectors.cmdRow);
            var cmdEventID = cmdRow.attr(CONST.Attr.cmdRow.eventID);
            var cmd = ROE.Troops.InOut2.findCommandByEventID_Rich(direction, cmdEventID);
            var content = "";
            var filterButton = null;
            var filterLabel = "";
            var iconUrl = "https://static.realmofempires.com/images/icons/M_FilterB.png";

            cmdRow.find('.filterButton').each(function () {
                filterButton = $(this);
                if (filterButton.is(CONST.Selectors.CmdDetails.filterButtons.destinationPlayer)) {
                    filterLabel = cmd.destinationPlayer.name;
                } else if (filterButton.is(CONST.Selectors.CmdDetails.filterButtons.destinationVillage)) {
                    filterLabel = cmd.destinationVillage.name;
                } else if (filterButton.is(CONST.Selectors.CmdDetails.filterButtons.originPlayer)) {
                    filterLabel = cmd.originPlayer.name;
                } else if (filterButton.is(CONST.Selectors.CmdDetails.filterButtons.originVillage)) {
                    filterLabel = cmd.originVillage.name;
                }

                content += "<div class=\"" + filterButton.attr("class") + " fontDarkGoldFrLClrg\" data-eid=" + cmdEventID + " >" + filterLabel + "</div>";
            });
            if (filter_toFromContainer && !$.isEmptyObject(filterByToFrom)) {
                content += filter_toFromContainer.clone().html();
            }

            var commandList = filterButton.parentsUntil('.templateContent').parent().first();
            ROE.Frame.simplePopopOverlay(iconUrl, "Filters", content, "filterPopup", commandList);

            commandList.find('.filterPopup .clearFilter').click(function clearFilterFromPopup() {
                commandList.find('.filterPopup').remove();
                filter_toFromContainer.addClass("hidden");
                filterByToFrom = {};
                _filterChanged();
            });            
        }

        //this builds a popup based on the old code / elements in the command row, perhaps there is a better way to do it
        var _cmdRowProfilePopup = function _cmdRowProfilePopup(event) {
            ROE.UI.Sounds.click();
            event.stopPropagation();
            var cmdRow = $(event.target).parents(CONST.Selectors.cmdRow);
            var content = "";
            var anchor = null;
            var anchorPrefix = "";
            var iconUrl = "https://static.realmofempires.com/images/forum/F_ico_07.png";
            content += "<div class='fontDarkGoldFrLCmed'> Tap on any of the following names to launch either the village or player profile screen. </div>";
            var valid, anchorContent;
            cmdRow.find('.lineInfo a').each(function () {
                anchor = $(this);
                anchorContent = anchor.html()
                valid = true;
                if (anchorContent == "" || anchorContent == "(,)") {
                    valid = false;
                } else {
                    if (anchor.hasClass('dv')) {
                        anchorPrefix = 'Village:';
                    } else if (anchor.hasClass('ov')) {
                        anchorPrefix = 'Village:';
                    } else if (anchor.hasClass('op')) {
                        anchorPrefix = 'Player:';
                    } else if (anchor.hasClass('dp')) {
                        anchorPrefix = 'Player:';
                    } else {
                        valid = false;
                    }
                }

                if (valid) {
                    content += "<div class='profileAnchor'><span class='profileAnchorPrefix fontDarkGoldFrLCmed'>" +
                    anchorPrefix + " </span><span class='fontDarkGoldFrLClrg'>" + anchor[0].outerHTML + "</span></div>";
                }
            });
            ROE.Frame.simplePopopOverlay(iconUrl, "Profiles", content, "profilePopup", anchor.parentsUntil('.templateContent').parent().first());
        }

        var _applyToFromFilter = function _applyToFromFilter(thefilter) {
            /// <summary>
            /// </summary>
            /// <param name="thefilter" type="ROE.Troops.InOutWidget.toFromFilter"></param>

            if (!thefilter) {
                thefilter = {};
            }

            filterByToFrom = thefilter;


            if (filter_toFromContainer) {
                //
                // if we allow filtering 
                //
                if (
                   ((filterByToFrom.originPlayer)
                    || (filterByToFrom.originVillage)
                    || (filterByToFrom.destinationPlayer)
                    || (filterByToFrom.destinationVillage))
                    ) {

                    filter_toFromContainer.find(".toVorP").addClass("hidden");
                    // create a template
                    if (filterByToFrom.destinationVillage)
                    {
                        filter_toFromContainer.find(".toVorP.toV").removeClass("hidden");
                        filter_toFromContainer.find(".actualVorP").text("%destinationVillage.name%(%destinationVillage.x%,%destinationVillage.y%)".format(filterByToFrom));
                    }
                    if (filterByToFrom.destinationPlayer) {
                        filter_toFromContainer.find(".toVorP.toP").removeClass("hidden");
                        filter_toFromContainer.find(".actualVorP").text("%destinationPlayer.name%".format(filterByToFrom));
                    }
                    if (filterByToFrom.originVillage) {
                        filter_toFromContainer.find(".toVorP.fromV").removeClass("hidden");
                        filter_toFromContainer.find(".actualVorP").text("%originVillage.name%(%originVillage.x%,%originVillage.y%)".format(filterByToFrom));
                    }
                    if (filterByToFrom.originPlayer) {
                        filter_toFromContainer.find(".toVorP.fromP").removeClass("hidden");
                        filter_toFromContainer.find(".actualVorP").text("%originPlayer.name%".format(filterByToFrom));
                    }
                    filter_toFromContainer.removeClass("hidden");
                    // filterTemplate += " <a class=clearFilter><img src='https://static.realmofempires.com/images/icons/M_X.png</a>' /></a>";
                    //filter_toFromContainer.html(filterTemplate);


                } else {
                    //
                    // filter empty, so clear it
                    //
                    filter_toFromContainer.addClass("hidden");
                    _filterChanged();
                }
            }

            _filterChanged();
        }

        var _doDetails_d2 = function (event) {
            var cmdTR = $($(event.target).parents(CONST.Selectors.cmdRow)[0]);
            $(event.target).parent().html('<img class=loading src="https://static.realmofempires.com/images/misc/busy_tinyred.gif" />');
            var eventID = cmdTR.attr(CONST.Attr.cmdRow.eventID);
            ROE.Api.call_troopMove_getDetails(_doDetails_d2_OnComplete, eventID)
        }

        function _doDetails_d2_OnComplete(data) {
            _populateD2RowDetails(data, tbody);
        }

        //_registerDataUpdatedListener(direction, _signalNewDataAvailable);

        currentMaxRowsToDisplay = settings.initPageSize;
        currentMoreRowsIncrament = currentMaxRowsToDisplay;

        _populate();

        ROE.Player.refresh(); // trigger a refresh of data so that we get latest asap


        return {
            ToFromFilter: function (applyFilterByToFrom) {
                /// <summary>
                /// </summary>
                /// <param name="filterByToFrom" type="Object"> { fromVID : X, fromPID : X, toVID : X, toPID : X } all params are optional, but object it self must be at least {}</param>
                _applyToFromFilter(applyFilterByToFrom);


            }
        };
    }

    var _findCmdInHtmlList = function _findCmdInHtmlList(cmdListHtml, cmdListHtmlCurLoc, cmdToLocate) {
        /// <summary>RETURNS 
        ///        There are just 2 states- if found and if not, and different object is returned for each case, see in code
        ///                 </summary>
        /// <param name="cmdListHtml" type="Object"></param>
        /// <param name="cmdListHtmlCurLoc" type="Object"></param>
        /// <param name="cmdToLocate" type="Object"></param>


        // Optimization ideas 
        //   - is this efficient? cmdHtml = $(cmdListHtml[i])

        // TODO 
        //  - probably be safer to always start parsing the htlm list from start cause for attacks that land on the same time, things could get tricky


        for (var i = cmdListHtmlCurLoc, cmdHtml; cmdHtml = cmdListHtml[i]; ++i) {
            cmdHtml = $(cmdHtml);

            // is this the item we want ?
            if (cmdHtml.attr('data-eid') == cmdToLocate.eid) {
                return { found: true, foundAtIndex: i, cmdHtml : cmdHtml };
            }

            // is the current html item land AFTER the item we want? if so, then we know we have gone too far
            if (parseInt(cmdHtml.attr('data-etime'), 10) > parseInt(cmdToLocate.etime, 10)) {
                break;
            }
        }


        return {found:false, stoppedOnIndex: i};
    }

    var _displayThisCommandByFilter = function _displayThisCommandByFilter(cmd, limitByCurrentVillage, cmdTypeFilterStatus, filterByToFrom, direction) {
        //
        // FILTER CHECK - is this command to be disaplyed due to filter settings ?
        //

        //
        // display all or limit by current village?
        if (limitByCurrentVillage) {
            if (direction == ROE.Troops.InOut2.Enum.Directions.incoming) {
                // incoming - make sure incoming is to current village only
                if (cmd.dvid !== ROE.SVID) {
                    return false;
                }
            } else {
                // outgoing - make sure outgoing is from current village only
                if (cmd.ovid !== ROE.SVID) {
                    return false;
                }
            }
        }

        //
        // limit by FROM / TO village/player
        //
        if (
               (filterByToFrom.originPlayer && filterByToFrom.originPlayer.id != cmd.opid)
            || (filterByToFrom.originVillage && filterByToFrom.originVillage.id != cmd.ovid)
            || (filterByToFrom.destinationPlayer && filterByToFrom.destinationPlayer.id != cmd.dpid)
            || (filterByToFrom.destinationVillage && filterByToFrom.destinationVillage.id != cmd.dvid

            )
            )
        {
            return false;
        }

        //
        // does the filter allow displaying this?
        if (cmd.hidden == 1) {
            if (!cmdTypeFilterStatus || cmdTypeFilterStatus.hidden == 0) {
                return false;
            }
        }
        if (cmdTypeFilterStatus) {
            if (cmd.type == ROE.Troops.InOut2.Enum.CommandTypes.attack && cmdTypeFilterStatus.attack == 0) {
                return false;
            }
            else if (cmd.type == ROE.Troops.InOut2.Enum.CommandTypes.support && cmdTypeFilterStatus.support == 0) {
                return false;
            } else if ((cmd.type == ROE.Troops.InOut2.Enum.CommandTypes.returning || cmd.type == ROE.Troops.InOut2.Enum.CommandTypes.supportRecall) && cmdTypeFilterStatus.returning == 0) {
                return false;
            }
        }

        return true;

    }

    var _reconsileHtmlandDataList = function _reconsileHtmlandDataList(data, cmdListHtml, limitByCurrentVillage, cmdTypeFilterStatus, filterByToFrom, direction, widgetID, rowTemplate) {
        /// <summary></summary>
        /// <param name="data" type="Object"></param>
        /// <param name="cmdListHtml" type="Object"></param>
        /// <param name="limitByCurrentVillage" type="Object"></param>
        /// <param name="cmdTypeFilterStatus" type="Object"></param>
        /// <param name="filterByToFrom" type="ROE.Troops.InOutWidget.toFromFilter"></param>
        /// <param name="direction" type="Object"></param>

        var cmdListHtmlCurLoc = 0; // the current location of the commands in HTML that we are in, we aw traverse it to reconsile html with new data
        var findCmdInHtmlListResult; // index of a command located in HTML (an index of the TR basically representing this command)
        var rebuiltCmdRowHtml;


        for (var i = 0, cmd; cmd = data.commands[i]; ++i) {
            // these two properties should not be there as they are cleaned up after they are used, however, 
            //  if there is some bug or somethign, then we must delete those or this function will not work. 
            //  as as a sanity check, we clean the slate
            delete cmd.insertBeforeThisHtml;
            delete cmd.insertAtTheEnd;

            //
            // check if we are to display this cmd due to filter settings
            //
            if (!_displayThisCommandByFilter(cmd, limitByCurrentVillage, cmdTypeFilterStatus, filterByToFrom, direction)) {
                continue;
            }

            //
            // locate this commad in html 
            //
            findCmdInHtmlListResult = _findCmdInHtmlList(cmdListHtml, cmdListHtmlCurLoc, cmd)
            if (findCmdInHtmlListResult.found) {
                //
                // found it, mark it as found. 
                //  if found to be changed, then update the row by recreating it. 
                //
                if (_hasRowChangedSomehow(cmd, findCmdInHtmlListResult.cmdHtml, data)) {
                    // since there are changed to the displayed row, recreate it and update the row
                    rebuiltCmdRowHtml = _buildNewCmdTR(data, cmd, direction, widgetID, rowTemplate);
                    findCmdInHtmlListResult.cmdHtml.replaceWith(rebuiltCmdRowHtml);
                    findCmdInHtmlListResult.cmdHtml = rebuiltCmdRowHtml;
                    BDA.Console.verbose('ROE.InOutW', 'updating row because it changed');
                }
                findCmdInHtmlListResult.cmdHtml.addClass(CONST.CssClass.cmdActiveStatus.ok);
                cmdListHtmlCurLoc = findCmdInHtmlListResult.foundAtIndex + 1; // this can be removed, it is just for performance so we dont always start at 0
            } else {
                if (findCmdInHtmlListResult.stoppedOnIndex >= cmdListHtml.length) {
                    cmd.insertAtTheEnd = true;
                } else {
                    cmd.insertBeforeThisHtml = $(cmdListHtml[findCmdInHtmlListResult.stoppedOnIndex]);
                }

            }
        }

    }

    function _hasRowChangedSomehow(newcmd, oldcmdHTML, data) {
        var changed = false;
        // check if owner of the villages has changed, or perhaps village names have changed 
        changed = changed ? changed : newcmd.opid != oldcmdHTML.attr(CONST.Attr.cmdRow.originPlayerID);
        changed = changed ? changed : newcmd.dpid != oldcmdHTML.attr(CONST.Attr.cmdRow.destPlayerID);
        changed = changed ? changed : data.villages[newcmd.dvid].name != oldcmdHTML.attr(CONST.Attr.cmdRow.destVillageName);
        if (oldcmdHTML.attr(CONST.Attr.cmdRow.originVillageID) != CONST.IDs.unknownVillageID) { // if attack is unknown... 
            changed = changed ? changed : data.villages[newcmd.ovid].name != oldcmdHTML.attr(CONST.Attr.cmdRow.originVillageName);
        }

        return changed;
    } 

    var _buildNewCmdTR = function _buildNewCmdTR(data, cmd, direction, widgetID, rowTemplate) {
        var op;
        var dv;
        var ov;
        var dp;
        var timeleft;
        var eventTime;
        var now = new Date();
        var rowPopulated;

        op = data.players[cmd.opid];    //origin player id -- can be nothing is origin of attack is unknow; or when this is your outgoing command
        dp = data.players[cmd.dpid];    //destination player id
        dv = data.villages[cmd.dvid];   //destination village id
        ov = data.villages[cmd.ovid];   //origin village id -- can be nothing is origin of attack is unknow
        eventTime = new Date(cmd.etime);
        timeleft = BDA.Utils.timeLeft(cmd.etime);
        thisRowTemplate = rowTemplate.clone();

        //
        // we dont display rebel or abandoned player names, so remove it
        if (ROE.isSpecialPlayer(cmd.opid)) { thisRowTemplate.find('.opname').remove() }
        if (ROE.isSpecialPlayer(cmd.dpid)) { thisRowTemplate.find('.dpname').remove() }
        //
        // if unknown origin (ie, visible to target ==1 ) then remove the player name etc. if not, then remove the "Unknown..." text
        if (cmd.visibleToTarget < 2 && direction == ROE.Troops.InOut2.Enum.Directions.incoming) {
            thisRowTemplate.find('.opname').remove();
            thisRowTemplate.find('.ov').remove();
            thisRowTemplate.find('.ovcords').remove();
        } else {
            thisRowTemplate.find('.ovunknown').remove();
        }


        //
        // populate the row 
        //
        rowPopulated = BDA.Templates.populate(thisRowTemplate[0].outerHTML
        , {
            op: op
            , dp: dp
            , ov: ov
            , dv: dv
            , cmd: cmd
            , timeleft: timeleft
            , arrivalTime: BDA.Utils.formatEventTime(new Date(cmd.etime), true)
            , type: getTypeShortStr(cmd.type)
            , hidden: cmd.hidden ? "hidden" : "vis"
            , widgetID : widgetID
            , speed: cmd.speed ? cmd.speed : '?'
            , morale: cmd.morale > -32768 ? cmd.morale : '?'
        });
        rowPopulated = $(rowPopulated);
        //rowPopulated.addClass(rowHighlightClass);

        if (!_canCancel(cmd.type, cmd.opid, eventTime, ROE.playerID)) {
            rowPopulated.find(".troopIconSheet.cancel").remove();
        }
        if (!_canSeeDetails(cmd.type, cmd.opid, ROE.playerID)) {
            rowPopulated.find(".troopIconSheet.rep").remove();
        }

        return rowPopulated;
    }

    var _populateRows2 = function (tbody, data, cmdTypeFilterStatus, limitByCurrentVillage, currentMaxRowsToDisplay, filterByToFrom, footer, direction, refreshButton, widgetID, rowTemplate) {
        /// <param name="filterByToFrom" type="Object"> { fromVID : X, fromPID : X, toVID : X, toPID : X } all params are optional, but object it self must be at least {}</param>

        var thisRowTemplate; // we modify the template sometimes to this holds the modoified template for just the current row
        var loadMore;
        var cmdListHtml; // the list of commands currently displayed in HTML
        var cmdListHtml_notOK;
        var counter =1;
        //var cmdInHtml; // command located in HTML (a TR basically representing this command)
        var rowToEnterNewRowBefore;
        var rowPopulated;
        var changesFound = false;
        var displayNewCmdsImmediatelly = false;


        // remove the no troops message if present. 
        tbody.find('tr.noTroopsMsg').remove();

        footer.hide(); // default bahaviour

        //TODO : handle 'load more'
        //TODO : what happens when user click on delete or cancelled command? will it crash? will details be disaplyed?


        /*
        PLAN 
            - step through the list of commands in 'data'
            - check if the command in data exists in the HTML
            -   if so, mark the row in html as "found"
            -   if not, remember the row where the new data need to be inserted before that row. 
            - go through the list of commands in 'data' again, and insert the new command to the table
            - once done with the list of commands in 'data', now look through the HTML table
            - for any row not marked as 'found', remove it from HTML (or at least signal that removal is needed)

        */
        cmdListHtml = tbody.find(CONST.Selectors.tbody.listOfCmds);
        if (cmdListHtml.length > 0) {
            cmdListHtml.removeClass(CONST.CssClass.cmdActiveStatus.ok);
            cmdListHtml.removeClass(CONST.CssClass.cmdActiveStatus.deleted);
        } else {
            // there are no comamnds at all right now displayed, so just display the new ones right away
            displayNewCmdsImmediatelly = true;
        }


        _reconsileHtmlandDataList(data, cmdListHtml, limitByCurrentVillage, cmdTypeFilterStatus, filterByToFrom, direction, widgetID,rowTemplate);

        //
        // go through the list, and add items that need to be added
        //
        var now = new Date().getTime();

        for (var l = 0, cmd; cmd = data.commands[l]; ++l) {
            if (cmd.etime < now) {
                continue; // we discard command which already (at least should have) completed
            }
            if (!cmd.insertBeforeThisHtml && !cmd.insertAtTheEnd) {
                continue;
            }
            changesFound = true;

            counter++;
            rowPopulated = _buildNewCmdTR(data, cmd, direction, widgetID, rowTemplate);
            rowPopulated.addClass(CONST.CssClass.cmdActiveStatus.ok);
            if (!displayNewCmdsImmediatelly) {
                rowPopulated.addClass(CONST.CssClass.cmdActiveStatus.newCmd);
            }

            if (cmd.insertBeforeThisHtml) {
                cmd.insertBeforeThisHtml.before(rowPopulated);
            } else {
                // ie, we know that cmd.insertAtTheEnd == true here
                tbody.append(rowPopulated);
            }

            //if data about event exists (memory only for now) then show it
            //but perhaps store the event in a more persistent state -farhad
            if (_storedInOutEvents[cmd.eid]) {
                _populateD2RowDetails(_storedInOutEvents[cmd.eid], tbody);
            }
            
        }

        //
        // clean up references from data to HTML as this could have disastrus memory leaks
        //
        for (var j = 0, cmd; cmd = data.commands[j]; ++j) {
            delete cmd.insertBeforeThisHtml;
            delete cmd.insertAtTheEnd;
        }

        //
        // now go through the HTML and remove any commands that are no longer active
        //
        cmdListHtml = tbody.find(CONST.Selectors.tbody.listOfCmds); // reload the list of commands in HTML
        cmdListHtml_notOK = cmdListHtml.not(CONST.Selectors.listOfCmds.activeStatusOK);
        cmdListHtml_notOK.addClass(CONST.CssClass.cmdActiveStatus.deleted).find(CONST.Selectors.countDown).remove();
        cmdListHtml_notOK.find(CONST.Selectors.arriveOn).remove();
        cmdListHtml_notOK.find(CONST.Selectors.sfxOpen).removeClass(CONST.CssClass.sfxOpen);

        if (!changesFound) {
            // well, we got no new rows, check if perhaps there were rows deleted 
            changesFound = tbody.find(CONST.Selectors.tbody.listOfCmds).not(CONST.Selectors.listOfCmds.activeStatusOK).length > 0;
        }

        if(refreshButton){
            if (displayNewCmdsImmediatelly) {
                refreshButton.stop().fadeOut('slow');
            } else if (changesFound) {
                refreshButton.stop().fadeIn('slow');
            }
        }

        _condenseActiveNewRows(tbody);
        initTimers(); // make sure the countdown timers are taken care of
        InitStripeTable(); // make sure the hover over on tables works, if in desktop

    }

    ///This will bunch newCommands together and show only the top one, and adds '.extraNewHidden' to the extra cmds to hide them
    var _condenseActiveNewRows = function (tbody) {

        tbody.find('.extraNewHidden').removeClass('extraNewHidden');
        tbody.find('.rowCounter').remove();

        var topNewCmd = null, newCmdCount = 1;
        tbody.find('tr.cmd').each(function () {
            if ($(this).hasClass('activeState-new')) {
                if ($(this).prev().hasClass('activeState-new')) {
                    newCmdCount++;
                    $(this).addClass('extraNewHidden');
                } else {
                    topNewCmd = $(this);
                }
            } else {
                if (topNewCmd) {
                    topNewCmd.append('<span class="rowCounter">' + newCmdCount + '</span>');
                    topNewCmd = null;
                    newCmdCount = 1;
                }
            }
        });
        if(topNewCmd){
            topNewCmd.append('<span class="rowCounter">' + newCmdCount + '</span>');
            topNewCmd = null;
            newCmdCount = 1;
        }


    }

    var getContextInfo = function getContextInfo() {

    };

    var _handleVillageNameClick = function (event) {
        //
        // handle village name link clicks in mobile mode
        //  if the village is mine, then just switch to VOV for this village
        //  if the village is not mine, then popup (or open a new window) showing the other village overview
        //
        // this event should only be fired when "ROE.isMobile" is true but we are not checking it again here
        //
        event.preventDefault();

        var link = $(event.target);
        if (ROE.isMe(parseInt(link.attr('opid'), 10))) {
            ROE.Frame.switchToVoV(link.attr('vid'));
            link.parents('.iFrameDiv').find('.IFrameDivTitle .action.close').click(); // close the popup. this code is generic and shoudl work for any popup
        } else {
            ROE.Frame.popupVillageProfile(link.attr('vid'));
            //popupOtherVilageOverview('villageoverviewotherpopup.aspx?ovid=' + link.attr('vid'), link.attr('vid'));
        }
    }

    var getTypeShortStr = function (typeID) {
        switch (typeID) {
            case 1:
                return 'att';
            case 0:
                return 'sup'
            default:
                return 'ret'
        }
    }

    var _canCancel = function (commandTypeID, originPlayerID, eventTime, currentPlayerID) {
        var allow = false;
        var isAttackOfSupport = false;

        if (eventTime > new Date()) {
            isAttackOfSupport = commandTypeID == 1 || commandTypeID == 0;

            if (currentPlayerID == originPlayerID && isAttackOfSupport) {
                allow = true;
            }
        }
        return allow;
    }

    var _canSeeDetails = function (commandTypeID, originPlayerID, currentPlayerID) {
        var allow = false;
        var isReturnOrRecall = false;


        isReturnOrRecall = !(commandTypeID == 1 || commandTypeID == 0);

        if (isReturnOrRecall) {
            allow = true;
        } else if (currentPlayerID == originPlayerID) {
            allow = true;
        }

        return allow;
    }

    var _addShowMore = function (commandTypeID, originPlayerID, eventTime, currentPlayerID) {
    }

    var getCommandClassName = function (commandType) {
        switch (commandType) {
            case ROE.Troops.InOut2.Enum.CommandTypes.attack:
                return "att";
                break;
            case ROE.Troops.InOut2.Enum.CommandTypes.support:
                return "sup";
                break;
            case ROE.Troops.InOut2.Enum.CommandTypes.returning:
            case ROE.Troops.InOut2.Enum.CommandTypes.supportRecall:
                return "ret";
                break;

        }
    };

    var getCommandClassName2 = function (commandType) {
        switch (commandType) {
            case ROE.Troops.InOut2.Enum.CommandTypes.attack:
                return ".attack";
            case ROE.Troops.InOut2.Enum.CommandTypes.support:
                return ".support";
            case ROE.Troops.InOut2.Enum.CommandTypes.returning:
            case ROE.Troops.InOut2.Enum.CommandTypes.supportRecall:
                return ".return";
        }
    }

    var _doDetails_mobile = function (eventID, container) {

        Troops.GetTroopMovementDetails(eventID, function (arg) { _doDetails_mobile_OnComplete(container, arg); }, Details_OnTimeOut, Details_OnError);
    }

    var _doDetails_mobile_OnComplete = function (container, arg) {
        var det = eval(arg);


        // this is the image that was clicked to get the details. it ids the row and table. 
        //var loadingImg = $('#loaddet_eid' + det.eventID);

        var troopListContainer = container.find(".cmdTroopList");

        if (det.eventID == -1) {

            // FINISH


            troopListContainer.html('Error. Session Expired. Refresh page and try again.');
        } else if (det.eventID == 0) {

            // FINISH
            troopListContainer.html('Cannot get command details. Perhaps it already expired. Refresh page.');
        } else {
            // hide the busy message
            troopListContainer.find('.loading').remove();

            // populate the troops display
            troopListContainer.find('.unitcountnumb').each(function () {
                var uid = $(this).attr('data-troopid');
                var unitcount = det['ut' + uid] || 0;
                $(this).text(unitcount);
                $(this).toggleClass('ZeroUnitCount', unitcount == 0)
            })
            troopListContainer.find('table').show();

        }
    }

    function _commandCountdownAtZero(eid) {
        //HACK - this is called by the counter; we are not sure which widget we are in, so we are just searching the entire DOM. smart.
        var finishedRow = $(CONST.Selectors.tbody.cmdInListByEventID(eid));
        finishedRow.addClass(CONST.CssClass.cmdActiveStatus.deleted);
        finishedRow.find(CONST.Selectors.CmdDetails.cancelTroopMovementIcon).remove();
        finishedRow.find(CONST.Selectors.CmdDetails.toggleHideIcon).remove();
        finishedRow.find(".loadDetails").remove();
    }


    var _getFilterAction = function _getFilterAction(fromToColumn, vid, vname, vx, vy, widgetID) {
        ///<param name="fromToColumn">tell you from which column this is from. either 'from' or 'to'</param>

        /*
            from column
                filter on the village from (origin village)            
              to column 
                filter on the village TO (destination village)
        */

        var returnObj = {}

        returnObj.actions = ["<a class='sfx2 inOutFilter' onclick=\"ROE.Frame.popupInOut_filterSpecificWidget_toVillage(%widgetID%, %vid%, '%vname%', %vx%, %vy%, '%toOrFrom%');return false;\"></a>".format(
            { widgetID: widgetID, vid: vid, vname: vname, vx: vx, vy: vy, toOrFrom: fromToColumn }
            )];

        returnObj.helpTexts = ["Filter by this village"];

        return returnObj;
    }

    var _populateD2RowDetails = function _populateD2RowDetails(data,tbody) {

        _storedInOutEvents[data.eventID] = data; //store the event in memory

        var cmdRow = tbody.find(CONST.Selectors.tbody.cmdInListByEventID(data.eventID));
        var troop;
        var onTroopCell;
        cmdRow.find('.troopCount').text("0");
        cmdRow.find('.loadDetails').remove();
        cmdRow.find('.loading').remove();

        for (var i = 0, troop; troop = data.troops[i]; ++i) {
            onTroopCell = cmdRow.find('.troopCount[data-unitid=%id%]'.format(troop));
            if (onTroopCell) {
                onTroopCell.text(troop.count);
                onTroopCell.removeClass("ZeroUnitCount");
            }
        }
    }

    //
    // Public interface
    //
    obj.init = _init;
    obj.getContextInfo = getContextInfo;
    obj.commandCountdownAtZero = _commandCountdownAtZero;
    obj.getFilterAction = _getFilterAction;
    obj.storedInOutEvents = _storedInOutEvents;
    
}(window.ROE.Troops.InOutWidget = window.ROE.Troops.InOutWidget || {}));


//For M only, also assumes there is only one inOut popup. May need to change if in the future we have multiple inOut popups in M
ROE.Troops.InOutWidget.inoutPopupSwitchMode = function _inoutPopupSwitchMode(mode) {
    if (mode == "toInc") {
        $('.templ_inoutpopup .switchModeArea').removeClass('outSelected').addClass('incSelected');
        $('.templ_inoutpopup .tabContent.tabs-out.outgoingContainer').hide();
        $('.templ_inoutpopup .tabContent.tabs-in.incomingContainer').show();
    } else if (mode == "toOut") {
        $('.templ_inoutpopup .switchModeArea').removeClass('incSelected').addClass('outSelected');
        $('.templ_inoutpopup .tabContent.tabs-in.incomingContainer').hide();
        $('.templ_inoutpopup .tabContent.tabs-out.outgoingContainer').show();
    }
}


ROE.Troops.InOutWidget.toFromFilter = function (originVillage, originPlayer, destinationVillage, destinationPlayer) {
    ///<summary>constructs a new filter object </summary>
    ///<param name="originVillage">object of type ROE.Class.Village</param>
    this.originVillage = originVillage;
    this.originPlayer = originPlayer;
    this.destinationVillage = destinationVillage;
    this.destinationPlayer = destinationPlayer;
}
ROE.Troops.InOutWidget.toFromFilter.prototype =
{
    originVillage : undefined
    , originPlayer : undefined
    , destinationVillage: undefined
    , destinationPlayer: undefined
}
