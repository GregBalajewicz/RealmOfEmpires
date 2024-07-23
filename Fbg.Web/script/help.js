
DumperMaxDepth = 4;
var helpTimers = new Array();
var helpDB = null;
var helpDBLoading = 0; // intrmented to track AJAX querys.

// Delays: 1000 = 1 second
appearDelay = 400;
disappearDelay = 1000; 


function initTutorial ()
{
    // make the tutorial (if it's open) dragable
    if ($('.Tutorial').length > 0) {
        makeTutDrag();
        if($.cookie('tutPos')) {
            p = $.cookie('tutPos').split(/,/);
            $('.Tutorial').css( {  position : 'absolute', top : p[1] + 'px', left : p[0] + 'px' } );
        }
        if ($('.Tutorial').attr('rel')) {
            $( $('.Tutorial').attr('rel').split(/,/) ).each( function () { id = this.toString(); $('.tut').filter( function () { return $(this).attr('rel') == id ? true : false; } ).addClass('TutorialHighlight'); } );
        }
        
        // invoke the loop for highlighting the tutorial elements
        $('.TutorialHighlight').css({ borderColor : 'rgb(255,175,175)', borderStyle : 'solid', borderWidth : '3px' });
        pulseHelp();
    }
}

var pulse = { current : 100, down : true, low : 0, high : 255, dif : 25, delay : 100 };
function pulseHelp ()
{
    if ($('.TutorialHighlight').length == 0)
        return false;
    
    // set a timeout callback
    setTimeout("pulseHelp();", pulse.delay);
    
    // work out what direction we're moving in..
    if (pulse.down) 
        pulse.current = pulse.current - pulse.dif;
    else
        pulse.current = pulse.current + pulse.dif;
        
    // if we reach the limit (low OR high) flip the direction
    if (pulse.current <= pulse.low || pulse.current >= pulse.high)
        pulse.down = !pulse.down
        
    // set the colour
    $('.TutorialHighlight').css('borderColor', 'rgb(' + pulse.current +', 255,55 )');
}

function makeTutDrag ()
{
    if ($('#tutDrag').length == 0) {
        $('.TutorialHeader:first').attr('id', 'tutDrag');
    } 
    
    if($('.Tutorial').data('ui-draggable')) {
        $('.Tutorial').draggable("destroy");
    }
    
    $('.Tutorial').draggable({
        opacity : 0.7,
        containment : 'document', 
        zIndex : 50, 
        handle : '#tutDrag', 
        onStop: function () {
            p = $('.Tutorial').position();
            var date = new Date();
            date.setTime(date.getTime() + (1000 * 60 * 30));
            $.cookie('tutPos', p.left + ',' + p.top, { path: '/', expires: date });
        }
    });
}



function initHelpLoadDB ()
{
    // Load the help DB
    if ( $('link[rel=help]').length > 0) {
        helpDBLoading = $('link[rel=help]').length;
        dump('Found ' + helpDBLoading + ' help databases');
        $('link[rel=help]').each(
            function () {
                url = $(this).attr('href');
                //$('body').append('<div>Loading: ' + url + '</div>');
                $.ajax(
                    {
                        'type': 'GET',
                        'url': url,
                        'dataType': 'json',
                        'success': function (msg) {
                            //DumperAlert(['HelpDB request: ', msg]);
                            if (helpDB == null)
                                helpDB = new Array();

                            $(msg.help).each(
                                            function () { helpDB.push(this); }
                                        );
                            //dump(Dumper(helpDB));
                            helpDBLoading--;
                        },
                        'error': function (r, m, e) {
                            if (r.status == 404) {
                                m = "Failed to load help database - 404 File not found";
                            } else {
                                alert("help loading help db");
                                //DumperAlert(['Loading help db', m, { 'exeception' : e, 'status' : [r.status, r.statusText], 'helpdb' : helpDB} ]);
                            }
                            jsErrorHelp(0, r.status, m);
                            helpCloseBalloon();

                            helpDBLoading--;
                        },
                        'beforeSend': function (r) { },
                        'async': false
                    }
                );
            }
        );
    } else {
        // alert('couldn\'t load help');
    }
}


