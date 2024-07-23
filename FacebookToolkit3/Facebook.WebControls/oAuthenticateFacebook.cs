using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Web;
using System.Collections.Specialized;
using System.Web.Script.Serialization;

namespace Facebook.WebControls
{

    public class oAuthFacebook
    {
        public enum Method { GET, POST };
        public const string AUTHORIZE = "https://graph.facebook.com/oauth/authorize";
        public const string ACCESS_TOKEN = "https://graph.facebook.com/oauth/access_token";
        //public const string CALLBACK_URL = "http://gmbc.selfip.com:9000/simwar_a/chooserealm.aspx";

        private string _consumerKey = "";
        private string _consumerSecret = "";
        private string _token = "";

        #region Properties

        public string ConsumerKey
        {
            get
            {
                if (_consumerKey.Length == 0)
                {
                    throw new InvalidCastException("ConsumerKey not set");
                }
                return _consumerKey;
            }
            set { _consumerKey = value; }
        }

        public string ConsumerSecret
        {
            get
            {
                if (_consumerSecret.Length == 0)
                {
                    throw new InvalidCastException("ConsumerSecret not set");
                }
                return _consumerSecret;
            }
            set { _consumerSecret = value; }
        }
        public string ReturnUri
        {
            get;
            set;
        }

        public string AdditionalPermissionsToRequest
        {
            get;
            set;
        }

        public string Token { get { return _token; } set { _token = value; } }

        #endregion

        /// <summary>
        /// Get the link to Facebook's authorization page for this application.
        /// </summary>
        /// <returns>The url with a valid request token, or a null string.</returns>
        public string AuthorizationLinkGet()
        {
            return string.Format("{0}?client_id={1}&redirect_uri={2}{3}", AUTHORIZE, this.ConsumerKey, this.ReturnUri
                , String.IsNullOrEmpty(AdditionalPermissionsToRequest) ? "" : String.Format("&scope={0}", AdditionalPermissionsToRequest));
        }

        class AccessTokenReturnJson {
            public string access_token;
            public string token_type;
            public int expires_in;
        }

  
        /// <summary>
        /// Exchange the Facebook "code" for an access token.
        /// </summary>
        /// <param name="authToken">The oauth_token or "code" is supplied by Facebook's authorization page following the callback.</param>
        public void AccessTokenGet(string authToken)
        {
            this.Token = authToken;
            string accessTokenUrl = string.Format("{0}?client_id={1}&redirect_uri={2}&client_secret={3}&code={4}",
            ACCESS_TOKEN, this.ConsumerKey, this.ReturnUri, this.ConsumerSecret, authToken);
            string ER;

            //ER = "GREG:" + accessTokenUrl;
            //Fbg.DAL.Logger.LogEvent("G&F", "access_token part1", accessTokenUrl, 1);

            string response =string.Empty;
            try
            {
                response = WebRequest(Method.GET, accessTokenUrl, String.Empty);
            }
            catch (System.Net.WebException webex)
            {
                if (webex.Message.Contains("The remote server returned an error: (400) Bad Request"))
                {
                    throw new Facebook.Exceptions.FacebookSessionExpiredException("got '(400) Bad Request'", webex);
                }
                else
                {
                    throw webex;
                }
            }

            if (response.Length > 0)
            {
                //Store the returned access_token
                NameValueCollection qs = HttpUtility.ParseQueryString(response);

               // throw new Exception(ER + "      --           " + response);
                JavaScriptSerializer json_serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                AccessTokenReturnJson at = json_serializer.Deserialize<AccessTokenReturnJson>(response);

                //throw new Exception(ER + "      --           " + at.access_token);
                
                    this.Token = at.access_token;
               
            }
        }

        /// <summary>
        /// Web Request Wrapper
        /// </summary>
        /// <param name="method">Http Method</param>
        /// <param name="url">Full url to the web resource</param>
        /// <param name="postData">Data to post in querystring format</param>
        /// <returns>The web server response.</returns>
        public string WebRequest(Method method, string url, string postData)
        {

            HttpWebRequest webRequest = null;
            StreamWriter requestWriter = null;
            string responseData = "";

            webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
            webRequest.Method = method.ToString();
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.UserAgent = "[You user agent]";
            webRequest.Timeout = 20000;

            if (method == Method.POST)
            {
                webRequest.ContentType = "application/x-www-form-urlencoded";

                #region performance logging
                DateTime beforeRun;
                TimeSpan duration;
                beforeRun = DateTime.Now;
                #endregion 

                //POST the data.
                requestWriter = new StreamWriter(webRequest.GetRequestStream());

                #region performance logging
                try
                {
                    duration = DateTime.Now.Subtract(beforeRun);
                    Fbg.DAL.Logger.LogEvent("FBAPI", url, postData, duration.Ticks);
                }
                catch (Exception)
                {

                }
                #endregion

                try
                {
                    requestWriter.Write(postData);
                }
                catch
                {
                    throw;
                }

                finally
                {
                    requestWriter.Close();
                    requestWriter = null;
                }
            }
            try
            {
                responseData = WebResponseGet(webRequest);
            }
            catch (Exception ex)
            {
                Exception ex2 = new Exception("error executing: " +webRequest.RequestUri.AbsoluteUri, ex);
                throw ex2;
            }
            webRequest = null;
            return responseData;
        }

        /// <summary>
        /// Process the web response.
        /// </summary>
        /// <param name="webRequest">The request object.</param>
        /// <returns>The response data.</returns>
        public string WebResponseGet(HttpWebRequest webRequest)
        {
            StreamReader responseReader = null;
            string responseData = "";

            try
            {
                responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
                responseData = responseReader.ReadToEnd();
            }
            catch
            {
                throw;
            }
            finally
            {
                webRequest.GetResponse().GetResponseStream().Close();
                responseReader.Close();
                responseReader = null;
            }

            return responseData;
        }
    }

}
