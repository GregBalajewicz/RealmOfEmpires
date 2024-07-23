/// <reference path="https://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.6.4-vsdoc.js"/>
/// <reference path="bda-ui-transition.js"/>
/// <reference path="roe-api.js"/>
/// <reference path="BDA.js" />
/// <reference path="BDA-exception.js" />
/// <reference path="BDA-utils.js" />
/// <reference path="roe-player.js" />
/// <reference path="roe-frame.js" />
/// <reference path="roe-frame_m.js" />
/// <reference path="countdown.js"/>





(function (ROE) {
}(window.ROE = window.ROE || {}));

(function (Research) {

    BDA.Console.setCategoryDefaultView('RSC', false);

    ///var CANVAS_WIDTH = 0, CANVAS_HEIGHT = 0; //global canvas dimensions, updated per strand and used in multiple places
    ///var canvasElement, ctx1; //canvas 1 variables

    var _village;
    var _currentRenderedStrand = null;
    var researchBoxes = {};
    var ResearchItems;
    var tierObject = {};

    ///these vars allow the render nodes to be customizable easily
    var researchBoxWidth = 107; ///width of rendered research boxes
    var researchBoxHeight = 107; ///height of rendered research boxes 
    var researchBoxTierGap = 50; ///distance between node columns
    var researchBoxRowGap = 5; ///distance between node rows
    var nodeOffsetY = 15; ///starting offset Y
    var nodeOffsetX = 15; ///starting offset X
    var totalLoaded = 0;
    var anyResearcherAvailable = false;
    var queueCompleted = false;
    var researchAssets = {};
    var researchManifest = [
        { src: "https://static.realmofempires.com/images/Research/Border02gray.png", id: "rBorderGray" },
        { src: "https://static.realmofempires.com/images/Research/Border02normal.png", id: "rBorderNormal" },
        { src: "https://static.realmofempires.com/images/Research/Border02selected.png", id: "rBorderSelected" },
        { src: "https://static.realmofempires.com/images/Research/Border02complete.png", id: "rBorderCompleted" },
        { src: "https://static.realmofempires.com/images/buttons/ResearchButton.png", id: "rResearchButton" },
        { src: "https://static.realmofempires.com/images/icons/PF_Plus2.png", id: "rGreenPlusIcon" },
        { src: "https://static.realmofempires.com/images/HourGlass.png", id: "rHourGlassIcon" },
        { src: "https://static.realmofempires.com/images/REsearch/allResearch.jpg", id: "rAllIcons" }
    ];

    var researchDesc = {};

    function initResearchPreload() {

        if (queueCompleted) {
            //BDA.Console.verbose('RSC', 'queue has been completed once, exiting preload');
            return;
        }

        Frame.busy(BDA.Dict.rsr_LoadingResearchAssets, 60000, $('#researchDialog'));

        var queue = new createjs.LoadQueue(false);

        queue.addEventListener("complete", function (event) {
            for (var i = 0; i < researchManifest.length; i++) {
                researchAssets[researchManifest[i].id] = queue.getResult(researchManifest[i].id);
            }
            //BDA.Console.verbose('RSC', 'BuildTree called from complete event');
            //BDA.Console.verbose('RSC', researchAssets);
            
            Frame.free($('#researchDialog'));
            queueCompleted = true;
            buildTechTree();

        });

        queue.loadManifest(researchManifest);
    }

    var Timer = window.BDA.Timer,
        Transition = window.BDA.UI.Transition,
        Utils = window.BDA.Utils;

    var Api = window.ROE.Api,
        Entities = window.ROE.Entities,
        Frame = window.ROE.Frame,
        Player = window.ROE.Player;

    var CONST = { popupName: "Research" },
        CACHE = {};

    CONST.Enum = {};

    CONST.Selector = {
        header: ".themeM-panel.header",
        footer: ".themeM-panel.footer",
        page: ".themeM-panel.research",

        buttonHome: ".themeM-view.detail .strandInfo",

        viewMaster: ".themeM-view.master",
        viewDetail: ".themeM-view.detail",

        listResearchType: "ul.researchType",
        listResearchItem: "ul.researchItem",
        listResearchMember: "ul.researchMember",

        icon: ".icon",
        more: ".more",
        name: ".name",
        progress: ".progress-indicator",
        handle: ".handle",
        researchMemberBusy: ".researchMemberBusy",
        researchMemberFree: ".researchMemberFree",
        researchMemberNone: ".researchMemberNone",
        busy: ".busy",
        free: ".free",
        none: ".none",
        some: ".some",
        avatar: ".avatar",
        status: ".status",
        time: ".time",
        hire: ".hire"
    };

    CONST.CssClass = {
        template: "template",
        selected: "selected",
        building: "building",
        unit: "unit",
        busy: "busy",
        free: "free",
        none: "none",
        some: "some",
        countdownTimer: "countdown"
    };

    CONST.DataAttr = {
        id: "data-id",
        countdownTimer: "data-refreshCall"
    };

    CONST.Data = {
        researchItem: "research",
        researchStatus: "researchState",
        researchAction: "research_do",
        researchMember: "research_getresearchers"
    };

    CONST.Enum.CanResearch = {
        OK: 0,
        NO_AlreadyResearched: 1,
        NO_ReqNotMet: 2,
        NO_ResearchersBusy: 3,
        NO_NoSilver: 4,
        NO_ResearchingNow: 5
    };

    CACHE.Selector = {
        header: null,
        footer: null,
        page: null,

        buttonHome: null,

        viewMaster: null,
        viewDetail: null,

        canvas: [],

        listResearchType: null,
        listResearchItem: null,
        listResearchMember: null
    };

    CACHE.Data = {
        entityBuilding: null,
        entityUnit: null,

        researchItem: null,
        researchTypeBuilding: null,
        researchTypeUnit: null,
        researchTypeMisc: null,
        researchType: {},
        researchTypeStatus: {},

        researchStatusResearch: null,
        researchStatusComplete: null,

        researchMember: null,
        researchMemberMax: 0,
        hoursBehind: 0

    };

    CACHE.hireResearcherTemplate=null;

    var _busy;
    var _view;

    var _focusOnId; //if not null, focuses and selects specific research Item

    function _showResearchPopupBySpecificItem(itemId) {
        _focusOnId = itemId;
        var item = ROE.Entities.Research.Items[itemId];
        var strandId;
        if (item.effectedBuildingTypeID && item.effectedBuildingTypeID != 0) {
            strandId = 'b' + item.effectedBuildingTypeID;
        } else if (item.unlocksUnitTypeID && item.unlocksUnitTypeID != 0) {
            strandId = 'u' + item.unlocksUnitTypeID;
        } else {
            strandId = 'm' + item.propertyID;
        }
        _showResearchPopup(strandId);

        //console.log(ROE.Entities.Research.Items[itemId]);
    }

    function _showResearchPopup(strandId) {
        _currentRenderedStrand = strandId;
        var template;
        var iconImgUrl = "https://static.realmofempires.com/images/icons/M_ResearchList.png";
        if (ROE.isMobile) {
            template = BDA.Templates.get("research", ROE.realmID);
            popupModalPopup(CONST.popupName, 'Research', undefined, undefined, undefined, undefined, 'closeResearch', iconImgUrl);
            $("#" + ROE.Frame.CONSTS.popupNameIDPrefix + CONST.popupName + ' .popupBody').html(template);
            //temp fix untill research popup on m is dialog as well
            $('#quickRecruitDialog').dialog('close');
            $('#villageListDialog').dialog('close');
        } else {
            template = BDA.Templates.get("researchD2", ROE.realmID);
            $('#researchDialog').html(template).dialog('open');
        }

        /*temp code to show a researcher sale to explain the 1 servant researcher cost */
        if (ROE.realmID < 76) {
            $('#researcherSale').remove();
        }
        /*temp code to show a researcher sale to explain the 1 servant researcher cost */

        ROE.Frame.busy(BDA.Dict.rsr_LoadingResearchInformation, 5000, $('#researchDialog'));
        ROE.Villages.getVillage(ROE.SVID, ROE.Research.init, ROE.Villages.Enum.ExtendedDataRetrieveOptions.ensureExists);
    }

    function init(village) {
        //console.log('R init',village);
        researchDesc = {
            buildingEffects: {
                "1": BDA.Dict.rsr_BarracksRecruitSpeed,
                "2": BDA.Dict.rsr_StablesRecruitSpeed,
                "3": BDA.Dict.rsr_ConstructionSpeed,
                "4": BDA.Dict.rsr_DefenseFactor,
                "5": BDA.Dict.rsr_SilverProduction,
                "6": BDA.Dict.rsr_TreasuryCapacity,
                "7": BDA.Dict.rsr_DefenseFactor,
                "8": BDA.Dict.rsr_FarmOutput,
                "10": BDA.Dict.rsr_WorkshopRecruitSpeed,
                "11": BDA.Dict.rsr_TradePostCapacity,
                "12": BDA.Dict.rsr_TavernRecruitSpeed
            },
            unitEffects: {
                "2": BDA.Dict.rsr_UnlocksInfantry,
                "6": BDA.Dict.rsr_UnlocksKnight,
                "5": BDA.Dict.rsr_UnlocksLightCavalry,
                "12": BDA.Dict.rsr_UnlocksSpy,
                "8": BDA.Dict.rsr_UnlocksTrebuchet
            },
            miscEffects: {
                "12": "Stronger defense",
                "13": "Stronger attack"
            }
        };


        _village = village;
        ROE.Frame.free($('#researchDialog'));
        //BDA.Console.verbose('RSC', 'RESEARCH INIT');

        CACHE.Selector.header = $(CONST.Selector.header);
        CACHE.Selector.footer = $(CONST.Selector.footer);
        CACHE.Selector.page = $(CONST.Selector.page);
        CACHE.Selector.buttonHome = $(CONST.Selector.buttonHome);
        CACHE.Selector.viewMaster = $(CONST.Selector.viewMaster);
        CACHE.Selector.viewDetail = $(CONST.Selector.viewDetail);
        CACHE.Selector.listResearchType = $(CONST.Selector.listResearchType);
        CACHE.Selector.listResearchMember = $(CONST.Selector.listResearchMember);

        _busy = 0;
        _view = CACHE.Selector.viewMaster;
        viewHome(); //for D2 adjustment

        CACHE.Selector.buttonHome.bind("click", function (event) {
            event.preventDefault();
            ROE.UI.Sounds.click();
            viewHome();
        });

        CACHE.Selector.listResearchType.delegate("a", "click", function (event) {
            event.preventDefault();
            ROE.UI.Sounds.click();
            _currentRenderedStrand = $(this).parent().attr('data-id');
            buildTechTree();           
        });

        CACHE.Selector.header.find(CONST.Selector.handle).bind("click", function (event) {
            event.preventDefault();
            ROE.UI.Sounds.click();
            if ($(this).hasClass('closed')) {
                $(this).removeClass('closed').addClass('opened');
                CACHE.Selector.header.addClass('expanded').attr('data-tutorial-opened', 'true');
                $(".themeM-panel.research").hide();
                $('#research .themeM-panel.header ul.researchMember li').stop().hide().each(function (index) {
                    $(this).delay(90*index).fadeIn(80); //this fixes a repaint problem, and it looks cool too
                });           
            } else {
                $(this).removeClass('opened').addClass('closed');
                CACHE.Selector.header.removeClass('expanded').attr('data-tutorial-opened', 'false');
                $(".themeM-panel.research").show();           
            }
        });

        CACHE.Selector.listResearchMember.delegate(".hire", "click", function (event) {
            event.preventDefault();
            ROE.UI.Sounds.click();
            var researcherIndex = $(event.currentTarget).closest('ul.researchMember li').attr('data-id');           
            showBuyInvitePanel2(researcherIndex);
        });

        $('#mainpane').empty();
        if (!ROE.isMobile) {
            _setupD2ResearchScrollDrag();
        }
                     

        if (!CACHE.Data.researchItem) {
            load();
        }
        else {

            if (CACHE.Data.researchStatusResearch
                && CACHE.Data.researchStatusComplete
                && CACHE.Data.researchMember) {
                list();
            }

            sync();
        }

        //BDA.Console.verbose('RSC', 'Research Preload Started');
        initResearchPreload();

    }

    function load() {
        /// <summary>
        /// Loads research data.
        /// </summary>
        CACHE.Data.entityBuilding = Entities.BuildingTypes;
        CACHE.Data.entityUnit = Entities.UnitTypes;
        CACHE.Data.researchItem = Entities.Research.Items;
        CACHE.Data.researchTypeBuilding = Entities.Research.BuildingsEffected;
        CACHE.Data.researchTypeUnit = Entities.Research.UnitUnlocks;
        CACHE.Data.researchTypeMisc = Entities.Research.MiscResearchGroups;
        load_onDataSuccess();
    }

    function load_onDataSuccess(data) {
        var rilist;

        //Filter out GOV research (1Bill cost ones mean gov tech)

        // Building
        for (var i = 0, il = CACHE.Data.researchTypeBuilding.length; i < il; ++i) {
            CACHE.Data.researchTypeBuilding[i].rilist = CACHE.Data.researchTypeBuilding[i].rilist.filter(function (id) {
                return CACHE.Data.researchItem[id].cost < 1000000000;
            });
            CACHE.Data.researchType["b" + CACHE.Data.researchTypeBuilding[i].id] = CACHE.Data.researchTypeBuilding[i];
        }

        // Unit
        for (var i = 0, il = CACHE.Data.researchTypeUnit.length; i < il; ++i) {
            CACHE.Data.researchTypeUnit[i].rilist = CACHE.Data.researchTypeUnit[i].rilist.filter(function (id) {
                return CACHE.Data.researchItem[id].cost < 1000000000;
            });
            CACHE.Data.researchType["u" + CACHE.Data.researchTypeUnit[i].id] = CACHE.Data.researchTypeUnit[i];
        }

        // Misc
        for (var i = 0, il = CACHE.Data.researchTypeMisc.length; i < il; ++i) {
            CACHE.Data.researchTypeMisc[i].rilist = CACHE.Data.researchTypeMisc[i].rilist.filter(function (id) {
                return CACHE.Data.researchItem[id].cost < 1000000000;
            });
            CACHE.Data.researchType["m" + CACHE.Data.researchTypeMisc[i].id] = CACHE.Data.researchTypeMisc[i];
        }

        sync();
    }

    function load_onDataFailure(data) {
        //
    }

    function sync() {
        /// <summary>
        /// Syncs research data.
        /// </summary>
        syncResearchStatus();
    }

    function syncResearchStatus() {
        /// <summary>
        /// Syncs research status data.
        /// </summary>
        Frame.busy('Updating research status.', 10000, $('#researchDialog'));
        ROE.Api.call(CONST.Data.researchStatus, {
            // Param
        },
        syncResearchStatus_onDataSuccess,
        syncResearchStatus_onDataFailure);
    }

    function syncResearchStatus_onDataSuccess(data) {

        CACHE.Data.researchStatusResearch = data.inProgress;
        CACHE.Data.researchStatusComplete = data.completedResearch;
        CACHE.Data.hoursBehind = data.hoursBehind;

        //create an easy to use object to grab 
        /*
        CACHE.Data.researchItemsInProgress = {};
        for (var p = 0; p < CACHE.Data.researchStatusResearch.length; p++) {
            CACHE.Data.researchItemsInProgress[CACHE.Data.researchStatusResearch[p].riid] = { completesOn: CACHE.Data.researchStatusResearch[p].completesOn }
        }
        */

        var researchType,
            researchItem,
            researchTypeStatus;

        // Sort into buckets by status
        // Building
        for (var i = 0, il = CACHE.Data.researchTypeBuilding.length; i < il; ++i) {
            researchType = CACHE.Data.researchTypeBuilding[i];

            researchTypeStatus = {
                research: [],
                complete: [],
                locked: [],
                available: [],
                listed: null
            };

            for (var j = 0, jl = researchType.rilist.length; j < jl; ++j) {
                researchItem = CACHE.Data.researchItem[researchType.rilist[j]];

                if (isStatusResearch(researchItem.id)) {
                    researchTypeStatus.research.push(researchItem.id);
                }
                else if (isStatusComplete(researchItem.id)) {
                    researchTypeStatus.complete.push(researchItem.id);
                }
                else if (isStatusLocked(researchItem.id)) {
                    researchTypeStatus.locked.push(researchItem.id);
                }
                else {
                    researchTypeStatus.available.push(researchItem.id);
                }
            }

            researchTypeStatus.listed = researchTypeStatus.available;

            CACHE.Data.researchTypeStatus["b" + researchType.id] = researchTypeStatus;
        }

        // Sort into buckets by status
        // Unit
        for (var i = 0, il = CACHE.Data.researchTypeUnit.length; i < il; ++i) {
            researchType = CACHE.Data.researchTypeUnit[i];

            researchTypeStatus = {
                research: [],
                complete: [],
                locked: [],
                available: [],
                listed: null
            };

            for (var j = 0, jl = researchType.rilist.length; j < jl; ++j) {
                researchItem = CACHE.Data.researchItem[researchType.rilist[j]];

                if (isStatusResearch(researchItem.id)) {
                    researchTypeStatus.research.push(researchItem.id);
                }
                else if (isStatusComplete(researchItem.id)) {
                    researchTypeStatus.complete.push(researchItem.id);
                }
                else if (isStatusLocked(researchItem.id)) {
                    researchTypeStatus.locked.push(researchItem.id);
                }
                else {
                    researchTypeStatus.available.push(researchItem.id);
                }
            }

            researchTypeStatus.listed = researchTypeStatus.available;

            CACHE.Data.researchTypeStatus["u" + researchType.id] = researchTypeStatus;
        }

        // Sort into buckets by status
        // Misc
        for (var i = 0, il = CACHE.Data.researchTypeMisc.length; i < il; ++i) {
            researchType = CACHE.Data.researchTypeMisc[i];

            researchTypeStatus = {
                research: [],
                complete: [],
                locked: [],
                available: [],
                listed: null
            };

            for (var j = 0, jl = researchType.rilist.length; j < jl; ++j) {
                researchItem = CACHE.Data.researchItem[researchType.rilist[j]];

                if (isStatusResearch(researchItem.id)) {
                    researchTypeStatus.research.push(researchItem.id);
                }
                else if (isStatusComplete(researchItem.id)) {
                    researchTypeStatus.complete.push(researchItem.id);
                }
                else if (isStatusLocked(researchItem.id)) {
                    researchTypeStatus.locked.push(researchItem.id);
                }
                else {
                    researchTypeStatus.available.push(researchItem.id);
                }
            }

            researchTypeStatus.listed = researchTypeStatus.available;

            CACHE.Data.researchTypeStatus["m" + researchType.id] = researchTypeStatus;
        }


        Frame.free($('#researchDialog'));
        syncResearchMember();

        ///this will hold a copy of all research items from cache
        ResearchItems = $.extend({}, CACHE.Data.researchItem);
        buildTechTree();

    }

    function syncResearchStatus_onDataFailure(data) {
        //
    }

    function syncResearchMember() {
        /// <summary>
        /// Syncs research member data.
        /// </summary>
        Frame.busy('Updating research faculty.', 10000, $('#researchDialog'));
        ROE.Api.call(CONST.Data.researchMember, {
            // Param
        },
        syncResearchMember_onDataSuccess,
        syncResearchMember_onDataFailure);
    }

    function syncResearchMember_onDataSuccess(data) {
        Frame.free($('#researchDialog'));
        CACHE.Data.researchMember = data.researchers;
        CACHE.Data.researchMemberMax = data.maxResearchers;
        list();
    }

    function syncResearchMember_onDataFailure(data) {
        //
    }

    function research(id) {
        /// <summary>
        /// Begins researching the research item with the specified ID "id".
        /// </summary>
        /// <param name="id" type="Number">The ID of the research item.</param>
        /// <param name="sure" type="Boolean">[Optional] Whether the user has confirmed he is sure he wants to begin researching the research item.</param>

        if (!isStatusAvailable(id)) {
            return;
        }

        var researchItem = CACHE.Data.researchItem[id];
        
        Frame.busy('Starting ' + researchItem.name + ' research.', 5000,$('#researchDialog'));

        Api.call(CONST.Data.researchAction, {
            rid: id,
            vid: ROE.SVID
        },
        research_onDataSuccess,
        research_onDataFailure);

    }

    function research_onDataSuccess(data) {
        Frame.free($('#researchDialog'));

        if (data) {
            if (data == 1) {
                // Already researched
                var junk = 1; // added this DUMMY code only to avoid a compile time warning that this code has no side effects
                //BDA.Console.verbose('RSC', 'Already researched');
            }
            else if (data == 2) {
                // Requirements not met
                var junk = 1; // added this DUMMY code only to avoid a compile time warning that this code has no side effects
                //BDA.Console.verbose('RSC', 'req not met');
            }
            else if (data == 3) {
                // All researchers busy
                var junk = 1; // added this DUMMY code only to avoid a compile time warning that this code has no side effects
                //BDA.Console.verbose('RSC', 'All researchers busy');
            }
        }
        else {
            onResearch();
        }
    }

    function research_onDataFailure(data) {
        //BDA.Console.verbose('RSC', 'research failed ' + data);
    }

    function list() {
        /// <summary>
        /// Lists research data.
        /// </summary>

        CACHE.Selector.listResearchType.each(function (i, o) {
            listResearchType($(this));
        });

        _listBonuses();
        
        CACHE.Selector.listResearchMember.each(function (i, o) {
            listResearchMember($(this));
        });

        //Single researcher mode
        if (CACHE.Data.researchMemberMax == 1) {
            $('#research').addClass('singleResearcher');

            //the speedup rewards panel
            var researcHeader = $('#research .themeM-panel.header');
            $('.speedUpItemRewards', researcHeader).remove();


            var rItem = CACHE.Data.researchStatusResearch[0];

            if (rItem) {

                if (ROE.isMobile) {
                    var researchOptionsBtn = $('<div class="researchOptionsBtn"></div>').click(function () {
                        $('.researchOptionsContainer', researcHeader).toggleClass('open');
                    });
                    var researchOptionsContainer = $('<div class="researchOptionsContainer"></div>');
                    $(researcHeader).append(researchOptionsBtn, researchOptionsContainer);
                }

                if (ROE.Items2.existsItemOfType('researchspeedup')) {
                    var btnSpeedUpRewards = $('<div class="speedUpItemRewards BtnBLg1 fontButton1L" onclick="ROE.Items2.showPopup(null,\'researchspeedup\');">SpeedUp Rewards<div class="icon"></div></div>');
                    if (ROE.isD2) {
                        $(researcHeader).append(btnSpeedUpRewards);
                    } else {
                        $('.researchOptionsBtn', researcHeader).addClass('display');
                        researchOptionsContainer.append('<div class="btnExplanation fontSilverFrSClrg">Use your collected rewards to speed up your research.</div>');
                        researchOptionsContainer.append(btnSpeedUpRewards);
                    }
                }

            }

            //if an item is being researched, AND there is some missed hours
            if (rItem && CACHE.Data.hoursBehind > 0.017 /* if more than a minute */) {

                var curResearchFinishesOn = rItem.completesOn;
                var hoursOfCurResearchLeft = ((curResearchFinishesOn - (new Date()).getTime()) / (1000 * 60 * 60)); //how many hours left of hte item currently being researched
                var hoursCanSpeedup = Math.min(hoursOfCurResearchLeft, CACHE.Data.hoursBehind);
                var costPerHour = 2; //servant cost per hour to catchup
                var cost = Math.max(Math.ceil(hoursCanSpeedup * costPerHour), 1); //cost in servats of finishing current item

                $('#research').addClass('canCatchup');

                var btnCatchup = $('<div class="hoursBehindCatchup BtnBLg1 fontButton1L" >Catch Up<div class="icon"></div></div>');

                btnCatchup.click(function () {
                    ROE.Frame.simplePopopOverlay('https://static.realmofempires.com/images/icons/researchCatchup.png', 'Research Catchup', '', 'hoursBehindCatchupOverlay', $('#research'));
                    var catchupWrapper = $('<div class="fontSilverFrLClrg catchupWrapper">');
                    var catchupText = $('<div class="catchupText">');
                    catchupText.append('You have missed out on ' + CACHE.Data.hoursBehind.toFixed(2) + ' hours of research.' +
                        '<br>This is time that your researchers have sat idle.' +
                        '<br>You can use portions of this banked time to speedup or finish the current researchering item.' +
                        '<br>This costs '+cost+' servants.');
                    catchupWrapper.append(catchupText);

                    //if enugh credits to speedup
                    if (cost <= ROE.Player.credits) {

                        var doCatchUpBtn = $('<div class="BtnBXLg1 fontButton1L doCatchUpBtn"></div>');
                        
                        if (CACHE.Data.hoursBehind >= hoursOfCurResearchLeft) {
                            doCatchUpBtn.html('Finish research: <span class="cost">' + cost + '</span>');
                        } else {
                            doCatchUpBtn.html('Speed up: <span class="cost">' + cost + '</span>');
                        }

                        doCatchUpBtn.click(function () {

                            var btn = $(this);
                            if (!btn.hasClass('areyousure')) {
                                btn.attr('data-label', btn.html());
                                btn.addClass('areyousure').html("Sure?");
                                window.setTimeout(function () {
                                    btn.removeClass('areyousure').html(btn.attr('data-label'));
                                }, 2500);
                                return;
                            }
                            $('.hoursBehindCatchupOverlay').remove();
                            Frame.busy("Recovering lost research...", 5000, $('#researchDialog'));
                            ROE.Api.call_research_catchup(function call_research_catchupCallback(data) {
                                Frame.free($('#researchDialog'));
                                sync();
                                ROE.Frame.refreshQuestCount();
                                ROE.Frame.refreshPFHeader(data.playersCreditsNow);
                            });

                        });
                        catchupWrapper.append(doCatchUpBtn);

                    } else {
                        //if not enough credits
                        var doCatchUpBtn = $('<div class="BtnBXLg1 fontButton1L doCatchUpBtn">Hire Servants!</div>').click(function () {
                            $('.hoursBehindCatchupOverlay').remove();
                            ROE.Frame.showBuyCredits();
                        });
                        catchupWrapper.append(doCatchUpBtn);
                    }

                    $('.hoursBehindCatchupOverlay .pContent').append(catchupWrapper);

                });
                

                if (ROE.isD2) {
                    $(researcHeader).append(btnCatchup);
                } else {
                    $('.researchOptionsBtn', researcHeader).addClass('display');
                    researchOptionsContainer.append('<div class="btnExplanation fontSilverFrSClrg">Use your lost research time to speed up your research and catchup.</div>');
                    researchOptionsContainer.append(btnCatchup);
                }

            } else {
                $('#research').removeClass('canCatchup');
                $('#research .hoursBehindCatchup').remove();
            }




        }

    }

    function listResearchType(node) {
        /// <summary>
        /// Populates the specified element "node" with list of research types.
        /// </summary>
        /// <param name="node" type="jQuery">The element to populate.</param>

        node.children(":not(." + CONST.CssClass.template + ")").remove();

        var template = node.children(),
            researchType,
            researchTypeStatus,
            o;

        // Building
        for (var i = 0, il = CACHE.Data.researchTypeBuilding.length; i < il; ++i) {
            researchType = CACHE.Data.researchTypeBuilding[i];
            researchTypeStatus = CACHE.Data.researchTypeStatus["b" + researchType.id];

            var researchPercent = ((researchTypeStatus.complete.length / researchType.rilist.length) * 100);

            if (!researchType.rilist.length) {
                continue;
            }

            o = template.clone()
                .removeClass(CONST.CssClass.template)
                .attr(CONST.DataAttr.id, "b" + researchType.id);

            if (researchPercent < 100) {
                o.addClass(CONST.CssClass.some);
            } else {
                o.addClass(CONST.CssClass.none);
            }

            o.find(CONST.Selector.icon)
                .addClass(CONST.CssClass.building)
                .children("img")
                    .attr("src", CACHE.Data.entityBuilding[researchType.id].IconUrl_ThemeM);

            o.find(CONST.Selector.more)
                .children("span")
                    .text(researchTypeStatus.listed.length);

            o.find(CONST.Selector.name)
                .children("span")
                    .text(CACHE.Data.entityBuilding[researchType.id].Name);

            o.find(CONST.Selector.progress)
                .css("width", (researchPercent + "%"));

            o.appendTo(template.parent());

        }

        // Unit
        for (var i = 0, il = CACHE.Data.researchTypeUnit.length; i < il; ++i) {
            researchType = CACHE.Data.researchTypeUnit[i];
            researchTypeStatus = CACHE.Data.researchTypeStatus["u" + researchType.id];

            var researchPercent = ((researchTypeStatus.complete.length / researchType.rilist.length) * 100);

            if (!researchType.rilist.length) {
                continue;
            }

            o = template.clone()
                .removeClass(CONST.CssClass.template)
                .attr(CONST.DataAttr.id, "u" + researchType.id);

            if (researchPercent < 100) {
                o.addClass(CONST.CssClass.some);
            } else {
                o.addClass(CONST.CssClass.none);
            }

            o.find(CONST.Selector.icon)
                .addClass(CONST.CssClass.unit)
                .children("img")
                    .attr("src", CACHE.Data.entityUnit[researchType.id].IconUrl_ThemeM);

            o.find(CONST.Selector.more)
                .children("span")
                    .text(researchTypeStatus.listed.length);

            o.find(CONST.Selector.name)
                .children("span")
                    .text('Unlock '+CACHE.Data.entityUnit[researchType.id].Name);

            o.find(CONST.Selector.progress)
                .css("width", (researchPercent + "%"));

            o.appendTo(template.parent());

        }


        // Misc groups
        for (var i = 0, il = CACHE.Data.researchTypeMisc.length; i < il; ++i) {
            researchType = CACHE.Data.researchTypeMisc[i];
            researchTypeStatus = CACHE.Data.researchTypeStatus["m" + researchType.id];

            var researchPercent = ((researchTypeStatus.complete.length / researchType.rilist.length) * 100);

            if (!researchType.rilist.length) {
                continue;
            }

            o = template.clone()
                .removeClass(CONST.CssClass.template)
                .attr(CONST.DataAttr.id, "m" + researchType.id);

            if (researchPercent < 100) {
                o.addClass(CONST.CssClass.some);
            } else {
                o.addClass(CONST.CssClass.none);
            }

            o.find(CONST.Selector.icon)
                .addClass(CONST.CssClass.unit)
                .children("img")
                    .attr("src", getMiscResearchGroupTypeIconByID(researchType.id));

            o.find(CONST.Selector.more)
                .children("span")
                    .text(researchTypeStatus.listed.length);

            o.find(CONST.Selector.name)
                .children("span")
                    .text(getMiscResearchGroupTypeNameByID(researchType.id));

            o.find(CONST.Selector.progress)
                .css("width", (researchPercent + "%"));

            o.appendTo(template.parent());

        }


    }

    //loops through building and misc research, adds the bonuses up for each strand, and displays them
    function _listBonuses() {
        var compOBJ = {};
        var compItem;
        for (var i = 0; i < CACHE.Data.researchStatusComplete.length; i++) {
            compItem = ROE.Entities.Research.Items[CACHE.Data.researchStatusComplete[i]];
            if (compItem.effectedBuildingTypeID) { //if its a building related research
                if (!compOBJ["b"+compItem.effectedBuildingTypeID]) {
                    compOBJ["b" + compItem.effectedBuildingTypeID] = 0;
                }
                compOBJ["b" + compItem.effectedBuildingTypeID] += compItem.buildingPercBonus * 100;
            } else if (compItem.propertyID) { //if its misc category
                if (!compOBJ["m" + compItem.propertyID]) {
                    compOBJ["m" + compItem.propertyID] = 0;
                }
                compOBJ["m" + compItem.propertyID] += compItem.propertyValue*100;
            }
        }

        $('#research .researchType li').each(function (i, o) {
            var strandID = $(this).attr('data-id');
            var curBonusElement = $('.curBonus', $(this));
            if (compOBJ[strandID]) {
                curBonusElement.html("Current Effect: +"+compOBJ[strandID]+"%");
            } else {
                curBonusElement.empty();
            }
        });
    }

    function getMiscResearchGroupTypeIconByID(researchPropertyID) {
        switch (researchPropertyID) {
            case 12:
                return 'https://static.realmofempires.com/images/icons/M_PF_Defense.png';
            case 13:
                return 'https://static.realmofempires.com/images/icons/M_PF_Attack.png';
        }
    }


    function getMiscResearchGroupTypeNameByID(researchPropertyID) {
        switch (researchPropertyID) {
            case 12:
                return 'Village Defense Factor';
            case 13:
                return 'Troop Attack Factor';
        }
    }

    function listResearchMember(node) {
        node.children(":not(." + CONST.CssClass.template + ")").remove();

        var template = node.children(),
            researchMember,
            researchItem,
            o;

        var t;
        
        // Busy
        for (var i = 0, il = CACHE.Data.researchStatusResearch.length; i < il; ++i) {
            researchMember = CACHE.Data.researchMember[i];
            researchItem = CACHE.Data.researchItem[CACHE.Data.researchStatusResearch[i].riid];

            o = template.clone()
                .removeClass(CONST.CssClass.template)
                .attr(CONST.DataAttr.id, i)
                .addClass(CONST.CssClass.busy);

            var avatar = o.find(CONST.Selector.avatar),
                status = o.find(CONST.Selector.status);
            if (researchMember) {
                avatar.css("background-image", "url('" + researchMember.url + "')")
            }

            status.attr('data-rid', researchItem.id);
            status.find(CONST.Selector.name).children("span").text(researchItem.name);
            //status.find(CONST.Selector.icon).css("background-image", "url('" + researchItem.image + "')");
            status.find(CONST.Selector.icon).css("background-position", "-"+(researchItem.shX * 0.44) + "px -" + (researchItem.shY * 0.44)  + "px");

            if (researchItem.effectedBuildingTypeID != 0) {
                var bonus = Math.round(researchItem.buildingPercBonus * 100) + "% ";
                bonus = bonus + researchDesc.buildingEffects[researchItem.effectedBuildingTypeID];
                status.find('.bonus').html(bonus);
            } else if (researchItem.propertyID != 0) {
                var bonus = Math.round(researchItem.propertyValue * 100) + "% ";
                bonus = bonus + researchDesc.miscEffects[researchItem.propertyID];
                status.find('.bonus').html(bonus);
            } else {
                var unlocks = researchDesc.unitEffects[researchItem.unlocksUnitTypeID];
                status.find('.bonus').addClass('unlocks').html(unlocks);
            }
            
            //only if its not completed do we setup the following timers, prevents timer craziness
            var completionTime = CACHE.Data.researchStatusResearch[i].completesOn;
            if (completionTime > (new Date()).getTime()) {
                status.find(CONST.Selector.time)
                .children("span")
                .addClass(CONST.CssClass.countdownTimer)
                .attr(CONST.DataAttr.countdownTimer, "window.ROE.Research._onComplete(" + researchItem.id + ");")
                .attr('data-finisheson', CACHE.Data.researchStatusResearch[i].completesOn);
            } else {
                status.find(CONST.Selector.time).children("span").html('finished');
            }
            
            o.appendTo(template.parent());
        }

        // Free
        for (var i = CACHE.Data.researchStatusResearch.length, il = CACHE.Data.researchMember.length; i < il; ++i) {
            researchMember = CACHE.Data.researchMember[i];

            o = template.clone()
                .removeClass(CONST.CssClass.template)
                .attr(CONST.DataAttr.id, i)
                .addClass(CONST.CssClass.free);

            var avatar = o.find(CONST.Selector.avatar);

            if (researchMember) {
                avatar.css("background-image", "url('" + researchMember.url + "')")
            }

            o.appendTo(template.parent());
        }

        // None
        for (var i = CACHE.Data.researchMember.length, il = CACHE.Data.researchMemberMax; i < il; ++i) {
            researchMember = CACHE.Data.researchMember[i];
            o = template.clone()
                .removeClass(CONST.CssClass.template)
                .attr(CONST.DataAttr.id, i)
                .addClass(CONST.CssClass.none);

           
            var avatar = o.find(CONST.Selector.avatar);
            if (researchMember) {              
                avatar.css("background-image", "url('" + researchMember.url + "')"); //
            } else {
               
                avatar.css("background-image", "url('https://static.realmofempires.com/images/emptyresearcher.png')");
                avatar.css("background-color", "#B9B9B9");
            }
            o.appendTo(template.parent()); 
        }

        CACHE.Selector.header
            .find(CONST.Selector.researchMemberBusy)
                .children("span")
                    .text(CACHE.Data.researchStatusResearch.length);

        CACHE.Selector.header
            .find(CONST.Selector.researchMemberFree)
                .children("span")
                    .text(CACHE.Data.researchMember.length - CACHE.Data.researchStatusResearch.length);

        anyResearcherAvailable = (Math.min(CACHE.Data.researchMember.length,CACHE.Data.researchMemberMax) - CACHE.Data.researchStatusResearch.length) > 0;

        CACHE.Selector.header
            .find(CONST.Selector.researchMemberNone)
                .children("span")
                    .text(CACHE.Data.researchMemberMax - CACHE.Data.researchMember.length);

        Timer.initTimers();
    }

    function viewHome() {
        /// <summary>
        /// Displays the default view.
        /// </summary>

        if (ROE.isMobile) {
            if (_busy || _view == CACHE.Selector.viewMaster) { return; }
            ++_busy;
            Transition.slideRight(CACHE.Selector.viewMaster, _view, function () {
                $('#mainpane').empty();
                --_busy;
            });      
        }else{
            $(CACHE.Selector.viewDetail).stop().animate({ 'left': '-100%' }, 333);
            $(CACHE.Selector.viewMaster).stop().animate({ 'left': '0px' }, 333, function () {
                $('#mainpane').empty();
            });           
        }
        _view = CACHE.Selector.viewMaster;
    }


    function viewDetail() {
        /// <summary>
        /// Switches to the item detail view.
        /// </summary>

        if (_busy || _view == CACHE.Selector.viewDetail) { return; }
        ++_busy;

        if (ROE.isMobile) {

            Transition.slideLeft(CACHE.Selector.viewDetail, _view, function () {
                CACHE.Selector.page.attr('data-tutorial-state', 'detail');
                _centerOnFirstAvailable();
                --_busy;
            });
        } else {
            $(CACHE.Selector.viewMaster).stop().animate({ 'left': '100%' }, 333);
            $(CACHE.Selector.viewDetail).stop().animate({ 'left': '0px' }, 333, function () {
                CACHE.Selector.page.attr('data-tutorial-state', 'detail');
                _centerOnFirstAvailable();
                --_busy;
            });
        }
        _view = CACHE.Selector.viewDetail;
    }


    function _centerOnFirstAvailable() {

        var mainPane = $('#mainpane');

        if (_focusOnId) {
            var selectedNode = mainPane.find('#cDiv' + _focusOnId);
            if (selectedNode.length) {
                mainPane.animate({
                    scrollLeft: selectedNode.position().left - (mainPane.width() / 2) + selectedNode.width() / 2,
                    scrollTop: selectedNode.position().top - (mainPane.height() / 2) + selectedNode.height() / 2
                }, 750, "easeInOutSine", function () {
                    selectedNode.click();
                });
            }
        } else {
            var firstNode = mainPane.find('.nodeContentDiv:not(.rBoxComplete):not(.rBoxLocked)').first();
            if (firstNode.length) {
                mainPane.animate({
                    scrollLeft: firstNode.position().left - (mainPane.width() / 2) + firstNode.width() / 2,
                    scrollTop: firstNode.position().top - (mainPane.height() / 2) + firstNode.height() / 2
                }, 750, "easeInOutSine");
            }
        }

        _focusOnId = null;
    }

    function isStatusResearch(id) {
        /// <summary>
        /// Gets whether the research status of the research item with the specified ID "id" is "research".
        /// </summary>
        /// <param name="id" type="Number">The ID of the research item.</param>
        /// <returns type="Boolean">True if the research status of the research item is "research", false otherwise.</returns>

        for (var i = 0, il = CACHE.Data.researchStatusResearch.length; i < il; ++i) {
            if (CACHE.Data.researchStatusResearch[i].riid == id) {
                return true;
            }
        }

        return false;
    }

    function isStatusComplete(id) {
        /// <summary>
        /// Gets whether the research status of the research item with the specified ID "id" is "complete".
        /// </summary>
        /// <param name="id" type="Number">The ID of the research item.</param>
        /// <returns type="Boolean">True if the research status of the research item is "complete", false otherwise.</returns>

        for (var i = 0, il = CACHE.Data.researchStatusComplete.length; i < il; ++i) {
            if (CACHE.Data.researchStatusComplete[i] == id) {
                return true;
            }
        }

        return false;
    }

    function isStatusLocked(id) {
        /// <summary>
        /// Gets whether the research status of the research item with the specified ID "id" is "locked".
        /// </summary>
        /// <param name="id" type="Number">The ID of the research item.</param>
        /// <returns type="Boolean">True if the research status of the research item is "locked", false otherwise.</returns>

        var researchItem = CACHE.Data.researchItem[id];

        if (!researchItem.parents) {
            return false;
        }

        for (var i = 0, il = researchItem.parents.length; i < il; ++i) {
            if (!isStatusComplete(researchItem.parents[i])) {
                return true;
            }
        }

        return false;
    }

    function isStatusAvailable(id) {
        /// <summary>
        /// Gets whether the research status of the research item with the specified ID "id" is "available".
        /// </summary>
        /// <param name="id" type="Number">The ID of the research item.</param>
        /// <returns type="Boolean">True if the research status of the research item is "available", false otherwise.</returns>

        var researchItem = CACHE.Data.researchItem[id];

        if (isStatusResearch(id)) {
            return false;
        }

        if (isStatusComplete(id)) {
            return false;
        }

        if (isStatusLocked(id)) {
            return false;
        }

        return true;
    }

    function onLoad() {
        /// <summary>
        /// Performs actions after research data is loaded.
        /// </summary>

        Frame.free($('#researchDialog'));
    }

    function onResearch(id) {
        /// <summary>
        /// Performs actions after researching a research item begins.
        /// </summary>
        /// <param name="id" type="Number">The ID of the research item.</param>

        CACHE.Selector.page.attr('data-tutorial-state', 'research');

        sync();

        Frame.reloadFrame();
    }

    function onComplete(id) {
        /// <summary>
        /// Performs actions after researching a research item ends.
        /// </summary>
        /// <param name="id" type="Number">The ID of the research item.</param>

        //this busy is redundent, synch has its own busy.
        //Frame.busy('Research completed. Updating information.', 8000, $('#researchDialog'));

        sync();
        ROE.Frame.refreshQuestCount();
        Frame.reloadFrame();
    }


    /*********************************************************************/
    /********************INVITE RESEARCHERS POPUP ************************/
    /*********************************************************************/

    var showBuyInvitePanel = function () {        
        popupModalOverlay("buyres", "", 10, 10);

        content = $(BDA.Templates.get('BuyInviteResearcher', ROE.realmID, {}));

        var popupContent = $("#" + ROE.Frame.CONSTS.popupNameIDPrefix + 'buyres' + ' .popupBody');

        popupContent.append(content);

        return false;
    };

    // Re-styled invite panel
    var showBuyInvitePanel2 = function (researcherIndex) {

        if (!CACHE.hireResearcherTemplate) {
            Frame.busy(undefined, undefined, $('#researchDialog'));
            var temp = BDA.Templates.getRawJQObj("HireResearcherTempl", ROE.realmID);           
            Frame.free($('#researchDialog'));
            CACHE.hireResearcherTemplate = temp;
        }
        var data = {};
        if(researcherIndex && researcherIndex > 0) { // Should test for max too? What is max??
            data.researcherAvatar = "https://static.realmofempires.com/images/research/researcher0" + researcherIndex + ".png";           
        } else {
            data.researcherAvatar = "https://static.realmofempires.com/images/research/researcher01.png";
        }

        var html = BDA.Templates.populate(CACHE.hireResearcherTemplate[0].outerHTML, data);
        

        
        ROE.Frame.simplePopopOverlay('https://static.realmofempires.com/images/icons/M_ResearchList.png', "Hire Reseacher", html, 'research_hireResearcherPopup', $('#researchDialog'));

        if (ROE.realmID >= 76) {
            $('.research_hireResearcherPopup .hireResearcherP').html('SUMMER SALE!<br/>Hire researcher for 1 Servant!');
        }
    }

    function _hireResearcher() {
        Frame.busy("Searching amongst Academia...", 5000, $('#researchDialog'));
        ROE.Api.call("buyResearcher", null, _onAPISuccess_hireResearcher);
    }

    function _onAPISuccess_hireResearcher(data) {
        Frame.free($('#researchDialog'));

        switch (data.status) {
            case "OK":
                // Completed hire, got new researcher
               
                $(".research_hireResearcherPopup .hireResearcher").hide();
                $(".research_hireResearcherPopup .researcherHired").fadeIn();
                ROE.Frame.refreshPFHeader(data.servants);
                ROE.Research.synch();
                break;
            case "lackservants":
                // Show: Not enough servants               
                $(".research_hireResearcherPopup .hireResearcher").hide();
                $(".research_hireResearcherPopup .notEnough").fadeIn();               
                break;
            case "maxresearchers":
                // Have maximum researchers              
                $(".research_hireResearcherPopup .hireResearcher").hide();
                $(".research_hireResearcherPopup .maxResearchers").fadeIn();               
                break;
            case "otherfailure":
                // Some other failure...               
                //$(".research_hireResearcherPopup .hireResearcher").hide();
                //$(".research_hireResearcherPopup .otherFailure").fadeIn();

                // Originally this is what happened on otherfailure
                // so lets keep it the same.
                $(".research_hireResearcherPopup .hireResearcher").hide();
                $(".research_hireResearcherPopup .notEnough").fadeIn();
                break;

        }
    }



    /********************   NEW SECTION   *********************/
    /********************   NEW SECTION   *********************/
    /********************   NEW SECTION   *********************/


    function buildTechTree() {

        if (_currentRenderedStrand == null) {
            //BDA.Console.verbose('RSC', 'BuildTree ABORT: user hasnt selected a strand');
            return;
        }

        if (!queueCompleted) {
            //BDA.Console.verbose('RSC', 'BuildTree ABORT: queue not completed');
            return;
        }

        //BDA.Console.verbose('RSC', 'BuildTree: Good to Start');
        //var miliStart = new Date().getTime();

        var rType = _currentRenderedStrand.charAt(0);
        var strandId = _currentRenderedStrand.slice(1, _currentRenderedStrand.length);
        ///updates the icon and text of the strand, shown above of the tree
        updateStrandInfo(rType, strandId);
        //BDA.Console.verbose('RSC', 'updated StrandInfo');

        ///reset and rebuild the tierObject, responsible for node placement, to ensure nothing overlaps        
        for (var i = 0; i < 20; i++) {
            tierObject[i] = {
                x: i * (researchBoxWidth + researchBoxTierGap),
                members: 0,
                //rIds: []
            }
        }
        tierObject['highestTier'] = 0;
        tierObject['highestRow'] = 0;

        ///the collection of Canvas research node "entity" objects
        researchBoxes = {};

        ///this will hold all items from ResearchItems that belong to a particular research strand
        var itemsInThisResearchStrand = {};

        //BDA.Console.verbose('RSC', 'starting build and place');
        ///b for build tech and u for unit tech

        if (rType == "b") {
            for (var item in ResearchItems) {
                if (ResearchItems[item].effectedBuildingTypeID == strandId) {
                    itemsInThisResearchStrand[item] = $.extend({}, ResearchItems[item]);
                }
            }
            for (var item in itemsInThisResearchStrand) {
                if (itemsInThisResearchStrand[item].parents != null) {
                    itemsInThisResearchStrand[item].parents = itemsInThisResearchStrand[item].parents.filter(function (parent) {
                        return ResearchItems[parent].effectedBuildingTypeID == strandId;
                    });
                    if (itemsInThisResearchStrand[item].parents.length < 1) {
                        itemsInThisResearchStrand[item].parents = null;
                    }
                }
                //after the above operation, if item parent(s) == null, we build them as root items
                if (itemsInThisResearchStrand[item].parents == null) {
                    buildAndPlaceResearchNode(itemsInThisResearchStrand[item], strandId, 0, itemsInThisResearchStrand, rType);
                }
            }
        } else if (rType == "u") {
            for (var item in ResearchItems) {
                if (ResearchItems[item].unlocksUnitTypeID == strandId) {
                    itemsInThisResearchStrand[item] = $.extend({}, ResearchItems[item]);
                }
            }
            for (var item in itemsInThisResearchStrand) {
                if (itemsInThisResearchStrand[item].parents != null) {
                    itemsInThisResearchStrand[item].parents = itemsInThisResearchStrand[item].parents.filter(function (parent) {
                        return ResearchItems[parent].unlocksUnitTypeID == strandId;
                    });
                    if (itemsInThisResearchStrand[item].parents.length < 1) {
                        itemsInThisResearchStrand[item].parents = null;
                    }
                }
                //after the above operation, if item parent(s) == null, we build them as root items
                if (itemsInThisResearchStrand[item].parents == null) {
                    buildAndPlaceResearchNode(itemsInThisResearchStrand[item], strandId, 0, itemsInThisResearchStrand, rType);
                }
            }
        
        } else if (rType == "m") {
            for (var item in ResearchItems) {
                if (ResearchItems[item].propertyID == strandId) {
                    itemsInThisResearchStrand[item] = $.extend({}, ResearchItems[item]);
                }
            }
            for (var item in itemsInThisResearchStrand) {
                if (itemsInThisResearchStrand[item].parents != null) {
                    itemsInThisResearchStrand[item].parents = itemsInThisResearchStrand[item].parents.filter(function (parent) {
                        return ResearchItems[parent].propertyID == strandId;
                    });
                    if (itemsInThisResearchStrand[item].parents.length < 1) {
                        itemsInThisResearchStrand[item].parents = null;
                    }
                }
                //after the above operation, if item parent(s) == null, we build them as root items
                if (itemsInThisResearchStrand[item].parents == null) {
                    buildAndPlaceResearchNode(itemsInThisResearchStrand[item], strandId, 0, itemsInThisResearchStrand, rType);
                }
            }
        }
        //BDA.Console.verbose('RSC', 'finished build and place');

        ///this will destroy and rebuild the canvas items to battle Android Canvas Ghosting
        redoCanvases(
            (tierObject.highestTier + 1) * (researchBoxWidth + researchBoxTierGap) + nodeOffsetX,
            tierObject.highestRow * (researchBoxHeight + researchBoxRowGap) + nodeOffsetY
        );
        //BDA.Console.verbose('RSC', 'finished redoCanvases');

        createContentDivs();
        //BDA.Console.verbose('RSC', "finished createContentDivs");

        //BDA.Console.verbose('RSC', 'started updateAndDrawC1');
        updateAndDrawC1();

        //var miliEnd = new Date().getTime();
        //BDA.Console.verbose('RSC', 'Build and Draw finished in ' + (miliEnd - miliStart) + 'ms');
      
        Timer.initTimers();
        viewDetail();
        Frame.free($('#researchDialog'));

        /* this tool helps with editing research times
        var count = 0;
        var outputdiv = $('<div style="position: fixed;background-color: #FFF;z-index: 99999999;color: #000;padding: 20px;"></div>').dblclick(function () { $(this).remove(); }).appendTo('body');
        var ticks = [600000000, 1200000000, 3000000000, 18000000000, 288000000000, 864000000000, 1728000000000, 2592000000000, 4320000000000, 6048000000000];
        for (t in tierObject) {
            var tierRIds = tierObject[t].rIds;
            if (!tierRIds) { continue; }
            for (var i = 0; i < tierRIds.length; i++) {
                outputdiv.append('update ResearchItems set ResearchTime = ' + ticks[count] + ' where ResearchItemID = ' + tierRIds[i] + '<br/>');
            }
            count++;
        }
        */
    }

    function updateStrandInfo(rType, strandId) {
        var name, src;
        if (rType == "b") {
            name = CACHE.Data.entityBuilding[strandId].Name;
            src = CACHE.Data.entityBuilding[strandId].IconUrl_ThemeM;
        } else if (rType == "u") {
            name = 'Unlock ' + CACHE.Data.entityUnit[strandId].Name;
            src = CACHE.Data.entityUnit[strandId].IconUrl_ThemeM;
        } else if (rType == "m") {
            name = getMiscResearchGroupTypeNameByID(parseInt(strandId));
            src = getMiscResearchGroupTypeIconByID(parseInt(strandId));
        }
        CACHE.Selector.viewDetail.find('.strandText').html(name);
        CACHE.Selector.viewDetail.find('.strandIcon').css("background-image", 'url(' + src + ')');
    }


    function buildAndPlaceResearchNode(researchItem, strandId, tierNumber, researchItemTypeGroup, rType) {

        if (researchItem == null) {
            return;
        }

        if (researchItem.cost > 0) {
            if (researchItem.cost > 9999999) {
                return; ///this means gov tech, dont render
            }
            researchItem.hasCost = true; ///this means old realm tech
        } else {
            researchItem.hasCost = false; ///this is normal case
        }

        if (typeof (researchItem.placed) == "undefined") {
            researchItem.placed = true;
        } else {
            return;
        }

        researchItem.tier = tierNumber;

        if (isStatusResearch(researchItem.id)) {
            researchItem["status"] = "researching";
        }
        else if (isStatusComplete(researchItem.id)) {
            researchItem["status"] = "complete";
        }
        else if (isStatusLocked(researchItem.id)) {
            researchItem["status"] = "locked";
        }
        else {
            researchItem["status"] = "available";
        }

        ///filter out parents that dont belong in the tech specialization, like if the item is in farming and has a parent in barracks
        if (researchItem.parents != null) {
            if (rType == "b") {
                researchItem.parents = researchItem.parents.filter(function (parent) {
                    return ResearchItems[parent].effectedBuildingTypeID == strandId;
                });
            } else if (rType == "u") {
                researchItem.parents = researchItem.parents.filter(function (parent) {
                    return ResearchItems[parent].unlocksUnitTypeID == strandId;
                });
            
            }  else if (rType == "m") {
                researchItem.parents = researchItem.parents.filter(function (parent) {
                    return ResearchItems[parent].propertyID == strandId;
                });
            }
        }

        placeItem(researchItem);

        ///filter out children that dont belong in the tech specialization, like if parent is silvermine and has a child in farming
        if (researchItem.children != null) {
            if (rType == "b") {
                researchItem.children = researchItem.children.filter(function (child) {
                    return ResearchItems[child].effectedBuildingTypeID == strandId;
                });
            }
            else if (rType == "u")
            {
                researchItem.children = researchItem.children.filter(function (child) {
                    return ResearchItems[child].unlocksUnitTypeID == strandId;
                });
            }
            else if (rType == "m")
            {
                researchItem.children = researchItem.children.filter(function (child) {
                    return ResearchItems[child].propertyID == strandId;
            });
}
            if (researchItem.children.length == 0) {
                researchItem.children = null;
            } else {
                for (var i = 0; i < researchItem.children.length; i++) {
                    var childId = researchItem.children[i];
                    buildAndPlaceResearchNode(researchItemTypeGroup[childId], strandId, tierNumber + 1, researchItemTypeGroup, rType);
                }
            }

        }

        ///add our research object instance to researchBoxes objects
        researchBoxes[researchItem.id] = researchBox(researchItem);
    }

    function placeItem(researchItem) {
        var itemTierNumber = researchItem.tier;
        researchItem.X = nodeOffsetX + tierObject[itemTierNumber].x;
        researchItem.Y = nodeOffsetY + tierObject[itemTierNumber].members * (researchBoxHeight + researchBoxRowGap);
        tierObject[itemTierNumber].members++;
        //tierObject[itemTierNumber].rIds.push(researchItem.id);
        if (tierObject.highestTier < itemTierNumber) {
            tierObject.highestTier = itemTierNumber;
        }
        if (tierObject.highestRow < tierObject[itemTierNumber].members) {
            tierObject.highestRow = tierObject[itemTierNumber].members;
        }
    }

    function redoCanvases(canvasWidth, canvasHeight) {
        ///CANVAS_WIDTH = canvasWidth;
        ///CANVAS_HEIGHT = canvasHeight;
        var svg = $("<svg id='researchSVG'width='" + canvasWidth + "' height='" + canvasHeight + "' ></svg>").click(canvas1Clicked);
        $('#mainpane').html(svg);  
        ///canvasElement = $("<canvas id='gameCanvas' width='" + canvasWidth + "' height='" + canvasHeight + "' ></canvas>").click(canvas1Clicked).appendTo('#mainpane');            
        ///ctx1 = canvasElement.get(0).getContext("2d");
    }

    function canvas1Clicked(event) {
        //BDA.Console.verbose('RSC', 'Canvas Click');
        event.stopPropagation();
        event.preventDefault();
        clickReset();
        updateAndDrawC1();
    }

    function clickReset() {
        $('#mainpane').find('.InitRsrchHolder').remove();
        //$('#mainpane .cCountdown ').remove();
        $('#mainpane .cAnim').remove();
        for (var boxItem in researchBoxes) {
            var box = researchBoxes[boxItem];
            box.clicked = false;
            box.selected = false;
        }
    }

    function researchBox(entity) {

        entity.active = true;
        entity.clicked = false;
        entity.selected = false;

        entity.W = researchBoxWidth;
        entity.H = researchBoxHeight;
        entity.halfW = entity.W / 2;
        entity.halfH = entity.H / 2;

        entity.X2 = entity.X + entity.W;
        entity.Y2 = entity.Y + entity.H;

        /// DEPENDING ON ID PICK A SPRITE SHEET

        var id = parseInt(entity.id);
        if (id < 450) {
            entity.spriteSheet = researchAssets.rAllIcons;
        } else {
            ///use new sprite sheet . . .
            entity.spriteSheet = researchAssets.rAllIcons; //use same one for now
        }


        if (entity.effectedBuildingTypeID != 0) {
            entity.bonusString = Math.round(entity.buildingPercBonus * 100) + "%";
        } else if (entity.propertyID != 0) {
            entity.bonusString = Math.round(entity.propertyValue * 100) + "%";
        }


        //entity.bonusString = Math.round(entity.buildingPercBonus * 100) + "%";

        entity.draw = function () {

            this.updateContentDiv();
            

            if (this.children != null) {
                var child, strokeString;
                for (var i = 0; i < this.children.length; i++) {
                    child = researchBoxes[this.children[i]];
                    if (child == null) { continue; }
                    if (child.selected) {
                        strokeColor = "#24c6dd";
                        strokeWidth = 3;
                    } else {
                        strokeColor = "rgba(36, 198, 221, 0.3)";
                        strokeWidth = 2;
                    }
                    drawItemConnection(this.X2, this.Y + this.halfH, child.X, child.Y + child.halfH, strokeColor, strokeWidth);
                }
            }

            /*
            ctx1.save();
            if (this.children != null) {
                for (var i = 0; i < this.children.length; i++) {
                    var child = researchBoxes[this.children[i]];
                    if (child == null) { continue; }
                    if (child.selected) {
                        ctx1.lineWidth = 3;
                        ctx1.strokeStyle = "#24c6dd";
                    } else {
                        ctx1.lineWidth = 2;
                        ctx1.strokeStyle = "rgba(36, 198, 221, 0.3)";
                    }
                    drawItemConnection(this.X2, this.Y + this.halfH, child.X, child.Y + child.halfH);
                }
            }
            ctx1.restore();
            */
            //BDA.Console.verbose('RSC', this.name + " id:" + this.id + " done.");
            
        };

        entity.createContentDiv = function () {
            
            var entity = this;
            var ss = this.spriteSheet;
            var ssSrc = "";
            if (ss) { ssSrc = ss.src; }
            var x = '-'+Math.abs(this.shX -3)+"px";
            var y = '-'+Math.abs(this.shY -3)+"px";
            var stDiv = $('#nodeContentDivTemplate').clone();
            if (this.hasCost) {
                stDiv.append('<span class="cCost">Cost: ' + this.cost + '</span>');
            }
            stDiv.attr('id', 'cDiv' + this.id)
                .click(function () { entity.click() })
                .css({
                    'left': this.X, 'top': this.Y,
                    'background-image': 'url("' + ssSrc + '")',
                    'background-position': x + ' ' + y
                })
                .appendTo($('#mainpane'));
        };

        entity.updateContentDiv = function () {
            var cDiv = $('#cDiv' + this.id);
            if (this.bonusString == "0%") { cDiv.addClass('noBonus'); }
            cDiv.find('.cName').html(this.name);
            cDiv.find('.cBonus').html(this.bonusString);
            cDiv.find('.cTime').html(this.time);

            switch (this.status) {
                case "researching":
                    cDiv.removeClass('rBoxComplete rBoxLocked');
                    cDiv.find('.cStatus').html(this.status);
                    //cDiv.append('<span class="cCountdown countdown" data-refreshcall="window.ROE.Research._onComplete('+this.id+');">' +
                    //    ROE.Utils.msToTime(CACHE.Data.researchItemsInProgress[this.id].completesOn) + '</span>');
                    cDiv.append('<span class="cAnim"</span>');
                    break;
                case "complete":
                    cDiv.addClass('rBoxComplete');
                    cDiv.find('.cStatus').empty();
                    break;
                case "locked":
                    cDiv.addClass('rBoxLocked');
                    cDiv.find('.cStatus').empty();
                    break;
                default:
                    cDiv.removeClass('rBoxComplete rBoxLocked');
                    cDiv.find('.cStatus').empty();
                    break;
            }

            if (this.clicked) {
                cDiv.addClass('rBoxClicked');
            } else {
                cDiv.removeClass('rBoxClicked');
            }
            
            if (this.selected) {
                cDiv.addClass('rBoxSelected');
            } else {
                cDiv.removeClass('rBoxSelected');
            }
            
        };

        entity.beginResearch = function () {
            research(this.id);
        }

        entity.click = function () {
            //BDA.Console.verbose('RSC', this.name + " clicked.");
            ROE.UI.Sounds.click();
            clickReset();
            this.clicked = true;
            this.select();
            updateAndDrawC1();
            displayClickedBox(this);
        }

        entity.select = function () {
            this.selected = true;
            if (this.parents != null) {
                for (var i = 0; i < this.parents.length; i++) {
                    if (researchBoxes[this.parents[i]] != null) {
                        researchBoxes[this.parents[i]].select();
                    }
                }
            }
        };

        return entity; 
    }

    function makeSVG(tag, attrs) {
        var el = document.createElementNS('http://www.w3.org/2000/svg', tag);
        for (var k in attrs)
            el.setAttribute(k, attrs[k]);
        return el;
    }


    function drawItemConnection(startX, startY, endX, endY, strokeColor,strokeWidth) {
        var path = makeSVG('path', { d: "M" + startX + "," + startY + " C" + (startX + 20) + "," + startY + " " + (endX - 20) + "," + endY + " " + endX + "," + endY, stroke: strokeColor, "stroke-width": strokeWidth });
        document.getElementById('researchSVG').appendChild(path);
        /*
        ctx1.beginPath();
        ctx1.moveTo(startX, startY);
        ctx1.bezierCurveTo(startX + 20, startY, endX - 20, endY, endX, endY);
        ctx1.stroke();
        */

    }

    function addResearchingAnimation(id, xPos, yPos) {
        var ss = new createjs.SpriteSheet(sp1);
        ss.getAnimation("run").next = "run";
        var tempBitmap = new createjs.BitmapAnimation(ss);
        tempBitmap.x = xPos;
        tempBitmap.y = yPos;
        tempBitmap.name = "anim_rsrchin" + id;
        tempBitmap.scaleX = tempBitmap.scaleY = 0.5;
        tempBitmap.gotoAndPlay("run");
        stage.addChild(tempBitmap);
    }

    function displayClickedBox(entity) {
        var cDiv = $('#cDiv' + entity.id);

        switch (entity.status) {
            case "researching":
                cDiv.find('.cStatus').html(entity.status);
                break;
            case "complete":
                cDiv.find('.cStatus').html(entity.status);
                break;
            case "locked":
                cDiv.find('.cStatus').html(entity.status);
                break;
            default:

                if (!anyResearcherAvailable) {
                    cDiv.find('.cStatus').html('Researchers Busy');
                    return;
                }

                cDiv.find('.cStatus').empty();
                if (entity.cost > _village.coins) {
                    $('<div>').addClass('InitResearchBtn InitRsrchHolder notEnoughSilver').html('Not enough silver').appendTo(cDiv);
                } else {
                    $('<div>').addClass('InitResearchBtn InitRsrchHolder')
                    .click(function () {
                        researchBoxes[entity.id].beginResearch();
                    })
                    .appendTo(cDiv);
                }

                break;
        }
    }


    function updateAndDrawC1() {
        if (/*ctx1 == null ||*/ !queueCompleted) { return; }
        $('#researchSVG').empty();
        //ctx1.clearRect(0, 0, CANVAS_WIDTH, CANVAS_HEIGHT);
        ////BDA.Console.verbose('RSC', "cleared rect");
        for (var researchItem in researchBoxes) {
            researchBoxes[researchItem].draw();
        }
    }

    function createContentDivs() {
        $('#mainpane').find('.nodeContentDiv').remove();
        for (var researchItem in researchBoxes) {
            researchBoxes[researchItem].createContentDiv();
        }
    }

    function _setupD2ResearchScrollDrag() {
        var curDown = false;
        var lastPosX = 0;
        var lastPosY = 0;
        $('#mainpane').css({ 'overflow' : 'hidden' });
        $('#mainpane').mousedown(function (e) {
            curDown = true; 
            var offset = $(this).offset();
            lastPosX = e.pageX - offset.left;
            lastPosY = e.pageY - offset.top;
        });
        $('#mainpane').mouseup(function (e) { curDown = false; });
        $('#mainpane').mouseleave(function (e) { curDown = false; });
        $('#mainpane').mousemove(function (e) {                
            if (curDown === true) {
                var offset = $(this).offset();
                var curX = e.pageX - offset.left;
                var curY = e.pageY - offset.top;
                $('#mainpane').scrollTop($('#mainpane').scrollTop() + (lastPosY - curY));
                $('#mainpane').scrollLeft($('#mainpane').scrollLeft() + (lastPosX - curX));
                lastPosX = curX;
                lastPosY = curY;
            }
        });
    }

    Research.showResearchPopup = _showResearchPopup;
    Research.showResearchPopupBySpecificItem = _showResearchPopupBySpecificItem;
    Research.init = init;
    Research._onResearch = onResearch;
    Research._onComplete = onComplete;
    Research.synch = syncResearchStatus;
    Research.hireResearcher = _hireResearcher;
    
}(window.ROE.Research = window.ROE.Research || {}));