function initHelp () 
{
    // ju for testing.
    // $('body').append('<div id="testArea"><span class="helpTopic">This is the item we want</span><a href="/" class="jHelpOnClick" rel="jOverdue" topic="#somenode">Test</a>');

    $('.help, .jHelpOnClick').each(
        function (i)
        {
            if (!$(this).attr('id'))
                $(this).attr('id', 'helpArea' + i);
        }
    );
    $('.help.auto').each(
        function ()
        {
            // Our unique ID, we hope - Turns out this won't be unique
            id = $(this).attr('rel');
            
            // Out UNIQUE ID
            uid = $(this).attr('id');
            
            dump('Hovered over: ' + id);
            
            // find our help
            help = helpFind(id);
            if (!help)
                return;
                
            //dump('found help: ' + Dumper(help));
            
            if (help.text) {
                // Check if we already have a [?] of our very own..
                if($('.helpQuestion').filter('[id=helpQ' + id + ']').length > 0)
                {
                    // we do! - Better pretend we were'nt trying to get rid of it..
                    helpCancelTimers(uid);
                } else {
                    helpCancelTimers(uid);
                    // we do not.. Lets make one!
                    helpTimers.push( { 'id' : uid, 'timer' : setTimeout("helpNewQuestion('" + uid + "', " + $(this).hasClass('perm') + ");", appearDelay), 'text' : $(this).text() } );
                }
                // cancel timers for any children help [?]s
                /*$(this).children().find('.help').each(
                    function ()
                    {
                        //$(this).remove();
                        
                    }
                );*/
            }
            if (help.shorttext) {
                //$('.helpShortText').text(help.shorttext);
                $('.helpShortText').empty().append(help.shorttext);
            }
        }
    );
    $('.help').not('.auto').hover(
        function (e) {
            // Our unique ID, we hope - Turns out this won't be unique
            id = $(this).attr('rel');
            
            // Out UNIQUE ID
            uid = $(this).attr('id');
            
            dump('Hovered over: ' + id);
            
            // find our help
            help = helpFind(id);
            if (!help)
                return;
                
            //dump('found help: ' + Dumper(help));
            
            if (help.text) {
                // Check if we already have a [?] of our very own..
                if($('.helpQuestion').filter('[id=helpQ' + id + ']').length > 0)
                {
                    // we do! - Better pretend we were'nt trying to get rid of it..
                    helpCancelTimers(uid);
                } else {
                    helpCancelTimers(uid);
                    // we do not.. Lets make one!
                    helpTimers.push( { 'id' : uid, 'timer' : setTimeout("helpNewQuestion('" + uid + "', " + $(this).hasClass('perm') + ");", appearDelay), 'text' : $(this).text() } );
                }
                // cancel timers for any children help [?]s
                /*$(this).children().find('.help').each(
                    function ()
                    {
                        //$(this).remove();
                        
                    }
                );*/
            }
            if (help.shorttext) {
                //$('.helpShortText').text(help.shorttext);
                $('.helpShortText').empty().append(help.shorttext);
            }
        },
        function () {
            // Our unique ID, we hope - Turns out this won't be unique
            id = $(this).attr('rel');
            
            // Out UNIQUE ID
            uid = $(this).attr('id');
            
            // Blank the short tooltip
            $('.helpShortText').empty().append('<br />');
            
            // there might be a timer ready to add a [?]..
            helpCancelTimers(uid);
        
            // mouse has moved away, we set a timer to remove the [?], if there is one. Unless it's a "p
            if (!$(this).hasClass('perm')) {
                myTimer = setTimeout("$('#helpQ" + uid + "').remove();", disappearDelay);
            }
            
            helpTimers.push( { 'id' : uid, 'timer' : myTimer } );
        }
    );
    $('.jHelpOnClick')
        .click( 
            function () 
            {
                id = $(this).attr('id');
                $(this).attr('help',"other");
                helpBalloon(id, true);
                return false;
            });
}
function helpNewQuestion (uid, stay)
{
    dump('adding [?] for: ' + uid);
    
    position = $('#' + uid).position();
    
    if ($('#' + uid).parents('.test').length > 0)
    {
        /*p = $('.test').position();
        position.top += p.top;
        position.left += p.left;*/
        // This will diable [?] for area's in the right hand div.test colum.
        return false;
    }
    
    // create the [?]
    if ($('#helpQ' + uid).length == 0)
        // Old code, ie [?]: 
        //$('body').append('<a id="helpQ' + uid + '" class="helpQuestion" onmouseover="Highlight(\'' + uid + '\'); helpCancelTimers(\'' + uid + '\'); " onmouseout="unHighlight(); $(\'#helpQ' + uid + '\').remove();" onclick="helpBalloon(\'' + uid + '\');" for="' + uid + '">[?]</a>');
        if (stay)
            $('body').append('<a id="helpQ' + uid + '" class="helpQuestion" onmouseover="Highlight(\'' + uid + '\'); helpCancelTimers(\'' + uid + '\'); " onmouseout="unHighlight();" onclick="helpBalloon(\'' + uid + '\');" for="' + uid + '"></a>');
        else
            $('body').append('<a id="helpQ' + uid + '" class="helpQuestion" onmouseover="Highlight(\'' + uid + '\'); helpCancelTimers(\'' + uid + '\'); " onmouseout="unHighlight(); $(\'#helpQ' + uid + '\').remove();" onclick="helpBalloon(\'' + uid + '\');" for="' + uid + '"></a>');
        
    width = $('#' + uid ).width();
    height = $('#' + uid).height();
    
    Qwidth = $('#helpQ' + id).width();
    Qheight = $('#helpQ' + id).height(); 
    
    // position the "?" - if it falls outside of the screen width wise, position it more to the left, otherwise position it to the right of the help item 
    var winWidth = $(window).width();
    if (winWidth >= (position.left + width) + 18) {
        $('#helpQ' + uid).css( {  'top'	: (position.top - Qheight) + 'px',
                                 'left'	: ((position.left + width) - Qwidth) + 'px'
                             });  
    } else {
        $('#helpQ' + uid).css( {  'top'	: (position.top - Qheight) + 'px',
                                 'left'	: ((position.left + width) - 18) + 'px'
                             });  
    }

}
function helpBalloon (uid, dif)
{
    id = $('#' + uid).attr('rel');
    
    if ( $('.helpBalloon').length > 0) {
        /* Dissabling the pin/upin functionality 
        if ( $('.helpBalloon').filter('.pinned').length > 0) {
            // We already have a 'pinned' help balloon open..
        } else {
            // We have a balloon open, but it isn't pinned.
            helpBalloonMove(uid, true);
        }*/
        if (dif) {
            //alert("other");
            $('#' + uid).parent().find(".helpTopic").attr('id', uid  + "_topic");
            $('.helpBalloon').attr('helpItem', uid  + "_topic");
        } else {
            $('.helpBalloon').attr('helpItem', uid);
        }
        helpBalloonMove(uid, true);
    } else {
        // No help balloon.
        $('body').append('<div class="helpBalloon" helpItem="' + uid + '" onmouseout="unHighlight();" onmouseover="HighlightSelected();"></div>');
        helpBalloonMove(uid, true);
    }
    helpBalloonUpdate(id);
}

