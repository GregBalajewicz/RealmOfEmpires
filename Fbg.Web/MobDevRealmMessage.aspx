<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MobDevRealmMessage.aspx.cs" Inherits="MobDevRealmMessage" %>

<%@ Register Src="Controls/ListOfRealms.ascx" TagName="ListOfRealms" TagPrefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title><%=RSc("RealmOfEmpires") %></title>

    
    <%if (isDevice == CONSTS.Device.iOS) { %>
    <meta name="viewport" content="width=320,inital-scale=1.0,maximum-scale=5.0,user-scalable=0">
    <%} else { %>
    <meta name="viewport" content="width=321"/>
    <%} %>

    <link href="main.2.css" rel="stylesheet" type="text/css" />
    <link href='https://fonts.googleapis.com/css?family=IM+Fell+French+Canon+SC' rel='stylesheet' type='text/css'>
    <link href="static/roe-ui.css" rel="stylesheet" type="text/css" />

    <script src="script-nochange/jquery-1.2.3.js" type="text/javascript"></script>

    <script src="script/countdown_old.js" type="text/javascript"></script>

    <script type="text/javascript">
        $(initTimers);
    </script>

    <style>
        #txtTitle {
            color: #FFD886;
        }

        html{
            position:fixed;
            width:100%;
            height:100%;
            overflow:hidden;
         }
         body{
            background-image: url(https://static.realmofempires.com/images/backgrounds/BGIntro.jpg);
            background-size: cover;
            background-position: center;
            background-repeat: no-repeat;
            background-attachment:fixed;
            height: 100%;
            width: 100%;
            margin: 0px;
            overflow-x:hidden;
            overflow-y:auto;
         }


	         #knightsL{
position: fixed;
pointer-events: none;
left: -295px;
top: 0px;
width: 100%;
height: 100%;
background-image: url(https://static.realmofempires.com/images/backgrounds/Warlords_Left.png);
background-size: auto 100%;
background-position: 50% 0px;
background-repeat: no-repeat;
         }
         #knightsR{
position: fixed;
pointer-events: none;
left: 295px;
top: 0px;
width: 100%;
height: 100%;
background-image: url(https://static.realmofempires.com/images/backgrounds/Warlords_Right.png);
background-size: auto 100%;
background-position: 50% 0px;
background-repeat: no-repeat;
         }

        #main {
position: relative;
width: 450px;
top: 45%;
left: 50%;
margin-left: -225px;
background-color: rgba(0, 0, 0, 0.8);
border-radius: 20px;
padding: 20px;
padding-top: 60px;
         }
            #main .mainContent {
                position: relative;
            }

         .roeLogo{
position: absolute;
left: 0px;
right: 0px;
top: -145px;
z-index: 1;
margin: 0 auto;
height: 196px;
background-image: url(https://static.realmofempires.com/images/d2test/HeaderWarlordsRising.png);
background-position: center;
background-repeat: no-repeat;
        }
         
         .holderbkg{
         position:absolute;
         left:0px;right:0px;bottom:0px;top:0px}
          .holderbkg .corner{position:absolute;background-size:100% 100%}
          .holderbkg .corner-top-left{width:35px;height:36px;left:0px;top:0px;background-image:url('https://static.realmofempires.com/images/forum/MainBox_TL.png')}
          .holderbkg .corner-top-right{width:35px;height:36px;right:0px;top:0px;background-image:url('https://static.realmofempires.com/images/forum/MainBox_TR.png')}
          .holderbkg .corner-bottom-left{width:35px;height:46px;left:0px;bottom:0px;background-image:url('https://static.realmofempires.com/images/forum/MainBox_BL.png')}
          .holderbkg .corner-bottom-right{width:35px;height:46px;right:0px;bottom:0px;background-image:url('https://static.realmofempires.com/images/forum/MainBox_BR.png')}
          .holderbkg .border{position:absolute;background-size:100% 100%}
          .holderbkg .lb-border-top{top:0px;left:35px;right:35px;height:36px;background-image:url('https://static.realmofempires.com/images/forum/MainBox_TC.png')}
          .holderbkg .lb-border-bottom{left:35px;right:35px;height:46px;bottom:0px;background-image:url('https://static.realmofempires.com/images/forum/MainBox_BC.png')}
          .holderbkg .lb-border-left{left:0px;top:36px;bottom:46px;width:35px;background-image:url('https://static.realmofempires.com/images/forum/MainBox_L.png')}
          .holderbkg .lb-border-right{right:0px;top:36px;bottom:46px;width:35px;background-image:url('https://static.realmofempires.com/images/forum/MainBox_R.png')}

        #btnBack {
            display: inline-block;
            height: 40px;
            width: 110px;
            text-align: center;
            box-sizing: border-box;
            padding: 0px;
            cursor: pointer;
            background-image: url("https://static.realmofempires.com/images/buttons/M_SP_Buttons.png");
            background-position: -100px -50px;
            overflow: hidden;
            font: 15px/1.0em "IM Fell French Canon SC", serif;
            color: #D7D7D7;
            text-shadow: 1px 1px 0 #000, -1px -1px 0 #000, 1px -1px 0 #000, -1px 1px 0 #000, 1px 1px 0 #000;
            border: none;
            background-color: rgba(0, 0, 0, 0);
            outline: none;
            text-decoration: none;
            padding-top: 11px;
        }
    </style>
</head>

<body>

    <div id="knightsR"></div>
    <div id="knightsL"></div>

    <div id=main >
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

        <div class="mainContent" style="text-align: center;">
            <div class="message fontSilverFrLClrg" style="margin-bottom:10px;">This realm is only accessible from the Realm of Empires Mobile App, currently available on Android, iOS and Kindle devices.</div>

            <div style="margin-bottom:10px;">
                <a class="mAdjustGooglePlay" target="google_play"  href="https://play.google.com/store/apps/details?id=com.realmofempires.roedroid&utm_source=global_co&utm_medium=prtnr&utm_content=Mar2515&utm_campaign=PartBadge&pcampaignid=MKT-AC-global-none-all-co-pr-py-PartBadges-Oct1515-1"><img alt="Get it on Google Play" style="width:135px;height: 40px;" src="https://play.google.com/intl/en_us/badges/images/apps/en-play-badge-border.png" /></a>
                <a href="https://itunes.apple.com/us/app/realm-of-empires/id596477295?mt=8&uo=4" target="itunes_store"style="display:inline-block;overflow:hidden;background:url(https://static.realmofempires.com/images/misc/downloadInAppStore.svg) no-repeat;width:135px;height:40px;@media only screen{background-image:url(https://static.realmofempires.com/images/misc/downloadInAppStore.svg);}"></a>
                <a target="_blank" href="http://www.amazon.com/BDA-Entertainment-Inc-Realm-Empires/dp/B00GCMQNSS/ref=sr_1_1?ie=UTF8&qid=1421439186&sr=8-1&keywords=realm+of+empires&pebp=1421439187163&peasin=B00GCMQNSS"><img src="https://images-na.ssl-images-amazon.com/images/G/01/AmazonMobileApps/amazon-apps-kindle-us-black.png" /></a>
            </div>

            <div style="margin-bottom:10px;">
                <a id="btnBack" href="logoutofrealm.aspx" > < Back</a>
            </div>

         </div>   
         
    </div>

</body>
</html>
