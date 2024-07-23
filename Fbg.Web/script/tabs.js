$(function() {
    $('.tabs li').click(function() {
        var tab = $(this).attr('tab')
        setab(tab);
    });

    function setab(tab) {
        $('.tabs ul li').removeClass('selected')
        $('.tabs ul li[tab=' + tab + ']').addClass('selected');

        $('.list .panel').hide();
        var p = $('.list .' + tab).show();

        //$('.troops .tabs ul li[tab=det]').html(tab == 'det' ? '[-]' : '[+]');
    }
});