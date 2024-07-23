using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Fbg.Common;
using Fbg.Bll;
using BDA.Neighbours;
using System.Text;
using BDA.Achievements;
using System.Collections.Generic;

public partial class Quests : MyCanvasIFrameBasePage
{
    public MasterBase_PopupFullFunct mainMasterPage;
    public Fbg.Bll.Village village = null;

    public Quests()
    {
        R_OverridePageName = "Quests.aspx";
    }

    new protected void Page_Load(object sender, EventArgs e)
    {

        base.Page_Load(sender, e);
        mainMasterPage = (MasterBase_PopupFullFunct)this.Master;
        mainMasterPage.Initialize(FbgPlayer, MyVillages);
       // mainMasterPage.SetDefaultBackgroundColor();
        village = mainMasterPage.CurrentlySelectedVillage;
        Populate();
        lblClainError.Visible = false;
        

        Label1.Visible = false;
       
        

        Populate();


    }
    bool q2Exists;
    bool q1Exists;
    bool showBrainQuests;
    List<Quest> claimRewardQuests;
    List<Quest> recommendedQuests;
    public void Populate()
    {
        if (FbgPlayer.Realm.IsTemporaryTournamentRealm)
        {
            return;
        }
        List<Quest> quests = FbgPlayer.Quests2.AllQuests;
        // remove some quests
        //quests.RemoveAll(delegate(Quest q) {return q.DependentQuest == null ? false: !q.DependentQuest.IsCompleted ; });

        claimRewardQuests = quests.FindAll(delegate(Quest q) { return q.IsCompleted && !q.IsRewardClaimed; });
        recommendedQuests = FbgPlayer.Quests2.NextQuestsInProgression;

        dvRecommendedQuests.DataSource = recommendedQuests;
        dvRecommendedQuests.DataBind();

        //dvSubjectTypes.DataSource = quests;
        //dvSubjectTypes.DataBind();

        if (claimRewardQuests.Count > 0)
        {
            pnlCompletedQuests.Visible = true;
            dlClaimRewardList.DataSource = claimRewardQuests;
            dlClaimRewardList.DataBind();
        }
        else
        {
            pnlCompletedQuests.Visible = false;
        }

        Quests1.Initialize(FbgPlayer, MyVillages, village, isMobile, isD2, new Controls_Quests.GetRewardClick(AutoSelectMasteryQuest));
        q2Exists = Q2.Initialize(FbgPlayer, MyVillages, village) && !isMobile && !isD2; // do not show these quests on mobile
        q1Exists = !Quests1.AreAllQuestsDone && Quests1.AreAnyQuestVisible ;
        showBrainQuests=false;
        if (!q1Exists && !q2Exists)
        {
            showBrainQuests = false;
        }
        else
        {
            // dont show brain quests if tutorial quest is visible
            if (recommendedQuests.Count == 0 || (recommendedQuests[0].Tag != Fbg.Bll.Player.QuestTags.Tutorial.ToString()))
            {
                showBrainQuests = true;
            }
        }

        // TODO: Circumvented the if's so Master quests are shown.
        // Comment out before posting this file
        //showBrainQuests = true;

        buttonBrainQuests.Visible = showBrainQuests;


        if (!IsPostBack && !mainMasterPage.isMobile)
        {
            AutoSelectQuest(Request.QueryString["selectquestid"]);
        }

    }
    public void AutoSelectQuest()
    {
        AutoSelectQuest(null);
    }
    public void AutoSelectQuest(string selectQuestID)
    {            //
        // default select quest
        //
        if (claimRewardQuests.Count > 0 && string.IsNullOrWhiteSpace(selectQuestID))
        {
            ShowSelectedQuest(dlClaimRewardList, claimRewardQuests[0], claimRewardQuests);
        }
        else
        {
            if (recommendedQuests.Count > 0)
            {
                if (!string.IsNullOrWhiteSpace(selectQuestID))
                {
                    ShowSelectedQuest(dvRecommendedQuests, Convert.ToInt32(selectQuestID), recommendedQuests);
                }
                else
                {
                    ShowSelectedQuest(dvRecommendedQuests, recommendedQuests[0], recommendedQuests);
                }
            }
            else if (showBrainQuests)
            {
                BraniacQuestClick();
            }
        }
    }
    public void AutoSelectMasteryQuest()
    {            //
        if (showBrainQuests) {
            BraniacQuestClick();
        }
    }

