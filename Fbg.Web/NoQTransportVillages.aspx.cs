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

public partial class NoQTransportVillages : MyCanvasIFrameBasePage
{
    private Fbg.Bll.VillageBasicB _village;
    new protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);

        main mainMasterPage = (main)this.Master;
        mainMasterPage.Initialize(FbgPlayer, MyVillages);
         _village = mainMasterPage.CurrentlySelectedVillageBasicB;
        if (!IsPostBack)
        {
            BindList();
        }

        #region localzing some controls
        //
        // this is needed for localiztion, so that <%# ... %> work
        //
        if (!IsPostBack)
        {
            this.btn_Add.DataBind();
        }
        #endregion
    }
    void BindList()
    {
        DataSet ds = Fbg.Bll.CoinTransport.GetNoTransportCoinsVillages(FbgPlayer);
        cmb_VillagesList.Items.Clear();
        foreach(DataRow dr in ds.Tables [Fbg.Bll.CoinTransport.CONSTS.NoQTransportCoinsTableIndex.YesVillages].Rows )
        {
            cmb_VillagesList.Items.Add(
                new ListItem(string.Format("{0} - ({1},{2})"
                    ,(string)dr[Fbg.Bll.CoinTransport.CONSTS.YesVillagesCloumnIndex.VillageName]
                    ,dr[Fbg.Bll.CoinTransport.CONSTS.YesVillagesCloumnIndex.VillageXCord ].ToString()
                    ,dr[Fbg.Bll.CoinTransport.CONSTS.YesVillagesCloumnIndex.VillageYCord].ToString())
                ,dr[Fbg.Bll.CoinTransport.CONSTS.YesVillagesCloumnIndex.VillageID].ToString ()));
        }

        int villageID = _village.id;
        if (ds.Tables[Fbg.Bll.CoinTransport.CONSTS.NoQTransportCoinsTableIndex.YesVillages].Select(Fbg.Bll.CoinTransport.CONSTS.NoQTransportCoinsCloumnNames.VillageID  +"="+ villageID.ToString()).Length != 0)
        {
            cmb_VillagesList.SelectedValue = villageID.ToString();
        }

        gv_NoList.DataSource = ds.Tables[Fbg.Bll.CoinTransport.CONSTS.NoQTransportCoinsTableIndex.NoVillages];
        gv_NoList.DataBind();
    }
    protected string BindVillage(object dataItem)
    {
        string villageName = (string)DataBinder.Eval(dataItem, "Name");
        int xCord = (int)DataBinder.Eval(dataItem, "XCord");
        int yCord = (int)DataBinder.Eval(dataItem, "YCord");

        return string.Format("{0} - ({1},{2})", villageName, xCord, yCord);

    }
    protected void gv_NoList_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        if (((GridView)sender).DataKeys.Count > 0)
        {
            int VillageID= Convert.ToInt32 (((GridView)sender).DataKeys[e.RowIndex].Value);

            Fbg.Bll.CoinTransport.DeleteFromNoTransportsCoinsList(FbgPlayer, VillageID); 
            BindList();           
        }
    }
    protected void btn_Add_Click(object sender, EventArgs e)
    {
        if (!String.IsNullOrEmpty(cmb_VillagesList.SelectedValue))
        {

            int VillageID = Convert.ToInt32(cmb_VillagesList.SelectedValue);
            Fbg.Bll.CoinTransport.AddToNoTransportsCoinsList(FbgPlayer, VillageID);
            BindList();
        }
       
    }
    protected void gv_NoList_OnPageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gv_NoList.PageIndex = e.NewPageIndex ;
        BindList();
    }
}
