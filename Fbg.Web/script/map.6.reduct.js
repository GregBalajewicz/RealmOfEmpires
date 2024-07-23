// global variable for map
var map = {};

function Debug (e)
{
    return;
    d = new Date();
    d.toUTCString();
    
    DumperMaxDepth = 5;
    $('body').append('<div style="background-color : #faa; border : 1px dashed #d55; margin : 5px; padding : 3px;">' + d.toUTCString() + '<pre>' + Dumper(e) + '</pre></div>');
}

function showCoOrd (x, y)
{
    $('.helpShortText').text("Co-Ordinates: " + x + "," + y);
}
function hideCoOrd ()
{
    $('.helpShortText').empty().append('&nbsp;');
}

function villageIconsRefresh(villages) {
    var f = 0;
    for (v in villages) {
        if (villages[v].PF == 1) {
            $("div.village[rel='" + villages[v].id + "']", map).append('<img src="https://static.realmofempires.com/images/map/ffb.png" class="flag fbfFlag" />');
            f++;
            //start = bench("found and taged village " + ++f, start);
        }
        if (villages[v].Sup > 0) {
            $("div.village[rel='" + villages[v].id + "']", map).append('<img src="https://static.realmofempires.com/images/support.png" class="flag sup" />');
        }
        if (villages[v].Att > 0) {
            $("div.village[rel='" + villages[v].id + "']", map).append('<img src="https://static.realmofempires.com/images/attack.png" class="flag att" />');
        }
    }
    return f;
}

$(
    function() {
        // if villages isn't defined, we're likely a mini/overview map. So let's return.
        if (!window.villages)
            return;

        // give the mapGrid an id, so it's cheap to find in the DOM
        $('.tlbMapGrid').addClass('hideFriends').attr('id', 'mapActual');
        var map = $('#mapActual')[0];

        var f = villageIconsRefresh(villages);

        var fbfTogMsg = "Highlight";
        $('.jsToggleFriends').prepend('<a id="fbfTog" href="#">' + fbfTogMsg + '</a>');
        $('#fbfTog')
            .attr('friends', f)
            .click(function() {
                if ($(this).attr('shown') == "yes") {
                    $('#fbfTog').text(fbfTogMsg);
                    $('#fbfTog').attr('shown', 'no');
                } else {
                    friends = parseInt($(this).attr('friends'), 10);
                    if (friends > 0) {
                        $('#fbfTog').text(friends + ' villages highlighted. Click to undo.');
                    } else
                        $('#fbfTog').text('No friends on map');

                    $('#fbfTog').attr('shown', 'yes');
                }
                $('#mapActual').toggleClass('hideFriends');

                return false;
            });


        // $('#jsFlagUI').
    }
);

$(
    function ()
    {
        $('.mapBracket').hover(
            function () 
            {
                img = $(this).children('img');
                $(img).attr('src', $(img).attr('src').replace(/1/, "2") );
            },
            function () 
            {
                img = $(this).children('img');
                $(img).attr('src', $(img).attr('src').replace(/2/, "1") );
            }
        );
    }
);

$(
    function ()
    {
        // Fide the map and get it's dimentions, we'll need these later
        if (!(mapp = $('.tlbMapGrid').position()))
            return; // we shouldn't be running, there is no map!
        //mapp = $('.tlbMapGrid').position();
        mapp.w = $('.tlbMapGrid').width();
        
        // sanity check!
        mapp.top = mapp.top > 100 ? 64 : mapp.top;
        
        // create the positioned div that will keep our map and legend
        $('body').append('<div id="MapPulldown"><div id="MapPulldownButtons"></div><div style="border : 1px solid #f00;" id="MapPulldownContent"></div></div>');
        
        // Hide the legend by default
        $('#MapPulldown').css({ left : mapp.left + 'px', top : mapp.top + 'px' });
        
        // create the fancy-pants buttons
        // $('#MapPulldownButtons').append('<a href="#" id="OverViewMapToggle"></a>').append('<a href="#" id="LegendToggle">Legend</a>');
        $('#OverViewMapToggle, .jsOverViewMapToggle').click(function () { overviewToggle(); return false; });
        $('#LegendToggle, .jsLegendToggle').click(function () { $('#MapPulldown .OverviewLegend').toggle(); return false; });

        // Append the divs to the overview pull down, and position them at the top of the map.
        $('.OverviewLegend, .OverviewParent').css('position', 'absolute').appendTo('#MapPulldownContent').removeClass('ui_def_hide').hide();
        $('.OverviewLegend').css({ left : '0px', width : '200px' });
        $('.OverviewParent').css({ left : (mapp.w - 200) + 'px', width : 200 + 'px' });
        
        $('#MapPulldown').css( { top : mapp.top + "px", left : mapp.left + "px" } );
        //$('.OverviewParent').remove();
        $('.OverviewMap').attr('id', 'OverviewReplace');
        
        
        if ($.cookie('showOverview') == 'true')
            overviewToggle(true);
    }
);
function overviewToggle (leaveCookie)
{
    $('#MapPulldown .OverviewParent').toggle();
    
    if ($('#MapPulldownContent iframe').length == 0)
    {
        query = window.location.toString().split(/\?/)[1];
        $('#OverviewReplace').before('<iframe src="MapOverview.aspx?' + query + '" scrolling="no" width="172" height="172" style="border : 0px solid #f00;" frameborder="0"></iframe>');
    }
    
    if (leaveCookie)
        return;

    if ($('#MapPulldown .OverviewParent').css('display') !== "none")
        $.cookie('showOverview', 'true');
    else
        $.cookie('showOverview', 'false');
}

