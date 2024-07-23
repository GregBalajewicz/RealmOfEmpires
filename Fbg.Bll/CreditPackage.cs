using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Fbg.Bll
{
    public class CreditPackage
    {
        DataRow _dr;

        public CreditPackage(DataRow dr)
        {
            _dr = dr;
        }

        public int ID
        {
            get
            {
                return (int)_dr[Realms.CONSTS.CreditPackages.CreditsPackageID];
            }
        }
        public int Credits
        { 
            get
            {
                return (int)_dr[Realms.CONSTS.CreditPackages.Credits];
            }
        }
        public double RealCost
        {
            get
            {
                return Convert.ToDouble(_dr[Realms.CONSTS.CreditPackages.RealmCost]);
            }
        }
    }

    public class CreditPackageDevice
    {
        public string ProductID {get; set;}
        public int Credits{get; set;}
        public double Price { get; set; }
        public int SaleType { get; set; }
        public string Icon { get; set; }
        public int DeviceType { get; set; }

        public CreditPackageDevice(DataRow dr)
        {
            ProductID = (string)dr[0];
            Credits = (int)dr[1];
            SaleType = (int)dr[2];
            Price = (double)dr[3];
            string iconID = "";
            try
            {
                iconID = ProductID.Substring(ProductID.LastIndexOf(".") + 1);
            }
            catch { }
            Icon = String.Format("https://static.realmofempires.com/images/M_BuyProductID_{0}.png", iconID);
            DeviceType = (int)dr[4];
        }      
    }
}
