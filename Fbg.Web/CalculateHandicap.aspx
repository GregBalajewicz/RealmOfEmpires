<%@ Page Language="C#" MasterPageFile="~/master_PopupFullFunct.master" AutoEventWireup="true" CodeFile="CalculateHandicap.aspx.cs" Inherits="QuickTransportCoinsPage" Title="Realm Of Empires - Quick Silver Transport" %>

<%@ Register Src="Controls/QuickTransportCoins.ascx" TagName="QuickTransportCoins"
    TagPrefix="uc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" Runat="Server">

    <% if(isD2) { %>
    <style>
        body {
            font-family: Georgia, 'Playfair Display';
            font-size: 12px !important;
            color:#fff !important;
            background-color: rgba(8, 18, 27, 1.0) !important;
            overflow: hidden;
        }

        .TDContent {
            font-size: 12px !important;
            font-weight: normal;           
            background:none;
           /* width:initial;*/
        }

        .TDContent table {
             font-size: 12px !important;
        }

        a, a:active, a:link {
        color: #d3b16f;
        }

         .TextBox {
            -webkit-box-sizing: border-box;
            -moz-box-sizing: border-box;
            box-sizing: border-box;
            background-color: rgba(255,255,255,0.8);
            font-weight: normal;
            font-size: 12px;
            font-family: Georgia, 'Playfair Display';
            padding: 4px;
            width: 100px !important;
        }
         .inputbutton  {
            color: #FFFFFF;
            font-weight: initial;
            padding: 6px !important;
            /*padding-bottom: 3px !important;*/
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

        #cph1_lblHandicap {
             font-size: 12px !important;
            font-weight: bold !important;        
        }

        .ui-menu .ui-menu-item {

            background-color: #fff;
            color: #000;
        }
         .d2Wrapper {
            padding:10px;
        }
        .d2Wrapper img {
            margin: 10px;
            width: 100px;
        }

    </style>

    <div class="d2Wrapper">

    <% } %>
    
    <img src='<%=RSic("d_Knights3e_A_R")%>' style="float: left" /><strong><%= RS("BattleHandicapTitle") %> </strong><%= RS("HandicapSentenceOne") %> <strong><%= RS("Handicap") %></strong>.
    <br />
    <br />
    <%= RS("HandicapSentenceTwo") %> 
    <%= RS("HandicapSentenceThree") %> 
    <br /><br />
    <%= RS("HandicapSentenceFour") %>
    <br />
    <br />
        <table style="font-size:11pt">
            <%if (isMobile) { %>
            <tr >
                <td colspan=5>
                    <%= RS("AttackersPoints") %>
                </td>
            </tr>
            <%} %>
            <tr >
                <%if (!isMobile) { %>
                <td>
                    <%= RS("AttackersPoints") %>
                </td>
                <%} %>
                <td>
                    <asp:TextBox ID="txtAttackersPoints" runat="server" CssClass="TextBox" Text="" Width="60" rel="txtAttackersPoints"></asp:TextBox>
                    <asp:RangeValidator ID="rv_AttackUnitAmount" runat="server" ControlToValidate="txtAttackersPoints" Display="Dynamic" ErrorMessage="*" MaximumValue="2000000000" MinimumValue="0" ToolTip='<%# RSc("NumbersOnly") %>' Type="Integer"></asp:RangeValidator>
                </td>
                <td>
                   |
                </TD>
                <td>
                    <asp:HyperLink runat="server" id="itsMe_attacker" NavigateUrl="#" onclick="return false;" class="AutoPop2" rel="txtAttackersPoints"><%# RSc("Me") %></asp:HyperLink><br />
                </td>
                <td nowrap style="font-size:10pt">
                   |</td>
                <td style="font-size:10pt">
                    <asp:HyperLink style="font-size:7pt;" onclick="return popupUnlock(this);" ID="linkPFAtt" CssClass="LockedFeatureLinkSml" runat="server"><%# RSc("FindPlayer") %></asp:HyperLink>                
                   <asp:Panel runat ="server" ID="spanFindAttacker" CssClass="jaxHide" rel='<%#RS("FindPlayer") %>'><asp:TextBox ID="txtAttackersName" runat="server" CssClass="TextBox jsplayers" Text="" Width="100" ValidationGroup="att"></asp:TextBox><asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtAttackersName" CssClass="Error" Display="Dynamic" ErrorMessage='<%# RSc("EnterName") %>' ToolTip='<%# RSc("EnterName") %>' ValidationGroup="att">*</asp:RequiredFieldValidator><asp:CustomValidator ID="cvAttacker" runat="server" ControlToValidate="txtAttackersName" CssClass="Error" Display="Dynamic" ErrorMessage='<%# RSc("PlayerNotFound") %>' OnServerValidate="cvAttacker_ServerValidate" ToolTip='<%# RSc("PlayerNotFound") %>' ValidationGroup="att">*</asp:CustomValidator>
                       <asp:LinkButton style="font-size:10pt" ID="btnFindAttacker" runat="server" Text='<%# RS("Find") %>' ValidationGroup="att" OnClick="btnFindAttacker_Click1" /></asp:Panel>
                </td>
            </tr>
            <%if (isMobile) { %>
            <tr >
                <td colspan=5>
                    <%= RS("DefendersPoints")%>
                </td>
            </tr>
            <%} %>
            <tr>
                <%if (!isMobile) { %>
                <td>
                    <%= RS("DefendersPoints") %>
                </td>
                <%} %>
                <td>
                    <asp:TextBox ID="txtDefendersPoints" runat="server" CssClass="TextBox" Text="" Width="60" rel="txtDefendersPoints"></asp:TextBox>
                    <asp:RangeValidator ID="RangeValidator1" runat="server" ControlToValidate="txtDefendersPoints"  Display="Dynamic" ErrorMessage="*" MaximumValue="2000000000" MinimumValue="0" ToolTip='<%# RSc("NumbersOnly") %>' Type="Integer"></asp:RangeValidator>
                </td>
                <td>
                   |
                </TD>
                <td>
                    <asp:HyperLink runat="server" id="itsme_defender" NavigateUrl="#" onclick="return false;" class="AutoPop2" rel="txtDefendersPoints"><%# RSc("Me") %></asp:HyperLink>
                  
                </td>
                <td nowrap style="font-size:10pt">
                   |</td>
                <td nowrap style="font-size:10pt">
                    <asp:HyperLink style="font-size:7pt;"  onclick="return popupUnlock(this);" ID="linkPFDef" CssClass="LockedFeatureLinkSml" runat="server"><%# RS("FindPlayer") %></asp:HyperLink>                
                    <asp:Panel runat ="server" ID="spanFindDefender" CssClass="jaxHide" rel='<%#RS("FindPlayer") %>'><asp:TextBox ID="txtDefendersName" runat="server" CssClass="TextBox jsplayers" Text="" Width="100" ValidationGroup="def"></asp:TextBox><asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtDefendersName" CssClass="Error" Display="Dynamic" ErrorMessage='<%# RS("EnterName") %>' ToolTip='<%# RS("EnterName") %>' ValidationGroup="def">*</asp:RequiredFieldValidator><asp:CustomValidator ID="cvDefender" runat="server" ControlToValidate="txtDefendersName" CssClass="Error" Display="Dynamic" ErrorMessage='<%# RS("PlayerNotFound") %>' OnServerValidate="cvAttacker_ServerValidate" ToolTip='<%# RS("PlayerNotFound") %>' ValidationGroup="def">*</asp:CustomValidator>
                       <asp:LinkButton style="font-size:10pt" ID="btnFindDefender" runat="server" Text='<%# RS("Find") %>' ValidationGroup="def" /></asp:Panel>
                </td>
            </tr>
        </table>
    <br />
    <asp:Button ID="btnCalc" runat="server" CssClass="inputbutton" OnClick="btnCalc_Click" Text='<%# RS("CalculateHandicap") %>' />
    <%=isMobile?"<BR>":"" %><asp:Label style="font-size:11pt;font-weight:bold;" ID="lblHandicap" runat="server" Visible="False"></asp:Label>
    &nbsp; &nbsp;
    <asp:HyperLink ID="linkClose" Visible="False" runat="server" NavigateUrl="#" Text='<%# RS("CopyToSim") %>' />  
    
    
        <asp:HyperLink ID="HyperLink1" Visible="False" runat="server" NavigateUrl="#" Text='<%# RS("CopyToSim") %>' />  

        <% if(isD2) { %></div><% } %>
</asp:Content>

