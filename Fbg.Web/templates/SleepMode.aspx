<%@ Page Title="" Language="C#" MasterPageFile="~/templates/masterPopupTemplate_m.master" AutoEventWireup="true" CodeFile="SleepMode.aspx.cs" Inherits="templates_SleepMode" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">

<style>

    #popup_SleepMode {
        position:fixed !important;
        width: 100%;
        height: 100%;
        background: url('https://static.realmofempires.com/images/backgrounds/M_BG_Sleep.jpg');
        background-size: cover;
        -webkit-box-shadow: 0px 0px 6px 6px rgba(0, 0, 0, 0.5);
        box-shadow: 0px 0px 6px 6px rgba(0, 0, 0, 0.5);
        left: 0px !important;
    }
        #popup_SleepMode .popupBody {
            position: relative;
            overflow: hidden;
            height: 100%;
            width: 100%;
        }

    #popup_SleepMode .title {
        border: 2px solid #846824;
        -webkit-border-radius: 15px;
        border-radius: 15px;
        height:22px;
        color: #FFD776;
        font: 20px/1.0em "IM Fell French Canon SC", serif;
        text-align: center;
        margin:8px;
        text-shadow: 0 0 4px #000000;
        background-color:black;
        padding: 2px 3px;
    }
    #popup_SleepMode .title:before {
        float: left;
        content: " ";
        margin-left: -9px;
        margin-top: -12px;
        width: 22px;
        height: 31px;
        background: url("https://static.realmofempires.com/images/misc/M_BoxTLGold.png") no-repeat;
    }
    #popup_SleepMode .titleClose {
        position: absolute;
        left: auto;
        top: -6px;
        right: 1px;
        bottom: auto;
        width: 44px;
        height: 44px;
        background-image: url("https://static.realmofempires.com/images/icons/M_X.png");
        background-repeat: no-repeat;
    }
    #popup_SleepMode .title .shadedBg {
        width:100%;
        height:100%;
        margin-top:-20px;
        border-radius: 15px;
        background-image: -webkit-gradient(linear, left top, right top, color-stop(0%, rgba(234, 234, 234, 0.5)), color-stop(100%, rgba(71, 71, 71, 0.5)));
        background-image: -webkit-linear-gradient(left, rgba(234, 234, 234, 0.5) 0%, rgba(71, 71, 71, 0.5) 100%);
        background-image: -moz-linear-gradient(left, rgba(234, 234, 234, 0.5) 0%, rgba(71, 71, 71, 0.5) 100%);
        background-image: -ms-linear-gradient(left, rgba(234, 234, 234, 0.5) 0%, rgba(71, 71, 71, 0.5) 100%);
        background-image: -o-linear-gradient(left, rgba(234, 234, 234, 0.5) 0%, rgba(71, 71, 71, 0.5) 100%);
        background-image: linear-gradient(to right, rgba(234, 234, 234, 0.5) 0%, rgba(71, 71, 71, 0.5) 100%);
    }

    #SleepModeBox {
        position: relative;
        height: 100%;
        width: 100%;
        overflow: auto;
        text-align: center;
        color: #FFF;
        font: 15px "IM Fell French Canon LC", serif;
    }

        #SleepModeBox .setContainer {
            position: relative;
            background-color: rgba(0, 0, 0, 0.8);
            margin-bottom: 10px;
            padding: 10px;
        }

        #SleepModeBox .infoBtn {
            position:relative;
            margin: 0 auto;
        }

        #SleepModeBox .section {
            display: none;
            position: relative;
        }
                #SleepModeBox .section .text.reactivateDate {
                    display: none;
                }

        #SleepModeBox .text {
            margin-bottom: 5px;
        }
        #SleepModeBox .red {
            color: #ff4444;
            text-shadow: 0px 1px black, 0px -1px black;
        }
        #SleepModeBox .hl {
            color:lime;
        }
        #SleepModeBox .addmargin {
            margin: 20px;
        }

        #SleepModeBox .customBtn {
            position: relative;
            width: 240px;
            height: 42px;
            margin: 0 auto;
            margin-bottom: 5px;
            overflow: hidden;
            color: #FFD886;
            font: 17px "IM Fell French Canon SC", serif;
            text-align: center;
            line-height: 38px;
            cursor:pointer;
            text-shadow: 0px 3px 3px #000, 0px -3px 3px #000, -4px 0px 3px #000, 4px 0px 3px #000;
            background: url('https://static.realmofempires.com/images/buttons/M_Btn2b_L.png'), 
                url('https://static.realmofempires.com/images/buttons/M_BtnL_P.png'), 
                url('https://static.realmofempires.com/images/buttons/M_Btn2b_R.png');
            background-size: 26px 100%, 188px 100%, 26px 100%;
            background-position: left top, center top, right top;
            background-repeat: no-repeat;
        }

            #SleepModeBox .customBtn.areyousure {
                color: #FF1D1D !important;
                text-shadow: 0px 1px 1px #000, 0px -1px 1px #000, -1px 0px 1px #000, 1px 0px 1px #000;
            }

        #SleepModeBox #vacationDuration {
            width: 40px;
            margin-left: 5px;
        }

        #SleepModeBox .countdown {
            color: #FFD776;
            font: 19px "IM Fell French Canon LC", serif;
        }

    #SleepModeBox .phrases{
        display: none;
    }
    #SleepModeBox .ON, #SleepModeBox .SleepHelpBox {
        display:block;
    }

        #SleepModeBox .infoPanel {
            display: none;
            position: absolute;
            top: 0px;
            bottom: 0px;
            left: 0px;
            right: 0px;
            padding: 20px;
            background-color: rgba(0, 0, 0, 0.95);
            overflow: auto;
            text-align: left;
            cursor: pointer;
        }


        #SleepModeBox .wmRequestPanel {
            display: none;
            position: absolute;
            top: 0px;
            bottom: 0px;
            left: 0px;
            right: 0px;
            padding: 20px;
            background-color: rgba(0, 0, 0, 0.95);
            overflow: auto;
            text-align: left;
            z-index: 1;
        }

            #SleepModeBox .wmRequestPanel .close {
                position: absolute;
                top: 5px;
                right: 5px;
                width: 33px;
                height: 33px;
                background-image: url(https://static.realmofempires.com/images/icons/M_X33.png);
                background-size:100% 100%;
                cursor:pointer;
            }

            #SleepModeBox .wmRequestPanel .wmActivationInputWrapper {
                margin: 10px 0px;
                text-align: center;
            }

            #SleepModeBox .wmRequestPanel .timeLabel {
                text-align: center;
                margin-bottom: 20px;
            }



    
