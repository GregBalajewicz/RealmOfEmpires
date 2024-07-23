using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Fbg.Bll;

public partial class Controls_Tutorial : BaseControl
{
    Fbg.Bll.Village _selectedVillage;

    public Controls_Tutorial()
    {
        BaseResName = "TutorialText.ascx"; 
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        #region Databinds
        if (!IsPostBack)
        {
            this.lblPercentage.DataBind();
            this.Menu4.DataBind();

            Menu4.Items[0].Text = RS("Back");
            Menu4.Items[0].ChildItems[0].Text = RS("Section1VillageOverview");
            Menu4.Items[0].ChildItems[1].Text = RS("Section2BuildingUpgrades");
            Menu4.Items[0].ChildItems[2].Text = RS("Section3Map");
            Menu4.Items[0].ChildItems[3].Text = RS("Section4Troops");
            Menu4.Items[0].ChildItems[4].Text = RS("FromBeginning");
        }
        #endregion
    } 

    private static class Consts
    {
        public static string CookieName = "Tutorial";
        public static string Cookie_isRunning = "isRunning";
        public static string Cookie_isRunning_Yes = "1";
        public static string Cookie_isRunning_No = "0";
        public static string Cookie_curStep = "curStep";
        public static string CookieTutorialPosName = "tutPos";

        //public static string CurrentStepText = RS("Step") + " {0}/5 - {1}";
        

        public static class Pages
        {
            public static string VillageOverview = "VillageOverview.aspx";
            public static string SilverMine = "building.aspx?bid=5";
            public static string VillageBuildings = "VillageBuildings.aspx";
            public static string Upgrade= "Upgrade.aspx";
            public static string Map = "map.aspx";
            public static string VillageOverviewOther = "VillageOverviewOther.aspx";
            public static string PlayerOverview = "Player.aspx";
        }

    }

    /// <summary>
    /// The tutorial displays these steps in this order. 
    /// </summary>
    public enum Steps : int
    {
        notRunning = 0
        ,
       // Step0_BookmarkApp
      //,
        Step1_Welcome
      ,
        Step_VilOverview0
      ,
        Step_VilOverview1
        ,
        Step_VilOverview2
        ,
        /// <summary>
        /// in silver mine
        /// </summary>
        Step_UpgradeBuildings_InSilverMine 
       ,
        Step_UpgradeBuildings_InSilverMine2
      ,
        Step_UpgradeBuildings_Finish
     ,
        Step_MapLeadIn,
        Step_Map1
      ,
        Step_Map1_1
      ,
        Step_Map1_2
      ,
        Step_Map2
  ,
        Step_Map2_1
  ,
        Step_Map_PlayerOverview
      ,
    //    Step_Stats_VilName
    //  ,
    //    Step_Stats_Silver
    //  ,
    //    Step_Stats_Silver2
    //  ,
    //    Step_Stats_Silver4
    //   ,
    //    Step_Stats_Food1
    //,
    //    Step_Stats_Food3
    //    ,
        Step_Army1
      ,
        Step_Army1_1
      ,
        Step_Army2

       , Done
    }
    
    private Steps currentStep = Steps.notRunning;

    /// <summary>
    /// You may pass in null for selectedVillage if tutorial is not running as
    ///  then it will not be used
    /// </summary>
    /// <param name="selectedVillage"></param>
    /// <returns></returns>
    public bool DisplayCurrentStep(Fbg.Bll.Village selectedVillage)
    {
        this._selectedVillage = selectedVillage;
        return DisplayCurrentStep();

    }

    /// <summary>
    /// Hides the tutorial so that it is not displayed 
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    public void Hide()
    {
        this.Visible = false;
    }


