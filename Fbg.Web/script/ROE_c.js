

//
//
// ROE object
//
//

// ROE.isMobile = false;// tells you if the application is currently running in mobile mode (on mobile device) or on desktop -- this is set by the ASPX page

ROE.realmID= 0; //id of the player . will be set by aspx page
ROE.playerID= 0; //id of the player. will be set by aspx page
ROE.isInPopup= false; // bool  - true if page is currently in an iframe popup, not in the main window, will be set by aspx page
ROE.playersNumVillages= 0; // number of villages this players has in total ,will be set by aspx page
ROE.isVPRealm = false; // true if relam is VP. will be set by aspx page
ROE.Realm=
    {
        BuildingTypeByID: function (buildingTypeID) {
            buildingTypeID = parseInt(buildingTypeID, 10);
            for (var i = 0; i < ROE.Realm.Buildings.length; i++) {
                if (ROE.Realm.Buildings[i].ID === buildingTypeID) { return ROE.Realm.Buildings[i]; }
            }
            return null;
        }
    };

ROE.CONST=
    {
        regex_VillageCordinates: /\s*\(\s*-?\s*\d{1,3}\s*,\s*-?\s*\d{1,3}\s*\)\s*/
        , VillageNameMaxLength: 25
        //
        // expect the following to be defined dynamically by server side scripts
        //
        // >specialPlayer_Rebel  integer 
        // >specialPlayer_Abandoned integer 
        // 
        , popupNameIDPrefix: "popup_"
        , headerRefreshRate: 15000
    };

    //
    // tells you if the passed in player is you, the current player
    //
ROE.isMe= function (playerID) {
        return playerID === ROE.playerID;
    };
    //
    // tells you if the player is a rebel or abandoned
    //
ROE.isSpecialPlayer= function (playerID) {
        return ROE.isSpecialPlayer_Rebel(playerID)
                || ROE.isSpecialPlayer_Abandoned(playerID);
    };
    //
    // tells you if the player is a rebel
    //
ROE.isSpecialPlayer_Rebel= function (playerID) {
        return (parseInt(playerID, 10) === ROE.CONST.specialPlayer_Rebel);
    };
    //
    // tells you if the player is an abandoned player ID
    //
ROE.isSpecialPlayer_Abandoned= function (playerID) {
        return (parseInt(playerID, 10) === ROE.CONST.specialPlayer_Abandoned);
    };


ROE.getAnchorMode_IconOnly= false;
ROE.helper_GetAnchorText= function (anchorText) {
        return ROE.getAnchorMode_IconOnly ? "" : anchorText
    };
    //
    // get proper upgrade anchor
    //



ROE.helper_AddHelpTextToElement = function (theElement, helpArea, helpText) {
    ///<summary>adds a hover over help text to theElement . helps goes to the helpArea. if helpArea is undefined, nothing is done</summary>
    ///<param name="theElement" >must be a query object representing the elemet on which to put help</param>

    if (helpArea) {
        theElement.hover(
            function () {
                $(helpArea).text(helpText);
            },
            function () {
                $(helpArea).text('');
            }
        )
    }
};


