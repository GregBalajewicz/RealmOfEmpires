using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for LoginModeHelper
/// </summary>
public class LoginModeHelper
{
    /// <summary>
    /// 
    /// </summary>
	private LoginModeHelper()
	{
	}
    /// <summary>
    /// constants
    /// </summary>
    private const string LT = "lt";
    private const string FB = "fb";
    private const string MOB = "mob";
    private const string BDA = "bda";
    private const string AG = "ag";
    /// <summary>
    /// 
    /// </summary>
    public enum LoginModeEnum
    {
        facebook
        , mobile
        , bda
        , kongregate
        , unknown
        , armoredgames
    }
    public class LoginMethodCookieValues
    {
        public static string fb = "fb";
        public static string bda = "bda";
        public static string kong = "kongregate";
        /// <summary>
        /// Armored Games
        /// </summary>
        public static string armoredgame = "ag";
    }
    /// <summary>
    /// converts login mode enum to strig
    /// </summary>
    /// <param name="loginMode"></param>
    /// <returns></returns>
    public static string ToString(LoginModeEnum loginMode)
    {
        string loginModeString = "";
        switch (loginMode)
        {
            case LoginModeEnum.facebook:
                loginModeString = FB;
                break;
            case LoginModeEnum.mobile:
                loginModeString = MOB;
                break;
            case LoginModeEnum.bda:
                loginModeString = BDA;
                break;
            case LoginModeEnum.armoredgames:
                loginModeString = AG;
                break;
            default:
                throw new ArgumentException("unrecognized value" + loginMode.ToString(), "loginMode");

        }
        return loginModeString;
    }
    /// <summary>
    /// converts loginmode string to Enum
    /// </summary>
    /// <param name="loginModeString"></param>
    /// <returns></returns>
    public static LoginModeEnum? ToEnum(string loginModeString)
    {
        if (loginModeString == ToString(LoginModeEnum.mobile))
        {
            return LoginModeEnum.mobile;
        }
        else
        {
            return null;
        }
    }
    /// <summary>
    /// encapsulate all of this here
    /// </summary>
    /// <param name="loginMode"></param>
    /// <param name="startPage"></param>
    /// <returns></returns>
    public static string GetROELoginUrl(LoginModeEnum loginMode, string startPage)
    {
        return string.Format("roe://login?type={0}&page={1}", LoginModeHelper.ToString(loginMode), startPage);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public static string LoginModeFromRequestQS(HttpRequest request)
    {
        return request.QueryString[LT];
    }
    /// <summary>
    /// tells you what the request tells us we should do. to see actual effective login mode, see LoginMode(...)
    ///
    /// </summary>
    public static LoginModeEnum? LoginModeFromRequest(HttpRequest request)
    {
        // check the QS first .. then look for a cookie
        string lt = LoginModeFromRequestQS(request);
        if (lt == null)
        {
            HttpCookie ltCookie = request.Cookies[LT];
            if (ltCookie != null)
            {
                lt = ltCookie.Value;
            }
        }
        return ToEnum(lt);
    }
    /// <summary>
    /// set cookie in Response
    /// </summary>
    /// <param name="response"></param>
    /// <param name="lt"></param>
    public static void SetLoginModeInResponse(HttpResponse response, string lt)
    {
        if (lt != null)
        {
            HttpCookie ltCookie = new HttpCookie(LT, lt);
            response.Cookies.Add(ltCookie);
        }
    }
    /// <summary>   
    /// </summary>
    public static LoginModeEnum LoginMode(HttpRequest request)
    {
        if (LoginModeFromRequest(request) == LoginModeEnum.mobile)
        {
            return LoginModeEnum.mobile;
        }
        else if (LoginModeFromRequest(request) == null)
        {
            HttpCookie loginMethod = request.Cookies[CONSTS.Cookies.LoginMethod];
            if (loginMethod != null && loginMethod.Value == LoginMethodCookieValues.bda)
            {
                return LoginModeHelper.LoginModeEnum.bda;
            }
            else if (loginMethod != null && loginMethod.Value == LoginMethodCookieValues.fb)
            {
                return LoginModeEnum.facebook;
            }
            else if (loginMethod != null && loginMethod.Value == LoginMethodCookieValues.kong)
            {
                return LoginModeEnum.kongregate;
            }
            else if (loginMethod != null && loginMethod.Value == LoginMethodCookieValues.armoredgame)
            {
                return LoginModeEnum.armoredgames;
            }
            //
            // since we dont have this cookie, lets see if the mobile app created the LT cookie
            //

            HttpCookie ltCookie = request.Cookies[LT];
            if (ltCookie != null)
            {
                if (ltCookie.Value == "bda") // TODO turn into constant
                {
                    return LoginModeEnum.bda;
                }
                else if (ltCookie.Value ==  FB)
                {
                    return LoginModeEnum.facebook;
                }
            }
        }

        //
        // default value; 
        //
        return LoginModeEnum.unknown;
    }
    /// <summary>
    /// short hand to LoginMode(request) == LoginModes.facebook
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public static bool isFB(HttpRequest request)
    {
        return LoginMode(request) == LoginModeEnum.facebook;
    }

    /// <summary>
    /// short hand to LoginMode(request) == LoginModes.mobileonly;
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public static bool isMob(HttpRequest request)
    {
        return LoginMode(request) == LoginModeEnum.mobile;
    }
    /// <summary>
    /// short hand to LoginMode(request) == LoginModeEnum.bdaaccount;
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public static bool isBDA(HttpRequest request)
    {
        return LoginMode(request) == LoginModeEnum.bda;
    }
    /// <summary>
    /// short hand to LoginMode(request) == LoginModeEnum.bdaaccount;
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public static bool isKongregate(HttpRequest request)
    {
        return LoginMode(request) == LoginModeEnum.kongregate;
    }
    /// <summary>
    /// short hand to LoginMode(request) == LoginModeEnum.armoredgames;
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public static bool isArmoredGames(HttpRequest request)
    {
        return LoginMode(request) == LoginModeEnum.armoredgames;
    }
}
