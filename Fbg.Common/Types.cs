using System;
using System.Collections.Generic;
using System.Text;

namespace Fbg.Common
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>it is critcal that female=0, M=2, U=3 as these correspond to DB values. Do not change</remarks>
    public enum Sex : short
    {
        Female=0,
        Male=1,
        Unknown=2
    }


    public enum TimeOfDay
    {
        day,
        night
    }

    /// <summary>
    /// a replacement for Fbg.Common.Utils.LoginType
    /// </summary>
    public enum UserLoginType : short
    {
        Unknown = 0,
        FB = 1,
        Bda = 2,
        Kong = 3,
        Mobile_Android = 4,
        Mobile_iOS = 5,
        Mobile_Amazon = 6,
        Mobile_Unknown = 7,
        FB_inferred = 8,
        Kong_inferred = 9,
        Mobile_Android_inferred = 10,
        Mobile_iOS_inferred = 11,
        Mobile_Amazon_inferred = 12,
        ArmoredGames=13,
    }

}