function helpLight (id)
{
    //disabled for now!
    return false;
    
//    $('.help').each(
//        function () 
//        {
//            if ($(this).attr('rel') != $('.helpBalloon').attr('help'))
//                if ($(this).attr('rel') == id)
//                    $(this).addClass('yellow');
//                else
//                    $(this).removeClass('yellow');
//        }
//    );
}

function helpDragable ()
{
    $('.helpBalloon').draggable(
    {
        opacity : 0.7,
        containment : 'document',
        //handle      : '#helpLinkMove',
        handle        : '#helpBalloonTitleMove',
        zIndex  : 50,
        onStop : function () 
        {
            // Work around for IE!
            $('.helpBalloon').draggable("destroy");
            helpDragable() ;
        }
    });
}
                                                                                                                                                                                                            
function helpBalloonUpdate (id)
{
    if ( $('.helpBalloon').children().length == 0)
    {
        // if .helpBalloon has zero children it means this is a fresh helpBalloon.
        $('.helpBalloon').append('<div class="helpBalloonTitle"></div>');
        $('.helpBalloonTitle').append('<div id="helpBalloonTitleMove"></div>');
        helpDragable(); // hack for IE!
        //$('.helpBalloonTitle').append('<a onclick="helpTogglePin();">Pin/Unpin</a> ');
        $('.helpBalloonTitle').append('<a onclick="helpCloseBalloon();" id="helpLinkClose">Close</a>');
        $('.helpBalloonTitle').append('<br /><span></span>');
        $('.helpBalloon').append('<div class="helpBalloonBody"></div>');
    }
    if (helpDB != null) {
        // we have the help database loaded!
        $('.helpBalloonBody').empty();
        
        help = helpFind(id);
        $('.helpBalloon').attr('help', id);
        
        
        
        if (help.text)
                if (help.title)
            $('.helpBalloonBody').append('<div><strong>' + help.title + '</strong></div><p>' + help.text + '</p>');
            else
                $('.helpBalloonBody').append('<p>' + help.text + '</p>');
        else
            $('.helpBalloonBody').text('No help found for this item');
        
    } else {
        $('.helpBalloonBody').text('Loading help database');
    }
}
function helpFind (itemID)
{
    var help = null;
    $(helpDB).each(
        function () {
            if (this.id == itemID) {
                help = this;
            }
        }
    );
    return help;
}

