using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Fbg.Bll;

public enum MessagesFolderType
{
    Inbox,
    Sent,
    Custom
}


public partial class Controls_MailMenu : BaseControl
{
    public Controls_MailMenu()
    {
        BaseResName = "MailMenu.ascx"; 
    }

    protected void Page_Load(object sender, EventArgs e)
    {
    }

    public enum MailMenuPages
    {
        CreateMail
        ,
        MessageList
       ,
        BlockedPlayers
        , none
    }

    private Player _player;
    private List<Folder> _folders;
    private bool _hasPF;
    private bool _showArchived;
    Folder _selectedFolder;
    MailMenuPages _mailMenuPages;
    MessagesFolderType _messageFolderType;
    public bool _isMobile;

    private MailMenuPages CurrentPage
    {
        get
        {
            return _mailMenuPages;
        }
        set
        {
            _mailMenuPages = value;
            SetSelectedLink(_mailMenuPages);
        }
    }

    /// <summary>
    /// The selected custom folder, null is none selected
    /// Valid only after calling Initialize(...)
    /// </summary>
    public Folder SelectedFolder
    {
        get
        {
            return _selectedFolder;
        }
    }

 
    /// <summary>
    /// What folder are looking at? inbox?send?or custom folder. If custom folder, see SelectedFolder property
    /// Valid only after calling Initialize(...).
    /// </summary>
    public MessagesFolderType MessagesFolderType
    {
        get
        {
            return _messageFolderType;
        }
    }

    /// <summary>
    /// If user wants to look at archived messages or not
    /// Valid only after calling Initialize(...)
    /// </summary>
    public bool ShowArchived
    {
        get
        {
            return _showArchived;
        }
    }



    public void Initialize(Fbg.Bll.Player player, MailMenuPages currentPage)
    {
        Initialize(player, currentPage, false);
    }

