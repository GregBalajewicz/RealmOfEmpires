using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Fbg.Bll
{
    public class BuildingTypeHQ : BuildingType
    {

        public BuildingTypeHQ(Realm realmRef, DataSet dsRef, DataRow drBuildingType) :base(realmRef,dsRef,drBuildingType)
        {
        }
    }
}
