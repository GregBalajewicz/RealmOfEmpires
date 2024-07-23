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


//
// purpose of this page : this page is mostly used to track where a person is coming from via the url param communication channel
//
public partial class welcome : System.Web.UI.Page
{
    string urlCommunicationChannelParams = string.Empty;
    string urlOtherImportantParams = string.Empty;
    string urlParams = string.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        #region if we got communication channel info, then lets not lose it

        //
        // if we got communication channel info, then save it in session so that we do not lose 
        //  it when person is asked to authorize the app. 
        //
        try
        {
            if (!String.IsNullOrEmpty(Request.QueryString[CONSTS.QuerryString.CommunicationChannelID]))
            {
                string channelID = Request.QueryString[CONSTS.QuerryString.CommunicationChannelID];
                string message = String.IsNullOrEmpty(Request.QueryString[CONSTS.QuerryString.CommunicationChannelMessage]) ? "" : Request.QueryString[CONSTS.QuerryString.CommunicationChannelMessage];
                string data = String.IsNullOrEmpty(Request.QueryString[CONSTS.QuerryString.CommunicationChannelData]) ? "" : Request.QueryString[CONSTS.QuerryString.CommunicationChannelData];

                Session[CONSTS.Session.CommunicationChannelTrackingObject] = new TrackingInfo() { ChannelID = channelID, Message = message, Data = data };             

                urlCommunicationChannelParams = String.Format("{0}={1}&{2}={3}&{4}={5}"
                    , CONSTS.QuerryString.CommunicationChannelID, channelID
                    , CONSTS.QuerryString.CommunicationChannelMessage, message
                    , CONSTS.QuerryString.CommunicationChannelData, data);
            }

        }
        catch (Exception ex)
        {
            //
            // this is not an essential functionlity so we eat the exception
            ExceptionManager.Publish(new Exception("UNREPORTED EXCEPTION: failure saving communication channel info", ex));
        }
        #endregion

        #region if we got other imporatnt URL param, maintain those as well.
        //
        // if we got other imporatnt URL param, maintain those as well. 
        //
        try
        {
            if (!String.IsNullOrEmpty(Request.QueryString[CONSTS.QuerryString.GoToCreatePlayerImmediatelly]))
            {
                string GoToCreatePlayerImmediatelly = Request.QueryString[CONSTS.QuerryString.GoToCreatePlayerImmediatelly];

                urlOtherImportantParams = String.Format("{0}={1}"
                    , CONSTS.QuerryString.GoToCreatePlayerImmediatelly, GoToCreatePlayerImmediatelly);
            }

        }
        catch (Exception ex)
        {
            //
            // this is not an essential functionlity so we eat the exception
            ExceptionManager.Publish(new Exception("UNREPORTED EXCEPTION: error maintaining other important params", ex));
        }
        #endregion
        //
        // construct the url params based on params we got 
        //
        urlParams = urlCommunicationChannelParams;
        if (!String.IsNullOrEmpty(urlOtherImportantParams))
        {
            if (!String.IsNullOrEmpty(urlParams))
            {
                urlParams += "&" + urlOtherImportantParams;
            }
            else
            {
                urlParams = urlOtherImportantParams;

            }
        }

        if (!IsPostBack)
        {
            //
            // test to make sure browser accepts cookies. this cookie we only use to make sure the browser accepts cookies
            //
            if (Request.QueryString[CONSTS.QuerryString.AcceptsCookies] == null)
            {
                Response.Cookies[CONSTS.Cookies.AcceptsCookies].Value = "ok";
                Response.Redirect("welcome2.aspx?" + urlParams, false);
                return;
            }

        }
    }
}
