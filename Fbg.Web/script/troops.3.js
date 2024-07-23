/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.6.4-vsdoc.js" />
/// <reference path="interfaces.js" />
/// <reference path="ROE_c.js" />
/// <reference path="ROE-utils.js" />
/// <reference path="troops.3.js" />

function Cancel(link, eventID) {
    ret = Troops.CancelTroopMovement(eventID, OnComplete, OnTimeOut, OnError);
    $(link).parent().append('<img id=loadcancel_eid' + eventID + ' src="https://static.realmofempires.com/images/misc/busy_tinyred.gif" />');
    $(link).remove();
}

function OnComplete(arg) 
{   
    
    // this is the image that was clicked to cancel. it ids the row and table. 
    var loadingImg = $('#loadcancel_eid' + arg);
    var row = $(loadingImg).parent().parent();
    var cell = $(loadingImg).parent();
    var table = $(row).parent();

    if (arg == -1) {
        $(row).removeClass('highlight').addClass('highlight_d1');
        $(row).after('<tr class=highlight_u1><TD></TD><td style=padding:0px colspan=5>Error. Session Expired. Refresh page and try again</td></tr>');
        $(loadingImg).remove();
        InitStripeTable();
    } else if (arg == 0) {
        $(row).removeClass('highlight').addClass('highlight_d1');
        $(row).after('<tr class=highlight_u1><TD></TD><td style=padding:0px colspan=5>Command NOT cancelled. Perhaps it already expired. Refresh page.</td></tr>');
        $(loadingImg).remove();
        InitStripeTable();
    } else {
        if ($(row).hasClass('highlight_d1')) {
        $(row).next().fadeOut('fast', function () 
                        {
                            $(this).remove();
                        });
        }
        $(row).fadeOut('fast', function () 
                        {
                            $(this).remove();
                        });
    }
}


function Details_get(link, eventID) {
    ret = Troops.GetTroopMovementDetails(eventID, Details_OnComplete, Details_OnTimeOut, Details_OnError);
    $(link).parent().append('<img id=loaddet_eid' + eventID + ' src="https://static.realmofempires.com/images/misc/busy_tinyred.gif" />');
    $(link).remove();
}



function Details_OnComplete(arg) 
{   
    var det = eval(arg);
    
    // this is the image that was clicked to get the details. it ids the row and table. 
    var loadingImg = $('#loaddet_eid' + det.eventID);
    var row = $(loadingImg).parent().parent();
    var cell = $(loadingImg).parent();
    var table = $(row).parent();

    if (det.eventID == -1) {
        $(row).removeClass('highlight').addClass('highlight_d1');
        $(row).after('<tr class=highlight_u1><TD></TD><td style=padding:0px colspan=5>Error. Session Expired. Refresh page and try again</td></tr>');
        $(loadingImg).remove();
        InitStripeTable();
    } else if (det.eventID == 0) {
        $(row).removeClass('highlight').addClass('highlight_d1');
        $(row).after('<tr class=highlight_u1><TD></TD><td style=padding:0px colspan=5>Cannot get command details. Perhaps it already expired. Refresh page.</td></tr>');
        $(loadingImg).remove();
        InitStripeTable();
    } else {
        str = '<table style="float:right;" border=0 cellpadding=0 cellspacing=1><tr class="TableHeaderRow"><td align="center"><img src="https://static.realmofempires.com/images/units/Militia.png" /></td><td align="center"><img title="Infantry" src="https://static.realmofempires.com/images/units/Infantry.png"/></td><td align="center"><img src="https://static.realmofempires.com/images/units/Cavalry.png" /></td><td align="center"><img title="Knight" src="https://static.realmofempires.com/images/units/Knight.png" /></td><td align="center"><img title="Ram" src="https://static.realmofempires.com/images/units/ram.png" /></td><td align="center"><img title="Trebuchet" src="https://static.realmofempires.com/images/units/treb.png" /></td><td align="center"><img title="Spy" src="https://static.realmofempires.com/images/units/Spy.png" /></td><td align="center"><img title="Governor" src="https://static.realmofempires.com/images/units/Governor.png"  /></td>';
	    str +=  '</tr><tr>';
		str += '<td align="right" ' + (det.ut11 ? '' : 'class=ZeroUnitCount ')+ '>' + (det.ut11 ? det.ut11 : 0) + '</td>';
		str += '<td align="right" ' + (det.ut2 ? '' : 'class=ZeroUnitCount ')+ '>' + (det.ut2 ? det.ut2 : 0) + '</td>';
		str += '<td align="right" ' + (det.ut5 ? '' : 'class=ZeroUnitCount ')+ '>' + (det.ut5 ? det.ut5 : 0) + '</td>';
		str += '<td align="right" ' + (det.ut6 ? '' : 'class=ZeroUnitCount ')+ '>' + (det.ut6 ? det.ut6 : 0) + '</td>';
		str += '<td align="right" ' + (det.ut7 ? '' : 'class=ZeroUnitCount ')+ '>' + (det.ut7 ? det.ut7 : 0) + '</td>';
		str += '<td align="right" ' + (det.ut8 ? '' : 'class=ZeroUnitCount ')+ '>' + (det.ut8 ? det.ut8 : 0) + '</td>';
		str += '<td align="right" ' + (det.ut12 ? '' : 'class=ZeroUnitCount ')+ '>' + (det.ut12 ? det.ut12 : 0) + '</td>';
		str += '<td align="right" ' + (det.ut10 ? '' : 'class=ZeroUnitCount ')+ '>' + (det.ut10 ? det.ut10 : 0) + '</td>';
	    str +=  '</tr></table>';

        $(row).removeClass('highlight').addClass('highlight_d1');
        $(row).after('<tr class=highlight_u1><TD></TD><td style=padding:0px colspan=5>' + str + '</td></tr>');
        $(loadingImg).remove();
        cell.append('<img id=det_eid' + det.eventID + ' src="https://static.realmofempires.com/images/details.png" />');
        
        var detImg =  $('#det_eid' + det.eventID, table);
        $(detImg).toggle(
            function () 
            {
                $(detImg).parent().parent().next().hide();
            },function () 
            {
                $(detImg).parent().parent().next().show();
            }
            );

        InitStripeTable();
    }
}


