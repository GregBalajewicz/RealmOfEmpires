(function (obj) {

    var _gameAvatars = {};
    var _container; //container for avatar picker


    /*
     
     type 1 - standard avatar, everyone has it, sees it, and can use it
     type 2 - everyone sees it, but must be owned to be used. It can be unlocked by doing something, or for some cost
     type 3 - only those who have been given it can see it and use it.
     
     status 0 - locked avatar, cant be used
     status 1 - unlocked avatar, can be used
     
     */

    var CONST = {
        type: {
            standard: 1,
            unlockable: 2,
            special: 3
        },
        status: {
            locked: 0,
            unlocked: 1
        }
    }
   
    obj.init = function (container,initialAvatarID) {
        _container = $(container);

        ROE.Frame.busy('Loading Avatars...', 5000, _container);
        
        ROE.Api.call_avatars_getall(_avatars_getAllCallback);

        function _avatars_getAllCallback(data) {

            ROE.Avatar.convertAvatarsArrayToObject(data);

            _build();

            //pick specific avatar, or pick first one
            if (initialAvatarID) {
                $('.avatarBox[data-avatarid="' + initialAvatarID + '"]', _container).click();
            } else {
                $('.avatarBox', _container).first().click();
            }
        }

        ROE.Avatar.lastPickedID = null;

    }

    //turn the array we get to an object, so we can referrence by ID
    function _convertAvatarsArrayToObject(array) { 
        for (var i = 0; i < array.length; i++) {
            _gameAvatars[array[i].id] = array[i];
        }
        ROE.Avatar.list = _gameAvatars;
    }

    function _build() {

        var _avatarContent = $('<div class="avatarPicker">');
        var _listPane = $('<div class="listPane">');
        var _dispPane = $('<div class="dispPane">');

        var avatarObj, avatarElement;
        for (var ava in _gameAvatars) {

            avatarObj = _gameAvatars[ava];

            //if avatar is type 3 and it isnt owned, dont show
            if (avatarObj.type == CONST.type.special && avatarObj.status == CONST.status.locked) {
                continue;
            }

            avatarElement = $('<div class="avatarBox" data-avatarid="' + avatarObj.id + '" style="background-image:url(' + avatarObj.imageUrlS + ')" data-status="' + avatarObj.status + '" >');

            if (avatarObj.status == CONST.status.locked) {
                avatarElement.addClass('locked').append('<div class="lockMask">');
            }

            avatarElement.click(_avatarClick);
            _listPane.append(avatarElement);
        }

        _avatarContent.append(_dispPane, _listPane );
        _container.empty().append(_avatarContent);

        _listPane.find('.locked').sort(function (a, b) {
            var avACost = _gameAvatars[$(a).attr('data-avatarid')].cost;
            var avBCost = _gameAvatars[$(b).attr('data-avatarid')].cost;

            if (avACost < avBCost) {
                return 1;
            }
            if (avACost > avBCost) {
                return -1;
            }
            return 0;
        }).appendTo(_listPane);

        ROE.Frame.free(_container);
        
    }

    function _avatarClick() {
        $('.clicked', _container).removeClass('clicked');
        $(this).addClass('clicked');

        var avatarID = $(this).attr('data-avatarid');
        var avatar = ROE.Avatar.list[avatarID];
        var displayPane = $('.dispPane', _container).empty();
        displayPane.css('background-image', 'url(' + avatar.imageUrlL + ')');
        
        var infoBox = $('<div class="infoBox"></div>');

        //add desc if avatar has desc
        if (avatar.info) {
            infoBox.append('<div class="desc">' + avatar.info + '</div>').show();
        }

        if (avatar.status === CONST.status.unlocked) {
            ROE.Avatar.lastPickedID = avatarID;
        } else {
            ROE.Avatar.lastPickedID = null; //cant set this as your avatar yet
            //if avatar has a cost, it is purchsable, add purchase button
            if (avatar.cost) {
                infoBox.append($('<div class="notice fontGoldFrLClrg"><div class="text">Premium Avatar: ' + avatar.cost + '<div class="sIcon"></div></div></div>'));
                infoBox.append($('<div class="unlockNow BtnBLg1 fontSilverFrSClrg">Unlock</div>').click(function () {
                    _unlockPopup(avatar);
                }));
            } else {
                //incase avatar is locked, and not purchasable, probably has to be clarified -farhad
                infoBox.append($('<div class="notice">LOCKED</div>'));
            }
        }

        displayPane.append(infoBox);
        
    }

    function _unlockPopup(avatar) {
        
        _container.find('.quickPopup').remove();

        var content = $('<div class="unlockNowPopup">' +
                '<div class="textInfo">Permanently add this avatar to your account collection.<br/>You can use it in any Realm and in the Throne Room.</div>' +
                '<div class="textCost">Unlock cost: ' + avatar.cost + ' servants. <div class="sIcon"></div></div>' +
                '<div class="purchaseBtn BtnBLg1 fontSilverFrSClrg">Unlock Avatar</div>'+
            '</div>');

        ROE.Frame.quickPopup({
            content: content,
            appendTo: _container,
            title: 'Unlock Avatar',
            icon: avatar.imageUrlS
        });

        //for TR for now gotta direct them to in realm
        if (ROE.Player.credits == null || ROE.Player.credits == undefined) {
            _container.find('.unlockNowPopup.qpContent').html('Please log into a realm for unlocking avatar feature.<br/>' +
            'Once an avatar is unlocked, it is usable in ALL of your realms and in your Throne room.');
            return;
        }

        //if user has enough credits
        if (ROE.Player.credits >= avatar.cost) {

            _container.find('.quickPopup .purchaseBtn').click(function () {
                var btn = $(this);
                //confirm, preventing accidental click
                if (!btn.hasClass('areyousure')) {
                    btn.attr('data-text', btn.text());
                    btn.addClass('areyousure').html("Confirm?");
                    window.setTimeout(function () {
                        btn.removeClass('areyousure').html(btn.attr('data-text'));
                    }, 2500);
                    return;
                }
                btn.removeClass('areyousure');
                _purchaseAvatar(avatar);
            });

        } else {
            var purchaseServants = '<div class="notEnoughServants">' +
                    '<div class="bigred">Not enough servants.</div>'+
                    '<div class="info">Servants are used to unlock the Premium Features.<br/>' +
                    'You have <b>' + ROE.Player.credits  + '</b> servants.</div>' +
                    '<div class="sfx2 buymore customButtomBG">Hire Servants!</div>' +
                '</div>';
            _container.find('.quickPopup .purchaseBtn').replaceWith(purchaseServants);
            _container.find('.quickPopup .buymore').click(function () {
                _container.find('.quickPopup').remove();
                ROE.Frame.showBuyCredits();
            });
        }

    }

    function _purchaseAvatar(avatar) {
        ROE.Frame.busy('Unlocking your Avatar!', 5000, _container);
        _container.find('.quickPopup').remove();
        ROE.Api.call_avatars_purchase(avatar.id, _avatars_purchaseCallback);
    }

    function _avatars_purchaseCallback(data) {

        if (data.result === 0) {
            ROE.Frame.refreshPFHeader(data.credits);
            _convertAvatarsArrayToObject(data.newList);
            _build(); //maybe do a cool animation here and before calling build? -farhad
            $('.avatarBox[data-avatarid="' + data.avatarID + '"]', _container).click();
            ROE.Frame.infoBar('Avatar Unlocked successfully!');
        } else if (data.result === 2) {
            ROE.Frame.errorBar('Purchase failed, not enough servants.');
        }

        ROE.Frame.free(_container);
    }
    
    obj.lastPickedID = null; //used by external modules, to change users avatars to this if not null
    obj.list = null;
    obj.convertAvatarsArrayToObject = _convertAvatarsArrayToObject;

}(window.ROE.Avatar = window.ROE.Avatar || {}));