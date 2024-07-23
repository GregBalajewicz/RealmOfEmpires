using System;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using System.Data;
using System.Web.UI.WebControls;
using Fbg.Bll;
using System.Text;
using System.Collections.Generic;
using System.Web.Script.Serialization;

/// <summary>
/// Summary description for Troops
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService]
public class Troops : System.Web.Services.WebService
{
    private Fbg.Bll.Player _fbgPlayer;

    public Fbg.Bll.Player FbgPlayer
    {
        get
        {
            if (_fbgPlayer == null)
            {
                _fbgPlayer = (Fbg.Bll.Player)Session[CONSTS.Session.fbgPlayer];
            }
            return _fbgPlayer;
        }
    }


    public Troops()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod(EnableSession = true)]
    public long CancelTroopMovement(long eventID)
    {
        if (FbgPlayer == null)
        {
            return -1;
        }
        if (Fbg.Bll.UnitMovements.Cancel(FbgPlayer, eventID))
        {
            return eventID;
        }

        return 0;
    }

    [WebMethod(EnableSession = true)]
    public string GetTroopMovementDetails(long eventID)
    {
        if (FbgPlayer == null)
        {
            return "-1";
        }

        DataTable tbl = Fbg.Bll.UnitMovements.GetUnitMovementDetails(FbgPlayer, eventID);
        StringBuilder s = new StringBuilder();
        foreach (DataRow dr in tbl.Rows)
        {
            s.AppendFormat(", ut{0} : {1}"
                , (int)dr[UnitMovements.CONSTS.UnitMovementDetColIndex.UnitTypeId]
                , (int)dr[UnitMovements.CONSTS.UnitMovementDetColIndex.UnitCount]);

        }

        return String.Format("det = {{eventID:'{0}' {1}}}", eventID, s.ToString());
    }

    [WebMethod(EnableSession = true)]
    public string GetTroopsInVillage(int villageID)
    {
        if (FbgPlayer == null)
        {
            return "-1";
        }
        // this is a PF, return nothing if player does not have it
        if (!FbgPlayer.PF_HasPF(Fbg.Bll.CONSTS.PFs.CommandTroopsEnhancements))
        {
            return String.Empty;
        }

        Village v = Fbg.Bll.Village.GetVillage(FbgPlayer, villageID, false, false);
        StringBuilder s = new StringBuilder();
        StringBuilder sTroopsTable = new StringBuilder();
        StringBuilder sUnitsHeader = new StringBuilder();
        StringBuilder sUnitCounts_yours = new StringBuilder();
        StringBuilder sUnitCounts_all = new StringBuilder();

        foreach (Fbg.Bll.UnitType ut in FbgPlayer.Realm.GetUnitTypes())
        {
            sUnitsHeader.AppendFormat("<TD><img src=\"{0}\"/></TD>", ut.IconUrl);
        }
        foreach (Fbg.Bll.UnitType ut in FbgPlayer.Realm.GetUnitTypes())
        {
            UnitInVillage u = v.GetVillageUnit(ut);
            sUnitCounts_yours.AppendFormat("<TD>{0}</td>", Utils.FormatCost(u.YourUnitsCurrentlyInVillageCount));
            sUnitCounts_all.AppendFormat("<TD>{0}</td>", Utils.FormatCost(u.TotalNowInVillageCount));
        }
        sTroopsTable.AppendFormat("<TR class=\"unitHdr\"><TD rowspan=2>Yours</td>{0}</tr>", sUnitsHeader.ToString());
        sTroopsTable.AppendFormat("<TR class=\"unitCnt\">{0}</tr>", sUnitCounts_yours.ToString());
        sTroopsTable.AppendFormat("<TR class=\"unitHdr\"><TD rowspan=2>all</td>{0}</tr>", sUnitsHeader.ToString());
        sTroopsTable.AppendFormat("<TR class=\"unitCnt\">{0}</tr>", sUnitCounts_all.ToString());
        s.AppendFormat(", htmltable: '<div class=\"unitsMapDisplay\"><div class=\"title\">Troops now in village (beta)</div><table cellspacing=1 cellpadding=1>{0}</table>'", sTroopsTable.ToString());



        return String.Format("det = {{VID:'{0}' {1}}}", villageID, s.ToString());

        //JavaScriptSerializer s = new JavaScriptSerializer();
        //s.
       
    }

    [WebMethod(EnableSession = true)]
    public string GetTroopsInVillageByType(int villageID, string type)
    {
        if (FbgPlayer == null) {
            return "-1";
        }
        // this is a PF, return nothing if player does not have it
        if (!FbgPlayer.PF_HasPF(Fbg.Bll.CONSTS.PFs.CommandTroopsEnhancements)) {
            return String.Empty;
        }

        Village v = Fbg.Bll.Village.GetVillage(FbgPlayer, villageID, false, false);
        StringBuilder s = new StringBuilder();
        StringBuilder sTroopsTable = new StringBuilder();
        StringBuilder sUnitsHeader = new StringBuilder();

        string[] types = type.Split(new [] { ',' });

        foreach(string t in types) 
        {
            foreach (Fbg.Bll.UnitType ut in FbgPlayer.Realm.GetUnitTypes())
            {
                sUnitsHeader.AppendFormat("<TD><img src=\"{0}\"/></TD>", ut.IconUrl);
                UnitInVillage u = v.GetVillageUnit(ut);
                if (t == "your-all") sUnitsHeader.AppendFormat("<TD>{0}</td>", Utils.FormatCost(u.YourUnitsTotalCount));
                if (t == "your") sUnitsHeader.AppendFormat("<TD>{0}</td>", Utils.FormatCost(u.YourUnitsCurrentlyInVillageCount));
                if (t == "all") sUnitsHeader.AppendFormat("<TD>{0}</td>", Utils.FormatCost(u.TotalNowInVillageCount));
                if (t == "support") sUnitsHeader.AppendFormat("<TD>{0}</td>", Utils.FormatCost(u.TotalNowInVillageCount - u.YourUnitsCurrentlyInVillageCount));
            }
            if (t == "all" || type == null)
            {
                sTroopsTable.AppendFormat("<TR class=\"unitHdr\"><td class=troopTypeIcon>A:</td>{0}</tr>", sUnitsHeader.ToString());
            }
            if (t == "your-all" || type == null)
            {
                sTroopsTable.AppendFormat("<TR class=\"unitHdr\"><td class=troopTypeIcon>Y:</td>{0}</tr>", sUnitsHeader.ToString());
            }
            if (t == "your" || type == null)
            {
                sTroopsTable.AppendFormat("<TR class=\"unitHdr\"><td class=troopTypeIcon>C:</td>{0}</tr>", sUnitsHeader.ToString());
            }
            if (t == "support" || type == null)
            {
                sTroopsTable.AppendFormat("<TR class=\"unitHdr\"><td class=troopTypeIcon>S:</td>{0}</tr>", sUnitsHeader.ToString());
            }

            sUnitsHeader = new StringBuilder();
        }

        s.AppendFormat(", htmltable: '<div class=\"unitsMapDisplay\"><table cellspacing=0 cellpadding=0>{0}</table>'", sTroopsTable.ToString());

        return String.Format("det = {{VID:'{0}' {1}}}", villageID, s.ToString());
    }

    [WebMethod(EnableSession = true)]
    public string ToggleHideTroopMovement(long eventID)
    {
        if (FbgPlayer == null)
        {
            // session expired
            eventID = -1;
        }

        int curHiddenState = Fbg.Bll.UnitMovements.ToggleHide(FbgPlayer, eventID);

        //curHiddenState will be 0 (not hidden) or 1 (hidden)
        return String.Format("ret = {{eventID:'{0}', curHiddenState :'{1}'}}", eventID, curHiddenState);

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="unitTypeID"></param>
    /// <param name="unitCount"></param>
    /// <param name="originVillageID"></param>
    /// <param name="targetVillageXCord"></param>
    /// <param name="targetVillageYCord"></param>
    /// <param name="commandType"> Support=0, Attack=1</param>
    /// <returns></returns>
    [WebMethod(EnableSession = true)]
    public string ExecuteOneUnitCommand(int unitTypeID, int unitCount, int originVillageID
        , int targetVillageXCord, int targetVillageYCord, int commandType, bool isLoggedinAsSteward)
    {
        //NOTE!!!
        // HACK - security issue!! we accept the senders "isLoggedinAsSteward" without checking it our selves meaning that a steward
        //  can still execute this call manually and launch the attacks but this is unlikely so we will not worry about it 

        if (FbgPlayer == null)
        {
            return "Session Expired. Refresh page and try again.";
        }
        // this is a PF, return nothing if player does not have it
        if (!FbgPlayer.PF_HasPF(Fbg.Bll.CONSTS.PFs.CommandTroopsEnhancements))
        {
            return String.Empty;
        }

       

        Village v = Village.GetVillage(FbgPlayer, originVillageID, false, false);
        Fbg.Bll.UnitType ut = FbgPlayer.Realm.GetUnitTypesByID(unitTypeID);
        VillageOther targetVill = VillageOther.GetVillage(FbgPlayer.Realm, targetVillageXCord, targetVillageYCord);
        Fbg.Common.UnitCommand cmd = new Fbg.Common.UnitCommand();
        cmd.command = (Fbg.Common.UnitCommand.CommandType)commandType;
        cmd.unitsSent = new List<Fbg.Common.UnitCommand.Units>(1);
        cmd.unitsSent.Add(new Fbg.Common.UnitCommand.Units(ut.ID, unitCount, -1));
        cmd.originVillageID = v.id;
        cmd.targetVillageID = targetVill.ID;
        if (v.CommandUnits(targetVill, ut, cmd, FbgPlayer.PF_HasPF(Fbg.Bll.CONSTS.PFs.CommandTroopsEnhancements), isLoggedinAsSteward, FbgPlayer.PF_HasPF(Fbg.Bll.CONSTS.PFs.attackSpeedUp)) == Village.CanCommandUnitsResult.Yes)
        {
            return String.Format("{0}: <img src='{1}'/>{2} from {3}({4},{5})", cmd.command.ToString() + "ing", ut.IconUrl, unitCount, v.name, v.xcord, v.ycord);
        }
        else
        {
            return "Problem executing command. Try executing it manually instead.";
        }
    }

    [WebMethod(EnableSession = true)]
    public string SendBackSupport(int SiVID, int SdVID)
    {
        if (FbgPlayer == null)
        {
            return "";
        }
        Fbg.Bll.UnitsSupporting.SendBack(FbgPlayer, SiVID, SdVID);
        return String.Format("jsRowID_SiVID{0}_SdVID{1}", SiVID, SdVID);
    }


    [WebMethod(EnableSession = true)]
    public string RecallSupport(int SiVID, int SdVID)
    {
        if (FbgPlayer == null)
        {
            return "";
        }
        Fbg.Bll.UnitsAbroad.RecallUnits(FbgPlayer, SiVID, SdVID, null);
        return String.Format("jsRowID_SiVID{0}_SdVID{1}", SiVID, SdVID);
    }


    class TroopCounts
    {
        public int uid;
        public int count;

    }
    [WebMethod(EnableSession = true)]
    public string RecallSomeSupport(int SiVID, int SdVID, string troopsInJSON)
    {
        if (FbgPlayer == null)
        {
            return "";
        }

        System.Web.Script.Serialization.JavaScriptSerializer json_serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

        List<TroopCounts> troops = json_serializer.Deserialize<List<TroopCounts>>(troopsInJSON);

        Fbg.Common.UnitCommand uc = new Fbg.Common.UnitCommand();
        uc.unitsSent = new List<Fbg.Common.UnitCommand.Units>(troops.Count);

        foreach (TroopCounts t in troops)
        {
            uc.unitsSent.Add(new Fbg.Common.UnitCommand.Units(t.uid, t.count, -1));
        }

        
        Fbg.Bll.UnitsAbroad.RecallUnits(FbgPlayer, SiVID, SdVID, uc);

        UnitsAbroad ua = FbgPlayer.GetUnitsAbroad(SiVID, null);
        return json_serializer.Serialize(from s in ua.GetUnitsSupporting(SiVID, SdVID) select new { count = Utils.FormatCost(s.count), uid = s.unitID });
               
    }
}

