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
using System.Collections.Generic;

using Fbg.Bll;

public partial class Tags_Filters : MyCanvasIFrameBasePage
{
    private string _filterName = "";
    Fbg.Bll.Filter filters = null;
    struct FilterAndItsTags
    {
        public FilterAndItsTags(Label  lblID, CheckBox  tag)
        {
            this.lbl = lblID ;
            this.chk = tag ;
        }
        public Label lbl;
        public CheckBox chk;
  
    }

    List<FilterAndItsTags> chkFilterTags;
    main mainMasterPage;
    new protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);
        mainMasterPage = (main)this.Master;
        mainMasterPage.Initialize(FbgPlayer, MyVillages);
        
        #region Localize Controls
        if (!IsPostBack)
        {
            CommandField cf = (CommandField)gv_Filters.Columns[0];
            cf.SelectText = RS("link_Select");
            gv_Filters.Columns[1].HeaderText = RS("order");
            gv_Filters.Columns[2].HeaderText = RS("h_FilterName");
            this.lblNotagsMsg.DataBind();
            this.lbl_NoSelectedTag.DataBind();
            this.lbl_UpdateResult.DataBind();
            this.btn_AddFilter.DataBind();
            this.btn_Delete.DataBind();
            this.btn_Save.DataBind();
            this.btn_Cancel.DataBind();
            this.btn_UpdateFilterTags.DataBind();
            this.RequiredFieldValidator1.DataBind();
            this.RangeValidator1.DataBind();
        }
        #endregion

        if (FbgPlayer.PF_HasPF(Fbg.Bll.CONSTS.PFs.TagsAndFilters))
        {
            //
            // if no tag defiend, display an appropriate message otherwise init the page
            if (Fbg.Bll.Tags.GetTags(FbgPlayer).Rows.Count != 0)
            {
                if (!IsPostBack)
                {
                    BindGrids();
                    gv_Filters.Columns[1].Visible = false;
                }
            }
            else
            {
                lblNotagsMsg.Visible = true;
                panelUI.Visible = false;
            }

            if (gv_Filters.SelectedRow != null)
            {
                gv_Filters.SelectedRow.Font.Bold = false;
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
    void BindGrids()
    {
        List<Fbg.Bll.FilterBase> filters = FbgPlayer.Filters;

        gv_Filters.DataSource = filters;
        gv_Filters.DataBind();
        gv_Filters.Columns[1].Visible = false;
        
    }
  
    void BindData(int filterID)
    {
        chkFilterTags = new List<FilterAndItsTags>();
        filters = Fbg.Bll.Filter.GetFilterByID(FbgPlayer, filterID );
        lbl_Name.Text = filters.Name;
        lbl_Desc.Text = filters.Desc;

        txt_Name.Text = filters.Name;
        txt_Desc.Text = filters.Desc;
        txt_Sort.Text = filters.Sort.ToString() ;

        _filterName = filters.Name;

        btn_Delete.CommandArgument = filters.ID.ToString();
        btn_Edit.CommandArgument = filters.ID.ToString();
        btn_Save.CommandArgument = filters.ID.ToString();
        btn_Cancel.CommandArgument = filters.ID.ToString();
        lbl_UpdateResult.Visible = false;

        for (int i = 0; i < gv_Filters.DataKeys.Count ; i++)
        {
            if (filterID ==(int) gv_Filters.DataKeys[i].Value)
            {
                gv_Filters.SelectedIndex = i;
            }
        }
        gv_Filters.SelectedRow.Font.Bold = true;

        //PopulateSelectedTags(filterID, filters.Tags);
        BindSelectedTags(filterID, filters.Tags);
    }
    protected void btn_Delete_Click(object sender, EventArgs e)
    {
        int filterID = Convert.ToInt32(btn_Delete.CommandArgument);
        Fbg.Bll.Filter.DeleteFilter (FbgPlayer, filterID );
        FbgPlayer.Filters_Invalidate();
        Response.Redirect("tags_filters.aspx");

    }
    protected void btn_AddFilter_Click(object sender, EventArgs e)
    {
        string filterName = Utils.ClearHTMLCode(txt_FilterName.Text);
        if (!string.IsNullOrEmpty(filterName))
        {
            if (Fbg.Bll.Filter.AddFilter(FbgPlayer, filterName))
            {
                txt_FilterName.Text = "";
                FbgPlayer.Filters_Invalidate();
                Response.Redirect("tags_filters.aspx");
            }
            else
            {
                lbl_Result.Text = RS("filterExists");
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
        gv_Filters.SelectedRow.Font.Bold = true;
        gv_Filters.Columns[1].Visible = action;
        txt_FilterName.Enabled = !action;
        btn_AddFilter.Enabled = !action;
    }
    protected void btn_Save_Click(object sender, EventArgs e)
    {
        Int16 sort = 0;
        int filterID = Convert.ToInt32(btn_Save.CommandArgument);
        txt_Name.Text = Utils.ClearHTMLCode(txt_Name.Text);
        txt_Desc.Text = Utils.ClearHTMLCode(txt_Desc.Text);

        if (!string.IsNullOrEmpty(txt_Name.Text) && Int16.TryParse(txt_Sort.Text, out sort))
        {
            if (Fbg.Bll.Filter.UpdateFilter(FbgPlayer, filterID, txt_Name.Text, txt_Desc.Text, sort))
            {
                FbgPlayer.Filters_Invalidate();
                BindGrids();
                BindData(filterID);
                EnableEditMode(false);
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
        int filterID = Convert.ToInt32(btn_Cancel.CommandArgument);
        EnableEditMode(false);
        BindData(filterID);
    }
    protected void gv_Filters_SelectedIndexChanged(object sender, EventArgs e)
    {

        int filterID = Convert.ToInt32(gv_Filters.SelectedValue);

        BindData(filterID);
        pnl_SelectedTag.Visible = true;
        lbl_NoSelectedTag.Visible = false;
        lbl_Result.Visible = false;
    }
    void BindSelectedTags(int filterID,DataTable selectedTags)
    {
        chklst_SelectedTags.DataSource = selectedTags;
        chklst_SelectedTags.DataBind();
        for (int i = 0; i < selectedTags.Rows.Count; i++)
        {
            if ((int)selectedTags.Rows[i][Fbg.Bll.Filter.CONSTS.SelectedTagColumnIndex.FilterID] == filterID)
            {
                chklst_SelectedTags.Items[i].Selected = true;
            }
        }
    }
    protected void btn_UpdateFilterTags_Click(object sender, EventArgs e)
    {
        string tagIDs="";
        int filterID=Convert.ToInt32(btn_Edit.CommandArgument);
        //BindData(filterID);
        foreach (ListItem   lt in chklst_SelectedTags.Items )
        {

            if (lt.Selected)
            {
                tagIDs += lt.Value  + ",";
            }
        }
        Fbg.Bll.Filter.UpdateFilterTags(FbgPlayer,filterID ,tagIDs );
        gv_Filters.SelectedRow.Font.Bold = true;
    }
    
}
