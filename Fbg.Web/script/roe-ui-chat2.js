// ensure ROE object exists
(function (obj, $, undefined) {
}(window.ROE = window.ROE || {}, jQuery));

// ensure ROE.UI object exists
(function (obj, $, undefined) {
}(window.ROE.UI = window.ROE.UI || {}, jQuery));

(function (obj, $, undefined) {
    var chat = ROE.Chat2.chat;

    var GroupTypeEnum = {
        Global: 0,
        OneOnOne: 1,
        Group: 2,
        Clan: 3
    };

    var _lastActiveWindowID; //this will hold the last active window ID which is groupID + "ID" which appears to be realm ID or something, -farhad

    var chatTemplate =
        //chatbar part
         $('<div><div class="BtnBLg1 fontButton1L chatBar chatButton unselectable">' +
                        '<div class="barName">%chatName%</div>' +
                        '<span class="notification"></span>' +
                        '<div class="chatButton close"></div>' +
                    '</div>' +
        //chatwindow
         '<div class="chatWindow displayed">' +
                        '<div class="userArea">' +
                            '<div class="fontSilverFrLCXlrg title">Users<div class="chatButton toggleUsers2"></div></div>' +
                            '<div class="users"></div>' +
                        '</div>' +
                        '<div class="chatArea">' +
                            '<div class="titleBar">' +
                                '<div class="headerL"></div><div class="headerM"></div><div class="headerR"></div><div class="headerOverlay"></div>' +
                                '<div class="chatIcon chatType toggleUsers"></div>' +
                                '<div class="fontSilverFrLCXlrg handle chatName">%chatName%</div>' +
                                '<div class="status"></div>' + //not in use
                                '<div class="chatButton addUser"></div>' +
                                '<div class="chatButton pinBtn"></div>' +
                                '<div class="chatButton minimize"></div>' +

                            '</div>' +
                            '<div class="messages"></div>' +
                            '<div class="fontSilverFrLClrg newMessages">New messages<div class="icon"></div></div>' +
                            '<div class="chatInput">' +
                                //'<textarea type="text" class="textbox" maxlength="256" placeholder="Type a message..."/>' +
                                '<div class="toggleTools">+</div>' +
                                '<div class="chatInputTextWrapper"><input type="text" class="textbox" maxlength="256" placeholder="Type a message..."/></div>' +
                                '<input type="button" class="send" value="Send" />' +
                            '</div>' +
                        '</div>' +
                    '</div></div>');

    //$.connection.hub.error(function (er) {
    //    console.log(er);
    //});

    //sends a message out to this client
    //does nothing if the chat window does not exist
    chat.client.broadcast = function (id, groupId, notifs, message, date) {
        var chatWindow = $('#group' + groupId + id);
        _broadcast(groupId, id, notifs, message, date, chatWindow);
    };

    //sends a message out to this client, and opens a window if the window does not already exist
    //extraparams are not used, but may be an object for any extra parameters needed in the future for specific group types or something else
    chat.client.broadcastOpen = function (id, groupId, realmId, type, name, earliestMsgDate, notifs, message, extraParams) {
        
        var chatWindow = $('#group' + groupId + id);
        //if window doesn't exist yet and is not muted
        if (!chatWindow.length && !$.cookie('mutedChat' + groupId + (ROE.Chat2.areaId || '') + id)) {
            /* 
            
            //commented out auto chat creation, based on new design to only open chats manually by user
            //this became a design flaw when many users could msg and bombard a player with many auto opening windows
            //so turning it off for now -farhad

            var settings = {
                chatName: name,
                realmId: realmId,
                id: id,
                groupId: groupId,
                container: $('#chatBarContainer'),
                earliestMsgDate: earliestMsgDate,
                type: type,
                extraParams: extraParams
            };
            $.when(obj.init(settings)).then(function (chatElems) {
                _scrollBottom(chatElems.window);
                _broadcast(groupId, id, notifs, message, earliestMsgDate, chatElems.window);
                //update chat history and users for this guy
                _updateLastActiveWindow(); //need to do this to update the chat history notifications
                ROE.Chat2.updateChatList();
                chat.server.updateUsers(groupId, id);
            });
            */

            _broadcast(groupId, id, notifs, message, earliestMsgDate, []);
            ROE.Chat2.updateChatList();
            chat.server.updateUsers(groupId, id);
        } else {
            //if user was previously kicked and re-added
            if (chatWindow.length) {
                chatWindow.removeClass('disabled');
                var textbox = chatWindow.find('.textbox');
                if (textbox.attr('disabled')) {
                    textbox.removeAttr('disabled');
                    ROE.Chat2.updateChatList();
                    chat.server.updateUsers(groupId, id);
                }
            }
            _broadcast(groupId, id, notifs, message, earliestMsgDate, chatWindow);
        }
    }

    //empties the users panel, and refills it with the updated results
    chat.client.updateUsers = function (groupId, id, users, type) {
        var chatWindow = $('#group' + groupId + id);
        var userPanel = chatWindow.find('.users');
        var realmId = chatWindow.data('realmId');
        var type = chatWindow.data('type');
        userPanel.empty();
        var user, userRow;
        for (var i = 0; i < users.length; i++) {
            user = users[i];
            userRow = $('<div data-id="' + user.id + '" data-name="' + user.name + '" class="name"><div class="icon"></div>' + user.name + '</div>');
            if (user.isSelf) { userRow.addClass('self'); }
            if (user.isDev) {
                userRow.addClass('dev');
                userPanel.prepend(userRow);
            } else {
                userPanel.append(userRow);
            }
        }
        userPanel.children().bind('contextmenu', function (e) {
            e.preventDefault();
            _openContextMenu(e, $(this), groupId, realmId, id, type);
        }).bind('click', function (e) { e.stopPropagation(); _openContextMenu(e, $(this), groupId, realmId, id, type); });
    };

    //kicking user from chat just disablees the chat textbox.
    chat.client.kickFromChat = function (groupId, id) {
        var chatWindow = $('#group' + groupId + id);
        if (chatWindow.length) {
            chatWindow.addClass('disabled');
            chatWindow.find('.textbox').attr('disabled', 'disabled');
            chatWindow.find('.users').children().unbind('click contextmenu');
        }
        ROE.Chat2.updateChatList();
    }

    chat.client.setStatus = function (windowId, isOnline) {
        var chatWindow = $('#group' + windowId);
        if (chatWindow.length) {
            var statusIcon = chatWindow.find('.status');
            if (isOnline) {
                statusIcon.addClass('online');
            } else {
                statusIcon.removeClass('online');
            }
        }
    }

    chat.client.createChatWindows = function (name, groupId, realmId, id, date, type, extraParams, shouldUpdateHistory) {

        ROE.Chat2.newestCreatedChatGroupID = groupId;

        var windowId = groupId + id;
        if (!$('#group' + windowId).length) {

            var settings = {
                chatName: name,
                realmId: realmId,
                groupId: groupId,
                id: id,
                type: type,
                container: $('#chatBarContainer'),
                earliestMsgDate: date,
                extraParams: extraParams
            };
            $.when(obj.init(settings)).then(function (chatElems) {

                //this becomes new active window, and reset its notifications
                _updateNewActiveWindow(windowId);
                _resetAndHideNotifications(windowId);

                //sometimes, we do not need to update chat history because it is already updated
                if (shouldUpdateHistory) {
                    ROE.Chat2.updateChatList();
                }

                chat.server.updateUsers(groupId, id);

                if (type == 3) {
                    $('.globalAndClanChatBars .chatBar.chatButton[data-type="3"]').click();
                }

            });
        } else {
            //chat window is already open so we just make it active
            _updateNewActiveWindow(windowId);
            _resetAndHideNotifications(windowId);
        }
    };

    chat.client.closeChat = function (windowId) {
        var window = $('#group' + windowId).remove();
        $('#bar' + windowId).remove();
        BDA.Broadcast.publish('contentchanged');
        removeChatCookie(window.data('id'), window.data('groupId'));
    }

    chat.client.updateChatList = function () {
        ROE.Chat2.updateChatList();
    }

    //this is basically the toggleMuteChat callback for _toggleChatMute in ROE.Chat2
    chat.client.toggleMuteChat = function (groupId, id) {
        //console.log('chat.client.toggleMuteChat', groupId, id);
        //$('#history' + groupId + id).toggleClass('muted');
    }

    //changes group name from bar, window, and chatlist
    chat.client.changeGroupName = function (id, groupId, realmId, type, name, earliestMsgDate, notifs, message, extraParams) {
        var windowId = groupId + id;
        var chatWindow = $('#group' + windowId);
        if (chatWindow.length) {
            var fullChatName = (isNaN(realmId) || ROE.Chat2.area == realmId ? '' : '(R' + realmId + ') ') + name;
            $('#history' + windowId).find('.labelName').html(name); //change chatlist name
            $('#bar' + windowId).find('.barName').html(fullChatName); //change bar name
            chatWindow.find('.chatName').html(fullChatName); //change chat window name
            _broadcast(groupId, id, notifs, message, earliestMsgDate, chatWindow); //broadcast the adminmsg
        } else {
            chat.client.broadcastOpen(id, groupId, realmId, type, name, earliestMsgDate, notifs, message, extraParams);
        }
    }

    //creates a new chat window + a button on the chatbar
    //the chatwindow will have data:
    //realmId
    //groupId
    //id - player or userid the chat is associated with

    //settings has numerous properties:
    //container - container for chatbar buttons
    //earliestMsgDate - datetime of the earliest msg loaded (used to load msgs older than this date)
    //realmId
    //id
    //groupId
    //type
    //chatName
    function _init(settings) {
        //     var template = $(chatTemplate).clone();//$(BDA.Templates.getRaw("Chat")).clone();

        var chatBar = chatTemplate.find('.chatBar').clone();
        var chatWindow = chatTemplate.find('.chatWindow').clone();

        var chatContainer = $('#chatContainer');

        var realmId = settings.realmId;
        var groupId = settings.groupId;
        var id = settings.id;
        var windowId = groupId + id;

        addChatCookie(id, groupId); //this window gets added to open chats cookies

        //prefix name with realm id if we are in throneroom and the chat belongs to a realm
        var chatName = (isNaN(realmId) || ROE.Chat2.area == realmId ? '' : '(R' + realmId + ') ') + settings.chatName;
        if (ROE.Chat2.area && settings.type === 0) {
            chatName = 'Realm Chat'; //InRealm realmchat should just be Realm Chat
        }

        //create the chatbar and chatwindow. store groupid, realmId, id, and type in the chatwindow as data
        var populatedBar = $(BDA.Templates.populate(chatBar[0].outerHTML, { chatName: chatName })).attr('id', 'bar' + windowId).attr('data-type', settings.type).addClass('active');
        var populatedWindow = $(BDA.Templates.populate(chatWindow[0].outerHTML, { chatName: chatName })).attr('id', 'group' + windowId).attr('type', settings.type)
            .data('earliestMsgDate', settings.earliestMsgDate).data('realmId', realmId).data('groupId', groupId).data('id', id).data('type', settings.type).css('bottom', $('#panelBottom').height());
        populatedBar.find('.notification').text(0).hide();

        var textbox = populatedWindow.find('.textbox');
        populatedWindow.find('.send').click(function () {
            _sendMessage(groupId, id, textbox);
            //    textbox.focus();
        });
        textbox.keypress(function (e) {
            if (e.which == 13) { _sendMessage(groupId, id, $(this)); }
        });

        var toggleTools = populatedWindow.find('.toggleTools');
        toggleTools.click(function () {
            _toggleChatTools(populatedWindow, groupId, id);
        });

        populatedWindow.find('.newMessages').hide().click(function () {
            var messageContainer = populatedWindow.find('.messages');
            messageContainer.animate({ scrollTop: messageContainer[0].scrollHeight }, 250);
            $(this).hide();
            textbox.focus();
        });
        populatedWindow.find('.minimize').click(function (e) {
            populatedWindow.removeClass('active displayed');
            $('.userArea', populatedWindow).removeClass('shown');
            populatedBar.removeClass('active');
            chat.server.setLastActive(null, null, null);
        });

        populatedWindow.find('.pinBtn').click(function (e) {
            populatedWindow.toggleClass('pinned');
        });



        //populatedWindow.find('.close').click(function () { _closeWindow(groupId, id); });

        $('.titleBar .chatIcon.chatType, .toggleUsers2', populatedWindow).click(function () {
            $('.userArea', populatedWindow).toggleClass('shown');
        });

        //for non-groupchats we remove the users list unless its global chat 
        if (!(ROE.Chat2.area == null && settings.type == GroupTypeEnum.Global && isNaN(realmId)) /*<-- these conditions are to check if its a global chat*/ && settings.type != GroupTypeEnum.Group) {
            populatedWindow.find('.userArea').remove();
            $('.chatIcon.chatType.toggleUsers', populatedWindow).removeClass('toggleUsers');
            //online/offlin functionality only for 1on1 but not needed anymore
            //populatedWindow.addClass('oneOnOne');
            //if (settings.extraParams && settings.extraParams.isOnline) {
            //    populatedWindow.find('.status').addClass('online');
            //}
        } else if (settings.type == GroupTypeEnum.Group) {

            populatedWindow.find('.addUser').click(function (e) {
                _addGroupMembersBtnClick(realmId, groupId, id);
            });

            //add the add users button
            populatedWindow.find('.userArea .title').append($('<div class="chatButton add"></div>').click(function () { _addGroupMembersBtnClick(realmId, groupId, id); }));

            //copy user list button
            populatedWindow.find('.userArea .title').append($('<div class="chatButton copyUserList"></div>').click(function () {

                ROE.Frame.simplePopopOverlay("https://static.realmofempires.com/images/icons/m_Reports.png", "Copy list of chat members", "", "copyUsers");

                var names = "";
                populatedWindow.find('.userArea .name').each(function () {
                    names += $(this).attr('data-name') + ", ";
                });
                var textNamesArea = $('<textarea class="namesArea"></textarea>').val(names);
                textNamesArea.bind('copy', function () {
                    setTimeout(function () {
                        $('.simplePopupOverlay.copyUsers').remove();
                    }, 100);
                });
                textNamesArea.bind('cut', function () {
                    setTimeout(function () {
                        $('.simplePopupOverlay.copyUsers').remove();
                    }, 100);
                });
                
                $('.simplePopupOverlay.copyUsers .pContent').append(textNamesArea);
                textNamesArea.select();
   
            }));

            //add the change name button
            //populatedWindow.find('.titleBar').append($('<div class="chatButton changeName"></div>').click(function () { _changeGroupNameBtnClick(groupId, id); }));

            /* disabling handle click to rename
            populatedWindow.find('.handle.chatName').click(function () {
                if (!populatedWindow.hasClass('recentDrag')) { //avoids accidental dragclick
                    _changeGroupNameBtnClick(groupId, id);
                }
            });
            */
        }

        if (settings.type != GroupTypeEnum.Group) {
            populatedWindow.find('.addUser').remove();
        }

        //using mousedown as opposed to click because when you drag a window, you want it to be at the top already
        populatedWindow.mousedown(function () {
            //if not already active, set this as new active
            if (!populatedWindow.hasClass('active')) {
                _updateNewActiveWindow(windowId);
                _resetAndHideNotifications(windowId);
            }
        })
        .draggable({
            handle: '.titleBar',
            containment: 'body',
            start: function (event,ui) {
                $(this).addClass('recentDrag');
            },
            stop: function (event,ui) {
                $(this).removeClass('dragVirgin'); //probably not needed any more, with the pin changes -farhad
                $(this).addClass('pinned');
                setTimeout(function () {
                    $(ui.helper).removeClass('recentDrag');
                }, 200);
            }
        })
        .resizable({
            handles: 'all',
            minWidth: 260,
            minHeight: 150,
            containment: 'body',
            resize: ROE.Chat2.fixResize,
            stop: function () {
                _updateScroll(windowId);
            }
        })
        .find('.messages').scroll(function () { _updateScroll(windowId); })
            .bind('scroll.loadOlder', function () {
                //function to load older messages when user scrolls to the top of the window
                if ($(this).scrollTop() == 0) {
                    _loadOlderMsgs(populatedWindow, groupId, realmId, id);
                }
            });

        populatedBar.click(function () {
            //if windows active, we hide it. If it isnt, we make it the new active window
            if (populatedWindow.hasClass('active')) {
                populatedBar.removeClass('active');
                populatedWindow.removeClass('active displayed');
                $('.userArea', populatedWindow).removeClass('shown');
                chat.server.setLastActive(null);
            } else {
                populatedBar.addClass('active');
                _updateNewActiveWindow(windowId);
                var messageContainer = populatedWindow.find('.messages')
                messageContainer.scrollTop(messageContainer[0].scrollHeight);
                _resetAndHideNotifications(windowId);
                populatedWindow.addClass('displayed');
            }
        });
        populatedBar.find('.close').click(function () { _closeWindow(groupId, id); });

        //combine global/clan chat for in realm
        if (ROE.Chat2.area != null && (settings.type == GroupTypeEnum.Global || settings.type == GroupTypeEnum.Clan)) {
            _setupGlobalClanChat(populatedWindow, populatedBar, chatContainer, windowId, settings.type);

        } else {
            settings.container.append(populatedBar);
            chatContainer.append(populatedWindow);
            _initialPlacement(populatedWindow);
        }


        BDA.Broadcast.publish('contentchanged');
        var returnValue = { window: populatedWindow, bar: populatedBar };
        var asyncPromise = $.Deferred();
        _loadOlderMsgs(populatedWindow, groupId, realmId, id, asyncPromise, returnValue);
        return asyncPromise.promise();    //don't finish until we load all older messages
        //this promise is needed for function calls such as broadcastopen, because we need to wait until all messages are loaded before sending the new message
    };

    //opens dialog to add group members
    function _addGroupMembersBtnClick(realmId, groupId, id) {
        var content = $('<div>' +
                            '<div class="chatPopupLabel">Please enter the player name(s) to be added:</div>' +
                        '</div>');
        var input = $('<input id="addGroupPlayer" type="text"/>');
        var autocompleteBox = $('<div class="autoCompleteBox"></div>');
        ROE.Chat2.autocompletePlayer(input, autocompleteBox, realmId, true);
        content.append(input);
        content.append($('<div class="BtnBLg1 fontButton1L chatPopupBtn addBtn" data-groupId="' + groupId + '" data-id="' + id + '">Add</div>').click(function () {
            chat.server.addPlayersToGroup($(this).attr('data-groupId'), input.val(), $(this).attr('data-id')).done(function (result) {
                if (result.success) {
                    $('#chatDialogNotif').empty().append('<div class="goodNotif">Players added!</div>');
                } else {
                    var errorDiv = $('#chatDialogNotif').empty().append('<div class="badNotif">The following players were not added:</div>');
                    for (var i = 0; i < result.errorMsgs.length; i++) {
                        errorDiv.append('<div style="color:red;">' + result.errorMsgs[i] + '</div>');
                    }
                }
            });
        }));
        content.append('<div id="chatDialogNotif"></div>');
        content.append(autocompleteBox);
        ROE.Chat2.popupChatDialog('Add Players', content, 400, 400);
    }

    //opens dialog to change group name
    function _changeGroupNameBtnClick(groupId, id) {
        var content = $('<div>' +
                            '<div class="chatPopupLabel">Please enter the new group name:</div>' +
                            '<input id="changeGroupName" type="text" maxlength="256"/>' +
                        '</div>');
        content.append($('<div class="BtnBLg1 fontButton1L chatPopupBtn saveBtn" data-groupId="' + groupId + '" data-id="' + id + '">Save</div>').click(function () {
            chat.server.changeGroupName($(this).attr('data-groupId'), $('#changeGroupName').val(), $(this).attr('data-id')).done(function (result) {
                if (result.success) {
                    $('#chatDialog').dialog('close');
                } else {
                    $('#chatDialogNotif').empty().append('<div class="badNotif">' + result.error + '</div>');
                }
            });
        }));
        content.append('<div id="chatDialogNotif"></div>');
        ROE.Chat2.popupChatDialog('Change Group Name', content);
    }

    function _initialPlacement(chatWindow) {
        chatWindow = $(chatWindow);
        var windowW = $(window).width();
        var windowH = $(window).height();
        var chatW = chatWindow.width();
        var chatH = chatWindow.height();
        var panelBottomH = $('#panelBottom').height();
        var chatListWindow = $('#chatList.displayed');

        if (chatListWindow.length) {
            var chatListWindowPos = chatListWindow.position();
            chatWindow.css({
                position: 'absolute',
                left: chatListWindowPos.left - chatW,
                top: chatListWindowPos.top
            });
        } else {
            chatWindow.css({
                position: 'absolute',
                left: windowW - chatW,
                top: windowH - chatH - panelBottomH
            });
        }

        /*
        if ($('.displayed.dragVirgin', chatContainer).length) {
            var lastVirginCHat = $('.displayed.dragVirgin', chatContainer).last();
            var lastVirginPosition = lastVirginCHat.position();
            chatWindow.css({
                position: 'absolute',
                left: Math.max(0, lastVirginPosition.left - 100),
                top: lastVirginPosition.top
            });

        } else {
            chatWindow.css({
                position: 'absolute',
                left: windowW - chatW,
                top: windowH - chatH - panelBottomH
            });
        }
        */

        chatWindow.addClass('displayed dragVirgin');
    }

    //sets up the global and clan chat panel in realm
    function _setupGlobalClanChat(populatedWindow, populatedBar, chatContainer, windowId, type) {
        populatedWindow.draggable('destroy').resizable('destroy').find('.titleBar').remove();
        populatedBar.find('.close').remove();

        if (type == GroupTypeEnum.Clan) {
            populatedWindow.removeClass('displayed');
            populatedBar.removeClass('active');
        }

        var globalClanChat = $('#globalAndClanChat');
        globalClanChat.find('.list').append(populatedWindow);
        globalClanChat.find('.globalAndClanChatBars').append(populatedBar);

        //rebind click handler for populatedbar 
        populatedBar.unbind('click').click(function () {
            //when these guys are clicked, they will always just show, and hide their counterpart (eg clan btn hides global chat)
            $('.displayed', globalClanChat).removeClass('displayed');
            $('.active', globalClanChat).removeClass('active');
            $('.selected', globalClanChat).removeClass('selected');
            //globalClanChat.find('.globalAndClanChatBars').removeClass('active selected');
            populatedBar.addClass('active selected');
            _updateNewActiveWindow(windowId);
            var messageContainer = populatedWindow.find('.messages')
            messageContainer.scrollTop(messageContainer[0].scrollHeight);
            _resetAndHideNotifications(windowId);
            populatedWindow.addClass('displayed');
        });

        //makes sure InRealm Realm Chat is always first tab
        $('.globalAndClanChatBars .chatBar.chatButton[data-type="0"]').prependTo($('.globalAndClanChatBars'));
    }

    //we always unbind click and then bind it again because there could be a previous handler on it
    function _openContextMenu(e, elem, groupId, realmId, id, type) {
        var targetId = elem.attr('data-id') || elem.parent().parent().attr('data-id'); //since there are 2 different places that call this fxn, the data-id can be either in the elem, or the parent's parent of elem
        var targetName = elem.attr('data-name') || elem.parent().parent().attr('data-name'); //same for name
        $('#userMenu').css({ 'left': e.pageX, 'top': e.pageY }).fadeIn(100, function () {
            $(document).bind('click', _hideUserContextMenu);
        }).children().hide(); //hide all context menu items because we only show some based on cases below
        if (!isNaN(realmId) && ROE.Chat2.area != null) { //show viewplayerprofile only if area is in realm
            $('#viewPlayerProfile').show().unbind('click').click(function () {
                ROE.Frame.popupPlayerProfile(targetName);
            });
        }
        if (!elem.hasClass('self') && !elem.parent().parent().hasClass('self')) { //do not show anything if contextmenu is on self
            chat.server.isUserBlocked(targetId, id).done(function (isBlocked) {
                //sorry, can't block devs 
                if (!elem.hasClass('dev') && !elem.parent().parent().hasClass('dev')) {
                    if (isBlocked) {
                        $('#unblockUser').show().unbind('click').click(function () {
                            chat.server.blockUser(false, groupId, id, targetId, targetName); //elem text used here
                            $(this).unbind('click');
                        });
                    } else {
                        $('#blockUser').show().unbind('click').click(function () {
                            chat.server.blockUser(true, groupId, id, targetId, targetName);
                            $(this).unbind('click');
                        });
                    }
                }

                $('#chatUser').show().unbind('click').click(function () {

                    chat.server.createGroupChat_CheckOneOnOneFirst('New Chat', realmId, id, targetId, targetName).done(function (result) {
                        //console.log(result) 
                        //will have success, msg, and groupID. msg will be 'new' or 'existing'
                    });
                 
                    $(this).unbind('click');
                });

                $('#viewThrone').show().unbind('click').click(function () {
                    if (isNaN(targetId)) { //for user throneroom lookup

                        if ($('body').hasClass('mobile')) {
                            ROE.Frame.showIframeOpenDialog($('#genericDialog'), 'throneroom.aspx?uid=' + targetId, 'Throne Room');
                        } else {
                            window['throneroomWindow'] = window.open('throneroom.aspx?uid=' + targetId);
                        }
                        
                    } else { //for player throneroom lookup

                        if ($('body').hasClass('mobile')) {
                            ROE.Frame.showIframeOpenDialog($('#genericDialog'), 'throneroom.aspx?rid=' + realmId + '&pid=' + targetId + '&viewerpid=' + id, 'Throne Room');
                        } else {
                            window['throneroomWindow'] = window.open('throneroom.aspx?rid=' + realmId + '&pid=' + targetId + '&viewerpid=' + id, 'throneroom');
                        }
                    }
                });

                if (type == GroupTypeEnum.Group) {
                    $('#kickUser').show().unbind('click').click(function () {
                        chat.server.kickUser(groupId, id, targetId, targetName);
                    });
                }
            });
        }
    }

    function _hideUserContextMenu() {
        $('#userMenu').hide(100);
        $(document).unbind('click', _hideUserContextMenu);
    }

    //resets notifications of old active window and sets this new window as active
    function _updateNewActiveWindow(windowId) {
        _updateLastActiveWindow();
        _lastActiveWindowID = windowId;
        $('#bar' + windowId).addClass('active');
        var newActiveWindow = $('#group' + windowId);
        newActiveWindow.addClass('displayed active'); //.find('.textbox').focus();
        chat.server.setLastActive(newActiveWindow.data('groupId'), newActiveWindow.data('realmId'), newActiveWindow.data('id'));
    }

    //resets notifications of last active and removes its active status
    function _updateLastActiveWindow() {
        //var prevActiveWindow = $('#chatContainer').find('.active');
        var prevActiveWindow = $('#group' + _lastActiveWindowID);
        if (prevActiveWindow.length) {
            prevActiveWindow.removeClass('active');
            chat.server.resetNotifications(prevActiveWindow.data('groupId'), prevActiveWindow.data('realmId'), prevActiveWindow.data('id'), false);
        }
    }

    function _resetAndHideNotifications(windowId) {
        var bar = $('#bar' + windowId);
        var window = $('#group' + windowId);

        //hide notifications in the chat history window
        var historyChat = $('#history' + windowId);
        historyChat.find('.notification').text(0).hide();
        _updateRealmLabelNotifications(historyChat.parent());

        //hide notifications in the chat bar
        bar.find('.notification').text(0).hide();
        bar.find('.barName').removeAttr('style');

        //update the titlebar notifications
        ROE.Chat2.updateDocumentTitleNotifications();

        //reset notifications
        chat.server.resetNotifications(window.data('groupId'), window.data('realmId'), window.data('id'), false);
    }

    function _loadOlderMsgs(populatedWindow, groupId, realmId, id, asyncPromise, returnValue) {
        chat.server.loadMessages(groupId, id, populatedWindow.data('earliestMsgDate')).done(function (results) {
            if (results.success) {
                var type = populatedWindow.data('type');
                populatedWindow.data('earliestMsgDate', results.earliestMsgDate);//update the new earliestmsg date for when user goes to load more msgs again
                var messageContainer = populatedWindow.find('.messages');

                var oldHeight = messageContainer[0].scrollHeight;
                var oldScrollPos = messageContainer.scrollTop();

                //do this for first msg just to save the lastdate data
                var olderMsg = $(results.olderMsgs[0]);
                //setup context menu for olderMsg
                olderMsg.find('.name').bind('contextmenu', function (e) {
                    e.preventDefault();
                    _openContextMenu(e, $(this), groupId, realmId, id, type);
                }).click(function (e) {
                    e.stopPropagation();
                    _openContextMenu(e, $(this), groupId, realmId, id, type);
                });
                messageContainer.prepend(olderMsg.data('lastDate', results.lastMsgDate)); //we save the data last date for the most recent message in order to group new messages into the same bubble if needed

                for (var i = 1; i < results.olderMsgs.length; i++) {
                    olderMsg = $(results.olderMsgs[i]);
                    olderMsg.find('.name').bind('contextmenu', function (e) {
                        e.preventDefault();
                        _openContextMenu(e, $(this), groupId, realmId, id, type);
                    }).click(function (e) {
                        e.stopPropagation();
                        _openContextMenu(e, $(this), groupId, realmId, id, type);
                    });
                    messageContainer.prepend(olderMsg);
                }

                messageContainer.scrollTop(oldScrollPos + messageContainer[0].scrollHeight - oldHeight); //keep the scrollbar to where the user was before adding all the new chat msgs
            } else {
                populatedWindow.find('.messages').unbind('scroll.loadOlder');
            }

            //this only happens in init chat
            if (asyncPromise && returnValue) {
                asyncPromise.resolve(returnValue);
            }
        });
    }

    function _sendMessage(groupId, id, textbox) {
        var msg = textbox.val();
        if (msg && msg.trim().length) {
            chat.server.send(groupId, id, msg);
            textbox.val('');
        }
    }

    function _sendMessageTarget(groupId, id, msg) {
        chat.server.sendMessageTarget(groupId, id, msg);
    }

    function _broadcast(groupId, id, notifs, message, date, chatWindow) {

        //if window exists broadcast the msg
        var windowId = groupId + id;
        if (chatWindow.length) {
            var realmId = chatWindow.data('realmId');
            var type = chatWindow.data('type');
            var messageContainer = chatWindow.find('.messages');
            var bottomScroll = messageContainer[0].scrollHeight - messageContainer.innerHeight(); //get scrollheight before adding message
            var lastMsg = messageContainer.children().last();
            var htmlMsg = $(message);
            var lastDate = lastMsg.data('lastDate');
            var milliDate = 0;
            var milliCurrDate = Date.parse(date);
            if (lastDate) {
                milliDate = Date.parse(lastDate) + 300000; //date + 5min in milliseconds
            }
            //if msg is within 5min from previous and is sent from same person, combine the msg into 1 bubble
            if (htmlMsg.hasClass('msg') && milliDate > 0 && milliDate > milliCurrDate &&
                lastMsg.find('.name').text() == htmlMsg.find('.name').text()) {
                lastMsg.find('.msgBody').append($('<div class="extraMsg"></div>').append(htmlMsg.find('.msgBody').contents()));
                lastMsg.data('lastDate', date);
                var lastMsgAvatarID = $('.avatar', lastMsg).attr('data-avatarid');
                var newMsgAvatarID = $('.avatar', htmlMsg).attr('data-avatarid');
                if(lastMsgAvatarID != newMsgAvatarID){
                    lastMsg.attr('data-avatarid', newMsgAvatarID);
                    $('.avatar', lastMsg).css({ 'background-image': "url('" + ROE.Avatar.list[newMsgAvatarID].imageUrlS + "')" });
                }
                //ensure latest avatarborderid is used
                $('.avatar', lastMsg).attr('data-avatarborderid', $('.avatar', htmlMsg).attr('data-avatarborderid'));
            } else {
                //new bubble
                htmlMsg.find('.name').bind('contextmenu', function (e) {
                    e.preventDefault();
                    _openContextMenu(e, $(this), groupId, realmId, id, type);
                }).click(function (e) {
                    e.stopPropagation();
                    _openContextMenu(e, $(this), groupId, realmId, id, type);
                });
                messageContainer.append(htmlMsg.data('lastDate', date));
            }
            if (messageContainer.scrollTop() + 5 >= bottomScroll) { //if scrollbar is at the bottom, then we set the scrollbar to stay at the bottom
                //there is an issue here that sometimes, people may have the scrollbar very close to the bottom but not exact, and then new messages pop up which is undesirable
                //can fix this by increasing the threshold (the + 5) a bit to still scroll to bottom if the scrollbar is a bit above the bottom
                messageContainer.scrollTop(messageContainer[0].scrollHeight);
            } else {
                chatWindow.find('.newMessages').show();
            }

            if (notifs > 0) {
                if (chatWindow.hasClass('active')) {
                    _resetAndHideNotifications(groupId + "" + id);
                } else {
                    var bar = $('#bar' + windowId);
                    var historyChat = $('#history' + windowId);
                    historyChat.find('.notification').text(notifs).show();
                    _updateRealmLabelNotifications(historyChat.parent());
                    bar.find('.notification').text(notifs).show();
                    bar.find('.barName').css('width', '72px');
                    ROE.Chat2.updateDocumentTitleNotifications();
                }
            }
        } else {
            //this happens when a chat is muted, we don't open the window, but we still want to update the notifications in the chatlist
            //therefor, the chatwindow won't be open, so we don't broadcast the msg, but still update the notifs in chatlist
            var chatList = $('#history' + windowId);
            if (chatList.length) {
                chatList.find('.notification').text(notifs).show();
                _updateRealmLabelNotifications(chatList.parent());
            }
        }
    }

    //close the current chat
    function _closeWindow(groupId, id) {
        var windowId = groupId + id;
        var chatWindow = $('#group' + windowId);
        //if the active param is true, chathub will update the last seen date so notifications work properly
        chat.server.closeChat(groupId, id, chatWindow.hasClass('active') || windowId == _lastActiveWindowID);
        $('#bar' + windowId).remove();
        chatWindow.remove();
        BDA.Broadcast.publish('contentchanged');
        removeChatCookie(id, groupId);
    }

    //windowId is the groupId concated with the realmId
    function _updateScroll(windowId) {
        var chatWindow = $('#group' + windowId);
        var newMessages = chatWindow.find('.newMessages');
        var messageContainer = chatWindow.find('.messages');
        //hides newmessages button if user scrolled to bottom
        if (newMessages.is(':visible')) {
            var bottomScroll = messageContainer[0].scrollHeight - messageContainer.innerHeight();
            if (messageContainer.scrollTop() + 5 >= bottomScroll) {
                newMessages.hide();
            }
        }
    }

    //scrolls to the bottom of chat window messages
    function _scrollBottom(chatWindow) {
        var messageContainer = chatWindow.find('.messages');
        messageContainer.scrollTop(messageContainer[0].scrollHeight);
    }

    //updates how many chats have notifications
    function _updateRealmLabelNotifications(realmSet) {

        var chatsWithNotifications = 0;
        realmSet.children('.chatLabel').each(function () {
            if ($(this).find('.notification').text() > 0) {
                chatsWithNotifications++;
            }
        });

        if (chatsWithNotifications > 0) {
            realmSet.children('.realmLabel').find('.notification').text(chatsWithNotifications).show();
        } else {
            realmSet.children('.realmLabel').find('.notification').text(0).hide();
        }
    }

    function _toggleChatTools(populatedWindow, chatGroupID, userPlayerID) {
        console.log(chatGroupID, userPlayerID);
        var chatInput = populatedWindow.find('.chatInput');
        var chatGroupType = populatedWindow.attr('type');
        var chatTextBox = populatedWindow.find('.textbox');
        var toolBox = $('<div class="chatToolBox"></div>');

        //if no showTools class, we want to show now, else hide now
        var showNow = !(populatedWindow.hasClass('showTools'));
        if (showNow) {
            _showChatTools(populatedWindow, chatTextBox, chatGroupID, userPlayerID, chatGroupType);
        } else {
            _hideChatTools(populatedWindow);
        }

    }

    function _showChatTools(populatedWindow, chatTextBox, chatGroupID, userPlayerID, chatGroupType) {
        populatedWindow.addClass('showTools');

        var toolBox = $('<div class="chatToolBox"></div>');

        //the following for ingroup chats / clan chats
        if (chatGroupType == GroupTypeEnum.Group || chatGroupType == GroupTypeEnum.Clan) {

            //call to support button
            var callSupportsBtn = $('<div class="cToolBtn callSupport helpTooltip" data-tooltipid="chatCallSupportsBtn">Call Support</div>');
            callSupportsBtn.click(function () {
                _callTargetOnChat(populatedWindow, chatGroupID, userPlayerID, chatTextBox, 1, $(this));
            });
            toolBox.append(callSupportsBtn);

            //call to attack button
            var callAttacksBtn = $('<div class="cToolBtn callAttack helpTooltip" data-tooltipid="chatCallAttacksBtn">Call Attack</div>');
            callAttacksBtn.click(function () {
                _callTargetOnChat(populatedWindow, chatGroupID, userPlayerID, chatTextBox, 2, $(this));
            });
            toolBox.append(callAttacksBtn);
        }

        populatedWindow.find('.chatInput').append(toolBox);
    }

    function _hideChatTools(populatedWindow) {
        $('.chatToolBox', populatedWindow).remove();
        $('.toolTipBox',populatedWindow).remove();
        populatedWindow.removeClass('showTools');
    }


    function _callTargetOnChat(populatedWindow, chatGroupID, userPlayerID, chatTextBox, type, btn) {

        var msgArr = ROE.Targets.bbCodeTargets(type);
        if (msgArr.length < 1) {
            var errorMsg = type == 1 ? 'No Support Targets found.' : 'No Attack Targets found.';
            ROE.Frame.errorBar(errorMsg);
            return;
        }

        var msg = "";
        for(var i = 0; i < msgArr.length; i++){
            msg += msgArr[i] + "\n";
        }     

        var callTargetToChatBox = $('<div class="callTargetToChatBox">Preview:</div>');
        var callTargetToChatTextInput = $('<textarea class="callTargetToChatTextInput" disabled="disabled"/>').val(msg);
        var confirmYes = $('<div class="confirmBtn yes">Send to chat</div>');
        confirmYes.click(function () {
            for (var i = 0; i < msgArr.length; i++) {
                _sendMessageTarget(chatGroupID, userPlayerID, msgArr[i]);
            }
            callTargetToChatBox.remove();
            _hideChatTools(populatedWindow);

        });

        var confirmCancel = $('<div class="confirmBtn cancel">Cancel</div>');
        confirmCancel.click(function () {
            callTargetToChatBox.remove();
        });

        callTargetToChatBox.append(callTargetToChatTextInput, confirmYes, confirmCancel);
        callTargetToChatBox.appendTo(populatedWindow);

    }




    //add window in cookies
    //areaId must be added in the key so we can distinguish the area. (different areas have different saved open windows
    //id must also be added in the key so we can distinguish between actual players. (another person logging in will not interfere with this guys cookies)
    function addChatCookie(id, groupId) {
        var playerChatList = JSON.parse($.cookie('chats' + (ROE.Chat2.areaId || '') + id) || '[]');
        if (!playerChatList) {
            playerChatList = [groupId];
        } else if (playerChatList.indexOf(groupId) == -1) {
            playerChatList.push(groupId);
        }
        $.cookie('chats' + (ROE.Chat2.areaId || '') + id, JSON.stringify(playerChatList), { path: '/' });
    }

    //remove window in cookies
    function removeChatCookie(id, groupId) {
        var playerChatList = JSON.parse($.cookie('chats' + (ROE.Chat2.areaId || '') + id) || '[]');
        if (playerChatList) {
            var index = playerChatList.indexOf(groupId);
            if (index != -1) {
                playerChatList.splice(index, 1);
                $.cookie('chats' + (ROE.Chat2.areaId || '') + id, JSON.stringify(playerChatList), { path: '/' });
            }
        }
    }

    obj.init = _init;
    obj.scrollBottom = _scrollBottom;
    obj.changeGroupNameBtnClick = _changeGroupNameBtnClick;
    obj.closeWindow = _closeWindow;

}(window.ROE.UI.Chat2 = window.ROE.UI.Chat2 || {}, jQuery));