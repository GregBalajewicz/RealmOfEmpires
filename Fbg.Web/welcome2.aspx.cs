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
//using System.Web.UI;

public partial class Welcome2 : MyCanvasIFrameBasePage//System.Web.UI.Page
{
    //protected Gmbc.Common.GmbcBaseClass.Trace TRACE;



    protected void Page_Load(object sender, EventArgs e)
    {
        //TRACE = new Gmbc.Common.GmbcBaseClass.Trace("fbg.web", "fbg.web");
        string urlCommunicationChannelParams = string.Empty;
        string urlOtherImportantParams = string.Empty;
        string urlParams = string.Empty;

        #region if we got communication channel info, then lets not lose it
        //
        // if we got communication channel info, then lets not lose it
        //
        try
        {
            if (!String.IsNullOrEmpty(Request.QueryString[CONSTS.QuerryString.CommunicationChannelID]))
            {
                string channelID = Request.QueryString[CONSTS.QuerryString.CommunicationChannelID];
                string message = String.IsNullOrEmpty(Request.QueryString[CONSTS.QuerryString.CommunicationChannelMessage]) ? "" : Request.QueryString[CONSTS.QuerryString.CommunicationChannelMessage];
                string data = String.IsNullOrEmpty(Request.QueryString[CONSTS.QuerryString.CommunicationChannelData]) ? "" : Request.QueryString[CONSTS.QuerryString.CommunicationChannelData];

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
            ExceptionManager.Publish(new Exception("UNREPORTED EXCEPTION: error maintaining comm channel info", ex));
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

        linkTryAgain.NavigateUrl = "~/welcome.aspx" + urlParams;



        if (Request.Cookies[CONSTS.Cookies.AcceptsCookies] == null)
        {
            // cookie not accepted, stay on this page and tell the person this. 
            Fbg.Bll.User.LogEvent(Guid.Empty, null, Fbg.Bll.CONSTS.UserLogEvents.SomeoneHasCookiesDisabled, "", urlCommunicationChannelParams + ":" + Request.ServerVariables["HTTP_USER_AGENT"]);
        }
        else
        {
            //TRACE.ErrorLine("in welcome2");
            // cookie accepted, lets get back to choosing the realm. 
            Response.Cookies[CONSTS.Cookies.AcceptsCookies].Expires = DateTime.Now.AddDays(-1);
            if (Config.InDev || (Session["In_FB_Frame"] != null && (bool) Session["In_FB_Frame"] == true))
            {
                //TRACE.ErrorLine("welcome2: redir to chooserealm");
                //Response.Redirect(String.Format("Chooserealm.aspx?{0}&noredir=1", urlParams, true));
                Response.Redirect(String.Format("Chooserealm.aspx", urlParams, true));
            }
            else
            {
                //TRACE.ErrorLine("welcome2: redir to app.facebook");
                Response.Redirect(FacebookConfig.CanvasPageUrl_Full);
            }
        }
    }
}
