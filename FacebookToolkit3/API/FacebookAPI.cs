using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml;
using Facebook.Entity;
using Facebook.Exceptions;
using Facebook.Parser;
using Facebook.Types;
using Facebook.Utility;
using Gmbc.Common.Diagnostics.ExceptionManagement;

namespace Facebook.API {

    /// <summary>
    /// Provides various methods to use the Facebook Platform API.
    /// </summary>
    public class FacebookAPI_old
    {
        #region Constants

        private const string ANDCLAUSE = " AND";
        private const string NEWLINE = "\r\n";
        private const string PREFIX = "--";
        private const string VERSION = "1.0";

        #endregion

        #region Private Data

        private string _applicationKey = string.Empty;
        private string _authToken = string.Empty;
        private bool _isDesktopApp = true;
        private XmlNamespaceManager _nsManager;
        private string _secret;
        private bool _sessionExpires;
        private string _sessionKey;
        private string _userId;

#if NETCF 
        private CultureInfo _installedCulture = CultureInfo.CurrentUICulture;
#else
        private CultureInfo _installedCulture = CultureInfo.InstalledUICulture;
#endif

        #endregion Private Data

        #region Constructors

        public FacebookAPI_old() {
        }

        #endregion Constuctors

        #region Properties

        /// <summary>
        /// Access Key required to use the API
        /// </summary>
        public string ApplicationKey {
            get { return String.IsNullOrEmpty(_applicationKey) ? string.Empty : _applicationKey; }
            set { _applicationKey = value == null ? string.Empty : value.Trim(); }
        }

        /// <summary>
        /// Whether or not this component is being used in a desktop application
        /// </summary>
        public bool IsDesktopApplication {
            get { return _isDesktopApp; }
            set { _isDesktopApp = value; }
        }

        /// <summary>
        /// Whether or not the session expires
        /// </summary>
        public bool SessionExpires {
            get { return _sessionExpires; }
        }

        /// <summary>
        /// Secret word
        /// </summary>
        public string Secret {
            get { return _secret; }
            set { _secret = value; }
        }

        /// <summary>
        /// Session key
        /// </summary>
        public string SessionKey {
            get { return _sessionKey; }
            set { _sessionKey = value; }
        }

        /// <summary>
        /// User Id
        /// </summary>
        public string UserId {
            get { return _userId; }
            set { _userId = value; }
        }

        /// <summary>
        /// Authorization token
        /// </summary>
        public string AuthToken {
            get { return _authToken; }
            set { _authToken = value; }
        }

        /// <summary>
        /// XML Namespace Manager
        /// </summary>
        public XmlNamespaceManager NsManager {
            get { return _nsManager; }
        }

        internal CultureInfo InstalledCulture
        {
            get { return _installedCulture; }
        }


        #endregion Properties
        #region Public Methods

        /// <summary>
        /// Creates a new session with Facebook.
        /// </summary>
        /// <param name="authToken">The auth token received from Facebook.</param>
        public void CreateSession(string authToken) {
            AuthToken = authToken;
            CreateSession();
        }

        /// <summary>
        /// Sends a direct FQL query to FB
        /// </summary>
        /// <param name="query">FQL Query</param>
        /// <returns>Result of the FQL query as an XML string</returns> 
        public string DirectFQLQuery(string query) {
            Dictionary<string, string> parameterList = new Dictionary<string, string>(3);
            parameterList.Add("method", "facebook.fql.query");

            if (!String.IsNullOrEmpty(query)) {
                parameterList.Add("query", query);
            }
            else {
                throw new FacebookException("Query string is required");
            }
            return ExecuteApiCallString(parameterList, true);
        }


        /// <summary>
        /// Gets all events for the logged in user.
        /// </summary>
        /// <returns>A collection of the events.</returns>
        public Collection<FacebookEvent> GetEvents() {
            return GetEvents(null, _userId, null, null);
        }

        /// <summary>
        /// Get all event ids for the logged in user.
        /// </summary>
        /// <returns>A collection of the event IDs.</returns>
        public Collection<string> GetEventIds()
        {
            Collection<string> eventIds = new Collection<string>();

            string xml = GetEventIdsXML();

            if (!String.IsNullOrEmpty(xml))
            {
                XmlDocument xmlDocument = LoadXMLDocument(xml);

                XmlNodeList nodeList = xmlDocument.GetElementsByTagName("fql_query_response");

                if (nodeList != null && nodeList.Count > 0)
                {
                    XmlNodeList results = xmlDocument.GetElementsByTagName("event_member");

                    foreach (XmlNode node in results)
                    {
                        eventIds.Add(XmlHelper.GetNodeText(node, "eid"));
                    }
                }
            }

            return eventIds;
        }

        /// <summary>
        /// Gets the XML representation of the ids of all events for the logged in user.
        /// </summary>
        /// <returns>Event id list as raw XML</returns>
        public string GetEventIdsXML()
        {
            Dictionary<string, string> parameterList = new Dictionary<string, string>(3);
            parameterList.Add("method", "facebook.fql.query");

            if (string.IsNullOrEmpty(_userId))
            {
                throw new FacebookException("User Id is required");
            }

            parameterList.Add("query", String.Format(CultureInfo.InvariantCulture,
                "SELECT eid FROM event_member WHERE uid = {0}", _userId));
            return ExecuteApiCallString(parameterList, true);
        }


        /// <summary>
        /// Get all events for the specified user.
        /// </summary>
        /// <param name="userId">User to return events for</param>
        /// <returns>event list</returns>
        public Collection<FacebookEvent> GetEvents(string userId) {
            return GetEvents(null, userId, null, null);
        }

        /// <summary>
        /// Get all events in the list of events.
        /// </summary>
        /// <param name="eventList">list of event ids</param>
        /// <returns>event list</returns>
        public Collection<FacebookEvent> GetEvents(Collection<string> eventList) {
            return GetEvents(eventList, null, null, null);
        }

        /// <summary>
        /// Get all events for the specified user and list of events
        /// </summary>
        /// <param name="userId">User to return events for</param>
        /// <param name="eventList">list of event ids</param>
        /// <returns>event list</returns>
        public Collection<FacebookEvent> GetEvents(Collection<string> eventList, string userId) {
            return GetEvents(eventList, userId, null, null);
        }

        /// <summary>
        /// Get all events for the specified user and list of events between 2 dates
        /// </summary>
        /// <param name="userId">User to return events for</param>
        /// <param name="eventList">list of event ids</param>
        /// <param name="startDate">events occuring after this date</param>
        /// <param name="endDate">events occuring before this date</param>
        /// <returns>event list</returns>
        public Collection<FacebookEvent> GetEvents(Collection<string> eventList, string userId, DateTime? startDate,
                                                   DateTime? endDate) {
            string xml = GetEventsXML(eventList, userId, startDate, endDate);


            Collection<FacebookEvent> events = new Collection<FacebookEvent>();

            if (!String.IsNullOrEmpty(xml)) {
                XmlDocument xmlDocument = LoadXMLDocument(xml);

                XmlNodeList nodeList = xmlDocument.GetElementsByTagName("fql_query_response");

                if (nodeList != null && nodeList.Count > 0) {
                    if (nodeList[0].HasChildNodes){ 
                        XmlNodeList results = xmlDocument.GetElementsByTagName("event");
                        foreach (XmlNode node in results) {
                            events.Add(FacebookEventParser.ParseEvent(node));
                        }
                    }
                }
            }
            return events;
        }

        /// <summary>
        /// Get the XML representation of all events for the specified user and list of events between 2 dates
        /// </summary>
        /// <param name="userId">User to return events for</param>
        /// <param name="eventList">list of event ids</param>
        /// <param name="startDate">events occuring after this date</param>
        /// <param name="endDate">events occuring before this date</param>
        /// <returns>event list as raw XML</returns> 
        public string GetEventsXML(Collection<string> eventList, string userId, DateTime? startDate, DateTime? endDate) {
            if (eventList == null || eventList.Count == 0)
            {
                eventList = GetEventIds();
            }

            Dictionary<string, string> parameterList = new Dictionary<string, string>(3);
            parameterList.Add("method", "facebook.fql.query");

            string eidClause = string.Empty;
            string uidClause = string.Empty;
            string startClause = string.Empty;
            string endClause = string.Empty;

            // Build an FQL query for retrieving events based on search criteria
            // Use FQL instead of direct REST call to insulate from REST changes
            // Similar to events.get in REST api
            eidClause = String.Concat(" eid IN (", StringHelper.ConvertToCommaSeparated(eventList), ")");

            if (!String.IsNullOrEmpty(userId)) {
                uidClause = String.Concat(ANDCLAUSE, " eid IN (SELECT eid FROM event_member WHERE uid=", userId, ")");
            }

            if (startDate != null) {
                startClause =
                    String.Concat(ANDCLAUSE," end_time >=",
                                  DateHelper.ConvertDateToDouble(startDate.Value).ToString(
                                      CultureInfo.InvariantCulture));
            }
            if (endDate != null) {                
                endClause =
                    String.Concat(ANDCLAUSE, " start_time <",
                                  DateHelper.ConvertDateToDouble(endDate.Value).ToString(
                                      CultureInfo.InvariantCulture));
            }
            parameterList.Add("query",
                              String.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}{4}",
                                            "SELECT eid, name, tagline, nid, pic, pic_big, pic_small, host, description, event_type, event_subtype, start_time, end_time, creator, update_time, location, venue FROM event WHERE",
                                            eidClause, uidClause, startClause, endClause));

