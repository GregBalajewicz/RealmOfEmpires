
(function (ROE) {
}(window.ROE = window.ROE || {}));

window.ROE.Frame = window.ROE.Frame || {};
window.ROE.Frame.isBusy = window.ROE.Frame.isBusy || function () { };

(function (obj) {

    BDA.Console.setCategoryDefaultView("ROE.Player.refresh", false);
    BDA.Console.setCategoryDefaultView("roe.api", false);
    BDA.Console.setCategoryDefaultView("BDA.Templates", false);
    BDA.Console.setCategoryDefaultView("MixPanel", false);

    //ThroneRoom variables
    var _busyMask, _assetStage, _panelTop, _panelBottom, _panelOne; //cached jQuery objects
    var _view; //owner: displayed for owner of the throne | observer: displayed for someone else looking at this throne
    var _throneUserData; //the throne data
    var _stageChildren = [];
    var _chooseRealmsBox; //rips and stores the realmsbox element that was created by .cs

    var _mThrotle = 0; //mouse vars
    var _windowW = 0, _windowH = 0; //window dimensions
    var _viewMode = "normal"; // "normal" || "compact", when window seize below threshold we go to "compact"
    var _natRatio = 1; //important ratio, gets calculate per window resize
    var _pauseMovement = false; //stops the parallax
    var _toolTipTimer = null; //stores the tooltip timeout
    var _mBrowser = false;

    var _urlGetParams = {}; //URL GET Params
    var _fromNewsLetter = false; //if there news letter url param is present
    var _loggedIn = false; //if the current viewer is logged in
    var _inFrame = window != window.top; //is teh throne room being viewed under a frame


    var _throneAssets = {

        bkg: {
            type: 'bkg',
            src: "https://static.realmofempires.com/imAges/throneRoom/BACkground_01-1.jpg"
        },
        glow: {
            type: 'glow',
            src: "https://static.realmofempires.com/imAges/throneRoom/wIndowGlow_01.png"
        },
        throne: {
            type: 'throne',
            src: "https://static.realmofempires.com/imAges/throneRoom/tHroneAndStairs_01.png"
        },
        columnsB: {
            type: 'columnsB',
            src: "https://static.realmofempires.com/imAges/throneRoom/cOlumnBack_01.png"
        },
        columnsF: {
            type: 'columnsF',
            src: "https://static.realmofempires.com/imAges/throneRoom/COlumnForward_01.png"
        },
        columnsArch: {
            type: 'columnsArch',
            src: "https://static.realmofempires.com/imAges/throneRoom/cOlumnArch_01.png"
        },
        fog: {
            type: 'fog',
            src: "https://static.realmofempires.com/imAges/throneRoom/fOg_01.png"
        },
        candlebra: {
            type: 'candleBra',
            src: "https://static.realmofempires.com/imAges/throneRoom/cAndelabra_01.png"
        },
        candleglow: {
            type: 'candleGlow',
            src: "https://static.realmofempires.com/imAges/throneRoom/cAndleGlow_01.png"
        },

        shield: {
            type: 'shield',
            src: "https://static.realmofempires.com/imAges/throneRoom/SHIeld_01.png"
        }

    }

    var _numberImages = [
        'https://static.realmofempires.com/imAges/throneRoom/num0_01.png',
        'https://static.realmofempires.com/imAges/throneRoom/num1_01.png',
        'https://static.realmofempires.com/imAges/throneRoom/num2_01.png',
        'https://static.realmofempires.com/imAges/throneRoom/num3_01.png',
        'https://static.realmofempires.com/imAges/throneRoom/num4_01.png',
        'https://static.realmofempires.com/imAges/throneRoom/num5_01.png',
        'https://static.realmofempires.com/imAges/throneRoom/num6_01.png',
        'https://static.realmofempires.com/imAges/throneRoom/num7_01.png',
        'https://static.realmofempires.com/imAges/throneRoom/num8_01.png',
        'https://static.realmofempires.com/imAges/throneRoom/num9_01.png'
    ]

    var _throneTooltips = {
        lnkBacktoCR: "Go back to choose realm page",
        btnRealmProfileList: "A history of all your plays",
        leaderBoardBtn: "Realm player stats and leaderboard",
        joinRealmChat: "Join the realm chat",
        displayStatus: "Realm display status. <br>When a realm display is off, no one viewing your throne room can see that you played on this realm, unless they play with you on that realm.",
        globalPlayerName: "Global player name",
        highestTitle: "Your highest title achieved",
        floatSupportQ: "Send us your feedback",
        chatPlaceholder: "Chat live with the entire Realm of Empires community!",
        shareTr: "Share your Throne Room with everyone, and showcase your achievements!",
        like: "Give this Throne a big thumbs up!",
        endedToggle: "Hide/Show realms that have ended.",
        btnSpeedRealmStandings: "Realm X and Realm S personal best place finishes",
        btnChangelog: "Change log and patches"
    }

    var _layerParallax = [10 * .5, 20 * .5, 40 * .5, 80 * .5];
    //var _layerParallax = [10, 20, 40, 80];

    //load when everything is ready, go with owner mode for now
    $(document).ready(function () {

        _loggedIn = $('#loginbutton').length == 0; //if login button exists, youre not logged in

        _busyMask = $('.throneBusyMask');
        _assetStage = $('#panelMain #assetStage');
        _panelTop = $('#panelTop');
        _panelBottom = $('#panelBottom');
        _panelOne = $('#panelOne');
        _chooseRealmsBox = $('#chooseRealmsBox').remove();

        var realmEndedDisplay = localStorage.getItem('realmEndedDisplay');
        if (realmEndedDisplay && realmEndedDisplay == "hiding") {
            _hideEndedRealms();
        } else {
            _showEndedRealms();
        }

        $(window).resize(_assetsLayout);

        if (isMobile || (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini|Silk|PlayBook/i.test(navigator.userAgent))) {
            _initMobile();
        } else {
            $('body').addClass('desktop');
            _assetStage.mousemove(function (e) {
                _mouseMovement(e.clientX / _windowW /*, e.clientY / _windowH*/);
            });

            //hijack realm login anchors to do our special navigation
            $("body").delegate("a.oneRealm, a.realmLink", "click", function (e) {
                event.preventDefault();
                if (_inFrame) {
                    window.open(this.href, '_self');
                } else {
                    window.open(this.href, 'realmLogin');
                }
                return false;
            });

            //setup tooltip interaction
            $("body").delegate(".helpTooltip", "mouseover", function () {
                var element = $(this);
                _toolTipTimer = setTimeout(function () { _showTooltip(element); }, 800);
            });
            $("body").delegate(".helpTooltip", "mouseout mouseleave", function () {
                $('.toolTipBox').stop().fadeOut(200, function () { $(this).remove(); });
                window.clearTimeout(_toolTipTimer);
            });
            $("body").delegate(".helpTooltip", "click", function () {
                $('.toolTipBox').remove();
            });

        }

        //if not logged in and chat is clicked
        if (!_loggedIn) {
            $("body").delegate(".chatWindow .chatInput", "click input", function () {
                _promptForLogin();
            });
        }

        _setupDialogs();
        _load();
    });

    function _load() {
        _busy('L O A D I N G', 15000, null);

        //Parse URL GET Params
        if (location.search) {
            var parts = location.search.substring(1).split('&');
            for (var i = 0; i < parts.length; i++) {
                var nv = parts[i].split('=');
                if (!nv[0]) continue;
                _urlGetParams[nv[0]] = nv[1] || true;
            }
        } else if (QSUID != "") {
            _urlGetParams["uid"] = QSUID;
        }

        _fromNewsLetter = _urlGetParams.newsletter;

        ROE.Api.call_tr_userinfo(_urlGetParams, _loaded);
    }

    function _loaded(data) {

        _free();
        _throneUserData = data;
        _view = data.View;
        _cleanUpThroneUserData();
        _sortPlayerRealmsByRealmID();


        if (!_loggedIn && _fromNewsLetter) {
            _panelBottom.append('<div class="BtnBLg1 fontButton1L chatBar chatPlaceholder helpTooltip" data-tooltipid="chatPlaceholder" ><div class="barName">Global Chat</div></div>');
            $('.chatPlaceholder', _panelBottom).click(_promptForLogin);
        }

        if (_view == "owner") {
            _initOwner();
            //_initUserVoice();
            ROE.Chat2.startHub(null, null, $('#chatBarContainer'), _urlGetParams["openChatPID"], _urlGetParams["openChatRID"]);
        } else {
            _initObserver();
            $('.mChatMask, .mChatToggle, #chatBarContainer').remove();
        }
    
        

        //if mobile AND window opener exists AND has a throneroomWindow child, add a close button
        if (_mBrowser && window.opener && window.opener.throneroomWindow) {
            $('<div id="windowCloser" ></div>').click(function () {
                window.opener.throneroomWindow.close();
                window.opener.throneroomWindow = null;
            }).appendTo('body');
        }
    }

    function _sortPlayerRealmsByRealmID() {
        var sortedPIDsByRealmID = [];
        for (var pid in _throneUserData.PlayerRealmsInfo) {
            sortedPIDsByRealmID.push({ pid: pid, value: _throneUserData.PlayerRealmsInfo[pid].Realm.ID });
        }
        sortedPIDsByRealmID.sort(function (x, y) { return x.value - y.value });
        _throneUserData['sortedPIDsByRealmID'] = sortedPIDsByRealmID;
    }

    function _cleanUpThroneUserData() {
        // remove any stat that is zero. 
        for (var prop in _throneUserData.GameWideTopStats) {
            value = _throneUserData.GameWideTopStats[prop].Value;
            pid = _throneUserData.GameWideTopStats[prop].AchievedByPlayerID;
            if (value === 0) {
                delete _throneUserData.GameWideTopStats[prop];
            }

        }

    }

    function _initMobile() {
        _mBrowser = true;
        $('body').addClass('mobile');

        $('.mChatMask ').click(function () {
            _mToggleChatOff();
        });

        $('.mChatToggle').click(function () {
            _mToggleChatList();
        });

        $("body").delegate(".chatButton.minimize", "click", function (e) {
            if ($('.chatWindow.displayed').length < 1) {
                _mToggleChatOff();
            }
        });

        
        if ((navigator.userAgent.match(/iPhone/i)) || (navigator.userAgent.match(/iPod/i))) {
            //if iPhone then dont do the chat inpput movement, as the app already squeeshes screen
            //it became apparent that the squish was causing new issues, so we do the following -farhad
            $("body").delegate(".chatWindow .chatInput", "focus", function () {
                setTimeout(function () {
                    $('#chatContainer').css({
                        top: 'auto',
                        height: window.innerHeight
                    });
                }, 100);
            });
            $("body").delegate(".chatWindow .chatInput", "blur", function () {
                $('#chatContainer').css({
                    top: '0px',
                    height: 'auto'
                });
            });
        } else {
            //android keyboard overlays the input, so we need to move it up
            $("body").delegate(".chatWindow .chatInput", "focus", function () {
                $(this).parents('.chatWindow').addClass('inputFocused');
            });
            $("body").delegate(".chatWindow .chatInput", "blur", function () {
                $(this).parents('.chatWindow').removeClass('inputFocused');
            });
        }
    }

    function _mToggleChatList() {
        $(".chatWindow.ui-draggable").draggable("destroy");
        $(".chatWindow.ui-resizable").resizable("destroy");
        
        if ($('#chatContainer').hasClass('show')) {
            _mToggleChatOff();
            //$('#chatContainer').removeClass('showChatlist');
        } else {
            _mToggleChatOn();
            $('.chatWindow').removeClass('displayed');
            $('#chatList').addClass('displayed');
            //$('#chatContainer').addClass('showChatlist');
        }
    }

    function _mToggleChatOff() {
        $('.mChatMask, #chatContainer, .mChatToggle').removeClass('show');
        $('.chatWindow').removeClass('displayed active');
    }

    function _mToggleChatOn() {
        if ($('.shield.clickedOn').length) {
            _shieldClickedOff();
        }

        $('.mChatMask, #chatContainer, .mChatToggle').addClass('show');

        //a solution to minimize M chats problem, better solution would be to not do draggable/resizable on my in the first place -farhad
        $(".chatWindow.ui-draggable").draggable("destroy");
        $(".chatWindow.ui-resizable").resizable("destroy");

    }

    function _initUserVoice() {

        // INIT USERVOICE
        UserVoice = window.UserVoice || []; (function () {
            var uv = document.createElement('script');
            uv.type = 'text/javascript';
            uv.async = true;
            uv.src = '//widget.uservoice.com/ELTLteIqJIib238hdW5w.js';
            var s = document.getElementsByTagName('script')[0];
            s.parentNode.insertBefore(uv, s);
        })();

        UserVoice.push(['set', {
            accent_color: '#808283',
            trigger_color: 'white',
            trigger_background_color: 'rgba(46, 49, 51, 0.6)',
            screenshot_enabled: false
        }]);

        UserVoice.push(['identify', {
            id: UVID, //comes from throneroom.aspx
            type: 'ThroneRoom'
        }]);
        UserVoice.push(['addTrigger', '#ideas', { mode: 'smartvote' }]);
        UserVoice.push(['addTrigger', '#supportq', { mode: 'contact' }]);

        $('.uservoice').show();
    }

    function _initOwner() {

        _initCommon();
        _populateDataOwner();


    }

    function _initObserver() {
        _initCommon();
        _populateDataSpectator();
    }

    function _initCommon() {
        _assetsCreate();
        _assetsLayout();

        _assetStage.show();
        _panelTop.fadeIn();
        _panelBottom.fadeIn();
        _panelOne.fadeIn();

        _populateDataCommon();
        _populateShields();
        _initTopStatsTicker();

        _initTourneyMedal();
    }

    function _populateDataCommon() {

        $('.vip').click(_popupVip);

        $('.topStatsTicker', _panelTop).click(_popupGameWideTopStats);

        $('.btnRealmProfileList', _panelOne).click(_popupRealmProfileList);
        $('.btnSpeedRealmStandings', _panelOne).click(function () {
            _populateTouerneyStatsContent();
        });

        $('.btnTrophies', _panelOne).click(function () {
            _popGeneric('Trophies', $('<div>placeholder: Trophies</div>'), 700, 500);
        });
        $('.btnChangelog', _panelOne).click(function () {
            var content = $('<div class="changeLogContent">' +
                    '<div class="logLink"><a href="http://realmofempires.blogspot.ca/2017/04/change-log.html" target="_blank">Detailed Change Log</a></div>'+
                    '<div class="logLink"><a href="http://realmofempires.blogspot.ca/2017/05/what-had-changed-guide-for-veterans.html" target="_blank">Summary of recent new features</a></div>' +
                '</div>');
            _popGeneric('Change Log', content, 300, 200);
        });

        var playerRealmsInfo = _throneUserData.PlayerRealmsInfo;
        var realmPlayer;
        var highestTitleId = 0;
        var highestTitleNameMale = "";
        var highestTitleNameFemale = "";

        for (var pid in playerRealmsInfo) {
            realmPlayer = playerRealmsInfo[pid];
            if (realmPlayer.highestTitleAchieved.ID > highestTitleId) {
                highestTitleId = realmPlayer.highestTitleAchieved.ID;
                highestTitleNameMale = realmPlayer.highestTitleAchieved.Name_Male;
                highestTitleNameFemale = realmPlayer.highestTitleAchieved.Name_Female;
            }
        }

        $('.playerNameBox .highestTitle')
            .attr('data-male', highestTitleNameMale)
            .attr('data-female', highestTitleNameFemale)
            .attr('data-titleID', highestTitleId);

        if (_throneUserData.User.Sex === 0) {   
            $('.playerNameBox .highestTitle').html(highestTitleNameFemale);
        } else {
            $('.playerNameBox .highestTitle').html(highestTitleNameMale);
        }

        //converts the avatars array data into an object, and puts it in ROE.Avatar.list
        ROE.Avatar.convertAvatarsArrayToObject(_throneUserData.AvatarList);
        if (ROE.Avatar.list) {
            var avatarID = _throneUserData.User.AvatarID || 1;
            var avatar = ROE.Avatar.list[avatarID];
            if (avatar) {
                $('.globalPlayerAvatar').css('background-image', 'url(' + avatar.imageUrlS + ')');
            }     
        }

        $('.userXP', _panelTop).html('Global XP: ' + BDA.Utils.formatNum(_throneUserData.User.XP));
        $('.userVacation', _panelTop).html('Total Vacation Days: ' + (_throneUserData.User.BonusVacationDays + 1));

        //$('.launchIntro').click(_launchIntroVideo).show();

        $('.likeTr').attr('data-count', _throneUserData.User.TRLikes).click(_like).show().find('.count').html(_throneUserData.User.TRLikes);

        $('.toggleBarContainer').click(function () {
            $('#chatBarContainer').toggleClass('expanded');
        });

        BDA.Broadcast.subscribe($('#chatBarContainer'), 'contentchanged', function () {
            if (_mBrowser) { return; }
            var chatBarContainer = $('#chatBarContainer');
            var chatBarContainerW = chatBarContainer.width();
            var chatBarSize = $('.chatBar.chatButton', chatBarContainer).not('#chatListBtn').first().width();
            var chatBars = $('.chatBar.chatButton', chatBarContainer).not('#chatListBtn').length;
            if (chatBarSize * chatBars > chatBarContainerW) {
                $('.toggleBarContainer', chatBarContainer).show();
            } else {
                $('.toggleBarContainer', chatBarContainer).hide();
            }
            
        });

    }

    function _populateDataOwner() {

        $('.btnChooseRealms', _panelBottom).click(_popupChooseRealm);

        var globalPlayerName = _throneUserData.User.GlobalPlayerName;
        if (_throneUserData.User.DisableRenameGlobalPlayerName) {
            $('.playerNameBox .globalPlayerName').unbind().html(globalPlayerName || 'anonymous');
        } else {
            $('.playerNameBox .globalPlayerName').html(globalPlayerName || 'anonymous (change)').css('cursor', 'pointer')
                .on('mouseenter', function () {
                    $('<div class="editGPN">').html('Pick your permanent global player name.')
                        .appendTo($(this)).animate({ 'left': '+=15', 'opacity': 1 }, 500);
                })
                .on('mouseleave', function () {
                    $('.editGPN').stop().animate({ 'left': '-=10', 'opacity': 0 }, 300, function () {
                        $(this).remove();
                    });
                })
                .on('click', _popupGPNRename);
        }
        
        $('.highestTitle', _panelTop).css('cursor', 'pointer').click(_toggleSex);
        $('.globalPlayerAvatar').css('cursor', 'pointer').click(_avatarPopup);
        $('.nextVacationXP', _panelTop).html('Next Vacation @ ' + BDA.Utils.formatNum(_throneUserData.User.NextVacationXP) + 'XP');
        $('.nextVacationBar', _panelTop).css('width', ((_throneUserData.User.XP / _throneUserData.User.NextVacationXP) * 100) + '%');
        $('.realmListCompact', _panelBottom).mouseover(function () { $(this).addClass('expanded') });
        $('.realmButtonsContainer', _panelBottom).mouseleave(function () { $('.realmListCompact', _panelBottom).removeClass('expanded') });
        $('.shareTr').click(_shareTrPopup).show();
        //$('.shareRafflePanel').click(_shareRafflePopup).show();   //Raffle is over for now, till next time!    
        $('.shareRafflePanel .tickets').html(_throneUserData.User.TRLikes);

        if (_mBrowser) {
            $('.shareBtn.direct').click(function () { window.prompt("Tap and copy link:", BaseURL + 'throne-' + UVID); });
        } else {
        $('.shareBtn.direct').click(function () { window.prompt("Copy (Ctrl+C) and Share your Throne Room", BaseURL + 'throne-' + UVID); });
        }
        
        //auto pop chooserealm if not suppressed before
        var TR_autoOpenChooseRrealm = localStorage.getItem('TR_autoOpenChooseRrealm');
        if (!TR_autoOpenChooseRrealm || TR_autoOpenChooseRrealm == "true") {
            if (!isMobile) {
                _popupChooseRealm();
            }
            
        }


        BDA.Timer.initTimers();
    }


    function _populateDataSpectator() {

        //remove unneeded elements
        $('.btnChooseRealms', _panelBottom).remove();
        $('.nextVacationProgress', _panelTop).remove();
        $('#lnkBacktoCR').remove();

        var globalPlayerName = _throneUserData.User.GlobalPlayerName;
        $('.playerNameBox .globalPlayerName').html(globalPlayerName || 'anonymous');

        /*
        if (!_fromNewsLetter) {
            $('.btnRealmProfileList').addClass('spectator').html(globalPlayerName + "'s<br>History");
        }
        */

        BDA.Timer.initTimers();
    }

    //VISUALS and ASSETS
    function _assetsCreate() {

        //Layer 0
        _assetStage.append(
            _constructAsset({
                id: 'throneBkg',
                assetObj: _throneAssets.bkg,
                natW: 2310,//2310,
                natH: 1080,//1080,
                natOffestX: 0,
                natOffestY: 0,
                layer: 0
            })
        );
        _assetStage.append(
            _constructAsset({
                id: 'glow',
                assetObj: _throneAssets.glow,
                natW: 1406,
                natH: 942,
                natOffestX: 0,
                natOffestY: 69,
                layer: 0
            })
        );

        //Layer1
        _assetStage.append(
            _constructAsset({
                id: 'columnL1M',
                assetObj: _throneAssets.columnsB,
                natW: 75,
                natH: 788,
                natOffestX: -268,
                natOffestY: 146,
                layer: 1
            })
        );
        _assetStage.append(
            _constructAsset({
                id: 'columnL1S',
                assetObj: _throneAssets.columnsB,
                natW: 75,
                natH: 788,
                natOffestX: -268 * 2,
                natOffestY: 146,
                layer: 1
            })
        );
        _assetStage.append(
            _constructAsset({
                id: 'columnR1M',
                assetObj: _throneAssets.columnsB,
                natW: 75,
                natH: 788,
                natOffestX: 268,
                natOffestY: 146,
                layer: 1,
                flipped: true
            })
        );
        _assetStage.append(
            _constructAsset({
                id: 'columnR1S',
                assetObj: _throneAssets.columnsB,
                natW: 75,
                natH: 788,
                natOffestX: 268 * 2,
                natOffestY: 146,
                layer: 1,
                flipped: true
            })
        );
        _assetStage.append(
            _constructAsset({
                id: 'archL1M',
                assetObj: _throneAssets.columnsArch,
                natW: 549,
                natH: 295,
                natOffestX: -267,
                natOffestY: -393,
                layer: 1
            })
        );
        _assetStage.append(
            _constructAsset({
                id: 'archL1S',
                assetObj: _throneAssets.columnsArch,
                natW: 549,
                natH: 295,
                natOffestX: -267 * 2,
                natOffestY: -393,
                layer: 1
            })
        );
        _assetStage.append(
            _constructAsset({
                id: 'archR1M',
                assetObj: _throneAssets.columnsArch,
                natW: 549,
                natH: 295,
                natOffestX: 267,
                natOffestY: -393,
                layer: 1,
                flipped: true
            })
        );
        _assetStage.append(
            _constructAsset({
                id: 'archR1S',
                assetObj: _throneAssets.columnsArch,
                natW: 549,
                natH: 295,
                natOffestX: 267 * 2,
                natOffestY: -393,
                layer: 1,
                flipped: true
            })
        );
        _assetStage.append(
            _constructAsset({
                id: 'candleStandL1M',
                assetObj: _throneAssets.candlebra,
                natW: 73,
                natH: 315,
                natOffestX: -211,
                natOffestY: 324,
                layer: 1,
            })
        );
        _assetStage.append(
            _constructAsset({
                id: 'candleStandL1S',
                assetObj: _throneAssets.candlebra,
                natW: 73,
                natH: 315,
                natOffestX: -211 * 2.3,
                natOffestY: 324,
                layer: 1,
            })
        );
        _assetStage.append(
            _constructAsset({
                id: 'candleStandR1M',
                assetObj: _throneAssets.candlebra,
                natW: 73,
                natH: 315,
                natOffestX: 211,
                natOffestY: 324,
                layer: 1,
                flipped: true
            })
        );
        _assetStage.append(
            _constructAsset({
                id: 'candleStandR1S',
                assetObj: _throneAssets.candlebra,
                natW: 73,
                natH: 315,
                natOffestX: 211 * 2.3,
                natOffestY: 324,
                layer: 1,
                flipped: true
            })
        );
        _assetStage.append(
            _constructAsset({
                id: 'throne',
                assetObj: _throneAssets.throne,
                natW: 592,
                natH: 447,
                natOffestX: 0,
                natOffestY: 317,
                layer: 1
            })
        );
        _assetStage.append(
            _constructShield({
                id: 'shield4',
                assetObj: _throneAssets.shield,
                natW: 66 * 1.25,
                natH: 94 * 1.25,
                natOffestX: -268,
                natOffestY: 99,
                layer: 1,
                slot: 4
            })
        );
        _assetStage.append(
            _constructShield({
                id: 'shield5',
                assetObj: _throneAssets.shield,
                natW: 66 * 1.25,
                natH: 94 * 1.25,
                natOffestX: 268,
                natOffestY: 99,
                layer: 1,
                slot: 5
            })
        );

        //LAYER 2
        _assetStage.append(
            _constructAsset({
                id: 'columnL2M',
                assetObj: _throneAssets.columnsF,
                natW: 99,
                natH: 995,
                natOffestX: -366,
                natOffestY: 63,
                layer: 2
            })
        );
        _assetStage.append(
            _constructAsset({
                id: 'columnL2S',
                assetObj: _throneAssets.columnsF,
                natW: 99,
                natH: 995,
                natOffestX: -366 * 2,
                natOffestY: 63,
                layer: 2
            })
        );
        _assetStage.append(
            _constructAsset({
                id: 'columnR2M',
                assetObj: _throneAssets.columnsF,
                natW: 99,
                natH: 995,
                natOffestX: 366,
                natOffestY: 63,
                layer: 2,
                flipped: true
            })
        );
        _assetStage.append(
            _constructAsset({
                id: 'columnR2S',
                assetObj: _throneAssets.columnsF,
                natW: 99,
                natH: 995,
                natOffestX: 366 * 2,
                natOffestY: 63,
                layer: 2,
                flipped: true
            })
        );
        _assetStage.append(
            _constructAsset({
                id: 'archL2M',
                assetObj: _throneAssets.columnsArch,
                natW: 698,
                natH: 375,
                natOffestX: -368,
                natOffestY: -601,
                layer: 2
            })
        );
        _assetStage.append(
            _constructAsset({
                id: 'archL2M',
                assetObj: _throneAssets.columnsArch,
                natW: 698,
                natH: 375,
                natOffestX: -368 * 2,
                natOffestY: -601,
                layer: 2
            })
        );
        _assetStage.append(
            _constructAsset({
                id: 'archR2M',
                assetObj: _throneAssets.columnsArch,
                natW: 698,
                natH: 375,
                natOffestX: 368,
                natOffestY: -601,
                layer: 2,
                flipped: true,
            })
        );
        _assetStage.append(
            _constructAsset({
                id: 'archR2S',
                assetObj: _throneAssets.columnsArch,
                natW: 698,
                natH: 375,
                natOffestX: 368 * 2,
                natOffestY: -601,
                layer: 2,
                flipped: true,
            })
        );
        _assetStage.append(
            _constructAsset({
                id: 'candleStandL2M',
                assetObj: _throneAssets.candlebra,
                natW: 103,
                natH: 448,
                natOffestX: -291,
                natOffestY: 339,
                layer: 2
            })
        );
        _assetStage.append(
            _constructAsset({
                id: 'candleStandL2S',
                assetObj: _throneAssets.candlebra,
                natW: 103,
                natH: 448,
                natOffestX: -291 * 2.23,
                natOffestY: 339,
                layer: 2
            })
        );
        _assetStage.append(
            _constructAsset({
                id: 'candleStandR2M',
                assetObj: _throneAssets.candlebra,
                natW: 103,
                natH: 448,
                natOffestX: 291,
                natOffestY: 339,
                layer: 2,
                flipped: true,
            })
        );
        _assetStage.append(
            _constructAsset({
                id: 'candleStandR2S',
                assetObj: _throneAssets.candlebra,
                natW: 103,
                natH: 448,
                natOffestX: 291 * 2.23,
                natOffestY: 339,
                layer: 2,
                flipped: true,
            })
        );
        _assetStage.append(
            _constructShield({
                id: 'shield2',
                assetObj: _throneAssets.shield,
                natW: 85 * 1.25,
                natH: 125 * 1.25,
                natOffestX: -367,
                natOffestY: 14,
                layer: 2,
                slot: 2
            })
        );
        _assetStage.append(
            _constructShield({
                id: 'shield3',
                assetObj: _throneAssets.shield,
                natW: 85 * 1.25,
                natH: 125 * 1.25,
                natOffestX: 367,
                natOffestY: 14,
                layer: 2,
                slot: 3
            })
        );
        _assetStage.append(
            _constructAsset({
                id: 'fog',
                assetObj: _throneAssets.fog,
                natW: 2128,
                natH: 750,
                natOffestX: 0,
                natOffestY: 166,
                layer: 2
            })
        );


        //LAYER 3
        _assetStage.append(
            _constructAsset({
                id: 'columnL3M',
                assetObj: _throneAssets.columnsF,
                natW: 119,
                natH: 1158,
                natOffestX: -519,
                natOffestY: 39,
                layer: 3
            })
        );
        _assetStage.append(
            _constructAsset({
                id: 'columnL3S',
                assetObj: _throneAssets.columnsF,
                natW: 119,
                natH: 1158,
                natOffestX: -519 * 2.1,
                natOffestY: 39,
                layer: 3
            })
        );
        _assetStage.append(
            _constructAsset({
                id: 'columnR3M',
                assetObj: _throneAssets.columnsF,
                natW: 119,
                natH: 1158,
                natOffestX: 519,
                natOffestY: 39,
                layer: 3,
                flipped: true
            })
        );
        _assetStage.append(
            _constructAsset({
                id: 'columnR3S',
                assetObj: _throneAssets.columnsF,
                natW: 119,
                natH: 1158,
                natOffestX: 519 * 2.1,
                natOffestY: 39,
                layer: 3,
                flipped: true
            })
        );
        _assetStage.append(
            _constructAsset({
                id: 'candleStandL3M',
                assetObj: _throneAssets.candlebra,
                natW: 151,
                natH: 654,
                natOffestX: -414,
                natOffestY: 375,
                layer: 3
            })
        );
        _assetStage.append(
            _constructAsset({
                id: 'candleStandR3M',
                assetObj: _throneAssets.candlebra,
                natW: 151,
                natH: 654,
                natOffestX: 414,
                natOffestY: 375,
                layer: 3,
                flipped: true
            })
        );
        _assetStage.append(
            _constructShield({
                id: 'shield0',
                assetObj: _throneAssets.shield,
                natW: 110 * 1.25,
                natH: 158 * 1.25,
                natOffestX: -521,
                natOffestY: -86,
                layer: 3,
                slot: 0
            })
        );
        _assetStage.append(
            _constructShield({
                id: 'shield1',
                assetObj: _throneAssets.shield,
                natW: 110 * 1.25,
                natH: 158 * 1.25,
                natOffestX: 521,
                natOffestY: -86,
                layer: 3,
                slot: 1
            })
        );


    }

    function _constructAsset(settings) {
        var asset = $('<div id="' + settings.id + '" class="throneAsset">')
            .data({
                id: settings.id,
                type: settings.assetObj.type,
                natW: settings.natW,
                natH: settings.natH,
                natOffestX: settings.natOffestX,
                natOffestY: settings.natOffestY,
                layer: settings.layer,
                flipped: settings.flipped,
                scale: settings.scale || 1
            }).css({ 'background-image': "url('" + settings.assetObj.src + "')" });

        if (settings.id == "glow") {
            asset.css({ 'opacity': 0 });
        }

        _stageChildren.push(asset);
        return asset;
    }

    function _constructShield(settings) {
        var asset = $('<div id="' + settings.id + '" class="throneAsset shield">')
            .data({
                id: settings.id,
                type: 'shield',
                natW: settings.natW,
                natH: settings.natH,
                natOffestX: settings.natOffestX,
                natOffestY: settings.natOffestY,
                layer: settings.layer,
                scale: settings.scale || 1,
                slot: settings.slot
            });
        /*.css({
            'background-image': "url('" + settings.assetObj.src + "')",
        });
        **/


        asset.mouseover(function (e) {
            $(this).data('hover', true);
        });

        asset.mouseout(function () {
            $(this).data('hover', false);
        });

        asset.click(function (e) {
            _shieldClickedOn($(this));
        });

        _stageChildren.push(asset);
        return asset;
    }

    function _populateShields() {

        var playerRealmsInfo = _throneUserData.PlayerRealmsInfo;

        var sortedByHighestPtsPids = []; //stores PIDS sorted by HighestVillagePoints_rank
        for (var pid in playerRealmsInfo) {
            sortedByHighestPtsPids.push({
                pid: pid,
                HighestVillagePoints_rank: playerRealmsInfo[pid].HighestVillagePoints_rank
            });
        }
        sortedByHighestPtsPids.sort(function (a, b) {
            var a, b;
            a = a.HighestVillagePoints_rank === 0 ? 1000 : parseInt(a.HighestVillagePoints_rank, 10)
            b = b.HighestVillagePoints_rank === 0 ? 1000 : parseInt(b.HighestVillagePoints_rank, 10)
            return a - b;
        });

        var shields = $('.shield').empty().removeClass('inUse');
        var realmPlayer, shield, shieldContent, currSlot = 0;
        for (var i = 0; i < sortedByHighestPtsPids.length; i++) {
            realmPlayer = playerRealmsInfo[sortedByHighestPtsPids[i].pid];
            if (realmPlayer.displayStatus == 0) { continue; }
            shield = $('#shield' + currSlot).addClass('inUse');
            shield.data('inUse', true);
            shield.data('realmPlayer', realmPlayer);

            shieldContent = $('<div class="shieldContent"></div>');

            shieldContent.append(_makeVisualRating('HA', realmPlayer.PointsAsAttacker_rank));
            shieldContent.append(_explainRating('PointsAsAttacker', realmPlayer.PointsAsAttacker_rank, realmPlayer.PointsAsAttacker));
            shieldContent.append(_makeVisualRating('HD', realmPlayer.PointsAsDefender_rank));
            shieldContent.append(_explainRating('PointsAsDefender', realmPlayer.PointsAsDefender_rank, realmPlayer.PointsAsDefender));
            shieldContent.append(_makeVisualRating('HG', realmPlayer.GovKilledAsDefender_rank));
            shieldContent.append(_explainRating('GovKilledAsDefender', realmPlayer.GovKilledAsDefender_rank, realmPlayer.GovKilledAsDefender));
            shieldContent.append(_makeVisualRating('HP', realmPlayer.HighestVillagePoints_rank));
            shieldContent.append(_explainRating('VillagePoints', realmPlayer.HighestVillagePoints_rank, realmPlayer.HighestVillagePoints));
            shieldContent.append(_makeVisualRating('HN', realmPlayer.HighestNumOfVillages_rank));
            shieldContent.append(_explainRating('HighestNumOfVillages', realmPlayer.HighestNumOfVillages_rank, realmPlayer.HighestNumOfVillages));

            shieldContent.append($('<div class="realmNumber"></div>').html(_makeRealmNumberGraphics(realmPlayer.Realm.ID)));
            shieldContent.append($('<div class="detail realmPlayerName"></div>').html(realmPlayer.Playername));
            shieldContent.append($('<div class="detail playerTitle"></div>').html(_throneUserData.User.Sex === 0 ? realmPlayer.highestTitleAchieved.Name_Female : realmPlayer.highestTitleAchieved.Name_Male));
            shield.html(shieldContent);
            currSlot++;

            if (currSlot > 5) { return; }
        }

        shields.each(function () {
            shield = $(this);
            if (!shield.hasClass('inUse')) {
                shield.empty().data('inUse', false);
                //shield.html('empty :(');
            }
        });

    }

    function _makeRealmNumberGraphics(realmNumber) {
        realmNumber = parseInt(realmNumber);
        if (isNaN(realmNumber)) { return '?'; }

        var realmNumberSplit = ("" + realmNumber).split("");
        var tempHolder = $('<div>');
        var numberDiv;
        for (var i = 0; i < realmNumberSplit.length; i++) {
            numberDiv = $('<div class="numberDiv">').css('background-image', "url('" + _numberImages[realmNumberSplit[i]] + "')");
            tempHolder.append(numberDiv);
        }
        return tempHolder.html();
    }

    function _makeVisualRating(item, rank) {
        var itemSet = $('<div class="itemSet ' + item + '"></div>');

        if (rank == 0) {
            itemSet.addClass('oneStar');
            itemSet.append('<div class="item">');
        } else if (rank == 1) {
            //ace and wings tier 1
            itemSet.addClass('ace1');
            itemSet.append('<div class="aceWing ace1">');
            itemSet.append('<div class="item ' + item + '">');
        } else if (rank <= 10) {
            //ace and wings tier 2
            itemSet.addClass('ace2');
            itemSet.append('<div class="aceWing ace2">');
            itemSet.append('<div class="item ' + item + '">');
        } else if (rank <= 25) {
            //3 items
            itemSet.addClass('threeStar');
            itemSet.append('<div class="item ' + item + '">');
            itemSet.append('<div class="item ' + item + '">');
            itemSet.append('<div class="item ' + item + '">');
        } else if (rank <= 50) {
            //2 items
            itemSet.addClass('twoStar');
            itemSet.append('<div class="item ' + item + '">');
            itemSet.append('<div class="item ' + item + '">');
        } else {
            //1 item
            itemSet.addClass('oneStar');
            itemSet.append('<div class="item ' + item + '">');
        }

        return itemSet;

    }

    function _explainRating(prop, rank, value) {
        var ratingDiv = $('<div class="itemSet detail ratingDiv"></div>');

        var pointsText = "";

        function ordinal_suffix_of(i) {
            var j = i % 10,
                k = i % 100;
            if (j == 1 && k != 11) {
                return i + "st";
            }
            if (j == 2 && k != 12) {
                return i + "nd";
            }
            if (j == 3 && k != 13) {
                return i + "rd";
            }
            return i + "th";
        }


        switch (prop) {
            case 'VillagePoints':
                pointsText = 'Village points';
                break;
            case 'HighestNumOfVillages':
                pointsText = 'Villages';
                break;

            case 'PointsAsAttacker':
                pointsText = 'Attack points';
                break;
            case 'PointsAsDefender':
                pointsText = 'Defence points';
                break;
            case 'GovKilledAsDefender':
                pointsText = 'Govs killed';
                break;
            default:
                pointsText = '???';
                break;
        }

        if (rank === 0) {
            ratingDiv.html('<div class="ordinal">Not Ranked</div><div class="value"> ' + BDA.Utils.formatNum(value) + ' ' + pointsText + '</div>');
        } else {
            ratingDiv.html('<div class="ordinal">' + ordinal_suffix_of(rank) + ' Place</div><div class="value"> ' + BDA.Utils.formatNum(value) + ' ' + pointsText + '</div>');
        }
        return ratingDiv;
    }

    function _shieldClickedOn(shield) {
        if (!shield.hasClass('inUse')) {
            return;
        }

        if (shield.hasClass('clickedOn')) {
            _shieldClickedOff();
            return;
        }
        _pauseMovement = true;
        var shieldClickedMask = $('<div class="shieldClickedMask">');

        var clickMask = $('<div class="clickMask">').click(function () {
            _shieldClickedOff();
        });

        if (_view == 'owner' || _fromNewsLetter) {
            var maskBtnJoinChat = $('<div class="BtnBLg1 fontButton1L maskBtn joinChat"><div class="rid">' + shield.data().realmPlayer.Realm.ID + '</div> Chat<span id="notification' + shield.data().realmPlayer.Realm.ID + '" class="chatNotification" style="display:none;"></span></div>').click(function () {
                _joinRealmChat(shield.data().realmPlayer.Realm.ID, shield.data().realmPlayer.PlayerID);
                _shieldClickedOff();
            });
        }

        var maskBtnRealmProfile = $('<div class="BtnBLg1 fontButton1L maskBtn realmProfile">Realm Profile</div>').click(function () {
            _popupRealmProfile(shield.data().realmPlayer.Realm.ID, shield.data().realmPlayer.PlayerID);
            _shieldClickedOff();
        });

        shieldClickedMask.append(clickMask, maskBtnRealmProfile, maskBtnJoinChat);

        _displayNotifications(shield.data().realmPlayer.Realm.ID, shield.data().realmPlayer.PlayerID, shieldClickedMask);

        _assetStage.append(shieldClickedMask);
        //timeout because css transition wont trigger if appened to page with the class
        setTimeout(function () { shieldClickedMask.addClass('fadeIn'); }, 1);
        shield.addClass('clickedOn nontract');
        //var assetData = shield.data();
        //var assetX = assetData.x;
        //var parllaxMoveAmount = Math.round(_layerParallax[assetData.layer] * _natRatio);
        //var newX = assetX + percPos * parllaxMoveAmount;

        var shieldHW = 531 / 2;
        var shieldHH = 764 / 2;

        if (_mBrowser) {
            var scaleX = .5;
            var scaleY = .5;
            shield.css({
                transform: 'scale(' + scaleX + ',' + scaleY + ')',
                top: ((_assetStage.height() / 2) - shieldHH),
                left: ((_assetStage.width() / 2) - shieldHW),
                'margin-left': 0
            });

            maskBtnRealmProfile.stop().show().animate({ 'opacity': 1 }, 200, function () { });
            if (maskBtnJoinChat) {
                maskBtnJoinChat.stop().show().animate({ 'opacity': 1 }, 150, function () { });
            }

            setTimeout(function () {
                $('.detail', shield).fadeIn();
                shield.removeClass('nontract'); //'nontract' fixes a transform glitch issue
            }, 300);

        } else {
            var scaleX = 1; //(assetData.flipped ? -1 : 1) * assetData.scale;
            var scaleY = 1; //1 * assetData.scale;

            shield.css({
                transform: 'translate(' + ((_assetStage.width() / 2) - shieldHW) + 'px,' + ((_assetStage.height() / 2) - shieldHH) + 'px) scale(' + scaleX + ',' + scaleY + ')'
            });
            setTimeout(function () {
                $('.detail', shield).fadeIn();
                maskBtnRealmProfile.stop().show().animate({ 'margin-left': '280px', 'opacity': 1 }, 200, function () {
                    if (maskBtnJoinChat) {
                        maskBtnJoinChat.stop().show().animate({ 'margin-left': '270px', 'opacity': 1 }, 150, function () { });
                    }
                });
                shield.removeClass('nontract'); //'nontract' fixes a transform glitch issue
            }, 200);
        }

    }

    function _shieldClickedOff() {
        var shield = $('.shield.clickedOn').hide().removeClass('clickedOn nontract').fadeIn();
        $('.detail', shield).stop().hide();
        var shieldClickedMask = $('.shieldClickedMask').removeClass('fadeIn');
        _pauseMovement = false;
        setTimeout(function () { shieldClickedMask.remove(); }, 200);
        if (_mBrowser) { _mShieldLayout(); }
    }

    function _assetsLayout() {

        _windowW = $(window).width();
        _windowH = $(window).height();

        if (_windowW < 900) {
            _viewMode = "compact";
            $('body').addClass('compact');
        } else {
            _viewMode = "normal";
            $('body').removeClass('compact');
        }

        //var naturalW = 2310; //natural stage proportions
        var naturalH = 1080; //natural stage proportions

        //var stageH = 746;
        var stageH = _windowH;
        _natRatio = stageH / naturalH; //the golden ratio, everything is scaled and positioned based on this
        var stageW = _windowW;
        var stageWHalf = stageW / 2;
        var stageHHalf = stageH / 2;

        //main asset container
        _assetStage.css({
            width: stageW,
            height: stageH,
            top: 0,
            //top:100,
            left: '50%',
            'margin-left': -stageWHalf
        });

        //resize and rescale all assets based on ratio
        $(_stageChildren).each(function () {
            var assetElement = $(this);
            var assetData = assetElement.data();
            var newW = Math.round(assetData.natW * _natRatio);
            var newH = Math.round(assetData.natH * _natRatio);
            var newY = Math.round(stageHHalf - (newH / 2) + assetData.natOffestY * _natRatio);
            var newX = Math.round(stageWHalf - (newW / 2) + assetData.natOffestX * _natRatio);
            var scaleX = (assetData.flipped ? -1 : 1) * assetData.scale;
            var scaleY = 1 * assetData.scale;

            assetElement.stop().css({
                width: newW,
                height: newH,
                transform: 'translate(' + newX + 'px,' + newY + 'px) scale(' + scaleX + ',' + scaleY + ')',
            }).data({
                w: newW,
                h: newH,
                y: newY,
                x: newX
            });
        });

        if (_mBrowser) {
            _mShieldLayout();
        }

        var movedCount = 0;
        var panelBottomH = _panelBottom.height();
        $('#chatContainer .chatWindow').each(function () {
            var chatWindow = $(this);
            var chatW = chatWindow.width();
            var chatH = chatWindow.height();
            var chatL = chatWindow.position().left;
            var chatT = chatWindow.position().top;
            if (chatL < 0 || chatT < 0 || chatL + chatW > _windowW || chatT + chatH > _windowH) {
                chatWindow.css({
                    left: _windowW - chatW - (movedCount * 150),
                    top: _windowH - chatH - panelBottomH
                });
                movedCount++;
            }
        });
       
    }

    function _mShieldLayout() {
        $('.throneAsset.shield').css('transform', 'none');
        var sW = 61;
        var sH = 88;
        var margin = (sW / 2);
        var offset = 80;
        var topOffset = (_windowH / 4) - 20;
        $('#shield0').css({ width: sW + 'px', height: sH + 'px', top: (130) + 'px', left: '50%', 'margin-left': -margin + offset });
        $('#shield1').css({ width: sW + 'px', height: sH + 'px', top: (130) + 'px', left: '50%', 'margin-left': -margin - offset });
        $('#shield2').css({ width: sW + 'px', height: sH + 'px', top: (130 + topOffset) + 'px', left: '50%', 'margin-left': -margin + offset });
        $('#shield3').css({ width: sW + 'px', height: sH + 'px', top: (130 + topOffset) + 'px', left: '50%', 'margin-left': -margin - offset });
        $('#shield4').css({ width: sW + 'px', height: sH + 'px', top: (130 + topOffset * 2) + 'px', left: '50%', 'margin-left': -margin + offset });
        $('#shield5').css({ width: sW + 'px', height: sH + 'px', top: (130 + topOffset * 2) + 'px', left: '50%', 'margin-left': -margin - offset });
    }

    function _mouseMovement(xPerc, yPerc) {

        if (_pauseMovement) { return; }

        var percPos = (xPerc - .5) * 2; //camera position percentage

        $(_stageChildren).each(function () {
            var assetElement = $(this);
            var assetData = assetElement.data();
            var assetX = assetData.x;
            var parllaxMoveAmount = Math.round(_layerParallax[assetData.layer] * _natRatio);
            var newX = assetX + percPos * parllaxMoveAmount;
            var scaleX = (assetData.flipped ? -1 : 1) * assetData.scale;
            var scaleY = 1 * assetData.scale;

            //for shield hover, scales up, in combination with css transitions
            if (assetData.inUse && assetData.hover) {
                scaleX *= 1.3;
                scaleY *= 1.3;
            }

            //the window glow behaves a bit different 
            if (assetData.id == "glow") {
                newX = assetX + (percPos * 1) * (parllaxMoveAmount * 10);
                assetElement.css({
                    transform: 'translate(' + newX + 'px,' + assetData.y + 'px) scale(' + scaleX + ',' + scaleY + ')',
                    opacity: (.8 * Math.abs(percPos))
                });
            } else {
                assetElement.css({
                    transform: 'translate(' + newX + 'px,' + assetData.y + 'px) scale(' + scaleX + ',' + scaleY + ')',
                });
            }

        });
    }


    //GlobalPlayerName functions
    function _popupGPNRename() {
        var title = "SET GLOBAL PLAYER NAME";
        var content = $('<div class="gpnRenameContent">');
        var info = $('<div class="info">').html('Please pick something tasteful that you want to keep. You can not change it again.');
        var input = $('<input id="globalPlayerNameInput" type="text" maxlength="16" />');
        var ok = $('<button id="globalPlayerNameOk" >OK</button>').click(_setGlobalPlayerName);
        var errorBox = $('<div class="nameErrorBox">');
        content.append(info, input, ok, errorBox);
        _popGeneric(title, content, 360);
    }

    function _setGlobalPlayerName() {
        var newName = $('#globalPlayerNameInput').val();
        $('.nameErrorBox', $('#genericDialog')).empty();
        _busy('Saving...', 5000, $('#genericDialog') );
        ROE.Api.call_tr_renameuser(newName, _setGlobalPlayerNameCallback);
    }

    function _setGlobalPlayerNameCallback(data) {

        _free($('#genericDialog'));

        var nameChanged = data.nameChanged;
        var ifNameNotChangedReasonCode = data.ifNameNotChangedReasonCode;
        var newName = data.newName;
        var errorMsg = "";

        if (nameChanged) {
            $('#genericDialog').dialog('close');
            $('.globalPlayerName', _panelTop).unbind().css('cursor', 'default').html(newName);
            ROE.Chat2.setName(newName);

            // loginToChat(newName);
        } else {
            if (ifNameNotChangedReasonCode == 1) {
                errorMsg = "Please don't use inappropriate words.";
            } else if (ifNameNotChangedReasonCode == 2) {
                errorMsg = "Name is already taken, please try a different one.";
            } else if (ifNameNotChangedReasonCode == 3) {
                errorMsg = "Rename not allowed.";
            } else if (ifNameNotChangedReasonCode == 4 || ifNameNotChangedReasonCode == 6) {
                errorMsg = "Name has to be minimum 3 to max 15 letters";
            } else if (ifNameNotChangedReasonCode == 5) {
                errorMsg = "Name can not have special characters. Only A-Z, 0-9, . and _ allowed.";
            } else {
                errorMsg = "Name Error, Please try a different one.";
            }
            $('.nameErrorBox', $('#genericDialog')).html(errorMsg);
        }


    }

    //Realm Profile functions
    function _popupRealmProfileList() {
        var title = "Realms Played";
        var content = $('<div class="realmProfileList">');
        var playerRealmsInfo = _throneUserData.PlayerRealmsInfo;
        var sortedPIDsByRealmID = _throneUserData.sortedPIDsByRealmID;
        var realmPlayer, realmBox;
        var buttonPanel;

            for (var i = 0; i < sortedPIDsByRealmID.length; i++) {

                realmPlayer = playerRealmsInfo[sortedPIDsByRealmID[i].pid];
                realmBox = $('<div class="realmBox" data-rid="' + realmPlayer.Realm.ID + '" data-pid="' + realmPlayer.PlayerID + '" >');
                realmBox.append('<div class="label">' + realmPlayer.Realm.Name + ': ' + realmPlayer.Realm.Desc + '</div>');
                realmBox.append('<div class="playerName">Player: ' + realmPlayer.Playername + '</div>');
                realmBox.append('<div class="VillagePoints"> #Villages: ' + realmPlayer.HighestNumOfVillages + ' Village Pts: ' + BDA.Utils.formatNum(realmPlayer.HighestVillagePoints) + '</div>');

                buttonPanel = $('<div class="buttonPanel">');
                realmBox.append(buttonPanel);

                content.append(realmBox);

                if (_view == 'owner' || _fromNewsLetter) {

                    var displaySwitch = $('<div class="BtnBLg1 fontButton1L btn displayStatus helpTooltip" data-tooltipid="displayStatus" data-pid="' + realmPlayer.PlayerID + '" data-displayStatus="' + (realmPlayer.displayStatus) + '" >Display: ' + (realmPlayer.displayStatus === 1 ? 'ON' : 'OFF') + '</div>')
                        .click(_realmDisplayToggle);
                    buttonPanel.append(displaySwitch);

                    var joinRealmChat = $('<div class="BtnBLg1 fontButton1L btn joinRealmChat helpTooltip" data-tooltipid="joinRealmChat" data-pid="' + realmPlayer.PlayerID + '" data-rid="' + realmPlayer.Realm.ID + '" >Chat<span id="notification' + realmPlayer.Realm.ID + '" class="chatNotification" style="display:none;"></span></div>').click(function () {
                        _joinRealmChat($(this).attr('data-rid'), $(this).attr('data-pid'));
                        $('#genericDialog').dialog('close');
                    });
                    buttonPanel.append(joinRealmChat);
                    //     if (_loggedIn) {
                    _displayNotifications(realmPlayer.Realm.ID, realmPlayer.PlayerID, content);
                    //      }
                }
                var leaderBoardBtn = $('<div class="BtnBLg1 fontButton1L btn leaderBoardBtn helpTooltip" data-tooltipid="leaderBoardBtn" data-rid="' + realmPlayer.Realm.ID + '" data-pid="' + realmPlayer.PlayerID + '">Profile</div>').click(function () {
                    _popupRealmProfile($(this).attr('data-rid'), $(this).attr('data-pid'));
                });
                buttonPanel.append(leaderBoardBtn);
            }

        _popGeneric(title, content, 700, 500);
    }

    function _joinRealmChat(realmID, playerID) {
        if (!_loggedIn) {
            _promptForLogin();
            return;
        }
        ROE.Chat2.joinRealm(realmID, playerID);
        if (_mBrowser) {
            _mToggleChatOn();
        }
    }

    function _displayNotifications(realmId, playerId, container) {
        ROE.Chat2.getNotifications(realmId, playerId, function (results) {
            if (results.notifs > 0) {
                container.find('#notification' + results.realmId).text(results.notifs).show();
            } else {
                container.find('#notification' + results.realmId).hide();
            }
        });
        //chat.server.getNotifications(realmId).done(function (results) {
        //    if (results.notifs > 0) {
        //        container.find('#notification' + results.realmId).text(results.notifs).show();
        //    } else {
        //        container.find('#notification' + results.realmId).hide();
        //    }
        //});
    }

    function _realmDisplayToggle() {

        if (!_loggedIn) {
            _promptForLogin();
            return;
        }

        var btn = $(this);
        var pid = btn.attr('data-pid');
        var currStatus = parseInt(btn.attr('data-displayStatus'));
        var setStatusTo = currStatus === 1 ? 0 : 1;

        _busy('Saving display setting', 5000, $('#genericDialog') );
        ROE.Api.call_tr_saveplayerlistsetting(pid, setStatusTo, _savePlayerListSettingCallback);
    }

    function _savePlayerListSettingCallback(data) {
        _free($('#genericDialog'));
        var listSettings = data.playerListSetting;
        for (var pid in listSettings) {
            //because there can be more PIDs in the callback, than there are realms in _throneUserData.PlayerRealmsInfo
            if (_throneUserData.PlayerRealmsInfo[pid]) {
                $('.realmBox[data-pid="' + pid + '"]').find('.displayStatus').attr('data-displayStatus', listSettings[pid].displaySetting)
                    .html('Display: ' + (listSettings[pid].displaySetting === 1 ? 'ON' : 'OFF'));
                _throneUserData.PlayerRealmsInfo[pid].displayStatus = listSettings[pid].displaySetting;
            }
        }
        _populateShields();
    }

    //WORK IN PROGRESS
    function _popupRealmProfile(rid, pid) {

        var content = $('<div class="realmProfileContent">');
        var realmPlayer = _throneUserData.PlayerRealmsInfo[pid];
        var realmBox = $('<div class="realmBox" data-rid="' + rid + '" data-pid="' + pid + '" >');
        var leftBox = $('<div class="leftBox">');
        var rightBox = $('<div class="rightBox">');

        var realmCloseText = realmPlayer.Realm.ClosingOn > (new Date()) ? "Realm is currently active." : "Closed: " + (new Date(realmPlayer.Realm.ClosingOn).toDateString());

        leftBox.append('<div class="label">' + realmPlayer.Realm.Name + ': ' + realmPlayer.Realm.Desc + '</div>');
        leftBox.append('<div class="playerName">Player: ' + realmPlayer.Playername + '</div>');
        leftBox.append('<div class="dateOpen">Opened: ' + (new Date(realmPlayer.Realm.OpenOn).toDateString()) + '</div>');
        leftBox.append('<div class="dateClose">' + realmCloseText + '</div>');
        rightBox.append('<div class="VillagePoints">Village Points: ' + BDA.Utils.formatNum(realmPlayer.HighestVillagePoints) + '</div>');
        rightBox.append('<div class="highestNumOfVillages"># Villages: ' + BDA.Utils.formatNum(realmPlayer.HighestNumOfVillages) + '</div>');
        rightBox.append('<div class="attackPoints">Attack Points: ' + BDA.Utils.formatNum(realmPlayer.PointsAsAttacker) + '</div>');
        rightBox.append('<div class="defencePoints">Defense Points: ' + BDA.Utils.formatNum(realmPlayer.PointsAsDefender) + '</div>');
        rightBox.append('<div class="govKillsAsDefender">Govs Killed: ' + BDA.Utils.formatNum(realmPlayer.GovKilledAsDefender) + '</div>');
        rightBox.append('<div class="highestTitleAchieved">Highest Title: ' + realmPlayer.highestTitleAchieved.Name_Male + '</div>');

        var leaderBoard = $('<div class="leaderBoard" >').html('<div class="label">Realm Highest Player Points</div><div class="content"></div>');

        realmBox.append(leftBox, rightBox);
        content.append(realmBox);
        content.append(leaderBoard);

        $('#realmProfile').html(content).dialog('open');
        _busy('Loading Leaderboard...', 5000, $('#realmProfile') );
        ROE.Api.call_tr_realmleaderboard(rid, _realmLeaderBoardCallback);
    }

    function _realmLeaderBoardCallback(data) {
        //console.log(data);

        var realmProfileDialog = $('#realmProfile');
        _free(realmProfileDialog);
        if (parseInt($('.realmBox', realmProfileDialog).attr('data-rid')) != data.RealmID) {
            return;
        }
        var playerNames = [];
        var currPlayer;
        for (var key in _throneUserData.PlayerRealmsInfo) {
            currPlayer = _throneUserData.PlayerRealmsInfo[key];
            if (currPlayer.Realm.ID == data.RealmID) { playerNames.push(currPlayer.Playername); }
        }
        var leaderTable = $('<table class="leaderTable"><tbody></tbody></table>');
        leaderTable.data({
            iStart: 0,
            iEnd: 100,
            leaderBoard: data.LeaderBoard,
            rid: data.RealmID,
            playerNames: playerNames
        });

        var headerRow = '<tr class="headerRow">' +
				    '<th>Player</th>' +
                    '<th class="clanNameContainer">Clan</th>' +
                    '<th class="num"># Of Villages</th>' +
                    '<th class="num">Village Pts</th>' +
                    '<th class="num">Attack Pts</th>' +
                    '<th class="num">Defense Pts</th>' +
                    '<th class="num">Govs Killed</th>';

        //add chat column for logged in users in their own throneroom
        if (_loggedIn && _view == 'owner') {
            headerRow += '<th class="chat">Chat</th></tr>';
            leaderTable.append(headerRow);
            _createOwnerLeaderTable(leaderTable);
        } else {
            headerRow += '</tr>';
            leaderTable.append(headerRow);
            _createLeaderTable(leaderTable);
        }

        $('.leaderBoard .content', realmProfileDialog).append(leaderTable);
        $('.leaderBoard .content', realmProfileDialog).scroll(function () {
            var content = $(this);
            if (content.hasClass('nomore')) { return; }
            var scrollBottomMax = content[0].scrollHeight - content.height();
            if (content.scrollTop() + 1 >= scrollBottomMax) {
                var iStart = leaderTable.data().iStart + 100;
                var iEnd = leaderTable.data().iEnd + 100;
                leaderTable.data({
                    iStart: iStart,
                    iEnd: iEnd
                });
                if (_loggedIn && _view == 'owner') {
                    _createOwnerLeaderTable(leaderTable);
                } else {
                    _createLeaderTable(leaderTable);
                }
            }
        });

        var searchBox = $('<input type="text" class="searchBox" placeholder="Search Names...">');
        searchBox.on('input', function () { //paste
            var val = $(this).val();
            leaderTable.find('.searched').remove();
            if (val && val.length) {
                val = val.toLowerCase();
                var tailoredBoard = [];
                var leaderBoard = data.LeaderBoard;
                var l = leaderBoard.length;
                var rowData, playerName;
                for (var i = 0; i < l; i++) {
                    rowData = leaderBoard[i];
                    playerName = rowData.pN.toLowerCase();
                    if (playerName.indexOf(val) > -1) {
                        tailoredBoard.push(rowData);
                    }
                }

                if (_loggedIn && _view == 'owner') {
                    _createOwnerLeaderTable(leaderTable, tailoredBoard);
                } else {
                    _createLeaderTable(leaderTable, tailoredBoard);
                }

                $('.leaderBoard .content', realmProfileDialog).addClass('nomore');
                $('.leaderBoard .content .main', realmProfileDialog).hide();
            } else {
                $('.leaderBoard .content', realmProfileDialog).removeClass('nomore');
                $('.leaderBoard .content .main', realmProfileDialog).show();
            }

        });
        $('.leaderBoard', realmProfileDialog).append(searchBox);


    }

    function _createLeaderTable(leaderTable, tailoredBoard) {

        if (tailoredBoard) {
            var leaderBoard = tailoredBoard;
            var startAt = 0;
            var endAt = Math.min(tailoredBoard.length, 500);
            var rowClass = "searched";
        } else {
            var leaderBoard = leaderTable.data().leaderBoard;
            var startAt = leaderTable.data().iStart;
            var endAt = Math.min(leaderBoard.length, leaderTable.data().iEnd);
            var rowClass = "main";
        }

        var rowData;
        for (var i = startAt; i < endAt; i++) {
            rowData = leaderBoard[i];
            leaderTable.append(
                '<tr class="dataRow ' + rowClass + '" data-pn="' + rowData.pN + '">' +
                    '<td class="playerName"><span>' + rowData.pTN + '</span> ' + rowData.pN + '</td>' +
                    '<td class="clanNameContainer">' + (rowData.pCN || " - ") + '</td>' +
                    '<td class="num">' + BDA.Utils.formatNum(rowData.hNumV) + '</td>' +
                    '<td class="num">' + BDA.Utils.formatNum(rowData.hVPts) + '</td>' +
                    '<td class="num">' + BDA.Utils.formatNum(rowData.pAtt) + '</td>' +
                    '<td class="num">' + BDA.Utils.formatNum(rowData.pDef) + '</td>' +
                    '<td class="num">' + BDA.Utils.formatNum(rowData.govKil) + '</td>' +
                '</tr>');
        }
    }

    function _createOwnerLeaderTable(leaderTable, tailoredBoard) {

        if (tailoredBoard) {
            var leaderBoard = tailoredBoard;
            var startAt = 0;
            var endAt = Math.min(tailoredBoard.length, 500);
            var rowClass = "searched";
        } else {
            var leaderBoard = leaderTable.data().leaderBoard;
            var startAt = leaderTable.data().iStart;
            var endAt = Math.min(leaderBoard.length, leaderTable.data().iEnd);
            var rowClass = "main";
        }

        var ownerCurrentPid = $('.realmProfileContent .realmBox').attr('data-pid');
        var realmID = leaderTable.data().rid;
        var playerNames = leaderTable.data().playerNames;
        var rowData;
        var rowElement;
        for (var i = startAt; i < endAt; i++) {
            rowData = leaderBoard[i];
            rowElement = $('<tr class="dataRow ' + rowClass + '" data-pid="' + rowData.pID + '"  data-pn="' + rowData.pN + '">' +
                    '<td class="playerName"><span>' + rowData.pTN + '</span> ' + rowData.pN + '</td>' +
                    '<td class="clanNameContainer">' + (rowData.pCN || " - ") + '</td>' +
                    '<td class="num">' + BDA.Utils.formatNum(rowData.hNumV) + '</td>' +
                    '<td class="num">' + BDA.Utils.formatNum(rowData.hVPts) + '</td>' +
                    '<td class="num">' + BDA.Utils.formatNum(rowData.pAtt) + '</td>' +
                    '<td class="num">' + BDA.Utils.formatNum(rowData.pDef) + '</td>' +
                    '<td class="num">' + BDA.Utils.formatNum(rowData.govKil) + '</td>' +
                    '<td><div class="chat">Chat</div></td>' +
                '</tr>');

            if ($.grep(playerNames, function (name) { return name == rowData.pN; }).length == 0) { //do this only if the playername is not one of this user's players
                rowElement.click(function (e) {
                    e.stopPropagation();
                    _leaderboardContextMenu(e, realmID, ownerCurrentPid, $(this).attr('data-pid'));
                })

                rowElement.find('.chat').remove();
                /*
                .find('.chat').click(function (e) {
                    e.stopPropagation();
                    _mToggleChatOff();
                    //ROE.Chat2.oneOnOneChat(realmID, ownerCurrentPid, $(this).parent().parent().attr('data-pid'));
                    $('#realmProfile').dialog('close');
                    $('#genericDialog').dialog('close');
                    _mToggleChatOn();
                });*/
            } else {
                rowElement.addClass('self').find('.chat').hide();
            }
            leaderTable.append(rowElement);
        }
    }

    function _leaderboardContextMenu(e, realmID, id, otherId) {
        $('#leaderboardUserMenu').css({ 'left': e.pageX, 'top': e.pageY }).fadeIn(100, function () {
            $(document).bind('click', _hideUserContextMenu);
        });

        $('#oneOnOneChat').remove();
        /*
        $('#oneOnOneChat').unbind('click').click(function () {
            _mToggleChatOff();
            //ROE.Chat2.oneOnOneChat(realmID, id, otherId);
            $('#realmProfile').dialog('close');
            $('#genericDialog').dialog('close');
            $(this).unbind('click');
            _mToggleChatOn();
        });
        */

        if (otherId && id) {
            $('#optionViewThrone').unbind('click').click(function () {
                if (_mBrowser) {
                    ROE.Frame.showIframeOpenDialog($('#genericDialog'), 'throneroom.aspx?rid=' + realmID + '&pid=' + otherId + '&viewerpid=' + id, 'Throne Room');
                } else {
                    window['throneroomWindow'] = window.open('throneroom.aspx?rid=' + realmID + '&pid=' + otherId + '&viewerpid=' + id, 'throneroom');
                }
                $(this).unbind('click');
            }).show();
        } else {
            $('#optionViewThrone').hide();
        }

    }

    function _hideUserContextMenu() {
        $('#leaderboardUserMenu').hide(100);
        $(document).unbind('click', _hideUserContextMenu);
    }

    //top stats
    function _initTopStatsTicker() {

        var title = "My Game Wide Top Stats";
        var container = $('.topStatsTicker').attr('data-tick', 0);
        var gameWideTopStats = _throneUserData.GameWideTopStats;
        var statBox, realmPlayer, pid, value;

        for (var prop in gameWideTopStats) {
            value = gameWideTopStats[prop].Value;
            pid = gameWideTopStats[prop].AchievedByPlayerID;
            realmPlayer = _throneUserData.PlayerRealmsInfo[pid];
            statBox = $('<div class="statBox ' + prop + '">').hide().html(_pointsPropToText(prop) + ': ' + BDA.Utils.formatNum(value));
            container.append(statBox);
        }

        $($('.statBox')[0], container).fadeIn();
        setInterval(function () {
            var tick = parseInt(container.attr('data-tick')) + 1;
            if (tick > 4) { tick = 0; }
            $('.statBox', container).fadeOut();
            $($('.statBox')[tick], container).fadeIn();
            container.attr('data-tick', tick);
        }, 3000);
    }

    //change the icon of the tourney medals popup launcher to reflect your best
    function _initTourneyMedal() {
        var icon = $('#panelOne .btnSpeedRealmStandings');
        if (icon.length) {

            //non participation defaults
            var rankClass = "nonRanked";
            var rank = "RX/RS";

            //if participated, get the best rank
            var bestRankObject = _getBestTourneyRank();
            if (bestRankObject.rank) {
                rankClass = _rankClass(bestRankObject.type, bestRankObject.rank);
                rank = bestRankObject.rank;
            }   
            icon.addClass(rankClass).html(rank);
        }
    }

    function _pointsPropToText(prop) {
        var outputString = "";
        switch (prop) {
            case 'HighestNumOfVillages':
                outputString = 'Highest number of villages';
                break;
            case 'VillagePoints':
                outputString = 'Highest village points';
                break;
            case 'PointsAsAttacker':
                outputString = 'Highest attack points';
                break;
            case 'PointsAsDefender':
                outputString = 'Highest defence points';
                break;
            case 'GovKilledAsDefender':
                outputString = 'Most govs killed in defence';
                break;
            default:
                outputString = '???';
                break;
        }
        return outputString;
    }

    function _popupGameWideTopStats() {
        var title = "Game Wide Top Stats";
        var content = $('<div class="gameWideTopStats">');
        var gameWideTopStats = _throneUserData.GameWideTopStats;
        var statBox, realmPlayer, pid, value;

        for (var prop in gameWideTopStats) {
            value = gameWideTopStats[prop].Value;
            pid = gameWideTopStats[prop].AchievedByPlayerID;
            realmPlayer = _throneUserData.PlayerRealmsInfo[pid];
            statBox = $('<div class="statBox" data-rid="' + realmPlayer.Realm.ID + '" data-pid="' + realmPlayer.PlayerID + '" >');
            statBox.html(_pointsPropToText(prop) + ' ' + BDA.Utils.formatNum(value) + ' in realm ' + realmPlayer.Realm.ID);
            content.append(statBox);
        }

        _popGeneric(title, content, 600, 350);
    }

    //Choose Realm popup
    function _popupChooseRealm() {
        var title = "Choose Realm";
        var content = _chooseRealmsBox.clone().show();

        var dialog = _popDisposable({
            ID: 'chooseRealmDialog',
            title: 'Choose Realm',
            content: content,
            width: 500,
            height: 600,
            modal: false,
            contentCustomClass: 'chooseRealmDialogContent'
        });

        var dialogtitlebar = $('.ui-dialog-titlebar', dialog.parent());

        //if not mobile, add auto popup checkbox
        if (!isMobile) {
            var autoOpenCheckbox = $('<input class="autoOpenCheckbox" type="checkbox" />');
            var TR_autoOpenChooseRrealm = localStorage.getItem('TR_autoOpenChooseRrealm');
            autoOpenCheckbox.prop('checked', !TR_autoOpenChooseRrealm || TR_autoOpenChooseRrealm == "true");
            autoOpenCheckbox.click(function () {
                localStorage.setItem('TR_autoOpenChooseRrealm', this.checked)
            });
            var openOnStart = $('<div class="openOnStart">auto open on startup </div>').append(autoOpenCheckbox);
            openOnStart.appendTo(dialogtitlebar);
        }


        //if any ended realms, add ended realm toggle button
        if ($('.realmEnded').length) {

            var endedToggle = $('<div class="endedToggle helpTooltip" data-tooltipid="endedToggle" >').appendTo(dialogtitlebar);
            endedToggle.click(function () {
                if ($(this).hasClass('showing')) {
                    _hideEndedRealms();
                } else {
                    _showEndedRealms();
                }
            });

            var realmEndedDisplay = localStorage.getItem('realmEndedDisplay');
            if (realmEndedDisplay && realmEndedDisplay == "hiding") {
                _hideEndedRealms();
            } else {
                _showEndedRealms();
            }

        }

    }

    function _hideEndedRealms() {
        $('.endedToggle').removeClass('showing').addClass('hiding');
        $('.realmEnded').hide();
        localStorage.setItem('realmEndedDisplay', "hiding");
    }

    function _showEndedRealms() {
        $('.endedToggle').removeClass('hiding').addClass('showing');
        $('.realmEnded').show();
        localStorage.setItem('realmEndedDisplay', "showing");
    }




    function _busy(overrideText, timeOutForTooLongMessage, container) {
        ROE.Frame.base_busy(overrideText, timeOutForTooLongMessage, container);
    }

    function _free(container) {
        ROE.Frame.base_free(container);
    }


    function _showTooltip(element) {
        
        //fixes zombie tooltips issue -farhad
        if (!$.contains(window.document, element[0])) {          
            return;
        }

        var text = _throneTooltips[element.attr('data-toolTipID')];
        var toolTipBox = $('<div>').addClass('toolTipBox').html(text).hide().appendTo('body').position({
            my: "center top",
            at: "center bottom+5",
            of: element,
            collision: "flipfit"
        }).fadeIn();
        toolTipBox.mouseover(function () { $(this).remove(); });
    }

    function _launchIntroVideo() {
        var title = "Throne Room How-To Video";
        var content = '<iframe width="560" height="315" src="https://www.youtube.com/embed/53RRm6EzMEM" frameborder="0" allowfullscreen></iframe>';
        _popGeneric(title, content, 800, 500);
    }

    function _shareTrPopup() {
        $('#shareDialog').dialog('open');
    }

    function _shareRafflePopup() {
        var title = "Throne Room Raffle";
        var content = $('<div class="shareRaffle">');
        content.append(
                '<div class="panel header">Get \'Likes\' to Win!</div>' +
                '<div class="panel">' +
                    '<div>Each time somebody \'Likes\' your Throne Room you get a raffle ticket for the upcoming prize draw. Share your Throne Room to get \'Likes\'. You currently have: ' + (_throneUserData.User.TRLikes) + ' raffle tickets! </div>' +
                    '<div class="fontSilverFrLCmed shareTr" >Share</div>' +
                '</div>' +
                '<div class="panel">' +
                    '<div class="prizeheader">We are giving out <span style="text-decoration: line-through;">20</span> 40 prizes!</div>' +
                    '<div class="prize kn">500 Knights X 10</div>' +
                    '<div class="prize lc">500 Light Cavalry X 10</div>' +
                    '<div class="prize if">500 Infantry X 10</div>' +
                    '<div class="prize sv">500 Silver X 10</div>' +
                '</div>' +
                '<div class="panel">Start collecting those \'likes\', the raffle ends in: <div class="countdown" data-finisheson="1448038800000" data-refreshcall="$(this).remove();"></div></div>'
                );
        _popGeneric(title, content, 600, 500);

        $('.shareTr', content).click(_shareTrPopup);
    }

    function _like() {
        var likeBtn = $('.likeTr');
        if ($('.containerBusyMask', likeBtn).length) {
            return;
        }
        _busy('...', 5000, likeBtn);
        var uid = _urlGetParams.uid || UVID;
        var pid = _urlGetParams.pid || null;
        ROE.Api.call_tr_setlike(uid, pid, _likeCallback);
    }

    function _likeCallback(data) {
        var likeBtn = $('.likeTr');
        _free(likeBtn);
        var likesCount = parseInt(likeBtn.attr('data-count'));
        likeBtn.attr('data-count', data.likes);
        $('.count', likeBtn).html(data.likes);
        var msg = $('<div class="msg">');
        if (data.likes > likesCount) {
            msg.addClass('new').html('+1');
        } else {
            msg.html('already liked');
        }
        likeBtn.append(msg);
        msg.stop().animate({ 'top': '-=50', 'opacity': 0 }, 1500, function () {
            $(this).remove();
        });
    }

    function _setUserAvatarID(avatarID) {
        if (!avatarID) { return; }
        _busy(' ', 5000, $('.globalPlayerAvatar'));
        ROE.Api.call_user_setavatarid(avatarID, _setUserAvatarIDCallback);
        ROE.Avatar.lastPickedID = null;
    }

    function _setUserAvatarIDCallback(data) {
        _free($('.globalPlayerAvatar'));
        if (data.result === 0) {
            var avatarID = data.AvatarID;
            var avatar = ROE.Avatar.list[avatarID];
            $('.globalPlayerAvatar').css('background-image', 'url(' + avatar.imageUrlS + ')');
            _throneUserData.User.AvatarID = avatarID;
            ROE.Avatar.lastPickedID = avatarID;
        } else {
            //avatar pick denied
        }
    }

    function _toggleSex() {
        if (_throneUserData.User.Sex === 1) {
            _setUserSex(0);
        } else {
            _setUserSex(1);
        }
    }

    function _setUserSex(sex) {
        _busy('Updating ...', 5000, _panelTop );
        ROE.Api.call_user_setsex(sex, _setUserSexCallback);
    }

    function _setUserSexCallback(data) {
        _free(_panelTop);
        if (data.result == 0) {
            _throneUserData.User.Sex = data.Sex;
            var element = $('.playerNameBox .highestTitle');
            if (_throneUserData.User.Sex === 0) {
                element.html(element.attr('data-female'));
            } else {
                element.html(element.attr('data-male'));
            }
            _populateShields();

            //send a notif about teh sex change
            $('.quicknotif').remove();
            var msg = data.Sex === 0 ? 'User sex set to Female.' : 'User sex set to Male.';
            $("body").append('<div class="quicknotif fontSilverFrLCmed ">' + msg + '</div>');
            setTimeout(function () {
                $('.quicknotif').fadeOut();
            }, 2000);

        }
    }

    function _avatarPopup() {
        var title = "Pick your Avatar";
        var content = $('<div class="avatarPopup">');
        ROE.Avatar.init(content, _throneUserData.User.AvatarID);
        _popGeneric(title, content, 630, 590);

        //add a binding to generic dialogs close button, to set the avatar
        //pretty crude way to do it, will improve it later, avatar picker will need its own dialog
        var closeBtn = $('.ui-dialog[aria-describedby="genericDialog"]').find('.ui-dialog-titlebar-close');
        if (!closeBtn.hasClass('avatarClose')) {
            closeBtn.addClass('avatarClose').click(function () {
                _setUserAvatarID(ROE.Avatar.lastPickedID);
            });
        }

    }


    function _populateTouerneyStatsContent(content) {

        if (!_throneUserData.TournamentRStats) { return; }

        console.log('_throneUserData.TournamentRStats', _throneUserData.TournamentRStats);

        var content = $('<div class="touerneyStatsContent"></div>');
        var bestFinishesContainer = $('<div class="bestFinishesContainer"></div>');
        var header = $('<div class="header"></div>').html('Realm X and Realm S personal best place finishes:').appendTo(bestFinishesContainer);

        //RX best
        var rxBest = _throneUserData.TournamentRStats.RXBestResults;
        var type = 'RX';
        for (var i = 0; i < rxBest.length; i++) {
            var finishBox = $('<div class="finishBox ' + type + ' ' + _rankClass(type, rxBest[i].rankByNumOfVillages) + '">' +
                    '<div class="info"><div class="type">type: <span>' + type + '</span></div><div class="count">count: ' + rxBest[i].numTimesRankAchieved + '</div></div>' +
                    '<div class="rank"><div class="label">Rank</div><div class="number">' + rxBest[i].rankByNumOfVillages + '</div></div>' +
                +'</div>');
            finishBox.appendTo(bestFinishesContainer);
        }

        //RS best
        var rsBest = _throneUserData.TournamentRStats.RSBestResults;
        var type = 'RS';
        for (var i = 0; i < rsBest.length; i++) {
            var finishBox = $('<div class="finishBox ' + type + ' ' + _rankClass(type, rsBest[i].rankByNumOfVillages) + '">' +
                    '<div class="info"><div class="type">type: <span>' + type + '</span></div><div class="count">count: ' + rxBest[i].numTimesRankAchieved + '</div></div>' +
                    '<div class="rank"><div class="label">Rank</div><div class="number">' + rsBest[i].rankByNumOfVillages + '</div></div>' +
                +'</div>');
            finishBox.appendTo(bestFinishesContainer);
        }


        //season standings tabs
        var tournamentSeasonsContainer = $('<div class="tournamentSeasonsContainer"><div class="label">Tournament Season Standings</div></div>');
        var seasonsTabs = $('<div class="seasonsTabs" ></div>');
        var seasonTab = null;
        for (var seasonID = 1; seasonID < 4; seasonID++) {
            seasonTab = $('<div class="seasonTab" data-seasonid="' + seasonID + '">Season ' + seasonID + '</div>');
            seasonTab.click(function () {
                _openSeasonStanding($(this).attr('data-seasonid'));
            });
            seasonsTabs.append(seasonTab);
        }
        tournamentSeasonsContainer.append(seasonsTabs);
        bestFinishesContainer.append(tournamentSeasonsContainer);
        //season standings tabs

        //probably TR owners-only data
        if (_throneUserData.TournamentRStats.AllTournamentRealmFinishes &&
            _throneUserData.TournamentRStats.AllTournamentRealmFinishes.length) {

            var allFinishDataContainer = $('<div class="allFinishDataContainer"><div class="label">A History of all your RX / RS finishes: </div></div>');

            var finishTable = $('<table class="finishTable" >' +
                    '<tr class="header"><th>Rank</th><th>Type</th><th>Duration</th>' /*+'<th>Opened</th>'*/ + '<th>Ended</th></tr>' +
                '</table>');

            var aFinish = null;
            for (var i = 0; i < _throneUserData.TournamentRStats.AllTournamentRealmFinishes.length; i++) {
                aFinish = _throneUserData.TournamentRStats.AllTournamentRealmFinishes[i];

                var rtype = aFinish.realmLengthInH <= 48 ? 'RX' : 'RS';
                var openedOn = new Date(aFinish.realmOpenOn).toDateString();
                var durationString = "";
                if (aFinish.realmLengthInH <= 48) {
                    durationString = aFinish.realmLengthInH + "h";
                } else {
                    durationString = Math.round(aFinish.realmLengthInH / 24) + "d";
                }

                var closedOn = new Date(aFinish.realmOpenOn + (aFinish.realmLengthInH * 60 * 1000)).toDateString();

                finishTable.append('<tr>' +
                        '<td>' + aFinish.rankByNumOfVillages + '</td>' +
                        '<td>' + rtype + '</td>' +
                        '<td>' + durationString + '</td>' +
                        //'<td>' + openedOn + '</td>' +
                        '<td>' + closedOn + '</td>' +
                    '</tr>');

            }
            
            allFinishDataContainer.append(finishTable);
            bestFinishesContainer.append(allFinishDataContainer);
            
        }

        bestFinishesContainer.appendTo(content);

        _popDisposable({
            ID: 'tourneyBesFinish',
            title: 'RX / RS Best Finishes:',
            content: content,
            width: 700,
            height: 550,
            modal: false,
            contentCustomClass: 'tourneyBesFinishContent'
        });
    }

    //returns a bestRankObject { type: "", rank: 123 }
    function _getBestTourneyRank() {
        var bestRankObject = { rank: null, type: null }; //output
        var bestRank = null; //RS or RX best rankByNumOfVillages {rankByNumOfVillages: int, numTimesRankAchieved: int}
        var type = null; //RS or RX

        //search RX best ranks
        bestRank = _throneUserData.TournamentRStats.RXBestResults;
        type = 'RX';
        for (var i = 0; i < bestRank.length; i++) {
            if (!bestRankObject.rank || bestRank[i].rankByNumOfVillages < bestRankObject.rank) {
                bestRankObject = { rank: bestRank[i].rankByNumOfVillages, type: type }
            }
        }

        //search RS best ranks
        bestRank = _throneUserData.TournamentRStats.RSBestResults;
        type = 'RS';
        for (var i = 0; i < bestRank.length; i++) {
            if (!bestRankObject.rank || bestRank[i].rankByNumOfVillages < bestRankObject.rank) {
                bestRankObject = { rank: bestRank[i].rankByNumOfVillages, type: type }
            }
        }

        return bestRankObject;
    }

    function _rankClass(type, rank) {

        if (type == 'RX') {

            if (rank == 1) {
                return 'master';
            } else if (rank <= 3) {
                return 'diamond';
            } else if (rank <= 5) {
                return 'platinum';
            } else if (rank <= 10) {
                return 'gold';
            } else if (rank <= 25) {
                return 'silver';
            } else {
                return 'bronze';
            }

        } else if (type == 'RS') {

            if (rank <= 3) {
                return 'master';
            } else if (rank <= 10) {
                return 'diamond';
            } else if (rank < 15) {
                return 'platinum';
            } else if (rank < 25) {
                return 'gold';
            } else if (rank < 50) {
                return 'silver';
            } else {
                return 'bronze';
            }

        }
    }

    function _openSeasonStanding(seasonNumber) {

        //var content = $();
        _popDisposable({
            ID: 'seasonStanding',
            title: 'Season ' + seasonNumber + ' standings ',
            content: '',
            width: 600,
            height: 600,
            modal: false,
            contentCustomClass: 'seasonStanding'
        });

        ROE.Frame.base_showIframeOpenDialog($('#seasonStanding'), 'https://www.realmofempires.com/rxseasonstandings.aspx?seasonID=season' + seasonNumber, null);

    }

    ///VIP stuff
    function _popupVip() {

        //maybe later we can have VIP lvl come from _throneUserData?
        //_throneUserData comes form the api call tr user info thing
        //console.log('_popupVip _throneUserData: ', _throneUserData.User.DisplayChatVIP);

        //example of using generic popup thing
        //using stats.aspx just for example, we can have content be iframe, or be created here, or anything
        var title = "V.I.P Panel";
        var content = $("#vipPopup").clone().show();

        var vipLevel = parseInt($('#vipBadge').attr('data-viplevel')); //get current VIP level
        var vipAvatars = [[30], [21,36], [29], [22]]; //bronze to diamond avatar IDs
        
        var avatarObj, avatarElement;

        //loop through and display all avatars unlocked for your vip level
        for (var i = 0; i < vipLevel; i++) {
            for (var k = 0; k < vipAvatars[i].length; k++) {
                avatarObj = ROE.Avatar.list[vipAvatars[i][k]];
                if (avatarObj) {
                    avatarElement = $('<div class="avatarBox" data-avatarid="' + avatarObj.id + '" style="background-image:url(' + avatarObj.imageUrlS + ')">');
                    $('.perks.avatars', content).append(avatarElement);
                }
            }
        }

        var h = 740;
        if (h > $(window).height() - 40) {
            h = $(window).height() - 40;
        }

        //setup click that toggles VIP Chat Border display
        content.find('.vipSwitch').click(function () {
            if ($(this).hasClass("off")) {
                _toggleChatVIP(1);
            } else {
                _toggleChatVIP(0);
            }
        });
        
        _popGeneric(title, content, 720, h);

        //affect VIP Chat Border toggle btn based on current status
        _toggleChatVIPCB({ newStatus: parseInt(_throneUserData.User.DisplayChatVIP) });

    }

    function _toggleChatVIP(status) {
        _busy("Updating....", 5000, $("#genericDialog"));
        ROE.Api.call_tr_togglechatvip(status, _toggleChatVIPCB);
    }

    function _toggleChatVIPCB(data) {
        _free($("#genericDialog"));
        if (data.newStatus == 1) {
            $('#genericDialog .vipSwitch').removeClass('off').addClass('on').html('VIP Chat Border is ON');
        } else {
            $('#genericDialog .vipSwitch').removeClass('on').addClass('off').html('VIP Chat Border is OFF');
        }
        _throneUserData.User.DisplayChatVIP = data.newStatus;
    }


    function _promptForLogin() {

        $('#loginDialog').dialog('destroy');

        var loginDialog = $('<div id="loginDialog" class="popupDialogs">');
        loginDialog.append('<div class="bkg"></div><div class="text">Please Login to use all Throne Room features.</div>');
        loginDialog.append($('#loginbutton').clone());
        loginDialog.append($('<div class="BtnBLg1 fontButton1L cancel">Cancel</div>').click(function () { loginDialog.dialog('close'); }));
        loginDialog.dialog({
            autoOpen: true,
            modal: true,
            title: 'Login',
            dialogClass: "throneDialog",
            width: 400,
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            create: function () {
                _addDialogStylingElements($(this));
            },
            close: function () {

                _commonDialogCloseFunction($(this));
                $(this).dialog('destroy');
            }
        });

    }


    //populate and open the reusable dialog
    function _popGeneric(title, content, w, h) {
        /*
        var genericDialog = $('#genericDialog').empty();

        genericDialog.append($(content).addClass('genericDialogContent'));
        genericDialog.dialog("option", "title", title)
        genericDialog.dialog("option", "width", w);
        genericDialog.dialog("option", "height", h);
        genericDialog.dialog('open');
        */


        _popDisposable({
            ID: 'genericDialog',
            title: title,
            content: content,
            width: w,
            height: h,
            modal: true,
            contentCustomClass: 'genericDialogContent'
        });

    }

    //create, and open a disposable dialog
    //it checks if exists or already open, when opening
    //on close, the dialog is destroyed, conserves memory and cleans DOM
    ///
    ///settings.ID - sets dialog ID, default 'Generic'
    ///settings.title - sets dialog title, default ''
    ///settings.content - sets dialog content, default ''
    ///settings.width - sets dialog width, default 350
    ///settings.height - sets dialog height, default auto
    ///settings.modal - sets dialog modal, default true
    ///settings.contentCustomClass - gives dialog's content a custom class

    function _popDisposable(settings) {

        settings.ID = settings.ID || 'genericDialog';
        settings.title = settings.title || '';
        settings.content = settings.content || '<div></div>';
        settings.width = settings.width || 350;
        settings.height = settings.height || 'auto';
        settings.modal = (settings.modal === false ? false : true);
        settings.contentCustomClass = settings.contentCustomClass || '';

        var dialog = $('#' + settings.ID).empty();

        if (dialog.length < 1) {
            dialog = $('<div id="' + settings.ID + '" class="popupDialogs"></div>').appendTo('body');
        }

        dialog.dialog({
            autoOpen: false,
            title: settings.title,
            width: settings.width,
            height: settings.height,
            modal: settings.modal,
            dialogClass: "throneDialog",
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            close: function () {
                _commonDialogCloseFunction($(this));
                dialog.dialog('destroy');
                dialog.remove();
            },
            create: function () {
                _addDialogStylingElements($(this));
            }
        });

        dialog.dialog('open');

        var dialogContent = $(settings.content).addClass(settings.contentCustomClass).appendTo(dialog);

        return dialog;

    }


    function _setupDialogs() {

        $('#genericDialog').dialog({
            autoOpen: false,
            modal: true,
            dialogClass: "throneDialog",
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            create: function () {
                _addDialogStylingElements($(this));
            },
            close: function () {
                $(this).empty();
                _commonDialogCloseFunction($(this));
            }
        });

        $('#realmProfile').dialog({
            autoOpen: false,
            modal: true,
            title: 'Realm Profile',
            dialogClass: "throneDialog",
            width: '800px',
            height: 600,
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            create: function () {
                _addDialogStylingElements($(this));
            },
            close: function () {
                $(this).empty();
                _commonDialogCloseFunction($(this));
            }
        });


        $('#shareDialog').dialog({
            autoOpen: false,
            modal: true,
            title: 'Share',
            dialogClass: "throneDialog",
            width: 500,
            height: 300,
            open: function () {
                _commonDialogOpenFunction($(this));
            },
            create: function () {
                _addDialogStylingElements($(this));
            },
            close: function () {
                _commonDialogCloseFunction($(this));
            }
        });

    }


    function _commonDialogOpenFunction(dialog) {
        if (_mBrowser) {
            dialog.dialog("option", "width", $(window).width());
            dialog.dialog("option", "height", $(window).height());
        }
    }

    function _commonDialogCloseFunction(dialog) {
        $('.toolTipBox').remove(); //not hiding in some cases, so here we nukem -farhad

        //remove this button so it doesnt linger on all generic dialogs. Maybe the TR choose-realm now needs its own dialog -farhad
        dialog.parent().find('.endedToggle').remove();
    }

    function _addDialogStylingElements(dialog) {
        if (!_mBrowser) {
            var titleBar = dialog.parent().find('.ui-dialog-titlebar');
            var titleBarElements = "<div class='roeStyleTitle'><div class='headerL'></div><div class='headerM'></div><div class='headerR'></div><div class='headerOverlay'></div></div>";
            var dialogContentElements = "<div class='roeStyleContent'><div class='contentBody'></div><div class='contentLT'></div><div class='contentLM'></div><div class='contentLB'></div><div class='contentBottom'></div><div class='contentRT'></div><div class='contentRM'></div><div class='contentRB'></div></div>";
            dialog.parent().prepend(dialogContentElements);
            titleBar.prepend(titleBarElements);
        }
    }

    obj.load = _load;
}(window.ROE.Throne = window.ROE.Throne || {}));