    /// <summary>
    /// returns true if tutorial is running 
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    private bool DisplayCurrentStep()
    {
        string page = Request.Url.Segments[Request.Url.Segments.Length - 1];
        int level;
        Village.CanUpgradeResult canUpgrade;
        imgAdvisor.Visible = true;

        BuildingTypeLevel nextUpgradeLevel=null;

        if (!IsPostBack && _selectedVillage != null)
        {
            if (_selectedVillage.owner.HasFlag(Fbg.Bll.Player.Flags.Advisor_BeginnerTutorialCompleted) != null)
            {
                panelTutNav.Visible = true;
            }
        }

        if (isRunning)
        {

            //
            // do any movement between steps here. 
            switch (currentStep)
            {
                case Steps.Step_VilOverview2:
                    if (Request.Url.PathAndQuery.ToLower().EndsWith(Consts.Pages.SilverMine))
                    {
                        GoNext();
                        nextUpgradeLevel = CanUpgradeBuilding(_selectedVillage, _selectedVillage.owner.Realm.BuildingType(Fbg.Bll.CONSTS.BuildingIDs.CoinMine), out canUpgrade);
                        if (canUpgrade != Village.CanUpgradeResult.Yes)
                        {
                            GoTo(Steps.Step_UpgradeBuildings_Finish);
                        }
                    }
                    break;
                case Steps.Step_UpgradeBuildings_InSilverMine:
                    nextUpgradeLevel = CanUpgradeBuilding(_selectedVillage, _selectedVillage.owner.Realm.BuildingType(Fbg.Bll.CONSTS.BuildingIDs.CoinMine), out canUpgrade);
                    if (canUpgrade != Village.CanUpgradeResult.Yes)
                    {
                        GoTo(Steps.Step_UpgradeBuildings_InSilverMine2); 
                    }
                    break;
                case Steps.Step_UpgradeBuildings_InSilverMine2:
                    if (page == Consts.Pages.VillageOverview)
                    {
                        GoTo(Steps.Step_UpgradeBuildings_Finish);
                    }
                    break;
                case Steps.Step_MapLeadIn:
                    if (String.Compare(page, Consts.Pages.Map, true) == 0)
                    {
                        GoNext();
                    }
                    break;
                case Steps.Step_Map2:
                    if (String.Compare(page, Consts.Pages.PlayerOverview, true) == 0)
                    {
                        GoNext();
                    }
                    break;

            } 
             
            //
            // Create proper tutorial help page
            //
            linkNext.CssClass = "StandoutLink";
            switch (currentStep)
            {
                //case Steps.Step0_BookmarkApp:
                //    lblText.Text = Resources.Tutorial.Step0;
                //    break;
                case Steps.Step1_Welcome:
                    lblText.Text = String.Format(R_Tutorial.GetString("Step1") ,_selectedVillage.owner.Name, FacebookConfig.FacebookApiKey);
                    imgAdvisor.Visible = false;
                    linkNext.CssClass = "jsNext";

                    // since we no longer do bookmarking, we always dispay the next button
                    //if (_selectedVillage.owner.HasFlag(Fbg.Bll.Player.Flags.Advisor_BeginnerTutorialCompleted) == null 
                    //    && !Config.InDev)
                    //{
                    //    linkNext.CssClass += " NextLinkHide";
                    //}
                    break;
                case Steps.Step_VilOverview0:
                    if (page == Consts.Pages.VillageOverview)
                    {
                        lblText.Text = R_Tutorial.GetString("Step1_0");
                    }
                    else
                    {
                        lblText.Text = String.Format(R_Tutorial.GetString("GoTo_VillageOverview"), _selectedVillage.owner.Name);
                        linkNext.Visible = false;
                    }
                    break;
                #region Section 1
                case Steps.Step_VilOverview1:
                    if (page == Consts.Pages.VillageOverview)
                    {
                        level = _selectedVillage.GetBuildingLevel(Fbg.Bll.CONSTS.BuildingIDs.CoinMine);
                        lblText.Text = String.Format(R_Tutorial.GetString("Step1_1"), level
                            , Utils.FormatCost(Convert.ToInt32(Math.Round(_selectedVillage.PerHourCoinIncome, 0)))); 
                    }
                    else
                    {
                        lblText.Text = String.Format(R_Tutorial.GetString("GoTo_VillageOverview"), _selectedVillage.owner.Name);
                        linkNext.Visible = false;
                    }

                    break;
                case Steps.Step_VilOverview2:
                    if (page == Consts.Pages.VillageOverview)
                    {
                        lblText.Text = R_Tutorial.GetString("Step1_1_1");
                        linkNext.Visible = false;
                    }
                    else
                    {
                        lblText.Text = String.Format(R_Tutorial.GetString("GoTo_VillageOverview"), _selectedVillage.owner.Name);;
                        linkNext.Visible = false;
                    }

                    break;
                //case Steps.Step_UpgradeBuildings_ClickHQ:
                //    if (page == Consts.Pages.VillageOverview)
                //    {
                //        lblText.Text = Resources.Tutorial.Step1_2;
                //        panelTutorial.Attributes.Add("Rel", CONSTS.TutorialScreenElements.HQLink);
                //    }
                //    else
                //    {
                //        lblText.Text = String.Format(Resources.Tutorial.GoTo_VillageOverview, _selectedVillage.owner.Name);;
                //        panelTutorial.Attributes.Add("Rel", CONSTS.TutorialScreenElements.VillageHUD_VilName);
                //    }
                //    linkNext.Visible = false;

                //    break;
                case Steps.Step_UpgradeBuildings_InSilverMine:
                    if (Request.Url.PathAndQuery.ToLower().Contains(Consts.Pages.SilverMine))
                    {
                        lblText.Text = String.Format(R_Tutorial.GetString("Step_UpgradeBuildings_InSilverMine1")
                                , nextUpgradeLevel.LevelForDisplay);
                    }
                    else
                    {
                        if (page == Consts.Pages.VillageOverview)
                        {
                            lblText.Text = R_Tutorial.GetString("Step1_1_1");
                            linkNext.Visible = false;
                        }
                        else
                        {
                            lblText.Text = String.Format(R_Tutorial.GetString("GoTo_SilverMine"), _selectedVillage.owner.Name);
                            linkNext.Visible = false;
                        }
                    }

                    break;
                case Steps.Step_UpgradeBuildings_InSilverMine2:
                    if (Request.Url.PathAndQuery.ToLower().Contains(Consts.Pages.SilverMine))
                    {
                        lblText.Text = R_Tutorial.GetString("Step_UpgradeBuildings_InSilverMine2");
                    }
                    else
                    {
                        if (page == Consts.Pages.VillageOverview)
                        {
                            lblText.Text = R_Tutorial.GetString("Step1_1_1");
                            linkNext.Visible = false;
                        }
                        else
                        {
                            lblText.Text = String.Format(R_Tutorial.GetString("GoTo_SilverMine"), _selectedVillage.owner.Name);
                            linkNext.Visible = false;
                        }

//                            lblText.Text = String.Format(Resources.Tutorial.GoTo_VillageOverview, _selectedVillage.owner.Name);
//                            linkNext.Visible = false;
                    }
                    linkNext.Visible = false;

                    break;
                case Steps.Step_UpgradeBuildings_Finish:

                   
                        lblText.Text = R_Tutorial.GetString("Step_UpgradeBuildings_Finish");
                   

                    break;
                #endregion 

                #region Section 2
//                case Steps.Step_Stats1:
//                    if (page == Consts.Pages.VillageBuildings)
//                    {
//                        lblText.Text = String.Format(Resources.Tutorial.Step2,_selectedVillage.owner.Name);
//                        panelTutorial.Attributes.Add("Rel", CONSTS.TutorialScreenElements.VillageHUD);
//                    }
//                    else
//                    {
//                        lblText.Text = String.Format(Resources.Tutorial.GoTo_VillageBuildings,_selectedVillage.owner.Name);
//                        panelTutorial.Attributes.Add("Rel", CONSTS.TutorialScreenElements.VillageHUD_VilName + ","
//                            + CONSTS.TutorialScreenElements.HQLink);
//                        linkNext.Visible = false;
//                    }

//                    break;
//                case Steps.Step_Stats_VilName:
//                    if (page == Consts.Pages.VillageBuildings)
//                    {
//                        lblText.Text = String.Format(Resources.Tutorial.Step2_2, _selectedVillage.name);
//                        panelTutorial.Attributes.Add("Rel", CONSTS.TutorialScreenElements.VillageHUD_VilName + "," 
//                            + CONSTS.TutorialScreenElements.VillageRaname);
//                    }
//                    else
//                    {
//                        lblText.Text = String.Format(Resources.Tutorial.GoTo_VillageBuildings,_selectedVillage.owner.Name);
//                        panelTutorial.Attributes.Add("Rel", CONSTS.TutorialScreenElements.VillageHUD_VilName + ","
//                            + CONSTS.TutorialScreenElements.HQLink);
//                        linkNext.Visible = false;
//                    }

//                    break;
//                case Steps.Step_Stats_Silver:
//                    lblText.Text = Resources.Tutorial.Step2_3;
//                    panelTutorial.Attributes.Add("Rel", CONSTS.TutorialScreenElements.VillageHUD_CurSilver);
//                    break;
//                case Steps.Step_Stats_Silver2:
//                    lblText.Text = Resources.Tutorial.Step2_4;
//                    panelTutorial.Attributes.Add("Rel", CONSTS.TutorialScreenElements.VillageHUD_TreasurySize);
//                    break;
//                case Steps.Step_Stats_Silver4:
//                    lblText.Text = Resources.Tutorial.Step2_5;
//                    panelTutorial.Attributes.Add("Rel", CONSTS.TutorialScreenElements.VillageHUD_SilverProd);
//                    break;
//                case Steps.Step_Stats_Food1:
//                    lblText.Text = Resources.Tutorial.Step2_6_1;
//                    panelTutorial.Attributes.Add("Rel", CONSTS.TutorialScreenElements.VillageHUD_Food);
//                    break;
////                case Steps.Step_Stats_Food2:
////                    lblText.Text = Resources.Tutorial.Step2_6_2;
//////                    lblCurrentStepNumber.Text = String.Format(Consts.CurrentStepText, 2.5, "Village Stats");
////                    panelTutorial.Attributes.Add("Rel", CONSTS.TutorialScreenElements.VillageHUD_Food);
////                    break;
//                case Steps.Step_Stats_Food3:
//                    lblText.Text = String.Format(Resources.Tutorial.Step2_6_3
//                        , _selectedVillage.CurrentPopulation
//                        , _selectedVillage.MaxPopulation );

//                    panelTutorial.Attributes.Add("Rel", CONSTS.TutorialScreenElements.VillageHUD_Food);
//                    break;
                #endregion

                #region Section 3
                case Steps.Step_Army1:
                    if (page == Consts.Pages.VillageOverview)
                    {
                        lblText.Text = R_Tutorial.GetString("Step3_1");
                        panelTutorial.Attributes.Add("Rel", CONSTS.TutorialScreenElements.TroopsList);
                    }
                    else
                    {
                        lblText.Text = String.Format(R_Tutorial.GetString("Step3_1a"), _selectedVillage.owner.Name);
                        linkNext.Visible = false;
                    }

                    break;
                case Steps.Step_Army1_1:
                    if (page == Consts.Pages.VillageOverview)
                    {
                        lblText.Text = R_Tutorial.GetString("Step3_1_1");
                        panelTutorial.Attributes.Add("Rel", CONSTS.TutorialScreenElements.TroopsList);
                    }
                    else
                    {
                        lblText.Text = String.Format(R_Tutorial.GetString("Step3_1a"),_selectedVillage.owner.Name);
                        linkNext.Visible = false;
                    }

                    break;
                case Steps.Step_Army2:
                    if (page == Consts.Pages.VillageOverview)
                    {
                        lblText.Text = R_Tutorial.GetString("Step3_2");
                        panelTutorial.Attributes.Add("Rel", CONSTS.TutorialScreenElements.TroopsList);
                    }
                    else
                    {
                        lblText.Text = String.Format(R_Tutorial.GetString("GoTo_VillageOverview"), _selectedVillage.owner.Name);;
                        panelTutorial.Attributes.Add("Rel", CONSTS.TutorialScreenElements.VillageHUD_VilName);
                        linkNext.Visible = false;
                    }

                    break;
                #endregion 

                #region Section 4
                case Steps.Step_MapLeadIn:
                    lblText.Text = R_Tutorial.GetString("Step4_1");
                    linkNext.Visible = false;
                    break;
                case Steps.Step_Map1:
                    if (String.Compare(page, Consts.Pages.Map, true) == 0) 
                    {
                        lblText.Text = R_Tutorial.GetString("Step4_2");
                        panelTutorial.Attributes.Add("Rel", "");
                    }
                    else
                    {
                        lblText.Text = String.Format(R_Tutorial.GetString("GoTo_Map"),_selectedVillage.owner.Name);
                        linkNext.Visible = false;
                    }
                    break;
                case Steps.Step_Map1_1:
                    if (String.Compare(page, Consts.Pages.Map, true) == 0)
                    {
                        lblText.Text = R_Tutorial.GetString("Step4_2_1");
                        panelTutorial.Attributes.Add("Rel", "");
                    }
                    else
                    {
                        lblText.Text = String.Format(R_Tutorial.GetString("GoTo_Map"), _selectedVillage.owner.Name);
                        linkNext.Visible = false;
                    }
                    break;
                case Steps.Step_Map1_2:
                    if (String.Compare(page, Consts.Pages.Map, true) == 0)
                    {
                        lblText.Text = R_Tutorial.GetString("Step4_2_2");
                        panelTutorial.Attributes.Add("Rel", "");
                    }
                    else
                    {
                        lblText.Text = String.Format(R_Tutorial.GetString("GoTo_Map"), _selectedVillage.owner.Name);
                        linkNext.Visible = false;
                    }
                    break;
                case Steps.Step_Map2:
                    if (String.Compare(page, Consts.Pages.Map, true) == 0)
                    {
                        lblText.Text = R_Tutorial.GetString("Step4_3");
                        panelTutorial.Attributes.Add("Rel", "");
                    }
                    else
                    {
                        lblText.Text = String.Format(R_Tutorial.GetString("GoTo_Map"), _selectedVillage.owner.Name);
                        linkNext.Visible = false;
                    }
                    break;
                case Steps.Step_Map2_1:
                    if (String.Compare(page, Consts.Pages.Map, true) == 0)
                    {
                        lblText.Text = R_Tutorial.GetString("Step4_3_1");
                        panelTutorial.Attributes.Add("Rel", "");
                    }
                    else
                    {
                        lblText.Text = String.Format(R_Tutorial.GetString("GoTo_Map"), _selectedVillage.owner.Name);
                        linkNext.Visible = false;
                    }
                    break;
            //    case Steps.Step_Map_VilOverviewOther:
            //        if (String.Compare(page, Consts.Pages.VillageOverviewOther, true) == 0)
            //        {
            //            lblText.Text = Resources.Tutorial.Step4_4;
            //            panelTutorial.Attributes.Add("Rel", CONSTS.TutorialScreenElements.VillageOverviewPlayersLink);
            //            linkNext.Visible = false;
            //        }
            //        else if (String.Compare(page, Consts.Pages.Map, true) == 0)
            //        {
            //            lblText.Text = Resources.Tutorial.Step4_3;
            //            panelTutorial.Attributes.Add("Rel", "");
            //            linkNext.Visible = false;
            //        }
            //        else 
            //        {
            //            lblText.Text = String.Format(Resources.Tutorial.GoTo_Map,_selectedVillage.owner.Name);
            //            panelTutorial.Attributes.Add("Rel", CONSTS.TutorialScreenElements.MapLink);
            //            linkNext.Visible = false;
            //        }

            ////        lblCurrentStepNumber.Text = String.Format(Consts.CurrentStepText, 4.4, "Map");
            //        break;
                case Steps.Step_Map_PlayerOverview:
                    if (String.Compare(page, Consts.Pages.Map, true) == 0)
                    {
                        lblText.Text = R_Tutorial.GetString("Step4_5");
                        panelTutorial.Attributes.Add("Rel", CONSTS.TutorialScreenElements.PlayerOverviewLink);
                    }
                    //else if (String.Compare(page, Consts.Pages.VillageOverviewOther, true) == 0)
                    //{
                    //    lblText.Text = Resources.Tutorial.Step4_4a;
                    //    panelTutorial.Attributes.Add("Rel", CONSTS.TutorialScreenElements.VillageOverviewPlayersLink);
                    //    linkNext.Visible = false;
                    //}
                    else if (String.Compare(page, Consts.Pages.Map, true) == 0)
                    {
                        lblText.Text = R_Tutorial.GetString("Step4_3");
                        panelTutorial.Attributes.Add("Rel", "");
                        linkNext.Visible = false;
                    }
                    else
                    {
                        lblText.Text = String.Format(R_Tutorial.GetString("GoTo_Map") ,_selectedVillage.owner.Name);
                        linkNext.Visible = false;
                    }
             //       lblCurrentStepNumber.Text = String.Format(Consts.CurrentStepText, 4.5, "Map");
                    break;
                #endregion

         //       #region Section 5
         //       case Steps.Step_Clan1:
         //           lblText.Text = String.Format(Resources.Tutorial.Step5_1,_selectedVillage.owner.Name);
         //           panelTutorial.Attributes.Add("Rel", "");
         //  //         lblCurrentStepNumber.Text = String.Format(Consts.CurrentStepText, 5.1, "Clan");
         //           break;
         //       case Steps.Step_Clan2:
         //           lblText.Text = Resources.Tutorial.Step5_2;
         //           panelTutorial.Attributes.Add("Rel", CONSTS.TutorialScreenElements.ClanIcon);
         ////           lblCurrentStepNumber.Text = String.Format(Consts.CurrentStepText, 5.2, "Clan");
         //           break;
         //       #endregion 


                case Steps.Done:
                    lblText.Text = String.Format(R_Tutorial.GetString("Done"),_selectedVillage.owner.Name, _selectedVillage.owner.User.ID);
                    panelTutorial.Attributes.Add("Rel", "");                 
                    linkNext.Visible = true;
                    linkNext.Text = RS("Finish");

                    break;
                default:
                    break;
                //throw new ArgumentException("unrecognized step=" + currentStep.ToString());
            }

            linkNext.CommandName = currentStep.ToString();
            this.Visible = true;

            panelProgressBar.Width = new Unit(Convert.ToInt16(100 * (int)currentStep / (float)Steps.Done),System.Web.UI.WebControls.UnitType.Percentage);
            if (panelProgressBar.Width.Value > 100)
            {
                panelProgressBar.Width = new Unit(100, System.Web.UI.WebControls.UnitType.Percentage);
            }
            lblPercentage.Text = Convert.ToInt16(100 * (int)currentStep / (float)Steps.Done).ToString() + " % " + RS("completed");


            return true;
        }
        else
        {
            this.Visible = false;
            return false;
        }
    }

