<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/templates/masterPopupTemplate_m.master" CodeFile="Items2.aspx.cs" Inherits="templates_Items2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">
     
    <div id="items2" >
        
        <div class="header">
            <div class="villageInfoContainer">
                <div class="villageIcon"></div>
                <div class="villageInfo fontGoldFrLClrg"></div>
                <div class="text1 fontDarkGoldFrLClrg">is currently set to use rewards</div>
            </div>
            <div class="smallRoundButtonDark reload"><div class="icon"></div></div>
        </div>

        <div class="categoryList">
        </div>

        <div class="itemsList">
        </div>  
        
        <!--
        <div class="phrases">
            <div ph="1">TEST</div>
        </div>
        -->
    </div>
   
    
</asp:Content>