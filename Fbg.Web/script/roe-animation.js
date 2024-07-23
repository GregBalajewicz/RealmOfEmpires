(function (obj) {


    var _paused = localStorage.getItem('animation_engineState') === "OFF"; // if _paused is true, return out of every tick. 
    var _fpsMeter; //will have an fps meter init into it.

    var _mapViewElement; //a dom canvas
    var _pixiRenderer; //renderer engine (webgl or canvas, auto detected)
    var _pixiStage; //root (pixi container) of the scene
    var _landMark; //cache roe.landmark before every tick, so all children can use it

    var _cache = {
        sheets: {}
    };

    var _resDictionary = {

        map_TokenOutAttk: 'https://static.realmofempires.com/images/animation/map_TokenOutAttk.png',
        map_TokenOutSupport: 'https://static.realmofempires.com/images/animation/map_TokenOutSupport.png',
        map_TokenReturning: 'https://static.realmofempires.com/images/animation/map_TokenReturning.png',

        map_lineOutAttk: 'https://static.realmofempires.com/images/animation/map_lineOutAttk.png',
        map_lineOutSupport: 'https://static.realmofempires.com/images/animation/map_lineOutSupport.png',
        map_lineReturning: 'https://static.realmofempires.com/images/animation/map_lineReturning.png',

        map_lineIncAttk: 'https://static.realmofempires.com/images/animation/map_lineIncAttk.png',
        map_lineIncSupport: 'https://static.realmofempires.com/images/animation/map_lineIncSupport.png',
        map_TokenIncAttk2: 'https://static.realmofempires.com/images/animation/map_TokenIncAttk2.png',
        map_TokenIncSupport: 'https://static.realmofempires.com/images/animation/map_TokenIncSupport.png',

        cloud1: 'https://static.realmofempires.com/images/animation/c3.png',

        targetSupport: 'https://static.realmofempires.com/images/icons/M_HD_SupportCAll.png',
        targetAttack: 'https://static.realmofempires.com/images/icons/M_HD_AttackCAll.png',

        //vignette assets
        ving_cloud_TL: 'https://static.realmofempires.com//images/animation/SmTLcloud.png',
        ving_cloud_TR: 'https://static.realmofempires.com//images/animation/SmTRcloud.png',
        ving_cloud_BL: 'https://static.realmofempires.com//images/animation/SmBLcloud.png',
        ving_cloud_BR: 'https://static.realmofempires.com//images/animation/SmBRcloud.png',

        ving_border_wood: 'https://static.realmofempires.com//images/animation/WoodLand_Vignette.png',
        ving_border_desert: 'https://static.realmofempires.com//images/animation/Desert_Vignette.png',

        /*
        ving_border_TL_wood: 'https://static.realmofempires.com//images/animation/Woodland_VignetteTL.png',
        ving_border_TR_wood: 'https://static.realmofempires.com//images/animation/Woodland_VignetteTR.png',
        ving_border_BL_wood: 'https://static.realmofempires.com//images/animation/Woodland_VignetteBL.png',
        ving_border_BR_wood: 'https://static.realmofempires.com//images/animation/Woodland_VignetteBR.png',
        */

    }

    var _currentLines = {}; //store ov to dv troop line objects, and use it to limit them

    var _settings = {
        direction: null,
        state_targets: null,
        state_vig: null
    }

    function _initMapAnimations() {

        _paused = localStorage.getItem('animation_engineState') === "OFF";
        _settings.direction = localStorage.getItem('animation_direction') || 2;
        _settings.state_targets = parseInt(localStorage.getItem('animation_state_targets') || 1); // 0/1 turns targets animations on and off, 
        _settings.state_vig = parseInt(localStorage.getItem('animation_state_vig') || 1); // 0/1 turns vignette animations on and off, 
        _updateEngineStatusChange(); //updates UI things


        //if turned off by user, dont even init (for cases where user boots up as paused)
        if (_paused) { return; }

        //setup view element
        _setupMapView();

        //only init once
        if (!_pixiStage) {

            //root of the scene graph
            _pixiStage = new PIXI.Container();

            /*
            _pixiStage.interactive = true;
            _pixiStage.on('mousemove', function () {
                var mousePosition = _pixiRenderer.plugins.interaction.mouse.global;
                console.log(mousePosition);
            });
            */

            _landMark = ROE.Landmark;

            _initVing();

            //_initExtras();
            //_initFpsMeter();

            //start the tick cycle
            _tick();

        }

        _animateDefinedTargets();
        _animationInOutChanged();

    }

    function _initExtras() {

        /*
        var btn1 = $('<div class="pixiCommand pcA">').html('A').click(function () {
            console.log(_pixiStage.children);
            _addCmdAwaitingResolution({ vx: 0, vy: 1, type: 1, eid: 1 });
        }).appendTo('.mainFooter');
        */

        /*
        var btn2 = $('<div class="pixiCommand pcB">').html('Fire').click(function () {
            if ($(this).hasClass('active')) { return; }
            $(this).addClass('active');
            ROE.Villages.getVillages(function (villages) {
                for (var i = 0; i < villages.length; i++) {
                    _addVillageFire(villages[i].x, villages[i].y);
                }
            });
        }).appendTo('.mainFooter');


        //video experiment, proof of concept, work towards using video sprites to simplify work
        var btn3 = $('<div class="pixiCommand pcC">').html('Vid').click(function () {
            if ($(this).hasClass('active')) { return; }
            $(this).addClass('active');

            //video element in memory
            var video = document.createElement('video');
            video.crossOrigin = 'anonymous';
            video.loop = 'loop';
            video.src = 'https://static.realmofempires.com/images/animation/goggles.webm';
            //video.src = 'https://static.realmofempires.com/images/animation/GreenDiamond01.mov';

            //texture from video
            var vidtexture = PIXI.Texture.fromVideo(video, null);

            //sprite from texture
            var videoSprite = new PIXI.Sprite(vidtexture);
            
            videoSprite.width = 540;
            videoSprite.height = 360;
            videoSprite.entity = { tick: function () { } };
            videoSprite.position.set(300, 100);

            videoSprite.interactive = true;
            videoSprite.on('click', function () {
                //vidtexture.baseTexture.source.src = ""; //setting source to null is a good texture nuke, but throwing webgl errors
                vidtexture.baseTexture.source.pause(); //pause instead
                videoSprite.destroy();
                vidtexture.destroy();
                _pixiStage.removeChild(videoSprite);
            })

            _pixiStage.addChild(videoSprite);
            

        }).appendTo('.mainFooter');
        */

        //_addVillageFire(0, 1);
        //_addVillageFire(-2, 3);
        //_addCmdAwaitingResolution({ vx: 0, vy: 1, type: 1, eid: 1 });
        //_addVillageFire(-1, 1);
        //_addVillageFire(0, -1);
        //_addCloud(50, 300, _textureById('cloud1'));

    }

    //setup a view element for pixi, aka a canvas element, if not already created
    function _setupMapView() {

        if (_mapViewElement) {
            $('#mapwrap').append(_mapViewElement);
            return;
        }

        _mapViewElement = $('<canvas id="pixiCanvas"></canvas>');

        //Big issue here, on M the animation canvas is blocking map interaction, temp solution
        if ($('body').hasClass('mobile')) {
            _mapViewElement.css('pointer-events', 'none');
        }

        BDA.Broadcast.subscribe(_mapViewElement, "InOutgoingDataChanged", _animationInOutChanged);

        _pixiRenderer = new PIXI.autoDetectRenderer(800, 600, { view: _mapViewElement[0], transparent: true });
        _pixiRenderer.roundPixels = true;
        //_pixiRenderer = new PIXI.CanvasRenderer(800, 600, { view: _mapViewElement[0], transparent: true }); //force canvas
        //_pixiRenderer = new PIXI.WebGLRenderer(800, 600, { view: _mapViewElement[0], transparent: true }); //force webgl           

        $('#mapwrap').append(_mapViewElement);

        //for keeping canvas fullscreen
        $(window).resize(function () {
            _windowResized();
        });
        function _windowResized() {
            var mapwrap = $('#mapwrap');
            _pixiRenderer.resize(mapwrap.width(), mapwrap.height());
        }
        _windowResized();

    }

    //Animation Tick
    var _needsOneClear = false;
    function _tick() {

        if (_fpsMeter) {
            _fpsMeter.tick();
        }

        if (!_paused) {

            //snap cache landmark for performance
            _landMark = ROE.Landmark;

            //tick all children
            var children = _pixiStage.children;
            for (var i = 0; i < children.length; i++) {
                children[i].entity.tick();
            }

            // render the container
            if (children.length) {
                _pixiRenderer.render(_pixiStage);
                _needsOneClear = true;
            } else {

                //if ever there are no children to render, and we get here, we need to do one clear
                if (_needsOneClear && _pixiRenderer._activeRenderTarget) {
                    _pixiRenderer.clear();
                    _needsOneClear = false;
                }

            }

        }

        requestAnimationFrame(_tick);
    }

    function _togglePause() {
        if (_paused) {
            _resume();
        } else {
            _pause();
        }
    }

    function _pause() {
        _paused = true;
        localStorage.setItem('animation_engineState', 'OFF');
        if (_mapViewElement) {
            _mapViewElement = _mapViewElement.remove();
        }
        ROE.Frame.infoBar("Map animations: Paused");
        _updateEngineStatusChange();
    }

    function _resume() {
        if (_paused) { //double checking _paused, to avoid cycling more ticks     
            _paused = false;
            if (_mapViewElement) {
                $('#mapwrap').append(_mapViewElement);
            }
            localStorage.setItem('animation_engineState', 'ON');
            _initMapAnimations();
            ROE.Frame.infoBar("Map animations: Resumed");
            _animationInOutChanged();
        }
    }

    //used to externally check of animation engine on or 
    function _isAnimationEngineOn() {
        return !_paused;
    }

    //external checks if a particular animation setting is on
    function _isAnimationSubsetOn(setting) {
        if (setting == "direction") {
            return _settings[setting] > 0 ? true : false;
        } else if (setting == "state_vig") {
            return _settings[setting] = _settings[setting] ? true : false;
        } else if (setting == "state_targets") {
            return _settings[setting] = _settings[setting] ? true : false;
        } else {
            return false;
        }
    }

    //for UI and other changes
    function _updateEngineStatusChange() {
        if (_paused) {
            $('.animationSettings .togglePause').removeClass('on').addClass('off');
        } else {
            $('.animationSettings .togglePause').removeClass('off').addClass('on');
        }
    }

    function _toggleDirection() {
        _settings.direction++;
        if (_settings.direction > 3) {
            _settings.direction = 0;
        };
        localStorage.setItem('animation_direction', _settings.direction);
        $('.animationSettings .toggleDirection').attr('data-direction', _settings.direction);
        var directionTexts = ['Display only Incoming', 'Display only Outgoing', 'Display both Directions', 'Hidden'];
        ROE.Frame.infoBar("Map troop animations: " + directionTexts[_settings.direction]);
        _animationInOutChanged();
    }

    function _toggleTargetState(state) {

        //if given a state, use it, else toggle current state
        if (state === 1 || state === 0) {
            _settings.state_targets = state;
        } else {
            _settings.state_targets = _settings.state_targets ? 0 : 1;
        }

        localStorage.setItem('animation_state_targets', _settings.state_targets);
        $('.animationSettings .toggleTargetState').attr('data-state', _settings.state_targets);
        var text = ['Targets Animations: OFF', 'Targets Animations: ON'];
        ROE.Frame.infoBar(text[_settings.state_targets]);
        if (_settings.state_targets == 0) {
            _removeChildrenByType("definedTarget");
        } else {
            _animateDefinedTargets();
        }
    }


    //toggle the vignette
    function _toggleVigState() {

        _settings.state_vig = _settings.state_vig ? 0 : 1;
        localStorage.setItem('animation_state_vig', _settings.state_vig);
        $('.animationSettings .toggleVigState').attr('data-state', _settings.state_vig);
        var text = ['Cloud and Shadow Effect: OFF', 'Cloud and Shadow Effect: ON'];
        ROE.Frame.infoBar(text[_settings.state_vig]);
        if (_settings.state_vig == 0) {
            _removeChildrenByType("vig");
        } else {
            _initVing();
        }


    }


    function _removeChildrenByType(type) {
        if (_pixiStage) {
            for (var i = _pixiStage.children.length - 1; i >= 0; i--) {
                if (_pixiStage.children[i].entity.type == type) {
                    _pixiStage.children[i].entity.remove();
                }
            }
        }
    }

    //load an image and create a sprite sheet texture array out of it. Uses cache if possible.
    function _loadCreateSpriteSheet(sheetCreateData) {

        //skip if already loaded and cached
        if (_cache.sheets[sheetCreateData.id]) {
            sheetCreateData.loadFunc(_cache.sheets[sheetCreateData.id]);
            return;
        }

        var loader = new PIXI.loaders.Loader(); // you can also create your own if you want
        loader.add('item', sheetCreateData.src);
        loader.on('error', function (e) {
            //problem: complete event is firing anyway...  must put error handling in asset load -farhad
            //console.log('error',e);
        });

        //loader.once('complete', _assetLoaded.bind(data));
        loader.once('complete', _assetLoaded);
        loader.load();

        function _assetLoaded(loader, resource) {

            //if asset load failed //resource[Object.keys(resource)[0]].error
            if (resource.item.error) {
                console.log('ASSET LOAD FAIL:', resource.item.error);
                return false;
            }

            //make a sprite sheet from a resource
            var generatedSheetArray = _generateSpriteSheet({
                src: resource.item.url,
                frameW: sheetCreateData.frameW,
                frameH: sheetCreateData.frameH,
                columns: sheetCreateData.columns,
                frameCount: sheetCreateData.frameCount
            });

            //if somehow url wasnt in texturecache
            if (!generatedSheetArray) {
                console.log('GENERATE SHEET FAIL URL:', resource.item.url);
                return false;
            }

            _cache.sheets[sheetCreateData.id] = generatedSheetArray;
            sheetCreateData.loadFunc(generatedSheetArray);

        }

    }

    //chop up and image into a texture array
    function _generateSpriteSheet(data) {

        var base = PIXI.utils.TextureCache[data.src];
        if (!base) {
            return null;
        }

        var frameW = data.frameW;
        var frameH = data.frameH;
        var columns = data.columns;
        var frameCount = data.frameCount;
        var texture;
        var outputFrames = [];

        for (var c = 0, r = 0; outputFrames.length < frameCount; c++) {

            if (c * frameW > base.width - frameW) {
                c = 0;
                r++;
            }

            texture = new PIXI.Texture(base);
            texture.frame = new PIXI.Rectangle(c * frameW, r * frameH, frameW, frameH);
            outputFrames.push(texture);

        }

        return outputFrames;
    }

    //finds from cache, or creates, and returns a texture
    function _textureById(textureId) {
        var textureUrl = _resDictionary[textureId];
        var texture = PIXI.utils.TextureCache[textureUrl];
        if (!texture) {
            texture = PIXI.Texture.fromImage(textureUrl);
        }
        return texture;
    }

    //notes to self
    //l.rx and ry are the overall offset of canvs0_0 from screen x,y of 0,0
    //earthpx is the size of the canvases, and by determining in which canvas an object belongs, and where in it, we can determine where to place pixi object

    //we can place objects on villages by getting the x,y of village or whatever coord, and x it by l.landpx
    function _updateAnimaMapMove(x, y) {
        //if each object is placed according to l.rx l.ry then we dont need update ticks
        //l.rx/ry doesnt work because its different at each zoom level

        //but the prinicipal is still the same, once given an origina, it can change from there via drag
        //hmmm but no, lrx was good because it would change and all animations xy were based on that,
        //we cant do it based on drag change because that is both not accurate and illperforming
        //question is still how to constantly place on animation canvas in relation to game map objects... hmm

        //hmm lrx could still work, if the object is originated based on x and l.landpx... must test

        /*
        var children = _pixiStage.children;
        for (var i = 0; i < children.length; i++) {
            children[i].entity.tick();
        }
        */
        //console.log(x, y);
    }

    //demo function, called from map when a village is clicked
    function _vClick(v) {
        //console.log('animation v click', v);

        //_addTroopMovement(0, 1, v.x, v.y, 'inc');

        //send out an attack
        /*
        var attackUnitList = [{ utID: 6, sendCount: 1, targetBuildingTypeID: 7 }];
        ROE.Api.call("cmd_execute", { ovid: 1, tvid: v.id, cmdtype: 1, units: $.toJSON(attackUnitList) }, function (data) {
            console.log('ATTACK SENT.', data);
        });

        */
    }

    function _initFpsMeter() {

        //this is for measuring FPS in a neat way, not related to pixi
        FPSMeter.theme.myTheme = {
            heatmaps: [],
            container: {
                background: "#aaa",
                padding: '3px 5px',
                position: {
                    top: 10,
                    right: 10
                }
            },
            count: {
            },
            legend: {
                top: 10,
                right: 10
            },
            graph: {
            },
            column: {}
        };

        _fpsMeter = new FPSMeter({
            theme: 'myTheme'
        });
    }


    ///
    /// ASSET AREA
    ///


    //Map vingnette cloud and border thing
    function _initVing() {

        //for now Theme 2 only
        if (ROE.Entities.Realm.Theme < 2) {
            return;
        }


        if (ROE.isMobile) {
            return;
        }

        if (_settings.state_vig == 0) {
            return;
        }

        var spriteCloudTR = makeSpriteClout("ving_cloud_TR");
        var spriteCloudTL = makeSpriteClout("ving_cloud_TL");
        var spriteCloudBR = makeSpriteClout("ving_cloud_BR");
        var spriteCloudBL = makeSpriteClout("ving_cloud_BL");

        //make the cloud sprites based on texture ID
        function makeSpriteClout(textureID) {

            var sprite = new PIXI.Sprite(_textureById(textureID));
            sprite.alpha = .7;
            sprite.scale.set(1.5, 1.5);
            sprite.interactive = true;

            sprite.entity = {
                fading: false,
                alphaFade: .2,
                alphaBase: .7
            }

            sprite.entity.tick = function tick() {
                if (sprite.entity.fading) {
                    if (sprite.alpha > sprite.entity.alphaFade) {
                        sprite.alpha -= .1;
                    }
                } else {
                    if (sprite.alpha < sprite.entity.alphaBase) {
                        sprite.alpha += .1;
                    }
                }
            }

            sprite.on('mouseover', function () {
                sprite.entity.fading = true;
            });
            sprite.on('mouseout', function () {
                sprite.entity.fading = false;
            });

            return sprite;
        }

        //use ving_border_desert later for desert theme
        var spriteBorder = new PIXI.Sprite(_textureById("ving_border_wood"));
        spriteBorder.position.x = 0;
        spriteBorder.position.y = 0;
        spriteBorder.alpha = .5;

        var container = new PIXI.Container();

        container.entity = {
            type: "vig",
            updateTick: 9,
            z: 0 //issue with clicking other interactive stuff, will need solving

        }

        container.entity.tick = function () {
            container.entity.updateTick++;

            if (container.entity.updateTick > 5) {
                spriteCloudTL.entity.tick();
                spriteCloudTR.entity.tick();
                spriteCloudBL.entity.tick();
                spriteCloudBR.entity.tick();
            }

            if (container.entity.updateTick > 10) {

                var rendererWidth = _pixiRenderer.width;
                var rendererHeight = _pixiRenderer.height;

                var cornerAdjust = 120; //pulls the clouds more inwards
                //Cloud TL
                spriteCloudTL.position.x = 0 - cornerAdjust;
                spriteCloudTL.position.y = 0 - cornerAdjust;
                //Cloud TR
                spriteCloudTR.position.x = rendererWidth - spriteCloudTR.width + cornerAdjust;
                spriteCloudTR.position.y = 0 - cornerAdjust - 40;
                //Cloud BL
                spriteCloudBL.position.x = 0 - cornerAdjust;
                spriteCloudBL.position.y = rendererHeight - spriteCloudBL.height + cornerAdjust;
                //Cloud BR
                spriteCloudBR.position.x = rendererWidth - spriteCloudBR.width + cornerAdjust;
                spriteCloudBR.position.y = rendererHeight - spriteCloudBR.height + cornerAdjust;

                //Shadow Border asset
                spriteBorder.width = rendererWidth;
                spriteBorder.height = rendererHeight;

                container.entity.updateTick = 0;
            }

        }

        container.entity.remove = function () {
            _pixiStage.removeChild(container);
        }

        container.addChild(spriteBorder);

        container.addChild(spriteCloudTR);
        container.addChild(spriteCloudTL);
        container.addChild(spriteCloudBR);
        container.addChild(spriteCloudBL);

        _pixiStage.addChild(container);

        _childrenChanged();

    }

    function _addVillageFire(x, y) {

        var sheetCreateData = {
            id: 'villageFireSheet',
            src: "https://static.realmofempires.com/images/animation/fireroll02.png",
            frameW: 64,
            frameH: 64,
            columns: 4,
            frameCount: 6,
            loadFunc: _spriteReadyFunction
        }

        _loadCreateSpriteSheet(sheetCreateData);

        function _spriteReadyFunction() {

            var container = new PIXI.Container();

            container.entity = {
                type: "villageFire",
                z: 0,
                fireCount: 0,
                age: 0,
                oX: x,
                oY: y,
                x: 0,
                y: 0,
                smokeTicker: 30
            }

            container.entity.tick = function () {

                container.entity.x = container.entity.oX * _landMark.landpx + _landMark.rx + _landMark.landpx / 2;
                container.entity.y = -container.entity.oY * _landMark.landpx + _landMark.ry + _landMark.landpx / 2 + (-10 * _landMark.scale);

                if (container.entity.fireCount < 8 && container.entity.age % 15 == 0) {
                    villageFireSprite();
                    container.entity.fireCount++;
                }

                var child;
                for (var i = 0; i < container.children.length; i++) {
                    child = container.children[i];
                    child.entity.tick();
                }

                container.entity.smokeTicker++;
                if (container.entity.smokeTicker > 30) {

                    _loadCreateSpriteSheet({
                        id: 'villageSmokeSheet',
                        src: "https://static.realmofempires.com/images/animation/smokeroll03.png",
                        frameW: 64,
                        frameH: 64,
                        columns: 4,
                        frameCount: 6,
                        loadFunc: function () {
                            addSmokePuff();
                        }
                    });

                    container.entity.smokeTicker = 0;
                }

                container.entity.age++;
            }

            function villageFireSprite() {

                var sprite = new PIXI.extras.MovieClip(_cache.sheets['villageFireSheet']);
                sprite.anchor.x = 0.5;
                sprite.anchor.y = 0.5;
                sprite.rotation = Math.random();
                sprite.alpha = .6;
                sprite.animationSpeed = .08;
                sprite.blendMode = PIXI.BLEND_MODES.ADD;

                sprite.entity = {
                    type: "villageFire1",
                    cX: 0,
                    cY: 0,
                    vY: 0,
                    vX: 0,
                    age: 0
                };

                sprite.entity.tick = function () {

                    if (sprite.currentFrame == 5) {
                        container.removeChild(sprite);
                        container.entity.fireCount--;
                    }

                    sprite.scale.x = .6 * _landMark.scale;
                    sprite.scale.y = .6 * _landMark.scale;

                    if (sprite.entity.age > 10) {
                        sprite.entity.vY -= .05;
                        sprite.entity.vX += .01;
                        sprite.rotation += .005;
                        sprite.alpha -= .0015;
                    }

                    sprite.entity.cX += sprite.entity.vX;
                    sprite.entity.cY += sprite.entity.vY;

                    sprite.position.x = container.entity.x + sprite.entity.cX;
                    sprite.position.y = container.entity.y + sprite.entity.cY;

                    sprite.entity.age++;
                }

                container.addChild(sprite);
                sprite.gotoAndPlay(Math.floor(Math.random() * 3));

            }

            function addSmokePuff() {

                var sprite = new PIXI.extras.MovieClip(_cache.sheets['villageSmokeSheet']);
                sprite.anchor.x = 0.5;
                sprite.anchor.y = 0.5;
                sprite.alpha = 1;
                sprite.animationSpeed = .01;
                //sprite.blendMode = PIXI.BLEND_MODES.ADD;

                sprite.entity = {
                    type: "villageSmoke",
                    cX: 0,
                    cY: 0,
                    vY: -.15,
                    vX: 0,
                    age: 0,
                    cS: 0,
                };

                sprite.entity.tick = function () {

                    sprite.scale.x = .3 * _landMark.scale + (sprite.entity.cS);
                    sprite.scale.y = .3 * _landMark.scale + (sprite.entity.cS);
                    sprite.entity.cS += .0005;

                    sprite.entity.vY -= .0003;
                    sprite.entity.vX += .0003;

                    sprite.entity.cX += sprite.entity.vX;
                    sprite.entity.cY += sprite.entity.vY;

                    sprite.position.x = container.entity.x + sprite.entity.cX;
                    sprite.position.y = container.entity.y + sprite.entity.cY;

                    sprite.entity.age++;
                    sprite.alpha -= .0015;
                    if (sprite.alpha < .1) {
                        container.removeChild(sprite);
                        return;
                    }
                }

                //container.addChild(sprite);
                container.addChildAt(sprite, 0);
                sprite.gotoAndPlay(Math.floor(Math.random() * 5));

            }

            _pixiStage.addChild(container);
        }

    }

    //the feedback placeholder between a fresh command and inout change update, return sthe animation entity if needed for use
    function _awaitingTroopCommand(data) {

        if (_paused) {
            return;
        }

        return _addTroopCommandPlaceholder({
            ovx: data.ovx,
            ovy: data.ovy,
            dvx: data.dvx,
            dvy: data.dvy
        });

    }

    function _addTroopCommandPlaceholder(data) {

        var troopLine = new PIXI.Graphics();
        var troopSprite = new PIXI.Sprite.fromImage('https://static.realmofempires.com/images/animation/HourGlass44.png');
        var troopContainer = new PIXI.Container();

        troopLine.entity = {
            type: "troopPhLine",
            ovx: data.ovx,
            ovy: data.ovy,
            dvx: data.dvx,
            dvy: data.dvy,
            angle: null,
            length: 0
        }

        troopSprite.entity = {
            type: "troopPhIcon",
        }

        troopContainer.entity = {
            type: "troopPhCon",
            age: 0
        };

        troopContainer.entity.tick = function () {

            troopContainer.entity.age++;
            if (troopContainer.entity.age > 600) {
                return troopContainer.entity.remove();
            }


            var lrxoffset = _landMark.rx + _landMark.landpx / 2;
            var lryoffset = _landMark.ry + _landMark.landpx / 2;

            var ox = troopLine.entity.ovx * _landMark.landpx + lrxoffset;
            var oy = -troopLine.entity.ovy * _landMark.landpx + lryoffset;

            var dx = troopLine.entity.dvx * _landMark.landpx + lrxoffset;
            var dy = -troopLine.entity.dvy * _landMark.landpx + lryoffset;

            if (troopLine.entity.angle == null) {
                troopLine.entity.angle = Math.atan2(oy - dy, ox - dx);
            }

            var angle = troopLine.entity.angle;
            var length = troopLine.entity.length = Math.sqrt(Math.pow((ox - dx), 2) + Math.pow((oy - dy), 2)); //length in pixels

            //draw the line
            troopLine.clear();
            troopLine.lineStyle(2, 0x808080);
            troopLine.moveTo(ox, oy);
            troopLine.lineTo(dx, dy);

            //draw arrowhead
            var headlen = (12 * _landMark.scale) + 3;
            troopLine.beginFill(0x808080);
            troopLine.moveTo(dx, dy);
            troopLine.lineTo(dx + headlen * Math.cos(angle - Math.PI / 7), dy + headlen * Math.sin(angle - Math.PI / 7));
            troopLine.lineTo(dx + headlen * Math.cos(angle + Math.PI / 7), dy + headlen * Math.sin(angle + Math.PI / 7));
            troopLine.lineTo(dx, dy);
            troopLine.lineTo(dx + headlen * Math.cos(angle - Math.PI / 7), dy + headlen * Math.sin(angle - Math.PI / 7));
            troopLine.endFill();

            //scale and place it in center of line
            troopSprite.scale.x = _landMark.scale * .8;
            troopSprite.scale.y = _landMark.scale * .8;
            troopSprite.pivot.set(troopSprite.width / 1.55, troopSprite.height / 1.55);

            var traveledDistance = length * .5;
            troopSprite.position.x = ox + (Math.cos(angle) * -(traveledDistance));
            troopSprite.position.y = oy + (Math.sin(angle) * -(traveledDistance));

            //rotate the sprite
            troopSprite.rotation += .01;

        }

        troopContainer.entity.remove = function () {
            _pixiStage.removeChild(troopContainer);
        }

        troopContainer.addChild(troopLine);
        troopContainer.addChild(troopSprite);

        _pixiStage.addChild(troopContainer);

        return troopContainer;

    }

    function _addTroopMovement(data) {
        //console.log('_addTroopMovement',data);
        if (!data.eid) {
            return;
        }

        if (data.etime < (new Date().valueOf())) {
            return;
        }

        var textureToken; //troop token
        //var textureTargetShape; //target shape
        var textureLine; //the movement line
        var tpVX; //tileposition change velocity
        var tint = null; //default tint //0xF00000 <red
        var colorMatrix = null;
        //var visibleTarget = true;
        var visibleToken = true;

        ///
        ///Directions = { incoming: 0, outgoing: 1 };
        ///CommandTypes = { support: 0, attack: 1, returning: 2, supportRecall: 3 };
        ///
        if (data.direction == 1) {

            //outgoing commons
            tpVX = .2;
            //tint = 0x25FF00;
            //colorMatrix.brightness(2);

            //outgoing attack
            if (data.type == 1) {
                textureToken = _textureById('map_TokenOutAttk');
                //textureTargetShape = _textureById('map_indOutAttk');
                textureLine = _textureById('map_lineOutAttk');

            } else if (data.type == 0) {
                textureToken = _textureById('map_TokenOutSupport');
                //textureTargetShape = _textureById('map_indOutSupport');
                textureLine = _textureById('map_lineOutSupport');
            }

        } else {

            //incomming commons
            tpVX = .2;
            //tint = 0x0070FF;
            //colorMatrix.brightness(3);


            //incoming attack
            if (data.type == 1) {

                /*
                // INVIS INC ATTACKS
                textureToken = PIXI.Texture.EMPTY;
                //textureTargetShape = _textureById('map_indIncAttk');
                textureLine = PIXI.Texture.EMPTY;
                visibleToken = false;
                */

                textureToken = _textureById('map_TokenIncAttk2');
                textureLine = _textureById('map_lineIncAttk');

                //colorMatrix = new PIXI.filters.ColorMatrixFilter();
                //colorMatrix.brightness(2);
                //tint = 0xEA0000;


                //returning
            } else if (data.type == 2 || data.type == 3) {

                textureToken = _textureById('map_TokenReturning');
                //textureTargetShape = PIXI.Texture.EMPTY;
                //visibleTarget = false;  
                textureLine = _textureById('map_lineReturning');

                //incoming support
            } else {

                /*
                // INVIS INC SUPPORT
                textureToken = PIXI.Texture.EMPTY;
                //textureTargetShape = PIXI.Texture.EMPTY;            
                //visibleTarget = false;
                textureLine = PIXI.Texture.EMPTY;
                */

                textureToken = _textureById('map_TokenIncSupport');
                textureLine = _textureById('map_lineIncSupport');

                //colorMatrix = new PIXI.filters.ColorMatrixFilter();
                //colorMatrix.brightness(2);
                //tint = 0x54FF00;


            }

        }

        //determing facing
        var facingRight = true;
        if (data.dvx < data.ovx) {
            facingRight = false;
        }


        var troopLine = new PIXI.extras.TilingSprite(textureLine, 0, 8);
        //var troopTarget = new PIXI.Sprite(textureTargetShape);
        var troopSprite = new PIXI.Sprite(textureToken);
        var troopContainer = new PIXI.Container();

        if (colorMatrix) {
            troopLine.filters = [colorMatrix];
            troopSprite.filters = [colorMatrix];
        }

        if (tint) {
            troopLine.tint = tint;
            troopSprite.tint = tint;
        }

        troopLine.anchor.set(0, .5);
        troopLine.entity = {
            type: "troopLine",
            ovx: data.ovx,
            ovy: data.ovy,
            dvx: data.dvx,
            dvy: data.dvy,
            speed: data.speed,
            angle: null,
            length: 0,
            tpX: 0, //tileposition
            tpY: 0,//tileposition
            tpVX: tpVX, //tile position change velocity
            tint: tint
        }

        /*
        troopTarget.anchor.set(.5, .5);
        //troopTarget.filters = [colorMatrix];
        //troopTarget.tint = tint;
        troopTarget.entity = {}
        */

        troopSprite.anchor.set(.5, .86);
        troopSprite.entity = {
            type: "troopSprite",
            changeX: 0,
            changeY: 0,
            flipX: facingRight ? 1 : -1
        }

        troopSprite.interactive = true;

        /* why the hell does it call mouse move on all of stage?
        troopSprite.on('mousemove', function () {
            _troopTokenMouseOver();
        });
        */

        troopSprite.on('mouseover', function () {
            _troopTokenMouseOver();
        });
        troopSprite.on('mouseout', function () {
            _troopTokenMouseOut();
        });

        var distance = Math.sqrt(Math.pow((data.ovx - data.dvx), 2) + Math.pow((data.ovy - data.dvy), 2)) //distance in squares
        var duration = data.etime - data.starttime;

        troopContainer.entity = {
            type: "troopCon",
            eid: data.eid,
            etime: data.etime,
            starttime: data.starttime,
            duration: duration,
            distance: distance,
            speed: data.speed,//distance / duration, //SQ per MS
            age: 0
        };

        troopContainer.entity.hoverInfo = function () {
            return {
                details: data, pos: troopSprite.position
            }
        }

        troopContainer.entity.tokenBound = function () {
            return troopSprite.getBounds();
        }

        //console.log('ADD TROOP MOVEMENT', troopContainer.entity.eid);

        troopContainer.entity.tick = function () {

            //for 1 second checks
            if (troopContainer.entity.age % 30 == 0) {
                //turn troop line on/off if a current one from origin to destination exists
                var curLine = _currentLines[data.ovid + ',' + data.dvid];
                if (!curLine) {
                    _currentLines[data.ovid + ',' + data.dvid] = data.eid;
                    troopLine.visible = true;
                } else {
                    if (curLine == data.eid) {
                        troopLine.visible = true;
                    } else {
                        troopLine.visible = false;
                    }
                }
            }

            var lrxoffset = _landMark.rx + _landMark.landpx / 2;
            var lryoffset = _landMark.ry + _landMark.landpx / 2;
            var ox = troopLine.entity.ovx * _landMark.landpx + lrxoffset;
            var oy = -troopLine.entity.ovy * _landMark.landpx + lryoffset;
            var dx = troopLine.entity.dvx * _landMark.landpx + lrxoffset;
            var dy = -troopLine.entity.dvy * _landMark.landpx + lryoffset;

            if (troopLine.entity.angle == null) {
                troopLine.entity.angle = Math.atan2(oy - dy, ox - dx);
            }

            var angle = troopLine.entity.angle;
            var length = troopLine.entity.length = Math.sqrt(Math.pow((ox - dx), 2) + Math.pow((oy - dy), 2)); //length in pixels

            if (troopLine.visible) {
                troopLine.position.x = ox;
                troopLine.position.y = oy;
                troopLine.width = -length;  //-length + (20 * _landMark.scale);
                troopLine.rotation = angle;
                troopLine.tilePosition.set(troopLine.entity.tpX, 0);
                //troopLine.tileScale.set(_landMark.scale,_landMark.scale);   
                troopLine.entity.tpX += -troopLine.entity.tpVX;
            }

            /*
            troopTarget.scale.x = _landMark.scale * 1;
            troopTarget.scale.y = _landMark.scale * 1;
            troopTarget.position.x = dx;
            troopTarget.position.y = dy;
            */

            var start = troopContainer.entity.starttime; //launchtime in ms
            if (start && troopContainer.entity.speed) {
                var now = new Date().valueOf() + ROE.timeOffset; //now in ms

                var elapsedTime = now - start; //elapsed time in ms
                var sqTraveled = elapsedTime * (troopContainer.entity.speed / 3600000); //time * speed (sq per ms)
                var travelPercent = sqTraveled / troopContainer.entity.distance; //% of road traveled


                if (travelPercent >= 1) {
                    travelPercent = 1;

                    if (data.direction == 1 && data.type == 1) {
                        _addCmdAwaitingResolution({
                            vx: troopLine.entity.dvx, vy: troopLine.entity.dvy, type: 1, eid: troopContainer.entity.eid
                        });
                    }
                    _currentLines[data.ovid + ',' + data.dvid] = false;
                    troopContainer.entity.remove();
                    return;

                }

                var traveledDistance = length * travelPercent; //length * Math.min(travelPercent, 1); //distance travelled in pixels

                troopSprite.scale.x = _landMark.scale * troopSprite.entity.flipX;
                troopSprite.scale.y = _landMark.scale;// * troopSprite.entity.flipY;
                troopSprite.position.x = Math.round(ox + (Math.cos(angle) * -(traveledDistance)));
                troopSprite.position.y = Math.round(oy + (Math.sin(angle) * -(traveledDistance)));
            }

            troopContainer.entity.age++;

        }

        troopContainer.entity.remove = function () {
            _pixiStage.removeChild(troopContainer);
        }

        troopContainer.addChild(troopLine);

        /*
        if (visibleTarget) {
            troopContainer.addChild(troopTarget);
        }
        */

        if (visibleToken) {
            troopContainer.addChild(troopSprite);
        }

        _pixiStage.addChild(troopContainer);
    }


    ///start of troop interaction

    function _troopTokenMouseOver() {

        var tokenInfoPack = $('#tokenInfoPack');
        if (tokenInfoPack.hasClass('hover')) {
            return;
        }

        var mousePosition = _pixiRenderer.plugins.interaction.mouse.global;
        var mX = mousePosition.x;
        var mY = mousePosition.y;

        var rect;
        var hoverInfoArray = [];

        for (var i = 0; i < _pixiStage.children.length; i++) {
            if (_pixiStage.children[i].entity.type == 'troopCon') {

                rect = _pixiStage.children[i].entity.tokenBound();

                if (mX + 5 > rect.x && mX - 5 < rect.x + rect.width && mY + 5 > rect.y && mY - 5 < rect.y + rect.height) {
                    //if (mX > rect.x && mX < rect.x + rect.width && mY > rect.y && mY < rect.y + rect.height) {
                    hoverInfoArray.push(_pixiStage.children[i].entity.hoverInfo());
                }

            }
        }

        if (hoverInfoArray.length < 1) {
            $('#tokenInfoPack').remove();
            return;
        }

        var newPack = true;
        var tokenInfoPack = $('#tokenInfoPack');
        if (tokenInfoPack.length) {
            newPack = false;
            clearTimeout(tokenInfoPack.data('timeout'));
            $('.mapTroopHover', tokenInfoPack).addClass('unscanned');
        } else {
            tokenInfoPack = $('<div id="tokenInfoPack" >');

            tokenInfoPack.on('mouseenter', function () {
                tokenInfoPack.addClass('hover');
            });

            tokenInfoPack.on('mouseleave', function () {
                tokenInfoPack.removeClass('hover');
                _troopTokenMouseOut();
            });


        }


        for (var i = 0; i < hoverInfoArray.length; i++) {
            var data = hoverInfoArray[i];


            var element = $('#mth' + data.details.eid);
            if (element.length) {
                element.removeClass('unscanned');
                continue;
            }

            var cmd = ROE.Troops.InOut2.findCommandByEventID_Rich(data.details.direction, data.details.eid);

            var tHover = $('<div id="mth' + data.details.eid + '" class="fontSilverFrLCmed mapTroopHover">');
            tHover.data('eid', data.details.eid);

            var tInfo = $('<div>' +

                //'<div style="position:absolute; top:0px; left:0px; color:lime; background:black;z-index: 1;padding: 3px;">' + data.details.eid + '</div>' +
                '<div class="vRow">' + cmd.originVillage.name + '<span> (' + cmd.originVillage.x + ',' + cmd.originVillage.y + ')</span></div>' +
                '<div class="vRow">' + cmd.destinationVillage.name + '<span> (' + cmd.destinationVillage.x + ',' + cmd.destinationVillage.y + ')</span></div>' +
                '<div class="tIcon t' + data.details.type + '"></div>' +
                '<div class="counter countdown" data-finisheson="' + data.details.etime + '" data-refreshcall="$(\'#mth' + data.details.eid + '\').remove();" >--:--:--</div>' +
                '<div class="effects"></div>' +
                '<div class="troops"></div>' +

            '</div>');

            if (data.details.type == 1 && data.details.isUnderBloodLust) {
                $('.effects', tInfo).show().append('<div class="effectIcon lust"></div>');
            }

            if (data.details.speed) {
                $('.effects', tInfo).show().append('<div class="effectIcon speed">' + data.details.speed + '</div>');
            }

            if (ROE.PlayerMoraleSettings.isActiveOnThisRealm && data.details.morale != undefined) {
                $('.effects', tInfo).show().append('<div class="effectIcon morale">' + data.details.morale + '</div>');
            }

            tHover.append(tInfo);

            if (ROE.Troops.InOutWidget.storedInOutEvents[data.details.eid]) {
                tHover.removeClass('busy').addClass('loaded');
                _tHoverPopulate(tHover, ROE.Troops.InOutWidget.storedInOutEvents[data.details.eid].troops);
            }

            tHover.on('mouseenter', function () {
                _tHoverMouseEnter($(this));
            });

            tHover.on('click', function () {
                ROE.Frame.popupInOut(data.details.direction == 0 ? 'in' : 'out');
            });


            tokenInfoPack.append(tHover);

            if (i >= 4) {

                break;
            }


        }

        $('.mapTroopHover.unscanned', tokenInfoPack).remove();

        $('body').append(tokenInfoPack);



        //tHover.data('x', eX);
        //tHover.data('y', eY);

        var x = hoverInfoArray[0].pos.x;
        var y = hoverInfoArray[0].pos.y;
        if (newPack) {
            tokenInfoPack.css({
                left: x - tokenInfoPack.width() / 2,
                top: y + 25
            });
            tokenInfoPack.stop().animate({ top: '-=10px' }, 500, "easeOutSine");
        } else {
            tokenInfoPack.css({
                left: x - tokenInfoPack.width() / 2,
                top: y + 15
            });
        }


    }




    function _tHoverPopulate(tHoverElement, troopsArray) {

        var troopsElement = $('.troops', tHoverElement);
        var unitEnt;
        if (troopsArray.length) {
            for (var i = 0, troop; troop = troopsArray[i]; ++i) {
                unitEnt = ROE.Entities.UnitTypes[troop.id];
                troopsElement.append('<div class="unit" data-unitid="' + troop.id + '" style="background-image:url(' + unitEnt.IconUrl + ');" >' + troop.count + '</div>');
            }
        } else {
            troopsElement.append('<div>unknown troop info</div>')
        }


    }

    function _tHoverMouseEnter(tHoverElement) {

        if (tHoverElement.hasClass('loaded')) {
            return;
        }

        tHoverElement.addClass('busy');
        ROE.Api.call_troopMove_getDetails(function (getDetailsCBdata) {
            tHoverElement.removeClass('busy').addClass('loaded');
            ROE.Troops.InOutWidget.storedInOutEvents[getDetailsCBdata.eventID] = getDetailsCBdata;
            _tHoverPopulate(tHoverElement, getDetailsCBdata.troops);
        }, tHoverElement.data('eid'));


    }

    function _troopTokenMouseOut() {
        var tokenInfoPack = $('#tokenInfoPack');
        clearTimeout(tokenInfoPack.data('timeout'));
        var timeout = setTimeout(function () {
            if (!tokenInfoPack.hasClass('hover')) {
                _troopTokenMouseOver(); //one last mousecheck
            }
        }, 250);
        tokenInfoPack.data('timeout', timeout);
    }

    ///end of troop interaction




    function _addCmdAwaitingResolution(data) {

        var sheetCreateData = {
            id: 'clashResolution',
            src: "https://static.realmofempires.com/images/animation/LoM_SwordclashSpriteSheet.png",
            frameW: 94,
            frameH: 63,
            columns: 20,
            frameCount: 34,
            loadFunc: _spriteReadyFunction
        }

        _loadCreateSpriteSheet(sheetCreateData);

        function _spriteReadyFunction() {

            var sprite = new PIXI.extras.MovieClip(_cache.sheets['clashResolution']);
            sprite.anchor.x = 0.5;
            sprite.anchor.y = 0.5;
            sprite.animationSpeed = .3;
            sprite.entity = {
                type: "clashSprite",
                cX: 0,
                cY: 0,
                vY: 0,
                vX: 0
            };

            var container = new PIXI.Container();

            container.entity = {
                type: "clashResolution",
                z: 3,
                oX: data.vx,
                oY: data.vy,
                x: 0,
                y: 0,
                eid: data.eid
            }

            container.entity.tick = function () {

                container.entity.x = container.entity.oX * _landMark.landpx + _landMark.rx + _landMark.landpx / 2;
                container.entity.y = -container.entity.oY * _landMark.landpx + _landMark.ry + _landMark.landpx / 2 + (-20 * _landMark.scale);

                sprite.scale.x = 1 * _landMark.scale;
                sprite.scale.y = 1 * _landMark.scale;
                sprite.position.x = container.entity.x;
                sprite.position.y = container.entity.y;

            }

            container.entity.remove = function () {
                _pixiStage.removeChild(container);
            }

            container.addChild(sprite);
            _pixiStage.addChild(container);

            sprite.gotoAndPlay(0);
        }

    }

    function _addCloud(x, y, tex) {
        var sprite = new PIXI.Sprite(tex);
        sprite.anchor.x = 0.5;
        sprite.anchor.y = 0.5;
        sprite.position.x = x;
        sprite.position.y = y;
        sprite.scale.x = .5;
        sprite.scale.y = .5;

        sprite.interactive = true;
        sprite.on('mouseover', function () { sprite.alpha = .3; })
            .on('mouseout', function () { sprite.alpha = 1; })
            .on('click', function () {
                sprite.position.x += 20;
            })

        sprite.entity = {
            type: "cloud",
            z: 10
        };

        sprite.entity.tick = function () {
            sprite.position.x += .04;
        }

        _pixiStage.addChild(sprite);
    }

    function _addDefinedTarget(target) {
        //console.log('_addDefinedTarget', target);

        var spriteId = target.Type == 1 ? 'targetSupport' : 'targetAttack';

        //is target defined by me or someone else
        var targetIsMine = target.TargetDefinitionOwnerPlayerID == ROE.playerID;

        var sprite = new PIXI.Sprite(_textureById(spriteId));

        sprite.anchor.x = 0.5;
        sprite.anchor.y = 0.5;

        var container = new PIXI.Container();

        container.entity = {
            type: "definedTarget",
            z: 3,
            oX: target.TargetVillageXCord,
            oY: target.TargetVillageYCord,
            x: 0,
            y: 0,
            cY: 0 - (10 * Math.random()),
            cYV: _landMark.landpx / 50,
            keep: true, //will be used to clean up entities of this kind
            target: target,
            mine: targetIsMine
        }

        container.entity.tick = function () {

            //we want to distinguish animations of your stuff vs others
            if (container.entity.mine) {

                if (container.entity.cY > -(_landMark.landpx / 15)) {
                    container.entity.cYV = -_landMark.landpx / 100;
                }

                container.entity.cY += container.entity.cYV;
                container.entity.cYV += .01;

            } else {

                if (container.entity.cY > -(_landMark.landpx / 15)) {
                    container.entity.cYV = -_landMark.landpx / 50;
                }

                container.entity.cY += container.entity.cYV;
                container.entity.cYV += .05;

            }

            container.entity.x = container.entity.oX * _landMark.landpx + _landMark.rx + _landMark.landpx / 2;
            container.entity.y = (-container.entity.oY * _landMark.landpx + _landMark.ry + _landMark.landpx / 2 + (-20 * _landMark.scale)) + container.entity.cY;

            //sprite.scale.x = 1 * _landMark.scale;
            //sprite.scale.y = 1 * _landMark.scale;
            var size = Math.round(_landMark.landpx * .80);
            sprite.width = size; //setting width will auto adjust scale from original texture
            sprite.height = size;

            container.position.x = container.entity.x;
            container.position.y = container.entity.y;


            var showResponses = false;
            if (size > 25) { // > 35
                showResponses = true;
            }

            if (responseContainer.children[0]) {
                responseContainer.children[0].position.x = -size / 4;
                responseContainer.children[0].position.y = size / 2;
                responseContainer.children[0].visible = showResponses;
            }

            if (responseContainer.children[1]) {
                responseContainer.children[1].position.x = 0;
                responseContainer.children[1].position.y = size / 2;
                responseContainer.children[1].visible = showResponses;
            }

            if (responseContainer.children[3]) {
                responseContainer.children[3].position.x = size / 4;
                responseContainer.children[3].position.y = size / 2;
                responseContainer.children[3].visible = showResponses;
            }


        }

        container.entity.remove = function () {
            _pixiStage.removeChild(container);
        }

        //adds response numbers to a container, based on target object responses
        var responseContainer = new PIXI.Container(); //will house response numbers
        container.entity.setupResponses = function () {

            responseContainer.removeChildren();

            var response;
            var countType1 = 0;
            var countType2 = 0;
            var countType3 = 0;
            for (var i = 0; i < container.entity.target.Responses.length; i++) {
                response = container.entity.target.Responses[i];
                if (response.ResponseTypeID == 1) { countType1++; }
                else if (response.ResponseTypeID == 2) { countType2++; }
                else if (response.ResponseTypeID == 3) {
                    countType3++;
                }
            }

            if (countType1) {
                var text1 = new PIXI.Text(countType1, {
                    fontFamily: 'Arial', fontSize: 19, fill: 'red', stroke: 'black', strokeThickness: 6
                });
                text1.anchor.x = 0.5;
                text1.anchor.y = 0.5;
                responseContainer.addChild(text1);
            }

            if (countType2) {
                var text2 = new PIXI.Text(countType2, {
                    fontFamily: 'Arial', fontSize: 19, fill: 'yellow', stroke: 'black', strokeThickness: 6
                });
                text2.anchor.x = 0.5;
                text2.anchor.y = 0.5;
                responseContainer.addChild(text2);
            }

            if (countType3) {
                var text3 = new PIXI.Text(countType3, {
                    fontFamily: 'Arial', fontSize: 19, fill: 'lime', stroke: 'black', strokeThickness: 6
                });
                text3.anchor.x = 0.5;
                text3.anchor.y = 0.5;
                responseContainer.addChild(text3);
            }

        }

        container.addChild(sprite);
        container.addChild(responseContainer);
        container.entity.setupResponses();
        _pixiStage.addChild(container);

    }





    ///
    ///Directions = { incoming: 0, outgoing: 1 };
    ///CommandTypes = { support: 0, attack: 1, returning: 2, supportRecall: 3 };
    ///
    function _animationInOutChanged() {

        if (_paused) {
            return;
        }

        _currentLines = {
        }; //reset the current lines storage
        _removeChildrenByType("clashResolution");      //remove awaitng resolution objects //is this a good idea?
        _removeChildrenByType("troopPhCon");    //remove placeholder commands
        _removeChildrenByType("troopCon");      //remove previous movements, to make new ones

        if (_settings.direction == 0) {
            _animateIncoming();
        } else if (_settings.direction == 1) {
            _animateOutgoing();
        } else if (_settings.direction == 2) {
            _animateIncoming();
            _animateOutgoing();
        } else {
            //do nothing if _settings.direction == 3
        }

        _childrenChanged();

    }

    function _animateOutgoing() {
        var outGoingData = ROE.Troops.InOut2.getData(1);
        var outGoingCommands = outGoingData.commands;
        var outGoingVillages = outGoingData.villages;
        //console.log('outGoingData', outGoingData);

        var cmd, ov, dv;
        for (var i = 0; i < outGoingCommands.length; i++) {
            cmd = outGoingCommands[i];

            if (cmd.hidden) {
                continue;
            }

            ov = outGoingVillages[cmd.ovid];
            dv = outGoingVillages[cmd.dvid];

            _addTroopMovement({
                ovid: cmd.ovid,
                dvid: cmd.dvid,
                ovx: ov.x,
                ovy: ov.y,
                dvx: dv.x,
                dvy: dv.y,
                eid: cmd.eid,
                etime: cmd.etime,
                starttime: cmd.starttime,
                hidden: cmd.hidden,
                direction: 1, //outgoing
                type: cmd.type,
                isUnderBloodLust: cmd.isUnderBloodLust,
                morale: cmd.morale,
                speed: cmd.speed
            });
        }
    }

    function _animateIncoming() {
        var incomingData = ROE.Troops.InOut2.getData(0);
        var incomingCommands = incomingData.commands;
        var incomingVillages = incomingData.villages;
        //console.log('incomingData', incomingCommands);

        var cmd, ov, dv;
        for (var i = 0; i < incomingCommands.length; i++) {
            cmd = incomingCommands[i];

            if (cmd.hidden) {
                continue;
            }

            ov = incomingVillages[cmd.ovid];
            dv = incomingVillages[cmd.dvid];

            //make sure there is OV, because some incomming might be from unkown vills
            if (cmd.starttime && ov) { //for now some incomming wont have start time

                _addTroopMovement({
                    ovid: cmd.ovid,
                    dvid: cmd.dvid,
                    ovx: ov.x,
                    ovy: ov.y,
                    dvx: dv.x,
                    dvy: dv.y,
                    eid: cmd.eid,
                    etime: cmd.etime,
                    starttime: cmd.starttime,
                    hidden: cmd.hidden,
                    direction: 0, //incoming
                    type: cmd.type,
                    speed: cmd.speed
                });

            }

        }
    }



    function _animateDefinedTargets() {

        if (!_pixiStage) {
            return;
        }

        if (_settings.state_targets == 0) {
            return;
        }

        if (!ROE.Player.Targets) {
            return;
        }

        //the idea behind doing it this complicated way and not simply removing and rebuilding targets per list
        //is because targets have a bounce state, and dont want to cause jitters per refresh

        var targetsPlaced = {
        };
        var currentTargetChildren = []; //current pixi children of type definedTarget
        var entity;
        for (var i = _pixiStage.children.length - 1; i >= 0; i--) {
            entity = _pixiStage.children[i].entity;
            if (entity.type == "definedTarget") {
                entity.keep = false; //will later set to true if to keep, else itll get removed
                currentTargetChildren.push(entity);
                targetsPlaced[entity.oX + "," + entity.oY] = entity.target.DefinedTargetID;
            }
        }

        //loop all targets, if child not already placed on a coord, create new one
        //if already placed, check to make sure
        var target, entity;
        for (var i = 0; i < ROE.Player.Targets.length; i++) {
            target = ROE.Player.Targets[i];
            //if not at least one in a coord, create a new one there
            if (!targetsPlaced[target.TargetVillageXCord + "," + target.TargetVillageYCord]) {
                targetsPlaced[target.TargetVillageXCord + "," + target.TargetVillageYCord] = target.DefinedTargetID;
                _addDefinedTarget(target);
                //if one IS found on a cord, look it up and set it to be kept, so is not removed
            } else {
                for (var c = 0; c < currentTargetChildren.length; c++) {
                    entity = currentTargetChildren[c];
                    if (entity.target.DefinedTargetID == target.DefinedTargetID) {
                        entity.keep = true;
                        entity.target = target;
                        entity.setupResponses();
                        break;
                    }
                }
            }
        }

        //remove children that were not to be kept (not in array of targets)
        for (var c = 0; c < currentTargetChildren.length; c++) {
            entity = currentTargetChildren[c];
            if (!entity.keep) {
                entity.remove();
            }
        }

        _childrenChanged();

    }

    function _childrenChanged() {

        //sort Z state
        _pixiStage.children.sort(function (a, b) {
            a.entity.z = a.entity.z || 0;
            b.entity.z = b.entity.z || 0;
            return a.entity.z - b.entity.z;
        });

    }

    function _openSettingsUI() {

        var popup = $('.quickPopup.animationSettings');

        if (popup.length) {
            $('.quickPopup.animationSettings').remove();
            return;
        }

        var settings = {
            content: "<div></div>",
            title: "Animation Settings",
            customQuickPopupContainerClass: "animationSettings",
            closeFunction: function () {
                $('.quickPopup.animationSettings').remove();
            }
        }
        ROE.Frame.quickPopup(settings);

        var qpContent = $('.quickPopup.animationSettings .qpContent');

        $('.quickPopup.animationSettings').mouseover(function () {
            $('#mapGuiHover, #mapGuiSelected').hide();
        })

        var btnTogglePause = $('<div class="setBtn togglePause helpTooltip" data-toolTipID="animationTogglePause">').click(_togglePause);
        btnTogglePause.addClass(_paused ? 'off' : 'on');
        qpContent.append(btnTogglePause);

        var btnToggleDirection = $('<div class="setBtn toggleDirection helpTooltip" data-toolTipID="animationToggleDirection" data-direction="' + _settings.direction + '">')
        .click(_toggleDirection);
        qpContent.append(btnToggleDirection);

        var btnToggleTargetState = $('<div class="setBtn toggleTargetState helpTooltip" data-toolTipID="animationToggleTargetState" data-state="' + _settings.state_targets + '">')
        .click(_toggleTargetState);
        qpContent.append(btnToggleTargetState);

        if (ROE.isD2 && ROE.Entities.Realm.Theme > 1) {
            var btnToggleVigState = $('<div class="setBtn toggleVigState helpTooltip" data-toolTipID="animationToggleVigState" data-state="' + _settings.state_vig + '">')
            .click(_toggleVigState);
            qpContent.append(btnToggleVigState);
        }

    }

    obj.openSettingsUI = _openSettingsUI;

    obj.initMapAnimations = _initMapAnimations;
    obj.pause = _pause;
    obj.resume = _resume;
    obj.togglePause = _togglePause;
    obj.isAnimationEngineOn = _isAnimationEngineOn;
    obj.isAnimationSubsetOn = _isAnimationSubsetOn;
    obj.toggleTargetState = _toggleTargetState;

    obj.mapDrag = _updateAnimaMapMove;
    obj.awaitingTroopCommand = _awaitingTroopCommand;
    obj.vClick = _vClick;

    obj.animateDefinedTargets = _animateDefinedTargets;



    ///FPSMETER
    /*! FPSMeter 0.3.1 - 9th May 2013 | https://github.com/Darsain/fpsmeter 
    (function (m, j) {
        function s(a, e) { for (var g in e) try { a.style[g] = e[g] } catch (j) { } return a } function H(a) { return null == a ? String(a) : "object" === typeof a || "function" === typeof a ? Object.prototype.toString.call(a).match(/\s([a-z]+)/i)[1].toLowerCase() || "object" : typeof a } function R(a, e) { if ("array" !== H(e)) return -1; if (e.indexOf) return e.indexOf(a); for (var g = 0, j = e.length; g < j; g++) if (e[g] === a) return g; return -1 } function I() {
            var a = arguments, e; for (e in a[1]) if (a[1].hasOwnProperty(e)) switch (H(a[1][e])) {
                case "object": a[0][e] =
                I({}, a[0][e], a[1][e]); break; case "array": a[0][e] = a[1][e].slice(0); break; default: a[0][e] = a[1][e]
            } return 2 < a.length ? I.apply(null, [a[0]].concat(Array.prototype.slice.call(a, 2))) : a[0]
        } function N(a) { a = Math.round(255 * a).toString(16); return 1 === a.length ? "0" + a : a } function S(a, e, g, j) { if (a.addEventListener) a[j ? "removeEventListener" : "addEventListener"](e, g, !1); else if (a.attachEvent) a[j ? "detachEvent" : "attachEvent"]("on" + e, g) } function D(a, e) {
            function g(a, b, d, c) { return y[0 | a][Math.round(Math.min((b - d) / (c - d) * J, J))] }
            function r() { f.legend.fps !== q && (f.legend.fps = q, f.legend[T] = q ? "FPS" : "ms"); K = q ? b.fps : b.duration; f.count[T] = 999 < K ? "999+" : K.toFixed(99 < K ? 0 : d.decimals) } function m() {
                z = A(); L < z - d.threshold && (b.fps -= b.fps / Math.max(1, 60 * d.smoothing / d.interval), b.duration = 1E3 / b.fps); for (c = d.history; c--;) E[c] = 0 === c ? b.fps : E[c - 1], F[c] = 0 === c ? b.duration : F[c - 1]; r(); if (d.heat) {
                    if (w.length) for (c = w.length; c--;) w[c].el.style[h[w[c].name].heatOn] = q ? g(h[w[c].name].heatmap, b.fps, 0, d.maxFps) : g(h[w[c].name].heatmap, b.duration, d.threshold,
                    0); if (f.graph && h.column.heatOn) for (c = u.length; c--;) u[c].style[h.column.heatOn] = q ? g(h.column.heatmap, E[c], 0, d.maxFps) : g(h.column.heatmap, F[c], d.threshold, 0)
                } if (f.graph) for (p = 0; p < d.history; p++) u[p].style.height = (q ? E[p] ? Math.round(O / d.maxFps * Math.min(E[p], d.maxFps)) : 0 : F[p] ? Math.round(O / d.threshold * Math.min(F[p], d.threshold)) : 0) + "px"
            } function k() { 20 > d.interval ? (x = M(k), m()) : (x = setTimeout(k, d.interval), P = M(m)) } function G(a) {
                a = a || window.event; a.preventDefault ? (a.preventDefault(), a.stopPropagation()) : (a.returnValue =
                !1, a.cancelBubble = !0); b.toggle()
            } function U() { d.toggleOn && S(f.container, d.toggleOn, G, 1); a.removeChild(f.container) } function V() {
                f.container && U(); h = D.theme[d.theme]; y = h.compiledHeatmaps || []; if (!y.length && h.heatmaps.length) {
                    for (p = 0; p < h.heatmaps.length; p++) {
                        y[p] = []; for (c = 0; c <= J; c++) {
                            var b = y[p], e = c, g; g = 0.33 / J * c; var j = h.heatmaps[p].saturation, m = h.heatmaps[p].lightness, n = void 0, k = void 0, l = void 0, t = l = void 0, v = n = k = void 0, v = void 0, l = 0.5 >= m ? m * (1 + j) : m + j - m * j; 0 === l ? g = "#000" : (t = 2 * m - l, k = (l - t) / l, g *= 6, n = Math.floor(g),
                            v = g - n, v *= l * k, 0 === n || 6 === n ? (n = l, k = t + v, l = t) : 1 === n ? (n = l - v, k = l, l = t) : 2 === n ? (n = t, k = l, l = t + v) : 3 === n ? (n = t, k = l - v) : 4 === n ? (n = t + v, k = t) : (n = l, k = t, l -= v), g = "#" + N(n) + N(k) + N(l)); b[e] = g
                        }
                    } h.compiledHeatmaps = y
                } f.container = s(document.createElement("div"), h.container); f.count = f.container.appendChild(s(document.createElement("div"), h.count)); f.legend = f.container.appendChild(s(document.createElement("div"), h.legend)); f.graph = d.graph ? f.container.appendChild(s(document.createElement("div"), h.graph)) : 0; w.length = 0; for (var q in f) f[q] &&
                h[q].heatOn && w.push({ name: q, el: f[q] }); u.length = 0; if (f.graph) { f.graph.style.width = d.history * h.column.width + (d.history - 1) * h.column.spacing + "px"; for (c = 0; c < d.history; c++) u[c] = f.graph.appendChild(s(document.createElement("div"), h.column)), u[c].style.position = "absolute", u[c].style.bottom = 0, u[c].style.right = c * h.column.width + c * h.column.spacing + "px", u[c].style.width = h.column.width + "px", u[c].style.height = "0px" } s(f.container, d); r(); a.appendChild(f.container); f.graph && (O = f.graph.clientHeight); d.toggleOn && ("click" ===
                d.toggleOn && (f.container.style.cursor = "pointer"), S(f.container, d.toggleOn, G))
            } "object" === H(a) && a.nodeType === j && (e = a, a = document.body); a || (a = document.body); var b = this, d = I({}, D.defaults, e || {}), f = {}, u = [], h, y, J = 100, w = [], W = 0, B = d.threshold, Q = 0, L = A() - B, z, E = [], F = [], x, P, q = "fps" === d.show, O, K, c, p; b.options = d; b.fps = 0; b.duration = 0; b.isPaused = 0; b.tickStart = function () { Q = A() }; b.tick = function () { z = A(); W = z - L; B += (W - B) / d.smoothing; b.fps = 1E3 / B; b.duration = Q < L ? B : z - Q; L = z }; b.pause = function () {
                x && (b.isPaused = 1, clearTimeout(x),
                C(x), C(P), x = P = 0); return b
            }; b.resume = function () { x || (b.isPaused = 0, k()); return b }; b.set = function (a, c) { d[a] = c; q = "fps" === d.show; -1 !== R(a, X) && V(); -1 !== R(a, Y) && s(f.container, d); return b }; b.showDuration = function () { b.set("show", "ms"); return b }; b.showFps = function () { b.set("show", "fps"); return b }; b.toggle = function () { b.set("show", q ? "ms" : "fps"); return b }; b.hide = function () { b.pause(); f.container.style.display = "none"; return b }; b.show = function () { b.resume(); f.container.style.display = "block"; return b }; b.destroy = function () {
                b.pause();
                U(); b.tick = b.tickStart = function () { }
            }; V(); k()
        } var A, r = m.performance; A = r && (r.now || r.webkitNow) ? r[r.now ? "now" : "webkitNow"].bind(r) : function () { return +new Date }; for (var C = m.cancelAnimationFrame || m.cancelRequestAnimationFrame, M = m.requestAnimationFrame, r = ["moz", "webkit", "o"], G = 0, k = 0, Z = r.length; k < Z && !C; ++k) M = (C = m[r[k] + "CancelAnimationFrame"] || m[r[k] + "CancelRequestAnimationFrame"]) && m[r[k] + "RequestAnimationFrame"]; C || (M = function (a) {
            var e = A(), g = Math.max(0, 16 - (e - G)); G = e + g; return m.setTimeout(function () {
                a(e +
                g)
            }, g)
        }, C = function (a) { clearTimeout(a) }); var T = "string" === H(document.createElement("div").textContent) ? "textContent" : "innerText"; D.extend = I; window.FPSMeter = D; D.defaults = { interval: 100, smoothing: 10, show: "fps", toggleOn: "click", decimals: 1, maxFps: 60, threshold: 100, position: "absolute", zIndex: 10, left: "5px", top: "5px", right: "auto", bottom: "auto", margin: "0 0 0 0", theme: "dark", heat: 0, graph: 0, history: 20 }; var X = ["toggleOn", "theme", "heat", "graph", "history"], Y = "position zIndex left top right bottom margin".split(" ")
    })(window); (function (m, j) {
        j.theme = {}; var s = j.theme.base = {
            heatmaps: [], container: { heatOn: null, heatmap: null, padding: "5px", minWidth: "95px", height: "30px", lineHeight: "30px", textAlign: "right", textShadow: "none" }, count: { heatOn: null, heatmap: null, position: "absolute", top: 0, right: 0, padding: "5px 10px", height: "30px", fontSize: "24px", fontFamily: "Consolas, Andale Mono, monospace", zIndex: 2 }, legend: {
                heatOn: null, heatmap: null, position: "absolute", top: 0, left: 0, padding: "5px 10px", height: "30px", fontSize: "12px", lineHeight: "32px", fontFamily: "sans-serif",
                textAlign: "left", zIndex: 2
            }, graph: { heatOn: null, heatmap: null, position: "relative", boxSizing: "padding-box", MozBoxSizing: "padding-box", height: "100%", zIndex: 1 }, column: { width: 4, spacing: 1, heatOn: null, heatmap: null }
        }; j.theme.dark = j.extend({}, s, { heatmaps: [{ saturation: 0.8, lightness: 0.8 }], container: { background: "#222", color: "#fff", border: "1px solid #1a1a1a", textShadow: "1px 1px 0 #222" }, count: { heatOn: "color" }, column: { background: "#3f3f3f" } }); j.theme.light = j.extend({}, s, {
            heatmaps: [{ saturation: 0.5, lightness: 0.5 }],
            container: { color: "#666", background: "#fff", textShadow: "1px 1px 0 rgba(255,255,255,.5), -1px -1px 0 rgba(255,255,255,.5)", boxShadow: "0 0 0 1px rgba(0,0,0,.1)" }, count: { heatOn: "color" }, column: { background: "#eaeaea" }
        }); j.theme.colorful = j.extend({}, s, { heatmaps: [{ saturation: 0.5, lightness: 0.6 }], container: { heatOn: "backgroundColor", background: "#888", color: "#fff", textShadow: "1px 1px 0 rgba(0,0,0,.2)", boxShadow: "0 0 0 1px rgba(0,0,0,.1)" }, column: { background: "#777", backgroundColor: "rgba(0,0,0,.2)" } }); j.theme.transparent =
        j.extend({}, s, { heatmaps: [{ saturation: 0.8, lightness: 0.5 }], container: { padding: 0, color: "#fff", textShadow: "1px 1px 0 rgba(0,0,0,.5)" }, count: { padding: "0 5px", height: "40px", lineHeight: "40px" }, legend: { padding: "0 5px", height: "40px", lineHeight: "42px" }, graph: { height: "40px" }, column: { width: 5, background: "#999", heatOn: "backgroundColor", opacity: 0.5 } })
    })(window, FPSMeter);
    */

}(window.ROE.Animation = window.ROE.Animation || {}));