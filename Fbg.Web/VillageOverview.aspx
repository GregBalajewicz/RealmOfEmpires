<%@ Page Language="C#" MasterPageFile="~/main.master" AutoEventWireup="true" CodeFile="VillageOverview.aspx.cs"
    Inherits="VillageOverview" Trace="false" %>


<%@ Register Src="Controls/Tutorial.ascx" TagName="Tutorial" TagPrefix="uc3" %>

<asp:Content ID="header" ContentPlaceHolderID="HeadPlaceHolder" runat="server">
    <link type="text/json" rel="help" href="static/help/b_VOV.json.aspx" />
    <script type="text/javascript" src="script/troopDisplay.js"></script>
    <link href="static/vov2.css" rel="stylesheet" type="text/css" />

    <style>
        .troopPlusInfo
        {
            color: rgb(51, 255, 0);
            font-weight: normal;
            font-size: 10px;
            float: right;
        }
        .fbPromoPopupMask {
            position: fixed;
            left: 0px;
            top: 0px;
            width: 100%;
            height: 100%;
            background-color:rgba(0,0,0,0.6);
            z-index: 11111110;
        }
        .fbPromoPopup {
            position: fixed;
            left: 125px;
            top: 200px;
            width: 560px;
            height: 200px;
            opacity: 0.01;
            background-image: url('https://static.realmofempires.com/images/misc/promoBG01.png');
            z-index: 11111111;
            box-shadow: 0px 10px 10px rgba(0, 0, 0, 0.5);
            border-radius: 20px;
        }
            .fbPromoPopup .confirmButtonPromo {
                position: absolute;
                
                bottom: 37px;
                width: 111px;
                height: 40px;
                color: #FFD886;
                font: 18px/1.0em "IM Fell French Canon SC", serif;
                text-align: center;
                line-height: 38px;
                cursor: pointer;
                -webkit-text-stroke: 1px rgba(0,0,140,0.3);
            }
                .fbPromoPopup .confirmButtonPromo.accept {
                    left: 145px;
                }
                .fbPromoPopup .confirmButtonPromo.cancel {
                    left: 310px;
                }
                .fbPromoPopup .confirmButtonPromo:hover {
                    color: #C0A469;
                    line-height: 40px;
                }

            .fbPromoPopup .confirmButtonPromo div {
                position: absolute;
                top: 0px;
                height: 100%;
                background-size: 100% 100%;
            }
            .fbPromoPopup .confirmButtonPromo .buttonL {
                left: 0%;
                right: 80%;
                background-image: url("https://static.realmofempires.com/images/buttons/M_Btn2b_L.png");
            }
            .fbPromoPopup .confirmButtonPromo .buttonC {
                left: 20%;
                right: 20%;
                background-image: url("https://static.realmofempires.com/images/buttons/M_Btn2_C.png");
                text-shadow: 0px 0px 10px #000;
            }
            .fbPromoPopup .confirmButtonPromo .buttonR {
                left: 80%;
                right: 0%;
                background-image: url("https://static.realmofempires.com/images/buttons/M_Btn2b_R.png");
            }
            .fbPromoPopup .confirmButtonPromo:hover .buttonL {
                background-image: url("https://static.realmofempires.com/images/buttons/M_Btn2b_L2.png");
            }
            .fbPromoPopup .confirmButtonPromo:hover .buttonC {
                background-image: url("https://static.realmofempires.com/images/buttons/M_Btn2_C2.png");
                text-shadow: 0px 2px 6px #000;
            }
            .fbPromoPopup .confirmButtonPromo:hover .buttonR {
                background-image: url("https://static.realmofempires.com/images/buttons/M_Btn2b_R2.png");
            }
        .BoxAges {
            font-size: 11px;
            padding: 0 4px;
            background-color: rgb(69, 79, 80);
        }
        .BoxAges IMG{
            height: 20px;
            margin: 1px 0 -6px 0;
        }
        .BoxAges SPAN{
            margin-bottom: -11px;
            padding: 0 8px;
            height: 30px;
            display: inline-block;
        }
        .BoxAges SPAN:hover{
            text-decoration:underline;
            cursor:pointer;
        }
    </style>

    <script>      
        <asp:Literal runat="server" ID="roeentities" EnableViewState=false></asp:Literal>

        page.fbinit.push(function ()
        {
            var h24 = 1000 * 60 * 60 * 24; // quantity of ms in one day
            var lastCheck = localStorage.FbPromo_LastCheck;

            if (lastCheck == 'false') return;

            if (!lastCheck || (new Date().getTime() - lastCheck > h24))
            {
                FB.api('/me?fields=is_eligible_promo', function (response)
                {
                    if (!response || response.error) { return; }
                    if (response.is_eligible_promo)
                    {
                        localStorage.FbPromo_LastCheck = new Date().getTime();
                        $('<div>').addClass('fbPromoPopupMask').appendTo('body');
                        var promoImg = $('<div>').addClass('fbPromoPopup')
                            .append('<div class="confirmButtonPromo accept" onclick="location.href = \'pfCredits_fb2.aspx\';"><div class="buttonL"></div><div class="buttonC">Accept</div><div class="buttonR"></div></div>')
                            .append('<div class="confirmButtonPromo cancel" onclick="$(\'.fbPromoPopupMask\').remove();$(\'.fbPromoPopup\').remove();"><div class="buttonL"></div><div class="buttonC">Cancel</div><div class="buttonR"></div></div>');
                        promoImg.appendTo('body');
                        window.setTimeout(function ()
                        {
                            promoImg.fadeTo(2000, 1);
                        }, 2000);
                    } else
                    {
                        localStorage.FbPromo_LastCheck = 'false';
                    }
                });
            }
        });
    </script>
    <%
    if (isD2)
    {%>
        <script type="text/javascript"> 
            $(function () {
                $(".vov.researchLink").remove()
                $("#ContentPlaceHolder1_gift_new").remove()
                $("#lblCurRealm").parent().remove()
                $(".footer").remove()
                $("#ContentPlaceHolder1_tblGameNews").remove()
                $("#ContentPlaceHolder1_Table1").remove()
                $("#ContentPlaceHolder1_tblAge").remove()
                $(".Box.chat").remove()
                $($("#ContentPlaceHolder1_linkQuests").parents('table')[0]).remove()
                $("#ContentPlaceHolder1_CoE1_UpdatePanel2").remove()
            });

        </script>
   <%}%>


     <asp:Literal runat="server" ID="Analytics_GetMixPanelScript" EnableViewState=false></asp:Literal>
   
  
     

