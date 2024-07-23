using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Gmbc.Common.Diagnostics.ExceptionManagement;

namespace Fbg.DAL
{
    public class Folders
    {
        public static DataTable GetFolders(string connectionStr, int playerID)
        {

            Database db;

            try
            {
                db = new DB(connectionStr); ;

                return db.ExecuteDataSet("qFolders", new object[] { playerID }).Tables[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qFolders", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("playerID", playerID.ToString());                
                throw ex;
            }
            finally
            {
            }
        }
        public static bool AddFolder(string connectionStr, int playerID, string folderName,Int16 folderType)
        {

            Database db;

            try
            {
                db = new DB(connectionStr); ;

                int ret = (int)db.ExecuteScalar("iFolder", new object[] { playerID, folderName, folderType });
                if (ret == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling iFolder", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("folderName", folderName);
                ex.AdditionalInformation.Add("folderFor", folderType.ToString());
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                throw ex;
            }
            finally
            {
            }
        }
        public static bool UpdateFolder(string connectionStr, int playerID,int folderID, string folderName,Int16 folderType)
        {

            Database db;

            try
            {
                db = new DB(connectionStr); ;

               int ret = (int)db.ExecuteScalar ("uFolder", new object[] { playerID,folderID , folderName ,folderType });
               if (ret == 0)
               {
                   return true;
               }
               else
               {
                   return false;
               }
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uFolder", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("folderID", folderID.ToString());
                ex.AdditionalInformation.Add("folderName", folderName);
                ex.AdditionalInformation.Add("folderType", folderType.ToString ());
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                throw ex;
            }
            finally
            {
            }
        }
        public static bool DeleteFolder(string connectionStr, int playerID, int folderID,Int16 folderType)
        {
            Database db;

            try
            {
                db = new DB(connectionStr); ;

                int ret = (int)db.ExecuteScalar("dFolder", new object[] { playerID, folderID ,folderType });
                if (ret == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling dFolder", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("folderID", folderID.ToString());
                ex.AdditionalInformation.Add("folderType", folderType.ToString());
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                throw ex;
            }
            finally
            {
            }
        }
        /// <summary>
        /// this function move (to another folder)and delete the selected folder
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="playerID"></param>
        /// <param name="folderID">selected folder that will be deleted</param>
        /// <param name="moveToFolderID">folder to move items to it</param>
        /// <returns>0 mean success ,1 means failed</returns>
        public static bool DeleteFolderAndMoveItems(string connectionStr, int playerID, int folderID, int moveToFolderID, Int16 folderType)
        {
            Database db;

            try
            {
                db = new DB(connectionStr); ;

                int ret = (int)db.ExecuteScalar("dMoveFolder", new object[] { playerID, folderID ,moveToFolderID,folderType });
                if (ret == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling dMoveFolder", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("folderID", folderID.ToString());
                ex.AdditionalInformation.Add("moveToFolderID", moveToFolderID.ToString());
                ex.AdditionalInformation.Add("folderType", folderType.ToString());
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                throw ex;
            }
            finally
            {
            }
        }
        public static bool IsFolderEmpty(string connectionStr, int playerID, int folderID,Int16 folderType)
        {
            Database db;

            try
            {
                db = new DB(connectionStr); ;

                int ret = (int)db.ExecuteScalar("qIsFolderEmpty", new object[] { playerID, folderID,  folderType });
                if (ret == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling dMoveFolder", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("folderID", folderID.ToString());
                ex.AdditionalInformation.Add("folderType", folderType.ToString());
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                throw ex;
            }
            finally
            {
            }

        }


    }
}
