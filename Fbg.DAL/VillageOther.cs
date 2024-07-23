using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Gmbc.Common.Diagnostics.ExceptionManagement;

namespace Fbg.DAL
{
    public class VillageOther
    {
        static Gmbc.Common.GmbcBaseClass.Trace TRACE = new Gmbc.Common.GmbcBaseClass.Trace("Fbg.DAL", "Fbg.DAL.Villages");

        /// <summary>
        /// returns VillageNotes
        /// </summary>
        /// <param name="VillageID"></param>
        /// <param name="OwnerID"></param>
        /// <param name="ConnectionStr"></param>
        /// <returns></returns>
        public static string getVillageNotes(int VillageID, int OwnerID, string ConnectionStr)
        {
            TRACE.InfoLine("in 'qVillageNotes()'");
            Database db;

            try
            {
                db = new DB(ConnectionStr); ;
                return (string)db.ExecuteScalar("qVillageNotes", new object[] { VillageID, OwnerID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qVillageNotes", e);
                ex.AdditionalInformation.Add("connectionStr", ConnectionStr);
                ex.AdditionalInformation.Add("VillageID", VillageID.ToString());
                ex.AdditionalInformation.Add("NoteOwnerPlayerID", OwnerID.ToString());
                throw ex;
            }
        }

        ///// <summary>
        ///// stores VillageNotes
        ///// </summary>
        ///// <param name="VillageID"></param>
        ///// <param name="OwnerID"></param>
        ///// <param name="Notes"></param>
        ///// <param name="ConnectionStr"></param>
        //public static void saveVillageNotes(int VillageID, int OwnerID, string Notes, string ConnectionStr)
        //{
        //    TRACE.InfoLine("in 'iVillageNote()'");
        //    Database db;

        //    try
        //    {
        //        db = new Microsoft.Practices.EnterpriseLibrary.Data.Sql.SqlDatabase(ConnectionStr);
        //        db.ExecuteNonQuery("iVillageNote", new object[] { VillageID, OwnerID, Notes });
        //    }
        //    catch (Exception e)
        //    {
        //        BaseApplicationException ex = new BaseApplicationException("Error while calling iVillageNotes", e);
        //        ex.AdditionalInformation.Add("connectionStr", ConnectionStr);
        //        ex.AdditionalInformation.Add("PlayerID", VillageID.ToString());
        //        ex.AdditionalInformation.Add("NoteOwnerPlayerID", OwnerID.ToString());
        //        ex.AdditionalInformation.Add("Notes", Notes);
        //        throw ex;
        //    }
        //}

        /// <summary>
        /// checks if VillageNote already exists from the PlayerID then it will updat it otherwise will make new entry
        /// </summary>
        /// <param name="VillageID"></param>
        /// <param name="OwnerID"></param>
        /// <param name="Notes"></param>
        /// <param name="ConnectionStr"></param>
        public static void saveVillageNotes(int VillageID, int OwnerID, string Notes, string ConnectionStr)
        {
            TRACE.InfoLine("in 'saveVillageNote()'");
            Database db;

            try
            {
                db = new DB(ConnectionStr); ;
                db.ExecuteNonQuery("saveVillageNote", new object[] { VillageID, OwnerID, Notes });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling saveVillageNotes", e);
                ex.AdditionalInformation.Add("connectionStr", ConnectionStr);
                ex.AdditionalInformation.Add("VillageID", VillageID.ToString());
                ex.AdditionalInformation.Add("NoteOwnerPlayerID", OwnerID.ToString());
                ex.AdditionalInformation.Add("Notes", Notes);
                throw ex;
            }
        }
    }
}
