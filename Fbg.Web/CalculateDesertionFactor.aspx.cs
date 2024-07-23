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

public partial class CalculateDesertionFactor : MyCanvasIFrameBasePage
{
    new  protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);

        MasterBase_PopupFullFunct masterPage = (MasterBase_PopupFullFunct)this.Master;
        masterPage.Initialize(FbgPlayer, MyVillages);
        Fbg.Bll.Village village = masterPage.CurrentlySelectedVillage;

        #region Localize Controls
        //localize controls using <%#...%>
        this.RangeValidator1.DataBind();
        this.RangeValidator2.DataBind();
        this.RangeValidator3.DataBind();
        this.RangeValidator4.DataBind();
        this.RequiredFieldValidator1.DataBind();
        this.RequiredFieldValidator2.DataBind();
        this.RequiredFieldValidator3.DataBind();
        this.RequiredFieldValidator4.DataBind();
        this.linkClose.DataBind();
        this.HyperLink1.DataBind();
        #endregion

        if (!IsPostBack)
        {
            txt_FromX.Text = village.Cordinates.X.ToString();
            txt_FromY.Text = village.Cordinates.Y.ToString();
        }
    }


    protected void btnCalcDistance_Click(object sender, EventArgs e)
    {
        if (!IsValid)
        {
            return;
        }

        double distance = Fbg.Bll.Village.CalculateDistance(Convert.ToInt32(txt_FromX.Text), Convert.ToInt32(txt_FromY.Text)
            , Convert.ToInt32(txt_ToX.Text), Convert.ToInt32(txt_ToY.Text));

        lblHandicap.Visible = true;
        lblHandicap.Text = RS("lblHandicapText") + distance.ToString("0.#");

        linkClose.Visible = true;
        linkClose.Attributes.Add("OnClick", "parent.ReceiveIframeInput('desertion', '" + distance.ToString("0.#") + "'); parent.closeIFrame('*');");

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