    public void Initialize(Fbg.Bll.Player player, MailMenuPages currentPage,bool isMobile)
    {
        _player = player;
        _isMobile = isMobile;
        CurrentPage = currentPage;
        _hasPF = _player.PF_HasPF(Fbg.Bll.CONSTS.PFs.MessageImprovements);
        _folders = _player.Folders.GetFolders(Folders.FolderType.Mail);
        HttpCookie selectedFolderCookie = null;
        string cookieKey_selectedFolder = _player.Realm + "_sf";
        string cookieKey_ShowArchived = _player.Realm + "_sa";
        int folderID = -1;

        _selectedFolder = null;
        _showArchived = false;
        //
        // find out:
        //  - what folder are we looking at
        //  - are we to display archived items. 
        //
        //  if we do not get this info from url, we look at the cookie. 
        //
        if (_hasPF)
        {
            if ((Request.QueryString[CONSTS.QuerryString.ViewArchived] != null && Request.QueryString[CONSTS.QuerryString.ViewArchived] != ""))
            {
                //  we got this info in the url
                _showArchived = Convert.ToBoolean(Request.QueryString[CONSTS.QuerryString.ViewArchived]);
            }
            else
            {
                // look at the cookie
                selectedFolderCookie = Request.Cookies[CONSTS.Cookies.Messages];
                if (selectedFolderCookie != null)
                {
                    string showArchivedStr = selectedFolderCookie[cookieKey_ShowArchived];
                    if (!String.IsNullOrEmpty(showArchivedStr))
                    {
                        _showArchived = Convert.ToBoolean(showArchivedStr);
                    }
                }
            }
        }
        else
        {
            _showArchived = false;
        }
        //
        // what 'folder' are we looking at ?
        if ((Request.QueryString[CONSTS.QuerryString.FolderID] != null && Request.QueryString[CONSTS.QuerryString.FolderID] != ""))
        {
            //
            // we got the folder in the url so we go with that
            folderID = Convert.ToInt32(Request.QueryString[CONSTS.QuerryString.FolderID]);
        }
        else
        {
            //
            // since no folder was specified, try to get it from the cookie
            selectedFolderCookie = Request.Cookies[CONSTS.Cookies.Messages];            
            if (selectedFolderCookie != null)
            {
                string selectedFolderIDStr = selectedFolderCookie[cookieKey_selectedFolder];
                if (!String.IsNullOrEmpty(selectedFolderIDStr))
                {
                    folderID = Convert.ToInt32(selectedFolderIDStr);
                }
            }
        }
        //
        // now that we got the folder, do some initalization depending on the folder
        //
        if (folderID == -1)
        {
            _messageFolderType = MessagesFolderType.Inbox;
        }
        else if (folderID == 0)
        {
            _messageFolderType = MessagesFolderType.Sent;
        }
        else
        {
            _messageFolderType = MessagesFolderType.Custom;
            _selectedFolder = _folders.Find(delegate(Folder f) { return f.ID == folderID; });
        }
        //
        // rememeber the folder, and showArchived in cookie. 
        //
        if (selectedFolderCookie == null)
        {
            selectedFolderCookie = new HttpCookie(CONSTS.Cookies.Messages);
        }
        selectedFolderCookie[cookieKey_selectedFolder] = folderID.ToString();
        selectedFolderCookie[cookieKey_ShowArchived] = _showArchived.ToString();
        Response.Cookies.Add(selectedFolderCookie);



        //
        // init list of folders
        //
        HyperLink h;
        h = new HyperLink();
        h.Text = RS("Inbox");
        h.CssClass = "folderPreDef";
        h.NavigateUrl = NavigationHelper.MessageList(-1);
        panelFolders.Controls.Add(h);

        h = new HyperLink();
        h.Text = RS("SentItems");
        h.CssClass = "folderPreDef";
        h.NavigateUrl = NavigationHelper.MessageList(0);
        panelFolders.Controls.Add(h);

        foreach (Folder folder in _folders)
        {
            h = new HyperLink();
            h.Text = folder.Name;

            if (!_hasPF)
            {
                h.Target = "_blank";
                h.Attributes.Add("OnClick", "return popupUnlock(this)");
                h.NavigateUrl = NavigationHelper.PFOffMessage(Fbg.Bll.CONSTS.PFs.MessageImprovements);
            }
            else
            {
                h.NavigateUrl = NavigationHelper.MessageList(folder.ID);
            }                        
            h.CssClass = " folder";
            panelFolders.Controls.Add(h);
        }

        h = new HyperLink();
        h.Text = RS("EditFolders");
        h.NavigateUrl = NavigationHelper.Folders(Folders.FolderType.Mail);
        h.CssClass = " configSml";
        panelFolders.Controls.Add(h);




        //
        // init some UI elements
        //
        if (_selectedFolder != null)
        {
            linkSelectedFolder.Text = _selectedFolder.Name;
            linkSelectedFolder.NavigateUrl = NavigationHelper.MessageList(_selectedFolder.ID);
        }
        else
        {

            if (_messageFolderType == MessagesFolderType.Inbox)
            {
                linkSelectedFolder.NavigateUrl = NavigationHelper.MessageList(-1);
                linkSelectedFolder.Text = RS("Inbox");
            }
            else
            {
                linkSelectedFolder.NavigateUrl = NavigationHelper.MessageList(0);
                linkSelectedFolder.Text = RS("SentItems");
            }
        }

        // MOBILE
        folders.Visible = !isMobile;

    }

    private void SetSelectedLink(MailMenuPages mailMenuPages)
    {
        switch (mailMenuPages)
        {
            case MailMenuPages.CreateMail:
                linkCreateMail.Font.Bold = true;
                if (_isMobile) {
                    linkCreateMail.CssClass = "SelectedMenuItem";
                }
                break;
            case MailMenuPages.MessageList:
                linkSelectedFolder.Font.Bold = true;
                if (_isMobile) {
                    HyperLink1.CssClass = "SelectedMenuItem";
                }
                break;
            case MailMenuPages.BlockedPlayers:
                linkBlockedPlayers.Font.Bold = true;
                if (_isMobile) {
                linkBlockedPlayers.CssClass = "SelectedMenuItem";
                }
                break;
            case MailMenuPages.none:
                break;
            default:
                throw new ArgumentException("not recognized value of mailMenuPages: " + mailMenuPages.ToString());
        }
    }
}
