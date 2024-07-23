<%@ Page Language="C#" MasterPageFile="~/main.master" AutoEventWireup="true" CodeFile="Tags_Filters.aspx.cs"
    Inherits="Tags_Filters" ValidateRequest="false" %>

<%@ Register Src="Controls/TagHelp.ascx" TagName="TagHelp" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="Tabs">
        <ul>
            <li><a href="Tags.aspx">
                <%= RSc("tab_MyTags") %></a> </li>
            <li><a href="Tags_VillageTags.aspx">
                <%= RSc("tab_CurVillTags") %></a> </li>
            <li class="selected"><a href="Tags_Filters.aspx">
                <%= RSc("tab_MyFilters") %></a> </li>
        </ul>
    </div>
    <div class="TabContent">
        <uc1:TagHelp ID="TagHelp1" runat="server" />
        <asp:Label ID="lblNotagsMsg" runat="server" Font-Bold="True" Font-Size="1.1em" Text='<%# RS("noTagsDef1") %>'
            Visible="False" />
        <asp:Panel ID="panelPF" runat="server" Visible="false" Font-Size="1.1em">
            <%= RSc("funcPremFeat") %>
            <asp:HyperLink ID="linkPF" runat="server" onclick="return popupUnlock(this);" Target="_blank"><%= RSc("unlockNow") %></asp:HyperLink>
        </asp:Panel>
        <asp:Panel ID="panelUI" runat="server">
            <table cellpadding="3">
                <tr>
                    <td valign="top">
                        <div class="BoxHeader">
                            <%= RSc("tab_MyFilters") %></div>
                        <div style="border: solid 1px #4B3D32; padding: 2px;">
                            <asp:TextBox ID="txt_FilterName" runat="server" CssClass="TextBox" MaxLength="30" />
                            <asp:Button ID="btn_AddFilter" runat="server" Text='<%# RS("add") %>' CssClass="inputbutton"
                                OnClick="btn_AddFilter_Click" />
                            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                <ContentTemplate>
                                    <asp:GridView ID="gv_Filters" runat="server" AutoGenerateColumns="false" OnSelectedIndexChanged="gv_Filters_SelectedIndexChanged"
                                        DataKeyNames="ID" CssClass="TypicalTable stripeTable" EmptyDataText='<%# RS("noTagsDef2") %>'
                                        GridLines="None" CellSpacing="1" Width="100%">
                                        <Columns>
                                            <asp:CommandField ButtonType="link" SelectText="Select" ShowSelectButton="True">
                                                <ItemStyle HorizontalAlign="Center" Width="20px" />
                                            </asp:CommandField>
                                            <asp:BoundField DataField="Sort" HeaderText="Order" />
                                            <asp:BoundField DataField="Name" HeaderText="Filter Name" />
                                        </Columns>
                                        <RowStyle CssClass="DataRowNormal" />
                                        <HeaderStyle CssClass="TableHeaderRow" />
                                        <AlternatingRowStyle CssClass="DataRowAlternate" />
                                    </asp:GridView>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            &nbsp;&nbsp;
                            <asp:Label ID="lbl_Result" runat="server" Text="Deletion Success" ForeColor="#00CC00"
                                Visible="False" />
                            <asp:UpdateProgress ID="UpdateProgress2" runat="server">
                                <ProgressTemplate>
                                    <img src='<%=RSic("busy_tinyred")%>' />
                                    <%= RS("_loading") %>...</ProgressTemplate>
                            </asp:UpdateProgress>
                        </div>
                    </td>
                    <td valign="top">
                        <div class="BoxHeader">
                            <%= RS("h_filterDetails") %></div>
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <div style="border: solid 1px #4B3D32; padding: 2px;">
                                    <asp:Label ID="lbl_NoSelectedTag" runat="server" Text='<%# RS("selectFromLeft")%>' />
                                    <asp:Panel ID="pnl_SelectedTag" runat="server" Visible="false">
                                        <asp:Panel ID="pnl_DisplayData" runat="server">
                                            <asp:Label ID="lbl_Name" runat="server" Font-Size="12pt" />
                                            &nbsp;&nbsp; &nbsp;
                                            <asp:LinkButton ID="btn_Edit" runat="server" 
                                                OnClick="btn_Edit_Click" ><img src="https://static.realmofempires.com/images/item_rename.png"></img></asp:LinkButton>
                                            <asp:LinkButton ID="btn_Delete" runat="server" 
                                                OnClick="btn_Delete_Click" message='<%# RS("sureToDelFilter")%>' OnClientClick='return confirm(this.getAttribute("message"))' >
                                                <img src="https://static.realmofempires.com/images/cancel.png" ></img>
                                            </asp:LinkButton>
                                            <br />
                                            <asp:Label ID="lbl_Desc" runat="server" Font-Italic="True" />
                                        </asp:Panel>
                                        <asp:Panel ID="pnl_EditData" runat="server" Visible="false">
                                            <table>
                                                <tr>
                                                    <td valign="top" style="font-weight: bold;">
                                                        <%= RS("name")%>
                                                        :
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txt_Name" runat="server" CssClass="TextBox" MaxLength="30"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" CssClass="Error" runat="server"
                                                            ErrorMessage="*" ControlToValidate="txt_Name" ValidationGroup="vg_submit" Text='<%# RS("noBlankName")%>' />
                                                        <asp:Label ID="lbl_UpdateResult" runat="server" Text='<%# RS("filterExists")%>' CssClass="Error"
                                                            Visible="False" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td valign="top" style="font-weight: bold;">
                                                        <%= RS("desc")%>
                                                        :
                                                    </td>
                                                    <td>
                                                        <asp:TextBox Width="250" ID="txt_Desc" runat="server" CssClass="TextBox" MaxLength="75"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td valign="top" style="font-weight: bold;">
                                                        <%= RS("order")%>
                                                        :
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txt_Sort" runat="server" CssClass="TextBox" Width="75px" />
                                                        <asp:RangeValidator CssClass="Error" ID="RangeValidator1" runat="server" ErrorMessage="*"
                                                            ToolTip='<%# RSc("NumbersOnly") %>' ControlToValidate="txt_Sort" MaximumValue="30000"
                                                            MinimumValue="-30000" Type="Integer" ValidationGroup="vg_submit" Text='<%# RSc("invNumRange")%>' />
                                                        <br />
                                                        <i>
                                                            <%= RS("allowsToOrder")%></i>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Button ID="btn_Save" runat="server" Text='<%# RS("btn_save")%>' CssClass="inputbutton"
                                                            OnClick="btn_Save_Click" ValidationGroup="vg_submit" />
                                                    </td>
                                                    <td>
                                                        <asp:Button ID="btn_Cancel" runat="server" Text='<%# RS("btn_cancel")%>' CssClass="inputbutton"
                                                            OnClick="btn_Cancel_Click" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                        <hr size="1" noshade />
                                        <div class="BoxHeader">
                                            <%= RS("h_FilterTags")%></div>
                                        <i>
                                            <%= RS("allowsToGroup")%><br />
                                        </i>
                                        <asp:CheckBoxList ID="chklst_SelectedTags" runat="server" DataValueField="TagID"
                                            DataTextField="Name" />
                                        <asp:Button ID="btn_UpdateFilterTags" runat="server" Text='<%# RS("btn_Update")%>'
                                            CssClass="inputbutton" OnClick="btn_UpdateFilterTags_Click" />
                                    </asp:Panel>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </div>
</asp:Content>
