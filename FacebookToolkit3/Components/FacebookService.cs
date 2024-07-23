using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Facebook.API;
using Facebook.Entity;
using Facebook.Exceptions;
using Facebook.Forms;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;
using Facebook.Types;

namespace Facebook.Components
{
    /// <summary>
    /// Provides various methods to use the Facebook Platform API.
    /// </summary>
#if !NETCF
    [ToolboxItem(true), ToolboxBitmap(typeof(FacebookService)), Designer(typeof(FacebookServiceDesigner))]
#endif
    public partial class FacebookService : Component
    {
        #region Private Data

        private readonly FacebookAPI_old _facebookAPI;

        private Uri _sendRequestResponseUrl;

        #endregion Private Data

        #region Accessors

        /// <summary>
        /// Access Key required to use the API
        /// </summary>
#if !NETCF
        [Category("Facebook"), Description("Access Key required to use the API")]
#endif
        public string ApplicationKey
        {
            get { return _facebookAPI.ApplicationKey; }
            set { _facebookAPI.ApplicationKey = value; }
        }

        /// <summary>
        /// User Id
        /// </summary>
#if !NETCF
        [Browsable(false)]
#endif
        public string UserId
        {
            get { return _facebookAPI.UserId; }
            set { _facebookAPI.UserId = value; }
        }

        /// <summary>
        /// Secret word
        /// </summary>
#if !NETCF
        [Category("Facebook"), Description("Secret Word")]
#endif
        public string Secret
        {
            get { return _facebookAPI.Secret; }
            set { _facebookAPI.Secret = value; }
        }

        /// <summary>
        /// same as AccessToken
        /// </summary>
#if !NETCF
        [Browsable(false)]
#endif
        public string SessionKey
        {
            get { return AccessToken; }
            set { AccessToken = value; }
        }

        public string AccessToken
        {
            get { return _facebookAPI.SessionKey; }
            set { _facebookAPI.SessionKey = value; }
        }

        /// <summary>
        /// Whether or not the session expires
        /// </summary>
#if !NETCF
        [Browsable(false)]
#endif
        public bool SessionExpires
        {
            get { return _facebookAPI.SessionExpires; }
        }

        /// <summary>
        /// Login Url
        /// </summary>
#if !NETCF
        [Browsable(false)]
#endif
        private string LoginUrl
        {
            get
            {
                object[] args = new object[2];
                args[0] = _facebookAPI.ApplicationKey;
                args[1] = _facebookAPI.AuthToken;

                return String.Format(CultureInfo.InvariantCulture, Facebook.Properties.Resources.FacebookLoginUrl, args);
            }
        }

		/// <summary>
		/// ExtendedPermissionUrl
		/// </summary>
#if !NETCF
		[Browsable(false)]
#endif
		private string ExtendedPermissionUrl(Enums.Extended_Permissions permission)
		{
			object[] args = new object[2];
			args[0] = _facebookAPI.ApplicationKey;
			args[1] = permission;

			return
				String.Format(CultureInfo.InvariantCulture, Facebook.Properties.Resources.FacebookRequestExtendedPermissionUrl, args);
		}

        /// <summary>
        /// LogOff Url
        /// </summary>
#if !NETCF
        [Browsable(false)]
#endif
        private string LogOffUrl
        {
            get
            {
                object[] args = new object[2];
                args[0] = _facebookAPI.ApplicationKey;
                args[1] = _facebookAPI.AuthToken;

                return String.Format(CultureInfo.InvariantCulture, Facebook.Properties.Resources.FacebookLogoutUrl, args);
            }
        }

        /// <summary>
        /// Whether or not this component is being used in a desktop application
        /// </summary>
#if !NETCF
        [Browsable(false)]
#endif
        public bool IsDesktopApplication
        {
            get { return _facebookAPI.IsDesktopApplication; }
            set { _facebookAPI.IsDesktopApplication = value; }
        }

        #endregion

        #region Constructors

        public FacebookService()
        {
            _facebookAPI = new FacebookAPI_old();
            InitializeComponent();
        }

        public FacebookService(IContainer container)
        {
            if (container != null)
                container.Add(this);

            _facebookAPI = new FacebookAPI_old();
            InitializeComponent();
        }

        #endregion Constuctors

        #region Public Methods

