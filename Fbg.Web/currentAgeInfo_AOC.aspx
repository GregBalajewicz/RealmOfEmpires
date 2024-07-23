<%@ Page Language="C#" MasterPageFile="~/main.master" AutoEventWireup="true" CodeFile="currentAgeInfo_AOC.aspx.cs" Inherits="currentAgeInfo_AOC" %>

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

        a, a:hover {
            text-decoration: none !important;
        }


        .detailedExplanation {
            text-align: left;
            color: #DDDDDD;
        }

            .detailedExplanation .sectionHeader {
                font-weight: bold;
                font-size: larger;
            }

        .ageHeader {
            background-color: rgba(0,0,0,.4);
            padding: 10px;
            font-size: 21px;
            margin-bottom: 10px;
        }

        .detailedAOCInfo {
            padding: 10px;
        }

        p {
            font-size: 15px;
        }

    </style>

    
    <% if (isD2) { %>
        <script>
            $(function () {

                $('.override a.changeme').attr('target', '_blank');
            });
        </script>
    <% } if (isMobile) { %>
        <script>
            $(function () {

                $('.override a.changeme').attr('target', '_parent');
            });
        </script>
    
    <% } %>



    <div class="override">

       <div class="ageHeader">

           Age of Cities Information
        </div>

        <!-- Content -->
        <div runat="server" id="aocInTheFuture" class="currentAge" visible="false">
            <div class="contentPanel ageCounter fontGoldFrLCXlrg">
                Next Age of Cities starts in 
                <asp:Label ID="lblTimeTillNextAge" runat="server" Text=""></asp:Label>




            </div>          
        </div>
        <div runat="server" id="aocNow" class="currentAge" visible="false">
            <div class="contentPanel ageCounter fontGoldFrLCXlrg">
                Age of Cities is upon us!
                <p>You have until  <b><%=c.TimeOfConsolidation.ToUniversalTime().ToString("dddd MMM d HH:mm") %></b> to promote your villages or they will be promoted 
                at random at the time of consolidation!</p>
                <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
            </div>          
        </div>
        <div class="detailedAOCInfo" visible="false">
            <div class="contentPanel ageCounter fontGoldFrLCXlrg">

                <div class="detailedExplanation">

                    <center><div class="sectionHeader">Most important info summary</div></center>
                    <ul> 
                    <li>Attack freeze starts on <b><%=c.AttackFreezeStartOn.ToUniversalTime().ToString("dddd MMM d HH:mm") %></b> (game time)</li>
                    <li>Consolidation occurs on <b><%=c.TimeOfConsolidation.ToUniversalTime().ToString("dddd MMM d HH:mm") %></b></li>
                    <li>Absorbtion ratio is <b><%=c.NumVillagesAbsorbed+1%>:1</b></li>
                    <li>Attack freeze ends on <b><%=c.AttackFreezeeEndsOn.ToUniversalTime().ToString("dddd MMM d HH:mm")%></b></li>
                                                 </ul>

                    <center><div class="sectionHeader">Details</div></center>
                    <div class="sectionHeader">Absorption ratio</div>
                    <p>
                        The absorption ratio for this realm is <b><%=c.NumVillagesAbsorbed+1%>:1</b> . This means that for every promoted City, <%=c.NumVillagesAbsorbed%> village will be absorbed.
                When promoting villages the following rules apply. 

                    </p>
                    <div class="sectionHeader">Troops from absorbed Villages</div>
                    <p>
                        After consolidation, promoted villages will become cities and the realm will be changed forever. Troops from absorbed villages will be converted into "Rewards" that you can use as any other reward on this realm. 

                    </p>
                    
                    <div class="sectionHeader">Attack Freeze & Promotion</div>
                    <p>
                        An attack freeze will go into place <b><%=c.AttackFreezeStartOn.ToUniversalTime().ToString("dddd MMM d HH:mm") %></b> game time, 
                    lasting until <b><%=c.AttackFreezeeEndsOn.ToUniversalTime().ToString("dddd MMM d HH:mm") %></b> game time. No attacks can land on player villages during the Attack Freeze. Rebels and Abandoned villages can still be attacked, however. (Ongoing attacks to Rebels and Abandoned will continue to land during attack freeze, even if taken over by another player)

                    </p>
                    <p>
                        Once the attack freeze is in place, the 'promotional tool' will be available for you. You will see a new thumb-up icon on the map HUD. This will allow you to promote your villages.

                    </p>
                    <p>
                        When promoting a village, the closest non-promoted, non-bonus villages will be marked for absorption. 
                        <BR /><span style="font-size:smaller"> Legendary villages are not considered bonus villages and will be treated like any other normal village</span>
                
                    </p>
                    <p>
                        If you do not promote you villages, the correct number will be randomly selected at the time of consolidation.


                        <div class="sectionHeader">Consolidation</div>
                    </p>
                    <p>
                        The consolidation itself will occur at <b><%=c.TimeOfConsolidation.ToUniversalTime().ToString("dddd MMM d HH:mm") %></b>. At this time, promoted villages will become cities and the realm will be changed forever. 
                    </p>
                    
                <ul>
                    <li>Cities will be pulled inwards towards the center of the map, in order to fill the empty spaces left by absorbed villages.
                    </li>
                    <li>Realm density and village spacing will be about the same as before consolidation.
                    </li>
                    <li>Rebels will be consolidated into Cities as well, with random distribution.
                    </li>
                    <li>If a Bonus village is promoted into a City, that bonus will be preserved. Multiple bonus villages will not stack, however.
                    </li>
                    <li>City points will be multiplied by the absorption ratio, so that overall point totals will remain roughly the same.
                    </li>
                    <li>
                    Governor costs are reduced in accordance with your smaller number of settlements.
                        </li>
                    <li>During the time of consolidation, the realm will be taken off-line and will not appear in the list of available realms to login to!</li>
                </ul>

                    <div class="sectionHeader">FAQ</div>
                    <p>
                        <b>What is an attack freeze ?</b>
                        <br />
                        An attack freeze is a lot like sleep mode, only stronger. While sleep mode only prevents attacks from being launched during sleep mode, an attack freeze also prevents attacks that would land while it is in effect.

