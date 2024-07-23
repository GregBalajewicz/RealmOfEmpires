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

public partial class Tags_VillageTags : MyCanvasIFrameBasePage
{
    main mainMasterPage;
    new protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);
        mainMasterPage = (main)this.Master;
        mainMasterPage.Initialize(FbgPlayer, MyVillages);

        #region Localize Control
        if (!IsPostBack)
        {
            this.lblNotagsMsg.DataBind();
        }
        #endregion

        if (FbgPlayer.PF_HasPF(Fbg.Bll.CONSTS.PFs.TagsAndFilters))
        {

            //
            // if no tag defiend, display an appropriate message otherwise init the page
            if (!IsPostBack)
            {
                if (Fbg.Bll.Tags.GetTags(FbgPlayer).Rows.Count != 0)
                {
                    int villageID = mainMasterPage.CurrentlySelectedVillageBasicB.id;
                    BindData(villageID);
                }
                else
                {
                    lblNotagsMsg.Visible = true;
                    panelUI.Visible = false;
                }
            }
        }
        else
        {
            //
            // no pf. Show no PF message
            //
            lblNotagsMsg.Visible = false;
            panelUI.Visible = false;
            panelPF.Visible = true;
            linkPF.NavigateUrl = NavigationHelper.PFOffMessage(Fbg.Bll.CONSTS.PFs.TagsAndFilters);
        }

    }
    protected void gv_TagsWithVillage_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Remove")
        {
            int villageID = Convert.ToInt32(lbl_VillageID.Text);
            int tagID = Convert.ToInt32(e.CommandArgument);
            Fbg.Bll.Tags.DeleteVillageTag(FbgPlayer, tagID, villageID);
            BindData(villageID);
            mainMasterPage.ReInitalize();
        }
    }
    protected void gv_TagsWithoutVillage_RowCommand(object sender, GridViewCommandEventArgs e)
    {

        if (e.CommandName == "AddTag")
        {
            int villageID = Convert.ToInt32(lbl_VillageID.Text);
            int tagID = Convert.ToInt32(e.CommandArgument);
            Fbg.Bll.Tags.AddVillageTag(FbgPlayer, tagID, villageID);
            BindData(villageID);
            mainMasterPage.ReInitalize();
        }
    }
    protected void gv_Villages_SelectedIndexChanged(object sender, EventArgs e)
    {
    }
    protected string BindVillage(object dataItem)
    {
        string villageName = (string)DataBinder.Eval(dataItem, "name");
        int xCord = (int)DataBinder.Eval(dataItem, "xcord");
        int yCord = (int)DataBinder.Eval(dataItem, "ycord");

        return string.Format("{0} ({1},{2})", villageName, xCord, yCord);

    }
    void BindData(int villageID)
    {
        DataSet ds=Fbg.Bll.Tags.GetTagsByVillageID(FbgPlayer,villageID);
        lbl_VillageName.Text = RS("canUnTag") +" - " + FbgPlayer.VillageBasicB( villageID ).name ;
        lbl_VillageID.Text = villageID.ToString();
        gv_TagsWithVillage.DataSource = ds.Tables[Fbg.Bll.Tags.CONSTS.VillageTagTableIndex.WithTags];
        gv_TagsWithVillage.DataBind();

        gv_TagsWithoutVillage.DataSource = ds.Tables[Fbg.Bll.Tags.CONSTS.VillageTagTableIndex.WithoutTags];
        gv_TagsWithoutVillage.DataBind();
        //gv_Villages.SelectedRow.Font.Bold = true;
    }
}
