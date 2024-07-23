using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;

/// <summary>
/// Summary description for JsonPage
/// </summary>
public class JsonPage : MyCanvasIFrameBasePage
{
    protected JavaScriptSerializer ser = new JavaScriptSerializer();

    public virtual object Result()
    {
        return new object();
    }

    protected override void Render(System.Web.UI.HtmlTextWriter writer)
    {
        ser.MaxJsonLength = CONSTS.MaxJsonLength;

        this.Response.ContentType = "text/javascript";

        try
        {
            writer.Write(ser.Serialize(new { success = true, @object = Result() }));
        }
        catch (Exception exc)
        {
            writer.Write(ser.Serialize(new { success = false, message = exc.Message, result = "error" }));
        }

        base.Render(writer);
    }
}