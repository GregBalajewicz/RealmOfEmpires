

//
// get proper vov anchor
//
ROE.getVOVAnchor = function (url, isThisVillageMine, vid) {
    ///<param name="helpArea" >a qjuery div obj that represents the area where the help text will go. if passed undefined, it will be ignore.</param>
    //

    var href = url;

    var isMine = isThisVillageMine;

    var theAnchor = BuildAnchor(ROE.helper_GetAnchorText("Village Overview"), href
    , { 'onclick': (isMine ? 'ROE.Frame.switchToVoV(' + vid + ');return false;' : 'ROE.Frame.popupVillageProfile(' + vid + ');return false;'), 'class': (isMine ? 'myvov sfx2' : 'othervov sfx2'), 'target': "_blank" });

    theAnchor = $(theAnchor);
   // ROE.helper_AddHelpTextToElement(theAnchor, helpArea, 'Go to Village Overview');
    return theAnchor;
};



