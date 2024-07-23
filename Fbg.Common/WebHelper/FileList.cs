using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace Fbg.Common.WebHelper
{
    /// <summary>
    /// Summary description for FileList
    /// </summary>
    public class FileList
    {

        public enum FOR
        {
            /// <summary>
            /// this is for the mobile app, main stateful page. this is only valid for mobile
            /// This is used by the main, stateful page when the game runs in mobile mode. 
            /// This masterpage will only be used when the game runs in mobile mode, ie, isMobile == true
            /// </summary>
            mMainPage,
            /// <summary>
            /// for main.Master - all main pages. mobile or desktop
            /// This is used my main.Master - page that vast majority of page use. these pages have the full header with village name, navigation etc.
            /// however, when the game runs in mobile, it sometimes uses pages which use the main.Master master page.
            /// Therefore, "mainPages" may be called with isMobile == true OR false
            /// </summary>
            mainPages,
            /// <summary>
            /// "fully-function" (ie, they know about SVID) popups.
            /// Used by :
            ///     master_PopupFullFunct.master 
            ///     AND masterPopupFullFunct_m.master
            /// The isMobile paramter tells you which master page this is - the "_m" is the mobile version
            /// </summary>
            popupfulFunct,
            /// <summary>
            /// used by the main frame page for desktop2
            /// </summary>
            mainPageDesktop2
        }

        enum FileType
        {
            js,
            css
        }


        /// <summary>
        /// returns you list of script blocks to add to your html
        /// </summary>
        static public string js(FileList.FOR forType, bool isMobile, bool isDebugOverride)
        {
            return js(forType, isMobile, isDebugOverride, false);
        }
        static public string js(FileList.FOR forType, bool isMobile, bool isDebugOverride, bool isAmazon)
        {
            bool isDebug = isDebugOverride || Config.UseDebugFiles;
            return GetHtmlByType(new ListType() { fileType = FileType.js, forType = forType, isMobile = isMobile, isDebug = isDebug, isAmazon = isAmazon });
        }
        static public string js(FileList.FOR forType, bool isMobile, bool isDebugOverride, bool isAmazon, System.Dynamic.ExpandoObject BetaCookies, bool isAdminOrTester)
        {
            bool isDebug = isDebugOverride || Config.UseDebugFiles;
            return GetHtmlByType(new ListType() { fileType = FileType.js, forType = forType, isMobile = isMobile, isDebug = isDebug, isAmazon = isAmazon, BetaCookies = BetaCookies, isAdminOrTester = isAdminOrTester });
        }
        /// <summary>
        /// returns you list of css include blocks to add to your html
        /// </summary>
        static public string css(FileList.FOR forType, bool isMobile, bool isDebugOverride)
        {
            bool isDebug = isDebugOverride || Config.UseDebugFiles;
            return GetHtmlByType(new ListType() { fileType = FileType.css, forType = forType, isMobile = isMobile, isDebug = isDebug});
        }
        static public string css(FileList.FOR forType, bool isMobile, bool isDebugOverride, System.Dynamic.ExpandoObject BetaCookies)
        {
            bool isDebug = isDebugOverride || Config.UseDebugFiles;
            return GetHtmlByType(new ListType() { fileType = FileType.css, forType = forType, isMobile = isMobile, isDebug = isDebug, BetaCookies = BetaCookies });
        }

        /// <summary>
        /// returns you list of script blocks to add to your html
        /// </summary>
        static public IEnumerable<string> jsCompileList(FileList.FOR forType, bool isMobile)
        {
            List<FileList.File> l = GetListByType(new ListType() { fileType = FileList.FileType.js, forType = forType, isMobile = isMobile, isDebug = true });

            var files = from file in l where file.doCompile select file.name;

            return files;
        }


        static List<FileList.File> generateList(FileType fileType, FileList.FOR forType, bool isMobile, bool isDebug, bool isAmazon, System.Dynamic.ExpandoObject BetaCookies, bool isAdminOrTester)
        {
            List<FileList.File> list;
            if (BetaCookies == null) {
                BetaCookies = new System.Dynamic.ExpandoObject();
            }

            if (fileType == FileType.js)
            {
                #region handle JS files
                list = new List<FileList.File>(isDebug ? 8 : 30);

                AddF("script-nochange/modernizr.custom.js", false, list);
                AddF(isDebug ? "https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.js" : "https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js", false, list);

                if (isDebug)
                {
                    if (forType == FOR.mMainPage || forType == FOR.mainPageDesktop2) { AddF(String.Format("script{0}/errorHandling.js", Config.jsScriptFileDirectoryPostfix), list); }
                    if (forType == FOR.mMainPage || forType == FOR.mainPageDesktop2) { AddF(String.Format("script{0}/StackTrace.js", Config.jsScriptFileDirectoryPostfix), list); }
                }
                
                AddF("script-nochange/jquery.cookie.js", false, list);
                AddF("script-nochange/jquery.json-2.2.js", false, list);
                AddF("script-nochange/jquery.tablesorter.min.js", false, list);
                AddF("https://ajax.googleapis.com/ajax/libs/jqueryui/1.11.0/jquery-ui.min.js", false, list);

                if (isMobile && isAmazon)
                {
                    AddF("https://amazon-web-app-resources.s3.amazonaws.com/v0/latest/Amazon-Web-App-API.min.js", false, list);
                    
                    if (isDebug)
                    {
                        AddF("script-nochange/amazon-web-app-api-tester.min.js", false, list);
                    }
                }

                if (isMobile || forType == FOR.mainPageDesktop2)
                {
                    AddF("script-nochange/createjs.js", false, list);
                }



                if (isDebug)
                {
                    AddF(String.Format("script{0}/interfaces.js", Config.jsScriptFileDirectoryPostfix), list);
                    AddF(String.Format("script{0}/bda.js", Config.jsScriptFileDirectoryPostfix), list);
                    AddF(String.Format("script{0}/bda-exception.js", Config.jsScriptFileDirectoryPostfix), list);
                    AddF(String.Format("script{0}/bda-utils.js", Config.jsScriptFileDirectoryPostfix), list);
                    AddF(String.Format("script{0}/bda-val.js", Config.jsScriptFileDirectoryPostfix), list);
                    AddF(String.Format("script{0}/bda-console.js", Config.jsScriptFileDirectoryPostfix), list);
                    if (isMobile || forType == FOR.mainPageDesktop2)  { AddF(String.Format("script{0}/bda-database.js", Config.jsScriptFileDirectoryPostfix), list);} // for now, we include for mob version only since it will cause issues on browsers without websql (ie9,ff) 
                    //if (isMobile || forType == FOR.mainPageDesktop2) { AddF(String.Format("script{0}/bda-database2.js", Config.jsScriptFileDirectoryPostfix), list); } // for now, we include for mob version only since it will cause issues on browsers without websql (ie9,ff) 
                    
                    AddF(String.Format("script{0}/bda-templates.js", Config.jsScriptFileDirectoryPostfix), list);
                    AddF(String.Format("script{0}/bda_dict.js.aspx", Config.jsScriptFileDirectoryPostfix), list);
                    AddF(String.Format("script{0}/tooltips.js.aspx", Config.jsScriptFileDirectoryPostfix), list);
                    AddF(String.Format("script{0}/bda-ui.js", Config.jsScriptFileDirectoryPostfix), list);
                    AddF(String.Format("script{0}/bda-ui-checkbox.js", Config.jsScriptFileDirectoryPostfix), list);
                    AddF(String.Format("script{0}/bda-ui-radio.js", Config.jsScriptFileDirectoryPostfix), list);
                    AddF(String.Format("script{0}/bda-ui-tabs.js", Config.jsScriptFileDirectoryPostfix), list);
                    if (isMobile || forType == FOR.mainPageDesktop2) { AddF(String.Format("script{0}/roe-device.js", Config.jsScriptFileDirectoryPostfix), list); }
                    AddF(String.Format("script{0}/roe-ui.js", Config.jsScriptFileDirectoryPostfix), list);
                    AddF(String.Format("script{0}/roe-ui-sounds.js", Config.jsScriptFileDirectoryPostfix), list);
                    AddF(String.Format("script{0}/roe-audio.js", Config.jsScriptFileDirectoryPostfix), list, BetaCookies, "AudioTools", "ON");

                    if (isMobile || forType == FOR.mMainPage) { AddF(String.Format("script{0}/bda-ui-touch.js", Config.jsScriptFileDirectoryPostfix), list); }
                    if (forType == FOR.mMainPage) 
                    {
                        AddF(String.Format("script{0}/bda-ui-anim.js", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("script{0}/bda-ui-viewport.js", Config.jsScriptFileDirectoryPostfix), list);
                    }
                    AddF(String.Format("script{0}/bda-ui-transition.js", Config.jsScriptFileDirectoryPostfix), list);
                    AddF(String.Format("script{0}/bda-broadcast.js", Config.jsScriptFileDirectoryPostfix), list);
                    AddF(String.Format("script{0}/bda-client.js", Config.jsScriptFileDirectoryPostfix), list);
                    AddF(String.Format("script{0}/menus_c.js", Config.jsScriptFileDirectoryPostfix), list);


                    if (isMobile)
                    {
                        AddF(String.Format("script{0}/menus_m.js", Config.jsScriptFileDirectoryPostfix), list);
                    }
                    else if (forType == FOR.mainPageDesktop2)
                    {
                        // NOTE! there is a mistake. it shoudl be menus_d2.js however this file was not created when this issue was noticed so it was left as-is for now
                        AddF(String.Format("script{0}/menus_m.js", Config.jsScriptFileDirectoryPostfix), list);
                    }
                    else
                    {
                        AddF(String.Format("script{0}/menus.4.js", Config.jsScriptFileDirectoryPostfix), list);
                    }



                    AddF(String.Format("script{0}/autopop.js", Config.jsScriptFileDirectoryPostfix), list);
                    AddF(String.Format("script{0}/misc-global-functions.js", Config.jsScriptFileDirectoryPostfix), list);
                    if (isMobile || forType == FOR.mainPageDesktop2)
                    {
                        AddF(String.Format("script{0}/bda-timer.js", Config.jsScriptFileDirectoryPostfix), list);
                    }
                    else
                    {
                        AddF(String.Format("script{0}/countdown_old.js", Config.jsScriptFileDirectoryPostfix), list);
                    }

                    AddF(String.Format("script{0}/help.js", Config.jsScriptFileDirectoryPostfix), list);

                    if (isMobile || forType == FOR.mainPageDesktop2)
                    {
                        AddF(String.Format("script{0}/clock2.js", Config.jsScriptFileDirectoryPostfix), list);
                    }
                    else
                    {
                    AddF(String.Format("script{0}/clock.js", Config.jsScriptFileDirectoryPostfix), list);
                    }

                    if (forType == FOR.mMainPage || forType == FOR.mainPages) { AddF(String.Format("script{0}/tutorial.js", Config.jsScriptFileDirectoryPostfix), list); } // we probably dont need to include this for FOR.mMainPage
                    AddF(String.Format("script{0}/misc.js", Config.jsScriptFileDirectoryPostfix), list);
                    AddF(String.Format("script{0}/roe-objectclick.js", Config.jsScriptFileDirectoryPostfix), list);
                    AddF(String.Format("script{0}/ROE_c.js", Config.jsScriptFileDirectoryPostfix), list);
                    if (forType == FOR.mMainPage || forType == FOR.mainPageDesktop2) { AddF(String.Format("script{0}/roe-classes.js", Config.jsScriptFileDirectoryPostfix), list); }
                    AddF(String.Format("script{0}/roe-api.js", Config.jsScriptFileDirectoryPostfix), list);
                    AddF(String.Format("script{0}/roe-utils.js", Config.jsScriptFileDirectoryPostfix), list);
                    if (isMobile)
                    {
                        AddF(String.Format("script{0}/ROE_d2orm.js", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("script{0}/ROE_m.js", Config.jsScriptFileDirectoryPostfix), list);
                    }
                    else if (forType == FOR.mainPageDesktop2)
                    {
                        AddF(String.Format("script{0}/ROE_d2orm.js", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("script{0}/ROE_d2.js", Config.jsScriptFileDirectoryPostfix), list);
                    }
                    else
                    {
                        AddF(String.Format("script{0}/ROE_d.js", Config.jsScriptFileDirectoryPostfix), list);
                    }
                    if (forType == FOR.mMainPage || forType == FOR.mainPageDesktop2) { AddF(String.Format("script{0}/ROE-Troops-InOutTroopsFilter.js", Config.jsScriptFileDirectoryPostfix), list); }
                    AddF(String.Format("script{0}/troops.3.js", Config.jsScriptFileDirectoryPostfix), list);
                    if (forType == FOR.mMainPage || forType == FOR.mainPageDesktop2) { AddF(String.Format("script{0}/roe-troops-InOut-OneVillageSummary.js", Config.jsScriptFileDirectoryPostfix), list); }
                    if (forType == FOR.mMainPage || forType == FOR.mainPageDesktop2) { AddF(String.Format("script{0}/roe-troops-InOut2.js", Config.jsScriptFileDirectoryPostfix), list); }
                    if (forType == FOR.mMainPage || forType == FOR.mainPageDesktop2) { AddF(String.Format("script{0}/roe-troops-InOutWidget.js", Config.jsScriptFileDirectoryPostfix), list); }
                    if (forType == FOR.mMainPage || forType == FOR.mainPageDesktop2) { AddF(String.Format("script{0}/roe-troops-InOutSummary.js", Config.jsScriptFileDirectoryPostfix), list); }
                    if (forType == FOR.mMainPage || forType == FOR.mainPageDesktop2) { AddF(String.Format("script{0}/roe-ui-footer.js", Config.jsScriptFileDirectoryPostfix), list); }
                    if (forType == FOR.mMainPage || forType == FOR.mainPageDesktop2) { AddF(String.Format("script{0}/roe-vov_2.js", Config.jsScriptFileDirectoryPostfix), list); }
                    if (forType == FOR.mainPageDesktop2) { AddF(String.Format("script{0}/roe-vov_2_d2.js", Config.jsScriptFileDirectoryPostfix), list); }


                    if (forType == FOR.mMainPage)
                    {
                        AddF(String.Format("script{0}/main_m.js", Config.jsScriptFileDirectoryPostfix), list);
                    }
                    else if (forType == FOR.mainPageDesktop2)
                    {
                        AddF(String.Format("script{0}/main_d2.js", Config.jsScriptFileDirectoryPostfix), list);
                    }
                    if ((forType == FOR.mainPages || forType == FOR.popupfulFunct) && isMobile) { AddF(String.Format("script{0}/main_forIframesOnMob.js", Config.jsScriptFileDirectoryPostfix), list); } // only for old pages running under mobile mod
                    AddF(String.Format("script{0}/roe-frame.js", Config.jsScriptFileDirectoryPostfix), list);
                    AddF(String.Format("script{0}/roe-FrameUtils.js", Config.jsScriptFileDirectoryPostfix), list);
                    if (forType == FOR.mMainPage || forType == FOR.mainPageDesktop2)
                    {
                        AddF(String.Format("script{0}/roe-LocalServerStorage.js", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("script{0}/roe-raids.js", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("script{0}/roe-player.js", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("script{0}/roe-playernew.js", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("script{0}/roe-avatar.js", Config.jsScriptFileDirectoryPostfix), list);
                        if (forType == FOR.mMainPage)
                        {
                            AddF(String.Format("script{0}/roe-frame_m.js", Config.jsScriptFileDirectoryPostfix), list);
                        }
                        else if (forType == FOR.mainPageDesktop2)
                        {
                            AddF(String.Format("script{0}/roe-frame_d2.js", Config.jsScriptFileDirectoryPostfix), list);
                        }
                        AddF(String.Format("script{0}/roe-village.js", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("script{0}/roe-villages.js", Config.jsScriptFileDirectoryPostfix), list);
                        //AddF(String.Format("script{0}/chat_m.js", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("script{0}/roe-map_2.js", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("script{0}/roe-mapevents.js", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("script{0}/roe-research2.js", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("script{0}/VoVAnimation.js", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("script{0}/roe-building2.js", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("script{0}/roe-quickbuild.js", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("script{0}/roe-quickrecruit.js", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("script{0}/roe-VillageResources.js", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("script{0}/roe-tutorial.js", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("script{0}/roe-reports.js", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("script{0}/roe-mail.js", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("script{0}/roe-credits.js", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("script{0}/roe-map-highlights.js", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("script{0}/roe-map-summary.js", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("script{0}/roe-ui-VillageOverview.js", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("script{0}/roe-gifts.js", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("script{0}/roe-commandtroops.js", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("script{0}/roe-ui-pfs.js", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("script{0}/roe-settingspopup.js", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("script{0}/roe-clanpopup.js", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("script{0}/roe-buildpopup.js", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("script{0}/roe-quickcommand.js", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("script{0}/roe-silvertransport.js", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("script{0}/roe-ui-villagelist.js", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("script{0}/roe-realtimeresource.js", Config.jsScriptFileDirectoryPostfix), list);
                        //AddF(String.Format("script{0}/roe-stats.js", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("script{0}/roe-account.js", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("script{0}/roe-items2.js", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("script{0}/roe-player-morale.js", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("script{0}/roe-targets.js", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("script{0}/roe-presetCommands.js", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("script{0}/roe-massactions.js", Config.jsScriptFileDirectoryPostfix), list);

                        /*if(isMobile)
                            AddF(String.Format("script{0}/roe-cordovainterface.js", Config.jsScriptFileDirectoryPostfix), list);   */
                    }

                    if (forType == FOR.mMainPage || forType == FOR.mainPages || forType == FOR.mainPageDesktop2)
                    {
                        AddF(String.Format("script{0}/roe-ui-govTypeSelect.js", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("script{0}/roe-dailyreward.js", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("script{0}/roe-restart.js", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("script{0}/roe-sleepmode.js", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("script{0}/roe-realmages.js", Config.jsScriptFileDirectoryPostfix), list);
                    }
                    if (forType == FOR.mainPages)
                    {
                        AddF(String.Format("script{0}/roe-bonusvillagechange.js", Config.jsScriptFileDirectoryPostfix), list);
                    }
                    if (forType == FOR.mMainPage || forType == FOR.mainPageDesktop2)
                    {
                        AddF(String.Format("script{0}/roe-bonusvillagechange2.js", Config.jsScriptFileDirectoryPostfix), list);
                    }
                    if (forType == FOR.mainPages) { AddF(String.Format("script{0}/header_d.js", Config.jsScriptFileDirectoryPostfix), list); }
                    AddF(String.Format("script{0}/bda-sessiondb.js", Config.jsScriptFileDirectoryPostfix), list);

                    if (forType == FOR.mMainPage || forType == FOR.mainPageDesktop2)
                    {
                    //adding chat js files
                    //AddF(String.Format("scripts/jquery.signalR-2.2.0.min.js", Config.jsScriptFileDirectoryPostfix), list);
                    //AddF(String.Format("signalr{0}/hubs", Config.jsScriptFileDirectoryPostfix), list);
                    //AddF(String.Format("script{0}/chat3.js", Config.jsScriptFileDirectoryPostfix), list);
                    AddF(String.Format("script{0}/roe-ui-chat2.js", Config.jsScriptFileDirectoryPostfix), list);

                    //webgl animation engine
                    AddF(String.Format("script{0}/pixi.js", Config.jsScriptFileDirectoryPostfix), list);
                    AddF(String.Format("script{0}/roe-animation.js", Config.jsScriptFileDirectoryPostfix), list);

                    AddF(String.Format("script{0}/roe-advisor.js", Config.jsScriptFileDirectoryPostfix), list);


                    }
                }
                else
                {
                    #region release files
                    // RELEASE VERSION
                    if (forType == FOR.mMainPage)
                    {
                        AddF(String.Format("script{0}/ROEmobile-min.js", Config.jsScriptFileDirectoryPostfix), false, list);
                    }
                    else if (forType == FOR.mainPageDesktop2)
                    {
                        AddF(String.Format("script{0}/ROE-mind2.js", Config.jsScriptFileDirectoryPostfix), false, list);
                    }
                    else
                    {
                        if (forType == FOR.mainPages)
                        {
                            if (isMobile)
                            {
                                AddF(String.Format("script{0}/ROE_m-min.js", Config.jsScriptFileDirectoryPostfix), false, list);
                            }
                            else
                            {
                                AddF(String.Format("script{0}/ROE-min.js", Config.jsScriptFileDirectoryPostfix), false, list);
                            }
                        }
                        else if (forType == FOR.popupfulFunct) 
                        {
                            if (isMobile)
                            {
                                AddF(String.Format("script{0}/ROEp_m-min.js", Config.jsScriptFileDirectoryPostfix), false, list);
                            }
                            else
                            {
                                AddF(String.Format("script{0}/ROEp-min.js", Config.jsScriptFileDirectoryPostfix), false, list);
                            }
                        }
                    }
                    AddF("script/bda_dict.js.aspx", list);
                    AddF("script/tooltips.js.aspx", list);
                    #endregion 
                }

                #endregion
            }
            else //if (fileType == FileType.css)
            {
                #region handle css files
                list = new List<FileList.File>(isDebug ? 15 : 15);

                    //AddF("https://static.realmofempires.com/fonts/CharlemagneStd-Bold.css", list);
                    AddF("https://fonts.googleapis.com/css?family=Eagle+Lake|IM+Fell+French+Canon+SC|IM+Fell+French+Canon+LC|Arapey", list);
                    AddF(String.Format("static{0}/main_c.css", Config.jsScriptFileDirectoryPostfix), list);
                    if (isMobile)
                    {
                        AddF(String.Format("static{0}/" + (isMobile ? "main_m.css" : "main_d.css"), Config.jsScriptFileDirectoryPostfix), list);
                    }
                    else if (forType == FOR.mainPageDesktop2)
                    {
                        AddF(String.Format("static{0}/main_d2.css", Config.jsScriptFileDirectoryPostfix), list);
                    }
                    else
                    {
                        AddF(String.Format("static{0}/main_d.css", Config.jsScriptFileDirectoryPostfix), list);
                    }
                    AddF(String.Format("static{0}/roe-VoV.css", Config.jsScriptFileDirectoryPostfix), list);
                    if (isMobile)
                    {
                        AddF(String.Format("static{0}/roe-VoV_m.css", Config.jsScriptFileDirectoryPostfix), list);
                    }
                    else if (forType == FOR.mainPageDesktop2)
                    {
                        AddF(String.Format("static{0}/roe-vov_d2.css", Config.jsScriptFileDirectoryPostfix), list);
                    }
                    AddF(String.Format("static{0}/menus_c.css", Config.jsScriptFileDirectoryPostfix), list);
                    if (isMobile)
                    {
                        AddF(String.Format("static{0}/menus_m.css", Config.jsScriptFileDirectoryPostfix), list);
                    }
                    else if (forType == FOR.mainPageDesktop2)
                    {
                        AddF(String.Format("static{0}/menus_d.css", Config.jsScriptFileDirectoryPostfix), list);
                    }
                    else
                    {
                        AddF(String.Format("static{0}/menus_d.css", Config.jsScriptFileDirectoryPostfix), list);
                    }

                    AddF(String.Format("static{0}/help.css", Config.jsScriptFileDirectoryPostfix), list);
                    AddF(String.Format("static{0}/jqueryui.css", Config.jsScriptFileDirectoryPostfix), list);
                    AddF(String.Format("static{0}/" + (isMobile ? "jqueryui_m.css" : "jqueryui_d.css"), Config.jsScriptFileDirectoryPostfix), list);
                    AddF(String.Format("static{0}/bda-ui.css", Config.jsScriptFileDirectoryPostfix), list);
                    AddF(String.Format("static{0}/bda-ui-checkmark.css", Config.jsScriptFileDirectoryPostfix), list);
                    AddF(String.Format("static{0}/bda-ui-radio.css", Config.jsScriptFileDirectoryPostfix), list);
                    if (isMobile || forType == FOR.mainPageDesktop2) { AddF(String.Format("static{0}/bda-ui-radio_m.css", Config.jsScriptFileDirectoryPostfix), list); }
                    if (forType == FOR.mMainPage) {
                        AddF(String.Format("static{0}/bda-ui-viewport.css", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("static{0}/bda-ui-scrolling.css", Config.jsScriptFileDirectoryPostfix), list);
                    }
                    AddF(String.Format("static{0}/bda-ui-transition.css", Config.jsScriptFileDirectoryPostfix), list);
                    AddF(String.Format("static{0}/roe-ui.css", Config.jsScriptFileDirectoryPostfix), list);

                    //M or D2 CSS
                    if (forType == FOR.mMainPage || forType == FOR.mainPageDesktop2)
                    {
                        AddF(String.Format("static{0}/roe-building2.css", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("static{0}/roe-quickbuild.css", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("static{0}/roe-silvertransport.css", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("static{0}/roe-quickrecruit.css", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("static{0}/roe-reports.css", Config.jsScriptFileDirectoryPostfix), list);
                        if (forType == FOR.mMainPage)
                        {
                            AddF(String.Format("static{0}/roe-reports_m.css", Config.jsScriptFileDirectoryPostfix), list);
                        }
                        else if (forType == FOR.mainPageDesktop2)
                        {
                            AddF(String.Format("static{0}/roe-reports_d2.css", Config.jsScriptFileDirectoryPostfix), list);
                        }
                        AddF(String.Format("static{0}/roe-report-details.css", Config.jsScriptFileDirectoryPostfix), list);
                        if (forType == FOR.mMainPage)
                        {
                            AddF(String.Format("static{0}/roe-report-details_m.css", Config.jsScriptFileDirectoryPostfix), list);
                        }
                        else if (forType == FOR.mainPageDesktop2)
                        {
                            AddF(String.Format("static{0}/roe-report-details_d2.css", Config.jsScriptFileDirectoryPostfix), list);
                        }
                        AddF(String.Format("static{0}/roe-mail.css", Config.jsScriptFileDirectoryPostfix), list);
                        if (forType == FOR.mMainPage)
                        {
                            AddF(String.Format("static{0}/roe-mail_m.css", Config.jsScriptFileDirectoryPostfix), list);
                        }
                        else if (forType == FOR.mainPageDesktop2)
                        {
                            AddF(String.Format("static{0}/roe-mail_d2.css", Config.jsScriptFileDirectoryPostfix), list);
                        }
                        AddF(String.Format("static{0}/roe-highlight.css", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("static{0}/roe-summary.css", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("static{0}/roe-VillageResources.css", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("static{0}/roe-credits.css", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("static{0}/roe-GiftsPopup.css", Config.jsScriptFileDirectoryPostfix), list);
                        if (forType == FOR.mMainPage)
                        {
                            AddF(String.Format("static{0}/roe-GiftsPopup_m.css", Config.jsScriptFileDirectoryPostfix), list);
                        }
                        else if (forType == FOR.mainPageDesktop2)
                        {
                            AddF(String.Format("static{0}/roe-GiftsPopup_d2.css", Config.jsScriptFileDirectoryPostfix), list);
                        }
                        AddF(String.Format("static{0}/roe-CommandTroopsPopup.css", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("static{0}/roe-PlayerNewPopup.css", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("static{0}/roe-avatar.css", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("static{0}/roe-PremiumFeatures.css", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("static{0}/roe-SettingsPopup.css", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("static{0}/roe-TroopInformation.css", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("static{0}/roe-VillagePopup.css", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("static{0}/roe-frame_c.css", Config.jsScriptFileDirectoryPostfix), list);
                        if (forType == FOR.mMainPage)
                        {
                            AddF(String.Format("static{0}/roe-frame_m.css", Config.jsScriptFileDirectoryPostfix), list);
                        }
                        else if (forType == FOR.mainPageDesktop2)
                        {
                            AddF(String.Format("static{0}/roe-frame_d2.css", Config.jsScriptFileDirectoryPostfix), list);
                        }
                        if (forType == FOR.mMainPage)
                        { AddF(String.Format("static{0}/chat_m.css", Config.jsScriptFileDirectoryPostfix), list); }
                        else if (forType == FOR.mainPageDesktop2)
                        { AddF(String.Format("static{0}/chat_d2.css", Config.jsScriptFileDirectoryPostfix), list); }
                        AddF(String.Format("static{0}/roe-map2.css", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("static{0}/roe-mapevents.css", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("static{0}/roe-ClanPopup.css", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("static{0}/roe-research2.css", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("static{0}/roe-QuickCommandPopup.css", Config.jsScriptFileDirectoryPostfix), list);
                        if (forType == FOR.mMainPage)
                        {
                            AddF(String.Format("static{0}/roe-QuickCommandPopup_m.css", Config.jsScriptFileDirectoryPostfix), list);
                        }
                        else if (forType == FOR.mainPageDesktop2)
                        {
                            AddF(String.Format("static{0}/roe-QuickCommandPopup_d2.css", Config.jsScriptFileDirectoryPostfix), list);
                        }
                        AddF(String.Format("static{0}/roe-ui-villagelist.css", Config.jsScriptFileDirectoryPostfix), list);
                        if (forType == FOR.mMainPage)
                        {
                            AddF(String.Format("static{0}/roe-ui-villagelist_m.css", Config.jsScriptFileDirectoryPostfix), list);
                        }
                        else if (forType == FOR.mainPageDesktop2)
                        {
                            AddF(String.Format("static{0}/roe-ui-villagelist_d2.css", Config.jsScriptFileDirectoryPostfix), list);
                        }
                        AddF(String.Format("static{0}/chat2.css", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("static{0}/roe-items.css", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("static{0}/roe-player-morale.css", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("static{0}/roe-targets.css", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("static{0}/roe-presetCommands.css", Config.jsScriptFileDirectoryPostfix), list);
                        AddF(String.Format("static{0}/roe-massactions.css", Config.jsScriptFileDirectoryPostfix), list);
                }

                    AddF(String.Format("static{0}/roe-BonusVillageChange.css", Config.jsScriptFileDirectoryPostfix), list);
                    AddF(String.Format("static{0}/roe-InOutTroops2.css", Config.jsScriptFileDirectoryPostfix), list);
                    if (forType == FOR.mMainPage)
                    {
                        AddF(String.Format("static{0}/roe-InOutTroops2_m.css", Config.jsScriptFileDirectoryPostfix), list);
                    }
                    else if (forType == FOR.mainPageDesktop2)
                    {
                        AddF(String.Format("static{0}/roe-InOutTroops2_d2.css", Config.jsScriptFileDirectoryPostfix), list);
                    }
                    AddF(String.Format("static{0}/roe-InOutTroopsPopup.css", Config.jsScriptFileDirectoryPostfix), list);
                    if (forType == FOR.mMainPage)
                    {
                        AddF(String.Format("static{0}/roe-InOutTroopsPopup_m.css", Config.jsScriptFileDirectoryPostfix), list);
                    }
                    else if (forType == FOR.mainPageDesktop2)
                    {
                        AddF(String.Format("static{0}/roe-InOutTroopsPopup_d2.css", Config.jsScriptFileDirectoryPostfix), list);
                    }
                    AddF(String.Format("static{0}/roe-troops-InOutTroopsFilter.css", Config.jsScriptFileDirectoryPostfix), list);
                    if (isMobile)
                    {
                        AddF(String.Format("static{0}/roe-troops-InOutTroopsFilter_m.css", Config.jsScriptFileDirectoryPostfix), list);
                    }
                    else if (forType == FOR.mainPageDesktop2)
                    {
                        AddF(String.Format("static{0}/roe-troops-InOutTroopsFilter_d2.css", Config.jsScriptFileDirectoryPostfix), list);
                    }
                    AddF(String.Format("static{0}/roe-troops-InOutSummary_c.css", Config.jsScriptFileDirectoryPostfix), list);
                    if (forType == FOR.mMainPage)
                    {
                        AddF(String.Format("static{0}/roe-troops-InOutSummary_m.css", Config.jsScriptFileDirectoryPostfix), list);
                    }
                    else if (forType == FOR.mainPageDesktop2)
                    {
                        AddF(String.Format("static{0}/roe-troops-InOutSummary_d2.css", Config.jsScriptFileDirectoryPostfix), list);
                    }
                
                    //AddF(String.Format("static{0}/roe-stats.css", Config.jsScriptFileDirectoryPostfix), list);
                    AddF(String.Format("static{0}/roe-account.css", Config.jsScriptFileDirectoryPostfix), list);

                    AddF(String.Format("static{0}/roe-advisor.css", Config.jsScriptFileDirectoryPostfix), list);

                    AddF(String.Format("static{0}/roe-raids.css", Config.jsScriptFileDirectoryPostfix), list);

                #endregion
            }
            return list;
        }


        static void AddF(string name, List<FileList.File> l)
        {
            AddF(name, true, l);
        }
        static void AddF(string name, bool doCompile, List<FileList.File> l)
        {
            l.Add(new File() { name = name, doCompile = doCompile });
        }
        static void AddF(string name, List<FileList.File> l, System.Dynamic.ExpandoObject BetaCookies, string cookieValue, string desiredState)
        {
            string storedValue = getNullcheckedFeatureValue(BetaCookies, cookieValue);
            if (storedValue == desiredState)
            {
                AddF(name, true, l);
            }

        }
        static string getNullcheckedFeatureValue(System.Dynamic.ExpandoObject BetaCookies, string featureName)
        {
            if (((IDictionary<String, object>)BetaCookies).ContainsKey(featureName))
            {
                var FeaturesByName = (IDictionary<string, object>)BetaCookies;
                return (string)FeaturesByName[featureName];
            }
            else
            {
                return "OFF";
            }

        }

        static private List<FileList.File> GetListByType(ListType type)
        {
            List<FileList.File> list =null;
            if (!listOfFileList.TryGetValue(type, out list))
            {
                AddDicEntry(type);
                list = listOfFileList[type];
                listOfFileListInHtml.Add(type, outputHtml(type));
            }

            return list;
        }
        static private string GetHtmlByType(ListType type)
        {
            List<FileList.File> list = GetListByType(type); // this will ensure proper HTML for this list is created

            try
            {
                return listOfFileListInHtml[type];
            }
            catch (Exception ex)
            {
                Gmbc.Common.Diagnostics.ExceptionManagement.BaseApplicationException bex = new Gmbc.Common.Diagnostics.ExceptionManagement.BaseApplicationException("error in GetHtmlByType", ex);
                type.SerializeToNameValueCollection(bex.AdditionalInformation);
                throw bex;
            }
        }

        static Dictionary<ListType, List<FileList.File>> listOfFileList;
        static Dictionary<ListType, string> listOfFileListInHtml;
        static FileList()
        {
            listOfFileList = new Dictionary<ListType, List<FileList.File>>();            
            listOfFileListInHtml = new Dictionary<ListType, string>(listOfFileList.Keys.Count);
        }


        static void AddDicEntry(FileList.ListType type )
        {
            listOfFileList.Add(type
                , generateList(type.fileType, type.forType, type.isMobile, type.isDebug, type.isAmazon, type.BetaCookies, type.isAdminOrTester));
        }


        static string outputHtml(ListType listtype)
        {
            List<FileList.File> l = GetListByType(listtype);

            StringBuilder sb = new StringBuilder(l.Count * 60);

            string format = listtype.fileType == FileType.js ? "<script src=\"{0}\" type=\"text/javascript\" onerror=\"ScriptErrorHandle(event)\"></script>\n"
                : "<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\" />\n";

            string ret = l.Aggregate(sb,
                 (sb2, v) => sb2.AppendFormat(format, v.name),
                 sb2 => sb2.ToString());

            return ret;
        }

        public static string GetStaticDir()
        {
            return String.Format("static{0}", Config.jsScriptFileDirectoryPostfix);
        }
        public static string GetScriptDir()
        {
            return String.Format("script{0}", Config.jsScriptFileDirectoryPostfix);
        }


        #region helper class
        private struct File
        {
            public string name;
            public bool doCompile;
        }
        private class ListType : Gmbc.Common.Diagnostics.ExceptionManagement.ISerializableToNameValueCollection2
        {
            public FileList.FileType fileType;
            public FileList.FOR forType;
            public bool isDebug;
            public bool isMobile;
            public bool isAmazon;
            public System.Dynamic.ExpandoObject BetaCookies;
            public bool isAdminOrTester;
            private System.Web.Script.Serialization.JavaScriptSerializer json_serializer;

            public ListType()
            {
                json_serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            }
            private string BetaCookiesHash
            {
                get
                {
                    return json_serializer.Serialize(BetaCookies);
                }
            }

            public override bool Equals(object obj)
            {
                if (obj == null) return false;
                return this == (ListType)obj;
            }

            static public bool operator ==(ListType x, ListType y)
            {
                if ((object)x == null && (object)y == null) return true;
                if ((object)x != null && (object)y != null)
                {
                    if (x.forType == y.forType
                        && x.isDebug == y.isDebug
                        && x.isMobile == y.isMobile
                        && x.fileType == y.fileType
                        && x.isAmazon == y.isAmazon
                        && x.BetaCookiesHash == y.BetaCookiesHash
                        ) return true;
                }
                return false;
            }

            static public bool operator !=(ListType x, ListType y)
            {
                if ((object)x == null && (object)y == null) return false;
                if ((object)x != null && (object)y != null)
                {
                    if (x.forType != y.forType
                        || x.isDebug != y.isDebug
                        || x.isMobile != y.isMobile
                        || x.fileType != y.fileType
                        || x.isAmazon != y.isAmazon
                        || x.BetaCookiesHash != y.BetaCookiesHash
                        ) return true;
                }
                else
                {
                    return true;
                }
                return false;
            }

            //
            // This is a terribly stupid implementation of gethashcode as it assumes the object's properties will never change. 
            //   they do not now, but nothing in the code prevents that ... would get good to change properties to read only
            //
            public int? _hashCode;
            public override int GetHashCode()
            {
                if (_hashCode == null)
                {
                    _hashCode = (forType.ToString() + isDebug.ToString() + fileType.ToString() + isAmazon.ToString() + BetaCookiesHash).GetHashCode();
                }
                return (int)_hashCode;
            }


            public void SerializeToNameValueCollection(System.Collections.Specialized.NameValueCollection col)
            {
                SerializeToNameValueCollection(col, "");
            }

            public void SerializeToNameValueCollection(System.Collections.Specialized.NameValueCollection col, string namePrePend)
            {
                try
                {
                    string pre = namePrePend;

                    if (col == null)
                    {
                        Gmbc.Common.Diagnostics.ExceptionManagement.ExceptionManager.Publish("Error in ListType.SerializeToNameValueCollection. argument 'col' is null");
                    }
                    else
                    {
                        col.Add(pre + "fileType", fileType.ToString());
                        col.Add(pre + "forType", forType.ToString());
                        col.Add(pre + "isDebug", isDebug.ToString());
                        col.Add(pre + "isMobile", isMobile.ToString());
                        col.Add(pre + "isAmazon", isAmazon.ToString());
                        col.Add(pre + "BetaCookiesHash", BetaCookiesHash);
                    }
                }
                catch (Exception e)
                {
                    Gmbc.Common.Diagnostics.ExceptionManagement.ExceptionManager.Publish("Error in ListType.SerializeToNameValueCollection. ExceptionManager.SerializeToString(e):" + Gmbc.Common.Diagnostics.ExceptionManagement.ExceptionManager.SerializeToString(e));
                }
            }
        }
        #endregion
  
    }
}
