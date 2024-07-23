var researcher_refreshReseachers = false;

function researcherBuyCallBack(data) {
    $("#popup_buyres .buyconfirm").hide();
    $("#popup_buyres .nottoday .closeMsg").text("OK - CLOSE");
    $("#popup_buyres .nottoday").fadeIn();
    if (ROE.isMobile) {
        ROE.Research.synch();
    }
    switch (data.status) {
        case "OK":
            $("#popup_buyres .buydone.ok").fadeIn();
            researcher_refreshReseachers = true;
            break;
        case "lackservants":
            $("#popup_buyres .buydone.lackservants").fadeIn();
            break;
        case "maxresearchers":
            $("#popup_buyres .buydone.maxres").fadeIn();
            researcher_refreshReseachers = true;
            break;
        case "otherfailure":
            $("#popup_buyres .buydone.lackservants").fadeIn();
            break;

    }
}
    

function researcherBuyInit() {
    $("#popup_buyres .buy").click(function () {
        $("#popup_buyres .button.buy").hide();
        $("#popup_buyres .buyconfirm").fadeIn();
    });

    $("#popup_buyres .buyconfirm .yes").click(function () {
        ROE.Api.call("buyResearcher", null, researcherBuyCallBack);
    });

    $("#popup_buyres .buyconfirm .cancel").click(function () {
        $("#popup_buyres .button.buy").fadeIn();
        $("#popup_buyres .buyconfirm").hide();
    });

    $("#popup_buyres .nottoday").click(function () {
        if (researcher_refreshReseachers) {
            $('.researchers .refresh').trigger('click');     
        }
        closeMe();
    });
}

researcherBuyInit();