function ToggleHide(link, eventID) {
    Troops.ToggleHideTroopMovement(eventID, ToggleHide_OnComplete, ToggleHide_OnTimeOut, ToggleHide_OnError);
    $(link).parent().append('<img id=loadToggleHide_eid' + eventID + ' src="https://static.realmofempires.com/images/misc/busy_tinyred.gif" />');
    $(link).remove();
}


function ToggleHide_OnComplete(arg) 
{   

    eval(arg);

    // this is the image that was clicked to cancel. it ids the row and table. 
    var loadingImg = $('#loadToggleHide_eid' + ret.eventID);
    var row = $(loadingImg).parent().parent();
    var cell = $(loadingImg).parent();
    var table = $(row).parent();

    if (arg == -1) {
        $(row).removeClass('highlight').addClass('highlight_d1');
        $(row).after('<tr class=highlight_u1><TD></TD><td style=padding:0px colspan=5>Error. Session Expired. Refresh page and try again</td></tr>');
        $(loadingImg).remove();
        InitStripeTable();
    } else if (arg == 0) {
        $(row).removeClass('highlight').addClass('highlight_d1');
        $(row).after('<tr class=highlight_u1><TD></TD><td style=padding:0px colspan=5>Error. Cannot complete action. Refresh page.</td></tr>');
        $(loadingImg).remove();
        InitStripeTable();
    } else {
        loadingImg.remove();
        row.toggleClass('hidden');
        if (ret.curHiddenState == 1) 
        {
            cell.append('<a onclick="ToggleHide(this, ' + ret.eventID + ', 0);return false;" href="#"><img src="https://static.realmofempires.com/images/hidden.png" style="border-width:0px;" /></a>');
        }
        else 
        {
            cell.append('<a onclick="ToggleHide(this, ' + ret.eventID + ', 0);return false;" href="#"><img src="https://static.realmofempires.com/images/visible.png" style="border-width:0px;" /></a>');
        }

    }
}

function ToggleHide_OnTimeOut(arg) 
{
    alert('TimeOut error. Refresh page and try again');
}

function ToggleHide_OnError(arg) 
{
    alert('Unexpected error. Refresh page and try again');
}
function Details_OnTimeOut(arg) 
{
    alert('TimeOut error while getting details. Refresh page and try again');
}

