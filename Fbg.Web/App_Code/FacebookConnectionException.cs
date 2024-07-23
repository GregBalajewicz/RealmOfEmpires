using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Runtime.Serialization;
using Gmbc.Common.Diagnostics.ExceptionManagement;
using System.Security.Permissions;

/// <summary>
/// Summary description for FacebookConnectionException
/// </summary>

[Serializable]
public class FacebookConnectionException : BaseApplicationException
{
    /// <summary>
    /// Basic constructor
    /// </summary>
    public FacebookConnectionException() : base() { }

    /// <summary>
    /// Constructor allowing the Message property to be set.
    /// </summary>
    /// <param name="message">String setting the message of the exception.</param>
    public FacebookConnectionException(string message)
        : base(message)
    {
    }
    /// <summary>
    /// Constructor allowing the Message and InnerException property to be set.
    /// </summary>
    /// <param name="message">String setting the message of the exception.</param>
    /// <param name="inner">Sets a reference to the InnerException.</param>
    public FacebookConnectionException(string message, Exception inner)
        : base(message, inner)
    {
    }


    /// <summary>
    /// Used for serialization
    /// </summary>
    [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
    }
}
