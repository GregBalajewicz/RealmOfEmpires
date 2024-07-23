using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Fbg.Bll;

public partial class PlayerStatHistoryAjax : JsonPage
{
    public override object Result()
    {
        int playerId = Convert.ToInt32(Request["pid"]);
        DateTime? d = Request["d"] == "last" ? DateTime.Now.AddDays(-30) : new DateTime?();
        
        var epoch = new DateTime(1970, 1, 1);

        Func<DataRow, double[]> func = r => new double[] { (r.Field<DateTime>("Date") - epoch).TotalMilliseconds, r.Field<int>("StatValue") };
        DataTable t = FbgPlayer.GetStatHistory(playerId, d);

        return new {
            villages = t.Select("StatID = " + (int)Fbg.Bll.Player.StatIDTypes.NumberOfVillages).Select(func).ToArray(),
            points = t.Select("StatID = " + (int)Fbg.Bll.Player.StatIDTypes.TotalVillagePoints).Select(func).ToArray(),
            attack = t.Select("StatID = " + (int)Fbg.Bll.Player.StatIDTypes.AttackPoints).Select(func).ToArray(),
            defence = t.Select("StatID = " + (int)Fbg.Bll.Player.StatIDTypes.DefencePoints).Select(func).ToArray()
        };
    }
}
