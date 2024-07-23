<%@ Page MasterPageFile="~/main.master" Language="C#" AutoEventWireup="true" CodeFile="ClanClaims.aspx.cs" Inherits="ClanClaims" %>

<%@ Register Src="Controls/ClanMenu.ascx" TagName="ClanMenu" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="server">
    <link type="text/json" rel="help" href="static/help/ga_ClanMembers.json.aspx" />
    <style>
        a.MissingRole {
            color: #000000;
            text-decoration: none;
        }

            a.MissingRole:hover {
                color: #00C125;
                text-decoration: none;
            }

        a.GotRole:hover {
            color: #FA0625;
            text-decoration: none;
        }

        a.GotRole {
            color: #FFFFFF;
            text-decoration: none;
        }

        .MissingRole {
            color: #000000;
            text-decoration: none;
        }

        .GotRole {
            color: #ffffff;
            text-decoration: none;
        }

        .bbcode_v, .bbcode_p, .bbcode_c {
            cursor: pointer;
        }

        .special_True {
            display: none;
        }
    </style>




    <%if (isMobile)
      { %>
    <style>
        .Popup a {
            text-decoration: none;
            font: 14px "IM Fell French Canon", serif;
            color: #e6cd90;
            text-shadow: 1px 1px 0 #000, -1px -1px 0 #000, 1px -1px 0 #000, -1px 1px 0 #000, 1px 1px 0 #000;
        }

        .TDContent table tr {
            background-color: rgba(0,0,0,.8);
        }

            .TDContent table tr td {
                padding-bottom: 5px;
                padding-left: 5px;
                font-size: 11px !important;
            }

        .TableHeaderRow {
            background-image: -webkit-linear-gradient(top, rgba(35, 35, 35, 0.3) 0%, rgba(108, 108, 108, 0) 100%);
            color: #FFF;
            font-weight: bold;
            padding: 1px;
        }

        .inputbutton, .inputsubmit {
            height: 40px;
            width: 110px !important;
            text-align: center;
            box-sizing: border-box;
            cursor: pointer;
            background-image: url("https://static.realmofempires.com/images/buttons/M_SP_Buttons.png");
            background-position: -100px -50px;
            border: none;
            background-color: rgba(0, 0, 0, 0);
            font: 15px/1.0em "IM Fell French Canon SC", serif;
            color: #D7D7D7;
            text-shadow: 2px 2px 0 #000, -1px -1px 0 #000, 1px -1px 0 #000, -1px 1px 0 #000, 1px 1px 0 #000;
        }

        .TDContent {
            position: absolute;
            left: 0px;
            right: 0px;
            top: 0px;
            bottom: 0px;
            overflow: auto;
        }

        .line {
            margin-top: 10px;
            margin-left: 10px;
        }



        .sectionDividerA {
            height: 8px;
            background-image: url('https://static.realmofempires.com/images/misc/M_SP_UIMisc.png');
            background-repeat: no-repeat;
            background-position: 0px 0px;
        }
        .notclaimed {
            color:gray;
        }
    </style>
    <script>


    </script>
    <%}
      else if (isD2)
      { %>
    <style>
        /* Clan Overview */
        html, body {
            background: none !important;
        }

        body {
            font-family: Georgia, 'Playfair Display';
            font-size: 12px;
        }

        a, a:active, a:link {
            color: #d3b16f;
        }

            a:hover {
                color: #d3b16f;
            }

        .tempMenu {
            background-color: rgba(88, 140, 173, 0.3);
            border-bottom: 2px solid #9d9781;
            padding: 4px;
            padding-bottom: 2px;
        }

            .tempMenu a {
                text-shadow: 1px 1px 1px #000;
            }

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
            margin-bottom: 12px;
        }

        .TextBox {
            /* so text area fits within width */
            -webkit-box-sizing: border-box;
            -moz-box-sizing: border-box;
            box-sizing: border-box;
            background-color: rgba(255,255,255,0.8);
            font-weight: normal;
            font-size: 12px;
            font-family: Georgia, 'Playfair Display';
            padding: 4px;
        }


        .inputbutton, .inputsubmit {
            font-weight: bold;
            font-size: 12px;
            font-family: Georgia, 'Playfair Display';
            margin-left: 0px;
            margin-top: 4px;
            padding: 4px 6px;
            color: #d3b16f;
            border-color: #efe1b9;
            background-color: rgba(25, 55, 74, 0.7);
            -moz-box-shadow: inset 0 0 5px 1px #000000;
            -webkit-box-shadow: inset 0 0 5px 1px #000000;
            box-shadow: inset 0 0 5px 1px #000000;
            height: initial;
        }

            .inputbutton:hover, .inputsubmit:hover {
                -moz-box-shadow: inset 0 0 5px 1px #efe1b9;
                -webkit-box-shadow: inset 0 0 5px 1px #efe1b9;
                box-shadow: inset 0 0 5px 1px #efe1b9;
            }



        /* Clan Public Profile */
        .TableHeaderRow {
            font-size: 12px !important;
            font-weight: normal;
            background-color: rgba(49, 81, 108, 0.7) !important;
            padding: 4px;
        }

        div.TableHeaderRow {
            /*  margin-top:12px;*/
        }

        .TableHeaderRow #ContentPlaceHolder1_lbl_ClanName {
            font-size: 12px !important;
            font-weight: bold;
        }

        #ContentPlaceHolder1_lblPageExplanationToMemebers {
            display: block;
        }

        #ContentPlaceHolder1_pnl_Profile td {
            padding: 4px;
        }



        .clanPage table {
            /* remove spacing on outside of table */
            border-collapse: separate;
            border-spacing: 1px;
            margin: -1px;
            margin-top: 4px;
        }

        #ContentPlaceHolder1_lbl_PublicProfile {
            padding: 4px;
            display: block;
        }
    </style>
    <%}       %>







    <%if (isMobile)
      { %>

    <script>

        $(function () {


            $('.bbcode_v').removeAttr('onclick');
            $('.bbcode_p').removeAttr('onclick');
            $('.bbcode_c').removeAttr('onclick');
            $('.bbcode_c').removeAttr('href');

            //
            // handle some bbcodes
            //
            $('.TDContent').delegate('.bbcode_p', 'click', function () {
                window.parent.ROE.Frame.popupPlayerProfile($(event.target).html());
                return false;
            });
            $('.TDContent').delegate('.bbcode_v', 'click', function () {
                window.parent.ROE.Frame.popupVillageProfile($(event.target).attr('data-vid'));
                return false;
            });
            $('.TDContent').delegate('.bbcode_c', 'click', function () {
                //since the clan popup window is a singleton, we cannot open a new one when we already got one opened...
                window.parent.ROE.Frame.popupClan($(event.target).attr('data-cid'));
                return false;
            });
        });

    </script>
    <%}
      else if (isD2)
      { %>

    <script>

        $(function () {


            $('.bbcode_v').removeAttr('onclick');
            $('.bbcode_p').removeAttr('onclick');
            $('.bbcode_c').removeAttr('onclick');
            $('.bbcode_c').removeAttr('href');

            //
            // handle some bbcodes
            //
            $('.TDContent').delegate('.bbcode_p', 'click', function () {
                window.parent.ROE.Frame.popupPlayerProfile($(event.target).html());
                return false;
            });
            $('.TDContent').delegate('.bbcode_v', 'click', function () {
                window.parent.ROE.Frame.popupVillageProfile($(event.target).attr('data-vid'));
                return false;
            });
            $('.TDContent').delegate('.bbcode_c', 'click', function () {
                //since the clan popup window is a singleton, we cannot open a new one when we already got one opened...
                window.parent.ROE.Frame.popupClan($(event.target).attr('data-cid'));
                return false;
            });
        });

    </script>

    <%}%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <uc1:ClanMenu ID="ClanMenu1" runat="server" />
    <br />

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
    <table border="0">
        <tr>
            <td>
                <asp:Label ID="lbl_Error" CssClass="Error" runat="server" Visible="False"></asp:Label>

                <asp:Panel runat="server" ID="ui">

                    <asp:Panel runat="server" ID="normalview_notFocusOn">
                        Active Claims
                        <asp:GridView ID="GridView1" runat="server" AllowPaging="false"
                        AutoGenerateColumns="False" CssClass="TypicalTable"
                        CellPadding="4" DataKeyNames="ClaimedVID" DataSourceID="SqlDataSource1" EnableModelValidation="True" GridLines="None" EmptyDataText="No claims yet (or you lack permissions to see full claim list)">

                        <Columns>
                            <asp:CommandField ShowDeleteButton="true" ButtonType="Image" DeleteImageUrl="https://static.realmofempires.com/images/cancel.png" />
                            <asp:TemplateField HeaderText="Claimed by">
                                <ItemTemplate>
                                    <a class="bbcode_p" data-pid="<%# Eval("PlayerID")%>"><%# Eval("Name") %></a>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Hours ago">
                                <ItemTemplate>
                                    <%# DateTime.Now.Subtract((DateTime)Eval("TimeOfClaim")).TotalHours.ToString("#.0") %>h
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Claimed">
                                <ItemTemplate>
                                    <a class="bbcode_v " data-vid="<%# Eval("ClaimedVID")  %>"><%# Eval("ClaimedVillageName")%> (<%# Eval("ClaimedVillageX") %>,<%# Eval("ClaimedVillageY") %>)</a>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="">
                                <ItemTemplate>
                                    <a class="bbcode_p special_<%#Fbg.Bll.utils.IsSpecialPlayer((int)Eval("ClaimedVillageOwnerID"), FbgPlayer.Realm) %>" data-pid="<%# Eval("ClaimedVillageOwnerID")%>" visible="<%#Fbg.Bll.utils.IsSpecialPlayer((int)Eval("ClaimedVillageOwnerID"), FbgPlayer.Realm) %>"><%# Eval("ClaimedVillageOwnerName") %></a>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="">
                                <ItemTemplate>
                                    <%# Eval("ClaimedVillageOwnerClanName") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    Focus On
                                     <a href="ClanClaims.aspx?ClaimedByPlayerID=<%# Eval("PlayerID")%>"><%# Eval("Name") %></a>
                                    <span class="special_<%#Fbg.Bll.utils.IsSpecialPlayer((int)Eval("ClaimedVillageOwnerID"), FbgPlayer.Realm) %>"> | </span>
                                    <a href="ClanClaims.aspx?ClaimedVillageOwnerID=<%# Eval("ClaimedVillageOwnerID")%>" class="special_<%#Fbg.Bll.utils.IsSpecialPlayer((int)Eval("ClaimedVillageOwnerID"), FbgPlayer.Realm) %>"><%# Eval("ClaimedVillageOwnerName") %> </a>


                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <RowStyle CssClass="DataRowNormal" />
                        <HeaderStyle CssClass="TableHeaderRow" />
                        <AlternatingRowStyle CssClass="DataRowAlternate" />

                    </asp:GridView>
                        <asp:GridView ID="GridView1_m" runat="server" AllowPaging="false" Width="100%"
                            AutoGenerateColumns="False" CssClass="TypicalTable"
                            CellPadding="4" DataKeyNames="ClaimedVID" DataSourceID="SqlDataSource1" EnableModelValidation="True" GridLines="None" EmptyDataText="No claims yet  (or you lack permissions to see full claim list)">

                            <Columns>
                                <asp:CommandField ShowDeleteButton="true" ButtonType="Image" DeleteImageUrl="https://static.realmofempires.com/images/cancel.png" />

                                <asp:TemplateField HeaderText="">
                                    <ItemTemplate>

                                        <a class="bbcode_p" data-pid="<%# Eval("PlayerID")%>"><%# Eval("Name") %></a>
                                        <%# DateTime.Now.Subtract((DateTime)Eval("TimeOfClaim")).TotalHours.ToString("#.0") %>h ago claimed:

                                    <div class="line">
                                        <a class="bbcode_v " data-vid="<%# Eval("ClaimedVID")  %>"><%# Eval("ClaimedVillageName")%> (<%# Eval("ClaimedVillageX") %>,<%# Eval("ClaimedVillageY") %>)</a>
                                        <span class="special_<%#Fbg.Bll.utils.IsSpecialPlayer((int)Eval("ClaimedVillageOwnerID"), FbgPlayer.Realm) %>">owned by :</span>
                                    </div>

                                        <div class="line">
                                            <a class="bbcode_p special_<%#Fbg.Bll.utils.IsSpecialPlayer((int)Eval("ClaimedVillageOwnerID"), FbgPlayer.Realm) %>" data-pid="<%# Eval("ClaimedVillageOwnerID")%>" visible="<%#Fbg.Bll.utils.IsSpecialPlayer((int)Eval("ClaimedVillageOwnerID"), FbgPlayer.Realm) %>"><%# Eval("ClaimedVillageOwnerName") %></a>
                                            <br />
                                            <%# Eval("ClaimedVillageOwnerClanName") %>
                                        </div>

                                        <div class="line" style="text-align: right;">


                                    Focus On
                                     <a href="ClanClaims.aspx?ClaimedByPlayerID=<%# Eval("PlayerID")%>"><%# Eval("Name") %></a>
                                    <span class="special_<%#Fbg.Bll.utils.IsSpecialPlayer((int)Eval("ClaimedVillageOwnerID"), FbgPlayer.Realm) %>"> | </span>
                                    <a href="ClanClaims.aspx?ClaimedVillageOwnerID=<%# Eval("ClaimedVillageOwnerID")%>" class="special_<%#Fbg.Bll.utils.IsSpecialPlayer((int)Eval("ClaimedVillageOwnerID"), FbgPlayer.Realm) %>"><%# Eval("ClaimedVillageOwnerName") %> </a>



                                        </div>


                                        <div class="sectionDividerA"></div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <RowStyle CssClass="DataRowNormal" />
                            <HeaderStyle CssClass="TableHeaderRow" />
                            <AlternatingRowStyle CssClass="DataRowAlternate" />

                        </asp:GridView>
                        <asp:SqlDataSource ID="SqlDataSource1" runat="server"
                            DeleteCommand="exec dPlayerVillageClaims @LoggedInPlayerID, @ClanID, @ClaimedVID"
                            SelectCommand="exec qAllPlayerVillageClaims @LoggedInPlayerID,@ClanID">
                            <DeleteParameters>
                                <asp:ControlParameter ControlID="aid" Name="LoggedInPlayerID" Type="Int32" />
                                <asp:Parameter Name="ClaimedVID" Type="Int32" />
                                <asp:ControlParameter ControlID="av" Name="ClanID" Type="string" />
                            </DeleteParameters>
                            <SelectParameters>
                                <asp:ControlParameter ControlID="aid" Name="LoggedInPlayerID" Type="Int32" />
                                <asp:ControlParameter ControlID="av" Name="ClanID" Type="string" />

                            </SelectParameters>

                        </asp:SqlDataSource>


                        <br />
                        Completed Claims
                        <asp:GridView ID="GridView2" runat="server" AllowPaging="false"
                        AutoGenerateColumns="False" CssClass="TypicalTable"
                        CellPadding="4" DataKeyNames="ClaimedVID" DataSourceID="SqlDataSource3" EnableModelValidation="True" GridLines="None" EmptyDataText="No completed claims yet">

                        <Columns>
                            <asp:CommandField ShowDeleteButton="true" ButtonType="Image" DeleteImageUrl="https://static.realmofempires.com/images/cancel.png" />
                            <asp:TemplateField HeaderText="Claimed by">
                                <ItemTemplate>
                                    <a class="bbcode_p" data-pid="<%# Eval("PlayerID")%>"><%# Eval("Name") %></a>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Hours ago">
                                <ItemTemplate>
                                    <%# DateTime.Now.Subtract((DateTime)Eval("TimeOfClaim")).TotalHours.ToString("#.0") %>h
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Claimed">
                                <ItemTemplate>
                                    <a class="bbcode_v " data-vid="<%# Eval("ClaimedVID")  %>"><%# Eval("ClaimedVillageName")%> (<%# Eval("ClaimedVillageX") %>,<%# Eval("ClaimedVillageY") %>)</a>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="">
                                <ItemTemplate>
                                    <a class="bbcode_p special_<%#Fbg.Bll.utils.IsSpecialPlayer((int)Eval("ClaimedVillageOwnerID"), FbgPlayer.Realm) %>" data-pid="<%# Eval("ClaimedVillageOwnerID")%>" visible="<%#Fbg.Bll.utils.IsSpecialPlayer((int)Eval("ClaimedVillageOwnerID"), FbgPlayer.Realm) %>"><%# Eval("ClaimedVillageOwnerName") %></a>
                                </ItemTemplate>
                            </asp:TemplateField>
                            
                        </Columns>
                        <RowStyle CssClass="DataRowNormal" />
                        <HeaderStyle CssClass="TableHeaderRow" />
                        <AlternatingRowStyle CssClass="DataRowAlternate" />

                    </asp:GridView>
                        <asp:GridView ID="GridView2_m" runat="server" AllowPaging="false" Width="100%"
                            AutoGenerateColumns="False" CssClass="TypicalTable"
                            CellPadding="4" DataKeyNames="ClaimedVID" DataSourceID="SqlDataSource3" EnableModelValidation="True" GridLines="None" EmptyDataText="No compled claims yet">

                            <Columns>
                                <asp:CommandField ShowDeleteButton="true" ButtonType="Image" DeleteImageUrl="https://static.realmofempires.com/images/cancel.png" />



                                <asp:TemplateField HeaderText="">
                                    <ItemTemplate>

                                        <a class="bbcode_p" data-pid="<%# Eval("PlayerID")%>"><%# Eval("Name") %></a>
                                        <%# DateTime.Now.Subtract((DateTime)Eval("TimeOfClaim")).TotalHours.ToString("#.0") %>h ago claimed:

                                    <div class="line">
                                        <a class="bbcode_v " data-vid="<%# Eval("ClaimedVID")  %>"><%# Eval("ClaimedVillageName")%> (<%# Eval("ClaimedVillageX") %>,<%# Eval("ClaimedVillageY") %>)</a>

                                    </div>

                                        <div class="sectionDividerA"></div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <RowStyle CssClass="DataRowNormal" />
                            <HeaderStyle CssClass="TableHeaderRow" />
                            <AlternatingRowStyle CssClass="DataRowAlternate" />

                        </asp:GridView>

                        <asp:SqlDataSource ID="SqlDataSource3" runat="server"
                            DeleteCommand="exec dPlayerVillageClaims @LoggedInPlayerID, @ClanID, @ClaimedVID"
                            SelectCommand="exec qAllPlayerVillageClaims_Completed @LoggedInPlayerID,@ClanID">
                            <DeleteParameters>
                                <asp:ControlParameter ControlID="aid" Name="LoggedInPlayerID" Type="Int32" />
                                <asp:Parameter Name="ClaimedVID" Type="Int32" />
                                <asp:ControlParameter ControlID="av" Name="ClanID" Type="string" />
                            </DeleteParameters>
                            <SelectParameters>
                                <asp:ControlParameter ControlID="aid" Name="LoggedInPlayerID" Type="Int32" />
                                <asp:ControlParameter ControlID="av" Name="ClanID" Type="string" />

                            </SelectParameters>

                        </asp:SqlDataSource>
                    </asp:Panel>
                     

                    <asp:Panel runat="server" ID="focusOnPlayerArea">
                        <a href="ClanClaims.aspx">< BACK</a>
                        <asp:GridView ID="GridView_focus" runat="server" AllowPaging="false"
                            AutoGenerateColumns="false" CssClass="TypicalTable"
                            CellPadding="4" DataKeyNames="ClaimedVID" DataSourceID="SqlDataSource2" EnableModelValidation="True" GridLines="None" >

                            <Columns>
                                <asp:TemplateField HeaderText="Claimed by">
                                    <ItemTemplate>
                                        <a class="bbcode_p" data-pid="<%# Eval("PlayerID")%>"><%# Eval("Name") %></a>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Hours ago">
                                    <ItemTemplate>
                                        <%# Eval("TimeOfClaim") is DBNull? "" : DateTime.Now.Subtract((DateTime)Eval("TimeOfClaim")).TotalHours.ToString("#.0h") %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Claimed">
                                    <ItemTemplate>
                                        <a class="bbcode_v " data-vid="<%# Eval("ClaimedVID")  %>"><%# Eval("ClaimedVillageName")%> (<%# Eval("ClaimedVillageX") %>,<%# Eval("ClaimedVillageY") %>)</a>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="">
                                    <ItemTemplate>
                                        <a class="bbcode_p special_<%#Fbg.Bll.utils.IsSpecialPlayer((int)Eval("ClaimedVillageOwnerID"), FbgPlayer.Realm) %>" data-pid="<%# Eval("ClaimedVillageOwnerID")%>" visible="<%#Fbg.Bll.utils.IsSpecialPlayer((int)Eval("ClaimedVillageOwnerID"), FbgPlayer.Realm) %>"><%# Eval("ClaimedVillageOwnerName") %></a>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="">
                                    <ItemTemplate>
                                        <%# Eval("ClaimedVillageOwnerClanName") %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                            </Columns>
                            <RowStyle CssClass="DataRowNormal" />
                            <HeaderStyle CssClass="TableHeaderRow" />
                            <AlternatingRowStyle CssClass="DataRowAlternate" />

                        </asp:GridView>
                        <asp:GridView ID="GridView_focus_m" runat="server" AllowPaging="false"
                            AutoGenerateColumns="False" CssClass="TypicalTable"
                            CellPadding="4" DataKeyNames="ClaimedVID" DataSourceID="SqlDataSource2" EnableModelValidation="True" GridLines="None" >

                            <Columns>
                                <asp:TemplateField HeaderText="">
                                    <ItemTemplate>
                                        <a class="bbcode_p" data-pid="<%# Eval("PlayerID")%>"><%# Eval("Name") %></a>
                                       
                                         <%# Eval("TimeOfClaim") is DBNull? "" : DateTime.Now.Subtract((DateTime)Eval("TimeOfClaim")).TotalHours.ToString("#.0h ago claimed:") %>

                                         <%# Eval("TimeOfClaim") is DBNull? "<span class=notclaimed>not claimed</span>" : "" %>

                                        <div class="line">
                                            <a class="bbcode_v " data-vid="<%# Eval("ClaimedVID")  %>"><%# Eval("ClaimedVillageName")%> (<%# Eval("ClaimedVillageX") %>,<%# Eval("ClaimedVillageY") %>)</a>
                                            <span class="special_<%#Fbg.Bll.utils.IsSpecialPlayer((int)Eval("ClaimedVillageOwnerID"), FbgPlayer.Realm) %>">owned by :</span>
                                        </div>

                                        <div class="line">
                                            <a class="bbcode_p special_<%#Fbg.Bll.utils.IsSpecialPlayer((int)Eval("ClaimedVillageOwnerID"), FbgPlayer.Realm) %>" data-pid="<%# Eval("ClaimedVillageOwnerID")%>" visible="<%#Fbg.Bll.utils.IsSpecialPlayer((int)Eval("ClaimedVillageOwnerID"), FbgPlayer.Realm) %>"><%# Eval("ClaimedVillageOwnerName") %></a>
                                            <br />
                                            <%# Eval("ClaimedVillageOwnerClanName") %>
                                        </div>

                                        <div class="sectionDividerA"></div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                            </Columns>
                            <RowStyle CssClass="DataRowNormal" />
                            <HeaderStyle CssClass="TableHeaderRow" />
                            <AlternatingRowStyle CssClass="DataRowAlternate" />

                        </asp:GridView>
                        <asp:SqlDataSource ID="SqlDataSource2" runat="server"
                            DeleteCommand="exec dPlayerVillageClaims @LoggedInPlayerID, @ClanID, @ClaimedVID"
                            SelectCommand="exec qAllPlayerVillageClaims_OfOnePlayer @LoggedInPlayerID,@ClanID, @ClaimedPlayerID">
                            <DeleteParameters>
                                <asp:ControlParameter ControlID="aid" Name="LoggedInPlayerID" Type="Int32" />
                                <asp:Parameter Name="ClaimedVID" Type="Int32" />
                                <asp:ControlParameter ControlID="av" Name="ClanID" Type="string" />
                            </DeleteParameters>
                            <SelectParameters>
                                <asp:ControlParameter ControlID="aid" Name="LoggedInPlayerID" Type="Int32" />
                                <asp:ControlParameter ControlID="av" Name="ClanID" Type="string" />
                                <asp:ControlParameter ControlID="ClaimedPlayerID" Name="ClaimedPlayerID" Type="string" />

                            </SelectParameters>

                        </asp:SqlDataSource>
                    </asp:Panel>


                    <asp:Panel runat="server" ID="focusOnPlayerArea_CliamsByPlayer">
                        <a href="ClanClaims.aspx">< BACK</a>
                        <asp:GridView ID="GridView_focusByPlayer" runat="server" AllowPaging="false"
                            AutoGenerateColumns="False" CssClass="TypicalTable"
                            CellPadding="4" DataKeyNames="ClaimedVID" DataSourceID="SqlDataSource4" EnableModelValidation="True" GridLines="None" >

                            <Columns>
                                <asp:CommandField ShowDeleteButton="true" ButtonType="Image" DeleteImageUrl="https://static.realmofempires.com/images/cancel.png" />
                                <asp:TemplateField HeaderText="">
                                    <ItemTemplate>
                                        <a class="bbcode_p" data-pid="<%# Eval("PlayerID")%>"><%# Eval("Name") %></a>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Hours ago">
                                    <ItemTemplate>
                                        <%# Eval("TimeOfClaim") is DBNull? "" : DateTime.Now.Subtract((DateTime)Eval("TimeOfClaim")).TotalHours.ToString("#.0h") %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Claimed">
                                    <ItemTemplate>
                                        <a class="bbcode_v " data-vid="<%# Eval("ClaimedVID")  %>"><%# Eval("ClaimedVillageName")%> (<%# Eval("ClaimedVillageX") %>,<%# Eval("ClaimedVillageY") %>)</a>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="">
                                    <ItemTemplate>
                                        <a class="bbcode_p special_<%#Fbg.Bll.utils.IsSpecialPlayer((int)Eval("ClaimedVillageOwnerID"), FbgPlayer.Realm) %>" data-pid="<%# Eval("ClaimedVillageOwnerID")%>" visible="<%#Fbg.Bll.utils.IsSpecialPlayer((int)Eval("ClaimedVillageOwnerID"), FbgPlayer.Realm) %>"><%# Eval("ClaimedVillageOwnerName") %></a>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="">
                                    <ItemTemplate>
                                        <%# Eval("ClaimedVillageOwnerClanName") %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                            </Columns>
                            <RowStyle CssClass="DataRowNormal" />
                            <HeaderStyle CssClass="TableHeaderRow" />
                            <AlternatingRowStyle CssClass="DataRowAlternate" />

                        </asp:GridView>
                        <asp:GridView ID="GridView_focusByPlayer_m" runat="server" AllowPaging="false"
                            AutoGenerateColumns="False" CssClass="TypicalTable"
                            CellPadding="4" DataKeyNames="ClaimedVID" DataSourceID="SqlDataSource4" EnableModelValidation="True" GridLines="None" >

                            <Columns>
                                <asp:CommandField ShowDeleteButton="true" ButtonType="Image" DeleteImageUrl="https://static.realmofempires.com/images/cancel.png" />
                               <asp:TemplateField HeaderText="">
                                    <ItemTemplate>

                                        <a class="bbcode_p" data-pid="<%# Eval("PlayerID")%>"><%# Eval("Name") %></a>
                                        <%# DateTime.Now.Subtract((DateTime)Eval("TimeOfClaim")).TotalHours.ToString("#.0") %>h ago claimed:

                                    <div class="line">
                                        <a class="bbcode_v " data-vid="<%# Eval("ClaimedVID")  %>"><%# Eval("ClaimedVillageName")%> (<%# Eval("ClaimedVillageX") %>,<%# Eval("ClaimedVillageY") %>)</a>
                                        <span class="special_<%#Fbg.Bll.utils.IsSpecialPlayer((int)Eval("ClaimedVillageOwnerID"), FbgPlayer.Realm) %>">owned by :</span>
                                    </div>

                                        <div class="line">
                                            <a class="bbcode_p special_<%#Fbg.Bll.utils.IsSpecialPlayer((int)Eval("ClaimedVillageOwnerID"), FbgPlayer.Realm) %>" data-pid="<%# Eval("ClaimedVillageOwnerID")%>" visible="<%#Fbg.Bll.utils.IsSpecialPlayer((int)Eval("ClaimedVillageOwnerID"), FbgPlayer.Realm) %>"><%# Eval("ClaimedVillageOwnerName") %></a>
                                            <br />
                                            <%# Eval("ClaimedVillageOwnerClanName") %>
                                        </div>

                                      

                                        <div class="sectionDividerA"></div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                            </Columns>
                            <RowStyle CssClass="DataRowNormal" />
                            <HeaderStyle CssClass="TableHeaderRow" />
                            <AlternatingRowStyle CssClass="DataRowAlternate" />

                        </asp:GridView>
                        <asp:SqlDataSource ID="SqlDataSource4" runat="server"
                            DeleteCommand="exec dPlayerVillageClaims @LoggedInPlayerID, @ClanID, @ClaimedVID"
                            SelectCommand="exec qAllPlayerVillageClaims_ByOnePlayer @LoggedInPlayerID,@ClanID, @ClaimedByPlayerID">
                            <DeleteParameters>
                                <asp:ControlParameter ControlID="aid" Name="LoggedInPlayerID" Type="Int32" />
                                <asp:Parameter Name="ClaimedVID" Type="Int32" />
                                <asp:ControlParameter ControlID="av" Name="ClanID" Type="string" />
                            </DeleteParameters>
                            <SelectParameters>
                                <asp:ControlParameter ControlID="aid" Name="LoggedInPlayerID" Type="Int32" />
                                <asp:ControlParameter ControlID="av" Name="ClanID" Type="string" />
                                <asp:ControlParameter ControlID="ClaimedByPlayerID" Name="ClaimedByPlayerID" Type="string" />

                            </SelectParameters>

                        </asp:SqlDataSource>
                    </asp:Panel>




                    <br />
                    Clan's Village Claim System.
                <br />
                    Villages are claimed from the map. 
                <br />
                    You can see your claims, and everyone else's claims if you have the sufficient clan permissions
                <br />
                    You can delete your own claim; Admins and Owners can delete any claim
                </asp:Panel>

            </td>
        </tr>
        <tr>
            <td></td>
        </tr>
    </table>
            </ContentTemplate>
    </asp:UpdatePanel>
    <div style="display: none">
        <asp:TextBox ID="aid" runat="server" runat="server"></asp:TextBox>
        <asp:TextBox ID="av" runat="server"></asp:TextBox>
        <asp:TextBox ID="ClaimedPlayerID" runat="server" Width="48px"></asp:TextBox>
               <asp:TextBox ID="ClaimedByPlayerID" runat="server" Width="48px"></asp:TextBox>
    </div>

</asp:Content>
