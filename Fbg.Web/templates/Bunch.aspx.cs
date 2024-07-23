using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using Fbg.Bll;
public partial class templates_Bunch : JsonPage
{
    public int realmID;

    public override object Result()
    {
        if (String.IsNullOrEmpty(Request.QueryString["rid"]))
        {
            throw new ArgumentException("rid expected in Request.QueryString");
        }

        //realmID = Convert.ToInt32(Request.QueryString["rid"]);

        Dictionary<string, string> d = new Dictionary<string, string>();

        foreach (string t in Request["tt"].Split(','))
        {
            StringWriter htmlStringWriter = new StringWriter();

            Server.Execute(String.Format("{0}{1}{2}", t, ".aspx?t=y", Request.QueryString.ToString()), htmlStringWriter);

            d.Add(t, htmlStringWriter.GetStringBuilder().ToString());
        }

        return d;
    }
}