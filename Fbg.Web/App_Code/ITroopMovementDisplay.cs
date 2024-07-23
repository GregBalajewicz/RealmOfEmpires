using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections.Generic;

/// <summary>
/// Summary description for ITroopMovementDisplay
/// </summary>
public interface ITroopMovementDisplay
{
    List<Fbg.Common.DataStructs.Player> GetToPlayers();
    List<Fbg.Common.DataStructs.Player> GetFromPlayers();

    /// <summary>
    /// </summary>
    /// <param name="playerID">send -1 if you just want all the villages</param>
    /// <returns></returns>
    List<Fbg.Common.DataStructs.Village> GetToVillages(int playerID);
    /// <summary>
    /// </summary>
    /// <param name="playerID">send -1 if you just want all the villages</param>
    /// <returns></returns>
    List<Fbg.Common.DataStructs.Village> GetFromVillages(int playerID);

}
