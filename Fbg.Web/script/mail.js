
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

function MessageDet(link, title) {
    var tr = $(link).parent().parent();
    $('.NewReport', tr).removeClass('NewReport');
    $('.NewReport', tr).addClass('IsViewed');
    $('input:image', tr).replaceWith("<img src='https://static.realmofempires.com/images/OldMail.PNG'/>");
    return genericModalPopup(link, title)
    // return false;       
}