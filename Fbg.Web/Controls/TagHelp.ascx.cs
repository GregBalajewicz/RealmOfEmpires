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

public partial class Controls_TagHelp : BaseControl
{
    public Controls_TagHelp()
    {
        BaseResName = "TagHelp.ascx";
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        linkHelpDetails.Text = RS("linkBtn_MoreDetail");
    }
    protected void LinkButton2_Click(object sender, EventArgs e)
    {
        panelHelpDetails.Visible = !panelHelpDetails.Visible;
        if (panelHelpDetails.Visible)
        {
            linkHelpDetails.Text = RS("linkBtn_LessDetail");
        }
        else
        {
            linkHelpDetails.Text = RS("linkBtn_MoreDetail");
        }
    }
}
