<%@ Page Language="C#" MasterPageFile="~/main.master" AutoEventWireup="true" CodeFile="Tags.aspx.cs"
    Inherits="Tags" ValidateRequest="false" %>
<%@ Register Src="Controls/TagHelp.ascx" TagName="TagHelp" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="Tabs">
        <ul>
            <li class="selected"><a href="Tags.aspx">
                <%= RSc("tab_MyTags") %>
            </a></li>
            <li><a href="Tags_VillageTags.aspx">
                <%= RSc("tab_CurVillTags")%>
            </a></li>
            <li><a href="Tags_Filters.aspx">
                <%= RSc("tab_MyFilters")%>
            </a></li>
        </ul>
    </div>
    <div class="TabContent">
        <uc1:TagHelp ID="TagHelp1" runat="server" />
        <asp:Panel ID="panelPF" runat="server" Font-Size="1.1em" Visible="false">
            <%= RS("funcPremium")%>&nbsp;
            <asp:HyperLink ID="linkPF" onclick="return popupUnlock(this);" runat="server" Target="_blank"><%= RS("link_UnlockNow")%></asp:HyperLink>
        </asp:Panel>
        <table runat="server" id="tblUI" cellpadding="3">
            <tr>
                <td valign="top">
                    <div class="BoxHeader">
                        <%= RSc("tab_MyTags") %>
                    </div>
                    <div style="border: solid 1px #4B3D32; padding: 2px;">
                        <asp:TextBox ID="txt_TagName" runat="server" CssClass="TextBox" MaxLength="30"></asp:TextBox>
                        <asp:Button ID="btn_AddTag" runat="server" Text='<%# RS("add") %>' CssClass="inputbutton" OnClick="btn_AddTag_Click" />
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                <asp:GridView ID="gv_Tags" runat="server" AutoGenerateColumns="false" OnSelectedIndexChanged="gv_Tags_SelectedIndexChanged"
                                    DataKeyNames="TagID" CssClass="TypicalTable stripeTable" EmptyDataText='<%# RS("notags") %>'
                                    GridLines="None" CellSpacing="1" Width="100%" >
                                    <Columns>
                                        <asp:CommandField ButtonType="link" SelectText="Select" ShowSelectButton="True">
                                            <ItemStyle HorizontalAlign="Center" Width="20px" />
                                        </asp:CommandField>
                                        <asp:BoundField DataField="Sort" HeaderText="Order" />
                                        <asp:BoundField DataField="Name" HeaderText="Tag Name" />
                                    </Columns>
                                    <RowStyle CssClass="DataRowNormal highlight" />
                                    <HeaderStyle CssClass="TableHeaderRow" />
                                    <AlternatingRowStyle CssClass="DataRowAlternate highlight" />
                                </asp:GridView>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        &nbsp;&nbsp;
                        <asp:Label ID="lbl_Result" runat="server" Text='<%# RS("lbl_DelSuccess") %>' ForeColor="#00CC00" Visible="False" />
                        <asp:UpdateProgress ID="UpdateProgress2" runat="server">
                            <ProgressTemplate>
                                <img src='<%=RSic("busy_tinyred")%>' />
                                <%= RS("_loading")%>...
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                    </div>
                </td>
                <td valign="top">
                    <div class="BoxHeader">
                        <%= RS("h_TagDetails")%></div>
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <div style="border: solid 1px #4B3D32; padding: 2px;">
                                <asp:Label ID="lbl_NoSelectedTag" runat="server" Text='<%# RS("lbl_selectTag") %>' />
                                <asp:Panel ID="pnl_SelectedTag" runat="server" Visible="false">
                                    <asp:Panel ID="pnl_DisplayData" runat="server">
                                        <asp:Label ID="lbl_Name" runat="server" Font-Size="12pt"></asp:Label>
                                        &nbsp;&nbsp; &nbsp;
                                        <asp:LinkButton ID="btn_Edit" runat="server" 
                                            OnClick="btn_Edit_Click"  ><img src="https://static.realmofempires.com/images/item_rename.png"></img></asp:LinkButton>
                                        <asp:LinkButton ID="btn_Delete" runat="server" 
                                            OnClick="btn_Delete_Click" message='<%# RS("sureToDelTag")%>' OnClientClick='return confirm(this.getAttribute("message"))' ><img src="https://static.realmofempires.com/images/cancel.png"></img></asp:LinkButton>
                                        <br />
                                        <asp:Label ID="lbl_Desc" runat="server" Font-Italic="True"></asp:Label>
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
                                                    <asp:RequiredFieldValidator CssClass="Error" ID="RequiredFieldValidator1" runat="server"
                                                        ErrorMessage="*" ControlToValidate="txt_Name" ValidationGroup="vg_submit" Text='<%# RS("rfv_NoBlankName") %>' />
                                                    <asp:Label ID="lbl_UpdateResult" runat="server" Text='<%# RS("lbl_TagExists") %>'
                                                        CssClass="Error" Visible="False" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td valign="top" style="font-weight: bold;">
                                                    <%= RS("desc")%>
                                                    :
                                                </td>
                                                <td>
                                                    <asp:TextBox Width="250" ID="txt_Desc" runat="server" CssClass="TextBox" MaxLength="75" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td valign="top" style="font-weight: bold;">
                                                    <%= RS("h_Order")%>
                                                    :
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txt_Sort" runat="server" CssClass="TextBox" Width="75px"></asp:TextBox>
                                                    <asp:RangeValidator CssClass="Error" ID="RangeValidator1" runat="server" ErrorMessage="*"
                                                        ToolTip='<%# RSc("NumbersOnly") %>' ControlToValidate="txt_Sort" MaximumValue="30000"
                                                        MinimumValue="-30000" Type="Integer" ValidationGroup="vg_submit" Text='<%# RSc("invNumRange")%>' />
                                                    <br />
                                                    <i>
                                                        <%= RS("allowsOrder")%></i>
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
                                        <asp:Label ID="lbl_VillagesWithTags" runat="server"></asp:Label></div>
                                    <asp:GridView ID="gv_VillagesWithTags" runat="server" AutoGenerateColumns="False"
                                        ShowHeader="false" GridLines="None" CellSpacing="1" CssClass="TypicalTable stripeTable"
                                        DataKeyNames="VillageID" OnRowCommand="gv_VillagesWithTags_RowCommand">
                                        <Columns>
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <%#BindVillage(Container.DataItem) %>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="LinkButton1" runat="server" CommandName="Remove" 
                                                        CommandArgument='<%#Eval("VillageID") %>' ><img src="https://static.realmofempires.com/images/cancel.png"></img></asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                        <RowStyle CssClass="DataRowNormal highlight" />
                                        <HeaderStyle CssClass="TableHeaderRow" />
                                        <AlternatingRowStyle CssClass="DataRowAlternate highlight" />
                                        <EmptyDataTemplate>
                                            <%= RS("noVillWTag")%></EmptyDataTemplate>
                                    </asp:GridView>
                                    <hr size="1" noshade />
                                    <div class="BoxHeader">
                                        <asp:Label ID="lbl_VillagesWithoutTags" runat="server"></asp:Label></div>
                                    <asp:GridView ID="gv_VillagesWithoutTags" runat="server" AutoGenerateColumns="False"
                                        ShowHeader="false" GridLines="None" CellSpacing="1" CssClass="TypicalTable stripeTable"
                                        DataKeyNames="VillageID" OnRowCommand="gv_VillagesWithoutTags_RowCommand">
                                        <Columns>
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <%#BindVillage(Container.DataItem) %>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="LinkButton1" runat="server" CommandName="AddTag" CommandArgument='<%#Eval("VillageID") %>'><%#BindTagName(Container.DataItem)%></asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                        <RowStyle CssClass="DataRowNormal highlight" />
                                        <HeaderStyle CssClass="TableHeaderRow" />
                                        <AlternatingRowStyle CssClass="DataRowAlternate highlight" />
                                        <EmptyDataTemplate>
                                            <%= RS("noVillWOTag")%></EmptyDataTemplate>
                                    </asp:GridView>
                                </asp:Panel>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
