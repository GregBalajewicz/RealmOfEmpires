<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/main.master" CodeFile="BattleSimulator.aspx.cs"
    Inherits="BattleSimulator" ValidateRequest="false" %>

<%@ Register Src="Controls/TroopsMenu.ascx" TagName="TroopsMenu" TagPrefix="uc1" %>
<asp:Content ID="header" ContentPlaceHolderID="HeadPlaceHolder" runat="server">
    <link type="text/json" rel="help" href="static/help/e_TransportSilver.json.aspx" />
    <link type="text/json" rel="help" href="static/help/q_BattleSimulator.json.aspx" />

    <script src="script/BattleSim.js.aspx?1" type="text/javascript"></script>
    
 <%if (isMobile) { %>
    <style>

        .battleSimulator {
            /*this was added as manual paddination, to solve the droid4.4 keyboard cover issue, 
                using paddinator2000 was just too cumbersome for this page*/
            padding-bottom:250px; 
        }
       .battleSimulator .action 
       {
            display:table-cell;
            padding: 7px;
       }

       .battleSimulator .jsFakeSelect
       {
        font-size:12pt;
       }

       .battleSimulator .battleResultColumn 
       {
            display:none;
       }


    </style>
    <script>
       
        function popupHandicapCalc(link)
        {
            
            var url = $(link).attr('href');
            var p = $(link).position();
            //var w = popupWindow2(url, 'unlock', 450, 350);
            
            var height = ROE.Frame.CONSTS.popupDefaultHeight;
            var width = ROE.Frame.CONSTS.popupDefaultWidth;
            var x = ROE.Frame.CONSTS.popupDefaultX;
            var y = ROE.Frame.CONSTS.popupDefaultY;
            console.log(x + ", " + y);
            if (isD2) {

            }

            return !popupIFrame(url, 'HandicapCalc', "<%=RS("HandicapCalculator")%>",  height, width, x, y);
        }


        function popupDesertionCalc(link)
        {
            var url = $(link).attr('href');
            var p = $(link).position();
            //var w = popupWindow2(url, 'unlock', 450, 350);       
    
            var height = ROE.Frame.CONSTS.popupDefaultHeight;
            var width = ROE.Frame.CONSTS.popupDefaultWidth;
            var x = ROE.Frame.CONSTS.popupDefaultX;
            var y = ROE.Frame.CONSTS.popupDefaultY;



            return !popupIFrame(url, "des", "<%=RS("DistanceCalculator")%>",  height, width, x, y);

        }

        function CustomOnLoad() {
            var textBox;
            var textBoxes = document.getElementsByClassName('TextBox');
            for (i = 0; i < textBoxes.length; i++) {
                textBoxes[i].setAttribute('type', 'number');
            }
       

            $('.inputbutton').click(function () {
                var textBox;
                var textBoxes = document.getElementsByClassName('TextBox');
                for (i = 0; i < textBoxes.length; i++) {
                    textBoxes[i].setAttribute('type', 'text');
                }
            })

        }


    </script>

       <% } else if(isD2) { %>

    <script>
        function popupHandicapCalc(link) {
            var url = $(link).attr('href');
            var p = $(link).position();
            //var w = popupWindow2(url, 'unlock', 450, 350);

            //return !popupIFrame(url, 'HandicapCalc', "<%=RS("HandicapCalculator")%>", 210, 550, p.left - 200, p.top - 300);
            return !popupIFrame(url, 'HandicapCalc', "<%=RS("HandicapCalculator")%>", 260, 550, 0, 0);
        }


        function popupDesertionCalc(link) {
            var url = $(link).attr('href');
            var p = $(link).position();
            //var w = popupWindow2(url, 'unlock', 450, 350);

            //return !popupIFrame(url, 'DistanceCalc', "<%=RS("DistanceCalculator")%>", 210, 550, p.left - 200, p.top - 300);
            return !popupIFrame(url, 'DistanceCalc', "<%=RS("DistanceCalculator")%>", 260, 550, 0, 0);
        }
    </script>
    <style>
         html {
        background-color:#000000;
        background: url('https://static.realmofempires.com/images/backgrounds/M_BG_Tower.jpg') no-repeat center center fixed; 
        -webkit-background-size: cover;
        -moz-background-size: cover;
        -o-background-size: cover;
        background-size: cover;
    }
    body { font-family: Georgia, 'Playfair Display'; font-size:12px; }
    a, a:active, a:link, a:visited { color: #d3b16f; }   
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
       background-color: rgba(88, 140, 173, 0.1);
    }
    .TypicalTable .DataRowAlternate {
       background:none;
    }
    .TDContent {
        background-color: rgba(6, 20, 31, 0.9) !important;
        height: 100%;
        position: absolute;
        overflow: auto;
    }

    .TypicalTable TD {
        padding: 4px;
        font-size:12px !important;
        font-weight:normal !important;
    }
    .Padding {
        padding: 4px;
    }

        .Sectionheader {
             color: #FFFFFF;
        font-weight: bold;
          background-color: rgba(49, 81, 108, 0.7);
        }
    .Sectionheader td {
       
        padding: 4px;
      
    }


     .TableHeaderRow {
        font-size: 12px !important;
        font-weight: normal;
        background-color: rgba(49, 81, 108, 0.7) !important;
        padding: 4px;
    }

        .stats {
            padding:12px;
        }

        .stats table {
            width:100%;
        }
        .stats table .TypicalTable {
            width:initial;
        }

        tr.selected {
            background-color: #3D372A !important;
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

        table.stripeTable {
            margin-top:0px;
            width: 100%;
        }

        .stripeTableHover td {
            background-color:rgba(88, 140, 173, 0.3);
        }

        .d2BattleTable {
            width:100%;
        }

        .highlightElement td {
            border:none !important;
        }

        #highlight {
            border: none;
        }


        #ContentPlaceHolder1_CopyDefRem {
            padding-left:8px;
        }

        .d2AdvancedTable {
           
        }
            .d2AdvancedTable td {
 padding-left:8px;
            }

        .d2PFList {
            padding-left:10px;
            font-size:12px !important;
            padding-bottom: 8px;
        
        }
        .jsFakeSelect {
            margin-left:10px;
        }
        #ContentPlaceHolder1_btn_Simulate {
            margin-left:10px;
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
            width: 50px !important;
        }

        .IFrameDivTitle {

            background-color:rgba(49, 81, 108, 1.0) !important;
            /*width: initial;
            padding: 6px;*/
        }

            .IFrameDivTitle span.title {
                color:#fff !important;
            }

        .iFrameDiv {
        border: solid 6px rgb(0, 0, 0) !important;
        border-radius: 6px;
        }

        .DropDown {
           border-color: #4B3D32;
            border-style: solid;
            border-width: 1px;
            color: #000000;
            box-sizing: border-box;
            background-color: rgba(255,255,255,0.8);
            font-weight: normal;
            font-family: Georgia, 'Playfair Display';
            font-size: 12px !important;
            padding: 4px;
        }
      
        #imgIframeClose {
            padding-right: 2px;
            padding-top: 2px;
        }

        #ContentPlaceHolder1_GridView1_panelFolders {
             background-color: rgba(8, 18, 27, 1.0) !important;
        }

        .d2FirstBR {
            display:none;
        }

    </style>
   

    <%} else { %>
    <style>
    </style>
    <%}%>


    


