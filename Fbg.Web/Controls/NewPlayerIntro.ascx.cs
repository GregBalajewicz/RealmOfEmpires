using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Fbg.Bll;

public partial class Controls_NewPlayerIntro : BaseControl
{
    public Controls_NewPlayerIntro()
    {
        BaseResName = "NewPlayerIntro.ascx";
    }
    protected void Page_Load(object sender, EventArgs e)
    {

    } 

    private  static class Consts
    {
        public static string CookieName = "QI";
        public static string Cookie_isRunning = "isRunning";
        public static string Cookie_isRunning_Yes = "1";
        public static string Cookie_isRunning_No = "0";
        public static string Cookie_curStep = "curStep";
        public static string CookieTutorialPosName = "tutPos";
    }

    /// <summary>
    /// The tutorial displays these steps in this order. 
    /// </summary>
    public enum Steps : int
    {
        s1 = 0
        ,
        s2
      ,
        s3
        , done
    }
    
    private Steps currentStep = Steps.done;

  

    /// <summary>
    /// Hides the tutorial so that it is not displayed 
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    public void Hide()
    {
        this.Visible = false;
    }

    public Player _player;

    /// <summary>
    /// returns true if tutorial is running 
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    public void DisplayCurrentStep(Player player)
    {
        _player = player;
        panelS1.Visible = false;
        panelS2.Visible = false;
        panelS3.Visible = false;
        HttpCookie cookie = Request.Cookies[Consts.CookieName];
        if (cookie != null)
        {
            currentStep = (Steps)Convert.ToInt32(cookie.Values[Consts.Cookie_curStep]);
        }

        this.Visible = true;

            switch (currentStep)
            {
                case Steps.done:
                    this.Visible = false;
                    break;
                case Steps.s1:
                    panelS1.Visible = true;
                    break;
                case Steps.s2:
                    panelS2.Visible = true;
                    break;
                case Steps.s3:
                    panelS3.Visible = true;
                    footer.Visible = false;
                    End();
                    break;

                default:
                    break;
            }

            linkNext.CommandName = currentStep.ToString();

    }

    public bool isRunning
    {
        get
        {
            HttpCookie cookie = Request.Cookies[Consts.CookieName];
            if (cookie != null)
            {
                currentStep = (Steps)Convert.ToInt32(cookie.Values[Consts.Cookie_curStep]);
                if (cookie.Values[Consts.Cookie_isRunning] == Consts.Cookie_isRunning_Yes)
                {
                    return true;
                }
            }
            return false;
        }
    }


    public void Start()
    {
        Start(Steps.s1);
    }

    public void Start(Steps step)
    {
        HttpCookie cookie = Request.Cookies[Consts.CookieName];
        if (cookie == null)
        {
            cookie = new HttpCookie(Consts.CookieName);
        }
        currentStep = Steps.s1;
        cookie.Expires = DateTime.Now.AddDays(10);
        cookie.Values.Clear();
        cookie.Values.Add(Consts.Cookie_curStep, ((int)step).ToString());
        Response.Cookies.Add(cookie);
    }


    /// <summary>
    /// call this if you want to 'start' the tutorial without starting it immediatelly. 
    /// This will just create a cookie that will tell the tutorial that it is runinig and it 
    /// will be displayed on a page that uses the tutorial. 
    /// 
    /// THis also start the tutorial from stop 0 - bookmark app in FB. 
    /// </summary>
    public void CreateRunningCookieFromStart()
    {
        HttpCookie cookie;
        currentStep = Steps.s1;
        cookie = new HttpCookie(Consts.CookieName);
        cookie.Expires = DateTime.Now.AddDays(10);
        cookie.Values.Add(Consts.Cookie_curStep, ((int)Steps.s1).ToString());
        Response.Cookies.Add(cookie);
    }


    protected void GoNext()
    {
        if (currentStep != Steps.done)
        {
            GoTo((Steps)((int)currentStep + 1));
        }
        else
        {
            End();
            this.Visible = false;
        }
    }

    public void End()
    {
        HttpCookie cookie = Request.Cookies[Consts.CookieName];
        if (cookie == null)
        {
            cookie = new HttpCookie(Consts.CookieName);
        }
        cookie.Expires = DateTime.Now.AddDays(-10);
        Response.Cookies.Add(cookie);
    }

    protected void GoTo(Steps step)
    {
        currentStep = step;
        HttpCookie cookie = Request.Cookies[Consts.CookieName];
        if (cookie != null)
        {
            cookie.Values[Consts.Cookie_curStep] = ((int)currentStep).ToString();
            cookie.Expires = DateTime.Now.AddDays(10);
            Response.Cookies.Add(cookie);
        }
    }

    protected void linkNext_Click(object sender, EventArgs e)
    {
        GoNext();
        DisplayCurrentStep(_player);
    }


}
