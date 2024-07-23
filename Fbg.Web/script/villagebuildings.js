
//function initUpgradeToLevels() {
//    // to debug: $('body').append('<td keepers="upgradeMenu1" class="tut" rel="upgrade3"><a class="StandoutLink" href="Upgrade.aspx?bid=3&amp;l=13&amp;vid=2">Upgrade to Level 13</a><span> (</span><a upgrademenu="1" class="jsUpgradeMenu" rel="2/13" href="Upgrade.aspx?bid=3&amp;l=10&amp;vid=2&amp;max=1">level 10</a><span>)</span></td>');
//    var i = 0;
//    $('.jsUpgradeMenu', $('#' + IDMapping['buildingsTable'])).each(
//        function () {
//            levels = $(this).attr("rel").split('/');
//            $(this).attr('max', levels[1]);
//            $(this).attr('min', levels[0]);
//            $(this)
//                .mouseover(UpgradeToMenu)
//                .attr("upgradeMenu", ++i);

//            $(this)
//                .parent()
//                .mouseout(genericMenusClearFast)
//                .mouseover(keepMine)
//                .attr("keepers", "upgradeMenu" + i);
//        }
//    );
//}


function UpgradeToMenu() {
    id = "upgradeMenu" + $(this).attr("upgradeMenu");
    url = $(this).attr("href").replace(/l=\d+/, 'l=###');
    td = $(this).parent()[0];
    p = $(td).position();
    p.top += $(td).height();
    p.width = $(td).width();

    if ($('#' + id).length == 0) {
        options = new Array();
        var min = parseInt($(this).attr("min"), 10);
        var max = parseInt($(this).attr("max"), 10);
        var l = Math.round(parseInt(min / 5) * 5) + 5;

        while (l <= (max - 5)) {
            options.push(l);
            l += 5;
        }
        while (min < max + 1) {
            if (min > (max - 5))
                options.push(min);

            min++;
        }

        $('#allMenus').append('<div style="background-color : rgb(91,102,96); padding : 3px 1px;" id="' + id + '"><span>&nbsp;Upgrade to:</span> </div>');
        for (i = 0; i < options.length; i++)
            $('#' + id).append('<a href="' + url.replace('###', options[i]) + '">' + options[i] + '</a>' + (i < options.length - 1 ? ", " : ""));

        $('#' + id)
        .css({
            position: 'absolute',
            left: p.left + 'px',
            top: p.top + 'px',
            width: p.width + 'px'
        })
        .addClass("ui_menu_fast")
        .mouseover(function () { $(this).attr('keep', 'yes') })
        .mouseout(genericMenusClearFast)
        .mouseout(function () { $(this).attr('keep', 'no') })
        .attr('keep', "yes");
    } else {
        $('#' + id)
        .show()
        .attr('keep', 'yes');
    }
}


function initHideBuildRequirements() {
    $('.hiddenRequirementsShow').click(
        function () {
            line = $(this).siblings('.hiddenRequirements');
            if (line[0].style.display == 'none')
            //$(this).siblings('.hiddenRequirements').attr('style', '').slideDown();
                $(this).siblings('.hiddenRequirements').attr('style', '').show();
            else
            //$(line).slideToggle();
                $(line).toggle();

            return false;
        }
    ); //.wrap('<a href="#"></a>');
}



function keepMine() {
    keep = $(this).attr('keepers');
    $('#' + keep).attr('keep', 'yes');
}
