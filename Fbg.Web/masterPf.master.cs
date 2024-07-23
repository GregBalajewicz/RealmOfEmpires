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

public partial class masterPf : BaseMasterPage
{
    public masterPf()
    {
        BaseResName = "masterPf.master";
    }

    Fbg.Bll.Player _player;
    protected void Page_Load(object sender, EventArgs e)
    {
       
    }

    public main MainContainerMasterPage
    {
        get
        {
            return (main)this.Master;
        }
    }

    public void Initialize(Fbg.Bll.Player player, List<VillageBase> villages
        , ManagePFPages currentPage)
    {
        _player = player;
        MainContainerMasterPage.Initialize(player, villages);

        #region Localize Controls
        if (!IsPostBack)
        {
            mnu_PF.Items[0].Text = RS("link_Unlock");
            mnu_PF.Items[1].Text = RSc("hireServants");
            mnu_PF.Items[2].Text = RS("link_TransServ");
            mnu_PF.Items[3].Text = RS("link_CancelFeat");
        }
        #endregion

        if (LoginModeHelper.isKongregate(Request))
        {
            mnu_PF.Items[1].NavigateUrl = "pfCredits_kongregate2.aspx";
        }
        else
        {
            mnu_PF.Items[3].Text = "";
        }

        //if (player.Realm.IsVPrealm && !IsPostBack)
        //{
        //    mnu_PF.Items.RemoveAt(3);
        //}

        CurrentPage = currentPage;
        BindCredits();

    }

    /// <summary>
    /// update the 'my credits' on screen 
    /// </summary>
    public void BindCredits()
    {
        lbl_AvilableGold.Text = RS("youHave") + "<span class=\"credit\">" + Utils.FormatCost(_player.User.Credits) + "</span>" + RS("_servants");
    }


    ManagePFPages _manageClanPages;
    public ManagePFPages CurrentPage
    {
        get
        {
            return _manageClanPages;
        }
        set
        {
            _manageClanPages = value;
            SetSelectedLink(_manageClanPages);
        }
    }
    public enum ManagePFPages
    {
        Credits = 0,
        Benefits = 1,
        /// <summary>
        /// this page is not valid in a VPRealm
        /// </summary>
        Cancel = 2,
        transfer = 3,
    }
    private void SetSelectedLink(ManagePFPages managePFPages)
    {
        switch (managePFPages)
        {
            case ManagePFPages.Benefits:
                mnu_PF.Items[0].Selected = true;
                panelHireMore.Visible = true;
                panelWhatAreServants.Visible = false;
                panelWhatAreServants_ForBenefitsPage.Visible = true;
                lblOffer.Visible = false;
                break;
            case ManagePFPages.Credits:
                mnu_PF.Items[1].Selected = true;
                panelHireMore.Visible = false;
                panelWhatAreServants.Visible = true;
                panelWhatAreServants_ForBenefitsPage.Visible = false;
                lblOffer.Visible = true;
                break;
            case ManagePFPages.Cancel:
                mnu_PF.Items[3].Selected = true;
                panelHireMore.Visible = true;
                panelWhatAreServants.Visible = false;
                panelWhatAreServants_ForBenefitsPage.Visible = false;
                lblOffer.Visible = false;
                break;
            case ManagePFPages.transfer:
                mnu_PF.Items[2].Selected = true;
                panelHireMore.Visible = true;
                panelWhatAreServants.Visible = false;
                panelWhatAreServants_ForBenefitsPage.Visible = false;
                lblOffer.Visible = false;

                break;
            default:
                throw new ArgumentException("not recognized value of managePFPages: " + managePFPages.ToString());
        }
    }


}
