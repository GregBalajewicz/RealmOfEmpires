"use strict";

(function (ROE) {
}(window.ROE = window.ROE || {}));

(function (obj) {

    var CONST = {
        Selector: {
            dialog: '#somedialog'
        }
    }
   
    //var _container; //this doesnt work properly, there can be multiple morale bar contains at a time

    function _display(container) {
        
        if (!container || container.length == 0) { return; }
        if (!ROE.PlayerMoraleSettings.isActiveOnThisRealm) {
            container.remove();
            return;
        }

        var moraleBar = $(container).find('.moraleBar');
        if (moraleBar.length) {
            _update(moraleBar);
        } else {
            _create(container);
        }

    }

    function _create(container) {

        var moraleBar = $('<div class="moraleBar">' +
                '<div class="outer">' +
                    '<div class="bar"></div>' +
                    //'<div class="line l1"></div>' +
                    //'<div class="line l2"></div>' +
                    '<div class="shimmer"></div>' +
                    //'<div class="segment part1"></div><div class="segment part3"></div><div class="segment part2"></div>' +
                '</div>' +
                '<div class="inner">' +
                    //deets get populated here
                '</div>' +
                //
                    '<div class="line l1"></div>' +
                    '<div class="line l2"></div>' +
                    '<div class="pointer"></div>' +
            '</div>');



        if (ROE.isMobile) {

            moraleBar.click(function () {
                if (moraleBar.hasClass('showInner')) {
                    moraleBar.removeClass('showInner').addClass('hideInner');
                    _moraleDetailsPopup();
                } else {
                    moraleBar.removeClass('hideInner').addClass('showInner');
                    setTimeout(function () {
                        $('.moraleBar', container).removeClass('showInner').addClass('hideInner');
                    }, 3000);
                }
            });

        } else {
            //D2 version
            $('.inner', moraleBar).click(function () { _moraleDetailsPopup(); });
        }

        container.append(moraleBar);

        _update(moraleBar);
    }


    function _update(moraleBar) {
        
        //adding a delay to this update, makes it asynch form the JS thread, and also makes sure the CSS is settled
        setTimeout(function () {

            //var moraleBar = $('.moraleBar', _container);
            var width = moraleBar.outerWidth();

            //calculate dynamic segment sizes
            var part1Range = Math.abs(ROE.PlayerMoraleSettings.minMorale_Normal - ROE.PlayerMoraleSettings.minMorale);
            var part2Range = Math.abs(ROE.PlayerMoraleSettings.maxMorale_Normal - ROE.PlayerMoraleSettings.minMorale_Normal);
            var part3Range = Math.abs(ROE.PlayerMoraleSettings.maxMorale - ROE.PlayerMoraleSettings.maxMorale_Normal);
            var totalRange = part1Range + part2Range + part3Range;
            var ratio = width / totalRange;

            //width % of segments
            var part1W = Math.round(((part1Range * ratio) / width) * 100);
            var part2W = Math.round(((part2Range * ratio) / width) * 100);
            var part3W = Math.round(((part3Range * ratio) / width) * 100);

            $('.line.l1', moraleBar).css({
                left: part1W + '%'
            });

            $('.line.l2', moraleBar).css({
                left: part1W + part2W + '%'
            });

            //because morale can be negative, we do it this way to find pointer percentage
            var moralePostion = (ROE.Player.morale + part1Range) / totalRange;
            moralePostion = Math.min(moralePostion, 1);
            moralePostion = Math.max(moralePostion, 0);

            var bar = $('.bar', moraleBar).removeClass('p1 p2 p3');
            bar.css({
                width: moralePostion * 100 + '%'
            });
            $('.pointer', moraleBar).css({
                left: moralePostion * 100 + '%'
            });

            var pm = ROE.Player.morale;
            var barColorR, barColorG, barColorB;

            //bonus section color
            if (pm > ROE.PlayerMoraleSettings.maxMorale_Normal) {
                bar.addClass('p3').css({ 
                    //'background-size': width + 'px 100%'
                    'background-size': '100% 100%'
                });

                //normal section color
            } else if (pm >= ROE.PlayerMoraleSettings.minMorale_Normal) {
                bar.addClass('p2').css({
                    //'background-size': width + 'px 100%'
                    'background-size': '100% 100%'
                });

                //penalty section color
            } else {
                bar.addClass('p1').css({
                    'background-size': Math.round(part1Range * ratio) + 'px 100%'
                });
            }

            var inner = $('.inner', moraleBar).empty();
            inner.append('<div class="deet d1" style="left:' + 0 + '%">' + ROE.PlayerMoraleSettings.minMorale + '</div>');
            inner.append('<div class="deet d2" style="left:' + part1W + '%">' + ROE.PlayerMoraleSettings.minMorale_Normal + '</div>');
            inner.append('<div class="deet d3" style="left:' + (part1W + part2W) + '%">' + ROE.PlayerMoraleSettings.maxMorale_Normal + '</div>');
            inner.append('<div class="deet d4" style="right:' + 0 + '%">' + ROE.PlayerMoraleSettings.maxMorale + '</div>');
            inner.append('<div class="deet d5" >Current Morale: ' + ROE.Player.morale + '</div>');

        }, 100);

    }



    /*
    function _update() {

        var moraleBar = $('.moraleBar', _container);
        var width = moraleBar.outerWidth();

        //calculate dynamic segment sizes
        var part1Range = Math.abs(ROE.PlayerMoraleSettings.minMorale_Normal - ROE.PlayerMoraleSettings.minMorale);
        var part2Range = Math.abs(ROE.PlayerMoraleSettings.maxMorale_Normal - ROE.PlayerMoraleSettings.minMorale_Normal);
        var part3Range = Math.abs(ROE.PlayerMoraleSettings.maxMorale - ROE.PlayerMoraleSettings.maxMorale_Normal);
        var totalRange = part1Range + part2Range + part3Range;
        var ratio = width / totalRange;

        //width % of segments
        var part1W = Math.round(((part1Range * ratio) / width) * 100);
        var part2W = Math.round(((part2Range * ratio) / width) * 100);
        var part3W = Math.round(((part3Range * ratio) / width) * 100);

        $('.part1', moraleBar).css({
            left: '0%',
            width: part1W + '%'
        });

        $('.part2', moraleBar).css({
            left: part1W + '%',
            width: part2W + '%'
        });

        $('.part3', moraleBar).css({
            left: part1W + part2W + '%',
            width: part3W + '%'
        });

        //because morale can be negative, we do it this way to find pointer percentage
        var moralePostion = (ROE.Player.morale + part1Range) / totalRange;
        moralePostion = Math.min(moralePostion, 1);
        moralePostion = Math.max(moralePostion, 0);

        $('.pointer', moraleBar).css({ left: moralePostion * 100 + '%' });

        var inner = $('.inner', moraleBar).empty();
        inner.append('<div class="deet d1" style="left:' + 0 + '%">' + ROE.PlayerMoraleSettings.minMorale + '</div>');
        inner.append('<div class="deet d2" style="left:' + part1W + '%">' + ROE.PlayerMoraleSettings.minMorale_Normal + '</div>');
        inner.append('<div class="deet d3" style="left:' + (part1W + part2W) + '%">' + ROE.PlayerMoraleSettings.maxMorale_Normal + '</div>');
        inner.append('<div class="deet d4" style="right:' + 0 + '%">' + ROE.PlayerMoraleSettings.maxMorale + '</div>');
        inner.append('<div class="deet d5" >Current Morale: ' + ROE.Player.morale + '</div>');
    }
    */
    function _moraleDetailsPopup(passedMorale) {

        var data = {
            decrease_normal: ROE.PlayerMoraleSettings.decrease_normal,
            decrease_npc: ROE.PlayerMoraleSettings.decrease_npc,
            increasePerHour: ROE.PlayerMoraleSettings.increasePerHour.toFixed(2),
            maxMorale: ROE.PlayerMoraleSettings.maxMorale,
            minMorale: ROE.PlayerMoraleSettings.minMorale,
            currentMorale: ROE.Player.morale
        };
        var MoraleDetailsTemplate = BDA.Templates.getRawJQObj("MoraleDetails", ROE.realmID);
        var content = $(BDA.Templates.populate(MoraleDetailsTemplate[0].outerHTML, data))
        ROE.Frame.popGeneric('Morale Details', content, 650, 600);

        var tableBody = $('.moraleEffectsTableBody tbody');

        /*
        Object.keys(ROE.PlayerMoraleSettings.Effects).sort(function (a,b) {
            if (parseInt(a) > parseInt(b)) {
                return -1;
            } else {
                return 1;
            }
        }).forEach(function (key) {
            var effect = ROE.PlayerMoraleSettings.Effects[key];
            tableBody.append('<tr data-index="' + key + '" >' +
                '<td>' + key + '</td><td>' + effect.attack + '</td><td>' + effect.moveSpeed + '</td><td>' + effect.carryCapacity + '</td>'  +
            '</tr>');
        });
        $('.moraleEffectsTableBody tbody .highlight').removeClass('highlight');
        var selectMoraleIndex = passedMorale || ROE.Player.morale;
        var selectedTr = $('.moraleEffectsTableBody tbody tr[data-index="' + selectMoraleIndex + '"]').addClass('highlight');
        selectedTr[0].scrollIntoView({ behavior: "smooth" });
        */

        var selectMorale = passedMorale || ROE.Player.morale;
        var alreadyHighlighted = false;
        for (var i = 0; i < ROE.PlayerMoraleSettings.EffectRangers.length; i++) {
            var effect = ROE.PlayerMoraleSettings.EffectRangers[i];
            var highLightClass = '';
            if (!alreadyHighlighted && selectMorale >= effect.minMorale) {
                highLightClass = 'highlight';
                alreadyHighlighted = true;
            }
            tableBody.append('<tr data-index="' + i + '" class="' + highLightClass + '" >' +
                '<td>' + effect.maxMorale + ' ... ' + effect.minMorale + '</td><td>' + effect.attack + '</td><td>' + effect.moveSpeed + '</td><td>' + effect.carryCapacity + '</td>' +
            '</tr>');
        }

        $('.moraleEffectsTableBody tbody .highlight')[0].scrollIntoView({ behavior: "smooth" });



    }

    //Public functions
    obj.display = _display;
    obj.update = _update;
    obj.moraleDetailsPopup = _moraleDetailsPopup;
    
}(window.ROE.PlayerMorale = window.ROE.PlayerMorale || {}));




