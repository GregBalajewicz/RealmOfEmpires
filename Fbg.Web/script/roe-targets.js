"use strict";

(function (ROE) {
}(window.ROE = window.ROE || {}));

(function (obj) {


    var CONST = {
        Selector: {
            dialog: '#somedialog'
        }
    }

    var _container;
    var _simpleVillageInfo; //basic info about the target village

    function _open(simpleVillageInfo, specificTarget) {

        //for now we assume all containers will be #targetsDialog
        _container = $('#targetsDialog').dialog('open');
        BDA.Broadcast.subscribe(_container, "MapSelectedVillageChanged", _handleMapSelectedVillageChangedEvent);
        _container.dialog('option', 'title', 'Target Requests');
        _container.empty();

        //solves a missing map info issue, probably from launching with -> _openTargetOnVillageCords function
        //since village info could be not ready in time
        //we frame busy here, and wait for "MapSelectedVillageChanged" to kick in from the "select command"
        if (!simpleVillageInfo) {
            ROE.Frame.busy('Getting Map Info...', 7000, _container);
            ROE.Landmark.checkvills(); //this will quickly check if we have map square info and get missing stuff
            //which then triggers MapSelectedVillageChanged event, which then gets back to _open, here, and continue
            return;
        }

        ROE.Frame.free(_container);

        _simpleVillageInfo = simpleVillageInfo;

        var targetsHeader = $('<div class="targetsHeader"></div>');

        var villageInfo = $('<div class="villageInfo">' +
                    '<div class="icon" style="background-image:url('+BDA.Utils.GetMapVillageIconUrl(_simpleVillageInfo.points, _simpleVillageInfo.type)+')"></div>' +
                    '<div class="v">' + _simpleVillageInfo.name + ' (' + _simpleVillageInfo.x + ',' + _simpleVillageInfo.y + ')</div>' +
                    '<div class="p">' + _simpleVillageInfo.player.PN + '</div>' +
                '</div>');

        if (_simpleVillageInfo.mine) {
            var miniSummary = ROE.Troops.InOut2.getIncomingMiniSummary(_simpleVillageInfo.id);
            if (miniSummary.length) {
                $('.v', villageInfo).append(ROE.Utils.getImcomingMiniIndicatorsHTML(miniSummary, 10, true));
            }
        }
        villageInfo.appendTo(targetsHeader);

        /*
        var clearAll = $('<div class="headerBtn dltAll">Delete All</div>').click(function () {
            console.log('delete all placeholder');
        }).appendTo(targetsHeader);
        */

        var newBtnAtk = $('<div class="BtnDSm2n fontButton1L headerBtn newAtk"><div class="icon"></div>Ask for Attack</div>').click(function () {
            _newTargetExpand(2);
        }).appendTo(targetsHeader);

        var newBtnSup = $('<div class="BtnDSm2n fontButton1L headerBtn newSup"><div class="icon"></div>Ask for Support</div>').click(function () {
            _newTargetExpand(1);
        }).appendTo(targetsHeader);

        _container.append(targetsHeader);

        var targetsList = $('<div class="targetsList"></div>');
        _container.append(targetsList);

        _buildTargetsList();

        if (specificTarget) {
            _expandTarget(specificTarget);
        }

    }

    function _openNew(simpleVillageInfo, targetType) {
        _open(simpleVillageInfo, null);
        _newTargetExpand(targetType);
    }

    function _buildTargetsList() {

        $('.targetsList', _container).empty();

        //build a target box element for every target entry on village
        if (ROE.Player.Targets && _simpleVillageInfo) { //this check is for when targets are being added or edited outside of popup
            var tempT, target;
            for (var i = 0; i < ROE.Player.Targets.length; i++) {
                tempT = ROE.Player.Targets[i];
                if (tempT.TargetVillageID == _simpleVillageInfo.id /*&& tempT.TargetDefinitionOwnerPlayerID == simpleVillageInfo.pid*/) {
                    target = ROE.Player.Targets[i];
                    _buildTarget(target);
                }
            }
        }

        $('.targetsList', _container).append(_buildInfoLinksSection());

        //for now we stick it in here, makes sense
        ROE.Animation.animateDefinedTargets();
    }

    function _buildInfoLinksSection() {
        var anchorTarget = ROE.isD2 ? 'target="blanc"' : '';
        var infoLinksSection = $('<div class="infoLinksSection">Give us your ' +
            '<a href="https://roebeta.uservoice.com/forums/273298-realm-of-empires-suggestions/category/185887-window-target-requests" ' + anchorTarget + ' >comments and suggestions</a>.</div>');
        infoLinksSection.append($('<div>Read the <a href="http://realmofempires.blogspot.ca/2017/05/target-system-release-v1.html" ' + anchorTarget + '>how-to blog</a> for more info.</div>'));
        return infoLinksSection;
    }

    function _buildTarget(target) {

        var typeLabel = 'Act on';
        if (target.Type == 1) {
            typeLabel = 'Support';
        } else if (target.Type == 2) {
            typeLabel = 'Attack';
        }
        var verbage = '<div class="tVerbage">' + typeLabel + ' this village </div>';

        var assignedElement = "";
        if (target.AssingedToPlayerID) {
            assignedElement = '<div class="tAssigned">' + target.AssignedToPlayerName + ' </div>';
            //if assigned to you, wed need a special class
            if (target.AssingedToPlayerID == ROE.Player.id) {
                //targetContent.addClass('assignedToMe');
            }
        }

        //try to get settime, if still in the future, make a countdown element
        var timerElement = "";
        var setTime = ROE.Utils.cDateToJsDate(target.SetTime);
        if (setTime > Date.now()) {
            timerElement = '<div class="tSetTime">by <span class="tTimer countdown" data-finisheson="' + setTime.valueOf() + '" data-refreshcall="" ></span></div>';
        }

        var noteString = "";
        if (target.Note) {
            var noteString = ROE.Utils.toTextarea(target.Note);
            if (noteString.length > 100) {
                noteString = noteString.substring(0, 100) + '...';
            }
            noteString = '<div class="reqNote">Note: ' + noteString + '</div>';
        }

        var targetBox = $('<div class="requestBox fontSilverFrLCmed" data-type="' + target.Type + '" data-id="' + target.DefinedTargetID + '">' +
                 assignedElement + verbage + timerElement + noteString +
                //'<div class="reqNote">' + ROE.Utils.toTextarea(target.Note) + '</div>' +
                //'<div class="reqOwner"  style="background-image:url(' + ROE.Avatar.list[target.TargetDefinitionOwnerAvatarID].imageUrlS + ')">' + target.TargetDefinitionOwnerName + '</div>' +
                //'<div class="reqTime">' + BDA.Utils.formatTimeDifference(ROE.Utils.cDateToJsDate(target.TimeCreated)) + '</div>' +
                '<div class="reqTime" data-time="' + ROE.Utils.cDateToJsDate(target.TimeCreated).valueOf() + '"></div>' +
             + '</div>'
        );

        //the click function launches expands target to view detail
        targetBox.click(function targetBoxClick() {
            _expandTarget(target);
        });

        $('.targetsList', _container).append(targetBox);

        BDA.Utils.formatTimeDifferenceLive($('.reqTime', targetBox));

    }

    function _expandTarget(target) {

        var expandArea = $('<div class="expandArea"  data-id="' + target.DefinedTargetID + '"></div>');
        var expandAreaContent = $('<div class="expandAreaContent" ></div>').appendTo(expandArea);

        var targetTypeText = target.Type == 1 ? 'Calling for SUPPORT to: ' : 'Calling for ATTACK to: ';
        expandAreaContent.append('<div class="callingFor">' + targetTypeText + '</div>');

        var detailsArea = $('<div class="detailsArea">' +
                                '<div class="expandHeader vill" style="background-image:url(' + BDA.Utils.GetMapVillageIconUrl(_simpleVillageInfo.points, _simpleVillageInfo.type) + ')">' +
                                    '<div class="vInfo">' + target.TargetVillageName + ' (' + target.TargetVillageXCord + ',' + target.TargetVillageYCord + ')' + '</div>' +
                                    '<div class="pInfo">' + target.TargetVillageOwnerPlayerName + '</div>' +
                                '</div>' +
                            '</div>');

        //days to keep section: days active before expiration
        detailsArea.append('<div class="expandHeader daysToKeepWrapper">Days to keep target: ' + '<input type="number" class="daysToKeepInput" min="1" max="7"  />' + '</div>');

        //update days to keep based Creation Time and Expire time (which was originally set because of days to keep)
        var dateValueCreate = BDA.Utils.fromMsJsonDate(target.TimeCreated).valueOf();
        var dateValueExpire = BDA.Utils.fromMsJsonDate(target.ExpiresOn).valueOf();
        var dayDiff = Math.ceil(((dateValueExpire + 60000) - dateValueCreate) / (1000 * 60 * 60 * 24)) - 1; //the extra time is to correct an issue right after creation
        $('.daysToKeepInput', detailsArea).val(dayDiff);
        if (target.TargetDefinitionOwnerPlayerID != ROE.playerID) {
            $('.daysToKeepWrapper', detailsArea).hide();
        }

        //assigned to player element
        var assignedPlayerElement = $('<div class="expandHeader assignedPlayerSection">Assigned to: <span class="assignedPlayerName" data-name=""></span></div>').appendTo(detailsArea);
        assignedPlayerElement.click(function () {
            _openTargetAssignPanel(expandAreaContent);
        });
        _updateTargetAssignElement(assignedPlayerElement, target.AssignedToPlayerName);

        //set time section
        var setTimeElement = $('<div class="expandHeader setTimeElement" data-settime="" data-targettype="' + target.Type + '"></div>').appendTo(detailsArea);
        if (target.TargetDefinitionOwnerPlayerID == ROE.playerID) { //editing only for target owner
            setTimeElement.click(function () {
                _openSetTimePanel(expandAreaContent);
            });
        }
        _updateSetTimeElement(setTimeElement, ROE.Utils.cDateToJsDate(target.SetTime).valueOf());

        //extra details: days to keep, create date, expire date, target owner/opener name
        detailsArea.append(
            '<div class="expandHeader dateCreated">Created: <span class="timeDiff" data-time="' + ROE.Utils.cDateToJsDate(target.TimeCreated).valueOf() + '"></span></div>' +
            '<div class="expandHeader dateExpire">Expires: <span class="timeDiff" data-time="' + ROE.Utils.cDateToJsDate(target.ExpiresOn).valueOf() + '"></span></div>' +
            '<div class="expandHeader opener">Opener: ' + target.TargetDefinitionOwnerName + '</div>'
        );

        //add the details panel and tool kit buttons
        expandAreaContent.append(detailsArea);


        //note section
        var textarea = $('<textarea class="textAreaInput" maxlength="200" placeholder="Optional note" />').val(ROE.Utils.toTextarea(target.Note));
        if (target.TargetDefinitionOwnerPlayerID != ROE.playerID) {
            textarea.addClass('disabled');
            textarea.attr("disabled", "disabled");
        }
        expandAreaContent.append(textarea);
        
        //add target auto fill button
        if (target.Type == 1 && target.TargetDefinitionOwnerPlayerID == ROE.playerID) {

            var incData = ROE.Troops.InOut2.getIncomingWarning(parseInt(_simpleVillageInfo.id));
            if (incData.count) {

                var autoFillBtn = $('<div class="BtnDSm2n fontButton1L expandTool autoFill">Auto Fill</div>').click(function targetAutoFill() {

                    var autoNote = _autoFillText(_simpleVillageInfo.id, target.Type);
                    var currentNote = textarea.val();
                    //if auto fill code exists in note, we replace and update only that portion
                    if (currentNote.indexOf('<auto>') > -1 && currentNote.indexOf('</auto>') > -1) {
                        currentNote = currentNote.replace(currentNote.substring(currentNote.indexOf("<auto>"), currentNote.lastIndexOf("</auto>") + ("</auto>".length)), autoNote);
                    } else {
                        currentNote = autoNote + currentNote;
                    }
                    textarea.val(currentNote);
                }).appendTo(expandAreaContent);
            }
        }

        //tool buttons section
        var toolKit = $('<div class="toolKit"></div>');

        //go back button
        var closeBtn = $('<div class="BtnDSm2n fontButton1L expandTool close">Cancel</div>').click(function () {
            expandArea.remove();
        }).appendTo(toolKit);

        if (target.TargetDefinitionOwnerPlayerID == ROE.playerID) {
            
            //save button
            var saveBtn = $('<div class="BtnDSm2n fontButton1L expandTool save">Save</div>').click(function targetEditSave() {
                var setTime = setTimeElement.attr('data-settime') || null;
                var assignTo = $('.assignedPlayerName', assignedPlayerElement).html();
                if (!assignTo || !assignTo.length) { assignTo = null }
                var note = textarea.val();
                var expiresInXDays = parseInt($('.daysToKeepInput', expandAreaContent).val());
                if (expiresInXDays < 1 || expiresInXDays > 7 || isNaN(expiresInXDays)) {
                    expiresInXDays = 7;
                    $('.daysToKeepInput', expandAreaContent).val(expiresInXDays);
                    return;
                }
                ROE.Frame.busy('Saving...', 5000, _container);
                ROE.Api.call_definedtargets_edit(target.DefinedTargetID, setTime, note, expiresInXDays, assignTo, _editTargetCallback);
            }).appendTo(toolKit);

            //delete button
            var deleteBtn = $('<div class="BtnDSm2n fontButton1L expandTool delete">Delete</div>').click(function targetDelete() {
                ROE.Frame.busy('Deleting...', 5000, _container);
                ROE.Api.call_definedtargets_delete(target.DefinedTargetID, _deleteTargetCallback);
                expandArea.remove();
            }).appendTo(toolKit);

        }

        expandAreaContent.append(toolKit);

        //target response section
        var responseArea = $('<div class="responseArea"></div>');
        var response;
        var haveYouResponded = false;

        for (var i = 0; i < target.Responses.length; i++) {
            response = target.Responses[i];

            var isSelfComment = response.PlayerID == ROE.playerID;
            if (!haveYouResponded) {
                haveYouResponded = isSelfComment;
            }
            var responseElement = $('<div class="response" data-typeid="' + response.ResponseTypeID + '" >' +
                '<div class="type" data-typeid="' + response.ResponseTypeID + '"></div>' +
                '<div class="avatar" style="background-image:url(' + ROE.Avatar.list[response.PlayerAvatarID].imageUrlS + ')"></div>' +
                '<div class="msg">' + response.Message + '</div>' +
                '<div class="player">' + response.PlayerName + '</div>' +
            '</div>');
            if (isSelfComment) {
                var dltBtn = $('<div class="dltBtn"></div>').click(function () { _deleteResponse(target); });
                responseElement.addClass('selfComment').append(dltBtn);
            }
            responseArea.append(responseElement);
        }

        //new response area, added only if not your own target and not already responded
        if (!(target.TargetDefinitionOwnerPlayerID == ROE.playerID) && !haveYouResponded) {
            var newRespondArea = $('<div class="newRespondArea">' +
                '<textarea class="responseInputMsg" placeholder="Leave a comment..." />' +
                '<div class="BtnDSm2n fontButton1L expandTool saveResponse" >Comment</div>' +
            '</div>');

            responseArea.prepend(newRespondArea);

            $('.saveResponse', newRespondArea).click(function () {
                _respondAs(target, 2)
            });

        }

        expandAreaContent.append(responseArea);
        expandAreaContent.append(_buildInfoLinksSection());  

        _container.append(expandArea);

        BDA.Utils.formatTimeDifferenceLive($('.expandHeader.dateCreated .timeDiff', _container));
        BDA.Utils.formatTimeDifferenceLive($('.expandHeader.dateExpire .timeDiff', _container));

    }

    //sets up a new target creation form
    function _newTargetExpand(type) {

        var expandArea = $('<div class="expandArea"></div>');
        var expandAreaContent = $('<div class="expandAreaContent" ></div>').appendTo(expandArea);
        //console.log('new target',_simpleVillageInfo);
        var targetTypeText = type == 1 ? 'Calling for SUPPORT to: ' : 'Calling for ATTACK to: ';
        expandAreaContent.append('<div class="callingFor">' +targetTypeText + '</div>');
        expandAreaContent.append('<div class="expandHeader vill" style="background-image:url(' + BDA.Utils.GetMapVillageIconUrl(_simpleVillageInfo.points, _simpleVillageInfo.type) + ')">' +
            '<div class="vInfo">' + _simpleVillageInfo.name + ' (' +_simpleVillageInfo.x + ',' +_simpleVillageInfo.y + ')' + '</div>' +
            '<div class="pInfo">' + _simpleVillageInfo.player.PN + '</div>' +
        '</div>');

        //days to keep section: days active before expiration
        expandAreaContent.append('<div class="expandHeader daysToKeepWrapper">Days to keep target: ' + '<input type="number" class="daysToKeepInput" min="1" max="7"  />' + '</div>');
        $('.daysToKeepInput', expandAreaContent).val(2);

        //assigned to player element
        var assignedPlayerElement = $('<div class="expandHeader assignedPlayerSection">Assigned to: <span class="assignedPlayerName" data-name=""></span></div>').appendTo(expandAreaContent);
        assignedPlayerElement.click(function () {
            _openTargetAssignPanel(expandAreaContent);
        });
        _updateTargetAssignElement(assignedPlayerElement, '');

        //set time section - hidden unless a time is set
        var setTimeElement = $('<div class="expandHeader setTimeElement" data-settime="" data-targettype="' + type + '"></div>').appendTo(expandAreaContent);
        setTimeElement.click(function () { _openSetTimePanel(expandAreaContent); });
        _updateSetTimeElement(setTimeElement, 0);
        //if creating a support target, and have an incomming, we use the first landtime as settime default
        if (type == 1) {
            var incData = ROE.Troops.InOut2.getIncomingWarning(parseInt(_simpleVillageInfo.id));
            if (incData.count) {
                var landTime = incData.earliestLandingTime; //land date time value
                _updateSetTimeElement(setTimeElement, landTime); //populates set time element content and attr
            }
        }

        //note section
        var textarea = $('<textarea class="textAreaInput" maxlength="200" placeholder="Optional note" />');
        expandAreaContent.append(textarea);
        //go ahead and auto fill initially
        //textarea.val(_autoFillText(_simpleVillageInfo.id, type));

        //add auto fill btn
        if (type == 1) {

            var incData = ROE.Troops.InOut2.getIncomingWarning(parseInt(_simpleVillageInfo.id));
            if (incData.count) {

                var autoFillBtn = $('<div class="BtnDSm2n fontButton1L expandTool autoFill">Auto Fill</div>').click(function targetAutoFill() {

                    var autoNote = _autoFillText(_simpleVillageInfo.id, type);
                    var currentNote = textarea.val();
                    //if auto fill code exists in note, we replace and update only that portion
                    if (currentNote.indexOf('<auto>') > -1 && currentNote.indexOf('</auto>') > -1) {
                        currentNote = currentNote.replace(currentNote.substring(currentNote.indexOf("<auto>"), currentNote.lastIndexOf("</auto>") + ("</auto>".length)), autoNote);
                    } else {
                        currentNote = autoNote + currentNote;
                    }
                    textarea.val(currentNote);

                }).appendTo(expandAreaContent);

            }
        }

        var toolKit = $('<div class="toolKit"></div>');

        //back button
        var closeBtn = $('<div class="BtnDSm2n fontButton1L expandTool close">Cancel</div>').click(function () {
            expandArea.remove();
        }).appendTo(toolKit);

        //save button
        var createBtn = $('<div class="BtnDSm2n fontButton1L expandTool save">Create</div>').click(function () {
            var setTime = setTimeElement.attr('data-settime') || null;
            var expiresInXDays = parseInt($('.daysToKeepInput', expandAreaContent).val());
            if (expiresInXDays < 1 || expiresInXDays > 7 || isNaN(expiresInXDays)) {
                expiresInXDays = 7;
                $('.daysToKeepInput', expandAreaContent).val(expiresInXDays);
                return;
            }
            var assignTo = $('.assignedPlayerName', assignedPlayerElement).html();
            if (!assignTo || !assignTo.length) { assignTo = null }

            ROE.Frame.busy('Creating Target...', 5000, _container);
            ROE.Api.call_definedtargets_add(_simpleVillageInfo.id, type, setTime, textarea.val(), expiresInXDays, assignTo, _targetAddCallback);
            expandArea.remove();
        }).appendTo(toolKit);

        expandAreaContent.append(toolKit);
        expandAreaContent.append(_buildInfoLinksSection());

        _container.append(expandArea);

    }


    //opens a mini panel to allow editing of set time
    function _openSetTimePanel(expandAreaContent) {

        //the element storing set time data in the expandArea
        var setTimeElement = $('.setTimeElement', expandAreaContent);

        var miniPanel = $('<div class="miniPanel setTimePanel"></div>');
        var setTimeLabel = $('<div class="setTimeLabel">The time players need to act before:</div>');

        //create a bunch of inputs for months days hours etc
        var setTimeInputContainer = $('<div class="setTimeInputContainer">' +
                'Time: <input class="setTimeTimePicker" tabindex="0"><br/>' +
                'Date: <input class="setTimeDatePicker" type="text" ><br/>' +
                '<div>(Game Time / UTC)</div>'
            + '</div>');

        //if no current date is already set, we use a few hours into a future for example
        var currentDateTime = setTimeElement.attr('data-settime');
        var adjustmentHoursInMilliseconds = 1000 * 60 * 60 * 0; //4 hours
        if (!(currentDateTime && currentDateTime.length)) {
            currentDateTime = Date.now().valueOf() + (adjustmentHoursInMilliseconds);
        }

        //if trying to open in the past, correct it
        if (currentDateTime <= Date.now().valueOf()) {
            currentDateTime = Date.now().valueOf() + (adjustmentHoursInMilliseconds);
        }

        //get UTC components of the current set time
        var landDate = new Date(parseInt(currentDateTime));
        var utcYear = landDate.getUTCFullYear();
        var utcMonth = landDate.getUTCMonth();
        var utcDayOfMonth = landDate.getUTCDate();
        var utcHour = landDate.getUTCHours();
        var utcMinute = landDate.getUTCMinutes();
        var utcSecond = landDate.getUTCSeconds();


        //set current time into the input and make into a timpicker
        //timepicker source: http://timepicker.co/
        $('.setTimeTimePicker', setTimeInputContainer).timepicker({
            //timeFormat: 'hh:mm:ss p',
            timeFormat: 'HH:mm:ss',
            defaultTime: utcHour + ':' + utcMinute + ':' + utcSecond,
            //dynamic: false,
            dropdown: false,
            scrollbar: false,
            change: function (time) {
                //console.log('timepicker change', time);
            }
        });

        // for day month year, we use a jquery date picker
        //add 1 to month because its 0 indexed
        $('.setTimeDatePicker', setTimeInputContainer).val((utcMonth + 1) + '/' + utcDayOfMonth + '/' + utcYear).datepicker({
            dateFormat: 'mm/dd/yy'
        });

        var set = $('<div class="BtnDSm2n fontButton1L miniPanelBtn set">Set</div>').click(function () {

            var timePickerVal = $('.setTimeTimePicker', setTimeInputContainer).val();
            var datePickerVal = $('.setTimeDatePicker', setTimeInputContainer).val();

            //Because firefox is stupid and cant parse date with '-'
            //by changing format in datepicker, we dont need this
            //datePickerVal = datePickerVal.replace(new RegExp('-', 'g'), '/');

            //here a local date is constructed from input material
            var constructedDateTime = Date.parse(datePickerVal + ' ' + timePickerVal);

            //converts local time to UTC
            var timezoneoffsetms = ((new Date().getTimezoneOffset()) * 60000);
            constructedDateTime = constructedDateTime - timezoneoffsetms;

            _updateSetTimeElement(setTimeElement, constructedDateTime);
            miniPanel.remove();

        });

        var clear = $('<div class="BtnDSm2n fontButton1L miniPanelBtn clear">Clear</div>').click(function () {
            _updateSetTimeElement(setTimeElement, 0);
            miniPanel.remove();
        });

        var cancel = $('<div class="BtnDSm2n fontButton1L miniPanelBtn cancel">Cancel</div>').click(function () {
            miniPanel.remove();
        });

        miniPanel.append(setTimeLabel, setTimeInputContainer, set, clear, cancel);
        miniPanel.appendTo(expandAreaContent);
    }

    //given datetime, populates an element with a readable format in UTC
    function _updateSetTimeElement(element, time) {
        time = parseInt(time);
        if (!isNaN(time) && time > 1) {
            var convertedDate = new Date(time);
            var utcMonth = convertedDate.getUTCMonth();
            var utcDayOfMonth = convertedDate.getUTCDate();
            var utcHour = convertedDate.getUTCHours();
            var utcMinute = convertedDate.getUTCMinutes();
            var utcSecond = convertedDate.getUTCSeconds();
            element.html(_typeText(element.attr('data-targettype')) + ' by: ' + BDA.Utils.monthes[convertedDate.getUTCMonth()] + ' ' + utcDayOfMonth + ' ' + utcHour + ':' + utcMinute + ':' + utcSecond + ' (Game Time)');
            element.attr('data-settime', time).show();
        } else {
            element.empty().attr('data-settime', null).html(_typeText(element.attr('data-targettype')) + ' before: [ optional, click to set time ]').show();
        }
    }


    //for assigning the target to a specifc player, or not (then to everone)
    function _openTargetAssignPanel(expandAreaContent, assignedToPlayerName) {
        var assignedPlayerElement = $('.assignedPlayerSection', expandAreaContent);
        var miniPanel = $('<div class="miniPanel assignPanel"></div>');
        var assignLabel1 = $('<div class="assignLabel">Assigning target to a specific player will give them a special indicator. ' +
                            'ONLY this player will see the target, no one else.</div>');
        var assignLabel2 = $('<div class="assignLabel">Player name to assign this target to:</div>');
        var assignInput = $('<input class="assignInput" type="text">');
        var previousInput = $('.assignedPlayerName', assignedPlayerElement).attr("data-name");
        assignInput.val(assignedToPlayerName || previousInput);
        var assignSave = $('<div class="BtnDSm2n fontButton1L miniPanelBtn save">Set</div>').click(function () {
            _updateTargetAssignElement(assignedPlayerElement, assignInput.val());
            miniPanel.remove();
        });
        var assignCancel = $('<div class="BtnDSm2n fontButton1L miniPanelBtn close">Cancel</div>').click(function () {
            miniPanel.remove();
        });
        var suggestionsBox = $('<div class="suggestionsBox"></div>');
        _autocompletePlayer(assignInput, suggestionsBox);
        miniPanel.append(assignLabel1, assignLabel2, assignInput, assignSave, assignCancel, suggestionsBox);
        miniPanel.appendTo(expandAreaContent);
    }

    function _updateTargetAssignElement($element, assignName) {
        if (assignName && assignName.length) {
            $('.assignedPlayerName', $element).attr('data-name',assignName).html(assignName);
        } else {
            $('.assignedPlayerName', $element).attr('data-name','').html('[ everyone, click to change ]');
        }
    }


    function _targetAddCallback(data) {
        ROE.Frame.free(_container);
        ROE.Player.Targets = data.definedTargets;
        _buildTargetsList();
        //var newTarget = _findTargetByID(data.newDefinedTargetID); //this is the newly created target, for use later
    }

    function _deleteTargetCallback(data) {
        ROE.Frame.free(_container);
        ROE.Player.Targets = data;
        _buildTargetsList();
    }

    function _editTargetCallback(data) {
        ROE.Frame.free(_container);
        ROE.Player.Targets = data;
        _buildTargetsList();
        _checkUpdateExpandedTarget();
    }

    //for adding a response to a target
    function _respondAs(target, typeID) {
        var msg = $('.responseInputMsg').val();
        ROE.Frame.busy('Responding...', 5000, _container);
        ROE.Api.call_definedtargetresponse_editdeladd(target.DefinedTargetID, typeID, msg, _respondAsCallback);
    }

    function _respondAsCallback(data) {
        ROE.Frame.free(_container);
        ROE.Player.Targets = data;
        _buildTargetsList();
        _checkUpdateExpandedTarget();
    }

    //deleting a response to a target
    function _deleteResponse(target) {
        ROE.Frame.busy('Deleting response...', 5000, _container);
        ROE.Api.call_definedtargetresponse_editdeladd(target.DefinedTargetID, 1, '', _deleteResponseCallback);
    }

    function _deleteResponseCallback(data) {
        ROE.Frame.free(_container);
        ROE.Player.Targets = data;
        _buildTargetsList();
        _checkUpdateExpandedTarget();
    }

    //if an expanded target is displayed, rebuild it to update it
    function _checkUpdateExpandedTarget() {
        var expandArea = $('.expandArea', _container);
        if (expandArea.length) {
            var expandedTargetID = expandArea.attr('data-id');
            var target = _findTargetByID(expandedTargetID);
            expandArea.remove();
            _expandTarget(target);
        }
    }



    function _handleMapSelectedVillageChangedEvent(villageData) {
        if ($("#targetsDialog").dialog("isOpen") === true) {
            ROE.Frame.free(_container);
            if (!_simpleVillageInfo || (_simpleVillageInfo.id != villageData.villageID)) {
                var simpleVillageInfo = ROE.Landmark.villages_byid[villageData.villageID];
                _open(simpleVillageInfo);
            }
        }
    }

    //used to auto enter generic text for a targets of different types
    function _autoFillText(vid, type) {

        if (type == 1) {
            return _genericSupportText(vid);
        } else if (type == 2) {
            return 'Lets attack this village...';
        } else {
            return 'misc type place holder';
        }
    }

    function _genericSupportText(vid) {
        var incData = ROE.Troops.InOut2.getIncomingWarning(parseInt(vid));
        if (incData.count) {
            var arrivalTime = BDA.Utils.formatEventTimeSimpleUTC(new Date(incData.earliestLandingTime));
            return '<auto>' + incData.count + ' incoming. First landing: ' + arrivalTime + '</auto>';
        } else {
            return '<auto>This village will need support.</auto>';
        }
    }

    //used mainly from incoming summary to auto create targets
    function _autoCreateUpdateSupportTarget(vid, busyElement) {

        //ROE.Frame.busy(' ', 5000, busyElement);
        busyElement.addClass('busy');

        //check to see if a Support Target Exists on village
        var target;
        for (var i = 0; i < ROE.Player.Targets.length; i++) {
            target = ROE.Player.Targets[i];
            if (target.Type == 1 && target.TargetDefinitionOwnerPlayerID == ROE.playerID && target.TargetVillageID == vid) {
                //found the target
                break;
            } else {
                target = null;
            }
        }

        //for now we assume autocreate only makes support targets

        var autoNote = _genericSupportText(vid);
        var timeToSet = null;
        var incData = ROE.Troops.InOut2.getIncomingWarning(parseInt(vid));
        if (incData.count) {
            timeToSet = incData.earliestLandingTime;
        }


        //if a target exists, we edit its content, else we create a new one
        if (target) {

            var currentNote = target.Note;

            //if auto fill code exists in note, we replace and update only that portion
            if (currentNote.indexOf('<auto>') > -1 && currentNote.indexOf('</auto>') > -1) {
                currentNote = currentNote.replace(currentNote.substring(currentNote.indexOf("<auto>"), currentNote.lastIndexOf("</auto>") + ("</auto>".length)), autoNote);
            } else {
                currentNote = autoNote + currentNote;
            }
            
            //if a settime has value of first incoming use it, else use the current value of settime form target
            timeToSet = timeToSet || ROE.Utils.cDateToJsDate(target.SetTime).valueOf();

            //to have set time, will need to convert target.SetTime to raw MS, but what 
            ROE.Api.call_definedtargets_edit(target.DefinedTargetID, timeToSet, currentNote, 2, null, _addEditCB);

        } else {
            ROE.Api.call_definedtargets_add(vid, 1, timeToSet, autoNote, 2, null, _addEditCB);
        }

        function _addEditCB(data) {
            busyElement.removeClass('busy').addClass('done');
            if (data.definedTargets) {
                //data from ADD
                ROE.Player.Targets = data.definedTargets;
                ROE.Frame.infoBar('Added Village SUPPORT target on Map');
            } else {
                //data from EDIT
                ROE.Player.Targets = data;
                ROE.Frame.infoBar('Updated Village SUPPORT target on Map');
            }
            _buildTargetsList();
            _checkUpdateExpandedTarget();
        }



    }

    function _broadCastTargetsToChat(targetsType, chatID) {

    }

    //create a formatted string of all the support targets YOU created
    function _bbCodeTargets(targetsType) {

        var stringArr = [];
        var target;
        for (var i = 0; i < ROE.Player.Targets.length; i++) {
            target = ROE.Player.Targets[i];
            if (target.Type == targetsType && target.TargetDefinitionOwnerPlayerID == ROE.playerID) {
                if (!target.AssingedToPlayerID) {
                    stringArr.push('- (' + target.TargetVillageXCord + ',' + target.TargetVillageYCord + ')' + ' - ' + target.Note);
                }
            }
        }

        return stringArr;
    }


    function _findTargetByID(DefinedTargetID) {
        for (var i = 0; i < ROE.Player.Targets.length; i++) {
            if (ROE.Player.Targets[i].DefinedTargetID == DefinedTargetID) {
                return ROE.Player.Targets[i];
            }
        }
    }

    function _findTargetsByType(Type) {
        var targetsOfType = [];
        if (ROE.Player.Targets) {
            for (var i = 0; i < ROE.Player.Targets.length; i++) {
                if (ROE.Player.Targets[i].Type == Type) {
                    targetsOfType.push(ROE.Player.Targets[i]);
                }
            }
        }
        return targetsOfType;
    }

    //given a village ID returns the first target on that village, priority given to an assigned one
    //format is object  { target: assignedTarget || targetsOnVillage[0], count: targetsOnVillage.length }
    function _returnPriorityTargetFromVillageID(villageID) {

        var assignedTarget = null;
        var targetsOnVillage = [];

        if (ROE.Player.Targets) {

            //go through player targets and add the ones on correct village to a temp array
            for (var i = 0; i < ROE.Player.Targets.length; i++) {
                if (ROE.Player.Targets[i].TargetVillageID == villageID) {
                    targetsOnVillage.push(ROE.Player.Targets[i]);
                }
            }

            //if one or more targets on village, see if there is an assigned one first
            if (targetsOnVillage.length) {
                for (var i = 0; i < targetsOnVillage.length; i++) {
                    if (targetsOnVillage[i].AssingedToPlayerID == ROE.Player.id) {
                        assignedTarget = targetsOnVillage[i];
                        break;
                    }
                }
                //if no assigned one returned, return first target on village
                return { target: assignedTarget || targetsOnVillage[0], count: targetsOnVillage.length };
            }

            //by this points no player targets were on village, return false
            return false;
        }

        //if no player targets return false
        return false;

    }

    //returns a type label 
    function _typeText(type) {
        if (type == 1) {
            return 'Support';
        } else if (type == 2) {
            return 'Attack';
        } else {
            return 'Act';
        }
    }









    //////////LAUNCH TARGETS LIST section
    var _targetsListDialog;

    function _launchTargetsList(targetType) {

        //embed the type into the dialog somehow
        //and only show support1 or attack2
        //also chaneg title based on type

        if (!ROE.Player.Targets) {
            ROE.Frame.infoBar('Targets not loaded yet, please wait.');
            return;
        }

        var typeLabel = '';
        if (targetType == 1) {
            typeLabel = 'Support';
        } else {
            typeLabel = 'Attack';
        }


        ROE.Frame.popDisposable({
            ID: 'targetsListDialog',
            title: 'Targets List: ' + typeLabel,
            content: '',
            width: 400,
            height: 550,
            modal: false,
            contentCustomClass: null
        });

        _targetsListDialog = $('#targetsListDialog');
        _targetsListDialog.attr('data-targettype', targetType);

        var content = $('<div class="targetsListContent"></div>').appendTo(_targetsListDialog);
        var header = $('<div class="targetsListHeader"></div>').appendTo(content);
        var animationsWarn = $('<div class="animationsWarn"></div>').appendTo(content);


        //maybe a refresh button here too
        header.append('<div class="c1 fontSilverFrSCmed">List of active ' + typeLabel + ' targets by Clan and Allies.</div>');

        var optionsContainer = $('<div class="optionsContainer fontSilverFrLCmed"></div>');
        var reload = $('<div class="smallRoundButtonDark reload"><div class="icon"></div></div>')
            .click(function () { _launchTargetsList(targetType); }).appendTo(optionsContainer);

/*
        var info = $('<div class="smallRoundButtonDark infoBtn helpTooltip" data-tooltipid="getMoreInfo"><div class="icon"></div></div>')
            .click(_toggleInfo).appendTo(optionsContainer);
*/

        optionsContainer.append('<div class="c2">Sort by:</div>');

        var sortByDistance = $('<div class="option sortByDistance">Village Distance</div>').click(function () {
            optionClick($(this));
        }).appendTo(optionsContainer);

        var sortByDistance = $('<div class="option sortByPlayer">Player Name</div>').click(function () {
            optionClick($(this));
        }).appendTo(optionsContainer);

        var sortByDistance = $('<div class="option sortByCreateDate active">Create Date</div>').click(function () {
            optionClick($(this));
        }).appendTo(optionsContainer);

/*
        var typeToShow1 = $('<div class="option typeToShow active" data-targettype="1">Show Support</div>').click(function () {
            $('.typeToShow', _targetsListDialog).removeClass('active');
            $(this).addClass('active');
            _optionsChanged();
        }).appendTo(optionsContainer);
        var typeToShow2 = $('<div class="option typeToShow" data-targettype="2">Show Attack</div>').click(function () {
            $('.typeToShow', _targetsListDialog).removeClass('active');
            $(this).addClass('active');
            _optionsChanged();
        }).appendTo(optionsContainer);
*/

        function optionClick($option) {
            if ($option.hasClass('active')) {
                return;
            }
            $('.option', _targetsListDialog).removeClass('active');
            $option.addClass('active');
            _optionsChanged();
        }

        header.append(optionsContainer);

        
        //if not animations, show warning

        if (!ROE.Animation.isAnimationEngineOn() || !ROE.Animation.isAnimationSubsetOn("state_targets")) {
            var warningText = "WARNING: Target Animations on the map are OFF. You will not see them animated on your map. Click here to turn animations on.";
            animationsWarn.html(warningText);
            animationsWarn.click(function () {
                animationsWarn.hide();
                ROE.Animation.toggleTargetState(1);
                ROE.Animation.resume();
            });
            animationsWarn.show();
        } else {
            animationsWarn.hide();
        }


        var targetsListBody = $('<div class="targetsListBody"></div>').appendTo(content);
        var list = $('<div class="list"></div>').appendTo(targetsListBody);



        ROE.Frame.busy('Loading Village List...', 5000, $('#targetsListDialog'));

        ROE.Villages.getVillages(function (villageList) {
            ROE.Frame.free($('#targetsListDialog'));
            _populateListItems(villageList);
            _optionsChanged();
        });

        _optionsChanged();

    }

    //Create Target list items
    function _populateListItems(villageList) {

        var listItemsContainer = $('.list', _targetsListDialog).empty();

        var targetsType = _targetsListDialog.attr('data-targettype');

        //if somehow dialog didnt get a Type embed there wa something wrong.
        if (!targetsType) {
            listItemsContainer.html('Target Type Error.');
            return; //must be an error somewhere
        }

        //create a label based on target type
        var typeLabel = '';
        if (targetsType == 1) {
            typeLabel = 'Support';
        } else if (targetsType == 2) {
            typeLabel = 'Attack';
        } else {
            typeLabel = 'Act on';
        }

        //get a list of a desired target type, from all targets
        var targetsOfType = _findTargetsByType(targetsType);

        //handle empty list.
        if (targetsOfType.length < 1) {
            listItemsContainer.html('No ' + typeLabel + ' Targets found.');
            return;
        }


        //loop through the list of targets of desired type
        targetsOfType.forEach(function (target) {

            var dist, nearestDistanceSquares = 999999;
            var ownVillage = target.TargetVillageOwnerPlayerID == ROE.Player.id;
            //skip this for your own village
            if (!ownVillage) {
                //find distance of your nearest village, to the Target's Village
                for (var ii = 0; ii < villageList.length; ii++) {
                    dist = ROE.Utils.CalculateDistanceBetweenVillages(villageList[ii].x, villageList[ii].y, target.TargetVillageXCord, target.TargetVillageYCord);
                    if (dist < nearestDistanceSquares) {
                        nearestDistanceSquares = dist;
                    }
                    //if already 1 or closer, then stop, that'll do
                    if (nearestDistanceSquares <= 1) {
                        break;
                    }
                }
            } else {
                nearestDistanceSquares = 0;
            }

            var groupClass;
            if (ownVillage) {
                groupClass = 'own';
            } else if (ROE.isSpecialPlayer_Rebel(target.TargetVillageOwnerPlayerID)) {
                groupClass = 'rebel';
            } else if (ROE.isSpecialPlayer_Abandoned(target.TargetVillageOwnerPlayerID)) {
                groupClass = 'abandoned';
            } else if (target.TargetVillageOwnerPlayerClanID == ROE.Player.Clan.id) {
                groupClass = 'clan';
            } else { //it could be ally, but we dont know x.x
                //groupClass = 'ally';
                groupClass = 'black';
            }

            var listItem = $(
                //i is index of ROE.Player.Targets[]
                '<div class="listItem" data-index="' + i + '" data-vx="' + target.TargetVillageXCord + '" ' + '" data-vy="' + target.TargetVillageYCord + '" ' +
                'data-vpn="' + target.TargetVillageOwnerPlayerName + '"' + 'data-ceatedate="' + ROE.Utils.cDateToJsDate(target.TimeCreated).valueOf() + '"' +
                'data-targettype="' + target.Type + '" data-groupclass="' + groupClass + '" data-dist="' + nearestDistanceSquares + '" >' +

                    //Target Type Icon - clicking it will go to village on map, and open Targets Dialog
                    '<div class="targetBit typeIcon type' + target.Type + '"></div>' +

                    //Sheild Flag of group class
                    //'<div class="targetBit groupFlag ' + groupClass + '"></div>' +

                    //Type Label
                    '<div class="targetBit typeLabel">' + typeLabel + '</div>' +

                    //Player Profile - if rebel or abandoned do nothing, if player launch player profile
                    ((groupClass == 'rebel' || groupClass == 'abandoned') ?
                    '<div class="targetBit TargetVillageOwnerPlayer gray" data-pid="' + target.TargetVillageOwnerPlayerID + '" >' + target.TargetVillageOwnerPlayerName + '</div>'
                    :
                    '<div class="targetBit TargetVillageOwnerPlayer" data-pid="' + target.TargetVillageOwnerPlayerID + '">' + target.TargetVillageOwnerPlayerName + '</div>') +

                    //Village Information - click launches village profile
                    '<div class="targetBit TargetVillage" data-vid="' + target.TargetVillageID + '" data-ownerpid="' + target.TargetVillageOwnerPlayerID + '" >'
                    + target.TargetVillageName + ' (' + target.TargetVillageXCord + ',' + target.TargetVillageYCord + ')</div>' +

                    //Distance between Target's village and your nearest village - show nothing if your own village
                    (groupClass != 'own' ? '<div class="distance">Your nearest village: ' + Math.round(nearestDistanceSquares) + 'sq </div>' : '') +

                    //Assigned player and set time area
                    '<div class="TargetAssignArea" >' +
                        '<div class="targetBit assignedPlayer" ></div>' +
                        '<div class="targetBit setTime" ></div>' +
                    '</div>' +

                    //Target Note
                    '<div class="targetBit TargetNote" >Note: ' + target.Note + '</div>' +

                    //created time
                    '<div class="TimeCreated"><span>created </span><span class="timer" data-time="' + ROE.Utils.cDateToJsDate(target.TimeCreated).valueOf() + '"></span></div>' +

                    //comment count
                    '<div class="targetResponses"></div>' +

                '</div>'
            );

            //hide empty notes
            if (target.Note.length < 1) {
                $('.TargetNote', listItem).hide();
            }

            //if target has been assigned to a player
            var assignedPlayerElement = $('.assignedPlayer', listItem);
            if (target.AssignedToPlayerName) {

                assignedPlayerElement.attr('data-name', target.AssignedToPlayerName);
                assignedPlayerElement.click(function () {
                    if (ROE.isMobile) {
                        $('#targetsListDialog').dialog('close');
                    }
                    ROE.Frame.popupPlayerProfile($(this).attr('data-name'));
                });

                //if assigned to you
                if (target.AssingedToPlayerID == ROE.Player.id) {
                    assignedPlayerElement.addClass('you').html('Assigned to you!');
                    listItem.addClass('assignedToYou');
                } else {
                    assignedPlayerElement.html('Assigned to: <span class="name">' + target.AssignedToPlayerName + '</span>');
                }

            } else {
                assignedPlayerElement.hide();
            }

            //target set time
            var setTimeRaw = ROE.Utils.cDateToJsDate(target.SetTime).valueOf();
            if (setTimeRaw && setTimeRaw > (Date.now())) {
                $('.setTime', listItem).html(_typeText(target.Type) + ' by: <span class="counter countdown" data-refreshcall="" data-finisheson="' + setTimeRaw + '"></span>');
            } else {
                $('.setTime', listItem).hide();
            }

            //setup response count
            var targetResponses = $('.targetResponses', listItem);
            if (target.Responses.length) {
                targetResponses.html(target.Responses.length);
            } else {
                targetResponses.hide();
            }

            //setup player name click
            var targetVillageOwnerPlayer = $('.TargetVillageOwnerPlayer', listItem);
            targetVillageOwnerPlayer.click(function (event) {
                event.stopPropagation();
                if (ROE.isMobile) {
                    $('#targetsListDialog').dialog('close');
                }
                ROE.Frame.popupPlayerProfile(target.TargetVillageOwnerPlayerName);
            });

            //setup village name click
            var targetVillage = $('.TargetVillage', listItem);
            targetVillage.click(function (event) {
                event.stopPropagation();
                if (ROE.isMobile) {
                    $('#targetsListDialog').dialog('close');
                }
                ROE.Frame.popupVillageProfile(target.TargetVillageID);
            });

            //setup entire row click
            listItem.click(function () {
                ROE.Targets.openTargetOnVillageCords(target.TargetVillageXCord, target.TargetVillageYCord, target);
            });

            listItemsContainer.append(listItem);

            BDA.Utils.formatTimeDifferenceLive($('.TimeCreated .timer', listItem));

        });

    }

    var _options = {
        sortByDistance: false,
        sortByPlayer: false,
        sortByCreateDate: false,
        typeToShow: 1
    }

    /*
    
        var sortByDistance = $('<div class="option sortByPlayer">Player Name</div>').click(function () {
            optionClick($(this));
        }).appendTo(optionsContainer);

        var sortByDistance = $('<div class="option sortByCreateDate">Create Date</div>').click(function () {
            optionClick($(this));
        }).appendTo(optionsContainer);    
    
    */

    function _optionsChanged() {
        _options.sortByDistance = $('.sortByDistance', _targetsListDialog).hasClass('active');
        _options.sortByPlayer = $('.sortByPlayer', _targetsListDialog).hasClass('active');
        _options.sortByCreateDate = $('.sortByCreateDate', _targetsListDialog).hasClass('active');
        _options.typeToShow = _targetsListDialog.attr('data-targettype');
        //_options.typeToShow = $('.typeToShow.active', _targetsListDialog).first().attr('data-targettype');
        _affectOptions();
    }

    function _affectOptions() {

        var listItems = $('.listItem', _targetsListDialog).detach();

        //show hide based on type
        listItems.hide().each(function () {
            if ($(this).attr('data-targettype') == _options.typeToShow) {
                $(this).show();
            }
        });

        if (_options.sortByDistance) {  //sort by distance to your nearest village

            listItems.sort(function (a, b) {
                var an = parseInt(a.getAttribute('data-dist')),
                    bn = parseInt(b.getAttribute('data-dist'));
                if (an > bn) { return 1; }
                if (an < bn) { return -1; }
                return 0;
            });

        } else if (_options.sortByPlayer) {  //sort by player name

            listItems.sort(function (a, b) {
                var an = a.getAttribute('data-vpn'),
                    bn = b.getAttribute('data-vpn');
                if (an > bn) { return 1; }
                if (an < bn) { return -1; }
                return 0;
            });

        } else if (_options.sortByCreateDate) { //sort by target creation date

            listItems.sort(function (a, b) {
                var an = parseInt(a.getAttribute('data-ceatedate')),
                    bn = parseInt(b.getAttribute('data-ceatedate'));
                if (an < bn) { return 1; } //newest first
                if (an > bn) { return -1; }
                return 0;
            });

        } else { //if somehow no sort, sort by target index

            listItems.sort(function (a, b) {
                var an = a.getAttribute('data-index'),
                    bn = b.getAttribute('data-index');
                if (an > bn) { return 1; }
                if (an < bn) { return -1; }
                return 0;
            });

        }

        $('.list', _targetsListDialog).append(listItems);

    }

    function _toggleInfo() {
        var infoPanel = $('<div class="infoPanel" >' +
                '<div class="legendArea">' +
                    '<div class="label">Legend</div>' +
                    '<div class="groupFlag own">Your village</div>' +
                    '<div class="groupFlag rebel">Rebel village</div>' +
                    '<div class="groupFlag abandoned">Abandoned village</div>' +
                    '<div class="groupFlag clan">Clanmate\'s village</div>' +
                    //'<div class="groupFlag ally"></div>' +
                    '<div class="groupFlag black">Other player\'s village</div>' +
                '</div>' +
                //'<div>more info ... ... ... </div><br>' 
         '</div>');

        infoPanel.click(function () {
            infoPanel.remove();
        });

        infoPanel.appendTo(_targetsListDialog);
    }


    function _openTargetOnVillageCords(vx, vy, specificTarget) {
        ROE.Landmark.gotodb(vx, vy);
        ROE.Landmark.select();
        ROE.Targets.open(ROE.Landmark.v, specificTarget);
    }

    //find the most pressing, assigned target, if one found add settime as counter to the targets list icon.
    function _updateTargetsListLaunchIcon(targetsListLaunchIcon) {

        //if no icon, abort
        if (targetsListLaunchIcon.length < 1) {
            return;
        }

        //empty icon
        targetsListLaunchIcon.empty();

        var targetType = targetsListLaunchIcon.attr('data-type');
        var targetsOfType = _findTargetsByType(targetType);

        //if no targets of correct type, hide icon and abort
        if (targetsOfType.length < 1) {
            targetsListLaunchIcon.hide();
            return;
        }

        var target = null;
        var earliestSetTimeRaw = null;
        var earliestPickedTarget = null;

        var countForYou = 0;
        var countRecent = 0;


        for (var i = 0; i < targetsOfType.length; i++) {
            target = targetsOfType[i];

            //skip your own targets from creating indicators
            if (target.TargetDefinitionOwnerPlayerID == ROE.Player.id) {
                continue;
            }

            var xDaysInMs =  1000 * 60 * 60 * 48; //48hours
            if (ROE.Utils.cDateToJsDate(target.TimeCreated).valueOf() > (Date.now() - xDaysInMs)) {
                countRecent++;
            }

            if (target.AssingedToPlayerID == ROE.Player.id && target.SetTime) {

                countForYou++;

                var setTimeRaw = ROE.Utils.cDateToJsDate(target.SetTime).valueOf();
                //if settime is in future
                if (setTimeRaw && setTimeRaw > (Date.now())) {

                    //if none picked yet pick it as the desired target
                    if (earliestPickedTarget == null) {
                        earliestSetTimeRaw = setTimeRaw;
                        earliestPickedTarget = target;

                        //else pick the earliest "most pressing" set time
                    } else if (setTimeRaw < earliestSetTimeRaw) {
                        earliestSetTimeRaw = setTimeRaw;
                        earliestPickedTarget = target;
                    }
                }
            }
        }

        //show count of assigned to you
        if (countForYou) {
            targetsListLaunchIcon.append('<div class="info countForYou">' + countForYou + '</div>');
        }

        //show count of recent ones (within 48hours)
        if (countRecent) {
            targetsListLaunchIcon.append('<div class="info countRecent">' + countRecent + '</div>');
        }

        //if an assigned target exists
        if (earliestPickedTarget) {
            var setTimeCounter = '<div class="counter countdown" data-refreshcall="" data-finisheson="' + earliestSetTimeRaw + '"></div>';
            targetsListLaunchIcon.append(setTimeCounter);
        }


        targetsListLaunchIcon.show();

    }


    //UTILS
    function _autocompletePlayer(input, suggestionsContainer) {
        function split(val) {
            return val.split(/,\s*/);
        }
        function extractLast(term) {
            return split(term).pop();
        }

        input.keyup(function () {
            var term = extractLast(input.val());
            if (term.length < 3) {
                suggestionsContainer.empty();
                return false;
            }
            ROE.Chat2.chat.server.playerAutocomplete(term, ROE.realmID).done(nameResponse);
        })
        .blur(function () {
            //setTimeout(function () { suggestionsContainer.empty() }, 100);
        });

        function nameResponse(data) {
            suggestionsContainer.empty();
            $.each(data, function (i, n) {
                suggestionsContainer.append(
                    $('<div class="suggestion" data-name="' + n.label + '">' + n.label + '</div>').click(function () {
                        input.val($(this).attr('data-name'));
                        suggestionsContainer.empty();
                    })
                );
            });
        }
    }




    //Public functions
    obj.open = _open;
    obj.openNew = _openNew;
    obj.openTargetOnVillageCords = _openTargetOnVillageCords;
    obj.findTargetByID = _findTargetByID;
    obj.bbCodeTargets = _bbCodeTargets;
    obj.autoCreateUpdateSupportTarget = _autoCreateUpdateSupportTarget;
    obj.launchTargetsList = _launchTargetsList;
    obj.returnPriorityTargetFromVillageID = _returnPriorityTargetFromVillageID;
    obj.updateTargetsListLaunchIcon = _updateTargetsListLaunchIcon;

}(window.ROE.Targets = window.ROE.Targets || {}));





