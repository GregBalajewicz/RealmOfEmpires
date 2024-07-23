(function (ROE) {
}(window.ROE = window.ROE || {}));

(function (obj) {

    obj.ended = ended;
    obj.recover = recover;
    obj.show = show;
    obj.start = start;
    obj.stepset = stepset;
    obj.startIfNotAlreadyRan = startIfNotAlreadyRan;
    obj.startIfRequested = startIfRequested;

    function ended() {
        if (ROE.collectAnalyticsOnThisRealm) {
            mixpanel.track('TutorialEnd', { 'RealmID': ROE.realmID });
        }

        var steps = ROE.Tutorial.steps[mininame()];
        if (steps.onclose)
            steps.onclose(isDontShowChecked());

        BDA.Database.LocalDel('TutorialStepAlways');
        BDA.Database.LocalDel('TutorialStep');
        BDA.Database.LocalDel('TutorialName');

        //$('#tutorial .dialog, #tutorial .pointer').hide();
        $('#tutorial').hide(); //why not hide everything tutorial related at this time? any problems? -farhad feb 11 2015
        $('#tutorial .dialog .dontShow input').prop('checked', false);
        $('.blockscreens .part, .blockscreens .whole').hide();
        $('#tutorial .dialog .next').unbind();
        clearInterval(this.timer);
        this.timer = null;
    };

    function mininame() {
        if (!BDA.Database.LocalGet('TutorialName')) {
            return ROE.isMobile ? "mainMobile" : "mainDesktop";
        }

        return BDA.Database.LocalGet('TutorialName'); 
    }

    function isDontShowChecked() {
        return $('#tutorial .dialog .dontShow input')[0].checked;
    }

    function startIfNotAlreadyRan(num, name) {
        if (!ROE.LocalServerStorage.get('tutorial_' + name) || $.cookie('TutorialStart') == 'true') {
            start(num, name);
        }
    }

    function startIfRequested(num, name) {
        if ($.cookie('TutorialStart') == 'true') {
            start(num, name);
        }
    }

    function flagRanSet() {
        ROE.LocalServerStorage.set('tutorial_' + mininame(), true);
    }

    function start(num, name) {

        if (ROE.rt == "X") {
            //no tutorial in RX
            return;
        }

        /*
        tutorial was firing too many times. fix when the tutorial is being redone. 
        for now, those events are being ommited in the config file

        - the issue is most likey due to the tutorial failing, some JS error that forces it to fail, but the JS code tries to restart it over and over again

        if (ROE.collectAnalyticsOnThisRealm) {
            mixpanel.track('mTutorialStart', { 'RealmID': ROE.realmID });
        }
        */
        ROE.Tutorial.stepset();

        // this is to restart tutorial after resfresh only for main tutorial
        if (!num && !name && !mininame().lastIndexOf('main') == 0) return;

        if (num >= 1 || num === 0) {
            BDA.Database.LocalSet('TutorialStep', num);
        }
        if (name) {
            BDA.Database.LocalSet('TutorialName', name);
        }
        ROE.Tutorial.recover();

        if (!ROE.isMobile) {
            $('#tutorial .dialog').draggable();
        }
    }

    function n(stepid) {
        if (typeof (stepid) == 'number') return stepid;
        if (typeof (stepid) == 'string') {
            var steps = ROE.Tutorial.steps[mininame()];

            for (var i = 0; i < steps.items.length; i++) {
                if (steps.items[i].id == stepid) {
                    return i;
                }
            }
        }
    }

    function recover() {
        var dialog = $('#tutorial .dialog');


        // recover executes second time, after closing popup
        if (this.timer == null) {
            if (BDA.Database.LocalGet('TutorialStep') != null || $.cookie('TutorialStart') == 'true') {




                // goes to start for a case when refreshed page, and possibly on step not from vov
                if (BDA.Database.LocalGet('TutorialStep') != "0") {
                    // uncomment this for case if you want to close after refresh
                    //this.ended(); return;

                    // now after refresh goes to 1 step
                    BDA.Database.LocalSet('TutorialStep', "0");
                }

                $.cookie('TutorialStart', '');

                $('.next, .skip', dialog).click(function TutorialNextClicked(e) {
                    var step = ROE.Tutorial.steps[mininame()].items[BDA.Database.LocalGet('TutorialStep')];

                    if (step.skip) {
                        BDA.Database.LocalSet('TutorialStep', n(step.skip));
                    } else {
                        if (!step.ifcond && !step.go) {
                            if (step.nextif || step.nextifno) {
                                return; /* could be fast click - do nothing */
                            }
                            BDA.Database.LocalInc('TutorialStep');
                        } else {
                            BDA.Database.LocalSet('TutorialStep',
                                n(!step.go ? ($(step.ifcond).length > 0 ? step.iftrue : step.iffalse) : step.go)
                            );
                        }
                    }
                    //
                    // if there was an action attached to this step, then do it now. 
                    //
                    if (typeof (step.actionOnNext) === 'function') {
                        step.actionOnNext(isDontShowChecked());
                    }

                    step = ROE.Tutorial.steps[mininame()].items[BDA.Database.LocalGet('TutorialStep')];
                    if (step == null) {
                        ROE.Tutorial.ended();
                    } else {
                        ROE.Tutorial.show(step);
                    }
                });

                //$('.tutclose', dialog).click(function TutorialDialogClose() { ROE.Tutorial.ended(); });

                ROE.Tutorial.show(ROE.Tutorial.steps[mininame()].items[BDA.Database.LocalGet('TutorialStep')]);

                // this timer cycling every 200 seconds, and maded for nextif, nextifno options
                // this options for case when you need to wait for jq selector that true or not exists any more
                // like after SM you wait for Popup appear
                obj.timer = setInterval(function TutorialIntervalForNext() {

                    //This prevents Tutorial from collapsing if 'TutorialStep' value was somehow wiped out
                    //this should not normally happen, but we are addressing a BDA.Database.LocalClear(); problem at this time,
                    //that happens during the first time a player is logging in - farhad
                    var storedStep = BDA.Database.LocalGet('TutorialStep');
                    if (storedStep == null || typeof (storedStep) == undefined) {
                        BDA.Database.LocalSet('TutorialStep', 0);
                    }

                    var step = ROE.Tutorial.steps[mininame()].items[BDA.Database.LocalGet('TutorialStep')];

                    // made for when, I call it Always steps, steps that waits for some condition
                    // that could happen in any moment, and should send back to real step after condition
                    if (BDA.Database.LocalGet('TutorialStepAlways') == null) {
                        var alwaysActive = $.grep(ROE.Tutorial.steps[mininame()].items, function TutorialFindAlwaysActive(n, i) {
                            if (n.when) {
                                if ($(n.when).length > 0) {
                                    BDA.Database.LocalSet('TutorialStepAlways', i);
                                    return true;
                                }
                            }
                        });

                        if (alwaysActive.length > 0) {
                            step = alwaysActive[0];
                            ROE.Tutorial.show(step);
                        }
                    } else {
                        step = ROE.Tutorial.steps[mininame()].items[BDA.Database.LocalGet('TutorialStepAlways')];
                    }

                    function TutorialNextStep(ifelem, expr, step, go) {
                        if (ifelem) {
                            if (expr($(ifelem).length)) {
                                if (!step.when) {
                                    if (go) {
                                        BDA.Database.LocalSet('TutorialStep', go);
                                    } else {
                                        BDA.Database.LocalInc('TutorialStep');
                                    }

                                    if (step.actionOnNext) {
                                        step.actionOnNext(isDontShowChecked());
                                    }
                                }

                                BDA.Database.LocalDel('TutorialStepAlways');
                                step = ROE.Tutorial.steps[mininame()].items[BDA.Database.LocalGet('TutorialStep')];

                                if (step == null) {
                                    // if step after go not found, ending tutorial
                                    ROE.Tutorial.ended();
                                } else {
                                    ROE.Tutorial.show(step);
                                }
                            }
                        }
                    }

                    function TutorialCheck1(l) { return l > 0; }
                    function TutorialCheck2(l) { return l == 0; }

                    try {
                        TutorialNextStep(step.nextif, TutorialCheck1, step, n(step.go));
                        TutorialNextStep(step.nextifno, TutorialCheck2, step, n(step.go));
                        TutorialNextStep(step.skipif, TutorialCheck1, step, n(step.skip));
                        TutorialNextStep(step.skipifno, TutorialCheck2, step, n(step.skip));
                    } catch (ex) {
                        var roeex = new BDA.Exception("Error in tutorial TutorialIntervalForNext", ex);
                        roeex.data.add('step', step);
                        roeex.data.add('mininame()', mininame());
                        roeex.data.add('stepnum', BDA.Database.LocalGetByPrefix('TutorialStep'));
                        BDA.latestException = roeex;
                        throw roeex;
                    }
                }, 200);
            }
        }
    }

    // processing step that givven
    function show(step) {
        $('#tutorial').show();
        $('#tutorial .pointer-circle').remove();

        var minitutor = ROE.Tutorial.steps[mininame()];

        if (step == null) step = minitutor.items[BDA.Database.LocalGet('TutorialStep')];

        if (step.show) {
            step.show();
        }

        var d = $('#tutorial .dialog').show();

        function TutorialGetElementCoords(elemSelector) {

            var elem = $(elemSelector);
            if (elem.length < 1) {               
                //it means the selector given was wrong or item doesnt exist in DOM at this moment
                //this shouldnt normally happen, if it does, something different has gone wrong, handle gracefully
                return { width: 0, height: 0, top: 0, left: 0 };
            }
           
            var elw = elem.outerWidth();
            var elh = elem.outerHeight();
            var elp = elem.offset();
            var elt = Math.ceil(elp.top);
            var ell = Math.ceil(elp.left);

            // offset works great, but dont count cases of transition scale, that is on vov
            if (elem.parents('.mainView .vovMain').length > 0) {
                elw = Math.ceil(elw * 0.67);                   
                elh = Math.ceil(elh * 0.67);
            }

            if (!ROE.isMobile && elem.parents('.vovMain').length > 0) {
                var ps = elem.parents('.vovMain').offset();
                var fo = $('#form1').offset();
                ps.top -= fo.top;
                ps.left -= fo.left;
                elw = Math.ceil(elw * 0.8);
                elh = Math.ceil(elh * 0.8);
                elt = Math.ceil(elt);
                ell = Math.ceil(ell);
            }

            return { width: elw, height: elh, top: elt, left: ell };
        }

        // specifing zone accessible for user, 
        // if - false, whole screen blocked
        // if - 'selector', find that element and its position, and surround it with blocking blocks
        if (!step.clickable) {
            $('.blockscreens .whole').show();
            $('.blockscreens .part').hide();
        } else {
            $('.blockscreens .whole').hide();
            $('.blockscreens .part').show();

            var epos = TutorialGetElementCoords(step.clickable);

            // made because sometimes top is -1 while height do default, that is 80px, so zerroing
            function zero(v) {
                return v < 0 ? 0 : v;
            }

            var pd = !step.padding ? 0 : step.padding; // padding
            var pdt = !step.paddingTop ? 0 : step.paddingTop; // paddingTop

            $('.blockscreens .part1').css({ height: zero(epos.top - pd - pdt) });
            $('.blockscreens .part2').css({ top: epos.top - pd - pdt, width: epos.left - pd });
            $('.blockscreens .part3').css({ top: epos.top + epos.height + pd, left: epos.left - pd });
            $('.blockscreens .part4').css({ top: epos.top - pd - pdt, left: epos.left + epos.width + pd, height: epos.height + (2 * pd) });
        }
        var t = $('#tutorial .dialog .textwrite');
        if (t.html() != step.text) { t.html(step.text).hide().fadeIn(500); }

        // arrow specify
        function TutorialArrow(step) {
            var w = TutorialGetElementCoords(step.arrow);
            _attentionGrabber(w);
            var p = $('#tutorial .pointer').show();

            if (!step.arrowon) {
                p.css({ top: w.top + w.height, left: w.left + parseInt(w.width / 2) - parseInt(p.outerWidth() / 2) });               
                p.removeClass('south');
            }
            else {
                p.css({ top: w.top - p.outerHeight(), left: w.left + parseInt(w.width / 2) - parseInt(p.outerWidth() / 2) });
                p.addClass('south');
            }

        }

        if (step.arrow) {
            TutorialArrow(step);
        } else {
            $('#tutorial .pointer').hide();
        }

        if (step.arrow && step.clickable && !step.nocircle) {
            var c = $('<div class="pointer-circle">').appendTo('#tutorial');
            c.position({
                my: "center center",
                at: "center center",
                of: step.arrow,
                collision: "none"
            });
        }

        
        // if used next event condition, next button should hided
        if (step.nextif || step.nextifno) {
            $('#tutorial .next').hide();
        } else {
            $('#tutorial .next').show();
        }

        if (step.done) {
            $('#tutorial .next').show().html("Done");
        } else {
            $('#tutorial .next').html("Next");
        }

        if (minitutor.dontShowText !== undefined) {
            var text = minitutor.dontShowText == '' ? "Don't tell me this again" : minitutor.dontShowText;

            $('#tutorial .dontShow').show().find('span').html(text);
        } else {
            $('#tutorial .dontShow').hide();
        }

        if (step.skip) {
            $('#tutorial .skip').show();
        } else {
            $('#tutorial .skip').hide();
        }

        // default coord
        if (step.coord == null) step.coord = [30, 200];

        $('#tutorial .dialog').css({ left: step.coord[0], top: step.coord[1] });

        // remove the close if requesed
        if (step.noClose) {
            $('#tutorial .tutclose').hide();
        }
    }

    // we do this in function, so that $('[rel=bMine_Level]').html() will be ready in time of onload
    function stepset() {
        // about steps
        // there are next properties:
        // coord:      by default [30, 200]
        // text:       text for dialog
        // arrow:      selector where to point
        // clickable:  selector what area accesed for touch
        // padding, 
        // paddingTop: manually resize area for clickable, if some elements not needed on full area
        // nextif, 
        // nextifno:   for what $(selector) we wait before go next
        // skip:       same as 'next' with 'go', but give to skip nextif. 
        //             means you can do nextif or skip to some step
        // go:         where to go after this step, without it - goes incrementally
        // ifcond, 
        // iftrue, 
        // iffalse:    in future could be merged with nextif, 
        // when:       step with when checked all the time, made specifically for LevelUp case, 
        //             that can show any time, and brake tutorial. After satisfy step in nextif(no)
        //             it will get back to last step, that interrupt it
        // actionOnNext : function that shoudl be called on click of the next button
        // noClose: remove the close link from this step
        this.steps = {
            newAlertSystemNotification: {
                items: [
                    {
                        coord: [30, 140],
                        text: "Welcome to RoE Mobile App Beta!<br><br>A major feature of this version is an improved alert system.",
                        clickable: false,
                        noClose: true
                    },
                    {
                        coord: [30, 195],
                        text: "Check out the alerts screen and configure them to your liking",
                        clickable: false,
                        actionOnNext: function () { ROE.Frame.settingsPopup(); },
                        noClose: true
                    }
                ]
            },
            mainDesktop: {
                onclose: function () {                   
                        flagRanSet();                   
                },
                items: [
                    {
                        coord: ['42%', '22%'],
                        text: "Welcome to Realm of Empires: Warlords Rising!",
                        clickable: false
                    },
                    {
                        coord: ['42%', '22%'],
                        text: "This is your village.",
                        arrow: '#surface #select',
                        clickable: false
                    },
                    {
                        coord: ['42%', '22%'],
                        text: "This is the zoomed-in look at your village",
                        clickable: false,
                        arrow: '#vovFrame',
                        arrowon: 'top',
                    },
                    {
                        coord: ['42%', '22%'],
                        text: "Around your village are villages of other players as well as villages owned by Rebels <img src='https://static.realmofempires.com/images/map/freb.png'>",
                        clickable: false,
                    },
                    {
                        coord: ['42%', '22%'],
                        text: "New players will appear around you so keep an eye out for new friends and foes",
                        clickable: false
                    },
                    {
                        coord: ['42%', '22%'],
                        text: "Your goal is to take over as many villages on the map as you can!  But first, lets grow the village you have.",
                        clickable: false
                    },
                    /*1 - As you can see level... */
                    {
                        coord: ['42%', '22%'],
                        text: BDA.Dict.SilverMineLevel.format({ l: $('[rel=bMine_Level], .level[data-bid=5]').html() }),
                        clickable: false,
                        arrow: '#linkMine_Level'
                    },
                    /*2 - Tap to enter mine */
                    {
                        coord: ['42%', '22%'],
                        text: BDA.Dict.tutorialEnterSM,
                        clickable: '#linkMine_Level',
                        arrow: '#linkMine_Level',
                        nextif: '#building_popup2:visible'
                    },
                    /*3 - This is the current level, if can upgrade to 4, else go to 5 */
                    {
                        coord: ['42%', '11%'],
                        text: "Here you can upgrade your silver mine",
                        //arrow: '#popup_Building2 .headerLevel',
                        ifcond: '#building_popup2 .upgradePanel[data-canupgrade="0"]',
                        iftrue: 'can-upgrade',
                        iffalse: 'can-not-upgrade'

                    },

                    /*4 */
                    {
                        id: 'can-upgrade',
                        coord: ['42%', '12%'],
                        text: BDA.Dict.Upgrade,
                        clickable: '#building_popup2 .upgradePanel .actionButton',
                        arrow: '#building_popup2 .upgradePanel .actionButton',
                        nextif: '#building_popup2 .upgradePanel[data-upgradestate="upgrading"]',
                        go: 'speedup1'
                    },

                    /*5 - if cannot upgrade*/
                    {
                        id: 'can-not-upgrade',
                        coord: ['42%', '12%'],
                        text: BDA.Dict.CanNotUpgrade,
                        clickable: false,
                        go: 'build_close'
                    },

                    /*6 - speedup click */
                    {
                        id: 'speedup1',
                        coord: ['42%', '12%'],
                        text: BDA.Dict.MineIsUpgrading,
                        clickable: '#building_popup2 .progressPanel .speedUpButton',
                        arrow: '#building_popup2 .progressPanel .speedUpButton',
                        skipifno: '#building_popup2 .progressPanel:visible',
                        nextif: '.speedUpOptionsPopup',
                        go: 'speedup2',
                        skip: 'build_close'
                    },

                    {
                        id: 'speedup2',
                        coord: ['42%', '12%'],
                        text: BDA.Dict.MineIsUpgrading,
                        clickable: '.speedUpOptionsPopup .speedUpButton:first',
                        arrow: '.speedUpOptionsPopup .speedUpButton:first',
                        nextif: '.speedUpOptionsPopup .sProgressBar.finished',
                        skipifno: '#building_popup2 .progressPanel:visible',
                        skip: 'speedup3_close'
                    },

                    {
                        id: 'speedup3_close',
                        coord: ['42%', '12%'],
                        text: BDA.Dict.CloseSpeedUp,
                        clickable: '.speedUpOptionsPopup .pHeader .pClose',
                        arrow: '.speedUpOptionsPopup .pHeader .pClose',
                        nextifno: '.speedUpOptionsPopup'
                    },

                    /*7 - close silver mine */
                    {
                        id: 'build_close',
                        text: "Well done! Now close the silver mine window",
                        clickable: '.ui-dialog[aria-describedby="buildingDialog"] button',
                        arrow: '.ui-dialog[aria-describedby="buildingDialog"] button',
                        nextifno: '#building_popup2:visible'
                    },

                    /*8 - point to map - selecting */
                    {
                        coord: ['42%', '12%'],
                        text: 'Now let\'s see how Research can help you grow faster! <br> Click on the research icon',
                        clickable: '#viewSwitch_res',
                        arrow: '#viewSwitch_res',
                        arrowon: 'top',
                        nextif: '.list-y.researchType:visible li[data-id=b5]'
                    },

                    /*9 - point to map - silver mine only clickable */
                    {
                        coord: ['42%', '12%'],
                        text: BDA.Dict.Research,
                        clickable: false
                    },

                    /*10 - point to map - silver mine only clickable */
                    {
                        coord: ['32%', '8%'],
                        text: BDA.Dict.SilverResearchGroup,
                        clickable: '.list-y.researchType li[data-id=b5]',
                        arrow: '.list-y.researchType li[data-id=b5]',
                        arrowon: 'top',
                        nextif: '.research[data-tutorial-state=detail]'
                    },

                    {
                        coord: ['32%', '8%'],
                        text: BDA.Dict.IncreaseSilverProduction,
                        clickable: false
                    },

                    /*13 - */
                    {
                        coord: ['32%', '8%'],
                        text: BDA.Dict.tutorialSelectResearch,
                        clickable: '.research .detail',
                        nocircle: true,
                        arrow: '.research .detail',
                        arrowon: 'top',
                        skip: 'research1',
                        nextif: '.research[data-tutorial-state=research]'
                    },

                    /*14 - */
                    {
                        id: 'research1',
                        coord: ['32%', '8%'],
                        text: BDA.Dict.ResearchEmpire,
                        clickable: false
                    },

                    {
                        coord: ['32%', '8%'],
                        text: "Well done! Now close the research window",
                        clickable: '.ui-dialog[aria-describedby="researchDialog"] button',
                        arrow: '.ui-dialog[aria-describedby="researchDialog"] button',
                        nextifno: '#researchDialog:visible',
                        show: function () {
                            ROE.Api.call("tutorial_done", { vid: ROE.SVID }, function () { });
                        }
                    },

                    {
                        text: BDA.Dict.OhLook,
                        clickable: false,
                        arrow: '#linkQuests'
                    },

                    {
                        text: BDA.Dict.TutorialIsNowComplete,
                        clickable: false,
                        done: true,
                        go: 100
                    },

                    {
                        text: BDA.Dict.AcceptNewTitle,
                        arrow: '.levelUpOverlay .pClose',
                        clickable: '.levelUpOverlay .pClose',
                        when: '.levelUpOverlay',
                        padding: 0,
                        nextifno: '.levelUpOverlay'
                    }
                ]
            },
            warRoomDesktop: {
                onclose: function (dontShow) {
                    if (dontShow) {
                        flagRanSet();
                    }
                },
                dontShowText: '',
                items: [
                    {
                        coord: ['75%', '50%'],
                        text: "Look, you gained access to war room!",
                        clickable: false
                    },
                    {
                        coord: ['75%', '50%'],
                        text: "You can see your villages and troops in them here",
                        clickable: false,
                        actionOnNext: function (dontShow) {
                            if (dontShow) {
                                flagRanSet();
                            }
                        }
                    }
                ]
            },
            warRoomMobile: {
                onclose: function (dontShow) {
                    if (dontShow) {
                        flagRanSet();
                    }
                },
                dontShowText: '',
                items: [
                    {
                        coord: [30, 325],
                        text: "My Liege, you've gained access to the War Room",
                        clickable: false
                    },
                    {
                        coord: [30, 325],
                        text: "Select which village to attack from, then choose the troops",
                        clickable: false
                    },
                    {
                        coord: [30, 325],
                        text: "Tap the # button to specify exact troop numbers",
                        clickable: '.ui-dialog[aria-describedby=quickCommandDialog]',
                        done: true,
                        go: 100,
                        nextifno: '.ui-dialog[aria-describedby=quickCommandDialog]:visible',
                        nextif: '#popup_attacks',
                        actionOnNext: function (dontShow) {
                            if (dontShow) {
                                flagRanSet();
                            }
                        }
                    }
                ]
            },
            incomingMobileFilter: {
                onclose: function (dontShow) {
                    if (dontShow) {
                        flagRanSet();
                    }
                },
                dontShowText: '',
                items: [
                    {
                        coord: [30, 280],
                        text: "My Liege, here you can see actual incoming troops.",
                        clickable: false
                    },
                    {
                        coord: [30, 280],
                        text: "To get to the SUMMARY VIEW, tap here",
                        clickable: false,
                        arrow: '.byFromOrToVillOrPlayer .clearFilter',
                        done: true,
                        actionOnNext: function (dontShow) {
                            if (dontShow) {
                                flagRanSet();
                            }
                        }
                    }
                ]
            },
            mainMobile: {
                onclose: function () {
                    flagRanSet();
                },
                items: [
                /*0 - Welcome */
                {
                    coord: [30, 240],
                    text: BDA.Dict.Welcome,
                    clickable: false
                },
                /*1 - As you can see level... */
                {
                    coord: [30, 240],
                    text: BDA.Dict.SilverMineLevel.format({ l: $('[rel=bMine_Level], .level[data-bid=5]').html() }),
                    clickable: false,
                    arrow: '#linkMine_Level'
                },
                /*2 - Tap to enter mine */
                {
                    coord: [30, 240],
                    text: BDA.Dict.TapToEnter,
                    clickable: '#linkMine_Level',
                    arrow: '#linkMine_Level',
                    nextif: '#building_popup2'
                },
                /*3 - This is the current level, if can upgrade to 4, else go to 5 */
                {
                    text: BDA.Dict.SilverCurrentLevel,
                    arrow: '#popup_Building2 .headerLevel',
                    ifcond: '#building_popup2 .upgradePanel[data-canupgrade="0"]',
                    iftrue: 4,
                    iffalse: 5

                },

                /*4 */
                {
                    coord: [30, 240],
                    text: BDA.Dict.Upgrade,
                    clickable: '#building_popup2 .upgradePanel .actionButton',
                    arrow: '#building_popup2 .upgradePanel .actionButton',
                    nextif: '#building_popup2 .upgradePanel[data-upgradestate="upgrading"]',
                    go: 'speedup1'
                },

                /*5 - if cannot upgrade*/
                {
                    text: BDA.Dict.CanNotUpgrade,
                    clickable: false,
                    go: 'build_close'
                },

                /*6 - speedup click */
                {
                    id: 'speedup1',
                    coord: [30, 325],
                    text: BDA.Dict.MineIsUpgrading,
                    clickable: '#building_popup2 .progressPanel .speedUpButton',
                    arrow: '#building_popup2 .progressPanel .speedUpButton',
                    skipifno: '#building_popup2 .progressPanel:visible',
                    nextif: '.speedUpOptionsPopup',
                    go: 'speedup2',
                    skip: 'build_close'
                },

                {
                    id: 'speedup2',
                    coord: [30, 325],
                    text: BDA.Dict.MineIsUpgrading,
                    clickable: '.speedUpOptionsPopup .speedUpButton:first',
                    arrow: '.speedUpOptionsPopup .speedUpButton:first',
                    nextif: '.speedUpOptionsPopup .sProgressBar.finished',
                    skipifno: '#building_popup2 .progressPanel:visible',
                    skip: 'speedup3_close'
                },

                {
                    id: 'speedup3_close',
                    coord: [30, 325],
                    text: BDA.Dict.CloseSpeedUp,
                    clickable: '.speedUpOptionsPopup .pHeader .pClose',
                    arrow: '.speedUpOptionsPopup .pHeader .pClose',
                    nextifno: '.speedUpOptionsPopup'
                },

                /*7 - close silver mine */
                {
                    id: 'build_close',
                    text: BDA.Dict.WellDone,
                    clickable: '.IFrameDivTitle .action.close',
                    arrow: '.IFrameDivTitle .action.close',
                    nextifno: '#building_popup2'
                },

                /*8 - point to map - selecting */
                {
                    coord: [30, 100],
                    text: BDA.Dict.GetMoreSilver,
                    clickable: '#viewSwitch_res',
                    arrow: '#viewSwitch_res',
                    arrowon: 'top',
                    nextif: '.list-y.researchType li[data-id=b5]'
                },

                /*9 - point to map - silver mine only clickable */
                {
                    coord: [30, 260],
                    text: BDA.Dict.Research,
                    clickable: false
                },

                /*10 - point to map - silver mine only clickable */
                {
                    coord: [30, 260],
                    text: BDA.Dict.SilverResearchGroup,
                    clickable: '.list-y.researchType li[data-id=b5]',
                    arrow: '.list-y.researchType li[data-id=b5]',
                    arrowon: 'top',
                    nextif: '.research[data-tutorial-state=detail]'
                },

                {
                    coord: [30, 325],
                    text: BDA.Dict.IncreaseSilverProduction,
                    clickable: false
                },



                /*13 - */
                {
                    coord: [30, 325],
                    text: BDA.Dict.TapResearch,
                    clickable: '.research .detail.slideLeftTo',
                    arrow: '.research .detail',
                    arrowon: 'top',
                    skip: 'research1',
                    nextif: '.research[data-tutorial-state=research]',
                    nocircle: true
                },

                // REMOVED STEPS WITH RESEARCHERS
                ///*13 - */
                //{
                //    coord: [30, 350],
                //    text: 'Now you are researching!',
                //    clickable: '#research .header',
                //    nextif: '#research .header-dropdown[data-tutorial-opened=true]'
                //},

                ///*14 - */
                //{
                //    coord: [30, 350],
                //    text: 'You can research more than 1 technology at a time if you hire more researchers!',
                //    clickable: '#research .header-dropdown .handle.sfx2',
                //    nextif: '#research .header-dropdown[data-tutorial-opened=false]'
                //},

                /*14 - */
                {
                    id: 'research1',
                    coord: [30, 325],
                    text: BDA.Dict.ResearchEmpire,
                    clickable: false
                },

                /*17 - point to map - clicked on button */
                //{
                //    text: BDA.Dict.EmpireGrasp,
                //    clickable: 'footer > .footernav > .togle',
                //    arrow: 'footer > .footernav > .togle',
                //    arrowon: 'top',
                //    nextif: '#viewSwitcher.showing'
                //},

                {
                    text: BDA.Dict.WellDone,
                    clickable: '.IFrameDivTitle .action.close',
                    arrow: '.IFrameDivTitle .action.close',
                    nextifno: '#popup_Research'
                },

                /*16 - point to map - selecting */
                {
                    text: BDA.Dict.EmpireGrasp,
                    clickable: 'footer > .footernav > .togle',
                    arrow: 'footer > .footernav > .togle',
                    arrowon: 'top',
                    nextif: '#map'
                },

                /*19 - map - nothing touchable */
                {
                    text: BDA.Dict.Neighbor,
                    clickable: false,
                    actionOnNext: function (dontShow) {
                        $('#map').attr('data-tutorial-1move-start', 'true')
                    }
                },

                /*20 - wait for first move */
                {
                    coord: [30, 300],
                    text: BDA.Dict.TappingMap,
                    clickable: '#mapwrap',
                    // maded for case 17871 - disallow clicking on the header of the map
                    // when no other way to find dom with needed size and position
                    paddingTop: -50,
                    // when you go beyond number in a list it ends, if will not be here, it will Inc to Always step
                    //go: 100

                    nextif: '#map[data-tutorial-1move=true]'
                },


                /*21 - point to map - selecting rebel */
                {
                    coord: [30, 300],
                    text: BDA.Dict.TapRebelVillage,
                    clickable: '#mapwrap',
                    paddingTop: -50,
                    nextif: '#map[data-tutorial-rebel-selected=true]',
                    skip: 'mapHeader1'
                },

                /*22 - map header */
                {
                    id:'mapHeader1',
                    coord: [30, 300],
                    text: BDA.Dict.TappingAVillage,
                    arrow: 'header .headernav .hmap.info',
                    clickable: false
                },

                /*23 - telling about header */
                {
                    coord: [30, 300],
                    text: BDA.Dict.ActionPanel,
                    clickable: 'header .headernav .hmap.info',
                    arrow: 'header .headernav .hmap.info'
                },

                /*24 - telling about header */
                {
                    coord: [30, 300],
                    text: BDA.Dict.GreatFirstTarget,
                    //arrow: 'header .headernav .hmap.info',
                    clickable: false
                },

                /*25 - explore map again and free from tutorial */
                {
                    coord: [30, 300],
                    text: BDA.Dict.ExploreTheMap,
                    paddingTop: -50,
                    clickable: '#mapwrap'
                },

                {
                    coord: [30, 300],
                    text: BDA.Dict.Friends,
                    paddingTop: -50,
                    clickable: '#mapwrap'
                },

                {
                    coord: [30, 300],
                    text: BDA.Dict.FreeMapExplore,
                    paddingTop: -50,
                    clickable: '#mapwrap'
                },

                {
                    text: BDA.Dict.ReturnToVoV,
                    clickable: 'footer > .footernav > .togle',
                    arrow: 'footer > .footernav > .togle',
                    arrowon: 'top',
                    nextif: '.vovMain',
                    show: function () {
                        ROE.Api.call("tutorial_done", { vid: ROE.SVID }, function () { });
                    }
                },

                {
                    text: BDA.Dict.OhLook,
                    clickable: false,
                    arrow: '.linkQuests'
                },

                {
                    text: BDA.Dict.TutorialIsNowComplete,
                    clickable: false,
                    done: true,
                    go: 100
                },

                {
                    coord: [40, 10],
                    text: BDA.Dict.AcceptNewTitle,
                    arrow: '.levelUpOverlay .pClose',
                    clickable: '.levelUpOverlay .pClose',
                    when: '.levelUpOverlay',
                    padding: 0,
                    nextifno: '.levelUpOverlay'
                }
                ]
            }
        };
    }

    //tries to direct the attention of a player to a given element.
    function _attentionGrabber(elPos) {

        var win = $(window);
        var tuto = $('#tutorial');
        $('.grabber').stop().remove();

        var gT = $('<div class="grabber gT">').appendTo(tuto);
        var gR = $('<div class="grabber gR">').appendTo(tuto);
        var gB = $('<div class="grabber gB">').appendTo(tuto);
        var gL = $('<div class="grabber gL">').appendTo(tuto);

        var speed = 800;
        var easing = "easeOutSine";
        var finalOpacity = .5;

        gT.css({ left: elPos.left + elPos.width / 2 - gT.width() / 2, top: -gT.height() });
        gR.css({ left: win.width(), top: elPos.top + elPos.height / 2 - gR.height() /2 });
        gB.css({ left: elPos.left + elPos.width / 2 - gB.width() / 2, top: win.height() });
        gL.css({ left: -gL.width(), top: elPos.top + elPos.height / 2 - gL.height() /2 });

        gT.animate({ top: elPos.top - gT.height(), opacity: finalOpacity }, speed, easing, function () {
            $('.grabber').stop().animate({ opacity: 0 }, 500, "easeInSine", function () {
                $('.grabber').stop().remove();
            });
        });
        gR.animate({ left: elPos.left + elPos.width, opacity: finalOpacity }, speed, easing, function () { });
        gB.animate({ top: elPos.top + elPos.height, opacity: finalOpacity }, speed, easing, function () { });
        gL.animate({ left: elPos.left - gL.width(), opacity: finalOpacity }, speed, easing, function () { });
    }

}(window.ROE.Tutorial = window.ROE.Tutorial || {}));

