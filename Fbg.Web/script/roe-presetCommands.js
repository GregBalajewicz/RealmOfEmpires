"use strict";

(function (ROE) {
}(window.ROE = window.ROE || {}));

(function (obj) {


    /*

    A preset structure:
    {
        customText: "4kCMhere" //player given label
        index: : 0 //sort order
        troops: [ { sendCount: 4000, targetBuildingTypeID: 1, utID: 11 } ] //standard command send troops
        type: "csv" //send from currently selected village, or nearest it can find
        commandType: 1,    //_attackType; //Support: 0, Attack: 1
    }

    */

    var _container = null;

    function _openEditor() {

        _container = $('#presetsDialog').dialog('open').empty();
        var header = $('<div class="presetsHeader fontSilverFrLCmed"></div>').appendTo(_container);
        var presetsEdit = $('<div class="presetsEdit fontSilverFrLCmed"></div>').hide().appendTo(_container);
        var presetsContainer = $('<div class="presetsContainer"></div>').appendTo(_container);

        _populateOpeningHeader();
        _populatePresetsContainer();

    }

    function _populateOpeningHeader() {
        var header = $('.presetsHeader', _container)
            .html('These are your Combat Presets. You can edit or delete them here. Drag them to order the way they show up in your Map UI. Only the first 8 are displayed in Map UI. To add a preset, go to Attack a village, but click save before sending.');

        var anchorTarget = ROE.isD2 ? 'target="blanc"' : '';
        var alpha = $('<div class="tempAlpha">We are looking for your ' +
            '<a href="https://roebeta.uservoice.com/forums/273298-realm-of-empires-suggestions/category/193423-window-combat-presets" ' + anchorTarget + ' >comments and suggestions</a>' +
            ' as we continue developing this feature.</div>');
        alpha.append($('<div>There is also a <a href="http://realmofempires.blogspot.ca/2017/01/combat-preset-attack-presets.html" ' + anchorTarget + '>HOW-TO Blog</a> for more info.</div>'));
        header.append(alpha);

    }

    //for every preset make a box
    function _populatePresetsContainer() {
        
        if (!_container.dialog('isOpen')) { return; }

        var presetsContainer = $('.presetsContainer', _container).empty();

        var presets = _getPresets();
        for (var i = 0; i < presets.length; i++) {
            var preset = presets[i];
            preset.index = i; //index will be used as ID, and orderring
            presetsContainer.append(_buildPresetBox(preset));
        }

        presetsContainer.sortable({
            items: ".presetBox",
            cursor: "move",
            //after sort stop, and dom changed
            update: function (event, ui) {
                _savePresetBoxChanges();
            }
        });

    }

    function _buildPresetBox(preset) {

        //if no command type, default to attack
        if (preset.commandType === undefined) {
            preset.commandType = 1;
        }

        var presetBox = $('<div class="presetBox" data-index="' + preset.index + '" data-commandtype="' + preset.commandType + '">' +
            '<div class="customText">' + preset.customText + '</div>' +
        '</div>').data({ preset: preset });

        var deleteBtn = $('<div class="delete"></div>').click(function (e) {
            e.stopPropagation();
            var thisPresetBox = $(this).parent();
            thisPresetBox.remove();
            var presetsEdit = $('.presetsEdit', _container).empty().hide();
            _savePresetBoxChanges();
        });

        presetBox.append(deleteBtn);

        var troopsContainer = $('<div class="troopContainer"></div>');
        for (var t = 0; t < preset.troops.length; t++) {

            var troopCount = preset.troops[t].sendCount;
            var troopID = preset.troops[t].utID;
            var troopEntity = ROE.Entities.UnitTypes[troopID];
            var troopBox = $('<div class="troopBox" data-troopID="' + troopID + '" style="background-image:url(' + troopEntity.IconUrl_ThemeM + ')">' + troopCount + '</div>');
            troopsContainer.append(troopBox);
        }

        presetBox.append(troopsContainer);

        presetBox.click(function () {
            _editPreset($(this));
        });

        return presetBox;

    }

    //given a preset Box, use its .data() and open it for editing
    function _editPreset(presetBoxElement) {

        _container.find('.selectedForEdit').removeClass('selectedForEdit');

        presetBoxElement.addClass('selectedForEdit');

        var presetBoxElementData = presetBoxElement.data('preset');

        var presetsEdit = $('.presetsEdit', _container).empty().show();
        presetsEdit.append('<div class="headerText">Edit Preset</div>');
        var presetLabel = $('<input type="text" class="presetLabel" maxlength="12" placeholder="Preset Label" />').appendTo(presetsEdit);

        if (presetBoxElementData.customText && presetBoxElementData.customText.length) {
            presetLabel.val(presetBoxElementData.customText);
        }
        var saveBtn = $('<div class="save BtnBSm2 fontSilverFrSClrg">Save</div>').click(function () {
            presetBoxElementData.customText = presetLabel.val();
            presetBoxElement.data('preset', presetBoxElementData);
            presetBoxElement.removeClass('dontSave');
            _savePresetBoxChanges();
        }).appendTo(presetsEdit);


        //Command Type: attack / support
        var commandTypeContainer = $('<div class="commandTypeContainer"></div>');
        var commandTypeAttack = $('<div class="commandTypeChangebox" data-commandtype="1">Attack</div>').click(function () {
            _commandTypeChange($(this).attr('data-commandtype'));
        });
        var commandTypeSupport = $('<div class="commandTypeChangebox" data-commandtype="0">Support</div>').click(function () {
            _commandTypeChange($(this).attr('data-commandtype'));
        });
        commandTypeContainer.append(commandTypeAttack, commandTypeSupport);
        presetsEdit.append(commandTypeContainer);
        function _commandTypeChange(commandType) {
            $('.commandTypeChangebox', _container).removeClass('selected');
            var typeChangeElement = $('.commandTypeChangebox[data-commandtype="' + commandType + '"]', _container).addClass('selected');
            presetBoxElementData.commandType = commandType;
            presetBoxElement.data('preset', presetBoxElementData);
        }
        _commandTypeChange(presetBoxElementData.commandType);


        //Preset Type
        var typeContainer = $('<div class="typeContainer"></div>');
        var typeChangeboxCSV = $('<div class="typeChangebox" data-type="csv">Send from Current Selected Village</div>').click(function () {
            _typeChange($(this).attr('data-type'));
        });
        var typeChangeboxNearest = $('<div class="typeChangebox" data-type="nearest">Send from Nearest Village that has the troops</div>').click(function () {
            _typeChange($(this).attr('data-type'));
        });
        typeContainer.append(typeChangeboxCSV, typeChangeboxNearest);
        presetsEdit.append(typeContainer);
        function _typeChange(type) {
            $('.typeChangebox', _container).removeClass('selected');
            var typeChangeElement =$('.typeChangebox[data-type="'+type+'"]', _container).addClass('selected');
            presetBoxElementData.type = type;
            presetBoxElement.data('preset', presetBoxElementData);
        }
        _typeChange(presetBoxElementData.type);


        var troopsContainer = $('<div class="troopContainer"></div>');
        for (var t = 0; t < presetBoxElementData.troops.length; t++) {
            var troopCount = presetBoxElementData.troops[t].sendCount;
            var troopID = presetBoxElementData.troops[t].utID;
            var troopEntity = ROE.Entities.UnitTypes[troopID];
            var troopBox = $('<div class="troopBox" data-troopID="' + troopID + '" style="background-image:url(' + troopEntity.IconUrl_ThemeM + ')">' + troopCount + '</div>');
            troopsContainer.append(troopBox);
        }

        presetsEdit.append(troopsContainer);
        presetLabel.focus();

    }

    //for when creating a preset from an external module
    function _externalNewPreset(newPreset) {

        //check to make sure there are some units being saved
        if (newPreset.troops.length < 1) {
            return;
        }

        //check to make sure there are some units being saved
        var hasTroops = false;
        for (var i = 0; i < newPreset.troops.length; i++) {
            if (newPreset.troops[i].sendCount) {
                hasTroops = true;
                break;
            }
        }
        if (!hasTroops) { return; }


        _openEditor();
        var newPresetBox = _buildPresetBox(newPreset);
        var presetsContainer = $('.presetsContainer', _container).prepend(newPresetBox);
        _editPreset(newPresetBox);
        newPresetBox.hide(); //if new, hide it for better UI reasons
        newPresetBox.addClass('dontSave'); // dont save this along with other changes, untill explicitly saved with save btn
    }

    //saves presets based on presetBox element data
    function _savePresetBoxChanges() {
        var newPresets = [];
        $('.presetBox', _container).each(function () {
            var thisPresetBox = $(this);
            if (!thisPresetBox.hasClass('dontSave')) {
                newPresets.push(thisPresetBox.data('preset'));
            }
        });
        _savePresets(newPresets);
    }

    //add a given preset to current presets, and save changes
    function _saveNewPreset(newPreset) {
        var presets = _getPresets();
        newPreset.customText = newPreset.customText || 'New Preset';
        //add new preset to begining of presets array
        presets.unshift(newPreset);
        _savePresets(presets);
    }



    //save to server storage
    function _savePresets(presetArray) {
        ROE.Frame.busy('Saving Presets', 5000, _container);
        //presetArray[0].commandType = undefined;
        ROE.LocalServerStorage.set('PresetCommands', presetArray, function savePresetsCallback(data) {
            //hacky way to force village map GUI to update despite no village selection change
            //not the best way, in command troops its causing a refresh
            ROE.Landmark.point_changed[0](ROE.Landmark.v.x, ROE.Landmark.v.y);
            _populatePresetsContainer();
            ROE.Frame.infoBar('Preset Changes saved.');
            ROE.Frame.free(_container);
        });
    }

    //gets your presets from localserver storage
    function _getPresets() {
        var storedPresets = ROE.LocalServerStorage.get('PresetCommands');
        if (storedPresets) {

            /*
            for (var i = 0; i < storedPresets.length; i++) {
                storedPresets[i].commandType = undefined;
            }
            */

            return storedPresets;
        } else {
            return [];
        }
    }

    //used in map gui to fill a container with buttons from presets
    function _populateMapGuiPresetButtons(mapGuiPresetPanel, mapTargetV) {
        
        var presets = _getPresets();

        var innerPanel = $('<div class="presetsInnerPanel"></div>');

        var addMore = $('<div class="presetBtn addMore">+</div>').appendTo(innerPanel);
        addMore.click(function () {
            _addMoreClicked($(this));
        });

        var preset;
        for (var i = 0; i < presets.length; i++) {
            if (i >= 8) { break; }//only display max 8 presets

            preset = presets[i];
            var presetBtn = $('<div class="presetBtn action" data-index="' + i + '" data-commandtype="' + preset.commandType + '">' + preset.customText + '</div>').appendTo(innerPanel);
            presetBtn.click(function () {
                _spawnCommand($(this).attr('data-index'), mapTargetV);
            });
        }

        innerPanel.appendTo(mapGuiPresetPanel);

    }

    function _addMoreClicked(btn) {
        _openEditor();
    }

    var _activeCommands = {}; //object that holds spawned commands
    var _commandIndex = 0; //counter helper for spawn IDs

    //this is meant to create a Command object responsible for calling and keeping track of issued preset instances
    function _spawnCommand(presetIndex, targetVillage) {

        var presets = _getPresets();
        var preset = presets[presetIndex];

        //setup an action text based on command type, if no command type default to attacking
        var actionLabel;

        preset.commandType = parseInt(preset.commandType);
        if (preset.commandType === 1) {
            actionLabel = "Attacking ";
        } else if (preset.commandType === 0) {
            actionLabel = "Supporting ";
        }else{
            preset.commandType = 1;
            actionLabel = "Attacking ";
        }

        var command = {
            id: _commandIndex++,
            preset: preset,
            element: $('<div class="presetCommand"></div>'),

            targetVillage: targetVillage,
            originVillage: { id: ROE.CSV.id, x: ROE.CSV.x, y: ROE.CSV.y }, //by default we set origin village to be csv, will populate more in .activate

            awaitingAnimationObject: null, //will hold the hourglass animation when activated

            //once a command object is created, we call this function of it to fire it off
            activate: function () {

                var cmdBodyContainer = $('#activeCommandsContainer');
                if (cmdBodyContainer.length < 1) {
                    cmdBodyContainer = $('<div id="activeCommandsContainer"></div>').appendTo('body');
                }

                command.element.click(function () { $(this).addClass('min'); });

                if (command.preset.type == 'csv') { //when sending with currently-selecte-village mode

                    command.element.html(actionLabel + command.targetVillage.name + '(' + command.targetVillage.x + ',' + command.targetVillage.y + ')<br/>Getting ' + actionLabel + 'village data...');

                    ROE.Villages.getVillage(command.originVillage.id, function (fullVillageData) {

                        //check to see if village has the troops required in command trooplist
                        if (command.checkVillageHasEnoughTroops(fullVillageData)) {

                            command.originVillage = fullVillageData; //store the new information about origin village
                            command.element.html(actionLabel + command.targetVillage.name + '(' + command.targetVillage.x + ',' + command.targetVillage.y + ')<br/>' +
                                'From ' + command.originVillage.name + '(' + command.originVillage.x + ',' + command.originVillage.y + ')');

                            //add a cmd waiting animaton
                            command.awaitingAnimationObject = ROE.Animation.awaitingTroopCommand({
                                ovid: command.originVillage.id,
                                ovx: command.originVillage.x,
                                ovy: command.originVillage.y,
                                dvid: command.targetVillage.id,
                                dvx: command.targetVillage.x,
                                dvy: command.targetVillage.y,
                                cmdtype: 1
                            });

                            //command api call
                            ROE.Api.call_cmd_execute(command.originVillage.id, command.targetVillage.id, command.preset.commandType, $.toJSON(command.preset.troops), command.executeCB);

                        } else {
                            command.genericFailFadeOut(
                                actionLabel + command.targetVillage.name + '(' + command.targetVillage.x + ',' + command.targetVillage.y + ')<br/>' +
                                'Failed: Preset troops not found in selected village.');
                        }

                    });

                } else if (command.preset.type == 'nearest') {//when sending with from nearest-village-with-troops mode

                    command.element.html('<div class="cmdLine l1">' + actionLabel + command.targetVillage.name + '(' + command.targetVillage.x + ',' + command.targetVillage.y + ') from nearest village.</div>' +
                        '<div class="cmdLine l2"></div><div class="cmdLine l3"></div>');

                    /*
                    
                    1. get village list
                    2. go through village by village
                    3. for each village get deep info
                    4. check the info vs the commands troopList
                    5. if matches, launch attack, if not, next village
                    6. if launched, go to callback, if non found, fade out

                    */

                    ROE.Villages.getVillages(function (villageList) {
                        var sortedVIllageList = _sortVillageList(villageList, command.targetVillage);

                        _searchNextVillage(0); //start recursively going through villages sorted by distance

                        function _searchNextVillage(index) {

                            //ran through all villages and couldn't launch
                            if (index >= sortedVIllageList.length) {

                                command.genericFailFadeOut(
                                    actionLabel + command.targetVillage.name + '(' + command.targetVillage.x + ',' + command.targetVillage.y + ')<br/>' +
                                    'Failed: Preset troops not found in any village.');

                                return false;
                            }

                            //get next village in list
                            var listedVillage = sortedVIllageList[index].village;

                            command.element.find('.l2').html('Searching: ' + listedVillage.name + '(' + listedVillage.x + ',' + listedVillage.y + ') ...');
                            index++;

                            //get deep village info
                            ROE.Villages.getVillage(listedVillage.id, function (villdata) {

                                //check to see if village has the troops required in command trooplist
                                if (command.checkVillageHasEnoughTroops(villdata)) {

                                    command.element.find('.l2').html('Found: ' + listedVillage.name + '(' + listedVillage.x + ',' + listedVillage.y + ')');
                                    command.element.find('.l3').html('Launching!');

                                    //set attack origin
                                    command.originVillage = villdata;

                                    //add a cmd waiting animaton
                                    command.awaitingAnimationObject = ROE.Animation.awaitingTroopCommand({
                                        ovid: command.originVillage.id,
                                        ovx: command.originVillage.x,
                                        ovy: command.originVillage.y,
                                        dvid: command.targetVillage.id,
                                        dvx: command.targetVillage.x,
                                        dvy: command.targetVillage.y,
                                        cmdtype: 1
                                    });

                                    //command api call
                                    ROE.Api.call_cmd_execute(villdata.id, command.targetVillage.id, command.preset.commandType, $.toJSON(command.preset.troops), command.executeCB);

                                } else {
                                    _searchNextVillage(index);
                                }

                            }, ROE.Villages.Enum.ExtendedDataRetrieveOptions.ensureExists, false);

                        }

                    });

                } else { //if not csv or nearest, then ?
                    return;
                }

                command.element.addClass('busy').appendTo(cmdBodyContainer);

            },

            //after activate calls the cmd execute api call, this is the callback
            executeCB: function (cbdata) {

                var fadeTimer = 5000;

                if (cbdata.canCommand === 0) { //success

                    //take the sending troops out of the village (temporary)
                    command.deductTroopsFromVillage();

                    ROE.Player.refresh(); //triggers reload of incoming/outgoing immediatelly 

                    command.element.removeClass('busy').addClass('success')
                    .html(actionLabel + targetVillage.name + '(' + targetVillage.x + ',' + targetVillage.y + ')<br/>' +
                    'From ' + command.originVillage.name + '(' + command.originVillage.x + ',' + command.originVillage.y + ')<br/>' +
                    'Sent.');

                    //Morale section
                    if (preset.commandType === 1 && ROE.PlayerMoraleSettings.isActiveOnThisRealm) {

                        if (ROE.Player.morale != cbdata.playerMoraleAfterCmd) {
                            ROE.Player.morale = cbdata.playerMoraleAfterCmd;
                            BDA.Broadcast.publish("PlayerMoraleChanged");
                            command.element.append('<div class="morale"> -> ' + ROE.Player.morale + '</div>');

                            //in case of low morale, give warning
                            if (ROE.Player.morale < ROE.PlayerMoraleSettings.minMorale_Normal) {
                                command.element.append('<div class="lowMorale">LOW MORALE WARNING</div>');
                            }

                        }

                    }




                } else { //some sort of fail

                    fadeTimer = 6000;

                    command.element.removeClass('busy').addClass('failed')
                        .html(actionLabel + targetVillage.name + '(' + targetVillage.x + ',' + targetVillage.y + ')<br/>' +
                        'From ' + command.originVillage.name + '(' + command.originVillage.x + ',' + command.originVillage.y + ')<br/>' +
                        'Failed: ' + _erroCodeText(cbdata.canCommand));

                    //this is a referrance from animation js, if still active, kill it too.
                    if (command.awaitingAnimationObject) {
                        command.awaitingAnimationObject.entity.remove();
                    }

                }

                //fade out after x time
                setTimeout(function () {
                    command.element.addClass('startFade');
                }, fadeTimer);

                //cleanup after x time
                setTimeout(function () {
                    command.element.remove();
                    _activeCommands[command.id] = null;
                }, fadeTimer + 1000);

            },


            genericFailFadeOut: function genericFailFadeOut(elementMsgHtml) {

                command.element.html(elementMsgHtml);

                command.element.removeClass('busy').addClass('failed');

                //fade out after x time
                setTimeout(function () {
                    command.element.addClass('startFade');
                }, 7000);

                //cleanup after x time
                setTimeout(function () {
                    command.element.remove();
                    _activeCommands[command.id] = null;
                }, 8000);

            },

            checkVillageHasEnoughTroops: function checkVillageHasEnoughTroops(villageData) {

                //check every troop in command.preset vs the troops in village
                //if one is insufficient, command will fail, else all is well
                for (var i = 0; i < command.preset.troops.length; i++) {
                    var presetTroop = command.preset.troops[i];
                    for (var ii = 0; ii < villageData.TroopList.length; ii++) {
                        var villageTroop = villageData.TroopList[ii];
                        if (presetTroop.utID == villageTroop.id) {
                            if (presetTroop.sendCount > villageTroop.YourUnitsCurrentlyInVillageCount) {
                                return false;
                            }
                            break;
                        }
                    }
                }
                return true;
            },

            //idea is to remove the trooplist from the village data, we assume
            deductTroopsFromVillage: function deductTroopsFromVillage() {

                for (var i = 0; i < command.preset.troops.length; i++) {
                    var presetTroop = command.preset.troops[i];
                    for (var ii = 0; ii < command.originVillage.TroopList.length; ii++) {
                        if (presetTroop.utID == command.originVillage.TroopList[ii].id) {
                            command.originVillage.TroopList[ii].YourUnitsCurrentlyInVillageCount -= presetTroop.sendCount;
                            break;
                        }
                    }
                }

            }

        }

        _activeCommands[command.id] = command;

        command.activate();
    }


    function _sortVillageList(villageList, targetVillage) {
        var i;
        var sortedVillageList = new Array();
        var fromVillageObj;
        for (i = 0; i < villageList.length; i++) {
            fromVillageObj = { village: villageList[i], distanceToTarget: ROE.Utils.CalculateDistanceBetweenVillages(villageList[i].x, villageList[i].y, targetVillage.x, targetVillage.y) };
            if (fromVillageObj.distanceToTarget == 0) { continue; } // this means that this village is the same as target village; 
            sortedVillageList.push(fromVillageObj);
        }

        sortedVillageList.sort(function sorter(a, b) {
            return (a.distanceToTarget - b.distanceToTarget);
        });

        return sortedVillageList;
    }


    function _erroCodeText(errorCode) {
        errorCode = errorCode.toString();
        var outputString = '';
        switch(errorCode){
            case '1':
                outputString = 'Not enough troops.';
                break;
            case '1a':
                outputString = 'Not enough troops.';
                break;
            case '2':
                outputString = 'Beginner Protection';
                break;
            case '3':
                outputString = 'Target Under SleepMode';
                break;
            case '4':
                outputString = 'Cannot Support Rebels';
                break;
            case '5':
                outputString = 'Cannot Attack Rebels More Than 22sq Away';
                break;
            case '6':
                outputString = 'Attack Freeze';
                break;
            case '7':
                outputString = 'Unknown Error';
                break;
            case '8':
                outputString = 'Target Village Same As Origin';
                break;
            case '9':
                outputString = 'All Troops Desert';
                break;
            case '13':
                outputString = 'Target is in Vacation Mode.';
                break;
            case '14':
                outputString = 'Target is in Weekend Mode.';
                break;
            case '15':
                outputString = 'Subscription not enabled';
                break;
            default:
                outputString = 'Attack failed.';
                break;
        }

        return outputString;

    }


    obj.getPresets = _getPresets;
    obj.externalNewPreset = _externalNewPreset;
    obj.openEditor = _openEditor;
    obj.populateMapGuiPresetButtons = _populateMapGuiPresetButtons;
    
}(window.ROE.PresetCommands = window.ROE.PresetCommands || {}));




