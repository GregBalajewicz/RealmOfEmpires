<%@ Page MasterPageFile="~/main.master" Language="C#" AutoEventWireup="true" CodeFile="ClanMembers.aspx.cs"
    Inherits="ClanMembers" %>

<%@ Register Src="Controls/ClanMenu.ascx" TagName="ClanMenu" TagPrefix="uc1" %>


<asp:Content ContentPlaceHolderID="HeadPlaceHolder" runat="server">

 <%if (isMobile) { %>
    <style>
        
        .clanPage .action
        {
            font-size: 12pt;
            margin: 8px 0px 8px 0px;
        }
        .clanPage .actions a
        {
            display:block;
            padding:5px;
        }
        .clanPage .tableaction
        {            
            padding: 0px 0px 10px 0px;
            display:block;
        }
        

        .clanPage, .clanPage .clanProfile
        {
            width: 100%;
        }
        .clanPage .column
        {
            display: inline;
            width: 100%;
        }
        .clanPage .pageSize
        {
            padding:10px;
        }

        .clanPage .members td
        {
            padding-top:10px;
            padding-bottom:10px;
        }

        .clanPage .actionCol,
        .clanPage .activityCol 
        {
            display:none;
        }



        a.MissingRole
        {
            color: rgb(82, 78, 78);
            text-decoration: none;
        }
        
        
        a.GotRole
        {
            color: #FFFFFF;
            text-decoration: none;
        }
        .MissingRole
        {
            color: rgb(82, 78, 78);
            text-decoration: none;
        }
        .GotRole
        {
            color: #ffffff;
            text-decoration: none;
        }

    </style>
    <script>

        page.load.push(function () {
            var viewIndexes = { 'infoCol': 0, 'activityCol': 1, 'actionCol': 2 };
            var viewNames = ['infoCol', 'activityCol', 'actionCol'];
            
            var clanMembers_ShowView = function (className) {
                if ($('.clanPage .' + className + ':visible').length === 0) {
                    $('.clanPage .activityCol, .clanPage .infoCol, .clanPage .actionCol').hide();
                    $('.clanPage .' + className).show();
                    $.cookie("clanmembers", className);
                }
            };

            <%if (isMobile && (FbgPlayer.Clan != null && FbgPlayer.Clan.ID == clanID) && (FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.Administrator) || FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.Owner)))
              { %>
            
            var radios = BDA.UI.Radio.init($('#viewType')
                , function (index) { clanMembers_ShowView(viewNames[index]); }
                , ['<%=RS("Basic")%>', '<%=RS("Activity")%>','<%=RS("Action")%>']
                , $.cookie("clanmembers") ? viewIndexes[$.cookie("clanmembers")] : 0);

            <% } else { %>
            var radios = BDA.UI.Radio.init($('#viewType')
                , function (index) { clanMembers_ShowView(viewNames[index]); }
                , ['<%=RS("Basic")%>', '<%=RS("Activity")%>', '<%=RS("Action")%>']
                , $.cookie("clanmembers") ? viewIndexes[$.cookie("clanmembers")] : 0);

            <% } %>
            clanMembers_ShowView($.cookie("clanmembers") ? $.cookie("clanmembers") : "infoCol");


        });

    </script>
        <%} else if(isD2) { %>
<style>
    /* Clan Overview */
    html,body {
        background:none !important;
    }
    body { font-family: Georgia, 'Playfair Display'; font-size:12px; }
    a, a:active, a:link { color: #d3b16f; }   
    a:hover { color: #d3b16f; }
    .tempMenu {
        background-color: rgba(88, 140, 173, 0.3);
        border-bottom:2px solid  #9d9781;
        padding: 4px;
        padding-bottom: 2px;
    }
    .tempMenu a { text-shadow:1px 1px 1px #000; }
    .BoxContent {
       background-color: rgba(88, 140, 173, 0.3);
    }
     .TypicalTable .DataRowNormal {
       background-color: rgba(88, 140, 173, 0.3);
    }
    .TypicalTable .DataRowAlternate {
       background-color: rgba(88, 140, 173, 0.2);
    }
    .TDContent {
        background-color: rgba(0, 0, 0, 0.7) !important;
        height: 100%;
        position: absolute;
        overflow: auto;
    }
     .clanPage {
        padding-left: 12px;
        padding-right: 12px;
        padding-bottom: 12px;
    }
    
    .TypicalTable TD {
        padding: 4px;
    }
    .Padding {
        padding: 4px;
    }
    .Sectionheader {
        color: #FFFFFF;
        font-weight: bold;
        padding: 4px;
        background-color: rgba(49, 81, 108, 0.7);
    }

    /* on this page, th is inside tableheaderrow which 
        messes with the styles so we have to manual
        style the th's instead */
    .TableHeaderRow th {
        font-size: 12px !important;
        font-weight: bold;
        background-color: rgba(49, 81, 108, 0.7) !important;
        padding: 4px;
    }
    .column {
        margin-bottom:12px;
    }

    .TextBox {
        /* so text area fits within width */
        -webkit-box-sizing: border-box;
        -moz-box-sizing: border-box;
        box-sizing: border-box;
        background-color: rgba(255,255,255,0.8);
        font-weight:normal;
        font-size: 12px;
        font-family: Georgia, 'Playfair Display';
        padding: 4px;
    }

    
    .inputbutton, .inputsubmit {
        font-weight:bold;
        font-size: 12px;
        font-family: Georgia, 'Playfair Display';
        margin-left: 0px;
        margin-top: 4px;
        padding: 4px 6px;
        color: #d3b16f;
        border-color: #efe1b9;
        background-color:rgba(25, 55, 74, 0.7);
        -moz-box-shadow: inset 0 0 5px 1px #000000;
        -webkit-box-shadow:  inset 0 0 5px 1px #000000;
        box-shadow:  inset 0 0 5px 1px #000000;
        height:initial;
    }

        .inputbutton:hover, .inputsubmit:hover {
        -moz-box-shadow: inset 0 0 5px 1px #efe1b9;
        -webkit-box-shadow: inset 0 0 5px 1px #efe1b9;
        box-shadow: inset 0 0 5px 1px #efe1b9;
    }



    /* Clan Public Profile */
    td.TableHeaderRow {
        font-size: 12px !important;
        font-weight: bold;
        background-color: rgba(49, 81, 108, 0.7) !important;
        padding: 4px;
    }

      div.TableHeaderRow {
    /*  margin-top:12px;*/  
    }

    .TableHeaderRow #ContentPlaceHolder1_lbl_ClanName {
         font-size: 12px !important;
         font-weight:bold;
    }

    #ContentPlaceHolder1_lblPageExplanationToMemebers {
        display:block;
    }

    #ContentPlaceHolder1_pnl_Profile td {
        padding:4px;
    }

    

    .clanPage table {
        /* remove spacing on outside of table */
        border-collapse: separate;
        border-spacing: 1px;
        margin: -1px;
        margin-top: 4px;
        border: 0 !important;
    }

    #ContentPlaceHolder1_lbl_PublicProfile {
        padding: 4px;
        display: block;
    }

        
           td.actionCol {
        text-align: left;
        }

        a.tableaction.MissingRole, 
        span.tableaction.MissingRole {
            color: rgb(113, 127, 138);
        }
         a.tableaction.GotRole {
             color:#d3b16f;
        }
    
    span.tableaction.GotRole {
         color:#ffffff;
    }
         a.tableaction,
        span.tableaction {
            margin-right: 6px;
        }

