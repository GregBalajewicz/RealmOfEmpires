using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Text.RegularExpressions;
using System.Text;

/// <summary>
/// Summary description for BBCodes
/// </summary>
public class BBCodes
{
    public enum Medium
    {
        InternalMail
        , ClanPublicProfile
        , ClanForum
        ,
        ClanForumQuickComm
            , Note
        ,PlayerProfile
        , Chat
    }

    public enum Env
    {
        Desktop,
        Desktop_InPopup,
        Mobile_NoIFramePopuBrowser,
        Mobile_IframePopupBrowser,
        Desktop2
    }



    #region static, recompiled Regex classes
    /// <summary>
    /// capture the village tag: [V] ( -55 , 66 )  [/V] 
    /// </summary>
    static private Regex Ex_Villages = new Regex(@"\[v\]\s*\(?\s*([-]?[0-9]{1,3})\s*\,\s*([-]?[0-9]{1,3})\s*\)?\s*\[/v\]", RegexOptions.IgnoreCase);
    /// <summary>
    /// capture the village tag: ( -55 , 66 )
    /// </summary>
    static private Regex Ex_Villages_implicit = new Regex(@"(?<!\[v\]\s*)(\(\s*([-]?[0-9]{1,3})\s*\,\s*([-]?[0-9]{1,3})\s*\))", RegexOptions.IgnoreCase);
    /// <summary>
    /// capture the village tag: [V id=44] ( -55 , 66 )  [/V] 
    /// </summary>
    static private Regex Ex_VillagesWithID = new Regex(@"\[v\s+id=([0-9]+)\]\s*\(?\s*([-]?[0-9]{1,3})\s*\,\s*([-]?[0-9]{1,3})\s*\)?\s*\[/v\]", RegexOptions.IgnoreCase);
    /// <summary>
    /// capture the village tag: [V id=44] ( -55 , 66 )  [/V] 
    /// </summary>
    static private Regex Ex_VillagesWithID_IgnoreID = new Regex(@"\[v\s+id=[0-9]+\]\s*\(?\s*([-]?[0-9]{1,3})\s*\,\s*([-]?[0-9]{1,3})\s*\)?\s*\[/v\]", RegexOptions.IgnoreCase);

    /// <summary>
    /// capture the player tag: [p] Some.Player_Name  [/P] 
    /// </summary>
    static private Regex Ex_Player = new Regex(@"\[p\]\s*([a-z0-9._]{1,25})\s*\[/p\]", RegexOptions.IgnoreCase);
    /// <summary>
    /// capture the player tag: [p id=33] Some.Player_Name  [/P]. 
    /// </summary>
    static private Regex Ex_PlayerWithID = new Regex(@"\[p\s+id=([0-9]+)\]\s*([a-z0-9._]{1,25})\s*\[/p\]", RegexOptions.IgnoreCase);
    /// <summary>
    /// capture the player tag: [p id=33] Some.Player_Name  [/P] but discard the id=33 part since we ignore this while pre-processing bb-codes 
    /// </summary>
    static private Regex Ex_PlayerWithID_IgnoreID = new Regex(@"\[p\s+id=[0-9]+\]\s*([a-z0-9._]{1,25})\s*\[/p\]", RegexOptions.IgnoreCase);


    /// <summary>
    /// capture the clan tag: [C] Some.@#$@#$ClanName  [/C] 
    /// </summary>
    static private Regex Ex_Clan = new Regex(@"\[c\]\s*([^\]]{1,30})\s*\[/c\]", RegexOptions.IgnoreCase);
    /// <summary>
    /// capture the clan tag: [c id=33] Some.@#$@#$ClanName   [/C]. 
    /// </summary>
    static private Regex Ex_ClanWithID = new Regex(@"\[c\s+id=([0-9]+)\]\s*([^\]]{1,30})\s*\[/c\]", RegexOptions.IgnoreCase);
    /// <summary>
    /// capture the player tag: [c id=33] comeclanname [/C] but discard the id=33 part since we ignore this while pre-processing bbcodes 
    /// </summary>
    static private Regex Ex_ClanWithID_IgnoreID = new Regex(@"\[c\s+id=[0-9]+\]\s*([^\]]{1,30})\s*\[/c\]", RegexOptions.IgnoreCase);
    /// <summary>
    /// capture the clan tag: [c id=33] Some.@#$@#$ClanName   [/C]. 
    /// </summary>
    static private Regex Ex_ClanWithID_IgnoreName = new Regex(@"\[c\s+id=([0-9]+)\]\s*[^\]]{1,30}\s*\[/c\]", RegexOptions.IgnoreCase);

    /// <summary>
    /// capture troops sequence: [troops] ?,333333,4,55,?,?,?,?[/troops]
    /// </summary>
    static private Regex Ex_Troops = new Regex(@"\[troops\]\s*\(?\s*(\??[0-9]{0,6})\s*\,\s*(\??[0-9]{0,6})\s*\)?\s*\,\s*(\??[0-9]{0,6})\s*\)?\s*\,\s*(\??[0-9]{0,6})\s*\)?\s*\,\s*(\??[0-9]{0,6})\s*\)?\s*\,\s*(\??[0-9]{0,6})\s*\)?\s*\,\s*(\??[0-9]{0,6})\s*\)?\s*\,\s*(\??[0-9]{0,6})\s*\)?\s*\[/troops\]", RegexOptions.IgnoreCase);
    static private Regex EX_Report = new Regex(@"\[report\](.+?)\[/report\]", RegexOptions.Singleline |RegexOptions.IgnoreCase );


