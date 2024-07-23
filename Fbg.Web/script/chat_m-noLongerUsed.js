/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.6.4.js" />
/// <reference path="interfaces.js" />
/// <reference path="BDA.js" />
/// <reference path="BDA-exception.js" />
/// <reference path="BDA-utils.js" />
/// <reference path="ROE_c.js" />
/// <reference path="roe-frame_m.js" />
/// <reference path="troops.3.js" />
/// <reference path="BDA-console.js" />
/* temp -  turn on chat code
$(function () {

    var MAXCHATSINMINI = 20;
    var pid = ROE.playerID;
    var inMiniMode = true; // tells u if the chat is displayed in the minimized mode

    var template_chatRow = '<div class="c1 %toClan%" time="%time%"><img class="avatar sfx2" src="%avatarUrl%"><span class="pn"><b>%playerName%</b></span><span class="pinfo"> - lvl %level% %title% (%timeForDisplay%)</span><span class=msg><img class="clan" src="https://static.realmofempires.com/images/NewForum.png" />%message%</span></div>';
    var template_warnRow = '<div class="c1 warn">%message%</div>'

    var initialTab;
    if (ROE.Player.Clan) {
        initialTab = "clan";
    } else {
        initialTab = "chat";
    }

    var tab = initialTab;


    $('.chat .tabs ul li[tab=chat]').attr('stop', $.cookie('chat.tab.stop:' + pid + ':chat'));
    $('.chat .tabs ul li[tab=clan]').attr('stop', $.cookie('chat.tab.stop:' + pid + ':clan'));


    //var chatInterval = setInterval(function () { alert("fsdsd"); }, 3000);

    var td;
    var _checkNewChats = function () {
       
        td = $('.chat .tabs ul li.selected');
        if (td.attr('loading') == 'true') return;
        if (td.attr('stop') == 'true') return;


        tab = td.attr('tab');
        inMiniMode = td.parents('.chatBox.mini').length > 0;

        if (tab == 'comm') return;
        if (tab == 'clan' && $('.chat .joinclan').size() > 0) { return; }

        var ts = $('.chat .list .' + tab + ' > div[time]:first').attr('time');
        if (!ts) ts = 'undefined';

        td.attr('loading', 'true');
        
        ajax("ChatUpdateAjax.aspx"
            , { isclan: tab != "chat", timestamp: ts, count: (typeof admin_scriptLoadSize === 'number' ? admin_scriptLoadSize : 20), returnAvatarID: true },
            _displayNewChats, _displayError);
    };

    var _displayNewChats = function (arr) {
        $('.chat .error-update').hide();

        var chatListPanel = $('.chat-container .list .' + tab + '.panel');

        // Check to see if we received an array of chat objects OR
        // something else (e.g. a string like "Not in clan")
        if (arr instanceof Array) {
            $.each(arr, function (i, obj) {
                chatListPanel.prepend(item(obj));
            });
        } else {
            // Did not receive an array, do not prepend chats.
        }
       
        //
        // delete old chat if we got a lot of them, in minimized mode only
        if (inMiniMode) {
            var chats = chatListPanel.find('.c1');
            if (chats.length > MAXCHATSINMINI) {
                var removeThese = chats.slice(MAXCHATSINMINI);
                removeThese.remove();
            }
        }

        td.attr('loading', 'false')
    };


    var _displayError = function (w) {
        $('.chat .error-update').show().html(w.message);
    };


    var chatInterval = setInterval(_checkNewChats, 3000);


    $('.chat .tabs li').click(function () {
        ROE.UI.Sounds.click();
        var tab = $(this).attr('tab')
        setab(tab);
        $.cookie('chat.tab:' + pid, tab);
    });

    function item(obj) {
      
        var d;
        if (obj.playerid == -1) {
            // warn message only 
            d = BDA.Templates.populate(template_warnRow
            , {
                message: obj.message
            });
        } else {
            
            //
            // regular chat message
            d = BDA.Templates.populate(template_chatRow
            , {
                playerName: obj.player
                , toClan: obj.ChatToClanInGlobal ? "toclan" : ""
                , message: obj.message
                , time: obj.timestamp
                , avatarUrl: ROE.Avatars[obj.avatarID].img
                , timeForDisplay: obj.time
                , level: obj.XPLevel
                , title: obj.Title
            });
        }

        d = $(d);

        //
        // remove the clan flag if not needed
        //
        if (!obj.ChatToClanInGlobal) {
            d.find('img.clan').remove();
        }
                
        //BDA.Console.verbose('chat', d);

        return d;

    };

    var _chatClicked = function (event) {

        if (!inMiniMode) {
            var pid = ROE.playerID;
            var chatBlock = $(event.currentTarget).parents(".c1");
            var playerNameToBlock = chatBlock.find('.pn').text();
            if (!playerNameToBlock) { return; }

            ROE.Frame.popupPlayerProfile(playerNameToBlock);
        }
    };

    function setab(tab) {
        $('.chat .tabs ul li').removeClass('selected')
        $('.chat .tabs ul li[tab=' + tab + ']').addClass('selected');
        $('.chat .list .panel').hide();
        var p = $('.chat .list .' + tab).show();

        $('.older', p).show();

        $('.chat .command input').prop('disabled', false);

        if (tab === "clan") {
            if (ROE.Player.Clan) {
                $('.older', p).show();
                $('.notInClan:visible', p).hide();
            } else {
                $('.chat .command input').prop('disabled', true);
                $('.older', p).hide();
                $('.notInClan', p).show();
            }
        }

    }

    $('.chat .older').click(function () {
        var tab = $('.chat .tabs ul li.selected').attr('tab');
        var ts = $('.chat .list .' + tab + ' > div[time]:last').attr('time');
        if (!ts) ts = 'undefined';

        ajax("ChatUpdateAjax.aspx", { isclan: tab != "chat", timestamp: ts, count: (typeof admin_scriptLoadSize === 'number' ? admin_scriptLoadSize : 10), older: true, returnAvatarID: true },
            function (arr) {
                $('.chat .error-update').hide();

                $.each(arr, function (i, obj) {
                    $('.chat .list .' + tab + ' > div[time]:last').after(item(obj));
                });

            }, function (w) {
                $('.chat .error-update').show().html(w.message);
            });
    });

    $('.chat .send').click(send);
    $('.chat .text').keypress(function (e) { if (e.which == '13') { e.preventDefault(); send(); } });

    function send() {
        var tab = $('.chat .tabs ul li.selected').attr('tab');
        var text = $('.chat .text').val();

        if ($.trim(text) == '') return;

        ajax("ChatMessageAddAjax.aspx",
                    { isclan: tab != "chat", message: text },
                    function (r) {
                        if (r != 'ok') {
                            var chatListPanel = $('.chat .list .' + tab);
                            chatListPanel.prepend($('<div class=error-add>').html(r));
                        }
                    }, function (w) {
                        var chatListPanel = $('.chat .list .' + tab);
                        chatListPanel.prepend($('<div class=error-add>').html(w.message));
                    });

        $('.chat .text').val('');
    }


    $('.chatBox .chat.panel').delegate('.avatar', 'click', _chatClicked);
    $('.chatBox .clan.panel').delegate('.avatar', 'click', _chatClicked);
    $('.chatBox .chat.panel').delegate('.pn', 'click', _chatClicked);
    $('.chatBox .clan.panel').delegate('.pn', 'click', _chatClicked);

    //
    // handle some bbcodes
    //
    $('.chatBox .chat.panel, .chatBox .clan.panel').delegate('.bbcode_p', 'click', function (event) {
        ROE.Frame.popupPlayerProfile($(event.target).html());
    });
    $('.chatBox .chat.panel, .chatBox .clan.panel').delegate('.bbcode_v', 'click', function (event) {      
        ROE.Frame.popupVillageProfile($(event.target).attr('data-vid'));
    });
    $('.chatBox .chat.panel, .chatBox .clan.panel').delegate('.bbcode_c', 'click', function (event) {
        ROE.Frame.popupClan($(event.target).attr('data-cid'));
    });
    $('.chatBox .chat.panel, .chatBox .clan.panel').delegate('.fp', 'click', function (event) {
        ROE.Frame.popupClan(undefined, $(event.target).attr('data-fpid'), "forumpost"); // launch clan popup with the forum already opened
    });


    if (initialTab) { setab(initialTab); }

});

*/