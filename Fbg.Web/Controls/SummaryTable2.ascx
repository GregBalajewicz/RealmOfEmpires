<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SummaryTable2.ascx.cs" Inherits="Controls_SummaryTable2" %>

<link type="text/json" rel="help" href="static/help/k_VillageSummary_Buildings.json.aspx" />
<link href="static/map.3.css" rel="stylesheet" type="text/css" />
<script type="text/javascript" src="script/myvillages.js?4"></script>
<script type="text/javascript">
    
    page.load.push(function() {
        $('.unitCountSelectors .unitCountSelector').click(function() {
            myvillages.setunit($(this));
        });

        myvillages.setunit($('.unitCountSelectors .unitCountSelector.selected'));

        $('.nox').click(function(e) {
            e.preventDefault(); var s = $(this);
            var rel = s.attr('rel');
            var vid = s.parents('tr:first').attr('vid');

            ajax('NoQAddDeleteAjax.aspx', { type: rel == 'k_tran' ? 'add' : 'delete', vid: vid }, function(d) {
                if (d) {
                    if (rel == 'k_tran') {
                        $('img', s).attr('src', "https://static.realmofempires.com/images/cancel_8x8.png");
                        $(s).attr('rel', 'k_notran');
                    } else {
                        $('img', s).attr('src', "https://static.realmofempires.com/images/checkmark2_8x8.gif");
                        $(s).attr('rel', 'k_tran');
                    }
                }
            });
        });

        $('.villages .name').timerpopup(function(hm, tp) {
            hm.empty();
            hm.append('<div class="hoverMenu"></div>');

            var a = hm.find('.hoverMenu');

            a.append(BuildAnchor('One-Click Support', BuildURL('MyVillages.aspx', ['x=' + tp.attr('x'), 'y=' + tp.attr('y'), 'cmd=0', 'oneclick=1', 'ru=CommandTroopsPopup.aspx%3fx%3d' + tp.attr('x') + '%26y%3d' + tp.attr('y') + '%26cmd%3d0']), { 'onclick': "return popupQuickSendTroops(this," + tp.attr('rel') + ",'Support' );", 'class': 'sup' }));
            a.append('<a href="QuickTransportCoins.aspx?svid=' + tp.attr('rel') + '" onclick="return popupQuickSilverTransport(this);">Quick send silver</a>');
        });

    });
    
</script>

<style>
    .smlIcon
    {
        width: 12px;
        height: 12px;
    }
    a.chest
    {
        white-space: nowrap;
        padding: 2px 5px 2px 22px;
        background: url('https://static.realmofempires.com/images/chesticon.png' ) 4px center no-repeat;
    }
    
    .jsST .zuc
    {
	    color: Gray;
    }
    
    .jsST .ycur
    ,.jsST .yttl
    ,.jsST .sup
    {
        display:none;
    }
    
    .unitCountSelectors a.selected
    {
        font-weight:bold;
    }
      
    #summary .tags .tag { text-decoration: none; cursor: pointer }
    #summary .tags .tag.on { color: #fff;  }
    #summary .tags .tag.on:hover { color: #fa0625; }
    #summary .tags .tag.off { color: #000;  }
    #summary .tags .tag.off:hover { color: #00C125; }
    
</style>

<script type="text/javascript">
    var i; var check = 0;
    function SelectAll() {
      $('.vills input').each( function () {
            this.checked = true;
        }); 
      return false;
    }

    page.load.push(function() {
        $('#summary .tags a').filter('.on,.off').click(function(e) {
            e.preventDefault(); var s = $(this);

            ajax('VillageTagAjax.aspx', { op: (s.hasClass('on') ? '-' : '+'), vilid: s.attr('vilid'), tagid: s.attr('tagid') }, function() {
                s.toggleClass('off').toggleClass('on');
            });
        }).mouseout(function() { $(this).removeClass('jc') }); ;
    });
            
</script>

