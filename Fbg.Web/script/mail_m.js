
function SelectAll() {
    $('.reportSet input').each(function () {
        this.checked = true;
    }
   ); return false;
}

function DeleteRow(recID) {
    $('.jsml tr[jsID$=rec' + recID + ']').fadeOut(500, function () {
        $(this).remove();
        if ($('.jsml tr').length == 1) {
            $('.jsml').hide();
            $('tr.jsDelIfNoM').hide();
        }
    }
    );


}

/*
function MessageDet(link, title) {
    var tr = $(link).parent().parent();
    $('#popupIframe_popup')
        .attr('keep', 'no')
        .remove();
    $('.NewReport', tr).removeClass('NewReport');
    $('.NewReport', tr).addClass('IsViewed');
    $('input:image', tr).replaceWith("<img src='https://static.realmofempires.com/images/OldMail.PNG'/>");
    //$('.messagesContainer').fadeOut();
    //$('.messagesContainer').addClass('block');
    return genericModalFlip(link, title)
}

//
//
// this is a temporary flip code. it must be fixed, cleaned up and modulerized
// 
//  note that the code to turn off the loading code exists in messagesDetail.aspx
//
//

function genericModalFlip(link, winName) {
    var url = $(link).attr('href');
    return !popupModalFlip(url, 'popup', winName, 1000, 680, 20, 40);
}
 

function popupModalFlip(url, name, title, height, width, x, y, f) {

    height = height ? height : 465;
    width = width ? width : 305;
    x = x ? x : 0;
    y = y ? y : 0;
    f = f ? f : 'closeModalIFrame';


    return popupFlip(url, name, title, height, width, x, y, f);
}



var intervalID;

function popupFlip(url, name, title, height, width, x, y, func) {

    // construct a unique-ish id
    var id = 'popupIframe_' + name;

    // create the div, if it doesn't exist!
    if ($('#' + id).length > 0) {
        $('#' + id)
            .show()                // display the (posiably) hidden iframe div
            .attr('keep', 'yes');   // flag it as active
    } else {
        $('#block').append('<div id="' + id + '" class="flipContainer back side" ></div>')
        $('#' + id)
            //.css({ top: y + 'px', left: x + 'px' })
            .append('<iframe name="' + name + '" border="0" style="height:' + height + 'px; width:' + width + 'px; border:none;" src="' + url + '" frameborder="0"></iframe>')
            .show()
            .attr('keep', 'yes')    // so that it's not instantly cleaned up.
            .mouseover(function () { $(this).attr('keep', 'yes'); }); // flag as active when it's hovered
    }

    $('#' + id).append("<span class=loading style='position:absolute; left:50px; top: 50px;font-size:14pt;'><img src='https://static.realmofempires.com/images/misc/ajax-loader1.gif' /></span>");

    intervalID = setInterval(loadingUpdate('#' + id + ' .loading'), 1500);

    $('#block').addClass('rotated')



    return true;
}




function loadingUpdate(selector) {    
    counter=0;
    messages = ['summoning royal scribes', 'lighting candles', 'brewing good coffee', 'summing royal scribes ...again!', 'hangging no-show scribes, hiring new ones']
    if ($(selector + " .msg").length === 0) {
        $(selector).append("<span class=msg></span>");
    }
    $(selector + " .msg").text(messages[counter]);
    counter++

    return function () {
        if ($(selector).length === 0) {
            clearInterval(intervalID);
        } else {
            $(selector + " .msg").text(messages[counter]);
            counter++;
        }
    }
}

*/
function MessageDet(link, title) {
    var tr = $(link).parent().parent();
    $('.NewReport', tr).removeClass('NewReport');
    $('.NewReport', tr).addClass('IsViewed');
    $('input:image', tr).replaceWith("<img src='https://static.realmofempires.com/images/OldMail.PNG'/>");
    return !popupIFrameFlip($(link).attr("href"), "messageDetail", title, $(window).height(), $(window).width(), ".messagesContainer");
}
