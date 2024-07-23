using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fbg.Bll;

public partial class InOutTroopsWidget2_d2 : TemplatePage
{
    public Realm realm;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (String.IsNullOrEmpty(Request.QueryString["rid"])) {
            throw new ArgumentException("rid expected in Request.QueryString");
        }

        realm = Realms.Realm(Convert.ToInt32(Request.QueryString["rid"]));

        //
        // this is ROE specific code. it reords the list based upon known types...  
        //
        List<Fbg.Bll.UnitType> uts = realm.GetUnitTypes();
        List<Fbg.Bll.UnitType> reorderedList = new List<Fbg.Bll.UnitType>(uts.Count);
        reorderedList.Add(uts.Find(delegate(Fbg.Bll.UnitType u) { return u.ID == 11; }));
        reorderedList.Add(uts.Find(delegate(Fbg.Bll.UnitType u) { return u.ID == 2; }));
        reorderedList.Add(uts.Find(delegate(Fbg.Bll.UnitType u) { return u.ID == 5; }));
        reorderedList.Add(uts.Find(delegate(Fbg.Bll.UnitType u) { return u.ID == 6; }));
        reorderedList.Add(uts.Find(delegate(Fbg.Bll.UnitType u) { return u.ID == 7; }));
        reorderedList.Add(uts.Find(delegate(Fbg.Bll.UnitType u) { return u.ID == 8; }));
        reorderedList.Add(uts.Find(delegate(Fbg.Bll.UnitType u) { return u.ID == 12; }));
        reorderedList.Add(uts.Find(delegate(Fbg.Bll.UnitType u) { return u.ID == 10; }));
     
        Repeater1.DataSource = reorderedList;
        Repeater1.DataBind();

        Repeater2.DataSource = reorderedList;
        Repeater2.DataBind();

    }
    public InOutTroopsWidget2_d2()
    {
        R_OverridePageName = "InOutTroopsWidget2.aspx";
    }
  
}