</style>
   
    <div class="title">
        <%=RS("SleepMode") %>
        <div class="shadedBg"></div>
        <div class="titleClose pressedEffect" onclick="ROE.UI.Sounds.click();"></div>
    </div>

    
    <div id="SleepModeBox">        

        <div class="setContainer sleepContainer">
            <div class="section set1" >
                <div class="text" >
                    <%=RS("Youwillbeinsleepmode") %><div class='countdown' ></div>
                    <%=RS("Thesleepmodeholds") %>
                </div>
            </div>
        
            <div class="section set2" >
                <div class="text" >
                    <%=RS("Sleepmodewillbeavailablein") %>
                    <div class="countdown" ></div>
                </div>
            </div>

            <div class="section set3" >
                <div class="text" >
                    <div class="fontGoldFrLCXlrg" >Sleep Mode</div>
                    <div><%=RS("SleepModeProtectYou") %></div>
                    <div><%=RS("Thesleepmodeholds") %></div>
                </div>
                <div class='customBtn activateSleep' onclick="ROE.UI.Sounds.click();" ></div>
            </div>

            <!-- Sleep Info btn-->
            <div class="BtnDLf1 fontButton1L infoBtn infoBtnSleep">More Info</div>           
        </div>


        <!-- WEEKEND MODE SECTION -->
        <div class="setContainer weekendModeContainer">
            <!-- WM - not requested -->
            <div class="section setWM-NR" >
                <div class="text fontGoldFrLCXlrg" >Weekend Mode</div>
                <div class="text wmReactivateDate red" >Can't Reactivate untill: <br />%wmReactivateAllowedDate%</div>
                <div class='customBtn sfx2 openSetupWM' >Setup Weekend Mode</div>
            </div>

            <!-- WM - requested-->
            <div class="section setWM-R" >
                <div class="text fontGoldFrLClrg" >Weekend Mode</div>
                <div class="text wmTakesEffectOn" >Takes effect on: %wmTakesEffectOn%</div>
                <div class="text wmEndsOn" >Ends on: %wmEndsOn%</div>
            </div>

            <!-- WM Info btn-->
            <div class="BtnDLf1 fontButton1L infoBtn infoBtnWeekendMode">More Info</div>  

        </div>

        <!-- WM Request Panel -->
        <div class="wmRequestPanel" >
            <div class="text fontGoldFrLClrg" >Weekend Mode Setup</div>
            <div class="text" >Setup your Weekend Mode here. Pick a time and date to enter protection.</div>
            <div class="text wmDurationDays" >Weekend mode can last %wmDurationDays% days, unless you cancel it early.</div>
            <div class="text wmActivationDelay" >Minimum time before it can take effect is in %wmActivationDelay% hours.</div>
            
            <div class="addmargin" ></div>

            <div class="text" >I want to enter Weekeend mode on:</div>
            <div class="wmActivationInputWrapper">Time: <input class="wmTimePicker" type="text" /></div>
            <div class="wmActivationInputWrapper">Date: <input class="wmDatePicker" type="text" /></div>
            <div class="timeLabel">(Game Time / UTC)</div>
            <div class='customBtn sfx2 requestWM' >Request Weekend Mode</div>
            <div class="close"></div>
        </div>
        <!-- WEEKEND MODE SECTION -->


        <div class="setContainer vacationContainer">
            <!-- Vacation Mode - not requested -->
            <div class="section set4" >
                <div class="text fontGoldFrLCXlrg" >Vacation Mode</div>
                <div class="text vacationDaysTotal" >Total days: %vacationDaysTotal%</div>
                <div class="text vacationDaysLeft" >Available days: %vacationDaysLeft%</div>
                <div class="text reactivateDate red" >Can't Reactivate untill: <br />%reactivateAllowedDate%</div>
                <div class='customBtn activateVacation' onclick="ROE.UI.Sounds.click();" >Request Vacation Mode</div>
            </div>

            <!-- Vacation Mode - requested-->
            <div class="section set5" >
                <div class="text fontGoldFrLClrg" >Vacation Mode</div>
                <div class="text vacationRequestedOn" >Requested on: %vacationRequestedOn%</div>
                <div class="text vacationTakesEffectOn" >Takes effect on: %vacationTakesEffectOn%</div>
                <div class="text vacationEndsOn" >Ends on: %vacationEndsOn%</div>
                <div class="timeLabel">(Game Time / UTC)</div>
            </div>

            <!-- Vacation Info btn-->
            <div class="BtnDLf1 fontButton1L infoBtn infoBtnVacation">More Info</div>  
        </div>
        
        <div class="infoPanel SleepHelpBox">          
            <%=RS("Info1") %>
            <li><%=RS("Info2") %></li>
            <li><%=RS("Info3") %></li>
            <li><%=RS("Info4") %></li>
            <li><%=RS("Info5") %></li>        
            <%=RS("Info6") %>
        </div>

        <div class="infoPanel VacationHelpBox">          
            <div>VacationMode allows you to step away from the game for an extended period, and be safe from attacks.</div><br />
            <div style="color: #E63131;">Attacks launched before your VacationMode takes effect, will still harm you!</div><br />
            <div>According to realm parameters, plus your own account's global XP, you get a set number of days you can use for VacationMode.</div><br />
            <div>In this realm:</div><br />
            <div>It takes %delay% days from your request date for VacationMode to become active.</div><br />
            <div>Minimum deducation per activation, even if canceled early, is %PerUseMinimum% days.</div><br />
            <div>Your next vacation mode protection can last a maximum of %PerUseMaximum% days.</div><br />
            <div>Once a vacation ends or is canceled, it takes %reactivation% days to allow reactivation.</div><br />
        </div>

        <div class="infoPanel WeekendModeHelpBox">          

            <div class="text fontGoldFrLCXlrg" >Weekend Mode</div>
            <div class="addmargin" ></div>

            <div class="text" >Weekend mode allows you an extended break from the game. Similar to sleep mode, but for longer periods.</div>
            <div class="text red" >Attacks launched before your Weekend Mode takes effect, will still harm you!</div><br />
            <div class="text" >In this realm Weekend mode can last <span class="hl">%wmDurationDays% days</span>.</div>
            <div class="text" >You have to set Weekend Mode a minimum of <span class="hl">%wmActivationDelay% hours</span> ahead of time.</div>
            <div class="text" >When Weekend Mode ends or is canceled, it cant be reactivated again for <span class="hl">%wmReactivationDelayDays% days</span>.</div>

            <!--
            <div class="addmargin" ></div>
            <div class="text" >SleepMode / WeekendMode Interaction Note:</div>
            <div class="text red" >Can not enter Weekend Mode within 16 hours Sleep Mode activation</div>
            <div class="text red" >Can not enter Sleep Mode within 16 hours of the time Weekend Mode is set to be activated.</div>
            -->

        </div>

        <div class="phrases">
            <div ph="1"><%=RS("ActivateHours") %></div>
            <div ph="2"><%=RS("ActivateMins") %></div>
        </div>

    </div>
</asp:Content>
