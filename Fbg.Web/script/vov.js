

$(
    function ()
    {
        var areaHQ = $('div[id$=divGraphicalVOV] area[id$=areaHQ]');        
        var imgHQ = $('div[id$=divGraphicalVOV] img[id$=linkHQ]');
        var imgTres = $('div[id$=divGraphicalVOV] img[id$=linkTreasury]');
        var imgMine = $('div[id$=divGraphicalVOV] img[id$=linkMine]');
        var imgFarm = $('div[id$=divGraphicalVOV] img[id$=linkFarm]');
        var imgStable = $('div[id$=divGraphicalVOV] img[id$=linkStable]');
        var imgBar = $('div[id$=divGraphicalVOV] img[id$=linkBarracks]');
        var imgSiege = $('div[id$=divGraphicalVOV] img[id$=linkSiege]');
        var imgSH = $('div[id$=divGraphicalVOV] img[id$=linkHidingSpot]');
        var imgTav = $('div[id$=divGraphicalVOV] img[id$=linkTavern]');
        var imgWall = $('div[id$=divGraphicalVOV] img[id$=linkWall]');
        var imgTowerR = $('div[id$=divGraphicalVOV] img[id$=linkTowerR]');
        var imgTowerL = $('div[id$=divGraphicalVOV] img[id$=linkTowerL]');
        var imgWallM = $('div[id$=divGraphicalVOV] img[id$=linkWall]');
        var imgWallL = $('div[id$=divGraphicalVOV] img[id$=linkWallLBL]');
        var imgWallR = $('div[id$=divGraphicalVOV] img[id$=linkWallLBR]');
        var imgTrade = $('div[id$=divGraphicalVOV] img[id$=linkTrade]');
        var imgPalace = $('div[id$=divGraphicalVOV] img[id$=linkPalace]');
        
        addHover($('area[id$=areaHQ]'), imgHQ) ;
        addHover($('area[id$=areaTreasury]'), imgTres) ;
        addHover($('area[id$=areaSilverMine]'), imgMine);
        addHover($('area[id$=areaFarmLand]'), imgFarm) ;
        addHover($('area[id$=areaStable]'), imgStable) ;
        addHover($('area[id$=areaBarracks]'), imgBar) ;
        addHover($('area[id$=areaSiege]'), imgSiege) ;
        addHover($('area[id$=areaHidingSpot]'), imgSH) ;
        addHover($('area[id$=areaTavern]'), imgTav) ;
        addHover($('area[id$=areaTrade]'), imgTrade) ;


       $('area[id$=areaTowersL]').mouseover(function(){imgTowerR.addClass('vovHover'); imgTowerL.addClass('vovHover');});
       $('area[id$=areaTowersL]').mouseout(function(){imgTowerR.removeClass('vovHover');imgTowerL.removeClass('vovHover');});
       $('area[id$=areaTowersR]').mouseover(function(){imgTowerR.addClass('vovHover'); imgTowerL.addClass('vovHover');});
       $('area[id$=areaTowersR]').mouseout(function(){imgTowerR.removeClass('vovHover');imgTowerL.removeClass('vovHover');});
       $('area[id$=areaWall]').mouseover(function () { imgWallM.addClass('vovHover'); imgWallL.addClass('vovHover');imgWallR.addClass('vovHover');});
       $('area[id$=areaWall]').mouseout(function () { imgWallM.removeClass('vovHover');imgWallL.removeClass('vovHover');imgWallR.removeClass('vovHover');});
       $('area[id$=areaPalace]').mouseover(function () { imgPalace.addClass('vovHover');});
       $('area[id$=areaPalace]').mouseout( function () { imgPalace.removeClass('vovHover');});

    }
);

function addHover(area, img) 
{
    area.mouseover(function(){img.addClass('vovHover');});
    area.mouseout(function (){img.removeClass('vovHover');});
}


$(
    function() {

        var linkBuildTimer;
        var linkBuildCounter = 0;

        $('a[id$=linkBuild]')
            .click(function() {

                var op = $('div[id$=panelBuild]').css('opacity');

                if (op != 0 && op != 1) { return false; }

                if (linkBuildTimer) {
                    off(); return false;
                }

                $('div[id$=panelBuild]').fadeIn('slow');

                linkBuildTimer = setInterval(function() {

                    if ($('div[id$=panelBuild]').hasClass('over') || $('a[id$=linkBuild]').hasClass('over')) {
                        linkBuildCounter = 0;
                    } else {
                        linkBuildCounter++;
                    }

                    if (linkBuildCounter > 3) {
                        off();
                    }

                }, 200);

                return false;
            });

        function off() {
            if (linkBuildTimer) {
                $('div[id$=panelBuild]').fadeOut('slow');
                clearInterval(linkBuildTimer);
                linkBuildCounter = 0;
                linkBuildTimer = null;
            }
        }

        $(document).click(off);

        $('div[id$=panelBuild], a[id$=linkBuild]').hoverClass('over');

        //        $('a[id$=linkBuild]').toggle(function(){$('div[id$=panelBuild]').fadeIn('slow');return false;}
        //        , function(){$('div[id$=panelBuild]').fadeOut('slow');return false;});
        // $('div[id$=panelBuild]').mouseout(function(){$('div[id$=panelBuild]').fadeOut('slow');});
    }
 );