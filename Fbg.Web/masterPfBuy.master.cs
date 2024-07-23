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

public partial class masterPfBuy : BaseMasterPage
{
    public masterPfBuy()
    {
        BaseResName = "masterPfBuy.master";
    }
    public bool _showFBCreditsOnly;
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public masterPf ContainerMasterPage
    {
        get
        {
            return (masterPf)this.Master;
        }
    }
    public Fbg.Bll.Player _player;
    public void Initialize(Fbg.Bll.Player player, List<VillageBase> villages, Tabs selectedTab,  bool showFBCreditsOnly)
    {
        ContainerMasterPage.Initialize(player, villages, masterPf.ManagePFPages.Credits);
        SetSelectedLink(selectedTab);
        _showFBCreditsOnly = showFBCreditsOnly;
        _player = player;
    }


    /// <summary>
    /// update the 'my credits' on screen 
    /// </summary>
    public void BindCredits()
    {
        ContainerMasterPage.BindCredits();
    }


    public enum Tabs
    {
        PayPal = 0,
        ROEOffers = 2,
        FbCredit = 3
    }
    private void SetSelectedLink(Tabs tab)
    {
        switch (tab)
        {
            case Tabs.PayPal:
                tabPayPal.Attributes.Add("class", "selected");
                break;
            case Tabs.ROEOffers:
                tabROEOffers.Attributes.Add("class", "selected");
                break;
            case Tabs.FbCredit:
                tabFbCredits.Attributes.Add("class", "selected");
                break;
            default:
                throw new ArgumentException("not recognized value of tab: " + tab.ToString());
        }
    }

}