$(
// init
    function() {

        

        if (!window.myVillages) {
            // this is more than likely the overview map (iframe).
            $('#ctl00_cph1_tblOverview td')
                .click(
                    function() {
                        var url = "Map.aspx?x=" + $(this).attr('x') + "&y=" + $(this).attr('y');
                        loadInParent(url);
                    }
                );

            $('#ctl00_cph1_tblOverview tr')
                .each(
                    function() {
                        c = $(this).attr('Rel').split(',', 2);
                        x = parseInt(c[0], 10);
                        y = parseInt(c[1], 10);

                        $(this).children("td").each(function() { $(this).attr('x', x++).attr('y', y); });
                    }
                );
        } else {
            DumperMaxDepth = 5;
            //DumperPopup({ "villages" : villages, "myVillage" : myVillages});
            // itterate over my villages, and flag them in the villages structure
            $(myVillages).each(
                function() {
                    if (this.id)
                        try {
                        villages['v' + this.id]['mine'] = true;
                    } catch (e) {
                        Debug(e);
                    }
                }
            );
            $('.village').hover(

                function(e) {
                    // cancel any timeout base effects
                    clearTimeout(map.timer);

                    // hover over image swap
                    img = $(this).children('img').not('.flag');
                    $(img).attr('src', $(img).attr('src').replace(/a\.png/, "b.png"));

                    village = $(this).attr('rel');
                    if ($('#hoverInfo').length > 0) {
                        $('#hoverInfo').show();
                    } else {
                        $('body').append('<div id="hoverInfo" class="facebookMenu">Loading...</div>');
                        $('#hoverInfo').hover(
                            function() {
                                if (map.menuShown)
                                    clearTimeout(map.timer);
                            },
                            function() {
                                map.menuShown = false;
                                $('#hoverInfo').hide();
                            }
                        );
                    }

                    position = $(this).parent().position();
                    position.width = $(this).width();
                    position.height = $(this).height();

                    //                    if (parseInt(position.left, 10) > parseInt($('#hoverInfo').width()) + 10) {
                    //                        $('#hoverInfo').css( { 'top' : parseInt(position.height, 10) + parseInt(position.top, 10) + 'px', 'left' : (parseInt(position.left, 10) - (parseInt($('#hoverInfo').width()) - parseInt(position.width, 10))) + 'px' } );
                    //                    } else {
                    $('#hoverInfo').css({ 'top': parseInt(position.height, 10) + parseInt(position.top, 10) + 'px', 'left': parseInt(position.left, 10) + 'px' });
                    //                    }

                    if (village != $('#hoverInfo').attr('village'))
                        UpdateHoverInfo(village);

                },
                function(e) {
                    // hover over(out) image swap
                    img = $(this).children('img').not('.flag');
                    $(img).attr('src', $(img).attr('src').replace(/b\.png/, "a.png"));

                    if (map.menuShown)
                        map.timer = setTimeout("$('#hoverInfo').hide();", 1000);
                    else
                        $('#hoverInfo').hide();
                }
            );
            $('.village').click(
                function(e) {
                    // find the village position
                    p = $(this).position();

                    map.menuShown = true;
                    $('#hoverMenu').show();
                    checkMapMenu(e.pageX, e.pageY);

                    if (!villages['v' + $(this).attr('rel')].mine) { return; }

                    Troops.GetTroopsInVillage($(this).attr('rel'),
                        function(f) {
                            var obj = eval(f);
                            
                            $('#hoverInfo .hoverHead .troops').html(obj.htmltable);

                            villages['v' + obj.VID].table = obj.htmltable;
                        }
                    )
                }
            );
        }
    }
);

