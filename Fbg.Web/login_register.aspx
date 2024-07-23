<%@ Page Language="C#" AutoEventWireup="true" CodeFile="login_register.aspx.cs" Inherits="login_register" MasterPageFile="~/main2.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMeta" runat="Server">
    <title>Register | <%=  RSc("GameName")%></title>
    <meta name="description" content="Register to realm" />

    <style>
        .smallInfoMsg {
            display: block;
            color: gray;
        }
    </style>

    <%if (!isMobile) { %>

    <style>

        .loginview {
            display: inline-block;
        }

        .introText {
            font-size: 20px;
            padding-bottom: 13px;
        }

                #LoginView1_CreateUserWizard1 {
            margin: 0 auto;
            width: 300px;
        }
                .validationError {
            
            text-align: center !important;
            color: #FF2121;
        }
        input[type="text"], input[type="password"] {
            font-size: 12px;
            width: 170px;
            padding: 5px 5px;
            border-radius: 5px;
            box-shadow: inset -2px 2px 5px rgba(0, 0, 0, 0.4);
            border: none;
        }
        input[type="submit"] {
            position: relative;
            height: 37px;
            width: 135px;
            cursor: pointer;
            border: none;
            background-color: rgba(0, 0, 0, 0);
            background-image: url("https://static.realmofempires.com/images/buttons/M_SP_Buttons.png");
            background-position: -100px -250px;
            overflow: hidden;
            font: 17px/0.83em "IM Fell French Canon SC", serif;
            color: #D7D7D7;
            text-shadow: 0px -3px 3px #081137, 0px -2px 0px #081137, 0px 3px 3px #081137, 0px 2px 0px #081137, -3px 0px 0px #081137, 3px 0px 0px #081137;
            text-decoration: none;
        }



        #gotobattle {
            position: relative;
            height: 37px;
            width: 135px;
            cursor: pointer;
            border: none;
            background-color: rgba(0, 0, 0, 0);
            background-image: url("https://static.realmofempires.com/images/buttons/M_SP_Buttons.png");
            background-position: -100px -250px;
            overflow: hidden;
            font: 17px/0.83em "IM Fell French Canon SC", serif;
            color: #D7D7D7;
            text-shadow: 0px -3px 3px #081137, 0px -2px 0px #081137, 0px 3px 3px #081137, 0px 2px 0px #081137, -3px 0px 0px #081137, 3px 0px 0px #081137;
            text-decoration: none;
            display: block;
            padding-top: 11px;
        }

        #Form1 {
            text-align: center;
        }

        #cph1_LoginView1_CreateUserWizard1 {
            display: inline-block;
        }
    </style>

    <%} else {%>

    <style>


        #LoginView1_CreateUserWizard1 {
            margin: 0 auto;
            width: 300px;
        }

        .introText {
            font-size: 20px;
            padding-bottom: 13px;
        }

        input[type="text"], input[type="password"] {
            font-size: 12px;
            width: 170px;
            padding: 5px 5px;
            border-radius: 5px;
            box-shadow: inset -2px 2px 5px rgba(0, 0, 0, 0.4);
            border: none;
        }
        input[type="submit"] {
            position: relative;
            height: 37px;
            width: 135px;
            cursor: pointer;
            border: none;
            background-color: rgba(0, 0, 0, 0);
            background-image: url("https://static.realmofempires.com/images/buttons/M_SP_Buttons.png");
            background-position: -100px -250px;
            overflow: hidden;
            font: 17px/0.83em "IM Fell French Canon SC", serif;
            color: #D7D7D7;
            text-shadow: 0px -3px 3px #081137, 0px -2px 0px #081137, 0px 3px 3px #081137, 0px 2px 0px #081137, -3px 0px 0px #081137, 3px 0px 0px #081137;
            text-decoration: none;
        }

        .validationError {
            display: block;
            text-align: center !important;
            color: #FF2121;
        }

        #gotobattle {
            position: relative;
            height: 37px;
            width: 135px;
            cursor: pointer;
            border: none;
            background-color: rgba(0, 0, 0, 0);
            background-image: url("https://static.realmofempires.com/images/buttons/M_SP_Buttons.png");
            background-position: -100px -250px;
            overflow: hidden;
            font: 17px/0.83em "IM Fell French Canon SC", serif;
            color: #D7D7D7;
            text-shadow: 0px -3px 3px #081137, 0px -2px 0px #081137, 0px 3px 3px #081137, 0px 2px 0px #081137, -3px 0px 0px #081137, 3px 0px 0px #081137;
            text-decoration: none;
            display: block;
            padding-top: 11px;
        }

       
    </style>

    <%} %>

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

                        </center>
                    </LoggedInTemplate>


                    <AnonymousTemplate>
                        <asp:CreateUserWizard OnContinueButtonClick="CreateUserWizard1_ContinueButtonClick" OnCreatedUser="CreateUserWizard1_CreatedUser" OnCreatingUser="CreateUserWizard1_CreatingUser" ID="CreateUserWizard1" runat="server" UserNameLabelText="Email :" DuplicateUserNameErrorMessage="The e-mail address that you entered is already in use. Please enter a different e-mail address." DisplayCancelButton="True" 
                            CancelDestinationPageUrl="login_how.aspx" 
                            EmailLabelText="Confirm E-mail:" EmailRegularExpression="\s*\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*\s*" ErrorMessageStyle-CssClass="reports" ValidatorTextStyle-CssClass="validationError" PasswordLabelText="Password:<span class='smallInfoMsg'>(6 character min)</span>">
                            <WizardSteps>
                                <asp:CreateUserWizardStep ID="CreateUserWizardStep1" runat="server">
                                </asp:CreateUserWizardStep>
                                <asp:CompleteWizardStep ID="CompleteWizardStep1" runat="server">
                                    <ContentTemplate>Account created! <a href="login_enter.aspx">Please login</a> </ContentTemplate>
                                </asp:CompleteWizardStep>
                            </WizardSteps>
                        </asp:CreateUserWizard>
                    </AnonymousTemplate>
                </asp:LoginView>

                <asp:Label ID="errorEmailInUse" CssClass="validationError" runat="server" Visible="false" Text="This email is already in use. If you feel this is a mistake, contact support at support@RealmOfEmpires.com"></asp:Label>
                <asp:Label ID="errorEmailMismatch" CssClass="validationError" runat="server" Visible="false" Text="Email entires do not match, please double check."></asp:Label>
            </form>
        </div>

    </div>

    <asp:Label ID="lblFriendsFreshedDebugMessage" runat="server" Text=""></asp:Label>
   
</asp:Content>
