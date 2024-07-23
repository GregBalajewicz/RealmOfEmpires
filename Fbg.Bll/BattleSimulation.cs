using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data; 

namespace Fbg.Bll
{ 
    public class BattleSimulation 
    {  
        private  const int UnitSpyID=Fbg.Bll.CONSTS.UnitIDs.Spy;

        
        /// <summary>
        /// </summary>
        /// <param name="ATs"></param>
        /// <param name="DTs"></param>
        /// <param name="BAs"> what do I pass here is building attacks???</param>
        /// <param name="spySucessChance">return number between 1 and 0. 1=100%, 0=0%, 0.5=50%</param>
        /// <param name="spyIdentityKnownChance">return number between 1 and 0. 1=100%, 0=0%, 0.5=50%</param>
        /// <param name="spyAttackVisibleChance">return number between 1 and 0. 1=100%, 0=0%, 0.5=50%</param>
        /// <param name="CurrentPlayer">DO NOT TAKE IN PLAYER but REALM object instead</param>
        /// <param name="distanceToTarget">distance to target in squares</param>
        public static void Simulate(ref List<AttackingTroops> ATs
            , ref  List<DefendingTroops> DTs
            , ref  List<BuildingAttacks> BAs
            , ref double spySucessChance
            , ref double spyIdentityKnownChance
            , ref double spyAttackVisibleChance
            , ref bool spyExists
            , Player CurrentPlayer
            , float handicap
            , double distanceToTarget
            , bool isAttackBouns
            , bool isDefendBouns
            , out double desertionFactor
            , float defendersResearchVillageDefenseBonus
            , float attackersOffensiveResearchBonus)

        {
            Simulate(ref ATs, ref DTs, ref BAs, ref spySucessChance, ref spyIdentityKnownChance
            , ref spyAttackVisibleChance
            , ref spyExists
            , CurrentPlayer
            , handicap
            , distanceToTarget
            , isAttackBouns
            , isDefendBouns
            , 0
            , 0
            , out desertionFactor
            , defendersResearchVillageDefenseBonus
            , attackersOffensiveResearchBonus
            , CurrentPlayer.Realm.Morale.MaxMorale_Normal);
        }