function UpdateHoverInfo (village)
{
    mine = false;
    var str;

    if (villages['v' + village]) {

        villageInfo =  villages['v' + village];
        playerInfo =  players['p' + villageInfo.PID];
        clanInfo =  clans['c' + playerInfo.CID];
        coords = ' (' + villageInfo.X + ',' + villageInfo.Y + ')';
        var villageURL = villageInfo.mine ? BuildURL('VillageOverview.aspx', ['svid=' + villageInfo.id]) : BuildURL('VillageOverviewOtherPopup.aspx', ['ovid=' + villageInfo.id]);
        var villageOnClick = villageInfo.mine ? '': "return popupOtherVoV(this,'" + villageInfo.id + "','' );"
        var playerURL = BuildURL('PlayerPopup.aspx', ['pid=' + playerInfo.id]);
        
        $('#hoverInfo').empty();
//        if (villageInfo.mine && $('#chShowTags').attr('checked')) {
            $('#hoverInfo').append('<table cellspacing=0 cellpadding=0><TR><TD><div id="hoverHead" class="hoverHead"></div><div id="hoverMenu" class="hoverMenu"></div></TD><TD style="border-left:solid rgb(195, 144, 55) 1px"><div class="hdr ui_item">TAGS</DIV><div id="hoverTags"></div></TD></TR></TABLE>');
  //      } else {
    //        $('#hoverInfo').append('<table cellspacing=0 cellpadding=0><TR><TD><div id="hoverHead" class="hoverHead"></div><div id="hoverMenu" class="hoverMenu"></div></TD></TR></TABLE>');
      //  }
        
        str = '<div class="ui_item vilName" id="villageName"><span class=mapPoints>A' + villageInfo.A + '</span> <label style="font-weight : bold;">' + BuildAnchor(villageInfo.n + coords , villageURL, { 'onclick' : villageOnClick, 'id':'vName'} ) + '</label> <span class=mapPoints>[' + villageInfo.P + ']</span>';
        if (villageInfo.Ve != '0') 
        {
            str += '<span class="notelink"> ' + BuildAnchor(villageInfo.Ve , villageURL, { 'onclick' : villageOnClick} )  + '</span>';
        }            
        str += '</div>';
        $('#hoverHead').append(str);
        var vovLink = $('#hoverHead').find('#vName');

        if (!ROE.isSpecialPlayer(villageInfo.PID)) {
            str = '<div class="ui_item"><label class=mapLable>Player: </label><span>' + BuildAnchor(playerInfo.PN, playerURL, { 'onclick' : "return popupOtherVoV(this,'" + playerInfo.id + "','" + playerInfo.PN+ "' );"}) + '</span> <span class=mapPoints>[' + playerInfo.PP + ']</span>' ;
            if (playerInfo.Pe != '0') 
            {
                // player with a note
                str += ' <span class="notelink">' + BuildAnchor(playerInfo.Pe, playerURL, { 'onclick' : "return popupOtherVoV(this,'" + playerInfo.id + "','" + playerInfo.PN+ "' );"}) + '</span>';
            }
            str += '</div>';
            $('#hoverHead').append(str);
        }

        if (clanInfo) {
            if (clanInfo.CP != '0' ) {
                $('#hoverHead').append('<div class="ui_item"><label class=mapLable>Clan: </label><span>' + BuildAnchor(clanInfo.CN, BuildURL('ClanPublicProfile.aspx', ['clanid=' + clanInfo.id])) + '</span> <span class=mapPoints>[' + clanInfo.CP + ']</span></div>');            
            } else {
                $('#hoverHead').append('<div class="ui_item"><label class=mapLable>Clan: </label><span>' + BuildAnchor(clanInfo.CN, BuildURL('ClanPublicProfile.aspx', ['clanid=' + clanInfo.id]))+ '</span></div>');            
            }
        }

        if ( villageInfo.mine )  {
            var found = false;
            for(a in tags)
            {            
                found = false;
                for(b in villageInfo.tags) 
                {
                    if (villageInfo.tags[b] == tags[a].id) 
                    {
                        found = true; break;
                    }
                }
                if (found) {                                          
                    $('#hoverTags').append('<div><a op="-" tagid="' + tags[a].id + '" vilid="'
                        + villageInfo.id + '" onclick="tagClick(this);return false;" href="#" class=Tag>' 
                        + tags[a].n + '</a></div>');                            
                } else if ($('#chTagsAll').is(":checked")) {
                    $('#hoverTags').append('<div><a op="+" tagid="' + tags[a].id + '" vilid="'
                        + villageInfo.id + '" onclick="tagClick(this);return false;" href="#" class=noTag>' 
                        + tags[a].n + '</a></div>');                            
                }
            }

        }

        if (window.movement) {
            var m = window.movement[villageInfo.id];
            if (m) {
                if (m.Att > 0) {
                    $('#hoverHead').append('<div class="ui_item mapAttSup"><img src="https://static.realmofempires.com/images/attack.png" /> ' + (villageInfo.mine ? 'You have ' : ' You are sending ') + m.Att + ' ' + BuildAnchor('attack' + (m.Att == 1 ? '' : 's'), villageURL, { 'onclick': villageOnClick }) + (villageInfo.mine ? ' incoming ' : '') + ' to this village</div>');
                }
                if (m.Sup > 0) {
                    $('#hoverHead').append('<div class="ui_item mapAttSup"><img src="https://static.realmofempires.com/images/support.png" /> ' + (villageInfo.mine ? 'You have ' : ' You are sending ') + m.Sup + ' ' + BuildAnchor('support', villageURL, { 'onclick': villageOnClick }) + ' to this village</div>');
                }
            }
        }

        var helpArea = $('<div>');

        helpArea.addClass('help');
            
        //

        // Hidden menu section of the hover
        $('#hoverInfo').append('<div id="hoverMenu"></div>');
        $('#hoverMenu').hide();
        
        // Seperating line
        $('#hoverMenu').append('<div class="hr"><hr /></div>');

        // menu items
        ROE.getAnchorMode_IconOnly = true;
    



        $('#hoverInfo .hoverHead').append('<div class="troops"></div>');
        
        var d = villages['v' + village].table;
        if (d) {
            $('#hoverInfo .hoverHead .troops').html(d);
        }

        if (villageInfo.promstat) {
            // Seperating line
            $('#hoverMenu').append('<div class="hr"><hr /></div>');
            if (villageInfo.promstat == 'normal') {
                $('#hoverMenu').append('<div><span></span><a class="promstat" op="+" vilid="' + village + '" href="#">PROMOTE</a></div>');
            }
            if (villageInfo.promstat == 'absorbed') {
                $('#hoverMenu').append('<div>Village will be absorbed</div>');
            }
            if (villageInfo.promstat == 'promoted') {
                $('#hoverMenu').append('<div><span>Village is promoted </span><a class="promstat" op="-" vilid="' + village + '" href="#">UNPROMOTE</a></div>');
            }
            $('#hoverMenu .promstat').click(function (e) {
                e.preventDefault();
                var s = $(this);

                s.empty().append('<img src="https://static.realmofempires.com/images/misc/busy_tinyRed.gif" />');

                ajax('VillagePromoteAjax.aspx', { op: s.attr('op'), vilid: s.attr('vilid') }, function (data) {
                    var url_map = 'https://static.realmofempires.com/images/map/';
                    var flag = '';
                    var villi;
                    var villageC = $('.village'); //cache this for loop performance
                    var villC;
                    var flagC;
                    for (var vil in villages) {
                        villi = villages[vil];
                        if (villi.mine) {
                            villi.promstat = data[villi.id];
                            if (data[villi.id] == 'absorbed') {
                                flag = "Shield4_Teal.png";
                            }
                            if (data[villi.id] == 'promoted') {
                                flag = "Shield4_Yellow.png";
                            }
                            if (data[villi.id] == 'normal') {
                                flag = "fme.png";
                            }

                            villageC.each(function () {
                                villC = $(this);
                                if (villC.is('[rel="' + villi.id + '"]')) {
                                    flagC = villC.find('.flag');
                                    if (!(flagC.hasClass('.sup') || flagC.hasClass('.att'))) {
                                        flagC.attr('src', url_map + flag);
                                    }
                                }
                            });
                        }
                    }

                    s.empty().attr('op', s.attr('op') == '+' ? '-' : '+').html(s.attr('op') == '+' ? 'PROMOTE' : 'UNPROMOTE');
                    s.prev('span').html(s.attr('op') == '+' ? '' : 'Village is promoted');
                });
            });
        }

           
    } else {
        $('#hoverInfo').empty().append("No data");
    }
    
   
}

