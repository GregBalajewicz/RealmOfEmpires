



$(function () {
 
    //
    // handle sounds
    //
    $(document).delegate(".sfx2", "click", function (event) {
        ROE.UI.Sounds.click();
        if ($(event.currentTarget).hasClass("sfx_suppress")) {
            ROE.UI.Sounds.suppressNext_click();
        }
    });//

});
