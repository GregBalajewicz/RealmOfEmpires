<%@ Page Language="C#" MasterPageFile="~/main.master" AutoEventWireup="true" CodeFile="globalStats.aspx.cs" Inherits="globalStats" %>

<%@ Register Src="~/Controls/RankingMenu.ascx" TagName="RankingMenuControl" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/GlobalStats.ascx" TagName="GlobalStatsControl" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
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
        padding: 4px;
    }
    .Padding {
        padding: 4px;
    }

        .Sectionheader {
             color: #FFFFFF;
        font-weight: bold;
          background-color: rgba(49, 81, 108, 0.7);
        }
    .Sectionheader td {
       
        padding: 4px;
      
    }


     .TableHeaderRow {
        font-size: 12px !important;
        font-weight: normal;
        background-color: rgba(49, 81, 108, 0.7) !important;
        padding: 4px;
    }

        .stats {
            padding:12px;
        }

        .stats table {
            width:100%;
        }
        .stats table .TypicalTable {
            width:initial;
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



    </style>
    <% } %> 
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<%/*=Utils.GetIframePopupHeaderForNotPopupBrowser("Global Statistics" , (isMobile && !IsiFramePopupsBrowser))*/%>
<uc1:RankingMenuControl ID="RMenuControl" runat="Server" />
<uc2:GlobalStatscontrol ID="GStats" runat="server" />
     <% if(isD2) { %>
    <script type="text/javascript">
        $('.Sectionheader a').attr("href", "#");

    </script>
    
    <% } %>

</asp:Content>