        /// <summary>
        /// </summary>
        /// <param name="ATs"></param>
        /// <param name="DTs"></param>
        /// <param name="BAs"> what do I pass here is building attacks???</param>
        /// <param name="spySucessChance">return number between 1 and 0. 1=100%, 0=0%, 0.5=50%</param>
        /// <param name="spyIdentityKnownChance">return number between 1 and 0. 1=100%, 0=0%, 0.5=50%</param>
        /// <param name="spyAttackVisibleChance">return number between 1 and 0. 1=100%, 0=0%, 0.5=50%</param>
        /// <param name="CurrentPlayer">DO NOT TAKE IN PLAYER but REALM object instead</param>
        /// <param name="distanceToTarget">distance to target in squares</param>
        /// <param name="defenderDefenceBonusFromVillageType">actual % bonus that defender gets because this is a bonus village. pass in 0.2 for 20%</param>
        /// <param name="defenderDefenceBonusFromResearch">actual % bonus the defender gets from wall research. pass 0.5 for 50%. min 0</param>
        /// <param name="defenderVillageDefenceBonusFromResearch">actual % bonus the defender gets from village defese research. pass 50 for 50%, pass 5 for 5%  min 0</param>
        /// <param name="attackersOffensiveResearchBonus">actual % bonus the attacker gets from offensive research. pass 0.5 for 50%. min 0</param>
        public static void Simulate(ref List<AttackingTroops> ATs
            , ref  List<DefendingTroops> DTs
            , ref  List<BuildingAttacks> BAs
            , ref double spySucessChance
            , ref double spyIdentityKnownChance
            , ref double spyAttackVisibleChance
            , ref bool spyExists
            , Player CurrentPlayer
            , float handicap
            , double distanceToTarget
            , bool isAttackBouns
            , bool isDefendBouns
            , float defenderDefenceBonusFromVillageType
            , float defenderDefenceBonusFromResearch
            , out double desertionFactor
            , float defenderVillageDefenceBonusFromResearch
            , float attackersOffensiveResearchBonus
            , int morale)
        {
            double spySucessChance_AlgA = 0;
            double spySucessChance_AlgB = 0;

            BattleSimulation.Simulate(ref ATs, ref DTs, ref BAs, ref spySucessChance_AlgA, ref spySucessChance_AlgB, CurrentPlayer, handicap, distanceToTarget, isAttackBouns, isDefendBouns, defenderDefenceBonusFromVillageType, defenderDefenceBonusFromResearch, out desertionFactor, defenderVillageDefenceBonusFromResearch, attackersOffensiveResearchBonus, morale);

            if (BattleSimulation.IsSpyExists(ATs))
            {
                spyExists = true;
                double spyFailureChance; // this is the effective chance for the type of lagorithm for this realm. 
                double spyFailureChance_AlgA = 0;
                double spyFailureChance_AlgB = 0;
                spyFailureChance_AlgA = 1 - spySucessChance_AlgA;
                spyFailureChance_AlgB = 1 - spySucessChance_AlgB;

                if (CurrentPlayer.Realm.SpyAlgorithmVersion == 1)
                {
                    spySucessChance = spySucessChance_AlgA;

                    if (BattleSimulation.AreSpiesAlone(ATs))
                    {
                        spyFailureChance = spyFailureChance_AlgA;
                        spyIdentityKnownChance = spyFailureChance * spyFailureChance;
                        spyAttackVisibleChance = 1 - spySucessChance;
                    }
                    else
                    {
                        spyIdentityKnownChance = 1;
                        spyAttackVisibleChance = 1;
                    }
                }
                else if (CurrentPlayer.Realm.SpyAlgorithmVersion == 2)
                {
                    spySucessChance = spySucessChance_AlgB;
                    if (BattleSimulation.AreSpiesAlone(ATs))
                    {
                        spyFailureChance = spyFailureChance_AlgB;
                        spyIdentityKnownChance = 0.1 + (0.8 * Math.Pow(spyFailureChance, 0.66666));
                        spyAttackVisibleChance = spyIdentityKnownChance;
                    }
                    else
                    {
                        spyIdentityKnownChance = 1;
                        spyAttackVisibleChance = 1;
                    }
                }
                else
                {
                    throw new InvalidOperationException("unrecognized SpyAlgorithmVersion");
                }
                //
                // if all spies were killed, success must be 0%
                //
                if (BattleSimulation.SpyAttackStrengthOfRemainingAttackTroops_noHandicap(ATs, CurrentPlayer.Realm) == 0)
                {
                    spySucessChance = 0;
                }
            }
            else
            {
                spyExists = false;
            }
        }


