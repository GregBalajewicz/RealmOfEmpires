$(function() {

    $('.jspnlOneClickSend a.jsEnable').click(function(e) {
        e.preventDefault();

        myvillages.oneclick(this);
    });

});


var myvillages = {
    setunit: function(s) {
        s.parent().find('.unitCountSelector').removeClass('selected');
        var rel = s.addClass('selected').attr('rel');
        var uc = s.parent().parent().find('table[rel=food] .uc');

        $('.uc', uc).hide();
        $('.' + rel, uc).show();
    },
    oneclick: function(s) {
        $('.villageList .js1Clickable').each(function() {
            $(this).wrap('<a class="jsc"></a>');
        });

        $('.villageList .jsc').each(function() {
            var s = $(this);

            s.click(function() {
                var a1 = $('span', s).attr('uid');
                var a2 = parseInt($('span', s).html().replace(',', ''));
                var a3 = s.parents('tr:first').find('.jsVID').html();
                var a4 = $('.jsTargetXCord').html();
                var a5 = $('.jsTargetYCord').html();
                var a6 = $('.jsCommandTypeID').html();
                var a7 = $('.jsiss').html();
                Troops.ExecuteOneUnitCommand(
                            a1, a2, a3, a4, a5, a6, a7,
                            function(d) {
                                $('#commandResults').append("<LI>" + d + "</li>");

                                if (d.indexOf("Problem executing command") != -1) {
                                    s.hide().after('<span><img src="https://static.realmofempires.com/images/cancel.png"/></span>');
                                    return;
                                }

                                s.fadeOut('fast', 'swing', function() {
                                    s.after('<span class="zero">-</span>');
                                });
                            }
                        );
            });
        });

        $(s).fadeOut('slow');
        $('#oneClickEnabledPanel').fadeIn('slow');
    }
};
