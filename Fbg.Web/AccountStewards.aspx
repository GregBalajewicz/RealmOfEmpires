<%@ Page Language="C#" MasterPageFile="~/main.master" AutoEventWireup="true" CodeFile="AccountStewards.aspx.cs" Inherits="AccountStewards" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <% if(isD2) { %>
    <style>
        body {
            font-family: Georgia, 'Playfair Display';
            font-size: 12px;
            background:none;
        }

        html {
            /*background-color: #000000;
            background: url('https://static.realmofempires.com/images/backgrounds/M_BG_Tower.jpg') no-repeat center center fixed;
            -webkit-background-size: cover;
            -moz-background-size: cover;
            -o-background-size: cover;
            background-size: cover;*/
            background: transparent;
        }

        .TDContent {
            background-color: rgba(6, 20, 31, 0.9) !important;
            height: 100%;
            position: absolute;
            overflow: auto;
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
            cursor:pointer;
        }

        .d2_boxedContent {
            padding:6px !important;
        }

        .d2_tableWrapper {
            border-spacing:0 !important;
        }

        .d2_tableWrapper td {
            padding: 0px;
            padding-bottom: 6px;
        }

      
        .d2_border {
           
            border:none !important;
        }

        .d2_stripeRow {
            background-color: rgba(88, 140, 173, 0.1);
        }

        .d2_stripeRowCol {
            padding:0;
        }
    </style>
    <% } %>
    <%=Utils.GetIframePopupHeaderForNotPopupBrowser("Stewardship", (isMobile && !IsiFramePopupsBrowser),"https://static.realmofempires.com/images/icons/M_Mail.png")%>
    <asp:Panel ID="panelStewardshipDisabled" runat="server"  style="font-size: 1.1em" Visible=false>
    <%=RS("Disabled")%>
    </asp:Panel>
    <asp:Panel ID="pnlUI" runat="server"  style="font-size: 1.1em">
        <br />
        <%if (isMobile) {%>
        Account Stewardship is a desktop function. You can manage it here, but must login via desktop browser to use it. 
        <%} else { %>
        <%=RS("DelegateControlToAnotherPlayer") %> &nbsp;<br />
        <br />
        
        <B><%=RS("InactiveForFourteenDaysDeletion") %></B> <div class="jaxHide" rel='<%=RS("TellMeMore") %>'>
        <%=RS("BeActiveAsMuchAsPossible") %> <em><%=RS("WhoHasNotAccepted") %></em>, <%=RS("AccountWillBeDeleted") %>.<br />
        </div>
        <br />
        <B><%=RS("StewardshipIsForUpToThirtyDays") %></B> <div class="jaxHide" rel='<%=RS("TellMeMore") %>'>
        <%=RS("StewardshipIsForTemporaryAbsence") %>
        <br /><%=RS("LoginEveryThirtyDays") %> 
        </div>
        <br />
        <B><%=RS("MakeSureToTrustYourSteward") %></B> <div class="jaxHide" rel='<%=RS("TellMeMore") %>'>
        <%=RS("EnsureThisIsSomeoneYouCanTrust") %><br /></div>
        <br />
        <%} %>
        <table runat="server" id="tblUI" cellpadding="3">
            <tr class="d2_stripeRow">
                <td valign="top" width="50%" class="d2_stripeRowCol">
                    <asp:Panel runat="server" ID="panelStewardsOfMyKingdom">
                        <div class="BoxHeader">
                            <%=RS("StewardOfMyKingdom") %></div>
                        <div style="border: solid 1px #4B3D32; padding: 2px;" class="d2_border">
                            <asp:Panel runat="server" ID="panelAppointSteward" CssClass="d2_boxedContent">
                                <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="Error" Width="400px" ValidationGroup="appoint" />
                                <%=RS("Appoint") %>
                                <asp:TextBox ID="txtPlayerName" runat="server" CssClass="TextBox" Font-Bold="False" ValidationGroup="appoint"></asp:TextBox><asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtPlayerName" CssClass="Error" Display="Dynamic" ErrorMessage='<%#RS("EnterPlayerName") %>' ToolTip='<%#RS("EnterPlayerName") %>' ValidationGroup="appoint">*</asp:RequiredFieldValidator><asp:CustomValidator ID="customValidator_StewardName" runat="server" ControlToValidate="txtPlayerName" CssClass="Error" Display="Dynamic" ErrorMessage='<%#RS("NickNameNotFound") %>' ToolTip='<%#RS("NickNameNotFound") %>' ValidationGroup="appoint">*</asp:CustomValidator><asp:CustomValidator ID="customValidator_StewardNameMe" runat="server" ControlToValidate="txtPlayerName"  CssClass="Error" Display="Dynamic" ErrorMessage='<%#RS("TryingToTransferStewardshipToYourself") %>' ToolTip='<%#RS("CannotTransferToYourSelf") %>' ValidationGroup="appoint">*</asp:CustomValidator>
                                <%=RS("AsMyAccountSteward") %><br />
                                <asp:Button ID="btnAppoint" runat="server" CssClass="inputbutton" Text='<%#RS("AppointNow") %>' OnClick="btnTransfer_Click" ValidationGroup="appoint"></asp:Button><br />
                                <br />
                            </asp:Panel>
                            <asp:GridView ID="gvMyStewards" runat="server" GridLines="None" CssClass="TypicalTable stripeTable" CellPadding="1" CellSpacing="1" AutoGenerateColumns="False" ShowHeader="False" OnRowDeleting="gvMyStewards_RowDeleting" DataKeyNames="RecordID" OnRowCommand="gvMyStewards_RowCommand" OnRowDataBound="gvMyStewards_RowDataBound">
                                <Columns>
                                    <asp:CommandField ShowDeleteButton="True" />
                                    <asp:BoundField ItemStyle-Font-Bold="true" DataField="Name" />
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:Label Font-Italic="true" ID="Label1" runat="server" Text='<%# BindState(Eval("State")) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </asp:Panel>
                    <asp:Panel runat="server" ID="panelStewardshipTransfer" Visible="false">
                        <div class="BoxHeader">
                            <%=RS("TransferToAnotherSteward") %></div>
                        <div style="border: solid 1px #4B3D32; padding: 2px;">
                            <%=RS("YouAreTheStewardOfThisAccount") %><br />
                            <asp:ValidationSummary ID="ValidationSummary2" runat="server" CssClass="Error" Width="400px" ValidationGroup="transfer" />
                            <%=RS("TransferStewardshipTo") %> &nbsp;<asp:TextBox ID="txtTransferToPlayerName" runat="server" CssClass="TextBox" Font-Bold="False" ValidationGroup="transfer"></asp:TextBox><asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtTransferToPlayerName" CssClass="Error" Display="Dynamic" ErrorMessage='<%#RS("EnterPlayerName") %>' ToolTip='<%#RS("EnterPlayerName") %>' ValidationGroup="transfer">*</asp:RequiredFieldValidator><asp:CustomValidator ID="customValidator_TransferPlayerName" runat="server" ControlToValidate="txtTransferToPlayerName" CssClass="Error" Display="Dynamic" ErrorMessage='<%#RS("NickNameNotFound") %>' ToolTip='<%#RS("NickNameNotFound") %>' ValidationGroup="transfer">*</asp:CustomValidator><asp:CustomValidator ID="customValidator_TransferToSelf" runat="server" ControlToValidate="txtTransferToPlayerName" CssClass="Error" Display="Dynamic" ErrorMessage='You are already a steward of this account' ToolTip='You are already a steward of this account' ValidationGroup="transfer">*</asp:CustomValidator>
                            <asp:Button ID="btnTransfer" runat="server" CssClass="inputbutton" Text="Transfer" OnClick="Button1_Click" ValidationGroup="transfer"></asp:Button><br />
                            <br />
                            <asp:Panel ID="panelTransfer_confirm" runat="server" Visible="False">
                                <strong><%=RS("PleaseConfirmTheTransfer") %></strong><br />
                                <br />
                                <strong><%=RS("TransferringAccountToSteward") %></strong><asp:HyperLink ID="linkTransferingTo" runat="server" Font-Bold="True" Target="_blank">[linkTransferingTo]</asp:HyperLink><br />
                                <br />
                                <%=RS("ConfirmThisIsTheRightPlayer") %><br />
                                <br />
                                <strong><%=RS("ProceedQuestionMark") %><br />
                                    <br />
                                </strong>
                                <asp:Button ID="btnTransfer_go" runat="server" CssClass="inputbuttonInYourFace" OnClick="btnTransfer_go_Click" Text='<%#RS("YesGo") %>' ValidationGroup="ts1" />
                                <asp:Button ID="btnTransfer_cancel" runat="server" CausesValidation="False" CssClass="inputbutton" OnClick="btnTransfer_cancel_Click" Text='<%#RS("Cancel") %>' /><br />
                                <br />
                            </asp:Panel>
                        </div>
                    </asp:Panel>
                </td>
                <td valign="top" width="50%"  runat="server" id="areapanelMyStewardship" class="d2_stripeRowCol">
                    <asp:Panel runat="server" ID="panelMyStewardship">
                        <div class="BoxHeader">
                            <%=RS("KingdomsIAmAStewardOf") %></div>
                        <div style="border: solid 1px #4B3D32; padding: 2px;" class="d2_border d2_boxedContent">
                            <asp:GridView ID="gvMyStewardship" runat="server" GridLines="None" CssClass="TypicalTable stripeTable d2_tableWrapper" CellPadding="4" CellSpacing="4" AutoGenerateColumns="False" ShowHeader="False" OnRowDeleting="gvMyStewardship_RowDeleting" DataKeyNames="RecordID" OnRowCommand="gvMyStewardship_RowCommand" OnRowDataBound="gvMyStewardship_RowDataBound" OnRowCancelingEdit="gvMyStewardship_RowCancelingEdit" EmptyDataText='<%#RS("NoneAtThisTime") %>'>
                                <Columns>
                                    <asp:CommandField ShowDeleteButton="True" DeleteText='<%=RS("SquareBracketsCancelAppointment") %>' />
                                    <asp:BoundField DataField="Name">
                                        <ItemStyle Font-Bold="True" />
                                    </asp:BoundField>
                                    <asp:TemplateField ShowHeader="False">
                                        <ItemTemplate>
                                            <asp:HyperLink ID="linkLogin"  runat="server" Text='<%#RS("LoginElipses") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <B><%=RS("Critical") %></B>:<span style="color: #D4D1D4;"> <%=RS("CannotHaveMultipleTabsWhenSteward") %> </span>
                        </div>

                    </asp:Panel>
                </td>
            </tr>
            <tr>
                <td colspan="2" valign="top">
                    <hr />
                </td>
            </tr>
            <tr>
                
                <td valign="top" runat="server" id="areaInfo">
                    <div class="BoxHeader">
                        <%=RS("HowKingdomStewardshipWorks") %></div>
                     <div class="d2_boxedContent">
                    <%=RS("AppointAStewardBeforeAccountDeletion") %>
                    <br />
                    <br />
                    <ul>
                        <%=RS("StewardshipPointsOne") %>
                    </ul>
                    </div>
                    <div class="BoxHeader">
                        <%=RS("StewardCannotPoints") %></div>
                     <div class="d2_boxedContent">
                        <%=RS("YourStewardCannot") %>
                    </ul>
                    </div>
                </td>
                <td valign="top">
                    <div class="BoxHeader">
                        <%=RS("Rules") %></div>
                    <div class="d2_boxedContent">
                    <%=RS("MustAcceptTheRules") %><br />
                    
                    
                        <asp:Label ID="lblAcceptRules" runat="server" style="font-size:medium;" CssClass="Error" Text='<%#RS("MustAcceptTheFollowing") %>' Visible="False"></asp:Label><br />
                        <asp:CheckBox ID="cbRulesAccepted" runat="server" Text='<%#RS("IAcceptTheFollowing") %>' /></strong>
                    <br />
                    <ul>                      
                        <%=RS("StewardshipPointsTwo") %>
                        <%=RS("StewardshipPointsThree") %>
                            <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="tou.aspx" Target="_blank"><%=RS("GeneralTermsOfUse") %></asp:HyperLink></li>
                    </ul>
                    
                    <%=RS("LastUpdatedDate") %></p>

                    </div>
                </td>
            </tr>
        </table>

    </asp:Panel>
</asp:Content>
