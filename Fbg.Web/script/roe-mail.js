(function (ROE) {
}(window.ROE = window.ROE || {}));

(function (obj) {

    var _container;
    var _viewingMail;
    var fromAjaxLoadedQuickerThanFromCache = 0; // 0 means do nothing.
    var fromAjaxLoadedQuickerThanFromSentCache = 0;
 
    var _lockOutTabSwitch;
    var _lastItemsDeleted = [];
    var _activeListSelector = ".list";
    var _filterOn = false;
    var _starredOn = false;

    // _currentTabIndex indices: 0 = create, 1 = inbox, 2 = outbox, 3 = block
    var _currentTabIndex = 1;
    var _totalChecked = 0;
    var _sentTotalChecked = 0;
   
    var _numItemsInFilter;
    var _alreadyLoaded = false;

   
    var _waitingForDeleteConfirm = false;
    var _confirmDeleteTimer;

    var _waitingForDeleteSelectedConfirm = false;
    var _confirmDeleteSelectedTimer;

    var CONST = {
        pageLength: 10,
        tableName: 'Mail',
        tableFullName : '',  // will be set in init 
        sentTableName: 'SentMail',
        sentTableFullName: '',  // will be set in init 
        staticUrl: 'https://static.realmofempires.com/images/'
    };

    CONST.TabEnum = { 
        create: 0,
        inbox: 1,
        outbox: 2,
        block: 3
    };

    CONST.Selector = {
        moreButton: "#mail_popup .more", // won't work in this context cause we have multiple lists with same more
        inbox: ".list",
        outbox: ".sentList",
        block: ".block",
        create: ".create1step",
        tabCreate: ".mailTabBtn.create",
        tabInbox: ".mailTabBtn.inbox",
        tabOutbox: ".mailTabBtn.outbox",
        tabBlock: ".mailTabBtn.block"
    };

    CONST.CSSClass = {
        listItemSeparator: "mailseparator"
    };

    CONST.LocalVars = {
        listLoaded: "MailListLoaded",
        sentListLoaded: "SentMailListLoaded"
    };

    obj.init = function (container, thereIsNewMail, prefilledMessage) {
        _container = container;
        CONST.tableFullName = CONST.tableName + '_' + BDA.Database.id();
        CONST.sentTableFullName = CONST.sentTableName + '_' + BDA.Database.id();

        if (ROE.isMobile || !_alreadyLoaded || thereIsNewMail) {

            // Grab the base template.
            if (ROE.isMobile) {
                _container.empty().append(BDA.Templates.getRawJQObj("MailPopup", ROE.realmID));
            } else {
                _container.empty().append(BDA.Templates.getRawJQObj("MailPopup_d2", ROE.realmID));
            }

            // Setup the tabs
            _lockOutTabSwitch = false;
            $('#mail_listtabs .inbox', _container).click(_gotoInbox);
            $('#mail_listtabs .outbox', _container).click(_gotoOutbox);
            $('#mail_listtabs .create', _container).click(_mailCreate);
            $('#mail_listtabs .block', _container).click(_mailBlock);
            _activeListSelector = ".list"; // Initially set to inbox.

            // List buttons
            $('.list .listTools .reload', _container).click(_handleReloadBtn);
            $('.sentList .listTools .reload', _container).click(_handleSentReloadBtn);
            $('.list .more', _container).click(_loadMore);
            $('.sentList .more', _container).click(_loadMoreSent);

            // Mail viewer buttons
            $('.detail .from', _container).click(_userNameClick);
            $('.detail .next', _container).click(_detailMove);
            $('.detail .prev', _container).click(_detailMove);
            $('.detail .reply', _container).click(_detailReplyStart);
            $('.detail .replyall', _container).click(_detailReplyAllStart);
            $('.detail .forward', _container).click(_detailForwardStart);
            $('.detail .delete', _container).click(_detailDelete);
            $('.doblock', _container).click(_blockUser);

            $('.action-back', _container).click(_mailBackToListFromDetail);

            $('.clearFilterBtn', _container).hide();
            $('.filterTerm', _container).hide();
            $('.hideDuringFilter', _container).show();

            // Setup auto-complete
            _autocomplete();
            $('.create1step .action-save', _container).click(_createSend);

            // Setup the broadcaster to track new mail
            BDA.Broadcast.subscribe(_container, "NewMail", _handleNewMailBroadcast);

          
            

            //
            // Load up the inbox data
            //
            try {
                // note, for situations where websql and indexeddb does not exist, this will result in no table, and will force the reload of mail 
                //  on first load and is necessary for reports to work properly
                if (!BDA.Database.DoesTableExist(CONST.tableFullName)) {
                    // The table doesn't so need to rebuild
                    BDA.Database.LocalDel('MailListLoaded');
                }
            } catch (e) {
                // Ignore, database is probably undefined
                BDA.Database.LocalDel('MailListLoaded');
            }
            fromAjaxLoadedQuickerThanFromCache = 1;
            _loadCache(0).done(_initLoadFromCache);

            if (!BDA.Database.LocalGet('MailListLoaded') || thereIsNewMail) {
                _reload();
            }

            //
            // load up the sent box data
            //
            try {
                // note, for situations where websql and indexeddb does not exist, this will result in no table, and will force the reload of mail 
                //  on first load and is necessary for reports to work properly
                if (!BDA.Database.DoesTableExist(CONST.sentTableFullName)) {
                    // The table doesn't so need to rebuild
                    BDA.Database.LocalDel('SentMailListLoaded');
                }
            } catch (e) {
                // Ignore, database is probably undefined
                BDA.Database.LocalDel('SentMailListLoaded');
            }
            fromAjaxLoadedQuickerThanFromSentCache = 1;
            _loadSentCache(0).done(_initLoadFromSentCache);

            if (!BDA.Database.LocalGet('SentMailListLoaded')) {
                _reloadSent();
            }

            _alreadyLoaded = true;
        }

        // Will open to create message with filled in details.
       if (prefilledMessage) {
           _createPrefilledMessage(prefilledMessage);
       }
    }
    
    function _userNameClick(e) {
        e.preventDefault();
        ROE.Frame.popupPlayerProfile($(this).html());
    }

    function _reload() {
  
        BDA.Console.verbose('ROE.Mail', 'reloading inbox');
        ROE.Frame.busy('Checking the Mail Bag...', 5000, _container);
        _totalChecked = 0;

       $('#mail_popup .listTools .reload').removeClass('effect-pulse newIndicator-round');
       $('.ui-buttonpanel-main .mail').toggleClass('newMail effect-pulse', false);
    
        ROE.Api.call('mail', {}, _list_onAjaxDataSuccess);
        $('.more', _container).attr('offset', 0);
       
       
    }

    function _reloadSent() {
        
        ROE.Frame.busy('Checking the Mail Bag...', 5000, _container);
     
        _sentTotalChecked = 0;
        ROE.Api.call('mail_sent', {}, _sentList_onAjaxDataSuccess);
        $('.sentList .more', _container).attr('offset', 0);
    }


    function _handleReloadBtn() {
        if(!ROE.isMobile)
            ROE.UI.Sounds.click();
        $(_activeListSelector + ' .deleteSelectedBtn').removeClass("active");
        $(_activeListSelector + " .filterStarredOnlyBtn").removeClass("active");
        _reload();
        //_clearFilter();
        _clearStarredFilter();
    }

    function _handleSentReloadBtn() {
        if (!ROE.isMobile)
            ROE.UI.Sounds.click();
        $(_activeListSelector + ' .deleteSelectedBtn').removeClass("active");
        $(_activeListSelector + " .filterStarredOnlyBtn").removeClass("active");
        _reloadSent();
        _clearFilter();
    }

    function _handleNewMailBroadcast(event) {
        BDA.Console.verbose('ROE.Mail', 'new mail broadcast received');           
        $('#mail_popup .listTools .reload').addClass('effect-pulse newIndicator-round');        
    }

    function _blockUser() {
        ajax("ChatBlockAjax.aspx", { block: $(this).parent().find('.from').html() }, function (obj) {
        });
    }

    function _initLoadFromCache(r) {
        if (fromAjaxLoadedQuickerThanFromCache == 2) {
            fromAjaxLoadedQuickerThanFromCache = 0; return;
        }
        _populate(r);
    }

    function _autocomplete() {
        function split(val) {
            return val.split(/,\s*/);
        }
        function extractLast(term) {
            return split(term).pop();
        }

        $(".create1step .to", _container)
            .keyup(function () {
                var v = $(this).val();
                var term = extractLast(this.value);
                if (term.length < 3) { return false; }
                $.getJSON("NamesAjax.aspx?what=playerNamesWClan", { term: term }, nameResponse);
            })
            .blur(function () {
                setTimeout(function () { $(".create1step .autocomplete", _container).empty() }, 100);
            });

        function nameResponse(r) {
            var ac = $(".create1step .autocomplete", _container).empty();
            var to = $(".create1step .to", _container);
            var terms;
            $.each(r, function (i, n) {
                ac.append($('<div v="' + n.value + '">' + n.label + '</div>').click(function autocompleteSelect() {                  
                    terms = split(to.val());
                    // remove the current input
                    terms.pop();
                    // add the selected item
                    terms.push($(this).attr('v'));
                    // add placeholder to get the comma-and-space at the end
                    terms.push("");
                    to.val(terms.join(", "));
                    $(".create1step .autocomplete", _container).empty();
                    to.focus().click();
                }));
            });            
        }
    }

    function _mailCreate() {
       
        if ($(this).hasClass('selected') || _lockOutTabSwitch) return;
        _lockOutTabSwitch = true;
        _selectTab(CONST.Selector.tabCreate);
       
        _createResetFields();
        _gotoCreate1();
    }

    function _mailBlock() {
        
        if ($(this).hasClass('selected') || _lockOutTabSwitch) return;
        _lockOutTabSwitch = true;
        _selectTab(CONST.Selector.tabBlock);

        _slideLeft('.block.section', _container);
        _currentTabIndex = 3;

        if (ROE.isD2) { _hideDetail(); }

        //gets Mail/Report blocked players
        ROE.Api.call('mail_blocked', {}, _mailBlock_Success);

        //gets Chat blocked players
        ROE.Chat2.chat.server.listPlayerBlockedChatUsers().done(_getChatBlockedSuccess);
    }
    
    function _mailBlock_Success(r) {
        $('.block .users > .user', _container).remove();
        $.each(r, _mailBlockUserItem);
        if (r.length == 0) {
            $('.block .empty', _container).show();
        } else {
            $('.block .empty', _container).hide();
        }
    }

    function _mailBlockUserItem(i, n) {
        var user = $('.template.user', _container).clone().removeClass('template');
        $('.name', user).html(n.name);
        $('.remove', user).click(_mailUnBlock);
        $('.users', _container).append(user);
        user.attr('uid', n.id);
    }

    function _mailUnBlock() {
        ROE.UI.Sounds.click();
        ROE.Api.call('mail_unblock', { bpid: $(this).parents('.user').attr('uid') }, _mailUnBlock_Success);
    }
    
    function _mailUnBlock_Success(r) {
        $('.users .user[uid=' + r + ']', _container).remove();
        if ($('.bg .block .users .user', _container).length == 0) $('.bg .block .empty', _container).show();
    }


    function _getChatBlockedSuccess(blockedUserData) {

        $('.chatBlockedUsers', _container).remove();
        var chatBlockedUsers = $('<div class="chatBlockedUsers">');
        
        if (blockedUserData.length) {
            chatBlockedUsers.append('<div class="blockedHeader">List of blocked chat users</div>');

            var aPlayer, blockedUserDiv;
            for (var i = 0; i < blockedUserData.length; i++) {
                blockedUserDiv = $('<div class="blockedUser"><div class="blockedPlayerInfo">' + blockedUserData[i].BlockedPlayerName + '</div></div>');
                blockedUserDiv.append($('<div class="unblockUserBtn" data-playerid="' + blockedUserData[i].BlockedPlayerId + '" >UnBlock</div>').click(function () { _unblockChatUser(this); }));
                chatBlockedUsers.append(blockedUserDiv);
            }

        } else {
            chatBlockedUsers.append('<div class="blockedHeader">You have no blocked chat-users</div>');
        }

        $('.block.section', _container).append(chatBlockedUsers);
    }

    function _unblockChatUser(btn) {
        var blockingPlayerId = ROE.Player.id;
        var playerIdToBeBlocked = $(btn).attr('data-playerid');
        ROE.Chat2.chat.server.unblockUser(blockingPlayerId, playerIdToBeBlocked).done(function (data) { /*no data */ });
        $(btn).parent().remove();
    }


    function _selectTab(sel) {
       
        // Remove all selected tabs
        $('.mailTabBtn', _container).removeClass('selected');
        if(sel)
            $(sel, _container).addClass('selected');
        else
            $(CONST.Selector.tabInbox, _container).addClass('selected');
        
        // TODO: verify if the following is still an issue or can it be removed?...
        //the next 3 lines force chrome to redraw, fixing a css draw problem.
        $('.mailTabBtn', _container).hide();
        $('.mailTabBtn', _container).offset();
        $('.mailTabBtn', _container).show();

       
    }

    function _gotoInbox() {
       
        if ($(this).hasClass('selected') || _lockOutTabSwitch) return;
        _lockOutTabSwitch = true;
        _selectTab(CONST.Selector.tabInbox);
        if (_currentTabIndex <= 1) {
            _slideLeft('.list');
        } else {
            _slideRight('.list');
        }
        _currentTabIndex = 1;

        _activeListSelector = CONST.Selector.inbox;      
    }

    function _gotoOutbox() {
       
        if ($(this).hasClass('selected') || _lockOutTabSwitch) return;
        _lockOutTabSwitch = true;
        _selectTab(CONST.Selector.tabOutbox);
             
        if (_currentTabIndex <= 2) {
            _slideLeft('.sentList');
        } else {
            _slideRight('.sentList');
        }
        _currentTabIndex = 2;

        _activeListSelector = CONST.Selector.outbox;      
    }


    function _gotoCreate1() {
        
        $('#mail_create1', _container).show().removeClass('slideLeftFrom');

        _slideRight('.create1step');
        _currentTabIndex = 0;
        
        if (ROE.isD2) { _hideDetail(); }
    }


    // TODO: Consider the usefulness of these 2 functions
    function _hideDetail() {      
        $('.noMsgSelected').show();
        $('.detail', _container).hide();
        $(_activeListSelector + ' .item').removeClass('selected');
    }
    function _showDetail() {
     
    }

    function _slide(where, show) {
        BDA.UI.Transition['slide' + where]($(show, _container).show().addClass('newshowed'), $('.section.showed', _container), _hideAfterSlide);
    }
    
    function _slideLeft(show) { _slide('Left', show); }
    function _slideRight(show) { _slide('Right', show); }

    function _hideAfterSlide() {
        $('.section.showed', _container).removeClass('showed');
        $('.section.newshowed', _container).removeClass('newshowed').addClass('showed');
        _lockOutTabSwitch = false;
    }

    function _notify(mes) {
       
        var mes = $('#mail_popup .phrases [ph=' + mes + ']', _container).html();
        $('.notification', _container).html(mes).fadeIn();
        setTimeout(function () { $('.notification', _container).fadeOut({ complete: function () { $(this).hide(); } }) }, 4000);
    }

    obj.sendmail = function (who) {       
        ROE.Frame.popupMailPrefilled(who, "", "");
    }

    obj.showBlock = function () {
        ROE.Frame.popupMail();
        BDA.UI.Transition.slideLeft($('#mail_listtabs', _container).show(), $(_activeListSelector + ' .action-back', _container));
        _mailBlock.apply($('.block[tab]', _container));
    }

    function _checkRecipients_onDataSuccess() { }

    function _checkRecipients_onFailure(r) {
        var note = "Player names are not valid:";
        $.each(r.object, function (i, n) { note += " <b>" + n + "</b>,"; });
        $('.notification', _container).html(note).fadeIn();
        setTimeout(function () { $('.notification', _container).fadeOut({ complete: function () { $(this).hide(); } }) }, 10000);
    }

    function _createSend() {
        
        var to = $('.create1step .to', _container);
        if (to.val().trim() == '') { to.focus().click(); _notify('NoPlayers'); return; }
       
        var subject = $('.create1step .subject', _container);
        if (subject.val().trim() == '') { subject.focus().click(); _notify('NoSubject'); return; }

        ROE.Frame.busy('Sending Mail Pigeons.', 6000, _container);
        ROE.Api.call('mail_create', {
            to: to.val(),
            subject: subject.val(),
            message: $('.create1step .message', _container).val(),
            hide: $('.create1step .to-hide', _container)[0].checked
        }, _create_onDataSuccess, _create_onFailure, true, undefined, 'POST');
    }

    function _createResetFields() {
        $('.step input, .step textarea', _container).val('').removeAttr('checked');
        $('.step .autocomplete', _container).empty();
    }

    function _create_onFailure(r) {
        ROE.Frame.free(_container);
        if (r.object == "PlayersCouldNotBeFound") {
            var to = $('.create1step .to', _container).val().trim();
            ROE.Api.call('mail_recipients_check', { to: to }, _checkRecipients_onDataSuccess, _checkRecipients_onFailure);
        } else {
            _notify(r.object);
        }
    }

    function _create_onDataSuccess() {
        ROE.Frame.free(_container);
        _createResetFields();
       
        _reload();
        _reloadSent();       
    }
        
    function _list_onAjaxDataSuccess(r) {
        BDA.Database.Delete(CONST.tableName).done(_afterCacheRemoved.apply(r));
    }

    function _afterCacheRemoved() {
        BDA.Database.Insert(CONST.tableName, this, _afterCacheUpdated.apply(this));
    }

    function _afterCacheUpdated() {
        BDA.Database.LocalSet('MailListLoaded', this.length);
        if (fromAjaxLoadedQuickerThanFromCache == 1)
            fromAjaxLoadedQuickerThanFromCache = 2;

        _populate($.grep(this, function (n, i) {
            return i < CONST.pageLength;
        }));

        ROE.Frame.free(_container);
    }

    // SENT MAIL list stuff
    function _sentList_onAjaxDataSuccess(r) {
        BDA.Database.Delete(CONST.sentTableName).done(_sent_afterCacheRemoved.apply(r));
    }

    function _sent_afterCacheRemoved() {
        BDA.Database.Insert(CONST.sentTableName, this, _sent_afterCacheUpdated.apply(this));
    }

    function _sent_afterCacheUpdated() {
     
        BDA.Database.LocalSet('SentMailListLoaded', this.length);
        if (fromAjaxLoadedQuickerThanFromSentCache == 1)
            fromAjaxLoadedQuickerThanFromSentCache = 2;

        _sentPopulate($.grep(this, function (n, i) {
            return i < CONST.pageLength;
        }));

        ROE.Frame.free(_container);
    }


    function _sentPopulate(r) {
        
        $('.sentList .items > .item', _container).remove();
        $(".sentList .items .mailseparator", _container).remove();
        $.each(r, _sentPopulateItem);
       
    }

    function _sentAppendRows(r) {
        $.each(r, _sentPopulateItem);
    }

    function _sentPopulateItem(i, n) {

        var item = $('.template.item', _container).clone().removeClass('template');
        $('.name', item).html((n.receiver ? n.receiver + ' - ' : '') + n.subject); //.attr('href', n.url);
        $('.time', item).html(BDA.Utils.formatEventTime(BDA.Utils.fromMsJsonDate(n.timesent), true));
        $('.icon', item).attr('src', CONST.staticUrl + 'icons/M_MailList' + (n.viewed == 'false' || !n.viewed ? 'New' : 'Old') + '.png');

        if (n.viewed == 'false' || !n.viewed) item.addClass('new');

        $('.sentList .items .more-wrap', _container).before(item);

        item.attr('rid', n.id);
        
        var hotspot = $('.hotspot', item);
        hotspot.attr('rid', n.id);
        hotspot.click(_detailClick);

        // Star any mail in STARRED or any other folder.
        if (n.folderID != null && n.folderID != -1)
            $('.lt-starToggleBtn .lt-starIcon', item).removeClass('off').addClass('on');

        $('.lt-checkboxToggleBtn', item).click(_clickedCheckbox);
        $('.lt-starToggleBtn', item).click(_clickedStar);

        item.after("<div class='mailseparator'> <div class='sectionDivider'></div></div>");
    }

    function _gotoSentList(doSlide) {
     
        if (doSlide) _slideRight('.sentList');

        if (ROE.isMobile) {
            _selectTab(CONST.Selector.tabOutbox);
            _currentTabIndex = 2;
        }

        if (_filterOn || _starredOn || BDA.Database.LocalGet('SentMailListLoaded') <= CONST.pageLength ||
            BDA.Database.LocalGet('SentMailListLoaded') == $('.sentList .items > .item', _container).length ||
            BDA.Database.LocalGet('SentMailListLoaded') == 0) {
            $('.more', _container).hide();
        } else {
            $('.more', _container).show();
            $('.empty', _container).hide();
        }

        if (BDA.Database.LocalGet('SentMailListLoaded') == 0) {
            $('.empty', _container).show();
        }
    }

    function _loadSentCache(offset) {       
        return BDA.Database.FindRange(CONST.sentTableName, CONST.pageLength, offset);
    }

    function _loadMoreSent(onDone) {
        var more = $('.sentList .more', _container);
        var offset = more.attr('offset');
        offset = parseInt(offset) + CONST.pageLength;
        more.attr('offset', offset);

        _loadSentCache(offset).done(_sentAppendRows).done(onDone);

        if (parseInt(BDA.Database.LocalGet('SentMailListLoaded')) - offset <= CONST.pageLength) {
            more.hide();
        }
    }

    function _initLoadFromSentCache(r) {
        if (fromAjaxLoadedQuickerThanFromSentCache == 2) {
            fromAjaxLoadedQuickerThanFromSentCache = 0; return;
        }
        _sentPopulate(r);
    } // End Sent 
    
    function _loadCache(offset) {     
        return BDA.Database.FindRange(CONST.tableName, CONST.pageLength, offset);
    }

    function _populate(r) {

        _updateNumResultsForFilter(r.length);

        $('.list .items > .item', _container).remove();
        $(".list .items .mailseparator", _container).remove();
        $.each(r, _populateItem);
        _gotoList();
    }

    function _appendRows(r) {
        $.each(r, _populateItem);
    }

    function _populateItem(i, n) {
        var item = $('.template.item', _container).clone().removeClass('template');

        $('.name', item).html((n.sender ? n.sender + ' - ' : '') + n.subject); //.attr('href', n.url);
        $('.time', item).html(BDA.Utils.formatEventTime(BDA.Utils.fromMsJsonDate(n.timesent), true));
        $('.icon', item).attr('src', CONST.staticUrl + 'icons/M_MailList' + (n.viewed == 'false' || !n.viewed ? 'New' : 'Old') + '.png');

        if (n.viewed == 'false' || !n.viewed) item.addClass('new');

        $('.list .items .more-wrap', _container).before(item);

        
        item.attr('rid', n.id);

        
        var hotspot = $('.hotspot', item);
        hotspot.attr('rid', n.id);
        hotspot.click(_detailClick);
       
        // Star any mail in STARRED or any other folder.
        if (n.folderID != null && n.folderID != -1)
            $('.lt-starToggleBtn .lt-starIcon', item).removeClass('off').addClass('on');
      

        $('.lt-checkboxToggleBtn', item).click(_clickedCheckbox);
        $('.lt-starToggleBtn', item).click(_clickedStar);
       
        item.after("<div class='mailseparator'> <div class='sectionDivider'></div></div>");
    }


    function _loadMore(onDone) {
        var more = $('.list .more', _container);
        var offset = more.attr('offset');
        offset = parseInt(offset) + CONST.pageLength;
        more.attr('offset', offset);

        _loadCache(offset).done(_appendRows).done(onDone);

        if (parseInt(BDA.Database.LocalGet('MailListLoaded')) - offset <= CONST.pageLength) {
            more.hide();
        }
    }

    function _gotoList(doSlide) {
        if (doSlide) _slideRight('.list');

        /*if (ROE.isMobile) {          
            _selectTab(CONST.Selector.tabInbox);
            _currentTabIndex = 1;
        }*/

        if (_filterOn || _starredOn || BDA.Database.LocalGet('MailListLoaded') <= CONST.pageLength ||
            BDA.Database.LocalGet('MailListLoaded') == $('.list .items > .item', _container).length ||
            BDA.Database.LocalGet('MailListLoaded') == 0) {
            $('.more', _container).hide();
        } else {
            $('.more', _container).show();
            $('.empty', _container).hide();
        }

        if (BDA.Database.LocalGet('MailListLoaded') == 0) {
            $('.empty', _container).show();
        }
    }


    /* MAIL VIEW DETAIL */

    function _detailClick(e) {
        e.preventDefault();
        if (!ROE.isMobile)
            ROE.UI.Sounds.click();

        $('.noMsgSelected').hide();

        ROE.Frame.busy('Opening letter...', 5000, _container);      

        // Need to choose the parent because hotspot is just
        // the trigger but we're using hte whole element        
        _chooseItem($(e.currentTarget).closest(".item"));
      
        if (ROE.isMobile) {
            _slideLeft('.detail');
            BDA.UI.Transition.slideLeft($('.action-back', _container), $('#mail_listtabs', _container));
        }

        ROE.Api.call('mail_detail', { recid: _viewingMail.attr('rid') }, _detail_onDataSuccess);
    }

    /// Pass a jq object
    function _chooseItem(itemElement) {
        // Highlight the selected  item

        // Verbose to be removed if issue is solved...
        BDA.Console.verbose('ROE.Mail', 'Removing selected, activeListSelect = ' + _activeListSelector);

        $(_activeListSelector +' .item', _container).removeClass('selected');      
        _viewingMail = itemElement.addClass('selected');
    }

    function _detail_onDataSuccess(r) {
        var jElement; // used in a $.each() loop
        var localActiveListSelector = _activeListSelector + " ";

        ROE.Frame.free(_container);
        if (ROE.isD2) {
            _showDetail();
            $(localActiveListSelector + ' .noMsgSelected').hide();
        }

        if (_activeListSelector == CONST.Selector.outbox) {
            BDA.Database.Update(CONST.sentTableName, { id: 'id', fields: [{ 'id': $(_viewingMail).attr('rid'), 'viewed': 'true' }] }, function () { });
        } else {
            BDA.Database.Update(CONST.tableName, { id: 'id', fields: [{ 'id': $(_viewingMail).attr('rid'), 'viewed': 'true' }] }, function () { });
        }

        if (ROE.isMobile) {
            // Mobile uses a separate detail pane for both list and sent list
            // Where as D2 has dual pane.
            localActiveListSelector = "";
            $('.action-back', _container).fadeIn();
        }
            
        
        $(localActiveListSelector + '.detail', _container).show();
        $(localActiveListSelector + '.detail .info .message', _container).html(r.message).attr('bbcode', r.message_bbcode);
        $(localActiveListSelector + '.detail .info .bbcode_p', _container).click(function () {
            ROE.Frame.popupPlayerProfile($(this).html());
        });

        $(localActiveListSelector + '.detail .info .bbcode_v', _container).click(function () {
            ROE.Frame.popupVillageProfile($(this).attr('data-vid'));
        });

        $(localActiveListSelector + '.detail .info .bbcode_c', _container).click(function () {
            ROE.Frame.popupClan($(this).attr('data-cid'));
        });

        $(localActiveListSelector + '.detail .info .subject', _container).html(r.subject);
        $(localActiveListSelector + '.detail .info .time', _container).html(BDA.Utils.formatEventTime(BDA.Utils.fromMsJsonDate(r.timesent), true, true, true));
        $(localActiveListSelector + '.detail .info .from', _container).html(r.from);

        //Open links in new tab on D2
        if (ROE.isD2) {
            $('.detail .info .message a', _container).attr("target", "_blank");
        }  
        
        function cleanOwnName(s) {
            return s.replace(', ' + ROE.Player.name, '').replace(ROE.Player.name + ', ', '').replace(ROE.Player.name, '');
        }
        var tocn = cleanOwnName(r.to);
        var tocnsp = $.map(tocn.split(', '), function (n) { return '<span class="tosi">' + n + '</span>'; }).join(', ');
        var $to = $(localActiveListSelector + '.detail .info .tos', _container).html(tocnsp);
        $('.tosi', $to).click(_userNameClick);

        if (tocn == '') { $to.parents('.to').hide(); } else { $to.parents('.to').show(); }

        $(localActiveListSelector + '.detail .next', _container).attr('recid', r.next);
        $(localActiveListSelector + '.detail .prev', _container).attr('recid', r.prev);
                
        // In D1 there were no prev/next for sent items
        // Currently, the r.next and r.prev are incorrect for sent mail
        if (_activeListSelector == CONST.Selector.outbox) {
            $(localActiveListSelector + '.detail .next', _container).hide();
            $(localActiveListSelector + '.detail .prev', _container).hide();
        } else {
            $(localActiveListSelector + '.detail .next', _container).show();
            $(localActiveListSelector + '.detail .prev', _container).show();
        }

        _viewingMail.removeClass('new');
        $(localActiveListSelector + '.icon', _viewingMail).attr('src', CONST.staticUrl + 'icons/M_MailListOld.png');

        // Find custom action buttons
        $(localActiveListSelector + '.detail .applyMobileAction').each(
            function (index, element) {
                jElement = $(element);
                if (jElement.attr('data-mobileText')) {
                    jElement.html(jElement.attr('data-mobileText'));
                }
                jElement.removeAttr("href");
                if (jElement.attr('data-mobileClickAction')) {
                    jElement.click(eval(jElement.attr('data-mobileClickAction')));
                }
            }
        );
    }

    function _mailBackToListFromDetail() {
       
        if (ROE.isMobile) {
            BDA.UI.Transition.slideRight($('#mail_listtabs', _container).show(), $('.action-back', _container).fadeOut(200));
        } else {
            BDA.UI.Transition.slideRight($('#mail_listtabs', _container).show(), $(_activeListSelector + ' .action-back', _container).fadeOut(200));
        }
        if (_activeListSelector == CONST.Selector.outbox) {
            _selectTab(CONST.Selector.tabOutbox);
            _gotoSentList(true);

        } else {
            _selectTab(CONST.Selector.tabInbox);
            _gotoList(true);
        }
    }

    function _detailMove() {
        var recid = $(this).attr('recid');
        if (recid) {
            ROE.Frame.busy('Opening letter...', 5000, _container);

            function MailMoveSetSelected() {
                $(_activeListSelector + ' .items .item', _container).removeClass('selected');
                _viewingMail = $(_activeListSelector + ' .items .item[rid=' + recid + ']', _container).addClass('selected');
            }

            if ($(_activeListSelector + ' .items .item[rid=' + recid + ']', _container).length == 0) {
                _loadMore(MailMoveSetSelected);
            } else {
                MailMoveSetSelected();
            }

            ROE.Api.call('mail_detail', { recid: recid }, _detail_onDataSuccess);
        }
    }

    function _detailDelete(e) {
       

        ROE.UI.Sounds.click();
        var deleteBtn = $(e.currentTarget);
        if (_waitingForDeleteConfirm) {
            clearTimeout(_confirmDeleteTimer);
            _waitingForDeleteConfirm = false;
            deleteBtn.html("");
            _detailDeleteConfirmed();
        } else {
            deleteBtn.html("<span>Sure?</span>");
            _waitingForDeleteConfirm = true;
            _confirmDeleteTimer = window.setTimeout(function () {
                deleteBtn.html("");
                _waitingForDeleteConfirm = false;
            }, 2000);
        }
    }

    function _detailDeleteConfirmed() {
        ROE.Frame.busy('Deleting Mail...', 5000, _container);
        ROE.Api.call('mail_delete', { recid: ~~_viewingMail.attr('rid') }, _detailDelete_AjaxSuccess);
    }


    function _detailDelete_AjaxSuccess() {
        if (_activeListSelector == CONST.Selector.outbox) {
            BDA.Database.DeleteRows(CONST.sentTableName, { id: 'id', list: [_viewingMail.attr('rid')] });
            BDA.Database.LocalSet('SentMailListLoaded', BDA.Database.LocalGet('SentMailListLoaded') - 1);                
        } else {
            BDA.Database.DeleteRows(CONST.tableName, { id: 'id', list: [_viewingMail.attr('rid')] });
            BDA.Database.LocalSet('MailListLoaded', BDA.Database.LocalGet('MailListLoaded') - 1);           
        }

        _mailBackToListFromDetail();     

        _viewingMail.fadeOut('slow', function () {
            // make sure we remove the separator 
            // or we'll run into problem with orphans in DOM
            var next = $(this).next();
            if (next && next.hasClass(CONST.CSSClass.listItemSeparator))
                next.remove();

            $(this).remove();
        });

        ROE.Frame.free(_container);
    }

    function _detailReplyStart() {
        _mailCreate();
       
        var localActiveListSelector = "";
        if (!ROE.isMobile) {
            localActiveListSelector = _activeListSelector + " ";
        }

        var from = $(localActiveListSelector + '.detail .from', _container).html();
        var subject = $(localActiveListSelector + '.detail .subject', _container).html();
        subject = subject.replace(/RE:/g, '');
        var message = $(localActiveListSelector + '.detail .message', _container).attr('bbcode').replace(/<br \/>/g, '\n');
        var remessage =
            '\n-------Original Message-------\n' +
            'From: ' + from + '\n' +
            'Sent: ' + $(localActiveListSelector + '.detail .info .time', _container).html() + '\n' +
            'Subject: ' + subject + '\n' + message + '\n';

        $('.create1step .to', _container).val(from);
        $('.create1step .subject', _container).val('RE:' + subject);
        $('.create1step .message', _container).val(remessage);

    }

    function _detailReplyAllStart() {
        _mailCreate();

        var localActiveListSelector = "";
        if (!ROE.isMobile) {
            localActiveListSelector = _activeListSelector + " ";
        }

        var from = $(localActiveListSelector + '.detail .from', _container).html();
        var subject = $(localActiveListSelector + '.detail .subject', _container).html();
        subject = subject.replace(/RE:/g, '');
        var message = $(localActiveListSelector + '.detail .message', _container).attr('bbcode').replace(/<br \/>/g, '\n')
        var to = $.map($(localActiveListSelector + '.detail .tosi', _container), function (n) { return $(n).html(); }).join(', ')
        var sendto = (to == '(names hidden)' ? '' : to) + ', ' + from;        
        sendto = sendto.replace(new RegExp(ROE.Player.name, 'g'), '')
                       .replace(/[, ]+/g, ', ').replace(/(,\s)$/g, '').replace(/^(,\s)/g, '');

        if (sendto == '') sendto = ROE.Player.name;

        var remessage =
            '\n-------Original Message-------\n' +
            'From: ' + from + '\n' +
            'Sent: ' + $(localActiveListSelector + '.detail .info .time', _container).html() + '\n' +
            'Subject: ' + subject + '\n' + message + '\n';

        $('.create1step .to', _container).val(sendto);
        $('.create1step .subject', _container).val('RE:' + subject);
        $('.create1step .message', _container).val(remessage);
    }

    function _detailForwardStart() {
        _mailCreate();

        var localActiveListSelector = "";
        if (!ROE.isMobile) {
            localActiveListSelector = _activeListSelector + " ";
        }

        var from = $(localActiveListSelector + '.detail .from', _container).html();
        var subject = $(localActiveListSelector + '.detail .subject', _container).html().replace(/Fwd:/g, '');
        var message = $(localActiveListSelector + '.detail .message', _container).attr('bbcode').replace(/<br \/>/g, '\n');
        var remessage =
            '\n-------Original Message-------\n' +
            'From: ' + from + '\n' +
            'Sent: ' + $(localActiveListSelector + '.detail .info .time', _container).html() + '\n' +
            'Subject: ' + subject + '\n' + message + '\n';

        $('.create1step .to', _container).val('');
        $('.create1step .subject', _container).val('Fwd:' + subject);
        $('.create1step .message', _container).val(remessage);
    }


    function _createPrefilledMessage(messageObj) {
        _mailCreate();

        $('.create1step .to', _container).val(messageObj.to);
        $('.create1step .subject', _container).val(messageObj.subject);
        $('.create1step .message', _container).val(messageObj.message);
    }
    
  
    /// An exposed func that grabs a filter to apply to the list
    function _filterList(listSelector) {
        if (!listSelector)
            return;
        _activeListSelector = listSelector;
                
        if (_activeListSelector == CONST.Selector.inbox) {
            var filterPhrase = $('#listFilterInput').val();
            // Has to be an actual phrase to continue
            if (typeof (filterPhrase) === "undefined" || filterPhrase === null || filterPhrase === "")
                return;

            $('#listFilterInput').blur();

            _filterOn = true;
            $(CONST.Selector.inbox +' .filterApplied').html(filterPhrase);
            $(CONST.Selector.inbox + ' .listToolsFilter .clearFilterBtn').show();
            _loadFilteredCache(filterPhrase).done(_populate);
        } else if (_activeListSelector == CONST.Selector.outbox) {
         
            var filterPhrase = $('#sentListFilterInput').val();
            // Has to be an actual phrase to continue
            if (typeof (filterPhrase) === "undefined" || filterPhrase === null || filterPhrase === "")
                return;

            $('#sentListFilterInput').blur();

            _filterOn = true;
            $(CONST.Selector.outbox + ' .filterApplied').html(filterPhrase);
            $(CONST.Selector.outbox + ' .listToolsFilter .clearFilterBtn').show();
            _loadFilteredCache(filterPhrase).done(_populate);
        }

        $(_activeListSelector + " .clearFilterListBtn").addClass('active');

    }

    /// An exposed func that clears the filter (and re-builds the list)
    function _clearFilterList(listSelector) {
 
        if (!listSelector)
            return;
        _activeListSelector = listSelector;
        _clearFilter();        
    }


    function _clearFilter(dontRefresh) {
       
        $(_activeListSelector + " .clearFilterListBtn").removeClass('active');       
        $(_activeListSelector + ' .deleteSelectedBtn').removeClass("active");
        if (_activeListSelector == CONST.Selector.outbox) {
            _sentTotalChecked = 0;            
        } else {
            _totalChecked = 0;
        }
        _hideDetail();
       
        _filterOn = false;
        _starredOn = false;
        $(_activeListSelector + ' .listToolsFilter .clearFilterBtn').hide();
        _numItemsInFilter = 0;
        $(_activeListSelector + ' .numItemsFound').html("");
        $(_activeListSelector + ' .filterApplied').html("");
        if (_activeListSelector == CONST.Selector.inbox) {
            $('#listFilterInput').val("");
        } else if (_activeListSelector == CONST.Selector.outbox) {
            $('#sentListFilterInput').val("");
        }

        
        // Reset more button offset.
        $(_activeListSelector + ' .more', _container).attr('offset', 0);
        if (parseInt(BDA.Database.LocalGet(CONST.LocalVars.listLoaded)) <= CONST.pageLength) {
            $(_activeListSelector + ' ' + CONST.Selector.moreButton, _container).hide();
        }


        $(_activeListSelector + ' .items').scrollTop(0);
        if (_activeListSelector == CONST.Selector.outbox) {
            _loadSentCache(0).done(_initLoadFromSentCache);
        } else {
            _loadCache(0).done(_initLoadFromCache);
        }
       
    }

    /// Get database rows which contain a filter match inside subject
    function _loadFilteredCache(sFilter) {      
        return BDA.Database.FindByFilter(CONST.tableName, "subject", sFilter);
    }

    /// Called when the num of filtered items changes
    function _updateNumResultsForFilter(num, listSelector) {
        if (_filterOn || _starredOn) {
            $(listSelector +' .numItemsFound').html(num);
        }
    }

    /// A special filter to only show starred reports
    function _filterStarredOnly(listSelector) {
        if (!listSelector)
            return;
        
        if (listSelector == CONST.Selector.inbox) {
            if (_starredOn) {
                $(listSelector + " .filterStarredOnlyBtn").removeClass("active");
                _clearFilterList(listSelector);

            } else {
                _starredOn = true;
                $(listSelector + " .filterStarredOnlyBtn").addClass("active");
                $('#listFilterInput').val(""); // incase they were searching for something
                // Piggy back off the filter feature               
                $('.listToolsFilter .clearFilterBtn').show();
                _loadStarredItems().done(_populate);
              
                $(_activeListSelector + " .clearFilterListBtn").addClass('active');
                _totalChecked = 0;
                $(_activeListSelector + ' .deleteSelectedBtn').removeClass("active");

                $('#mail_popup .clearFilterBtn').show();
                $('#mail_popup .filterTerm').show();
                $('#mail_popup .hideDuringFilter').hide();
            }
        }       
    }

    function _clearStarredFilter() {
       
        // Only the mail inbox has filters so this is OK to do...
        $(CONST.Selector.inbox+" .filterStarredOnlyBtn").removeClass("active");
        _clearFilterList(CONST.Selector.inbox);

        $('#mail_popup .clearFilterBtn').hide();
        $('#mail_popup .filterTerm').hide();
        $('#mail_popup .hideDuringFilter').show();
    }

    function _updateNumResultsForFilter(num) {
        if (_starredOn) {
            $('#numMailFound').html(num);
        }
    }

    /// Supply .list or .sentList depending on which list you want to apply this to
    function _deleteSelected(listSelector, deleteBtn) {
        if (!listSelector)
            return;
        if ($(listSelector + ' .deleteSelectedBtn').hasClass('active'))
            ROE.UI.Sounds.click();

        _activeListSelector = listSelector;
               
      
        
         var listOfSelectedItems = $("#mail_popup " + _activeListSelector + " .lt-checkboxToggleBtn[data-selected='true']").closest('.item')

        
        if(listOfSelectedItems.length != 0) {

            if (_waitingForDeleteSelectedConfirm) {
                clearTimeout(_confirmDeleteSelectedTimer);
                _waitingForDeleteSelectedConfirm = false;
                deleteBtn.html("");
                _confirmDeleteSelected(listOfSelectedItems);
            } else {
                deleteBtn.html("Sure?");
                _waitingForDeleteSelectedConfirm = true;
                _confirmDeleteSelectedTimer = window.setTimeout(function () {
                    deleteBtn.html("");
                    _waitingForDeleteSelectedConfirm = false;
                }, 2000);
            }
        }

    }

    function _confirmDeleteSelected(listOfSelectedItems) {
        _deleteMultipleItems(listOfSelectedItems);
    }




    function _deleteMultipleItems(itemList) {
        if (itemList.length == 0)
            return;

        ROE.Frame.busy(undefined, undefined, _container);

        _lastItemsDeleted = itemList;
        var listOfIds = new Array();
        _lastItemsDeleted.each(function (i, item) {
            listOfIds.push(~~$(item).attr('rid'));
        });
        var joinedlistOfIds = listOfIds.join(",");
              
        ROE.Api.call('mail_delete', { recid: joinedlistOfIds }, _deleteMultipleItems_AjaxSuccess);
    }

    function _deleteMultipleItems_AjaxSuccess() {
        var listOfIds = $(_lastItemsDeleted).map(function () { return ~~($(this).attr("rid")); }).get();
        BDA.Database.DeleteRows(CONST.tableName, { id: 'id', list: listOfIds });

        BDA.Database.LocalSet(CONST.LocalVars.listLoaded, BDA.Database.LocalGet(CONST.LocalVars.listLoaded) - _lastItemsDeleted.length);
       
        _lastItemsDeleted.each(function (i, item) {
            $(this).fadeOut('slow', function () {
                // make sure we remove the separator as well
                // or we'll run into problem with orphans
                var next = $(this).next();
                if (next && next.hasClass(CONST.CSSClass.listItemSeparator))
                    next.remove();

            });
        }).promise().done(function () {
            var l = _lastItemsDeleted.length;
            _lastItemsDeleted.remove();
            _refreshList(l);
        });

        ROE.Frame.free(_container);
    }

    // Refreshes the list after delete.
    function _refreshList(quantity) {
               
        // If we are filtering, we don't need to append anything
        // because currently filter and starred show all (no "more" button)
        if (_filterOn || _starredOn) {
            _numItemsInFilter--;
            if (_numItemsInFilter <= 0) {

            } else {
                _updateNumResultsForFilter(_numItemsInFilter);
            }
            return;
        }

        var offset = parseInt($(CONST.Selector.moreButton, _container).attr('offset'));

        if (quantity) {
            _loadQuantityCache(offset, quantity).done(_appendRows);
        } else {          
            _loadOneCache(offset).done(_appendRows);
        }

        if (parseInt(BDA.Database.LocalGet(CONST.LocalVars.listLoaded)) - offset <= CONST.pageLength) {
            $(CONST.Selector.moreButton, _container).hide();
        }
    }

    // Useful for a single delete
    function _loadOneCache(offset) {
        return _loadQuantityCache(offset, 1);
    }

    // Handle getting various amounts of records
    function _loadQuantityCache(offset, quantity) {
        var finalOffset = offset + (CONST.pageLength - quantity);       
        return BDA.Database.FindRange(CONST.tableName, quantity, finalOffset);
    }

    /// Supply .list or .sentList depending on which list you want to select all
    function _selectAllItems(listSelector) {       
        if (!listSelector)
            return;

        $('#mail_popup ' + listSelector + ' .item:not(.template) .lt-checkboxToggleBtn').each(function (i, elem) {
            var toggle = $(elem);
            var checkmark = $('.checkmark', toggle);
            if (checkmark.hasClass('off')) {
                checkmark.removeClass('off').addClass('on');
                toggle.attr('data-selected', 'true');
                if (listSelector == CONST.Selector.outbox) {
                    _sentTotalChecked++;
                } else {
                    _totalChecked++;
                }
            }
        });

        if (listSelector == CONST.Selector.outbox) {           
            if (_sentTotalChecked > 0) {
                // Show delete
                $(_activeListSelector + ' .deleteSelectedBtn').addClass("active");
            }
        } else {
            _totalChecked++;
            if (_totalChecked > 0) {
                // Show delete
                $(_activeListSelector + ' .deleteSelectedBtn').addClass("active");
            }
        }
       
    }
    
    function _deselectAllItems(listSelector) {
        if (!listSelector)
            return;
       
        $('#mail_popup ' + listSelector + ' .item:not(.template) .lt-checkboxToggleBtn').not('.template').each(function (i, elem) {
            var toggle = $(elem);
            var checkmark = $('.checkmark', toggle);
            if (checkmark.hasClass('on')) {
                checkmark.removeClass('on').addClass('off');
                toggle.attr('data-selected', 'false');
            }
        });
        if (listSelector == CONST.Selector.outbox) {
            _sentTotalChecked = 0;
        } else {
            _totalChecked = 0;
        }
       
        $(_activeListSelector + ' .deleteSelectedBtn').removeClass("active");
    }
      
    function _loadStarredItems() {
        return BDA.Database.Find(CONST.tableName, "folderID", ["~null"]);
    }
   
    function _clickedStar(e) {
        if (!ROE.isMobile)
            ROE.UI.Sounds.click();
        var toggle = $(e.currentTarget);
        var item = $(toggle).closest('.item');
        var star = $('.lt-starIcon', toggle);
        
        if (star.hasClass('on')) {
            star.removeClass('on').addClass('off');
            ROE.Api.call('unstar_mail', {
                rids: item.attr('rid')
            }, _starredMailAPISuccessCallback);
           
        } else {
            star.removeClass('off').addClass('on');
            ROE.Api.call('star_mail', {
                rids: item.attr('rid')
            }, _starredMailAPISuccessCallback);
        }
             
       
    }
   
    function _starredMailAPISuccessCallback(r) {
      
        BDA.Database.Update(CONST.tableName, { id: 'id', fields: r });
    }

    /// Check mark stuff
    function _clickedCheckbox(e) {
        if (!ROE.isMobile)
            ROE.UI.Sounds.click();
        var toggle = $(e.currentTarget);
        var checkmark = $('.checkmark', toggle);
        if (checkmark.hasClass('on')) {
            /*if (_activeListSelector == CONST.Selector.outbox) {
                _sentTotalChecked--;
                if (_sentTotalChecked < 1) {
                    // Hide delete
                    $(_activeListSelector + ' .deleteSelectedBtn').toggleClass("active", false);
                }
            } else {
                _totalChecked--;
                if (_totalChecked < 1) {
                    // Hide delete
                    $(_activeListSelector + ' .deleteSelectedBtn').toggleClass("active", false);
                }
            }*/
            _updateTotalChecked(-1);
            checkmark.removeClass('on').addClass('off');
            toggle.attr('data-selected', 'false');
        } else {
           /* if (_activeListSelector == CONST.Selector.outbox) {
                _sentTotalChecked++;

                if (_sentTotalChecked > 0) {
                    // Show delete
                    $(_activeListSelector + ' .deleteSelectedBtn').toggleClass("active", true);
                }
            } else {
                _totalChecked++;

                if (_totalChecked > 0) {
                    // Show delete
                    $(_activeListSelector + ' .deleteSelectedBtn').toggleClass("active", true);
                }
            }*/
            _updateTotalChecked(1);
            checkmark.removeClass('off').addClass('on');
            toggle.attr('data-selected', 'true');
        }
    }


    function _updateTotalChecked(n) {
        if (_activeListSelector == CONST.Selector.outbox) {
            _sentTotalChecked+=n;
            if (_sentTotalChecked < 1) {
                _sentTotalChecked = 0;
                // Hide delete
                $(_activeListSelector + ' .deleteSelectedBtn').toggleClass("active", false);
            } else {
                $(_activeListSelector + ' .deleteSelectedBtn').toggleClass("active", true);
            }
        } else {
            _totalChecked+=n;
            if (_totalChecked < 1) {
                _totalChecked = 0;
                // Hide delete
                $(_activeListSelector + ' .deleteSelectedBtn').toggleClass("active", false);
            } else {
                $(_activeListSelector + ' .deleteSelectedBtn').toggleClass("active", true);
            }
        }
    }

    
    obj.filterList = _filterList;
    obj.clearFilterList = _clearFilterList;
    obj.filterStarredOnly = _filterStarredOnly;
    obj.clearStarredFilter = _clearStarredFilter;
    obj.deleteSelected = _deleteSelected;   
    obj.selectAllItems = _selectAllItems;
    obj.deselectAllItems = _deselectAllItems;
    obj.reload = _reload;

}(window.ROE.Mail = window.ROE.Mail || {}));