

(function (ROE) {
}(window.ROE = window.ROE || {}));

(function (obj) {
    BDA.Console.setCategoryDefaultView('ROE.RAIDS', false);


    var _raidRrarity = {
        '0': 'Common',
        '1': 'UnCommon',
        '2': 'Rare',
        '3': 'Epic',
        '4': 'Legendary'
    }

    var _raidType = {
        '0': 'Solo',
        '1': 'Clan',
        '2': 'Global'
    }

   
    var _playerRaidsData = []; //holds latest raids info

    var _showPopup = function () {

        var content = $('<div class="raidsContent">' +
            '<div class="header fontSilverFrLClrg">'+
                '<div>Active Raids</div>' +
            '</div>' +

            '<div class="list"></div>' +

            '<div class="links">' +
                /*
                '<a class="link uv fontGoldFrLCsm" href="https://roebeta.uservoice.com/forums/273298-beta-ui-feedback" target="uv">' +
                    'Got ideas to improve Raids?</a>' +*/
            '</div>' +

        '</div>');

        ROE.Frame.popDisposable({
            ID: 'raidsDialog',
            title: 'Raids',
            content: content,
            width: 380,
            height: 500,
            modal: false,
            contentCustomClass: 'raidsDialogContent'
        });

        $('#raidsDialog').dialog({ maxHeight: 620 });

        if (ROE.isMobile) {
            var uvLink = $('<div class="link blog fontGoldFrLClrg">Click here to read more about Raids</div>').click(function () {
                ROE.Frame.popGeneric('Raids Blog', '', 320, 320);
                ROE.Frame.showIframeOpenDialog('#genericDialog', 'http://realmofempires.blogspot.ca/2017/09/raids.html');
            });
            $('#raidsDialog').find('.raidsContent .links').append(uvLink);
        } else {
            $('#raidsDialog').find('.raidsContent .links').append('<a class="link blog fontGoldFrLCsm" href="http://realmofempires.blogspot.ca/2017/09/raids.html" target="uv">' +
            'Click here to read more about Raids.</a><br/>');
        }



        $('#raidsDialog').parent().append();

        _load();

    }

    function _load() {

        ROE.Frame.busy('Getting raid info...', 5000, $('#raidsDialog'));

        ROE.Api.call_raids_getPlayerRaids(function (cbData) {
            ROE.Frame.free($('#raidsDialog'));
            _playerRaidsData = cbData.playerRaids;
            _populateRaidsList();
        })

    }




    function _populateRaidsList() {

        var _container = $('#raidsDialog');

        if (!_container.dialog('isOpen')) {
            return;
        }


        var contentList = _container.find('.list').empty();

        if (_playerRaidsData.length) {

            $.each(_playerRaidsData, function _addRow(index, raid) {
                contentList.append( _makeRaidRow(raid));
            });

        } else {
            contentList.html('<div class="noRaids fontSilverFrLClrg">No active Raids available.</div>');
        }


    }

    function _makeRaidRow(raidObject) {

        var raidRow = $('<div class="raidRow" data-raidid="' + raidObject.raidID + '">' +
                '<div class="icon" style="background-image:url(' + raidObject.imageUrlIcon + ')"></div>' +

                '<div class="name fontSilverFrLCXlrg">' + raidObject.name + '</div>' +
                '<div class="desc fontSilverFrLClrg">' + raidObject.desc + '</div>' +

                '<div class="status">' +
                    '<div class="raidRarity ' + _raidRrarity[raidObject.raidRarity] + '">[' + _raidRrarity[raidObject.raidRarity] + ']</div>' +
                    '<div class="raidType ' + _raidType[raidObject.raidType] + '">[' + _raidType[raidObject.raidType] + ' Raid]</div>' +
                    '<div class="raidExpire" > Expires: ' + '<span class="expiration countdown" format="long2"></span>' + '</div>' +
                '</div>' +

                '<div class="health">' +
                    '<div class="barOuter"><div class="barInner"></div></div>' +
                    '<div class="barLabel">HP ' + BDA.Utils.formatNum(raidObject.currentHealth) + ' / ' + BDA.Utils.formatNum(raidObject.maxHealth) + '</div>' +
                '</div>' +
            + '</div>');

        var perc = raidObject.currentHealth / raidObject.maxHealth * 100;
        raidRow.find('.barInner').css('width', perc + '%');

        raidRow.click(function () {
            _openRaid(raidObject);
        });

        if (raidObject.currentHealth > 0) {

            //Expiration check
            if (raidObject.actByDuration === 0) { //raid not expirable
                raidRow.find('.raidExpire').remove();
            } else if (raidObject.expirationDateMilli > Date.now().valueOf()) { //raid active and not expired 
                raidRow.find('.expiration').attr("data-finisheson", raidObject.expirationDateMilli);
            } else { //expired
                raidRow.find('.raidExpire').html('Expired');
                raidRow.addClass('expired');
            }

        } else {
            //no expiration for beaten raids
            raidRow.find('.raidExpire').html('Raid Beaten!');
            raidRow.addClass('beaten');
        }

        return raidRow;

    }

    //silently get latest raid data and update list
    //if given a raid ID, update the raid panel too
    var _silentUpdateTimeout;
    function _silentUpdate(raidID) {

        clearTimeout(_silentUpdateTimeout);

        _silentUpdateTimeout = setTimeout(function () {

            //console.log('_silentUpdate', raidID);

            ROE.Api.call_raids_getPlayerRaids(function _silentUpdateCB(cbData) {

                _playerRaidsData = cbData.playerRaids;

                var container = $('#raidsDialog');

                if (!container.dialog('isOpen')) {
                    return;
                }

                _populateRaidsList();

                if (raidID) {
                    var raidPanel = $('.raidPanel', container);
                    if (raidPanel.length) {
                        var raidObject = _getRaidByRaidID(raidID);
                        _openRaid(raidObject); //this will lead to an api call update of movements
                    }
                }
            });

        }, 500); //throttle 

    }


    function _openRaid(raid) {

        //console.log('_openRaid:', raid);

        var container = $('#raidsDialog');

        if (!container.dialog('isOpen')) {
            return;
        }

        var raidPanel = $('.raidPanel', container);
        if (raidPanel.length) { //if already exists, this is an update

            var radiPanelRaidData = raidPanel.data('raid');

            //discard update call if raidIDs dont match
            if (radiPanelRaidData.raidID != raid.raidID) {
                return;
            }

            raidPanel.empty();
        } else {
            raidPanel = $('<div class="raidPanel" ></div>');
            $('.raidsDialogContent', container).append(raidPanel);
        }

        raidPanel.css('background-image', 'url(' + raid.imageUrlBG + ')');

        raidPanel.data('raid', raid);

        raidPanel.append(

            '<div class="subPanel head">' +
                //'<div class="icon" style="background-image:url(' + raid.imageUrlIcon + ')"></div>' +
                
                '<div class="name fontSilverFrLCXlrg">' + raid.name + '</div>' +
                '<div class="desc fontSilverFrLClrg">' + raid.desc + '</div>' +

                '<div class="status">' +
                    '<div class="raidRarity '+ _raidRrarity[raid.raidRarity] +'">[' + _raidRrarity[raid.raidRarity] + ']</div>' +
                    '<div class="raidType ' + _raidType[raid.raidType] + '">[' + _raidType[raid.raidType] + ' Raid]</div>' +
                    '<div class="raidExpire" > Expires: ' + '<span class="expiration countdown"  format="long2"></span>' + '</div>' +
                '</div>' +

                '<div class="health">' +
                    '<div class="barOuter"><div class="barInner"></div></div>' +
                    '<div class="barLabel">HP ' + BDA.Utils.formatNum(raid.currentHealth) + ' / ' + BDA.Utils.formatNum(raid.maxHealth) + '</div>' +
                '</div>' +

            '</div>'+

            '<div class="subPanel details">' +
                '<div class="label fontGoldFrLClrg">Details</div>' +
                //'<div>Each ' + raid.strengthToBeat  + ' Attack Power sent deals 1HP damage.</div>' +
                (raid.casualtyPerc > 0 ? '<div class="perish">' + raid.casualtyPerc + '% of troops sent perish per attack.</div>' : '') + //display casualty detail
                '<div>Travel distance is ' + raid.distance + ' squares.</div>' +
            '</div>' +

            '<div class="subPanel rewards">' +
                '<div class="label fontGoldFrLClrg">Rewards</div>' +
                '<div class="rewardsList"> - - - </div>' +
                '<div class="acceptReward BtnBLg2 fontButton1L">Collect Rewards</div>' +
                '<div class="loadMask"></div>' +
            '</div>' +

            '<div class="subPanel attack">' +
                '<div class="sendAttack BtnBLg1 fontButton1L">Attack!</div>' +
            '</div>' +

            '<div class="subPanel outgoing">' +
                '<div class="label fontGoldFrLClrg">Attacks in progress</div>' +
                '<div class="totalOutgoing"><span>0</span> Total damage outgoing.</div>' +
                '<div class="outgoingList"> - - - </div>' +
                '<div class="loadMask"></div>' + 
            '</div>' +

            '<div class="subPanel history">' +
                '<div class="label fontGoldFrLClrg">Raid History</div>' +
                '<div class="resultList"> - - - </div>' +
                '<div class="loadMask"></div>' +
            '</div>' +

            '<div class="close"></div>'

        );

        //close panel btn
        raidPanel.find('.close').click(function () {
            raidPanel.remove();
        });

        //set HP
        var perc = raid.currentHealth / raid.maxHealth * 100;
        raidPanel.find('.barInner').css('width', perc + '%');

        //Expiration check
        if (raid.currentHealth > 0) {

            if (raid.actByDuration === 0) { //raid not expirable
                raidPanel.find('.raidExpire').remove();
            } else if (raid.expirationDateMilli > Date.now().valueOf()) { //raid active and not expired 
                raidPanel.find('.expiration').attr("data-finisheson", raid.expirationDateMilli);
            } else { //expired
                raidPanel.find('.raidExpire').html('Expired');
                raidPanel.addClass('expired');
                raidPanel.find('.sendAttack').remove();
                raidPanel.find('.subPanel.attack').append('<div class="cantattackexpired">Raid Expired</div>');

            }

        } else {
            //no expiration for beaten raids
            raidPanel.find('.raidExpire').html('Raid Beaten!');
            raidPanel.addClass('beaten');
        }

        //if raid still alive
        if (raid.currentHealth > 0) {

            raidPanel.find('.sendAttack').click(function () {
                _openAttackPanel(raid);
            });

            raidPanel.find('.acceptReward').remove();

        } else { //else if raid is beaten

            raidPanel.find('.subPanel.attack').remove();

            if (raid.acceptedReward) {

                raidPanel.find('.acceptReward').replaceWith("<div class='rewardAccepted'>Reward Accepted</div>");

            } else {

                //if raid was beaten and you did some work
                if (raid.totalPlayerDamage > 0) {

                    raidPanel.find('.acceptReward').click(function () {

                        ROE.Frame.busy('Looting...', 5000, $('#raidsDialog'));

                        ROE.Api.call_raids_acceptRewards(raid.raidID, function acceptRewardsCallback(data) {

                            ROE.Frame.free($('#raidsDialog'));

                            if (data.acceptResult == true) {
                                raid.acceptedReward = true;

                                //rebuild raid panel
                                _openRaid(raid);

                                //animate rewards
                                $.each(raid.raidRewards, function rewardAnimation(i, r) {
                                    var startPosElement = raidPanel.find('.rewardAccepted');

                                    var endPosElement = $('#linkItems');
                                    var endPosTop = 0;
                                    var endPosLeft = 0;
                                    if (endPosElement.length) {
                                        endPosTop = endPosElement.offset().top;
                                        endPosLeft = endPosElement.offset().left;
                                    }

                                    var seed = Math.random();
                                    var timeDelay = (i * 300) + (50 * seed);
                                    setTimeout(function () {
                                        ROE.Frame.spawnFlyIcon({
                                            parentElement: null,
                                            startPos: { top: startPosElement.offset().top, left: startPosElement.offset().left + (startPosElement.width() / 2) + (30 - 60 * seed) },
                                            iconUrl: r.icon,
                                            endPos: { top: endPosTop, left: endPosLeft }
                                        });
                                    }, timeDelay);
                                });

                                ROE.Player.refresh();

                            }
                        });
                    });

                //if raid was beaten but you did no damage
                } else {

                    //raidPanel.find('.acceptReward').replaceWith("<div class='noParticipation'>You did no damage to this raid.</div>");
                    raidPanel.find('.acceptReward').remove();
                }
            }
        }
        
        _populateRaidRewards(raid, raidPanel.find('.rewardsList'));
        _populateRaidResults(raid, raidPanel.find('.resultList'));
        _populateRaidMovements(raid, raidPanel.find('.outgoingList'));

        //update the raid row in the list as well
        var raidListRaidRow = $('#raidsDialog .list .raidRow[data-raidid="' + raid.raidID + '"]');
        if (raidListRaidRow.length) {
            raidListRaidRow.replaceWith(_makeRaidRow(raid));
        }


        //!!!do this post append
        //if raid object hasnt had its details filled in, make a call to do so
        if (!raid.hasDetails) {

            raidPanel.find('.outgoing, .rewards, .history').addClass('loading');
 
            ROE.Api.call_raids_getraiddetails(raid.raidID,
                function getRaidDetailsCB(data) {
                   
                    if (data.raidID != raid.raidID) {
                        return;
                    }
                    //console.log('getRaidDetailsCB', data);
                    _openRaid(data.raid);
                    
                }
            );

        }

    
    }

    function _populateRaidRewards(raidObject, listElement) {

        listElement.parent().removeClass('loading');
        listElement.empty();
        if (raidObject.raidRewards.length) {

            if (_raidType[raidObject.raidType] == "Solo") {

                if (raidObject.currentHealth > 0){
                    var soloRewardPanel = $('<div class="soloRewardPanel"></div>');
                    $('.label', listElement.parent()).after(soloRewardPanel);
                    soloRewardPanel.append('Once the raid is beaten you get: ');
                }

                var raidRewardObject, raidRewardElement;
                for (var rri = 0; rri < raidObject.raidRewards.length; rri++) {
                    raidRewardObject = raidObject.raidRewards[rri];
                    raidRewardElement = $('<div class="raidReward" style="background-image:url(' + raidRewardObject.icon + ')"> ' +
                        BDA.Utils.formatNum(raidRewardObject.count) + ' ' + raidRewardObject.label + '</div>');
                    raidRewardElement.appendTo(listElement);
                }

                $('#raidsDialog .raidPanel').data('optimalDamage', 1 * raidObject.maxHealth);

            }else if (_raidType[raidObject.raidType] == "Clan"){
                
                var clanRewardPanel = $('<div class="clanRewardPanel"></div>');
                $('.label', listElement.parent()).after(clanRewardPanel);

                var averagePlayerSize = raidObject.size / raidObject.playerCount;
                //a ratio of this player size vs average player size
                var inverseClampedSizeFactor = Math.min(1.25, Math.max(0.75, 1 / (raidObject.playerVillageCount / averagePlayerSize)));

                //percantage of damage this player did
                var totalDamageDone = raidObject.totalPlayerDamage;
                var percentageOfDamageDone = Math.min(1, totalDamageDone / raidObject.maxHealth);

                //diminishing factor for going over the soft cap
                var differenceDiminishFactor = 0.5;

                if (raidObject.currentHealth > 0) {


                    //var displayValue = 1 / raidObject.playerCount; //initial dmg value just even split of players
                    var displayValue = (1 / inverseClampedSizeFactor) * (1 / raidObject.playerCount); //optimal point
                    displayValue = Number(Math.round(displayValue + 'e' + 2) + 'e-' + 2);
                    displayValue = Math.min(displayValue, 1); //cap it at 100%

                    clanRewardPanel.append(
                        '<div class="c1">You have done <span class="v1">-%</span> damage so far.<br/>' +
                        'If your clan beats the raid, you get rewards based on your contribution. ' +
                        'Your optimal damage goal is <span class="v3">' + (Math.round(displayValue * 100)) + '%</span><br/>' +
                        'If you do <span class="v2">-%</span> damage you get:</div>');

                    $('#raidsDialog .raidPanel').data('optimalDamage', displayValue * raidObject.maxHealth);

                    //if player has already done more % than default value, show it instead
                    if (raidObject.totalPlayerDamage / raidObject.maxHealth > displayValue) {
                        displayValue = Math.min(1, raidObject.totalPlayerDamage / raidObject.maxHealth);
                    }

                    var damageValueDisplaySlider = $('<div class="damageValueDisplaySlider"></div>').slider({
                        value: (displayValue * 100),
                        slide: function (event, ui) {
                            _rewardsForPercentage(ui.value / 100);
                        }
                    });

                    clanRewardPanel.append(damageValueDisplaySlider);

                } else {
                    clanRewardPanel.append('<div class="c1">You did <span class="v1">-%</span> <span class="vt">(' + BDA.Utils.formatNum(raidObject.totalPlayerDamage) + ')</span>' +
                        ' of the clan raid damage.</div>');
                    var displayValue = Math.min(1, raidObject.totalPlayerDamage / raidObject.maxHealth);
                }

                _rewardsForPercentage(displayValue);

            } else if (_raidType[raidObject.raidType] == "Global") {
                listElement.html('Global Raid Rewards Not Implemented.');
            }

        } else {

            listElement.html('Undefined Raid Rewards');

        }

        function _rewardsForPercentage(damagePercantageDisplayValue) {

            listElement.empty();

            clanRewardPanel.find('.v1').html(Math.round(percentageOfDamageDone * 100) + '%');
            clanRewardPanel.find('.v2').html(Math.round(damagePercantageDisplayValue * 100) + '%');

            $.each(raidObject.raidRewards, function (i, raidRewardObject) {

                var rewardPool = raidRewardObject.count * raidObject.playerCount;
                var rewardPlayerShare = Math.ceil(inverseClampedSizeFactor * damagePercantageDisplayValue * rewardPool);
                var rewardPerPlayerSoftCap = raidRewardObject.count;
                var softCapOverage = Math.max(0, rewardPlayerShare - rewardPerPlayerSoftCap);
                var lootCount = 0;
                if (rewardPlayerShare <= rewardPerPlayerSoftCap) {
                    lootCount = rewardPlayerShare;
                }
                else {
                    lootCount = rewardPerPlayerSoftCap + Math.ceil(softCapOverage * differenceDiminishFactor);
                }

                raidRewardElement = $('<div class="raidReward" style="background-image:url(' + raidRewardObject.icon + ')"> ' +
                    BDA.Utils.formatNum(lootCount) + ' ' + raidRewardObject.label + '</div>');
                raidRewardElement.appendTo(listElement);

            });
        }

    }

    function _populateRaidResults(raidObject, listElement) {
        listElement.parent().removeClass('loading');
        listElement.empty();

        if (raidObject.raidResults.length) {

            //Solo Raid results display
            if (raidObject.raidType === 0) {

                _detailedView();

                //Clan Raid results display
            } else if (raidObject.raidType === 1) {

                _summaryView();

                var historyPanel = $('#raidsDialog .history');
                $('.label', historyPanel).html('Clan Raid History');

                var toggle = $('<div class="historyToogle">[Toggle View]</div>');
                toggle.click(function () {
                    if (toggle.hasClass('detailedView')) {
                        toggle.removeClass('detailedView');
                        _summaryView();
                    } else {
                        toggle.addClass('detailedView');
                        _detailedView();
                    }
                })
                $('.label', historyPanel).append(toggle);

            } else {
                listElement.html('No attacks done yet.');
            }

            function _detailedView() {
                listElement.empty();

                var resultTableElement = $('<table class="resultsTable" >' +
                                                '<tr class="headRow fontDarkGoldFrLCmed">' +
                                                    '<th class="name">Player</th>' +
                                                    '<th class="date">Date</th>' +
                                                    '<th class="damage">Damage done</th>' +
                                                '</tr>' +
                                            '</table>');

                listElement.append(resultTableElement);

                var aResult;
                for (var ri = 0; ri < raidObject.raidResults.length; ri++) {
                    aResult = raidObject.raidResults[ri];

                    resultTableElement.append('<tr class="result">' +
                            '<td class="name">' + aResult.playerName + '</td>' +
                            '<td class="date">' + BDA.Utils.formatEventTimeSimpleUTC(new Date(aResult.landTimeMilli)) + '</td>' +
                            '<td class="damage">' + BDA.Utils.formatNum(aResult.damageHP) + '</td>' +
                        '</tr>');

                }

            }

            function _summaryView() {

                listElement.empty();

                var resultPlayers = {}; //add up all raid results into their players
                var aResult;
                for (var i = 0; i < raidObject.raidResults.length; i++) {
                    aResult = raidObject.raidResults[i];
                    if (!resultPlayers[aResult.playerID]) {
                        resultPlayers[aResult.playerID] = {
                            playerName: aResult.playerName,
                            damageHP: 0
                        }
                    }
                    resultPlayers[aResult.playerID].damageHP += aResult.damageHP;

                }

                //put the entires in an array so we can sort easy
                var resultPlayerArray = [];
                for (var rP in resultPlayers) {
                    resultPlayerArray.push(resultPlayers[rP]);
                }
                resultPlayerArray.sort(function (a, b) {
                    return b.damageHP - a.damageHP;
                });

                //make a table using the data
                var resultTableElement = $('<table class="resultsTable" >' +
                                                '<tr class="headRow fontDarkGoldFrLCmed">' +
                                                    '<th class="name">Player</th>' +
                                                    '<th class="damage">Total Damage</th>' +
                                                    '<th class="percent">Total %</th>' +
                                                '</tr>' +
                                            '</table>');

                listElement.append(resultTableElement);

                var aResultPlayer;
                for (var ri = 0; ri < resultPlayerArray.length; ri++) {
                    aResultPlayer = resultPlayerArray[ri];

                    resultTableElement.append('<tr class="result">' +
                            '<td class="name">' + aResultPlayer.playerName + '</td>' +
                            '<td class="damage">' + BDA.Utils.formatNum(aResultPlayer.damageHP) + '</td>' +
                            '<td class="percent">' + Math.round(aResultPlayer.damageHP / raidObject.maxHealth * 100) + '%</td>' +
                        '</tr>');

                }

            }

        }

    }

    function _populateRaidMovements(raidObject, listElement) {
        listElement.parent().removeClass('loading');
        listElement.empty();

        //console.log('_populateRaidMovements', movementsListArray);

        /*

            movementsList : {        
                eventID
                landTimeMilli
                originVillageID
                playerID
                raidID
                startTimeMilli
                unitsList: Array[{ count, utID }]
                villageName
                villageXCord
                villageYCord
            }

        */

        var movementsListArray = raidObject.raidMovements;
        var totalOutgoingDamage = 0;
        if (movementsListArray.length) {

            var movementObject;
            var movementElement;
            for (var i = 0; i < movementsListArray.length; i++) {
                movementObject = movementsListArray[i];
                movementElement = $('<div class="movement">' +
                        '<div class="info">' +
                            '<div class="vill">' +
                                '<span class="villName fontGoldFrLCmed">' + movementObject.villageName + '</span>' +
                                '<span class="villCoors fontDarkGoldNumbersSm"> (' + movementObject.villageXCord + ',' + movementObject.villageYCord + ')</span>' +
                            '</div>' +
                            '<div class="troops"></div>' +
                        '</div>' +
                        '<div class="progress">' +
                            '<div class="barOuter"><div class="barInner countdownProgressBar id_movement_event_' + movementObject.eventID + '"></div></div>' +
                            '<div class="barLabel">' +
                                '<span class="countdown"  data-progressid="movement_event_' + movementObject.eventID + '" data-refreshcall="ROE.Raids.silentUpdate(' + raidObject.raidID + ');" ' + //format="long2"
                                ' data-totalms="' + (movementObject.landTimeMilli - movementObject.startTimeMilli) + '" data-finisheson="' + movementObject.landTimeMilli + '"></span>' +
                            '</div>' +
                        '</div>' +
                    +'</div>');

                //show units in movement
                for (var ui = 0; ui < movementObject.unitsList.length; ui++) {
                    totalOutgoingDamage += ROE.Entities.UnitTypes[movementObject.unitsList[ui].utID].AttackStrength * movementObject.unitsList[ui].count;
                    $('.troops', movementElement).append('<div class="troop" style="background-image:url(' + ROE.Entities.UnitTypes[movementObject.unitsList[ui].utID].IconUrl_ThemeM + ')">' +
                        movementObject.unitsList[ui].count +
                    '</div>');
                }


                /*               
                //no need for manual update, uts auto updated by countdown
                var timeprogress = (Date.now() - movementObject.startTimeMilli) / (movementObject.landTimeMilli - movementObject.startTimeMilli) * 100;
                var timeprogressRounded = Math.min((Math.round(timeprogress * 100) / 100), 100); //round it for ease, then clamp it to 100%
                $('.barInner', movementElement).css('width', timeprogressRounded + '%');
                */

                listElement.append(movementElement);

            }
        }

        var raidPanel = $('#raidsDialog .raidPanel').data('totalOutgoingDamage', totalOutgoingDamage);
        listElement.parent().find('.totalOutgoing span').html(BDA.Utils.formatNum(totalOutgoingDamage));

    }



    function _openAttackPanel(raid) {

        var container = $('#raidsDialog');

        var attackPanel = $('<div class="attackPanel" ></div>');

        attackPanel.append(

             '<div class="head">' +
                //'<div class="name fontSilverFrLCXlrg">Village List</div>' +
                '<div class="desc fontSilverFrLCXlrg">Villages with raid-able troops</div>' +
                '<div class="total fontSilverFrLCmed">You have <span>' + ($('#raidsDialog .outgoing .totalOutgoing span').text() ) + '</span> damage currently outgoing to this raid.</div>' +
             '</div>' +

             '<div class="villageList">' +
             '</div>' +

             '<div class="villageListMask">' +
             '</div>' +

             '<div class="footer">' +
                '<div class="checkBox checked"></div>' +
                '<div class="label fontSilverFrLCmed">Prevent sending more than optimal damage</div>' +
             '</div>' +

             '<div class="close"></div>'

         );

        attackPanel.find('.close').click(function () {
            attackPanel.remove();
        });

        attackPanel.find('.checkBox').click(function () {

            var checkBox = $(this);

            if (checkBox.hasClass('checked')) {
                checkBox.removeClass('checked');
                $('#raidsDialog .attackPanel').addClass('overRule');
            } else {
                checkBox.addClass('checked');
                $('#raidsDialog .attackPanel').removeClass('overRule');

            }

            _updateAttackPanel(raid);
        });


        $('.raidsDialogContent', container).append(attackPanel);

        _buildAttackList(raid);
        _updateAttackPanel(raid);

    }

    function _buildAttackList(raid) {

       
        var listOfVillages; //array of village objects
        var villageIndex = 0; //the village index from village list to load
        var loadRange = 10; //inititally load 10 vills

        var villageListContainer = $('#raidsDialog .villageList');
        BDA.Broadcast.subscribe(villageListContainer, "VillageExtendedInfoUpdated", _handleVillageExtendedInfoUpdatedOrPopulated);

        //setup scoll to load more
        villageListContainer.on("scroll", function (event) {
            var maxScroll = $(this).prop('scrollHeight') - $(this).outerHeight();
            if ($(this).scrollTop() >= maxScroll) {
                //make sure not already in load cycle
                if (villageListContainer.hasClass('canLoadMoreNow')) {  
                    loadRange += 10;
                    _loadNextVillage(villageIndex);
                }
            }
        });

        ROE.Frame.busy('Getting Village List...', 5000, villageListContainer);

        ROE.Villages.getVillages(_listIsReady);

        //the get villagelist callback
        function _listIsReady(dataListOfVillages) {
            listOfVillages = dataListOfVillages;
            _loadNextVillage(villageIndex);
            ROE.Frame.free(villageListContainer);
        }

        function _loadNextVillage() {

            //if attack panel is closed, stop.
            if ($('#raidsDialog .attackPanel').length < 1) {
                return;
            }

            villageListContainer.removeClass('canLoadMoreNow');

            //end of village list 
            if (villageIndex >= listOfVillages.length) {    
                return;
            }

            //end of load range
            if ($('.villageRow ').length >= loadRange) {
                villageListContainer.addClass('canLoadMoreNow');
                return;
            }


            var vill = listOfVillages[villageIndex];
            var villElement = _makeVillElement(vill);

            villElement.append('<div class="loader"></div>');
            villageListContainer.append(villElement);
 
            villageIndex++;
            ROE.Villages.getVillage(vill.id, function _getVillageCB(fullVillageData) {
                _villageLoaded(fullVillageData, villElement);
                _loadNextVillage();
            }, ROE.Villages.Enum.ExtendedDataRetrieveOptions.ensureExistsAndTriggerGetLatest);

        }

        function _makeVillElement(vill) {

            var villRow = $('<div class="villageRow fontGoldFrLCmed " data-villageid="' + vill.id + '">' +
                    '<div class="icon" style="background-image:url(' + BDA.Utils.GetMapVillageIconUrl(vill.points, vill.villagetypeid) + ')"></div>' +
                    '<div class="info">' + vill.name + '<span class="villCoors fontDarkGoldNumbersSm"> (' + vill.x + ',' + vill.y + ')</span></div>' +
                    '<div class="troops">checking troops...</div>' +
                    '<div class="sendInfo"></div>' +
                '</div>');

            var launchButton = $('<div class="launchRaid BtnBMed1 fontButton1L">Send!</div>').click(function () {
                _launchRaid(villRow);
            });

            villRow.append(launchButton);

            //villRow.data('vill', vill);
            //villRow.data('status','new');

            return villRow;
        }

        function _villageLoaded(vill, villElement) {

            villElement.find('.loader').remove();

            //console.log('_villageLoaded', vill.id, villElement);

            var troopContainer = villElement.find('.troops').empty();
            var troopList = vill.TroopList;
            var troop;
            var troopElement;
            var unitEntity;
            var troopAttackPower = 0;
            var totalAttackPower = 0;

            for (var t = 0; t < troopList.length; t++) {

                troop = troopList[t];

                //only certain troops count
                if (troop.id == 2 /*Infantry*/ || troop.id == 5 /*LC*/
                    || troop.id == 6 /*KN*/ || troop.id == 11 /*CM*/ ) {

                    unitEntity = ROE.Entities.UnitTypes[troop.id];
                    troopAttackPower = troop.YourUnitsCurrentlyInVillageCount * unitEntity.AttackStrength;
                    totalAttackPower += troopAttackPower;

                    troopElement = $('<div class="troop ' + (troopAttackPower > 1 ? 'hasAP' : 'zeroAP') + '" data-ap="' + troopAttackPower + '" data-traveltime="'+ ( raid.distance / unitEntity.Speed ) + '">' +
                            '<div class="tIcon" style="background-image:url(' + unitEntity.IconUrl_ThemeM + ')"></div>' +
                            '<div class="tCount">' + troop.YourUnitsCurrentlyInVillageCount + '</div>' +
                        '</div>');

                    troopElement.data('troopID', troop.id);
                    troopElement.data('troopCount', troop.YourUnitsCurrentlyInVillageCount);

                    troopElement.click(function () {
                        _troopSelectionChange($(this), villElement);
                    });

                    troopContainer.append(troopElement);

                }
          
            }

            //if village has enough attack power for 1 attack
            if (totalAttackPower < 1) {

                //villElement.hide();
                //return;
           
                villElement.addClass('notEnoughAttackPower');
                villElement.find('.troops').html('Village has no Raiding troops');

                /*
                villElement.animate({ opacity: 0.5 }, 2000, function () {
                    //villElement.remove();
                });
                */

            } else {
                villElement.removeClass('notEnoughAttackPower');

                //villElement.stop().css({ 'opacity': 1 });
                //villElement.show();
            }


        }

        function _troopSelectionChange(troopElement, villRow) {

            //dont allow selecting 0 count troops
            if (troopElement.hasClass('zeroAP')) {
                return;
            }

            //clean troops selected form other rows
            villageListContainer.find('.troop').not($('.troop', villRow)).removeClass('selected');
            villageListContainer.find('.villageRow').removeClass('selected launchReady');

            if (troopElement.hasClass('selected')) {
                troopElement.removeClass('selected');
            } else {
                troopElement.addClass('selected');
            }

            var villRowTotalAP = 0;
            var villRowTravelTime = 0;

            $('.troop.selected', villRow).each(function () {
                villRowTotalAP += parseInt($(this).attr('data-ap'));
                var travelTime = parseFloat($(this).attr('data-traveltime'));
                if (travelTime > villRowTravelTime) {
                    villRowTravelTime = travelTime;
                }
            });

            if (villRowTotalAP > 0) {
                villRow.addClass('selected launchReady');
            }

            var villRowTravelTimeString;
            if (villRowTravelTime >= 1) {
                villRowTravelTimeString = '<span>' + (+villRowTravelTime.toFixed(2)) + '</span> hours travel time.';
            } else {
                villRowTravelTimeString = '<span>' + (+(villRowTravelTime * 60).toFixed(2)) + '</span> minutes travel time.';
            }

            villRow.find('.sendInfo').html(
                '<div class="selectedTotalDamge"><span>' + BDA.Utils.formatNum(villRowTotalAP) + '</span> damage selected.</div>' +
                '<div class="selectedTravelTime">' + villRowTravelTimeString + '</div>'
            );


        }

        function _launchRaid(villRow) {

            var villageID = villRow.attr('data-villageid');

            var raidID = raid.raidID;

            var attackUnitList = [];
            villRow.find('.troop.selected').each(function () {
                var troop = $(this);
                attackUnitList.push({ utID: troop.data('troopID'), sendCount: troop.data('troopCount'), targetBuildingTypeID: 1 });
            });

            
            _sendRaid(villageID, raidID, attackUnitList);
        }

        function _sendRaid(fromVillageID, raidID, attackUnitList) {
            ROE.Frame.busy('Sending...', 5000, $('#raidsDialog'));
            ROE.Api.call_raids_sendRaid(fromVillageID, raidID, $.toJSON(attackUnitList), _sendRaidCallback);
        }

        function _sendRaidCallback(sentRaidData) {


            var container = $('#raidsDialog');

            if (!container.dialog('isOpen')) {
                return;
            }

            ROE.Frame.free(container);
         
            //result 0 => success 
            //result 1 => village missing troop type issue (in iSend sp)
            //result 2 => attack unit issue
            //result 3 => raid not found
            //result 4 => raid already beaten
            //result 5 => raid expired

            //success
            if (sentRaidData.sendResults.result === 0) {

                var villageRow = $('.villageRow[data-villageid="' + sentRaidData.villageID + '"]');
                villageRow.find('.troop.selected').each(function () {
                    $(this).removeClass('hasAP selected').addClass('zeroAP').find('.tCount').html('0');
                });
                ROE.Frame.infoBar('Raid attack sent successfully.');
                
                //if raid panel is open, and IDs match, update its movement list section!
                var raidPanel = $('.raidPanel', container);
                if (raidPanel.length) { 
                    //var raidObject = _getRaidByRaidID(sentRaidData.raidID); //raid object associated with raidID of callback
                    var raidPanelRaidData = raidPanel.data('raid'); //raidObject of panel that is open
                    if (raidPanelRaidData.raidID == sentRaidData.raidID) {
                        _openRaid(sentRaidData.sendResults.raid);
                        _updateAttackPanel(sentRaidData.sendResults.raid);


                    }
                }

                ROE.Villages.getVillage(sentRaidData.villageID, function _getVillageCB(fullVillageData) {
                    /* doesnt seem to be getting true latest troops fast enough */
                    //_villageLoaded(fullVillageData, villageRow); 
                    //but it does trigger the event for village info change!
                }, ROE.Villages.Enum.ExtendedDataRetrieveOptions.ensureExistsAndTriggerGetLatest);

                //can also consider get latest village data from isend and setting it here:
                //ROE.Villages.__populateExtendedData(sentRaidData.villageID, null /* villageObject */, villageData);


            } else {
                ROE.Frame.errorBar('Raid attack failed!');
            }

            //console.log('_sendRaidCallback', sentRaidData);
        }

        function _handleVillageExtendedInfoUpdatedOrPopulated(village) {
            if (village.changes.troops) {

                //console.log('_handleVillageExtendedInfoUpdatedOrPopulated', village);

                var villageRow = $('.villageRow[data-villageid="' + village.id + '"]');
                if (villageRow.length) {
                    _villageLoaded(village, villageRow);
                }

            }
            
        }


    }

    function _updateAttackPanel(raid) {

        //update the total outgoing in attack panel as well
        $('#raidsDialog .attackPanel .total span').html($('#raidsDialog .outgoing .totalOutgoing span').text());

        //update information regarding damage share 
        var totalPlayerDamageDone = raid.totalPlayerDamage;
        var totalPlayerDamageOutgoing = parseInt($('#raidsDialog .raidPanel').data('totalOutgoingDamage')); 
        var totalPlayerDamageShare = parseInt($('#raidsDialog .raidPanel').data('optimalDamage'));

        if (totalPlayerDamageDone + totalPlayerDamageOutgoing >= totalPlayerDamageShare) {
            $('#raidsDialog .attackPanel').addClass('metDamageShare');

            if (raid.raidType === 0) {

                $('#raidsDialog .attackPanel .villageListMask').html(
                    '<div>' +
                        '<div class="fontGoldFrLClrg">Well done!</div>' +
                        '<div class="">Enough damage in progress to beat the raid.</div>' +
                    '</div>'
                );

            } else if (raid.raidType === 1) {

                $('#raidsDialog .attackPanel .villageListMask').html(
                    '<div>' +
                        '<div class="fontGoldFrLClrg">Optimal damage met.</div>' +
                        '<div class="">Great work! You have sent enough troops to beat your optimal portion of the raid. ' +
                        'Sending more can result in more losses than gains. Tread carefully.</div>' +
                    '</div>'
                );

            }


        }

    }

    //find the most pressing, assigned target, if one found add settime as counter to the targets list icon.
    function _updateRaidIcon(raidIconElement) {

        //if no icon, abort
        if (raidIconElement.length < 1) {
            return;
        }

        //empty icon
        raidIconElement.empty();

        //if any raid still active, we show icon
        var nextRaidTime = ROE.Player.nextRaid;
        if (nextRaidTime > Date.now()) {
            //if a raid is active, make a counter and show the icon
            var nextRaidCounter = '<div class="counter countdown" data-refreshcall="" data-finisheson="' + nextRaidTime + '"></div>';
            raidIconElement.append(nextRaidCounter).addClass('highlight');
            if (ROE.Player.numOfRaidRewardsToCollect) {
                raidIconElement.append('<div class="collectCount">' + ROE.Player.numOfRaidRewardsToCollect + '</div>');
            }
            return;

        }

        //else if a reward to collect, show the icon
        if (ROE.Player.numOfRaidRewardsToCollect > 0) {
            raidIconElement.addClass('highlight');
            raidIconElement.append('<div class="collectCount">' + ROE.Player.numOfRaidRewardsToCollect + '</div>');
            return;
        }

        //if no rewards to collect and no active raids, hide icon
        raidIconElement.removeClass('highlight');
        return;
    }




    function _getRaidByRaidID(raidID) {

        for (var i = 0; i < _playerRaidsData.length; i++) {
            if (_playerRaidsData[i].raidID === raidID) {
                return _playerRaidsData[i];
            }
        }

        return null;

    }
    
    obj.showPopup = _showPopup;
    obj.silentUpdate = _silentUpdate;
    obj.updateRaidIcon = _updateRaidIcon;

}(window.ROE.Raids = window.ROE.Raids || {}));






