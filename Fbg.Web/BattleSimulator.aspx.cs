using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic ;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Fbg.Bll;
public partial class BattleSimulator : MyCanvasIFrameBasePage
{
    enum ReportPasteAs
    {
        attacker,
        defender
    }
    enum ReportPasteWho
    {
        attacker,
        defender
    }

    readonly int DESERTED_COLUMN = 4;

    Fbg.Bll.Village _village = null;
    int recordID;
    Report_BattleDet report;
    ReportPasteAs report_pasteAs;
    ReportPasteWho report_pasteWho;
    DataRow[] report_unitsToPaste =null;
    Fbg.Bll.Village village = null;
    new protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);
        MasterBase_Main mainMasterPage = (MasterBase_Main)this.Master;
        mainMasterPage.Initialize(FbgPlayer, MyVillages,true );
        village = mainMasterPage.CurrentlySelectedVillage;

        #region localzing some controls
        //
        // this is needed for localiztion, so that <%# ... %> work
        //
        this.lnk_AttackBouns.DataBind();
        this.lnk_DefenseBouns.DataBind();
        this.rv_WallLevel.DataBind();
        this.rv_WallLevelMaxLevel.DataBind();
        this.rv_TowerLevel.DataBind();
        this.rv_TowerLevelMaxLevel.DataBind();
        this.RangeValidator1.DataBind();
        this.RangeValidator2.DataBind();
        this.lbl_BuildingName.DataBind();
        this.btn_Simulate.DataBind();
        #endregion 

        // Update the wall level validator with the realm's max level for wall
        this.rv_WallLevelMaxLevel.MaximumValue = FbgPlayer.Realm.BuildingType_Wall.MaxLevel+"";
        this.rv_WallLevelMaxLevel.ToolTip = this.rv_WallLevelMaxLevel.ToolTip + FbgPlayer.Realm.BuildingType_Wall.MaxLevel;

        // Update the tower level validator with the realm's max level for tower
        this.rv_TowerLevelMaxLevel.MaximumValue = FbgPlayer.Realm.BuildingType_DefenseTower.MaxLevel + "";
        this.rv_TowerLevelMaxLevel.ToolTip = this.rv_TowerLevelMaxLevel.ToolTip + FbgPlayer.Realm.BuildingType_DefenseTower.MaxLevel;

        TroopsMenu1.Initialize(FbgPlayer.NumberOfVillages, Controls_TroopsMenu.ManageTroopsPages.BattleSimulator);
        TroopsMenu1.Visible = !isMobile;
        //
        // if we got a report ID AND if we have the PF, paste remaining troops of defender to battle sim
        //
        if (!IsPostBack)
        {
            GridView1.Columns[DESERTED_COLUMN].Visible = false; // hide the attacking troops deserted column
            if (!String.IsNullOrEmpty(Request.QueryString[CONSTS.QuerryString.RecordID]))
            {
                #region Paste report to sim

                recordID = Convert.ToInt32(Request.QueryString[CONSTS.QuerryString.RecordID]);

                report = Report.GetBattleReportDetails(FbgPlayer, recordID);
                int battleSimDefendingPlayersPoints=0; // points of the player attacking in this simulation
                int battleSimAttackingPlayersPoints=0; // points of the player defending in this simulation
                int battleSimDefendingPlayerID; // points of the player attacking in this simulation
                int battleSimAttackingPlayerID; // points of the player defending in this simulation
                Fbg.Bll.CONSTS.VillageType defendingVillageType;
  
                //
                // did we get a report ?
                //
                if (report.IsRetrievedReportValid)
                {
                    #region WE GOT a report of some kind. process it
                    linkBackToReport.Visible = true;
                    linkBackToReport.NavigateUrl = NavigationHelper.Reports_AttackDetails(recordID);
                    //
                    // who are we pasting - attacker or defender?
                    //  get the right troops 
                    //
                    if (Request.QueryString["who"]== "def") {
                        report_pasteWho = ReportPasteWho.defender;
                        report_unitsToPaste = report.DefenderUnits;
                    } else {
                        report_pasteWho = ReportPasteWho.attacker;
                        report_unitsToPaste = report.AttackerUnits;
                    }
                    //
                    // if we got troops, then lets move on 
                    //
                    if (report_unitsToPaste != null)
                    {
                        //
                        // who do we paste the troops as ?
                        //
                        report_pasteAs = Request.QueryString["as"] == "att" ? ReportPasteAs.attacker : ReportPasteAs.defender;

                        //
                        // calc the handicap. 
                        //   We make certain assumptions here. 
                        //      (1) rebels or abandoned can never be pasted as attacker since they never attack so we alwyas assume that
                        //          when pasting as attacker, this player is a regular player
                        //
                        if (!report.IsAttacker
                            && !report.DoesDefenderKnownsAttackersIdentity)
                        {
                            // if point of view is not the attacker, and attackers identity is not know, the we do not calc the handicap
                            txtHandicap.Text = "0";

                        } else {

                            PlayerOther po;
                            if (report_pasteAs == ReportPasteAs.defender)
                            {
                                if (report_pasteWho == ReportPasteWho.attacker)
                                {
                                    battleSimDefendingPlayerID = report.AttackerPlayerID;
                                }
                                else
                                {
                                    battleSimDefendingPlayerID = report.DefenderPlayerID;
                                }
                                po = PlayerOther.GetPlayer(FbgPlayer.Realm
                                    , battleSimDefendingPlayerID
                                    , FbgPlayer.ID);

                                if (po != null)
                                {
                                    battleSimDefendingPlayersPoints = po.Points;
                                }

                                battleSimAttackingPlayersPoints = FbgPlayer.Points;
                                defendingVillageType = Fbg.Bll.utils.GetVillageType(FbgPlayer.Realm, battleSimDefendingPlayerID);
                            }
                            else
                            {
                                // pasting from report as attacker
                                if (report_pasteWho == ReportPasteWho.attacker)
                                {
                                    battleSimAttackingPlayerID = report.AttackerPlayerID;
                                }
                                else
                                {
                                    battleSimAttackingPlayerID = report.DefenderPlayerID;
                                }
                                po = PlayerOther.GetPlayer(FbgPlayer.Realm
                                    , battleSimAttackingPlayerID
                                    , FbgPlayer.ID);

                                if (po != null)
                                {
                                    battleSimAttackingPlayersPoints = po.Points;
                                }

                                battleSimDefendingPlayersPoints = FbgPlayer.Points;
                                defendingVillageType = Fbg.Bll.CONSTS.VillageType.Normal;
                            }

                            //
                            // actually calculate the handicap
                            txtHandicap.Text = (FbgPlayer.Realm.CalcBattleHandicap(
                                battleSimAttackingPlayersPoints
                                , battleSimDefendingPlayersPoints
                                , defendingVillageType
                                ) * 100).ToString("0.#");
                        
                        }

                        //
                        // get wall and tower only if pasting the defender as defender
                        //
                        if (report_pasteWho == ReportPasteWho.defender && report_pasteAs == ReportPasteAs.defender)
                        { 
                            int towerLevel = report.GetBuildingLevelFromAgregatedIntel(Fbg.Bll.CONSTS.BuildingIDs.DefenseTower);
                            int wallLevel = report.GetBuildingLevelFromAgregatedIntel(Fbg.Bll.CONSTS.BuildingIDs.Wall);
                            txt_DefenderTowerLevel.Text = towerLevel == -1 ? "0" : towerLevel.ToString();
                            txt_DefenderWallLevel.Text = wallLevel == -1 ? "0" : wallLevel.ToString();
                        }
                    }
                    #endregion
                }
                else
                {
                    //
                    // report retrieval failed
                    //
                    report = null;
                    report_unitsToPaste = null;

                }
                #endregion
            }
        }

        if (!IsPostBack)
        {
            GridView1.DataSource = FbgPlayer.Realm.GetUnitTypes();
            GridView1.DataBind();

            if (FbgPlayer.Realm.Research.MaxBonus_DefenceFactor() > 0)
            {
                txtResearchDefBonus.Text = (Convert.ToInt32(FbgPlayer.Realm.Research.MaxBonus_DefenceFactor() * 100)).ToString();
                RangeValidator3.MaximumValue = (Convert.ToInt32(FbgPlayer.Realm.Research.MaxBonus_DefenceFactor() * 100)).ToString();
                RangeValidator3.ToolTip = string.Format("Valid numbers are between 0 and {0}", RangeValidator3.MaximumValue);
            }

            if (FbgPlayer.Realm.Research.MaxBonus_VillageDefenceFactor() > 0)
            {
                txtResearchDefBonus_VillageDef.Text = (Convert.ToInt32(FbgPlayer.Realm.Research.MaxBonus_VillageDefenceFactor() * 100)).ToString();
                RangeValidator4.MaximumValue = (Convert.ToInt32(FbgPlayer.Realm.Research.MaxBonus_VillageDefenceFactor() * 100)).ToString();
                RangeValidator4.ToolTip = string.Format("Valid numbers are between 0 and {0}", RangeValidator4.MaximumValue);
            }
            else
            {
                trResearchBonusVillageBonus.Visible = false;
            }
            if (FbgPlayer.Realm.Research.MaxBonus_AttackBonus() > 0)
            {
                txtAttackResearchBonus.Text = (Convert.ToInt32(FbgPlayer.Realm.Research.MaxBonus_AttackBonus() * 100)).ToString();
                RangeValidator5.MaximumValue = (Convert.ToInt32(FbgPlayer.Realm.Research.MaxBonus_AttackBonus() * 100)).ToString();
                RangeValidator5.ToolTip = string.Format("Valid numbers are between 0 and {0}", RangeValidator5.MaximumValue);
            }
            else
            {
                trAttackerResearchBonus.Visible = false;
            }
            if (FbgPlayer.Realm.Morale.IsActiveOnThisRealm)
            {
                panelMorale.Visible = true;
                txtMorale.Text = FbgPlayer.Realm.Morale.MaxMorale_Normal.ToString();
                RangeValidator_morale.MinimumValue = FbgPlayer.Realm.Morale.MinMorale.ToString();
                RangeValidator_morale.MaximumValue = FbgPlayer.Realm.Morale.MaxMorale.ToString();
                RangeValidator_morale.ToolTip = string.Format("Valid numbers are between {0} and {1}", RangeValidator_morale.MinimumValue,  RangeValidator_morale.MaximumValue);

            }

            // bonus villages which effects defensive bonus
            foreach (VillageTypes.VillageType vt in FbgPlayer.Realm.VillageTypes)
            {
                if (vt.Bonus_DefenceFactor() > 0)
                {
                    ddBonusVillage.Items.Add(new ListItem(
                        String.Format("{0}... ( + {1}% )", vt.Name.Substring(0, vt.Name.Length < 20 ? vt.Name.Length-1 : 19)  , (vt.Bonus_DefenceFactor() * 100))
                        , (vt.Bonus_DefenceFactor() * 100).ToString()));
                }
                else if (vt.Bonus_DefenceFactor() < 0)
                {
                    ddBonusVillage.Items.Add(new ListItem(
                        String.Format("{0}... ( {1}% )", vt.Name.Substring(0, vt.Name.Length < 20 ? vt.Name.Length - 1 : 19), (vt.Bonus_DefenceFactor() * 100))
                        , (vt.Bonus_DefenceFactor() * 100).ToString()));
                }
            }
            if (ddBonusVillage.Items.Count > 0)
            {
                ddBonusVillage.Items.Insert(0, new ListItem("", "0"));
            }
            else
            {
                trBonusVillage.Visible = false;
            }

        }

        ShowHideAdvanced();

        if (FbgPlayer.Realm.IsVPrealm)
        {
            MyPFs.Visible = !isMobile; // show it when not on mobile
            
            HandleActiveFeatures();
        }
        else
        {
            MyPFs.Visible = false;
            
        }

        RangeValidator1.MaximumValue = (FbgPlayer.Realm.BattleHandicap.Param_MaxHandicap * 100).ToString();
        RangeValidator1.ToolTip = String.Format(RS("Between0and50"), RangeValidator1.MaximumValue);
        
    }

    protected override void Render(HtmlTextWriter writer)
    {
        //according to this link this should fix the problem
        //http://blogs.msdn.com/amitsh/archive/2007/07/31/why-i-get-invalid-postback-or-callback-argument-errors.aspx

        foreach (GridViewRow  row in GridView1.Rows )

            ClientScript.RegisterForEventValidation(row.UniqueID.ToString() + ":_ctl0");

        base.Render(writer);

    }
    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DropDownList cmbBuildings = (DropDownList)e.Row.FindControl("cmb_Buildings");
            TextBox txtTargetBuildingLevel = (TextBox)e.Row.FindControl("txt_TargetBuildingLevel");
            RangeValidator rvTargetBuildingLevelMaxLevel = (RangeValidator)e.Row.FindControl("rv_TargetBuildingLevelMaxLevel");

            Panel pnlTargetLevel = (Panel)e.Row.FindControl("panelTargetLevel");
            Panel pnlTarget = (Panel)e.Row.FindControl("pnl_Target");
            TextBox txtDefendingTroops = (TextBox)e.Row.FindControl("txt_DefendUnitAmount");
            TextBox txtAttackingTroops = (TextBox)e.Row.FindControl("txt_AttackUnitAmount");

            Label lblUnitID = (Label)e.Row.FindControl("lbl_UnitID");
            int unitID = Convert.ToInt32(lblUnitID.Text);
            Fbg.Bll.UnitType ut = FbgPlayer.Realm.GetUnitTypesByID(unitID);
            List<BuildingType> BTs = ut.AttackableBuildings;
            DataRow[] defenderUnits;
            //
            // if we got defenders units from report, then lets populate the text box with them. 
            if (report_unitsToPaste != null)
            {
                if (report_pasteAs == ReportPasteAs.defender)
                {
                    txtDefendingTroops.Text = GetUnitcountFromReportIfAvail(ut).ToString();
                }
                else
                {
                    txtAttackingTroops.Text = GetUnitcountFromReportIfAvail(ut).ToString();
                }
            }

            if (BTs.Count > 0)
            {
                cmbBuildings.DataSource = BTs;
                cmbBuildings.DataBind();
                rvTargetBuildingLevelMaxLevel.MaximumValue = FbgPlayer.Realm.BuildingType(Convert.ToInt32(cmbBuildings.SelectedValue)).MaxLevel.ToString();
                rvTargetBuildingLevelMaxLevel.ToolTip = RS("MaxLevel") + FbgPlayer.Realm.BuildingType(Convert.ToInt32(cmbBuildings.SelectedValue)).MaxLevel.ToString();

                if (unitID == Fbg.Bll.CONSTS.UnitIDs.Ram)//ram
                {
                    pnlTarget.Visible = false;
                    cmbBuildings.Visible = false;
                    txtTargetBuildingLevel.Visible = false;
                    //lblTarget.Visible = false;
                    //lblTargetLevel.Visible = false;
                }
                if (unitID == Fbg.Bll.CONSTS.UnitIDs.Treb)//treb
                {

                    pnlTarget.Visible = true;
                    cmbBuildings.SelectedValue = Fbg.Bll.CONSTS.BuildingIDs.DefenseTower.ToString();
                    txtTargetBuildingLevel.Visible = false;
                    pnlTargetLevel.Visible = false;
                }

                // for now, for mobile, we dont allow changing of target
                if (isMobile) {
                    pnlTarget.Visible = false;
                }
            }
            else
            {
                pnlTarget.Visible = false;
                cmbBuildings.Visible = false;
                txtTargetBuildingLevel.Visible = false;
                //lblTarget.Visible = false;
                pnlTargetLevel.Visible = false;
               
            }
            ///bind image  and link 
            HyperLink lnkUnitName = (HyperLink)e.Row.FindControl("lnk_UnitName");
            HyperLink lnkUnitImage = (HyperLink)e.Row.FindControl("lnk_UnitImage");
            //Text
            lnkUnitName.NavigateUrl = NavigationHelper.UnitHelp(FbgPlayer.Realm.ID, ut.ID);
            lnkUnitName.Text = ut.Name;
            lnkUnitName.Visible = !isMobile;

            //Image
            lnkUnitImage.NavigateUrl = NavigationHelper.UnitHelp(FbgPlayer.Realm.ID, ut.ID);
            lnkUnitImage.ImageUrl = ut.IconUrl;
            ///
        }
        else if (e.Row.RowType == DataControlRowType.Header)
        {
            bool hasPF = FbgPlayer.PF_HasPF(Fbg.Bll.CONSTS.PFs.BattleSimImprovements);
            ((Panel)e.Row.FindControl("pnl_AvialablePF")).Visible = hasPF;
            ((Panel)e.Row.FindControl("pnl_UNAvialablePF")).Visible = !hasPF;
        }
    }

    protected void Simulate_Click(object sender, EventArgs e)
    {
        if (this.IsValid)
        {

            List<AttackingTroops> AT_Objs = new List<AttackingTroops>();
            List<DefendingTroops> DT_Objs = new List<DefendingTroops>();
            bool isAttackBouns=false;
            bool isDefendBouns=false;
            foreach (GridViewRow gvRow in GridView1.Rows)
            {
                if (gvRow.RowType == DataControlRowType.DataRow)
                {

                    //gather attacking troops info
                    DropDownList cmbBuildings = (DropDownList)gvRow.FindControl("cmb_Buildings");
                    int SelectedBuilding = 0;
                    if (cmbBuildings != null && cmbBuildings.SelectedValue != "")
                    {
                        SelectedBuilding = int.Parse(cmbBuildings.SelectedValue);
                    }
                    TextBox txtAttackUnitAmount = (TextBox)gvRow.FindControl("txt_AttackUnitAmount");
                   
                    //txtAttackUnitAmount.Text = Utils.ClearHTMLCode(txtAttackUnitAmount.Text);
                    Label lblUnitID = (Label)gvRow.FindControl("lbl_UnitID");
                    AttackingTroops AT = new AttackingTroops(parseToInt(lblUnitID.Text), parseToInt(txtAttackUnitAmount.Text), SelectedBuilding);
                    //gather defending troops info
                    TextBox txtDefendUnitAmount = (TextBox)gvRow.FindControl("txt_DefendUnitAmount");
                    
                    //txtDefendUnitAmount.Text = Utils.ClearHTMLCode(txtDefendUnitAmount.Text);
                    TextBox txtTargetBuildingLevel = (TextBox)gvRow.FindControl("txt_TargetBuildingLevel");

                    //txtTargetBuildingLevel.Text = Utils.ClearHTMLCode(txtTargetBuildingLevel.Text);
                    DefendingTroops DT = new DefendingTroops(parseToInt(lblUnitID.Text), parseToInt(txtDefendUnitAmount.Text), parseToInt(txtTargetBuildingLevel.Text), SelectedBuilding);

                    //add item to lists
                    AT_Objs.Add(AT);
                    DT_Objs.Add(DT);

                }
                
            }
           

            List<BuildingAttacks> BAs = BattleSimulation.InitDefenderBuilding(FbgPlayer);
            BAs = BattleSimulation.GetBuildingUnderAttack(BAs, AT_Objs, DT_Objs, parseToInt(txt_DefenderWallLevel.Text), parseToInt(txt_DefenderTowerLevel.Text), FbgPlayer);
            double spySucessChance = 0;
            bool spyExists = false;
            double spyIdentityKnownChance = 0;
            double spyAttackVisibleChance = 0;
            float handicap; //. this will be a value betwen 0 and 0.5
            double distanceToTarget;
            double desertionFactor;
            float defenderDefenceBonusFromVillageType = 0.0f;
            float defenderDefenceBonusFromResearch;
            float defenderVillageDefenceBonusFromResearch;
            float attackersOffensiveResearchBonus;
            //float defendersDefensivePenaltyOrBonus;
            int morale = FbgPlayer.Realm.Morale.MaxMorale_Normal;

            if (FbgPlayer.Realm.Morale.IsActiveOnThisRealm)
            {
                morale = Convert.ToInt32(txtMorale.Text);
                lblMorale.Text = string.Format("Attack Strength Multiplier: {0}", FbgPlayer.Realm.Morale.GetEffect(morale).AttackMultiplier.ToString("0.00"));
            }

            handicap = txtHandicap.Text.Trim() == string.Empty ? 0 : Convert.ToSingle(txtHandicap.Text) / 100;
            distanceToTarget = txtDesertionDistance.Text.Trim() == string.Empty ? 0.0 : Convert.ToDouble(txtDesertionDistance.Text);
            
            isAttackBouns = chk_AttackBouns.Checked;
            isDefendBouns = chk_DefendBouns.Checked;

            defenderDefenceBonusFromVillageType = 0;
            if (!String.IsNullOrWhiteSpace(ddBonusVillage.SelectedValue))
            {
                defenderDefenceBonusFromVillageType = Convert.ToInt32(ddBonusVillage.SelectedValue);
            }
            defenderDefenceBonusFromVillageType = defenderDefenceBonusFromVillageType == 0 ? defenderDefenceBonusFromVillageType : defenderDefenceBonusFromVillageType / 100;
            defenderDefenceBonusFromResearch = txtResearchDefBonus.Text.Trim() == string.Empty ? 0 : Convert.ToSingle(txtResearchDefBonus.Text.Trim()) / 100;
            defenderVillageDefenceBonusFromResearch = txtResearchDefBonus_VillageDef.Text.Trim() == string.Empty ? 0 : Convert.ToSingle(txtResearchDefBonus_VillageDef.Text.Trim()) ;
            attackersOffensiveResearchBonus = txtAttackResearchBonus.Text.Trim() == string.Empty ? 0 : Convert.ToSingle(txtAttackResearchBonus.Text.Trim()) / 100;
            //defendersDefensivePenalty = txtDefendersDefensivePenalty.Text.Trim() == string.Empty ? 0 : Convert.ToSingle(txtDefendersDefensivePenalty.Text.Trim()) / 100;

           // defendersDefensivePenaltyOrBonus = Convert.ToInt32( ddBonusVillage.SelectedValue);

            BattleSimulation.Simulate(ref AT_Objs
                , ref  DT_Objs
                , ref BAs
                , ref spySucessChance
                , ref spyIdentityKnownChance
                , ref spyAttackVisibleChance
                , ref spyExists
                , FbgPlayer
                , FbgPlayer.Realm.BattleHandicap.IsActive ? handicap : 0
                , distanceToTarget
                , isAttackBouns
                , isDefendBouns
                , defenderDefenceBonusFromVillageType
                , defenderDefenceBonusFromResearch
                , out desertionFactor
                , defenderVillageDefenceBonusFromResearch
                , attackersOffensiveResearchBonus
                , morale);

            lblDesertionFactor.Text = (desertionFactor *100).ToString("0.##") + "%";

            #region Display Spies

            if (spyExists)
            {
                pnl_SpyExists.Visible = true;
                lbl_SpySucessChance.Text = Convert.ToString(Math.Round(spySucessChance * 100, 2)) + "%";
                lbl_SpyIdentityKnown.Text = Math.Round(spyIdentityKnownChance * 100, 2).ToString() + "%";
                lbl_SpiesComming.Text = Math.Round(spyAttackVisibleChance * 100, 2).ToString() + "%";


            }
            else
            {
                pnl_SpyExists.Visible = false;
            }

            #endregion

            bool someDeserted = false;

            for (int i = 0; i < GridView1.Rows.Count; i++)
            {
                GridViewRow gvRow = GridView1.Rows[i];

                if (gvRow.RowType == DataControlRowType.DataRow)
                {
                    Label lblUnitID = (Label)gvRow.FindControl("lbl_UnitID");
                    if (AT_Objs[i].UnitType == int.Parse(lblUnitID.Text) && DT_Objs[i].UnitType == int.Parse(lblUnitID.Text))
                    {
                        Label lblAttackingLost = (Label)gvRow.FindControl("lbl_AttackingLost");
                        Label lblAttackingDeserted = (Label)gvRow.FindControl("lbl_AttackingDeserted");
                        Label lblAttackingRemaining = (Label)gvRow.FindControl("lbl_AttackingRemaining");
                        Label lblDefendingLost = (Label)gvRow.FindControl("lbl_DefendingLost");
                        Label lblDefendingRemaining = (Label)gvRow.FindControl("lbl_DefendingRemaining");
                        TextBox txtTargetBuildingLevel = (TextBox)gvRow.FindControl("txt_TargetBuildingLevel");

                        if (AT_Objs[i].UnitDeserted > 0)
                        {
                            someDeserted = true;
                        }

                        lblAttackingDeserted.Text = Utils.FormatCost(AT_Objs[i].UnitDeserted);
                        lblAttackingDeserted.CssClass = lblAttackingDeserted.Text == "0" ? "ZeroUnitCount" : "";

                        lblAttackingLost.Text = Utils.FormatCost(AT_Objs[i].UnitKilled);
                        lblAttackingLost.CssClass = lblAttackingLost.Text == "0" ? "ZeroUnitCount" : "";


                        lblAttackingRemaining.Text = Utils.FormatCost((AT_Objs[i].UnitAmount - AT_Objs[i].UnitKilled));
                        lblAttackingRemaining.CssClass = lblAttackingRemaining.Text == "0" ? "ZeroUnitCount" : "";

                        lblDefendingLost.Text = Utils.FormatCost(DT_Objs[i].UnitKilled);
                        lblDefendingLost.CssClass = lblDefendingLost.Text == "0" ? "ZeroUnitCount" : "";

                        lblDefendingRemaining.Text = Utils.FormatCost((DT_Objs[i].UnitAmount - DT_Objs[i].UnitKilled));
                        lblDefendingRemaining.CssClass = lblDefendingRemaining.Text == "0" ? "ZeroUnitCount" : "";
                        //   txtTargetBuildingLevel.Text = Utils.FormatCost(DT_Objs[i].BuildingLevel);
                    }


                }
            }
            //
            // if no troops deserted, then hide the Attacking Troops Deserted column
            GridView1.Columns[DESERTED_COLUMN].Visible= someDeserted; 


            bool otherbuildingExists = false;
            foreach (BuildingAttacks ba in BAs)
            {
                if (ba.BuildingType == Fbg.Bll.CONSTS.BuildingIDs.Wall)//wall
                {
                    if (ba.CurrentLevel > 0)
                    {
                        lbl_WallAfterAttack.Visible = true;
                        lbl_WallAfterAttack.Text = RS("LevelAfterAttack") + ba.LevelAfterAttack.ToString();
                    }
                    else
                    {
                        lbl_WallAfterAttack.Visible = false;
                    }


                }
                else if (ba.BuildingType == Fbg.Bll.CONSTS.BuildingIDs.DefenseTower)//Towers
                {
                    if (ba.CurrentLevel > 0)
                    {
                        lbl_TowerAfterAttack.Visible = true;
                        lbl_TowerAfterAttack.Text = RS("LevelAfterAttack") + ba.LevelAfterAttack.ToString();
                    }
                    else
                    {
                        lbl_TowerAfterAttack.Visible = false;
                    }

                }
                else
                {
                    if (ba.CurrentLevel > 0)
                    {
                        //   txt_DefenderTowerLevel.Text = ba.LevelAfterAttack.ToString();
                        lbl_BuildingName.Visible = true;
                        lbl_BuildingAttackedresult.Visible = true;
                        lbl_BuildingAttackedresult.Text = "(" + FbgPlayer.Realm.BuildingType(ba.BuildingType).Name + ")  " + RS("LevelAfterAttack") + ba.LevelAfterAttack;
                        otherbuildingExists = true;
                    }
                    else
                    {
                        if (!otherbuildingExists)
                        {
                            lbl_BuildingName.Visible = false;
                            lbl_BuildingAttackedresult.Visible = false;
                        }
                    }
                }
            }


            CopyDefRem.Visible = true;


        } // this.IsValid
    }
    protected void cmb_Buildings_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (((DropDownList)sender).SelectedValue == Fbg.Bll.CONSTS.BuildingIDs.DefenseTower.ToString() || ((DropDownList)sender).SelectedValue == Fbg.Bll.CONSTS.BuildingIDs.Wall.ToString())//defevsive tower
        {
            for (int i = 0; i < GridView1.Rows.Count; i++)
            {
                GridViewRow gvRow = GridView1.Rows[i];
                DropDownList cmbBuildings = (DropDownList)gvRow.FindControl("cmb_Buildings");
                if (gvRow.RowType == DataControlRowType.DataRow)
                {
                    Panel pnlTargetLevel = (Panel)gvRow.FindControl("panelTargetLevel");
                    TextBox txtTargetBuildingLevel = (TextBox)gvRow.FindControl("txt_TargetBuildingLevel");
                    txtTargetBuildingLevel.Visible = false;
                    pnlTargetLevel.Visible = false;
                   
                }

            }
        }
        else
        {
            for (int i = 0; i < GridView1.Rows.Count; i++)
            {
                GridViewRow gvRow = GridView1.Rows[i];
                DropDownList cmbBuildings = (DropDownList)gvRow.FindControl("cmb_Buildings");
                if (gvRow.RowType == DataControlRowType.DataRow && cmbBuildings.Visible == true)
                {
                    Panel pnlTargetLevel = (Panel)gvRow.FindControl("panelTargetLevel");
                    TextBox txtTargetBuildingLevel = (TextBox)gvRow.FindControl("txt_TargetBuildingLevel");
                    RangeValidator rvTargetBuildingLevelMaxLevel = (RangeValidator)gvRow.FindControl("rv_TargetBuildingLevelMaxLevel");
                    rvTargetBuildingLevelMaxLevel.MaximumValue = FbgPlayer.Realm.BuildingType(Convert.ToInt32(((DropDownList)sender).SelectedValue)).MaxLevel.ToString();
                    rvTargetBuildingLevelMaxLevel.ToolTip = RS("MaxLevel") + FbgPlayer.Realm.BuildingType(Convert.ToInt32(((DropDownList)sender).SelectedValue)).MaxLevel.ToString();
                    txtTargetBuildingLevel.Visible = true ;
                    //lblTargetLevel.Text = "Target Level:";
                    pnlTargetLevel.Visible = true;
                }

            }
        }
        
        //DropDownList cmbBuildings = ((GridView)sender).Rows//gvRow.FindControl("cmb_Buildings");
       // TextBox txtTargetBuildingLevel = (sender)e.Row.FindControl("txt_TargetBuildingLevel");
        //if (cmbBuildings.SelectedValue == 7)//defevsive tower
        //{
        //    txtTargetBuildingLevel.Visible = false;
        //}
    }

    protected void btnPopMyTroopsAttack_Click(object sender, EventArgs e)
    {
        
       
    }
    private void ClearGridBoxs()
    {
        //foreach (GridViewRow r in GridView1.Rows)
        //{

        //    if (((TextBox)r.FindControl("txt_AttackUnitAmount")).Text != "0")
        //    {
        //        ((TextBox)r.FindControl("txt_AttackUnitAmount")).Text = "0";
        //    }
        //    if (((TextBox)r.FindControl("txt_AttackUnitAmount")).Text != "0")
        //    {
        //        ((TextBox)r.FindControl("txt_AttackUnitAmount")).Text = "0";
        //    }
        //}
    }
    /// <summary>
    /// this function handle the Attack & Defend Bouns 20% for VRealm 
    /// </summary>
    void HandleActiveFeatures()
    {
        DataTable dt = FbgPlayer.PF_PlayerPFPackages;
        DataRow[] drAttackBouns = dt.Select(Fbg.Bll.Realm.CONSTS.PFPackgesPrimaryColName.PackageID + "=24" );
        DataRow[] drDefendBouns = dt.Select(Fbg.Bll.Realm.CONSTS.PFPackgesPrimaryColName.PackageID + "=23");

        if (drAttackBouns.Length > 0
            && Convert.ToDateTime(drAttackBouns[0][Fbg.Bll.Player.CONSTS.PlayerPFPackageColIndex.ExpiresOn]) > DateTime.Now)
        {
            TimeSpan ts = (Convert.ToDateTime(drAttackBouns[0][Fbg.Bll.Player.CONSTS.PlayerPFPackageColIndex.ExpiresOn]) - DateTime.Now);
            if (ts.TotalDays <= 0)
            {
                lnk_AttackBouns.InnerText = "Activate to fix";
            }
            else
            {
                lnk_AttackBouns.InnerText = "Click for No-Risk extend";
            }
            lblPDAttack.Text = "is Active, expires in " + ts.TotalDays.ToString("0.00") + " days. ";
        }
        else
        {
            lblPDAttack.Text = "is NOT Active. ";
        }
        if (drDefendBouns.Length > 0
            && Convert.ToDateTime(drDefendBouns[0][Fbg.Bll.Player.CONSTS.PlayerPFPackageColIndex.ExpiresOn]) > DateTime.Now)
        {
            TimeSpan ts = (Convert.ToDateTime(drDefendBouns[0][Fbg.Bll.Player.CONSTS.PlayerPFPackageColIndex.ExpiresOn]) - DateTime.Now);
            if (ts.TotalDays <= 0)
            {
                lnk_DefenseBouns.InnerText = "Activate to fix";
            }
            else
            {
                lnk_DefenseBouns.InnerText = "Click for No-Risk extend";
            }
             lblPFDefence.Text = "is Active, expires in " + ts.TotalDays.ToString("0.00") + " days. ";
        }
        else
        {
            lblPFDefence.Text = "is NOT Active. ";
        }
    }
    void PopulateAttackUnits(bool isAll)
    {
        ClearGridBoxs();
        foreach (GridViewRow r in GridView1.Rows)
        {
            Label lblUnitID = (Label)r.FindControl("lbl_UnitID");
            TextBox txtAttackUnitAmount = (TextBox)r.FindControl("txt_AttackUnitAmount");
            int unitID = Convert.ToInt32(lblUnitID.Text);
            Fbg.Bll.UnitType ut = FbgPlayer.Realm.GetUnitTypesByID(unitID);
            if (txtAttackUnitAmount.Text == "0" || txtAttackUnitAmount.Text.Trim() == "")
            {
                if (isAll)
                {
                    txtAttackUnitAmount.Text = village.GetVillageUnit(ut).YourUnitsTotalCount.ToString();

                }
                else
                {
                    txtAttackUnitAmount.Text = village.GetVillageUnit(ut).YourUnitsCurrentlyInVillageCount.ToString();
                }
            }
        }
    }
    void PopulateDefendUnits(bool isAll)
    {
       
        ClearGridBoxs();
        foreach (GridViewRow r in GridView1.Rows)
        {
            Label lblUnitID = (Label)r.FindControl("lbl_UnitID");
            TextBox txt_DefendUnitAmount = (TextBox)r.FindControl("txt_DefendUnitAmount");
            int unitID = Convert.ToInt32(lblUnitID.Text);
            Fbg.Bll.UnitType ut = FbgPlayer.Realm.GetUnitTypesByID(unitID);
            if (txt_DefendUnitAmount.Text == "0" || txt_DefendUnitAmount.Text.Trim() == "")
            {
                if (isAll)
                {
                    txt_DefendUnitAmount.Text = village.GetVillageUnit(ut).YourUnitsTotalCount.ToString();

                }
                else
                {
                    txt_DefendUnitAmount.Text = village.GetVillageUnit(ut).YourUnitsCurrentlyInVillageCount.ToString(); ;
                }
            }
        }
        // wall id =4
        txt_DefenderWallLevel.Text = village.GetBuildingLevel(4).ToString();
        // tower id=7
        txt_DefenderTowerLevel.Text = village.GetBuildingLevel(7).ToString();
    }
    protected void lnk_PopAttackAll_Click(object sender, EventArgs e)
    {
        PopulateAttackUnits(true);

    }
    protected void lnk_PopAttackCurrent_Click(object sender, EventArgs e)
    {
        PopulateAttackUnits(false);
    }
    protected void lnk_PopDefendAll_Click(object sender, EventArgs e)
    {
        PopulateDefendUnits(true);
    }
    protected void lnk_PopDefendCurrent_Click(object sender, EventArgs e)
    {
        PopulateDefendUnits(false);
    }

    protected int GetUnitcountFromReportIfAvail(Fbg.Bll. UnitType ut)
    {
        if (report_unitsToPaste != null)
        {
            foreach (DataRow dr in report_unitsToPaste)
            {
                if ((int)dr[Fbg.Bll.Report.CONSTS.BattleReport.UnitsTblColIndex.UnitTypeID] == ut.ID) 
                {
                    return (int)dr[Fbg.Bll.Report.CONSTS.BattleReport.UnitsTblColIndex.ReaminingUnitCount];
                }
            }
        }
        return 0;
    }

    private void ShowHideAdvanced()
    {
        if (FbgPlayer.Realm.IsVPrealm && ShowAdvancedParameters)
        {
            trPFAtt.Visible = true;
            trPFDef.Visible = true;
            HandleActiveFeatures();
        }
        else
        {
            trPFAtt.Visible = false;
            trPFDef.Visible = false;
        }



        if (ddBonusVillage.Items.Count > 0)
        {
            trBonusVillage.Visible = ShowAdvancedParameters;
        }
        else
        {
            trBonusVillage.Visible = false;
        }

        if (FbgPlayer.Realm.Research.IsResearchActive && ShowAdvancedParameters)
        {
            trResearchBonus.Visible = true;
            trResearchBonusVillageBonus.Visible = true;
            trAttackerResearchBonus.Visible = true;
        }
        else
        {
            trResearchBonus.Visible = false;
            trResearchBonusVillageBonus.Visible = false;
            trAttackerResearchBonus.Visible = false;
        }


        panelMorale.Visible = FbgPlayer.Realm.Morale.IsActiveOnThisRealm && ShowAdvancedParameters;
        literalHandicap.Visible = FbgPlayer.Realm.BattleHandicap.IsActive && ShowAdvancedParameters;
        panelDesertion.Visible = FbgPlayer.Realm.UnitDesertionScalingFactor != 0 && ShowAdvancedParameters;
        trDefensivePenalty.Visible = ShowAdvancedParameters;

    }


    private bool ShowAdvancedParameters
    {
        get
        {
            if (ViewState["showAdvanced"] == null)
            {
                return false;
            }
            return (bool)ViewState["showAdvanced"];
        }
    }

    protected void LinkButton1_Click(object sender, EventArgs e)
    {

        trShowAdvancedButton.Visible = false;
        ViewState.Add("showAdvanced", true);
        ShowHideAdvanced();
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

    private int parseToInt(string stringToParse) 
    {
        int retVal = 0 ;
        int.TryParse(stringToParse, out retVal);
        return retVal;
    }


}