    static private Regex EX_QUOTE = new Regex(@"\[quote\](.+?)\[/quote\]", RegexOptions.IgnoreCase | RegexOptions.Singleline);
    static private Regex EX_B = new Regex(@"\[b\](.+?)\[/b\]", RegexOptions.IgnoreCase | RegexOptions.Singleline);
    static private Regex Ex_I = new Regex(@"\[i\](.+?)\[/i\]", RegexOptions.IgnoreCase | RegexOptions.Singleline);
    static private Regex Ex_U = new Regex(@"\[u\](.+?)\[/u\]", RegexOptions.IgnoreCase | RegexOptions.Singleline);
    static private Regex Ex_S = new Regex(@"\[s\](.+?)\[/s\]", RegexOptions.IgnoreCase | RegexOptions.Singleline);
    static private Regex Ex_Url = new Regex(@"\[url\=([^\]]+)\]([^\]]+)\[/url\]", RegexOptions.IgnoreCase);
    static private Regex Ex_Img1 = new Regex(@"\[img\]([^\]]+)\[/img\]", RegexOptions.IgnoreCase);
    static private Regex Ex_Img2 = new Regex(@"\[img\=([^\]]+)\]([^\]]+)\[/img\]", RegexOptions.IgnoreCase);
    static private Regex Ex_Color1 = new Regex(@"\[color\=([^\]]+)\]([^\]]+)\[/color\]", RegexOptions.IgnoreCase);
    static private Regex Ex_Color2 = new Regex(@"\[colour\=([^\]]+)\]([^\]]+)\[/colour\]", RegexOptions.IgnoreCase);
    static private Regex Ex_Size = new Regex(@"\[size\=([^\]]+)\]([^\]]+)\[/size\]", RegexOptions.IgnoreCase);
    static private Regex EX_TABLE = new Regex(@"\[table\](.+?)\[/table\]", RegexOptions.IgnoreCase);
    static private Regex EX_TR = new Regex(@"\[tr\](.+?)\[/tr\]", RegexOptions.IgnoreCase);
    static private Regex EX_TD = new Regex(@"\[td\](.+?)\[/td\]", RegexOptions.IgnoreCase);

    #endregion 

    
    static public string PlayerProfileToHTML(Fbg.Bll.Realm realm, string str, bool isInPopup)
    {
        return ToHTML(null, realm, Medium.PlayerProfile, str, isInPopup ? Env.Desktop_InPopup : Env.Desktop);
    }
    static public string NoteToHTML(Fbg.Bll.Realm realm, string str, bool isInPopup)
    {
        return ToHTML(null, realm, Medium.Note, str, isInPopup ? Env.Desktop_InPopup : Env.Desktop);
    }
    /// <summary>
    /// for mobile
    /// </summary>
    /// <param name="realm"></param>
    /// <param name="str"></param>
    /// <returns></returns>
    static public string InternalMailToHTMLm(Fbg.Bll.Player player, string str, bool isPopupBrowser)
    {
        return ToHTML(player, player.Realm, Medium.InternalMail, str, isPopupBrowser ? Env.Mobile_IframePopupBrowser : Env.Mobile_NoIFramePopuBrowser);
    }
    static public string InternalMailToHTML(Fbg.Bll.Realm realm, string str)
    {
        return InternalMailToHTML(realm, str, false);
    }
    static public string InternalMailToHTML(Fbg.Bll.Realm realm, string str, bool isInPopup)
    {
        return ToHTML(null, realm, Medium.InternalMail, str, isInPopup ? Env.Desktop_InPopup : Env.Desktop);
    }
    static public string ClanForumBodyToHTML(Fbg.Bll.Realm realm, string str, Env env)
    {
        return ToHTML(null, realm, Medium.ClanForum, str, env);
    }
    static public string ClanForumBodyToHTML(Fbg.Bll.Realm realm, string str)
    {
        return ClanForumBodyToHTML(realm, str, Env.Desktop);
    }
    static public string ClanForumQuickCommToHTML(Fbg.Bll.Realm realm, string str)
    {
        return ToHTML(null, realm, Medium.ClanForumQuickComm, str);
    }
    static public string ClanPublicProfileToHTML(Fbg.Bll.Realm realm, string str, Env env)
    {
        return ToHTML(null, realm, Medium.ClanPublicProfile, str, env);
    }
    static public string ClanPublicProfileToHTML(Fbg.Bll.Realm realm, string str)
    {
        return ClanPublicProfileToHTML(realm, str, Env.Desktop);
    }
    static public string ChatHTML(Fbg.Bll.Realm realm, string str)
    {
        return ToHTML(null, realm, Medium.Chat, str);
    }
    static public string ToHTML(Fbg.Bll.Player player, Fbg.Bll.Realm realm, Medium medium, string str)
    {
        return ToHTML(player, realm, medium, str, Env.Desktop);
    }
    static public string ToHTML(Fbg.Bll.Player player, Fbg.Bll.Realm realm, Medium medium, string str, Env env)
    {
        return ToHtmlComplexBBCodes(player, realm, medium, ToHTMLSimpleBBCodes(medium, str, (env == Env.Desktop_InPopup ? true : false) ), env);
    }

