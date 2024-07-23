using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fbg.Bll;

public partial class templates_QuickCommandPopup : TemplatePage
{

    protected class UnitTypeWithWidth
    {
        public string widthclass { get; set; }
        public Fbg.Bll.UnitType ut { get; set; }
        public UnitTypeWithWidth(Fbg.Bll.UnitType ut, string width)
        {
            widthclass = width;
            this.ut = ut;
        }
    }
    public Realm realm;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (String.IsNullOrEmpty(Request.QueryString["rid"]))
        {
            throw new ArgumentException("rid expected in Request.QueryString");
        }

        realm = Realms.Realm(Convert.ToInt32(Request.QueryString["rid"]));

        //
        // this is ROE specific code. it reords the list based upon known types...  
        //
        List<Fbg.Bll.UnitType> uts = realm.GetUnitTypes();
        List<UnitTypeWithWidth> reorderedList = new List<UnitTypeWithWidth>(uts.Count);
        reorderedList.Add(new UnitTypeWithWidth(uts.Find(delegate(Fbg.Bll.UnitType u) { return u.ID == 11; }), ""));
        reorderedList.Add(new UnitTypeWithWidth(uts.Find(delegate(Fbg.Bll.UnitType u) { return u.ID == 2; }), ""));
        reorderedList.Add(new UnitTypeWithWidth(uts.Find(delegate(Fbg.Bll.UnitType u) { return u.ID == 5; }), "w4"));
        reorderedList.Add(new UnitTypeWithWidth(uts.Find(delegate(Fbg.Bll.UnitType u) { return u.ID == 6; }), "w4"));
        reorderedList.Add(new UnitTypeWithWidth(uts.Find(delegate(Fbg.Bll.UnitType u) { return u.ID == 7; }), "w3"));
        reorderedList.Add(new UnitTypeWithWidth(uts.Find(delegate(Fbg.Bll.UnitType u) { return u.ID == 8; }), "w3"));
        reorderedList.Add(new UnitTypeWithWidth(uts.Find(delegate(Fbg.Bll.UnitType u) { return u.ID == 12; }), "w3"));
        reorderedList.Add(new UnitTypeWithWidth(uts.Find(delegate(Fbg.Bll.UnitType u) { return u.ID == 10; }), "w1"));

        Repeater2.DataSource = reorderedList;
        Repeater2.DataBind();
        Repeater3.DataSource = reorderedList;
        Repeater3.DataBind();
    }

    public templates_QuickCommandPopup()
    {
       
       
        R_OverridePageName = "templates_QuickCommandPopup.aspx";
    }
}