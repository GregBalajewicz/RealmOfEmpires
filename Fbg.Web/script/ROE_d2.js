
//
// get proper vov anchor
//
ROE.getVOVAnchor = function (url, isThisVillageMine, vid, helpArea) {
    ///<param name="helpArea" >a qjuery div obj that represents the area where the help text will go. if passed undefined, it will be ignore.</param>
    //

    var isMine = isThisVillageMine;

    var theAnchor = BuildAnchor(ROE.helper_GetAnchorText("Village Overview"), "#"
    , { 'onclick':  'ROE.UI.Sounds.click();ROE.Frame.popupVillageProfile(' + vid + ');return false;', 'class': 'othervov', 'target': "_blank" });

    theAnchor = $(theAnchor);
    ROE.helper_AddHelpTextToElement(theAnchor, helpArea, 'Go to Village Profile');
    return theAnchor;
};