    /// <summary>
    /// Converts some bbcodes into simpler ones that will be easier to display later. 
    /// it does not convert bb-codes into html but rather bb-codes to other bb-codes. 
    /// 
    /// For example, [p]playername[/p] is turned into [p id=PlayerID]playername[/p]. 
    ///     The playername in the original bbcode is validated and this new bbcode is created that does not
    ///     require a trip to the DB when displaying
    /// </summary>
    /// <param name="realm"></param>
    /// <param name="str"></param>
    /// <returns></returns>
    static public string PreProcessBBCodes(Fbg.Bll.Realm realm, Medium medium, string str)
    {
        //
        // handle the players bb-coded of type [p]playername[p] - bb code that requires a trip to the DB. 
        //  duplicate player names are handled by the datalayer
        //
        str = PreProcessBBCodes_PlayerHelper(Ex_Player, str, realm, medium);
        //
        // handle the players bb-coded of type [p id=XXX]playername[/p]. Such BB-code should really not be here
        //  since it is an internal, preprocessed version of [p]playername[/p] but in case it does happen to be here, 
        //  we treat it the same as [p]playername[/p] - completely ignore the ID and we replace it with the correct ID based
        //  on the player name
        //
        str = PreProcessBBCodes_PlayerHelper(Ex_PlayerWithID_IgnoreID, str, realm, medium);

        //
        // handle the clan bb-coded of type [c]clanname[/c] - bb code that requires a trip to the DB. 
        //  duplicate clan names are handled by the datalayer
        //
        str = PreProcessBBCodes_ClanHelper(Ex_Clan, str, realm, medium);
        //
        // handle the clan bb-coded of type [c id=XXX]clanname[/c]. Such BB-code should really not be here
        //  since it is an internal, preprocessed version of [c]clanname[/c] but in case it does happen to be here, 
        //  we treat it the same as [c]clanname[/c] - completely ignore the ID and we replace it with the correct ID based
        //  on the clan name
        //
        str = PreProcessBBCodes_ClanHelper(Ex_ClanWithID_IgnoreID, str, realm, medium);

        //
        // convert any case of villge coordinates not being wrapped in [v][/v] bb code ( something like "(xxx,yyy)") to a bb-code wrapped one
        //
        // i.e. "(xxx,yyy)"  -> "[v](xxx,yyy)[/v]"
        //
        str = Ex_Villages_implicit.Replace(str, "[v]($2,$3)[/v]");
        //
        // handle the village bb-coded of type [v]xx,yy[/v] - bb code that requires a trip to the DB. 
        //  duplicate cords are handled by the datalayer
        //
        str = PreProcessBBCodes_VillageHelper(Ex_Villages, str, realm, false, medium); /*when prepocessing bbcodes, we dont care if we are in a popup or not*/
        //
        // handle the village bb-coded of type [v id=44]xx,yy[/v] Such BB-code should really not be here
        //  since it is an internal, preprocessed version of [v]xx,yy[/v] but in case it does happen to be here, 
        //  we treat it the same as [v]xx,yy[/v] - completely ignore the ID and we replace it with the correct ID based
        //  on the cords
        //
        str = PreProcessBBCodes_VillageHelper(Ex_VillagesWithID_IgnoreID, str, realm, false, medium); /*when prepocessing bbcodes, we dont care if we are in a popup or not*/


        return str;
    }


    /// <summary>
    /// Since internal mail has preprocessed bbcodes stored in the DB, we want to convert back to original, player entered bb-codes 
    /// and this does so. Use this method to clean up messages stored in the DB before letting the player edit them. 
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public static string CleanUpPreProcessedBBCodes(Fbg.Bll.Realm realm, string str)
    {
        str = Ex_PlayerWithID_IgnoreID.Replace(str, "[P]$1[/P]");
        str = Ex_ClanWithID_IgnoreID.Replace(str, "[C]$1[/C]");
        str = Ex_VillagesWithID_IgnoreID.Replace(str, "[V]$1,$2[/V]");
              
        return str;
    }

    static private string PreProcessBBCodes_PlayerHelper(Regex regex, string str, Fbg.Bll.Realm realm, Medium medium)
    {
        string listOfPlayerName = GetMatchesListAsString(regex, str, 20);
        DataTable dtPlayers = Fbg.Bll.BBCodes.CheckPlayerNames(realm, listOfPlayerName);
        PlayerBBCodeMatchEvaluatorHelper2 helper = new PlayerBBCodeMatchEvaluatorHelper2(dtPlayers, medium);

        MatchEvaluator evaluator = new MatchEvaluator(helper.ReplacePlayerWithPreProcessedBBCode);
        str = regex.Replace(str, evaluator);

        return str;
    }

    static private string PreProcessBBCodes_ClanHelper(Regex regex, string str, Fbg.Bll.Realm realm, Medium medium)
    {
        string listOfNames = GetMatchesListAsString(regex, str, 20);
        DataTable dt = Fbg.Bll.BBCodes.CheckClanNames(realm, listOfNames);
        ClanBBCodeMatchEvaluatorHelper2 helper = new ClanBBCodeMatchEvaluatorHelper2(dt, medium);

        MatchEvaluator evaluator = new MatchEvaluator(helper.ReplaceClan);
        str = regex.Replace(str, evaluator);

        return str;
    }
    static private string PreProcessBBCodes_VillageHelper(Regex regex, string str, Fbg.Bll.Realm realm, bool isInPopup, Medium medium)
    {
        string whereClause = GetVillageMatchesListAswhereClause(regex, str);

        if (whereClause.Length > 0)
        {
            DataTable dt = Fbg.Bll.BBCodes.CheckVillageCords(realm, whereClause);
            VillageBBCodeMatchEvaluatorHelper helper = new VillageBBCodeMatchEvaluatorHelper(dt, medium, (isInPopup ? Env.Desktop_InPopup : Env.Desktop), null);

            MatchEvaluator evaluator = new MatchEvaluator(helper.ReplaceVillageWithPreProcessedBBCode);
            str = regex.Replace(str, evaluator);
        }
        return str;
    }


    private class PlayerBBCodeMatchEvaluatorHelper_ByName
    {
        DataRow dr;
        DataTable dt;
        Env _env;
        Medium _medium;
        public PlayerBBCodeMatchEvaluatorHelper_ByName(DataTable dt, Env env, Medium medium)
        {
            this.dt = dt;
            _env = env;
            _medium = medium;
        }

