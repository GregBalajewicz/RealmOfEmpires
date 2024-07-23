(function (ROE) {
}(window.ROE = window.ROE || {}));

(function (obj) {
    var _connectionStarted = false;
    var _chat = $.connection.chatHub;
    var _callingLogin = false; //variable to lock login to 1 call per person
    var _realmID = null;

    function _startHub(realmID) {
        _realmID = realmID;
      //  $.connection.hub.logging = true;
        $.connection.hub.start().done(function () {
            _connectionStarted = true;
            $('body').append('<div id="chatContainer"></div>' +
                            '<nav class="contextMenu" id="userMenu">' +
                                '<div id="chatUser" class="menuItem">Chat</div>' +
                                '<div id="blockUser" class="menuItem">Block</div>' + 
                                '<div id="unblockUser" class="menuItem">Unblock</div>' +
                                '<div id="viewThrone" class="menuItem">View Throne</div>' +
                            '</nav>');
            _login(false);
        });

        window.onbeforeunload = function () {
            $.connection.hub.stop();
        };

        $.connection.hub.reconnecting(function () {
            _reconnectWarning();
        });

        $.connection.hub.reconnected(function () {
            _login(true);
        });

        $.connection.hub.disconnected(function () {
            _connectionStarted = false;
            _reconnectWarning();
            setTimeout(function () {
                $.connection.hub.start().done(function () {
                    _connectionStarted = true;
                    _login(true);
                });
            }, 5000); // Restart connection after 5 seconds.
          });
    };

    function _setName(newName) {
        if (_connectionStarted) {
            _chat.server.setName(newName);
        }
    };
    function _joinRealm(realmId) {
        if (_connectionStarted) {
            _chat.server.joinRealm(realmId);
        }
    };
    function _getNotifications(realmId, callback) {
        if (_connectionStarted) {
            _chat.server.getNotifications(realmId).done(callback);
        }
    };

    function _login(reconnect) {
        if (!_callingLogin) {
            _callingLogin = true;
            var settings = {
                container: $('#chatBarContainer'),
            };
            _chat.server.login(_realmID).done(function (results) {
                if (reconnect) {
                    $('#lostConnection').remove();
                }
                if (results) {
                    settings.earliestMsgDate = results.date;

                    for (var i = 0; i < results.chats.length; i++) {
                        var currChat = results.chats[i];
                        settings.realmId = currChat.realmId;
                        settings.groupId = currChat.groupId;
                        settings.isOneOnOne = currChat.isOneOnOne;
                        settings.chatName = currChat.name;
                        settings.barName = currChat.barName;
                        $.when(ROE.UI.Chat.init(settings), currChat.realmId, currChat.groupId, currChat.notifs).then(function (newChatWindow, realmId, groupId, notifs) {
                            var window = newChatWindow.window;
                            if (notifs > 0) {
                                newChatWindow.bar.find('.notification').text(notifs).show();
                                newChatWindow.bar.find('.barName').css('width', '72px');
                            }
                            if (realmId == 'GlobalChat' && (groupId == null || groupId == '')) {
                                window.find('.close').remove();
                                newChatWindow.bar.find('.close').remove();
                            }
                            ROE.UI.Chat.scrollBottom(window);
                            _chat.server.loginComplete(groupId + realmId);

                            if (realmId == _realmID) {
                                var body = $('body');
                                window.css({
                                    left: (body.width() - window.width())/2,
                                    top: (body.height()- window.height())/2
                                });
                            }
                        });
                    }
                } else {
                    console.log('Error in calling login');
                }
                _callingLogin = false;
            }).fail(function () {
                _callingLogin = false;
                console.log('Error in calling login');
            });
        }
    }

    function _oneOnOneChat(playerName, realmId) {
        if (_connectionStarted) {
            _chat.server.oneOnOneChat(playerName, realmId, 'popup');
        }
    }

    function _reconnectWarning() {
        $('#chatContainer').empty();
        var btmPanel = $('#chatBarContainer');
        btmPanel.empty();
        btmPanel.append($('<div id="lostConnection"><div>Chat lost connection. Attempting to reconnect...</div><div>Click here to refresh the page.</div></div>').click(function () { location.reload(); }));
    }

    obj.chat = _chat
    obj.startHub = _startHub;
    obj.setName = _setName;
    obj.joinRealm = _joinRealm;
    obj.getNotifications = _getNotifications;
    obj.login = _login;
    obj.oneOnOneChat = _oneOnOneChat;
} (window.ROE.Chat = window.ROE.Chat || {}));