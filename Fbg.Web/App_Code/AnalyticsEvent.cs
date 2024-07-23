using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections.Generic;

/// <summary>
/// Summary description for TrackingInfo
/// </summary>
public class AnalyticsEvent
{
    public AnalyticsEvent() { attribs = new List<Attrib>(); RealmID = -1; }

    /// <summary>
    /// Specifying a realm ID, will ensure that the event fires only if we are collecting data on this realm. 
    /// Specify -1 to make sure the event fires on any realm
    /// </summary>
    public int RealmID{ get; set; }
    public string EventName { get; set; }
    public List<Attrib> attribs;


    public class Attrib {
        public string name{ get; set; }
        public string value{ get; set; }

        public Attrib(string name, string value) { this.name = name; this.value = value; }
        public Attrib(string name, object value) { this.name = name; this.value = value.ToString(); }
    }
}
