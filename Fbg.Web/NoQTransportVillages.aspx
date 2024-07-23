<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/main.master" CodeFile="NoQTransportVillages.aspx.cs" Inherits="NoQTransportVillages" %>

<asp:Content ContentPlaceHolderID=HeadPlaceHolder runat="server">

    
    <%if (isMobile ) { %>
    <style>
    .header {display:none;}

    .TDContent~center 
    ,.TDContent~hr 
    ,.TDContent~span
    , .ctl00_Menu1_4
    {display:none;}

    .TDContent {
    min-width:0px !important;
    }


    .biau .buildingImages {display :none;} 

    .biau .panelUpgrade{width:270px !important;}

    .biau .BoxHeader {display:none;}
    #ctl00_ContentPlaceHolder1_controlUnitRecruit_tblBuildings .StandoutLink {display :none;}
    </style>
    <%}%>



    <%if (isMobile ) { %>
    <script>

        function CustomOnLoad() {
            $("#ctl00_ContentPlaceHolder1_BuildingInfoAndUpgrade1_UpdatePanel1 td").removeAttr("nowrap");
            $("#ctl00_ContentPlaceHolder1_BuildingInfoAndUpgrade1_UpdatePanel1 td").css("white-space", "normal");

            $("#ctl00_ContentPlaceHolder1_controlUnitRecruit_tblBuildings td").removeAttr("nowrap");
            $("#ctl00_ContentPlaceHolder1_controlUnitRecruit_tblBuildings td").css("white-space", "normal");

            $(".bLevelInfoExt a").removeAttr("href");
            $(".bLevelInfoExt a").click(function () { });
            $(".bLevelInfoExt a").removeAttr("onclick");

            $("#ctl00_ContentPlaceHolder1_tblUnitHelp > tbody > tr >td").wrap("<TR>");

            $("#ctl00_ContentPlaceHolder1_tblUnitHelp .NoPad~td").wrap("<TR>");
            $("#ctl00_ContentPlaceHolder1_tblUnitHelp .NoPad").wrap("<TR>");
        }
    </script>
    <%}%>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
<br />
<div class="Tabs">
<ul>
	<li><a href="TransportSilver.aspx"><%=RS("TransportSilver") %></a></li>
	<li class="selected"><a href="NoQTransportVillages.aspx" style="width: 15em;"><%=RS("ConfigureQuickTransport") %></a></li>
</ul>

</div>


<div class="TabContent">
    <span style="font-size: 1.1em"><%=RS("QuickTransportParagraphOne") %><br />
    </span>
    <br />
    <asp:DropDownList ID="cmb_VillagesList" runat="server" DataValueField="VillageID" DataTextField="Name">
    
    </asp:DropDownList>
    <asp:Button ID="btn_Add" runat="server" Text='<%#RS("AddToNoTransportList") %>' OnClick="btn_Add_Click" CssClass="inputbutton" />
    <br />
    <br />
    <strong><%=RS("ListWillNotAppear") %> </strong>
    <br />
    <asp:GridView ID="gv_NoList" runat="server" AutoGenerateColumns="False"
                    ShowHeader="false"   OnRowDeleting="gv_NoList_RowDeleting"
                    GridLines="None" CellSpacing="1" CssClass="TypicalTable" 
                    AllowPaging="True" PageSize=100 DataKeyNames="VillageID" 
                    OnPageIndexChanging ="gv_NoList_OnPageIndexChanging" >
                    <Columns>
                       <asp:TemplateField>
                        <ItemTemplate>
                          <%#BindVillage(Container.DataItem) %>
                        </ItemTemplate>
                        </asp:TemplateField>
                        <asp:ButtonField ButtonType="Image" ImageUrl="https://static.realmofempires.com/images/cancel.png" CommandName="Delete">
                            <ItemStyle HorizontalAlign="Center" Width="20px" />
                        </asp:ButtonField>
                    </Columns>
                    <RowStyle CssClass="DataRowNormal" />
                    <HeaderStyle CssClass="TableHeaderRow" />
                    <AlternatingRowStyle CssClass="DataRowAlternate" />
                    <EmptyDataTemplate><%=RS("NoVillagesAtThisTime") %></EmptyDataTemplate>
                    
                </asp:GridView>
   </div>
</asp:Content>


