// JScript File

    function InitAutoPop()
    {
        $('.AutoPop').click(
            function ()
            {
                limit = $(this).text().match(/([\d\,]+)/);
                limit = limit[1].replace(/,/g, '');
                
                $("input[rel='" + $(this).attr('rel') + "']").val(limit);
            }
        );
        //$('.AutoPop').css( { 'color' : 'rgb(0,0,255)', 'text-decoration' : 'underline', 'cursor' : 'pointer' } );
        $('.AutoPop').wrap('<a href="#" onclick="return false;"></a>');
    }
    function InitAutoPop2()
    {
        $('.AutoPop2').click(
            function ()
            {                
                $("input[rel='" + $(this).attr('rel') + "']").val($(this).attr('val'));
            }
        );
    }
    function InitAutoPop3()
    {
        $('.AutoPop3').click(
            function ()
            {                
                $("input[rel='" + $(this).attr('rel1') + "']").val($(this).attr('val1'));
                $("input[rel='" + $(this).attr('rel2') + "']").val($(this).attr('val2'));
            }
        );
    }


