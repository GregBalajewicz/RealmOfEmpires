<%@ Page Language="C#" MasterPageFile="~/master_PopupFullFunct.master" AutoEventWireup="true"
    CodeFile="CalculateDesertionFactor.aspx.cs" Inherits="CalculateDesertionFactor" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">
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
            width: 40px !important;
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


        #cph1_UpdatePanel1 {
            padding:10px;
        }
         #cph1_UpdatePanel1 img {
            margin: 10px;
            width: 100px;
        }


    </style>
    <% } %>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <img src='<%=RSic("d_Troops5e_A_L")%>' style="float: left" />
    <%=RS("infoParagraph")%>
    <br />
    <br />
    <table style="font-size: 11pt">
        <tr>
            <td>
                <%=RS("fromVillage") %>
                <%=isMobile ? "<br />" : "" %><strong>(</strong></b>
                <asp:TextBox ID="txt_FromX" runat="server" CssClass="TextBox" Width="25" />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txt_FromX"
                    ErrorMessage="*" ToolTip='<%# RS("xReq") %>' Display="Dynamic" />
                <asp:RangeValidator ID="RangeValidator1" runat="server" Type="Integer" MinimumValue="-999"
                    MaximumValue="999" ControlToValidate="txt_FromX" ErrorMessage="*" ToolTip='<%# RS("numRange") %>'
                    Display="Dynamic" />
                <strong>,</strong>
                <asp:TextBox ID="txt_FromY" runat="server" CssClass="TextBox" Width="25" />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txt_FromY"
                    ErrorMessage="*" ToolTip='<%# RS("yReq") %>' Display="Dynamic" />
                <strong>
                    <asp:RangeValidator ID="RangeValidator2" runat="server" Type="Integer" MinimumValue="-999"
                        MaximumValue="999" ControlToValidate="txt_FromY" ErrorMessage="*" ToolTip='<%# RS("numRange") %>'
                        Display="Dynamic" />
                    )</strong>
                <%=isMobile ? "<br />" : "" %><%=RS("toVillage") %>
                <%=isMobile ? "<br />" : "" %><strong>(</strong></b>
                <asp:TextBox ID="txt_ToX" runat="server" CssClass="TextBox" Width="25" />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txt_ToX"
                    ErrorMessage="*" ToolTip='<%# RS("xReq") %>' Display="Dynamic" />
                <asp:RangeValidator ID="RangeValidator3" runat="server" Type="Integer" MinimumValue="-999"
                    MaximumValue="999" ControlToValidate="txt_ToX" ErrorMessage="*" ToolTip='<%# RS("numRange") %>'
                    Display="Dynamic" />
                <strong>,</strong>
                <asp:TextBox ID="txt_ToY" runat="server" CssClass="TextBox" Width="25" />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txt_ToY"
                    ErrorMessage="*" ToolTip='<%# RS("yReq") %>' Display="Dynamic" />
                <strong>
                    <asp:RangeValidator ID="RangeValidator4" runat="server" Type="Integer" MinimumValue="-999"
                        MaximumValue="999" ControlToValidate="txt_ToY" ErrorMessage="*" ToolTip='<%# RS("numRange") %>'
                        Display="Dynamic" />
                    )</strong>
            
                <%=isMobile ? "<br />" : "" %><asp:LinkButton ID="btnCalcDistance" runat="server" OnClick="btnCalcDistance_Click"><%=RS("calculate") %></asp:LinkButton>
            </td>
        </tr>
    </table>
    <br />
    <asp:Label Style="font-size: 11pt; font-weight: bold;" ID="lblHandicap" runat="server"
        Visible="False"></asp:Label>
    &nbsp; &nbsp;<%=isMobile ? "<br />" : "" %>
    <asp:HyperLink ID="linkClose" Visible="False" runat="server" NavigateUrl="#" Text='<%#RS("cpyToSim") %>' />
    <asp:HyperLink ID="HyperLink1" Visible="False" runat="server" NavigateUrl="#" Text='<%#RS("cpyToSim") %>' />
    </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
