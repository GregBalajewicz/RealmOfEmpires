using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Fbg.Bll
{
    public class BuildingTypeTP : BuildingType
    {

        public BuildingTypeTP(Realm realmRef, DataSet dsRef, DataRow drBuildingType) :base(realmRef,dsRef,drBuildingType)
        {
        }
    }
}