            return ExecuteApiCallString(parameterList, true);
        }

        /// <summary>
        /// Get all event members for the specified event
        /// </summary>
        /// <param name="eventId">Event to return users for</param>
        /// <returns>evet user list with rsvp status</returns>
        public Collection<EventUser> GetEventMembers(string eventId) {
            Collection<EventUser> eventUsers = new Collection<EventUser>();
            string xml = GetEventMembersXML(eventId);

            if (!String.IsNullOrEmpty(xml)) {
                XmlDocument xmlDocument = LoadXMLDocument(xml);

                XmlNodeList nodeList = xmlDocument.GetElementsByTagName("fql_query_response");

                if (nodeList != null && nodeList.Count > 0) {
                    if (nodeList[0].HasChildNodes) {
                        XmlNodeList results = xmlDocument.GetElementsByTagName("event_member");
                        Dictionary<string, EventUser> eventUserDict = new Dictionary<string, EventUser>();
                        foreach (XmlNode node in results) {
                            EventUser eventUser = EventUserParser.ParseEventUser(node);
                            eventUserDict.Add(eventUser.UserId, eventUser);
                        }

                        // Get the profile information for every user invited to this event
                        Collection<User> users = GetUserInfo(StringHelper.ConvertToCommaSeparated(eventUserDict.Keys));
                        foreach (User user in users) {
                            if (eventUserDict.ContainsKey(user.UserId)) {
                                //populate the user portion of the EventUser
                                eventUserDict[user.UserId].User = user;
                                eventUsers.Add(eventUserDict[user.UserId]);
                            }
                        }
                    }
                }
            }
            return eventUsers;
        }

        /// <summary>
        /// Get all event members for the specified event
        /// </summary>
        /// <param name="eventId">Event to return users for</param>
        /// <returns>evet user list with rsvp status</returns>
        public string GetEventMembersXML(string eventId) {
            Dictionary<string, string> parameterList = new Dictionary<string, string>(3);
            parameterList.Add("method", "facebook.fql.query");

            // Build an FQL query for retrieving members for a particular event
            // Use FQL instead of direct REST call to insulate from REST changes
            // Similar to events.getMembers in REST api
            if (!string.IsNullOrEmpty(eventId)) {
                parameterList.Add("query",
                                  String.Format(CultureInfo.InvariantCulture, "{0}{1}",
                                                "SELECT uid, eid, rsvp_status FROM event_member WHERE eid=", eventId));
            }
            else {
                throw new FacebookException("Event Id is required");
            }

            return ExecuteApiCallString(parameterList, true);
        }

        /// <summary>
        /// Get all the friends for the logged in user
        /// </summary>
        /// <returns>user profile of each friend</returns>
        public Collection<User> GetFriends() {
            
            // Get the profile information for all the friends that were found
            Collection<string> friendList = GetFriendIds();
            
            return GetUserInfo(StringHelper.ConvertToCommaSeparated(friendList));
            
        }
        /// <summary>
        /// Get all the friends for the logged in user
        /// </summary>
        /// <returns>user profile of each friend</returns>
        public Collection<string> GetFriendIds()
        {
            Collection<string> friendList = new Collection<string>();

            string xml = GetFriendsXML();

            if (!String.IsNullOrEmpty(xml))
            {
                XmlDocument xmlDocument = LoadXMLDocument(xml);

                XmlNodeList nodeList = xmlDocument.GetElementsByTagName("fql_query_response");

                if (nodeList != null && nodeList.Count > 0)
                {
                    XmlNodeList results = xmlDocument.GetElementsByTagName("friend_info");

                    foreach (XmlNode node in results)
                    {
                        friendList.Add(XmlHelper.GetNodeText(node, "uid2"));
                    }
                }
            }

            return friendList;
        }

        /// <summary>
        /// Get all the friends for the logged in user and returns the results as raw XML
        /// </summary>
        /// <returns>The XMl representation of the user profile of each friend</returns>
        public string GetFriendsXML() {
            Dictionary<string, string> parameterList = new Dictionary<string, string>(3);
            parameterList.Add("method", "facebook.fql.query");

            if (!string.IsNullOrEmpty(_userId)) {
                parameterList.Add("query",
                                  String.Format(CultureInfo.InvariantCulture, "{0}{1}",
                                                "SELECT uid2 FROM friend WHERE uid1=", _userId));
            }
            else {
                throw new FacebookException("User Id is required");
            }

            return ExecuteApiCallString(parameterList, true);
        }

        /// <summary>
        /// Get all the friends for the logged in user that do not use the current application 
        /// </summary>
        /// <returns>user profile of each friend</returns>
        public Collection<User> GetFriendsNonAppUsers()
        {
            Collection<User> friends = new Collection<User>();
            Collection<string> friendList = GetFriendIds();
            string xml = GetFriendsAppUsersXML();

            if (!String.IsNullOrEmpty(xml))
            {
                XmlDocument xmlDocument = LoadXMLDocument(xml);
                    
                XmlNodeList nodeList = xmlDocument.GetElementsByTagName("fql_query_response");

                if (nodeList != null && nodeList.Count > 0)
                {
                    if (nodeList[0].HasChildNodes)
                    {
                        XmlNodeList results = xmlDocument.GetElementsByTagName("user");
                        
                        foreach (XmlNode node in results)
                        {
                            friendList.Remove(XmlHelper.GetNodeText(node, "uid"));
                        }
                    }
                }
            }
            if (friendList.Count > 0)
            {
                friends = GetUserInfo(StringHelper.ConvertToCommaSeparated(friendList));
            }
            return friends;
        }

        /// <summary>
        /// Get all the friends for the logged in user that use the current application 
        /// </summary>
        /// <returns>user profile of each friend</returns>
        public Collection<User> GetFriendsAppUsers() {
            Collection<User> friends = new Collection<User>();
            string xml = GetFriendsAppUsersXML();

            if (!String.IsNullOrEmpty(xml)) {
                XmlDocument xmlDocument = LoadXMLDocument(xml);

                XmlNodeList nodeList = xmlDocument.GetElementsByTagName("fql_query_response");

                if (nodeList != null && nodeList.Count > 0) {
                    if (nodeList[0].HasChildNodes) {
                        XmlNodeList results = xmlDocument.GetElementsByTagName("user");
                        Collection<string> friendList = new Collection<string>();

                        if (results.Count > 0) {
                            foreach (XmlNode node in results) {
                                friendList.Add(XmlHelper.GetNodeText(node, "uid"));
                            }
                            friends = GetUserInfo(StringHelper.ConvertToCommaSeparated(friendList));
                        }
                        else {
                            return friends;
                        }
                    }
                }
            }
            return friends;
        }
        /// <summary>
        /// Get all the friends for the logged in user that use the current application 
        /// </summary>
        /// <returns>user profile of each friend</returns>
        public string GetFriendsAppUsersList()
        {
            System.Text.StringBuilder friendList = new StringBuilder(0);
            string xml = GetFriendsAppUsersXML();

            if (!String.IsNullOrEmpty(xml))
            {
                XmlDocument xmlDocument = LoadXMLDocument(xml);

                XmlNodeList nodeList = xmlDocument.GetElementsByTagName("fql_query_response");

                if (nodeList != null && nodeList.Count > 0)
                {
                    if (nodeList[0].HasChildNodes)
                    {
                        XmlNodeList results = xmlDocument.GetElementsByTagName("user");
                        friendList = new System.Text.StringBuilder(results.Count * 8);

                        if (results.Count > 0)
                        {
                            foreach (XmlNode node in results)
                            {
                                friendList.Append(XmlHelper.GetNodeText(node, "uid"));
                                friendList.Append(",");
                            }                            
                        }
                    }
                }
            }
            return friendList.ToString() ;
        }

        /// <summary>
        /// Get all the friends for the logged in user that use the current application 
        /// </summary>
        /// <returns>user profile of each friend</returns>
        public string GetFriendsAppUsersXML() {
            Dictionary<string, string> parameterList = new Dictionary<string, string>(3);
            parameterList.Add("method", "facebook.fql.query");

            // Build an FQL query for retrieving friends that are also users of this application
            // Use FQL instead of direct REST call to insulate from REST changes
            // Similar to friends.getAppUsers in REST api
            if (!string.IsNullOrEmpty(_userId)) {
                parameterList.Add("query",
                                  String.Format(CultureInfo.InvariantCulture, "{0}{1}{2}",
                                                "SELECT uid FROM user WHERE uid IN (SELECT uid2 FROM friend WHERE uid1=",
                                                _userId, ") AND is_app_user"));
            }
            else {
                throw new FacebookException("User Id is required");
            }

            return ExecuteApiCallString(parameterList, true);
        }

        /// <summary>
        /// Determine if the two specified users are friends
        /// </summary>
        /// <param name="user1">User to check</param>
        /// <param name="user2">User to check</param>
        /// <returns>whether specified users are friends or not</returns>
        public bool AreFriends(User user1, User user2) {
            if (user1 == null) {
                throw new ArgumentNullException("user1");
            }

            if (user2 == null) {
                throw new ArgumentNullException("user2");
            }

            return AreFriends(user1.UserId, user2.UserId);
        }

        /// <summary>
        /// Determine if the two specified users are friends
        /// </summary>
        /// <param name="userId1">User to check</param>
        /// <param name="userId2">User to check</param>
        /// <returns>whether specified users are friends or not</returns>
        public bool AreFriends(string userId1, string userId2) {
            Dictionary<string, string> parameterList = new Dictionary<string, string>();
            parameterList.Add("method", "facebook.fql.query");

            // Build an FQL query for determining if 2 users are facebook friends
            // Use FQL instead of direct REST call to insulate from REST changes
            // Similar to friends.areFriends in REST api
            if (!string.IsNullOrEmpty(userId1) && !string.IsNullOrEmpty(userId2)) {
                parameterList.Add("query",
                                  String.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}",
                                                "SELECT uid1, uid2 FROM friend WHERE uid1=", userId1, " AND uid2=",
                                                userId2));
            }
            else
                throw new FacebookException("userids are empty or null");

            XmlDocument xmlDocument = ExecuteApiCall(parameterList, true);

            XmlNodeList nodeList = xmlDocument.GetElementsByTagName("fql_query_response");
            bool areFriends = false;
            if (nodeList != null && nodeList.Count > 0) {
                if (nodeList[0].HasChildNodes) {
                    areFriends = true;
                }
            }

            return areFriends;
        }

        /// <summary>
        /// returns if the application is added
        /// </summary>
        /// <returns>user profile list</returns>
        public bool IsAppAdded()
        {
            bool results = false;

            Dictionary<string, string> parameterList = new Dictionary<string, string>();
            string xml = null;

            try
            {
                parameterList.Add("method", "users.isAppUser");
                xml = ExecuteApiCallString(parameterList, true);
                if (!String.IsNullOrEmpty(xml))
                {
                    XmlDocument xmlDocument = LoadXMLDocument(xml);

                    XmlNodeList nodeList = xmlDocument.GetElementsByTagName("users_isAppUser_response");

                    if (nodeList == null || nodeList.Count == 0)
                    {
                        nodeList = xmlDocument.GetElementsByTagName("users_isappuser_response");
                    }


                    if (nodeList != null && nodeList.Count > 0)
                    {
                        if (nodeList[0].HasChildNodes)
                        {
                            //throw new Exception(nodeList[0].InnerText);
                            if (nodeList[0].InnerText == "1")
                            {
                                results = true;
                            }
                            else
                            {
                                results = false;
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("UNRECOGNIZED XML RESPONSE:" + xmlDocument.InnerXml);
                    }
                }
            }
            catch (Exception e)
            {
                BaseApplicationException bex = new BaseApplicationException("error in IsAppAdded()", e);
                bex.AddAdditionalInformation("parameterList", parameterList);
                bex.AddAdditionalInformation("xml", xml);
                bex.AddAdditionalInformation("results", results);
                throw bex;
            }

            return results;
        }


        /// <summary>
        /// Build the user profile for the logged in user
        /// </summary>
        /// <returns>user profile</returns>
        public User GetUserInfo() {
            Collection<User> users = GetUserInfo(_userId);
            if (users.Count > 0)
                return users[0];

            return null;
        }

        /// <summary>
        /// Build the user profile for the list of users
        /// </summary>
        /// <param name="userIds">Comma separated list of user ids</param>
        /// <returns>user profile list</returns>
        public Collection<User> GetUserInfo(string userIds) {
            Collection<User> users = new Collection<User>();
            string xml = GetUserInfoXml(userIds);

            if (!String.IsNullOrEmpty(xml)) {
                XmlDocument xmlDocument = LoadXMLDocument(xml);

                XmlNodeList nodeList = xmlDocument.GetElementsByTagName("fql_query_response");

                if (nodeList != null && nodeList.Count > 0) {
                    if (nodeList[0].HasChildNodes) {
                        XmlNodeList results = xmlDocument.GetElementsByTagName("user");
                        foreach (XmlNode node in results) {
                            users.Add(UserParser.ParseUser(node));
                        }
                    }
                }
            }
            return users;
        }


        /// <summary>
        /// Build the user profile for the list of users
        /// </summary>
        /// <param name="userIds">A collection of userId strings</param>
        /// <returns>user profile list</returns>
        public Collection<User> GetUserInfo(Collection<string> userIds) {
            StringBuilder paramBuilder = new StringBuilder();

            // Simply parse the collection and let the overload handle it.
            for (int i = 0; i < userIds.Count; i++) {
                if (i != 0)
                    paramBuilder.Append(',');
                paramBuilder.Append(userIds[i]);
            }

            return GetUserInfo(paramBuilder.ToString());
        }


        /// <summary>
        /// Builds the user profile for the list of users and returns the results as raw xml
        /// </summary>
        /// <param name="userIds">Comma separated list of user ids</param>
        /// <returns>The xml representation of the user profile list</returns>
        public string GetUserInfoXml(string userIds) {
            Dictionary<string, string> parameterList = new Dictionary<string, string>(3);

            parameterList.Add("method", "facebook.fql.query");
            parameterList.Add("query",
                              String.Format(CultureInfo.InvariantCulture, "{0}{1}{2}",
                                            "SELECT uid, first_name, last_name, name, sex, pic_square FROM user WHERE uid IN (",
                                            userIds, ")"));
            //parameterList.Add("query",
            //      String.Format(CultureInfo.InvariantCulture, "{0}{1}{2}",
            //                    "SELECT uid, first_name, last_name, name, pic_small, pic_big, pic_square, pic, affiliations, profile_update_time, timezone, religion, birthday, sex, hometown_location, meeting_sex, meeting_for, relationship_status, significant_other_id, political, current_location, activities, interests, is_app_user, music, tv, movies, books, quotes, about_me, hs_info, education_history, work_history, notes_count, wall_count, status FROM user WHERE uid IN (",
            //                    userIds, ")"));

            return ExecuteApiCallString(parameterList, true);
        }

		/// <summary>
		/// Update your status on Facebook, it will not include "IS" prepended.
		/// 
		/// Requires Facebook extended permission of "status_update"
		/// </summary>
		/// <param name="statusMessage">Your status message to be posted</param>
		public string SetStatus(string statusMessage)
		{
			return SetStatus(statusMessage, false);
		}

		/// <summary>
		/// Update your status on Facebook
		/// 
		/// Requires Facebook extended permission of "status_update"
		/// </summary>
		/// <param name="statusMessage">Your status message to be posted</param>
		/// <param name="IncludeIsVerb">Have your message start with the word "is"</param>
		public string SetStatus(string statusMessage, bool IncludeIsVerb)
		{
			string results = string.Empty;

			Dictionary<string, string> parameterList = new Dictionary<string, string>(5);
			parameterList.Add("method", "Users.setStatus");
			parameterList.Add("status", statusMessage);
			parameterList.Add("status_includes_verb", IncludeIsVerb.ToString());

			string xml = ExecuteApiCallString(parameterList, true);
			if (!String.IsNullOrEmpty(xml))
			{
				XmlDocument xmlDocument = LoadXMLDocument(xml);

				XmlNodeList nodeList = xmlDocument.GetElementsByTagName("users_setStatus_response");

				if (nodeList != null && nodeList.Count > 0)
				{
					if (nodeList[0].HasChildNodes)
					{
						results = nodeList[0].InnerText;
					}
				}
			}
			return results;
		}

		/// <summary>
		/// Do you have permission to do the task needed?
		/// </summary>
		/// <param name="permission">Your status message to be posted</param>
		public bool HasPermission(Enums.Extended_Permissions permission)
		{
			string results = string.Empty;

			Dictionary<string, string> parameterList = new Dictionary<string, string>(5);
			parameterList.Add("method", "Users.hasAppPermission");
			parameterList.Add("ext_perm", permission.ToString());

			string xml = ExecuteApiCallString(parameterList, true);
			if (!String.IsNullOrEmpty(xml))
			{
				XmlDocument xmlDocument = LoadXMLDocument(xml);

				XmlNodeList nodeList = xmlDocument.GetElementsByTagName("Users_hasAppPermission_response");

				if (nodeList != null && nodeList.Count > 0)
				{
					if (nodeList[0].HasChildNodes)
					{
						results = nodeList[0].InnerText;
					}
				}
			}
			return (results == "1");
		}

        /// <summary>
        /// Return the notifications
        /// </summary>
        /// <returns>user profile list</returns>
        public Notifications GetNotifications() {
            Notifications notifications = new Notifications();
            string xml = GetNotificationsXml();

            if (!String.IsNullOrEmpty(xml)) {
                XmlDocument xmlDocument = LoadXMLDocument(xml);
                XmlNodeList nodeList = xmlDocument.GetElementsByTagName("notifications_get_response");
                if (nodeList != null && nodeList.Count > 0) {
                    if (nodeList[0].HasChildNodes) {
                        XmlNode node = nodeList[0];
                        notifications = NotificationsParser.ParseNotifications(node);
                    }
                }
            }
            return notifications;
        }

        /// <summary>
        /// Builds the list of notifications and returns the results as raw xml
        /// </summary>
        /// <returns>The xml representation of the user profile list</returns>
        public string GetNotificationsXml() {
            Dictionary<string, string> parameterList = new Dictionary<string, string>(2);

            parameterList.Add("method", "facebook.notifications.get");

            return ExecuteApiCallString(parameterList, true);
        }

        /// <summary>
        /// Get the photos for a specified album
        /// </summary>
        /// <param name="albumId">The album</param>
        /// <returns>photos</returns>
        public Collection<Photo> GetPhotos(string albumId) {
            return GetPhotos(albumId, null, null);
        }

        /// <summary>
        /// Get the photos for a specified list of photos
        /// </summary>
        /// <param name="photoList">Collection of photo ids</param>
        /// <returns>photos</returns>
        public Collection<Photo> GetPhotos(Collection<string> photoList) {
            return GetPhotos(null, photoList, null);
        }

        /// <summary>
        /// Get the photos for a specified User
        /// </summary>
        /// <param name="user">The user</param>
        /// <returns>photos</returns>
        public Collection<Photo> GetPhotos(User user) {
            return GetPhotos(null, null, user);
        }

        /// <summary>
        /// Get the photos for a specified album and list of photos
        /// </summary>
        /// <param name="albumId">The album</param>
        /// <param name="photoList">Collection of photo ids</param>
        /// <returns>photos</returns>
        public Collection<Photo> GetPhotos(string albumId, Collection<string> photoList) {
            return GetPhotos(albumId, photoList, null);
        }

        /// <summary>
        /// Get the photos for a specified album, photo list, and User
        /// </summary>
        /// <param name="albumId">The album</param>
        /// <param name="photoList">Collection of photo ids</param>
        /// <param name="user">The user</param>
        /// <returns>photos</returns>
        public Collection<Photo> GetPhotos(string albumId, Collection<string> photoList, User user) {
            Collection<Photo> photos = new Collection<Photo>();
            string xml = GetPhotosXML(albumId, photoList, user);

            if (!String.IsNullOrEmpty(xml)) {
                XmlDocument xmlDocument = LoadXMLDocument(xml);

                XmlNodeList nodeList = xmlDocument.GetElementsByTagName("fql_query_response");

                if (nodeList != null && nodeList.Count > 0) {
                    if (nodeList[0].HasChildNodes) {
                       XmlNodeList results = xmlDocument.GetElementsByTagName("photo");
                        foreach (XmlNode node in results) {
                            photos.Add(PhotoParser.ParsePhoto(node));
                        }
                    }
                }
            }
            return photos;
        }

        /// <summary>
        /// Get the photos for a specified album and list of photos
        /// </summary>
        /// <param name="albumId">The album</param>
        /// <param name="photoList">Collection of photo ids</param>
        /// <returns>photos</returns>
        [Obsolete("We now include a more extensive version which includes a User as well")]
        public string GetPhotosXML(string albumId, Collection<string> photoList) {
            // Eventually, this version should be depreciated.
            // Simply call the newer function.
            return GetPhotosXML(albumId, photoList, null);
        }

        /// <summary>
        /// Get the photos for a specified album, list of photos, and User
        /// </summary>
        /// <param name="albumId">The album</param>
        /// <param name="photoList">Collection of photo ids</param>
        /// <param name="user">The user</param>
        /// <returns>photos</returns>
        public string GetPhotosXML(string albumId, Collection<string> photoList, User user) {
            Dictionary<string, string> parameterList = new Dictionary<string, string>(3);
            parameterList.Add("method", "facebook.fql.query");
            string aidClause = string.Empty;
            string pidClause = string.Empty;
            string uidClause = string.Empty;

            // Build an FQL query for determining retrieving the photos for an album or list of photos
            // Use FQL instead of direct REST call to insulate from REST changes
            // Similar to photos.get in REST api
            if (!String.IsNullOrEmpty(albumId)) {
                aidClause = String.Concat(" aid =", albumId);
            }
            if (photoList != null) {
                if (!String.IsNullOrEmpty(albumId)) {
                    pidClause = ANDCLAUSE;
                }
                pidClause =
                    String.Concat(pidClause, " pid IN (", StringHelper.ConvertToCommaSeparated(photoList), ")");
            }
            if ((user != null) && (!String.IsNullOrEmpty(user.UserId))) {
                if ((!String.IsNullOrEmpty(albumId)) || (photoList != null)) {
                    uidClause = ANDCLAUSE;
                }
                uidClause =
                    String.Concat(uidClause, " pid IN (SELECT pid FROM photo_tag WHERE subject=", user.UserId, ")");
            }
            parameterList.Add("query",
                              String.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}",
                                            "SELECT pid, aid, owner, src, src_big, src_small, link, caption, created FROM photo WHERE",
                                            aidClause, pidClause, uidClause));

            return ExecuteApiCallString(parameterList, true);
        }

        /// <summary>
        /// Get the albums for the logged in user
        /// </summary>
        /// <returns>albums</returns>
        public Collection<Album> GetPhotoAlbums() {
            return GetPhotoAlbums(_userId, null);
        }

        /// <summary>
        /// Get the albums for the specified user
        /// </summary>
        /// <param name="userId">user to return albums for</param>
        /// <returns>albums</returns>
        public Collection<Album> GetPhotoAlbums(string userId) {
            return GetPhotoAlbums(userId, null);
        }

        /// <summary>
        /// Get the albums for the specified list of albums
        /// </summary>
        /// <param name="albumList">collection of album ids</param>
        /// <returns>albums</returns>
        public Collection<Album> GetPhotoAlbums(Collection<string> albumList) {
            return GetPhotoAlbums(null, albumList);
        }

        /// <summary>
        /// Get the albums for the specified user and list of albums
        /// </summary>
        /// <param name="userId">user to return albums for</param>
        /// <param name="albumList">collection of album ids</param>
        /// <returns>albums</returns>
        public Collection<Album> GetPhotoAlbums(string userId, Collection<string> albumList) {
            string xml = GetPhotoAlbumsXML(userId, albumList);
            Collection<Album> albums = new Collection<Album>();

            if (!String.IsNullOrEmpty(xml)) {
                XmlDocument xmlDocument = LoadXMLDocument(xml);

                XmlNodeList nodeList = xmlDocument.GetElementsByTagName("fql_query_response");

                if (nodeList != null && nodeList.Count > 0) {
                    if (nodeList[0].HasChildNodes) {
                        XmlNodeList results = xmlDocument.GetElementsByTagName("album");
                        foreach (XmlNode node in results) {
                            albums.Add(AlbumParser.ParseAlbum(node));
                        }
                    }
                }
            }
            return albums;
        }

        /// <summary>
        /// Get the albums for the specified user and list of albums
        /// </summary>
        /// <param name="userId">user to return albums for</param>
        /// <param name="albumList">collection of album ids</param>
        /// <returns>albums</returns>
        public string GetPhotoAlbumsXML(string userId, Collection<string> albumList) {
            Dictionary<string, string> parameterList = new Dictionary<string, string>(3);
            parameterList.Add("method", "facebook.fql.query");
            string uidClause = string.Empty;
            string aidClause = string.Empty;

            if (!String.IsNullOrEmpty(userId)) {
                uidClause = String.Concat(" owner =", userId);
            }
            if (albumList != null) {
                if (!String.IsNullOrEmpty(userId)) {
                    aidClause = ANDCLAUSE;
                }
                aidClause =
                    String.Concat(aidClause, " aid IN (", StringHelper.ConvertToCommaSeparated(albumList), ")");
            }
            parameterList.Add("query",
                              String.Format(CultureInfo.InvariantCulture, "{0}{1}{2}",
                                            "SELECT aid, cover_pid, owner, name, created, modified, description, location, size FROM album WHERE",
                                            uidClause, aidClause));

            return ExecuteApiCallString(parameterList, true);
        }

        /// <summary>
        /// Get the tags for the specifed photos
        /// </summary>
        /// <param name="photoList">collection of photo ids</param>
        /// <returns>photo tags</returns>
        public Collection<PhotoTag> GetTags(Collection<string> photoList) {
            string xml = GetTagsXML(photoList);
            Collection<PhotoTag> photoTags = new Collection<PhotoTag>();

            if (!String.IsNullOrEmpty(xml)) {
                XmlDocument xmlDocument = LoadXMLDocument(xml);

                XmlNodeList nodeList = xmlDocument.GetElementsByTagName("fql_query_response");

                if (nodeList != null && nodeList.Count > 0) {
                    if (nodeList[0].HasChildNodes) {
                        XmlNodeList results = xmlDocument.GetElementsByTagName("photo_tag");
                        foreach (XmlNode node in results) {
                            photoTags.Add(PhotoTagParser.ParsePhotoTag(node));
                        }
                    }
                }
            }
            return photoTags;
        }

        /// <summary>
        /// Get the tags for the specifed photos
        /// </summary>
        /// <param name="photoList">collection of photo ids</param>
        /// <returns>photo tags</returns>
        public string GetTagsXML(Collection<string> photoList) {
            Dictionary<string, string> parameterList = new Dictionary<string, string>(3);
            parameterList.Add("method", "facebook.fql.query");
            if (photoList != null && photoList.Count > 0) {
                parameterList.Add("query",
                                  String.Format(CultureInfo.InvariantCulture, "{0}{1}",
                                                "SELECT pid, subject, xcoord, ycoord FROM photo_tag WHERE pid IN",
                                                StringHelper.ConvertToCommaSeparated(photoList)));
            }
            else {
                throw new FacebookException("photo list not specified");
            }

            return ExecuteApiCallString(parameterList, true);
        }

        /// <summary>
        /// Gets the FBML on the logged-in user's profile.
        /// </summary>
        /// <returns>The FBML from the user's profile.</returns>
        public string GetFBML()
        {
            return GetFBML(null);
        }

        /// <summary>
        /// Gets the FBML on a user's profile
        /// </summary>
        /// <param name="userId">The id of the user to get the FBML from.</param>
        /// <returns>The FBML markup from the user's profile.</returns>
        public string GetFBML(string userId)
        {
            string results = string.Empty;

            Dictionary<string, string> parameterList = new Dictionary<string, string>(1);
            parameterList.Add("method", "facebook.profile.getFBML");

            if (!string.IsNullOrEmpty(userId))
            {
                parameterList.Add("uid", userId);
            }

            string xml = ExecuteApiCallString(parameterList, true);
            if (!String.IsNullOrEmpty(xml))
            {
                XmlDocument xmlDocument = LoadXMLDocument(xml);

                XmlNodeList nodeList = xmlDocument.GetElementsByTagName("profile_getFBML_response");

                if (nodeList != null && nodeList.Count > 0)
                {
                    if (nodeList[0].HasChildNodes)
                    {
                        results = nodeList[0].InnerText;
                    }
                }
            }
            return results;
        }

        /// <summary>
        /// Set the FBML on a profile
        /// </summary>
        /// <param name="profileFBML">Profile FBML markup</param>
        /// <param name="profileActionFBML">Profile Action FBML markup</param>
        /// <param name="mobileFBML">Mobile Profile FBML markup</param>
        /// <param name="userId">user id</param>
        public string SetFBML(string profileFBML, string profileActionFBML, string mobileFBML, string userId)
        {
            string results = string.Empty;

            Dictionary<string, string> parameterList = new Dictionary<string, string>(3);
            parameterList.Add("method", "facebook.profile.setFBML");

            if (!string.IsNullOrEmpty(profileFBML))
            {
                parameterList.Add("profile", string.Format(_installedCulture, profileFBML.Replace("{", "{{").Replace("}", "}}")));
            }

            if (!string.IsNullOrEmpty(profileActionFBML))
            {
                parameterList.Add("profile_action",string.Format(_installedCulture,profileActionFBML.Replace("{", "{{").Replace("}", "}}")));
            }

            if (!string.IsNullOrEmpty(mobileFBML))
            {
                parameterList.Add("mobile_fbml",string.Format(_installedCulture, mobileFBML.Replace("{", "{{").Replace("}", "}}")));
            }

            if (!string.IsNullOrEmpty(userId))
            {
                parameterList.Add("uid", string.Format(_installedCulture, userId));
            }

            string xml = ExecuteApiCallString(parameterList, true);
            if (!String.IsNullOrEmpty(xml))
            {
                XmlDocument xmlDocument = LoadXMLDocument(xml);

                XmlNodeList nodeList = xmlDocument.GetElementsByTagName("profile_setFBML_response");

                if (nodeList != null && nodeList.Count > 0)
                {
                    if (nodeList[0].HasChildNodes)
                    {
                        results = nodeList[0].InnerText;
                    }
                }
            }


            return results;
        }

        #region Deprecated Facebook API: Unsupported as of 1/17/2008

        ///// <summary>
        ///// Set the FBML on a profile
        ///// </summary>
        ///// <param name="markup">html markup</param>
        //public string SetFBML(string markup)
        //{
        //    return SetFBML(markup, null);
        //}

        ///// <summary>
        ///// Set the FBML on a profile
        ///// </summary>
        ///// <param name="markup">html markup</param>
        ///// <param name="userId">user id</param>
        //public string SetFBML(string markup, string userId)
        //{
        //    string results = string.Empty;

        //    Dictionary<string, string> parameterList = new Dictionary<string, string>(3);
        //    parameterList.Add("method", "facebook.profile.setFBML");
        //    parameterList.Add("markup", string.Format(_installedCulture, markup.Replace("{", "{{").Replace("}", "}}")));
        //    if (!string.IsNullOrEmpty(userId))
        //    {
        //        parameterList.Add("uid", string.Format(_installedCulture, userId));
        //    }

        //    string xml = ExecuteApiCallString(parameterList, true);
        //    if (!String.IsNullOrEmpty(xml))
        //    {
        //        XmlDocument xmlDocument = LoadXMLDocument(xml);

        //        XmlNodeList nodeList = xmlDocument.GetElementsByTagName("profile_setFBML_response");

        //        if (nodeList != null && nodeList.Count > 0)
        //        {
        //            if (nodeList[0].HasChildNodes)
        //            {
        //                results = nodeList[0].InnerText;
        //            }
        //        }
        //    }

        //    return results;
        //}

        ///// <summary>
        ///// Set the FBML on a profile
        ///// </summary>
        ///// <param name="markup">html markup</param>
        //public string SetFBML(string markup) {
        //    return SetFBML(markup, null);
        //}

        #endregion Deprecated Facebook API: Unsupported as of 1/17/2008

        /// <summary>
        /// Associates a specified handle with a piece of FBML markup.
        /// </summary>
        /// <param name="handle">The handle to use as a reference.</param>
        /// <param name="markup">The FBML markup to be referenced.</param>
        /// <returns>The result of the method call as a string.</returns>
        public string SetRefHandle(string handle, string markup)
        {
            string results = string.Empty;

            Dictionary<string, string> parameterList = new Dictionary<string, string>(3);
            parameterList.Add("method", "facebook.fbml.setRefHandle");
            parameterList.Add("handle", string.Format(_installedCulture, handle));
            parameterList.Add("fbml", string.Format(_installedCulture, markup));
            string xml = ExecuteApiCallString(parameterList, true);
            if (!String.IsNullOrEmpty(xml))
            {
                XmlDocument xmlDocument = LoadXMLDocument(xml);

                XmlNodeList nodeList = xmlDocument.GetElementsByTagName("fbml_setRefHandle_response");

                if (nodeList != null && nodeList.Count > 0)
                {
                    if (nodeList[0].HasChildNodes)
                    {
                        results = nodeList[0].InnerText;
                    }
                }
            }
            return results;
        }

        /// <summary>
        /// Tells Facebook to fetches and re-cache the content stored at the
        /// given URL for use in a fb:ref FBML tag.
        /// </summary>
        /// <param name="url">The URL of the content to refresh.</param>
        /// <returns>The result of the method call as a string.</returns>
        public string RefreshRefUrl(string url)
        {
            string results = string.Empty;

            Dictionary<string, string> parameterList = new Dictionary<string, string>(2);
            parameterList.Add("method", "facebook.fbml.refreshRefUrl");
            parameterList.Add("url", string.Format(_installedCulture, url));
            string xml = ExecuteApiCallString(parameterList, true);
            if (!String.IsNullOrEmpty(xml))
            {
                XmlDocument xmlDocument = LoadXMLDocument(xml);

                XmlNodeList nodeList = xmlDocument.GetElementsByTagName("fbml_refreshRefUrl_response");

                if (nodeList != null && nodeList.Count > 0)
                {
                    if (nodeList[0].HasChildNodes)
                    {
                        results = nodeList[0].InnerText;
                    }
                }
            }
            return results;
        }

        /// <summary>
        /// Tells Facebook to fetche and re-cache the image stored at the given
        /// URL for use in non-canvas pages on Facebook.
        /// </summary>
        /// <param name="url">The URL of the image to refresh.</param>
        /// <returns>The result of the method call as a string.</returns>
        public string RefreshImgSrc(string url)
        {
            string results = string.Empty;

            Dictionary<string, string> parameterList = new Dictionary<string, string>(2);
            parameterList.Add("method", "facebook.fbml.refreshImgSrc");
            parameterList.Add("url", string.Format(_installedCulture, url));
            string xml = ExecuteApiCallString(parameterList, true);
            if (!String.IsNullOrEmpty(xml))
            {
                XmlDocument xmlDocument = LoadXMLDocument(xml);

                XmlNodeList nodeList = xmlDocument.GetElementsByTagName("fbml_refreshImgSrc_response");

                if (nodeList != null && nodeList.Count > 0)
                {
                    if (nodeList[0].HasChildNodes)
                    {
                        results = nodeList[0].InnerText;
                    }
                }
            }
            return results;
        }

        /// <summary>
        /// Send a notification
        /// </summary>
        /// <param name="markup">fbml markup</param>
        /// <param name="toList">list of users to be notified</param>
        public string SendNotification(string markup, string toList, bool isUserToUser)
        {
            return SendNotification(markup, toList, null, isUserToUser);
        }
        /// <summary>
        /// Send a notification - app_to_user by default
        /// </summary>
        /// <param name="markup">fbml markup</param>
        /// <param name="toList">list of users to be notified</param>
        public string SendNotification(string markup, string toList)
        {
            return SendNotification(markup, toList, null, false);
        }
        /// <summary>
        /// Send a notification - - app_to_user by default
        /// </summary>
        /// <param name="markup">fbml markup</param>
        /// <param name="toList">list of users to be notified</param>
        /// <param name="email">fbml of email</param>
        public string SendNotification(string markup, string toList, string email)
        {
            return SendNotification(markup, toList, email, false);
        }
        /// <summary>
        /// Send a notification
        /// </summary>
        /// <param name="markup">fbml markup</param>
        /// <param name="toList">list of users to be notified</param>
        /// <param name="email">fbml of email</param>
        public string SendNotification(string markup, string toList, string email, bool isUserToUser) {
            string results = string.Empty;

            Dictionary<string, string> parameterList = new Dictionary<string, string>(5);
            parameterList.Add("method", "facebook.notifications.send");
            parameterList.Add("notification", string.Format(_installedCulture, markup));
            parameterList.Add("to_ids", toList);

            if (!isUserToUser)
            {
                parameterList.Add("type", "app_to_user"); // this is a application to user notification
            }

            if (!string.IsNullOrEmpty(email))
            {
                parameterList.Add("email", string.Format(_installedCulture, email));
            }


            string xml = ExecuteApiCallString(parameterList, isUserToUser); // this is a application to user notification, so no session necessary. 
            if (!String.IsNullOrEmpty(xml)) {
                XmlDocument xmlDocument = LoadXMLDocument(xml);

                XmlNodeList nodeList = xmlDocument.GetElementsByTagName("notifications_send_response");

                if (nodeList != null && nodeList.Count > 0) {
                    if (nodeList[0].HasChildNodes) {
                       results = nodeList[0].InnerText;
                    }
                }
            }
            return results;
        }

        /// <summary>
        /// ban
        /// </summary>
        public bool BanUser(string facebookUserIDToBan)
        {
            bool results = false;

            Dictionary<string, string> parameterList = new Dictionary<string, string>(5);
            parameterList.Add("method", "Admin.banUsers");

            List<long> list = new List<long>(1);
            list.Add(Convert.ToInt64(facebookUserIDToBan));

            AddJSONArray(parameterList, "uids", list);


            string xml = ExecuteApiCallString(parameterList, false);
            if (!String.IsNullOrEmpty(xml))
            {
                XmlDocument xmlDocument = LoadXMLDocument(xml);

                XmlNodeList nodeList = xmlDocument.GetElementsByTagName("Admin_banUsers_response");

                if (nodeList != null && nodeList.Count > 0)
                {
                    if (nodeList[0].HasChildNodes)
                    {
                        results = Convert.ToBoolean(Convert.ToInt32(nodeList[0].InnerText));
                    }
                }
            }
            return results;
        }

        /// <summary>
        /// unban
        /// </summary>
        public bool UnBanUser(string facebookUserIDToBan)
        {
            bool results = false;

            Dictionary<string, string> parameterList = new Dictionary<string, string>(5);
            parameterList.Add("method", "Admin.unbanUsers");

            List<long> list = new List<long>(1);
            list.Add(Convert.ToInt64(facebookUserIDToBan));

            AddJSONArray(parameterList, "uids", list);


            string xml = ExecuteApiCallString(parameterList, false);
            if (!String.IsNullOrEmpty(xml))
            {
                XmlDocument xmlDocument = LoadXMLDocument(xml);

                XmlNodeList nodeList = xmlDocument.GetElementsByTagName("Admin_unBanUsers_response");

                if (nodeList != null && nodeList.Count > 0)
                {
                    if (nodeList[0].HasChildNodes)
                    {
                        results = Convert.ToBoolean(Convert.ToInt32(nodeList[0].InnerText));
                    }
                }
            }
            return results;
        }


        /// <summary>
        /// Publishes a story to the logged-in user's feed.
        /// </summary>
        /// <param name="title">The title of the story.</param>
        /// <param name="body">The body of the story.</param>
        /// <param name="images">A list of images, with associated links, to be
        /// used in the story.</param>
        /// <remarks>The priority is set to 1 by default.</remarks>
        /// <returns>The result of the method call as a string.</returns>
        public string PublishStory(string title, string body, Collection<PublishImage> images) {
            return PublishStory(title, body, images, 1);
        }

        /// <summary>
        /// Publishes a story to the logged-in user's feed.
        /// </summary>
        /// <param name="title">The title of the story.</param>
        /// <param name="body">The body of the story.</param>
        /// <param name="images">A list of images, with associated links, to be
        /// used in the story.</param>
        /// <param name="priority">The priority of the story.</param>
        /// <returns>The result of the method call as a string.</returns>
        public string PublishStory(string title, string body, Collection<PublishImage> images, int priority) {
            string results = string.Empty;

            Dictionary<string, string> parameterList = new Dictionary<string, string>();
            parameterList.Add("method", "facebook.feed.publishStoryToUser");
            if (!string.IsNullOrEmpty(title)) {
                parameterList.Add("title", title);
            }
            else {
                throw new FacebookException("Title is required");
            }
            if (!string.IsNullOrEmpty(body)) {
                parameterList.Add("body", body);
            }
            parameterList.Add("priority", priority.ToString());
            if (images != null)
            {
                if(images.Count> 5)
                {
                    throw new FacebookException("Maximum 5 images allowed");
                }
                for (int i = 0; i < images.Count;i++ )
                {
                    if (!string.IsNullOrEmpty(images[i].ImageLocation))
                    {
                        parameterList.Add("image_" + (i + 1), images[i].ImageLocation);
                        if (!string.IsNullOrEmpty(images[i].ImageLink))
                        {
                            parameterList.Add("image_" + (i + 1) + "_link", images[i].ImageLink);
                        }
                    }
                    else
                    {
                        throw new FacebookException("Image location missing of image " + (i + 1));
                    }

                }
            }
            string xml = ExecuteApiCallString(parameterList, true);
            if (!String.IsNullOrEmpty(xml)) {
                XmlDocument xmlDocument = LoadXMLDocument(xml);

                XmlNodeList nodeList = xmlDocument.GetElementsByTagName("feed_publishStoryToUser_response");

                if (nodeList != null && nodeList.Count > 0) {
                    if (nodeList[0].HasChildNodes) {
                        results = nodeList[0].InnerText;
                    }
                }
            }


            return results;
        }

        /// <summary>
        /// Publishes an action to the logged-in user's mini-feed, and to the
        /// news feeds of the user's friends who have added the application.
        /// </summary>
        /// <param name="title">The title of the action.</param>
        /// <param name="body">The body of the action.</param>
        /// <param name="images">A list of images, with associated links, to be
        /// used in the action.</param>
        /// <remarks>The priority is set to 1 by default.</remarks>
        /// <returns>The result of the method call as a string.</returns>
        public string PublishAction(string title, string body, Collection<PublishImage> images) {
            return PublishAction(title, body, images, 1);
        }

        /// <summary>
        /// Publishes an action to the logged-in user's mini-feed, and to the
        /// news feeds of the user's friends who have added the application.
        /// </summary>
        /// <param name="title">The title of the action.</param>
        /// <param name="body">The body of the action.</param>
        /// <param name="images">A list of images, with associated links, to be
        /// used in the action.</param>
        /// <param name="priority">The priority of the action.</param>
        /// <returns>The result of the method call as a string.</returns>
        public string PublishAction(string title, string body, Collection<PublishImage> images, int priority) {
            string results = string.Empty;

            Dictionary<string, string> parameterList = new Dictionary<string, string>();
            parameterList.Add("method", "facebook.feed.publishActionOfUser");
            if (!string.IsNullOrEmpty(title)) {
                parameterList.Add("title", title);
            }
            else {
                throw new FacebookException("Title is required");
            }
            if (!string.IsNullOrEmpty(body)) {
                parameterList.Add("body", body);
            }
            parameterList.Add("priority", priority.ToString());
            if (images != null)
            {
                if (images.Count > 5)
                {
                    throw new FacebookException("Maximum 5 images allowed");
                }
                for (int i = 0; i < images.Count; i++)
                {
                    if (!string.IsNullOrEmpty(images[i].ImageLocation) && !string.IsNullOrEmpty(images[i].ImageLink))
                    {
                        parameterList.Add("image_" + (i + 1), images[i].ImageLocation);
                        parameterList.Add("image_" + (i + 1) + "_link", images[i].ImageLink);
                    }
                    else
                    {
                        throw new FacebookException("Image location and link are required.  Missing for image " + (i + 1));
                    }

                }
            }
            string xml = ExecuteApiCallString(parameterList, true);
            if (!String.IsNullOrEmpty(xml)) {
                XmlDocument xmlDocument = LoadXMLDocument(xml);

                XmlNodeList nodeList = xmlDocument.GetElementsByTagName("feed_publishActionOfUser_response");

                if (nodeList != null && nodeList.Count > 0) {
                    if (nodeList[0].HasChildNodes) {
                        results = nodeList[0].InnerText;
                    }
                }
            }


            return results;
        }
 
        public bool PublishUserAction(string templateBundleID, Dictionary<String, String> templateData)
        {
            return PublishUserAction(templateBundleID, templateData, null, Enums.StorySize.OneLine);
        }
        const string templateDataPairTemplate = "\"{0}\":\"{1}\"";
        const string templateImagesTemplate = "\"images\":[{0}]";
        const string templateImageTemplate = "{{\"src\":\"{0}\", \"href\":\"{1}\"}},";
        public bool PublishUserAction(string templateBundleID, Dictionary<String, String> templateData, Collection<PublishImage> images, Enums.StorySize storySize)
        {
            /*
             {"villagecount":"11"
                ,"images":[{"src": "http://www.realmofempires.com/images/map/Village1a.png", "href":"http://www.facebook.com"}
                         ,{"src":"http://www.realmofempires.com/images/map/Village2a.png", "href":"http://www.facebook.com"}
                         , {"src": "http://www.realmofempires.com/images/map/Village3a.png", "href":"http://www.facebook.com"}
                        , {"src": "http://www.realmofempires.com/images/map/Village4a.png", "href":"http://www.facebook.com"}
              ]}         
         
              */

            if (string.IsNullOrEmpty(templateBundleID))
            {
                throw new ArgumentNullException("templateBundleID");
            }
            if (templateData == null)
            {
                throw new ArgumentNullException("templateData");
            }

            bool results = false;
            Dictionary<string, string> parameterList = new Dictionary<string, string>();
            StringBuilder sbImages;

            parameterList.Add("method", "facebook.Feed.publishUserAction");
            parameterList.Add("template_bundle_id", templateBundleID);
            parameterList.Add("story_size", ((int)storySize).ToString());

            string value;
            StringBuilder sbData = new StringBuilder(templateData.Keys.Count * 20);
            //
            // add template data
            //
            sbData.Append("{");
            if (templateData.Count > 0)
            {
                foreach (string key in templateData.Keys)
                {
                    value = templateData[key];
                    sbData.AppendFormat(templateDataPairTemplate, key, value);
                    sbData.Append(",");
                }                

                // if there are no images then remove the trailing ,
                if (images == null)
                {
                    sbData.Remove(sbData.Length - 1, 1);
                }
            }
            //
            // add images if any
            //
            if (images != null)
            {
                sbImages = new StringBuilder(images.Count * 50);
                for (int i = 0; i < images.Count; i++)
                {
                    if (!string.IsNullOrEmpty(images[i].ImageLocation) && !string.IsNullOrEmpty(images[i].ImageLink))
                    {
                        sbImages.AppendFormat(templateImageTemplate,  images[i].ImageLocation, images[i].ImageLink);
                    }
                    else
                    {
                        throw new FacebookException("Image location and link are required.  Missing for image " + (i + 1));
                    }
                }
                sbImages.Remove(sbImages.Length - 1, 1);

                sbData.AppendFormat(templateImagesTemplate, sbImages);
            }
            sbData.Append("}");
            parameterList.Add("template_data", sbData.ToString());
            
             
            string xml = ExecuteApiCallString(parameterList, true);
            if (!String.IsNullOrEmpty(xml))
            {
                XmlDocument xmlDocument = LoadXMLDocument(xml);

                XmlNodeList nodeList = xmlDocument.GetElementsByTagName("Feed_publishUserAction_response");

                if (nodeList != null && nodeList.Count > 0)
                {
                    if (nodeList[0].HasChildNodes)
                    {
                        results = Convert.ToBoolean(Convert.ToInt32(nodeList[0].InnerText));
                    }
                }
            }


            return results;
        }


        /// <summary>
        /// Uploads the specified photo to the specified album.
        /// </summary>
        /// <param name="albumId">The album to upload to. If not specified, will use default album.</param>
        /// <param name="uploadFile">The .jpg file to upload.</param>
        /// <returns>A result containing the ids for the photo and album.</returns>
        public UploadPhotoResult UploadPhoto(string albumId, FileInfo uploadFile) {
            return UploadPhoto(albumId, uploadFile, null);
        }

        /// <summary>
        /// Uploads the specified photo to the specified album.
        /// </summary>
        /// <param name="albumId">The album to upload to. If not specified, will use default album.</param>
        /// <param name="uploadFile">The .jpg file to upload.</param>
        /// <param name="caption">The caption to attach to the photo.</param>
        /// <returns>A result containing the ids for the photo and album.</returns>
        public UploadPhotoResult UploadPhoto(string albumId, FileInfo uploadFile, string caption)
        {
            Dictionary<string, string> parameterList = new Dictionary<string, string>(4);
            parameterList.Add("method", "photos.upload");
            if (!string.IsNullOrEmpty(albumId))
            {
                parameterList.Add("aid", albumId);
            }
            if (!string.IsNullOrEmpty(caption))
            {
                parameterList.Add("caption", caption);
            }
            parameterList.Add("session_key", _sessionKey);

            string xml = ExecuteApiUpload(uploadFile, parameterList);

            if (string.IsNullOrEmpty(xml))
            {
                return null;
            }

            XmlDocument xmlDocument = LoadXMLDocument(xml);
            XmlNodeList nodeList = xmlDocument.GetElementsByTagName("photos_upload_response");

            if (nodeList == null || nodeList.Count == 0 || !nodeList[0].HasChildNodes)
            {
                return null;
            }

            return new UploadPhotoResult(XmlHelper.GetNodeText(nodeList[0], "pid"), XmlHelper.GetNodeText(nodeList[0], "aid"));
        }

        /// <summary>
        /// Add tag to a photo.  Only allowed on photos in pending state.
        /// </summary>
        /// <param name="photoId">The id of the photo to tag</param>
        /// <param name="tagText">The text of the tag.  Need to specify either this of tagUserId</param>
        /// <param name="tagUserId">The facebook id the person that was tagged</param>
        /// <param name="xCoord">The x position of the tag on the photo</param>
        /// <param name="yCoord">The y position of the tag on the photo</param>
        public void AddTag(string photoId, string tagText, string tagUserId, int xCoord, int yCoord) {
            Dictionary<string, string> parameterList = new Dictionary<string, string>(7);
            parameterList.Add("method", "photos.addTag");
            parameterList.Add("pid", photoId);
            if (!string.IsNullOrEmpty(tagText)) {
                parameterList.Add("tag_text", tagText);
            }
            else if (!string.IsNullOrEmpty(tagUserId)) {
                parameterList.Add("tag_uid", tagUserId);
            }
            else {
                throw new FacebookException("either text or user id must be specified");
            }
            parameterList.Add("x", xCoord.ToString(CultureInfo.InvariantCulture));
            parameterList.Add("y", yCoord.ToString(CultureInfo.InvariantCulture));
            ExecuteApiCall(parameterList, true);
        }

        /// <summary>
        /// Create Album.
        /// </summary>
        /// <param name="name">The name of the album</param>
        /// <param name="location">The location of the album.  (Optional)</param>
        /// <param name="description">The description of the album.  (Optional)</param>
        public Album CreateAlbum(string name, string location, string description) {
            Dictionary<string, string> parameterList = new Dictionary<string, string>(5);
            parameterList.Add("method", "photos.createAlbum");

            if (!string.IsNullOrEmpty(name)) {
                parameterList.Add("name", name);
            }
            else {
                throw new FacebookException("album name must be specified");
            }

            if (!string.IsNullOrEmpty(location)) {
                parameterList.Add("location", location);
            }
            if (!string.IsNullOrEmpty(description)) {
                parameterList.Add("description", description);
            }
            XmlDocument xml = ExecuteApiCall(parameterList, true);
            if (xml.GetElementsByTagName("photos_createAlbum_response").Count > 0)
            {
                return AlbumParser.ParseAlbum(xml.GetElementsByTagName("photos_createAlbum_response")[0]);

            }
            else
            {
                throw new FacebookException("Album creation failed");
            }
        }

        /// <summary>
        /// Get the groups that the logged in user belongs to
        /// </summary>
        /// <returns>groups</returns>
        public Collection<Group> GetGroups() {
            return GetGroups(_userId, null);
        }

        /// <summary>
        /// Get the groups that the specified user belongs to
        /// </summary>
        /// <param name="userId">The id of the user to return groups for</param>
        /// <returns>groups</returns>
        public Collection<Group> GetGroups(string userId) {
            return GetGroups(userId, null);
        }

        /// <summary>
        /// Get the groups for the group list
        /// </summary>
        /// <param name="groupsList">Collection of group ids</param>
        /// <returns>groups</returns>
        public Collection<Group> GetGroups(Collection<string> groupsList) {
            return GetGroups(null, groupsList);
        }

        /// <summary>
        /// Get the groups for the specified user and group list
        /// </summary>
        /// <param name="userId">The id of the user to return groups for</param>
        /// <param name="groupsList">Collection of group ids</param>
        /// <returns>groups</returns>
        public Collection<Group> GetGroups(string userId, Collection<string> groupsList) {
            string xml = GetGroupsXML(userId, groupsList);
            Collection<Group> groups = new Collection<Group>();

            if (!String.IsNullOrEmpty(xml)) {
                XmlDocument xmlDocument = LoadXMLDocument(xml);

                XmlNodeList nodeList = xmlDocument.GetElementsByTagName("fql_query_response");

                if (nodeList != null && nodeList.Count > 0) {
                    if (nodeList[0].HasChildNodes) {
                        XmlNodeList results = xmlDocument.GetElementsByTagName("group");
                        foreach (XmlNode node in results) {
                            groups.Add(GroupParser.ParseGroup(node));
                        }
                    }
                }
            }
            return groups;
        }

        /// <summary>
        /// Get the groups for the specified user and group list
        /// </summary>
        /// <param name="userId">The id of the user to return groups for</param>
        /// <param name="groupsList">Collection of group ids</param>
        /// <returns>groups</returns>
        public string GetGroupsXML(string userId, Collection<string> groupsList) {
            Dictionary<string, string> parameterList = new Dictionary<string, string>(3);
            parameterList.Add("method", "fql.query");
            string uidClause = string.Empty;
            string gidClause = string.Empty;
            if (!String.IsNullOrEmpty(userId)) {
                uidClause = String.Concat(" gid IN (SELECT gid FROM group_member WHERE uid=", userId, ")");
            }
            if (groupsList != null) {
                if (!String.IsNullOrEmpty(userId)) {
                    gidClause = ANDCLAUSE;
                }
                gidClause =
                    String.Concat(gidClause, " gid IN (", StringHelper.ConvertToCommaSeparated(groupsList), ")");
            }
            parameterList.Add("query",
                              String.Format(CultureInfo.InvariantCulture, "{0}{1}{2}",
                                            "SELECT gid, name, nid, description, group_type, group_subtype, recent_news, pic, pic_big, pic_small, creator, update_time, office, website, venue FROM group WHERE",
                                            uidClause, gidClause));

            return ExecuteApiCallString(parameterList, true);
        }

        /// <summary>
        /// Get the members of the specified group
        /// </summary>
        /// <param name="groupId">The id of the group to return members for</param>
        /// <returns>Group members (user profiles, and group roles)</returns>
        public Collection<GroupUser> GetGroupMembers(string groupId) {
            string xml = GetGroupMembersXML(groupId);
            Collection<GroupUser> groupUsers = new Collection<GroupUser>();

            if (!String.IsNullOrEmpty(xml)) {
                XmlDocument xmlDocument = LoadXMLDocument(xml);
                XmlNodeList nodeList = xmlDocument.GetElementsByTagName("fql_query_response");

                if (nodeList != null && nodeList.Count > 0) {
                    if (nodeList[0].HasChildNodes) {
                        XmlNodeList results = xmlDocument.GetElementsByTagName("group_member");
                        Dictionary<string, GroupUser> groupUserDict = new Dictionary<string, GroupUser>();
                        foreach (XmlNode node in results) {
                            GroupUser groupUser = GroupUserParser.ParseGroupUser(node);
                            groupUserDict.Add(groupUser.UserId, groupUser);
                        }
                        Collection<User> users = GetUserInfo(StringHelper.ConvertToCommaSeparated(groupUserDict.Keys));
                        foreach (User user in users) {
                            if (groupUserDict.ContainsKey(user.UserId)) {
                                groupUserDict[user.UserId].User = user;
                                groupUsers.Add(groupUserDict[user.UserId]);
                            }
                        }
                    }
                }
            }
            return groupUsers;
        }

        /// <summary>
        /// Get the members of the specified group
        /// </summary>
        /// <param name="groupId">The id of the group to return members for</param>
        /// <returns>Group members (user profiles, and group roles)</returns>
        public string GetGroupMembersXML(string groupId) {
            Dictionary<string, string> parameterList = new Dictionary<string, string>(3);
            parameterList.Add("method", "fql.query");
            if (!string.IsNullOrEmpty(groupId)) {
                parameterList.Add("query",
                                  String.Format(CultureInfo.InvariantCulture, "{0}{1}",
                                                "SELECT uid, gid, positions FROM group_member WHERE gid=", groupId));
            }
            else {
                throw new FacebookException("Group Id is required");
            }

            return ExecuteApiCallString(parameterList, true);
        }

        /// <summary>
        /// Get the facebook user id of the user associated with the current session
        /// </summary>
        /// <returns>facebook userid</returns>
        public string GetLoggedInUser() {
            string results = string.Empty;

            Dictionary<string, string> parameterList = new Dictionary<string, string>();
            parameterList.Add("method", "users.getLoggedInUser");
            string xml = ExecuteApiCallString(parameterList, true);
            if (!String.IsNullOrEmpty(xml)) {
                XmlDocument xmlDocument = LoadXMLDocument(xml);

                XmlNodeList nodeList = xmlDocument.GetElementsByTagName("users_getLoggedInUser_response");

                if (nodeList != null && nodeList.Count > 0) {
                    if (nodeList[0].HasChildNodes) {
                        results = nodeList[0].InnerText;
                    }
                }
            }


            return results;
        }

        /// <summary>
        /// Publishes a templatized action to the logged-in user's mini-feed,
        /// and to the news feeds of the user's friends who have added the application.
        /// </summary>
        /// <remarks>More documentation on the template parameters can be found
        /// on the Facebook Developers web site.
        /// </remarks>
        /// <param name="titleTemplate">Templated markup for the title of the action.</param>
        /// <param name="titleData">A dictionary of token values.</param>
        /// <param name="bodyTemplate">Templated markup for the body content.</param>
        /// <param name="bodyData">A dictionary of token values for the body.</param>
        /// <param name="images">A collection of images, and their associated
        /// links, to be shown in the action.</param>
        /// <returns>The string result of the call.</returns>
        public string PublishTemplatizedAction(string titleTemplate, Dictionary<string, string> titleData,
            string bodyTemplate, Dictionary<string, string> bodyData, Collection<PublishImage> images)
        {
            return PublishTemplatizedAction(titleTemplate, titleData, bodyTemplate, bodyData, null, null, images);
        }

        /// <summary>
        /// Publishes a templatized action to the logged-in user's mini-feed,
        /// and to the news feeds of the user's friends who have added the application.
        /// </summary>
        /// <remarks>More documentation on the template parameters can be found
        /// on the Facebook Developers web site.
        /// </remarks>
        /// <param name="titleTemplate">Templated markup for the title of the action.</param>
        /// <param name="titleData">A dictionary of token values.</param>
        /// <param name="bodyTemplate">Templated markup for the body content.</param>
        /// <param name="bodyData">A dictionary of token values for the body.</param>
        /// <param name="bodyGeneral">General content (markup) to be added to the action</param>
        /// <param name="targetIds">The ids of other users to "target" with this action</param>        
        /// <param name="images">A collection of images, and their associated
        /// links, to be shown in the action.</param>
        /// <returns>The string result of the call.</returns>
        public string PublishTemplatizedAction(string titleTemplate, Dictionary<string, string> titleData,
            string bodyTemplate, Dictionary<string, string> bodyData, string bodyGeneral, Collection<string> targetIds,
            Collection<PublishImage> images)
        {
            Dictionary<string, string> parameterList = new Dictionary<string, string>();
            parameterList.Add("method", "feed.publishTemplatizedAction");
            parameterList.Add("actor_id", UserId);
            parameterList.Add("title_template", titleTemplate);

            if (titleData != null && titleData.Count > 0)
            {
                parameterList.Add("title_data", ToJsonAssociativeArray(titleData));
            }

            if (!string.IsNullOrEmpty(bodyTemplate))
            {
                parameterList.Add("body_template", bodyTemplate);
            }

            if (bodyData != null && bodyData.Count > 0)
            {
                parameterList.Add("body_data", ToJsonAssociativeArray(bodyData));
            }

            if (!string.IsNullOrEmpty(bodyGeneral))
            {
                parameterList.Add("body_general", bodyGeneral);
            }

            if (targetIds != null && targetIds.Count > 0)
            {
                parameterList.Add("target_ids", string.Join(",", new List<string>(targetIds).ToArray()));
            }

            if (images != null)
            {
                for (int i = 1; i <= images.Count; i++)
                {
                    parameterList.Add("image_" + i, images[i - 1].ImageLocation);
                    parameterList.Add("image_" + i + "_link", images[i - 1].ImageLink);
                }
            }

            XmlDocument xmlResults = ExecuteApiCall(parameterList, true);
            XmlNodeList nodeList = xmlResults.GetElementsByTagName("feed_publishTemplatizedAction_response");

            if (nodeList != null && nodeList.Count > 0)
            {
                if (nodeList[0].HasChildNodes)
                {
                    return nodeList[0].InnerText;
                }
            }

            return "";
        }

        /// <summary>
        /// Forgets all connection information so that this object may be used for another connection.
        /// </summary>
        public void LogOff()
        {
            _authToken = null;
            _sessionKey = null;
            _userId = null;
        }

        /// <summary>
        /// Send an email
        /// </summary>
        /// <param name="recipients">A comma-separated list of recipient IDs. The recipients must be people who have already added your application. You can email up to 100 people at a time.</param>
        /// <param name="subject">The subject of the email message.</param>
        /// <param name="text">The plain text version of the email content. You must include at least one of either the fbml or text parameters. </param>
        /// <param name="fbml">The FBML version of the email. You must include at least one of either the fbml or text parameters. The fbml parameter is a stripped-down set of FBML that allows only tags that result in text, links and linebreaks.</param>
        public string SendEmail(string recipients, string subject, string text, string fbml)
        {
            string results = string.Empty;

            Dictionary<string, string> parameterList = new Dictionary<string, string>(5);
            parameterList.Add("method", "facebook.notifications.sendEmail");
            parameterList.Add("recipients", recipients);
            parameterList.Add("subject", subject);
            parameterList.Add("text", text);

            if (!string.IsNullOrEmpty(fbml))
            {
                parameterList.Add("fbml", string.Format(_installedCulture, fbml));
            }

            string xml = ExecuteApiCallString(parameterList, true);
            if (!String.IsNullOrEmpty(xml))
            {
                XmlDocument xmlDocument = LoadXMLDocument(xml);

                XmlNodeList nodeList = xmlDocument.GetElementsByTagName("notificationssendEmailresponse");

                if (nodeList != null && nodeList.Count > 0)
                {
                    if (nodeList[0].HasChildNodes)
                    {
                        results = nodeList[0].InnerText;
                    }
                }
            }
            return results;
        }

        #endregion

        #region Private functions

        /// <summary>
        /// Method creates a session
        /// </summary>
        internal void CreateSession() {
            Dictionary<string, string> parameterList = new Dictionary<string, string>(3);
            parameterList.Add("method", "auth.getSession");
            parameterList.Add("auth_token", AuthToken);
            XmlDocument xmlDocument = ExecuteApiCall(parameterList, false);

            _sessionKey = xmlDocument.DocumentElement.SelectSingleNode("Facebook:session_key", NsManager).InnerText;
            _userId = xmlDocument.DocumentElement.SelectSingleNode("Facebook:uid", NsManager).InnerText;

            XmlNode secretNode = xmlDocument.DocumentElement.SelectSingleNode("Facebook:secret", NsManager);
            if (_isDesktopApp && (secretNode != null && !string.IsNullOrEmpty(secretNode.InnerText))) {
                _secret = secretNode.InnerText;
            }
            double expires =
                double.Parse(xmlDocument.DocumentElement.SelectSingleNode("Facebook:expires", NsManager).InnerText,
                             CultureInfo.InvariantCulture);

            _sessionExpires = false;

            if (expires > 0)
                _sessionExpires = true;
        }

        /// <summary>
        /// Get Query Response
        /// </summary>
        /// <param name="requestUrl">Request Url</param>
        /// <param name="postString">posted query</param>
        /// <returns>Response data</returns>
        internal static string GetQueryResponse(string requestUrl, string postString)
        {
            // Create a web request for input path.
            WebRequest webRequest = WebRequest.Create(requestUrl);
            webRequest.Method = "POST";
            webRequest.Timeout = 5000;
            webRequest.ContentType = "application/x-www-form-urlencoded";

            if (!String.IsNullOrEmpty(postString)) {
                byte[] parameterString = Encoding.ASCII.GetBytes(postString);
                webRequest.ContentLength = parameterString.Length;

                using (Stream buffer = webRequest.GetRequestStream()) {
                    buffer.Write(parameterString, 0, parameterString.Length);
                    buffer.Close();
                }
            }

            WebResponse webResponse = webRequest.GetResponse();

            string responseData;
            using (StreamReader streamReader = new StreamReader(webResponse.GetResponseStream())) {
                responseData = streamReader.ReadToEnd();
            }
            return responseData;
        }

        /// <summary>
        /// Get File Query Response
        /// </summary>
        /// <param name="parameterDictionary">parameter list</param>
        /// <param name="uploadFile">uploaded file info</param>
        /// <returns>Response data</returns>
        internal static string GetFileQueryResponse(IEnumerable<KeyValuePair<string, string>> parameterDictionary,
                                                   FileSystemInfo uploadFile) {
            string responseData;

            string boundary = DateTime.Now.Ticks.ToString("x", CultureInfo.InvariantCulture);
            string sRequestUrl = Facebook.Properties.Resources.FacebookRESTUrl;

            // Build up the post message header
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string> kvp in parameterDictionary) {
                sb.Append(PREFIX).Append(boundary).Append(NEWLINE);
                sb.Append("Content-Disposition: form-data; name=\"").Append(kvp.Key).Append("\"");
                sb.Append(NEWLINE);
                sb.Append(NEWLINE);
                sb.Append(kvp.Value);
                sb.Append(NEWLINE);
            }
            sb.Append(PREFIX).Append(boundary).Append(NEWLINE);
            sb.Append("Content-Disposition: form-data; filename=\"").Append(uploadFile.Name).Append("\"").Append(NEWLINE);
            sb.Append("Content-Type: image/jpeg").Append(NEWLINE).Append(NEWLINE);

            byte[] postHeaderBytes = Encoding.ASCII.GetBytes(sb.ToString());
