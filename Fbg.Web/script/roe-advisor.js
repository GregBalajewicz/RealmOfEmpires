


(function (obj) {


    var _started = false;
    var _advisorMiniPanelArea; //area where bubbles are attached
    var _advisorLaunchIcon; //advisor launch icon 
    var _advisorMiniPanelIsDocked = false; //shows or hides the big advisor mode
    var _missions = {}; //holds all the mission objects
    var _questMissions = {}; //object holds are quest type missions
    var _activeMissionCount = 0; //total active missions and quest missions, basically used for launch icon handling
    var _missionStatus = {}; //mission and step completion status

    function _init() {

        /**/
        if (ROE.Player.numberOfVillages > 1 || ROE.realmID < 130 || ROE.rt != "NOOB") {
            $('#advisorMiniPanelArea').remove();
            $('#advisorLaunchIcon').remove();
            return;
        }
       

        if (_started) {
            return;
        }

        _started = true;

        //mission and step completion status
        _missionStatus = _getMissions();

        /* for testing
        _missionStatus = {
            //"introAttack": { 'counter-cm':1, 'counter-boost':1,'counter-speed':1,'counter-locate':1 }
        }
        */

        _addMissions();

        _advisorMiniPanelArea = $('#advisorMiniPanelArea');
        //_advisorLaunchIcon = $('#advisorLaunchIcon').click(function () { ROE.UI.Sounds.click(); ROE.Advisor.launchMain() });
        _advisorLaunchIcon = $('#advisorLaunchIcon').click(function () {
            ROE.UI.Sounds.click();
            _dockAdvisorMiniPanel(false);
        });


        //_tick();
        window.setInterval(function () {
            _tick();
        }, 1000);

    }

    //start looking at steps, look for changes, close steps, open steps etc
    //for every mission, it calls its active steps tick method

    //go through all active missions missions, call ticks, add minipanels, etc
    function _tick() {

        _activeMissionCount = 0;

        var missionObject, currentStepObject;
        var oneMissionAlreadyprocessed = false;
        for (var m in _missions) {

            missionObject = _missions[m];

            if (!missionObject.completed) {

                _activeMissionCount++;

                //only need to do this once
                if (!oneMissionAlreadyprocessed) {
                    oneMissionAlreadyprocessed = true;
                    currentStepObject = missionObject.getStepByStepID(missionObject.currentStepID);
                    missionObject.addMiniPanel(currentStepObject);
                    if (currentStepObject && !currentStepObject.completed) {
                        if (currentStepObject.onTick) {
                            currentStepObject.onTick();
                        }
                    }
                }


            } 

        }

        //updates quest missions based on ROE.Player.nextRecommendeQuests
        _processQuests(oneMissionAlreadyprocessed);
    

        if (_advisorMiniPanelIsDocked) {

            _advisorLaunchIcon.removeClass('hide');

        } else {

            if ($('#advisorMain').dialog('isOpen') === true) {
                _advisorMiniPanelArea.addClass('hide');
                _advisorLaunchIcon.addClass('hide');
            } else {
                if (_advisorMiniPanelArea.find('.shown').length < 1) {
                    _advisorMiniPanelArea.addClass('hide');
                    _advisorLaunchIcon.removeClass('hide');
                } else {
                    _advisorMiniPanelArea.removeClass('hide');
                    _advisorLaunchIcon.addClass('hide');
                }
            }

        }



        _updateAdvisorIcon();

    }


    function _processQuests(otherMissionShown) {


        //loop through and add all recommended quests as objects
        var quest;
        for (var i = 0; i < ROE.Player.nextRecommendeQuests.length; i++) {
            quest = ROE.Player.nextRecommendeQuests[i];
            if (!_questMissions[quest.ID]) {
                if (_questValidToAdd(quest)) {
                    _questMissions[quest.ID] = _createQuestMission(quest);
                } else {
                    _questMissions[quest.ID] = { validAdvisorQuest: false };
                }
            }
        }

        //then go through all current add quest missions and pick a valid one to serve
        //what makes it valid:
        //in the recommended array
        //valid advisor quest
        //not dismissed
        //valid to serve now
        var questMissionObject = null;
        var oneQuestDisplayed = false;
        for (var key in _questMissions) {

            questMissionObject = _questMissions[key];

            if (!_missionExistsInRecommendedArray(questMissionObject.id)) {

                //if quest was in array before and now is not, it got done, do a lil animation
                if (questMissionObject.validAdvisorQuest) {
                    questMissionObject.flyPanel();
                }

                questMissionObject = null;
                delete _questMissions[key];
                continue;
            }

            if (!questMissionObject.validAdvisorQuest) {
                continue;
            }

            if (questMissionObject.dismissed) {
                continue;
            }

            if (!otherMissionShown && !oneQuestDisplayed && questMissionObject.validateNow()) {
                oneQuestDisplayed = true;
                questMissionObject.showMiniPanel();
                _activeMissionCount += 1;
            } else {
                questMissionObject.hideMiniPanel();
            }

        }

        function _missionExistsInRecommendedArray(missionQuestID) {

            for (var i = 0; i < ROE.Player.nextRecommendeQuests.length; i++) {
                if (missionQuestID == ROE.Player.nextRecommendeQuests[i].ID) {
                    return true;
                }
            }
            return false;
        }




    }



    function _updateAdvisorIcon() {

        if (_activeMissionCount) {
            _advisorLaunchIcon.html('<div>' + _activeMissionCount + '</div>').removeClass('zero');
        } else {
            _advisorLaunchIcon.addClass('zero');
        }

    }

    //launches the main popup
    function _launchMain(missionIDToOpen, stepIDToOpen) {

        var content = $('<div class="advisorContent">' +
                '<div class="header fontSilverFrLClrg">Mission Log</div>' +
                '<div class="list"></div>'
            +'</div>');

        ROE.Frame.popDisposable({
            ID: 'advisorMain',
            title: 'Advisor',
            content: content,
            width: 550,
            height: 450,
            modal: false,
            contentCustomClass: 'advisorMainDialog'
        });

        _listMissions();

        if (missionIDToOpen) {
            _openMission(_missions[missionIDToOpen], stepIDToOpen)
        }

    }

    //for every mission in _missions create an element in the main panel list
    function _listMissions() {

        var dialog = $('#advisorMain');

        if (!dialog.dialog('isOpen')) {
            return;
        }

        var listContainer = dialog.find('.advisorContent .list').empty();

        var mission;
        for (var m in _missions) {

            mission = _missions[m];
            missionBox = $('<div class="missionBox fontSilverFrLCmed" data-missionid="' + mission.id + '">' +
                    
                    '<div class="img" style="background-image:url(' + mission.image + ')"></div>' +
                    '<div class="label">' + mission.label + '</div>'+

                '</div>');

            missionBox.click(function () {
                var missionID = $(this).attr('data-missionid');
                var missionObject = _missions[missionID];
                _missionClick(missionObject);
            });

            if (mission.completed) {
                missionBox.addClass('completed');
                missionBox.append('<div class="completeElement"></div>');
            }

            listContainer.append(missionBox);

        }
    }

    function _missionClick(missionObject) {

        var dialog = $('#advisorMain');
        if (!(dialog.hasClass("ui-dialog-content")) && dialog.dialog('isOpen') === false) {
            _launchMain();
        }

        _openMission(missionObject);

    }

    //open a main mission, centered on a step, and show prgoress
    function _openMission(missionObject, stepIDToOpen) {

        //console.log('mission', missionObject);

        var dialog = $('#advisorMain');

        dialog.dialog('option', 'title', missionObject.label);

        var expandArea = $('<div class="expandArea"  data-missionid="' + missionObject.id + '" ></div>');
        var expandAreaContent = $('<div class="expandAreaContent fontSilverFrLCmed" >' +   
                '<div class="panel detailsArea"></div>' +  //area with the current mission step details
                '<div class="panel progressArea">PROGRESS AREA</div>' +         //area with general mission prgoress
            '</div>').appendTo(expandArea);

        //close mission button
        var closeBtn = $('<div class="close"></div>').click(function () {
            expandArea.remove();
            dialog.dialog('option', 'title', 'Advisor');
        }).appendTo(expandAreaContent);


        expandArea.appendTo(dialog);


        //setup progressarea
        _buildProgressArea(missionObject);

        //select a step (given step, or current mission step, or if completed all, last step), load the mission step details
        _selectStep(stepIDToOpen || missionObject.currentStepID || missionObject.steps[missionObject.steps.length -1]);

    }


    function _selectStep(stepID) {

        var dialog = $('#advisorMain');
        var expandArea = dialog.find('.expandArea');
        if (expandArea.length < 1) { return; }

        var missionID = expandArea.attr('data-missionid');
        var missionObject = _missions[missionID];
        var stepCount = missionObject.stepCount();

        var stepIndex = missionObject.getStepIndexByStepID(stepID);

        //do nothing if already on this step
        if (expandArea.attr('data-showingstepid') == stepID) {
            return;
        }
        //setup step bounds
        if (stepIndex < 0 || stepIndex >= stepCount) {
            return;
        }

        //recrod current viewing step
        expandArea.attr('data-showingstepid', stepID);

        _loadMissionStep(stepID);

        _updateProgressArea(missionObject);

    }


    function _loadMissionStep(stepID) {

        var dialog = $('#advisorMain');
        var expandArea = dialog.find('.expandArea');
        if (expandArea.length < 1) { return; }

        var missionID = expandArea.attr('data-missionid');
        var missionObject = _missions[missionID];

        var stepIndex = missionObject.getStepIndexByStepID(stepID);
        var missionStepObject = missionObject.steps[stepIndex];

        //area with teh current mission step details
        var detailsArea = expandArea.find('.detailsArea').empty();
        detailsArea.html(
            //'<div class="detail image" style="background-image:url(' + missionStepObject.image + ')"></div>' +
            //'<div class="detail header">' + missionObject.label + '</div>' +

            //'<div class="detail header2">Step ' + (loadMissionStepIndex + 1) + ': ' + missionStepObject.label + '</div>' +
            '<div class="detail header2">' + missionStepObject.label + '</div>' +
            //'<div class="detail desc">' + missionStepObject.desc + '</div>' +
            '<div class="detail smallRoundButtonLight nav prev"><div class="arrow"></div></div>' +
            '<div class="detail smallRoundButtonLight nav next"><div class="arrow"></div></div>'
        );

        expandArea.css('background-image', 'url(' + missionStepObject.image + ')');

        var navBtnPrev = detailsArea.find('.nav.prev').click(function () {
            missionObject.goPrevStep(expandArea.attr('data-showingstepid'));
        });

        var navBtnNext = detailsArea.find('.nav.next').click(function () {
            missionObject.goNextStep(expandArea.attr('data-showingstepid'));
        });

        //hide or show prev step button
        var navBtnPrev = expandArea.find('.nav.prev').show();
        if (stepIndex < 1) {
            navBtnPrev.hide();
        }

        //hide or show next step button
        var navBtnNext = expandArea.find('.nav.next').show();
        if (stepIndex >= missionObject.stepCount() - 1) {
            var navBtnNext = expandArea.find('.nav.next').hide();
        }

        missionStepObject.onView(detailsArea);
        
    }



    //setup progress area based on mission steps
    function _buildProgressArea(missionObject) {

        var dialog = $('#advisorMain');
        var expandArea = dialog.find('.expandArea');
        if (expandArea.length < 1) { return; }


        var progressArea = expandArea.find('.progressArea').empty();

        //progressArea.append($("<svg id='advisorProgressSVG' ></svg>"));

        var stepsContainer = $('<div class="stepsContainer"></div>');

        //visualize the steps
        $.each(missionObject.steps, function (index, stepObject) {

            var stepProgressElement = $('<div class="step" data-stepid="' + stepObject.id + '">' +
                    '<div class="label">' + (index+1) + '</div>' +
                '</div>');

            if (index < missionObject.stepCount() - 1) {
                stepProgressElement.append('<div class="pointer"></div>');
            }
            
            stepProgressElement.click(function () {
                if (stepProgressElement.hasClass('current') || stepProgressElement.hasClass('completed')) {
                    _selectStep(stepObject.id);
                }
            });

            stepsContainer.append(stepProgressElement);
        });

        progressArea.append(stepsContainer);
        
    }

    function _updateProgressArea(missionObject) {

        var dialog = $('#advisorMain');
        var expandArea = dialog.find('.expandArea');
        if (expandArea.length < 1) { return; }

        var currentViewStepID = expandArea.attr('data-showingstepid');
        var $stepElements = expandArea.find('.progressArea .step');

        //var svg = $('#advisorProgressSVG');

        $stepElements.each(function (index, stepProgressElement) {

            //make element jquery, remove all progress classes
            stepProgressElement = $(stepProgressElement).removeClass('completed current selected');
            stepProgressElement.find('.completeElement').remove();
            stepProgressElement.find('.currentElement').remove();

            //get the step ID the element represents
            var stepProgressElementStepID = stepProgressElement.attr('data-stepid');

            //the step has been completed
            if (missionObject.getStepByStepID(stepProgressElementStepID).completed) {
                stepProgressElement.addClass('completed');
                stepProgressElement.append('<div class="completeElement"></div>');
            }

            //the current step player is on, not complete yet
            if (missionObject.currentStepID == stepProgressElementStepID) {
                stepProgressElement.addClass('current');
                stepProgressElement.append('<div class="currentElement"></div>');
            }

            //step that is being viewed
            if (currentViewStepID == stepProgressElementStepID) {
                stepProgressElement.addClass('selected');
            }

        });

    }


    function _addMissions() {

        //INTRO ATTACK MISSION
        _addMission({



            //general info about the mission
            id: "introAttack",
            label: "Rebel Counter-attack",
            desc: "We are gonna get these rebels back for what they did.",
            image: "https://static.realmofempires.com/images/npc/M_BG_Rebel.jpg",


            //holds all the steps of the missio, the steps are objects
            steps: [




                //STEP - deploy troops
                {

                    id: 'counter-cm',
                    label: "Lets Counter-Attack",
                    image: "https://static.realmofempires.com/images/npc/M_BG_Rebel.jpg", //used for the mission step splash screen,
                    desc: "Lets hit those rebels back!",

                    //when step is being displayed in advisorPanel
                    onViewCustom: function (container) {


                        var thisStep = this;
                        if (thisStep.completed) {

                            //common way to generate an onComplete element, but we could add custom html as well
                            var extra = thisStep.commonCompleteElement('50 Citizen Militia deployed! Well done!');

                        } else {

                            var extra = $('<div class="onviewArea">' +
                                '<div class="head1">Recruit militia</div>' +
                                '<div class="item" style="background-image: url(https://static.realmofempires.com/images/units/Militia_M.png);">' +
                                    '<div class="text fontGoldFrLCmed ">50 Citizen Militia</div>' +
                                    '<div class="c">x1</div>' +
                                '</div>' +
                                '<div>Click to view your items</div>' +
                            +'</div>');

                            extra.find('.item').click(function () {
                                ROE.Items2.showPopup(null, 'troops');
                            });


                        }

                        extra.hide().fadeIn(1000);
                        container.append(extra);
                    },

                    //when on current step, the tick gets called on interval
                    onTick: function () {

                        var thisStep = this;

                        if (!thisStep.data.callingForItems) {

                            /* HACK WAY TO GET THE REWARDS WE NEED FOR THIS MISSION */
                            //if one of the key items doesnt exist, we make sure we ping the quest for it.
                            if (!(ROE.Items2.existsItemByGroupID('troops50 Citizen Militia')) ||
                                !(ROE.Items2.existsItemByGroupID('pfd2h Blood Lust Spell')) ||
                                !(ROE.Items2.existsItemByGroupID('pfd24h Rebel Rush Spell'))) {

                                thisStep.data.callingForItems = true;

                                ROE.Frame.busy("Preparing mission...", 10000, $('#advisorMain'));
                                ROE.Api.call_quest_getrewardforcompleted('B_SM_lvl1', function (data) {
                                    ROE.Api.call_items2_myitemgroups('1', _callback_items2_myitemgroups);
                                    function _callback_items2_myitemgroups(data2) {
                                        ROE.Player.itemGroups = data2;
                                        ROE.Frame.free($('#advisorMain'));
                                    }

                                });

                            }

                        }



                        ROE.Villages.getVillage(ROE.SVID, tickGetVillageCB/*, ROE.Villages.Enum.ExtendedDataRetrieveOptions.ensureExistsAndTriggerGetLatest*/);

                        function tickGetVillageCB(village) {

                            //check if 50CM in village
                            $(village.TroopList).each(function (i, t) {

                                if (t.id == 11 && t.YourUnitsCurrentlyInVillageCount >= 50) {
                                    thisStep.complete();
                                    $('#items2Dialog').dialog('close');
                                    _spawnFlyElement({ src: "https://static.realmofempires.com/images/units/Militia_M.png", w: 176, h: 176, destinationElement: $('#vovFrame .troopRow .troop').first() });
                                }

                            });


                        }


                        thisStep.data.ticker = thisStep.data.ticker || 1;
                        thisStep.data.ticker++;
                        if (!thisStep.data.firstflash || thisStep.data.ticker % 4 == 0) {

                            //attention grab the item in items popup
                            var dialog = $('#items2Dialog');
                            if (dialog.dialog('isOpen')) {
                                var itemCatElement = $('.categoryItem[data-type="troops"]');
                                if (itemCatElement.length) {
                                    if (itemCatElement.hasClass('selected')) {
                                        var itemPopupElement = dialog.find('.item.troops[data-groupid="troops50 Citizen Militia"]');
                                        if (itemPopupElement.length) {
                                            ROE.Utils.attentionGrabber(itemPopupElement, 1);
                                            thisStep.data.firstflash = true; //flash once soon as its found
                                        }
                                    } else {
                                        ROE.Utils.attentionGrabber(itemCatElement, 1);
                                    }
                                }
                            }


                        }



                        

                    },



                },

                //STEP - boost troops
                {

                    id: 'counter-boost',
                    label: "Lets boost our troops.",
                    image: "https://static.realmofempires.com/images/npc/M_BG_Rebel.jpg", //used for the mission step splash screen,
                    desc: "Infuse our troops with Blood Lust!",

                    //when step is being displayed in advisorPanel
                    onViewCustom: function (container) {

                        var thisStep = this;
                        if (thisStep.completed) {

                            //common way to generate an onComplete element, but we could add custom html as well
                            var extra = thisStep.commonCompleteElement('Your troops gain more attack power!');

                        } else {

                            var extra = $('<div class="onviewArea">' +
                                '<div class="head1">Boost your Troops</div>' +
                                '<div class="item" style="background-image: url(https://static.realmofempires.com/images/icons/M_PF_Attack.png);">' +
                                    '<div class="text fontGoldFrLCmed ">Blood Lust</div>' +
                                '</div>' +
                                '<div>Click to view your items. Cast Blood Lust.</div>' +
                            +'</div>');

                            extra.find('.item').click(function () {
                                ROE.Items2.showPopup(null, 'pfd');
                            });


                        }

                        extra.hide().fadeIn(1000);
                        container.append(extra);
                    },

                    //when on current step, the tick gets called on interval
                    onTick: function () {
                        //return;
                        var thisStep = this;

                        //if blood lust active
                        if (ROE.Player.PFPckgs.isActive(24)) {
                            thisStep.complete();
                            $('#items2Dialog').dialog('close');
                            _spawnFlyElement({ src: "https://static.realmofempires.com/images/icons/M_PF_Attack.png", w: 176, h: 176, destinationElement: $('.ui-panel-main .pfStatus [data-pfpackageid="24"]').first() });
                            return;
                        }

                        thisStep.data.ticker = thisStep.data.ticker || 1;
                        thisStep.data.ticker++;
                        if (!thisStep.data.firstflash || thisStep.data.ticker % 4 == 0) {

                            var dialog = $('#items2Dialog');
                            if (dialog.dialog('isOpen')) {
                                var itemCatElement = $('.categoryItem[data-type="pfd"]');
                                if (itemCatElement.length) {
                                    if (itemCatElement.hasClass('selected')) {
                                        var itemPopupElement = dialog.find('.item.pfd[data-groupid="pfd2h Blood Lust Spell"]');
                                        if (itemPopupElement.length) {
                                            ROE.Utils.attentionGrabber(itemPopupElement, 1);
                                            thisStep.data.firstflash = true; //flash once soon as its found
                                        }
                                    } else {
                                        ROE.Utils.attentionGrabber(itemCatElement, 1);
                                    }
                                }
                            }
                        }



                    },



                },

                //STEP: speed troops
                {

                    id: 'counter-speed',
                    label: "Speed up your troops!",
                    image: "https://static.realmofempires.com/images/npc/M_BG_Rebel.jpg", //used for the mission step splash screen,
                    desc: "Rally your troops for a speedy attack.",

                    //when step is being displayed in advisorPanel
                    onViewCustom: function (container) {

                        var thisStep = this;
                        if (thisStep.completed) {

                            //common way to generate an onComplete element, but we could add custom html as well
                            var extra = thisStep.commonCompleteElement('Your troops gain more faster move speed!');

                        } else {

                            var extra = $('<div class="onviewArea">' +
                                '<div class="head1">Speed up your troops!</div>' +
                                '<div class="item" style="background-image: url(https://static.realmofempires.com/images/icons/M_PF_RebelRush.png);">' +
                                    '<div class="text fontGoldFrLCmed ">Rebel Rush</div>' +
                                '</div>' +
                                '<div>Click to view your items. Cast Rebel Rush</div>' +
                            +'</div>');

                            extra.find('.item').click(function () {
                                ROE.Items2.showPopup(null, 'pfd');
                            });


                        }

                        extra.hide().fadeIn(1000);
                        container.append(extra);
                    },

                    //when on current step, the tick gets called on interval
                    onTick: function () {
                        //return;
                        var thisStep = this;

                        //if blood lust active
                        if (ROE.Player.PFPckgs.isActive(32)) {
                            thisStep.complete();
                            $('#items2Dialog').dialog('close');
                            _spawnFlyElement({ src: "https://static.realmofempires.com/images/icons/M_PF_RebelRush.png", w: 176, h: 176, destinationElement: $('.ui-panel-main .pfStatus [data-pfpackageid="32"]').first() });
                            return;
                        }


                        thisStep.data.ticker = thisStep.data.ticker || 1;
                        thisStep.data.ticker++;
                        if (!thisStep.data.firstflash || thisStep.data.ticker % 4 == 0) {

                            //attention grab the item in items popup
                            var dialog = $('#items2Dialog');
                            if (dialog.dialog('isOpen')) {
                                var itemCatElement = $('.categoryItem[data-type="pfd"]');
                                if (itemCatElement.length) {
                                    if (itemCatElement.hasClass('selected')) {
                                        var itemPopupElement = dialog.find('.item.pfd[data-groupid="pfd24h Rebel Rush Spell"]');
                                        if (itemPopupElement.length) {
                                            ROE.Utils.attentionGrabber(itemPopupElement, 1);
                                            thisStep.data.firstflash = true; //flash once soon as its found
                                        }
                                    } else {
                                        ROE.Utils.attentionGrabber(itemCatElement, 1);
                                    }
                                }
                            }

                        }



                    },



                },



                //STEP: locate rebel vill
                {

                    id: 'counter-locate',
                    label: "Locate their village",
                    image: "https://static.realmofempires.com/images/npc/M_BG_Rebel.jpg", //used for the mission step splash screen,
                    desc: "We must find where the rebels struck us from.",

                    //when step is being displayed in advisorPanel
                    onViewCustom: function (container) {

                        var thisStep = this;
                        if (thisStep.completed) {

                            var extra = thisStep.commonCompleteElement('Well done! We found them!');

                        } else {

                            //var villIcon = BDA.Utils.GetMapVillageIconUrl(16, 0);

                            var extra = $('<div class="onviewArea">' +
                                '<div class="head1">Lets find where they hit us from</div>' +
                                '<div class="item rebel">' +
                                    '<div class="rebelSkull"></div>' +
                                    '<div class="text fontGoldFrLCmed ">Rebel Village</div>' +
                                '</div>' +
                                '<div>Rebel villages have the skull icon on the map.</div>' +
                            +'</div>');

                            extra.find('.item').click(function () {

                                var dialog = $('#advisorMain');
                                var closestRebelVillage = _nearestRebelToCSV();
                                if (closestRebelVillage) {

                                    var DialogParent = dialog.parent();
                                    DialogParent.animate({ left: "0px", top: "0px" }, 500, function () {
                                        //saves the last position to dialog widget, as css moving alone doesn't

                                        dialog.dialog('option', 'position', [$(this).offset().left, $(this).offset().top]);
                                        ROE.Landmark.gotodb(closestRebelVillage.x, closestRebelVillage.y);
                                        ROE.Landmark.select();
                                        ROE.Utils.attentionGrabber($('#select'), 3);

                                        setTimeout(function () {
                                            thisStep.complete();
                                        }, 1000);

                                    });

                                } else {
                                    console.log('um.. somehow there aint no rebel vill found!');
                                }

                            });

                        }

                        extra.hide().fadeIn(1000);
                        container.append(extra);

                    },


                },

                //STEP: send the attack
                {

                    id: 'counter-strike',
                    label: "Time to strike!",
                    image: "https://static.realmofempires.com/images/npc/M_BG_Rebel.jpg",
                    desc: "Lets teach them a lesson they wont forget.",

                    //when step is being displayed in advisorPanel
                    onViewCustom: function (container) {

                        var thisStep = this;
                        if (thisStep.completed) {

                            var extra = thisStep.commonCompleteElement('Your troops are marching towards the enemy!<br/>We shall get a report of the battle soon.');

                        } else {

                            var extra = $('<div class="onviewArea">' +
                                '<div class="head1">Send the attack!</div>' +
                                '<div>Click the village and open the attack panel.<br/>The icon is a sword:</div>' +
                                '<div class="item" style="background-image: url(https://static.realmofempires.com/images/icons/Q_Attack.png);">' +
                                    '<div class="text fontGoldFrLCmed ">Attack Icon</div>' +
                                '</div>' +
                                '<div>Now, Send your 50 Citizen Militia at them!</div>' +
                            +'</div>');

                            extra.find('.item').click(function () {

                                //find closest rebel vill, navigate to it, ping it, and open attacks panel

                                var closestRebelVillage = _nearestRebelToCSV();
                                if (closestRebelVillage) {

                                    var dialog = $('#advisorMain');
                                    var DialogParent = dialog.parent();
                                    //  animate dialog down to not cover vill
                                    DialogParent.animate({ left: "0px", top: "60%" }, 500, function () {

                                        //saves the last position to dialog widget, as css moving alone doesn't
                                        dialog.dialog('option', 'position', [$(this).offset().left, $(this).offset().top]);

                                        //go to the rebel again, just to make sure
                                        ROE.Landmark.gotodb(closestRebelVillage.x, closestRebelVillage.y);
                                        ROE.Landmark.select();
                                        ROE.Utils.attentionGrabber($('#select'), 1);

                                        //open attacks panel
                                        setTimeout(function () {
                                            ROE.Frame.popupCommandTroops(1, closestRebelVillage.id, closestRebelVillage.x, closestRebelVillage.y, ROE.CSV.id);
                                        }, 1000);

                                    });

                                }

                            });


                        }

                        extra.hide().fadeIn(1000);
                        container.append(extra);

                    },

                    //when on current step, the tick gets called on interval
                    onTick: function () {
                        //return;
                        var thisStep = this;

                        var closestRebelVillage = _nearestRebelToCSV();
                        if (closestRebelVillage) {
                            var outgoingToVillages = ROE.Troops.InOut2.getOutgoingData().villages;
                            var toProperVillage = outgoingToVillages[closestRebelVillage.id];
                            if (toProperVillage) {
                                $('#CommandTroopsPopup').dialog('close');
                                thisStep.complete();
                            }
                        }

                        thisStep.data.ticker = thisStep.data.ticker || 1;
                        thisStep.data.ticker++;
                        if (!thisStep.data.firstflash || thisStep.data.ticker % 4 == 0) {

                            if ($('#CommandTroopsPopup').dialog('isOpen') && $('#AttackUnit_11 .attackMax').length) {
                                ROE.Utils.attentionGrabber($('#AttackUnit_11 .attackMax'), 1);
                                thisStep.data.firstflash = true;
                            }

                        }

                    },


                },




                //STEP: battle report
                {

                    id: 'counter-report',
                    label: "Outcome",
                    image: "https://static.realmofempires.com/images/npc/M_BG_Rebel.jpg", //used for the mission step splash screen,
                    desc: "When your troops arrive, open your Reports and find the battle that took place.",


                    //when step is being displayed in advisorPanel
                    onViewCustom: function (container) {

                        var thisStep = this;
                        if (thisStep.completed) {

                            var extra = thisStep.commonCompleteElement('WE HAVE VICTORY!<br/>According to the report you crushed the rebels and took our silver back!');

                        } else {

                            var extra = $('<div class="onviewArea">' +
                                '<div class="head1">Get the Battle report</div>' +
                                '<div class="item" style="background-image: url(https://static.realmofempires.com/images/icons/m_Reports.png);">' +
                                    '<div class="text fontGoldFrLCmed ">Reports</div>' +
                                '</div>' +
                                '<div>Wait for your troops to arrive, then open the battle report</div>' +
                            +'</div>');

                            extra.find('.item').click(function () {
                                //ROE.Frame.popupReports();

                                var dialog = $('#advisorMain');
                                var DialogParent = dialog.parent();
                                //  animate dialog down to not cover vill
                                DialogParent.animate({ left: "0px", top: "20%" }, 500, function () {
                                    ROE.Utils.attentionGrabber($('#linkReports'), 1);
                                });

                            });


                        }

                        extra.hide().fadeIn(1000);
                        container.append(extra);

                    },

                    //when on current step, the tick gets called on interval
                    onTick: function () {

                        var thisStep = this;

                        /* instead of getting actual report data, why dont we just do a dumb scan for a 'victoryBanner' element
                        if (!this.data.tickerCount) {
                            this.data.tickerCount = 1;
                        } else {
                            this.data.tickerCount++;
                        }
    
                        if (this.data.tickerCount > 10) {
                            this.data.tickerCount = 1;
                            ROE.Api.call('report', {}, function (r) {
                                console.log(r)
                            });
                        }
                        */


                        if ($('#reportDetails_section .victoryBanner').length) {
                            thisStep.complete();
                        }

                    }

                }




            ],

        });

        /*
            //TEST MISSION
            _addMission({
        
        
                //general info about the mission
                id: "testMission",
                label: "This is some filler test mission",
                desc: "desc...desc... desc...desc... desc...desc... desc...desc... desc...desc... ",
                image: "https://static.realmofempires.com/images/backgrounds/M_BG_Tavern.jpg",
        
        
                //holds all the steps of the missio, the steps are objects
                steps: [
        
                ],
        
            });
        
            //TEST MISSION
            _addMission({
        
        
                //general info about the mission
                id: "testMission2",
                label: "some other pretty cool mission",
                desc: "desc...desc... desc...desc... desc...desc... desc...desc... desc...desc... ",
                image: "https://static.realmofempires.com/images/backgrounds/M_BG_PremiumFeatures.jpg",
        
        
                //holds all the steps of the missio, the steps are objects
                steps: [
        
                ],
        
            });
        
            */




    }

    //based on given data, add some comon functions and output the mission object
    function _addMission(missionObject) {

        //init a data object so we can add custom data later
        missionObject.data = {};

        //mission progress status
        if (!_missionStatus[missionObject.id]) {
            _missionStatus[missionObject.id] = {};
        }

        //loop the missions steps and do stuff
        //ADDING things here adds it to steps of all missions by dafault
        $(missionObject.steps).each(function (index, missionStep) {

            //init a data object so we can add custom data later
            missionStep.data = {};

            //add a missionID referrence to each step so we could access their parent if we wanted to
            missionStep.missionID = missionObject.id;

            //mission step completion initiated from storage
            missionStep.completed = _missionStatus[missionObject.id][missionStep.id];

            //completes the step
            missionStep.complete = function () {

                //abort of already completed
                if (missionStep.completed) {
                    return;
                }

                missionStep.completed = true;

                _missionStatus[missionObject.id][missionStep.id] = true;
                _saveProgress(missionStep.onComplete);
                

            }

            //after step is completed
            missionStep.onComplete = function () {
                missionObject.removeMiniPanel();
                missionObject.reIndexSteps();
                _updateProgressArea(missionObject);
                missionStep.onView(); //updates the onview to display completed state
            }



            //BASE ON VIEW - comon events that happen when mission is viewed in advisor panel
            missionStep.onView = function (container) {

                //if no container given, find the default one
                if (!container) {
                    var dialog = $('#advisorMain');
                    var expandArea = dialog.find('.expandArea');
                    if (expandArea.length) {
                        container = expandArea.find('.detailsArea');
                    }
                }
                //if still no container, abort
                if (!container) {
                    return;
                }

                container.find('.onviewArea').remove();

                if (this.onViewCustom) {
                    this.onViewCustom(container);
                }


            }

            missionStep.commonCompleteElement = function (text) {

                var onViewArea = $('<div class="onviewArea">' +
                                        '<div>' + text + '</div>' +
                                        '<div class="stepComplete">Step Complete</div>' +
                                    '</div>');

                //only next button added for non-last steps
                var stepIndex = missionObject.getStepIndexByStepID(missionStep.id);
                if (stepIndex < missionObject.stepCount() -1) {
                    var nextBtn = $('<div class="stepCompleteBtn BtnBSm2 fontSilverFrSClrg">NEXT</div>').appendTo(onViewArea);
                    nextBtn.click(function () {
                        missionObject.goNextStep(missionStep.id);
                    });
                }

                return onViewArea;

            }

        });



        missionObject.completed = false;

        //go through all missions and find the first doable step
        //if all steps are completed, then mission is completed
        missionObject.reIndexSteps = function () {

            for (var i = 0; i < missionObject.steps.length; i++) {
                if (!missionObject.steps[i].completed) {
                    missionObject.currentStepID = missionObject.steps[i].id;
                    return;
                }
            }

            //if we get here, all steps were complete, and mission is complete
            missionObject.onComplete();

        }

        //mission complete event, when all steps are complete
        missionObject.onComplete = function () {
            missionObject.completed = true;
            missionObject.removeMiniPanel();
            _listMissions();
        }


        missionObject.miniPanelElement = null;

        missionObject.removeMiniPanel = function () {
            if (missionObject.miniPanelElement) {
                missionObject.miniPanelElement.remove();
                missionObject.miniPanelElement = null;
            }      
        }

        //given a mission stepObject makes an advisor mini panel for the mission step
        missionObject.addMiniPanel = function (stepObject) {

            missionObject.removeMiniPanel();

            if (stepObject.dismissed === true) {
                return;
            }

            var advisorMiniPanel = $('<div class="advisorMiniPanel shown" data-missionid="' + missionObject.id + '" data-stepid="' + stepObject.id + '">' +
                    '<div class="label">' + stepObject.label + '</div>' +
                    '<div class="desc">' + stepObject.desc + '</div>' +
                    '<div class="view BtnBSm2 fontButton1L">Continue</div>' +
                +'</div>');

            //advisorMiniPanel.css('background-image', 'url(' + stepObject.image + ')');

            var dismissBtn = $('<div class="dismiss"></div>').click(function () {
                //stepObject.dismissed = true;
                //missionObject.removeMiniPanel();
                _dockAdvisorMiniPanel(true);
                setTimeout(function () {
                    if (!(_advisorLaunchIcon.hasClass('hide') || _advisorLaunchIcon.hasClass('zero'))) {
                        ROE.Utils.attentionGrabber(_advisorLaunchIcon, 1);
                    }
                }, 250);
                _tick();
            });

            advisorMiniPanel.append(dismissBtn);

            advisorMiniPanel.click(function () {
                _launchMain(missionObject.id, stepObject.id);
            });

            advisorMiniPanel.appendTo(_advisorMiniPanelArea);

            missionObject.miniPanelElement = advisorMiniPanel;

        }

        //which step of this mission is the player on
        missionObject.currentStepID = null; //the step the player is on and needs to complete
        missionObject.reIndexSteps(); //this updates the currentstep

        //get sthe total step count of the mission
        missionObject.stepCount = function stepCount() {
            return missionObject.steps.length;
        };


        missionObject.goPrevStep = function (fromStepID) {
            var stepIndex = missionObject.getStepIndexByStepID(fromStepID);
            var prevStepObject = missionObject.steps[stepIndex - 1];
            if (prevStepObject) {
                _selectStep(prevStepObject.id);
            }
        };

        missionObject.goNextStep = function (fromStepID) {
            var stepIndex = missionObject.getStepIndexByStepID(fromStepID);
            var nextStepObject = missionObject.steps[stepIndex + 1];
            if (nextStepObject) {
                _selectStep(nextStepObject.id);
            }           
        };

        missionObject.getStepIndexByStepID = function (stepID) {

            for (var i = 0; i < missionObject.steps.length; i++) {
                if (missionObject.steps[i].id == stepID) {
                    return i;
                }
            }

        }

        missionObject.getStepByStepID = function (stepID) {

            for (var i = 0; i < missionObject.steps.length; i++) {
                if (missionObject.steps[i].id == stepID) {
                    return missionObject.steps[i];
                }
            }

        }
        //add the fleshed out mission object to list of missions
        _missions[missionObject.id] = missionObject;

    }


    //check if a given quest has advisor valid tags, and is at the moment advisable
    //as in a research quest shouldnt be advised if a research is happening, or a building quest during building etc
    function _questValidToAdd(quest) {

        var questTag = quest.Tag;

        //if its a research quest, then there must be researchers available
        if (_tagFind(questTag, 'Res_') ||
            _tagFind(questTag, 'B_') ||
            _tagFind(questTag, 'Avatar_') ||
            _tagFind(questTag, 'ChangeVillageName') ||
            _tagFind(questTag, 'JoinAClan') ||
            //_tagFind(questTag, 'AttackRebel') ||
            _tagFind(questTag, 'SpyOnVillage') ||
            _tagFind(questTag, 'CaptureAVillage1') ||
            _tagFind(questTag, 'CaptureAVillage2') ||
            _tagFind(questTag, 'CaptureAVillage3') ||
            _tagFind(questTag, 'Gifts_BuySilver') ) {
            return true;
        } else {
            return false;
        }

    }


    //as in a research quest shouldnt be advised if a research is happening, or a building quest during building etc
    function _questValidToDoNow(quest) {

        var questTag = quest.Tag;

        //if its a research quest, then there must be researchers available
        if (_tagFind(questTag,'Res_')) {
            if (ROE.Player.MyResearch.numOfIdleResearchers > 0) {
                return true;
            } else {
                return false;
            }
        }

        //if building, a building quest isnt advisable
        if (_tagFind(questTag, 'B_')) {
            if ($('#vov').hasClass('constructing')) {
                return false;
            } else {
                return true;
            }
        }

        return true;

/*
        //misc valid quests
        if (_tagFind(questTag, 'Avatar_') ||
            _tagFind(questTag, 'ChangeVillageName') ||
            _tagFind(questTag, 'JoinAClan') ||
            //_tagFind(questTag, 'AttackRebel') ||
            _tagFind(questTag, 'SpyOnVillage')) {
            return true;
        } else {
            return false;
        }
*/

    }

    function _tagFind(questTag, tag) {
        return questTag.search(tag) > -1;
    }

    function _getquestminiPanelimage(quest) {
        var questTag = quest.Tag;

        //if its a research quest, then there must be researchers available
        if (_tagFind(questTag, 'Res_')) {
            return 'https://static.realmofempires.com/images/icons/M_ResearchList2.png';
        } else if (_tagFind(questTag, 'B_')) {
            return 'https://static.realmofempires.com/images/icons/m_quickbuild.png';
        } else {
            return 'https://static.realmofempires.com/images/icons/m_Quests2.png';
        }

    }

    
    function _createQuestMission(quest) {


        var missionObject = {
            id: quest.ID,
            quest: quest,
            label: quest.Title,
            desc: quest.Goal,
            data: {},
            completed: false,
            miniPanelElement: null,
            dismissed: false,
            validAdvisorQuest: true
        };



        missionObject.removeMiniPanel = function () {
            if (missionObject.miniPanelElement) {
                missionObject.miniPanelElement.remove();
                missionObject.miniPanelElement = null;
            }
        }



        //given a mission stepObject makes an advisor mini panel for the mission step
        missionObject.addMiniPanel = function () {

            if (missionObject.miniPanelElement) {
                return;
            }

            var advisorMiniPanel = $('<div class="advisorMiniPanel questMiniPanel shown" data-missionid="' + missionObject.id + '" data-stepid="">' +
                    '<div class="label">' + missionObject.label + '</div>' +
                    '<div class="desc">' + missionObject.desc + '</div>' +
                    '<div class="view BtnBSm2 fontButton1L">Continue</div>' +
                +'</div>');


            advisorMiniPanel.css('background-image', 'url(' + _getquestminiPanelimage(missionObject.quest) + ')');

            var dismissBtn = $('<div class="dismiss"></div>').click(function (event) {
                event.stopPropagation();
                _dockAdvisorMiniPanel(true);
                setTimeout(function () {
                    if (!(_advisorLaunchIcon.hasClass('hide') || _advisorLaunchIcon.hasClass('zero'))) {
                        ROE.Utils.attentionGrabber(_advisorLaunchIcon, 1);
                    }
                }, 250);
                _tick();
            });

            advisorMiniPanel.append(dismissBtn);

            advisorMiniPanel.click(function () {
                ROE.Frame.showIframeOpenDialog('#questsDialog', 'Quests.aspx?selectquestid=' + missionObject.id);
                //console.log(missionObject.quest);
            });

            advisorMiniPanel.appendTo(_advisorMiniPanelArea);

            missionObject.miniPanelElement = advisorMiniPanel;

        }

        missionObject.flyPanel = function () {
            if (missionObject.miniPanelElement) {

                var flyPanel = $('<div class="advisorFlyPanel"></div>').appendTo('body');
                flyPanel.css({
                    top: missionObject.miniPanelElement.offset().top || '60%',
                    left: missionObject.miniPanelElement.offset().left || '400px',
                });
                flyPanel.animate({ top: '-=30px' }, 500, "easeOutSine")

                    .animate({
                        top: $('#linkQuests').offset().top, left: $('#linkQuests').offset().left,
                        width: '44px', height: '44px'
                    }, 1200, "easeInSine", function () {
                        flyPanel.remove();
                        missionObject.removeMiniPanel();
                    });
         
                missionObject.removeMiniPanel();

            }
            
        }

        missionObject.showMiniPanel = function () {
            if (missionObject.miniPanelElement) {
                missionObject.miniPanelElement.addClass('shown');
            } else {
                missionObject.addMiniPanel();
            }
        }

        
        missionObject.hideMiniPanel = function () {
            if (missionObject.miniPanelElement) {
                missionObject.miniPanelElement.removeClass('shown');
            }
            
        }


        //checks if a quest is valid to do right now
        missionObject.validNow = false;
        missionObject.validateNow = function () {

            var isValidNow = _questValidToDoNow(missionObject.quest);

            //if current validity changed, do a refresh of recommended quests 
            if (missionObject.validNow != isValidNow) {
                _refreshRecommendedQuests();
            }
            
            missionObject.validNow = isValidNow;
            return isValidNow;
        }


        

        return missionObject;

        
        
    }


    function _dockAdvisorMiniPanel(dock) {
        if (dock) {
            _advisorMiniPanelArea.addClass('dock');
            _advisorMiniPanelIsDocked = true;
        } else {
            _advisorMiniPanelArea.removeClass('dock');
            _advisorLaunchIcon.addClass('hide');
            _advisorMiniPanelIsDocked = false;
        }

    }


    //utils

    function _nearestRebelToCSV() {

        var landmarkVillages = ROE.Landmark.villages;
        var village;
        var dist = 0, nearestDist = 99999;
        var closestVillage = null;

        for (var x_y in landmarkVillages) {
            village = landmarkVillages[x_y];

            if (village.player.PN == "*Rebels*") {

                dist = ROE.Utils.CalculateDistanceBetweenVillages(ROE.CSV.x, ROE.CSV.y, village.x, village.y);
                if (dist < nearestDist) {
                    nearestDist = dist;
                    closestVillage = village;
                }

            }
        }

        return closestVillage;

    }

    //save to server storage
    function _saveProgress(onCompleteFunc) {
        ROE.Frame.busy('Saving progress...', 5000, $('#advisorMain'));
        ROE.LocalServerStorage.set('Advisor-MissionStatus', _missionStatus, function saveCallback(data) {
            ROE.Frame.free($('#advisorMain'));
            if (onCompleteFunc) { onCompleteFunc(); }
        });
    }

    //gets your mission status from localserver storage
    function _getMissions() {
        var savedMissionStatus = ROE.LocalServerStorage.get('Advisor-MissionStatus');
        if (savedMissionStatus) {
            _missionStatus = savedMissionStatus;
        }
        return _missionStatus || {};
    }

    //spawn an image and animate it somewhere
    function _spawnFlyElement(data) {

        var src = data.src || "";
        var w = data.w || 88;
        var h = data.h || 88;
        var destinationElement = data.destinationElement;


        var flyElement = $('<div class="advisorSpawnFly">').appendTo($('body'));
        flyElement.css({
            left: $(window).width() / 2 - flyElement.width() / 2,
            top: $(window).height() / 2 - flyElement.height() / 2,
            width: w,
            height: h,
            'background-image': 'url(' + src + ')'
        });

        if (destinationElement && destinationElement.length) {

            var rewardsIdestinationElementOffset = destinationElement.offset();
            flyElement.animate({ top: '-=30px' }, 500, "easeOutSine")
                .animate({ top: rewardsIdestinationElementOffset.top, left: rewardsIdestinationElementOffset.left, 
                    width:'44px', height: '44px' }, 1200, "easeInSine", function () {
                        $(this).remove();
                    });

        } else {

            flyElement.animate({ top: '-=30px' }, 1500, "easeOutSine").animate({ top: '-=5px', opacity: 0 }, 300, "easeOutSine", function () {
                $(this).remove();
            });

        }
    }




    function _refreshRecommendedQuests() {
        ROE.Api.call_next_recommended_quest(function _refreshRecommendedQuestsCB(data) {
            if (data.nextQuests) {
                ROE.Player.nextRecommendeQuests = data.nextQuests;
                _tick();
            }
        });
    }

    obj.init = _init;
    obj.launchMain = _launchMain;



    //TESTNG - nukes progress data
    window.ByeMrAdvisor = function () {
        ROE.LocalServerStorage.set('Advisor-MissionStatus', {}, function saveCallback(data) {
            window.location.reload();
        });
    }


}(window.ROE.Advisor = window.ROE.Advisor || {}));