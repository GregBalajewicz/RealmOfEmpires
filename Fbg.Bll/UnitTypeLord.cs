using System;
using System.Data;
using System.Collections.Generic;
using System.Text;

using Gmbc.Common.Diagnostics.ExceptionManagement;

namespace Fbg.Bll
{
    public class UnitTypeLord : UnitType 
    {
        public static int LordUnitID = 10;


        public UnitTypeLord(Realm realmRef, DataSet dsRef, DataRow drUnitType, DataRow[] drsDefense, DataRow[] drsAttackableBuildings)
            : base(realmRef, dsRef, drUnitType, drsDefense, drsAttackableBuildings)
        {
            if (ID != UnitTypeLord.LordUnitID)
            {
                throw new ArgumentException("the drUnitType parameter should present the a lord unit type but its ID is:" + ID.ToString());
            }
        }

       
       /// <summary>
       /// get the cost of the next governer chests required
       /// this function call DB each time
       /// </summary>
       /// <param name="player"></param>
       /// <returns></returns>
        public int CostInChests(Player player)
        {
            
             return  DAL.Units.GetLordCostMultiplier(_realmRef.ConnectionStr, player.ID);
          
        }
    }
}


