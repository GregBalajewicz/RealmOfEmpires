$(function () {

    if ($('.chat .send').size() == 0) return;

    var pid = ROE.playerID

    var tab = $.cookie('chat.tab:' + pid);

    $('.chat .tabs ul li[tab=chat]').attr('stop', $.cookie('chat.tab.stop:' + pid + ':chat'));
    $('.chat .tabs ul li[tab=clan]').attr('stop', $.cookie('chat.tab.stop:' + pid + ':clan'));

    if (tab) { setab(tab); }

    var chatInterval = setInterval(function () {

        var td = $('.chat .tabs ul li.selected');
        if (td.attr('loading') == 'true') return;
        if (td.attr('stop') == 'true') return;

        var tab = td.attr('tab');

        if (tab == 'comm') return;
        if (tab == 'clan' && $('.chat .joinclan').size() > 0) { return; }

        var ts = $('.chat .list .' + tab + ' > div[time]:first').attr('time');
        if (!ts) ts = 'undefined';

        td.attr('loading', 'true');

        ajax("ChatUpdateAjax.aspx", { isclan: tab != "chat", timestamp: ts, count: (typeof admin_scriptLoadSize === 'number' ? admin_scriptLoadSize : 10) },
            function (arr) {
                $('.chat .error-update').hide();

                $.each(arr, function (i, obj) {
                    $('.chat .list .' + tab).prepend(item(obj));
                });

                td.attr('loading', 'false')

            }, function (w) {
                $('.chat .error-update').show().html(w.message);
            });

    }, 3000);

    $('.chat .tabs li').click(function () {
        var tab = $(this).attr('tab')
        setab(tab);
        $.cookie('chat.tab:' + pid, tab);
    });

    function item(obj) {
        var ispopup = $('.chat').attr('ispopup');

        var nandt = '<img class=avatar src=\'' + obj.avatarImgUrl + '\'></img> ' + (ispopup != 'True' ?
                ' <a playerid="' + obj.playerid +
                  '" onclick="return popupPlayerOverview(\'PlayerPopup.aspx?pid=' +
                  obj.playerid + '\',\'' + obj.player + '\')" href="#"> ' : '<B><span style="color: #B0B0B0 ">') +
            obj.player + (ispopup != 'True' ? ' </a> ' : '</b></span>')
            + (obj.playerid == -1 ? '' : ' <span class=pinfo >lvl ' + obj.XPLevel + ' ' + obj.Title.toString() + ' (' + obj.time + ')</span>') + '<BR> '

        var d = $('<div class=\'' + (obj.ChatToClanInGlobal ? 'c1 toclan' : 'c1') + '\' ' + (obj.playerid == -1 ? '' : ' time="' + obj.timestamp + '"') + '>' +
            (obj.playerid == -1 ? '' : nandt) + (obj.ChatToClanInGlobal ? '<span style="color: grey">[<img class="clan" src="https://static.realmofempires.com/images/NewForum.png" /> to clan]</span>' : '') + obj.message +
          '</div>');


        return d;
    }

    function setab(tab) {
        $('.chat .tabs ul li').removeClass('selected')
        $('.chat .tabs ul li[tab=' + tab + ']').addClass('selected');
        $('.chat .list .panel').hide();
        var p = $('.chat .list .' + tab).show();

        var stop_val = $('.chat .tabs ul li.selected').attr('stop');

        $('.stop a', p).html((stop_val == 'true' ? 'Enable' : 'Disable') + '');

        var s = (tab == 'comm' || (tab == 'clan' && $('.chat .joinclan').size() > 0)) ? 'hide' : 'show';

        $('.stop, .older, .popup', p)[s]();
        $('.chat .command')[s]();

        $('.chat .empty')[s == 'hide' ? 'show' : 'hide']();
        $('.chat .empty .map').attr('href', $('.header .map').attr('href'));
    }

    $('.chat .older').click(function () {
        var tab = $('.chat .tabs ul li.selected').attr('tab');
        var ts = $('.chat .list .' + tab + ' > div[time]:last').attr('time');
        if (!ts) ts = 'undefined';

        ajax("ChatUpdateAjax.aspx", { isclan: tab != "chat", timestamp: ts, count: (typeof admin_scriptLoadSize === 'number' ? admin_scriptLoadSize : 10), older: true },
            function (arr) {
                $('.chat .error-update').hide();

                $.each(arr, function (i, obj) {
                    $('.chat .list .' + tab + ' > div[time]:last').after(item(obj));
                });

            }, function (w) {
                $('.chat .error-update').show().html(w.message);
            });
    });

    $('.chat .stop').click(function () {
        var sel = $('.chat .tabs ul li.selected');
        var tab = sel.attr('tab');
        var p = $('.chat .list .' + tab);

        sel.attr('stop', sel.attr('stop') == 'true' ? 'false' : 'true');
        $('.stop a', p).html((sel.attr('stop') == 'true' ? 'Enable' : 'Disable'));

        $.cookie('chat.tab.stop:' + pid + ':' + tab, sel.attr('stop'));
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
                            $('.chat .error-add').show().html(r);
                        } else {
                            $('.chat .error-add').hide();
                        }
                    }, function (w) {
                        $('.chat .error-add').show().html(w.message);
                    });

        $('.chat .text').val('');
    }


    //
    // handle some bbcodes
    //
    $('.chat').delegate('.bbcode_p', 'click', function () {
        if ($('.chatInPopup').length > 0) {
            return window.opener.popupPlayerOverview('playerpopup.aspx?pid=' + $(event.target).attr('data-pid'), $(event.target).attr('data-pid'));
        } else {
            return popupPlayerOverview('playerpopup.aspx?pid=' + $(event.target).attr('data-pid'), $(event.target).attr('data-pid'));
        }
    });
    $('.chat').delegate('.bbcode_v', 'click', function () {
        var vid = parseInt($(event.target).attr('data-vid'), 10);
        if ($('.chatInPopup').length > 0) {
            return !window.opener.popupModalIFrame('VillageOverviewOtherPopup.aspx?ovid=' + vid, vid + Math.floor(Math.random() * 1000), '', 500, 680, 20, 40);
        } else {
            return !popupModalIFrame('VillageOverviewOtherPopup.aspx?ovid=' + vid, vid + Math.floor(Math.random() * 1000), '', 500, 680, 20, 40);
        }
    });
    $('.chat').delegate('.bbcode_c', 'click', function () {
        if ($('.chatInPopup').length > 0) {
            window.opener.location.href = 'clanpublicprofile.aspx?clanid=' + $(event.target).attr('data-cid');
        } else {
            window.location.href = 'clanpublicprofile.aspx?clanid=' + $(event.target).attr('data-cid');
        }
    });
    // forum post link
    $('.chat').delegate('.fp', 'click', function () {
        if ($('.chatInPopup').length > 0) {
            window.opener.location.href = 'ShowThread.aspx?ID=' + $(event.target).attr('data-fpid');
        } else {
            window.location.href = 'ShowThread.aspx?ID=' + $(event.target).attr('data-fpid');
        }
    });



});