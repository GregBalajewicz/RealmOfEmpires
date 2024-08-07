using System;
using System.Collections.Generic;
using System.Text;

namespace Facebook.Forms
{
    /// <summary>
    /// This class is only needed because the ToString method in the original
    /// URI class prints Unicode characters instead of their http-escaped
    /// versions. Since the .Net Compact Framework can only support the URI
    /// constructor for the WebBrowser control, we have to use this new URI
    /// class. 
    /// </summary>
    public class UnicodeUri : Uri
    {
        public override string ToString()
        {
            return this.Host + this.PathAndQuery;
        }

        public UnicodeUri(string url)
            : base(url)
        {            
        }

    }
}
