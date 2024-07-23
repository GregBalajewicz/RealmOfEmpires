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

public partial class Controls_TroopsMenu : BaseControl
{
    public enum ManageTroopsPages
    {
        CommandTroops
        ,RecruitTroops
        , Incoming
        ,Outgoing
        , TroopsAbroad
        , Support
        ,BattleSimulator
    }

    //ManageTroopsPages _manageTroopsPages;
    //public ManageTroopsPages CurrentPage
    //{
    //    get
    //    {
    //        return _manageTroopsPages;
    //    }
    //    set
    //    {
    //        _manageTroopsPages = value;
    //        SetSelectedLink(_manageTroopsPages);
    //    }
    //}

    private void SetSelectedLink(ManageTroopsPages manageTroopsPages) 
    {
        switch (manageTroopsPages)
        {
            case ManageTroopsPages.CommandTroops:
                linkCommand.Font.Bold = true;
                break;
            case ManageTroopsPages.RecruitTroops:
                linkRecruit.Font.Bold = true;
                break;
            case ManageTroopsPages.Incoming:
                linkIncoming.Font.Bold = true;
                break;
            case ManageTroopsPages.Outgoing:
                linkOutgoing.Font.Bold = true;
                break;
            case ManageTroopsPages.TroopsAbroad:
                linkAbroad.Font.Bold = true;
                break;
            case ManageTroopsPages.Support:
                linkSupport.Font.Bold = true;
                break;
            case ManageTroopsPages.BattleSimulator:
                linkSim.Font.Bold = true;
                break;
            default:
                throw new ArgumentException("not recognized value of manageTroopsPages: " + manageTroopsPages.ToString());
        }
    }

    //    protected void Page_Load(object sender, EventArgs e)
    //{
    //    BaseResName = "TroopsMenu.ascx";
    //}

        public Controls_TroopsMenu()
    {
        BaseResName = "TroopsMenu.ascx"; 
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="numVillages">the number of village the player has</param>
    public void Initialize(int numVillages, ManageTroopsPages manageTroopsPages)
    {
        if (numVillages == 1) 
        {
            linkAbroad.CssClass = String.Empty;
            linkAbroad.Text= linkAbroad.Text.Replace(".","");
            linkSupport.CssClass = String.Empty;
            linkSupport.Text = linkSupport.Text.Replace(".", "");
            linkIncoming.CssClass = String.Empty;
            linkIncoming.Text = linkIncoming.Text.Replace(".", "");
            linkOutgoing.CssClass = String.Empty;
            linkOutgoing.Text = linkOutgoing.Text.Replace(".", "");
        }

        SetSelectedLink(manageTroopsPages);

    }
}
