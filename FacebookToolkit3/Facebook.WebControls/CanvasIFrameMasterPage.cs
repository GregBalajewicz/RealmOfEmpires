using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace Facebook.WebControls
{
    public class CanvasIFrameMasterPage : MasterPage
    {
        Facebook.Components.FacebookService _fbService = new Facebook.Components.FacebookService();
        private string _api = null;
        private string _secret = null;
        private bool _useSession = true;
        private bool _autoAdd = true;
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

        protected void Page_Init(object sender, EventArgs e)
        {
            // ApplicationKey and Secret are acquired when you sign up for 
            _fbService.ApplicationKey = _api;
            _fbService.Secret = _secret;

            // removed this since i am not using the master base page
            //BasePageHelper.LoadIFramePage(_fbService, _useSession, _autoAdd, Request, Response, Session);
        }
    }
}
