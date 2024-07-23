using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using Gmbc.Common.Diagnostics.ExceptionManagement;
namespace Fbg.Bll
{
   public class ClanDiplomacy
    {
        internal class CONSTS
        {

            public class ClanDiplomacyTableIndex
            {
                public static int Allay = 0;
                public static int Enemy = 1;
                public static int NAP = 2;
            }
            public class ClanDiplomacyColName
            {
                public static string OtherClanID = "OtherClanID";
                
            }

        }
        private DataSet _ds;
        public ClanDiplomacy()
        {
         
        }
        public ClanDiplomacy(DataSet ds)
        {
            _ds = ds;
        }
        public DataTable  GetAllies()
        {
            return _ds.Tables[CONSTS.ClanDiplomacyTableIndex.Allay];
        }
        public bool IsAlly(int OtherClanID)
        {
            if (_ds.Tables[CONSTS.ClanDiplomacyTableIndex.Allay].Select(CONSTS.ClanDiplomacyColName.OtherClanID + "=" + OtherClanID).Length != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public DataTable GetEnemies()
        {
            return _ds.Tables[CONSTS.ClanDiplomacyTableIndex.Enemy];
        }
        public bool IsEnemy(int OtherClanID)
        {
            if (_ds.Tables[CONSTS.ClanDiplomacyTableIndex.Enemy].Select(CONSTS.ClanDiplomacyColName.OtherClanID +"="+ OtherClanID).Length != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public DataTable GetNAP()
        {
            return _ds.Tables[CONSTS.ClanDiplomacyTableIndex.NAP];
        }
        public bool IsNAP(int OtherClanID)
        {
            if (_ds.Tables[CONSTS.ClanDiplomacyTableIndex.NAP].Select(CONSTS.ClanDiplomacyColName.OtherClanID + "=" + OtherClanID).Length != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
