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

public partial class BlockedPlayers : MyCanvasIFrameBasePage
{
    new protected void Page_Load(object sender, EventArgs e)
    {
        MasterBase_Main mainMasterPage = (MasterBase_Main)this.Master;
        mainMasterPage.Initialize(FbgPlayer, MyVillages);
        MailMenu1.Initialize(FbgPlayer,Controls_MailMenu.MailMenuPages.BlockedPlayers, isMobile);

        #region localzing some controls
        //
        // this is needed for localiztion, so that <%# ... %> work
        //
        if (!IsPostBack)
        {
            this.gv_BlockedPlayers.DataBind();
        }
        #endregion

        BindGird();
    }
    void BindGird()
    {
        gv_BlockedPlayers.DataSource = Fbg.Bll.Mail.GetBlockedPlayers(FbgPlayer);
        gv_BlockedPlayers.DataBind();
    }
    protected void gv_BlockedPlayers_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "UnBlock")
        {
            int BlockedPlayerID= Convert.ToInt32(e.CommandArgument);
            Fbg.Bll.Mail.UnBlockPlayer(FbgPlayer, BlockedPlayerID);
            BindGird();
        }
    }
    protected void gv_BlockedPlayers_OnPageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gv_BlockedPlayers.PageIndex = e.NewPageIndex;
        BindGird();
    }
    protected override void OnPreInit(EventArgs e)
    {

        if (isMobile)
        {
            base.MasterPageFile = "masterMain_m.master";
        }
        base.OnPreInit(e);
    }
}
