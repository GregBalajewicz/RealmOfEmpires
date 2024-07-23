<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/templates/masterPopupTemplate_m.master" CodeFile="MoraleDetails.aspx.cs" Inherits="templates_MoraleDetails" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
     
    <div id="moraleDetailsContent">

        <div class="header">
            <div class="moraleIcon"></div>
            <p>Your troops attack with certain level of "Morale". Being in high morale gives various bonuses to your armies, and being in very low morale gives certain penalties. Morale decreases with every attack, and increases slowly over time
            </p>
        </div>

        <div class="detailsList">
            <ul>
                <li>Attacks against Players cost %decrease_normal% morale.</li>
                <li>Attacks against Rebels / Abandonds (NPCs) cost %decrease_npc% morale.</li>
                <li>Minimum morale is %minMorale% and Maximum morale is %maxMorale%</li>
                <li>Morale regenerates at rate of %increasePerHour% per hour.</li>
                <li>Your current morale is: %currentMorale%</li>
                <li>Morale affects: Attack Strength, Carry Capacity, and Move Speed</li>
                <li>Morale affects troops speed only against NPC's (Rebels and Abandoned)</li>
                <li>Attacks including a Governor will always move at normal speed</li>
                <li>Spy-only attacks don't cost morale, and don't get affected by morale.</li>
                <li>Canceling an attack within 3 minutes of launching, will refund its morale.</li>
            </ul>
        </div>

        <table class="moraleEffectsTableHeader">
            <thead> <tr> <th>Morale</th> <th>Attack multiplier</th> <th>Speed multiplier</th> <th>Carry multiplier</th> <!--<th>Desertion</th>--> </tr> </thead>
        </table>
        <div class="moraleEffectsTableBodyScroll">
            <table class="moraleEffectsTableBody">
                <tbody></tbody>
            </table>
        </div>

        <div>Examples: Multiplier of 1.2 means 20% bonus, multiplier of 0.9, means 10% penalty, multiplier of 1 means no bonus or penalty.</div>
        
        <div> Do you have suggestions how to improve the morale system? <a href=" https://roebeta.uservoice.com/forums/273298-realm-of-empires-suggestions/category/188320-morale-system" target="_blank">let us know here</a></div>
        


    </div>
   
</asp:Content>