        /// <summary>
        /// Displays an integrated browser to allow the user to log on to the
        /// Facebook web page.
        /// </summary>
        public void ConnectToFacebook()
        {
            DialogResult result;
            SetAuthenticationToken();

            using (FacebookAuthentication formLogin = new FacebookAuthentication(LoginUrl))
            {
                result = formLogin.ShowDialog();
            }
            if (result == DialogResult.OK)
            {
                _facebookAPI.CreateSession();
            }
            else
            {
                throw new FacebookInvalidUserException("Login attempt failed");
            }
        }

        /// <summary>
        /// Creates a new session with Facebook.
        /// </summary>
        public void CreateSession(string accessToken)
        {            
            this.AccessToken = accessToken;
            _facebookAPI.SessionKey = accessToken;

            User u = GetUserInfo(false);
            UserId = u.UserId;
            _facebookAPI.UserId = UserId;
            //Facebook.FacebookAPI api = new Facebook.FacebookAPI(AccessToken);
            //Facebook.JSONObject me;
            //me = api.Get("/me");
            //UserId = me.Dictionary["id"].String;
            //_facebookAPI.UserId = UserId;
        }

        /// <summary>
        /// Sends a direct FQL query to Facebook.
        /// </summary>
        /// <param name="query">An FQL Query.</param>
        /// <returns>The result of the FQL query as an XML string.</returns> 
        public string DirectFQLQuery(string query)
        {
            string results = string.Empty;
            if (!IsSessionActive() && IsDesktopApplication)
            {
                ConnectToFacebook();
            }

            try
            {
                results = _facebookAPI.DirectFQLQuery(query);
            }
            catch (FacebookTimeoutException)
            {
                // Reconnect because of timed out session
                _facebookAPI.SessionKey = null;
                if (IsDesktopApplication)
                {
                    ConnectToFacebook();
                    DirectFQLQuery(query);
                }
                else
                {
                    throw;
                }
            }

            return results;
        }


        /// <summary>
        /// Get all the friends for the logged in user
        /// </summary>
        /// <returns>user profile of each friend</returns>
        public Collection<User> GetFriends()
        {
            if (!IsSessionActive() && IsDesktopApplication)
            {
                ConnectToFacebook();
            }
            return _facebookAPI.GetFriends();
        }


        /// <summary>
        /// Get all the friends for the logged in user that use the current application 
        /// </summary>
        /// <returns>user profile of each friend</returns>
        public Collection<User> GetFriendsNonAppUsers()
        {
            if (!IsSessionActive() && IsDesktopApplication)
            {
                ConnectToFacebook();
            }
            return _facebookAPI.GetFriendsNonAppUsers();
        }

        /// <summary>
        /// Get all the friends for the logged in user that use the current application 
        /// </summary>
        /// <returns>comma deliminated list of friends</returns>
        public string GetFriendsAppUsersList()
        {
            if (!IsSessionActive() && IsDesktopApplication)
            {
                ConnectToFacebook();
            }
            string results = string.Empty;

            try
            {
                // results = _facebookAPI.GetFriendsAppUsersList();




                Facebook.FacebookAPI api = new Facebook.FacebookAPI(AccessToken);
                api.Method = Method.api;
                Facebook.JSONObject result;

                result = api.Get("/method/friends.getAppUsers"
                    , new Dictionary<string, string>() { {"format", "json"}});

                StringBuilder friendsCommaDeliminated;
                if (result.IsArray)
                {
                    JSONObject[] friends = result.Array;
                    friendsCommaDeliminated = new StringBuilder(friends.Length * 10);
                    for (int i = 0; i < friends.Length; i++)
                    {
                        friendsCommaDeliminated.Append(friends[i].String);
                        friendsCommaDeliminated.Append(",");
                    }
                }
                else
                {
                    friendsCommaDeliminated = new StringBuilder(10);
                    friendsCommaDeliminated.Append(result.String);
                }
                return friendsCommaDeliminated.ToString();


            }
            catch (FacebookTimeoutException)
            {
                _facebookAPI.SessionKey = null;
                if (IsDesktopApplication)
                {
                    ConnectToFacebook();
                    GetFriendsAppUsersList();
                }
                else
                {
                    throw;
                }
            }

            return results;
        }


