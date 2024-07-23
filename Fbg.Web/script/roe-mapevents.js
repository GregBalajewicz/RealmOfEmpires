(function (obj) {

    // MAP EVENTS SECTION

    //this gets called when a map square is selected
    var _checkPlayerMapEvents = function _checkPlayerMapEvents(mapXCord, mapYCord) {
        var mapEvent = ROE.Player.MapEventsGrouped[mapXCord + "," + mapYCord];
        if (mapEvent) {

            //to prevent multiple calls of same coord, check if this map coord has a busy mask, meaning still busy with a call
            if ($("#MEM" + mapXCord + mapYCord).length) { return true; }

            //NOTE: ISSUE HERE, IF THE FIRST EVENT DOES NOT GO AWAY AFTER CALLBACK, OTHER EVENTS WILL NEVER GET A CHANCE TO BE CLICKED/PROCESSED
            //perhaps later when we add multiple DIFFERENT map event types to same coord, we will handle it via a ui-menu
            var firstEvent = mapEvent[0];
            if (firstEvent) {
                _playerMapEventsClicked(firstEvent);
            }
            return true;
        }
        return false;
    }

    var _playerMapEventsClicked = function _playerMapEventsClicked(playerMapEvent) {

        $('.mapEventGui').remove();

        if (playerMapEvent.typeID == 2) { //clicking loot camp
            _caravanClicked(playerMapEvent);
        } else {
            _processPlayerMapEvent(playerMapEvent);
        }

    }

    function _buildMapEventGui(playerMapEvent) {
        var mapEventGui = $('<div class="mapEventGui fontSilverFrLCmed" data-eventid="' + playerMapEvent.eventID + '"><div class="content"></div></div>');
        var closeBtn = $('<div class="close"></div>').click(function () { mapEventGui.remove() });
        mapEventGui.append(closeBtn);
        mapEventGui.on("mouseover", function () { $('#mapGuiHover').hide(); });
        $('body').append(mapEventGui);
        return mapEventGui;
    }

    var _lootReveal = {}; //holds caravan revealed loot data

    function _caravanClicked(playerMapEvent, fromVOV) {

        //clicking a map gui removes all before, so must make new one here
        var mapEventGui = _buildMapEventGui(playerMapEvent);
        mapEventGui.addClass('travellingCaravan');

        var mustReveal = false; //if true, will force a reveal requirement

        //Prep mapEvent StateData
        var splitEventStateData = playerMapEvent['stateData'].split(',');
        if (splitEventStateData.length > 1) {
            var collectedTotal = parseInt(splitEventStateData[0]);
            var collectedThisLevel = parseInt(splitEventStateData[1]);
            var campLevel = parseInt(splitEventStateData[2]);
        } else { //for old or incompatible data, will force a reveal first
            mustReveal = true;
        }

        //Prep mapEvent Data
        var splitEventData = playerMapEvent['data'].split(',');
        if (splitEventData.length > 1) {
            var lootID1 = splitEventData[0];
            var lootID2 = splitEventData[1];
            var lootID3 = splitEventData[2];
            var rerolled = parseInt(splitEventData[3]);
        } else { //for old or incompatible data, will force a reveal first
            mustReveal = true;
        }

        if (!lootID1 || lootID1 == 'x') {
            mustReveal = true;
        }

        var lootRevealData = _lootReveal[playerMapEvent.eventID];
        if (!lootRevealData) {
            mustReveal = true;
        }

        //because IE older than Edge fails at css flips
        var ua = window.navigator.userAgent;
        var oldIE = ua.indexOf('MSIE ') > 0 || ua.indexOf('Trident/') > 0;
        //var iOS = ua.match(/(iPhone|iPod)/g);

        var content = $('<div class="content ' + (oldIE ? 'oldIE' : '') + '">' +
                    '<div class="title">Travelling Caravan ( Level ' + campLevel + ' )</div>' +

                    '<div class="cards">' +

                        '<div class="card" data-tooltipid="caravanLootCard1" data-index="1" >' +
                            '<div class="flipper">' +
                                '<div class="front"></div>' +
                                '<div class="back"></div>' +
                            '</div>' +
                        '</div>' +

                        '<div class="card" data-tooltipid="caravanLootCard2" data-index="2" >' +
                            '<div class="flipper">' +
                                '<div class="front"></div>' +
                                '<div class="back"></div>' +
                            '</div>' +
                        '</div>' +

                        '<div class="card" data-tooltipid="caravanLootCard3" data-index="3" >' +
                            '<div class="flipper">' +
                                '<div class="front"></div>' +
                                '<div class="back"></div>' +
                            '</div>' +
                        '</div>' +

                    '</div>' +

                    '<div class="infos fontSilverFrLCmed"></div>' +

                '</div>');

        fromVOV = fromVOV || $('.content', mapEventGui).hasClass('fromVOV'); //check if launched from VOV or was previously launched from vov for callbacks {
        if(fromVOV){ content.addClass('fromVOV'); } //so make sure new content has class too

        $('.content', mapEventGui).replaceWith(content);


        if (mustReveal) {
            ROE.Frame.busy('Revealing!', 5000, content);
            ROE.Api.call_playermapevent_caravan_cardreveal(playerMapEvent.eventID, _cardRevealCallback);
        } else {
            _fillCaravanContent(playerMapEvent);
        }


        if (ROE.isD2) {

            if (fromVOV) {

                mapEventGui.css({
                    left: 90,
                    bottom: 240
                });
                mapEventGui.animate({ bottom: '+=30px' }, 300, "easeOutSine");

            } else {

                var xCord = playerMapEvent.xCord;
                var yCord = playerMapEvent.yCord;
                var top1 = (ROE.Landmark.landpx * -yCord + ROE.Landmark.ry) - mapEventGui.height();
                var top2 = Math.max(top1 - ROE.Landmark.landpx / 3, 0); //clamp Y
                var left = (ROE.Landmark.landpx * xCord + ROE.Landmark.rx) + (ROE.Landmark.landpx / 2) - (mapEventGui.width() / 2);
                left = Math.min(left, $(window).width() - mapEventGui.outerWidth()); //clamp X

                mapEventGui.css({
                    left: left,
                    top: top1,
                });
                mapEventGui.animate({ top: top2 + 'px' }, 300, "easeOutSine");

            }

        } else {
            mapEventGui.css({
                left: $(window).width() / 2 - (mapEventGui.outerWidth() / 2),
                top: $(window).height() / 2 - (mapEventGui.outerHeight() / 2),
            });
            mapEventGui.animate({ top: '-=30px' }, 300, "easeOutSine");
        }

    }

    


    function _fillCaravanContent(playerMapEvent) {

        var mapEventGui = $('.travellingCaravan[data-eventid="' + playerMapEvent.eventID + '"]');
        if (mapEventGui.length < 1) {
            return;
        }

        var splitEventStateData = playerMapEvent['stateData'].split(',');
        var collectedTotal = parseInt(splitEventStateData[0]);
        var collectedThisLevel = parseInt(splitEventStateData[1]);
        var campLevel = parseInt(splitEventStateData[2]);

        var splitEventData = playerMapEvent['data'].split(',');
        var lootID1 = splitEventData[0];
        var lootID2 = splitEventData[1];
        var lootID3 = splitEventData[2];
        var rerolled = parseInt(splitEventData[3]);
        
        var lootRevealData = _lootReveal[playerMapEvent.eventID];

        mapEventGui.find('.infos').empty();
        if (campLevel < lootRevealData.travellingCaravanMaxLevel) {
            mapEventGui.find('.infos').append('<div class="info collectedThisLevel"><span>' + collectedThisLevel + ' / ' +lootRevealData.travellingCaravanMaxToLevelUp + '</span> to next level</div>');
        } else {
            mapEventGui.find('.infos').append('<div class="info collectedThisLevel">Reached max level</div>');
        }

        //stats about how many collections possible in realm till today, and how many youve collected
        //if too behind, can use catchup option
        var collectedTotalYours = _lootReveal['collectedTotalYours'];
        var collectedTotalPossible = _lootReveal['collectedTotalPossible'];
        if (collectedTotalPossible) {
            mapEventGui.find('.infos').append('<div class="info collectedTotal"><span>' + collectedTotalYours + ' / ' + collectedTotalPossible + '</span> collected</div>');

            //if there is only 1 lootcamp on map, and additional spawns wouldnt go over max
            if (_countActiveLootcamps() < 2 && collectedTotalYours + lootRevealData['travellingCaravanCatchupSpawns'] <= collectedTotalPossible) {

                if (collectedTotalYours + 30 < collectedTotalPossible) {
                    //msg when behind a lot
                    mapEventGui.find('.infos').append('<div class="info youreBehindMsg">Looks like you\'ve missed a few Caravans. You can spawn ' + lootRevealData['travellingCaravanCatchupSpawns'] + ' more to catch up.</div>');
                } else {
                    //msg when there is more to collect
                    mapEventGui.find('.infos').append('<div class="info youreBehindMsg">You can collect today\'s Caravans faster, by spawning ' + lootRevealData['travellingCaravanCatchupSpawns'] + ' more.</div>');
                }

                
                if (ROE.Player.credits < lootRevealData['travellingCaravanCatchupCost']) { //when not enough servants for catchup
                    var catchUpBtn = $('<div class="noServCatchup"><div class="text">Not enough servants to use CatchUp.</div>');
                    catchUpBtn.click(function () {
                        $('.mapEventGui').remove();
                        ROE.Frame.showBuyCredits();
                    });
                } else {
                    var catchUpBtn = $('<div class="BtnBXLg1 fontButton1L catchUpBtn"><div class="text">Catch Up: costs ' + lootRevealData['travellingCaravanCatchupCost'] + '</div></div>');
                    catchUpBtn.click(function () {
                        ROE.Frame.busy('More Caravans!...', 5000, mapEventGui);
                        ROE.Api.call_playermapevent_caravan_catchup(playerMapEvent.eventID, _catchUpCallback);
                        $('.youreBehindMsg', mapEventGui).remove();
                        $(this).remove();
                    });
                }
                mapEventGui.find('.infos').append(catchUpBtn);
            }
        }


        //setup cards
        $('.card', mapEventGui).each(function (index) {
            var card = $(this);
            var cardIndex = parseInt(card.attr('data-index'));
            var lootDetail = _lootReveal[playerMapEvent.eventID]['loot' + cardIndex];
            if (!lootDetail) { return; }

            //set dynamic tooltip
            card.addClass('helpTooltip');
            ROE.Tooltips["caravanLootCard" + cardIndex] = lootDetail.name + "<br/>(" + lootDetail.trait + " card)";

            //setup card front content: add rarity class, lootID, and content
            card.find('.front').removeClass('common uncommon epic rare legendary');
            card.find('.front').addClass(lootDetail.trait).attr('data-lootID', lootDetail.lootID).html(
                '<div class="count">' + lootDetail.countString + '</div>' +
                '<div class="icon" style="background-image:url(' + lootDetail.icon + ');"></div>'
            );

            //setup cardpick handler
            card.unbind().click(function () {
                ROE.Frame.busy('Awarding...', 5000, mapEventGui);
                ROE.Api.call_playermapevent_caravan_cardpick(cardIndex, playerMapEvent.eventID, _cardPickCallback);
            });

            //flipem up!
            setTimeout(function () { card.addClass('flip') }, index * 200);

        });

        //setup reroll if they havent done it yet
        if (rerolled < 1) {
            if (ROE.Player.credits < 1) { //when not enough servants for reshuffle
                var noServ = $('<div class="noServ">Not enough servants to reshuffle.</div>');
                noServ.click(function () {
                    $('.mapEventGui').remove();
                    ROE.Frame.showBuyCredits();
                });
                $('.cards', mapEventGui).append(noServ);
            } else {
                var rerollBtn = $('<div class="BtnBXLg1 fontButton1L rerollBtn"><div class="text">Reshuffle: costs 1</div></div>');
                rerollBtn.click(function () {
                    ROE.Frame.busy('Reshuffling...', 5000, mapEventGui);
                    $('.cards .card', mapEventGui).removeClass('flip');
                    ROE.Api.call_playermapevent_caravan_cardreroll(playerMapEvent.eventID, _cardRevealCallback);
                });
                $('.cards', mapEventGui).append(rerollBtn);
            }
        } else {
            //if they cant reroll any more, clean up
            $('.noServ', mapEventGui).remove();
            $('.rerollBtn', mapEventGui).remove();
        }

        //reposition due to content change
        if (ROE.isD2) {
            if ($('.content', mapEventGui).hasClass('fromVOV')) {
                mapEventGui.css({
                    bottom: 270
                });
            } else {
                var yCord = playerMapEvent.yCord;
                mapEventGui.css({
                    top: Math.max((((ROE.Landmark.landpx * -yCord + ROE.Landmark.ry) - mapEventGui.height()) - (ROE.Landmark.landpx / 3)), 0)
                });
            }

        }

    }

    function _cardRevealCallback(data) {

        ROE.Frame.free($('.mapEventGui'));
        _singleMapEventUpdate(data.updatedEvent);
        _lootReveal[data.updatedEvent.eventID] = data; //store revealed data including loot detail

        //store general reveal data
        _lootReveal['collectedTotalYours'] = data.collectedTotalYours;
        _lootReveal['collectedTotalPossible'] = data.collectedTotalPossible;

        ROE.Frame.refreshPFHeader(data.credits);

        _fillCaravanContent(data.updatedEvent);
    }

    function _cardPickCallback(data) {

        ROE.Frame.free($('.mapEventGui'));

        //update events
        _commonEventReturn(data);

        //update player rewards
        ROE.Items2.update(data.playerItems);

        //Animate
        var cardElement = $('.mapEventGui .card[data-index="' + data.cardIndex + '"]');
        var boxH = 44;
        var boxW = 44;
        var left = cardElement.offset().left;
        var top = cardElement.offset().top;
        var claimDiv = $('<div class="eventClaimReward">');//.html('<div>+' + data.pickedLoot.itemGotCount + '</div>');

        $('body').append(claimDiv);
        claimDiv.css({
            left: left,
            top: top,
            height: boxH,
            width: boxW,
            'background-image': 'url(' + data.pickedLoot.icon + ')'
        });

        var rewardsIcon = $('#linkItems');

        $('.mapEventGui').remove();

        if (rewardsIcon.length) {

            var rewardsIconPos = rewardsIcon.offset();

            claimDiv.animate({ top: '-=30px' }, 300, "easeOutSine")
                .animate({ top: rewardsIconPos.top, left: rewardsIconPos.left, opacity: 1 }, 700, "easeInSine", function () {
                    $(this).remove();
                });

        } else {
            claimDiv.animate({ top: '-=30px' }, 1000, "easeOutSine").animate({ top: '-=5px', opacity: 0 }, 300, "easeOutSine", function () {
                $(this).remove();
            });
        }
    }

    function _catchUpCallback(data) {
        ROE.Frame.free($('.mapEventGui'));
        if (data.success) {
            ROE.Player.MapEvents = data.MapEvents;
            ROE.Player.regroupMapEvents();
            ROE.Landmark.refill(true);
            ROE.Frame.refreshPFHeader(data.credits);
        } else {
            ROE.Frame.errorBar("Catchup failed, no servants used.");
        }
    }

    var _processPlayerMapEvent = function _processPlayerMapEvent(playerMapEvent) {

        var select = $('#select');
        var selectPosition = select.position();
        var selectSize = select.width();
        var mapEventMaskID = "MEM" + playerMapEvent.xCord + playerMapEvent.yCord;

        $('<div id="' + mapEventMaskID + '" class="mapEventMask">').css({
            left: selectPosition.left,
            top: selectPosition.top,
            height: selectSize,
            width: selectSize
        }).appendTo('#surface');

        var cb;
        if (playerMapEvent.typeID == 1) { //a credit farm event
            cb = _creditFarmReturn;
        } else if (playerMapEvent.typeID == 2) {
            //type 2 is processed differently
            //_processPlayerMapEvent is not called when clicking a type 2
        }
        else if (playerMapEvent.typeID == 3) {
            cb = undefined; // not finished
        }
        ROE.Api.call_playermapevent_activate(playerMapEvent.eventID, playerMapEvent.typeID, playerMapEvent.xCord, playerMapEvent.yCord, cb);

    }

    ///Updates an individual map event in ROE.Player.MapEvents, with a given updated mapevent
    function _singleMapEventUpdate(singleEventData) {
        var ev;
        for (var i = 0; i < ROE.Player.MapEvents.length; i++) {
            ev = ROE.Player.MapEvents[i];
            if (ev.eventID == singleEventData.eventID) {
                ROE.Player.MapEvents[i] = singleEventData;
                ROE.Player.regroupMapEvents();
                //ROE.Landmark.refill(true); //do we need to do this yet?
                //_updateMapEventNotifications();
                return;
            }
        }
    }

    function _countActiveLootcamps() {
        var count = 0;
        for (var i = 0; i < ROE.Player.MapEvents.length; i++) {
            if (ROE.Player.MapEvents[i].typeID == 2) {
                count++;
            }
        }
        return count;
    }

    function _commonEventReturn(data) {
        //remove the map coord mask
        $("#MEM" + data.xCord + data.yCord).remove();

        //update some player properties with new data
        ROE.Player.MapEvents = data.MapEvents;
        ROE.Player.regroupMapEvents();

        //updates canvas, ROE.Landmark.refresh(); was too slow since it made an ajax call
        //based on the updated .MapEvents it'll draw new events 
        ROE.Landmark.refill(true);
        _updateMapEventNotifications();
        _checkAddVOVCaravan();
    }

    //MapEvent callback: CreditFarm 
    var _creditFarmReturn = function _creditFarmReturn(data) {

        _commonEventReturn(data);

        //data.result will be a string in "x,y" format where X = players updated total credits, and Y = credits claimed in this call
        var results = data.result.msg.split(',');
        var creditsUpdated = parseInt(results[0]);
        var creditsClaimed = parseInt(results[1]);

        //in a credit farm call a claim of 0 was probably erroneous, or doesnt need any further action
        if (creditsClaimed < 1) { return; }

        //Animate the claim
        var xCord = data.xCord;
        var yCord = data.yCord;
        var boxH = 25;
        var boxW = 20;
        var left = (ROE.Landmark.landpx * xCord + ROE.Landmark.rx) + (ROE.Landmark.landpx / 2) - (boxW / 2);
        var top = ROE.Landmark.landpx * -yCord + ROE.Landmark.ry;
        var credElementPos = $('.playerCredits').offset();
        var claimDiv = $('<div class="eventClaimDiv">').html('+' + creditsClaimed);
        $('body').append(claimDiv);
        claimDiv.css({
            left: left,
            top: top,
            height: boxH,
            width: boxW
        }).animate({ top: credElementPos.top, left: credElementPos.left, opacity: .5 }, 1000, "easeOutSine", function () {
            $(this).remove();
            ROE.Frame.refreshPFHeader(creditsUpdated);
        });
    }


    function _updateMapEventNotifications(forceShow) {
        //forceShow will make sure the notifications are reshown, even if Xed out

        $('#eventNotifWrapper').empty();
        if (forceShow) {
            $('#eventNotifWrapper').data('closedTypes', {});
        }

        var mapEvent;
        for (var i = 0; i < ROE.Player.MapEvents.length; i++) {
            mapEvent = ROE.Player.MapEvents[i];
            if (mapEvent.typeID == 1) {
                _addMapEventNotification(1);
                break;
            }
        }
    }
    
    //if a caravan exists in map events, add it to vov
    function _checkAddVOVCaravan() {
        $('#vovCaravan').remove();
        var vov = $('#vov');
        if (vov.length) {
            var mapEvent;
            for (var i = 0; i < ROE.Player.MapEvents.length; i++) {
                mapEvent = ROE.Player.MapEvents[i];
                if (mapEvent.typeID == 2) {
                    var vovCaravan = $('<div id="vovCaravan"></div>').click(function () {
                        _vovCaravanClick();
                    });
                    vov.append(vovCaravan);
                    return;
                }         
            }
        }
    }
    
    //the reason we do it this way, so we can make sure it gets latest mapevent data
    function _vovCaravanClick() {
        $('.mapEventGui').remove();
        var mapEvent;
        for (var i = 0; i < ROE.Player.MapEvents.length; i++) {
            mapEvent = ROE.Player.MapEvents[i];
            if (mapEvent.typeID == 2) {
                _caravanClicked(mapEvent, true);
                return;
            }
        }
    }

    function _addMapEventNotification(typeID) {
        var mapElement = $('#mapwrap');

        if (mapElement.length == 0) { return; }

        var eventNotifWrapper = $('#eventNotifWrapper');
        if (eventNotifWrapper.length == 0) {
            eventNotifWrapper = $('<div id="eventNotifWrapper" class="fontSilverFrLCmed">').data('closedTypes', {}).appendTo(mapElement);
        }

        var notif = $('<div class="notif">');
        switch (typeID + "") {
            case "1":
                var closedOnce = eventNotifWrapper.data('closedTypes')["1"];
                if (closedOnce) {
                    return;
                }
                var creditsTotal = 0;
                for (var i = 0; i < ROE.Player.MapEvents.length; i++) {
                    if (ROE.Player.MapEvents[i].typeID == 1) { //credit farm event
                        creditsTotal += parseInt(ROE.Player.MapEvents[i].data);
                    }
                }

                //Special Credit Farm bonus events
                //the date, multiplier, desc and icon come from realm attrs 57,58,59,60 -farhad
                if (ROE.Realm.creditFarmBonusDateEnds > (new Date()) && ROE.Realm.creditFarmBonusDesc) {
                    notif.css('background-image', 'url(' + ROE.Realm.creditFarmBonusIcon + ')')
                        .html(ROE.Realm.creditFarmBonusDesc + ' Unclaimed: ' + creditsTotal + '<br>Event ends in: <span class="countdown" data-finishesOn="' + ROE.Realm.creditFarmBonusDateEnds + '"></span>');
                } else {
                    var icon = 'https://static.realmofempires.com/images/icons/servantCarry_m.png';
                    notif.css('background-image', 'url(' + icon + ')').html('You have ' + creditsTotal + ' rescued servants on the map!');
                }

                break;
        }

        var why = $('<div class="btn why">?</div>').click(function (e) {
            e.stopPropagation();
            ROE.Frame.mapNotifWhy(typeID);
        });

        var close = $('<div class="btn close">X</div>').click(function (e) {
            e.stopPropagation();
            var closedTypes = eventNotifWrapper.data('closedTypes');
            closedTypes[typeID] = "true";
            eventNotifWrapper.data('closedTypes', closedTypes);
            notif.stop().animate({ 'right': '-=300px', 'opacity': 0 }, 200, function () {
                notif.remove();
            });
        });

        notif.append(why, close);
        eventNotifWrapper.append(notif);
    }

    

/*
    function detectIE() {
        var ua = window.navigator.userAgent;

        // Test values; Uncomment to check result …

        // IE 10
        // ua = 'Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Trident/6.0)';

        // IE 11
        // ua = 'Mozilla/5.0 (Windows NT 6.3; Trident/7.0; rv:11.0) like Gecko';

        // Edge 12 (Spartan)
        // ua = 'Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.71 Safari/537.36 Edge/12.0';

        // Edge 13
        // ua = 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2486.0 Safari/537.36 Edge/13.10586';

        var msie = ua.indexOf('MSIE ');
        if (msie > 0) {
            // IE 10 or older => return version number
            return parseInt(ua.substring(msie + 5, ua.indexOf('.', msie)), 10);
        }

        var trident = ua.indexOf('Trident/');
        if (trident > 0) {
            // IE 11 => return version number
            var rv = ua.indexOf('rv:');
            return parseInt(ua.substring(rv + 3, ua.indexOf('.', rv)), 10);
        }
    }
*/




    obj.checkPlayerMapEvents = _checkPlayerMapEvents;
    obj.updateMapEventNotifications = _updateMapEventNotifications;
    obj.checkAddVOVCaravan = _checkAddVOVCaravan;


}(window.ROE.MapEvents = window.ROE.MapEvents || {}));