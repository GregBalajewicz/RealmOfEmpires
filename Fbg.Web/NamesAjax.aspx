<%@ Page Inherits="MyCanvasIFrameBasePage" Language="C#" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="System.Data" %>

<script runat="server">
System.Web.Script.Serialization.JavaScriptSerializer json_serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

protected string s() 
{
    string startsWith = Request.QueryString["term"];
    string what = Request.QueryString["what"];
       
    
    switch (what) 
    {
        case "playerNamesWClan":
            return PlayerNameWithClan(startsWith);
        case "clans":
            return Clans(startsWith);
        default:
            return PlayerName(startsWith);
    }
 }



protected string PlayerName(string startsWith) 
{
    


    System.Data.DataTable dt = Fbg.Bll.Stats.GetPlayerRanking(FbgPlayer.Realm, 0, string.Empty);


    var
        players = from r in dt.AsEnumerable()
                  where r.Field<string>(Fbg.Bll.Stats.CONSTS.PlayerRanking.PlayerName).StartsWith(startsWith, StringComparison.CurrentCultureIgnoreCase)
                  orderby r.Field<string>(Fbg.Bll.Stats.CONSTS.PlayerRanking.PlayerName)
                  select new { value = r.Field<string>(Fbg.Bll.Stats.CONSTS.PlayerRanking.PlayerName).ToString() };



    return json_serializer.Serialize(players);
 }




protected string PlayerNameWithClan(string startsWith)
{

 
    System.Data.DataTable dt = Fbg.Bll.Stats.GetPlayerRanking(FbgPlayer.Realm, 0, string.Empty);


     var   players = from r in dt.AsEnumerable()
                  where r.Field<string>(Fbg.Bll.Stats.CONSTS.PlayerRanking.PlayerName).StartsWith(startsWith, StringComparison.CurrentCultureIgnoreCase)
                  orderby r.Field<string>(Fbg.Bll.Stats.CONSTS.PlayerRanking.PlayerName)
                  select new
                  {
                      label = r.Field<string>(Fbg.Bll.Stats.CONSTS.PlayerRanking.PlayerName).ToString()
                      +
                         (r.Field<object>(Fbg.Bll.Stats.CONSTS.PlayerRanking.ClanName) == null ? "" : (String.Format(" ({0})", r.Field<string>(Fbg.Bll.Stats.CONSTS.PlayerRanking.ClanName).ToString())))
                      ,
                      value = r.Field<string>(Fbg.Bll.Stats.CONSTS.PlayerRanking.PlayerName).ToString()
                  };
   



    return json_serializer.Serialize(players);
}



protected string Clans(string startsWith)
{

    System.Data.DataTable dt = Fbg.Bll.Stats.GetClanRanking(FbgPlayer.Realm, 0);

    var clans2 = from r in dt.AsEnumerable()
                 where r.Field<string>(Fbg.Bll.Stats.CONSTS.ClanRanking.ClanName).StartsWith(startsWith, StringComparison.CurrentCultureIgnoreCase)
                 select new { value = r.Field<string>(Fbg.Bll.Stats.CONSTS.ClanRanking.ClanName).ToString() };


    return json_serializer.Serialize(clans2);
}

 

</script>
<%=s() %>
