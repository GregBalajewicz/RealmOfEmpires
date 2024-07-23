using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fbg.Bll;

public partial class templates_GiftsPopup_d2 : TemplatePage
{
  

    public string exclude;
    public string content;

    int _numTimesThisPageSeen = 0;
    List<Fbg.Bll.Gift> _gifts;

    protected void Page_Load(object sender, EventArgs e)
    {
        //base.Page_Load(sender, e);

        exclude = GetExcludeList(FbgUser);


        //dlGifts.DataSource = FbgPlayer.GiftsAvailability;
        //dlGifts.DataBind();
    }

    public templates_GiftsPopup_d2()
    {
        // Use the same resource file as mobile
        R_OverridePageName = "templates_GiftsPopup.aspx";
    }

    protected string BindSendGiftMessage(object dataItem)
    {
        var giftAvail = (GiftAvailability)dataItem;
        return giftAvail.Gift.FBRequestContentText + " Could you help me by sending a gift back?";
    }

    private string GetExcludeList(Fbg.Bll.User user)
    {
        string friendsList = null;

        //
        // get ppl the player has already sent gifts to today
        //
        DataTable dt = user.Gifts_GetTodaysGifts();
        foreach (DataRow dr in dt.Rows)
        {
            friendsList += (string)dr[Fbg.Bll.User.CONSTS.TodaysGiftsColIndex.SentTo] + ",";
        }
        return friendsList;

    }
}