               /// <summary>
        /// </summary>
        /// <param name="ATs"></param>
        /// <param name="DTs"></param>
        /// <param name="BAs"> what do I pass here is building attacks???</param>
        /// <param name="spySucessChance"></param>
        /// <param name="spyIdentityKnownChance"></param>
        /// <param name="spyAttackVisibleChance"></param>
        /// <param name="CurrentPlayer">DO NOT TAKE IN PLAYER but REALM object instead</param>
        /// <param name="handicap">should be a number representing %. number between 0 and 1. </param>
        public static void Simulate(ref List<AttackingTroops> ATs
            , ref  List<DefendingTroops> DTs
            , ref  List<BuildingAttacks> BAs
            , ref double spySucessChance
            , Player CurrentPlayer
            , float handicap
            , double distanceToTarget)
        {          
           
            bool isAttackBouns = false;
            bool isDefendBouns = false;
            BattleSimulation.Simulate(ref ATs, ref  DTs, ref BAs, ref spySucessChance, CurrentPlayer, handicap, distanceToTarget,isAttackBouns,isDefendBouns, 0.0f, 0.0f, 0.0f,0.0f);
        }
        public static void Simulate(ref List<AttackingTroops> ATs
          , ref  List<DefendingTroops> DTs
          , ref  List<BuildingAttacks> BAs
          , ref double spySucessChance
          , Player CurrentPlayer
          , float handicap
          , double distanceToTarget
            , bool isAttackBouns
            , bool isDefendBouns
            , float defenderDefenceBonusFromVillageType
            , float defenderDefenceBonusFromResearch
            , float defenderVillageDefenceBonusFromResearch
            , float attackersOffensiveBonusFromResearch)
        {
            double spySucessChance_AlgA = 0;
            double spySucessChance_AlgB = 0;
            double desertionFactor;
            BattleSimulation.Simulate(ref ATs, ref  DTs, ref BAs, ref spySucessChance_AlgA, ref spySucessChance_AlgB, CurrentPlayer, handicap, distanceToTarget, isAttackBouns, isDefendBouns, defenderDefenceBonusFromVillageType
                , defenderDefenceBonusFromResearch, out desertionFactor, defenderVillageDefenceBonusFromResearch
                , attackersOffensiveBonusFromResearch
                , CurrentPlayer.Realm.Morale.MaxMorale_Normal);

            if (CurrentPlayer.Realm.SpyAlgorithmVersion == 1)
            {
                spySucessChance = spySucessChance_AlgA;
            }
            else if (CurrentPlayer.Realm.SpyAlgorithmVersion == 2)
            {
                spySucessChance = spySucessChance_AlgB;
            }
            else
            {
                throw new InvalidOperationException("unrecognized SpyAlgorithmVersion");
            }

        }