#if NETCF
            byte[] fileData = Facebook.Compact.FileHelper.ReadAllBytes(uploadFile.FullName);
#else
            byte[] fileData = File.ReadAllBytes(uploadFile.FullName);
#endif
            byte[] boundaryBytes = Encoding.ASCII.GetBytes(String.Concat(NEWLINE, PREFIX, boundary, PREFIX, NEWLINE));

            HttpWebRequest webrequest = (HttpWebRequest) HttpWebRequest.Create(sRequestUrl);
            webrequest.ContentLength = postHeaderBytes.Length + fileData.Length + boundaryBytes.Length;
            webrequest.ContentType = String.Concat("multipart/form-data; boundary=", boundary);
            webrequest.Method = "POST";

            using (Stream requestStream = webrequest.GetRequestStream()) {
                requestStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
                requestStream.Write(fileData, 0, fileData.Length);
                requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);
            }
            HttpWebResponse response = (HttpWebResponse) webrequest.GetResponse();
            using (StreamReader streamReader = new StreamReader(response.GetResponseStream())) {
                responseData = streamReader.ReadToEnd();
            }


            return responseData;
        }

        internal string ExecuteApiUpload(FileSystemInfo uploadFile, IDictionary<string, string> parameterList)
        {
            parameterList.Add("api_key", _applicationKey);
            parameterList.Add("v", VERSION);
            parameterList.Add("call_id", DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture));
            parameterList.Add("sig", GenerateSignature(parameterList));

            // Get response
            return GetFileQueryResponse(parameterList, uploadFile);
        }

        internal XmlDocument ExecuteApiCall(IDictionary<string, string> parameterDictionary, bool useSession)
        {

            XmlDocument doc=null;
            try {
                doc = LoadXMLDocument(ExecuteApiCallString(parameterDictionary, useSession));
            }
            catch (Exception e)
            {
                BaseApplicationException bex = new BaseApplicationException("error in ExecuteApiCallString", e);
                bex.AddAdditionalInformation("parameterDictionary", parameterDictionary);
                bex.AddAdditionalInformation("doc.InnerXml", doc == null ? "null" : doc.InnerXml);
                throw bex;
            }

            return doc;
        }

        internal string ExecuteApiCallString(IDictionary<string, string> parameterDictionary, bool useSession)
        {
            string requestUrl=null;
            string parameters=null;
            string queryResponse=null;
            try
            {
                if (useSession)
                {
                    parameterDictionary.Add("access_token", _sessionKey);
                }

                requestUrl = GetRequestUrl(true);
                parameters = CreateHTTPParameterList(parameterDictionary);
                queryResponse = GetQueryResponse(requestUrl, parameters);
            }
            catch (Exception e)
            {
                BaseApplicationException bex = new BaseApplicationException("error in ExecuteApiCallString", e);
                bex.AddAdditionalInformation("parameterDictionary", parameterDictionary);
                bex.AddAdditionalInformation("requestUrl", requestUrl);
                bex.AddAdditionalInformation("parameters", parameters);
                bex.AddAdditionalInformation("queryResponse", queryResponse);
                throw bex;
            }
            return queryResponse;
        }

        internal XmlDocument LoadXMLDocument(string rawXML)
        {
            // Parse the data and extract the session details
            XmlDocument xmlDocument = new XmlDocument();

            try
            {
                xmlDocument.LoadXml(rawXML);

                _nsManager = new XmlNamespaceManager(xmlDocument.NameTable);
                _nsManager.AddNamespace("Facebook", Facebook.Properties.Resources.FacebookNamespace);

                ErrorCheck(xmlDocument);
            }
            catch (FacebookSessionExpiredException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                BaseApplicationException bex = new BaseApplicationException("error in LoadXMLDocument", e);
                bex.AddAdditionalInformation("rawXML", rawXML);
                bex.AddAdditionalInformation("_authToken", this._authToken);
                bex.AddAdditionalInformation("_sessionExpires", this._sessionExpires);
                bex.AddAdditionalInformation("_sessionKey", this._sessionKey);
                bex.AddAdditionalInformation("_userId", this._userId);

                throw bex;
            }
            return xmlDocument;
        }

        /// <summary>
        /// Gets the request url
        /// </summary>
        /// <param name="useSSL">True if the request should use SSL, otherwise False</param>
        /// <returns>Request Url</returns>
        internal string GetRequestUrl(bool useSSL)
        {
            string returnValue = Facebook.Properties.Resources.FacebookRESTUrl;
            
            if (useSSL) {
                returnValue = returnValue.Replace("http", "https");
            }

            return returnValue;
        }

        internal string CreateHTTPParameterList(IDictionary<string, string> parameterList)
        {
            StringBuilder queryBuilder = new StringBuilder();

            parameterList.Add("api_key", _applicationKey);
            parameterList.Add("v", VERSION);
            parameterList.Add("call_id", DateTime.Now.Ticks.ToString("x", CultureInfo.InvariantCulture));
            parameterList.Add("sig", GenerateSignature(parameterList));

            // Build the query
            foreach (KeyValuePair<string, string> kvp in parameterList) {
                queryBuilder.Append(kvp.Key);
                queryBuilder.Append("=");
                queryBuilder.Append(HttpUtility.UrlEncode(kvp.Value));
                queryBuilder.Append("&");
            }
            queryBuilder.Remove(queryBuilder.Length - 1, 1);

            return queryBuilder.ToString();
        }

        internal List<string> ParameterDictionaryToList(IEnumerable<KeyValuePair<string, string>> parameterDictionary)
        {
            List<string> parameters = new List<string>();

            foreach (KeyValuePair<string, string> kvp in parameterDictionary) {
                parameters.Add(String.Format(CultureInfo.InvariantCulture, "{0}", kvp.Key));
            }
            return parameters;
        }

        /// <summary>
        /// This method generates the signature based on parameters supplied
        /// </summary>
        /// <param name="parameters">List of paramenters</param>
        /// <returns>Generated signature</returns>
        internal string GenerateSignature(IDictionary<string, string> parameters)
        {
            StringBuilder signatureBuilder = new StringBuilder();
            // Sort the keys of the method call in alphabetical order
            List<string> keyList = ParameterDictionaryToList(parameters);
            keyList.Sort();
            // Append all the parameters to the signature input paramaters
            foreach (string key in keyList)
                signatureBuilder.Append(String.Format(CultureInfo.InvariantCulture, "{0}={1}", key, parameters[key]));

            // Append the secret to the signature builder
            signatureBuilder.Append(_secret);
            byte[] hash;

            MD5 md5 = MD5.Create();
            // Compute the MD5 hash of the signature builder
            hash = md5.ComputeHash(Encoding.UTF8.GetBytes(
                                       signatureBuilder.ToString().Trim()));

            // Reinitialize the signature builder to store the actual signature
            signatureBuilder = new StringBuilder();

            // Append the hash to the signature
            foreach (byte hashByte in hash)
                signatureBuilder.Append(hashByte.ToString("x2", CultureInfo.InvariantCulture));

            return signatureBuilder.ToString();
        }

        internal bool IsSessionActive()
        {
            if (!String.IsNullOrEmpty(_sessionKey))
                return true;
            return false;
        }
        /// <summary>
        /// Parse the Facebook result for an error, and throw an exception. 
        /// For some of the different types of exceptions, custom action might be desirable.
        /// </summary>
        /// <param name="doc">The XML result.</param>
        internal static void ErrorCheck(XmlDocument doc) {
            XmlNodeList errors = doc.GetElementsByTagName("error_response");

            if (errors.Count > 0) {
                int errorCode = int.Parse(XmlHelper.GetNodeText(errors[0], "error_code"), CultureInfo.InvariantCulture);

                // Custom exception for some of the errors
                switch (errorCode) {
                    case 1:
                        throw new FacebookUnknownException(XmlHelper.GetNodeText(errors[0], "error_msg"));
                    case 2:
                        throw new FacebookServiceUnavailableException(XmlHelper.GetNodeText(errors[0], "error_msg"));
                    case 4:
                        throw new FacebookRequestLimitException(XmlHelper.GetNodeText(errors[0], "error_msg"));
                    case 102:
                    case 190:
                        throw new FacebookSessionExpiredException(XmlHelper.GetNodeText(errors[0], "error_msg"));
                    case 104:
                        throw new FacebookSigningException(XmlHelper.GetNodeText(errors[0], "error_msg"));
                    case 110:
                        throw new FacebookInvalidUserException(XmlHelper.GetNodeText(errors[0], "error_msg"));
                    case 120:
                        throw new FacebookInvalidAlbumException(XmlHelper.GetNodeText(errors[0], "error_msg"));
                    case 210: // user not visible
                    case 220: // album not visible
                    case 221: // photo not visible
                        throw new FacebookNotVisibleException(XmlHelper.GetNodeText(errors[0], "error_msg"));
                    case 601:
                        throw new FacebookInvalidFqlSyntaxException(XmlHelper.GetNodeText(errors[0], "error_msg"));
                    default:
                        throw new FacebookException(XmlHelper.GetNodeText(errors[0], "error_msg"));
                }
            }
        }

        internal string ToJsonAssociativeArray(Dictionary<string, string> paramDictionary)
        {
            List<string> nameValuePairs = new List<string>();

            foreach (KeyValuePair<string, string> pair in paramDictionary)
            {
                nameValuePairs.Add("\"" + EscapeJsonString(pair.Key) + "\":\"" + EscapeJsonString(pair.Value) + "\"");
            }

            return "{" + string.Join(",", nameValuePairs.ToArray()) + "}";
        }

        internal string EscapeJsonString(string originalString)
        {
            // escape backslashes and double quotes
            return originalString.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        internal void AddJSONArray(IDictionary<string, string> dict, string key, List<long> value)
        {
            if (value != null && value.Count > 0)
            {
                dict.Add(key, ConvertToJSONArray(value));
            }
        }


        public string ConvertToJSONArray(List<long> list)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("[");
            foreach (var item in list)
            {
                builder.Append(string.Format("{0}{1}{0},", IsJSONArray(item.ToString()) || IsBoolean(item.ToString()) ? string.Empty : "\"", EscapeJsonString(item.ToString())));
            }
            builder.Replace(",", "]", builder.Length - 1, 1);
            return builder.ToString();
        }


        public static bool IsJSONArray(string test)
        {
            return test.StartsWith("{") && !test.StartsWith("{*") || test.StartsWith("[");
        }
        public static bool IsBoolean(string test)
        {
            return test.Equals("false") || test.Equals("true");
        }
        #endregion
    }
}