<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <div id="summary">
        <table cellpadding="0" cellspacing="0">
            <%if(!IsMobileOrD2){ %>
            <tr>
                <td colspan="2" style="padding: 1px">
                    <div class="BoxHeader">
                        <span rel="k_SelectedFilter" class="help">
                            <asp:Image ID="imgFilter" runat="server" ImageUrl="https://static.realmofempires.com/images/funnel2.png" />
                            <asp:Label ID="lblSelectedFilter" runat="server" Text="Showing all villages"></asp:Label>
                        </span>
                    </div>
                </td>
            </tr>
            <%} %>
            <tr>
                <td colspan="2">
                    <asp:Table  ID="tblBuildings" runat="server" rel="food" CssClass="help stripeTable TypicalTable villages" CellPadding="2" CellSpacing="1">
                        <asp:TableRow ID="TableRow1" runat="server" CssClass="Sectionheader">
                            <asp:TableCell ID="TableCell2" runat="server" Wrap="False"></asp:TableCell>
                            <asp:TableCell ID="TableCell3" runat="server">Points</asp:TableCell>
                            <asp:TableCell ID="TableCell4" runat="server">Silver</asp:TableCell>
                            <asp:TableCell ID="TableCell5" runat="server" Wrap="False">Food</asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                    <asp:Table ID="tblUnits" runat="server" CellPadding="2" CellSpacing="1" rel="food" CssClass="jsST help stripeTable TypicalTable villages">
                        <asp:TableRow ID="tblUnits_TableRow1" runat="server" CssClass="Sectionheader">
                            <asp:TableCell ID="tblUnits_TableCell2" runat="server" Wrap="False"></asp:TableCell>
                            <asp:TableCell ID="TableCell6" runat="server">Points</asp:TableCell>
                            <asp:TableCell ID="tblUnits_TableCell1" runat="server">Silver</asp:TableCell>
                            <asp:TableCell ID="tblUnits_TableCell4" runat="server" Wrap="False">Food</asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                    <div runat="server" class="unitCountSelectors" id="unitCountSelectors">SHOW: <a class="unitCountSelector selected" rel="ttl" href="#">All troops in village</a> | <a class="unitCountSelector" rel="yttl" href="#">Your troops - all</a> | <a class="unitCountSelector" rel="ycur" href="#">Your troops -  in village now</a> | <a class="unitCountSelector" rel="sup" href="#">Support</a></div>
                    <asp:Table ID="tblVillageList" runat="server" rel="food" CssClass="help stripeTable TypicalTable" CellPadding="2" CellSpacing="1">
                        <asp:TableRow ID="TableRow2" runat="server" CssClass="Sectionheader">
                            <asp:TableCell ID="TableCell8" runat="server" Wrap="False"></asp:TableCell>
                            <asp:TableCell ID="TableCell9" runat="server">Points</asp:TableCell>
                            <asp:TableCell ID="TableCell10" runat="server">Silver</asp:TableCell>
                            <asp:TableCell ID="TableCell11" runat="server" Wrap="False">Food</asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                    <asp:Table ID="tblTags" runat="server" CssClass="help stripeTable TypicalTable tags" CellPadding="2" CellSpacing="1">
                        <asp:TableRow ID="TableRow3" runat="server" CssClass="Sectionheader">
                            <asp:TableCell ID="TableCell7" runat="server" Wrap="False">Name</asp:TableCell>
                            <asp:TableCell ID="TableCell12" runat="server" Wrap="False">Tags</asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                    <center>
                        
                        <asp:Panel ID="panelPF" runat="server" Visible="true">
                            <asp:HyperLink CssClass="LockedFeatureLink" Style="font-size: 10pt;" ID="linkPF" runat="server" Target="_blank" OnClick="return popupUnlock(this);">Unlock this Premium Feature now and see all your villages!</asp:HyperLink>
                        </asp:Panel>
                    </center>
                </td>
            </tr>
        </table>
        </div>
        <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
    </ContentTemplate>
</asp:UpdatePanel>
