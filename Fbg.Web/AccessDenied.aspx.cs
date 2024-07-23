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


    public partial class AccessDenied : MyCanvasIFrameBasePage
   {
      protected void Page_Load(object sender, EventArgs e)
      {
          main mainMasterPage = (main)this.Master;
          mainMasterPage.Initialize(FbgPlayer, MyVillages);
          ClanMenu1.Player = FbgPlayer;

          #region localzing some controls
          //
          // this is needed for localiztion, so that <%# ... %> work
          //
          if (!IsPostBack)
          {
              this.lbl_Error.DataBind();
          }
          #endregion 
      }
   }
