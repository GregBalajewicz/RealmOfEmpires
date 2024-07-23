using System;
using System.Collections.Generic;
using System.Text;

namespace Fbg.Common
{
    public class PFs
    {
        public enum RefundTypes : int
        {
            ForDepreciatedPackage = 0,
            ForActivePackage = 1,
            /// <summary>
            /// used when activating NP and wnat to refund all features beloning to is activated as part of it
            /// </summary>
            ForNobilityPackage=2
            
        }
    }
}
