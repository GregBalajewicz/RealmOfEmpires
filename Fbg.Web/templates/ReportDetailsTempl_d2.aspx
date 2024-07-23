<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/templates/masterPopupTemplate_m.master" CodeFile="ReportDetailsTempl_d2.aspx.cs" Inherits="templates_ReportDetailsTempl_d2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">
      
    <section class="theme-view miscReport">
        <div class="reportDetailScroll">
            <div class="reportDetailTop">
                <div class="titleBanner">%reportTitle%</div>
                <div class="reportTime fontSilverFrLCmed noShadow">%reporttime%</div>
                <div class="miscBlock">
                        <div class="sectionDivider"></div>
                        <p class="fontDarkGoldFrLClg noShadow miscDescription">%description%</p>                    
                        <div class="sectionDivider"></div>
                </div>
            </div>
        </div>
    </section>

    <section class="themeM-view supportAttackedReport template" style="overflow-y:auto; display:block">
        <div class="reportDetailScroll">
            <div class="reportDetailTop">
                <div class="titleBanner supportAttackedBanner">%reportTitle%</div>
                <div class="reportTime fontSilverFrLCmed noShadow">%reporttime%</div>
                <div class="sectionDivider lighter"></div>

                <!-- Support Attacked Layout -->
                 <div class="versus2">
                    <div class="rowLine supportedView">                        
                        <div class="both">
                            <div class="iconsRow">
                                
                                <div class="villageIconWrapper">
                                    <div class="noteIconOverlapContainer">
                                        <div class="villageIconShiner"><a class="icon village-name jsV roundedIcon" data-x="%supportedVillageX%" data-y="%supportedVillageY%" data-vid="%supportedVillageID%" data-opid="%supportedPlayerID%" onclick="ROE.Reports.showVillageProfile($(this));" href="#" style="background-image:url('%supportedVillageImage%'), url('https://static.realmofempires.com/images/map/bigtile_5.jpg');"></a></div>
                                        <div class="note doNotCatpureClick supportedVillageNote"></div>
                                    </div>  
                                    <div class="fontSilverNumbersSm noShadow villageLocation">(%supportedVillageX%, %supportedVillageY%)</div>
                                </div>
                                <div class="playerIconWrapper noteIconOverlapContainer">                                
                                    <div class="icon playerIcon supportedAvatar player-name support-name" data-pn="%supportedPlayerName%"  style="background-image: url('%supportedPlayerAvatar%');"></div>  
                                    <div class="note doNotCatpureClick supportedPlayerNote"></div>
                                </div>

                            </div>
                            <div class="namesRow">
                                <p>
                                <a class="noShadow village-name" data-vid="%supportedVillageID%" data-opid="%supportedPlayerID%" onclick="ROE.Reports.showVillageProfile($(this));" href="#">%supportedVillageName%</a><br />
                                <a class="noShadow player-name support-name nameOfPlayer" data-pn="%supportedPlayerName%" href="#">%supportedPlayerName%</a>

                                </p>

                            </div>

                        
                        </div>                                         
                    </div>
                </div>


                <div class="supportedText">
                    <p class="fontTanFrLCmed centered">
                    <%=RS("supportedText") %>
                        </p>
                </div>
                <div class="sectionDivider"></div>
            </div>

            <div class="reportTables"></div> 
        </div>
       
    </section>






    <section class="themeM-view attackReport template" style="overflow-y:auto; display:block;"> 
        <div class="reportDetailScroll">
            <div class="reportDetailTop">

                

                <div class="titleBanner">%reportTitle%</div>
                <div class="reportTime fontSilverFrLCmed noShadow">%reporttime%</div>
                <div class="sectionDivider lighter"></div>

                <!-- Attack Layout -->
                <div class="versus2">
                    <div class="rowLine">
                        <div class="leftSide">
                            
                                <div class="iconsRow">

                                    <div class="villageIconWrapper">
                                        <div class="noteIconOverlapContainer">
                                            <div class="villageIconShiner"><a class="icon attackerVillage village-name jsV roundedIcon" data-x="%attackerVillageXCord%" data-y="%attackerVillageYCord%" data-vid="%attackerVillageID%" data-opid="%attackerPlayerID%" href="#" style="background-image:url('%attackerVillageIcon%'), url('https://static.realmofempires.com/images/map/bigtile_5.jpg');"></a></div>
                                            <div class="note doNotCatpureClick attackerVillageNote"></div>
                                        </div>  
                                        <div class="fontSilverNumbersSm noShadow villageLocation">(%attackerVillageXCord%, %attackerVillageYCord%)</div>
                                    </div>
                                    <div class="playerIconWrapper noteIconOverlapContainer">                                
                                        <div class="icon playerIcon attackerAvatar player-name attacker-name" data-pn="%attackerPlayerName%"  style="background-image: url('%attackerPlayerAvatar%');"></div>  
                                        <div class="note doNotCatpureClick attackerPlayerNote"></div>
                                    </div>

                                </div>
                                <div class="namesRow">
                                    <p>
                                        <a class="noShadow attackerVillage village-name jsV" data-x="%attackerVillageXCord%" data-y="%attackerVillageYCord%"  data-vid="%atackerVillageID%" data-opid="%attackerPlayerID%"  href="#">%attackerVillageName%</a>
                                        <br class="attacker villageInlineNote" />
                                        <a class="noShadow attacker villageInlineNote" onclick="ROE.Reports.showNotePopup($(this));" data-note="%attackerVillageNote%">%attackerVillageNote%</a><br />
                                        <a class="noShadow nameOfPlayer player-name attacker-name" data-pn="%attackerPlayerName%" href="#">%attackerPlayerName%</a>
                                        <br class="attacker playerInlineNote" /><a class="noShadow attacker playerInlineNote" onclick="ROE.Reports.showNotePopup($(this));" data-note="%attackerPlayerNote%">%attackerPlayerNote%</a>
                                        <br class="brAfterAttacker" />
                                        <span class="role fontSilverFrSCsm2 noShadow"><%=RS("attacker") %></span>
                                    </p>
                                </div>
                            
                        </div>                    
                        <div class="rightSide">
                            
                                <div class="iconsRow">
                                    
                                    <div class="villageIconWrapper">
                                        <div class="noteIconOverlapContainer">
                                            <div class="villageIconShiner"><a class="icon defenderVillage village-name  jsV roundedIcon" data-x="%defenderVillageXCord%" data-y="%defenderVillageYCord%" data-vid="%defenderVillageID%" data-opid="%defenderPlayerID%" href="#" style="background-image:url('%defenderVillageIcon%'), url('https://static.realmofempires.com/images/map/bigtile_5.jpg');"></a></div>                                            
                                            <div class="note doNotCatpureClick defenderVillageNote"></div>
                                        </div>
                                        <div class="fontSilverNumbersSm noShadow villageLocation">(%defenderVillageXCord%, %defenderVillageYCord%)</div>
                                    </div>
                                    <div class="playerIconWrapper noteIconOverlapContainer">                                
                                        <div class="icon playerIcon defenderAvatar player-name defender-name" data-pn="%defenderPlayerName%"  style="background-image: url('%defenderPlayerAvatar%');"></div>
                                        <div class="note doNotCatpureClick defenderPlayerNote"></div>
                                    </div>
                                    
                                </div>
                                <div class="namesRow">
                                    <p>
                                        <!--fontGoldFrSCmed-->
                                        <a class="noShadow defenderVillage village-name  jsV" data-x="%defenderVillageXCord%" data-y="%defenderVillageYCord%" data-vid="%defenderVillageID%" data-opid="%defenderPlayerID%" href="#">%defenderVillageName%</a>
                                        <br class=" defender villageInlineNote" />
                                            <a class="noShadow defender villageInlineNote" onclick="ROE.Reports.showNotePopup($(this));" data-note="%defenderVillageNote%">%defenderVillageNote%</a><br />
                                        <!-- fontGoldFrSClrg -->
                                        <a class="noShadow nameOfPlayer player-name defender-name" data-pn="%defenderPlayerName%" href="#">%defenderPlayerName%</a>
                                        <br class=" defender playerInlineNote" />
                                        <a class="noShadow defender playerInlineNote" onclick="ROE.Reports.showNotePopup($(this));" data-note="%defenderPlayerNote%">%defenderPlayerNote%</a>
                                        <br class="brAfterDefender" />
                                        <span class="role fontSilverFrSCsm2 noShadow"><%=RS("defender") %></span>       
                                    </p>
                                 </div>
                           
                        </div>
                        <div class="vsIcon icon" style="background-image: url('%vsIconImageUrl%');"></div>
                    </div>                    
                </div> 

                <div class="sectionDivider"></div>
            </div>

            <div class="reportDetailBody">
                <div class="themeM-progress loyalty data" style="display:none;">
                    <div class="bg">
                        <div class="progress-container">
                            <div class="indicator-after progressBar" style="width: %afterYeaPercentage%; background-color: rgb(%barColorR%, %barColorG%, %barColorB%) !important;"></div>                            
                        </div>
                    </div>
                    <div class="fg">
                        <div class="label fontSilverFrLCmed">
                            <span><%=RS("councilOfEldersApprovalChange") %> %beforeYeaPercentage% <%=RS("to") %> %afterYeaPercentage% <%=RS("yea") %></span>
                        </div>
                    </div>
                </div>

                <div class="bonusCallOut capturedVillage" style="display:none;">
                    <div class="icon" style="background-image:url('https://static.realmofempires.com/images/icons/M_VillGain.png');"></div>
                    <div class="bonusDetails fontTanFrLCmed"><%=RS("villageCapturedSuccessfully") %></div>
                </div>
                <div class="sectionDivider capturedVillage" style="display:none;"></div>

                <div class="reportTables"></div> 

                <div style="width:100%; overflow:auto;">
                    <div class="leftSide">
                        <a class="pasteToBattleSimBtn asDefender" href="#" onclick="ROE.UI.Sounds.click(); ROE.Frame.popupBattleSim('%reportId%', 'def', 'att');"><%=RS("pasteAsDefender") %></a>
                        <a class="pasteToBattleSimBtn asAttacker" href="#" onclick="ROE.UI.Sounds.click(); ROE.Frame.popupBattleSim('%reportId%', 'att', 'att');"><%=RS("pasteAsAttacker") %></a>
                    </div>
                    <div class="rightSide">
                        <a class="pasteToBattleSimBtn asDefender" href="#" onclick="ROE.UI.Sounds.click(); ROE.Frame.popupBattleSim('%reportId%', 'def', 'def');"><%=RS("pasteAsDefender") %></a>
                        <a class="pasteToBattleSimBtn asAttacker" href="#" onclick="ROE.UI.Sounds.click(); ROE.Frame.popupBattleSim('%reportId%', 'att', 'def');"><%=RS("pasteAsAttacker") %></a>
                    </div>

                    <div class="plunder"></div>
                </div>

                <div class="sectionBar"></div>

                <!-- intellegence data -->
                <div class="versus briefingText" style="display:none;">
                    <div class="rowLine" style="margin-bottom:8px;">
                        <div class="leftSide textColumnMode">
                                                        
                        </div>
                        <div class="rightSide textColumnMode">
                            
                        </div>
                    </div>
                    <div class="sectionDivider"></div>
                </div>

                <div class="moraleInfo fontTanFrLCmed" style="text-align:center; display:none;"></div>
                
                <div class="damagedBuildings"></div>

                <div class="cannotSeeNoSurvivingTroops" style="display:none;">
                    <p class="fontTanFrLCmed centered">
                        <%=RS("noneOfYourTroopsSurvived") %>
                    </p>
                    <div class="sectionDivider" style="margin-top:6px;"></div>
                </div>

                <div class="spyStatusMessage" style="display:none;">
                    
                    <!-- Spy status -->
                    
                    <p class="spyStatus cannotSeeSpyNotSuccessful fontTanFrLCmed centered" style="display:none;">
                        <%=RS("notAbleToObtainInfoFromSpies") %>
                    </p>
                    <p class="spyStatus youGotSpiedSuccess fontTanFrLCmed centered" style="display:none;">
                        <%=RS("villageSpiedUponSuccessfully") %>
                    </p>
                    <p class="spyStatus youGotSpiedFailed fontTanFrLCmed centered" style="display:none;">
                        <%=RS("villageSpiedUponFailed") %>
                    </p>
                    <p class="spyStatus spiesSuccessIdentityKnown fontTanFrLCmed centered" style="display:none;">
                        <%=RS("spiesInfiltratedVillageWithOwnerKnowing") %>
                    </p>
                    <p class="spyStatus spiesSuccessIdentityUnknown fontTanFrLCmed centered" style="display:none;">
                        <%=RS("spiesInfiltratedVillageWithoutOwnerKnowing") %>
                    </p>
                    <p class="spyStatus spiesFailedIdentityKnown fontTanFrLCmed centered" style="display:none;">
                        <%=RS("spiesUnsuccessfulSneakingInVillageSecretUnkept") %>
                    </p>
                    <p class="spyStatus spiesFailedIdentityUnknown fontTanFrLCmed centered" style="display:none;">
                        <%=RS("spiesUnsuccessfulSneakingInVillageSecretKept") %>
                    </p>
                     <p class="spyStatus spiesSuccessfulWithInfo fontTanFrLCmed centered" style="display:none;">
                        <%=RS("spiesSuccessfulWithInfo") %>
                    </p>
                    <p class="spyStatus spiesUnsuccessfulWithNoInfo fontTanFrLCmed centered" style="display:none;">
                        <%=RS("spiesUnsuccessfulWithNoInfo") %>
                    </p>
                </div>

                <!-- Example: spy report -->
                <div class="spyReport spiedVillageImage fontWhiteNoShadFrLCmed" style="display:none;">
                    
                    <div class="villageColumn">
                    </div>
                    <div class="levelColumnContainer">
                        <div class="levelColumn levCol1"></div>   
                        <div class="levelColumn levCol2"></div> 
                        <div class="levelColumn levCol3"></div>
                    </div>                 
                </div>

                <div class="bonusCallOut bonusVillage" style="display:none;">
                    <div class="icon" style="background-image:url('https://static.realmofempires.com/images/icons/Q_RecruitHover.png');"></div>
                    <div class="bonusDetails fontGoldFrLCmed"><%=RS("bonusVillage") %> <span class="bonusVillageName"></span></div>
                </div>

            </div>

        </div>        

    </section>
    
    <!-- TEMPLATES -->
    
    <div class="template versus">
        <div class="rowLine">
            <div class="leftSide">
                <div class="village" style="width:100%">
                    <div class="info infoForSupportedAttacker"><span class="fontLucidaGrande noShadow"><%=RS("attackerIdentityUnknown") %></span><br /><span class="fontSilverNumbersSm noShadow">(?,?)</span></div>
                </div>
            </div>
            <div class="rightSide">
                <div class="village">
                    <!--<div class="noteIconOverlapContainer">
                        <div class="note hideNote"></div>
                        <div class="icon" style="background-image:url('%villageImage%');"></div>
                    </div>-->
                    <div class="info"><a class="fontLucidaGrande noShadow village-name" data-vid="%villageID%" onclick="ROE.Reports.showVillageProfile($(this));" href="#">%villageName%</a><br /><span class="fontSilverNumbersSm noShadow">(%villageX%,%villageY%)</span></div> 
                                                  
                </div>
            </div>
        </div>
    </div>
    
    <table>
        <tr class="template headingsRow">
            <td class="deployed unitCount"><%=RS("columnAtk") %></td><td class="lost unitCount"><%=RS("columnLost") %></td><td class="remaining unitCount"><%=RS("columnLeft") %></td><td class="unitIcon"><div style="width:16px; height:16px;"></div></td><td class="deployed unitCount"><%=RS("columnDef") %></td><td class="lost unitCount"><%=RS("columnLost") %></td><td class="remaining unitCount"><%=RS("columnLeft") %></td>
        </tr>
        <tr class="template unitsRow">
            <td class="deployed unitCount">%leftDeployed%</td><td class="lost unitCount">%leftLost%</td><td class="remaining unitCount">%leftRemaining%</td><td class="unitIcon"><div class="unitImg" title="%unitName%" alt="%unitName%" style="background-image:url(%unitImage%)"></div></td><td class="deployed unitCount">%rightDeployed%</td><td class="lost unitCount">%rightLost%</td><td class="remaining unitCount">%rightRemaining%</td>
        </tr>
        <tr class="template dividerRow">
            <td colspan="7"><div class="sectionDivider lighter"></div></td>
        </tr> 
    </table>

    <div class="template damagedBuilding bonusCallOut" style="background-image:url('%damagedBuildingIcon%');">
        
        <div class="bonusDetails fontTanFrLCmed">%damangedBuildingName% <%=RS("damagedFrom") %> <span class="fontWhiteNumbersMed">%damagedOldLevel%</span> <%=RS("to") %> <span class="fontWhiteNumbersMed">%damagedNewLevel%</span></div>
    </div>

    <div class="template buildingLevel" style="background-image:url(%buildingIcon%);"> <%=RS("lvl") %> %levelValue%<br /></div>

    <div class="template buildingLevel2" style="background-image:url(%buildingIcon%);"><div class='buildingNameCol'><span class="fontTanFrLCmed">%buildingName%</div><div class='buildingLevelCol'><span class="fontTanFrLCmed"><%=RS("lvl") %></span> <span class="fontWhiteNumbersMed">%levelValue%</span></div><br /></div>
    <!--<div class="template buildingLevel2" style="background-image:url(%buildingIcon%);"><%=RS("lvl") %></span> <span class="fontWhiteNumbersMed">%levelValue%</span> <span class="fontTanFrLCmed">(%buildingName%)<br /></div>-->

    <!-- Forwarded By -->
    <div class="template reportForwarded">
        <div class="forwardedByWho">
            <div class="forwardedTitle fontTanFrLCmed">Forwarded By: </div>                        
            <div class="forwardedPlayerAvatar" style="background-image:url('%forwardedByAvatar%');"></div>
            <a id="forwardedByName" class="fontGoldFrSClrg noShadow player-name forward-name" href="PlayerPopup.aspx?pid=%forwardedByID%" target="_blank">%forwardedByName%</a>                       
            <a id="blockForwardPlayer" class="blockPlayerBtn"></a>
        </div>
        <div class="sectionDivider lighter"></div>
        <div class="reportTime fontSilverFrLCmed noShadow"><%=RS("forwardedOn") %> %forwardedTime%</div>
    </div>

    <!--<div class="template notePopup">       
        <div class="NotesText" style="display: none;"></div>
        <div class="EditMessage">
            <textarea rows="10" id="EditorNotes" class="editNotesTextarea" autofocus="" onfocus="ROE.Frame.inputFocused();" onblur="ROE.Frame.inputBlured();"></textarea>
            <div class="noteButtonWrapper">
                 <div class="saveButton">
                    <a class="Edit customButtomBG" onclick="ROE.UI.VillageOverview.notePopup_save(); $(this).closest('.simplePopupOverlay').remove();">Save</a>
                </div>
                <div class="clearButton">
                    <a class="Edit customButtomBG" onclick="$('.village_notePopup .editNotesTextarea').val('');">Clear</a>
                </div>
            </div>
            <div class="paddinator2000"></div>
        </div>        
    </div>-->
    
  
    <!-- Phrases -->
    <div id="ReportDetailPhrases" style="display:none;">
        <div ph="Plunder"><%=RS("plunderColon") %></div>
        <div ph="SilverInTreasury"><%=RS("silverInTreasury") %></div>
        <div ph="WasAttackedAndDamaged"><%=RS("wasAttackedAndDamaged") %></div>
        <div ph="ToLevel"><%=RS("toLevel") %></div>
        <div ph="Silver"><%=RS("silver") %></div>
        <div ph="Unknown"><%=RS("unknown") %></div>
        <div ph="TitleVictory"><%=RS("titleVictory") %></div>
        <div ph="TitleSuccess"><%=RS("titleSuccess") %></div>
        <div ph="TitleDefeat"><%=RS("titleDefeat") %></div>
        <div ph="TitleWarning"><%=RS("titleWarning") %></div>
        <div ph="TitleVillageCaptured"><%=RS("titleVillageCaptured") %></div>     
        <div ph="TitleReport"><%=RS("titleReport") %></div>    
    </div>

</asp:Content>