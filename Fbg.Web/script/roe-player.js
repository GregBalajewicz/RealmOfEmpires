/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.6.4.js" />
/// <reference path="interfaces.js" />
/// <reference path="BDA.js" />
/// <reference path="BDA-exception.js" />
/// <reference path="BDA-utils.js" />
/// <reference path="BDA-val.js" />
/// <reference path="roe-api.js" />

ROE.Player =
{
    Enum: { CacheItems: {incoming : 1, outgoing: 2} }

    , _lastHandledVillageCacheTimeStamp : undefined
    , Clan: null // if null, that means the player is not part of a clan. otherwise, get {clanID = X}
    , credits: null
    , Ranking: { points: 0, xp: 0, title: '', titleID: function () { return this.titleLvl - 1; }, titleLvl: 0, titleLvlR: 'I', titleProgress: 0.0, levelUp: false }
    , Indicators: { report: 0, mail: 0, clanForum: 0 }
    , id: 0
    , questsCompl: 0
    //, IncomingAtt: { NumAttacks: 0 }
    , CacheItems: {}
    , chooseGovType: false
    , dailyRewardAvail: false
    , dailyRewardLevel: 0
    , dailyRewardNext: 0
    , recEmailState: 0
    , recEmail: ""
    , recEmailSet: false
    , recEmail_isDeviceLogin : false
    , recEmail_isTacticaLogin : false
    , recEmail_isKLogin: false
    , restartCost: 0
    , isSteward: false
    , sleepModeOn: 0
    , sleepModeAvailable: false
    , isSaleActive: false
    , approvalBoostAllowedWhen: null
    , morale: 0
    , chestCount : 0
    , MyResearch: { numOfIdleResearchers: 0, researcherIdleIn: new Date() }
    , PFPckgs: { list: {},
        isActive: function (pfPackageID) {
            ///<summary>tells you if </summary>
            var pck = this.list[pfPackageID];
            if (pck) {
                return (new Date()) <= (new Date(pck.ExpiresOn));
            }
            return false;
        }
    }//Premium feature packages that this player has or had active.
    , items: [] //reward items
    , va :0


    , _setlastHandledVillageCacheTimeStamp: function _setlastHandledVillageCacheTimeStamp(timestamp) {
        ROE.Player._lastHandledVillageCacheTimeStamp = timestamp;
        localStorage.setItem("_lastHandledVillageCacheTimeStamp", timestamp.getTime());
    }
    , _getlastHandledVillageCacheTimeStamp: function _getlastHandledVillageCacheTimeStamp() {
        if (ROE.Player._lastHandledVillageCacheTimeStamp == undefined) {
            // if we dont have the time stamp, then try to get it from localStorage
            if (localStorage.getItem("_lastHandledVillageCacheTimeStamp") != undefined) {
                // we got it in local storage, so try to make a date out of this; in case of some failure, set to 1970 and we go from there
                try {                    
                    ROE.Player._lastHandledVillageCacheTimeStamp = new Date(parseInt(localStorage.getItem("_lastHandledVillageCacheTimeStamp"), 10))
                }catch(e) {
                    ROE.Player._lastHandledVillageCacheTimeStamp = new Date("Jan 1 1970");
                }
            } else {
                // we dont have it in localstorage, so set to 1970 and go from there
                ROE.Player._lastHandledVillageCacheTimeStamp = new Date("Jan 1 1970");
            }
            
        }
        return ROE.Player._lastHandledVillageCacheTimeStamp;
    }


    , load: function (callback) {
        ///<summary> loads the entire object, all fields, and then calls the callback when done</summar>
        ROE.Api.call_playerFull(ROE.SVID, function (data) 
        { 
            ROE.Player._load_callback(data);
            if (typeof (callback) === 'function') {
                callback();
            }
        });
    }

    , _load_callback: function (data) {
        ROE.Player.commonCallback(data);

        ROE.Player.id = data.playerID;
        ROE.Player.Ranking.points = data.ppoints;
        ROE.Player.Ranking.titleLvl = data.titleLvl;
        ROE.Player.Ranking.titleLvlR = data.titleIDR;
        ROE.Player.Ranking.title = data.titleName;
        ROE.Player.Ranking.levelUp = false;
        ROE.Player.updateTitleProgress(data.titleProgress);
        ROE.Player.credits = data.credits;
        ROE.Player.questsCompl = data.questsCompl;
        //ROE.Player.IncomingAtt = data.incomingAtt;
        ROE.Player.Indicators = data.Indicators;
        ROE.Player.PFPckgs.list = data.PFPckgs;
        ROE.Player.Clan = data.Clan;
        ROE.Player.CacheItems = data.CacheItems;

        ROE.Player.recEmailState = data.RecEmailState;
        ROE.Player.recEmail = data.RecEmail;
        ROE.Player.recEmail_isDeviceLogin = data.RecEmail_isDeviceLogin;
        ROE.Player.recEmail_isTacticaLogin = data.RecEmail_isTacticaLogin;
        ROE.Player.recEmail_isKLogin = data.RecEmail_isKLogin;
        ROE.Player.restartCost = data.RestartCost;
        ROE.Player.numberOfVillages = data.NumberOfVillages;
        ROE.Player.chestCount = data.chestCount;
        ROE.LocalServerStorage.init(data.LocalStorageOnServer);

        ROE.Realm.isConsolidationAttackFreezeActiveNow = data.IsConsolidationAttackFreezeActiveNow;

        ROE.Player.hasOffer2 = data.hasOffer2;

        ROE.playersNumVillages = data.PlayersNumVillages;

        ROE.Player.MyResearch = data.MyResearch;

        ROE.Player.itemGroups = data.itemGroups;

        //credit farm bonus events related
        ROE.Realm.creditFarmBonusDateEnds = data.creditFarmBonusDateEnds; //the date comes from realm attribute 57
        ROE.Realm.creditFarmBonusMultiplier = data.creditFarmBonusMultiplier; //the value comes from realm attribute 58
        ROE.Realm.creditFarmBonusDesc = data.creditFarmBonusDesc; //the value comes from realm attribute 59
        ROE.Realm.creditFarmBonusIcon = data.creditFarmBonusIcon; //the value comes from realm attribute 60

        //converts the avatars array data into an object, and puts it in ROE.Avatar.list
        ROE.Player.avatarID = data.avatarID;
        ROE.Avatar.convertAvatarsArrayToObject(data.AvatarList);

        ROE.Player.approvalBoostAllowedWhen = data.approvalBoostAllowedWhen;


    }

    , updateTitleProgress: function (titleProgress) {
        ///<summary>update the player's title progress correctly. do not not set Ranking.titleProgress directly</summary>
        ///<param name="titleProgress">must be INT! between 0 and 100 inclusive</param>
        var paramsValid = BDA.Val.required(titleProgress, "titleProgress not specified", true, ROE.Player.getContextInfo);
        paramsValid = paramsValid && BDA.Val.assertINT(titleProgress, "titleProgress wrong type", true, ROE.Player.getContextInfo);

        if (paramsValid) {
            if (titleProgress === 100) {
                ROE.Player.Ranking.levelUp = true;
                ROE.Player.Ranking.titleProgress = 100;
            } else if (titleProgress === -1) {
                ROE.Player.Ranking.titleProgress = 100;
            } else {
                ROE.Player.Ranking.titleProgress = parseInt(titleProgress, 10);
            }
        }
    }

    , refreshInProgressStack: 0  // ensures that we dont call player.refresh when its already in progress
    , refresh_LastCalledTimeStamp: new Date() // ensures that if something goes wrong, and we never reset the refreshInProgressStack, then we still resume calls after some time
    , refresh: function (callback, errorcallback_timeout, errorcallback_error) {
        ///<summary> loads the entire object, all fields, and then calls the callback when done</summary>
        //
        // if refresh is in progress, check the last time a call was made. if over some time, then break the lock as there could be a problem (bug) and try again
        //
        if (ROE.Player.refreshInProgressStack) {
            var now = new Date();
            var timeDiff = Math.abs(now.getTime() - ROE.Player.refresh_LastCalledTimeStamp.getTime()); //get diff in MS
            if (timeDiff > 60000) {// if nore tham 60 seconds passed, break the lock                
                ROE.Player.refreshInProgressStack = 0;
                BDA.Console.error("ROE.Player.refresh - have to break lock");
            }
        }
        //
        // make the api call, unless already in progress
        //
        if (!ROE.Player.refreshInProgressStack) {
            ROE.Player.refreshInProgressStack = 1;
            ROE.Player.refresh_LastCalledTimeStamp = new Date();

            ROE.Api.call_playerRefresh(ROE.SVID,
                BDA.Utils.GetDateForApiCall(ROE.Player._getlastHandledVillageCacheTimeStamp()),
                function (data) {
                    ROE.Player._refresh_callback(data);
                    if (typeof callback === 'function') {
                        callback();
                    }
                    BDA.Console.verbose("ROE.Player.refresh", "api OK");
                    ROE.Player.refreshInProgressStack=0;
                }
                , function (jqXHR, textStatus, errorThrown) {
                    BDA.Console.error("ROE.Player.refresh", "api failre : %textStatus%, %errorThrown%".format({ textStatus: textStatus, errorThrown: errorThrown }));
                    ROE.Player.refreshInProgressStack = 0;
                    if (textStatus == 'timeout') {
                        if (errorcallback_timeout) {
                            errorcallback_timeout();
                        }
                    } else {
                        if (errorcallback_error) {
                            errorcallback_error();
                        }
                    }
                }
                );
        } else {
            BDA.Console.log("ROE.Player.refresh", "not calling, already in progress");
        }
        
    
    }

    , _refresh_callback: function (data) {
        var i;
        var ts;

        if (ROE.Player.id != data.playerID) {
            window.location.reload();
        }

        ROE.Player.commonCallback(data);

        ROE.Player.Ranking.points = data.ppoints;
        //ROE.Player.IncomingAtt = data.incomingAtt;
        ROE.Player.Indicators = data.Indicators;
        ROE.Player.updateTitleProgress(data.titleProgress);
        ROE.Player.Clan = data.Clan;
        ROE.Player.questsCompl = data.questsCompl;
        ROE.Player.CacheItems = data.CacheItems;

        ROE.Player.chooseGovType = data.chooseGovType;
        ROE.Player.recEmailState = data.RecEmailState;
        ROE.Player.recEmail = data.RecEmail;
        ROE.Player.recEmail_isDeviceLogin = data.RecEmail_isDeviceLogin;
        ROE.Player.recEmail_isTacticaLogin = data.RecEmail_isTacticaLogin;
        ROE.Player.recEmail_isKLogin = data.RecEmail_isKLogin;
        ROE.Player.restartCost = data.RestartCost;

        ROE.Troops.InOut2.checkIfFrefreshIsNeeded(ROE.Player.CacheItems[ROE.Player.Enum.CacheItems.incoming], ROE.Player.CacheItems[ROE.Player.Enum.CacheItems.outgoing])
        ROE.playersNumVillages = data.PlayersNumVillages;
        ROE.CurrentBuildID = data.CurrentBuildID;
        ROE.CurrentBuildIDWhatsNew = data.CurrentBuildIDWhatsNew;
        ROE.Player.numberOfVillages = data.NumberOfVillages;
        ROE.Player.chestCount = data.chestCount;
        ROE.LocalServerStorage.init(data.LocalStorageOnServer);

        ROE.Realm.isConsolidationAttackFreezeActiveNow = data.IsConsolidationAttackFreezeActiveNow;

        ROE.Realm.isSaleActive = data.isSaleActive;
        ROE.Realm.friendRewardBonusUntil = data.FriendRewardBonusUntil;
        ROE.Player.hasOffer2 = data.hasOffer2;

        ROE.Player.beginnerProtected = data.beginnerProtected;
        ROE.Player.beginnerProtection = data.beginnerProtection;
        ROE.Player.approvalBoostAllowedWhen = data.approvalBoostAllowedWhen;


        $(document).trigger('player.refresh', data);

        ROE.Player.MyResearch = data.MyResearch;

        //
        // got any changed villags? then refresh them and grab the oldest timestamp as the last one handled
        //
        if (data.ChangedVillages != undefined) {
            for (var i = 0; i < data.ChangedVillages.length; i++) {
                ts = new Date(data.ChangedVillages[i].ts);
                if (ROE.Player._getlastHandledVillageCacheTimeStamp() < ts) {
                    ROE.Player._setlastHandledVillageCacheTimeStamp(ts);
                }
                BDA.Console.verbose('VillChanges', "VID:%vid% [Player] - trigger load latest if older than".format({ vid: data.ChangedVillages[i].vid }));
                ROE.Villages.ExtendedInfo_loadLatestIFOlderThan(data.ChangedVillages[i].vid, ts);
            }
        }

        // see if the DB needs to change
        ROE.localDBVersion = data.LocalDBVersion;

        
        //if (BDA.Database.ReCreateIfNeededByVersion()) {
        //    ROE.Villages.clearCache();
        // }
        // ReCreateIfNeededByVersion is async so needs to be called this way.
        // It passes true if recreate needed, else false.
        BDA.Database.ReCreateIfNeededByVersion().done(function (needsRecreate) {
            if (needsRecreate) {
                ROE.Villages.clearCache();
            }
        });

        //update player targets, also let animations know
        ROE.Player.Targets = data.Targets;
        ROE.Animation.animateDefinedTargets();
        ROE.Frame.handleTargetsListIcon();

        //update raid icon
        ROE.Raids.updateRaidIcon($('#launchRaidsPopup'));

        //Warning, this seemed like a good idea, but causes a bad loop in some modules such as SingleAttack
        //will need to think of a different way to refresh map regularly
        //ROE.Landmark.refresh();

        if (data.challenge) {
            if ($('#challenge').length == 0) {
                ROE.Frame.popDisposable({
                    ID: 'challenge',
                    title: 'Are you human?',
                    content: '',
                    width: 500,
                    height: 500,
                    modal: true,
                    contentCustomClass: 'challengeContent'
                });

                ROE.Frame.showIframeOpenDialog('#challenge', 'challenge.aspx');
            }
        }

        ROE.Player.va = data.vaa;
    }

    //Common Data things between Load and Refresh
    , commonCallback: function (data) {

        //Vacation Mode    
        ROE.Player.VacationInRealm = data.VacationInRealm;
        ROE.Player.VacationStatus = data.VacationStatus;
        if (ROE.Player.VacationStatus.active) {
            window.location = 'VacationMode_Active_NoLogin.aspx';
        }

        //Vacation Mode    
        ROE.Player.WeekendModeInRealm = data.WeekendModeInRealm;
        ROE.Player.WeekendModeStatus = data.WeekendModeStatus;
        if (ROE.Player.WeekendModeStatus.active) {
            window.location = 'WeekendMode_Active_NoLogin.aspx';
        }

        //Sleep Mode
        ROE.Player.sleepModeOn = data.SleepModeOn;
        ROE.Player.sleepModeAvailable = false;
        ROE.Player.SleepModeCountdown = data.SleepModeCountdown;
        ROE.Player.SleepinRealm = data.SleepinRealm;
        ROE.Player.SleepMode_IsActiveNow = data.SleepMode_IsActiveNow;
        if (ROE.Player.SleepMode_IsActiveNow) {
            window.location = 'SleepMode_Active_NoLogin.aspx';
        }

        //Daily Reward
        ROE.Player.dailyRewardAvail = data.DailyRewardAvail;
        ROE.Player.dailyRewardLevel = data.DailyRewardLevel;
        ROE.Player.dailyRewardNext = data.DailyRewardNext;

        //MapEvents
        var mapEventChange = false;
        if (ROE.Player.MapEvents) {
            mapEventChange = ROE.Player.MapEvents.length != data.MapEvents.length;
        }        
        ROE.Player.MapEvents = data.MapEvents;
        ROE.Player.regroupMapEvents(); //remakes the ROE.Player.MapEventsGrouped object using the new ROE.Player.MapEvents
        if (mapEventChange) {
            ROE.Landmark.refill(true);
            ROE.MapEvents.updateMapEventNotifications(true);
            ROE.MapEvents.checkAddVOVCaravan();
        }

        // update morale, broadcast if changed
        if (ROE.Player.morale != data.morale) {
            ROE.Player.morale = data.morale;
            BDA.Broadcast.publish("PlayerMoraleChanged");
        }

        //Quest
        ROE.Player.nextRecommendeQuests = data.nextRecommendeQuests;

        //Raids
        ROE.Player.numOfRaidRewardsToCollect = data.NumOfRaidRewardsToCollect;
        ROE.Player.nextRaid = data.NextRaid;

        
    }

    //group mapevents by their co-ords for faster consumption in mapcode
    , regroupMapEvents: function () {
        ROE.Player.MapEventsGrouped = {};

        $(ROE.Player.MapEvents).each(function () {
            var ex = this.xCord, ey = this.yCord;
            if (!ROE.Player.MapEventsGrouped[ex + "," + ey]) {
                ROE.Player.MapEventsGrouped[ex + "," + ey] = [];
            }
            ROE.Player.MapEventsGrouped[ex + "," + ey].push(this);
        });
    }

    , refreshPFPackages: function (callback) {
        ///<summary> refresh ROE.Player.PFPckgs</summary>
        ///<param name="callback">OPTIONAL - pass in function that you want called when the refresh is completed</param>
        ROE.Api.call_getPlayerPFPackages(function (data) { ROE.Player._refreshPFPackages_callback(data); if (callback === 'function') { callback(); } });
    }

    , _refreshPFPackages_callback: function (data) {
        ROE.Player.PFPckgs.list = data.PFPckgs;
    }

    // these properties are loaded only once, or updatad only as part of user action (like changing avatar) 
    , name: ''
    , avatar: ''

    , getContextInfo: function getContextInfo() {
        return {
            'id': ROE.Player.id
        };
    }
}