</style>
    <%}
      else { %>
    <style>
        .clanPage, .clanPage .clanProfile
        {
            width: 100%;
        }
        lblClanSizeLimit .clanPage .clanProfile
        {
            display: table;
            border-collapse: separate;
            border-spacing: 1px;
        }
        .column
        {
            display: table-cell;
            width: 50%;
            vertical-align: top;
            margin: 1px 1px 1px 1px;
        }
        
        
        a.MissingRole
        {
            color: #000000;
            text-decoration: none;
        }
        
        a.MissingRole:hover
        {
            color: #00C125;
            text-decoration: none;
        }
        
        a.GotRole:hover
        {
            color: #FA0625;
            text-decoration: none;
        }
        
        a.GotRole
        {
            color: #FFFFFF;
            text-decoration: none;
        }
        .MissingRole
        {
            color: #000000;
            text-decoration: none;
        }
        .GotRole
        {
            color: #ffffff;
            text-decoration: none;
        }

    </style>
    <%}%>


    
    <script type="text/javascript">
        $(function() {


            $('a.MissingRole').click(function(e) {
                e.preventDefault();
                gotrole($(this));
            });

            $('a.GotRole').click(function(e) {
                e.preventDefault();
                
                <%if (!isMobile) { %>
                if (confirm('<%= RS("confirmRoleRemoval")%>') == false) {
                    return false;
                }
                <%} %>

                gotrole($(this));
            });

            function gotrole(d) {
                ajax('ClanMembersAjax.aspx', {
                    '<%= CONSTS.QuerryString.Action %>': d.attr('<%= CONSTS.QuerryString.Action %>'),
                    '<%= CONSTS.QuerryString.PlayerID %>': d.attr('<%= CONSTS.QuerryString.PlayerID %>'),
                    '<%= CONSTS.QuerryString.RoleID %>': d.attr('<%= CONSTS.QuerryString.RoleID %>')
                },
                    function(obj) {
                        d.attr('action', obj.action);
                        d.addClass(obj.action == 1 ? 'MissingRole' : 'GotRole');
                        d.removeClass(obj.action == 0 ? 'MissingRole' : 'GotRole');
                    }
                );
            }

        });
    
    </script>
        
    <link type="text/json" rel="help" href="static/help/ga_ClanMembers.json.aspx" />
    
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
<%=Utils.GetIframePopupHeaderForNotPopupBrowser(RS("Members") , (isMobile & !IsiFramePopupsBrowser),"https://static.realmofempires.com/images/icons/M_Clan.png")%>
    <uc1:ClanMenu ID="ClanMenu1" runat="server" />
    <br />
