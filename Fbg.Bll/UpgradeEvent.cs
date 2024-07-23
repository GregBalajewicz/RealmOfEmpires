using System;
using System.Collections.Generic;
using System.Text;

namespace Fbg.Bll
{
    public abstract class UpgradeEvent
    {
        public UpgradeEvent(long ID, BuildingTypeLevel upgradeToLevel, DateTime completionTime)
        {
            this.upgradeToLevel = upgradeToLevel;
            this.completionTime = completionTime;
            this.ID = ID;
        }
        public BuildingTypeLevel upgradeToLevel;
        public DateTime completionTime;
        public long ID;
    }


    public class UpgradeEvent_CurrentlyUpgrading : UpgradeEvent
    {
        public UpgradeEvent_CurrentlyUpgrading(long eventID, BuildingTypeLevel upgradeToLevel, DateTime completionTime)
            : base(eventID, upgradeToLevel, completionTime)
        {
            this.eventID = eventID;
        }
        public long eventID;
    }

    public class UpgradeEvent_UpgradeInQ :UpgradeEvent
    {
        public UpgradeEvent_UpgradeInQ(long qEntryID, BuildingTypeLevel upgradeToLevel, DateTime completionTime)
            : base(qEntryID, upgradeToLevel, completionTime)
        {
            this.qEntryID = qEntryID;
        }
        public long qEntryID;
    }
}
