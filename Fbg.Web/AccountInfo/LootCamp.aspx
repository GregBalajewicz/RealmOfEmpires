<%@ Page Language="C#" AutoEventWireup="true" CodeFile="LootCamp.aspx.cs" Inherits="AccountInfo_LootCamp"
    Trace="false" ValidateRequest="false" MasterPageFile="masterAI.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">

    <div class="header" >
        <center>
        <strong class="d2_title">
            My Traveling Caravan loot log</strong>
        <br />
        <br />

        

             Here is a number of times you have gotten the loot from the Traveling Caravan, in the last 5 days<br />
        <asp:GridView ID="GridView2" runat="server"  DataSourceID="SqlDataSource2" 
            BackColor="White" BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" 
            CellPadding="4" ForeColor="Black" GridLines="Vertical" AutoGenerateColumns="false">
            <RowStyle BackColor="#F7F7DE" />                 
            <FooterStyle BackColor="#CCCC99" />
            <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
            <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
            <AlternatingRowStyle BackColor="White" />
            

            <Columns>
                <asp:TemplateField HeaderText="Date" ItemStyle-Wrap="false" >
                    <ItemTemplate>                                                                            
                        <asp:Label ID="Label1" runat="server" Text='<%# ((DateTime)Eval("Day")).ToUniversalTime().ToLongDateString() %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
               
                <asp:TemplateField HeaderText="# of caravans accepted that day" ItemStyle-HorizontalAlign="Center" >
                    <ItemTemplate>                                                                            
                        <asp:Label ID="Label1" runat="server" Text='<%# String.Format("{0}", Eval("cnt") )%>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                            
            </Columns>
        </asp:GridView>

        <asp:SqlDataSource ID="SqlDataSource2" runat="server" 
                    
            ProviderName="System.Data.SqlClient" 
            SelectCommand="
                        select cast(AddedOn as date) as Day, count(*) as cnt
                             from  PlayerMapEvents with(nolock) where PlayerID = @PlayerID 
                               and TypeID = 2 
                                and AddedOn > dateadd(day, -5, cast(getdate() as date)) 
                                and IsActive = 0
                        group by cast(AddedOn as date) order by cast(AddedOn as date) desc
                        "
                            >
            <SelectParameters>
                <asp:ControlParameter ControlID="txtPID" Name="PlayerID" 
                    PropertyName="Text" Type="Int32" />
            </SelectParameters>
                   
        </asp:SqlDataSource>



        <br />
            Here is a detailed list of Traveling Caravan Loot, in the last 2 days <br />
        <asp:GridView ID="GridView1" runat="server" 
 DataSourceID="SqlDataSource1" AutoGenerateColumns="false"
            BackColor="White" BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" 
            CellPadding="4" ForeColor="Black" GridLines="Vertical">
            <RowStyle BackColor="#F7F7DE" />                 
            <FooterStyle BackColor="#CCCC99" />
            <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
            <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
            <AlternatingRowStyle BackColor="White" />
            <Columns>
                <asp:TemplateField HeaderText="Time" ItemStyle-Wrap="false" >
                    <ItemTemplate>                                                                            
                        <asp:Label ID="Label1" runat="server" Text='<%# ((DateTime)Eval("AddedOn")).ToUniversalTime().ToString() %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Location" >
                    <ItemTemplate>                                                                            
                        <asp:Label ID="Label1" runat="server" Text='<%# String.Format("({0}, {1})", Eval("Xcord"), Eval("YCord")) %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Status" ItemStyle-HorizontalAlign="Center" >
                    <ItemTemplate>                                                                            
                        <asp:Label ID="Label1" runat="server" Text='<%# String.Format("{0}", (bool)Eval("isActive") == true ? "Available, check map" : "accepted")%>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>                            
            </Columns>
        </asp:GridView>

        <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
                    
            ProviderName="System.Data.SqlClient" 
            SelectCommand="
                select AddedOn , Xcord,Ycord, isActive
                             from  PlayerMapEvents with(nolock) where PlayerID = @PlayerID and TypeID = 2 and AddedOn > dateadd(day, -2, cast(getdate() as date)) order by AddedOn desc" 
                            >
            <SelectParameters>
                <asp:ControlParameter ControlID="txtPID" Name="PlayerID" 
                    PropertyName="Text" Type="Int32" />
            </SelectParameters>
                   
        </asp:SqlDataSource>




        </center>
    </div>
     <asp:TextBox ID="txtPID" runat="server" ReadOnly="true" Visible="false" Enabled=false></asp:TextBox>  


</asp:Content>