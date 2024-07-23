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
using Gmbc.Common.Diagnostics.ExceptionManagement;

public partial class masterNoCurVillageHeader : BaseMasterPage
{

    Fbg.Bll.Player _player;
    List<VillageBase> _villages;
    bool _getFullVillageObject;

    private bool _IsForumChanged;
    protected void Page_Load(object sender, EventArgs e)
    {
    }
  

    protected override void Render(HtmlTextWriter writer)
    {  
        base.Render(writer);
    }
    

    public void Initialize(Fbg.Bll.Player player, List<VillageBase> villages)
    {
        Initialize(player, villages, false);
    }
    public void Initialize(Fbg.Bll.Player player, List<VillageBase> villages, bool getFullVillageObject)
    {
        _player = player;
        _villages = villages;
        // tutorial needs the full village object so get it if it is running

    }
   
    /// <summary>
    /// Call Initialize again.
    /// When this is to be used? when ever a page changes something that effect the header. The call this so that the header will re-init itself.
    /// </summary>
    public void ReInitalize()
    {
        this.Initialize(_player, _villages, _getFullVillageObject);

    }

     public bool isMobile
    {
        get
        {
            return Utils.isMobile(Request);
        }
    }
    public bool isD2
    {
        get
        {
            return Utils.isD2(Request);
        }
    }
}
