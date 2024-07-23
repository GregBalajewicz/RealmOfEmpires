using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using Fbg.Bll;
using Gmbc.Common.Diagnostics.ExceptionManagement;



public partial class Gift_AccepteFirstTime : MyCanvasIFrameBasePage
{
    int _hourlySilverProd;
    int _numTimesThisPageSeen=0;
    Fbg.Bll.Village _village;
    new protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);

        master_PopupFullFunct masterPage = (master_PopupFullFunct)this.Master;
        masterPage.Initialize(FbgPlayer, MyVillages);
        _village = masterPage.CurrentlySelectedVillage;

        Gift gift = FbgPlayer.Realm.GiftByID(FbgPlayer.User.Gift_GiftRecentlyAccepted);
        if (gift == null)
        {
            Response.Redirect("villageoverview.aspx");
        }
        imgGift.ImageUrl = gift.AvailableImageUrl;
        LinkReturnFavour.NavigateUrl = String.Format("~/gift_send.aspx?{0}={1}", CONSTS.QuerryString.GiftID, gift.Id);
    }
}



