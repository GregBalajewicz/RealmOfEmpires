
(function (ROE) {
}(window.ROE = window.ROE || {}));

(function (obj, $, undefined) {

    //init
    $(document).ready(function () {
        $('.audioToolsWrapper .toggleButton').click(function () {
            $('.audioToolsWrapper').toggleClass('min max');
        });
        $('.audioToolsWrapper .run').click(function () {
            var code = $('.codearea').val();
            eval(code);
        });
    });

    var _collection = {};

    var _create = function _create(settings) {
        //settings = {}
        //name: string - id for later access and manipulation
        //url: string - location of soundfile
        //loop: bool - to loop or not (optional, defaults false)    
        //volume: int - volume (optional, defaults 1)
        //endfunc: function - what to do after sound reaches end (optional, defaults null)


        //check name
        if (!settings.name && typeof (settings.name) != "string") {
            alert('failed to make new sound: needs a name string');
            return;
        } 

        //check  url
        if (!settings.url && typeof (settings.url) != "string") {
            alert('failed to make new sound: needs a url string');
            return;
        }

        var name = settings.name;
        var volume = settings.volume || 1;
        var endfunc = settings.endfunc;

        //set url (note: automatically starts preloading)
        var newAudio = new Audio(settings.url);

        //set loop (note: looped sounds dont do their end function)
        newAudio.loop = settings.loop === true || false;

        //set volume
        newAudio.volume = settings.volume || 1;

        //set on ending function
        if (settings.endfunc) {
            $(newAudio).on('ended', settings.endfunc);
        }
                
        //newAudio.play();
        _collection[settings.name] = newAudio;

    }

    var _exists = function _exists(name){
        if (!_collection[name]) {
            alert('Sound ' + name + ' not found. Was it created?');
            return false;
        } else {
            return true;
        }        
    }

    var _play = function _play(name) {
        if (!_exists(name)) { return }
        _collection[name].play();
    }

    var _pause = function _pause(name) {
        if (!_exists(name)) { return }
        _collection[name].pause();
    }

    var _stop = function _stop(name) {
        if (!_exists(name)) { return }
        _collection[name].pause();
        _collection[name].currentTime = 0;
    }

    var _volume = function _volume(name,amount) {
        if (!_exists(name)) { return }
        _collection[name].volume = amount;
    }

    var _time = function _time(name, seconds) {
        if (!_exists(name)) { return }
        _collection[name].currentTime = seconds;
    }

    var _fade = function _fade(name, amount, time) {
        if (!_exists(name)) { return }
        $(_collection[name]).animate({ volume: amount }, time, function () {
            /* should a fade to 0 stop the sound?
            if(amount == 0){ 
                _stop(name);
                _volume(name, amount);
            }
            */
        });
    }

    var _crossfade = function _crossfade() {

    }


    obj.create = _create;
    obj.play = _play;
    obj.pause = _pause;
    obj.stop = _stop;
    obj.volume = _volume;
    obj.time = _time;
    obj.fade = _fade;




    /*

    ROE.Audio.create({
        name: '1',
        url: 'https://static.realmofempires.com/sfx/sfx_ui_action_click.ogg'
    });
    ROE.Audio.create({
        name: '2',
        url: 'https://static.realmofempires.com/sfx/sfx_event_defeat.ogg'
    });
    ROE.Audio.create({
        name: '3',
        url: 'https://static.realmofempires.com/sfx/sfx_event_levelup.ogg'
    });
    ROE.Audio.create({
        name: '4',
        url: 'https://static.realmofempires.com/sfx/sfx_event_reward.ogg'
    });
    ROE.Audio.create({
        name: '5',
        url: 'https://static.realmofempires.com/sfx/sfx_event_spell.ogg'
    });
    ROE.Audio.create({
        name: '6',
        url: 'https://static.realmofempires.com/sfx/sfx_event_victory.ogg'
    });
    ROE.Audio.create({
        name: '7',
        url: 'https://static.realmofempires.com/sfx/sfx_menu_exit.ogg'
    });
    ROE.Audio.create({
        name: '8',
        url: 'https://static.realmofempires.com/sfx/sfx_building_enter.ogg'
    });
    ROE.Audio.create({
        name: '9',
        url: 'https://static.realmofempires.com/sfx/sfx_ui_action_swipe.ogg'
    });
    ROE.Audio.create({
        name: '10',
        url: 'https://static.realmofempires.com/sfx/sfx_ui_action_scroll.ogg'
    });
    ROE.Audio.create({
        name: '11',
        url: 'https://static.realmofempires.com/sfx/sfx_ui_action_open.ogg'
    });
    ROE.Audio.create({
        name: 'music1',
        url: 'https://static.realmofempires.com/sfx/m_01A_underscore_loop.ogg'
    });
    */

    ROE.Audio.event = function _event(eventName) {
        console.log('ROE.Audio.event not overwritten yet.');
    }

    /*

    ROE.Audio.event = function _event(eventName) {
        eventName = eventName.toString();
        console.log('eventName', eventName);

        switch (eventName) {

            case "playMusic1": //PLAY MUSIC1
                ROE.Audio.play('music1');
                break;

            case "stopMusic1": //STOP MUSIC1
                ROE.Audio.stop('music1');
                break;
            
            case "1": //CLICK
                ROE.Audio.play(eventName);
                break;

            case "7": //CLOSE
                ROE.Audio.play(eventName);
                break;

            case "8": //ENTER BUILDING
                ROE.Audio.play(eventName);
                break;

            case "5": //CAST SPELL
                ROE.Audio.play(eventName);
                break;

            case "11": //OPEN REPORT
                ROE.Audio.play(eventName);
                break;

            case "3": //LEVEL UP
                ROE.Audio.play(eventName);
                break;

            case "2": //REPORT DEFEAT
                ROE.Audio.play(eventName);
                break;

            case "6": //REPORT VICTORY
                ROE.Audio.play(eventName);
                break;

        }

    }


    */



    /* sample of code sound dude would write

    //creation examples
    ROE.Audio.create({
        name: 'metal',
        url: 'http://rpg.hamsterrepublic.com/wiki-images/7/72/Metal_Hit.ogg'
    });

    ROE.Audio.create({
        name: 'music1',
        url: 'http://www.vorbis.com/music/Epoq-Lepidoptera.ogg',
        volume: .2,
        loop: true
    });

    ROE.Audio.create({
        name: 'music2',
        url: 'http://www.vorbis.com/music/Hydrate-Kenny_Beltrey.ogg',
        volume: .4,
        endfunc: function () {
            console.log('music2 ended yo');
        }
    });

    //direct command examples

    ROE.Audio.volume('music1', 1);
    ROE.Audio.time('music1', 60);
    ROE.Audio.pause('music1');
    ROE.Audio.stop('music1');

     */
    /* end of sample */




    


}(window.ROE.Audio = window.ROE.Audio || {}, jQuery));