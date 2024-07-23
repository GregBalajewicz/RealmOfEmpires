using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Gmbc.Common.Diagnostics.ExceptionManagement;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;




public class WebRequestHelper 
{
   

    public enum HttpVerb
    {
        GET,
        POST,
        DELETE
    }


    /// <summary>
    /// Encode a dictionary of key/value pairs as an HTTP query string.
    /// </summary>
    /// <param name="dict">The dictionary to encode</param>
    /// <param name="questionMark">Whether or not to start it
    /// with a question mark (for GET requests)</param>
    private static  string EncodeDictionary(Dictionary<string, string> dict,
                                    bool questionMark)
    {
        StringBuilder sb = new StringBuilder();
        if (questionMark)
        {
            sb.Append("?");
        }
        foreach (KeyValuePair<string, string> kvp in dict)
        {
            sb.Append(HttpUtility.UrlEncode(kvp.Key));
            sb.Append("=");
            sb.Append(HttpUtility.UrlEncode(kvp.Value));
            sb.Append("&");
        }
        sb.Remove(sb.Length - 1, 1); // Remove trailing &
        return sb.ToString();
    }

    /// <summary>
    /// Make an HTTP request, with the given query args
    /// </summary>
    /// <param name="url">The URL of the request</param>
    /// <param name="verb">The HTTP verb to use</param>
    /// <param name="args">Dictionary of key/value pairs that represents
    /// the key/value pairs for the request</param>
    public static string MakeRequest(Uri url, HttpVerb httpVerb,
                               Dictionary<string, string> args)
    {
        #region performance logging
        DateTime beforeRun;
        TimeSpan duration;
        string functionName = url.AbsoluteUri;
        beforeRun = DateTime.Now;
        #endregion

        string result = "";
        if (args != null && args.Keys.Count > 0 && httpVerb == HttpVerb.GET)
        {
            url = new Uri(url.ToString() + EncodeDictionary(args, true));
        }

        
        HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
        
        // no longer needed since we've upgraded to 4.6
        //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        request.Method = httpVerb.ToString();

        if (httpVerb == HttpVerb.POST)
        {
            string postData = EncodeDictionary(args, false);

            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] postDataBytes = encoding.GetBytes(postData);

            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postDataBytes.Length;

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(postDataBytes, 0, postDataBytes.Length);
            requestStream.Close();
        }


        using (HttpWebResponse response
                = request.GetResponse() as HttpWebResponse)
        {
            StreamReader reader
                = new StreamReader(response.GetResponseStream());

            result = reader.ReadToEnd();
        }


        #region performance logging
        try
        {
            duration = DateTime.Now.Subtract(beforeRun);
            Fbg.DAL.Logger.LogEvent("WebRequest", functionName, url.AbsoluteUri, duration.Ticks);
        }
        catch (Exception)
        {

        }
        #endregion


        return result;
    }
   
}