</asp:Content>


<asp:Content ID="c1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <%=Utils.GetIframePopupHeaderForNotPopupBrowser("Battle Simulator" , (isMobile && !IsiFramePopupsBrowser),"https://static.realmofempires.com/images/icons/M_BattleSim.png")%>
    <style>
        a.folderPreDef
        {
            background: url('https://static.realmofempires.com/images/ThreadHot_sml.gif') 2px center no-repeat;
            text-decoration: none;
            padding: 2px 4px 2px 16px;
            font-weight: bold;
        }
        a.folderPreDef:hover
        {
            text-decoration: underline;
        }
        a.folder
        {
            background: url('https://static.realmofempires.com/images/folder_sml.gif') 2px center no-repeat;
            text-decoration: none;
            padding: 2px 4px 2px 16px;
        }
        a.folder:hover
        {
            text-decoration: underline;
        }
        a.Disabled
        {
            background: url('https://static.realmofempires.com/images/LockedFeature.sml.png') 2px center no-repeat;
            text-decoration: none;
            padding: 2px 4px 2px 16px;
            font-weight: bold;
        }
        a.function
        {
            background: url('https://static.realmofempires.com/images/rightarrow0.png') 2px center no-repeat;
            text-decoration: none;
            padding: 2px 4px 2px 16px;
            font-weight: bold;
        }
        a.function:hover
        {
            background: url('https://static.realmofempires.com/images/rightarrow.png') 2px center no-repeat;
            text-decoration: underline;
        }


    </style>
    
    <% if(!isD2) { %><uc1:TroopsMenu ID="TroopsMenu1" runat="server" /><% } %>
