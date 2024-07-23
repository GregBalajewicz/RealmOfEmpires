

(function (obj) {

    BDA.Console.setCategoryDefaultView('ROE.InOut.Cache', false); // by default, do not display this category. 
    BDA.Console.setCategoryDefaultView('ROE.InOut', false); // by default, do not display the category. 

    //
    // CONSTS + Emums
    //
    var CONST =  //private constants
    obj.CONST = { apiNameByDirection: ['Incoming', 'Outgoing'], directions: [0, 1], directionNameByDirection: ['incoming', 'outgoing'] }; //public constants
    obj.Enum = {}; // enums
    obj.Enum.Directions = { incoming: 0, outgoing: 1 };
    obj.Enum.CommandTypes = { support: 0, attack: 1, returning: 2, supportRecall: 3 };

    //
    // Enum validators 
    //
    obj.Enum.Directions.validate = function (direction) {
        var dir;
        for (var i = 0; i < CONST.directions.length; ++i) {
            if (CONST.directions[i] === direction) {
                return true
            }
        }
        return false;
    };

    //
    // Private data and variables
    //
    var _cmdData = [{ players: [], villages: [], commands: [] }, { players: [], villages: [], commands: [] }];
    var _cacheTimeStamps = [];

    var _getData = function (direction) {
        ///<summary>get the incoming or outgoing troops data</summary>        
        BDA.Val.validate(direction, "unrecognized direction", false, ROE.Troops.InOut2.Enum.Directions.validate, obj.getContextInfo);
        return _cmdData[direction];
    };

    var _getIncomingData = function () {
        ///<summary>get the incoming troops data</summary>
        return _getData(obj.Enum.Directions.incoming);
    };
    var _getOutgoingData = function () {
        ///<summary>get the outgoing troops data</summary>
        return _getData(obj.Enum.Directions.outgoing);
    };


    //var invalidateCache = function () {
    //    _cmdData = [];
    //}
    var _removeCommandByEventID = function (eventID) {
        var data;
        var cmd;
        for (var i = 0; i < CONST.directions.length; ++i) {
            data = _cmdData[i];
            for (var j = 0, cmd; cmd = data.commands[j]; ++j) {
                if (cmd.eid == eventID) {
                    data.commands.splice(j, 1);
                    break;
                }
            }
        }
    }

    var _findVillage = function (direction, vid) {
        ///<summary> returns the data village object or null if not found</summary>
        BDA.Val.validate(direction, "unrecognized direction", false, obj.Enum.Directions.validate, obj.getContextInfo);

        var data = _cmdData[direction];
        return data.villages[vid];
    }

    var _findPlayer = function (direction, pid) {
        ///<summary> returns the data player object or null if not found</summary>
        BDA.Val.validate(direction, "unrecognized direction", false, obj.Enum.Directions.validate, obj.getContextInfo);

        var data = _cmdData[direction];
        return data.players[pid];
    }

    var _findCommandByEventIDAnyDir = function (eventID) {
        ///<summary> find command by this id from either incoming or outgoing troops</summary>
        var cmd;
        for (var i = 0; i < CONST.directions.length; ++i) {
            cmd = _findCommandByEventID(CONST.directions[i], eventID);
            if (cmd !== null) {
                return cmd;
            }
        }
        return null;
    }

    var _findCommandByEventID = function (direction, eventID) {
        BDA.Val.validate(direction, "unrecognized direction", false, obj.Enum.Directions.validate, obj.getContextInfo);

        var data = _cmdData[direction];
        var cmd;
        for (var i = 0, cmd; cmd = data.commands[i]; ++i) {
            if (cmd.eid == eventID) {
                return cmd;
            }
        }
        return null;
    }


    var _findCommandByEventID_Rich = function (direction, eventID) {
        var cmd = _findCommandByEventID(direction, eventID);
        var originVillage = _findVillage(direction, cmd.ovid) || "unknown village";
        var destVillage = _findVillage(direction, cmd.dvid);
        var destPlayer = cmd.dpid == ROE.playerID ? ROE.Player : _findPlayer(direction, cmd.dpid);
        var originPlayer;
        cmd.opid == -999 ? originPlayer = { name: "unknown player" } : 
        (originPlayer = cmd.opid == ROE.playerID ? ROE.Player : _findPlayer(direction, cmd.opid));       
        var cmdRich = new ROE.Troops.InOut2.Command(cmd.eid
            , new ROE.Class.Village(cmd.ovid, originVillage.name, originVillage.x, originVillage.y)
            , new ROE.Class.Player(cmd.opid, originPlayer.name)
            , new ROE.Class.Village(cmd.dvid, destVillage.name, destVillage.x, destVillage.y)
            , new ROE.Class.Player(cmd.dpid, destPlayer.name)
            );

        return cmdRich;
    }

    var _toggleHiddenStatusByEventID = function (eventID) {
        for (var i = 0; i < CONST.directions.length; ++i) {
            cmd = _findCommandByEventID(CONST.directions[i], eventID);
            if (cmd) {
                cmd.hidden = cmd.hidden == 0 ? 1 : 0;

                // since this action does not cause a refresh from back end (does not invalidate the command list), we trigger this to ensure all UI updates
                _summary[CONST.directions[i]] = undefined;
                BDA.Broadcast.publish("InOutgoingDataChanged", CONST.directions[i]);
            }
        }
    }


    // This has been moved to ROE-Troops-InOutWidget.js; leaving here for desktop version of the UI 
    var cancel = function (clickedElement, eventID) {
        ///<summary>called when cancel is clicked to cancel the command</summary>
        var container = $(clickedElement).parents('.widget_inOutTroops');


        ret = Troops.CancelTroopMovement(eventID,
            function (arg) { _cancel_OnComplete(container, arg); }, OnTimeOut, OnError);

        var infotext = "Troops movement cancelled";
        ROE.Frame.infoBar(infotext);

    }

    // This has been moved to ROE-Troops-InOutWidget.js; leaving here for desktop version of the UI 
    var _cancel_OnComplete = function (container, eventID) {
        ///<summary>called when cancel WS call completes</summary>
        if (eventID == -1) {

            //FINISH 

            $(row).removeClass('highlight').addClass('highlight_d1');
            $(row).after('<tr class=highlight_u1><TD></TD><td style=padding:0px colspan=5>Error. Session Expired. Refresh page and try again</td></tr>');
            $(loadingImg).remove();
            InitStripeTable();
        } else if (eventID == 0) {

            //FINISH 

            $(row).removeClass('highlight').addClass('highlight_d1');
            $(row).after('<tr class=highlight_u1><TD></TD><td style=padding:0px colspan=5>Command NOT cancelled. Perhaps it already expired. Refresh page.</td></tr>');
            $(loadingImg).remove();
            InitStripeTable();
        } else {
            //
            // update the cached commands - remove it. This must be also done in the desktop version
            //
            BDA.UI.Transition.slideRight(container.find('.cmdList'), container.find('.troopMoveDetails'));

            _removeCommandByEventID(eventID);

            var row = container.find('.cmd[data-eid=' + eventID + ']')
            $(row).fadeOut('slow', function () {
                $(this).remove();
            });
        }
    }

    // This will need to move to ROE-Troops-InOutWidget.js
    var details = function (clickedElement, eventID) {
        var container = $(clickedElement).parents('.widget_inOutTroops');
        ret = Troops.GetTroopMovementDetails(eventID, function (arg) { _details_OnComplete(container, arg); }, Details_OnTimeOut, Details_OnError);
        container.find('.troopMoveActions').fadeOut();
    }

    // This will need to move to ROE-Troops-InOutWidget.js
    var _details_OnComplete = function (container, arg) {
        var det = eval(arg);


        // this is the image that was clicked to get the details. it ids the row and table. 
        //var loadingImg = $('#loaddet_eid' + det.eventID);
        var row = container.find('.cmd[data-eid=' + det.eventID + ']')
        var cell = row.find(".data");
        var table = $(row).parent();

        if (det.eventID == -1) {

            // FINISH

            $(row).removeClass('highlight').addClass('highlight_d1');
            $(row).after('<tr class=highlight_u1><TD></TD><td style=padding:0px colspan=5>Error. Session Expired. Refresh page and try again</td></tr>');
            $(loadingImg).remove();
            InitStripeTable();
        } else if (det.eventID == 0) {

            // FINISH

            $(row).removeClass('highlight').addClass('highlight_d1');
            $(row).after('<tr class=highlight_u1><TD></TD><td style=padding:0px colspan=5>Cannot get command details. Perhaps it already expired. Refresh page.</td></tr>');
            $(loadingImg).remove();
            InitStripeTable();
        } else {
            str = '<table  class=troopsdetails border=0 cellpadding=0 cellspacing=1><tr class="TableHeaderRow"><td align="center"><img src="https://static.realmofempires.com/images/units/Militia.png" /></td><td align="center"><img title="Infantry" src="https://static.realmofempires.com/images/units/Infantry.png"/></td><td align="center"><img src="https://static.realmofempires.com/images/units/Cavalry.png" /></td><td align="center"><img title="Knight" src="https://static.realmofempires.com/images/units/Knight.png" /></td><td align="center"><img title="Ram" src="https://static.realmofempires.com/images/units/ram.png" /></td><td align="center"><img title="Trebuchet" src="https://static.realmofempires.com/images/units/treb.png" /></td><td align="center"><img title="Spy" src="https://static.realmofempires.com/images/units/Spy.png" /></td><td align="center"><img title="Governor" src="https://static.realmofempires.com/images/units/Governor.png"  /></td>';
            str += '</tr><tr>';
            str += '<td align="right" ' + (det.ut11 ? '' : 'class=ZeroUnitCount ') + '>' + (det.ut11 ? det.ut11 : 0) + '</td>';
            str += '<td align="right" ' + (det.ut2 ? '' : 'class=ZeroUnitCount ') + '>' + (det.ut2 ? det.ut2 : 0) + '</td>';
            str += '<td align="right" ' + (det.ut5 ? '' : 'class=ZeroUnitCount ') + '>' + (det.ut5 ? det.ut5 : 0) + '</td>';
            str += '<td align="right" ' + (det.ut6 ? '' : 'class=ZeroUnitCount ') + '>' + (det.ut6 ? det.ut6 : 0) + '</td>';
            str += '<td align="right" ' + (det.ut7 ? '' : 'class=ZeroUnitCount ') + '>' + (det.ut7 ? det.ut7 : 0) + '</td>';
            str += '<td align="right" ' + (det.ut8 ? '' : 'class=ZeroUnitCount ') + '>' + (det.ut8 ? det.ut8 : 0) + '</td>';
            str += '<td align="right" ' + (det.ut12 ? '' : 'class=ZeroUnitCount ') + '>' + (det.ut12 ? det.ut12 : 0) + '</td>';
            str += '<td align="right" ' + (det.ut10 ? '' : 'class=ZeroUnitCount ') + '>' + (det.ut10 ? det.ut10 : 0) + '</td>';
            str += '</tr></table>';

            cell.find('.troopsdetails').remove(); // remove the table if already there - BETTER to not allow get details again instead
            cell.append(str);


            InitStripeTable();
        }
    }

    var getContextInfo = function getContextInfo() {
        return {
            cmdData: _cmdData
        };
    };

    var checkIfFrefreshIsNeeded = function (timeStamp_incoming, timeStamp_outgoing) {
        var newTimeStamps = [timeStamp_incoming, timeStamp_outgoing];
        for (var i = 0; i < CONST.directions.length; ++i) {

            //
            // if, our saved time stamp (which represents when we got the data last time) is UNDEFINED, then we must get the data no matter what; 
            // else, we only get the data, if the new timestamp (which represents the last time the data changed) is higher then the timestamp when we last 
            //  retrived the value
            //
            if (
                _cacheTimeStamps[i] === undefined ||
                 _cacheTimeStamps[i] < newTimeStamps[i] 
                )
            {
                BDA.Console.verbose('ROE.InOut.Cache', 'In Refresh - getting ' + CONST.directionNameByDirection[i])
              
                ROE.Api.call(CONST.apiNameByDirection[i], {}, function () {
                    var currentDirection = i;
                    return function (data) { _getInOrOutGoing_loaded(currentDirection, data); }
                }()
                    );
            } else {
                BDA.Console.verbose('ROE.InOut.Cache', 'In Refresh - NOT getting ' + CONST.directionNameByDirection[i])
            }
        }
    }

    var _getInOrOutGoing_loaded = function (direction, data) {
        _cmdData[direction] = data;
        _cacheTimeStamps[direction] = data.cacheTimeStamp;
        _summary[direction] = undefined;

        BDA.Broadcast.publish("InOutgoingDataChanged", direction);
    }


    function _getIncomingMiniList(destinationVillagID) {
        ///<summary>get any incoming in short format</summary>
        ///<param name="destinationVillagID">village ID, OPTIONAL!</param>

        var cmdData = _getIncomingData();
        var cmd;
        var i;
        var incomingList = [] //[{ eventID, type, landingTime, isHidden }...]
        var now = new Date().getTime();

        for (i = 0, cmd; cmd = cmdData.commands[i]; ++i) {
            // if destinationVillagID specified, then make sure this command is for this village, if not, then just ignore it
            if (!destinationVillagID || cmd.dvid == destinationVillagID) {
                if (cmd.etime > now) {
                    if (!(cmd.type != ROE.Troops.InOut2.Enum.CommandTypes.attack || cmd.type != ROE.Troops.InOut2.Enum.CommandTypes.support)) {
                        continue;
                    }
                    incomingList.push({ eventID: cmd.eid, type: cmd.type, landingTime: cmd.etime, isHidden: cmd.hidden });
                }
            }
        }

        return incomingList;
    }

    function _getIncomingMiniSummary(destinationVillagID) {
        ///<summary>get any incoming in short format</summary>
        ///<param name="destinationVillagID">village ID, OPTIONAL!</param>

        var cmdData = _getIncomingData();
        var cmd;
        var i;
        var incomingList = [] //[{ type, isHidden, count }...]
        var now = new Date().getTime();
        var currentItem; //[{ type, isHidden, count }]
        for (i = 0, cmd; cmd = cmdData.commands[i]; ++i) {
            // if destinationVillagID specified, then make sure this command is for this village, if not, then just ignore it
            if (!destinationVillagID || cmd.dvid == destinationVillagID) {
                if (cmd.etime > now) {
                   
                    if (currentItem === undefined)
                    {
                        currentItem = { count: 0 };
                        incomingList.push(currentItem);
                    } else {
                        if (
                               (currentItem.type == ROE.Troops.InOut2.Enum.CommandTypes.attack && cmd.type != ROE.Troops.InOut2.Enum.CommandTypes.attack) // if last command attack but this one is not
                            || (currentItem.type == ROE.Troops.InOut2.Enum.CommandTypes.support // if last command support but this one not support, return or recall (note that support, return or recall are all considered support here)
                                && !(   cmd.type == ROE.Troops.InOut2.Enum.CommandTypes.support 
                                        || cmd.type == ROE.Troops.InOut2.Enum.CommandTypes.returning
                                        || cmd.type == ROE.Troops.InOut2.Enum.CommandTypes.supportRecall
                                    )
                                )
                            || currentItem.isHidden != cmd.hidden
                            )
                        {
                            // if this command is different than the last one 
                            
                            currentItem = incomingList[incomingList.push({}) - 1];
                            currentItem.count = 0;
                        }
                    }
                    currentItem.type = cmd.type == ROE.Troops.InOut2.Enum.CommandTypes.attack ? ROE.Troops.InOut2.Enum.CommandTypes.attack : ROE.Troops.InOut2.Enum.CommandTypes.support;
                    currentItem.isHidden = cmd.hidden;
                    currentItem.count++;
                }
            }
        }

        return incomingList;
    }

    function _getIncomingWarning(destinationVillagID) {
        ///<summary>get any incoming attacks. count and earliest arrival</summary>
        ///<param name="destinationVillagID">village ID, OPTIONAL!</param>

        var cmdData = _getIncomingData();
        var cmd;
        var i;
        var incomingWarningInfo = { count: 0, earliestLandingTime: Number.MAX_VALUE };
        var now = new Date().getTime();

        for (i = 0, cmd; cmd = cmdData.commands[i]; ++i) {
            if (cmd.type === ROE.Troops.InOut2.Enum.CommandTypes.attack) {

                // if destinationVillagID specified, then make sure this command is for this village, if not, then just ignore it
                if (!destinationVillagID || cmd.dvid === destinationVillagID) {
                    if (cmd.etime > now) {
                        incomingWarningInfo.count++;
                        incomingWarningInfo.earliestLandingTime = Math.min(incomingWarningInfo.earliestLandingTime, cmd.etime)
                    }
                }
            }
        }

        return incomingWarningInfo;
    }
    function _getOutgoingWarning(originVillagID) {
        ///<summary>get any incoming attacks. count and earliest arrival</summary>
        ///<param name="destinationVillagID">village ID, OPTIONAL!</param>

        var cmdData = _getOutgoingData();
        var cmd;
        var i;
        var outgoingWarningInfo = { count: 0 };
        var now = new Date().getTime();


        for (i = 0, cmd; cmd = cmdData.commands[i]; ++i) {
            // if originVillagID specified, then make sure this command is for this village, if not, then just ignore it
            if (!originVillagID || cmd.ovid === originVillagID) {
                if (cmd.etime > now) {
                    outgoingWarningInfo.count++;
                }
            }
        }

        return outgoingWarningInfo;
    }
    
    var _summary = {}; //{ [villageID] : { village:{name, x, y, id}, numIncoming, earliestLanding }, sortedList}
    function _getSummary(direction) {
        BDA.Val.validate(direction, "unrecognized direction", false, ROE.Troops.InOut2.Enum.Directions.validate, obj.getContextInfo);

        // see if we got it already, otherwise, create it
        var summary = _summary[direction];
        if (summary) {
            return summary;
        } else {
            _summary[direction] = { sortedList: [] };
            summary = _summary[direction];
        }

        var cmdData = _getData(direction);
        var cmd;
        var i;
        var now = new Date().getTime();
        var currentItem; // : ROE.Troops.InOut.OneVillageSummary
        var village;
        var player;
        for (i = 0, cmd; cmd = cmdData.commands[i]; ++i) {
            if (cmd.etime > now) {
                
                currentItem = summary[cmd.dvid];
                if (currentItem === undefined) {
                    village = cmdData.villages[cmd.dvid];
                    player = ROE.isSpecialPlayer(cmd.dpid) ? undefined : cmdData.players[cmd.dpid];
                    
                    currentItem = new ROE.InOutOneVillageSummary();
                    currentItem.village.ownerPID = cmd.dpid;
                    currentItem.village.ownerName = player == undefined ? "": player.name;
                    currentItem.village.name = village.name;
                    currentItem.village.x = village.x;
                    currentItem.village.y =  village.y;
                    currentItem.village.id = cmd.dvid;
                    summary[cmd.dvid] = currentItem;
                    summary.sortedList.push(cmd.dvid);
                }
                if (cmd.type == ROE.Troops.InOut2.Enum.CommandTypes.attack) {
                    if (cmd.hidden) {
                        currentItem.numIncomingAttackHidden++;
                        currentItem.earliestLandingAttackHidden = Math.min(currentItem.earliestLandingAttackHidden, cmd.etime)
                    } else {
                        currentItem.numIncomingAttack++;
                        currentItem.earliestLandingAttack = Math.min(currentItem.earliestLandingAttack, cmd.etime)
                    }
                } else {
                    if (cmd.hidden) {
                        currentItem.numIncomingOtherHidden++;
                        currentItem.earliestLandingOtherHidden = Math.min(currentItem.earliestLandingOtherHidden, cmd.etime)
                    } else {
                        currentItem.numIncomingOther++;
                        currentItem.earliestLandingOther = Math.min(currentItem.earliestLandingOther, cmd.etime)
                    }
                }
            }
        }

        summary.sortedList.sort(function sorter(a, b) {
            return (summary[a].getMostSignificantIncoming().earliestLanding - summary[b].getMostSignificantIncoming().earliestLanding);
        });

        return summary;
    }

    //
    // Public interface
    //
    obj.getData = _getData;
    obj.getIncomingData = _getIncomingData;
    obj.getOutgoingData = _getOutgoingData;
    obj.cancel = cancel;
    obj.details = details;
    obj.getContextInfo = getContextInfo;
    obj.checkIfFrefreshIsNeeded = checkIfFrefreshIsNeeded;
    obj.findCommandByEventIDAnyDir = _findCommandByEventIDAnyDir;
    obj.findCommandByEventID_Rich = _findCommandByEventID_Rich;

    obj.findCommandByEventID = _findCommandByEventID;
    //obj.toggleHiddenStatusOnCommand = _toggleHiddenStatusOnCommand;
    obj.removeCommandByEventID = _removeCommandByEventID;
    obj.toggleHiddenStatusByEventID = _toggleHiddenStatusByEventID;
    obj.getIncomingWarning = _getIncomingWarning;
    obj.getOutgoingWarning = _getOutgoingWarning;
    obj.getIncomingMiniList = _getIncomingMiniList;
    obj.getIncomingMiniSummary = _getIncomingMiniSummary;
    obj.getSummary = _getSummary;

}(window.ROE.Troops.InOut2 = window.ROE.Troops.InOut2 || {}));



ROE.Troops.InOut2.Command = function (eventID, originVillage, originPlayer, destinationVillage, destinationPlayer) {
    ///<summary>constructs a new rich command object </summary>
    ///<param name="eventID">eventID</param>
    ///<param name="originVillage">object of type ROE.Class.Village</param>
    ///<param name="originPlayer">object of type ROE.Class.Player</param>
    ///<param name="destinationVillage">object of type ROE.Class.Village</param>
    ///<param name="destinationPlayer">object of type ROE.Class.Player</param>
    this.eventID = eventID;
    this.originVillage = originVillage;
    this.originPlayer = originPlayer;
    this.destinationVillage = destinationVillage;
    this.destinationPlayer = destinationPlayer;
}
ROE.Troops.InOut2.Command.prototype =
{
    eventID: undefined
    , originVillage: undefined
    , originPlayer: undefined
    , destinationVillage: undefined
    , destinationPlayer: undefined
}