    /// <summary>
    /// allows the application to let us know when an upgrade occured. 
    /// </summary>
    public void SignalBuildingUpgradePerformed()
    {
        //if (isRunning)
        //{
        //    if (currentStep == Steps.Step_UpgradeBuildings_Upgrade)
        //    {
        //        // If we are on Upgrade page, and this steps, that measn the user clicked upgrade a building
        //        //  so we consider this step to be completed. 
        //        GoNext();
        //    }
        //}
    }

    public bool isRunning
    {
        get
        {
            HttpCookie cookie = Request.Cookies[Consts.CookieName];
            if (cookie != null)
            {
                currentStep = (Steps)Convert.ToInt32(cookie.Values[Consts.Cookie_curStep]);
                if (cookie.Values[Consts.Cookie_isRunning] == Consts.Cookie_isRunning_Yes)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public void Start()
    {
        Start(Steps.Step1_Welcome);
    }

 
    public void Start(Steps step)
    {
        HttpCookie cookie = Request.Cookies[Consts.CookieName];
        if (cookie != null)
        {
            currentStep = (Steps)Convert.ToInt32(cookie.Values[Consts.Cookie_curStep]);
            cookie.Values[Consts.Cookie_isRunning] = Consts.Cookie_isRunning_Yes;

            cookie.Expires = DateTime.Now.AddDays(10);
            Response.Cookies.Add(cookie);

            Response.Redirect("~/VillageOverview.aspx", false);
        }
        else
        {
            currentStep = Steps.Step1_Welcome;
            cookie = new HttpCookie(Consts.CookieName);
            cookie.Expires = DateTime.Now.AddDays(10);
            cookie.Values.Add(Consts.Cookie_isRunning, Consts.Cookie_isRunning_Yes);
            cookie.Values.Add(Consts.Cookie_curStep, ((int)step).ToString());
            Response.Cookies.Add(cookie);

            Response.Redirect(GetStepStartUrl(step), false);
        }

        Response.Cookies.Add(new HttpCookie(Consts.CookieTutorialPosName));
    }

    private string GetStepStartUrl(Steps step)
    {
        if (step <= Steps.Step_UpgradeBuildings_InSilverMine)
        {
            return "~/VillageOverview.aspx";
        }
        else if (step <= Steps.Step_MapLeadIn)
        {
            return "~/VillageOverview.aspx";
        }
        else if (step < Steps.Step_Army1)
        {
            return "~/VillageBuildings.aspx";
        }
        else
        {
            return "~/VillageOverview.aspx";
        }
    }

    /// <summary>
    /// call this if you want to 'start' the tutorial without starting it immediatelly. 
    /// This will just create a cookie that will tell the tutorial that it is runinig and it 
    /// will be displayed on a page that uses the tutorial. 
    /// 
    /// THis also start the tutorial from stop 0 - bookmark app in FB. 
    /// </summary>
    public void CreateRunningCookieFromStart()
    {
        HttpCookie cookie;
        currentStep = Steps.Step1_Welcome;
        cookie = new HttpCookie(Consts.CookieName);
        cookie.Expires = DateTime.Now.AddDays(10);
        cookie.Values.Add(Consts.Cookie_isRunning, Consts.Cookie_isRunning_Yes);
        cookie.Values.Add(Consts.Cookie_curStep, ((int)Steps.Step1_Welcome).ToString());
        Response.Cookies.Add(cookie);
    }

    public void Stop()
    {
        HttpCookie cookie = Request.Cookies[Consts.CookieName];
        if (cookie != null)
        {
            cookie.Values[Consts.Cookie_isRunning] = Consts.Cookie_isRunning_No;
            cookie.Expires = DateTime.Now.AddDays(10);
            Response.Cookies.Add(cookie);
        }

        this.Visible = false;
        Response.Redirect(Request.Url.OriginalString); // this had to be added when the tutorial was wrapped in an UpdatePanel

    }

    public void End()
    {
        HttpCookie cookie = Request.Cookies[Consts.CookieName];
        if (cookie != null)
        {
            cookie.Values[Consts.Cookie_isRunning] = Consts.Cookie_isRunning_No;
            cookie.Values[Consts.Cookie_curStep] = ((int)Steps.Step1_Welcome).ToString();
            cookie.Expires = DateTime.Now.AddDays(10);
            Response.Cookies.Add(cookie);
        }

        this.Visible = false;
        //send player to post story page

        if (!LoginModeHelper.isBDA(Request))
        {
            Response.Redirect(String.Format("allowss.aspx?{0}={1}", CONSTS.QuerryString.StoryToPublish, (int)StoriesToPublish.TutorialComplete), false);
        }
    }

    public void Restart()
    {
        Request.Cookies.Remove(Consts.CookieName);
        Start();
    }
    public void Restart(int section)
    {
        Request.Cookies.Remove(Consts.CookieName);
        if (section == 0)
        {
            Start();
        }
        else if (section == 1)
        {
            Start(Steps.Step_VilOverview1);
        }
        else if (section == 2)
        {
            Start(Steps.Step_UpgradeBuildings_InSilverMine);
        }
        else if (section == 3)
        {
            Start(Steps.Step_MapLeadIn);
        }
        else if (section == 4)
        {
            Start(Steps.Step_Army1);
        }
        else
        {
            throw new ArgumentException("Unrecognized value for section=" + section.ToString());
        }
    }
    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        this.Stop();
    }


    protected void GoNext()
    {
        if (currentStep != Steps.Done)
        {
            GoTo((Steps)((int)currentStep + 1));
        }
        else
        {
            _selectedVillage.owner.SetFlag(Player.Flags.Advisor_BeginnerTutorialCompleted);
            _selectedVillage.owner.Quests2.CompletedQuests_RewardNotClaimed_Invalidate();
            this.End();
        }
    }
    protected void GoTo(Steps step)
    {
        currentStep = step;
        HttpCookie cookie = Request.Cookies[Consts.CookieName];
        if (cookie != null)
        {
            cookie.Values[Consts.Cookie_curStep] = ((int)currentStep).ToString();
            cookie.Expires = DateTime.Now.AddDays(10);
            Response.Cookies.Add(cookie);
        }
    }

    protected void linkNext_Click(object sender, EventArgs e)
    {
        GoNext();
        DisplayCurrentStep();
    }


    #region step specific functions

    /// <summary>
    /// if we return null, then it means no building is eligable for upgrade at this time. 
    /// </summary>
    private BuildingTypeLevel CanUpgradeBuilding(Fbg.Bll.Village village)
    {

        int curLevel;
        int nextPotentialUpgradeLevel;
        int upgradeLevel;
        BuildingTypeLevel nextBuildingTypeLevel;
        BuildingTypeLevel curBuildingTypeLevel;

        BuildingTypeLevel UpgradeTo_NotSilverMine = null;
        BuildingTypeLevel UpgradeTo_SilverMine;

        foreach (BuildingType bt in village.owner.Realm.BuildingTypes)
        {
            curBuildingTypeLevel = bt.Level(village.GetBuildingLevel(bt.ID));
            curLevel = curBuildingTypeLevel == null ? 0 : curBuildingTypeLevel.Level;
            upgradeLevel = village.GetUpgradingBuildingLevel(bt.ID);

            // get the next upgrade level
            nextPotentialUpgradeLevel = upgradeLevel > curLevel ? upgradeLevel : curLevel; // get the larger of the two
            nextPotentialUpgradeLevel++;

            //this may be NULL! if nextPotentialUpgradeLevel > then max level
            nextBuildingTypeLevel = bt.Level(nextPotentialUpgradeLevel);

            // get a list of unsatisfied req. no req if list is empty
            List<BuildingTypeLevel> unsatisfiedReqs;
            if (nextBuildingTypeLevel != null)
            {
                unsatisfiedReqs = nextBuildingTypeLevel.GetUnsatisfiedRequirements(village);
            }
            else
            {
                unsatisfiedReqs = new List<BuildingTypeLevel>(0);
            }


            if (unsatisfiedReqs.Count != 0)
            {
            }
            else if (nextBuildingTypeLevel != null
                && (nextBuildingTypeLevel.Population > village.RemainingPopulation && nextBuildingTypeLevel.Population != 0))
            {
            }
            else if (nextBuildingTypeLevel != null && nextBuildingTypeLevel.Cost > village.coins)
            {
            }
            else
            {
                if (nextBuildingTypeLevel != null)
                {
                    UpgradeTo_SilverMine = bt.Level(nextPotentialUpgradeLevel);
                    if (bt.ID == Fbg.Bll.CONSTS.BuildingIDs.CoinMine)
                    {
                        return UpgradeTo_SilverMine;
                    }
                    else
                    {
                        UpgradeTo_NotSilverMine = UpgradeTo_SilverMine;
                    }
                }
            }
        }

        return UpgradeTo_NotSilverMine;
    }


       /// <summary>
    /// if we return null, then it means no building is eligable for upgrade at this time. 
    /// </summary>
    private BuildingTypeLevel CanUpgradeBuilding(Fbg.Bll.Village village, BuildingType bt, out Village.CanUpgradeResult canUpgrade)
    {
        int currentlyUpgradingToLevel;
        List<BuildingTypeLevel> unsatisfiedReqs;
        bool alternateRow = false;
        bool notEnoughSilver;
        bool notEnoughFood;
        BuildingTypeLevel nextBuildingTypeLevel;
        BuildingTypeLevel curBuildingTypeLevelPlus1;
        BuildingTypeLevel curBuildingTypeLevel;
        int curBuildingLevel;
        BuildingTypeLevel maxNextBuildingTypeLevel;

        canUpgrade = village.CanUpgrade(bt
            , out nextBuildingTypeLevel
            , out curBuildingTypeLevel
            , out maxNextBuildingTypeLevel
            , out unsatisfiedReqs
            , out notEnoughSilver
            , out notEnoughFood
            , _selectedVillage.owner.NightBuild_IsActive);

        return nextBuildingTypeLevel;
    }

    #endregion 
}
