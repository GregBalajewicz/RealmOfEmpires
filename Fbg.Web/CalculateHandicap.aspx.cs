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

public partial class QuickTransportCoinsPage : MyCanvasIFrameBasePage
{
    new  protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);

        MasterBase_PopupFullFunct masterPage = (MasterBase_PopupFullFunct)this.Master;
        masterPage.Initialize(FbgPlayer, MyVillages);
        Fbg.Bll.Village village = masterPage.CurrentlySelectedVillage;

        #region localzing some controls
        //
        // this is needed for localiztion, so that <%# ... %> work
        //
        this.rv_AttackUnitAmount.DataBind();
        this.itsMe_attacker.DataBind();
        this.linkPFAtt.DataBind();
        this.linkPFDef.DataBind();
        this.linkClose.DataBind();
        this.spanFindAttacker.DataBind();
        this.spanFindDefender.DataBind();
        this.btnFindAttacker.DataBind();
        this.btnFindDefender.DataBind();
        this.btnCalc.DataBind();
        this.RangeValidator1.DataBind();
        this.itsme_defender.DataBind();
        this.HyperLink1.DataBind();
        #endregion

        //
        // init screen based on Battle Sim PF 
        if (FbgPlayer.PF_HasPF(Fbg.Bll.CONSTS.PFs.BattleSimImprovements))
        {
            linkPFDef.Visible = false;
            linkPFAtt.Visible = false;
        }
        else
        {
            linkPFDef.NavigateUrl = NavigationHelper.PFOffMessage(Fbg.Bll.CONSTS.PFs.BattleSimImprovements);
            linkPFAtt.NavigateUrl = NavigationHelper.PFOffMessage(Fbg.Bll.CONSTS.PFs.BattleSimImprovements);
            spanFindDefender.Visible = false;
            spanFindAttacker.Visible = false;

            itsMe_attacker.CssClass = "LockedFeatureLinkSml";
            itsMe_attacker.Target = "_blank";
            itsMe_attacker.Attributes.Add("OnClick", "return popupUnlock(this)");
            itsMe_attacker.Style.Add("font-size", "7pt");
            itsMe_attacker.NavigateUrl = NavigationHelper.PFOffMessage(Fbg.Bll.CONSTS.PFs.BattleSimImprovements);

            itsme_defender.CssClass = "LockedFeatureLinkSml";
            itsme_defender.Target = "_blank";
            itsme_defender.Attributes.Add("OnClick", "return popupUnlock(this)");
            itsme_defender.Style.Add("font-size", "7pt");
            itsme_defender.NavigateUrl = NavigationHelper.PFOffMessage(Fbg.Bll.CONSTS.PFs.BattleSimImprovements);
        }

        itsMe_attacker.Attributes.Add("val", FbgPlayer.Points.ToString());
        itsme_defender.Attributes.Add("val", FbgPlayer.Points.ToString());
        
    }

    protected void btnCalc_Click(object sender, EventArgs e)
    {
        int attackersPoints;
        int defendersPoints;

        attackersPoints = Convert.ToInt32(txtAttackersPoints.Text.Trim() == String.Empty ? "0" : txtAttackersPoints.Text);
        defendersPoints = Convert.ToInt32(txtDefendersPoints.Text.Trim() == String.Empty ? "0" : txtDefendersPoints.Text);

        //
        // make sure the text boxes have the proper points
        //
        txtAttackersPoints.Text = attackersPoints.ToString();
        txtDefendersPoints.Text = defendersPoints.ToString();

        double handicap = FbgPlayer.Realm.CalcBattleHandicap(attackersPoints, defendersPoints, Fbg.Bll.CONSTS.VillageType.Normal);
        lblHandicap.Visible = true;
        lblHandicap.Text = RS("HandicapColon") + " " + (handicap*100).ToString("0.#") + "%";

        linkClose.Visible = true;
        linkClose.Attributes.Add("OnClick", "parent.ReceiveIframeInput('handicap', '" + (handicap * 100).ToString("0.#") + "'); parent.closeIFrame('*');");
    }
    protected void cvAttacker_ServerValidate(object source, ServerValidateEventArgs args)
    {
        Fbg.Bll.PlayerOther po = Fbg.Bll.PlayerOther.GetPlayer(FbgPlayer.Realm, args.Value, FbgPlayer.ID);

        if (po == null)
        {
            args.IsValid = false;
            if ((CustomValidator)source == cvAttacker)
            {
                spanFindAttacker.CssClass = "";
            }
            else
            {
                spanFindDefender.CssClass = "";
            }

            
        }
        else
        {
            if ((CustomValidator)source == cvAttacker)
            {
                txtAttackersPoints.Text = po.Points.ToString();
            }
            else
            {
                txtDefendersPoints.Text = po.Points.ToString();
            }
        }

    }
    protected void btnFindAttacker_Click(object sender, EventArgs e)
    {

    }
    protected void btnFindAttacker_Click1(object sender, EventArgs e)
    {

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
