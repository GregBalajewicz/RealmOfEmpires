<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" MasterPageFile="~/main2.master" %>


<asp:Content ID="c1" ContentPlaceHolderID="cphMeta" runat="Server">
    <title><%=  RSc("GameName")%></title>

    <meta name="description" content="Serious strategy. Real gamers. Realm of Empires: Warlords Rising is a game like no other. 
            Guide your fledgling empire from its beginnings as a single small village. 
            Form alliances and clans, and work together to strengthen your hold on a hostile landscape. 
            By word or by sword, the realm is yours to conquer!" />
        
    <style>

        #banner {
            position: relative !important;
            transition: height 1s, width 1s;
            box-shadow: 0px 10px 20px rgba(0, 0, 0, 0.5);
        }
            .roeMainLogo {
                position: absolute;
                top: 50%;
                left: 50%;
                margin-top: -270px;
                margin-left: -277px;
                width: 554px;
                height: 311px;
                background-image: url(https://static.realmofempires.com/images/D2test/RoEBanner.png);
            }
            .enterButton {
                position: absolute;
                top: 50%;
                left: 50%;
                margin-top: 30px;
                margin-left: -99px;
                width: 189px;
                height: 88px;
                cursor: pointer;
                background-image: url("https://static.realmofempires.com/images/D2test/enterButton.png");
                text-decoration: none;
                margin-bottom: 15px;
            }



        .spacer{
            margin-bottom:20px;
        }
        .container {
            position:relative;
        }
            .container .block {
                position: relative;
                background-color: rgba(4, 23, 27, 0.95);
                border-radius: 20px;
                margin: 0 auto;
                margin-bottom: 20px;
            }
                .container .block .content {
                    position: relative;
                    padding: 30px;
                    overflow: hidden;
                }

        .textSmall {
            text-align: center;
            font-family: calibri light;
            font-size: 22px;
            color: #CBCBCB;
            text-shadow: 0px 1px 1px black;
        }

        .textBig {
            text-align: center;
            margin: 15px 0px;
            font-family: "IM Fell French Canon SC";
            font-size: 29px;
            color: #E6E1BD;
            text-shadow: 1px 1px #000, -1px 1px #000,1px -1px #000,-1px -1px #000, 0px 2px 2px #000;
        }

        .content .thumb {
            position: relative;
            display: inline-block;
            width: 160px;
            height: 90px;
            margin: 5px;
            cursor: pointer;
            background-size: cover;
            background-position: center;
            background-repeat: no-repeat;
            box-shadow: 0px 0px 3px rgba(0, 0, 0, 0.7);
        }
            .content .thumb:first-child {
                width: 550px;
                height: 300px;
            }

        .yoloBoxOuter {
            position: fixed;
            top: 0px;
            left: 0px;
            width: 100%;
            height: 100%;
            cursor:pointer;
            background-color: rgba(0, 0, 0, 0.9);
        }
            .yoloBoxOuter .thumb {
                position: absolute;
                top: 10%;
                left: 10%;
                width: 80%;
                height: 80%;
                cursor: pointer;
                background-size: 100% auto;
                background-repeat: no-repeat;
                background-position: center;
            }

    </style>


</asp:Content>

<asp:Content ID="c2" ContentPlaceHolderID="cph1" runat="Server">

    <div class="roeMainLogo"></div>
    <asp:HyperLink ID="linkEnter" CssClass="enterButton" runat="server" NavigateUrl="ChooseRealm.aspx"></asp:HyperLink>

    <div class="spacer"></div>
    <div class="container">

        <div class="block" style="width: 630px;">
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
            <div class="content" style="text-align:center;">
                <div class="textSmall">Serious strategy. Real gamers. Realm of Empires: Warlords Rising is a game like no other. 
                    Guide your fledgling empire from its beginnings as a single small village. 
                    Form alliances and clans, and work together to strengthen your hold on a hostile landscape. 
                    By word or by sword, the realm is yours to conquer!</div>
                <div class="textBig">Start small. Grow big. Conquer the realm!</div>
            </div>
        </div>
        
        <div class="block" style="width: 630px;">
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
            <div class="content" style="text-align:center;">
                <div class="thumb"style="background-image: url(https://static.realmofempires.com/images/misc/screenshot_01.jpg);"></div>
                <div class="thumb"style="background-image: url(https://static.realmofempires.com/images/misc/sceenshot_02.jpg);"></div>
                <div class="thumb"style="background-image: url(https://static.realmofempires.com/images/misc/screenshot_03.jpg);"></div>
                <div class="thumb"style="background-image: url(https://static.realmofempires.com/images/misc/screenshot_04.jpg);"></div>
                <div class="thumb"style="background-image: url(https://static.realmofempires.com/images/misc/screenshot_05.jpg);"></div>
                <div class="thumb"style="background-image: url(https://static.realmofempires.com/images/misc/screenshot_06.jpg);"></div>
                <div class="thumb"style="background-image: url(https://static.realmofempires.com/images/misc/screenshot_07.jpg);"></div>
            </div>
        </div>

    </div>
    

    <!-- Google Analutics Script-->
    <script>
        (function (i, s, o, g, r, a, m) {
            i['GoogleAnalyticsObject'] = r; i[r] = i[r] || function () {
                (i[r].q = i[r].q || []).push(arguments)
            }, i[r].l = 1 * new Date(); a = s.createElement(o),
            m = s.getElementsByTagName(o)[0]; a.async = 1; a.src = g; m.parentNode.insertBefore(a, m)
        })(window, document, 'script', '//www.google-analytics.com/analytics.js', 'ga');

        ga('create', 'UA-65069512-1', 'auto');
        ga('send', 'pageview');
    </script>

    <!-- Gallery Script-->
    <script>
        $(document).ready(function () {
            $('.thumb').click(thumbClick);

            $(document).ready(function () {

                //animate BG landsacpe
                setInterval(function () {
                    $('.coverBG').css({ 'background-position': '+=1px 50%' });
                }, 66);

            });

        });

        function thumbClick() {
            var yoloBox = $('<div class="yoloBoxOuter"></div>').click(function(){$(this).remove();});
            yoloBox.append($(this).clone());
            $('body').append(yoloBox);
        }
    </script>

</asp:Content>
