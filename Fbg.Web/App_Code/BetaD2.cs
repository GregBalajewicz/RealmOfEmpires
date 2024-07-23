using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Amazon.SimpleEmail.Model;
using Amazon.SimpleEmail;

using Gmbc.Common.Diagnostics.ExceptionManagement;

public class BetaD2
{
    public static int D1OnlyRealm = -999;

    public static bool isD2OnlyRealm(int realmID)
    {
        return true;
    }

    //public static bool canenter(HttpContext context, Fbg.Bll.User u)
    //{
    //    return canenter(context.User.IsInRole("Admin") || context.User.IsInRole("tester"), u);
    //}
    public static bool canenterD2(HttpContext context, Fbg.Bll.User u, Fbg.Bll.Realm r, DateTime registeredOn)
    {
        return true;

    }

    public static bool canSeeRealmInListOfRealms(bool isAdminOrTester, Fbg.Bll.User u, Fbg.Bll.Realm r)
    {

        return true;

    }

    public static bool canenterD2(bool isAdminOrTester, Fbg.Bll.User u, Fbg.Bll.Realm r, DateTime registeredOn)
    {
        return true;
    }
    public static bool canenterD2(bool isAdminOrTester, Fbg.Bll.User u)
    {
        return true;
    }

    public static bool isEarlyTester(Fbg.Bll.User u)
    {
        return false;
    }

    public static bool canEnterD1(bool isAdminOrTester, DateTime registeredOn)
    {
        return false;
    }



}