function helpCloseBalloon ()
{
    $('.helpBalloon').fadeOut('fast', function () { $(this).remove(); unHighlight(); } );
}
function helpTogglePin ()
{
    $('.helpBalloon').toggleClass('pinned');
}
function helpBalloonMove (where, effect)
{
    position = $('#' + where).position();
    width = $('#' + where).width();
    
    x = (position.left + width + 10)
    if ((x + $('.helpBalloon').width()) > $(document).width())
        x = $(document).width() - $('.helpBalloon').width() -5 ;

    $('.helpBalloon').css({ 'top': position.top + 'px', 'left': x + 'px' });
    $('.helpBalloon').fadeIn();
}
function helpCancelTimers (id)
{
    $(helpTimers).each( function () { if (this.id == id) { clearTimeout(this.timer); } } );
}
//function helpEvalQuestion ()
//{
//    if (helpShowQuestion > 0) {
//        
//    }
//}
function helpShowQuestionYX (e)
{
    helpShowQuestionX = e.pageX;
    helpShowQuestionY = e.pageY;
}
function dump (text)
{
    return;
//    if ($('#debug').length == 0)
//        $('body').append('<div id="debug"></div>');
//        
//    d = new Date();
//    $('#debug').prepend('<div>' + d.toUTCString() + '<pre>' + text + '</pre></div>');
}

function jsErrorHelp (area, code, message)
{
    alert('error message:' + message + "(" + area + ":" + code + ")");
}

function Highlight(uid)
{
    if ($('#highlight').length == 0)
        $('body').append('<div id="highlight"></div>');
    
    $('#highlight').hide();
    
    $('#' + uid).addClass('highlightElement');
    
    p = $('#' + uid).position();
    w = $('#' + uid).width();
    h = $('#' + uid).height();
    
    //alert('need to find: ' + uid);
    $('#highlight').css( { top : (p.top - 2) + 'px', left : (p.left - 2) + 'px', width : (w + 4) + 'px', height : (h + 4) + 'px' } );
    $('#highlight').show();
}

function HighlightSelected ()
{
    Highlight( $('.helpBalloon').attr('helpItem') );
}

function unHighlight()
{
    $('#highlight').remove();
    $('.highlightElement').removeClass('highlightElement');
}
