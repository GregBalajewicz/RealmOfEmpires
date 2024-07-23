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


public abstract class MasterBase_PopupFullFunct : BaseMasterPage
{
    //protected static string villagesScriptTemplate = "myVillages = [{0}];";
    //protected static string oneVillageScriptTemplate = "{{ 'id':{0},'name':'{1}','X':'{2}','Y':'{3}','P':'{4}','S':'{5}'}},";

    //public Fbg.Bll.Player _player;
    //protected List<VillageBase> _villages;
    //protected bool _getFullVillageObject;

    //protected bool _IsForumChanged;

    //public MasterBase_PopupFullFunct()
    //{
		
    //}



    abstract public Fbg.Bll.Village CurrentlySelectedVillage
    {
        get;
    }

    ///// <summary>
    ///// This gives you access to the currently selected village in form of VillageBasicB.
    ///// If you called Initialize(...) and passed in true for 'getFullVillageObject', then you can also 
    ///// get the currently selected village in form of a Village object from CurrentlySelectedVillage
    ///// </summary>
    //abstract public Fbg.Bll.VillageBasicB CurrentlySelectedVillageBasicB
    //{
    //    get;
    //}
    public void Initialize(Fbg.Bll.Player player, List<VillageBase> villages)
    {
        Initialize(player, villages, false);
    }
    public abstract void Initialize(Fbg.Bll.Player player, List<VillageBase> villages, bool getFullVillageObject);



    abstract public bool isMobile { get; }

    abstract public CONSTS.Device isDevice { get; }

    abstract public void SetDefaultBackgroundColor();
    
}