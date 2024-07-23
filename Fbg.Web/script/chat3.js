(function (ROE) {
}(window.ROE = window.ROE || {}));

(function (obj) {
    var _connectionStarted = false; //bool just to figure out if there is a signalr connection started
    var _chat = $.connection.chatHub2;  //signalr hub object
    var _callingLogin = false; //variable to lock login to 1 call per person
    var _chatSetup = false; //initially false, and set to true when all the chat elements are added (under _setupChat())
    var _area = null; //area user is in (ex. 84, 57, null for TR)
    var _areaId = null;// playerId for that area (null for TR)
    var _pid = null; //open chat PID parameter
    var _rid = null; //open chat RID parameter
    var _chatBarContainer = null; //container for putting chat bar buttons
    
    var _mutedChats = []; //muted chats, groupID array

    _chat.client.relogin = function () {
        if (!_chatSetup) { _setupChat(); } //setup chat if it was never set up
        else { _login(); } //otherwise only need to call login
    }

    //area - place user is in (ex. 84, 57, null for TR)
    //areaId - playerId for that area (null for TR)
    //openChatPid - paramter in url to open a realm chat on login
    //openChatRid - parameter in url to open a realm chat on login for anonymous users
    function _startHub(area, areaId, chatBarContainer, openChatPid, openChatRid) {

        obj.area = _area = area;
        obj.areaId = _areaId = areaId;
        _rid = openChatRid;
        _pid = openChatPid;
        _chatBarContainer = chatBarContainer;

        $.connection.hub.logging = false; //does all the console logging
        //this start function is where signalr sets up the websocket connection and gives the client a connectionId to the server
        $.connection.hub.start().done(function () {
            //console.log('ChatHub - hub started: ' + $.connection.hub.transport.name);
            _setupChat();
        });

        //elaborate
        /*Without a manual call to stop the connection, Signalr detects disconnection by sending pings to the client every couple seconds for a response. 
         * If there is no response within 30 seconds, then it realizes the client has disconnected. However, this may not be ideal, because when users close the page,
         * it will take 30 seconds or more until they are actually logged out of the system. This method is a sort of hack to instantly disconnect the client when he or she closes the page*/
        window.onbeforeunload = function () {
            $.connection.hub.stop();
        };

        //Clients actually go into reconnecting state very often, but they reestablish their connection fairly quickly, so we don't need to do anything
        //$.connection.hub.reconnecting(function () {
        //    _reconnectWarning();
        //});

        //$.connection.hub.reconnected(function () {
        //    _login(true);
        //});

        /* SCENARIOS FOR DISCONNECTING AND RECONNECTING
            1. client disconnects for a small period of time (<30s) and reconnects 
                    - reconnecting and reconnected is called, but we don't need to do anything since nothing changes
            2. client disconnects for a longer period of time (>30s) and reconnects within a reasonable period (unsure exact time, but lets say ~1min) 
                    - reconnecting is called, and then disconnected, but then reconnected gets called after client gains connection again
                    - must re login user on reconnected as he or she was disconnected already
            3. client disconnects for a long period of time and reconnects much much later
                    - reconnnecting is called, and then disconnected, and then signalr stops pinging for connection
                    - as a result, on reconnection, nothing will happen, so we do another hack which tries to restart the connection every 5 seconds after disconnect
                        -therefore, on reconnection, within 5 seconds, Signalr will get called to start up again and login will get called, and everything will proceed normally
                        -NOTE: during the time frame of disconnected to before login is called, users may not send any messages or get any updates. Here, we may want to give them a warning that
                         they are being reconnected to the chat
        */
        //TODO give user msg that connection is slow
        $.connection.hub.disconnected(function () {
            _connectionStarted = false;
            //this is the part where we try to restart the hub every 5 seconds, and also where we would probably want to give the warning to user that he or she is in reconnecting state
            setTimeout(function () {
                $.connection.hub.start().done(function () {
            _connectionStarted = true;
                    if (!_chatSetup) { _setupChat(); } //setup chat if it was never set up
                    else { _login(); } //otherwise only need to call login
                });
            }, 5000); // Restart connection after 5 seconds.
        });
    };

    function _setupChat() {
        _connectionStarted = true;
        //add the chatcontainer, chatlist, and contextmenus to the body
            $('body').append('<div id="chatContainer">' +
                                '<div class="chatWindow" id="chatList" >' +
                                     '<div class="titleBar">' +
                                        '<div class="headerL"></div><div class="headerM"></div><div class="headerR"></div><div class="headerOverlay"></div>' +
                                        '<div class="chatIcon chatType"></div>' +
                                        '<div class="fontSilverFrLCXlrg chatName">Chat List</div>' +
                                        '<div class="chatButton minimize"></div>' +
                                    '</div>' +
                                    '<div class="listItems"></div>' +
                                '</div>' +
                             '</div>' +

                            '<nav class="contextMenu" id="userMenu">' +
                                '<div id="viewPlayerProfile" class="menuItem">View Profile</div>' +
                                '<div id="chatUser" class="menuItem">Chat</div>' +
                                '<div id="blockUser" class="menuItem">Block</div>' +
                                '<div id="unblockUser" class="menuItem">Unblock</div>' +
                                '<div id="viewThrone" class="menuItem">View Throne</div>' +
                                '<div id="kickUser" class="menuItem">Kick</div>' +
                            '</nav>' +
                            
                            '<nav class="contextMenu" id="chatMenu">' +
                                '<div id="muteChat" class="menuItem">Mute</div>' + //will dynamically be Mute or Unmute
                                '<div id="changeName" class="menuItem">Change Name</div>' +
                                '<div id="leaveGroup" class="menuItem">Leave Group</div>' +
                            '</nav>' +

                            '<div id="chatDialog" class="popupDialogs"></div>');
        //case when we are in realm
            if (_area != null) {
                //add special global and clan chat container 
                var chatContainer = $('#chatContainer').append('<div id="globalAndClanChat" class="globalAndClanChat">' +
                                                '<div class="globalAndClanChatBars">' +
                                                    '<div id="chatListBtn" class="chatButton unselectable helpTooltip" data-toolTipID="chatListBtn">' +
                                                        '<div class="icon"></div>' +
                                                        '<div class="notableNotifications"></div>' +
                                                    '</div>' +
                                                '</div>' +
                                                '<div class="handle"></div>' +
                                                '<div class="list"></div>' +
                                            '</div>');
               
                //to minimize maximise the inrealm chatcontainer
                $('#globalAndClanChat .handle', chatContainer).click(function (event) {
                    ROE.UI.Sounds.click();
                    chatContainer.toggleClass('large mini');
                    chatContainer.hasClass('large') ? $('#PromoWrapper').hide() : $('#PromoWrapper').show();
                    
                    $('#globalAndClanChat .chatWindow').each(function () {
                        ROE.UI.Chat2.scrollBottom($(this));
                    });
                    event.stopPropagation();
                });

                //where there is a css transition, must do scroll after animation
                $('#globalAndClanChat').on("transitionend webkitTransitionEnd oTransitionEnd MSTransitionEnd", function (event) {
                    $('#globalAndClanChat .chatWindow').each(function () {
                        ROE.UI.Chat2.scrollBottom($(this));
                    });
                    event.stopPropagation();
                });

                chatContainer.mouseover(function () { $('.mapGuiPanel').stop().hide(); });
                chatContainer.on('click', '#globalAndClanChat .chatArea', function () {
                    ROE.UI.Sounds.click();
                    chatContainer.addClass('large').removeClass('mini');
                    chatContainer.hasClass('large') ? $('#PromoWrapper').hide() : $('#PromoWrapper').show();
                });

            } else {
                //add chatlist button
                _chatBarContainer.append('<div id="chatListBtn" class="BtnBLg1 fontButton1L chatBar chatButton unselectable helpTooltip" data-toolTipID="chatListBtn">' +
                                            '<div class="barName"></div>' + '<div class="icon"></div>' +
                                            '<span class="notification" style="display:none;"></span>' +
                                        '</div>');
            }
            _login();

        //generic chat dialog
            $('#chatDialog').dialog({
                autoOpen: false,
                modal: true,
                dialogClass: "chatDialog",
                open: function () {
                    if ($('body').hasClass('mobile')) {
                        $(this).dialog("option", "width", $(window).width());
                        $(this).dialog("option", "height", $(window).height());
                    }
                },
                create: function () {
                    if ($('body').hasClass('desktop')) {
                        var titleBar = $(this).parent().find('.ui-dialog-titlebar');
                        var titleBarElements = "<div class='roeStyleTitle'><div class='headerL'></div><div class='headerM'></div><div class='headerR'></div><div class='headerOverlay'></div></div>";
                        var dialogContentElements = "<div class='roeStyleContent'><div class='contentBody'></div><div class='contentLT'></div><div class='contentLM'></div><div class='contentLB'></div><div class='contentBottom'></div><div class='contentRT'></div><div class='contentRM'></div><div class='contentRB'></div></div>";
                        $(this).parent().prepend(dialogContentElements);
                        titleBar.prepend(titleBarElements);
                    }
                },
                close: function () {
                    $(this).empty();
                }
            });

        //setup bbcodes if the area is in a realm
            if (_area != null) {
                $('#chatContainer').addClass('mini inRealm').delegate('.bbcode_p', 'click', function (event) {
                    ROE.Frame.popupPlayerProfile($(event.target).html());
                }).delegate('.bbcode_v', 'click', function (event) {
                    ROE.Frame.popupVillageProfile($(event.target).attr('data-vid'));
                }).delegate('.bbcode_c', 'click', function (event) {
                    ROE.Frame.popupClan($(event.target).attr('data-cid'));
                }).delegate('.fp', 'click', function (event) {
                    ROE.Frame.popupClan(undefined, $(event.target).attr('data-fpid'), "forumpost"); // launch clan popup with the forum already opened
                });
            }

        _chatSetup = true;
    }

    function _setName(newName) {
        if (_connectionStarted) {
            _chat.server.setName(newName);
        }
    };
    function _joinRealm(realmId, playerId) {
        if (_connectionStarted) {
            _chat.server.joinRealm(realmId, playerId);
        }
    };
    function _getNotifications(realmId, playerId, callback) {
        if (_connectionStarted) {
            _chat.server.getNotifications(realmId, playerId).done(callback);
        }
    };

    function _login() {
        if (!_callingLogin) {
            _callingLogin = true;
            //sets up client/player and all its chats that should be opened
            _chat.server.login(_area, _areaId, _pid, _rid).done(_loginComplete).fail(function () {
                _callingLogin = false;
            });
        }
    }

    //results contains the list of chats to be opened, as well as a datetimenow as earliest message date (earliestmsgdate is used to load older msgs before this date)
    function _loginComplete(results) {
        var settings = {
            container: _chatBarContainer,
        };
        if (results) {
            settings.earliestMsgDate = results.date;
            
            //get muted chats data "groupid,groupid,groupid" etc, and turn into array
            ROE.LocalServerStorage.init(results.LocalStorageOnServer);
            if (ROE.LocalServerStorage.getGlobal('mutedChats')) {
                _mutedChats = ROE.LocalServerStorage.getGlobal('mutedChats').split(',');
            }
            
            for (var i = 0; i < results.chats.length; i++) {
                var currChat = results.chats[i];
                settings.realmId = currChat.realmId;
                settings.id = currChat.id;
                settings.groupId = currChat.groupId;
                settings.type = currChat.type;
                settings.chatName = currChat.name;
                //only create window if it doesn't exist already and is not muted
                
                if ((settings.realmId == "Global" && settings.type == 0 ) || //TR 'universal' global chat
                    (_area != null && (settings.type == 0 || settings.type == 3)) && //in-realm and (realmchat or clanchat)
                    !$('#group' + settings.groupId + settings.id).length && //already existing
                    //!$.cookie('mutedChat' + currChat.groupId + (_areaId || '') + currChat.id)) { //muted chat
                    !_isChatMuted(currChat.groupId)){ //and not not muted
                    $.when(ROE.UI.Chat2.init(settings), currChat.realmId, currChat.id, currChat.groupId, currChat.notifs, currChat.type).then(_chatInitComplete);
                }
            }

            //if inrealm chat we wana select a tab by default
            if (_area != null) {

                //try clan chat first then realm chat
                //a bit of a crude way, but ensures element exists so chatarea isn't empty
                if ($('.globalAndClanChatBars .chatBar.chatButton[data-type="3"]').length) {
                    $('.globalAndClanChatBars .chatBar.chatButton[data-type="3"]').click();
                } else {
                    $('.globalAndClanChatBars .chatBar.chatButton[data-type="0"]').click();
                }

            }

            $('#chatListBtn').unbind('click').click(function (e) {
                var chatList = $('#chatList').toggleClass('displayed');
                chatList.position({
                    my: 'right bottom',
                    at: 'right top',
                    of: $('#chatListBtn')
                });
                //ROE.UI.Sounds.click();
                //$('.mobile #chatContainer').removeClass('mini').addClass('large');
                
                e.stopPropagation();
            });

            _setupChatListWindow();
            _updateChatList();
        } else {
            console.log('Error in calling login: bad parameters');
        }
        _callingLogin = false;
    }

    function _chatInitComplete(newChatWindow, realmId, id, groupId, notifs, type) {
        var window = newChatWindow.window;
        if (notifs > 0) {
            newChatWindow.bar.find('.notification').text(notifs).show();
            newChatWindow.bar.find('.barName').css('width', '72px');
        }
  
        ROE.UI.Chat2.scrollBottom(window);
        /*This call to server to update users is only for the this client. Nobody else gets their users updated because they were already updated before:
            Upon login, this player gets added to these chats and after the addition, the server updates all users for all the other clients in the same chat, but since the chat window is not created yet
            for this user, nothing will happen, so we must manually call updateUsers only on self after the window is created. */
        _chat.server.updateUsers(groupId, id);

        if (realmId == _rid || id == _pid) {
            var body = $('body');
            window.css({
                left: (body.width() - window.width()) / 2,
                top: (body.height() - window.height()) / 2
            });
        }

    }


    //start oneononechat
    /*
    function _oneOnOneChat(realmId, id, otherId) {
        if (_connectionStarted) {
            _chat.server.oneOnOneChat(realmId, id, otherId, 'popup');
        }
    }
    */

    //function _reconnectWarning() {
    //    $('#chatContainer').empty();
    //    var btmPanel = $('#chatBarContainer');
    //    btmPanel.empty();
    //    btmPanel.append($('<div id="lostConnection"><div>Chat lost connection. Attempting to reconnect...</div><div>Click here to refresh the page.</div></div>').click(function () { location.reload(); }));
    //}

    function _updateChatList() {

        _chat.server.getAllChats().done(function (results) {

            var chatSection = $('#chatList').find('.listItems').empty();
            var result;
            var realmSet;
            var realmLabel;

            //EACH RESULT IS A REALMSET
            for (var i = 0; i < results.length; i++) {
                result = results[i];
                var isOpened = $.cookie(result.realmId + result.id);

                realmSet = $('<div class="realmSet" data-id="' + result.id + '" data-realmId="' + result.realmId + '"></div>');
                realmLabel = $('<div class="realmLabel">' +
                                    '<div class="realmName">' + result.realmLabel + '</div>' +
                                '</div>').click(function () {
                                    var elem = $(this);
                                    var parent = elem.parent();
                                    var cookieId = parent.attr('data-realmId') + parent.attr('data-id');
                                    if (elem.hasClass('closed')) { //when its closed, we open it
                                        elem.removeClass('closed').siblings().slideDown(200);
                                        elem.find('.arrowIcon').text('▲');
                                        $.cookie(cookieId, true);   //save cookie for this subsection to be opened
                                    } else {
                                        elem.addClass('closed').siblings().slideUp(200);
                                        elem.find('.arrowIcon').text('▼');
                                        $.cookie(cookieId, ''); //remove the cookie so the subsection will be closed
                                    }
                                });

                //Realmset notification
                var realmLabelNotification = $('<span class="notification"  >' + result.chatsWithNotifications + '</span>');
                realmLabel.append(realmLabelNotification);
                realmSet.append(realmLabel);

                //btn CREATE-NEW-CHAT
                var newChatBtn = $('<div class="BtnBXLg1 fontButton1L chatListButtons createGroupBtn"><div class="icon"></div>Create a Chat</div>').click(_createNewChat);
                realmSet.append(newChatBtn);


                var chat;
                var chatLabel;

                //INDIVIDUAL CHAT ENTRIES IN A REALMSET
                for (var j = 0; j < result.chats.length; j++) {
                    chat = result.chats[j];

                    //if this chat is type clan or global, we only show it in TR's chatlist, since its unclosable in realm
                    if (_area == null || chat.type == 2) {   
                        chatLabel = $('<div class="chatLabel" id="history' + chat.groupId + result.id + '" data-groupId="' + chat.groupId + '" data-groupType="' + chat.type + '" data-name="' + chat.name + '">' +
                                                '<div class="typeIcon"></div>' +
                                                '<div class="labelName">' + chat.name + '</div>' +
                                                '<div class="icon mute"></div>' +
                                                '<div class="icon gear"></div>'+
                                        + '</div>').click(function () {
                                            var elem = $(this);
                                            _chatListChatClick(elem.attr('data-groupId'), elem.parent().attr('data-id'));
                                            _chat.server.joinChatInHistory(elem.attr('data-groupId'), elem.parent().attr('data-realmId'), elem.parent().attr('data-id'), elem.attr('data-name'), elem.attr('data-groupType'));
                                            _closeChatlistIfM();
                                        });

                        //SETUP GEAR ICON
                        var gearIcon = chatLabel.find('.gear');
                        gearIcon.click(function (e) {

                            var chatLabelElement = $(this).parent();
                            var groupType = chatLabelElement.attr('data-groupType');
                            var groupId = chatLabelElement.attr('data-groupId');
                            var id = chatLabelElement.parent().attr('data-id');

                            //ADD CHAT MENU
                            var chatMenu = $('#chatMenu');
                            chatMenu.css({ 'left': e.clientX - (chatMenu.width() + 15), 'top': e.clientY })
                            .fadeIn(100, function () {
                                $(document).bind('click', _hideChatContextMenu);
                            }).children().hide();

                            $('#muteChat', chatMenu).html(_isChatMuted(groupId) ? 'Unmute' : 'Mute')
                                .show().unbind('click').click(function () {
                                    _toggleChatMute(groupId, id);
                                });

                            //LEAVE OPTION FOR GROUP CHATS
                            if (groupType == 2) {
                                $('#leaveGroup', chatMenu).show().unbind('click').click(function () {
                                    _chat.server.leaveGroup(chatLabelElement.attr('data-groupId'), chatLabelElement.parent().attr('data-id'));
                                    ROE.UI.Chat2.closeWindow(chatLabelElement.attr('data-groupId'), chatLabelElement.parent().attr('data-id'));
                                });
                                $('#changeName', chatMenu).show().unbind('click').click(function () {
                                    ROE.UI.Chat2.changeGroupNameBtnClick(chatLabelElement.attr('data-groupId'), chatLabelElement.parent().attr('data-id'));
                                });
                            }

                            e.stopPropagation();

                        });

                        //add notifications for individual chats, regardless of mute status
                        if (chat.notifications > 0) {
                            chatLabel.prepend('<span class="notification">' + chat.notifications + '</span>');
                        } else {
                            chatLabel.prepend('<span class="notification" style="display:none;"></span>');
                        }

                        //add muted class to label if it is muted
                        if (_isChatMuted(chat.groupId)) {
                            chatLabel.addClass('muted');
                        } else if (chat.notifications > 0) {
                            //if the chat isnt muted, AND has notifications, then also show the Realmset's notification as well
                            realmSet.addClass('notable');
                        }

                        if (chat.name == "New Chat") {
                            chatLabel.insertAfter(newChatBtn);
                        } else {
                            realmSet.append(chatLabel);
                        }
                        
                    }

                }

                //LATEST CREATED CHAT GROUP ID
                //ROE.Chat2.newestCreatedChatGroupID = groupId;
                //can be used maybe for a better way to pin new chats to top

                //SORT CHATS IN REALMSET
                if (result.chatsWithNotifications) {
                    
                    realmSet.find('.chatLabel').sort(function (a, b) {

                        var $labelA = $(a);
                        var notifCountA = 0;
                        var nameA = $labelA.attr('data-name');
                        if (nameA == "New Chat") {
                            notifCountA = 3;
                        } else {
                            notifCountA = parseInt($labelA.find('.notification').text());
                            if (notifCountA > 0) {
                                if ($labelA.hasClass('muted')) {
                                    notifCountA = 1; //has notification but is muted
                                } else {
                                    notifCountA = 2; //has notification
                                }
                            } else {
                                notifCountA = 0
                            }
                        }

                        var $labelB = $(b);
                        var notifCountB = 0;
                        var nameB = $labelB.attr('data-name');
                        if (nameB == "New Chat") {
                            notifCountB = 3;
                        } else {
                            notifCountB = parseInt($labelB.find('.notification').text());
                            if (notifCountB > 0) {
                                if ($labelB.hasClass('muted')) {
                                    notifCountB = 1; //has notification but is muted
                                } else {
                                    notifCountB = 2; //has notification
                                }
                            } else {
                                notifCountB = 0
                            }
                        }


                        return notifCountA < notifCountB ? 1 : notifCountA > notifCountB ? -1 : 0;

                    }).appendTo(realmSet);
                }

                
                if (isOpened) {
                    realmLabel.prepend('<span class="arrowIcon">▲</span>');
                } else {
                    realmLabel.prepend('<span class="arrowIcon">▼</span>').addClass('closed');
                    realmSet.children().not('.realmLabel').hide();
                }
                chatSection.append(realmSet);

                //remove the realmlabel for in realm stuff
                if (_area != null) {
                    chatSection.attr('data-id', result.id).attr('data-realmId', result.realmId);
                    realmSet.children().show().unwrap();
                    realmLabel.hide();
                }
            }


            //SORT REALMSET CHATS
            //ChatList Icon notifcations
            var realmSets = chatSection.find('.realmSet');

            realmSets.sort(function (a, b) {

                var $setA = $(a);
                var levelA = 0;
                if($setA.attr('data-realmid') == "Global"){
                    levelA = 2;
                }else if($setA.hasClass('notable') ){
                    levelA = 1;
                }

                var $setB = $(b);
                var levelB = 0;
                if ($setB.attr('data-realmid') == "Global") {
                    levelB = 2;
                } else if ($setB.hasClass('notable')) {
                    levelB = 1;
                }

                return levelA < levelB ? 1 : levelA > levelB ? -1 : 0;
                
            }).appendTo(chatSection);
          
            _updateDocumentTitleNotifications();

        });

    }
    
    //looks at dictionary of muted chats, key is chat groupid
    function _isChatMuted(groupId){
        return _mutedChats.indexOf(groupId) > -1;
    }

    function _toggleChatMute(groupId, id) {
        if (_isChatMuted(groupId)) {
            _unMuteChatGroupID(groupId); //unmute
        } else {
            _mutedChats.push(groupId); //mute
        }
        var chatLabel = $('.chatLabel[data-groupid="' + groupId + '"]').toggleClass('muted', _isChatMuted(groupId));
        ROE.LocalServerStorage.setGlobal('mutedChats', _mutedChats.join());
        _chat.server.toggleMuteChat(groupId, id);
    }

    function _unMuteChatGroupID(groupId) {
        for (var i = _mutedChats.length - 1; i >= 0; i--) {
            if (_mutedChats[i] === groupId) {
                _mutedChats.splice(i, 1);
            }
        }
    }

    //when a chat line in chatlist is clicked, we do this first
    //go through all open windows, and close non pinned ones
    function _chatListChatClick(gid, id) {

        $('.chatWindow').each(function () {
            var thisChat = $(this).removeClass('chatFlash');
            if(thisChat.is('#group' + gid + id) && thisChat.is('.chatWindow.pinned')){
                /*
                var chatListWindowPos = $('#chatList').position();
                thisChat.css({
                    position: 'absolute',
                    left: chatListWindowPos.left - thisChat.width(),
                    top: chatListWindowPos.top
                });
                */
                thisChat.addClass('chatFlash');
                setTimeout(function () { thisChat.removeClass('chatFlash'); }, 1000);
            } else if (!(thisChat.is('#chatList') || thisChat.is('#globalAndClanChat .chatWindow') || thisChat.is('.chatWindow.pinned'))) {
                var chatData = $(this).data();
                ROE.UI.Chat2.closeWindow(chatData.groupId,chatData.id);
            } 
        });

    }

    /*
    function _create1on1BtnClick() {
        var rid = $(this).parent().attr('data-realmId');
        var id = $(this).parent().attr('data-id');
        var content = $('<div>' +
                            '<div class="chatPopupLabel">Please enter the player name to chat with:</div>' +
                        '</div>');
        var input = $('<input id="addGroupPlayer" type="text"/>');
        var autocompleteBox = $('<div class="autoCompleteBox"></div>');
        _autocompletePlayer(input, autocompleteBox, rid, false);
        content.append(input);
        content.append($('<div class="BtnBLg1 fontButton1L chatPopupBtn oneOnOneChatBtn">Start</div>').click(function () {
            _chat.server.oneOnOneChatByName(rid, id, input.val()).done(function (result) {
                if (result.success) {
                    $('#chatDialog').dialog('close');
                    _closeChatlistIfM();
                } else {
                    $('#chatDialogNotif').empty().append('<div class="badNotif">' + result.error + '</div>');
                }
            });
        }));
        content.append('<div id="chatDialogNotif"></div>');
        content.append(autocompleteBox);
        _popupChatDialog('Add Players', content,400,400);
    }
    */

    /*
    function _createGroupBtnClick() {
        var content = $('<div>' +
                            '<div class="createGroupLabel">Please enter a name for the group:</div>' +
                            '<input id="groupChatName" type="text" maxlength="256"/> ' +
                        '</div>');
        content.append($('<div class="BtnBLg1 fontButton1L chatPopupBtn okBtn" data-id="' + $(this).parent().attr('data-id') + '" data-realmId="' + $(this).parent().attr('data-realmId') + '">Create</div>').click(function () {
            _chat.server.createGroupChat($('#groupChatName').val(), $(this).attr('data-realmId'), $(this).attr('data-id')).done(function (result) {
                if (result.success) {
                    $('#chatDialog').dialog('close');
                    _closeChatlistIfM();
                } else {
                    $('#chatDialogNotif').empty().append('<div class="badNotif">' + result.error + '</div>');
                }
            });
        }));
        content.append('<div id="chatDialogNotif"></div>');
        _popupChatDialog('Create Group', content);
    }
    */

    function _createNewChat() {

        var listItem = $(this).parent();
        _chat.server.createGroupChat('New Chat', listItem.attr('data-realmId'), listItem.attr('data-id')).done(function (result) {
            if (result.success) {
                _closeChatlistIfM();
            } 
        });
        

        /* 
        var content = $('<div>' +
                            '<div class="createGroupLabel">Please enter a name for the group:</div>' +
                            '<input id="groupChatName" type="text" maxlength="256"/> ' +
                        '</div>');
        content.append($('<div class="BtnBLg1 fontButton1L chatPopupBtn okBtn" data-id="' + $(this).parent().attr('data-id') + '" data-realmId="' + $(this).parent().attr('data-realmId') + '">Create</div>').click(function () {
            _chat.server.createGroupChat($('#groupChatName').val(), $(this).attr('data-realmId'), $(this).attr('data-id')).done(function (result) {
                if (result.success) {
                    $('#chatDialog').dialog('close');
                    _closeChatlistIfM();
                } else {
                    $('#chatDialogNotif').empty().append('<div class="badNotif">' + result.error + '</div>');
                }
            });
        }));
        content.append('<div id="chatDialogNotif"></div>');
        _popupChatDialog('Create Group', content);
        */
       

    }


    function _closeChatlistIfM() {
        if ($('body').hasClass('mobile')) {
            $('#chatList').removeClass('displayed');
        }
    }

    function _updateDocumentTitleNotifications() {
        //show how many unread chats on title
        var allChatsWithNotifs = 0;
        $('.notification', _chatBarContainer).each(function () {
            if ($(this).text() > 0) {
                allChatsWithNotifs++;
            }
        });
        document.title = document.title.replace(/\(.*\)/, '');
        if (allChatsWithNotifs > 0) {
            document.title = document.title + ' (' + allChatsWithNotifs + ')';
        }

        //ChatList Icon notifcations
        var notableNotifications = 0; //certain notifications only, to show in chatlist icon
        $('#chatList .chatLabel .notification').each(function () {
            if ($(this).text() > 0 && !($(this).parent().hasClass('muted'))) {
                notableNotifications++;
            }            
        });

        if (notableNotifications > 0) {
            $('#chatListBtn .notableNotifications').html(notableNotifications).show();
        } else {
            $('#chatListBtn .notableNotifications').hide();
        }
        
    }

    function _setupChatListWindow() {
        var listWindow = $('#chatList');
        listWindow.addClass('dragVirgin').mousedown(function () {
            var prevActiveWindow = $('#chatContainer').find('.active');
            if (prevActiveWindow.length) {
                prevActiveWindow.removeClass('active');
                _chat.server.resetNotifications(prevActiveWindow.data('groupId'), prevActiveWindow.data('realmId'), prevActiveWindow.data('id'), false);
                _chat.server.setLastActive(null, null, null);
            }
        });

        if ($('body').hasClass('desktop')) {
            listWindow.draggable({
                handle: '.titleBar',
                containment: 'body',
                stop: function () { $(this).removeClass('dragVirgin'); }
            })
            .resizable({
                handles: 'all',
                minWidth: 250,
                minHeight: 150,
                containment: 'body',
                resize: _fixResize
            }).css('width', 250);
        }

        $('.minimize', listWindow).click(function () {
            $('#chatList').removeClass('displayed');
        });
        
    }

    function _joinOrCreateClanChat(clanId) {
        _chat.server.createOrJoinClanChat(clanId, _area).done(function () {
            $('.globalAndClanChatBars .chatBar.chatButton[data-type="3"]').click();
        });       
    }

    function _deleteClanChat(clanId) {
        _chat.server.nukeClanChat(clanId, _area).done(function () {
            $('.globalAndClanChatBars .chatBar.chatButton[data-type="0"]').click();
        });
    }

    function _dismissClanChat(playerId, clanId) {
        _chat.server.removeClanChat(playerId, clanId, _area).done(function () {
            $('.globalAndClanChatBars .chatBar.chatButton[data-type="0"]').click();
        });
    }

    function _leaveClanChat(clanId) {
        _chat.server.removeClanChat(null, clanId, _area).done(function () {
            $('.globalAndClanChatBars .chatBar.chatButton[data-type="0"]').click();
        });         
    }

    //used on resize events of chat windows
    function _fixResize(event, ui) {
        //fixes issue of resize snapping chatwindow back to corner
        var diffW = ui.size.width - ui.originalSize.width;
        var diffH = ui.size.height - ui.originalSize.height;
        var elem = $(this);
        var handle = elem.data('ui-resizable').axis;
        switch (handle) {
            //all these different directions of resizing, have to change it slightly so that it resizes towards the right direction
            case 'nw':
                elem.css('left', ui.originalPosition.left - diffW);
                elem.css('top', ui.originalPosition.top - diffH);
                break;
            case 'ne':
                elem.css('left', ui.originalPosition.left);
                elem.css('top', ui.originalPosition.top - diffH);
                break;
            case 'sw':
                elem.css('left', ui.originalPosition.left - diffW);
                elem.css('top', ui.originalPosition.top);
                break;
            case 'se':
                elem.css('left', ui.originalPosition.left);
                elem.css('top', ui.originalPosition.top);
                break;
            case 'n':
                elem.css('left', ui.originalPosition.left);
                elem.css('top', ui.originalPosition.top - diffH);
                break;
            case 'e':
                elem.css('left', ui.originalPosition.left);
                elem.css('top', ui.originalPosition.top);
                break;
            case 's':
                elem.css('left', ui.originalPosition.left);
                elem.css('top', ui.originalPosition.top);
                break;
            case 'w':
                elem.css('left', ui.originalPosition.left - diffW);
                elem.css('top', ui.originalPosition.top);
                break;
        }
    }

    function _popupChatDialog(title, content, w, h) {
        var dialog = $('#chatDialog').empty();
        w = w || 350;
        h = h || 'auto';
        dialog.append($(content).addClass('genericDialogContent'));
        dialog.dialog("option", "title", title)
        dialog.dialog("option", "width", w);
        dialog.dialog("option", "height", h);
        dialog.dialog('open');
    }

    function _hideChatContextMenu() {
        $('#chatMenu').hide(100);
        $(document).unbind('click', _hideChatContextMenu);
    }

    function _autocompletePlayer(input, autocompleteContainer, realmId, isGroup) {
        function split(val) {
            return val.split(/,\s*/);
        }
        function extractLast(term) {
            return split(term).pop();
        }

        input.keyup(function () {
            var term = extractLast(input.val());
            $('#chatDialogNotif').empty();
            if (term.length < 3) {
                autocompleteContainer.empty();
                return false;
            }
            _chat.server.playerAutocomplete(term, realmId).done(nameResponse);
        })
        .blur(function () {
            //setTimeout(function () { autocompleteContainer.empty() }, 100);
        });

        function nameResponse(r) {
            autocompleteContainer.empty();
            var terms;
            var ids;
            $.each(r, function (i, n) {
                autocompleteContainer.append($('<div data-name="' + n.label + '">' + n.label + '</div>').click(function () {
                    terms = split(input.val());
                    // remove the current input
                    terms.pop();
                    // add the selected item
                    terms.push($(this).attr('data-name'));

                    // add placeholder to get the comma-and-space at the end
                    if (isGroup) { terms.push(""); }
                    input.val(terms.join(", "))
                    autocompleteContainer.empty();
                    input.focus();
                }));
            });
        }
    }


    function _adminBroadcast(adminMessage) {
        _chat.server.adminBroadcast(adminMessage);
    }

    obj.chat = _chat
    obj.startHub = _startHub;
    obj.setName = _setName;
    obj.joinRealm = _joinRealm;
    obj.getNotifications = _getNotifications;
    obj.login = _login;
    //obj.oneOnOneChat = _oneOnOneChat;
    obj.fixResize = _fixResize;
    obj.updateChatList = _updateChatList;
    obj.popupChatDialog = _popupChatDialog;
    obj.joinOrCreateClanChat = _joinOrCreateClanChat;
    obj.deleteClanChat = _deleteClanChat;
    obj.dismissClanChat = _dismissClanChat;
    obj.leaveClanChat = _leaveClanChat;
    obj.autocompletePlayer = _autocompletePlayer;
    obj.updateDocumentTitleNotifications = _updateDocumentTitleNotifications;

    obj.adminBroadcast = _adminBroadcast;
    obj.mutedChats = _mutedChats;
    obj.isChatMuted = _isChatMuted;

}(window.ROE.Chat2 = window.ROE.Chat2 || {}));