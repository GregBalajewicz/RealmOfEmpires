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
using MB.TheBeerHouse;


    public partial class ManageForums : MyCanvasIFrameBasePage
   {
        int ClanID = 0;
        int ForumID = 0;
        new protected void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            MasterBase_Main mainMasterPage = (MasterBase_Main)this.Master;
            mainMasterPage.Initialize(FbgPlayer, MyVillages);
            ClanMenu1.Player = FbgPlayer;
            ClanMenu1.IsMobile = isMobile;
            ClanMenu1.CurrentPage = Controls_ClanMenu.ManageClanPages.ForumAdmin;
            if (isMobile)
            {
                ClanMenu1.Visible = false;
            }
            #region Localize Controls
            if (!IsPostBack)
            {
                this.cbShowSafe.DataBind();
            }
            #endregion

            //
            // 
            // get the clan of the logged in player and DO SECURITY CHECK
            //    Ensure this player is part of this clan and has proper roles to see this page. 
            //
            ClanID = FbgPlayer.Clan == null ? 0 : FbgPlayer.Clan.ID;
            if (ClanID == 0)
            {
                Response.Redirect("AccessDenied.aspx");
            }
            if (!FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.Owner)
                && !FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.ForumAdministrator)
                && !FbgPlayer.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.Administrator))
            {
                Response.Redirect("AccessDenied.aspx");
            }

            //
            // display the forums table
            //
            if (!IsPostBack)
            {
                BindForums();
            }
        }


        protected void BindForums()
        {
            gvwForums.DataSource = new Fbg.Forum.SqlForumsProvider().GetForumsByClanID(
                    FbgPlayer.Clan.ID
                    , FbgPlayer.ID
                    , false
                    , FbgPlayer.Realm.ConnectionStr);

                gvwForums.DataBind();
                if (gvwForums.SelectedValue != null)
                {
                    ForumID = (int)gvwForums.SelectedValue;
                }
        }

      protected void gvwForums_SelectedIndexChanged(object sender, EventArgs e)
      {
         dvwForum.ChangeMode(DetailsViewMode.Edit);
         List<Fbg.Forum.ForumDetails> Obj = new List<Fbg.Forum.ForumDetails>();
         Obj.Add(new Fbg.Forum.SqlForumsProvider().GetForumByID((int)gvwForums.SelectedValue, FbgPlayer.ID, FbgPlayer.Realm.ConnectionStr));
         dvwForum.DataSource = Obj;
        
         dvwForum.DataBind();
        
          ((DropDownList)dvwForum.FindControl("cmb_SecurityLevel")).SelectedValue = Obj[0].SecurityLevel.ToString() ;

          
      }

      protected void gvwForums_RowDeleted(object sender, GridViewDeletedEventArgs e)
      {
         gvwForums.SelectedIndex = -1;
         gvwForums.DataBind();
         dvwForum.ChangeMode(DetailsViewMode.Insert);
      }

      protected void gvwForums_RowCreated(object sender, GridViewRowEventArgs e)
      {
         if (e.Row.RowType == DataControlRowType.DataRow)
         {
            ImageButton btn = e.Row.Cells[1].Controls[0] as ImageButton;
            btn.OnClientClick = "if (confirm('" + RS("sureToDelete") + "') == false) return false;";

            dvwForum.Fields[1].HeaderText = RS("createdOn");
            dvwForum.Fields[2].HeaderText = RS("createdBy");
         }
      }

      protected void dvwForum_ItemInserted(object sender, DetailsViewInsertedEventArgs e)
      {
         gvwForums.SelectedIndex = -1;
         gvwForums.DataBind();
      }

      protected void dvwForum_ItemUpdated(object sender, DetailsViewUpdatedEventArgs e)
      {
         gvwForums.SelectedIndex = -1;
         gvwForums.DataBind();
      }

      protected void dvwForum_ItemCreated(object sender, EventArgs e)
      {
         if (dvwForum.CurrentMode == DetailsViewMode.Insert)
         {
          //  TextBox txtImportance = dvwForum.FindControl("txtImportance") as TextBox;
          //  txtImportance.Text = "0";
             DropDownList ddl = dvwForum.FindControl("cmb_SecurityLevel") as DropDownList;
             ddl.Items.Insert(0, new ListItem(RS("li_allMembers"), "0"));
             ddl.Items[0].Selected = true;
             ddl.Items.Insert(1, new ListItem(RS("li_onlyOwnAdm"), "1"));
             ddl.Items.Insert(2, new ListItem(RS("li_onlyOwnAdmDip"), "2"));
             ddl.Items.Insert(3, new ListItem(RS("li_onlyOwnAdmDipFA"), "3"));
             ddl.Items.Insert(4, new ListItem(RS("li_allRoles"), "4"));
         }

         if (dvwForum.CurrentMode == DetailsViewMode.Edit)
         {
             DropDownList ddl = dvwForum.FindControl("cmb_SecurityLevel") as DropDownList;
             ddl.Items.Insert(0, new ListItem(RS("li_allMembers"), "0"));
             ddl.Items[0].Selected = true;
             ddl.Items.Insert(1, new ListItem(RS("li_onlyOwnAdm"), "1"));
             ddl.Items.Insert(2, new ListItem(RS("li_onlyOwnAdmDip"), "2"));
             ddl.Items.Insert(3, new ListItem(RS("li_onlyOwnAdmDipFA"), "3"));
             ddl.Items.Insert(4, new ListItem(RS("li_allRoles"), "4"));
         }
      }

      protected void dvwForum_ItemCommand(object sender, DetailsViewCommandEventArgs e)
      {
         if (e.CommandName == "Cancel")
         {
             Server.Transfer("~/ManageForums.aspx", false);
            //gvwForums.SelectedIndex = -1;
            //gvwForums.DataBind();
         }
         else if (e.CommandName == "Insert")
         {

             TextBox txtTitle = dvwForum.FindControl("txtTitle") as TextBox;
            // TextBox txtImportance = dvwForum.FindControl("txtImportance") as TextBox;
             //TextBox txtImageUrl = dvwForum.FindControl("txtImageUrl") as TextBox;
             TextBox txtDescription = dvwForum.FindControl("txtDescription") as TextBox;
             CheckBox chkModirated = dvwForum.FindControl("chkModerated") as CheckBox;
             TextBox txtSortOrder = dvwForum.FindControl("txtImportance") as TextBox;
             CheckBox chkAlertClanMembers = dvwForum.FindControl("chkAlertClanMembers") as CheckBox;
             DropDownList cmb_SecurityLevel = dvwForum.FindControl("cmb_SecurityLevel") as DropDownList;

             int sortOrder = 0;
             int.TryParse(txtSortOrder.Text, out sortOrder);
             new Fbg.Forum.SqlForumsProvider().InsertForum(DateTime.Now, FbgPlayer.Name, txtTitle.Text, chkModirated.Checked, sortOrder, "", txtDescription.Text, FbgPlayer.Clan.ID,chkAlertClanMembers.Checked ,byte.Parse(cmb_SecurityLevel.SelectedValue ), FbgPlayer.Realm.ConnectionStr);
             Response.Redirect("ManageForums.aspx");
         }
         else if (e.CommandName == "Update")
         {
             Label lblID = dvwForum.FindControl("lblID") as Label;
             TextBox txtTitle = dvwForum.FindControl("txtTitle") as TextBox;
            // TextBox txtImportance = dvwForum.FindControl("txtImportance") as TextBox;
            // TextBox txtImageUrl = dvwForum.FindControl("txtImageUrl") as TextBox;
             TextBox txtDescription = dvwForum.FindControl("txtDescription") as TextBox;
             CheckBox chkModirated = dvwForum.FindControl("chkModerated") as CheckBox;
             TextBox txtSortOrder = dvwForum.FindControl("txtImportance") as TextBox;
             CheckBox chkAlertClanMembers = dvwForum.FindControl("chkAlertClanMembers") as CheckBox;
             DropDownList cmb_SecurityLevel = dvwForum.FindControl("cmb_SecurityLevel") as DropDownList;
             int sortOrder = 0;
             int.TryParse(txtSortOrder.Text, out sortOrder);
             new Fbg.Forum.SqlForumsProvider().UpdateForum(txtTitle.Text, chkModirated.Checked, sortOrder, "", txtDescription.Text,chkAlertClanMembers.Checked ,byte.Parse(cmb_SecurityLevel.SelectedValue), Convert.ToInt32(lblID.Text) , FbgPlayer.Realm.ConnectionStr);
             Response.Redirect("ManageForums.aspx");

            
         }
      }
        protected void dvwForum_ItemInserting(object sender, DetailsViewInsertEventArgs e)
        {
            //new MB.TheBeerHouse.DAL.SqlClient.SqlForumsProvider().InsertForum (new MB.TheBeerHouse.DAL.ForumDetails (0,DateTime.Now,FbgPlayer.Name ,e.Values[0].ToString (),bool.Parse( e.Values[1].ToString()),int.Parse(e.Values[2].ToString ()),e.Values[4].ToString(),e.Values[3].ToString(),ClanID ),FbgPlayer.Realm.ConnectionStr );
            
        }
        protected void gvwForums_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {   
            int ForumID = Convert.ToInt32(((GridView)sender).DataKeys[e.RowIndex].Value);
            new Fbg.Forum.SqlForumsProvider().DeleteForum(ForumID, FbgPlayer.Realm.ConnectionStr);
            Response.Redirect("ManageForums.aspx");
        }

        protected void objCurrForum_Inserting(object sender, ObjectDataSourceMethodEventArgs e)
        {
            //TextBox txtTitle = dvwForum.FindControl("txtTitle") as TextBox;
            //TextBox txtImportance = dvwForum.FindControl("txtImportance") as TextBox;
            //TextBox txtImageUrl = dvwForum.FindControl("txtImageUrl") as TextBox;
            //TextBox txtDescription = dvwForum.FindControl("txtDescription") as TextBox;
            //CheckBox chkModirated = dvwForum.FindControl("chkModerated") as CheckBox;
            //new MB.TheBeerHouse.DAL.SqlClient.SqlForumsProvider().InsertForum( DateTime.Now, FbgPlayer.Name, txtTitle.Text, chkModirated.Checked, int.Parse(txtImportance.Text), txtDescription.Text, txtImageUrl.Text, FbgPlayer.Clan.ID , FbgPlayer.Realm.ConnectionStr);
            //Response.Redirect("ManageForums.aspx");
        }
        protected void dvwForum_ItemUpdating(object sender, DetailsViewUpdateEventArgs e)
        {

        }
        protected void objCurrForum_Updating(object sender, ObjectDataSourceMethodEventArgs e)
        {
            // Label  lblID = dvwForum.FindControl("lblID") as Label ;
            //TextBox txtTitle = dvwForum.FindControl("txtTitle") as TextBox;
            //TextBox txtImportance = dvwForum.FindControl("txtImportance") as TextBox;
            //TextBox txtImageUrl = dvwForum.FindControl("txtImageUrl") as TextBox;
            //TextBox txtDescription = dvwForum.FindControl("txtDescription") as TextBox;
            //CheckBox chkModirated = dvwForum.FindControl("chkModerated") as CheckBox;
            //new MB.TheBeerHouse.DAL.SqlClient.SqlForumsProvider().UpdateForum ( txtTitle.Text, chkModirated.Checked, int.Parse(txtImportance.Text), txtDescription.Text, txtImageUrl.Text,Convert.ToInt32( lblID.Text) , FbgPlayer.Realm.ConnectionStr);
            //Response.Redirect("ManageForums.aspx");
        }
        protected void dvwForum_ModeChanging(object sender, DetailsViewModeEventArgs e)
        {

            
        }

        protected string RemoveHtmlConditional(object text)
        {
            if (cbShowSafe.Checked)
            {
                return Utils.ChangeTabBreak(((string)text).Replace("<", "&lt").Replace(">", "&gt"));
            }
            else
            {
                return (string)text;
            }
        }


        protected void cbShowSafe_CheckedChanged(object sender, EventArgs e)
        {
            BindForums();

        }
    protected override void OnPreInit(EventArgs e)
    {

        if (isMobile) 
        {
            base.MasterPageFile = "masterMain_m.master";
        }
        else if (isD2)
        {
            base.MasterPageFile = "masterMain_d2.master";
        }
        base.OnPreInit(e);
    }

    }