    protected void dvSubjectTypes_SelectedIndexChanged(object sender, EventArgs e)
    {
    }

    protected void Button1_Command(object sender, CommandEventArgs e)
    {
        int questID = Convert.ToInt32(e.CommandArgument);
        Quest q = FbgPlayer.Quests_CompleteAQuest(questID);
        if (q != null)
        {
            //add a JS like data object
            string qComplData = "{credits:" + FbgUser.Credits + ", completedQuests:" + FbgPlayer.Quests2.CompletedQuests_RewardNotClaimed().Count + " }";

            ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "refresh_coins", "refreshCoins(" + qComplData + ");", true);
        }
        Populate();
        AutoSelectQuest();
    }




    protected void ShowSelectedQuest(DataList dl, int selectedQuestID, List<Quest> quests)
    {
        ShowSelectedQuest(dl, quests.Find(f => f.ID == selectedQuestID), quests);
    }

    protected void ShowSelectedQuest(DataList dl, BDA.Achievements.Quest q, List<Quest> quests)
    {
        dvRecommendedQuests.SelectedIndex = -1;
        dlClaimRewardList.SelectedIndex = -1;
        buttonBrainQuests.CssClass = "questName";
        pnOldQuests.Visible = false;
        DetailsView1.Visible = true;
        if (q != null)
        {
            dl.SelectedIndex = quests.IndexOf(q);
            dl.DataSource = quests;
            dl.DataBind();

            List<Quest> oneQuest = new List<Quest>();
            oneQuest.Add(q);
            DetailsView1.DataSource = oneQuest;
            DetailsView1.DataBind();
        }
        if (mainMasterPage.isMobile)
        {
            ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "show", "ShowHideDetails(true);", true);
            //ClientScript.RegisterStartupScript(this.Page.GetType(), "show", "<script type='text/javascript'>alert('fff');</script>");
        }
    }

    protected void LinkButtonRecommendedQuest_Command(object sender, CommandEventArgs e)
    {

        int questID;
        if (Int32.TryParse(e.CommandArgument.ToString(), out questID))
        {
            List<Quest> quests = FbgPlayer.Quests2.AllQuests;
            Quest q = quests.Find(delegate(Quest q2) { return q2.ID == questID; });
            if (q != null)
            {
                ShowSelectedQuest(dvRecommendedQuests, q, recommendedQuests);
            }
        }
    }
    protected void LinkButtonCompletedQuests_Command(object sender, CommandEventArgs e)
    {
        
        int questID ;
        if (Int32.TryParse(e.CommandArgument.ToString(), out questID))
        {

            Quest q = claimRewardQuests.Find(delegate(Quest q2) { return q2.ID == questID; });
            if (q != null)
            {
                ShowSelectedQuest(dlClaimRewardList, q, claimRewardQuests);
            }
        }
    }
    protected void buttonBrainQuests_Click(object sender, EventArgs e)
    {
        BraniacQuestClick();
    }

    private void BraniacQuestClick() {
        dvRecommendedQuests.SelectedIndex = -1;
        dlClaimRewardList.SelectedIndex = -1;

        buttonBrainQuests.CssClass += " questNameSelected";
        pnOldQuests.Visible = true;
        DetailsView1.Visible = false;
        if (mainMasterPage.isMobile) {
            ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "show", "ShowHideDetails(true);", true);
            //ClientScript.RegisterStartupScript(this.Page.GetType(), "show", "<script type='text/javascript'>alert('fff');</script>");
        }
    }



    protected override void OnPreInit(EventArgs e)
    {

        if (isMobile)
        {
            base.MasterPageFile = "masterPopupFullFunct_m.master";
        }
        base.OnPreInit(e);
    }
    
}