Note that the freeze only effects player controlled villages. If there are rebels or abandoned villages you can still go after them, and if someone else takes your target first there will still be a crossfire as those attacks were already in motion against a (at the time of launch) legal target.


                    </p>
                    <p>
                        <b>If the cost of Governors will be reduced do we get to keep the chests in our collection?</b>
                        <br />
                        Yes
 
                    </p>
                    <p>
                        <b>What will happen to supporting troops? Will they walk home upon absorption?</b> or Will they all be consolidated into the city into one big stack?
                        <br />
                        Before villages are absorbed, troops out supporting others will be automatically sent home. So you do not need to worry about losing troops in promoted Cities because they were off defending a village marked for absorption.

Troops from villages marked for absorption will also be sent home. All troops in transit will return to their villages.


                    </p>
                    <p>
                        <b>What happens to fractions? If I have 67 villages, how many Cities do I end up with?</b>
                        <br />
                        Fractions are rounded up. So if the ratio was 3:1, you would end up with 23 Cities.


                    </p>
                    <p>
                        <b>Can we choose which villages gets marked for absorption?</b> In areas where there are four or five villages, I would like to choose which get absorbed.
                        <br />
                    Not directly. You only choose which village will be promoted into a City. Because the closest non-bonus villages are taken, you can usually pick things out fairly precisely, though. One reason you cannot select the villages for absorption directly is that would allow players to abandon entire clusters, and concentrate all of their remaining Cities in one place. 




                </div>






            </div>          
        </div>

    </div>

    

</asp:Content>