function mapRenameVillage(vID) 
{
    var link = $('#hoverInfo #villageName a#vName');
    ROE.Utils.RenameVillage(vID
        , link
        , null
        , function(result) {
            villages['v' + vID.toString()].n = result;
            }
    ); 
}

function checkMapMenu (x, y)
{
    if (!(mapp = $('.tlbMapGrid').position()))
        return; // we shouldn't be running, there is no map!
        
    mapp.width = $('.tlbMapGrid').width();
    mapp.height = $('.tlbMapGrid').height();
    
    menu = $('#hoverInfo').position();
    menu.width = $('#hoverInfo').width();
    menu.height = $('#hoverInfo').height();
    
    //DumperAlert([map.top, map.height, menu.top, menu.height, '(map.top + map.height) < (menu.top + menu.height)']);
    
    if ((mapp.top + mapp.height) < (menu.top + menu.height))
        t = (mapp.top + mapp.height) - menu.height;
    else 
        t = (y - 2);
    
    if (mapp.width + mapp.left < x + menu.width)
        x = (mapp.width + mapp.left) - menu.width;
    else
        x = x - 2;
        
    $('#hoverInfo').animate( { top : t + 'px', left : x + 'px' },100 );
}

function popupOtherVoV(link, targetVid, villName)
{
    var url = $(link).attr('href');
    return !popupModalIFrame(url, targetVid + Math.floor(Math.random() * 1000), villName, 500, 680, 20, 40);
}



