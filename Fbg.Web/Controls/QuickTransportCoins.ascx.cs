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
using Fbg.Common;
using Gmbc.Common.Diagnostics.ExceptionManagement;

public partial class QuickTransportCoins : BaseControl
{
    public delegate void CoinsUpdateNeededDelegate(int AmountOfCoinsTransported);
    public event CoinsUpdateNeededDelegate CoinsUpdateNeededFoundEvent;

    private Village _selectedvillage;
    public  Fbg.Bll.Player _player;
    private bool isInitialized = false;
    List<VillageBasicA> _villages = null;
    DataView _dv_QTransports;
    /// <summary>
    /// true if player has the nececssary PF to execute this. 
    /// </summary>
    public bool playerHasPF;
    const int DEFAULT_MIN_TRANSPORT_PER_VILLAGE = 1000;

    public QuickTransportCoins()
    {
        BaseResName = "QuickTransportCoins.ascx"; 
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Image1.ImageUrl = VillageOverviewImages.GetBuildingImageForHelpScreen(Fbg.Bll.CONSTS.BuildingIDs.TradePost);

    }
    public bool IsMobile { get; set; }
    public bool IsD2 { get; set; }
    public void Initialize(Fbg.Bll.Player player, Village selectedVillage, List<VillageBasicA> villages, bool isMobile, bool isD2)
    {
        IsMobile = isMobile;
        IsD2 = isD2;
        if (player == null)
        {
            throw new ArgumentNullException("Fbg.Bll.Player player");
        }
        if (selectedVillage == null)
        {
            throw new ArgumentNullException("Village selectedVillage");
        }

        _player = player;
        _selectedvillage = selectedVillage;
        _villages = villages;

        playerHasPF = player.PF_HasPF(Fbg.Bll.CONSTS.PFs.ConvenientSilverTransport);

        isInitialized = true;

        if (playerHasPF)
        {
            panelHasPF.Visible = true;
            panelNoPF.Visible = false;

            lblCurVillage.Text = String.Format("{0} ({1},{2})", selectedVillage.name
                , selectedVillage.Cordinates.X
                , selectedVillage.Cordinates.Y);

            BindList();
        }
        else
        {
            LinkButton2.NavigateUrl = NavigationHelper.PFOffMessage(Fbg.Bll.CONSTS.PFs.ConvenientSilverTransport);
            HyperLink2.NavigateUrl = NavigationHelper.PFOffMessage(Fbg.Bll.CONSTS.PFs.ConvenientSilverTransport);

            linkLocked.NavigateUrl = NavigationHelper.PFOffMessage(Fbg.Bll.CONSTS.PFs.ConvenientSilverTransport);
            linkLocked.Visible = true;
        }

        #region localzing some controls
        //
        // this is needed for localiztion, so that <%# ... %> work
        //
        if (!IsPostBack)
        {
            this.lnk_GetMax.DataBind();
        }
        #endregion 
    }
     
 
    private void BindList()
    {
        lbl_Message.Visible = false;
        lnk_GetMax.Text = "";
        pnl_TransportList.Visible = false;
        pnl_NoTransports.Visible = true;
        int minAmountPerVillage;
        int firstXVillages;

        if (!IsPostBack)
        {
            //by default, do not show village with less then 1000 to transport
            minAmountPerVillage = DEFAULT_MIN_TRANSPORT_PER_VILLAGE;
            firstXVillages = 20;
        }
        else
        {
            //on post back, if we are showing the message that min is 1000, then only get 1000 min
            minAmountPerVillage = panelShowVillagesWith1000Silver.Visible ? DEFAULT_MIN_TRANSPORT_PER_VILLAGE : 0;
            firstXVillages = panelShow20VillsOnly.Visible ? 20 : 1000;
        }


        if (!isInitialized)
        {
            throw new Exception("Initialize not called. Must first call QuickTransportCoins.Initialize");
        }
        try
        {
            if (_player.NumberOfVillages > 1)
            {

                DataTable dt = Fbg.Bll.CoinTransport.GetNearestVillages(_player, _selectedvillage, _villages, minAmountPerVillage, firstXVillages);
                _dv_QTransports = new DataView(dt);
                int maxCoins = Fbg.Bll.CoinTransport.GetMaxCoins(_dv_QTransports, _selectedvillage);

                dl_NearestVillages.DataSource = _dv_QTransports;
                dl_NearestVillages.DataBind();

                if (maxCoins > 0)
                {
                    pnl_TransportList.Visible = true;
                    pnl_NoTransports.Visible = false;
                    if (playerHasPF)
                    {
                        lnk_GetMax.Text = RS("GETMAX") + " " + Utils.FormatCost(maxCoins) + " " + RS("FromClosestVillages");
                    }
                    else
                    {
                        linkGetMax_NoPF.NavigateUrl = NavigationHelper.PFOffMessage(Fbg.Bll.CONSTS.PFs.ConvenientSilverTransport);
                        linkGetMax_NoPF.Text = RS("GETMAX") + " " + Utils.FormatCost(maxCoins) + " " + RS("FromClosestVillages");
                        linkGetMax_NoPF.Visible = true;
                        lnk_GetMax.Visible = false;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            BaseApplicationException be = new BaseApplicationException("Error Bind Quick Transport", ex);
            be.AddAdditionalInformation("_selectedvillage", _selectedvillage);

            throw be;
        }
   
    }
    protected string BindAmount(object dataItem)
    {
        int MinAmount = (int)DataBinder.Eval(dataItem, "MinAmount");
        int VillageID = (int)DataBinder.Eval(dataItem, "VillageID");
        //int ret=GetMinAmount(MinAmount, VillageID);
        if (MinAmount == 0)
        {
            return "";
        }
        else
        {
            return MinAmount.ToString();
        }
        

    }
    private int GetMinAmount(int MinAmount, int VillageID)
    {
        VillageBasicA v = _player.VillageBasicA(VillageID, _villages);

        // v.coins ,MinAmount,treasury size
        int min1 = Math.Min(v.coins, MinAmount);
        return Math.Min(min1, _selectedvillage.TreasurySize - _selectedvillage.coins);
    }
    protected string BindVillages(object dataItem)
    {
        int MinAmount = (int)DataBinder.Eval(dataItem, "MinAmount");
        long  duration = (long )DataBinder.Eval(dataItem, "TravelTime");
        TimeSpan TravelTime = new TimeSpan(duration);
        int XCord = (int)DataBinder.Eval(dataItem, "XCord");
        int YCord = (int)DataBinder.Eval(dataItem, "YCord");
        string  VillageName = (string)DataBinder.Eval(dataItem, "Name");
        int VillageID = (int)DataBinder.Eval(dataItem, "VillageID");
        //int Coins = (int )DataBinder.Eval(dataItem, "Coins");
         string Time;
         if (TravelTime.TotalSeconds <= 0)
        {
            //TODO - this should really not happen. ..perhasp we should sleep an second or two now? allow time for the event
            //  to get processed? add a refresh meta tag ?
             Time=  "0:00:00";
        }
        else
        {
            Time = Utils.FormatDuration(TravelTime);
            
        }
       // int amount = GetMinAmount(MinAmount, VillageID);
        if (MinAmount > 0)
        {
           // ShowGrid = true;
            return RS("Get") + " " + Utils.FormatCost(MinAmount  ) + " " + RS("From") + " " + VillageName + "(" + XCord + "," + YCord + ") " + RS("In") + " " + Time;
        }
        else
        {
            return null;
        }

    }
    protected void dl_NearestVillages_ItemCommand(object source, DataListCommandEventArgs e)
    {
        if (!isInitialized)
        {
            throw new Exception("Must first call QuickTransportCoins.Initialize");
        }

        if (playerHasPF)
        {
            int villageID = 0;
            int amountToTransport = 0;
            VillageBasicA sendFromVillage;
            int CurrentAmount = 0;
            try
            {
                villageID = int.Parse(e.CommandArgument.ToString());
                Int32.TryParse(e.CommandName, out amountToTransport);

                if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("lbl_MinAmount")).Text))
                {
                    amountToTransport = Convert.ToInt32(((Label)e.Item.FindControl("lbl_MinAmount")).Text);

                    sendFromVillage = _player.VillageBasicA(villageID, _villages);
                    if (amountToTransport > 0)
                    {
                        TransportResult result = Fbg.Bll.CoinTransport.Transport(_player
                            , sendFromVillage.id
                            , _selectedvillage.id
                            , sendFromVillage.Cordinates.X
                            , sendFromVillage.Cordinates.Y
                            , _selectedvillage.Cordinates.X
                            , _selectedvillage.Cordinates.Y
                            , amountToTransport
                            , amountToTransport
                            , true);
                        if (result == TransportResult.Success)
                        {
                            _selectedvillage.coins += amountToTransport;
                            sendFromVillage.coins -= amountToTransport;

                            BindList();

                            //this part to update coins immediatly in the label
                            if (CoinsUpdateNeededFoundEvent != null)
                            {
                                CoinsUpdateNeededFoundEvent(amountToTransport);
                            }

                            lbl_Message.Visible = true;
                            lbl_Message.Text = GetMessage(result);
                        }
                        else
                        {
                            //throw new Exception(result.ToString());
                            // we eat the error
                        }

                    }
                }


            }
            catch (Exception ex)
            {
                BaseApplicationException be = new BaseApplicationException("Error Get Quick Transport", ex);
                be.AdditionalInformation.Add("villageID", villageID.ToString());
                be.AdditionalInformation.Add("amountToTransport", amountToTransport.ToString());
                be.AdditionalInformation.Add("CurrentAmount", CurrentAmount.ToString());

                throw be;
            }
        }

    }
    protected string GetMessage(TransportResult result)
    {
        string msg = string.Empty;
        switch (result)
        {
            case TransportResult.Coins_More_then_Allowed :
            case TransportResult.Coins_must_be_greater_then_Zero :
                msg = RS("TransportNoLongerPossible");
                break;
            case TransportResult.Only_Numbers_Accepted :
                msg = RS("OnlyNumbersAccepted");
                break;
            case TransportResult.Success:
                msg = RS("TransportSent");
                break;
            default:
                throw new Exception("Unrecognized value of TransportResult:" + result.ToString());
        }
        return msg;
    }
    protected void lnk_GetMax_Click(object sender, EventArgs e)
    {

        if (playerHasPF)
        {
            Fbg.Bll.CoinTransport.DoGetMaxCoins(_player, _dv_QTransports, _selectedvillage);
            int maxcoins = Fbg.Bll.CoinTransport.GetMaxCoins(_dv_QTransports, _selectedvillage);
            _selectedvillage.coins += maxcoins;

            if (CoinsUpdateNeededFoundEvent != null)
            {

                CoinsUpdateNeededFoundEvent(maxcoins);
            }
            BindList();

            lbl_Message.Visible = true;
            lbl_Message.Text = RS("TransportsSent");
        }
    }
    protected void linkNoLimitTo1000Silver_Click(object sender, EventArgs e)
    {
        panelShowVillagesWith1000Silver.Visible = false;
        BindList();
    }
    protected void linkNoLimitTo20Vills_Click(object sender, EventArgs e)
    {
        panelShow20VillsOnly.Visible = false;
        BindList();
    }
}
