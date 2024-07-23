using System;
using System.Collections.Generic;
using System.Text;

namespace Fbg.Bll
{
    public abstract class MassRecruitRule : IComparable
    {
        int Priority;

        public MassRecruitRule(int priority)
        {
            this.Priority = priority;
        }

        public int CompareTo(object obj)
        {
            if (obj is MassRecruitRule)
            {
                if (this.Priority < ((MassRecruitRule)obj).Priority)
                {
                    return -1;
                }
                else if (this.Priority > ((MassRecruitRule)obj).Priority)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        public abstract void ClearResults();
    }

    public abstract class MassRecruitRule_SingleUnit : MassRecruitRule
    {
        public Fbg.Bll.Village.CanRecruitResult Result_RecruitResult;
        public int Result_AmountRecruited;
        public UnitType Unit;

        public MassRecruitRule_SingleUnit(int priority, UnitType unit): base(priority)
        {
            Unit = unit;
        }
        public override void ClearResults()
        {
            Result_RecruitResult = Village.CanRecruitResult.Yes;
            Result_AmountRecruited = 0;
        }
    }

    public class MassRecruitRule_KeepBusy : MassRecruitRule
    {
        public UnitType FirstUnit;
        public UnitType SecondUnit;
        public Fbg.Bll.Village.CanRecruitResult Result_FirstUnit_RecruitResult;
        public Fbg.Bll.Village.CanRecruitResult Result_SecondUnit_RecruitResult;
        public int Result_FirstUnit_AmountRecruited;
        public int Result_SecondUnit_AmountRecruited;


        public MassRecruitRule_KeepBusy(int priority, UnitType firstUnit, UnitType secondUnit) : base    (priority)
        {
            FirstUnit = firstUnit;
            SecondUnit = secondUnit;
        }

        public override string ToString()
        {
            return String.Format("<LI>Recruit as much as possible of <B>{0}</B> and <b>{1}</b>.</li> (half and half)", FirstUnit.Name, SecondUnit.Name);
        }

        public override void ClearResults()
        {
            Result_FirstUnit_RecruitResult = Village.CanRecruitResult.Yes;
            Result_SecondUnit_RecruitResult = Village.CanRecruitResult.Yes;
            Result_FirstUnit_AmountRecruited = 0;
            Result_SecondUnit_AmountRecruited = 0;
        }
    }

    public class MassRecruitRule_RecruitMaxUpTo : MassRecruitRule_SingleUnit
    {
        public int MaxToHaveInVillage;
        public bool Result_MaxAlreadyReached = false;

        public MassRecruitRule_RecruitMaxUpTo(int priority, UnitType unit, int maxToHave)
            : base(priority, unit)
        {
            Unit = unit;
            MaxToHaveInVillage = maxToHave;
        }

        public override string ToString()
        {
            return String.Format("<LI>Recruit as much as possible of <B>{0}</B> up to <B>{1}</B> max total in village</li>", Unit.Name, MaxToHaveInVillage);
        }
        public override void ClearResults()
        {
            base.ClearResults();
            Result_MaxAlreadyReached = false;           
        }

    }


    public class MassRecruitRule_RecruitX : MassRecruitRule_SingleUnit
    {
        public int NumToRecruit;

        public MassRecruitRule_RecruitX(int priority, UnitType unit, int numToRecruit)
            : base(priority, unit)
        {
            NumToRecruit = numToRecruit;
        }

        public override string ToString()
        {
            return String.Format("<LI>Recruit <B>{0}</B> (or as as much as possible up to {0}) of <B>{1}</B> .", NumToRecruit, Unit.Name);
        }

    }

}

