(function () { }(window.ROE = window.ROE || {}));
(function () { }(window.ROE.Map = window.ROE.Map || {}));

(function (obj) {

    obj.init = function (container) {
        var expcol = {
            expand: 'https://static.realmofempires.com/images/expand_button.gif',
            collapse: 'https://static.realmofempires.com/images/collapse_button.gif'
        };
        
        var ot = $('<ul></ul>');
        var tmp;

        for (var cli in ROE.Landmark.clans) {
            var clan = ROE.Landmark.clans[cli];
            
            var cname = '<a href="ClanPublicProfile.aspx?clanid=%id% target="_blank" class="clan" data-cid="%id%" onclick="ROE.UI.Sounds.click();">%CN%</a>'.format(clan);
            ot.append($('<li></li>').html(cname + ' <span style="color:#D0D5D9;">[' + clan.CP + ']</span>').append(players(clan.id).li));
        }

        // handle unclaned players
        tmp = players('');
        if (tmp.foundSome) {
            ot.append($('<li>Not in clan</li>').append(tmp.li));
        }

        // handled abandoned and rebels 
        var rab = $('<li>Rebels and Abandoned<ul><li class="rebels"><img class="collapse" src="' + expcol.expand + '" />Rebels</li><li class="abandoned"><img class="collapse" src="' + expcol.expand + '" />Abandoned</li></ul></li>');


        tmp = villages(ROE.CONST.specialPlayer_Rebel);
        if (tmp.foundSome) {
            rab.find('.rebels').append($('<li class="rebels"><img class="collapse" src="' + expcol.expand + '" />Abandoned</li>').append(tmp.li).find('ul').css('display', 'none'));
        } else {
            rab.find('.rebels').remove();
        }
        tmp = villages(ROE.CONST.specialPlayer_Abandoned);
        if (tmp.foundSome) {
            rab.find('.abandoned').append($('<li class="abandoned"><img class="collapse" src="' + expcol.expand + '" />Abandoned</li>').append(tmp.li).find('ul').css('display', 'none'));
        } else {
            rab.find('.abandoned').remove();
        }

        ot.append(rab);



        // clan num or 0
        function players(clan) {
            var li = $('<ul></ul>');
            var retval = { foundSome: false };
            for (var pli in ROE.Landmark.players) {
                var player = ROE.Landmark.players[pli];

                if (player.CID != clan || ROE.isSpecialPlayer(player.id)) continue;

                var pname = '<a href="#" onclick="ROE.UI.Sounds.click(); ROE.Frame.popupPlayerProfile(\'' + player.PN + '\');">' + player.PN + '</a>';
                var ppoint = '<span class="point">[' + player.PP + ']</span>';
                var padd = '<a pn="' + player.PN + '" class="torecip" type="add" href="#">+</a>';
                var pcol = '<img class="collapse" src="' + expcol.expand + '" />';

                var player_dom_data = [pcol, padd, pname, ppoint];
                if (player.Pe != 0) player_dom_data.push('<i>' + player.Pe + '</i>');

                var player_dom = $('<span>' + player_dom_data.join(' ') + '</span>');

                $('.torecip', player_dom).click(function (e) {

                    var s = $(this);
                    var ot = s.attr('type');
                    var nt = ot == 'remove' ? 'add' : 'remove';
                    s.attr('type', nt);
                    s.html(nt == 'remove' ? '-' : '+');

                    var m = $('.message', container);
                    var pl = m.data('players');
                    if (ot == 'add') {
                        pl.push(s.attr('pn'));
                    } else {
                        pl.splice($.inArray(s.attr('pn'), pl), 1);
                    }

                    var pl_names = m.data('players');

                    m.html(pl_names == '' ? '' : m.attr('template').replace('{0}', pl_names.join(', ')));
                    m.attr('href', 'messages_create.aspx?To=' + pl_names.join(','));
                    m.attr('to', pl_names.join(','));

                    e.preventDefault();
                });

                // used .css('display', 'none') cause in jquery 1.2.3 we have a chrome not working with hide while adding elements
                li.append($('<li></li>').append(player_dom).append(villages(player.id).li)).find('ul').css('display', 'none');
                retval.foundSome = true;
            }
            retval.li = li;
            return retval;
        }

        
        $("#popup_mapsummary").append('<img id=background src="https://static.realmofempires.com/images/backgrounds/M_LoginCastleNight.jpg" class="stretch" alt="" />');

        // player
        function villages(player) {
            var li = $('<ul></ul>');
            var retval = { foundSome: false };
            for (var vli in ROE.Landmark.villages_byid) {
                var village = ROE.Landmark.villages_byid[vli];

                if (village.pid != player) continue;

                var vtitle = village.name + ' (' + village.x + ',' + village.y + ')';
                var vname = '<a onclick="ROE.UI.Sounds.click(); ROE.Frame.popupVillageProfile(' + village.id + ');return false;" href="VillageOverviewOtherPopup.aspx?ovid=' + village.id + '">' + vtitle + '</a>';

                li.append($('<li></li>').html(vname + (village.note == 0 ? '' : '<i>' + village.note + '</i>')));
                retval.foundSome = true;
            }
            retval.li = li;
            return retval;
        }

        $('.collapse', ot).click(function (e) {
            var ul = $(this).parents('li:first').find('ul');

            if ($(this).attr('src') == expcol.expand) {
                $(this).attr('src', expcol.collapse); ul.show();
            } else {
                $(this).attr('src', expcol.expand); ul.hide();
            }
        });

        var message_link = '<a class="message" href="#" template="Click here to message {0}"></a>';

        container.empty()
            .append("<div>Here is a summary of all villages on the map. You can explore this list and also create a mailing list by clicking the +/- links next to player names</div>")
            .append($(message_link).data('players', []))
            .append(ot)
            .append($(message_link).data('players', []));

        $('.message', container).click(function (e) {
                e.preventDefault();
                ROE.Mail.sendmail($(this).attr('to'));
        });
        $('a.clan', container).click(function (e) {
            e.preventDefault();
            ROE.Frame.popupClan($(this).attr('data-cid'));
        });
    };

}(window.ROE.Map.Summary = window.ROE.Map.Summary || {}));