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
using Gmbc.Common.Diagnostics.ExceptionManagement;

public partial class Controls_Quests2 : System.Web.UI.UserControl
{


    private enum Quests 
    {
        Q2_CreateTags=1,
        Q2_ApplyTags=2,
        Q2_Filters = 3,
        Q2_FiltersSummaryPages =4,
        Q2_MultiFilter=5
    }


    static int[] _questsRewards = new int[] {
        0 // this is for Quests.Wait
        ,50000 //Q2_CreateTags
        ,10000 //Q2_ApplyTags
        ,10000 //Q2_Filters
        ,10000 // Q2_FiltersSummaryPages
        ,50000 // Q2_MultiFilter
    };
    static object[] _questCompletedFlags = new object[] { 
        null // this is for Quests.Wait which is never used
        , Fbg.Bll.Player.Flags.Quest2_CreateTags
        , Fbg.Bll.Player.Flags.Quest2_ApplyTags
        , Fbg.Bll.Player.Flags.Quest2_Filters
        , Fbg.Bll.Player.Flags.Quest2_FiltersSummaryPages //     Q2_FiltersSummaryPages
        , Fbg.Bll.Player.Flags.Quest2_MultiFilter //     Q2_MultiFilter
        };

    Fbg.Bll.Player _player;
    Quests _currentQuest;
    List<Fbg.Bll.VillageBase> _myVillages;
    Village _village;
   


    protected void Page_Load(object sender, EventArgs e)
    {
      

    }

    /// <summary>
    /// return value says if there is any quest to display
    /// </summary>
    /// <param name="player"></param>
    /// <param name="myVillages"></param>
    /// <param name="_currentVillage"></param>
    /// <returns></returns>
    public bool Initialize(Fbg.Bll.Player player, List<Fbg.Bll.VillageBase> myVillages, Village _currentVillage)
    {
        _player = player;
        _myVillages = myVillages;
        _village = _currentVillage;


        //
        // tournament realms have no quests
        //
        if (_player.Realm.IsTemporaryTournamentRealm)
        {
            this.Visible = false;
            return false;
        }

        //
        // display the quests
        //
        if (!DisplayCurrentQuest())
        {
            //
            // all quests are completed
            //
            this.Visible = false;
            return false;
        }
        else
        {
            return true;
        }
    }


    private bool DisplayCurrentQuest()
    {
        try
        {
            bool isQuestDisplayed = false;
            //
            // hide everything
            //
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Panel)
                {
                    ctrl.Visible = false;
                }
            }



            //TODO: we do >= 5 for NOW but change this soon to >5 
            #region see if we are to display the tag quests
            //
            // see if we are to display the tag quests
            //
            if (_player.NumberOfVillages >= 5 && _player.NumberOfVillages <= 7
                && _player.HasFlag(Player.Flags.Quest2_Tags_Alldone) == null)
            {
                if (_player.HasFlag(Fbg.Bll.Player.Flags.Quest2_CreateTags) == null)
                {
                    //
                    // Create tags
                    //
                    _currentQuest = Quests.Q2_CreateTags;
                    panel_QCreateTags.Visible = true;
                    isQuestDisplayed = true;
                }
                else if (_player.HasFlag(Fbg.Bll.Player.Flags.Quest2_ApplyTags) == null)
                {
                    //
                    // apply  tags
                    //
                    _currentQuest = Quests.Q2_ApplyTags;
                    panel_QApplyTags.Visible = true;
                    isQuestDisplayed = true;
                }
                else if (_player.HasFlag(Fbg.Bll.Player.Flags.Quest2_Filters) == null)
                {
                    //
                    // select filter
                    //
                    _currentQuest = Quests.Q2_Filters;
                    panel_QFilters.Visible = true;
                    isQuestDisplayed = true;
                }
                else if (_player.HasFlag(Fbg.Bll.Player.Flags.Quest2_FiltersSummaryPages) == null)
                {
                    //
                    // go to summary pages
                    //
                    _currentQuest = Quests.Q2_FiltersSummaryPages;
                    panel_QFiltersSummary.Visible = true;
                    isQuestDisplayed = true;
                }
                else if (_player.HasFlag(Fbg.Bll.Player.Flags.Quest2_MultiFilter) == null)
                {
                    //
                    // create a custom, multi tag filter.
                    //
                    _currentQuest = Quests.Q2_MultiFilter;
                    panel_QMultiFilter.Visible = true;
                    isQuestDisplayed = true;
                }
                else
                {
                    //
                    // no more tag quests. so display a message telling player this. 
                    //  we return right away on purpose. 
                    //
                    panelAllTagQuestsCompleted.Visible = true;
                    return true;
                }
            }
            #endregion 

