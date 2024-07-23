(function() {} (window.ROE = window.ROE || {}));
(function() {} (window.ROE.Map = window.ROE.Map || {}));

/**
 * TableDnD plug-in for JQuery, allows you to drag and drop table rows
 * You can set up various options to control how the system will work
 * Copyright © Denis Howlett <denish@isocra.com>
 * Licensed like jQuery, see http://docs.jquery.com/License.
 *
 * Configuration options:
 * 
 * onDragStyle
 *     This is the style that is assigned to the row during drag. There are limitations to the styles that can be
 *     associated with a row (such as you can't assign a borderâ€”well you can, but it won't be
 *     displayed). (So instead consider using onDragClass.) The CSS style to apply is specified as
 *     a map (as used in the jQuery css(...) function).
 * onDropStyle
 *     This is the style that is assigned to the row when it is dropped. As for onDragStyle, there are limitations
 *     to what you can do. Also this replaces the original style, so again consider using onDragClass which
 *     is simply added and then removed on drop.
 * onDragClass
 *     This class is added for the duration of the drag and then removed when the row is dropped. It is more
 *     flexible than using onDragStyle since it can be inherited by the row cells and other content. The default
 *     is class is tDnD_whileDrag. So to use the default, simply customise this CSS class in your
 *     stylesheet.
 * onDrop
 *     Pass a function that will be called when the row is dropped. The function takes 2 parameters: the table
 *     and the row that was dropped. You can work out the new order of the rows by using
 *     table.rows.
 * onDragStart
 *     Pass a function that will be called when the user starts dragging. The function takes 2 parameters: the
 *     table and the row which the user has started to drag.
 * onAllowDrop
 *     Pass a function that will be called as a row is over another row. If the function returns true, allow 
 *     dropping on that row, otherwise not. The function takes 2 parameters: the dragged row and the row under
 *     the cursor. It returns a boolean: true allows the drop, false doesn't allow it.
 * scrollAmount
 *     This is the number of pixels to scroll if the user moves the mouse cursor to the top or bottom of the
 *     window. The page should automatically scroll up or down as appropriate (tested in IE6, IE7, Safari, FF2,
 *     FF3 beta)
 * 
 * Other ways to control behaviour:
 *
 * Add class="nodrop" to any rows for which you don't want to allow dropping, and class="nodrag" to any rows
 * that you don't want to be draggable.
 *
 * Inside the onDrop method you can also call $.tableDnD.serialize() this returns a string of the form
 * <tableID>[]=<rowID1>&<tableID>[]=<rowID2> so that you can send this back to the server. The table must have
 * an ID as must all the rows.
 *
 * Known problems:
 * - Auto-scoll has some problems with IE7  (it scrolls even when it shouldn't), work-around: set scrollAmount to 0
 * 
 * Version 0.2: 2008-02-20 First public version
 * Version 0.3: 2008-02-07 Added onDragStart option
 *                         Made the scroll amount configurable (default is 5 as before)
 * Version 0.4: 2008-03-15 Changed the noDrag/noDrop attributes to nodrag/nodrop classes
 *                         Added onAllowDrop to control dropping
 *                         Fixed a bug which meant that you couldn't set the scroll amount in both directions
 *                         Added serialise method
 */