        public string ReplacePlayer(Match m)
        {
            dr = dt.Rows.Find(m.Groups[1].Value);
            if (dr == null)
            {                
                if (_medium == Medium.Chat)
                {
                    return m.Value ;
                }
                else
                {
                    return "<span class='Error'>" + m.Value + "</span>";
                }
            }
            else
            {
                if (_medium == Medium.Chat)
                {
                    // we handle BBcodes in chat differently, in fact, we'll migrate all BB-codes to work this way:
                    //  rather then attaching the proper on click handler here, we just put a class on the link, and we let the client decide how it wants to handle it
                    return String.Format("<a class='bbcode_p' data-pid='{0}'>{1}</a>"
                        , dr[Fbg.Bll.utils.CONSTS.GetPlayersByNameColIndex.PlayerID]
                        , dr[Fbg.Bll.utils.CONSTS.GetPlayersByNameColIndex.Name]);
                }
                else
                {
                    switch (_env)
                    {
                        case Env.Desktop_InPopup:
                            return String.Format("<a href=\"{0}\">{1}</a>"
                                , NavigationHelper.PlayerPublicOverview_NoTildaPopup((int)dr[Fbg.Bll.utils.CONSTS.GetPlayersByNameColIndex.PlayerID])
                                , dr[Fbg.Bll.utils.CONSTS.GetPlayersByNameColIndex.Name]);
                        case Env.Desktop:
                            return String.Format("<a onclick=\"return popupPlayerOverview('{0}','{1}')\">{1}</a>"
                                , NavigationHelper.PlayerPublicOverview_NoTildaPopup((int)dr[Fbg.Bll.utils.CONSTS.GetPlayersByNameColIndex.PlayerID])
                                , dr[Fbg.Bll.utils.CONSTS.GetPlayersByNameColIndex.Name]);
                        case Env.Mobile_IframePopupBrowser:
                        case Env.Mobile_NoIFramePopuBrowser:
                        case Env.Desktop2:
                            return String.Format("<a class='bbcode_p' data-pid='{0}'>{1}</a>"
                                , dr[Fbg.Bll.utils.CONSTS.GetPlayersByNameColIndex.PlayerID]
                                , dr[Fbg.Bll.utils.CONSTS.GetPlayersByNameColIndex.Name]);
                        default:
                            return (string)dr[Fbg.Bll.utils.CONSTS.GetPlayersByNameColIndex.Name];
                    }
                }

            }
        }
    }
    private class PlayerBBCodeMatchEvaluatorHelper2
    {
        DataRow dr;
        DataTable dt;
        Medium _medium;
        public PlayerBBCodeMatchEvaluatorHelper2(DataTable dt, Medium medium)
        {
            this.dt = dt;
            _medium = medium;
        }

        public string ReplacePlayerWithPreProcessedBBCode(Match m)
        {
            dr = dt.Rows.Find(m.Groups[1].Value);
            if (dr == null)
            {
                // player name not found. we not this by making ID=0
                
                if (_medium == Medium.Chat)
                {
                    return String.Format("{0}", m.Groups[1].Value);
                }
                else
                {
                    return String.Format("[P id=0]{0}[/P]", m.Groups[1].Value);
                }
            }
            else
            {
                return String.Format("[P id={0}]{1}[/P]"
                    , (int)dr[Fbg.Bll.utils.CONSTS.GetPlayersByNameColIndex.PlayerID]
                    , dr[Fbg.Bll.utils.CONSTS.GetPlayersByNameColIndex.Name]);
            }

        }
    }
    private class PlayerBBCodeMatchEvaluatorHelper_ByID
    {
        Env _env;
        Medium _medium; 
        public PlayerBBCodeMatchEvaluatorHelper_ByID(Env env, Medium medium)
        {
            _env = env;
            _medium = medium;
        }

        public string ReplacePlayer(Match m)
        {
            
            if (m.Groups[1].Value == "0")
            {                
                if (_medium == Medium.Chat)
                {
                    return "[P]" + m.Groups[2].Value + "[/P]";
                }
                else
                {
                    return "<span class='Error'>[P]" + m.Groups[2].Value + "[/P]</span>";
                }
            }
            else
            {
                if (_medium == Medium.Chat)
                {
                    // we handle BBcodes in chat differently, in fact, we'll migrate all BB-codes to work this way:
                    //  rather then attaching the proper on click handler here, we just put a class on the link, and we let the client decide how it wants to handle it
                    return String.Format("<a class='bbcode_p' data-pid='{0}'>{1}</a>", m.Groups[1].Value, m.Groups[2].Value);
                }
                else
                {

                    switch (_env)
                    {
                        case Env.Desktop_InPopup:
                            return String.Format("<a class='bbcode_p' data-pid='{0}' href=\"playerpopup.aspx?pid={0}\">{1}</a>", m.Groups[1].Value, m.Groups[2].Value);
                        case Env.Desktop:
                            return String.Format("<a class='bbcode_p' data-pid='{0}' href=\"#\" onclick=\"return popupPlayerOverview('playerpopup.aspx?pid={0}','{0}')\">{1}</a>", m.Groups[1].Value, m.Groups[2].Value);
                        case Env.Mobile_NoIFramePopuBrowser:
                        case Env.Mobile_IframePopupBrowser:
                        case Env.Desktop2:
                            return String.Format("<a class='bbcode_p' data-pid='{0}'>{1}</a>", m.Groups[1].Value, m.Groups[2].Value);
                        default:
                            return m.Groups[2].Value;
                    }
                }
            }
        }
    }
    private class ClanBBCodeMatchEvaluatorHelper2
    {
        DataRow dr;
        DataTable dt;
        Medium _medium;
        public ClanBBCodeMatchEvaluatorHelper2(DataTable dt, Medium medium)
        {
            this.dt = dt;
            _medium = medium;
        }

