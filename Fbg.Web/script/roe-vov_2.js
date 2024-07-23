/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.6.4-vsdoc.js" />
/// <reference path="interfaces.js" />
/// <reference path="bda-val.js" />
/// <reference path="bda-ui-anim.js" />
/// <reference path="VoVAnimation.js" />
/// <reference path="http://code.createjs.com/easeljs-0.5.0.min.js" />
/// <reference path="http://code.createjs.com/movieclip-0.5.0.min.js " />
/// <reference path="http://code.createjs.com/tweenjs-0.2.0.min.js" />

ROE.vov = ROE.vov || {};
ROE.vov._stage = null;
ROE.vov._canvases = null;
ROE.vov._animations = {};
ROE.vov._isDay = true;
ROE.vov.anima = {}; //will be array of Smart animated objects
ROE.vov._asynchronousCalls = [];
ROE.vov._crone = null;
ROE.vov._croneTicker = 0;
ROE.vov._animateAnimations = true;
ROE.vov._animationsPaused = false;
ROE.vov._initFunctionList = null;
ROE.vov._globalTickListenerAdded = false;
ROE.vov._cachedVOVDOM = null;

ROE.vov.oninit = [];
BDA.Console.setCategoryDefaultView('VOV2', false);

// Temporarily wrapped in a function. In the future this should hopefully
// be the only implementation, so this wrapping function will be removed.
ROE.vov._newVOVInitialized = false;
ROE.vov.CONST = {
    EnableAnimations: {
        FALSE: 0,
        TRUE: 1,
        DEFAULT: 2
    },
    Selectors: {
        viewContainer: '.TDContent',
        view: '.TDContent > div',
        vovMain: '.TDContent .vovMain',
        vovInner: '#vov',
        background: '.vovMain .bckg',
        canvases: '#vov canvas',
        animationCanvas: '#vov #animationlayer', // Technically the #vov is unnecessary since there should only be one #animationlayer but we will keep it here for consistency
        paintToCanvas: '#vov .vovPaintToCanvas',
        paintToCanvasNonempty: '#vov .vovPaintToCanvas[src!=""]',
        html5Swipe: {
            wrapper: "#vov .html5SwipeWrapper",
            floater: "#vov .html5SwipeFloater"
        },
        troopsAndTrees: {
            all: '#vov .vov_troops',
            byId: function (treeId) { return '#vov #' + treeId; }
        },
        buildings: {
            images: {
                all: '#vov .vovM',
                byId: function (id) { return '#vov .vovM[data-bid=' + id + ']'; }
            },
            scaffolding: {
                all: '#vov .vovC',
                byId: function (id) { return '#vov .vovC[data-bid=' + id + ']'; }
            },
            labels: {
                all: '#vov .vov, #vov .vovC, #vov .level',
                level: {
                    all: '#vov .level',
                    byId: function (id) { return '#vov .level[data-bid=' + id + ']'; }
                },
                buildCounter: {
                    byId: function (id) { return '#vov .buildcount[data-bid=' + id + ']'; }
                },
                hammerIcon: {
                    byId: function (id) { return '#vov .anim[data-bid=' + id + ']'; }
                },
                recruitCounter: {
                    byId: function (id) { return '#vov .recruitcount[data-bid=' + id + ']'; }
                },
                daggerIcon: {
                    byId: function (id) { return '#vov .ranim[data-bid=' + id + ']'; }
                },
                shieldIcon: {
                    byId: function (id) { return '#vov .vovShieldIcon[data-bid=' + id + ']'; }
                }
            }
        },
        qbBtnTimerText: "#openQuickBuildBtn .qbBtnCountdown, #hdrBuild .qbBtnCountdown"
    }
};

