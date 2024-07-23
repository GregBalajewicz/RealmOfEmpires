using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

/// <summary>
/// Summary description for SessionCache
/// </summary>
public class SessionCache
{
    static private HttpSessionState _session
    {
        get
        {
            return System.Web.HttpContext.Current.Session;
        }
    }

    public static void put(string objectName, long msToCacheFor)
    {
        _session[CONSTS.Session.SessionCachePrefix + objectName + "_timeIn"] = DateTime.Now;
        _session[CONSTS.Session.SessionCachePrefix + objectName + "_msToCacheFor"] = msToCacheFor;
    }

    public static bool stillInCache(string objectName)
    {
        if (_session[CONSTS.Session.SessionCachePrefix + objectName + "_timeIn"] != null)
        {
            DateTime timeIn = (DateTime)_session[CONSTS.Session.SessionCachePrefix + objectName + "_timeIn"];
            long msToCacheFor = (long)_session[CONSTS.Session.SessionCachePrefix + objectName + "_msToCacheFor"];
            if (timeIn.AddMilliseconds(msToCacheFor) > DateTime.Now)
            {
                return true;
            }
        }
        return false;
    }

    public static long minBeforeExpires(string objectName)
    {
        if (_session[CONSTS.Session.SessionCachePrefix + objectName + "_timeIn"] != null)
        {
            DateTime timeIn = (DateTime)_session[CONSTS.Session.SessionCachePrefix + objectName + "_timeIn"];
            long msToCacheFor = (long)_session[CONSTS.Session.SessionCachePrefix + objectName + "_msToCacheFor"];
            return Convert.ToInt64(timeIn.AddMilliseconds(msToCacheFor).Subtract(DateTime.Now).TotalMilliseconds);
        }
        return 0;
    }
}