$(function () {
    var pid = ROE.playerID;
    var tab = $.cookie('troops.tab:' + pid);
    if (tab) { setab(tab); }

    $('.troops .tabs li').click(function () {
        var tab = $(this).attr('tab')
        setab(tab);
        $.cookie('troops.tab:' + pid, tab);
    });

    function setab(tab) {
        $('.troops .tabs ul li').removeClass('selected')
        $('.troops .tabs ul li[tab=' + tab + ']').addClass('selected');

        $('.troops .list .panel').hide();
        var p = $('.troops .list .' + tab).show();

        //$('.troops .tabs ul li[tab=det]').html(tab == 'det' ? '[-]' : '[+]');
    }
});