ROE.vov._initializeNewVOV = function () {

    if (ROE.vov._newVOVInitialized) {
        return;
    }
    ROE.vov._newVOVInitialized = true;


    // properties:
    //      bg: string
    //      isday: number (WHY???????)
    //      animations: [
    //          type: string
    //          x: number
    //          y: number
    //      ]
    //      buildings: [
    //          id: string
    //          level: number
    //          built: number (HELP ME)
    //          cnstr: number ? ? ? ?
    //          image: string
    //          image_c: ??
    //          images: desTROY me
    //          buildcount: nnnnumber?
    //          recruitcount: mmm
    //      ]
    //      trees: { string: string }
    ROE.vov.init = function (properties_, callback) {

        // Clone the properties, so we can mutate it without affecting the call site.
        var properties = {};
        for (var key in properties_) {
            if (Object.prototype.hasOwnProperty.call(properties_, key)) {
                properties[key] = properties_[key];
            }
        }

        var vovMain = $(ROE.vov.CONST.Selectors.vovMain);
        if (vovMain.length) {
            // We're coming to the VOV from another VOV.
            $(ROE.vov.CONST.Selectors.background).attr('src', properties.bg);
            properties.isNewDOM = false;
            properties.hasBeenRemoved = false;
        } else {
            if (ROE.vov._cachedVOVDOM === null) {
                // We're coming to the VOV for the first time.
                var newVov = $('<div class="vovMain"></div>').append($(BDA.Templates.get("VillageOverview", ROE.realmID, properties)));
                ROE.vov._cachedVOVDOM = newVov[0];
                $(ROE.vov.CONST.Selectors.viewContainer).append(newVov);
                properties.isNewDOM = true;
            } else {
                // We're coming to the VOV from another screen (map, research).
                $(ROE.vov.CONST.Selectors.view).remove();
                $(ROE.vov.CONST.Selectors.viewContainer).append(ROE.vov._cachedVOVDOM);
                $(ROE.vov.CONST.Selectors.background).attr('src', properties.bg);
                properties.isNewDOM = false;
            }
            properties.hasBeenRemoved = true;
        }

        // One or more of these initializers are asynchronous, so in order to
        // avoid prematurely calling the ROE.vov.oninit functions, we call
        // them in continuation-passing form. We only initialize it once to reduce
        // garbage collection.
        if (ROE.vov._initFunctionList === null) {
            ROE.vov._initFunctionList = [
                ROE.vov._initTreesAndTroops,
                ROE.vov._initBuildings,
                ROE.vov._initBuildingLabels,
                ROE.vov._initBuildingRequirements,
                ROE.vov._initCanvases,
                ROE.vov._initAnimations,
                ROE.vov._initScrollSwipe,
                ROE.vov._initTimers,
                ROE.vov._initCallbacks
            ];
        }

        // This is a fancy way of saying that each function will call the
        // next function in the list as a callback.
        var currentIndex = 0;
        (function nextFunction() {
            if (currentIndex < ROE.vov._initFunctionList.length) {
                ROE.vov._initFunctionList[currentIndex++].call(ROE.vov, properties, nextFunction);
            } else {
                if (callback) {
                    callback();
                }
            }
        }());

        BDA.Broadcast.subscribe($('.vovMain'), "VillageExtendedInfoUpdated", ROE.vov._handleVillageExtendedInfoUpdatedOrPopulated);
    }

    ROE.vov._preloadImages = function (manifest, callback) {
        var loaded = 0;
        var result = {};
        for (var i = 0, len = manifest.length; i < len; ++i) {
            var source = manifest[i];         
            var image = new Image();
            image.onload = function () {
                ++loaded;
                result[this.src] = this;
                BDA.Console.verbose('VOV2', 'Loded: ' + this.src);
                if (loaded === len) {
                    callback(result);
                }
            };
            image.onerror = function () {
                ++loaded;
                result[this.src] = null;
                BDA.Console.verbose('VOV2', 'Failed to preload image: ' + this.src);
                if (loaded === len) {
                    callback(result);
                }
            };
            image.src = source;
        }
    };

    ROE.vov._initTreesAndTroops = function (properties, callback) {
        // Naturally, the CSS class for trees and troops is "vov_troops".
        // Likewise, both trees and troops are sent in properties.trees.
        $(ROE.vov.CONST.Selectors.troopsAndTrees.all).attr('src', null);
        for (var treeId in properties.trees) {
            $(ROE.vov.CONST.Selectors.troopsAndTrees.byId(treeId)).attr('src', properties.trees[treeId]);
        }

        if (callback) {
            callback();
        }
    };

    ROE.vov._initBuildings = function (properties, callback) {
        $(ROE.vov.CONST.Selectors.buildings.images.all).attr('src', null).hide();
        $(ROE.vov.CONST.Selectors.buildings.scaffolding.all).attr('src', null).hide();
        $(ROE.vov.CONST.Selectors.buildings.labels.all).hide();
        $('.vovFreeBtn').remove(); //destroy any previous vov free speedup buttons

        var somethingBuilding = false;

        for (var i = 0, len = properties.buildings.length; i < len; i++) {
            var building = properties.buildings[i];
            if (building.built) {
                var buildingNode = $(ROE.vov.CONST.Selectors.buildings.images.byId(building.id));
                        
                if (!building.images) {
                    buildingNode.attr('src', building.image).show();
                } else {
                    buildingNode.each(function (i, node) {
                        $(node).attr('src', building.images[$(node).attr('ims')]).show();
                    });
                }

                if (building.level > 0) {
                    $(ROE.vov.CONST.Selectors.buildings.labels.level.byId(building.id)).html(building.level).show();
                } else {
                    $(ROE.vov.CONST.Selectors.buildings.labels.level.byId(building.id)).hide();
                }

                if (building.image_c) {
                    $(ROE.vov.CONST.Selectors.buildings.scaffolding.byId(building.id)).attr('src', building.image_c).show();
                } else {
                    $(ROE.vov.CONST.Selectors.buildings.scaffolding.byId(building.id)).attr('src', null).hide();
                }
            }

            //Contruction timer
            if (building.cnstr) {

                somethingBuilding = true;
                $(ROE.vov.CONST.Selectors.buildings.labels.buildCounter.byId(building.id)).attr('data-finisheson', building.buildcount).show().addClass("countdown");
                $(ROE.vov.CONST.Selectors.buildings.labels.hammerIcon.byId(building.id)).show();

                //Setup FreeSpeedup setup
                $(ROE.vov.CONST.Selectors.buildings.labels.buildCounter.byId(building.id)).attr('data-eptime', 30000).attr('data-epfunc', 'ROE.vov.freeUpgradeReady(' + building.id + ')');

            } else {

                $(ROE.vov.CONST.Selectors.buildings.labels.buildCounter.byId(building.id)).html(building.buildcount).hide().removeClass("countdown");
                $(ROE.vov.CONST.Selectors.buildings.labels.hammerIcon.byId(building.id)).hide();
            }

            //Recruitment timer
            if (building.recruitcount) {
                $(ROE.vov.CONST.Selectors.buildings.labels.recruitCounter.byId(building.id)).attr('data-finisheson', building.recruitcount).show().addClass("countdown");
                $(ROE.vov.CONST.Selectors.buildings.labels.daggerIcon.byId(building.id)).show();
                if (building.id == 3) {
                    $(ROE.vov.CONST.Selectors.qbBtnTimerText).attr('data-finisheson', building.recruitcount).show().addClass("countdown");
                    ROE.Frame.iconNeedsAttention($('.actionButton.bu, #hdrBuild'), false);
                }               
            } else {
                $(ROE.vov.CONST.Selectors.buildings.labels.recruitCounter.byId(building.id)).html(building.recruitcount).hide().removeClass("countdown");
                $(ROE.vov.CONST.Selectors.buildings.labels.daggerIcon.byId(building.id)).hide();
                if (building.id == 3) {
                    $(ROE.vov.CONST.Selectors.qbBtnTimerText).html(building.recruitcount).hide().removeClass("countdown");
                    ROE.Frame.iconNeedsAttention($('.actionButton.bu, #hdrBuild'), true);
                }
            }

        }


        $('#vov').toggleClass('constructing', somethingBuilding);

    
        if (callback) {
            callback();
        }
    };

    ROE.vov._initBuildingLabels = function (properties, callback) {
        if (properties.hasBeenRemoved) {
            // We only want to add the click handlers if the VOV DOM subtree is brand new or if it
            // was removed and then put back into the DOM (which seems to nullify its click handlers.)
            $(ROE.vov.CONST.Selectors.buildings.labels.level.all).click(function (event) {
                event.preventDefault(); 
                
                ROE.Building2.showBuildingPagePopup($(event.currentTarget).attr("data-bid"));
            });
        }

        // Position hammer icons and countdown timers so that they're a
        // uniform distance from the respective shield icon.
        // We use a for-loop here to reduce the number of closures the
        // interpreter has to make.
        for (var buildings = $(ROE.vov.CONST.Selectors.buildings.labels.level.all), i = 0, len = buildings.length; i < len; ++i) {
            
            var building = buildings[i];
            var extraOffset = 0;
            if (building){
                if (building.classList) {
                    if (building.classList.contains('zoom1')) {
                        extraOffset = -8;
                    } else if (building.classList.contains('zoom2')) {
                        extraOffset = -2;
                    } else if (building.classList.contains('zoom3')) {
                        extraOffset = 4;
                    }
                }

                var jqB = $(building);
                var dataBid = jqB.attr("data-bid");
                var buildCounters = $(ROE.vov.CONST.Selectors.buildings.labels.buildCounter.byId(dataBid));
                var recruitCounters = $(ROE.vov.CONST.Selectors.buildings.labels.recruitCounter.byId(dataBid));
                var hammerIcons = $(ROE.vov.CONST.Selectors.buildings.labels.hammerIcon.byId(dataBid));
                var daggerIcons = $(ROE.vov.CONST.Selectors.buildings.labels.daggerIcon.byId(dataBid));
                var topcss =parseInt(jqB.css('top'));
                var leftcss = parseInt(jqB.css('left'));

                buildCounters.css('top', (topcss + 48 + extraOffset) + 'px');
                buildCounters.css('left', (leftcss + 10) + 'px');
                recruitCounters.css('top', (topcss + 48 + 24 + extraOffset) + 'px');
                recruitCounters.css('left', (leftcss + 10) + 'px');
                hammerIcons.css('top', (topcss + 42 + extraOffset) + 'px');
                hammerIcons.css('left', (leftcss - 20) + 'px');
                daggerIcons.css('top', (topcss + 42 + 24 + extraOffset) + 'px');
                daggerIcons.css('left', (leftcss - 20) + 'px');
            }
        }

        if (callback) {
            callback();
        }
    };

    ROE.vov._initBuildingRequirements = function (properties, callback) {
        $(ROE.vov.CONST.Selectors.buildings.labels.level.all).css('background-image', '');
        for (var i = 0, len = properties.buildings.length; i < len; ++i) {
            var building = properties.buildings[i];
            if (building.level === 0 && building.areRequirementsSatisfied) {
                $(ROE.vov.CONST.Selectors.buildings.labels.level.byId(building.id))
                    .show()
                    .html('')
                    .css('background-image', 'url(' + $(ROE.vov.CONST.Selectors.buildings.labels.shieldIcon.byId(building.id)).data('icon') + ')');
            }
        }

        if (callback) {
            callback();
        }

    };

    ROE.vov._initCanvases = function (properties, callback) {
        var paintToCanvas = $(ROE.vov.CONST.Selectors.paintToCanvasNonempty);        
        var paintToCanvasSrcs = [];
        for (var i = 0, len = paintToCanvas.length; i < len; ++i) {
            if (paintToCanvas[i].src) { //despite the selector above, some assets were still grabbed with NO src
                paintToCanvasSrcs.push(paintToCanvas[i].src);
            }           
        }

        var paintCallbackCallFlags = { invalid: false };
        ROE.vov._asynchronousCalls.push(paintCallbackCallFlags);

        ROE.vov._preloadImages(paintToCanvasSrcs, function (result) {
            //return;
            if (paintCallbackCallFlags.invalid) {
                return;
            }

            var canvases = ROE.vov._canvases = $(ROE.vov.CONST.Selectors.canvases);
            var contexts = {};
            canvases.each(function (index) {
                var context = contexts[$(this).attr("data-canvaslayer")] = this.getContext('2d');
            });

            for (var i = 0, len = canvases.length; i < len; ++i) {
                // This is the only way to clear a canvas on android that does not
                // incur the ghosting glitch.
                canvases[i].width = canvases[i].width;
            };

            for (i = 0, len = paintToCanvas.length; i < len; ++i) {
                $(paintToCanvas[i]).attr("data-domindex", i);
            }

            paintToCanvas.sort(function (a, b) {
                // We sort first by the z-index of elements, and if they compare equal,
                // we then compare by the order they appear in the DOM.

                if (!a.style || !b.style) { //this is a hack fix for 4.1
                    BDA.Console.verbose('VOV2', "STYLE FAIL " + a.id);
                    return 0;
                }

                var ai = parseInt($(a).attr("data-domindex"));
                var bi = parseInt($(b).attr("data-domindex"));
                if (+a.style.zIndex > +b.style.zIndex) {
                    return 1;
                } else if (+a.style.zIndex < +b.style.zIndex) {
                    return -1;
                } else {
                    if (+ai > +bi) {
                        return 1;
                    } else if (+ai < +bi) {
                        return -1;
                    } else {
                        return 0;
                    }
                }
            });

            for (i = 0, len = paintToCanvas.length; i < len; ++i) {
                var image = paintToCanvas[i];
                //check for natural width incase image is .complete but is broken
                //.complete seems to not be a reliable crossbrowser check
                if (image.src && image.complete && image.naturalWidth !== 0) {
                    var left = +image.style.left.replace(/px$/, '') || 0;
                    var top = +image.style.top.replace(/px$/, '') || 0;
                    var resultImage = result[image.src];
                    if (resultImage) {
                        try {
                            contexts[$(image).attr('data-canvaslayer') || 'background'].drawImage(resultImage, left, top);
                        } catch (e) {
                            var roeex = new BDA.Exception("VOV Canvas .drawImage fail: " + e.message);
                            roeex.data.add('image.src', image.src);
                            throw roeex;
                        }
                    }
                } 
            }

            $(ROE.vov.CONST.Selectors.paintToCanvas).hide();

            if (callback) {
                callback();
            }
        });
 
    };

    ROE.vov._initAnimations = function (properties, callback) {
        //console.log(properties.animations); //so 'properities' comes from backend?

        //for now clean all Anima objects. If VoV was redone from scratch the approach would be a little different. -farhad
        ROE.vov.anima = {}; 

        var enableAnimations = localStorage.getItem('enableVOVAnimations');
        if (enableAnimations == ROE.vov.CONST.EnableAnimations.TRUE) {
            ROE.vov._animateAnimations = true;
        } else if (enableAnimations == ROE.vov.CONST.EnableAnimations.FALSE) {
            ROE.vov._animateAnimations = false;
        } else {
            // In versions of Android below 4.4, we do not show animations because
            // just about every possible strategy for animations is horribly
            // broken. See the R&D document for details.
            // Instead, we just show a single static frame of each animation.
            ROE.vov._animateAnimations = (ROE.isDevice !== ROE.Device.CONSTS.Android || ROE.Browser.androidApiLevel >= 19);
        }
        //console.log('_initAnimations', properties);
        //console.log('ROE.vovAnimations', ROE.vovAnimations)
        ROE.vov._isDay = properties.isday;

        ROE.vov._stage = ROE.vov._stage || new createjs.Stage($(ROE.vov.CONST.Selectors.animationCanvas)[0]);

        // If animations are disabled, we want to draw a single frame of each sprite up-front.
        // thus we have to make sure every image is loaded first.
        var manifest;
        if (!ROE.vov._animateAnimations) {
            manifest = [];
        }

        ROE.vov._stage.removeAllChildren();

        var getSprite = function (animation) {
            
            var animationIdentifier = ROE.vov._isDay ? animation.type : animation.type + '_n';

            //ROE.vovAnimations is FULL list of possible animations, from server
            var animationRecord = ROE.vovAnimations[animationIdentifier];
            if (!animationRecord) {
                //this is logging non-errors such as a night only animation 'tavern_glow_1_n' not having a day version 'tavern_glow_1'
                //BDA.Console.verbose('ROE.vov._initAnimations', 'Requested nonexistent animation: ' + animationIdentifier);
                return undefined;
            }

            if (!animationRecord.animations) {
                throw new Error('cannot create animation without animation data');
            }

            if (!ROE.vov._animateAnimations) {
                manifest.push(animationRecord.spritesheet);
            }

            var spritesheet = new createjs.SpriteSheet({
                images: [animationRecord.spritesheet],
                frames: { width: animationRecord.width, height: animationRecord.height },
                animations: animationRecord.animations
            })

            var sprite;
            if (animationIdentifier in ROE.vov._animations) {
                sprite = ROE.vov._animations[animationIdentifier];
            } else {
                sprite = ROE.vov._animations[animationIdentifier] = new createjs.Sprite(spritesheet, 'first');
            }

            sprite.x = animation.x;
            sprite.y = animation.y;
            
            if (animation.type === 'crone') {
                ROE.vov._crone = sprite;
            } 

            return sprite;
        };

        for (var i = 0, len = properties.animations.length; i < len; ++i) {

            //skip scarecrow in desert theme
            if (properties.animations[i].type === 'scarecrow' && ROE.Entities.Realm.Theme === 1) {
                continue;
            }

            var sprite = getSprite(properties.animations[i]);
            if (!sprite) { continue; }

            if (properties.animations[i].type === 'knightHorse') {

                ROE.vov.anima['knightHorse'] = {

                    alive: false,
                    age: 0,

                    construct: function () {
                        if (this.alive) { return; }
                        this.alive = true;
                        this.sprite = sprite;
                        ROE.vov._stage.addChild(this.sprite);
                        this.start();
                    },

                    tick: function () {
                        if (!this.alive || !this.sprite || this.sprite.paused) {
                            return;
                        }

                        if (this.sprite.currentAnimation == 'last') {
                            this.sprite.stop();
                            setTimeout(function restartKnightHorse() {
                                if (ROE.vov.anima['knightHorse']) {
                                    ROE.vov.anima['knightHorse'].start();
                                }                              
                            }, Math.floor((Math.random() * 6000)) + 3000);
                        }

                    },

                    start: function () {
                        if (!this.sprite) { return; }
                        this.sprite.gotoAndPlay('first');
                    }

                }
                ROE.vov.anima['knightHorse'].construct();
            } else {
                //just add the sprite and yolo animation
                ROE.vov._stage.addChild(sprite);
            }

        }

        
        ROE.vov.anima['geese'] = {

            alive: false,
            age: 0,

            construct: function () {
                if (this.alive) { return; }
                this.alive = true;
                this.sprite = getSprite({ type: 'geese', x: -480, y: 0 });
                if (!sprite) { return; }
                ROE.vov._stage.addChild(this.sprite);
                this.start();
            },

            tick: function () {
                if (!this.alive || !this.sprite || this.sprite.paused) {
                    return;
                }

                if (this.sprite.x < -54) {
                    this.sprite.stop();
                    this.sprite.vX = 0;
                    setTimeout(function restartGeese() {
                        if (ROE.vov.anima['geese']) {
                            ROE.vov.anima['geese'].start();
                        }
                    }, Math.floor((Math.random() * 10000)) + 10000);
                } else {
                    this.sprite.x -= 2;
                }                      
            },

            start: function () {
                if (!this.sprite) { return; }
                this.sprite.gotoAndPlay('first');
                this.sprite.vX = Math.floor(Math.random() * 2);
                this.sprite.x = 480;
                this.sprite.y = 15 + Math.floor((Math.random() * 20));
            }

        }

        ROE.vov.anima['goose'] = {

            alive: false,
            age: 0,

            construct: function () {
                if (this.alive) { return; }
                this.alive = true;
                this.sprite = getSprite({ type: 'goose', x: -480, y: 0 });
                if (!sprite) { return; }
                ROE.vov._stage.addChild(this.sprite);
                this.start();
            },

            tick: function () {
                    
                if (!this.alive || !this.sprite || this.sprite.paused) {
                    return;
                }

                if (this.sprite.x < -41) {
                    this.sprite.stop();
                    this.sprite.vX = 0;
                    setTimeout(function restartGoose() {
                        if (ROE.vov.anima['goose']) {
                            ROE.vov.anima['goose'].start();
                        }
                    }, Math.floor((Math.random() * 10000)) + 10000);
                } else {
                    this.sprite.x -= this.sprite.vX;              
                }

            },

            start: function () {
                if (!this.sprite) { return; }
                this.sprite.gotoAndPlay('first');
                this.sprite.vX = 1 + Math.floor(Math.random() * 3);
                this.sprite.x = 480;
                this.sprite.y = 40 + Math.floor((Math.random() * 20));
            }

        }

        ROE.vov.anima['barnOwl'] = {

            alive: false,
            age: 0,

            construct: function () {
                if (this.alive) { return; }
                this.alive = true;
                this.sprite = getSprite({ type: 'barnOwlFlight', x: -480, y: 0 });
                if (!sprite) { return; }
                ROE.vov._stage.addChild(this.sprite);
                this.start();
            },

            tick: function () {
                    
                if (!this.alive || !this.sprite || this.sprite.paused) {
                    return;
                }

                if (this.sprite.x < -40) {
                    this.sprite.stop();
                    this.sprite.vX = 0;
                    setTimeout(function restartGoose() {
                        if (ROE.vov.anima['barnOwl']) {
                            ROE.vov.anima['barnOwl'].start();
                        }                        
                    }, Math.floor((Math.random() * 10000)) + 10000);
                } else {
                    this.sprite.x -= this.sprite.vX;              
                }

            },

            start: function () {
                if (!this.sprite) { return; }
                this.sprite.gotoAndPlay('first');
                this.sprite.vX = 1;
                this.sprite.x = 480;
                this.sprite.y = 40 + Math.floor((Math.random() * 20));
            }

        }

        /*
        ROE.vov.anima['caravan'] = {

            alive: false,
            age: 0,

            construct: function () {
                if (this.alive) { return; }
                this.alive = true;
                this.sprite = sprite;
                ROE.vov._stage.addChild(this.sprite);
                this.start();
            },

            tick: function () {
                if (!this.alive || !this.sprite || this.sprite.paused) {
                    return;
                }
            },

            start: function () {
                if (!this.sprite) { return; }
                this.sprite.gotoAndPlay('first');
            }

        }
        ROE.vov.anima['caravan'].construct();
        */

        if (ROE.vov._animateAnimations) {
            //ROE.vov._isDay = false;

            if (ROE.vov._isDay) {
                setTimeout(function startTheGeese() { ROE.vov.anima['geese'].construct(); }, 1000 + Math.floor(Math.random() * 10000));
                setTimeout(function startTheGoose() { ROE.vov.anima['goose'].construct(); }, 1000 + Math.floor(Math.random() * 30000));               
            }else{
                setTimeout(function startTheBarnOwl() { ROE.vov.anima['barnOwl'].construct(); }, 1000 + Math.floor(Math.random() * 20000 ));
            }

            if (!ROE.vov._globalTickListenerAdded) {
                createjs.Ticker.addEventListener('tick', onTick);
                ROE.vov._globalTickListenerAdded = true;
                createjs.Ticker.useRAF = true;
                createjs.Ticker.setFPS(10);
            }

            if (callback) {
                callback();
            }

        } else {
            // If we aren't animating, then we simply draw each character exactly once, instead of putting it in a ticker loop.
            // We have to wait for each image to have been loaded first, though, since otherwise, EaselJS just draws nothing.
            if (ROE.vov._globalTickListenerAdded) {
                createjs.Ticker.removeEventListener('tick', onTick);
                ROE.vov._globalTickListenerAdded = false;
            }
            ROE.vov._preloadImages(manifest, function () {
                ROE.vov._stage.update();
                if (callback) {
                    callback();
                }
            });
        }
        function onTick() {
            ROE.vov.tick();
        }
    };

    ROE.vov._initScrollSwipe = function (properties, callback) {
        var wrapper = $(ROE.vov.CONST.Selectors.html5Swipe.wrapper);
        if (wrapper.length == 0) { //if the div doesnt exist, stop here, this is amazon only
            if (callback) {
                return callback();
            }
            return;
        }
        var floater = $(ROE.vov.CONST.Selectors.html5Swipe.floater);
        wrapper.append(floater);
        $(ROE.vov.CONST.Selectors.vovInner).append(wrapper);
        var wrapperWidth = wrapper.width();
        var floaterWidth = floater.width();
        wrapper.scrollLeft(floaterWidth / 2 - wrapperWidth / 2).scroll(function () {
            window.clearTimeout(window.ROE.Building2['scrollTimeout']);
            var scroll = wrapper.scrollLeft();
            var scrollMax = floater.width() - wrapper.width();
            if (scroll == 0) {
                window.ROE.Frame.swipeRight();
            } else if (scroll >= scrollMax) {
                window.ROE.Frame.swipeLeft();
            } else {
                window.ROE.Building2['scrollTimeout'] = window.setTimeout(function () {
                    var scrollCenter = floater.width() / 2 - wrapper.width() / 2;
                    wrapper.stop().animate({ 'scrollLeft': scrollCenter }, 50, "easeInCirc");
                }, 200);
            }
        });

        if (callback) {
            callback();
        }
    };

    ROE.vov._initTimers = function (properties, callback) {
        initTimers();
        callback();
    };

    ROE.vov._initCallbacks = function (properties, callback) {
        for (var i = 0, len = ROE.vov.oninit.length; i < len; ++i) {
            ROE.vov.oninit[i]();
        }
        callback();
    };

    ROE.vov.tick = function () {

        for (var actor in ROE.vov.anima) {
            ROE.vov.anima[actor].tick();
        }

        if (ROE.vov._crone) {
            // The ticker allows us to update the crone once every other frame in order to make
            // the animation less smooth, making it look more like the rest of the scene. It also
            // means we can move the crone forward only when its cane is touching the ground.
            ROE.vov._croneTicker = (ROE.vov._croneTicker + 1);
            if (ROE.vov._crone.activeAnimationName === 'first') {
                // The crone walk cycle is six frames long. The first three of
                // these involve the crone's cane touching the ground, so we move
                // it forward. Each animation frame lasts three vsync frames.
                var animationFrame = ROE.vov._croneTicker % 18;
                if (animationFrame >= 15 || animationFrame < 6) {
                    if (ROE.vov._croneTicker % 3 === 0) ROE.vov._crone.x -= 2;
                }
            } else if (ROE.vov._crone.activeAnimationName === 'walk') {
                if (ROE.vov._croneTicker % 2 === 0) ROE.vov._crone.x += 2;
            } else {
                ROE.vov._croneTicker = 0;
            }
        }
        
        ROE.vov._stage.update();
    };

    ROE.vov.pauseAnimation = function () {
        ROE.vov._animationsPaused = true;
        createjs.Ticker.setPaused(true);
    };

    ROE.vov.unpauseAnimation = function () {
        ROE.vov._animationsPaused = false;
        createjs.Ticker.setPaused(false);
    };
    ROE.vov._handleVillageExtendedInfoUpdatedOrPopulated = function (village) {
        if (village.id === ROE.SVID) {
            if (
               village.changes.buildings
               || village.changes.type // might require new images, and we dont check if images have changed atm
               || village.changes.buildingWork
               ) {
                BDA.Console.verbose('VillChanges', "VID:%vid% [in vov] - reloading".format({ vid: village.id }));
                ROE.vov._initializeNewVOV();
                ROE.vov.init(village.VOV);
            } else {
                BDA.Console.verbose('VillChanges', "VID:%vid% [in vov] - no changes".format({ vid: village.id }));

            }

        }
    };


    //FREE SPEED UP FUNCTIONS
    ROE.vov.freeUpgradeReady = function (bid) {

        //disabled for RX, also disabled in backend
        if (ROE.rt == "X") { return; }

        var counter = $(ROE.vov.CONST.Selectors.buildings.labels.buildCounter.byId(bid));
        var freeBtn = $('<div class="vovFreeBtn BtnBLg1 fontButton1L effect-smoothpulse" >Finish<div class="icon"></div></div>');
        var topPos = parseInt(counter.css('top')) + 17;
        var leftPos = parseInt(counter.css('left')) - 50;
        freeBtn.css({ top: topPos, left: leftPos }).click(_doSpeedUpFree);
        freeBtn.appendTo(counter.parent());
    }
    var _vovVillage;
    function _doSpeedUpFree() {
        ROE.Frame.busy('Completing the building...', 5000, $('#currentVOV'));
        $('.vovFreeBtn').remove();
        ROE.Villages.getVillage(ROE.SVID, function _doSpeedUpFreeGotVillage(village) {
            _vovVillage = village;
            var firstEventInQ = village.upgrade.Q.UpgradeEvents[0];
            if (firstEventInQ) {
                ROE.Api.call_upgrade_speedupupgradefree(ROE.SVID, firstEventInQ.ID, _doSpeedUpUpgradeReturn);
            }

        }, ROE.Villages.Enum.ExtendedDataRetrieveOptions.ensureExists)
   
    }  
    function _doSpeedUpUpgradeReturn(data) {
        ROE.Frame.free($('#currentVOV'));
        //if village changed before call came back, discard data
        if (ROE.SVID != data.Village.Village.id) { return; }
        if (data.cutSuccessful) {
            ROE.Villages.__populateExtendedData(data.Village.id, _vovVillage, data.Village);
        }
    }
    //FREE SPEED UP FUNCTIONS

};