//Because coutndown JS is expecting a global function.. needs cleanup! must change countdown to use the UTILS one
function padDigits(num) {
    ///<summary> REPRECIATED. use ROE.Utils.padDigits();</summary>
    return ROE.Utils.padDigits(num);
}

//jquery cookie
!function (e) { "function" == typeof define && define.amd ? define(["jquery"], e) : e("object" == typeof exports ? require("jquery") : jQuery) }(function (e) { function n(e) { return u.raw ? e : encodeURIComponent(e) } function o(e) { return u.raw ? e : decodeURIComponent(e) } function i(e) { return n(u.json ? JSON.stringify(e) : String(e)) } function r(e) { 0 === e.indexOf('"') && (e = e.slice(1, -1).replace(/\\"/g, '"').replace(/\\\\/g, "\\")); try { return e = decodeURIComponent(e.replace(c, " ")), u.json ? JSON.parse(e) : e } catch (n) { } } function t(n, o) { var i = u.raw ? n : r(n); return e.isFunction(o) ? o(i) : i } var c = /\+/g, u = e.cookie = function (r, c, f) { if (void 0 !== c && !e.isFunction(c)) { if (f = e.extend({}, u.defaults, f), "number" == typeof f.expires) { var a = f.expires, d = f.expires = new Date; d.setTime(+d + 864e5 * a) } return document.cookie = [n(r), "=", i(c), f.expires ? "; expires=" + f.expires.toUTCString() : "", f.path ? "; path=" + f.path : "", f.domain ? "; domain=" + f.domain : "", f.secure ? "; secure" : ""].join("") } for (var p = r ? void 0 : {}, s = document.cookie ? document.cookie.split("; ") : [], m = 0, x = s.length; x > m; m++) { var v = s[m].split("="), k = o(v.shift()), l = v.join("="); if (r && r === k) { p = t(l, c); break } r || void 0 === (l = t(l)) || (p[k] = l) } return p }; u.defaults = {}, e.removeCookie = function (n, o) { return void 0 === e.cookie(n) ? !1 : (e.cookie(n, "", e.extend({}, o, { expires: -1 })), !e.cookie(n)) } });