function Details_OnError(arg) 
{
    alert('Unexpected error while getting details. Refresh page and try again');
}


function OnTimeOut(arg) 
{
    alert('TimeOut error while cancelling the command. Refresh page and try again');
}

function OnError(arg) 
{
    alert('Unexpected error while cancelling the command. Refresh page and try again');
}



function SendBack(link, SiVID, SdVID) {
    ret = Troops.SendBackSupport(SiVID, SdVID, SendBack_OnComplete, Generic_OnTimeOut, Generic_OnError);
    $(link).parent().append('<img id=jsRowID_SiVID' + SiVID +  '_SdVID' + SdVID + ' src="https://static.realmofempires.com/images/misc/busy_tinyred.gif" />');
    $(link).remove();
}

function SendBack_OnComplete(arg) 
{       
    // this is the image that was clicked to cancel. it ids the row and table. 
    var loadingImg = $('#' + arg);
    var row = $(loadingImg).parent().parent();
    var cell = $(loadingImg).parent();
    var table = $(row).parent();

    if (arg == "") {
        $(row).removeClass('highlight').addClass('highlight_d1');
        $(row).after('<tr class=highlight_u1><TD></TD><td style=padding:0px colspan=55>Error. Session Expired. Refresh page and try again</td></tr>');
        $(loadingImg).remove();
        InitStripeTable();
    } else {
        $(row).fadeOut('fast', function () 
                        {
                            $(this).remove();
                        });
    }
}


function Generic_OnTimeOut(arg) 
{
    alert('TimeOut error. Refresh page and try again');
}

function Generic_OnError(arg) 
{
    alert('Unexpected error. Refresh page and try again');
}


function RecallSupport(link, SiVID, SdVID) {
    ret = Troops.RecallSupport(SiVID, SdVID, RecallSupport_OnComplete, Generic_OnTimeOut, Generic_OnError);
    $(link).parent().append('<img id=jsRowID_SiVID' + SiVID +  '_SdVID' + SdVID + ' src="https://static.realmofempires.com/images/misc/busy_tinyred.gif" />');
    $(link).remove();
}

function RecallSupport_OnComplete(arg) 
{       
    // this is the image that was clicked to cancel. it ids the row and table. 
    var loadingImg = $('#' + arg);
    var row = $(loadingImg).parent().parent();
    var cell = $(loadingImg).parent();
    var table = $(row).parent();

    if (arg == "") {
        $(row).removeClass('highlight').addClass('highlight_d1');
        $(row).after('<tr class=highlight_u1><TD></TD><td style=padding:0px colspan=55>Error. Session Expired. Refresh page and try again</td></tr>');
        $(loadingImg).remove();
        InitStripeTable();
    } else {
        $(row).fadeOut('fast', function () 
                        {
                            $(this).remove();
                        });
    }
}


$(function () {

    $('.jsTroopsAbroad .jsRecallSome').click(TroopFuncts.Recall.RecallSomeClicked);
    $('.jsTroopsSupporting .jsRecallSome').click(TroopFuncts.Recall.RecallSomeClicked);
});

