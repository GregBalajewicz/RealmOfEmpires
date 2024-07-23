// ensure ROE object exists
(function (obj, $, undefined) {
}(window.ROE = window.ROE || {}, jQuery));

// ensure ROE.UI object exists
(function (obj, $, undefined) {
}(window.ROE.UI = window.ROE.UI || {}, jQuery));

// ensure ROE.UI.PFs object exists
(function (obj, $, undefined) {

    var chat = ROE.Chat.chat;

    var chatTemplate = 
         '<div><div class="BtnBLg1 fontButton1L chatBar chatButton unselectable">' +
                        '<div class="barName">%chatName%</div>' +
                        '<span class="notification"></span>' +
                        '<div class="chatButton close"></div>' +
                    '</div>'+
         '<div class="chatWindow">' +
                        '<div class="userArea">' +
                            '<div class="fontSilverFrLCXlrg title">Users</div>' +
                            '<div class="users"></div>' +
                        '</div>' +
                        '<div class="chatArea">' +
                            '<div class="titleBar">' +
                                '<div class="fontSilverFrLClrg handle chatName">%chatName%</div>' +
                                '<div class="chatButton minimize">-</div>' +
                                '<div class="chatButton toggleUsers">users</div>' +
                            '</div>' +
                            '<div class="messages"></div>' +
                            '<div class="fontSilverFrLClrg newMessages">New messages<div class="icon"></div></div>' +
                            '<div class="chatInput">' +
                                '<input type="text" class="textbox" maxlength="256" placeholder="Type a message..."/>' +
                                '<input type="button" class="send" value="Send" />' +
                            '</div>' +
                        '</div>' +
                    '</div></div>';

    //$.connection.hub.error(function (er) {
    //    console.log(er);
    //});
    chat.client.closeAllChatsAndLogin = function () {
        $('#chatContainer').empty();
        $('#chatBarContainer').empty().append($('<div id="lostConnection"><div>Chat lost connection. Attempting to reconnect...</div><div>Click here to refresh the page.</div></div>').click(function () { location.reload(); }));
        ROE.Chat.login(true);
    };

    chat.client.broadcast = function (realmId, groupId, notifs, message) {
        var chatWindow = $('#realm' + (groupId + realmId));
        _broadcast(realmId, groupId, notifs, message, chatWindow);
    };

    chat.client.broadcastOneOnOne = function (realmId, groupId, name, earliestMsgDate, notifs, message) {
        var chatWindow = $('#realm' + groupId + realmId);
        //when a user chats with another user 1 on 1, the other user's chat window is not opened yet
        if (!chatWindow.length) {
            var settings = {
                chatName: name,
                barName: name,
                realmId: realmId,
                groupId: groupId,
                container: $('#chatBarContainer'),
                earliestMsgDate: earliestMsgDate,
                isOneOnOne: true
            };
            $.when(obj.init(settings)).then(function (chatElems) {
                _scrollBottom(chatElems.window);
                _broadcast(realmId, groupId, notifs, message, chatElems.window);

            });
        } else {
            _broadcast(realmId, groupId, notifs, message, chatWindow);
        }
    }

    chat.client.updateUsers = function (realmId, groupId, users) {
        var userPanel = $('#realm' + groupId + realmId + ' .users');
        userPanel.empty();
        var user, userRow;
        for (var i = 0; i < users.length; i++) {
            user = users[i];
            userRow = $('<div data-uid="' + user.uid + '" class="name"><div class="icon"></div>' + (user.name && user.name.length > 0 ? user.name : 'Anonymous') + '</div>').data('isBlocked', user.isBlocked);
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
            _openContextMenu(e, $(this), realmId, groupId);
        }).bind('click', function (e) { e.stopPropagation(); _openContextMenu(e, $(this), realmId, groupId); });
    };

    chat.client.createChatWindows = function (name, realmId, groupId, date, isOneOnOne) {
        var settings = {
            chatName: name,
            barName: 'Realm ' + realmId,
            realmId: realmId,
            groupId: groupId,
            container: $('#chatBarContainer'),
            earliestMsgDate: date,
            isOneOnOne: isOneOnOne
        };

        obj.init(settings);
    };

    chat.client.joinRealmCallback = function (status, windowId/*, name*/) {
        //join chat for first time
        if (status == 0) {
            _updateNewActiveWindow(windowId);
            $('#bar' + windowId).addClass('active');
            $('#realm' + windowId).find('.textbox').focus();
        //chat is already open
        } else if (status == 1) {
            _updateNewActiveWindow(windowId);
            _resetAndHideNotifications(windowId, false);
            $('#realm' + windowId).find('.textbox').focus();
        }
    };

    chat.client.oneOnOneChatCallback = function (realmId, groupId, name, earliestMsgDate) {
        var windowId = groupId + realmId;
        var chatWindow = $('#realm' + windowId);
        //chat is not created due to first time chatting or after closing chat window
        if (!chatWindow.length) {
            var settings = {
                chatName: name,
                barName: name,
                realmId: realmId,
                groupId: groupId,
                container: $('#chatBarContainer'),
                earliestMsgDate: earliestMsgDate,
                isOneOnOne: true
            };
            $.when(obj.init(settings)).then(function (chatElems) {
                _updateNewActiveWindow(windowId);
                chatElems.window.find('.textbox').focus();
                _scrollBottom(chatElems.window);  
            });  
        } else {
            _updateNewActiveWindow(windowId);
            _resetAndHideNotifications(windowId, false);
            $('#realm' + windowId).find('.textbox').focus();
        }
    }

    chat.client.changeOneOnOneChatName = function (windowId, name) {
        $('#realm' + windowId).find('.chatName').text(name);
        var bar = $('#bar' + windowId + ' .barName').text(name);
    }

    chat.client.closeChat = function (windowId) {
        $('#realm' + windowId).remove();
        $('#bar' + windowId).remove();
    }

    function _init(settings) {
        var template = $(chatTemplate).clone();//$(BDA.Templates.getRaw("Chat")).clone();

        var chatBar = template.find('.chatBar').remove().clone();
        var chatWindow = template.find('.chatWindow').remove().clone();

        var chatContainer = $('#chatContainer');
        if (!chatContainer.length) {
            chatContainer = $('<div id="chatContainer"></div>');
            $('body').append(chatContainer);
        }

        var realmId = settings.realmId;
        var groupId = settings.groupId;
        var windowId = groupId + realmId;

        var populatedBar = $(BDA.Templates.populate(chatBar[0].outerHTML, { chatName: settings.barName })).attr('id', 'bar' + windowId).addClass('active');
        var populatedWindow = $(BDA.Templates.populate(chatWindow[0].outerHTML, { chatName: settings.chatName })).attr('id', 'realm' + windowId)
            .data('earliestMsgDate', settings.earliestMsgDate).data('realmId', realmId).css('bottom', $('#panelBottom').height());
        populatedBar.find('.notification').text(0).hide();
        
        var textbox = populatedWindow.find('.textbox');
        populatedWindow.find('.send').click(function () {
            _sendMessage(realmId, groupId, textbox);
            textbox.focus();
        });
        textbox.keypress(function (e) {
            if (e.which == 13) { _sendMessage(realmId, groupId, $(this)); }
        });
        populatedWindow.find('.newMessages').hide().click(function () {
            var messageContainer = populatedWindow.find('.messages');
            messageContainer.animate({ scrollTop: messageContainer[0].scrollHeight }, 250);
            $(this).hide();
            textbox.focus();
        });
        populatedWindow.find('.minimize').click(function (e) {
            populatedWindow.removeClass('displayed');
            populatedWindow.hide();
            populatedWindow.removeClass('active');
            populatedBar.removeClass('active');
            chat.server.setLastActive(null);
        });
        populatedWindow.find('.close').click(function () { _closeWindow(windowId); });
        populatedWindow.find('.toggleUsers').click(function () {
            $('.userArea', populatedWindow).toggleClass('shown');
        });

        if (settings.isOneOnOne) {
            populatedWindow.find('.userArea').remove();
            populatedWindow.find('.toggleUsers').remove();
                }

        populatedWindow.mousedown(function () {
            if (!populatedWindow.hasClass('active')) {
                _updateNewActiveWindow(windowId);
                _resetAndHideNotifications(windowId, false);
            }
        })
        .draggable({
            handle: '.titleBar',
            containment: 'body',
            stop: function () { $(this).removeClass('dragVirgin'); }
        })
        .resizable({
            handles: 'all',
            minWidth: 260,
            minHeight: 150,
            containment: 'body',
            resize: function (event, ui) {
                //fixes issue of resize snapping chatwindow back to corner
                var diffW = ui.size.width - ui.originalSize.width;
                var diffH = ui.size.height - ui.originalSize.height;
                $(this).css('left', ui.originalPosition.left - diffW);
                $(this).css('top', ui.originalPosition.top - diffH);                
            },
            stop: function () {
                _updateScroll(realmId, groupId);
            }
        })
        .find('.messages').scroll(function () { _updateScroll(realmId, groupId); })
                            .bind('scroll.loadOlder', function () { 
                                if ($(this).scrollTop() == 0) {
                                    _loadOlderMsgs(populatedWindow, realmId, groupId);
                                }
                            });

        

        populatedBar.click(function () {        
            if (populatedWindow.hasClass('active')) {
                populatedWindow.hide();
                populatedWindow.removeClass('active');
                populatedBar.removeClass('active');
                populatedWindow.removeClass('displayed');
                chat.server.setLastActive(null);
            } else {
                populatedWindow.show();
                _updateNewActiveWindow(windowId);
                var messageContainer = populatedWindow.find('.messages')
                messageContainer.scrollTop(messageContainer[0].scrollHeight);
                _resetAndHideNotifications(windowId, false);
                populatedWindow.addClass('displayed');
            }
            //if (populatedWindow.toggle().is(':visible')) {
            //    _updateNewActiveWindow(windowId);
            //    var messageContainer = populatedWindow.find('.messages')
            //    messageContainer.scrollTop(messageContainer[0].scrollHeight);
            //    _resetAndHideNotifications(windowId, false);
            //    populatedWindow.addClass('displayed');
            //} else {
            //    populatedWindow.removeClass('active');
            //    populatedBar.removeClass('active');
            //    populatedWindow.removeClass('displayed');
            //    chat.server.setLastActive(null);
            //}
        });
        populatedBar.find('.close').click(function () { _closeWindow(windowId); });

        settings.container.append(populatedBar);
        chatContainer.append(populatedWindow);
        _initialPlacement(populatedWindow);
        var returnValue = { window: populatedWindow, bar: populatedBar };
        var asyncPromise = $.Deferred();
        _loadOlderMsgs(populatedWindow, realmId, groupId, asyncPromise, returnValue);
        return asyncPromise.promise();    //don't finish until we load all older messages
    };

    function _initialPlacement(chatWindow) {

        chatWindow = $(chatWindow);
        var windowW = $(window).width();
        var windowH = $(window).height();
        var chatW = chatWindow.width();
        var chatH = chatWindow.height();
        var panelBottomH = $('#panelBottom').height();
        
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

        chatWindow.addClass('displayed dragVirgin');
    }

    function _getContextMenu(e, nameElem, realmId, groupId) {
        chat.server.isUserBlocked(nameElem.text(), realmId).done(function (isBlocked) {
            nameElem.data('isBlocked', isBlocked);
            _openContextMenu(e, nameElem, realmId, groupId);
        });
    }

    function _openContextMenu(e, elem, realmId, groupId) {
        if (!elem.hasClass('self') && !elem.parent().parent().hasClass('self')) {
            $('#userMenu').css({ 'left': e.pageX, 'top': e.pageY }).fadeIn(100, function () {
                $(document).bind('click', _hideUserContextMenu);
            }).children().hide();

            if (!elem.hasClass('dev') && !elem.parent().parent().hasClass('dev')) {
                if (elem.data('isBlocked')) {
                    $('#unblockUser').show().unbind('click').click(function () {
                        chat.server.blockUser(elem.text(), false, realmId, groupId);
                        $(this).unbind('click');
                    });
                } else {
                    $('#blockUser').show().unbind('click').click(function () {
                        chat.server.blockUser(elem.text(), true, realmId, groupId);
                        $(this).unbind('click');
                    });
                }
            }

            $('#chatUser').show().unbind('click').click(function () {
                chat.server.oneOnOneChat(elem.text(), realmId, groupId);
                $(this).unbind('click');
            });

            var uid = elem.attr('data-uid') || elem.parent().parent().attr('data-uid');
            if (uid) {
                $('#viewThrone').show().unbind('click').click(function () {
                    window.open('throneroom.aspx?uid=' + uid);
                });
            }
        }
    }

    function _hideUserContextMenu() {
        $('#userMenu').hide(100);
        $(document).unbind('click', _hideUserContextMenu);
    }

    function _updateNewActiveWindow(windowId) {
        var prevWindowId = $('#chatContainer').find('.active').removeClass('active').attr('id');
        if (prevWindowId) {
            chat.server.resetNotifications(prevWindowId.replace('realm', ''), false);
        }
        $('#realm' + windowId).addClass('active').find('.textbox').focus();
        chat.server.setLastActive(windowId);
    }

    function _resetAndHideNotifications(windowId, timed) {
        var bar = $('#bar' + windowId).addClass('active');
        bar.find('.notification').text(0).hide();
        bar.find('.barName').removeAttr('style');
        chat.server.resetNotifications(windowId, timed);
    }

    function _loadOlderMsgs(populatedWindow, realmId, groupId, asyncPromise, returnValue) {
        chat.server.loadMessages(realmId, groupId, populatedWindow.data('earliestMsgDate')).done(function (results) {
            if (results.success) {
                populatedWindow.data('earliestMsgDate', results.earliestMsgDate);
                var messageContainer = populatedWindow.find('.messages');

                var oldHeight = messageContainer[0].scrollHeight;
                var oldScrollPos = messageContainer.scrollTop();

                var olderMsg = $(results.olderMsgs[0]);
                olderMsg.find('.name').bind('contextmenu', function (e) {
                    e.preventDefault();
                    _getContextMenu(e, $(this), realmId, groupId);
                }).click(function (e) {
                    e.stopPropagation();
                    _getContextMenu(e, $(this), realmId, groupId);
                });
                messageContainer.prepend(olderMsg.data('lastDate', parseInt(results.lastMsgDate)));
                for (var i = 1; i < results.olderMsgs.length; i++) {
                    olderMsg = $(results.olderMsgs[i]);
                    olderMsg.find('.name').bind('contextmenu', function (e) {
                        e.preventDefault();
                        _getContextMenu(e, $(this), realmId, groupId);
                    }).click(function (e) {
                        e.stopPropagation();
                        _getContextMenu(e, $(this), realmId, groupId);
                    });
                    messageContainer.prepend(olderMsg);
                }

                messageContainer.scrollTop(oldScrollPos + messageContainer[0].scrollHeight - oldHeight); //keep the scrollbar to where the user was before adding all the new chat msgs
            } else {
                populatedWindow.find('.messages').unbind('scroll.loadOlder');
            }
            if (asyncPromise && returnValue) {
                asyncPromise.resolve(returnValue);
            }
        });
    }

    function _sendMessage(realmId, groupId, textbox) {
        var msg = textbox.val();
        chat.server.send(realmId, groupId, msg);
        textbox.val('');
    }

    function _broadcast(realmId, groupId, notifs, message, chatWindow) {
        if (chatWindow.length) {
            var windowId = groupId + realmId;
            var messageContainer = chatWindow.find('.messages');
            var bottomScroll = messageContainer[0].scrollHeight - messageContainer.height(); //get scrollheight before adding message
            var lastMsg = messageContainer.children().last();
            var htmlMsg = $(message);
            var lastDate = lastMsg.data('lastDate') + 5;
            if (lastDate >= 2400) { lastDate -= 2400; }
            var currDate = parseInt(htmlMsg.find('.date').text().replace(':', ''));
            if (lastDate > currDate && lastMsg.find('.name').text() == htmlMsg.find('.name').text()) {
                lastMsg.find('.msgBody').append($('<div class="extraMsg"></div>').append(htmlMsg.find('.msgBody').contents()));
                lastMsg.data('lastDate', currDate);
            } else {
                htmlMsg.find('.name').bind('contextmenu', function (e) {
                    e.preventDefault();
                    _getContextMenu(e, $(this), realmId, groupId);
                }).click(function (e) {
                    e.stopPropagation();
                    _getContextMenu(e, $(this), realmId, groupId);
                });
                messageContainer.append(htmlMsg.data('lastDate', currDate));
            }
            if (messageContainer.scrollTop() >= bottomScroll) { //if scrollbar is at the bottom, then we set the scrollbar to stay at the bottom
                messageContainer.scrollTop(messageContainer[0].scrollHeight);
            } else {
                chatWindow.find('.newMessages').show();
            }

            if (notifs > 0) {
                if (chatWindow.hasClass('active')) {
                    chat.server.resetNotifications(windowId, true);
                } else {
                    var bar = $('#bar' + windowId);
                    bar.find('.notification').text(notifs).show();
                    bar.find('.barName').css('width', '72px');
                }
            }
        }
    }

    //close the current chat
    function _closeWindow(windowId) {
        var chatWindow = $('#realm' + windowId);
        chat.server.closeChat(windowId, chatWindow.hasClass('active'));
        $('#bar' + windowId).remove();
        chatWindow.remove();
    }

    //windowId is the groupId concated with the realmId
    function _updateScroll(realmId, groupId) {
        var chatWindow = $('#realm' + (groupId + realmId));
        var newMessages = chatWindow.find('.newMessages');
        var messageContainer = chatWindow.find('.messages');
        if (newMessages.is(':visible')) {
            var bottomScroll = messageContainer[0].scrollHeight - messageContainer.height();
            if (messageContainer.scrollTop() >= bottomScroll) {
                newMessages.hide();
            }
        }
    }

    function _scrollBottom(chatWindow) {
        var messageContainer = chatWindow.find('.messages');
        messageContainer.scrollTop(messageContainer[0].scrollHeight);
    }

    obj.init = _init;
    obj.scrollBottom = _scrollBottom;
}(window.ROE.UI.Chat = window.ROE.UI.Chat || {}, jQuery));