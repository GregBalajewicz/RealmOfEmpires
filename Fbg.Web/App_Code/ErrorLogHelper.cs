using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gmbc.Common.Diagnostics.ExceptionManagement;


/// <summary>
/// Summary description for ErrorLogHelper
/// </summary>
public class ErrorLogHelper
{
    public ErrorLogHelper()
    {

    }

    public static string GetFullErrorMessage(HttpRequest request, HttpResponse response, System.Web.SessionState.HttpSessionState session
        , BaseApplicationException bex, Fbg.Bll.Player player)
    {
        try
        {

            //
            // Request cookies
            //
            HttpCookie cookie;
            foreach (string cookieName in request.Cookies.AllKeys)
            {
                cookie = request.Cookies[cookieName];
                bex.AdditionalInformation.Add("request.Cookies[" + cookieName + "].Value", cookie.Value);
                foreach (String s in cookie.Values.AllKeys)
                {
                    bex.AdditionalInformation.Add("request.Cookies[" + cookie.Name + "].Values[" + s + "]", cookie.Values[s]);
                }
            }
            //
            // Response cookies
            //
            foreach (string cookieName in response.Cookies.AllKeys)
            {
                cookie = response.Cookies[cookieName];
                bex.AdditionalInformation.Add("Response.Cookies[" + cookieName + "].Value", cookie.Value);
                foreach (String s in cookie.Values.AllKeys)
                {
                    bex.AdditionalInformation.Add("Response.Cookies[" + cookie.Name + "].Values[" + s + "]", cookie.Values[s]);
                }
            }

            //
            // Request headers
            //
            foreach (String s in request.Headers.AllKeys)
            {
                bex.AdditionalInformation.Add("request.Headers[" + s + "]", request.Headers[s]);
            }


            //
            // Form
            //
            foreach (String s in request.Form.AllKeys)
            {
                bex.AdditionalInformation.Add("request.Form[" + s + "]", request.Form[s]);
            }
            //
            // QueryString
            //
            foreach (String s in request.QueryString.AllKeys)
            {
                bex.AdditionalInformation.Add("request.QueryString[" + s + "]", request.QueryString[s]);
            }

            //
            // request.ServerVariables
            //
            foreach (String s in request.ServerVariables.AllKeys)
            {
                bex.AdditionalInformation.Add("request.ServerVariables[" + s + "]", request.ServerVariables[s]);
            }

            //
            // Session 
            //
            foreach (string s in session.Keys)
            {
                object o = session[s];
                if (s != "fullErrorMessage")
                {
                    bex.AddAdditionalInformation( "Session[" + s + "]", o);
                }
            }
        }
        catch (Exception ex)
        {
            bex.AdditionalInformation.Add("Error occured in error.Page_Load while gathering context data:", ex.Message);
        }

        DateTime now = DateTime.Now;
        return "[[ GUID ]] : n/a"
                        + "[[ Date ]] :" + now.ToString("yyyy MM dd HH:mm:ss:fffff") + Environment.NewLine
                        + "[[ Player ]] :" + (player == null ? "null" : player.Name) + " {0}"
                        + ExceptionManager.SerializeToString(bex);

    }


}