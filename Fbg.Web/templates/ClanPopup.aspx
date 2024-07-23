<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/templates/masterPopupTemplate_m.master" CodeFile="ClanPopup.aspx.cs" Inherits="templates_ClanPopup" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">
     
    <div id="clan_popup" >
            
        <section id="clanMain" class="bgold">
            <div class="clanMainPage" >
            <div class="clanName"></div>
            
            <div class="renameClan"></div>

            <div class="clanCreate">
                <div class="clanEntry" >
                    <span><%=RS("CreateYourClan") %></span>
                    <div><%=RS("ClanName") %>:
                        <input type="text" id="inputClanName">
                        </BR><div id='clanNameSubmit' class='customButtomBG' ><%=RS("Create") %></div>
                    </div>                    
                </div>
            </div>

            <ul>
                <li id="clanProfile" class="extra customButtomBG2" ><%=RS("PublicProfile") %></li>                
                <li id="clanMembers" class="customButtomBG2" ><%=RS("Members") %>: <span></span></li>                
                <li id="clanForum" class="extra customButtomBG2" >Clan Forum <img class="effect-pulse newPostsOnForumIndicator" src="https://static.realmofempires.com/images/NewForum.PNG" /></li>
                <li id="clanForumAdmin" class="extra customButtomBG2" >Clan Forum Admin</li>
                <li id="clanDiplomacy" class="extra customButtomBG2" ><%=RS("Diplomacy") %></li>
                <li id="clanInvitations" class="extra customButtomBG2" ><%=RS("Invitations") %></li>
                <li id="clanSettings" class="extra customButtomBG2" ><%=RS("Settings") %></li>
                <li id="clanEmailMembers" class="extra customButtomBG2" >Email Members</li>
                <li id="clanClaims" class="extra customButtomBG2" >Clan Claims</li>
                
            </ul>

            <div id="otherClanProfile" >                
                <div class="clanRank"><%=RS("Rank") %>: <span></span></div>
                <div class="clanPoints"><%=RS("Points") %>: <span></span></div>
                <div class="clanMessage"></div>
                <div class="separator" ><img src="https://static.realmofempires.com/images/misc/m_listbar2.png" ></div>
                <div class="clanRequestInviteBtn customButtomBG"><%=RS("RequestInvite") %></div>
            </div>
            
            <div class="clanAction">
                <div class="separator" ><img src="https://static.realmofempires.com/images/misc/m_listbar2.png" ></div>
                <span class='leaveClanButton customButtomBG sfx2' ><%=RS("Info39LeaveClan") %></span>
                <span class='renameClanButton customButtomBG sfx2' ><%=RS("Info40RenameClan") %></span>
                <span class='disbandClanButton customButtomBG sfx2' ><%=RS("Info21DisbandClan") %></span>
            </div>

            <div id="clanInvites" >
                <div class="separator" ><img src="https://static.realmofempires.com/images/misc/m_listbar2.png" ></div>
                <%=RS("InvitationsFromClans") %>
                <div class="clanInvitelist"></div>
            </div>

            </div>
        </section>

        <section class="clanpages bgold slideLeftTo" >
            
            <div class="clanBackButton themeM-panel style-link linkBack BarTouch">
                <div class="bg">
                    <div class="corner-br"></div>
                </div>

                <div class="fg">
                    <div class="themeM-more">
                        <div class="bg">
                        </div>

                        <div class="fg">
                            <div class="label">
                                <span></span><br />
                            </div>

                            <div class="arrow-l"></div>
                        </div>
                    </div>

                    <div class="label">
                        <span><%=RS("BackButton") %></span><br />
                    </div>
                </div>
            </div>
         
                <div class="clanProfile clanDetail" >
                    <div class="clanPageTitle"><%=RS("PublicProfile") %></div>
                    
                    <div class="clanProfilePoints">
                        <div class="clanName"></div>
                        <div class="clanRank"><%=RS("Rank") %>: <span></span></div>
                        <div class="clanPoints"><%=RS("Points") %>: <span></span></div>
                    </div>
                    <div class="clanProfileEdits">
                        <div class="clanEdit customButtomBG" ><%=RS("Info26Edit") %></div>
                    </div>
                    <div class="clanMessage"></div>
                    <div class="clanEditMessage">
                        <textarea id="clanEditorMessage" autofocus ></textarea>
                    </div>
                </div> 
                 
                <div class="clanInvitations clanDetail" >
                    <div class="clanInvitePlayer">
                        <div class="clanName"><%=RS("InvitePlayers") %></div>
                        <%=RS("EnterPlayerName") %>
                        <div>
                            <input type="text" id="clanInvitePlayer" /> 
                            <div class="autocomplete"></div>
                            <div class="clanInviteButton  customButtomBG sfx2" ><%=RS("InviteButton") %></div>
                        </div>
                    </div>
                    <div >
                        <div class="clanName"><%=RS("CurrentInvitations") %></div>
                        <%=RS("PeopleInvitedToclan") %>
                        <div class="claninviteLeft"></div>
                        <div class="clanInviteTable"></div>
                    </div>
                </div>  
           

                <div class="clanMembers clanDetail" >
                    <div class="clanPageTitle"><%=RS("Members") %></div>
                    <div class="clanPageTitleLoading"><%=RS("loading") %></div>
                    <div class="clanDefaultRole" >
                        <a href="#" class="clanSetRole_Inviter sfx2"></a><%=RS("Info8DefaultRolesText") %>
                        <div class="separator2"></div>
                    </div>
                    <div class="clanMembersTable">

                        <table>
                        <tr class="cmRow" id="memberlist_0">
                            <td class="cmName sfx2">
                                <span></span>
                                <div class="cmRole"></div>
                            </td>
                            <td class="cmPoint"><br><img class="villNum">
                                </td><td class="cmMore sfx2" data-info="">
                                    <svg><circle cx="17" cy="17" r="16" style="fill:rgb(255,0,0)"></circle></svg>
                                    <div class="clanMoreInfo"></div>
                            </td>
                        </tr>
                        </table>
                        
                    </div>


                    <a class="messageAllMembers" href="#" >Message all Members</a>
                </div>  
                 <div class="clanForum clanDetail">
                     <iframe id="clanForumIframe" src="" data-src="ClanForum.aspx" style="width:100%;height:100%"></iframe>
                </div>  
                 <div class="clanForumAdmin clanDetail">
                     <iframe id="clanForumAdminIframe" src="" data-src="ManageForums.aspx" style="width:100%;height:100%"></iframe>
                </div>  
                 <div class="clanEmailMembers clanDetail">
                     <iframe id="clanEmailMembersiFrame" src="" data-src="ClanEmailMembers.aspx" style="width:100%;height:100%"></iframe>
                </div>  
                 <div class="clanClaims clanDetail">
                     <iframe id="clanClaimsiFrame" src="" data-src="clanClaims.aspx" style="width:100%;height:100%"></iframe>
                </div>  
                <div class="clanSettings clanDetail">
                     <iframe id="clanSettingsiFrame" src="" data-src="ClanSettings.aspx" style="width:100%;height:100%"></iframe>
                </div>  
                
                <div class="clanDiplomacy clanDetail" >
                    <div class="clanPageTitle"><%=RS("Diplomacy") %></div>
                    <div class="clanAddDimpl"><%=RS("AddClanName") %><BR>
                        <input type="text" id="clanAddClanName" >
                            <div class="autocomplete"></div>
                        <span id="addAlly" class="addDipl customButtomBG sfx2" data-dipltyle="0" ><%=RS("Ally") %></span>
                        <span id="addEnemy" class="addDipl customButtomBG sfx2" data-dipltyle="1" ><%=RS("Enemy") %></span>                        
                        <span id="addNAP" class="addDipl customButtomBG sfx2" data-dipltyle="2" ><%=RS("NAP") %></span>
                    </div>
                    <div class="clanDiplTitleLoading"><%=RS("loading") %></div>
                    <div class="separator2"></div>                    
                    <div class="clanDiplTable"></div>
                </div>  
        </section>

        <div class="phrases">
            <div ph="1"><%=RS("Info1NoInvite") %></div>
            <div ph="2"><%=RS("Info2NotInClan") %></div>
            <div ph="3"><%=RS("Info3CreateYourClan") %></div>
            <div ph="4"><%=RS("Info4Allies") %></div>
            <div ph="5"><%=RS("Info5Enemies") %></div>
            <div ph="6"><%=RS("Info6NAP") %></div>
            <div ph="7"><%=RS("Info7DefaultRoles") %></div>
            <div ph="8"><%=RS("Info8DefaultRolesText") %></div>
            <div ph="9"><%=RS("Info9ExpiresOn") %></div>
            <div ph="10"><%=RS("Info10Days") %></div>
            <div ph="11"><%=RS("Info11BriefClanInfo") %></div>
            <div ph="12"><%=RS("Info12Revoke") %></div>
            <div ph="13"><%=RS("Info13NoPendingInvite") %></div>
            <div ph="14"><%=RS("Info14InviteNumbers") %></div>
            <div ph="15"><%=RS("Info15MoreInviteOn") %></div>
            <div ph="16"><%=RS("Info16ProcessingInvite") %></div>
            <div ph="17"><%=RS("Info17Accept") %></div>
            <div ph="18"><%=RS("Info18Reject") %></div>
            <div ph="19"><%=RS("Info19RejectingClanInvite") %></div>
            <div ph="20"><%=RS("Info20LeaveClanConfirm") %></div>
            <div ph="21"><%=RS("Info21DisbandClan") %></div>
            <div ph="22"><%=RS("Info22JoiningClanConfirm") %></div>
            <div ph="23"><%=RS("Info23RevokeInvite") %></div>
            <div ph="24"><%=RS("Info24ClanNameChanged") %></div>
            <div ph="25"><%=RS("Info25Save") %></div>
            <div ph="26"><%=RS("Info26Edit") %></div>
            <div ph="27"><%=RS("Info27UpdatingProfile") %></div>
            <div ph="28"><%=RS("Info28UpdatingDiplomacy") %></div>
            <div ph="29"><%=RS("Info29SavingDiplomacy") %></div>
            <div ph="30"><%=RS("Info30SavingSetting") %></div>
            <div ph="31"><%=RS("Info31RemoveRoleConfirm") %></div>
            <div ph="32"><%=RS("Info32RoleChanged") %></div>
            <div ph="33"><%=RS("Info33DismissConfirm") %></div>
            <div ph="34"><%=RS("Info34JoinClanConfirm") %></div>
            <div ph="35"><%=RS("Info35DiplomacyExist") %></div>
            <div ph="36"><%=RS("Info36ClanDontExist") %></div>
            <div ph="37"><%=RS("Info37ThisIsYourClan") %></div>
            <div ph="38"><%=RS("Info38ClanAlreadyExist") %></div>
            <div ph="39"><%=RS("Info39LeaveClan") %></div>
            <div ph="40"><%=RS("Info40RenameClan") %></div>
            <div ph="41"><%=RS("CantLeaveClan") %></div>
            <div ph="42"><%=RS("NotValidInvite") %></div>
            <div ph="43"><%=RS("ClanMemberLimit") %></div>
            <div ph="44"><%=RS("InvalidChars") %></div>
            <div ph="45"><%=RS("adminCannotDismiss") %></div>
            <div ph="46"><%=RS("onlyOwnerNoDismiss") %></div>
            <div ph="47"><%=RS("PlayerDismissed") %></div>
            <div ph="48"><%=RS("OnlyOwnerDisband") %></div>
            <div ph="49"><%=RS("None") %></div>
            <div ph="50"><%=RS("Dismiss") %></div>
            <div ph="51"><%=RS("Profile") %></div>
            <div ph="52"><%=RS("Steward") %></div>
            <div ph="53">Leaving and joining clans no longer allowed</div>
            <img src="https://static.realmofempires.com/images/icons/clanMoreInfo2.png" ></img>
        </div>
    </div>
   
    
</asp:Content>