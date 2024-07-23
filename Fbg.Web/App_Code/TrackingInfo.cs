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

/// <summary>
/// Summary description for TrackingInfo
/// </summary>
public class TrackingInfo
{
    /// <summary>
    /// the raw number sent to us in the url. number like "0" or "4". 
    /// Fbg.Bll.CONSTS.UserLogEvents.CommunicationChannel needs to be added to it. 
    /// ChannelIDAsInt has this already added
    /// </summary>
    public string ChannelID { get; set; }
    /// <summary>
    /// Fbg.Bll.CONSTS.UserLogEvents.CommunicationChannel + ChannelID
    /// </summary>
    public int ChannelIDAsInt { get { return Fbg.Bll.CONSTS.UserLogEvents.CommunicationChannel + Convert.ToInt32(ChannelID); } }

    public string Message { get; set; }
    public string Data { get; set; }
}
