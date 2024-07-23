

function tabs_init() {
    $('.themeM-tabPanel .themeM-tabs li > a').click(function () {
        setab($(this));
    });
}

function setab(clickedTab) {
    var tabName = clickedTab.attr('tab')
    var tabsContentArea = clickedTab.parents(".themeM-tabPanel").find('.themeM-tabContent');
    //
    // show the right tab (content) 
    tabsContentArea.find(".tabContent").hide();
    tabsContentArea.find(".tabContent." + tabName).show();
    //
    // put the right class on tab (the clickable part) to give it proper look 
    clickedTab.parents(".themeM-tabs").find("li > a").removeClass('selected');
    clickedTab.addClass('selected')
}

