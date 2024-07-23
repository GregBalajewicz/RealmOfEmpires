using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

using Fbg.Bll;


public partial class UtilsAjax : JsonPage
{
    public override object Result()
    {
        string function = Request["func"].ToUpper();

        switch (function)
        {
            case "REMV":
                return RenameVillage();
            default:
                throw new Exception(String.Format("Function '{0}' not defined", function));
        }
    }

    public object RenameVillage()
    {
        string villageName = Request["vn"];
        int villageID = Convert.ToInt32( Request["vid"]);

        villageName = villageName.Trim();
        villageName = Utils.ClearHTMLCode(villageName);
        villageName = Utils.ClearInvalidChars(villageName);
        villageName = Utils.StripNonAscii(villageName);
        villageName = villageName.Length <= 25 ? villageName : villageName.Substring(0, 25);

        Regex strPattern = new Regex("^[a-zA-Z0-9%)(*[\\]._\\,\\-\\^\\#\\@\\|\\+\\~\\!\\{\\} ]{1,25}$"); //"^[0-9A-Za-z_]*$"


        if (!String.IsNullOrEmpty(villageName) && strPattern.IsMatch(villageName))
        {
            Village v = FbgPlayer.Village(villageID);
            v.SetName(villageName);
            FbgPlayer.Quests2.SetQuestAsCompleted(Fbg.Bll.Player.QuestTags.ChangeVillageName.ToString());
        }

        return new { name = villageName, id = villageID };
    }
}
