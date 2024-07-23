<%@ Page Language="C#" MasterPageFile="~/main.master" AutoEventWireup="true" CodeFile="Tags_VillageTags.aspx.cs"
    Inherits="Tags_VillageTags" %>

<%@ Register Src="Controls/TagHelp.ascx" TagName="TagHelp" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="Tabs">
        <ul>
            <li><a href="Tags.aspx">
                <%= RSc("tab_MyTags") %></a></li>
            <li class="selected"><a href="Tags_VillageTags.aspx">
                <%= RSc("tab_CurVillTags") %></a></li>
            <li><a href="Tags_Filters.aspx">
                <%= RSc("tab_MyFilters") %></a></li>
        </ul>
    </div>
    <div class="TabContent">
        <uc1:TagHelp ID="TagHelp1" runat="server" />
        <asp:Label ID="lblNotagsMsg" runat="server" Font-Size="1.1em" Text='<%# RS("noTagsDefined") %>'
            Font-Bold="True" Visible="False"></asp:Label>
        <asp:Panel ID="panelPF" runat="server" Font-Size="1.1em" Visible="False">
            <%= RSc("funcPremFeat") %>
            <asp:HyperLink ID="linkPF" runat="server" onclick="return popupUnlock(this);" Target="_blank"><%= RSc("unlockNow") %></asp:HyperLink>
        </asp:Panel>
        <asp:Panel ID="panelUI" runat="server">
            <b>
                <asp:Label ID="lbl_VillageName" runat="server" Font-Size="1.1em"></asp:Label></b>
            <asp:Label ID="lbl_VillageID" runat="server" Visible="false"></asp:Label>
            <table cellpadding="3">
                <tr>
                    <td valign="top">
                        <table>
                            <tr>
                                <td>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <div class="BoxHeader">
                                        <%= RS("tagsAppToVil")%></div>
                                    <asp:UpdateProgress ID="UpdateProgress2" runat="server">
                                        <ProgressTemplate>
                                            <img src='<%=RSic("busy_tinyred")%>' />
                                            <%= RS("_loading") %>...</ProgressTemplate>
                                    </asp:UpdateProgress>
                                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                        <ContentTemplate>
                                            <asp:GridView ID="gv_TagsWithVillage" runat="server" AutoGenerateColumns="False"
                                                CellSpacing="1" CssClass="TypicalTable" DataKeyNames="TagID" GridLines="None"
                                                OnRowCommand="gv_TagsWithVillage_RowCommand" ShowHeader="false">
                                                <Columns>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="LinkButton1" runat="server" CommandArgument='<%#Eval("TagID") %>'
                                                                CommandName="Remove"  ><img src="https://static.realmofempires.com/images/cancel.png"></img> </asp:LinkButton>
                                                            <%#Eval("Name") %>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                                <RowStyle CssClass="DataRowNormal" />
                                                <HeaderStyle CssClass="TableHeaderRow" />
                                                <AlternatingRowStyle CssClass="DataRowAlternate" />
                                                <EmptyDataTemplate>
                                                    <%= RS("none") %></EmptyDataTemplate>
                                            </asp:GridView>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <div class="BoxHeader">
                                        <%= RS("tagsNOTApp")%></div>
                                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                        <ContentTemplate>
                                            <asp:GridView ID="gv_TagsWithoutVillage" runat="server" AutoGenerateColumns="False"
                                                CellSpacing="1" CssClass="TypicalTable" DataKeyNames="TagID" GridLines="None"
                                                OnRowCommand="gv_TagsWithoutVillage_RowCommand" ShowHeader="false">
                                                <Columns>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="LinkButton1" runat="server" CommandArgument='<%#Eval("TagID") %>'
                                                                CommandName="AddTag"><%= RS("add")%></asp:LinkButton>
                                                            <%#Eval("Name") %>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                                <RowStyle CssClass="DataRowNormal" />
                                                <HeaderStyle CssClass="TableHeaderRow" />
                                                <AlternatingRowStyle CssClass="DataRowAlternate" />
                                                <EmptyDataTemplate>
                                                    <%= RS("none")%></EmptyDataTemplate>
                                            </asp:GridView>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </div>
</asp:Content>
