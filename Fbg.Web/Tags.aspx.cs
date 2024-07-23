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

public partial class Tags : MyCanvasIFrameBasePage
{
    private string _tagName = "";
    Fbg.Bll.Tags tags = null;
    main mainMasterPage;
    new protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);
        mainMasterPage = (main)this.Master;
        mainMasterPage.Initialize(FbgPlayer, MyVillages);

        if (FbgPlayer.PF_HasPF(Fbg.Bll.CONSTS.PFs.TagsAndFilters))
        {
            if (!IsPostBack)
            {
                CommandField cf = (CommandField)gv_Tags.Columns[0];
                cf.SelectText = RS("link_Select");
                gv_Tags.Columns[1].HeaderText = RS("h_Order");
                gv_Tags.Columns[2].HeaderText = RS("h_tagName");
                #region Localize Controls
                this.lbl_NoSelectedTag.DataBind();
                this.lbl_Result.DataBind();
                this.lbl_UpdateResult.DataBind();
                this.btn_AddTag.DataBind();
                this.btn_Delete.DataBind();
                this.btn_Save.DataBind();
                this.btn_Cancel.DataBind();
                this.RequiredFieldValidator1.DataBind();
                this.RangeValidator1.DataBind();
                #endregion
                BindGrids();
                gv_Tags.Columns[1].Visible = false;
            }
            if (gv_Tags.SelectedRow != null)
            {
                gv_Tags.SelectedRow.Font.Bold = false;
            }
        }
        else
        {
            //
            // no pf. Show no PF message
            //
            tblUI.Visible = false;
            panelPF.Visible = true;
            linkPF.NavigateUrl = NavigationHelper.PFOffMessage(Fbg.Bll.CONSTS.PFs.TagsAndFilters);
        }
    }
    void BindGrids()
    {
        DataTable dt = Fbg.Bll.Tags.GetTags(FbgPlayer);

        gv_Tags.DataSource = dt;
        gv_Tags.DataBind();
        gv_Tags.Columns[1].Visible = false;
    }
    protected void gv_Tags_SelectedIndexChanged(object sender, EventArgs e)
    {
        int tagID = Convert.ToInt32( gv_Tags.SelectedValue);

        BindData(tagID);
        pnl_SelectedTag.Visible = true;
        lbl_NoSelectedTag.Visible = false;
        lbl_Result.Visible = false ;
    }
    void BindData(int tagID)
    {
        tags = Fbg.Bll.Tags.GetTagByID(FbgPlayer, tagID);
        lbl_Name.Text = tags.Name;
        lbl_Desc.Text = tags.Desc;

        txt_Name.Text = tags.Name;
        txt_Desc.Text = tags.Desc;
        txt_Sort.Text = tags.Sort.ToString() ;

        _tagName = tags.Name;

        btn_Delete.CommandArgument = tags.ID.ToString();
        btn_Edit.CommandArgument = tags.ID.ToString();
        btn_Save.CommandArgument = tags.ID.ToString();
        btn_Cancel.CommandArgument = tags.ID.ToString();
        gv_VillagesWithTags.DataSource = tags.WithVillages;
        gv_VillagesWithTags.DataBind();

        gv_VillagesWithoutTags.DataSource = tags.WithoutVillages;
        gv_VillagesWithoutTags.DataBind();
        lbl_UpdateResult.Visible = false;

        for (int i = 0; i < gv_Tags.DataKeys.Count ; i++)
        {
            if (tagID ==(int) gv_Tags.DataKeys[i].Value)
            {
                gv_Tags.SelectedIndex = i;
            }
        }
        gv_Tags.SelectedRow.Font.Bold = true;

        lbl_VillagesWithTags.Text = RS("villWith") + " <I>" + _tagName + "</I> " + RS("_tag") + ":";
        lbl_VillagesWithoutTags.Text = RS("villWithout") + " <I>" + _tagName + "</I> " + RS("_tag") + ":";
    }
    protected void btn_Delete_Click(object sender, EventArgs e)
    {
        int tagID = Convert.ToInt32(btn_Delete.CommandArgument);
        Fbg.Bll.Tags.DeleteTag(FbgPlayer, tagID);
        Response.Redirect("tags.aspx");
        ////back to frist load status
        //pnl_SelectedTag.Visible = false;
        //lbl_NoSelectedTag.Visible = true;
        //lbl_Result.Visible = true;

    }
    protected string BindVillage(object dataItem)
    {
        string villageName = (string)DataBinder.Eval(dataItem, "Name");
        int xCord = (int)DataBinder.Eval(dataItem, "XCord");
        int yCord = (int)DataBinder.Eval(dataItem, "YCord");

        return string.Format("{0} ({1},{2})", villageName, xCord, yCord);

    }
    protected string BindTagName(object dataItem)
    {
        return RS("add") + " " + _tagName + " " + RS("_tag");

    }
    protected void btn_AddTag_Click(object sender, EventArgs e)
    {
        string tagName = Utils.ClearHTMLCode(txt_TagName.Text);
        tagName = Utils.ClearInvalidChars(tagName);
        if (!string.IsNullOrEmpty(tagName))
        {
            if (Fbg.Bll.Tags.AddTag(FbgPlayer, tagName))
            {
                txt_TagName.Text = "";
                FbgPlayer.Filters_Invalidate();
                Response.Redirect("tags.aspx");
            }
            else
            {
                lbl_Result.Text = RS("tagExists");
                lbl_Result.CssClass = "Error";
                lbl_Result.Visible = true;
            }
        }
    }
    protected void btn_Edit_Click(object sender, EventArgs e)
    {
        EnableEditMode(true);   
    }
    void EnableEditMode(bool action)
    {
        pnl_DisplayData.Visible = !action;
        pnl_EditData.Visible = action;
        gv_Tags.SelectedRow.Font.Bold = true;
        gv_Tags.Columns[1].Visible = action;
        //gv_Tags.ShowHeader = action;
        txt_TagName.Enabled = !action;
        btn_AddTag.Enabled = !action;
    }
    protected void btn_Save_Click(object sender, EventArgs e)
    {
        Int16 sort = 0;
        int tagID = Convert.ToInt32(btn_Save.CommandArgument);
        txt_Name.Text = Utils.ClearHTMLCode(txt_Name.Text);
        txt_Name.Text = Utils.ClearInvalidChars(txt_Name.Text);

        txt_Desc.Text = Utils.ClearHTMLCode(txt_Desc.Text);
        
        if (!string.IsNullOrEmpty(txt_Name.Text) && Int16.TryParse(txt_Sort.Text, out sort))
        {
            if (Fbg.Bll.Tags.UpdateTag(FbgPlayer, tagID, txt_Name.Text, txt_Desc.Text, sort))
            {

                BindGrids();
                BindData(tagID);
                EnableEditMode(false);

                main mainMasterPage = (main)this.Master;
                mainMasterPage.ReInitalize();
            }
            else
            {
                lbl_UpdateResult.Visible = true;
            }
        }
    }
    protected void btn_Cancel_Click(object sender, EventArgs e)
    {
        int tagID = Convert.ToInt32(btn_Cancel.CommandArgument);
        EnableEditMode(false);
        BindData(tagID);
    }
    protected void gv_VillagesWithTags_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Remove")
        {
            int villageID = Convert.ToInt32(e.CommandArgument);
            int tagID = Convert.ToInt32(btn_Edit.CommandArgument);
            Fbg.Bll.Tags.DeleteVillageTag(FbgPlayer, tagID, villageID);
            BindData(tagID);
        }
        
    }
    protected void gv_VillagesWithoutTags_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "AddTag")
        {
            int villageID = Convert.ToInt32(e.CommandArgument);
            int tagID = Convert.ToInt32(btn_Edit.CommandArgument);
            Fbg.Bll.Tags.AddVillageTag(FbgPlayer, tagID, villageID);
            BindData(tagID);
        }
    }
}
