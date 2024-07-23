<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Tutorial.ascx.cs" Inherits="Controls_Tutorial" %>



    <asp:Panel ID="panelTutorial" runat="server" CssClass="Tutorial">
    <style> a.NextLinkHide { display:none;} </style>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
<ContentTemplate>
    
    <table id="Table1" runat="server" cellpadding="0" cellspacing="0" width="100%" style="border:solid 1px #2F4A00;">
        <tr>
            <td class="TutorialHeader" align="center"> 
                <img runat="server" id="imgAdvisor" style="WIDTH: 70px; position:absolute;top:-5px;left:-10px;padding-right:5px;" src="https://static.realmofempires.com/images/tutorial_advisor2.png"><span style="margin-left:80px;"><%=RS("Tutorial") %> </span>
            </td>
            <td class="TutorialHeader" align="right">
                <asp:LinkButton  CssClass="tutCloseLink" ID="LinkButton1" runat="server" OnClick="LinkButton1_Click"
                    OnClientClick="return confirm('My Liege!! The people depend on you! I beg you, finish this tutorial as it will help you greatly understand your tasks ahead. Are sure you want to quit?')" ><img border="0" src="https://static.realmofempires.com/images/cancel.png"/></asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td colspan="2" class="TutorialBody">
                <asp:Label ID="lblText" runat="server"></asp:Label><br />
                <br />
                <center>
                    &nbsp;<asp:LinkButton ID="linkNext" runat="server" CssClass="StandoutLink" style="font-size:12pt;" OnClick="linkNext_Click"><%=RS("NEXT") %></asp:LinkButton>
                </center>
            </td>
        </tr>
        <tr>
            <td class="TutorialFooter" nowrap width="99%">
                <asp:Panel ID="panelProgressBarContainer" runat="server" Width="175px" style="background-color:#477100;border:solid 1px #2F4A00;">
                    <asp:Panel ID="panelProgressBar" runat="server" Width="50%" style="background-color:#6FAF00;">
                        <asp:Label ID="lblPercentage" runat="server" Text="Label"></asp:Label></asp:Panel>
                </asp:Panel>
            </td>
            <td class="TutorialFooter">
                <asp:Panel runat=server ID=panelTutNav Visible=false>
                <asp:Menu ID="Menu4" runat="server" DynamicHorizontalOffset="5" Orientation="Horizontal"
                    StaticSubMenuIndent="10px" Font-Bold="false"  RenderingMode="Table">
                    <StaticMenuItemStyle ItemSpacing="0px" HorizontalPadding="1px" VerticalPadding="2px"
                        ForeColor="white" />
                    <StaticHoverStyle />
                    <DynamicMenuItemStyle ForeColor="White" />
                    <DynamicMenuStyle HorizontalPadding="2" VerticalPadding="2" BackColor="#518304" />
                    <Items>
                        <asp:MenuItem PopOutImageUrl="https://static.realmofempires.com/images/transpSquare.gif" Text='<%=RS("Back") %>' ToolTip=""
                            Value="Manage Troops" NavigateUrl="~/TutorialGoBack.aspx?s=0">
                            <asp:MenuItem ImageUrl="https://static.realmofempires.com/images/menuItemSeperator.png" NavigateUrl="~/TutorialGoBack.aspx?s=1"
                                Text='<%=RS("Section1VillageOverview") %>' />
                            <asp:MenuItem ImageUrl="https://static.realmofempires.com/images/menuItemSeperator.png" NavigateUrl="~/TutorialGoBack.aspx?s=2"
                                Text='<%=RS("Section2BuildingUpgrades") %>' />
                            <asp:MenuItem ImageUrl="https://static.realmofempires.com/images/menuItemSeperator.png" NavigateUrl="~/TutorialGoBack.aspx?s=3"
                                Text='<%=RS("Section3Map") %>' />
                            <asp:MenuItem ImageUrl="https://static.realmofempires.com/images/menuItemSeperator.png" NavigateUrl="~/TutorialGoBack.aspx?s=4"
                                Text='<%=RS("Section4Troops") %>' />
                            <asp:MenuItem ImageUrl="https://static.realmofempires.com/images/menuItemSeperator.png" NavigateUrl="~/TutorialGoBack.aspx?s=0"
                                Text='<%=RS("FromBeginning") %>' />
                        </asp:MenuItem>
                    </Items>
                </asp:Menu>
                </asp:Panel>
            </td>
        </tr>
    </table>
    </ContentTemplate>
</asp:UpdatePanel>

    </asp:Panel>
    
