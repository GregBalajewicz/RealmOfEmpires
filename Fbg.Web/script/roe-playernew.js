(function (obj) {

    var _container = null;

    obj.init = function (container, name) {

        //var tmo;

        _container = container;
        _container.empty().append(BDA.Templates.getRawJQObj("PlayerNewPopup", ROE.realmID));

        ROE.Frame.busy('Loading profile...', 5000, _container);

        ROE.Api.call('player_other', { pname: name }, _playerprofile);

        //if your own profile
        if (name == ROE.Player.name) {
            //$('.sendMessage', _container).hide();
            $('.chatWith', _container).hide();
            $('.inviteClan', _container).hide();
            $('.avatorChanger', _container).show();
            $('.sideset .editButton', _container).show();
            $('.block', _container).hide();
            $('.mainset .title', _container).css('cursor', 'pointer').click(_togglePlayerSex);
            _addAvatarPicker();
        }

        // button clicks
        $(".profile .Edit", _container).click((function (e) { ROE.UI.Sounds.click(); _editProfile(e); }));
        $(".notes .Edit", _container).click((function (e) { ROE.UI.Sounds.click(); _editNotes(e); }));
        $('.chatWith', _container).click(function () { ROE.UI.Sounds.click(); _chatWith(); });
        $('.block', _container).click(function (e) { ROE.UI.Sounds.click(); e.stopPropagation(); _blockPlayer(); });

        $('.inviteClan', _container).click(function () {
            ROE.UI.Sounds.click();
            ROE.Api.call('player_other_invite_to_clan', { pname: name }, function (ret) {
                ROE.Frame.infoBar(ret.message);
            });
        });


        $(".mainset .infos", _container).click(function (e) {
            ROE.UI.Sounds.clickActionOpen();
            $(".sideset", _container).show();
            var type = $(e.currentTarget).attr("data-info");

            $(".sideset .info:not(." + type + ") ", _container).hide();
            $(".sideset .info." + type, _container).show();

            BDA.UI.Transition.slideLeft($(".sideset"), $(".mainset"), function () { $(".mainset", _container).hide(); });

            switch (type) {
                case "vills":
                    ROE.Api.call('player_other_villages', { pname: name }, _populate_villages);
                    break;
                case "profile":
                    $(".sideset .ProfileText", _container).html(playerProfile);
                    break;
                case "notes":
                    $(".sideset .NotesText", _container).html(playerNotes);
                    break;
                case "stats":
                    load('last');
                    break;
            }
        });


        $(".sideset .mainBack", _container).click(function () {
            ROE.Api.call('player_other', { pname: name }, _playerprofile);
            $(".mainset", _container).show();
            BDA.UI.Transition.slideRight($(".mainset"), $(".sideset"), function () { $(".sideset", _container).hide(); });
        });


        function _populate_villages(response) {

            var t = '<table><tr><td>' + _phrases(2) + '</td><td>' + _phrases(3) + '</td></tr>';

            for (var pi = 0; pi < response.length; pi++) {
                t += '<tr><td class="sfx2" ><a href="#" onclick="ROE.Frame.popupVillageProfile(' + response[pi].vid + '); return false;">' + response[pi].name + " (" + response[pi].x + ', ' + response[pi].y + ")" + '</a></td><td>' + response[pi].points + '</td></tr>';
            }

            t += '</table>';

            $(".sideset .villtable", _container).empty().append(t);

        }


        function _playerprofile(response) {

            var playerClanID = response.CID;
            playerID = response.id;
            playerProfile = response.profile;
            playerNotes = response.Pe;
            var govtype = response.gov;
            var playerAvatar = ROE.Avatar.list[response.Av || 1];

            $(".templateContent", _container).css("background-image", 'url(' + playerAvatar.imageUrlL + ')');

            if (playerClanID != 0) {

                if (ROE.Player.Clan && ROE.Player.Clan.id == playerClanID) {
                    $('.inviteClan', _container).hide();
                }
                if (!ROE.Player.Clan) {
                    $('.inviteClan', _container).hide();
                }
            }

            $('.mainset .title', _container).attr('data-sex',response.sex).html(response.title);
            $('.mainset .plname', _container).html(response.PN);
           // $('.mainset .level SPAN', _container).html(response.level);
            $('.mainset .rank SPAN:first-child', _container).html(response.rank);
            $('.mainset .villages SPAN:first-child', _container).html(BDA.Utils.formatShortNum(response.PP));

            var govImage = "https://static.realmofempires.com/images/illustrations/GovSm";
            if (govtype != "") {

                $(".govtype", _container).css("background", "url(" + govImage + govtype + ".png)");
                $(".govtype", _container).show();
            } else {
                $(".govtype", _container).hide();
            }


            if (playerProfile != "") {
                var ptext = playerProfile.split("<br />").join(" ");
            }
            else {
                var ptext = "<span class='lands' >" + _phrases(6) + "</span>";
            }

            $('.mainset .profile', _container).html(ptext);


            if (playerNotes != "") {
                var ntext = playerNotes.split("<br />").join(" ");
            }
            else {
                var ntext = "<span class='lands' >" + _phrases(7) + "</span>";
            }

            $('.mainset .notes', _container).html(ntext);

            $('#EditorNotes', _container).val(ROE.Utils.toTextarea(response.Pe));
            $('#EditorProfile', _container).val(ROE.Utils.toTextarea(response.profile));

            if (response.CID != 0) {

                $(".clanlink", _container).html(response.clan.CN);
                $(".claninfo", _container).css("visibility", "visible");
                $(".clanlink", _container).click(function () { ROE.UI.Sounds.click(); ROE.Frame.popupClan(response.CID); });

            }
            else {
                $(".claninfo", _container).css("visibility", "hidden");
            }


            if (ROE.isD2) {
                $(".viewThrone", _container).click(function () {
                    if (playerID == ROE.Player.id) {
                        window.open('throneroom.aspx', '_blank');
                    } else {
                        window.open('throneroom.aspx?rid=' + ROE.realmID + '&pid=' + playerID + '&viewerpid=' + ROE.Player.id, '_blank');
                    }
                });
            } else {
                $(".viewThrone", _container).remove();
            }


            ROE.Frame.free(_container);
        }

        //for player's own profile, add teh new avatar picker.
        function _addAvatarPicker() {
            var avatarPickerBtn = $('<div class="avatarPickerBtn helpTooltip" data-tooltipid="changeAvatar"></div>');
            avatarPickerBtn.click(function () {
                _avatarPopup();
            });
            $('.mainset',_container).append(avatarPickerBtn);
        }

        function _avatarPopup() {
            var title = "Pick your Avatar";
            var content = $('<div class="avatarPopup">');
            ROE.Avatar.init(content, ROE.Player.avatarID);
            ROE.Frame.popGeneric(title, content, 630, 590);

            var closeBtn = $('.ui-dialog[aria-describedby="genericDialog"]').find('.ui-dialog-titlebar-close');
            if (!closeBtn.hasClass('avatarClose')) {
                closeBtn.addClass('avatarClose').click(function () {
                    _setPlayerAvatarID(ROE.Avatar.lastPickedID);
                });
            }
        }

        function _setPlayerAvatarID(avatarID) {
            if (!avatarID) { return; }
            ROE.Frame.busy('Commissioning painting...', 5000, _container);
            ROE.Api.call_player_setavatarid(avatarID, _setPlayerAvatarIDCallback);
            ROE.Avatar.lastPickedID = null;
        }

        function _setPlayerAvatarIDCallback(data) {

            ROE.Frame.free(_container);

            if (data.success) {
                var avatarID = data.AvatarID;
                var avatar = ROE.Avatar.list[avatarID];
                
                $(".templateContent", _container).css("background-image", 'url(' + avatar.imageUrlL + ')');

                if (ROE.isD2) {
                    $('.player-avatar-picture').css('background-image', 'url(' + avatar.imageUrlS + ')');
                }

                ROE.Player.avatarID = avatarID;
                ROE.Avatar.lastPickedID = avatarID;

                //update map related avatar stuff
                if (ROE.Landmark.players[ROE.playerID]) {
                    ROE.Landmark.players[ROE.playerID]["Av"] = avatarID;
                    BDA.Database.LocalSet('MapPlayers', JSON.stringify(ROE.Landmark.players));
                }

                //because avatar change quest might get done
                ROE.Frame.refreshQuestCount();

            } else {
                //avatar pick denied
            }
        }

        //0female 1male
        function _togglePlayerSex() {
            ROE.Frame.busy('Updating Player Info...', 5000, _container);
            var curSex = parseInt($('.mainset .title', _container).attr('data-sex'));
            ROE.Api.call('player_setsex', { sex: curSex === 0 ? 1 : 0 }, _sexPlayerSexReturn);
        }

        function _sexPlayerSexReturn(data) {
            $('.mainset .title', _container).attr('data-sex', data.sex).html(data.title); //update title in player profile
            $(".player-ranking .player-title").html(data.title); //update title in D2
            ROE.Frame.infoBar(data.sex === 0 ? 'Player sex set to Female.' : 'Player sex set to Male.'); //send a notif
            ROE.Frame.free(_container);
        }
        

        function load(last) {
            ajax('PlayerStatHistoryAjax.aspx?pid=' + playerID + '&d=' + last, {}, function (obj) {

                var hpan = $('.hs.panel', _container);
                if (hpan.data('obj') == null || last == 'all') {
                    hpan.data('obj', obj);
                }
                var sel = $('.hs.panel a[tp].sel', _container);
                plot(sel.length == 0 ? 'villages' : sel.attr('tp'));

                $('.hs.panel a[time]', _container).removeClass('sel');
                $('.hs.panel a[time=' + last + ']', _container).addClass('sel');
            });
        }
        var plo;

        function plot(section) {

            var d = new Date();
            d.setDate(d.getDate() - 30);

            plo = $.plot($('.hs.panel .graph', _container),
                [$('.hs.panel', _container).data('obj')[section]],
                {
                    yaxis: {
                        tickDecimals: 0
                    },
                    xaxis: {
                        mode: "time",
                        min: d.getTime(),
                        max: (new Date()).getTime()
                    },
                    zoom: { interactive: true },
                    pan: { interactive: true }
                }
            );

            $('.hs.panel a[tp]', _container).removeClass('sel');
            $('.hs.panel a[tp=' + section + ']', _container).addClass('sel');
        }

        $('.hs.panel a[tp]', _container).click(function (e) {
            e.preventDefault();
            plot($(this).attr('tp'));
        });

        $('.hs.panel a[time]', _container).click(function (e) {
            e.preventDefault();
            load($(this).attr('time'));
        });



        function _editNotes(event) {

            event.stopPropagation();
            ROE.UI.Sounds.click();

            var saveit = $(".notes .Edit", _container).hasClass("SAVE");

            if (saveit) {
                ROE.Frame.busy('Saving notes...', 5000, _container);
                var text = $('#EditorNotes', _container).val();

                ROE.Api.call('player_other_note_save', { pname: name, text: text }, function (t) {

                    ROE.Frame.free(_container);

                    $(".notes .EditMessage", _container).hide();
                    $(".notes .NotesText", _container).html(t).show();
                    $(".notes .Edit", _container).text("Edit").removeClass("SAVE");

                    ROE.Landmark.refill(true);
                });
            }
            else {
                $(".notes .NotesText", _container).hide();
                $(".notes .EditMessage", _container).show();
                $(".notes .Edit", _container).text("Save").addClass("SAVE");
            }
        }


        function _editProfile(event) {

            event.stopPropagation();
            ROE.UI.Sounds.click();

            var saveit = $(".profile .Edit", _container).hasClass("SAVE");

            if (saveit) {
                ROE.Frame.busy('Saving profile...', 5000, _container);
                var text = $('#EditorProfile', _container).val();

                ROE.Api.call('player_profile_set', { pname: name, text: text }, function (t) {

                    ROE.Frame.free(_container);
                    $(".profile .EditMessage", _container).hide();
                    $(".profile .ProfileText", _container).html(t).show();
                    $(".profile .Edit", _container).text(_phrases(4)).removeClass("SAVE");
                });
            }
            else {
                $(".profile .ProfileText", _container).hide();
                $(".profile .EditMessage", _container).show();
                $(".profile .Edit", _container).text(_phrases(5)).addClass("SAVE");
            }
        }


        function _phrases(id) {
            return $('.phrases [ph=' + id + ']', _container).html();
        }


        function _blockPlayer() {

            var confirmtext = "Block <span class='chatAlertName' >" + name + "</span>? <DIV class='chatAlertText' >" + "You will not see any global chats from them, receive any mail, or forwarded reports. Player will be notifed that someone blocked them but not who did it. Player will be flagged for investigation. Abusing this function is prohibited.</div>";

            ROE.Frame.Confirm(confirmtext, "Yes", "No", "rgba(0,0,0,0.8)", blocking_confirmation, undefined);

            function blocking_confirmation() {

                ajax("ChatBlockAjax.aspx",
                    { block: name },

                    function (r) {
                        if (r != 'ok') {
                            ROE.Frame.errorBar("Error blocking player. <a onclick=' ROE.UI.Sounds.click(); ROE.Mail.showBlock();'>See all blocked players</a>");
                        } else {
                            ROE.Frame.infoBar("Player blocked!. <a onclick=' ROE.UI.Sounds.click(); ROE.Mail.showBlock();'>See all blocked players</a>");
                        }
                    }, function (w) {
                        ROE.Frame.infoBar("Player blocked!. <a onclick=' ROE.UI.Sounds.click(); ROE.Mail.showBlock();'>See all blocked players</a>");
                    }
                );

            }
        }


        function _chatWith() {

            var realmId = ROE.realmID;
            var myPlayerID = ROE.Player.id;
            var otherPlayerID = playerID;
            var otherPlayerName = name;
                  
            //console.log('createGroupChat_CheckOneOnOneFirst', realmId, myPlayerID, otherPlayerID, otherPlayerName);
            ROE.Chat2.chat.server.createGroupChat_CheckOneOnOneFirst('New Chat', realmId, myPlayerID, otherPlayerID, otherPlayerName).done(function (result) {
                //console.log(result)
                if (result.success == false && result.msg == "blocked") {
                    ROE.Frame.errorBar("Cant start chat. "+otherPlayerName+" has blocked you.");
                }
                //will have success, msg, and groupID. msg will be 'new' or 'existing'
            });
            
        }

    }

}(window.ROE.PlayerNew = window.ROE.PlayerNew || {}));