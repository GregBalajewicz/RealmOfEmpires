page.load.push(function() {
    $('.FT .tags a').filter('.Tag,.noTag').click(function(e) {
        e.preventDefault(); var s = $(this); //.addClass('loading');

        ajax('VillageTagAjax.aspx', { op: s.attr('op'), vilid: s.attr('vilid'), tagid: s.attr('tagid') }, function() {
            s.toggleClass('noTag').toggleClass('Tag').addClass('jc');
            //removeClass('loading'); //just changed
            if (s.attr('op') == "-") {
                s.attr('op', '+')
            } else {
                s.attr('op', '-');           
            }            
        });

        s.parents('.FT:first').attr('keep', 'yes');
    }).mouseout(function() { $(this).removeClass('jc') }); ;
});