<div class=clanPage>
    <table border="0" width="100%">
        <tr>
            <td>
               
            </td>
            <td  align="right" >
            <%if (!isMobile) { %>
                <asp:HyperLink ID="hlnk_MessageMembersTop" NavigateUrl="messages_create.aspx?action=3" runat="server"> <%= RS("msgAllMem") %></asp:HyperLink>
            <%} %>
            </td>
        </tr>
        <tr>
            <td colspan=2>
                <div id=viewType class="sfx2"></div>
                <asp:Label ID="lbl_Error" CssClass="Error" runat="server" Visible="False"></asp:Label>
                <asp:GridView ID="gvw_Members" DataKeyNames="PlayerID" runat="server" EmptyDataText="No Data"
                    AutoGenerateColumns="False" OnRowCommand="gvw_Members_RowCommand" GridLines="None"
                    CellSpacing="1" CssClass="TypicalTable help members" ShowHeader="true" AllowPaging="true"
                    OnPageIndexChanging="gvw_Members_PageIndexChanging">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton CssClass="tableaction sfx2" id="LinkButton1" runat="server" CommandName="Dismiss" message= '<%# RS("dismissOCC")%>'  OnClientClick='return confirm(this.getAttribute("message"));'> <%= RS("dismissLnkBtn") %></asp:LinkButton>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Right" CssClass="actionCol" />
                            <HeaderStyle CssClass="actionCol "/>
                        </asp:TemplateField>
                        <%--<asp:BoundField DataField="Name" HeaderText="Member Name" />--%>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                 <%= RS("lastActive") %>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <%# BindLastActive(Container.DataItem )%>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Right" CssClass=activityCol />
                            <HeaderStyle CssClass=activityCol />
                        </asp:TemplateField>
                        <asp:TemplateField  >
                        <HeaderTemplate>
                                 <%= RS("memberName") %>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:HyperLink runat="server" ID="LnkMemberName" class="sfx2" Text=' <%# BindPlayerName(Container.DataItem )%>' NavigateUrl=' <%# BindPlayerURL(Container.DataItem )%>'></asp:HyperLink> 
                                <asp:Image ID="imgAway" style="width: 22px; height: 22px; vertical-align: middle;" 
                                    ImageUrl="<%# BindAwayStatus(Container.DataItem).icon %>" runat="server" visible= '<%# BindAwayStatus(Container.DataItem).away %>'/>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                 <%=  RS("rank")%>
                            </HeaderTemplate>
                            <ItemStyle HorizontalAlign="Right" CssClass=infoCol />
                            <HeaderStyle CssClass=infoCol />
                            <ItemTemplate>
                                <%#Container.DataItemIndex +1 %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                 <%=  RS("points")%>
                            </HeaderTemplate>
                            <ItemStyle HorizontalAlign="Right" CssClass=infoCol />
                            <HeaderStyle CssClass=infoCol />
                            <ItemTemplate>
                                <%#FormatNumber((int) Eval("VillagePoints")) %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                 <%= RS("numVillages")%>
                            </HeaderTemplate>
                            <ItemStyle HorizontalAlign="Right" CssClass=infoCol />
                            <HeaderStyle CssClass=infoCol />
                            <ItemTemplate>
                                <%#Eval("VillagesCount") %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                 <%= RS("roles") %>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <%# BindRole(Container.DataItem )%>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Right" CssClass="actionCol sfx2"/>
                            <HeaderStyle CssClass="actionCol sfx2"/>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                 <%= RS("steward") %>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <%# BindSteward(Container.DataItem )%>
                           </ItemTemplate>
                            <ItemStyle HorizontalAlign="Right" CssClass=activityCol />
                            <HeaderStyle CssClass=activityCol  />
                        </asp:TemplateField>
                    </Columns>
                    <RowStyle CssClass="DataRowNormal" />
                    <HeaderStyle CssClass="TableHeaderRow" />
                    <AlternatingRowStyle CssClass="DataRowAlternate" />
                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <PagerStyle HorizontalAlign="Center" CssClass="ListPager" />
                    <PagerSettings  Mode="NumericFirstLast" FirstPageImageUrl="https://static.realmofempires.com/images/LeftArrow.png" 
                        FirstPageText="First Page" LastPageImageUrl="https://static.realmofempires.com/images/RightArrow.png" LastPageText= "Last Page" />
                    
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td colspan="2" align="right" runat=server id=trAdminMsgAllMembers class=actions>
                <%if (!isMobile) { %>| 
                <asp:HyperLink class="sfx2" ID="hlnk_MessageMembersDown" NavigateUrl="messages_create.aspx?Action=3" runat="server" > <%= RS("msgAllMem")%></asp:HyperLink>
                <asp:HyperLink ID="hlnk_EmailMembers" NavigateUrl="ClanEmailMembers.aspx?messageall=1" runat="server"> <i>Email</i> All Members (New)</asp:HyperLink>
                    <%} %>
            </td>
        </tr>
        <tr><td>
         <asp:Panel runat="server" ID="panelRows">
                    <%= RS("numMemDisp") %>
                    <asp:DropDownList ID="ddlPage" runat="server" AutoPostBack="True" CssClass="DropDown"
                        OnSelectedIndexChanged="ddlPage_SelectedIndexChanged">
                        <asp:ListItem Value="25">25</asp:ListItem>
                        <asp:ListItem Value="50">50</asp:ListItem>
                        <asp:ListItem Value="100">100</asp:ListItem>
                        <asp:ListItem Value="1000">all</asp:ListItem>
                    </asp:DropDownList>
                </asp:Panel>
        </td></tr>
    
    </table>
    </div>
</asp:Content>
