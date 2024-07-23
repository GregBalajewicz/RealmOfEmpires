<%@ Page Language="C#" MasterPageFile="~/main.master" AutoEventWireup="true" CodeFile="Stats.aspx.cs"
    Inherits="Stats" %>

<%@ Register Assembly="Facebook.WebControls" Namespace="Facebook.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Controls/RankingMenu.ascx" TagName="RankingMenuControl" TagPrefix="uc1" %>
<%@ Register src="Controls/PlayerRanking.ascx" tagname="PlayerRanking" tagprefix="uc2" %>
<asp:Content ContentPlaceHolderID="HeadPlaceHolder" ID="h" runat="server">
    <style type="text/css">
        tr.selected
        {
            background-color: Maroon;
        }
    </style>   
    <% if(isD2) { %>
    <style>
         html {
        background-color:#000000;
        background: url('https://static.realmofempires.com/images/backgrounds/M_BG_Tower.jpg') no-repeat center center fixed; 
        -webkit-background-size: cover;
        -moz-background-size: cover;
        -o-background-size: cover;
        background-size: cover;
    }
    body { font-family: Georgia, 'Playfair Display'; font-size:12px; }
    a, a:active, a:link { color: #d3b16f; }   
    a:hover { color: #d3b16f; }
    .tempMenu {
        background-color: rgba(88, 140, 173, 0.3);
        border-bottom:2px solid  #9d9781;
        padding: 4px;
        padding-bottom: 2px;
    }
    .tempMenu a { text-shadow:1px 1px 1px #000; }
    .BoxContent {
       background-color: rgba(88, 140, 173, 0.3);
    }
       .TypicalTable .DataRowNormal {
       background-color: rgba(88, 140, 173, 0.1);
    }
    .TypicalTable .DataRowAlternate {
       background:none;
    }
    .TDContent {
        background-color: rgba(6, 20, 31, 0.9) !important;
        height: 100%;
        position: absolute;
        overflow: auto;
    }

    .TypicalTable TD {
        padding: 2px;
    }
    .Padding {
        padding: 4px;
    }
    .Sectionheader {
        color: #FFFFFF;
        font-weight: bold;
        padding: 4px;
        background-color: rgba(49, 81, 108, 0.7);
    }

     .TableHeaderRow {
        font-size: 12px !important;
        font-weight: normal;
        background-color: rgba(49, 81, 108, 0.7) !important;
        padding: 4px;
    }

        .TableHeaderRow th {
             padding: 4px 6px;
        }

        .TableHeaderRow th.dividerColumn {
             padding: 0px;
        }
    

        .playerRanking {
        padding-left: 12px;
        padding-right: 12px;
        padding-bottom: 12px;
        }

        tr.selected {
            background-color: #3D372A !important;
        }


        .inputbutton  {
            color: #FFFFFF;
            font-weight: initial;
            padding: 4px !important;
            padding-bottom: 3px !important;
            background-color: #181819 !important;
            -moz-box-shadow: inset 0 0 14px #154B86,  0px 1px 2px 0px #A69D85;
            -webkit-box-shadow: inset 0 0 14px #154B86,  0px 1px 2px 0px #A69D85;
            box-shadow: inset 0 0 14px #154B86,  0px 1px 2px 0px #A69D85;
            border:1px solid #A69D85;
            -moz-border-radius: 6px;
            -webkit-border-radius: 6px;
            border-radius: 6px;
            font-size: 12px !important;
            font-family: Georgia, 'Playfair Display';
            height:initial;
        }

        table.stripeTable {
            margin-top:8px;
        }

                .ui-menu-item {
            color:#000;
            background-color: #CED5DA;
        }

        .areaHelpInlineImage {
           width: 307px;
            height: 267px;
            position: absolute;
            top: 10px;
            right: 10px;
            box-shadow: 0px 5px 10px 2px #000;
        }

        .areaHelpOn img.areaHelpInlineImage {
            display:block;
        }

        .areaHelpOff img.areaHelpInlineImage {
            display:none;
        }

        td.generalStatsColumn, td.battleStatsColumn {
            font-family: 'Droid Sans Mono', monospace;
            text-shadow: 1px 1px 0 #000,1px 1px 0 #000;
        }

    </style>
    <% } %> 
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
<%/*=Utils.GetIframePopupHeaderForNotPopupBrowser("Player Ranking" , (isMobile && !IsiFramePopupsBrowser),"https://static.realmofempires.com/images/icons/m_ranking.png")*/%>    
    <uc1:RankingMenuControl ID="RMenuControl" runat="Server" />
    <%if (!isMobile)
      { %><br /> <%} %>
    <uc2:PlayerRanking ID="PlayerRanking1" runat="server" />
    <% if(isD2) { %>
    <script type="text/javascript">

        var areaHelp = $('.areaHelp');
        areaHelp.addClass('areaHelpOff');
        var href = areaHelp.attr('href');
        areaHelp.append($("<img class='areaHelpInlineImage' src='" + href + "' />"));

        $('.areaHelp').click(function (e) {
            e.preventDefault();
           
            var a = $(e.currentTarget);
            if (a.hasClass('areaHelpOff')) {
                a.removeClass('areaHelpOff').addClass('areaHelpOn');
            } else {
                a.removeClass('areaHelpOn').addClass('areaHelpOff');
            }

        });

    </script>
     <% } %> 
</asp:Content>