jQuery.tableDnD = {
    /** Keep hold of the current table being dragged */
    currentTable: null,
    /** Keep hold of the current drag object if any */
    dragObject: null,
    /** The current mouse offset */
    mouseOffset: null,
    /** Remember the old value of Y so that we don't do too much processing */
    oldY: 0,

    /** Actually build the structure */
    build: function (options) {
        // Make sure options exists
        options = options || {};
        // Set up the defaults if any

        this.each(function () {
            // Remember the options
            this.tableDnDConfig = {
                onDragStyle: options.onDragStyle,
                onDropStyle: options.onDropStyle,
                // Add in the default class for whileDragging
                onDragClass: options.onDragClass ? options.onDragClass : "tDnD_whileDrag",
                onDrop: options.onDrop,
                onDragStart: options.onDragStart,
                scrollAmount: options.scrollAmount ? options.scrollAmount : 5
            };
            // Now make the rows draggable
            jQuery.tableDnD.makeDraggable(this);
        });

        // Now we need to capture the mouse up and mouse move event
        // We can use bind so that we don't interfere with other event handlers
        jQuery(document)
            .bind('mousemove', jQuery.tableDnD.mousemove)
            .bind('mouseup', jQuery.tableDnD.mouseup);

        // Don't break the chain
        return this;
    },

    /** This function makes all the rows on the table draggable apart from those marked as "NoDrag" */
    makeDraggable: function (table) {
        // Now initialise the rows
        var rows = table.rows; //getElementsByTagName("tr")
        var config = table.tableDnDConfig;
        for (var i = 0; i < rows.length; i++) {
            // To make non-draggable rows, add the nodrag class (eg for Category and Header rows) 
            // inspired by John Tarr and Famic
            var nodrag = $(rows[i]).hasClass("nodrag");
            if (!nodrag) { //There is no NoDnD attribute on rows I want to drag
                jQuery(rows[i]).mousedown(function (ev) {
                    var td = $(ev.target).parents('td:first');
                    if (td.length > 0) {
                        jQuery.tableDnD.moveBeenMade = false;
                        jQuery.tableDnD.dragObject = this;
                        jQuery.tableDnD.currentTable = table;
                        jQuery.tableDnD.mouseOffset = jQuery.tableDnD.getMouseOffset(this, ev);
                        if (config.onDragStart) {
                            // Call the onDrop method if there is one
                            config.onDragStart(table, this);
                        }
                        return false;
                    }
                }).css("cursor", "move"); // Store the tableDnD object
            }
        }
    },

    /** Get the mouse coordinates from the event (allowing for browser differences) */
    mouseCoords: function (ev) {
        if (ev.pageX || ev.pageY) {
            return { x: ev.pageX, y: ev.pageY };
        }
        return {
            x: ev.clientX + document.body.scrollLeft - document.body.clientLeft,
            y: ev.clientY + document.body.scrollTop - document.body.clientTop
        };
    },

    /** Given a target element and a mouse event, get the mouse offset from that element.
        To do this we need the element's position and the mouse position */
    getMouseOffset: function (target, ev) {
        ev = ev || window.event;

        var docPos = this.getPosition(target);
        var mousePos = this.mouseCoords(ev);
        return { x: mousePos.x - docPos.x, y: mousePos.y - docPos.y };
    },

    /** Get the position of an element by going up the DOM tree and adding up all the offsets */
    getPosition: function (e) {
        var left = 0;
        var top = 0;
        /** Safari fix -- thanks to Luis Chato for this! */
        if (e.offsetHeight == 0) {
            /** Safari 2 doesn't correctly grab the offsetTop of a table row
            this is detailed here:
            http://jacob.peargrove.com/blog/2006/technical/table-row-offsettop-bug-in-safari/
            the solution is likewise noted there, grab the offset of a table cell in the row - the firstChild.
            note that firefox will return a text node as a first child, so designing a more thorough
            solution may need to take that into account, for now this seems to work in firefox, safari, ie */
            e = e.firstChild; // a table cell
        }

        while (e.offsetParent) {
            left += e.offsetLeft;
            top += e.offsetTop;
            e = e.offsetParent;
        }

        left += e.offsetLeft;
        top += e.offsetTop;

        return { x: left, y: top };
    },

    mousemove: function (ev) {
        if (jQuery.tableDnD.dragObject == null) {
            return;
        }

        var dragObj = jQuery(jQuery.tableDnD.dragObject);
        var config = jQuery.tableDnD.currentTable.tableDnDConfig;
        var mousePos = jQuery.tableDnD.mouseCoords(ev);
        var y = mousePos.y - jQuery.tableDnD.mouseOffset.y;
        //auto scroll the window
        var yOffset = window.pageYOffset;
        if (document.all) {
            // Windows version
            //yOffset=document.body.scrollTop;
            if (typeof document.compatMode != 'undefined' &&
	             document.compatMode != 'BackCompat') {
                yOffset = document.documentElement.scrollTop;
            }
            else if (typeof document.body != 'undefined') {
                yOffset = document.body.scrollTop;
            }

        }

        if (mousePos.y - yOffset < config.scrollAmount) {
            window.scrollBy(0, -config.scrollAmount);
        } else {
            var windowHeight = window.innerHeight ? window.innerHeight
                    : document.documentElement.clientHeight ? document.documentElement.clientHeight : document.body.clientHeight;
            if (windowHeight - (mousePos.y - yOffset) < config.scrollAmount) {
                window.scrollBy(0, config.scrollAmount);
            }
        }


        if (y != jQuery.tableDnD.oldY) {
            // work out if we're going up or down...
            var movingDown = y > jQuery.tableDnD.oldY;
            // update the old value
            jQuery.tableDnD.oldY = y;
            // update the style to show we're dragging
            if (config.onDragClass) {
                dragObj.addClass(config.onDragClass);
            } else {
                dragObj.css(config.onDragStyle);
            }
            // If we're over a row then move the dragged row to there so that the user sees the
            // effect dynamically
            var currentRow = jQuery.tableDnD.findDropTargetRow(dragObj, y);
            if (currentRow) {
                // TODO worry about what happens when there are multiple TBODIES
                if (movingDown && jQuery.tableDnD.dragObject != currentRow) {
                    jQuery.tableDnD.dragObject.parentNode.insertBefore(jQuery.tableDnD.dragObject, currentRow.nextSibling);
                    jQuery.tableDnD.moveBeenMade = true;
                } else if (!movingDown && jQuery.tableDnD.dragObject != currentRow) {
                    jQuery.tableDnD.dragObject.parentNode.insertBefore(jQuery.tableDnD.dragObject, currentRow);
                    jQuery.tableDnD.moveBeenMade = true;
                }
            }
        }

        return false;
    },

    /** We're only worried about the y position really, because we can only move rows up and down */
    findDropTargetRow: function (draggedRow, y) {
        var rows = jQuery.tableDnD.currentTable.rows;
        for (var i = 0; i < rows.length; i++) {
            var row = rows[i];
            var rowY = this.getPosition(row).y;
            var rowHeight = parseInt(row.offsetHeight) / 2;
            if (row.offsetHeight == 0) {
                rowY = this.getPosition(row.firstChild).y;
                rowHeight = parseInt(row.firstChild.offsetHeight) / 2;
            }
            // Because we always have to insert before, we need to offset the height a bit
            if ((y > rowY - rowHeight) && (y < (rowY + rowHeight))) {
                // that's the row we're over
                // If it's the same as the current row, ignore it
                if (row == draggedRow) { return null; }
                var config = jQuery.tableDnD.currentTable.tableDnDConfig;
                if (config.onAllowDrop) {
                    if (config.onAllowDrop(draggedRow, row)) {
                        return row;
                    } else {
                        return null;
                    }
                } else {
                    // If a row has nodrop class, then don't allow dropping (inspired by John Tarr and Famic)
                    var nodrop = $(row).hasClass("nodrop");
                    if (!nodrop) {
                        return row;
                    } else {
                        return null;
                    }
                }
            }
        }
        return null;
    },

    mouseup: function (e) {
        if (jQuery.tableDnD.currentTable && jQuery.tableDnD.dragObject) {
            var droppedRow = jQuery.tableDnD.dragObject;
            var config = jQuery.tableDnD.currentTable.tableDnDConfig;
            // If we have a dragObject, then we need to release it,
            // The row will already have been moved to the right place so we just reset stuff
            if (config.onDragClass) {
                jQuery(droppedRow).removeClass(config.onDragClass);
            } else {
                jQuery(droppedRow).css(config.onDropStyle);
            }
            jQuery.tableDnD.dragObject = null;
            if (config.onDrop) {
                // Call the onDrop method if there is one
                config.onDrop(jQuery.tableDnD.currentTable, droppedRow);
            }
            jQuery.tableDnD.moveBeenMade = false;
            jQuery.tableDnD.currentTable = null; // let go of the table too
        }
    },

    serialize: function () {
        if (jQuery.tableDnD.currentTable) {
            var result = "";
            var tableId = jQuery.tableDnD.currentTable.id;
            var rows = jQuery.tableDnD.currentTable.rows;
            for (var i = 0; i < rows.length; i++) {
                if (result.length > 0) result += "&";
                result += tableId + '[]=' + rows[i].id;
            }
            return result;
        } else {
            return "Error: No Table id set, you need to set an id on your table and every row";
        }
    }
}

