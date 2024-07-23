<%@ Page Language="C#" MasterPageFile="~/main.master" AutoEventWireup="true" CodeFile="currentAgeInfo.aspx.cs" Inherits="currentAgeInfo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
       

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <% if (isD2) { %>
    <style>
        
    </style>
    <% } %>

    <% if (isMobile) { %>
        <style>
        
        </style>
    <% } %>


   


    <!-- common style -->
    <style>

        .TDContent {
            position: absolute;
            background: url(https://static.realmofempires.com/images/backgrounds/M_BG_Tower.jpg);
            background-size:cover;
            background-position:50% 50%;
            height: 100%;
        }

        .override {
            position: absolute;
            width: 100%;
            height: 100%;
            padding: 0px;
            box-sizing: border-box;
            background-color: rgba(6, 20, 31, 0.9);
            overflow: auto;
            font: 13px "IM Fell French Canon";
            text-align:center;
        }



        .ageHeader {
            position: absolute;
            left: 0px;
            right: 0px;
            top: 0px;
            height: 72px;
            background-color: rgba(0,0,0,.4);
        }

        a, a:hover {
            text-decoration: none !important;
        }

        .ageNav {
            position: absolute;
            top: 13px;
        }

            .ageNav.agePrev {
                left:10px;
            }

            .ageNav.ageNext {
                right:10px;
            }

            .ageNav .ageNavIcon {
                display: inline-block;
                position: relative;
                width: 44px;
                height: 44px;
                background-size: 100% 100%;
            }
            .ageNav.agePrev .ageNavIcon {

            }
            .ageNav.ageNext .ageNavIcon {

            }

            .ageNav .ageNavArrow {
                display: inline-block;
                position: relative;
                width: 22px;
                height: 44px;
                background-size: 20px 26px;
                background-position: 50% 50%;
                background-repeat: no-repeat;
            }
                .ageNav .ageNavArrow.ageNavArrowPrev {
                    background-image: url(https://static.realmofempires.com/images/misc/M_ArrowL.png);
                }
                .ageNav .ageNavArrow.ageNavArrowNext {
                    background-image: url(https://static.realmofempires.com/images/misc/M_ArrowR.png);
                }
        .currentAgeIcon {
            position: absolute;
            left: 50%;
            width: 72px;
            height: 72px;
            margin-left: -36px;
        }
        .currentAge {
            position: absolute;
            top: 72px;
            left: 0px;
            right: 0px;
            bottom: 0px;
            overflow: auto;
            padding: 10px;
        }

        .ageCounter {
            text-align: center;
        }

        .contentPanel {
            margin-bottom:20px;
        }

        /*Pie Chart style - override pie css*/
        #chartContainer {
            text-align: center;
            padding-top: 10px;
        }
            #chartContainer #chart {
                display: inline-block;
                float: none;
                cursor: pointer;
            }
            #chartContainer #chartData {
                display: inline-block;
                vertical-align: top;
                border-collapse: collapse;
                background-color: rgba(0, 0, 0, 0.4);
                border-spacing: 0px;
            }

                #chartContainer #chartData tr {
                    text-shadow: 0px 1px black, 0px -1px black;
                }
                #chartContainer #chartData th, #chartData td {
                    border: 0px solid #000 !important;
                }

        .newrealm {
            border: solid black 1px;
        }

            .newrealm.scheduled .opendMsg {
                display: none;
            }
            .newrealm.openedNow .openingMsg {
                display: none;
            }


        .oneRankRewars {
            padding: 5px;
            margin: 5px;
            background: rgba(0, 0, 0, 0.4);
        }
            .oneRankRewars .rankRange {
                margin:5px;
            }

        #Prizes .listOfRewards .item {
            position: relative;
            display: inline-block;
            width: 70px;
            height: 70px;
            margin: 1px;
            box-sizing: border-box;
            overflow: hidden;
            background-color: rgba(0, 0, 0, 0.8);
            background-size: 32px auto;
            background-position: 50% 10px;
            background-repeat: no-repeat;
            border-radius: 3px;
            border-top: 1px solid #676131;
            border-bottom: 1px solid #27261A;
            text-align: center;
        }
            #Prizes .listOfRewards .item .c {
                position: absolute;
                top: 0px;
                right: 0px;
                background-color: #000;
                padding: 0px 4px;
                border-radius: 10px;
                min-width: 12px;
                text-align: center;
                font-size: 11px;
            }
            #Prizes .listOfRewards .item .text {
                position: absolute;
                left: 0px;
                bottom: 0px;
                right: 0px;
                padding: 2px;
                font-size: 11px;
            }

    .ageInformation, .wcMessage, .rankRange {
        font-size: 20px;
}
    </style>





    <div class="override">

        <div class="ageHeader">

            <!-- Previous Age Button -->
            <asp:Panel ID="prevAge" CssClass="ageNav agePrev" runat="server">   
                <a href="currentAgeInfo.aspx?age=<% =previousRealmAge == null ? "" : previousRealmAge.AgeNumber.ToString() %>">
                    <div class="ageNavArrow ageNavArrowPrev"></div>
                    <div class="ageNavIcon" style="background-image: url('https://static.realmofempires.com/images/icons/Age<%=previousRealmAge == null ? "" : previousRealmAge.AgeNumber.ToString() %>L.png')"></div>
                </a>
            </asp:Panel>

            <!-- Next Age Button -->
            <asp:Panel ID="nextAge" CssClass="ageNav ageNext" runat="server"> 
                <a href="currentAgeInfo.aspx?age=<% =nextRealmAge == null ? "" : nextRealmAge.AgeNumber.ToString() %>">      
                    <div class="ageNavIcon"  style="background-image: url('https://static.realmofempires.com/images/icons/Age<%=nextRealmAge == null ? "" : nextRealmAge.AgeNumber.ToString() %>L.png')"></div>
                    <div class="ageNavArrow ageNavArrowNext"></div>
                </a>
            </asp:Panel>

            <!-- Current Age Icon -->
            <div class="currentAgeIcon" style="background-image: url('https://static.realmofempires.com/images/icons/Age<% =thisRealmAge.AgeNumber.ToString() %>L.png')"></div>
        
        </div>

        <!-- Content -->
        <div runat="server" id="currentAge" class="currentAge">

            <div class="ageInformation"><%=thisRealmAge.Info %></div>

            <!-- Counter till new age panel  -->
            <div class="contentPanel ageCounter fontGoldFrLCXlrg">
                <asp:Label ID="lblTimeTillNextAge" runat="server" Text=""></asp:Label>
            </div>

            <!-- Win Condition Panel -->
            <asp:Panel ID="pnlWC" CssClass="contentPanel winCondition" runat="server">

                <div class="wcTitlte fontGoldFrLC21">END OF GAME WIN CONDITION</div>
                <%if (FbgPlayer.Realm.RealmSubType == Fbg.Bll.Realm.RealmSubTypes.NoClans) { %>
                    <div class="wcMessage">10 players remaining</div>
                <%} else { %>
                    <div class="wcMessage">2 allied clans remaining.</div>
                <%} %>

                    Once the win condition is satisfied, <a href="contactsupport.aspx"> contact us</a> and we'll start the process of ending the realm. 
                <br />
                    <!-- pie chart code -->
                    <asp:Panel ClientIDMode="Static" ID="r0_pie_0" runat="server" CssClass="infoPanel">

                        <link href="static/pie.css" rel="stylesheet" type="text/css" />
                        <script src="script/pie.js" type="text/javascript"></script>

                        <div class="wcTitlte fontGoldFrLC21">Current top clan standings</div>

                        <div id="chartContainer">
                            <table id="chartData">

                                <%              
                                //string[] colors = new string[] { "#ED5713", "#0DA068", "#5F91DC", "#ED9C13", "#057249", "#057249", };
                                string[] colors = new string[] { "#c1a955", "#615aaa", "#0c6e31", "#207383", "#706BA3", "#9143c7" };

                                int villCount = 0;
                                for (int i = 0; i <= (dt.Rows.Count > 3 ? 3 : dt.Rows.Count - 1); i++)
                                {
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
                                <tr style="color: #5a3a3a">
                                    <td>Others
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
            </asp:Panel>

            <!-- Prizes Panel -->
            <asp:Panel ClientIDMode="Static"  ID="Prizes" runat="server"  CssClass="contentPanel winCondition">
                    
                <div class="fontGoldFrLC21">END OF GAME PRIZES</div>
                <div>
                    Rank accoring to village points on the realm<br />
                    These are realm-neutral rewards - they can be used on any realm. 
                </div>

                <%if (FbgPlayer.Realm.RealmSubType == Fbg.Bll.Realm.RealmSubTypes.NoClans || FbgPlayer.Realm.RealmSubType == Fbg.Bll.Realm.RealmSubTypes.Retro || FbgPlayer.Realm.RealmType == "CLASSIC") { %>
                <div class="oneRankRewars">     
                    <div class="listOfRewards">
                        <div class="rankRange">Rank 1</div>
                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_ElvenEfficiency.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Elven Efficiency</div>
                            <div class="c">x5</div>
                        </div>
                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Defense.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Bravery Spell</div>
                            <div class="c">x5</div>
                        </div>
                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Attack.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Blood Lust Spell</div>
                            <div class="c">x5</div>
                        </div>
                        <div class="item pfd" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Return.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h God Speed Spell</div>
                            <div class="c">x5</div>
                        </div>
                        <div class="item pfd" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_RebelRush.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">1h Rebel Rush Spell</div>
                            <div class="c">x5</div>
                        </div>
                        <div class="item buildingspeedup"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/Q_Upgrade2.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">2h building speedup</div>
                            <div class="c">x5</div>
                        </div>
                        <div class="item researchspeedup" data-groupid="researchspeedup30m research speedup" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_ResearchList.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">12h research speedup</div>
                            <div class="c">x1</div>
                        </div>
                    </div>
                </div>
                <div class="oneRankRewars">     
                    <div class="listOfRewards">
                        <div class="rankRange">Rank 2</div>
                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_ElvenEfficiency.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Elven Efficiency</div>
                            <div class="c">x4</div>
                        </div>
                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Defense.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Bravery Spell</div>
                            <div class="c">x4</div>
                        </div>
                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Attack.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Blood Lust Spell</div>
                            <div class="c">x4</div>
                        </div>
                        <div class="item pfd" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Return.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h God Speed Spell</div>
                            <div class="c">x4</div>
                        </div>
                        <div class="item pfd" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_RebelRush.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">1h Rebel Rush Spell</div>
                            <div class="c">x4</div>
                        </div>
                        <div class="item buildingspeedup"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/Q_Upgrade2.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">2h building speedup</div>
                            <div class="c">x4</div>
                        </div>
                        <div class="item researchspeedup" data-groupid="researchspeedup30m research speedup" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_ResearchList.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">12h research speedup</div>
                            <div class="c">x1</div>
                        </div>
                    </div>
                </div>
                <div class="oneRankRewars">     
                    <div class="listOfRewards">
                        <div class="rankRange">Rank 3</div>
                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_ElvenEfficiency.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Elven Efficiency</div>
                            <div class="c">x3</div>
                        </div>
                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Defense.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Bravery Spell</div>
                            <div class="c">x3</div>
                        </div>
                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Attack.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Blood Lust Spell</div>
                            <div class="c">x3</div>
                        </div>
                        <div class="item pfd" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Return.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h God Speed Spell</div>
                            <div class="c">x3</div>
                        </div>
                        <div class="item pfd" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_RebelRush.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">1h Rebel Rush Spell</div>
                            <div class="c">x3</div>
                        </div>
                        <div class="item buildingspeedup"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/Q_Upgrade2.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">2h building speedup</div>
                            <div class="c">x3</div>
                        </div>
                        <div class="item researchspeedup" data-groupid="researchspeedup30m research speedup" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_ResearchList.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">12h research speedup</div>
                            <div class="c">x1</div>
                        </div>
                    </div>
                </div>

                <div class="oneRankRewars">
                    <div class="rankRange">Rank 4-6</div>
                    <div class="listOfRewards">

                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_ElvenEfficiency.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Elven Efficiency</div>
                            <div class="c">x2</div>
                        </div>
                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Defense.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Bravery Spell</div>
                            <div class="c">x2</div>
                        </div>
                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Attack.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Blood Lust Spell</div>
                            <div class="c">x2</div>
                        </div>
                        <div class="item pfd" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Return.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h God Speed Spell</div>
                            <div class="c">x2</div>
                        </div>
                        <div class="item pfd" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_RebelRush.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">1h Rebel Rush Spell</div>
                            <div class="c">x2</div>
                        </div>
                        <div class="item buildingspeedup"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/Q_Upgrade2.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">2h building speedup</div>
                            <div class="c">x2</div>
                        </div>
                        <div class="item researchspeedup" data-groupid="researchspeedup30m research speedup" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_ResearchList.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">6h research speedup</div>
                            <div class="c">x1</div>
                        </div>
                    </div>
                </div>
                <div class="oneRankRewars">
                    <div class="rankRange">Rank 7-10</div>
                    <div class="listOfRewards">

                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_ElvenEfficiency.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Elven Efficiency</div>
                            <div class="c">x1</div>
                        </div>
                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Defense.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Bravery Spell</div>
                            <div class="c">x1</div>
                        </div>
                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Attack.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Blood Lust Spell</div>
                            <div class="c">x1</div>
                        </div>
                        <div class="item pfd" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Return.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h God Speed Spell</div>
                            <div class="c">x1</div>
                        </div>
                        <div class="item pfd" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_RebelRush.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">1h Rebel Rush Spell</div>
                            <div class="c">x1</div>
                        </div>
                        <div class="item buildingspeedup"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/Q_Upgrade2.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">2h building speedup</div>
                            <div class="c">x1</div>
                        </div>
                        <div class="item researchspeedup" data-groupid="researchspeedup30m research speedup" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_ResearchList.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">6h research speedup</div>
                            <div class="c">x1</div>
                        </div>
                    </div>
                </div>

                <%} else if (FbgPlayer.Realm.ID < 124) {%>
                <div class="oneRankRewars">
     
                    <div class="listOfRewards">
                        <div class="rankRange">Rank 1-5</div>
                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_ElvenEfficiency.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Elven Efficiency</div>
                            <div class="c">x10</div>
                        </div>
                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Defense.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Bravery Spell</div>
                            <div class="c">x10</div>
                        </div>
                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Attack.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Blood Lust Spell</div>
                            <div class="c">x10</div>
                        </div>
                        <div class="item pfd" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Return.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h God Speed Spell</div>
                            <div class="c">x10</div>
                        </div>
                        <div class="item pfd" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_RebelRush.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">1h Rebel Rush Spell</div>
                            <div class="c">x10</div>
                        </div>
                        <div class="item buildingspeedup"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/Q_Upgrade2.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">2h building speedup</div>
                            <div class="c">x10</div>
                        </div>
                        <div class="item researchspeedup" data-groupid="researchspeedup30m research speedup" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_ResearchList.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">12h research speedup</div>
                            <div class="c">x2</div>
                        </div>
                    </div>
                </div>

                <div class="oneRankRewars">
                    <div class="rankRange">Rank 6-10</div>
                    <div class="listOfRewards">

                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_ElvenEfficiency.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Elven Efficiency</div>
                            <div class="c">x8</div>
                        </div>
                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Defense.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Bravery Spell</div>
                            <div class="c">x8</div>
                        </div>
                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Attack.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Blood Lust Spell</div>
                            <div class="c">x8</div>
                        </div>
                        <div class="item pfd" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Return.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h God Speed Spell</div>
                            <div class="c">x8</div>
                        </div>
                        <div class="item pfd" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_RebelRush.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">1h Rebel Rush Spell</div>
                            <div class="c">x8</div>
                        </div>
                        <div class="item buildingspeedup"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/Q_Upgrade2.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">2h building speedup</div>
                            <div class="c">x7</div>
                        </div>
                        <div class="item researchspeedup" data-groupid="researchspeedup30m research speedup" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_ResearchList.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">12h research speedup</div>
                            <div class="c">x1</div>
                        </div>
                        <div class="item researchspeedup" data-groupid="researchspeedup30m research speedup" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_ResearchList.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">6h research speedup</div>
                            <div class="c">x1</div>
                        </div>
                    </div>
                </div>
                <div class="oneRankRewars">
                    <div class="rankRange">Rank 11-20</div>
                    <div class="listOfRewards">

                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_ElvenEfficiency.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Elven Efficiency</div>
                            <div class="c">x4</div>
                        </div>
                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Defense.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Bravery Spell</div>
                            <div class="c">x4</div>
                        </div>
                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Attack.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Blood Lust Spell</div>
                            <div class="c">x4</div>
                        </div>
                        <div class="item pfd" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Return.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h God Speed Spell</div>
                            <div class="c">x4</div>
                        </div>
                        <div class="item pfd" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_RebelRush.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">1h Rebel Rush Spell</div>
                            <div class="c">x4</div>
                        </div>
                        <div class="item buildingspeedup"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/Q_Upgrade2.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">2h building speedup</div>
                            <div class="c">x4</div>
                        </div>
                        <div class="item researchspeedup" data-groupid="researchspeedup30m research speedup" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_ResearchList.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">12h research speedup</div>
                            <div class="c">x1</div>
                        </div>
                    </div>
                </div>
                <div class="oneRankRewars">
                    <div class="rankRange">Rank 21+</div>
                    <div class="listOfRewards">

                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_ElvenEfficiency.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Elven Efficiency</div>
                            <div class="c">x1</div>
                        </div>
                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Defense.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Bravery Spell</div>
                            <div class="c">x1</div>
                        </div>
                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Attack.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Blood Lust Spell</div>
                            <div class="c">x1</div>
                        </div>
                        <div class="item pfd" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Return.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h God Speed Spell</div>
                            <div class="c">x1</div>
                        </div>
                        <div class="item pfd" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_RebelRush.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">1h Rebel Rush Spell</div>
                            <div class="c">x1</div>
                        </div>
                        <div class="item buildingspeedup"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/Q_Upgrade2.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">2h building speedup</div>
                            <div class="c">x2</div>
                        </div>
                        <div class="item researchspeedup" data-groupid="researchspeedup30m research speedup" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_ResearchList.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">6h research speedup</div>
                            <div class="c">x1</div>
                        </div>
                    </div>
                </div>


                <%} else if (FbgPlayer.Realm.ID < 138) {%>
            <div class="oneRankRewars">
     
                    <div class="listOfRewards">
                        <div class="rankRange">Rank 1-5</div>
                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_ElvenEfficiency.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Elven Efficiency</div>
                            <div class="c">x5</div>
                        </div>
                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Defense.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Bravery Spell</div>
                            <div class="c">x5</div>
                        </div>
                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Attack.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Blood Lust Spell</div>
                            <div class="c">x5</div>
                        </div>
                        <div class="item pfd" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Return.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h God Speed Spell</div>
                            <div class="c">x5</div>
                        </div>
                        <div class="item pfd" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_RebelRush.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">1h Rebel Rush Spell</div>
                            <div class="c">x5</div>
                        </div>
                        <div class="item buildingspeedup"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/Q_Upgrade2.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">2h building speedup</div>
                            <div class="c">x5</div>
                        </div>
                        <div class="item researchspeedup" data-groupid="researchspeedup30m research speedup" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_ResearchList.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">12h research speedup</div>
                            <div class="c">x2</div>
                        </div>
                    </div>
                </div>

                <div class="oneRankRewars">
                    <div class="rankRange">Rank 6-10</div>
                    <div class="listOfRewards">

                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_ElvenEfficiency.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Elven Efficiency</div>
                            <div class="c">x4</div>
                        </div>
                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Defense.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Bravery Spell</div>
                            <div class="c">x4</div>
                        </div>
                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Attack.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Blood Lust Spell</div>
                            <div class="c">x4</div>
                        </div>
                        <div class="item pfd" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Return.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h God Speed Spell</div>
                            <div class="c">x4</div>
                        </div>
                        <div class="item pfd" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_RebelRush.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">1h Rebel Rush Spell</div>
                            <div class="c">x4</div>
                        </div>
                        <div class="item buildingspeedup"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/Q_Upgrade2.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">2h building speedup</div>
                            <div class="c">x4</div>
                        </div>
                        <div class="item researchspeedup" data-groupid="researchspeedup30m research speedup" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_ResearchList.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">12h research speedup</div>
                            <div class="c">x1</div>
                        </div>
                        <div class="item researchspeedup" data-groupid="researchspeedup30m research speedup" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_ResearchList.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">6h research speedup</div>
                            <div class="c">x1</div>
                        </div>
                    </div>
                </div>
                <div class="oneRankRewars">
                    <div class="rankRange">Rank 11-20</div>
                    <div class="listOfRewards">

                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_ElvenEfficiency.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Elven Efficiency</div>
                            <div class="c">x3</div>
                        </div>
                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Defense.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Bravery Spell</div>
                            <div class="c">x3</div>
                        </div>
                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Attack.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Blood Lust Spell</div>
                            <div class="c">x3</div>
                        </div>
                        <div class="item pfd" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Return.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h God Speed Spell</div>
                            <div class="c">x3</div>
                        </div>
                        <div class="item pfd" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_RebelRush.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">1h Rebel Rush Spell</div>
                            <div class="c">x3</div>
                        </div>
                        <div class="item buildingspeedup"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/Q_Upgrade2.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">2h building speedup</div>
                            <div class="c">x3</div>
                        </div>
                        <div class="item researchspeedup" data-groupid="researchspeedup30m research speedup" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_ResearchList.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">12h research speedup</div>
                            <div class="c">x1</div>
                        </div>
                    </div>
                </div>
                <div class="oneRankRewars">
                    <div class="rankRange">Rank 21+</div>
                    <div class="listOfRewards">

                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_ElvenEfficiency.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Elven Efficiency</div>
                            <div class="c">x1</div>
                        </div>
                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Defense.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Bravery Spell</div>
                            <div class="c">x1</div>
                        </div>
                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Attack.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Blood Lust Spell</div>
                            <div class="c">x1</div>
                        </div>
                        <div class="item pfd" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Return.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h God Speed Spell</div>
                            <div class="c">x1</div>
                        </div>
                        <div class="item pfd" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_RebelRush.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">1h Rebel Rush Spell</div>
                            <div class="c">x1</div>
                        </div>
                        <div class="item buildingspeedup"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/Q_Upgrade2.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">2h building speedup</div>
                            <div class="c">x2</div>
                        </div>
                        <div class="item researchspeedup" data-groupid="researchspeedup30m research speedup" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_ResearchList.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">6h research speedup</div>
                            <div class="c">x1</div>
                        </div>
                    </div>
                </div>
                <%} else { %>
                <div class="oneRankRewars">
     
                    <div class="listOfRewards">
                        <div class="rankRange">Rank 1</div>
                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_ElvenEfficiency.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Elven Efficiency</div>
                            <div class="c">x5</div>
                        </div>
                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Defense.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Bravery Spell</div>
                            <div class="c">x5</div>
                        </div>
                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Attack.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Blood Lust Spell</div>
                            <div class="c">x5</div>
                        </div>
                        <div class="item pfd" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Return.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h God Speed Spell</div>
                            <div class="c">x5</div>
                        </div>
                        <div class="item pfd" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_RebelRush.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">1h Rebel Rush Spell</div>
                            <div class="c">x5</div>
                        </div>
                        <div class="item buildingspeedup"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/Q_Upgrade2.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">2h building speedup</div>
                            <div class="c">x5</div>
                        </div>
                        <div class="item researchspeedup" data-groupid="researchspeedup30m research speedup" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_ResearchList.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">12h research speedup</div>
                            <div class="c">x2</div>
                        </div>
                    </div>
                </div>

                <div class="oneRankRewars">
                    <div class="rankRange">Rank 2</div>
                    <div class="listOfRewards">

                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_ElvenEfficiency.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Elven Efficiency</div>
                            <div class="c">x4</div>
                        </div>
                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Defense.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Bravery Spell</div>
                            <div class="c">x4</div>
                        </div>
                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Attack.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Blood Lust Spell</div>
                            <div class="c">x4</div>
                        </div>
                        <div class="item pfd" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Return.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h God Speed Spell</div>
                            <div class="c">x4</div>
                        </div>
                        <div class="item pfd" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_RebelRush.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">1h Rebel Rush Spell</div>
                            <div class="c">x4</div>
                        </div>
                        <div class="item buildingspeedup"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/Q_Upgrade2.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">2h building speedup</div>
                            <div class="c">x4</div>
                        </div>
                        <div class="item researchspeedup" data-groupid="researchspeedup30m research speedup" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_ResearchList.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">12h research speedup</div>
                            <div class="c">x1</div>
                        </div>
                        <div class="item researchspeedup" data-groupid="researchspeedup30m research speedup" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_ResearchList.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">6h research speedup</div>
                            <div class="c">x1</div>
                        </div>
                    </div>
                </div>
                <div class="oneRankRewars">
                    <div class="rankRange">Rank 3</div>
                    <div class="listOfRewards">

                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_ElvenEfficiency.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Elven Efficiency</div>
                            <div class="c">x3</div>
                        </div>
                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Defense.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Bravery Spell</div>
                            <div class="c">x3</div>
                        </div>
                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Attack.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Blood Lust Spell</div>
                            <div class="c">x3</div>
                        </div>
                        <div class="item pfd" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Return.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h God Speed Spell</div>
                            <div class="c">x3</div>
                        </div>
                        <div class="item pfd" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_RebelRush.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">1h Rebel Rush Spell</div>
                            <div class="c">x3</div>
                        </div>
                        <div class="item buildingspeedup"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/Q_Upgrade2.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">2h building speedup</div>
                            <div class="c">x3</div>
                        </div>
                        <div class="item researchspeedup" data-groupid="researchspeedup30m research speedup" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_ResearchList.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">12h research speedup</div>
                            <div class="c">x1</div>
                        </div>
                    </div>
                </div>
                <div class="oneRankRewars">
                    <div class="rankRange">Rank 4+</div>
                    <div class="listOfRewards">

                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_ElvenEfficiency.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Elven Efficiency</div>
                            <div class="c">x1</div>
                        </div>
                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Defense.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Bravery Spell</div>
                            <div class="c">x1</div>
                        </div>
                        <div class="item pfd"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Attack.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h Blood Lust Spell</div>
                            <div class="c">x1</div>
                        </div>
                        <div class="item pfd" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_Return.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">24h God Speed Spell</div>
                            <div class="c">x1</div>
                        </div>
                        <div class="item pfd" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_PF_RebelRush.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">1h Rebel Rush Spell</div>
                            <div class="c">x1</div>
                        </div>
                        <div class="item buildingspeedup"  style="background-image: url(&quot;https://static.realmofempires.com/images/icons/Q_Upgrade2.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">2h building speedup</div>
                            <div class="c">x2</div>
                        </div>
                        <div class="item researchspeedup" data-groupid="researchspeedup30m research speedup" style="background-image: url(&quot;https://static.realmofempires.com/images/icons/M_ResearchList.png&quot;); display: inline-block;">
                            <div class="text fontGoldFrLCmed ">6h research speedup</div>
                            <div class="c">x1</div>
                        </div>
                    </div>
                </div>
                <%} %>
            </asp:Panel>

            <a href="RealmInfo.aspx">Full realm paramaters and ranking</a>


        </div>

    </div>

    

</asp:Content>
