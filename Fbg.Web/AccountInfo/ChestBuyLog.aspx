<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ChestBuyLog.aspx.cs" Inherits="AccountInfo_ChestBuyLog"
    Trace="false" ValidateRequest="false" MasterPageFile="masterAI.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">

    <div class="header" >
        <center>
        <strong class="d2_title">
            Chest Purchase Log</strong>
        <br />
        <br />
        

             Here is the last 1000 chest purchase records<br />
       <asp:GridView ID="GridView1" runat="server"  DataSourceID="SqlDataSource1" 
            BackColor="White" BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" 
            CellPadding="4" ForeColor="Black" GridLines="Vertical" AutoGenerateColumns="true">
            <RowStyle BackColor="#F7F7DE" />                 
            <FooterStyle BackColor="#CCCC99" />
            <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
            <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
            <AlternatingRowStyle BackColor="White" />
            

        </asp:GridView>

        <asp:SqlDataSource ID="SqlDataSource1" runat="server"  ConnectionString="<%$ ConnectionStrings:fgb %>" 
                    
            ProviderName="System.Data.SqlClient" 
            SelectCommand="
                        select top 1000 DATEADD(hh, DATEDIFF(hh, GETDATE(), GETUTCDATE()), time) as Time, data as Action
                        from UserLog where eventid = 15 and playerid = @PlayerID order by Time desc 
                        "
                            >
            <SelectParameters>
                <asp:ControlParameter ControlID="txtPID" Name="PlayerID" 
                    PropertyName="Text" Type="Int32" />
            </SelectParameters>
                   
        </asp:SqlDataSource>
            <br />
            


        </center>
    </div>
     <asp:TextBox ID="txtPID" runat="server" ReadOnly="true" Visible="false" Enabled=false></asp:TextBox>  


</asp:Content>