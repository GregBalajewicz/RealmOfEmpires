using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class MapTroopsMove : JsonPage
{
    public override object Result()
    {
        int x = Convert.ToInt32(Request["x"]);
        int y = Convert.ToInt32(Request["y"]);
        int size = Convert.ToInt32(Request["size"]);
        Fbg.Bll.Map.MovementType type = (Fbg.Bll.Map.MovementType)
            Enum.Parse(typeof(Fbg.Bll.Map.MovementType), Request["type"]);

        var dir = Fbg.Bll.Map.GetMapTroopsMove(FbgPlayer.Realm, type, x, y, size, FbgPlayer.ID)
                    .ToDictionary(m => m.VillageID.ToString(), m => new { id = m.VillageID, Sup = m.SupportCount, Att = m.AttackCount });

        return dir;
    }
}
