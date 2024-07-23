<%@ Page MasterPageFile="~/main.master" Language="C#" AutoEventWireup="true" CodeFile="ClanEmailMembers.aspx.cs"
    Inherits="ClanEmailMembers" %>

<%@ Register Src="Controls/ClanMenu.ascx" TagName="ClanMenu" TagPrefix="uc1" %>
<asp:Content ContentPlaceHolderID="HeadPlaceHolder" runat="server">
    
    <%if (isMobile) { %>
    <style>
        .Popup a {
            text-decoration: none;
            font: 14px "IM Fell French Canon", serif;
            color: #e6cd90;
            text-shadow: 1px 1px 0 #000, -1px -1px 0 #000, 1px -1px 0 #000, -1px 1px 0 #000, 1px 1px 0 #000;
        }
        .TDContent table tr {
            background-color:rgba(0,0,0,.8);
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
    </style>
    <script>


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
        height:initial !important;
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

    }

    #ContentPlaceHolder1_lbl_PublicProfile {
        padding: 4px;
        display: block;
    }

    
    /* Clan Settings */
    .inputbutton, .inputsubmit {
         height:initial !important;
    }    

</style>
    <% }  %>

    <script>

	
            $(function () {
                function split(val) {
                    return val.split(/,\s*/);
                }
                function extractLast(term) {
                    return split(term).pop();
                }

                $(".jsmultipleplayers")
                // don't navigate away from the field on tab when selecting an item
                .bind("keydown", function (event) {
                    if (event.keyCode === $.ui.keyCode.TAB &&
                            $(this).data("autocomplete").menu.active) {
                        event.preventDefault();
                    }
                })
                .autocomplete({
                    source: function (request, response) {
                        $.getJSON("NamesAjax.aspx?what=playerNamesWClan", {
                            term: extractLast(request.term)
                        }, response);
                    },
                    search: function () {
                        // custom minLength
                        var term = extractLast(this.value);
                        if (term.length < 3) {
                            return false;
                        }
                    },
                    focus: function () {
                        // prevent value inserted on focus
                        return false;
                    },
                    select: function (event, ui) {
                        var terms = split(this.value);
                        // remove the current input
                        terms.pop();
                        // add the selected item
                        terms.push(ui.item.value);
                        // add placeholder to get the comma-and-space at the end
                        terms.push("");
                        this.value = terms.join(", ");
                        return false;
                    }
                });
            });
	
    </script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <uc1:ClanMenu ID="ClanMenu1" runat="server" />
    <%if (!isMobile) { %><br /> <%} %>
  <table border=0 cellpadding=0 cellspacing=1>
      <tr>
          <td colspan="2" style="font-size: 11pt;">
            <%= RS("DescTitle")%>
            <div class="jaxHide" rel='<%= RS("DescTitleMore")%>' >
                <%=RS ("SendMessages")%>
            </div>
          <br />
          <B><%=RS ("YouCanSend")%>
              <asp:Label ID="lblMaxEmails" runat="server" Text=""></asp:Label><%=RS ("Emails")%></B>
          </td>
      </tr>
      <tr>
            <td style="vertical-align: top" width="1%" class="TableHeaderRow Padding">
                <strong><%=RSc ("To") %></strong>
            </td>
            <td>
                <asp:TextBox runat="Server" ID="TxtTo" Width="80%" CssClass="TextBox jsmultipleplayers"></asp:TextBox><asp:RequiredFieldValidator
                    ID="RequiredFieldValidator1" runat="server" ErrorMessage='Enter player name(s)'
                    ControlToValidate="TxtTo" ValidationGroup="M1" CssClass="Error" Display="Dynamic"
                    ToolTip='Enter player name(s)'>*</asp:RequiredFieldValidator><br />
                   <%=RS ("PutComma")%>
            </td>
        </tr>
        <tr>
            <td class="TableHeaderRow Padding">
                <strong><%=RSc("Subject")%></strong>
            </td>
            <td>
                <%=RS ("ClanLeaderEmail")%> 
                <asp:Label ID="lblSendersName" runat="server" Text=""></asp:Label>: "<asp:TextBox runat="Server" ID="TxtSubject" Width="90%" MaxLength="90" CssClass="TextBox"></asp:TextBox>"<asp:RequiredFieldValidator
                    ID="RequiredFieldValidator2" runat="server" ErrorMessage='Enter Subject' ControlToValidate="TxtSubject"
                    CssClass="Error" ValidationGroup="M1" Display="Dynamic" ToolTip='Enter Subject'>*</asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td style="vertical-align: top" class="TableHeaderRow Padding">
                <strong><%=RSc("Message") %></strong>
            </td>
            <td>
                <asp:TextBox runat="Server" ID="TxtMessage" CssClass="TextBox" Width="90%" TextMode="MultiLine"
                    Columns="50" Rows="20" Wrap="true"></asp:TextBox>
                <br /><asp:RequiredFieldValidator
                    ID="RequiredFieldValidator3" runat="server" ErrorMessage='Enter a message'
                    ControlToValidate="TxtMessage" ValidationGroup="M1" CssClass="Error" Display="Dynamic"
                    ToolTip='Enter player name(s)'>Enter a message</asp:RequiredFieldValidator>
               <%=RS ("PlainText")%>
            </td>
        </tr>
        <tr>
            <td colspan="2" align="center" class="TableHeaderRow Padding" style="padding-top:4px;padding-bottom:4px;">
                <asp:Button runat="Server" ID="BtnSend" CssClass="inputbutton" Text='SEND' OnClick="BtnSend_Click"
                    ValidationGroup="M1" Width="92px" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;               
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <br />
                <asp:Label runat="server" ID="LblErrMsg" Text="" Visible="false" CssClass="Error"></asp:Label>
                <asp:Label ID="lblConfirm" runat="server" CssClass="ConfirmationMsg" style="font-size:11pt;" Font-Bold="True"
                    Text='' Visible="False"></asp:Label>
            </td>
        </tr>
      
      <tr><td colspan=2 style="font-size:11pt;">
        <asp:CheckBox ID="cbRulesAccepted" runat="server" Text='I accept the following rules:' />
        <ul>
        <li><%=RS ("NoSpam")%></li>
        <li><%=RS ("General")%></li>
        </ul>
        <%=RS ("WeMaintain")%>
        <br /><asp:Label ID="lblBanned" runat="server" Text=""></asp:Label>
      </td></tr>
    </table>   
</asp:Content>
