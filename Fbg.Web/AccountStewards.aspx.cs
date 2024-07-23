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


public partial class AccountStewards : MyCanvasIFrameBasePage
{

    public class localCONST
    {
        public class MyStewardsGridColumnIndex
        {
            public static int Cancel = 0;
        }
        public class StewardshipGridColumnIndex
        {
            public static int Status = 2;
            public static int Cancel = 0;
        }
    }

    MasterBase_Main mainMasterPage;

    new protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);

        mainMasterPage = (MasterBase_Main)this.Master;
        mainMasterPage.Initialize(FbgPlayer, MyVillages);

        if (FbgPlayer.Realm.StewardshipType == Fbg.Bll.Realm.StewardshipTypes.NoStewardShip)
        {
            panelStewardshipDisabled.Visible = true;
            pnlUI.Visible = false;
            return;
        }

        if (isMobile)
        {
            areaInfo.Visible = false;
            areapanelMyStewardship.Visible = false;
        }

        #region localzing some controls
        //
        // this is needed for localiztion, so that <%# ... %> work
        //
        if (!IsPostBack)
        {
            this.RequiredFieldValidator2.DataBind();
            this.customValidator_StewardName.DataBind();
            this.customValidator_StewardNameMe.DataBind();
            this.RequiredFieldValidator1.DataBind();
            this.customValidator_TransferPlayerName.DataBind();
            this.btnTransfer_go.DataBind();
            this.btnTransfer_cancel.DataBind();
            this.gvMyStewardship.DataBind();
            this.btnAppoint.DataBind();
            this.lblAcceptRules.DataBind();
            this.cbRulesAccepted.DataBind();
        }
        #endregion 

        ((CommandField) gvMyStewards.Columns[localCONST.MyStewardsGridColumnIndex.Cancel]).DeleteText = RS("SquareBracketsCancelAppointment");

        lblAcceptRules.Visible = false;
        if (IsLoggedInAsSteward)
        {
            panelStewardshipTransfer.Visible = true;
            panelStewardsOfMyKingdom.Visible = false;
            panelMyStewardship.Visible = false;
        }
        else
        {
            DataBindMyStewards();

            gvMyStewardship.DataSource = FbgPlayer.Stewardship_GetMyStewardship();
            gvMyStewardship.DataBind();
        }
    }

    private void DataBindMyStewards()
    {
        gvMyStewards.DataSource = FbgPlayer.Stewardship_GetMyStewards();
        gvMyStewards.DataBind();

        if (((DataTable)gvMyStewards.DataSource).Rows.Count > 0)
        {
            panelAppointSteward.Visible = false;
        }
        else
        {
            panelAppointSteward.Visible = true;
        }
    }

    protected void btnTransfer_Click(object sender, EventArgs e)
    {
        if (!IsValid)
        {
            return;
        }
        if (!cbRulesAccepted.Checked)
        {
            lblAcceptRules.Visible = true;
            return;
        }

        //
        // THIS IS WRONG!!
        //   First of all, it should be in FbgPlayer.SetSteward, and secondly, we dont know who the loggedin player is
        //
        //if (txtPlayerName.Text.Trim().ToLower() == FbgPlayer.Name.ToLower())
        //{
            // do not allow transfer to my self
          //  customValidator_StewardNameMe.IsValid = false;
        //}

        Fbg.Bll.Player.SetStewardResult result = FbgPlayer.Stewardship_SetSteward(txtPlayerName.Text.Trim());
        if (result.result == Fbg.Bll.Player.SetStewardResult.ResultCode.fail_playerNotFound)
        {
            customValidator_StewardName.IsValid = false;
        }
        DataBindMyStewards();
    }
    protected void gvMyStewards_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        FbgPlayer.Stewardship_DeleteAppointedSteward((int)((GridView)sender).DataKeys[e.RowIndex][0]);//gvMyStewards.Rows[e.RowIndex].Cells["RecordID"]);
        DataBindMyStewards();
    }
    protected void gvMyStewards_RowCommand(object sender, GridViewCommandEventArgs e)
    {

    }

    public string BindState(object o)
    {
        Int16 state = (Int16)o;
        string ret = string.Empty;
        switch (state)
        {
            case 1:
                ret = RS("PendingAcceptance");
                break;
            case 2:
                ret = RS("AcceptedAndActive");
                break;
            default:
                throw new NotImplementedException("urecognized:" + state.ToString());
        }
        return ret;
    }
    protected void gvMyStewardship_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {

    }
    protected void gvMyStewardship_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int recordID = Convert.ToInt32(e.CommandArgument);
        switch (e.CommandName)
        {
            case "accept":
                if (!cbRulesAccepted.Checked)
                {
                    lblAcceptRules.Visible = true;
                    return;
                }
                FbgPlayer.Stewardship_AcceptStewardship(recordID);
                break;
            case "cancel":
                FbgPlayer.Stewardship_CancelStewardship(recordID);
                break;
        }

        gvMyStewardship.DataSource = FbgPlayer.Stewardship_GetMyStewardship();
        gvMyStewardship.DataBind();
    }
    protected void gvMyStewardship_DataBinding(object sender, EventArgs e)
    {
        
    }
    protected void gvMyStewardship_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {            
            HyperLink linkLogin= (HyperLink)e.Row.FindControl("linkLogin");
            LinkButton btnCancelAccept = (LinkButton)e.Row.Cells[localCONST.StewardshipGridColumnIndex.Cancel].Controls[0];
            switch ((Int16)((DataTable)gvMyStewardship.DataSource).Rows[e.Row.RowIndex][Fbg.Bll.Player.CONSTS.MyStewardshipColIndex.State])
            {
                case 1:
                    linkLogin.Visible = false;
                    btnCancelAccept.Text = RS("AcceptSquareBrackets");
                    btnCancelAccept.CommandName = "accept";
                    btnCancelAccept.CommandArgument = ((DataTable)gvMyStewardship.DataSource).Rows[e.Row.RowIndex]["RecordID"].ToString();
                    break;
                case 2:
                    linkLogin.Visible = true;

                    linkLogin.NavigateUrl = Config.BaseUrl+ String.Format("AccountStewards_LoginAs.aspx?{0}={1}"
                        , CONSTS.QuerryString.RecordID, (Int32)((DataTable)gvMyStewardship.DataSource).Rows[e.Row.RowIndex][Fbg.Bll.Player.CONSTS.MyStewardshipColIndex.RecordID]);

                    if (isD2) {
                        linkLogin.Target = "_parent";
                    }

                    btnCancelAccept.Text = RS("CancelSquareBrackets");
                    btnCancelAccept.CommandName = "cancel";
                    btnCancelAccept.CommandArgument = ((DataTable)gvMyStewardship.DataSource).Rows[e.Row.RowIndex]["RecordID"].ToString();
//------------------Could Not Add RS Tag                    
                    btnCancelAccept.Attributes.Add("onclick", "return confirm('Are you sure? This cannot be undone. Consider transferring the stewardship to another player. Login as this player to do so and go to Account Stewardship again.');");
                    break;
            }
        }
    }
    protected void gvMyStewardship_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        if (!IsValid)
        {
            return;
        }
        if (!cbRulesAccepted.Checked)
        {
            lblAcceptRules.Visible = true;
            return;
        }


        Fbg.Bll.Player.TransferStewardResult result = FbgPlayer.Stewardship_TransferSteward_Try(txtTransferToPlayerName.Text.Trim());
        if (result.result == Fbg.Bll.Player.TransferStewardResult.ResultCode.fail_playerNotFound)
        {
            customValidator_TransferPlayerName.IsValid = false;
            return;
        }
        else if (result.result == Fbg.Bll.Player.TransferStewardResult.ResultCode.fail_triedTransferToSelf)
        {
            customValidator_TransferToSelf.IsValid = false;
            return;
        }

        txtTransferToPlayerName.Enabled = false;
        btnTransfer.Enabled = false;

        panelTransfer_confirm.Visible = true;

        linkTransferingTo.Text = result.steward.PlayerName;
        linkTransferingTo.NavigateUrl = NavigationHelper.PlayerPublicOverview(result.steward.PlayerID);
    }

    protected void btnTransfer_go_Click(object sender, EventArgs e)
    {
        FbgPlayer.Stewardship_TransferSteward(txtTransferToPlayerName.Text.Trim());
        Response.Redirect("LogoutOfRealm.aspx");
    }
    protected void btnTransfer_cancel_Click(object sender, EventArgs e)
    {

        txtTransferToPlayerName.Enabled = true;
        btnTransfer.Enabled = true;

        panelTransfer_confirm.Visible = false;
    }
    protected void gvMyStewards_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            LinkButton btnCancel = (LinkButton)e.Row.Cells[localCONST.MyStewardsGridColumnIndex.Cancel].Controls[0];
            switch ((Int16)((DataTable)gvMyStewards.DataSource).Rows[e.Row.RowIndex][Fbg.Bll.Player.CONSTS.MyStewardsColIndex.State])
            {
                case 2:
//------------------Could Not Add RS Tag
                    btnCancel.Attributes.Add("onclick", "return confirm('Are you sure? This will prevent your steward from being able to manage your kingdom.');");
                    break;
            }
        }
    }
    protected override void OnPreInit(EventArgs e)
    {

        if (isMobile)
        {
            base.MasterPageFile = "masterMain_m.master";
        }

        if (isD2)
        {
            base.MasterPageFile = "masterMain_d2.master";
        } 
    

        base.OnPreInit(e);
    }

}
