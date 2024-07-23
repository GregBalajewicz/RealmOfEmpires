using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Fbg.Bll
{
    public class BattleHandicap
    {
        public bool IsActive { get; internal set; }
        public int Param_StartRatio { get; set; }
        public int Param_Steepness { get; set; }
        public double Param_MaxHandicap { get; set; }

        public BattleHandicap()
        {
            IsActive = false;
            Param_MaxHandicap = 0.5;
            Param_StartRatio = 4;
            Param_Steepness = 1;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="attackersPoints"></param>
        /// <param name="defendersPoints">ignore if villageType is rebel of abandoned</param>
        /// <param name="villageType"></param>
        /// <returns>number between 0 and 1 </returns>
        public double CalcBattleHandicap(int attackersPoints, int defendersPoints, Fbg.Bll.CONSTS.VillageType villageType)
        {
         

            double actualHandicap = 0;
            if (IsActive)
            {
                if (villageType == Fbg.Bll.CONSTS.VillageType.Abandoned || villageType == Fbg.Bll.CONSTS.VillageType.Rebel)
                {
                    defendersPoints = 100000000; //hack, no handicap on those, so setting to ridiculously large number
                }

                if (attackersPoints <= 0 || defendersPoints <= 0)
                {
                    actualHandicap = 0;
                }
                else
                {
                    double ratio = attackersPoints / (double)defendersPoints;
                    if (ratio <= Param_StartRatio)
                    {
                        actualHandicap = 0;
                    }
                    else
                    {
                        //double logValB = 2 * Math.Log10(ratio - 3);

                        //actualHandicap = 0.5 - 0.5 * Math.Pow(4, -0.25 * ((logValB * logValB)));
                        double logValB = 2 * Math.Log10(ratio - Param_StartRatio + 1);

                        actualHandicap = Param_MaxHandicap - Param_MaxHandicap * Math.Pow(Param_StartRatio, -0.25 * Param_Steepness * ((logValB * logValB)));
                    }

                }
            }
            return actualHandicap;
        }

    }
}