            if (isQuestDisplayed)
            {
                //
                // if we have the minimize cookie, then hide the quest details. 
                //
               
                    panelClaimReward.Visible = true;
               
                lblTitle.Text = Resources.QuestTitles.ResourceManager.GetString(_currentQuest.ToString());
                panelReward.Visible = true;
                lblRewardAmount.Text = Utils.FormatCost(_questsRewards[(int)_currentQuest]);
            }

            return isQuestDisplayed;
        }

        catch (Exception ex)
        {
            BaseApplicationException bex = new BaseApplicationException("Error in Display quest", ex);
            bex.AddAdditionalInformation("_currentQuest", _currentQuest);
            bex.AddAdditionalInformation("_village", _village);
            throw bex;
        }


        return true;
    }

    private double GetQuestRewardMultiplier()
    {
        return _questsRewards[(int)_currentQuest];
    }

    protected void lbClaimYourReward_Click(object sender, EventArgs e)
    {
        int iVal, iVal2;
        double dVal;
        bool bFailedQuest=false;
        int currentQuestReward;
        int treasurySize;
        Tags tags;
        Filter filter;

        try
        {
            // 
            // what it the reward and can the treasury handle it???
            //
            currentQuestReward = _questsRewards[(int)_currentQuest];
            treasurySize = _village.TreasurySize;
            if (treasurySize - _village.coins < currentQuestReward)
            {
                lblClainError.Visible = true;
                lblClainError.Text = Resources.QuestsNotCompletedMsgs.NoRoomInTreasury2;
            }
            else
            {
                #region complete the quest
                //
                // figure out if quest is completed
                //
                lblClainError.Visible = false; //lets start by saying there is no erorr. 
                switch (_currentQuest)
                {
                    case Quests.Q2_CreateTags:
                        if (_player.Tags.Count < 6)
                        {
                            lblClainError.Visible = true;
                        }
                        break;
                    case Quests.Q2_ApplyTags:
                        // just check if there are at least 4 village& tag combinations. ie, 2 tags applied to 2 villages each. 
                        iVal = 0;
                        foreach (TagBase tag in _player.Tags)
                        {
                            tags = Fbg.Bll.Tags.GetTagByID(_player, tag.ID);
                            iVal = iVal + tags.WithVillages.Rows.Count;
                            if (iVal >= 4)
                            {
                                break;
                            }
                        }
                        if (iVal < 4)
                        {
                            lblClainError.Visible = true;
                        }
                        break;
                    case Quests.Q2_Filters:
                        if (_player.SelectedFilter == null)
                        {
                            lblClainError.Visible = true;
                        }
                        break;
                    case Quests.Q2_FiltersSummaryPages:
                        break;
                    case Quests.Q2_MultiFilter:
                        // just check if there is a filter with at least 2 tags selected
                        iVal = 0;
                        foreach (FilterBase filterbase in _player.Filters)
                        {
                            filter = Fbg.Bll.Filter.GetFilterByID(_player, filterbase.ID);
                            if (filter.Tags.Select(Filter.CONSTS.SelectedTagColumnNames.FilterID + " <> 0").Length >= 2)
                            {
                                iVal = 1;
                                break;
                            }
                        }
                        if (iVal != 1)
                        {
                            lblClainError.Visible = true;
                        }
                        break;


                        
                    default:
                        return; // invalid quest, no reward.
                }


                if (!lblClainError.Visible)
                {
                    //
                    // apply the reward for the quest
                    //
                    _village.UpdateCoins(_questsRewards[(int)_currentQuest]);

                    //
                    // Mark the quest completed
                    //
                    _player.SetFlag((Player.Flags)_questCompletedFlags[(int)_currentQuest]);

                    //
                    // hide evertything and display quest completed message only 
                    //
                    foreach (Control ctrl in this.Controls)
                    {
                        if (ctrl is Panel)
                        {
                            ctrl.Visible = false;
                        }
                    }
                    //lblRewardAmount2.Text = Utils.FormatCost(_questsRewards[(int)_currentQuest]);
                    panelCompletedQuests.Visible = true;
                    panel_QuestCompletedTitle.Visible = true;
                    lblQuestCompletedMsg.Text = GetQuestCompletedMsg();
                }
                else
                {
                    lblClainError.Text = Resources.QuestsNotCompletedMsgs.ResourceManager.GetString(_currentQuest.ToString());

                }
                #endregion
            }
        }
        catch (Exception ex)
        {
            BaseApplicationException bex = new BaseApplicationException("Error in claim your reward", ex);
            bex.AddAdditionalInformation("_currentQuest", _currentQuest);
            bex.AddAdditionalInformation("_village", _village);
            throw bex;
        }
    }

    private string GetQuestCompletedMsg()
    {
        switch (_currentQuest)
        {
            default:
                return Resources.QuestsCompletedMsgs.ResourceManager.GetString(_currentQuest.ToString());
        }
    }


    protected void btnNextQuest_Click(object sender, EventArgs e)
    {
    }


    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        _player.SetFlag(Player.Flags.Quest2_Tags_Alldone);
        this.Visible = false;
    }
   
}
