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
using Gmbc.Common.Diagnostics.ExceptionManagement;
using Fbg.Bll;
using System.Collections.Generic;

public partial class Controls_BlockPerson : BaseControl
{
    private Fbg.Bll.Player _player;
    string _playerNameToBlock;

    public Controls_BlockPerson()
    {
        BaseResName = "BlockPerson.ascx"; 
    }

    protected void Page_Load(object sender, EventArgs e)
    {
    }

    public void Initalize(Player player, string playerNameToBlock)
    {
        Initalize(player, playerNameToBlock, String.Empty);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="player"></param>
    /// <param name="playerNameToBlock"></param>
    /// <param name="blockPlayerLinkText">set String.Empty if you dont want to change the default text</param>
    public void Initalize(Player player, string playerNameToBlock, string blockPlayerLinkText)
    {
        _player = player;
        _playerNameToBlock = playerNameToBlock;
        if (playerNameToBlock == null)
        {
            pnl_BlockPlayer.Visible = false;
        }
        else
        {
            pnl_BlockPlayer.Visible = true;
        }

        if (!String.IsNullOrEmpty(blockPlayerLinkText))
        {
            lnk_BlockPlayerTop.Text = blockPlayerLinkText;
        }

        if (!IsPostBack)
        {
            this.lblConfirm.DataBind();
        }
    }

    protected void lnk_BlockPlayer_Click(object sender, EventArgs e)
    {
        Fbg.Common.Mail.BlockPlayerResult result = Fbg.Bll.Mail.BlockPlayer(_player, _playerNameToBlock);
        if (result == Fbg.Common.Mail.BlockPlayerResult.Success)
        {
            lnk_BlockPlayerTop.Visible = false;
            lblConfirm.Visible = true;
        }
        else
        {
            lbl_Error.Visible = true;
            lbl_Error.Text = GetBlockPlayerMessage(result);
        }


    }

    public string GetBlockPlayerMessage(Fbg.Common.Mail.BlockPlayerResult result)
    {
        string msg = string.Empty;
        switch (result)
        {
            case Fbg.Common.Mail.BlockPlayerResult.Blocked_Player_Not_Exist:
                msg = RS("msg_plyrDontExits");
                break;
            case Fbg.Common.Mail.BlockPlayerResult.Cannot_Block_Yourself:
                msg = RS("msg_cantBlockSelf");
                break;
            case Fbg.Common.Mail.BlockPlayerResult.Player_Already_Blocked:
                msg = RS("msg_plyrIsBlocked");
                break;
            case Fbg.Common.Mail.BlockPlayerResult.Success:
                break;
            default:
                throw new Exception("Unrecognized value of Mail.BlockPlayerResult:" + result.ToString());
        }
        return msg;
    }
}