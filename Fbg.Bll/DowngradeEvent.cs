using System;
using System.Collections.Generic;
using System.Text;

namespace Fbg.Bll
{
    public abstract class DowngradeEvent
    {
        public DowngradeEvent( BuildingType bt, DateTime completionTime)
        {
            this.bt = bt;
            this.completionTime = completionTime;
        }
        public BuildingType bt;
        public DateTime completionTime;
    }


    public class DowngradeEvent_CurrentlyDowngrading : DowngradeEvent
    {
        public DowngradeEvent_CurrentlyDowngrading(long eventID, BuildingType bt, DateTime completionTime)
            : base(bt, completionTime)
        {
            this.eventID = eventID;
        }
        public long eventID;
    }

    public class DowngradeEvent_DowngradeInQ :DowngradeEvent
    {
        public DowngradeEvent_DowngradeInQ(long qEntryID, BuildingType bt, DateTime completionTime)
            : base(bt, completionTime)
        {
            this.qEntryID = qEntryID;
        }
        public long qEntryID;
    }
}
