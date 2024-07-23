<%@ Page Language="C#" MasterPageFile="~/main.master" AutoEventWireup="true" CodeFile="Help.aspx.cs"
    Inherits="Help" Title="Help" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
    <link type="text/json" rel="help" href="static/help/s_help.json.aspx" />

 
    <%if (isMobile ) { %>
   
    <%}%>



    <%if (isMobile ) { %>
    <script>

        function CustomOnLoad() {

            $("#ctl00_ContentPlaceHolder1_tblUnitHelp > tbody > tr >td").wrap("<TR>");

            $("#ctl00_ContentPlaceHolder1_tblUnitHelp .NoPad~td").wrap("<TR>");
            $("#ctl00_ContentPlaceHolder1_tblUnitHelp .NoPad").wrap("<TR>");
        }
    </script>
    <%}%>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <script type="text/javascript" language="javascript">
        function checkExpandCollapse(ID)
        {
            var where_is_mytool;
			var mytool_array;
			var imgName;
			where_is_mytool=document.getElementById("Img"+ID).src;
            mytool_array=where_is_mytool.split("/");
            imgName=mytool_array[mytool_array.length-1];
            
            if(imgName=="expand_button.gif")
            {
                document.getElementById("Tbody"+ID).style.display="";
                document.getElementById("Img"+ID).src=where_is_mytool.replace("expand","collapse");
            }
            else
            {
                document.getElementById("Tbody"+ID).style.display="none";
                document.getElementById("Img"+ID).src=where_is_mytool.replace("collapse","expand");
            }
        }
        
        function ExpandCollapse()
        {
            var where_is_mytool;
			var mytool_array;
			var imgName;
			where_is_mytool=document.getElementById("ctl00_ContentPlaceHolder1_Img0").src;
            mytool_array=where_is_mytool.split("/");
            imgName=mytool_array[mytool_array.length-1];
            
            if(imgName=="expand_button.gif")
            {
                document.getElementById("TbodyOverview").style.display="";
                document.getElementById("ctl00_ContentPlaceHolder1_Img0").src=where_is_mytool.replace("expand","collapse");
            }
            else
            {
                document.getElementById("TbodyOverview").style.display="none";
                document.getElementById("ctl00_ContentPlaceHolder1_Img0").src=where_is_mytool.replace("collapse","expand");
            }
        }
    </script>

    <div id="DivOverview">
    <%if (!isMobile) { %>
        <span style="font-size: 10pt">
            <img src='<%=RSic("helpicon")%>' style="float:left;padding-right:4px;" />
            <B><%=RS("NotSureWhatThenNumsMean")%> </B><BR /><%=RS("HoverYourMouseOverTheNumber")%>
            <img src='<%=RSic("helpicon")%>' />
            <%=RS("MarkToAppear")%>
            <br />
        </span>
    <%} %>
        <asp:Table runat="Server" ID="tblUnitOverview" CssClass="TypicalTable stripeTable"
            CellSpacing="1">
            <asp:TableHeaderRow runat="Server" ID="HeaderRow" CssClass="TableHeaderRow">
                <asp:TableHeaderCell ColumnSpan="6" ID="HCellBlank" runat="server" Text='<%#RS("AllUnitsOverview") %>'>
                </asp:TableHeaderCell>
                <asp:TableHeaderCell runat="server" ID="HCellDefense" Text='<%#RS("DefenseStrength") %>' HorizontalAlign="center"
                    Font-Bold="true">
                </asp:TableHeaderCell>
            </asp:TableHeaderRow>
            <asp:TableHeaderRow runat="Server" ID="HeaderRow1" CssClass="TableHeaderRow">
            </asp:TableHeaderRow>
        </asp:Table>
   </div>
    <br />
    <div id="DivMain">
    </div>
    <asp:Label ID="lbl_MainTable" runat="server" ></asp:Label>
</asp:Content>
