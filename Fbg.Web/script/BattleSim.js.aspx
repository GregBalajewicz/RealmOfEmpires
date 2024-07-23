<%@ Inherits="BasePageWithRes"  %>

function popupHandicapCalc(link)
{
    var url = $(link).attr('href');
    var p = $(link).position();
    //var w = popupWindow2(url, 'unlock', 450, 350);
    
    return !popupIFrame(url, 'HandicapCalc', "<%=RS("HandicapCalculator")%>", 210, 550, p.left-200, p.top - 300);
}


function popupDesertionCalc(link)
{
    var url = $(link).attr('href');
    var p = $(link).position();
    //var w = popupWindow2(url, 'unlock', 450, 350);
    
    return !popupIFrame(url, 'DistanceCalc', "<%=RS("DistanceCalculator")%>", 210, 550, p.left-200, p.top - 300);
}

  function CopyRemDef(e) {
        var inputsEffected = Array();


        $('span[id*=DefendingRemaining]').each(
                
                function () {
                    //
                    // Copy defending troops remaining to defending troops input
                    //
                    var remTroopsEl = $(this);
                    var remTroopsNum = parseInt(remTroopsEl.text().replace(",",""), 10);
                    remTroopsNum = remTroopsNum || 0;

                    // assuming that the 'span[id*=DefendingRemaining]' is a span inside a cell inside a row, we get the containing row
                    var containingRow = remTroopsEl.first().parents("tr");

                    //now find the defending troops input in that row, and set the value
                    var defendingTroopsInput = $('input[id*=DefendUnitAmount]', containingRow[0]).first();

                    defendingTroopsInput.val(remTroopsNum);
                    inputsEffected.push(defendingTroopsInput);
                }
            );
        //
        // copy wall and tower levels
        //
        if ($('span[id$=WallAfterAttack]').length > 0) {
            var newlvl = $('span[id$=WallAfterAttack]').first().text().match(/\d+/);
            $('input[id$=DefenderWallLevel]').val(newlvl);
            inputsEffected.push($('input[id$=DefenderWallLevel]'));
        }
        if ($('span[id$=TowerAfterAttack]').length > 0) {
            var newlvl = $('span[id$=TowerAfterAttack]').first().text().match(/\d+/);
            $('input[id$=DefenderTowerLevel]').val(newlvl);
            inputsEffected.push($('input[id$=DefenderTowerLevel]'));
        }
        $(e).unbind();
        //
        // animate to show what happend
        //
        $(e).fadeOut('fast', function () {
            $(this).text("done!").stop().fadeIn('fast');
        });

       
        // TODO: This code is failing. For some reason everything
        // fades out and in, except the first box (militia) doesn't fade
        // all the way and doesn't fade in after.
        /*       
        $(inputsEffected).each(function () {
            $(this).first().stop().fadeTo('fast', 0.33, function () {
                $(this).first().fadeTo('fast', 1);
            });
        });
        */
    }  