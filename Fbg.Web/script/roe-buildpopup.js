(function (ROE) {
}(window.ROE = window.ROE || {}));


(function (obj) {

    var _container;
    obj.init = function (container) {
        _container = container;
        ROE.Frame.busy('Gathering building blueprints...', 5000, $('#buildDialog'));
        _container.append(BDA.Templates.getRawJQObj("BuildPopup", ROE.realmID));
        ROE.Api.call("upgrade_all_getupgradeinfo", { vid: ROE.SVID }, _populate);       
    }
    
    function _populate(response) {
               
        var buildingList = response.Buildings;
        var buildTable = "";
        var notBuildTable = "";
        var buildButtonText = _phrases(1);
        var detailButtonText = _phrases(2);
        var detailLevelText = _phrases(3);
        
        for (var i = 0; i < buildingList.length; i++) {
            
            var bID = buildingList[i].buildingID;
            var bIcon = ROE.Entities.BuildingTypes[bID].IconUrl_ThemeM;
            var bName = ROE.Entities.BuildingTypes[bID].Name;
            var curLevel = buildingList[i].curLevel;
            var canUpgrade = buildingList[i].Upgrade.canUpgrade;
            var MaxLevel = ROE.Entities.BuildingTypes[bID].MaxLevel;
            var buildReq = buildingList[i].Upgrade.unsatisfiedRequirementsIfAny;
            
            if (curLevel == 0) {

                if (buildReq.length == 0) {

                    buildTable += "<div class='buildBox' ><img src='" + bIcon + "'><span class='buildName fontGoldFrLClrg' >" + bName + "</span>";
                    buildTable += "<div class='buildButton sfx2' data-bid='" + bID + "' >";
                    buildTable += "<div class='customButtomBG2' >" + buildButtonText + "</div></div>";
                    buildTable += "</div><div class='separator'><img src='https://static.realmofempires.com/images/misc/M_ListBar2.png'></div>";

                }
                else {
                    notBuildTable += "<div class='buildBox' ><img src='" + bIcon + "'><span class='buildName fontGoldFrLClrg' >" + bName + "</span>";
                    notBuildTable += "<div class='buildDetailButton sfx2' data-bid='" + bID + "'>";
                    notBuildTable += "<div class='customButtomBG' >" + detailButtonText + "</div></div>";
                    notBuildTable += "<div class='buildingDetailPanel' >";

                    for (var j = 0; j < buildReq.length; j++) {

                        var bDetailName = ROE.Entities.BuildingTypes[buildReq[j].btid].Name;

                        notBuildTable += "<div class='buildDetailInfo' >" + bDetailName + ": " + detailLevelText + " " + buildReq[j].level + "</div>";
                    }

                    notBuildTable += "</div></div><div class='separator'><img src='https://static.realmofempires.com/images/misc/M_ListBar2.png'></div>";

                }

            } else if (curLevel != MaxLevel) {
                //if building already built, but not max level, show an upgrade button instead
                buildTable += "<div class='buildBox' ><img src='" + bIcon + "'><span class='buildName fontGoldFrLClrg' >" + bName + "</span>";
                buildTable += "<div class='buildButton sfx2' data-bid='" + bID + "' >";
                buildTable += "<div class='customButtomBG2' >" + "Upgrade" + "</div></div>";
                buildTable += "</div><div class='separator'><img src='https://static.realmofempires.com/images/misc/M_ListBar2.png'></div>";
            }
        }

        if (buildTable == "" && notBuildTable == "") { buildTable += "<div class='buildBox' ><center>" + _phrases(4) + "<br><br></center></div>"; }

        $("#buildpagePopup").css("height", document.height + "px");

        $("#buildpagePopup").append(buildTable);
        $("#buildpagePopup").append(notBuildTable);

        $(".BuildPopup .buildDetailButton").click(function (event) {          
            $(this).next().toggleClass("infoOpen");
        });

        $('.BuildPopup .buildButton').click(function (event) {
            ROE.UI.Sounds.suppressNext_click();
            closeModalPopupAndReloadHeader('build', true);
            ROE.Building2.showBuildingPagePopup($(event.currentTarget).attr("data-bid"));
            event.preventDefault();
        });

        $("#buildpagePopup").prepend("<div class='noteAboutQuickbuild fontGoldFrLClrg'>" + _phrases("noteAboutQuickbuild") + "</div>");
        ROE.Frame.free($('#buildDialog'));
    }



    function _phrases(id) {
        return $('.phrases [ph=' + id + ']').html();
    }

}(window.ROE.Build = window.ROE.Build || {}));