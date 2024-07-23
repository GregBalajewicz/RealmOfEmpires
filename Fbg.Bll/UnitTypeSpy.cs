using System;
using System.Data;
using System.Collections.Generic;
using System.Text;

using Gmbc.Common.Diagnostics.ExceptionManagement;

namespace Fbg.Bll
{
    public class UnitTypeSpy : UnitType
    {
        public static int SpyUnitID = 12;


        public UnitTypeSpy(Realm realmRef, DataSet dsRef, DataRow drUnitType, DataRow[] drsDefense, DataRow[] drsAttackableBuildings)
            : base(realmRef, dsRef, drUnitType, drsDefense, drsAttackableBuildings)
        {
            if (ID != SpyUnitID)
            {
                throw new ArgumentException("the drUnitType parameter should present the a lord unit type but its ID is:" + ID.ToString());
            }
        }
    }
}