        /// <summary>
        /// Build the user profile for the logged in user. same as calling  GetUserInfo(false);
        /// </summary>
        /// <returns>user profile</returns>
        public User GetUserInfo()
        {
            return GetUserInfo(false);
        }
        /// <summary>
        /// Build the user profile for the logged in user
        /// </summary>
        /// <returns>user profile</returns>
        public User GetUserInfo(bool useCache)
        {
            User u=null;
            if (useCache && HttpContext.Current.Session["FBUSERCACHE"] != null)
            {
                u = (User)HttpContext.Current.Session["FBUSERCACHE"];
            }

            if (u == null)
            {
                try
                {
                    Facebook.FacebookAPI api = new Facebook.FacebookAPI(AccessToken);
                    Facebook.JSONObject me;
                    me = api.Get("/me");
                    u = new User();
                    u.LoadFromDic(me.Dictionary);
                    HttpContext.Current.Session["FBUSERCACHE"] = u;
                   
                }
                catch (FacebookAPIException e)
                {

                    throw new FacebookSessionExpiredException("", e);
                }
            }

            return u;

        }

        /// <summary>
        /// Build the user profile for the list of users
        /// </summary>
        /// <param name="userIds">Comma separated list of user ids</param>
        /// <returns>user profile list</returns>
        public Collection<User> GetUserInfo(string userIds)
        {
            if (!IsSessionActive() && IsDesktopApplication)
            {
                ConnectToFacebook();
            }
            return _facebookAPI.GetUserInfo(userIds);
        }



        /// <summary>
        /// ban a user
        /// </summary>
        /// <param name="facebookUserIDToBan">to ban</param>
        public bool BanUser(string facebookUserIDToBan)
        {
            return _facebookAPI.BanUser(facebookUserIDToBan);
        }
        /// <summary>
        /// unban a user
        /// </summary>
        /// <param name="facebookUserIDToBan">to unban</param>
        public bool UnBanUser(string facebookUserID)
        {
            return _facebookAPI.UnBanUser(facebookUserID);
        }

        /// <summary>
        /// Get the facebook user id of the user associated with the current session
        /// </summary>
        /// <returns>facebook userid</returns>
        public string GetLoggedInUser()
        {
            return GetUserInfo().UserId;
        }

        /// <summary>
        /// Determine if the current user is a user of this application already
        /// </summary>
        /// <returns>facebook userid</returns>
        public bool IsAppAdded()
        {
            /*
            if (!IsSessionActive() && IsDesktopApplication)
            {
                ConnectToFacebook();
            }
            bool results = false;
            try
            {
                results = _facebookAPI.IsAppAdded();
            }
            catch (FacebookTimeoutException)
            {
                _facebookAPI.SessionKey = null;
                _facebookAPI.UserId = null;
                if (IsDesktopApplication)
                {
                    ConnectToFacebook();
                    IsAppAdded();
                }
                else
                {
                    throw;
                }
            }
            
            return results;
             */

            return true;
        }


        #endregion

        #region Private Methods

        private bool IsSessionActive()
        {
            return _facebookAPI.IsSessionActive();
        }

        private void EnsureConnected()
        {
            if (!IsSessionActive() && IsDesktopApplication)
            {
                ConnectToFacebook();
            }
        }

        private Collection<string> ParseResponse(string response)
        {
            string decodedResponse = response.Replace("%5B", "[").Replace("%5D", "]");
            MatchCollection matches = Regex.Matches(decodedResponse, @"ids\[]=(\d+)");
            Collection<string> ids = new Collection<string>();

            foreach (Match match in matches)
            {
                string id = match.Groups[1].Value;
                ids.Add(id);
            }

            return ids;
        }

        /// <summary>
        /// Creates and sets a new authentication token.
        /// </summary>
        private void SetAuthenticationToken()
        {
            Dictionary<string, string> parameterList = new Dictionary<string, string>(2);
            parameterList.Add("method", "facebook.auth.createToken");

            XmlDocument xmlDocument = _facebookAPI.ExecuteApiCall(parameterList, false);

            // Get the authToken
            _facebookAPI.AuthToken =
                xmlDocument.SelectSingleNode("Facebook:auth_createToken_response", _facebookAPI.NsManager).InnerText;
        }

        #endregion

        #region Internal Methods

        internal void ReceiveRequestData(Uri responseUrl)
        {
            _sendRequestResponseUrl = responseUrl;
        }

        #endregion
    }
}