        public string ReplaceClan(Match m)
        {
            dr = dt.Rows.Find(m.Groups[1].Value);
            if (dr == null)
            {
                // clan name not found. we not this by making ID=0
                if (_medium == Medium.Chat)
                {
                    return String.Format("{0}", m.Groups[1].Value); ;
                }
                else
                {
                    return String.Format("[C id=0]{0}[/C]", m.Groups[1].Value);
                }            
            }
            else
            {
                return String.Format("[C id={0}]{1}[/C]"
                    , (int)dr[Fbg.Bll.utils.CONSTS.GetClansByXColIndex.ID]
                    , dr[Fbg.Bll.utils.CONSTS.GetClansByXColIndex.Name]);
            }

        }
    }
    private class ClanBBCodeMatchEvaluatorHelper1
    {
        DataRow dr;
        DataTable dt;
        bool _isInPopup;
        Medium _medium;
        public ClanBBCodeMatchEvaluatorHelper1(DataTable dt, bool isInPopup, Medium medium)
        {
            this.dt = dt;
            _isInPopup = isInPopup;
            _medium = medium;
        }

        public string ReplaceClan(Match m)
        {
            dr = dt.Rows.Find(m.Groups[1].Value);
            if (dr == null)
            {
                // clan name not found. 
                if (_medium == Medium.Chat)
                {
                    return String.Format("{0}", m.Groups[2].Value);
                }
                else
                {
                    return String.Format("<span class='Error'>[C]{0}[/C]</span>", m.Groups[2].Value);
                }
            }
            else
            {

                if (_medium == Medium.Chat)
                {
                    // we handle BBcodes in chat differently, in fact, we'll migrate all BB-codes to work this way:
                    //  rather then attaching the proper on click handler here, we just put a class on the link, and we let the client decide how it wants to handle it
                    return String.Format("<a class='bbcode_c' data-cid={0}>{1}</a>"
                        , dr[Fbg.Bll.utils.CONSTS.GetClansByXColIndex.ID]
                        , dr[Fbg.Bll.utils.CONSTS.GetClansByXColIndex.Name]);
                }
                else
                {
                    return String.Format("<a class='bbcode_c' data-cid={0} {2} href=clanpublicprofile.aspx?clanid={0}>{1}</a>"
                        , dr[Fbg.Bll.utils.CONSTS.GetClansByXColIndex.ID]
                        , dr[Fbg.Bll.utils.CONSTS.GetClansByXColIndex.Name]
                        , _isInPopup ? "target=_parent" : "");
                }
            }
        }
    }
    private class ClanBBCodeMatchEvaluatorHelper3
    {
        DataRow dr;
        DataTable dt;
        Env _env;
        Medium _medium;
        public ClanBBCodeMatchEvaluatorHelper3(DataTable dt, Env env, Medium medium)
        {
            this.dt = dt;
            _env = env;
            _medium = medium;
        }

        public string ReplaceClan(Match m)
        {
            dr = dt.Rows.Find(m.Groups[1].Value);
            if (dr == null)
            {
                // clan name not found. this should never really happen!! since this deal with the 
                //  preprocessed bbcode so it should be right... 
                if (_medium == Medium.Chat)
                {
                    return String.Format("{0}", m.Groups[2].Value);
                }
                else
                {
                    return String.Format("<span class='Error'>[C]{0}[/C]</span>", m.Groups[2].Value);
                }
            }
            else
            {
                if (_medium == Medium.Chat)
                {
                    // we handle BBcodes in chat differently, in fact, we'll migrate all BB-codes to work this way:
                    //  rather then attaching the proper on click handler here, we just put a class on the link, and we let the client decide how it wants to handle it
                    return String.Format("<a class='bbcode_c' data-cid={0}>{1}</a>"
                    , dr[Fbg.Bll.utils.CONSTS.GetClansByXColIndex.ID]
                    , dr[Fbg.Bll.utils.CONSTS.GetClansByXColIndex.Name]);
                }
                else
                {
                    switch (_env)
                    {
                        case Env.Desktop:
                        case Env.Desktop_InPopup:
                            return String.Format("<a  class='bbcode_c' data-cid='{0}' {2} href=clanpublicprofile.aspx?clanid={0}>{1}</a>"
                                , dr[Fbg.Bll.utils.CONSTS.GetClansByXColIndex.ID]
                                , dr[Fbg.Bll.utils.CONSTS.GetClansByXColIndex.Name]
                                , ((_env == Env.Desktop_InPopup) ? "target=_parent" : "")
                                );
                        case Env.Mobile_NoIFramePopuBrowser:
                            return String.Format("<a class='bbcode_c' data-cid='{0}'>{1}</a>"
                            , dr[Fbg.Bll.utils.CONSTS.GetClansByXColIndex.ID]
                            , dr[Fbg.Bll.utils.CONSTS.GetClansByXColIndex.Name]
                            );
                        case Env.Mobile_IframePopupBrowser:
                           
                        case Env.Desktop2:
                            return String.Format("<a  class='bbcode_c' data-cid='{0}'>{1}</a>"
                            , dr[Fbg.Bll.utils.CONSTS.GetClansByXColIndex.ID]
                            , dr[Fbg.Bll.utils.CONSTS.GetClansByXColIndex.Name]
                            );
                        default:
                            return "";

                    }
                }
            }

        }
    }
    private class VillageBBCodeMatchEvaluatorHelper
    {
        DataRow dr;
        DataTable dt;
        Env _env;
        /// <summary>
        /// only valid (non-null) if env is mobile
        /// </summary>
        int? _playerID;
        Medium _medium;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="env"></param>
        /// <param name="playerID">only valid if env is mobile</param>
        public VillageBBCodeMatchEvaluatorHelper(DataTable dt, Medium medium, Env env, int? playerID)
        {
            this.dt = dt;
            _env = env;
            _playerID = playerID;
            _medium = medium;
        }