        /// <summary>
        /// </summary>
        /// <param name="ATs"></param>
        /// <param name="DTs"></param>
        /// <param name="BAs"> what do I pass here is building attacks???</param>
        /// <param name="spySucessChance"></param>
        /// <param name="spyIdentityKnownChance"></param>
        /// <param name="spyAttackVisibleChance"></param>
        /// <param name="CurrentPlayer">DO NOT TAKE IN PLAYER but REALM object instead</param>
        /// <param name="handicap">should be a number representing %. number between 0 and 1. </param>
        /// <param name="defenderDefenceBonusFromVillageType">actual % bonus that defender gets because this is a bonus village. pass in 0.2 for 20%</param>
        /// <param name="defenderDefenceBonusFromResearch">actual % bonus the defender gets from research. pass 0.5 for 50%. min 0, max 1</param>
        /// <param name="defenderVillageDefenceBonusFromResearch">actual % bonus the defender gets from village defese research. pass 50 for 50%, or 5 for 5%.  min 0</param>
        /// <param name="attackersOffensiveResearchBonus">actual % bonus the attaker gets from offensive research. pass 0.5 for 50%. min 0, max 1</param>
        public static void Simulate(ref List<AttackingTroops> ATs
            , ref  List<DefendingTroops> DTs
            ,ref  List<BuildingAttacks> BAs
            ,ref double spySucessChance_AlgA
            , ref double spySucessChance_AlgB
            , Player CurrentPlayer
            , float handicap
            , double distanceToTarget
            , bool isAttackBouns
            , bool isDefendBouns
            , float defenderDefenceBonusFromVillageType
            , float defenderDefenceBonusFromResearch
            , out double desertionFactor
            , float defenderVillageDefenceBonusFromResearch
            , float attackersOffensiveResearchBonus
            , int morale)
        {
           // CreateTestingScript(ATs, DTs); 
            float AttackStrength = 0; 
            float TargetStrength = 0; 
            int TotalNumberOfAttackingUnitsByPop = 0; 
            float AttackSpyStrength = 0;
            float AttackToTargetStrenghRatio_NoBuildingBonus = 0;
            float TargetStrengthWithBuildingBounus = 0;
            float BuildingDefenseBonus_PostAttack = 0; 
            float BuildingDefenseBonus_PreAttack=0;
            float BuildingDefenseBonus_Delta=0;
            float BuildingDefenseBonus_EffectiveDelta = 0;
            float AttackerKillFactor = 0;
            float TargetKillFactor = 0;
            float AttackBouns = 1;
            float DefenseBouns = 1;
            float handicapInverse = 1 - handicap; // so if handicap is 30%, handicapInverse will be 70%

            desertionFactor = CalculateDesertionFactor(ATs, distanceToTarget, CurrentPlayer.Realm);
                
           // List<BuildingAttacks> BAs = new List<BuildingAttacks>();

            if (isAttackBouns )
            {
                AttackBouns = 1.2f;
            }
            
            if (isDefendBouns )
            {
                DefenseBouns = 1.2f;
            }

            // got 0.2, want it in format of a factor, like 1.2
            defenderDefenceBonusFromVillageType = defenderDefenceBonusFromVillageType + 1;
            defenderDefenceBonusFromResearch = defenderDefenceBonusFromResearch + 1;
            foreach (AttackingTroops at in ATs)
            {
                at.UnitDeserted = Fbg.Bll.UnitMovements.GetUnitsDeserted(desertionFactor, at.UnitAmount);
                at.UnitAmount -= at.UnitDeserted;
                if (CurrentPlayer.Realm.GetUnitTypesByID(at.UnitType).AttackStrength > 0)
                {
                    AttackStrength += at.UnitAmount * CurrentPlayer.Realm.GetUnitTypesByID(at.UnitType).AttackStrength * handicapInverse*AttackBouns ;
                    TotalNumberOfAttackingUnitsByPop += at.UnitAmount * CurrentPlayer.Realm.GetUnitTypesByID(at.UnitType).Pop;
                }
                AttackSpyStrength += at.UnitAmount * CurrentPlayer.Realm.GetUnitTypesByID(at.UnitType).SpyAbility * handicapInverse*AttackBouns ;

            }

            // apply attackers research bonus is any 
            if (attackersOffensiveResearchBonus != 0)
            {
                AttackStrength = AttackStrength * (attackersOffensiveResearchBonus + 1);
            }
            // apply morale bonus, if morale active
            if (CurrentPlayer.Realm.Morale.IsActiveOnThisRealm)
            {
                float attackMultiplierFromMorale;
                attackMultiplierFromMorale = CurrentPlayer.Realm.Morale.GetEffect(morale).AttackMultiplier;

                AttackStrength = AttackStrength * attackMultiplierFromMorale;
            }

            //BuildingDefenseBonus_EffectiveDelta
            foreach (BuildingAttacks ba in BAs)
            {

                if (ba.CurrentLevel  > 0)
                {
                    DataRow[] dr = CurrentPlayer.Realm.GetLevelProperties().Select("BuildingTypeID=" + ba.BuildingType.ToString() + " and Level=" + ba.CurrentLevel.ToString() + "and PropertyID=9");
                    if (dr.Length > 0)
                    {
                        BuildingDefenseBonus_PreAttack += (Convert.ToSingle(dr[0]["PropertyValue"]) - 100) * defenderDefenceBonusFromResearch;
                    }
                }
            }
            BuildingDefenseBonus_PreAttack += defenderVillageDefenceBonusFromResearch;
            //GET DEFENSE STRENGTH, no building defense bonus 
            foreach (DefendingTroops dt in DTs)
            {
                float DefendingUnit_DS = 0;//this to hold defending units defence strength againest all attacking unit
                foreach (AttackingTroops at in ATs)
                {
                    int DefendStrength = 0;
                    CurrentPlayer.Realm.GetUnitTypesByID(dt.UnitType).DefenseStrength.TryGetValue(CurrentPlayer.Realm.GetUnitTypesByID(at.UnitType), out DefendStrength);
                    if (CurrentPlayer.Realm.GetUnitTypesByID(at.UnitType).AttackStrength > 0)
                    {
                        DefendingUnit_DS += at.UnitAmount * CurrentPlayer.Realm.GetUnitTypesByID(at.UnitType).Pop / Convert.ToSingle(TotalNumberOfAttackingUnitsByPop) * DefendStrength;
                    }

                }
                dt.DefenceStrength = DefendingUnit_DS * dt.UnitAmount;
                TargetStrength += DefendingUnit_DS * dt.UnitAmount *DefenseBouns *  defenderDefenceBonusFromVillageType;
            }
            
            ////
            if (Single.IsNaN(TargetStrength))
            {
                TargetStrength = 0;
            }
            if ( TargetStrength != 0 )
            {
                AttackToTargetStrenghRatio_NoBuildingBonus  = AttackStrength / TargetStrength;
                    if (AttackToTargetStrenghRatio_NoBuildingBonus  > 1) 
                    {
                        AttackToTargetStrenghRatio_NoBuildingBonus  = 1;
                    }
            }
            else if (AttackStrength  == 0)
            {
                AttackToTargetStrenghRatio_NoBuildingBonus = 0;
            }
            else
            {
                AttackToTargetStrenghRatio_NoBuildingBonus = 1;
            }
            //////////////
          
            foreach (BuildingAttacks ba in BAs )
            {
                ba.AttackStrength = ba.AttackStrength * AttackToTargetStrenghRatio_NoBuildingBonus;   
            }
           
            foreach (BuildingAttacks ba in BAs)
            {
                foreach (BuildingType bt in CurrentPlayer.Realm.BuildingTypes)
                {
                    if (bt.ID == ba.BuildingType)
                    {
                        foreach (BuildingTypeLevel btl in bt.Levels)
                        {
                            if (btl.Level == ba.CurrentLevel)
                            {
                                ba.PointsAfterAttack = btl.CumulativeLevelStrength - ba.AttackStrength;
                            }
                        }

                    }
                }
            }
            foreach (BuildingAttacks ba in BAs)
            {
                foreach (BuildingType bt in CurrentPlayer.Realm.BuildingTypes)
                {
                    if (bt.ID == ba.BuildingType)
                    {
                        foreach (BuildingTypeLevel btl in bt.Levels)
                        {
                            if (ba.PointsAfterAttack + btl.LevelStrength > btl.CumulativeLevelStrength)
                            {
                                ba.LevelAfterAttack = btl.Level;
                            }
                        }
                    }
                }
            }
           //GET DEFENSE STRENGTH, WITH building defense bonus;  

            foreach (BuildingAttacks ba in BAs)
            {

                if (ba.LevelAfterAttack > 0)
                    {
                        DataRow[] dr = CurrentPlayer.Realm.GetLevelProperties().Select("BuildingTypeID=" + ba.BuildingType.ToString() + " and Level=" + ba.LevelAfterAttack.ToString() + "and PropertyID=9");
                        if (dr.Length >0)
                        {
                                BuildingDefenseBonus_PostAttack += (Convert.ToSingle(dr[0]["PropertyValue"]) - 100) * defenderDefenceBonusFromResearch;
                        }
                    }              
            }
            BuildingDefenseBonus_PostAttack += defenderVillageDefenceBonusFromResearch;

            	// Compute the difference between pre attack and post attack bonus, then the actual delta, then the actual bonus	
            if (Single.IsNaN ( BuildingDefenseBonus_PreAttack))
            {
                BuildingDefenseBonus_PreAttack=0;
            }
             if (Single.IsNaN ( BuildingDefenseBonus_PostAttack))
            {
                BuildingDefenseBonus_PostAttack=0;
            }
            BuildingDefenseBonus_Delta = BuildingDefenseBonus_PreAttack - BuildingDefenseBonus_PostAttack;
            BuildingDefenseBonus_EffectiveDelta =Convert.ToSingle ( 0.6 * Math.Sin(0.5 * Math.PI * (BuildingDefenseBonus_Delta / 100)));
	
	         TargetStrengthWithBuildingBounus = TargetStrength * (1 +(BuildingDefenseBonus_PreAttack/100 - BuildingDefenseBonus_EffectiveDelta) );
           
            if (AttackStrength > TargetStrengthWithBuildingBounus)
            {
                AttackerKillFactor = (float )Math.Pow(Convert.ToDouble( TargetStrengthWithBuildingBounus / AttackStrength), 1.5);
                TargetKillFactor = 1;
            }
            else if (TargetStrengthWithBuildingBounus > AttackStrength)
            {
                TargetKillFactor = (float)Math.Pow(Convert.ToDouble(AttackStrength / TargetStrengthWithBuildingBounus), 1.5);
                AttackerKillFactor = 1;
            }
            else if (TargetStrengthWithBuildingBounus == 0 && AttackStrength == 0)
            {
                AttackerKillFactor = 1;
                TargetKillFactor = 0;
            }
            else
            {
                AttackerKillFactor = 1;
                TargetKillFactor = 1;
            }
            foreach (AttackingTroops at in ATs)
            {
                if (at.UnitType != UnitSpyID)//don't update spies unit amount
                {
                    if ((int)Math.Round(at.UnitAmount * AttackerKillFactor, 0, MidpointRounding.AwayFromZero) > 0)
                    {
                        at.UnitKilled = (int)Math.Round(at.UnitAmount * AttackerKillFactor, 0, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        at.UnitKilled = 0;
                    }
                }

            }
            foreach (DefendingTroops dt in DTs)
            {
                if ((int)Math.Round(dt.UnitAmount * TargetKillFactor, 0, MidpointRounding.AwayFromZero) > 0)
                {
                    dt.UnitKilled = (int)Math.Round(dt.UnitAmount * TargetKillFactor, 0, MidpointRounding.AwayFromZero);
                }
                else
                {
                    dt.UnitKilled = 0;
                }
               
            }
            for (int i = 0; i < DTs.Count && i < ATs.Count; i++)
            {
                foreach (BuildingAttacks ba in BAs)
                {
                    if (ATs[i].TargetBuilding == ba.BuildingType)
                    {
                        DTs[i].BuildingLevel = ba.LevelAfterAttack;
                    }
                }
            }

            #region Handle spies
            //
            // Handle spies
            //  
            //  hack: here we get 'spySpyability' which breaks the whole flexibility of 'any unit can have spy ability'
            //      since here we assume only
            //
            double targetCounterSpyStrength = 0; 
            double spyFailureChance_AlgA = 0;
            int spySpyability = CurrentPlayer.Realm.GetUnitTypesByID(CONSTS.UnitIDs.Spy).SpyAbility;
            if (AttackSpyStrength > 0)
            {
                foreach (DefendingTroops dt in DTs)
                {
                    targetCounterSpyStrength += CurrentPlayer.Realm.GetUnitTypesByID(dt.UnitType).CounterSpyAbility  * (dt.UnitAmount - dt.UnitKilled);
                }
                targetCounterSpyStrength += 20;

                spySucessChance_AlgA = AttackSpyStrength / (AttackSpyStrength + targetCounterSpyStrength);
                spySucessChance_AlgB = 1 - Math.Pow(1 - (spySpyability / targetCounterSpyStrength), AttackSpyStrength/spySpyability);
                spyFailureChance_AlgA = 1 - spySucessChance_AlgA;
            }
            //
            // now kill of some spies if necessary
            if (targetCounterSpyStrength > 20) //only if at least 1 spy was defending
            {
                foreach (AttackingTroops at in ATs)
                {
                    if (at.UnitType == UnitSpyID)//update spies unit amount
                    {
                        at.UnitKilled = (int)Math.Round(at.UnitAmount * spyFailureChance_AlgA, 0, MidpointRounding.AwayFromZero);
                    }
                }
            }

            #endregion 
        }
        public static List<BuildingAttacks> InitDefenderBuilding(Player CurrentPlayer)
        {
            List<BuildingAttacks> BAs = new List<BuildingAttacks>();
            foreach (BuildingType bt in CurrentPlayer.Realm.BuildingTypes)
            {
                BuildingAttacks ba = new BuildingAttacks();
                ba.BuildingType = bt.ID;
                ba.MinimumLevelAllowed = bt.MinimumLevelAllowed;

                BAs.Add(ba);
            }
            return BAs;
        }
        public static List<BuildingAttacks> GetBuildingUnderAttack(List<BuildingAttacks>BAs,List<AttackingTroops> ATs, List<DefendingTroops> DTs, int DefenderWallLevel, int DefenderTowersLevel,Player CurrentPlayer)
        {
            foreach (BuildingAttacks ba in BAs)
            {
                int BuildingAttackStrength = 0;
                foreach (AttackingTroops at in ATs)
                {
                    if (at.TargetBuilding == ba.BuildingType)
                    {

                        BuildingAttackStrength += at.UnitAmount * CurrentPlayer.Realm.GetUnitTypesByID(at.UnitType).GetBuildingAttackStrength(ba.BuildingType);
                    }
                }
                ba.AttackStrength = BuildingAttackStrength;
            }

            foreach (BuildingAttacks ba in BAs)
            {
                foreach (DefendingTroops dt in DTs)
                {
                    if (dt.BuildingType == ba.BuildingType)
                    {
                        ba.CurrentLevel = dt.BuildingLevel;
                    }
                }
            }

            foreach (BuildingAttacks ba in BAs)
            {
                if (ba.BuildingType == 4)//wall
                {
                    //if (ba.AttackStrength > 0 || DefenderWallLevel>0)
                   // {
                        ba.CurrentLevel = DefenderWallLevel;
                   // }
                }
                else if (ba.BuildingType == 7)
                {
                  //  if (ba.AttackStrength > 0 || DefenderTowersLevel>0)
                  //  {
                        ba.CurrentLevel = DefenderTowersLevel;
                  //  }
                }
            }
            return BAs;
        }

        /// <summary>
        /// returns true if there are spies in this list of troops
        /// </summary>
        /// <param name="ATs"></param>
        /// <returns></returns>
        public static bool IsSpyExists(List<AttackingTroops> ATs)
        {
            foreach (AttackingTroops at in ATs)
            {
                if (at.UnitType == UnitSpyID)
                {
                    if (at.UnitAmount > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static float SpyAttackStrengthOfRemainingAttackTroops_noHandicap(List<AttackingTroops> ATs, Realm realm)
        {
            float attackSpyStrength_AfterSpiesKilled = 0;
            foreach (AttackingTroops at in ATs)
            {
                attackSpyStrength_AfterSpiesKilled += (at.UnitAmount - at.UnitKilled) * realm.GetUnitTypesByID(at.UnitType).SpyAbility;
            }
            return attackSpyStrength_AfterSpiesKilled;
        }

        /// <summary>
        /// returns true if and only if only sspies in this troops list;false if there any troops other than spies OR the list is empty
        /// </summary>
        /// <param name="ATs"></param>
        /// <returns></returns>
        public static bool AreSpiesAlone(List<AttackingTroops> ATs)
        {
            int spyUnitAmount = 0;
            foreach (AttackingTroops at in ATs)
            {
                if (at.UnitType != UnitSpyID)
                {
                    if (at.UnitAmount > 0)
                    {
                        return false;
                    }
                }
                else
                {
                    spyUnitAmount = at.UnitAmount;
                }
            }
            if (spyUnitAmount == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
            
        }

        public static double CalculateDesertionFactor(List<AttackingTroops> ATs
           , double distanceToTarget, Realm realm)
        {
            //
            // calc population size of the attack, check if gov present
            //
            int unitsByPop = 0;
            bool isGovPresent = false;
            foreach (AttackingTroops a in ATs)
            {
                unitsByPop +=  realm.GetUnitTypesByID(a.UnitType).Pop * a.UnitAmount;
                if (a.UnitType == Fbg.Bll.CONSTS.UnitIDs.Gov && a.UnitAmount >0 )
                {
                    isGovPresent = true;
                }
            }

            return Fbg.Bll.UnitMovements.GetUnitDesertionFactor(distanceToTarget, unitsByPop, isGovPresent, false, realm);
        }
    }

    public class AttackingTroops
    {
        private int _unittype;
        private int _unitamount;
        private int _targetbuilding;
        //  private int _targetbuildingattackstregnth;
        private int _unitkilled;
        private int _unitDeserted;
        private AttackingTroops()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="AttackingUnitType"></param>
        /// <param name="AttackingUnitAmount"></param>
        /// <param name="TargetBuildingID">what if no building is targeted?</param>
        public AttackingTroops(int AttackingUnitType, int AttackingUnitAmount, int TargetBuildingID)
        {
            UnitType = AttackingUnitType;
            UnitAmount = AttackingUnitAmount;
            TargetBuilding = TargetBuildingID;
        }

        public int UnitType
        {
            get
            {
                return _unittype;
            }
            set
            {
                _unittype = value;
            }
        }
        public int UnitAmount
        {
            get
            {
                return _unitamount;
            }
            set
            {
                _unitamount = value;
            }
        }
        public int TargetBuilding
        {
            get
            {
                return _targetbuilding;
            }
            set
            {
                _targetbuilding = value;
            }
        }
        public int UnitKilled
        {
            get
            {
                return _unitkilled;
            }
            set
            {
                _unitkilled = value;

            }
        }
        public int UnitDeserted
        {
            get
            {
                return _unitDeserted;
            }
            set
            {
                _unitDeserted = value;

            }
        }
    }
    public class DefendingTroops
    {
        private int _unittype;
        private int _unitamount;
        private int _buildinglevel;
        private int _buildingtype;
        private double _defencestrength;
        private int _unitkilled;
        private DefendingTroops()
        {
        }
        public DefendingTroops(int DefendingUnitType, int DefendingUnitAmount, int BuildingLevelID,int BuildingTypeID)
        {
            UnitType = DefendingUnitType;
            UnitAmount = DefendingUnitAmount;
            BuildingLevel = BuildingLevelID;
            BuildingType = BuildingTypeID;
        }

        public int UnitType
        {
            get
            {
                return _unittype;
            }
            set
            {
                _unittype = value;
            }
        }
        public int UnitAmount
        {
            get
            {
                return _unitamount;
            }
            set
            {
                _unitamount = value;
            }
        }
        public int BuildingLevel
        {
            get
            {
                return _buildinglevel;
            }
            set
            {
                _buildinglevel = value;
            }
        }
        public int BuildingType
        {
            get
            {
                return _buildingtype;
            }
            set
            {
                _buildingtype = value;
            }
        }
        public double DefenceStrength
        {
            get
            {
                return _defencestrength;
            }
            set
            {
                _defencestrength = value;
            }
        }
        public int UnitKilled
        {
            get
            {
                return _unitkilled;
            }
            set
            {
                _unitkilled = value;

            }
        }
    }
    public class BuildingAttacks
    {
        private int _buildingtype;
        private double _attackstrength;
        private int _currentlevel;
        private int _minimumlevelallowed;
        private double  _pointsafterattack;
        private int _levelafterattack;
        public  BuildingAttacks()
        {
            _buildingtype = 0;
            _attackstrength = 0;
            _currentlevel = 0;
            _minimumlevelallowed = 0;
            _pointsafterattack = 0;
            _levelafterattack = 0;
        }

        public int BuildingType
        {
            get
            {
                return _buildingtype;
            }
            set
            {
                _buildingtype = value;
            }
        }
        public double AttackStrength
        {
            get
            {
                return _attackstrength;
            }
            set
            {
                _attackstrength = value;
            }
        }
        public int CurrentLevel
        {
            get
            {
                return _currentlevel;
            }
            set
            {
                _currentlevel = value;
            }
        }
        public int MinimumLevelAllowed
        {
            get
            {
                return _minimumlevelallowed;
            }
            set
            {
                _minimumlevelallowed = value;
            }
        }
        public double PointsAfterAttack
        {
            get
            {
                return _pointsafterattack;
            }
            set
            {
                _pointsafterattack = value;

            }
        }
        public int LevelAfterAttack
        {
            get
            {
                return _levelafterattack;
            }
            set
            {
                _levelafterattack = value;
            }
        }
    }
   
}