jQuery.fn.extend(
	{
	    tableDnD: jQuery.tableDnD.build
	}
);

(function (obj) {

    var flags = {
        path: 'https://static.realmofempires.com/images/map/',

        list: {

            "x1": 'Shield2_Black.png',
            "x2": 'Shield2_Blue.png',
            "x3": 'Shield2_Green.png',
            "x4": 'Shield2_Orange.png',
            "x5": 'Shield2_Purple.png',
            "x6": 'Shield2_Red.png',
            "x7": 'Shield2_White.png',
            "x8": 'Shield2_Yellow.png',

            "xb1": 'Shield2b_Black.png',
            "xb2": 'Shield2b_Blue.png',
            "xb3": 'Shield2b_Green.png',
            "xb4": 'Shield2b_Orange.png',
            "xb5": 'Shield2b_Purple.png',
            "xb6": 'Shield2b_Red.png',
            "xb7": 'Shield2b_Teal.png',
            "xb8": 'Shield2b_Yellow.png',

            "xc1": 'Shield2c_Black.png',
            "xc2": 'Shield2c_Blue.png',
            "xc3": 'Shield2c_Green.png',
            "xc4": 'Shield2c_Orange.png',
            "xc5": 'Shield2c_Purple.png',
            "xc6": 'Shield2c_Red.png',
            "xc7": 'Shield2c_Teal.png',
            "xc8": 'Shield2c_Yellow.png',

            "z1": 'Shield3_Black.png',
            "z2": 'Shield3_Blue.png',
            "z3": 'Shield3_Green.png',
            "z4": 'Shield3_Orange.png',
            "z5": 'Shield3_Purple.png',
            "z6": 'Shield3_Red.png',
            "z7": 'Shield3_White.png',
            "z8": 'Shield3_Yellow.png',

            "zb1": 'Shield3b_Black.png',
            "zb2": 'Shield3b_Blue.png',
            "zb3": 'Shield3b_Green.png',
            "zb4": 'Shield3b_Orange.png',
            "zb5": 'Shield3b_Purple.png',
            "zb6": 'Shield3b_Red.png',
            "zb7": 'Shield3b_Teal.png',
            "zb8": 'Shield3b_Yellow.png',

            "zc2": 'Shield3c_Blue.png',
            "zc3": 'Shield3c_Green.png',
            "zc4": 'Shield3c_Orange.png',
            "zc5": 'Shield3c_Purple.png',
            "zc6": 'Shield3c_Red.png',
            "zc7": 'Shield3c_Teal.png',
            "zc8": 'Shield3c_Yellow.png',

            "y1": 'Shield4_Black.png',
            "y2": 'Shield4_Blue.png',
            "y3": 'Shield4_Green.png',
            "y4": 'Shield4_Orange.png',
            "y5": 'Shield4_Purple.png',
            "y6": 'Shield4_Red.png',
            "y7": 'Shield4_White.png',
            "y8": 'Shield4_Yellow.png',

            "yb1": 'Shield4b_Black.png',
            "yb2": 'Shield4b_Blue.png',
            "yb3": 'Shield4b_Green.png',
            "yb4": 'Shield4b_Orange.png',
            "yb5": 'Shield4b_Purple.png',
            "yb6": 'Shield4b_Red.png',
            "yb7": 'Shield4b_Teal.png',
            "yb8": 'Shield4b_Yellow.png',

            "yc2": 'Shield4c_Blue.png',
            "yc3": 'Shield4c_Green.png',
            "yc4": 'Shield4c_Orange.png',
            "yc5": 'Shield4c_Purple.png',
            "yc6": 'Shield4c_Red.png',
            "yc7": 'Shield4c_Teal.png',
            "yc8": 'Shield4c_Yellow.png',

            "teal2": 'flag_Teal2.png',
            "teal1": 'flag_Teal1.png',
            "yellow2": 'flag_Yellow2.png',
            "red2": 'flag_Red2.png',
            "purple2": 'flag_Purple2.png',
            "purple1": 'flag_Purple1.png',
            "pink1": 'flag_Pink1.png',
            "pink2": 'flag_Pink2.png',
            "orange1": 'flag_Orange1.png',
            "orange2": 'flag_Orange2.png',
            "green2": 'flag_Green2.png'
        },


        def: ["orange1"]
    };

    obj.flags = flags;

    var _container;
    var _flagIconPopup;
    obj.init = function (container) {
        _container = container;
        container.html(BDA.Templates.getRawJQObj("MapHighlights", ROE.realmID));

        $('.addNewRule', container).click(function () {
            ROE.UI.Sounds.click();
            _slideLeft('.add.section');
        });

        $('.add div[type]', container).click(_add);
        $('.add .back', container).click(function () {
            ROE.UI.Sounds.click();
            $('.edit', _container).removeAttr('num');
            _slideRight('.list.section');
        });

        $('.edit .saving', container).click(_save);
        $('.edit .remove', container).click(_remove);
        $('.edit .back,.edit .cancel', container).click(function () {
            ROE.UI.Sounds.click();
            _slideRight('.list.section');
        });
        $('.edit .changeShield', _container).click(function () {
            ROE.UI.Sounds.click();
            ROE.Frame.simplePopopOverlay('https://static.realmofempires.com/images/icons/highlight2.png', "Select Shield", _flagIconPopup, "shieldPickPopup", _container);
            $(".shieldPickPopup .legendicon", _container).click(function () {
                var flag = $(this).attr('who');
                $('.edit .changeShield', _container).attr('flag', flag).attr('data-src', flags.path + flags.list[flag]);
                $('.edit .changeShield .flag', _container).css('background-image', 'url(' + flags.path + flags.list[flag] + ')');
                $(".shieldPickPopup", _container).remove();
            });
        });

        _buildFlagSelector();
        _load();
        _autocomplete();
    }

    obj.getflag = function(village) {
        var listj = ROE.LocalServerStorage.get('Highlights');
        if (!listj) listj = [];
        var list = listj;
        var highlight;

        if (!village) return null;
        if (!village.player) return null;

        for (var f = 0; f < list.length; f++) {
            highlight = list[f];
            if (!highlight) {
                continue;
            }

            switch (highlight.type) {
                case "player":
                    var t = comp(comp_nocase, village.player.PN, highlight);
                    if (t) { return t; } continue;
                case "clan":
                    if (highlight.type == 'clan' && !village.player.clan) continue;
                    var t = comp(comp_nocase, village.player.clan.CN, highlight);
                    if (t) { return t; } continue;
                case "playernote":
                    var t = comp(comp_contains, village.player.Pe, highlight);
                    if (t) { return t; } continue;
                case "villagenote":
                    var t = comp(comp_contains, village.note, highlight);
                    if (t) { return t; } continue;
            }
        }

        function comp_nocase(f, s) { return f.toLowerCase() == s.toLowerCase(); }
        function comp_contains(f, s) { return f.toLowerCase().indexOf(s.toLowerCase()) != -1; }

        function comp(cmp, n, f) {
            if (cmp(n, $.trim(f.keyword)))
                return flags.list[f.flag];
        }
        
        return null;
    }

    function _getLocalItems() {
        var hs = ROE.LocalServerStorage.get('Highlights');
        if (hs == null)
            var res = [];
        else
            var res = hs;
        return res;
    }

    function _buildFlagSelector() {
        var groups = [8, 8, 8, 8, 8, 7, 8, 8, 7, 11];
        var counter = 0;
        var inGroup = 0;
        _flagIconPopup = "";

        for (var fl in flags.list) {

            if (counter >= groups[inGroup]) {
                inGroup++;
                counter = 0;
                _flagIconPopup += '<br>';
            }

            _flagIconPopup += "<img class='legendicon ButtonTouch sfx2' src='" + flags.path + flags.list[fl] + "' who='" + fl + "' >";
            
            counter++;
        }  
    }

    function _slide(where, show) {
        BDA.UI.Transition['slide' + where]($(show, _container).addClass('newshowed'), $('.section.showed', _container), _hideAfterSlide);
    }

    function _slideLeft(show) { _slide('Left', show); }
    function _slideRight(show) { _slide('Right', show); }

    function _hideAfterSlide() {
        $('.section.showed', _container).removeClass('showed');
        $('.section.newshowed', _container).removeClass('newshowed').addClass('showed');
    }

    function _showHideEmpty() {
        if ($('.list .item', _container).length > 0) {
            $('.empty', _container).hide();
        } else {
            $('.empty', _container).show();
        }
    }

    function _phrase(mes) {
        return $('#highlight_popup .phrases [ph=' + mes + ']', _container).html();
    }

    function _getNotSelectedFlag() {
        var fl = null;
        var selected = {};

        $('.item', _container).each(function (i, n) {
            selected[$('.flag-icon', n).attr('flag')] = 1;
        });

        for (var f in flags.list) {
            if (!selected[f]) { fl = f; }
        }
        if (!fl) { fl = flags.def[0]; }

        return fl;
    }

    function _save() {
        ROE.UI.Sounds.click();
        var keyword = $('.edit .keyword', _container).val();
        if (keyword.search(/\w/) == -1) {
            ROE.Frame.infoBar(_phrase('InfoEmptyKeyword'));
            return;
        }

        var res = _getLocalItems();
        var num = $('.edit', _container).attr('num');
        if (!num || !res[num]) {
            res.push({});
            num = res.length-1;
        }

        res[num].type = $('.edit', _container).attr('type');
        res[num].flag = $('.edit .changeShield', _container).attr('flag');
        res[num].keyword = keyword;

        //BDA.Database.LocalSet('Highlights', JSON.stringify(res));
        ROE.LocalServerStorage.set('Highlights', res);
        ROE.Landmark.refill(true);
        $('.edit', _container).removeAttr('num');
        _load();
        _slideRight('.list.section');
    }

    function _autocomplete() {
        $(".keyword", _container)
            .keyup(function () {
                var type = $(this).data('highlightType');
                if (type != 'clan' && type != 'player') { return }

                var term = $(this).val();
                if (term.length < 3) { return false; }

                $.getJSON("NamesAjax.aspx?what=" +
                    ($(this).parents('[type=clan]').length == 0 ? "playerNamesWClan" : 'clans'),
                    { term: term }, nameResponse);
            }).blur(function () {
                setTimeout(function () { $(".autocomplete", _container).empty() }, 100);
            });

        function nameResponse(r) {
            var ac = $(".autocomplete", _container).empty();
            var to = $(".keyword", _container);
            $.each(r, function (i, n) {
                ac.append($('<div v="' + n.value + '">' + (!n.label ? n.value : n.label) + '</div>').click(function autocompleteSelect() {            
                    to.val($(this).attr('v'));
                    $(".autocomplete", _container).empty();
                    return;
                }));
            });
        }
    }

    function _add() {
        var o = { type: $(this).attr('type'), flag: _getNotSelectedFlag() };

        var v = ROE.Landmark.v;
        // highlight selected on map village
        if (!ROE.isSpecialPlayer(v.pid)) {
            var list = _getLocalItems();

            if (o.type == 'player' && $.grep(list, function (n) { return n.type == 'player' && n.keyword == v.player.PN }).length == 0) {
                o.keyword = v.player.PN;
            }

            if (v.player.clan) {
                if (o.type == 'clan' && $.grep(list, function (n) { return n.type == 'clan' && n.keyword == v.player.clan.CN }).length == 0) {
                    o.keyword = v.player.clan.CN;
                }
            }
        }

        _edit(o);
    }

    function _populateRow(fl) {
        var item = $('.template', _container).clone().removeClass('template');

        $('.flag-icon', item)
            .attr('flag', fl)
            .attr('src', flags.path + flags.list[fl]);

        //$('.remove', item).click(_remove);
        item.click(function () {
            var num = ~~$(this).attr('num');
            _edit(_getLocalItems()[num]).attr('num', num);
        });

        return item;
    }

    function _remove(e) {
        e.stopPropagation();
        ROE.UI.Sounds.click();
        var item = $(this).parents('.edit');      
        var res = _getLocalItems();
        res.splice(~~item.attr('num'), 1);
        $('.edit', _container).removeAttr('num');
        ROE.LocalServerStorage.set('Highlights', res);
        ROE.Landmark.refill(true);     
        _load();
        _slideRight('.list.section');
    }

    function _edit(item) {
        ROE.UI.Sounds.click();
        var editContainer = $('.edit', _container).attr('type', item.type);
        $('.type', editContainer).html($('.add .addType[type=' + item.type + ']', _container).html());
        $('.changeShield', editContainer).attr('flag', item.flag).attr('data-src', flags.path + flags.list[item.flag]);
        $('.changeShield .flag', editContainer).css('background-image', 'url(' + flags.path + flags.list[item.flag] + ')');
        $('.keyword', editContainer).val(item.keyword).data('highlightType', item.type);       
        $('.autocomplete', editContainer).empty();
        _slideLeft('.edit.section');
        return editContainer;
    }

    function _reorder(){
        if (jQuery.tableDnD.moveBeenMade) {
            var newOrder = [];
            var current = _getLocalItems();
            $('.list .item', _container).each(function () {
                newOrder.push(current[~~$(this).attr('num')]);
            });

            ROE.LocalServerStorage.set('Highlights',newOrder);
            ROE.Landmark.refill(true);
            _load();
        }
    }
    

    function _load() {
        var listj = ROE.LocalServerStorage.get('Highlights');
        if (!listj) listj = [];
        var list = listj;
        var rows = $('.list .rows', _container);
        $('tr', rows).remove();       
        $.each(list, function (i, n) {
            if (n) {
                var item = _populateRow(n.flag).attr('num', i);
                var tr = $('<tr><td></td></tr>');
                $('tbody', rows).append(tr);
                $('td', tr).append(item);
                $('.type', item).html($('.add .addType[type=' + n.type + ']', _container).html());
                $('.keyword', item).html(n.keyword);
            }
        });

        rows.tableDnD({ onDrop: _reorder });

        _showHideEmpty();
    }
   

}(window.ROE.Map.Highlights = window.ROE.Map.Highlights || {}));