        public string ReplaceVillageWithPreProcessedBBCode(Match m)
        {
            dr = dt.Rows.Find(new object[]{m.Groups[1].Value, m.Groups[2].Value});
            if (dr == null)
            {
                // clan name not found. we not this by making ID=0
                return String.Format("[V id=0]{0},{1}[/V]", m.Groups[1].Value, m.Groups[2].Value);
            }
            else
            {
                return String.Format("[V id={0}]{1},{2}[/V]"
                    , dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.ID]
                    , dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.XCord]
                    , dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.YCord]);
            }

        }
        public string ReplaceVillageToHtmlViaCords(Match m)
        {
            dr = dt.Rows.Find(new object[] { m.Groups[1].Value, m.Groups[2].Value });
            if (dr == null)
            {
                // clan name not found. we not this by making ID=0
                if (_medium == Medium.Chat)
                {
                    return String.Format("({0},{1})", m.Groups[1].Value, m.Groups[2].Value);
                }
                else
                {
                    return String.Format("<span class='Error'>[V]{0},{1}[/V]</span>", m.Groups[1].Value, m.Groups[2].Value);
                }
            }
            else
            {
                if (_medium == Medium.Chat)
                {
                    // we handle BBcodes in chat differently, in fact, we'll migrate all BB-codes to work this way:
                    //  rather then attaching the proper on click handler here, we just put a class on the link, and we let the client decide how it wants to handle it
                    return String.Format("<a class='bbcode_v' data-vid='{3}' data-ownerpid='{4}'>{0}({1},{2})</a>"
                        , dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.Name]
                        , dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.XCord]
                        , dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.YCord]
                        , dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.ID]
                        , dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.OwnerPlayerID]);
                }
                else 
                {
                    switch (_env)
                    {
                        case Env.Desktop_InPopup:
                            return String.Format("<a class='bbcode_v {4}' data-vid='{3}' href=\"{0}\">{1}({2},{3})</a>"
                                , NavigationHelper.VillageOtherOverview_NoTildaPopup((int)dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.ID])
                                , dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.Name]
                                , dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.XCord]
                                , dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.YCord]
                                , dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.ID]
                                , (int)dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.OwnerPlayerID] == (int)_playerID ? "mine" : "");
                        case Env.Desktop:
                            return String.Format("<a class='bbcode_v {4}' data-vid='{3}' href=\"#\" onclick=\"popupVilageOverview('{0}','{4}');\">{1}({2},{3})</a>"
                                , NavigationHelper.VillageOtherOverview_NoTildaPopup((int)dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.ID])
                                , dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.Name]
                                , dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.XCord]
                                , dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.YCord]
                                , dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.ID]
                                , (int)dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.OwnerPlayerID] == (int)_playerID ? "mine" : "");
                        case Env.Mobile_NoIFramePopuBrowser:
                        case Env.Mobile_IframePopupBrowser:
                        case Env.Desktop2:
                            return String.Format("<a class='bbcode_v {4}' data-vid='{3}'>{0}({1},{2})</a>"
                                , dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.Name]
                                , dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.XCord]
                                , dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.YCord]
                                , dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.ID]
                                , (int)dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.OwnerPlayerID] == (int)_playerID ? "mine" : "");
                        default:
                            throw new ArgumentException("unknown val", "_env");
                    }
                }            
            }
        }

        public string ReplaceVillageToHtmlViaID(Match m)
        {
            dr = dt.Rows.Find(new object[] { m.Groups[1].Value});
            if (dr == null)
            {
                // village not found
               
                if (_medium == Medium.Chat)
                {
                    return String.Format("({0},{1})", m.Groups[2].Value, m.Groups[3].Value);
                }
                else
                {
                    return String.Format("<span class='Error'>[V]{0},{1}[/V]</span>", m.Groups[2].Value, m.Groups[3].Value);
                }
            }
            else
            {
                if (_medium == Medium.Chat)
                {
                    // we handle BBcodes in chat differently, in fact, we'll migrate all BB-codes to work this way:
                    //  rather then attaching the proper on click handler here, we just put a class on the link, and we let the client decide how it wants to handle it
                    return String.Format("<a class='bbcode_v' data-vid='{3}' data-ownerpid='{4}'>{0}({1},{2})</a>"
                        , dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.Name]
                        , dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.XCord]
                        , dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.YCord]
                        , dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.ID]
                        , (int)dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.OwnerPlayerID]);
                }
                else
                {
                    switch (_env)
                    {
                        case Env.Desktop_InPopup:
                            return String.Format("<a class='bbcode_v {4}' data-vid='{3}' href=\"{0}\">{1}({2},{3})</a>"
                                , NavigationHelper.VillageOtherOverview_NoTildaPopup((int)dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.ID])
                                , dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.Name]
                                , dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.XCord]
                                , dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.YCord]
                                , dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.ID]
                                , (int)dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.OwnerPlayerID] == (int)(_playerID == null ? 0 :_playerID)  ? "mine" : "");
                        case Env.Desktop:
                            return String.Format("<a class='bbcode_v {5}' data-vid='{4}' href=\"#\" onclick=\"popupVilageOverview('{0}','{4}');\">{1}({2},{3})</a>"
                                , NavigationHelper.VillageOtherOverview_NoTildaPopup((int)dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.ID])
                                , dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.Name]
                                , dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.XCord]
                                , dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.YCord]
                                , dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.ID]
                                , (int)dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.OwnerPlayerID] == (int)(_playerID == null ? 0 : _playerID) ? "mine" : "");
                        case Env.Mobile_NoIFramePopuBrowser:
                        case Env.Mobile_IframePopupBrowser:
                        case Env.Desktop2:
                            return String.Format("<a class='bbcode_v {4}' data-vid='{3}'>{0}({1},{2})</a>"
                                , dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.Name]
                                , dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.XCord]
                                , dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.YCord]
                                , dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.ID]
                                , (int)dr[Fbg.Bll.utils.CONSTS.GetVillagesByXColIndex.OwnerPlayerID] == (int)(_playerID == null ? 0 : _playerID) ? "mine" : "");
                        default:
                            throw new ArgumentException("unknown val", "_env");
                    }
                }
            }

        }
    }    

    /// <summary>
    /// 
    /// </summary>
    /// <param name="player">MAY BE NULL!!! ONly implemented in mobile now!</param>
    /// <param name="realm"></param>
    /// <param name="medium"></param>
    /// <param name="str"></param>
    /// <param name="env"></param>
    /// <returns></returns>
    static private string ToHtmlComplexBBCodes(Fbg.Bll.Player player, Fbg.Bll.Realm realm, Medium medium, string str, Env env)
    {
        string list;
        DataTable dt;
        MatchEvaluator evaluator;


        #region Village [V] tags

        //
        // handle the village bb-coded of type [v]xx,yy[/v] - bb code that requires a trip to the DB. 
        //  duplicate cords are handled by the datalayer. Those bb codes should not really be there as they should be 
        //  preprocessed to [v id=zzz]xx,yy[/v] type of bbcode but in case something slip through, we convert them to html 
        //
        list = GetVillageMatchesListAswhereClause(Ex_Villages, str);
        if (list.Length > 0)
        {
            dt = Fbg.Bll.BBCodes.CheckVillageCords(realm, list);
            VillageBBCodeMatchEvaluatorHelper villhelper = new VillageBBCodeMatchEvaluatorHelper(dt, medium, env, player ==null ?  null : (int?)player.ID);

            evaluator = new MatchEvaluator(villhelper.ReplaceVillageToHtmlViaCords);
            str = Ex_Villages.Replace(str, evaluator);
        }
        //
        // handle the village bb-coded of type [v id=zz]xx,yy[/v] - bb code that requires a trip to the DB. 
        //
        list = GetMatchesListAsString(Ex_VillagesWithID, str, 4);
        if (list.Length > 0)
        {
            dt = Fbg.Bll.BBCodes.CheckVillageIDs(realm, list);
            VillageBBCodeMatchEvaluatorHelper villhelper = new VillageBBCodeMatchEvaluatorHelper(dt, medium, env, player == null ? null : (int?)player.ID);

            evaluator = new MatchEvaluator(villhelper.ReplaceVillageToHtmlViaID);
            str = Ex_VillagesWithID.Replace(str, evaluator);
        }
        #endregion

        #region PLAYER [p] tags
        //
        // handle the players bb-coded of type [p]playername[p] - bb code that requires a trip to the DB. 
        //  duplicate player names are handled by the datalayer
        //
        list = GetMatchesListAsString(Ex_Player, str, 20);
        if (list.Length > 0)
        {
            DataTable dtPlayers = Fbg.Bll.BBCodes.CheckPlayerNames(realm, list);
            PlayerBBCodeMatchEvaluatorHelper_ByName helper = new PlayerBBCodeMatchEvaluatorHelper_ByName(dtPlayers, env, medium);

            evaluator = new MatchEvaluator(helper.ReplacePlayer);
            str = Ex_Player.Replace(str, evaluator);
        }

        //
        // handle the players bb-coded of type [p id=XXX]playername[p] - bb code that does not require a database trip
        //  duplicate player names are handled by the datalayer
        //
        //str = Ex_PlayerWithID.Replace(str, "<a href=\"player.aspx?pid=$1\">$2</a>");
        PlayerBBCodeMatchEvaluatorHelper_ByID helper3 = new PlayerBBCodeMatchEvaluatorHelper_ByID(env, medium);
        evaluator = new MatchEvaluator(helper3.ReplacePlayer);
        str = Ex_PlayerWithID.Replace(str, evaluator);
        #endregion 

        #region CLAN [C] tags
        //
        // format the clan tag: [c] SOMEClanName [/C]
        // 
        list = GetMatchesListAsString(Ex_Clan, str, 20);
        if (list.Length > 0)
        {
            dt = Fbg.Bll.BBCodes.CheckClanNames(realm, list);
            ClanBBCodeMatchEvaluatorHelper1 clanhelper = new ClanBBCodeMatchEvaluatorHelper1(dt, (env == Env.Desktop_InPopup ? true : false), medium);
            evaluator = new MatchEvaluator(clanhelper.ReplaceClan);
            str = Ex_Clan.Replace(str, evaluator);
        }

        //
        // format the clan tag: [c id=33] SOMEClanName [/C]
        // 
        list = GetMatchesListAsString(Ex_ClanWithID, str, 4);
        if (list.Length > 0)
        {
            dt = Fbg.Bll.BBCodes.CheckClanIDs(realm, list);
            ClanBBCodeMatchEvaluatorHelper3 clanhelper3 = new ClanBBCodeMatchEvaluatorHelper3(dt, env, medium );
            evaluator = new MatchEvaluator(clanhelper3.ReplaceClan);
            str = Ex_ClanWithID.Replace(str, evaluator);
        }

        #endregion 


        return str;
    }



    static private string ToHTMLSimpleBBCodes(Medium medium, string str, bool isInPopup)
    {
        if (medium == Medium.Chat)
        {
            // for chat, we dont do any of these; 
            return str;
        }

        Regex exp;
        // format the bold tags: [b][/b]
        // becomes: <strong></strong>
        str = EX_B.Replace(str, "<strong>$1</strong>");

        // format the italic tags: [i][/i]
        // becomes: <em></em>
        str = Ex_I.Replace(str, "<em>$1</em>");

        // format the underline tags: [u][/u]
        // becomes: <u></u>
        str = Ex_U.Replace(str, "<u>$1</u>");

        // format the strike tags: [s][/s]
        // becomes: <strike></strike>
        str = Ex_S.Replace(str, "<strike>$1</strike>");

        if ( medium == Medium.Note)
        {
            // format the url tags: [url=www.website.com]my site[/url]
            // becomes: <a href="www.website.com">my site</a>
            str = Ex_Url.Replace(str, String.Format("<a {0} href=$1>$2</a>", isInPopup ? "target=_parent" : "")); 

            // format the img tags: [img]www.website.com/img/image.jpeg[/img]
            // becomes: <img src="www.website.com/img/image.jpeg" />
            str = Ex_Img1.Replace(str, "<img src=\"$1\" />");

            // format img tags with alt: [img=www.website.com/img/image.jpeg]this is the alt text[/img]
            // becomes: <img src="www.website.com/img/image.jpeg" alt="this is the alt text" />
            str = Ex_Img2.Replace(str, "<img src=\"$1\" alt=\"$2\" />");
        }

        if (medium == Medium.ClanForumQuickComm)
        {
            //strip the font bbcode
            str = Ex_Color1.Replace(str, "$2");
            str = Ex_Color2.Replace(str, "$2");
        }
        else
        {
            //format the colour tags: [color=red][/color]
            // becomes: <font color="red"></font>
            // supports UK English and US English spelling of colour/color
            str = Ex_Color1.Replace(str, "<font color=\"$1\">$2</font>");
            str = Ex_Color2.Replace(str, "<font color=\"$1\">$2</font>");
        }

        if (medium == Medium.ClanForumQuickComm)
        {
            //strip the size bbcode
            str = Ex_Size.Replace(str, "$2");
        }
        else
        {

            // format the size tags: [size=3][/size]
            // becomes: <font size="+3"></font>
            str = Ex_Size.Replace(str, "<font size=\"+$1\">$2</font>");
        }

        str = ToHTMLSimpleBBCodes_TABLE(str);

        if (medium == Medium.ClanForumQuickComm)
        {
            str = str.Replace("\r\n", "");
        }
        else
        {
            // replace any new line characters with <br />
            str = str.Replace("\r\n", "<br />\r\n");
        }


        if (medium == Medium.ClanForumQuickComm)
        {
            str = EX_QUOTE.Replace(str, "QUOTE:\"$1\"");
        }
        else
        {
            str = EX_QUOTE.Replace(str, "<blockquote><hr noshade=\"noshade\" size=\"1\"/>$1<hr noshade=\"noshade\" size=\"1\"/></blockquote>");
        }

        str = Ex_Troops.Replace(str, "<table cellspacing=1 cellpadding=2 class='bbcodeTroops'><tr class=hdr><td><img src='https://static.realmofempires.com/images/units/Militia.png' /></td><td><img src='https://static.realmofempires.com/images/units/Infantry.png'/></td><td ><img src='https://static.realmofempires.com/images/units/Cavalry.png'/></td><td><img src='https://static.realmofempires.com/images/units/Knight.png' /></td><td ><img src='https://static.realmofempires.com/images/units/ram.png' /></td><td><img src='https://static.realmofempires.com/images/units/treb.png'' /></td><td><img src='https://static.realmofempires.com/images/units/Spy.png' /></td><td ><img src='https://static.realmofempires.com/images/units/Governor.png' /></td></tr><tr><td>$1</td><td>$2</td><td >$3</td><td>$4</td><td>$5</td><td>$6</td><td>$7</td><td>$8</td></tr></table>");

        str = EX_Report.Replace(str, "<DIV class='bbcodeRepHeader'>Report</DIV><DIV class=bbcodeRepContent>$1</DIV>");

        return str;
    }

    static private string ToHTMLSimpleBBCodes_TABLE(string str)
    {
        //MatchCollection matches = EX_TABLE.Matches(str);
        //foreach (Match match in matches)
        //{
        //    whereClause.AppendFormat("(xcord={0} and ycord={1}) or", match.Groups[1].Value, match.Groups[2].Value);
        //}
        return str;
    }


    static private string GetMatchesListAsString(Regex ex, string str, int estMatchStringLength)
    {
        MatchCollection matches = ex.Matches(str);
        StringBuilder list = new StringBuilder(matches.Count * estMatchStringLength);
        foreach (Match match in matches)
        {
            list.Append(match.Groups[1].Value);
            list.Append(",");
        }
        if (list.Length > 0)
        {
            list.Remove(list.Length - 1, 1);
        }

        return list.ToString();
    }
    static private string GetVillageMatchesListAswhereClause(Regex ex, string str)
    {
        MatchCollection matches = ex.Matches(str);
        StringBuilder whereClause = new StringBuilder(matches.Count * 32);
        foreach (Match match in matches)
        {
            whereClause.AppendFormat("(xcord={0} and ycord={1}) or", match.Groups[1].Value, match.Groups[2].Value);
        }
        if (whereClause.Length > 0)
        {
            whereClause.Remove(whereClause.Length - 2, 2);
        }
        return whereClause.ToString();
    }

}
