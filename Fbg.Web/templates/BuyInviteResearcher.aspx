<%@ Page Title="" Language="C#" MasterPageFile="~/templates/masterPopupTemplate_m.master" AutoEventWireup="true" CodeFile="BuyInviteResearcher.aspx.cs" Inherits="templates_BuyInviteResearcher" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">

<%if (isMobile ) { %>
<style>
    #popup_buyres > div
    {
        width: 280px;
    }          
    #popup_buyres div.buydone
    ,#popup_buyres div.buyconfirm
	,#popup_buyres div.buydone.lackservants
    ,#popup_buyres div.button
    {
        height: 120px;
    }

</style>
<%} else {%>
<style>
    #popup_buyres > div
    {
        width: 400px;
    }  
    #popup_buyres div.buydone
    ,#popup_buyres div.buyconfirm
	,#popup_buyres div.buydone.lackservants
    ,#popup_buyres div.button
    {
        height: 100px;
    }
</style>
<%}%>

    <style>
       /*MOVE THESE STYLES TO .css file*/
       
	   	body {
			background: rgb(150, 140, 112); margin: 0px; padding: 0px; width: 100% !important; text-align: left; font-family: "lucida grande" , tahoma, verdana, arial, sans-serif; font-size: 11px;
		}
	   
        #popup_buyres
        {
            font: 20px/18px "IM Fell French Canon lc", serif;
			color: rgb(59, 35, 20);
        }
        #popup_buyres .small
        {
            font-size: 10pt;
        }

          
          
        #popup_buyres div.nottoday
        {
             cursor: pointer;
            
            margin-bottom: 2px;
            height: 30px;
            text-align: center;
            padding: 3px;
            vertical-align: middle;
			
background: #e5ddc4; /* Old browsers */
background: -moz-linear-gradient(top,  #e5ddc4 0%, #c6b699 92%, #958362 95%, #a69b7b 100%); /* FF3.6+ */
background: -webkit-gradient(linear, left top, left bottom, color-stop(0%,#e5ddc4), color-stop(92%,#c6b699), color-stop(95%,#958362), color-stop(100%,#a69b7b)); /* Chrome,Safari4+ */
background: -webkit-linear-gradient(top,  #e5ddc4 0%,#c6b699 92%,#958362 95%,#a69b7b 100%); /* Chrome10+,Safari5.1+ */
background: -o-linear-gradient(top,  #e5ddc4 0%,#c6b699 92%,#958362 95%,#a69b7b 100%); /* Opera 11.10+ */
background: -ms-linear-gradient(top,  #e5ddc4 0%,#c6b699 92%,#958362 95%,#a69b7b 100%); /* IE10+ */
background: linear-gradient(top,  #e5ddc4 0%,#c6b699 92%,#958362 95%,#a69b7b 100%); /* W3C */
filter: progid:DXImageTransform.Microsoft.gradient( startColorstr='#e5ddc4', endColorstr='#a69b7b',GradientType=0 ); /* IE6-9 */


        }

        #popup_buyres div.buydone
        {
            cursor: pointer;
            margin-bottom: 2px;
            text-align: center;
            padding: 3px;
            vertical-align: middle;
			
background: #958362; /* Old browsers */
background: -moz-linear-gradient(top,  #958362 0%, #a69b7b 92%, #958362 95%, #a69b7b 100%); /* FF3.6+ */
background: -webkit-gradient(linear, left top, left bottom, color-stop(0%,#958362), color-stop(92%,#a69b7b), color-stop(95%,#958362), color-stop(100%,#a69b7b)); /* Chrome,Safari4+ */
background: -webkit-linear-gradient(top,  #958362 0%,#a69b7b 92%,#958362 95%,#a69b7b 100%); /* Chrome10+,Safari5.1+ */
background: -o-linear-gradient(top,  #958362 0%,#a69b7b 92%,#958362 95%,#a69b7b 100%); /* Opera 11.10+ */
background: -ms-linear-gradient(top,  #958362 0%,#a69b7b 92%,#958362 95%,#a69b7b 100%); /* IE10+ */
background: linear-gradient(top,  #958362 0%,#a69b7b 92%,#958362 95%,#a69b7b 100%); /* W3C */
filter: progid:DXImageTransform.Microsoft.gradient( startColorstr='#958362', endColorstr='#a69b7b',GradientType=0 ); /* IE6-9 */

			
        }
		

		
        #popup_buyres div.buyconfirm
        {
            cursor: pointer;
            
            margin-bottom: 2px;
            text-align: center;
            padding: 3px;
            vertical-align: middle;
			
background: #958362; /* Old browsers */
background: -moz-linear-gradient(top,  #958362 0%, #a69b7b 92%, #958362 95%, #a69b7b 100%); /* FF3.6+ */
background: -webkit-gradient(linear, left top, left bottom, color-stop(0%,#958362), color-stop(92%,#a69b7b), color-stop(95%,#958362), color-stop(100%,#a69b7b)); /* Chrome,Safari4+ */
background: -webkit-linear-gradient(top,  #958362 0%,#a69b7b 92%,#958362 95%,#a69b7b 100%); /* Chrome10+,Safari5.1+ */
background: -o-linear-gradient(top,  #958362 0%,#a69b7b 92%,#958362 95%,#a69b7b 100%); /* Opera 11.10+ */
background: -ms-linear-gradient(top,  #958362 0%,#a69b7b 92%,#958362 95%,#a69b7b 100%); /* IE10+ */
background: linear-gradient(top,  #958362 0%,#a69b7b 92%,#958362 95%,#a69b7b 100%); /* W3C */
filter: progid:DXImageTransform.Microsoft.gradient( startColorstr='#958362', endColorstr='#a69b7b',GradientType=0 ); /* IE6-9 */




        }

		#popup_buyres div.buydone.lackservants,
        #popup_buyres div.button
        {
            cursor: pointer;
            margin-bottom: 2px;
            text-align: center;
            padding: 3px;
            vertical-align: middle;
			color: rgb(59, 35, 20);
			
background: #e5ddc4; /* Old browsers */
background: -moz-linear-gradient(top,  #e5ddc4 0%, #c6b699 92%, #958362 95%, #a69b7b 100%); /* FF3.6+ */
background: -webkit-gradient(linear, left top, left bottom, color-stop(0%,#e5ddc4), color-stop(92%,#c6b699), color-stop(95%,#958362), color-stop(100%,#a69b7b)); /* Chrome,Safari4+ */
background: -webkit-linear-gradient(top,  #e5ddc4 0%,#c6b699 92%,#958362 95%,#a69b7b 100%); /* Chrome10+,Safari5.1+ */
background: -o-linear-gradient(top,  #e5ddc4 0%,#c6b699 92%,#958362 95%,#a69b7b 100%); /* Opera 11.10+ */
background: -ms-linear-gradient(top,  #e5ddc4 0%,#c6b699 92%,#958362 95%,#a69b7b 100%); /* IE10+ */
background: linear-gradient(top,  #e5ddc4 0%,#c6b699 92%,#958362 95%,#a69b7b 100%); /* W3C */
filter: progid:DXImageTransform.Microsoft.gradient( startColorstr='#e5ddc4', endColorstr='#a69b7b',GradientType=0 ); /* IE6-9 */




        }
		#popup_buyres div.buydone.lackservants:hover,
        #popup_buyres div.button:hover,
         #popup_buyres div.nottoday:hover
        {

background: #958362; /* Old browsers */
background: -moz-linear-gradient(top,  #958362 0%, #a69b7b 92%, #958362 95%, #a69b7b 100%); /* FF3.6+ */
background: -webkit-gradient(linear, left top, left bottom, color-stop(0%,#958362), color-stop(92%,#a69b7b), color-stop(95%,#958362), color-stop(100%,#a69b7b)); /* Chrome,Safari4+ */
background: -webkit-linear-gradient(top,  #958362 0%,#a69b7b 92%,#958362 95%,#a69b7b 100%); /* Chrome10+,Safari5.1+ */
background: -o-linear-gradient(top,  #958362 0%,#a69b7b 92%,#958362 95%,#a69b7b 100%); /* Opera 11.10+ */
background: -ms-linear-gradient(top,  #958362 0%,#a69b7b 92%,#958362 95%,#a69b7b 100%); /* IE10+ */
background: linear-gradient(top,  #958362 0%,#a69b7b 92%,#958362 95%,#a69b7b 100%); /* W3C */
filter: progid:DXImageTransform.Microsoft.gradient( startColorstr='#958362', endColorstr='#a69b7b',GradientType=0 ); /* IE6-9 */


        }
        
        #popup_buyres div.button.buy img,
        #popup_buyres div.button.invite img,
        #popup_buyres div.buyconfirm img,
        #popup_buyres div.buydone img
        {
            float: left;
            height: 80px;
        }
        
        #popup_buyres div.buyconfirm
        , #popup_buyres div.buydone
        {
            display:none;
        }

        #popup_buyres div.button.invite a
        {
            display: block;
            width: 100%;
            height: 100%;
            color: rgb(59, 35, 20);
        }
        
        #popup_buyres div.button.invite a:hover
        ,#popup_buyres div.buydone a:hover
        {
            text-decoration: none;
        }

        #popup_buyres .buyconfirm .yes,
        #popup_buyres .buyconfirm .cancel
        {
            cursor: pointer;
        }
    </style>
    
    <span id="popup_buyres">
        <%if (!isMobile) { %>
        <div class="button invite">
            <a target="_parent" href="invite.aspx?r=1">
                <img src="https://static.realmofempires.com/images/misc/notice.png" border="0" />
                <BR />Invite Friend to join your panel of esteemed researchers </a>
        </div>
        <%} %>

        <div class="button buy">
            <img src="https://static.realmofempires.com/images/misc/tutorial_advisor_crop.png" />
            Hire researcher for
            <%=BuyCost() %>
            Servants
            <br />
            <span class="small">
                <br />
                This gives you 1 extra researcher on this realm
                <br />
                
        </div>
        <div class="buyconfirm">
            <img src="https://static.realmofempires.com/images/misc/faq.png" />
            Buy 1 researcher for
            <%=BuyCost() %>
            Servants?
            <br /><br />
            <span class=yes>YES</span>  <span class="small cancel">- cancel</span>
        </div>
        <div class="buydone lackservants">       
            <img src="https://static.realmofempires.com/images/misc/redX.png" />     
            Hire Failed.
            <BR />Not enough servants to hire a researcher.
            </BR></BR><a class="sfx2 buymore customButtomBG" onclick="closeMe();ROE.Credits.showPopup()" ><%=RSc("HireServant") %></a>
            
        </div>
        <div class="buydone maxres">       
            <a target=_parent href=pfcredits.aspx>
            <img src="https://static.realmofempires.com/images/misc/redX.png" />     
            Hire Canceled.
            <BR />Maximum number of researchers already reached. Congrats!
            </a>
        </div>
        <div class="buydone otherErr">       
            <img src="https://static.realmofempires.com/images/misc/redX.png" />     
            Opps! Something went wrong... 
            <BR />Please close the window and try again
        </div>
        <div class="buydone ok">       
            <a target=_parent href=pfcredits.aspx>
            <img src="https://static.realmofempires.com/images/misc/yesGreen.PNG" />     
            <BR />Researcher Hired!
             <span class="small">
                <br />
                Close the research window for the changes to come into effect
                <br />
            </a>
        </div>
        
        <div class="nottoday"><center><span class=closeMsg>Not today, thanks.</span></center></div>
    </span>

<script><%=IsInDesignMode ? "ROE.Api.apiLocationPrefix = '../';" : "" %></script>
<script src="<%=IsInDesignMode ? "" : "templates/" %>BuyInviteResearcher.js" type="text/javascript"></script>
</asp:Content>

