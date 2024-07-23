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

public partial class masterPfOffers : BaseMasterPage
{
    public masterPfOffers()
    {
        BaseResName = "masterPfOffers.master";
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public masterPfBuy ContainerMasterPage
    {
        get
        {
            return (masterPfBuy)this.Master;
        }
    }

    public void Initialize(Fbg.Bll.Player player, List<VillageBase> villages, Tabs selectedTab)
    {
     //   ContainerMasterPage.Initialize(player, villages, masterPfBuy.Tabs.ThridPartyOffers, true);
        SetSelectedLink(selectedTab);
    }


    public enum Tabs
    {
        OfferPal = 0,
        AdParlor= 1,
        SuperRewards =2,
        gWallet=3
    }
    private void SetSelectedLink(Tabs tab)
    {
        switch (tab)
        {
            case Tabs.OfferPal:
                tabOfferPal.Attributes.Add("class", "selected");
                break;
            //case Tabs.AdParlor:
            //    tabAdParlor.Attributes.Add("class", "selected");
            //    break;
            case Tabs.SuperRewards:
              //  tabSuperRewards.Attributes.Add("class", "selected");
                break;
            case Tabs.gWallet:
                tabGWallet.Attributes.Add("class", "selected");
                break;
            default:
                throw new ArgumentException("not recognized value of tab: " + tab.ToString());
        }
    }
}
