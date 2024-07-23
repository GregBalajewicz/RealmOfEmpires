"use strict";
/**
* @suppress {suspiciousCode}
*/
(function (obj) {

    var CONST = {
        IDs: {unknownPlayerID : -999, unknownVillageID : -999}
    };
    CONST.Enum = {};

    CONST.Selectors = {
    }


    CONST.CssClass = {
    };

    CONST.Attr = {
    };
    var _rowTemplates = {};



    var _init = function (direction, containerForTable, options) {
        ///<param name="direction">one of ROE.Troops.InOut2.Enum.Directions</param>)
        ///<param name="containerForTable">jquery object where the table of commands should be located</param>
        ///<param name="options">options. 
        ///</param>

        BDA.Val.validate(direction, "unrecognized direction", false, ROE.Troops.InOut2.Enum.Directions.validate, obj.getContextInfo);

        var direction = direction;
        var template;
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
           
        }, options);

        // WAS var _loadIncoming_loaded = function (data) {
        var _populate = function () {

            if (ROE.isMobile) {
                template = $(BDA.Templates.getRaw("InOutTroopsSummary_m", ROE.realmID));
            } else {
                template = $(BDA.Templates.getRaw("InOutTroopsSummary_d2", ROE.realmID));
            }
            containerForTable.append(template);
            tbody = template.find(".inoutsummary .summary tbody");

            if (!_rowTemplates[direction]) {
                _rowTemplates[direction] = tbody.find('.summaryItem[data-direction=%d%]'.format({d:direction}));
                tbody.find('.summaryItem').remove();                
            }

            BDA.Broadcast.subscribe(containerForTable, "InOutgoingDataChanged", _handleInOutChanged);

            tbody.on("click", ".setupSupport ", _handleSetupSupportTargetClick);
            tbody.on("click", ".filtersCmdBtn", _handleFilterClick);
            tbody.on("click", ".helpButton", _handleHelpClick);

            template.find(".title .showall").click(_handleShowAllClick);
                    
            _populateContent(template, tbody, direction);

            //
            // populate all rows
            //

            //
            // handle some events
            //
        };

        function _handleSetupSupportTargetClick(event) {
            var clickedBtn = $(event.target);

            if (clickedBtn.hasClass('busy')) {
                return;
            }

            var summaryItem = clickedBtn.parents('.summaryItem');
            //console.log(summaryItem.attr('data-dvid'), summaryItem.attr('data-dvname'), summaryItem.attr('data-dvx'), summaryItem.attr('data-dvy'));
            ROE.Targets.autoCreateUpdateSupportTarget(summaryItem.attr('data-dvid'), clickedBtn);
        }

        function _handleFilterClick(event) {
            var filterButton = $(event.target);
            var summaryItem = filterButton.parents('.summaryItem');
            var FilterObj = new ROE.Troops.InOutWidget.toFromFilter(undefined, undefined
                , new ROE.Class.Village(summaryItem.attr('data-dvid'), summaryItem.attr('data-dvname'), summaryItem.attr('data-dvx'), summaryItem.attr('data-dvy')));
            ROE.Frame.popupInOut(direction ==  ROE.Troops.InOut2.Enum.Directions.incoming ? "in": "out", FilterObj);
        }

        function _handleShowAllClick(event) {
            var showAllButton = $(event.target);
            var direction = showAllButton.attr("data-direction");
            var FilterObj;
            if (direction == ROE.Troops.InOut2.Enum.Directions.incoming) {
                FilterObj = new ROE.Troops.InOutWidget.toFromFilter(undefined, undefined, undefined, new ROE.Class.Player(ROE.playerID, ROE.Player.name));
            } else {
                FilterObj = new ROE.Troops.InOutWidget.toFromFilter(undefined, new ROE.Class.Player(ROE.playerID, ROE.Player.name));
            }
            ROE.Frame.popupInOut(direction == ROE.Troops.InOut2.Enum.Directions.incoming ? "in" : "out", FilterObj);
        }

        function _handleHelpClick(event) {
            var button = $(event.target);
            var summaryItem = button.parents('.summaryItem');
            var i;
            var oneSummary;
            var helpText="";
            var oneSummaryObj;
            var mostSignificantIncoming;
            var miniSummary;
            var miniSummaryHtml;

            oneSummary = ROE.Troops.InOut2.getSummary(direction)[summaryItem.attr('data-dvid')];
            if (oneSummary) {
                mostSignificantIncoming = oneSummary.getMostSignificantIncoming();
                if (direction == ROE.Troops.InOut2.Enum.Directions.incoming) {
                    miniSummary = ROE.Troops.InOut2.getIncomingMiniSummary(summaryItem.attr('data-dvid'), 10);
                    miniSummaryHtml = ROE.Utils.getImcomingMiniIndicatorsHTML(miniSummary, 15, true);
                } else {
                    miniSummary = { length: 0 };
                    miniSummaryHtml = '';
                }
                helpText = "You have %num% %type% %intORout% to %vn%(%vx%,%vy%) with the first landing in <span class=countdown data-finishesOn='%finishesOn%'></span>".format(
                    {
                        num: mostSignificantIncoming.num,
                        type: _helper_getIncomingName(mostSignificantIncoming),
                        intORout: _helper_directionToText(direction),
                        vn: oneSummary.village.name,
                        vx: oneSummary.village.x,
                        vy: oneSummary.village.y,
                        finishesOn: mostSignificantIncoming.earliestLanding
                    }
                    );

                if (miniSummary.length > 1) {
                    helpText += "<BR><BR>And the %intORout% summary :<center><span class='incoming incomingMiniIndicators longMiniSummary'>%minisummary%</span></center><BR>tells you that you have:".format(
                        {
                            intORout: _helper_directionToText(direction),
                            minisummary: miniSummaryHtml
                        }
                        );

                    for (i = 0, oneSummaryObj; oneSummaryObj = miniSummary[i]; ++i) {
                        helpText += "<BR>-%num% %hidden% %type% %f% ".format(
                      {
                          num: oneSummaryObj.count,
                          hidden: oneSummaryObj.isHidden ? "hidden" : "",
                          type: oneSummaryObj.type == ROE.Troops.InOut2.Enum.CommandTypes.attack ? ("attack" + (oneSummaryObj.count < 2 ? "" : "s"))
                              : "support",
                          intORout: _helper_directionToText(direction),
                          f: (i + 1 != miniSummary.length && miniSummary.length >1) ? "followed by" : ""
                      }
                      );

                    }

                    helpText += "<BR>...%intORout% to this village".format(
                      {                         
                          intORout: _helper_directionToText(direction)
                      }
                      );


                }
               

                ROE.Frame.Confirm(helpText, "OK", undefined, "rgba(33,33,33,0.8)", function () { }, undefined, undefined,true);
            }
        }

        function _handleInOutChanged(dir) {
            if (dir === direction) {
                _populateContent(template, tbody, direction);
            }

            
        }

        _populate();

        ROE.Player.refresh(); // trigger a refresh of data so that we get latest asap
    }


    function _helper_directionToText(direction) {
        if (direction == 0 ) {
            return "incoming";
        } else {
            return "outgoing";
        }
    }
    function _helper_getIncomingName(mostSignificantIncoming) {
        if (mostSignificantIncoming.type == MostSignificantIncomingType["Attack"]) {            
            return 'attack' + (mostSignificantIncoming.num < 2 ? "" : "s");
        }
        else if (mostSignificantIncoming.type == MostSignificantIncomingType["AttackHidden"]) {
            return 'hidden attack' + (mostSignificantIncoming.num < 2 ? "" : "s");
        }
        else if (mostSignificantIncoming.type == MostSignificantIncomingType["Support"]) {
            return 'support';
        }
        else if (mostSignificantIncoming.type == MostSignificantIncomingType["SupportHidden"]) {
            return 'hidden support';
        }
    }
   
  
    /** @suppress {suspiciousCode} */
    function _populateContent(template, tbody, direction) {
        var summary = ROE.Troops.InOut2.getSummary(direction);

        template.find('.title:not([data-direction=%d%])'.format({ d: direction })).remove();
        if (summary.sortedList.length == 0) {
            tbody.find('.noSummaryMsg:not(.noSummaryMsg[data-direction=%d%])'.format({ d: direction })).remove();
            template.find('.title').remove();
            return;
        } else {
            tbody.find('.noSummaryMsg').remove();
        }

        var villageID;
        var data;
        var rowPopulated;
        var thisRowTemplate = _rowTemplates[direction].clone();
        var i;
        var miniSummary;

        tbody.empty();
        for (i = 0, villageID; villageID = summary.sortedList[i]; ++i) {
            data = summary[villageID];
            data.showPlayer = data.village.ownerName ? "" : "display:none";
            
            rowPopulated = BDA.Templates.populate(thisRowTemplate[0].outerHTML, data);
            rowPopulated = $(rowPopulated);

            //
            // figure out which type of indicator to keep and which to hide
            //
            if (data.numIncomingAttack > 0) {
                rowPopulated.find('.incomingMiniIndicator:not(.indicatorTypeAttack)').remove();
                rowPopulated.find('.incomingMiniIndicator.indicatorHidden').remove();
                rowPopulated.find('.earliestLanding:not(.attack)').remove();
            }
            else if (data.numIncomingAttackHidden > 0) {
                rowPopulated.find('.incomingMiniIndicator:not(.indicatorTypeAttack)').remove();
                rowPopulated.find('.incomingMiniIndicator:not(.indicatorHidden)').remove();
                rowPopulated.find('.earliestLanding:not(.attackHidden)').remove();
            }
            else if (data.numIncomingOther > 0) {
                rowPopulated.find('.incomingMiniIndicator:not(.indicatorTypeSupport)').remove();
                rowPopulated.find('.incomingMiniIndicator.indicatorHidden').remove();
                rowPopulated.find('.earliestLanding:not(.other)').remove();
            }
            else if (data.numIncomingOtherHidden > 0) {
                rowPopulated.find('.incomingMiniIndicator:not(.indicatorTypeSupport)').remove();
                rowPopulated.find('.incomingMiniIndicator:not(.indicatorHidden)').remove();
                rowPopulated.find('.earliestLanding:not(.otherHidden)').remove();
            }

            if (direction == ROE.Troops.InOut2.Enum.Directions.incoming) {
                miniSummary = ROE.Troops.InOut2.getIncomingMiniSummary(villageID);
                if (miniSummary.length > 1) {
                    rowPopulated.find('.incomingMiniIndicators.longMiniSummary').html(ROE.Utils.getImcomingMiniIndicatorsHTML(miniSummary, 10, true));
                } else {
                    rowPopulated.find('.longMiniSummaryContainer').remove();
                }
            }

            //if no attacks incoming, remove the setup target support btn
            if (data.numIncomingAttack < 1) {
                rowPopulated.find('.setupSupport').remove();
            }

            tbody.append(rowPopulated);

        }
    }


    //
    // Public interface
    //
    obj.init = _init;


}(window.ROE.Troops.InOutSummary = window.ROE.Troops.InOutSummary || {}));
