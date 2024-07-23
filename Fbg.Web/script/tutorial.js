var imgPointer;
var vov;

function tutorial_init() {
    imgPointer = $('img[id$=imgPointer]');
    vov = $('div[id$=divGraphicalVOV]');
    //page.load.push(tutorial_init);
}

$(tutorial_init);


 
function PointToLevel(levelLinkName ) {

    tutorial_init();
    
    var level = $('span[id$=' + levelLinkName+ ']'); 

    imgPointer.fadeIn('slow',function(){
      $(this).animate(
            {'top': level.position().top + vov.position().top + 10
            , 'left': level.position().left + vov.position().left - (imgPointer.width()/2) + 5} 
            ,'2000');
    });
}


 
function PointToSilverProd() {

    tutorial_init();
    
    var lblProd = $('span[id$=ctrlVillageHeaderInfo_lblProduction]');

    imgPointer.fadeIn('slow',function(){
      $(this).animate(
            {'top': lblProd.position().top  + 10
            , 'left': lblProd.position().left - (imgPointer.width()/2)} 
            ,'2000');
    });
}


 
function PointToBuilding(buildingImgName) {

    tutorial_init();
    
    var img = $('img[id$=' + buildingImgName+ ']'); 

    imgPointer.fadeIn('slow',function(){
      $(this).animate(
            {'top': img.position().top + vov.position().top + (img.height()/2)
            , 'left': img.position().left + vov.position().left} 
            ,'2000');
    });

}

function PointToVilName() {

    tutorial_init();
    
    var lblProd = $('a[id$=linkVillageName]');

    imgPointer.fadeIn('slow',function(){
      $(this).animate(
            {'top': lblProd.position().top  + 10
            , 'left': lblProd.position().left} 
            ,'2000');
    });
}
function PointToMapLink() {

    tutorial_init();
    
    var map = $('a[id$=linkMap]');

    imgPointer.fadeIn('slow',function(){
      $(this).animate(
            {'top': map.position().top  + 10
            , 'left': map.position().left} 
            ,'2000');
    });
}


function PointToTroopsTable() {

    tutorial_init();
    
    var tblTroops = $('#troopsTable');

    imgPointer.fadeIn('slow',function(){
      $(this).animate(
            {'top': tblTroops.position().top  + 40
            , 'left': tblTroops.position().left - (imgPointer.width()/2) + 130} 
            ,'3000');
    });
}


function PointToCM() {

    tutorial_init();
    
    var tblTroops = $('#troopsTable');

    imgPointer.fadeIn('slow',function(){
      $(this).animate(
            {'top': tblTroops.position().top  + 40
            , 'left': tblTroops.position().left - (imgPointer.width()/2) + 30} 
            ,'3000');
    });
}



 
function PointToBuild(loc) {

    tutorial_init();
    
    var itm = $('.buildLink'); 

    if(loc) {
        //start the pointer at location of click 
        imgPointer.animate(
            {'top': $(loc).position().top
            , 'left': $(loc).position().left} 
            ,1)
    }

    imgPointer.fadeIn('slow',function(){
      $(this).animate(
            {'top': itm.position().top + vov.position().top + 15
            , 'left': itm.position().left + vov.position().left} 
            ,1000, function() {
            imgPointer.fadeOut(6000);
            });
    });

    
}

function PointToResearch(loc) {

    tutorial_init();
    
    var itm = $('.researchLink'); 

    if(loc) {
        //start the pointer at location of click 
        imgPointer.animate(
            {'top': $(loc).position().top
            , 'left': $(loc).position().left} 
            ,1)
    }

    imgPointer.fadeIn('slow',function(){
      $(this).animate(
            {'top': itm.position().top + vov.position().top + 15
            , 'left': itm.position().left + vov.position().left} 
            ,1000, function() {
            imgPointer.fadeOut(6000);
            });
    });

    
}


function PointToUpgrade() {

    tutorial_init();
    
    var img = $('a[id$=buttonUpgrade]');
    imgPointer.fadeIn('slow',function(){
      $(this).animate(
            {'top': img.position().top + 20
            , 'left': img.position().left + 50 } 
            ,'2000');
    });

}