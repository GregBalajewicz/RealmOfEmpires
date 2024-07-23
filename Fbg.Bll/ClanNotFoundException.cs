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
using System.Runtime.Serialization;
using System.Security.Permissions;

using Gmbc.Common.Diagnostics.ExceptionManagement;
namespace Fbg.Bll
{
    /// <summary>
    /// WARNING: THIS CLASS IS DEPRECIATED... USE Gmbc.Common.Configuration.ConfigurationException Exception 
    /// or Exceptions derived from this one
    ///  
    /// Represents a exception that means that a key, which was supposed to be provided
    /// in the applications .config file, is missing or is somehow invalid. See remarks for more info
    /// </summary>
    /// <remarks>
    /// <B>When to use this exception</B>
    /// <p>This exception should be used when ever appropriate but more specifically, with the current pattern 
    /// in GN's code, it should most likely be used in the Config class - this is the class that many applications
    /// which need to read the .config file implement for this purpose. If a key is found to be missing or invalid
    /// this exception should be thrown
    /// </p>
    /// </remarks>
    [Serializable]
    public class ClanNotFoundException : BaseApplicationException
    {
        #region Constructors
        /// <summary>
        /// Basic constructor
        /// </summary>
        public ClanNotFoundException() : base() { }

        /// <summary>
        /// Constructor allowing the Message property to be set.
        /// </summary>
        /// <param name="message">String setting the message of the exception.</param>
        public ClanNotFoundException(string message)
            : base(message)
        {
        }
        /// <summary>
        /// Constructor allowing the Message and InnerException property to be set.
        /// </summary>
        /// <param name="message">String setting the message of the exception.</param>
        /// <param name="inner">Sets a reference to the InnerException.</param>
        public ClanNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Constructor used for deserialization of the exception class.
        /// </summary>
        /// <param name="info">Represents the SerializationInfo of the exception.</param>
        /// <param name="context">Represents the context information of the exception.</param>
        protected ClanNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            requestedClanID = info.GetInt32("requestedClanID");


        }
        public ClanNotFoundException(string message, int clanID)
            : base(message)
        {
            requestedClanID = clanID;
            
        }
        #endregion

        #region Declare Member Variables
        private int requestedClanID;
        #endregion

        #region Public Properties
        /// <summary>
        /// Get/Set the configuration section name that was being read. 
        /// If this does not apply to you, you may leave it blank
        /// </summary>
        public int RequestedClanID
        {
            get
            {
                return requestedClanID;
            }
            set
            {
                requestedClanID = value;
            }
        }
    
        #endregion

        /// <summary>
        /// Used for serialization
        /// </summary>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("requestedClanID", requestedClanID, typeof(int));

            base.GetObjectData(info, context);
        }


    }
}
