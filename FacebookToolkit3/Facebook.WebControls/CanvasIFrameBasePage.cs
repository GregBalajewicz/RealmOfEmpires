using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using Facebook.Components;


namespace Facebook.WebControls
{
    public class CanvasIFrameBasePage : Page
    {
        Facebook.Components.FacebookService _fbService = new Facebook.Components.FacebookService();
        private string _api = null;
        private string _secret = null;
        private bool _useSession = true;
        private bool _autoAdd = true;
        private string _returnToUrl;

        /// <summary>
        /// return to url for player who just authorizes the app
        /// </summary>
        public string ReturnToUrl
        {
            get { return _returnToUrl; }
            set { _returnToUrl = value; }
        }
        private string _permissions;

        /// <summary>
        /// comma deliminated list of permissons to request when user authorizes the app. see http://developers.facebook.com/docs/authentication/permissions
        /// </summary>
        public string Permissions
        {
            get { return _permissions; }
            set { _permissions = value; }
        } 

        public string Api
        {
            get { return _api; }
            set { _api = value; }
        }
        public string Secret
        {
            get { return _secret; }
            set { _secret = value; }
        }
        public bool UseSession
        {
            get { return _useSession; }
            set { _useSession = value; }
        }
        public bool AutoAdd
        {
            get { return _autoAdd; }
            set { _autoAdd = value; }
        }
        public Facebook.Components.FacebookService FBService
        {
            get { return _fbService; }
        }

        protected override void OnPreRender(EventArgs e)
        {
            Response.AppendHeader("P3P", "CP=\"CAO PSA OUR\"");
            base.OnPreRender(e);
        }

        BasePageHelper _basePageHelper;
        protected void Page_Load(object sender, EventArgs e)
        {
            // ApplicationKey and Secret are acquired when you sign up for 
            _fbService.ApplicationKey = _api;
            _fbService.Secret = _secret;


            _basePageHelper = BasePageHelper.LoadIFramePage(_fbService, _useSession, _autoAdd, Request, Response, Session, ReturnToUrl, Permissions);
        }

        public void RedirectToLogin()
        {
            _basePageHelper.RedirectToLogin();
        }
    }
}
