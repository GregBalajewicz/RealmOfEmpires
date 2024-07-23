<%@ Application Language="C#" %>

<script runat="server">

    void Application_Start(object sender, EventArgs e) 
    {
        // Code that runs on application startup

    }
    
    void Application_End(object sender, EventArgs e) 
    {
        //  Code that runs on application shutdown

    }
        
    void Application_Error(object sender, EventArgs e) 
    {
        Exception ex = Server.GetLastError();

        if (ex is System.Web.HttpException)
        {
            if (String.Compare(ex.Message, "Invalid viewstate.", true) == 0 
                && Request.ServerVariables["PATH_INFO"].Contains("WebResource.axd")
                && Request.ServerVariables["QUERY_STRING"].StartsWith("d="))
            {
                // this error is due to known bug in IE8 https://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=434997 
                // it does not effect user experience so we ignore it. On June 2009, this was generating about 1000 errors per day
                return;
            }
            if (String.Compare(ex.Message, "This is an invalid webresource request.", true) == 0)
            {
                // this is either the same problem as above (ie error is due to known bug in IE8 https://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=434997)
                // or a webcrawler requested WebResource.axd without params. Search the web for this problem. 
                return;
            } 
            if (
                    (ex.Message.Contains("was not found.") || ex.Message.Contains("does not exist."))
                &&
                    (ex.Message.Contains(".axd'") || ex.Message.Contains("WebResource"))
                )
            {
                // this error is due to known bug in IE8 https://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=434997 
                //  this is supposed to catch 'System.Web.HttpException' with messages like this:
                //  - Path '/scripe.axd' was not found.                 
                //  - Path '/scriprce.axd' was not found.
                //  - Path '/scripbResource.axd' was not found.
                //  - Path '/script/autopop.jsWebResource.axd' was not found.
                //  - The file '/WebResource.rview.aspx' does not exist.
                //  - and many more... 
                // it does not effect user experience so we ignore it. On July 2009, this was generating about 100 errors per day
                return;
            }
            
            
              
        }
        
       //HttpContext.Current.Session["LastError"] = ex;
     //  Server.Transfer("~/error.aspx");

    }

    void Session_Start(object sender, EventArgs e) 
    {
        // Code that runs when a new session is started

    }

    void Session_End(object sender, EventArgs e) 
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.

    }
       
</script>