<div class=battleSimulator>
    <% if(!isMobile && !isD2) { %>
    <asp:HyperLink ID="linkBackToReport" Visible="false" runat="server"><%=RS ("linkBackToReport") %></asp:HyperLink>
    <% } %>
    <br class="d2FirstBR" />
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
        <table class="d2BattleTable">
        <tr><td>
            <asp:GridView ID="GridView1" runat="server" CssClass="TypicalTable stripeTable" CellSpacing="1"
                GridLines="None" AutoGenerateColumns="False" OnRowDataBound="GridView1_RowDataBound">
                <Columns>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Panel ID="pnl_AvialablePF" runat="server">
                                <div class="jsFakeSelect">
                                    <asp:HyperLink Text='<%# isMobile ? "" : RS ("InsertTroops") %>' runat="server" ID="linkSelectedFolder" CssClass="jsMaster jsTriger"
                                        Style="background: url('https://static.realmofempires.com/images/downarrow.png') top right no-repeat;
                                        padding: 0px 15px 0px 0px;" />
                                    <asp:Panel ID="panelFolders" runat="server" class="jsOptions ui_menu FT" Style="border: 1px solid rgb(30,30,30);
                                        background-color: rgb(75, 61, 48);">
                                        <asp:LinkButton ID="lnk_PopAttackAll" CssClass="function" Style="font-weight: normal;"
                                            runat="server" OnClick="lnk_PopAttackAll_Click"><%=RS ("PopAttackAll")  %></asp:LinkButton>
                                        <asp:LinkButton ID="lnk_PopAttackCurrent" CssClass="function" Style="font-weight: normal;"
                                            runat="server" OnClick="lnk_PopAttackCurrent_Click"><%=RS ("PopAttackCurrent") %></asp:LinkButton>
                                        <asp:LinkButton ID="lnk_PopDefendAll" CssClass="function" Style="font-weight: normal;"
                                            runat="server" OnClick="lnk_PopDefendAll_Click"><%=RS ("PopDefendAll") %></asp:LinkButton>
                                        <asp:LinkButton ID="lnk_PopDefendCurrent" CssClass="function" Style="font-weight: normal;"
                                            runat="server" OnClick="lnk_PopDefendCurrent_Click"><%=RS ("PopDefendCurrent") %>
                                        </asp:LinkButton>
                                    </asp:Panel>
                                </div>
                            </asp:Panel>
                            <asp:Panel ID="pnl_UNAvialablePF" runat="server">
                                <div class="jsFakeSelect">
                                    <asp:HyperLink Text='<%#RS ("InsertTroops") %>' runat="server" ID="HyperLink1" CssClass="jsMaster jsTriger"
                                        Style="background: url('https://static.realmofempires.com/images/downarrow.png') top right no-repeat;
                                        padding: 0px 15px 0px 0px;" />
                                    <asp:Panel ID="panel2" runat="server" class="jsOptions ui_menu FT" Style="border: 1px solid rgb(30,30,30);
                                        background-color: rgb(75, 61, 48);">
                                        <asp:HyperLink ID="HyperLink2" runat="server" CssClass="Disabled" Style="font-weight: normal;"
                                            NavigateUrl='<%#NavigationHelper.PFOffMessage(Fbg.Bll.CONSTS.PFs.BattleSimImprovements) %>'
                                            onclick="return popupUnlock(this)"><%=RS ("PopAttackAll") %></asp:HyperLink>
                                        <asp:HyperLink ID="HyperLink3" runat="server" CssClass="Disabled" Style="font-weight: normal;"
                                            NavigateUrl='<%#NavigationHelper.PFOffMessage(Fbg.Bll.CONSTS.PFs.BattleSimImprovements) %>'
                                            onclick="return popupUnlock(this)"><%=RS ("PopAttackCurrent") %></asp:HyperLink>
                                        <asp:HyperLink ID="HyperLink4" runat="server" CssClass="Disabled" Style="font-weight: normal;"
                                            NavigateUrl='<%#NavigationHelper.PFOffMessage(Fbg.Bll.CONSTS.PFs.BattleSimImprovements) %>'
                                            onclick="return popupUnlock(this)"><%=RS ("PopDefendAll") %></asp:HyperLink>
                                        <asp:HyperLink ID="HyperLink5" runat="server" CssClass="Disabled" Style="font-weight: normal;"
                                            NavigateUrl='<%#NavigationHelper.PFOffMessage(Fbg.Bll.CONSTS.PFs.BattleSimImprovements) %>'
                                            onclick="return popupUnlock(this)"><%=RS ("PopDefendCurrent") %></asp:HyperLink>
                                    </asp:Panel>
                                </div>
                            </asp:Panel>
                        </HeaderTemplate>
                        <ItemStyle Wrap="false" />
                        <ItemTemplate>
                            <asp:HyperLink ID="lnk_UnitImage" runat="server" CssClass="StandoutLink"></asp:HyperLink>
                            <asp:HyperLink ID="lnk_UnitName" runat="server" CssClass="StandoutLink"></asp:HyperLink>
                            <asp:Panel ID="pnl_Target" runat="server" CssClass="<% if(!isD2 && !isMobile) { %>help<% } %>" rel="q_TargetLevel">
                                <%=RS ("Target") %>
                                <asp:DropDownList ID="cmb_Buildings" runat="server" AutoPostBack="true" CssClass="DropDown"
                                    DataTextField="Name" DataValueField="ID" OnSelectedIndexChanged="cmb_Buildings_SelectedIndexChanged">
                                </asp:DropDownList>
                                <asp:Panel runat="server" ID="panelTargetLevel">
                                    <%=RS ("TargetLevel") %><asp:TextBox ID="txt_TargetBuildingLevel" runat="server" MaxLength="2"
                                        Width="20"  CssClass="TextBox"></asp:TextBox><asp:RangeValidator CssClass="Error"
                                            ID="rv_TargetBuildingLevel" runat="server" ControlToValidate="txt_TargetBuildingLevel"
                                            ErrorMessage="*" ToolTip="only numbers allowed" MaximumValue="2000000000" MinimumValue="0"
                                            Type="Integer" Display="Dynamic"></asp:RangeValidator><asp:RangeValidator CssClass="Error"
                                                ID="rv_TargetBuildingLevelMaxLevel" runat="server" ControlToValidate="txt_TargetBuildingLevel"
                                                ErrorMessage="*" MaximumValue="2000000000" MinimumValue="0" Type="Integer" Display="Dynamic"></asp:RangeValidator></asp:Panel>
                            </asp:Panel>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField Visible="false">
                        <HeaderTemplate>
                            UnitID</HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lbl_UnitID" runat="server" Text='<%#Eval("ID") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-Wrap="false">
                        <HeaderStyle CssClass="TableHeaderRow" />
                        <HeaderTemplate>
                            <%=RS ("Attacking") %><%=RS ("Troops") %>
                            </HeaderTemplate>
                        <ItemStyle Wrap="false" />
                        <ItemTemplate>
                            <asp:TextBox ID="txt_AttackUnitAmount" runat="server" Width="40"  CssClass="TextBox"></asp:TextBox>
                            <asp:RangeValidator CssClass="Error" ID="rv_AttackUnitAmount" runat="server" ControlToValidate="txt_AttackUnitAmount"
                                ErrorMessage="*" ToolTip='<%#RS ("NumbersOnly") %>' MaximumValue="2000000000" MinimumValue="0"
                                Type="Integer" Display="Dynamic"></asp:RangeValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderStyle CssClass="TableHeaderRow battleResultColumn" ForeColor="LightSalmon" />
                        <ItemStyle CssClass="battleResultColumn" />
                        <HeaderTemplate>
                            <%=RS ("Attacking") %>
                            <%=RS ("Troops") %>
                            <%=RS ("Killed") %></HeaderTemplate>
                        <ItemStyle HorizontalAlign="Right" ForeColor="LightSalmon" />
                        <ItemTemplate>
                            <asp:Label ID="lbl_AttackingLost" runat="server" Text=""></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderStyle CssClass="TableHeaderRow battleResultColumn" ForeColor="LightSalmon" />
                        <ItemStyle CssClass="battleResultColumn" />
                        <HeaderTemplate>
                            <%=RS ("Attacking") %>
                            <%=RS ("Troops") %>
                            <%=RS ("Deserted") %></HeaderTemplate>
                        <ItemStyle HorizontalAlign="Right" ForeColor="LightSalmon" />
                        <ItemTemplate>
                            <asp:Label ID="lbl_AttackingDeserted" runat="server" Text=""></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderStyle CssClass="TableHeaderRow " ForeColor="lightgreen" />
                        <ItemStyle CssClass="" />
                        <HeaderTemplate>
                            <%=RS ("Attacking") %>
                            <%=RS ("Troops") %>
                            <%=RS ("Remaining") %></HeaderTemplate>
                        <ItemStyle HorizontalAlign="Right" Font-Bold="true" Font-Size="Larger" ForeColor="lightgreen" />
                        <ItemTemplate>
                            <asp:Label ID="lbl_AttackingRemaining" runat="server" Text=""></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderStyle CssClass="TableHeaderRow" />
                        <HeaderTemplate>
                            <%=RS ("Defending") %><%=RS ("Troops") %>
                            </HeaderTemplate>
                        <ItemStyle Wrap="false" />
                        <ItemTemplate>
                            <asp:TextBox ID="txt_DefendUnitAmount" runat="server" Width="40"  CssClass="TextBox"></asp:TextBox>
                            <asp:RangeValidator CssClass="Error" ID="rv_DefendUnitAmount" runat="server" ControlToValidate="txt_DefendUnitAmount"
                                ErrorMessage="*" ToolTip='<%#RS ("NumbersOnly") %>' MaximumValue="2000000000" MinimumValue="0"
                                Type="Integer" Display="Dynamic"></asp:RangeValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderStyle CssClass="TableHeaderRow battleResultColumn" ForeColor="LightSalmon" />
                        <HeaderTemplate>
                            <%=RS ("Defending") %>
                            <%=RS ("Troops") %>
                            <%=RS ("Killed") %></HeaderTemplate>
                        <ItemStyle HorizontalAlign="Right" ForeColor="LightSalmon" CssClass="battleResultColumn"/>
                        <ItemTemplate>
                            <asp:Label ID="lbl_DefendingLost" runat="server" Text=""></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderStyle CssClass="TableHeaderRow " ForeColor="lightgreen" />
                        <ItemStyle CssClass="battleResultColumn" />
                        <HeaderTemplate>
                            <%=RS ("Defending") %>
                            <%=RS ("Troops") %>
                            <%=RS ("Remaining") %></HeaderTemplate>
                        <ItemStyle HorizontalAlign="Right" Font-Bold="true" Font-Size="Larger" ForeColor="lightgreen" CssClass=""  />
                        <ItemTemplate>
                            <asp:Label ID="lbl_DefendingRemaining" runat="server" Text=""></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <RowStyle CssClass="DataRowNormal highlight" />
                <HeaderStyle CssClass="TableHeaderRow highlight" />
                <AlternatingRowStyle CssClass="DataRowAlternate highlight" />
            </asp:GridView>
            </td>
            <td class="d2RemoveRightTD" valign=bottom>
            </td>
            </tr>
            </table>
            <a runat=server id=CopyDefRem href="#" onclick="CopyRemDef(this);return false;" visible=false class=action  >Copy <I>Defending troops remaining</I> to <I>Defending Troops</I></a>

            <table class="stripeTable d2AdvancedTable">
                
                <tr class="<% if(!isD2 && !isMobile) { %>help<% } %> highlight" rel="q_DefenderWallLevel">
                    
                    <td>
                        <%=RS ("WallLevel") %>
                    </td>
                    <td>
                        <asp:TextBox ID="txt_DefenderWallLevel" runat="server" Width="30" CssClass="TextBox"
                            MaxLength="2"></asp:TextBox>
                        <asp:RangeValidator CssClass="Error" ID="rv_WallLevel" runat="server" ControlToValidate="txt_DefenderWallLevel"
                            ErrorMessage="*" ToolTip='<%#RS ("NumbersOnly") %>' MaximumValue="2000000000" MinimumValue="0"
                            Type="Integer" Display="Dynamic"></asp:RangeValidator>
                        <asp:RangeValidator CssClass="Error" ID="rv_WallLevelMaxLevel" runat="server" ControlToValidate="txt_DefenderWallLevel"
                            ErrorMessage="*" ToolTip='<%#RS ("MaxLevel") %>' MaximumValue="10" MinimumValue="0"
                            Type="Integer" Display="Dynamic"></asp:RangeValidator>
                        <asp:Label ID="lbl_WallAfterAttack" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr class="<% if(!isD2 && !isMobile) { %>help<% } %> highlight" rel="q_DefenderTowerLevel">
                    <td>
                        <%=RS ("TowerLevel") %>
                    </td>
                    <td>
                        <asp:TextBox ID="txt_DefenderTowerLevel" runat="server" Width="30" CssClass="TextBox"
                            MaxLength="2"></asp:TextBox>
                        <asp:RangeValidator CssClass="Error" ID="rv_TowerLevel" runat="server" ControlToValidate="txt_DefenderTowerLevel"
                            ErrorMessage="*" ToolTip='<%#RS ("NumbersOnly") %>' MaximumValue="2000000000" MinimumValue="0"
                            Type="Integer" Display="Dynamic"></asp:RangeValidator>
                        <asp:RangeValidator CssClass="Error" ID="rv_TowerLevelMaxLevel" runat="server" ControlToValidate="txt_DefenderTowerLevel"
                            ErrorMessage="*" ToolTip='<%#RS ("MaxLevel") %>' MaximumValue="10" MinimumValue="0"
                            Type="Integer" Display="Dynamic"></asp:RangeValidator>
                        <asp:Label ID="lbl_TowerAfterAttack" runat="server"></asp:Label>
                    </td>
                </tr>
                  <tr runat="server" id="trShowAdvancedButton" class="<% if(!isD2 && !isMobile) { %>help<% } %> highlight" rel="q_PFAtt">
                    <td colspan="2">
                        <asp:LinkButton ID="btnShowAdvanced" runat="server" onclick="LinkButton1_Click"><%=RS("AdvancedBattleParameters") %></asp:LinkButton>
                    </td>
                </tr>
                <tr runat="server" id="trPFAtt" class="<% if(!isD2 && !isMobile) { %>help<% } %> highlight" rel="q_PFAtt">
                    <td>
                        <%=RS ("BloodLust") %>
                    </td>
                    <td>
                        <asp:CheckBox ID="chk_AttackBouns" runat="server" />
                        <%=RS ("AttackBonus")%>
                    </td>
                </tr>
                <tr runat="server" id="trPFDef" class="<% if(!isD2 && !isMobile) { %>help<% } %> highlight" rel="q_PFDef">
                    <td>
                       <%=RS ("BraverySpell") %> 
                    </td>
                    <td>
                        <asp:CheckBox ID="chk_DefendBouns" runat="server" />
                        <%=RS ("DefenceBonus") %>
                    </td>
                </tr>
                <tr runat="server" id="trBonusVillage" class="<% if(!isD2 && !isMobile) { %>help<% } %> highlight" rel="q_PFDef">
                    <td>
                        Defenders bonus/penalty from village type
                    </td>
                    <td>
                      
                        <asp:DropDownList ID="ddBonusVillage" runat="server"></asp:DropDownList>

                    </td>
                </tr>
                <tr  runat="server" id="trResearchBonusVillageBonus" class="<% if(!isD2 && !isMobile) { %>help<% } %> highlight" rel="q_RB">
                    <td>
                         Defender's Village Defence Research Bonus
                    </td>
                    <td>
                        <asp:TextBox ID="txtResearchDefBonus_VillageDef" runat="server" CssClass="TextBox" Width="30"></asp:TextBox>%
                        <asp:RangeValidator CssClass="Error" ID="RangeValidator4" runat="server" ControlToValidate="txtResearchDefBonus_VillageDef" Display="Dynamic" ErrorMessage="*" MaximumValue="100" MinimumValue="0" ToolTip='Valid numbers are between 0 and 100' Type="Integer"></asp:RangeValidator>
                    </td>
                </tr>
                <tr  runat="server" id="trResearchBonus" class="<% if(!isD2 && !isMobile) { %>help<% } %> highlight" rel="q_RB">
                    <td>
                        Defender's Wall Research Defence Bonus                       
                    </td>
                    <td>
                        <asp:TextBox ID="txtResearchDefBonus" runat="server" CssClass="TextBox" Width="30"></asp:TextBox>%
                        <asp:RangeValidator CssClass="Error" ID="RangeValidator3" runat="server" ControlToValidate="txtResearchDefBonus" Display="Dynamic" ErrorMessage="*" MaximumValue="100" MinimumValue="0" ToolTip='Valid numbers are between 0 and 100' Type="Integer"></asp:RangeValidator>
                    </td>
                </tr>
                 <tr  runat="server" id="trDefensivePenalty" class="<% if(!isD2 && !isMobile) { %>help<% } %> highlight" rel="q_RB">
                    <td>
                         Defender's Village Defensive Bonus/Penalty
                    </td>
                    <td>
                        <asp:TextBox ID="txtDefendersDefensivePenalty" runat="server" CssClass="TextBox" Width="30" ></asp:TextBox>%
                        <asp:RangeValidator CssClass="Error" ID="RangeValidator6" runat="server" ControlToValidate="txtDefendersDefensivePenalty" Display="Dynamic" ErrorMessage="*" MaximumValue="0" MinimumValue="-100" ToolTip='Valid numbers are between -100 and 0' Type="Integer"></asp:RangeValidator>
                    </td>
                </tr>
                <tr  runat="server" id="trAttackerResearchBonus" class="<% if(!isD2 && !isMobile) { %>help<% } %> highlight" rel="q_RB">
                    <td>
                         Attacker's Offensive Research Bonus
                    </td>
                    <td>
                        <asp:TextBox ID="txtAttackResearchBonus" runat="server" CssClass="TextBox" Width="30"></asp:TextBox>%
                        <asp:RangeValidator CssClass="Error" ID="RangeValidator5" runat="server" ControlToValidate="txtAttackResearchBonus" Display="Dynamic" ErrorMessage="*" MaximumValue="100" MinimumValue="0" ToolTip='Valid numbers are between 0 and 100' Type="Integer"></asp:RangeValidator>
                    </td>
                </tr>
                <asp:Panel runat="server" ID="panelMorale" Visible="false">
                    <tr class="<% if(!isD2 && !isMobile) { %>help<% } %> highlight" rel="q_Handicap">
                        <td>
                            Morale
                        </td>
                        <td>
                            <asp:TextBox ID="txtMorale" runat="server" CssClass="TextBox jsiframeInput"
                                Width="30" ></asp:TextBox>
                            <asp:RangeValidator CssClass="Error" ID="RangeValidator_morale" runat="server" ControlToValidate="txtMorale"
                                Display="Dynamic" ErrorMessage="*" MaximumValue="50" MinimumValue="0" ToolTip=''
                                Type="Double"></asp:RangeValidator>
                            <asp:Label ID="lblMorale" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                </asp:Panel>
                <asp:Panel runat="server" ID="literalHandicap" Visible="false">
                    <tr class="<% if(!isD2 && !isMobile) { %>help<% } %> highlight" rel="q_Handicap">
                        <td>
                            <%=RS ("BattleHandicap") %>
                        </td>
                        <td>
                            <asp:TextBox rel="handicap" ID="txtHandicap" runat="server" CssClass="TextBox jsiframeInput"
                                Width="30"></asp:TextBox>%
                            <asp:RangeValidator CssClass="Error" ID="RangeValidator1" runat="server" ControlToValidate="txtHandicap"
                                Display="Dynamic" ErrorMessage="*" MaximumValue="50" MinimumValue="0" ToolTip=''
                                Type="Double"></asp:RangeValidator>
                            <a onclick="return popupHandicapCalc(this);" href="CalculateHandicap.aspx" target="_blank">
                                <%=RS ("Calculate") %></a>
                        </td>
                    </tr>
                </asp:Panel>
                <asp:Panel runat="server" ID="panelDesertion" Visible="false">
                    <tr class="<% if(!isD2 && !isMobile) { %>help<% } %> highlight" rel="q_desert">
                        <td valign="top">
                            <%=RS ("Desertion") %>
                        </td>
                        <td>
                            <asp:Label ID="lblDesertionFactor" runat="server" Text="0%"></asp:Label>
                            <br />
                            <%=RS ("TargetDistance") %> <asp:TextBox rel="desertion" ID="txtDesertionDistance"
                                runat="server" CssClass="TextBox jsiframeInput" Width="30"></asp:TextBox>
                            <asp:RangeValidator CssClass="Error" ID="RangeValidator2" runat="server" ControlToValidate="txtDesertionDistance"
                                Display="Dynamic" ErrorMessage="*" MaximumValue="1000" MinimumValue="0" ToolTip='<%#RS ("Between0and1000") %>'
                                Type="Double"></asp:RangeValidator>
                                                                                       
                            <a  onclick="return popupDesertionCalc(this);" 
                            href="CalculateDesertionFactor.aspx" target="_blank"><%=RS ("Calculate") %></a>
                        </td>
                    </tr>
                </asp:Panel>
                <tr class="highlight">
                    <td>
                        <asp:Label ID="lbl_BuildingName" runat="server" Text='<%#RS ("BuildingAttacked") %>' Visible="False"
                            Font-Bold="True"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lbl_BuildingAttackedresult" runat="server" Visible="False"></asp:Label>
                    </td>
                </tr>
                <asp:Panel ID="pnl_SpyExists" runat="server" Visible="false">
                    <tr class="highlight">
                        <td>
                            <%=RS ("SpyChance") %>
                        </td>
                        <td>
                            <asp:Label ID="lbl_SpySucessChance" runat="server" ></asp:Label>
                        </td>
                    </tr>
                    <tr class="highlight">
                        <td>
                            <%=RS ("SpyIdentifyChance") %>
                        </td>
                        <td>
                            <asp:Label ID="lbl_SpyIdentityKnown" runat="server" ></asp:Label>
                        </td>
                    </tr>
                    <tr class="highlight">
                        <td>
                            <%=RS ("SpySeeChance") %>
                        </td>
                        <td>
                            <asp:Label ID="lbl_SpiesComming" runat="server" ></asp:Label>
                        </td>
                    </tr>
                </asp:Panel>

            </table>
            <asp:Button ID="btn_Simulate" CssClass="inputbutton" runat="server" OnClick="Simulate_Click"
                Text='<%#RS("Simulate") %>' />
            <asp:Panel runat="server" ID="MyPFs" style="font-size:11pt;" CssClass="d2PFList">
                <br />
                Your
                <B><%=RS ("AttackBonus") %></B>
                <asp:Label ID="lblPDAttack" runat="server" Text=""></asp:Label>
                <% if(!isD2) { %><a href="pfbenefits.aspx" id="lnk_AttackBouns" runat="server">
                    <%# RS("ActivateFeature")%></a><% } %>                
                <br />
                Your
                <B><%=RS ("DefenceBonus") %></B>
                <asp:Label ID="lblPFDefence" runat="server" Text=""></asp:Label>
                <% if(!isD2) { %><a href="pfbenefits.aspx" id="lnk_DefenseBouns" runat="server">
                    <%# RS("ActivateFeature")%></a><% } %>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>

    <% if(isD2) { %>
    <script type="text/javascript">
        $('#ContentPlaceHolder1_MyPFs').hide();
        $('a.StandoutLink').attr('href', '#');
        $('.d2RemoveRightTD').remove();
        // Difficult to replace the close button because it is an iframe inside of an iframe
        // and the x button gets added through jquery in menu. Leaving as is.
        //$('#imgIframeClose').attr('src', 'https://static.realmofempires.com/images/icons/M_X.png');
    </script>
    <% } %>
</asp:Content>
