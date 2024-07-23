/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.6.4-vsdoc.js" />
/// <reference path="interfaces.js" />
/// <reference path="bda-val.js" />

(function (obj, $, undefined) {

} (window.BDA.UI = window.BDA.UI || {}, jQuery));

(function (obj, $, undefined) {

    var _init = function (canvas) 
    {
        var stage = {};
        stage.sprites = [];

        stage.canvas = canvas;
        stage.context = canvas.getContext('2d');

        stage.addAnimation = function (sprite) 
        {
            sprite.currentFrameIndx = 0;
            sprite.tick = 0;
            stage.sprites.push(sprite);
        };

        stage.go = function () {        
            setInterval(
                function() 
                {
                    _showFrames(stage.context, stage.sprites);
                }
                , 1000 / 8);
        }


        return stage;   

    }

    var _showFrames = function (context, sprites) 
    {
        var sprite
        for (var i = 0, sprite; sprite = sprites[i]; ++i) 
        {
            _showFrame(context, sprite);
        }
    }
    var _showFrame = function (context, sprite) 
    {           
        context.clearRect(sprite.Frame.x, sprite.Frame.y, sprite.Frame.size, sprite.Frame.size);

        context.drawImage(sprite.image
            , sprite.currentFrameIndx * sprite.Frame.size, 0, sprite.Frame.size, sprite.Frame.size
            , sprite.Frame.x, sprite.Frame.y, sprite.Frame.size, sprite.Frame.size);

        sprite.currentFrameIndx++;

        //if our index is higher than our total number of frames, we're at the end and better start over
        if (sprite.currentFrameIndx + 1 >= sprite.Frame.count) {
            sprite.currentFrameIndx = 0;
        }
    }

    obj.init = _init; 

} (window.BDA.UI.Anim = window.BDA.UI.Anim || {}, jQuery));