function tagClick(e) {
    var s = $(e);

    ajax('VillageTagAjax.aspx', { op: s.attr('op'), vilid: s.attr('vilid'), tagid: s.attr('tagid') }, function() {
        s.toggleClass('noTag').toggleClass('Tag');
        
        var v = villages['v' + s.attr('vilid')];
        var tagID = s.attr('tagid');
        if (s.attr('op') == "-") {
            s.attr('op', '+')
            // if removing the tag, remove it from array            
            for(i=0; i<v.tags.length; i++) 
            {
               if(v.tags[i] == tagID) 
               {
                    v.tags[i] = "";
               }
            }
        } else {
            s.attr('op', '-');           
            // if adding a tag, add it to array
            v.tags[v.tags.length+1] = tagID;     
        }
    });


    //s.parents('.FT:first').attr('keep', 'yes');        
}


 mapObj = {
            showTagsSave: function () {
                if (window.localStorage) {
                    localStorage.map_ShowTags = $('#chShowTags').first().prop('checked');
                    localStorage.map_ShowTagsAll = $('#chTagsAll').first().prop('checked');
                }
            },

            togleShowShowAllTags: function () {
                $('#chShowTags').first().prop('checked') ? $('#chTagsAllSpan').fadeIn() : $('#chTagsAllSpan').fadeOut();
            }

        }

$(function () {
    if (window.localStorage) {
        $('#chShowTags').first().prop('checked', localStorage.map_ShowTags === "true");
        $('#chTagsAll').first().prop('checked', localStorage.map_ShowTagsAll === "true");
    }
    if ($('#chShowTags').first().prop('checked')) {
        mapObj.togleShowShowAllTags();
    }
});

function showValue(varValue) {
    document.getElementById('divVillageInfo').style.visibility = 'visible';
    document.getElementById('divVillageInfo').style.display = 'block';
    document.getElementById('spanVillageInfo').innerHTML = varValue;
}
        

$(function() {
            $('.troop-moves .button').click(function(e) { 
                var b = $(this);
                var bpref = b.hasClass('outgoing') ? "Outgoing" : "Incoming";
                $('img', b).attr('src', 'https://static.realmofempires.com/images/misc/busy_tinyRed.gif').removeClass('inactive');
                
                
                ajax('MapTroopsMove.aspx', {x: mapObj.bottomLeftX, y: mapObj.bottomLeftY , size: mapObj.mapSize, type: bpref }, 
                    function(d){
                        window.movement = d;
                        
                        var map = $('#mapActual');
                        $('img.flag.sup, img.flag.att', map).remove();
                        
                        for (v in d) {
                            if (d[v].Sup > 0) {
                                $("div.village[rel='" + d[v].id + "']", map).append('<img src="https://static.realmofempires.com/images/support.png" class="flag sup" />');
                            }
                            if (d[v].Att > 0) {
                                $("div.village[rel='" + d[v].id + "']", map).append('<img src="https://static.realmofempires.com/images/attack.png" class="flag att" />');
                            }
                        }
                        
                        $('img', b).attr('src', 'https://static.realmofempires.com/images/CheckMark_Quests.png');
                        
                        b.parent().find(b.hasClass('outgoing') ? '.incoming img' : '.outgoing img')
                                  .addClass('inactive');
                    });
            });
        });