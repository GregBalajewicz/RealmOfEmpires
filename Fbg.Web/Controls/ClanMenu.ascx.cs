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

public partial class Controls_ClanMenu : BaseControl
{

    protected void Page_Load(object sender, EventArgs e)
    {
        mnu_OtherClan.Items[0].NavigateUrl = "~/ClanPublicProfile.aspx?clanid=" + Request.QueryString["clanid"];
        mnu_OtherClan.Items[1].NavigateUrl = "~/ClanMembers.aspx?clanid=" + Request.QueryString["clanid"];

        BaseResName = "ClanMenu.ascx";
        mnu_MyClan.Items[0].Text = RS("Overview");
        mnu_NoClan.Items[0].Text = RS("Overview");
        mnu_MyClanAdmin.Items[0].Text = mnu_MyClanAdmin_m.Items[0].Text = RS("Overview");
        mnu_MyClan.Items[1].Text = mnu_MyClan_m.Items[1].Text = RS("PublicProfile");
        mnu_MyClanAdmin.Items[1].Text = mnu_MyClanAdmin_m.Items[1].Text  = RS("PublicProfile");
        mnu_OtherClan.Items[0].Text = RS("PublicProfile");
        mnu_MyClan.Items[2].Text = mnu_MyClan_m.Items[2].Text = RS("Invitations");
        mnu_MyClanAdmin.Items[2].Text = mnu_MyClanAdmin_m.Items[2].Text = RS("Invitations");
        mnu_MyClan.Items[3].Text = mnu_MyClan_m.Items[3].Text = RS("Members");
        mnu_MyClanAdmin.Items[3].Text = mnu_MyClanAdmin_m.Items[3].Text = RS("Members");
        mnu_OtherClan.Items[1].Text = RS("Members");
        mnu_MyClan.Items[4].Text = mnu_MyClan_m.Items[4].Text = RS("Forum");
        mnu_MyClanAdmin.Items[4].Text = RS("Forum");
        mnu_MyClanAdmin.Items[5].Text = IsMobile ?  RS("ForumAdmin") : RS("AdminBrackets");
        mnu_MyClan.Items[5].Text = mnu_MyClan_m.Items[4].Text = RS("Diplomacy");
        mnu_MyClanAdmin.Items[6].Text = mnu_MyClanAdmin_m.Items[4].Text  = RS("Diplomacy");
        mnu_MyClan.Items[6].Text = RS("Events");
        mnu_MyClanAdmin.Items[7].Text = RS("Events");
        mnu_MyClanAdmin.Items[8].Text = mnu_MyClanAdmin_m.Items[5].Text = RS("Settings");
        mnu_MyClanAdmin.Items[10].Text = mnu_MyClan.Items[7].Text = "Claims";

        
        if (Request.QueryString[CONSTS.QuerryString.ClanID] != null)
        {
            int clanid;
            if (Int32.TryParse(Request.QueryString[CONSTS.QuerryString.ClanID ], out clanid))//try to get clan id
            {
                if (Player.Clan != null)//player alreday have its clan
                {
                    if (Player.Clan.ID == clanid)//player try to see his clan public profile
                    {
                        if (Player.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.Owner)||Player.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.ForumAdministrator )||Player.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.Administrator))
                        {
                            mnu_MyClanAdmin.Visible = true;
                            mnu_OtherClan.Visible = false;
                            mnu_MyClan.Visible=false;
                        }
                        else
                        {
                            mnu_MyClan.Visible = true;
                            mnu_OtherClan.Visible = false;
                            mnu_MyClanAdmin.Visible=false;
                        }

                        //
                        // highlight the forum link if there is a new forum post indicator
                        //
                        if (_player.ForumChanged)
                        {
                            mnu_MyClan.Items[3].SeparatorImageUrl = "https://static.realmofempires.com/images/newforum.png";
                            mnu_MyClanAdmin.Items[3].SeparatorImageUrl = "https://static.realmofempires.com/images/newforum.png";
                        }
                    }
                    else//player try to see other clan
                    {
                        mnu_MyClanAdmin.Visible=false;
                        mnu_MyClan.Visible = false;
                        mnu_OtherClan.Visible = true;
                    }
                }
                else//player try to see other clan
                {
                    mnu_MyClanAdmin.Visible = false;
                    mnu_MyClan.Visible = false;
                    mnu_OtherClan.Visible = true;
                }
               

            }
            else//player try to play with th system with adding invalid char
            {
                mnu_MyClanAdmin.Visible = false ;
                mnu_MyClan.Visible = true;
                mnu_OtherClan.Visible = false;

            }

        }
        else
        {
            if (Player.Clan != null)
            {
                if (Player.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.Owner) || Player.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.ForumAdministrator) || Player.Role.IsPlayerPartOfRole(Fbg.Bll.Role.MemberRole.Administrator))
                {

                    mnu_MyClanAdmin.Visible = true;
                    mnu_OtherClan.Visible = false;
                    mnu_MyClan.Visible = false;
                }
                else
                {
                    mnu_MyClan.Visible = true;
                    mnu_OtherClan.Visible = false;
                    mnu_MyClanAdmin.Visible = false;
                }

                //
                // highlight the forum link if there is a new forum post indicator
                //
                if (_player.ForumChanged)
                {
                    mnu_MyClan.Items[3].SeparatorImageUrl = "https://static.realmofempires.com/images/newforum.png";
                    mnu_MyClanAdmin.Items[3].SeparatorImageUrl = "https://static.realmofempires.com/images/newforum.png";
                }

            }
            else
            {
                mnu_OtherClan.Visible = false;
                mnu_MyClan.Visible = false;
                mnu_NoClan.Visible = true;
            }

        }


        //
        //
        //
        mnu_MyClan_m.Visible = IsMobile ? mnu_MyClan.Visible : false;
        mnu_MyClan.Visible = !IsMobile ? mnu_MyClan.Visible : false;

        mnu_MyClanAdmin_m.Visible = IsMobile ? mnu_MyClanAdmin.Visible : false;
        mnu_MyClanAdmin.Visible = !IsMobile ? mnu_MyClanAdmin.Visible : false;

    }
    ManageClanPages  _manageClanPages;
    public ManageClanPages CurrentPage
    {
        get
        {
            return _manageClanPages;
        }
        set
        {
            _manageClanPages = value;
            SetSelectedLink(_manageClanPages);
        }
    }
    public enum ManageClanPages
    {
        Overview = 0,
        Public_Profile = 1,
        Invitations = 2,
        Members = 3,
        Forum = 4,
        Diplomacy = 5,
        Events=6,
        Settings=7,
        ForumAdmin=8,
        Email=9,
        Claims= 10
    }
    public bool IsMobile { get; set; }
    private void SetSelectedLink(ManageClanPages manageClanPages)
    {
        switch (manageClanPages)
        {
            case ManageClanPages.Overview:
                mnu_MyClan.Items[0].Selected = true;
                mnu_MyClan_m.Items[0].Selected = true;
                mnu_NoClan.Items[0].Selected = true;
                mnu_MyClanAdmin.Items[0].Selected = true;
                mnu_MyClanAdmin_m.Items[0].Selected = true;
                break;
            case ManageClanPages.Public_Profile:
                mnu_MyClan.Items[1].Selected = true;
                mnu_MyClan_m.Items[1].Selected = true;
                mnu_MyClanAdmin.Items[1].Selected = true;
                mnu_MyClanAdmin_m.Items[1].Selected = true;
                mnu_OtherClan.Items[0].Selected = true;
                break;
            case ManageClanPages.Invitations :
                mnu_MyClan.Items[2].Selected = true;
                mnu_MyClanAdmin.Items[2].Selected = true;
                mnu_MyClan_m.Items[2].Selected = true;
                mnu_MyClanAdmin_m.Items[2].Selected = true;
                break;
            case ManageClanPages.Members:
                mnu_MyClan.Items[3].Selected = true;
                mnu_MyClanAdmin.Items[3].Selected = true;
                mnu_MyClan_m.Items[3].Selected = true;
                mnu_MyClanAdmin_m.Items[3].Selected = true;
                mnu_OtherClan.Items[1].Selected = true;
                break;
            case ManageClanPages.Forum:
                mnu_MyClan.Items[4].Selected = true;
                mnu_MyClanAdmin.Items[4].Selected = true;
                break;
            case ManageClanPages.ForumAdmin:
                mnu_MyClanAdmin.Items[5].Selected = true;
                break;
            case ManageClanPages.Diplomacy:
                mnu_MyClan.Items[5].Selected = true;
                mnu_MyClanAdmin.Items[6].Selected = true;
                mnu_MyClan_m.Items[4].Selected = true;
                mnu_MyClanAdmin_m.Items[4].Selected = true;
                break;
            case ManageClanPages.Events:
                mnu_MyClan.Items[6].Selected = true;
                mnu_MyClanAdmin.Items[7].Selected = true;
                break;
            case ManageClanPages.Settings:
                mnu_MyClanAdmin.Items[8].Selected = true;
                mnu_MyClanAdmin_m.Items[5].Selected = true;
                break;
            case ManageClanPages.Email:
                mnu_MyClanAdmin.Items[9].Selected = true;
                break;
            case ManageClanPages.Claims:
                mnu_MyClanAdmin.Items[10].Selected = true;
                break;
            default:
                throw new ArgumentException("not recognized value of manageClanPages: " + manageClanPages.ToString());
        }
    }
    private Fbg.Bll.Player _player;
    public Fbg.Bll.Player Player
    {
        set
        {
            _player = value;
        }
        get 
        {
            return  _player;
        }
    }
}
