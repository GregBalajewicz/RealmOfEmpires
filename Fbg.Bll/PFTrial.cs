using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Fbg.Bll
{
    public class PFTrial
    {
        DataRow _dr;

        public PFTrial(DataRow dr)
        {
            _dr = dr;
        }

        public Fbg.Bll.CONSTS.PFTrials ID
        {
            get
            {
                return (Fbg.Bll.CONSTS.PFTrials)_dr[Realm.CONSTS.PFTrailsColIndex.PFTrialID];
            }
        }
        public int Duration
        {
            get
            {
                return (int)_dr[Realm.CONSTS.PFTrailsColIndex.Duration];
            }
        }

    }
}
