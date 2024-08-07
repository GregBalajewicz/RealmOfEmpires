using System;
#if !NETCF
using System.Runtime.Serialization;
#endif

namespace Facebook.Exceptions
{
    /// <summary>
    /// Exception returned for ERRORNO 210, 220 or 221
    /// </summary>
    [Serializable]
    public class FacebookNotVisibleException : FacebookException
    {
        /// <summary>
        /// Empty constructor.
        /// </summary>
        public FacebookNotVisibleException()
            : base()
        { }

        /// <summary>
        /// Constructor with Error Message.
        /// </summary>
        public FacebookNotVisibleException(string message)
            : base(message)
        { }

        /// <summary>
        /// Exception constructor with a custom message after catching an exception.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Exception caught.</param>
        public FacebookNotVisibleException(string message, Exception innerException)
            : base(message, innerException)
        { }

#if !NETCF
        /// <summary>
        /// Constructor used for serialization.
        /// </summary>
        /// <param name="si">The info.</param>
        /// <param name="sc">The context.</param>
        protected FacebookNotVisibleException(SerializationInfo si, StreamingContext sc)
            : base(si, sc)
        { }
#endif

    }
}
