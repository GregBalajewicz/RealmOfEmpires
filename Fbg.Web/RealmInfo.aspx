<%@ Page Language="C#" MasterPageFile="~/main.master" AutoEventWireup="true" CodeFile="RealmInfo.aspx.cs" Inherits="RealmInfo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <% if (isD2) { %>
    <style>
        body {
            font-family: Georgia, 'Playfair Display';
            font-size: 12px;
            background: none;
        }

        html {
            background-color: #000;
            background: url('https://static.realmofempires.com/images/backgrounds/M_BG_Tower.jpg') no-repeat center center fixed;
            -webkit-background-size: cover;
            -moz-background-size: cover;
            -o-background-size: cover;
            background-size: cover;
        }

        .TDContent {
            background-color: rgba(6, 20, 31, 0.9) !important;
            height: 100%;
            position: absolute;
            overflow: auto;

            box-sizing: border-box;
            font-size: 13px;
        }

        .BoxHeader {
            color: #FFFFFF;
            font-size: 14px !important;
            font-weight: normal;
            background-color: rgba(49, 81, 108, 0.7) !important;
            padding: 4px;
        }

        .inputbutton {
            color: #FFFFFF;
            font-weight: initial;
            padding: 4px !important;
            padding-bottom: 3px !important;
            background-color: #181819 !important;
            -moz-box-shadow: inset 0 0 14px #154B86, 0px 1px 2px 0px #A69D85;
            -webkit-box-shadow: inset 0 0 14px #154B86, 0px 1px 2px 0px #A69D85;
            box-shadow: inset 0 0 14px #154B86, 0px 1px 2px 0px #A69D85;
            border: 1px solid #A69D85;
            -moz-border-radius: 6px;
            -webkit-border-radius: 6px;
            border-radius: 6px;
            font-size: 12px !important;
            font-family: Georgia, 'Playfair Display';
            height: initial;
            margin: 4px 0px;
            cursor: pointer;
        }

        .d2_boxedContent {
            padding: 6px !important;
        }

        .d2_tableWrapper {
            border-spacing: 0 !important;
        }

            .d2_tableWrapper td {
                padding: 0px;
                padding-bottom: 6px;
            }


        .d2_border {
            border: none !important;
        }

        .d2_stripeRow {
            background-color: rgba(88, 140, 173, 0.1);
        }

        .d2_stripeRowCol {
            padding: 0;
        }

        .infoPanel {
            float: left;
            position: relative;
            min-width: 310px;
            background-color: rgba(0, 0, 0, 0.3);
            padding: 10px;
            font-size: 14px;
            padding-top: 5px;
            color: #FFFFFF;
            margin: 5px;
        }
    </style>
    <% } %>

    <% if (isMobile) { %>
        <style>
            .infoPanel {
                float: left;
                position: relative;
                min-width: 310px;
                background-color: rgba(0, 0, 0, 0.7);
                padding: 10px;
                font-size: 14px;
                padding-top: 5px;
                color: #FFFFFF;
                margin: 5px;
            }
        </style>
    <% } %>

    <!-- common style -->
    <style>
        .headSection {
            margin-bottom: 20px;
        }

        .textBoxStyle {
            border-color: #4B3D32;
            border-style: solid;
            border-width: 1px;
            color: #000;
            box-sizing: border-box;
            background-color: rgba(255, 255, 255, 0.8);
            font-weight: normal;
            font-size: 12px;
            font-family: Georgia, 'Playfair Display';
            padding: 4px;
            width: 150px !important;
        }

        .inputButtonStyle {
            color: #FFF;
            font-weight: initial;
            padding: 6px !important;
            background-color: #181819 !important;
            -moz-box-shadow: inset 0 0 14px #154B86, 0px 1px 2px 0px #A69D85;
            -webkit-box-shadow: inset 0 0 14px #154B86, 0px 1px 2px 0px #A69D85;
            box-shadow: inset 0 0 14px #154B86, 0px 1px 2px 0px #A69D85;
            border: 1px solid #A69D85;
            -moz-border-radius: 6px;
            -webkit-border-radius: 6px;
            border-radius: 6px;
            font-size: 12px !important;
            font-family: Georgia, 'Playfair Display';
            height: initial;
            cursor: pointer;
        }

        .step {
            margin-top: 20px;
            font-size: 16px;
            display: block;
        }

        .title {
        }

        .subtitle {
            font-size: 16px;
        }

        .maintable {
            width: 100%;
        }


        .success {
            color: green;
        }

        .btnHideIcon {
        }


        .rxLogo {
            position: relative;
            left: 50%;
            margin-left: -37px;
            margin-bottom: 10px;
            width: 75px;
        }
        .realmDesc {
        
        }
        .realmEndingIn, .realmConcluded, .linkLeaderboard {
            margin:5px;
            text-align:center;
        }


        /*overide pie css*/
        #container #chart {
            display: block;
            float: none;
            cursor: pointer;
        }
        #container #chartData {
            border-collapse: collapse;
            background-color: #D2D2D2;
            border-spacing: 0px;
        }
        #container #chartData th, #chartData td{
                border: 0px solid #000 !important;
        }
    </style>


    
    
    

 
 
    
    
    
     <asp:Panel ID="realminfo" CssClass="infoPanel" runat="server" >
            <center>
            <%=FbgPlayer.Realm.ExtendedDesc %>
                </center>              
        <br /><br />
           

    </asp:Panel>    
    
     <asp:Panel ID="r0_curstand" CssClass="infoPanel" runat="server" >
            <center>
                Current Top Clans
            <% try { %>
                <div style="margin:10px;"><b>Current Top Players</b></div>
                <%=(string)Fbg.Bll.Stats.GetPlayerRanking(_player.Realm, 0, Fbg.Bll.Stats.CONSTS.PlayerRanking_SortExp.Points).Rows[0][Fbg.Bll.Stats.CONSTS.PlayerRanking.PlayerName]%>
                #1 in village points
                <br>
                <%=(string)Fbg.Bll.Stats.GetPlayerRanking(_player.Realm, 0, Fbg.Bll.Stats.CONSTS.PlayerRanking_SortExp.AttackPoints).Rows[0][Fbg.Bll.Stats.CONSTS.PlayerRanking.PlayerName]%>
                #1 in attack points
                <br>
                <%=(string)Fbg.Bll.Stats.GetPlayerRanking(_player.Realm, 0, Fbg.Bll.Stats.CONSTS.PlayerRanking_SortExp.Defencepoints).Rows[0][Fbg.Bll.Stats.CONSTS.PlayerRanking.PlayerName]%>
                #1 in defense points
                <br>
                <%=(string)Fbg.Bll.Stats.GetPlayerRanking(_player.Realm, 0, Fbg.Bll.Stats.CONSTS.PlayerRanking_SortExp.GovKilled).Rows[0][Fbg.Bll.Stats.CONSTS.PlayerRanking.PlayerName]%>
                #1 in # of govs killed
                <br>
                <%=(string)Fbg.Bll.Stats.GetClanRanking(_player.Realm, 0).Rows[0][Fbg.Bll.Stats.CONSTS.ClanRanking.ClanName]%>
                #1 clan (in village pts)
                <br>
                <%} catch{} %>
                </center>              
        <br /><br />
           

    </asp:Panel>    

  

    <asp:Panel  ID="r0_pie_0" runat="server"  CssClass="infoPanel" style="height:300px;">
        <!--[if IE]>
        <script src="http://explorercanvas.googlecode.com/svn/trunk/excanvas.js"></script>
        <![endif]-->
        <link href="static/pie.css" rel="stylesheet" type="text/css" />
        <script src="script/pie.js" type="text/javascript"></script>
        <center>            
        <div id="container">
           
            <table id="chartData">
                
                <%              
                    string[] colors = new string[] { "#ED5713", "#0DA068", "#5F91DC", "#ED9C13", "#057249", "#057249", };      
                    System.Data.DataTable dt = Fbg.Bll.Stats.GetClanRanking(_player.Realm, 0, Fbg.Bll.Stats.CONSTS.ClanRanking_SortExp.NumVillages);
                    System.Data.DataTable dtglobal = Fbg.Bll.Stats.GetGlobalStats(_player.Realm).Tables[Fbg.Bll.Stats.CONSTS.GlobalStatsTable.Players];
                    int villCount =0;   
                    for (int i = 0; i <= (dt.Rows.Count > 3 ? 3 : dt.Rows.Count-1); i++) {
                        villCount += (int)dt.Rows[i][Fbg.Bll.Stats.CONSTS.ClanRanking.VillageCount];
                     %>
                <tr style='color: <%=colors[i]%>'>
                    <td>
                        <%=dt.Rows[i][Fbg.Bll.Stats.CONSTS.ClanRanking.ClanName]%>
                    </td>
                    <td>
                        <%=dt.Rows[i][Fbg.Bll.Stats.CONSTS.ClanRanking.VillageCount]%>
                    </td>
                </tr>
                     <%}
                         int allvillages = (int)dtglobal.Rows[0][Fbg.Bll.Stats.CONSTS.GlobalStatsNumberOfColumns.NoOfVillages];
                         if (allvillages > villCount)
                         {
                             
                                 %>
                                    <tr style="color: #111111">
                                    <td>
                                        Others
                                    </td>
                                    <td>
                                        <%=(allvillages - villCount).ToString()%>
                                    </td>
                                </tr>
                                   <%                                                                  
                             
                         }
                       %>               
            </table>
             <canvas id="chart" width="250" height="250"></canvas>
        </div>

       
       
    </asp:Panel>


    

</asp:Content>