</asp:Content>

<asp:Content ID="c1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <table border="0" cellpadding="0" cellspacing="0" width="100%" >
        <tr>
            <td valign="top">
                <%if (!isMobile) { %>          
                 <center><asp:Label ID="Label1" runat="server" Text="" Visible="false" style="top:-10px;font-size:10pt;"></asp:Label></center>
                <span class="help" rel="bIncomingTroops" >
          
                </span>
                <br />
                <span class="help" rel="bOutgoingTroops" >
               
                </span>
                <%} %>
            </td>
            <td valign="top" style="width: 260px;" runat="server" id="otherInfo">
                <div style="padding-bottom: 1px;">
                </div>
                <div style="padding-bottom: 1px">
                    <table runat="server" id="Table1" border="0" cellpadding="0" cellspacing="0" class="Box" width="100%">
                        <tr>
                            <td class="BoxContent Padding pfs">
                                <asp:DataList EnableViewState="false" RepeatDirection="Horizontal" RepeatColumns="5" ID="dlPFs" runat="server" CellPadding="0" CellSpacing="0">
                                    <ItemTemplate>
                                        <a href="pfbenefits.aspx" class='<%#Eval("cssclass") %>' title="<%#Eval("tooltip") %>"></a>
                                    </ItemTemplate>
                                    <ItemStyle Wrap="false" CssClass="QuickSilverTransportTbl" />
                                </asp:DataList>
                            </td>
                        </tr>
                    </table>
                </div>
                <div style="padding-bottom: 0px">
                    <%if (FbgPlayer.Realm.NewDailyReward) {%>
                        <div id="gift_new" runat="server" class="dailyReward effect-pulse" style="position:absolute; left: 479px;top: 52px;width: 50px;height: 50px;background-image:url('https://static.realmofempires.com/images/icons/dailyBonus.png');" onclick="ROE.DailyReward.showPopup();"></div>   
                        <%} else { %>
                        <%} %>
                    
                </div>
                 

                <div style="padding-bottom: 1px">
                    <table border="0" cellpadding="0" cellspacing="0" class="Box" style="width: 100%">
                        <tr>
                            <td class="BoxHeader" nowrap>
                                <asp:HyperLink runat="server" ID="linkQuests" onclick="return !popupModalIFrame('Quests.aspx', 'popup', '', 600, 680, 30, 70, 'closeAndReload');" NavigateUrl="Quests.aspx" Text="Quests"></asp:HyperLink>
                            </td>                 

                            <td style="width: 1px;">
                            </td>
                            
                        </tr>
                    </table>
                </div>
                <div style="padding-bottom: 1px">
                    <table id="troopsTable" border="0" cellpadding="0" cellspacing="0" class="Box stripeTable help troops" rel="bTroopsTable" width="100%">
                        <tr>
                            <td class="BoxHeader TroopsHeader tabs">
                                <ul>
                                    <li class="selected" tab="normal">
                                        <%= RS("troops")%></li>
                                    <li class="det" tab="det">
                                        <%= RS("_details")%></li>
                                </ul>
                                <asp:HyperLink CssClass="troopPlusInfo" ID="troopPlusInfo" NavigateUrl="~/Gift_Use.aspx" Visible="false"  runat="server"  onclick="return !popupModalIFrame('gift_use.aspx', 'UseMyGifts', 'Gifts', 450, 450, 100, 50, 'closeAndReload');"></asp:HyperLink>
                            </td>
                        </tr>
                        <tr>
                            <td class="BoxContent list">
                                <div class="normal panel">
                                    <asp:DataList EnableViewState="false" RepeatDirection="Horizontal" RepeatColumns="4" ID="dlGifts" runat="server" CellPadding="0" CellSpacing="0">
                                        <ItemTemplate>
                                            <asp:HyperLink runat="server" NavigateUrl='<%# NavigationHelper.UnitHelp(FbgPlayer.Realm.ID, (int)Eval("UnitType.ID")) %>'> <img border="0" runat="server" src='<%#Eval("UnitType.IconUrl") %>' /><span class='<%#(int)Eval("YourUnitsCurrentlyInVillageCount") ==  0 ? "ZeroUnitCount" : ""%>'><%#((int)Eval("YourUnitsCurrentlyInVillageCount")).ToString("#,###0")%></span></asp:HyperLink>
                                        </ItemTemplate>
                                        <ItemStyle Wrap="false" CssClass="QuickSilverTransportTbl" />
                                    </asp:DataList>
                                </div>
                                <div class="det panel">
                                    <asp:Table CssClass="TypicalTable tlbHideSome" CellSpacing="1" CellPadding="0" ID="tblTroops" runat="server" Width="100%">
                                        <asp:TableRow ID="TableRow1" runat="server" CssClass="TableHeaderRow">
                                            <asp:TableCell ID="TableCell1" runat="server" ColumnSpan="1" RowSpan="2" VerticalAlign="Top"></asp:TableCell>
                                            <asp:TableCell CssClass="help" rel="bTotalCol" runat="server" HorizontalAlign="Center" VerticalAlign="Top" RowSpan="2"><%= RS("capTotal")%></asp:TableCell>
                                            <asp:TableCell ID="TableCell2" runat="server" ColumnSpan="2" HorizontalAlign="Center"><%= RS("yourTroops")%></asp:TableCell>
                                            <asp:TableCell CssClass="help" rel="bSupportCol" ID="TableCell4" HorizontalAlign="Center" VerticalAlign="Top" runat="server" RowSpan="2"><%= RS("support")%></asp:TableCell>
                                        </asp:TableRow>
                                        <asp:TableRow ID="txtTroopsTableRow2" runat="server" CssClass="TableHeaderRow">
                                            <asp:TableCell CssClass="help" rel="bInVillageNowCol" ID="TableCell5" runat="server" Width="55px" HorizontalAlign="Center"><%= RS("inVillage")%></asp:TableCell>
                                            <asp:TableCell CssClass="help" rel="bYourTroopsTotalCol" ID="TableCell6" runat="server" HorizontalAlign="Center"><%= RS("total")%></asp:TableCell>
                                        </asp:TableRow>
                                    </asp:Table>
                                </div>
                             </td>
                        </tr>
                    </table>
                </div>
                <div style="padding-bottom: 1px;">
                </div>
                <div style="padding-bottom: 1px;">
                <table runat="server" id="tblGameNews" border="0" cellpadding="0" cellspacing="0" class="Box" width="100%">
                    
                    <tr>
                        <td class="BoxContent Padding">
                        </td>
                    </tr>
                </table>
                </div>
                <div style="padding-bottom: 1px">
                    <table runat="server" id="tblAge" border="0" cellpadding="0" cellspacing="0" class="Box" width="100%" visible="false">
                        <tr>
                            <td class="BoxAges Padding">
                                <a ID="lblAge" runat="server" onClick="ROE.RealmAges.showPopup()"></a>
                            </td>
                        </tr>
                    </table>
                </div>
                <br />
                <asp:HyperLink onclick="return popupUnlock(this);" ID="linkPF" CssClass="LockedFeatureLink" runat="server"><%= RS("vilOVEnhance")%></asp:HyperLink>
            </td>
        </tr>
    </table>
   
   <div id='DivImageMap' runat="server">
    </div>
     
<div class="preload">
    <img src='https://static.realmofempires.com/images/misc/promoBG01.png' />    
    <img src='<%=RSic("pf_attack2")%>' />
    <img src='<%=RSic("pf_defence2")%>' />
    <img src='<%=RSic("pf_silvermore2")%>' />
    <img src='<%=RSic("pf_fasterreturn2")%>' />
    <img src='<%=RSic("pf_np2")%>' />
    <img src='<%=RSic("pf_map2")%>' />
</div>

</asp:Content>
