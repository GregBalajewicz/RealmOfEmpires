using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections.Generic;

public partial class ChatUpdateAjax : JsonPage
{
    public class Chat
    {
        public int playerid;
        public string player;
        public string message; 
        public string time;
        public string timestamp;
        /// <summary>
        /// true ONLY if in global chat and the chat is to your clan
        /// </summary>
        public bool ChatToClanInGlobal;
        public string Title;
        public int XPLevel;
    }

    public class ChatWithAvatarImg :  Chat
    {
        public string avatarImgUrl;
    }
    public class ChatWithAvatarID: Chat
    {
        public string avatarID;
    }

    public int ChatWarnMess10
    {
        get
        {
            if (Session["chat.warn.10mess"] == null) {
                Session["chat.warn.10mess"] = 0;
            }
            return (int)Session["chat.warn.10mess"];
        }
        set
        {
            Session["chat.warn.10mess"] = value;
        }
    }

    public override object Result()
    {
        try
        {
            DateTime cookt;

            if (Request.Cookies["chat.warn"] == null){
                cookt = DateTime.Now;
                Response.AppendCookie(new HttpCookie("chat.warn", DateTime.Now.Ticks.ToString()));
            } else {
                cookt = new DateTime(Convert.ToInt64(Request.Cookies["chat.warn"].Value));
            }

            bool isClan = Convert.ToBoolean(Request["isclan"]);
            DateTime dt = Request["timestamp"] == "undefined" ? DateTime.Now.AddYears(-10) : new DateTime(Convert.ToInt64(Request["timestamp"]));
            int count = Convert.ToInt32(Request["count"] ?? "10");
            bool older = Convert.ToBoolean(Request["older"] ?? "false");
            bool avatarIDOnly = Convert.ToBoolean(Request["returnAvatarID"] ?? "false");
            bool isMobile = avatarIDOnly;
            IDataReader re = Fbg.DAL.Villages.GetChatByPlayer(FbgPlayer.Realm.ConnectionStr, FbgPlayer.ID, isClan, dt, count, older);

            List<Chat> chats = new List<Chat>();

            using (re)
            {
                while (re.Read())
                {
                    if (re.FieldCount == 1)
                    {
                        return re["Mes"];
                    }

                    var t = ((DateTime)re["Time"]);
                    var tu = t.ToUniversalTime();

                    if (avatarIDOnly) {
                        chats.Add(new ChatWithAvatarID
                        {
                            playerid = (int)re["PlayerId"],
                            player = re["PlayerName"].ToString(),
                            message = re["Msg"].ToString(),
                            time = tu.Hour.ToString("D2") + ":" + tu.Minute.ToString("D2"),
                            timestamp = t.Ticks.ToString(),
                            avatarID= re["AvatarID"].ToString(),
                            ChatToClanInGlobal = isClan ? false : !(re["ClanID"] is DBNull)
                            ,
                            Title = (string)re["Title"]
                            ,
                            XPLevel = Fbg.Bll.UsersXP.CalcLevel((int)re["xp_cached"])
                        });
                    }
                    else {
                        chats.Add(new ChatWithAvatarImg
                        {
                            playerid = (int)re["PlayerId"],
                            player = re["PlayerName"].ToString(),
                            message = re["Msg"].ToString(),
                            time = tu.Hour.ToString("D2") + ":" + tu.Minute.ToString("D2"),
                            timestamp = t.Ticks.ToString(),
                            avatarImgUrl = re["Image1Url"].ToString(),
                            ChatToClanInGlobal = isClan ? false : !(re["ClanID"] is DBNull)
                            ,
                            Title = (string)re["Title"]
                            ,
                            XPLevel = Fbg.Bll.UsersXP.CalcLevel((int)re["xp_cached"])
                        });
                    }
                }
            }

            var tn = DateTime.Now;
            var tnu = DateTime.UtcNow;

            ChatWarnMess10 += chats.Count;

            if (cookt.Ticks < DateTime.Now.AddMinutes(-10).Ticks && ChatWarnMess10 >= 10)
            {
                
                chats.Add(new Chat {
                    playerid = -1,
                    player = "bot",
                    message = (isMobile ? "No bad language, no personal attacks, respect other players - have fun! Click avatar image to block abusive players; This also flags player for investigation."
                        : "No bad language, no personal attacks, respect other players, English only! Have fun! Block abusive players; This also flags player for investigation."),
                    time = tnu.Hour.ToString("D2") + ":" + tnu.Minute.ToString("D2"),
                    timestamp = tn.Ticks.ToString()
                });

                Session["chat.warn.10mess"] = 0;
                Response.AppendCookie(new HttpCookie("chat.warn", DateTime.Now.Ticks.ToString()));
            }

            if (older)
            {
                return chats.OrderByDescending(c => c.timestamp).ToArray();
            }
            else
            {
                return chats.OrderBy(c => c.timestamp).ToArray();
            }

        }
        catch (Exception exc)
        {
            throw exc;
        }
    }
}
