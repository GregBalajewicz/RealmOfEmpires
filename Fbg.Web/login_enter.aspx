<%@ Page Language="C#" AutoEventWireup="true" CodeFile="login_enter.aspx.cs" Inherits="login_enter" MasterPageFile="~/main2.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMeta" runat="Server">
    <title>Login | <%=  RSc("GameName")%></title>
    <meta name="description" content="Welcome back to Realm of Empires." />
</asp:Content>

<asp:Content ID="c1" ContentPlaceHolderID="cph1" runat="Server">

    <div id="main">
        <div class="roeLogo"></div>
        <div class="holderbkg">
            <div class="corner corner-top-left"></div>
            <div class="corner corner-top-right"></div>
            <div class="corner corner-bottom-left"></div>
            <div class="corner corner-bottom-right"></div>
            <div class="border lb-border-top"></div>
            <div class="border lb-border-right"></div>
            <div class="border lb-border-bottom"></div>
            <div class="border lb-border-left"></div>
        </div>

        <div id="content">
            <form id="Form1" runat="server">


                <asp:LoginView ID="LoginView1" runat="server">
                    <LoggedInTemplate>
                        <center>
                        Welcome back!
                        <br /><br /><a id="gotobattle" href="ChooseRealm.aspx"> Go to battle!</a>
                        
                        <br /><br />If you are experiencing trouble, please <a id="A1" href="logout.aspx">Logout</a> and try again
                        </center>
                    </LoggedInTemplate>


                    <AnonymousTemplate>

                        <asp:Login ID="Login1" runat="server"  VisibleWhenLoggedIn="False" OnLoggedIn="Login1_LoggedIn" DisplayRememberMe="true" UserNameLabelText="E-mail:" DestinationPageUrl="ChooseRealm.aspx"></asp:Login>
                        
                        <div class="additionals">
                            <a href="login_register.aspx">Not yet registered?</a>
                            <a href="login_resetpassword.aspx">Forgot your password?</a>
                        </div>

                        
                    </AnonymousTemplate>
                </asp:LoginView>
            </form>
        </div>
        <%if (isMobile) { %>
        <div class="backToDefault BtnDSm2n fontButton1L" onclick="window.location.href ='ChooseLoginType.aspx'">Back<span class="smallArrowLeft"></span></div>
        <%} %>
    </div>

    <asp:Label ID="lblFriendsFreshedDebugMessage" runat="server" Text=""></asp:Label>

</asp:Content>