TroopFuncts = {
    Recall: {
        isRecallingSome: false // set to true if player is currently in the process of recalling SOME troops
        , RecallSomeClicked: function () {
            var link = $(this);
            var tr = link.parent().parent(); //assuming <TR><TD>*LINK*</TD></TR>
            var originalLinkText;
            var recallNow;
            originalLinkText = link.text();

            // put a CANCEL link, and recall link
            //
            recallNow = $('<a style="margin-left:10px;">' + link.text() + '</a>');
            recallNow.click(function () { TroopFuncts.Recall.RecallSomeConfirmed_GO(tr, link, recallNow, originalLinkText) });
            link.parent().append(recallNow);

            link.text("CANCEL");
            link.unbind("click");
            link.click(function () { TroopFuncts.Recall.RecallSomeCancelClicked(originalLinkText, link, recallNow); });
            tr.find('.jsRecall').fadeOut();
            TroopFuncts.Recall.isRecallingSome = true;

            //
            // replace all units with text boxes
            //
            var newInputs = [];
            tr.find('td[uid]').each(function () {
                var td = $(this);
                var input = $('<input type=number class=TextBox style="width:40px;text-align: right;"></input>');
                newInputs.push(input);
                input.val(td.attr("data-cnt"));
                input.attr('unitcount', td.text());

                td.find("span").replaceWith(input);
            });
            if (newInputs.length > 0) {
                newInputs[0].focus();

                $(newInputs).each(function () {
                    $(this).fadeIn('fast');
                });
            }
        },

        RecallSomeCancelClicked: function (oldText, link, recallNowLink) {
            TroopFuncts.Recall.isRecallingSome = false;
            link.text(oldText);
            link.unbind("click");
            link.click(TroopFuncts.Recall.RecallSomeClicked);
            link.fadeIn();

            var tr = link.parent().parent(); //assuming <TR><TD>*LINK*</TD></TR>
            tr.find('.jsRecall').fadeIn();
            recallNowLink.fadeOut().remove();
            //
            // replace all units with text boxes
            //
            tr.find('td[uid]').each(function () {
                var td = $(this);
                var input = td.find('input');
                if (input.length > 0) {
                    td.append($("<span>").text(input.attr('unitcount')));
                    input.remove();
                }
            });
        },

        RecallSomeConfirmed_GO: function (tr, cancelLink, recallNowLink, originalLinkText) {
            var td = cancelLink.parent();
            var validationError = false;
            var supportingVillageID;
            var supportedVillageID;

            supportingVillageID = parseInt(tr.attr("sivid"), 10);
            supportedVillageID = parseInt(tr.attr("sdvid"), 10);

            tr.find('.Error').remove();
            //
            // validate & clean up input
            tr.find('td').each(function () {
                if ($(this).attr('uid')) {
                    if ($(this).find('input').val().trim() === "") {
                        $(this).find('input').val("0");
                    }
                    if (parseInt($(this).find('input').val(), 10).toString() !== $(this).find('input').val()) {
                        $(this).find('input').after("<span class='Error' title='Invalid number'> ! </span>").fadeIn();
                        validationError = true;
                    }
                }
            });

            if (validationError) { return; }

            //
            // prepare the UI for the ajax call
            recallNowLink.fadeOut('fast');
            cancelLink.fadeOut('fast');
            td.append('<img class=busy src="https://static.realmofempires.com/images/misc/busy_tinyred.gif" />');
            tr.find('td input').attr('disabled', true);

            //
            //
            var troopsToRecall = [];
            var uid;
            tr.find('td').each(function () {
                if ($(this).attr('uid')) {
                    uid = parseInt($(this).attr('uid'), 10);
                    troopsToRecall[troopsToRecall.length] = { uid: uid, count: $(this).find('input').val() };
                }
            });

            //
            // do the recall
            //
            ret = Troops.RecallSomeSupport(supportingVillageID, supportedVillageID, $.toJSON(troopsToRecall)
            , function (arg) { TroopFuncts.Recall.RecallSome_OnComplete(arg, tr, recallNowLink, cancelLink, originalLinkText) }, Generic_OnTimeOut, Generic_OnError);
        },

        RecallSome_OnComplete: function (arg, tr, recallNowLink, cancelLink, originalLinkText) {
            TroopFuncts.Recall.isRecallingSome = false;
            var troops = $.evalJSON(arg);

            if (troops.length > 0) {
                tr.find('img.busy').remove();
                recallNowLink.remove();
                cancelLink.text(originalLinkText);
                cancelLink.unbind("click");
                cancelLink.click(TroopFuncts.Recall.RecallSomeClicked);
                cancelLink.fadeIn();
                tr.find('.jsRecall').fadeIn();

                // foreach TD with troops, see if found in returned array. update if found, remove necessary attributes if not
                tr.find('td[uid]').each(function () {
                    uid = parseInt($(this).attr('uid'), 10);

                    var count = "0";
                    for (var i in troops) {
                        if (troops[i].uid === uid) {
                            count = troops[i].count;
                            break;
                        }
                    }

                    $(this).find("input").fadeOut('fast', function () { $(this).remove(); });
                    $(this).append($("<span>").text(ROE.Utils.formatNum(count)));
                    if (count === "0") {
                        $(this).removeAttr('uid'); // 0 units left so remove the attribute 
                        $(this).removeAttr('uid');
                    }
                });
            } else {
                // no troops left in this village
                tr.fadeOut('fast', function () { $(this).remove(); });
            }
        }


    }
}

