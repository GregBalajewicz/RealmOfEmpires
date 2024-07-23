using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;

using Gmbc.Common.Diagnostics.ExceptionManagement;

namespace Fbg.DAL
{
    public class Map
    {
        static Gmbc.Common.GmbcBaseClass.Trace TRACE = new Gmbc.Common.GmbcBaseClass.Trace("Fbg.DAL", "Fbg.DAL.Villages");

        public static DataSet GetMapTroopsMoveIncoming(string connectionStr, int bottomLeftX, int bottomLeftY
             , int regularMapSize, int playerID)
        {
            Database db;

            try
            {
                db = new DB(connectionStr);

                return db.ExecuteDataSet("qMapTroopsMove_Incoming", new object[] { bottomLeftX, bottomLeftY, regularMapSize, playerID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qMapTroopsMove_Incoming", e);
                ex.AddAdditionalInformation("bottomLeftX", bottomLeftX);
                ex.AddAdditionalInformation("bottomLeftY", bottomLeftY);
                ex.AddAdditionalInformation("regularMapSize", regularMapSize);
                ex.AddAdditionalInformation("playerID", playerID);
                throw ex;
            }
        }

        public static DataSet GetMapTroopsMoveOutgoing(string connectionStr, int bottomLeftX, int bottomLeftY, int regularMapSize, int playerID)
        {
            Database db;

            try
            {
                db = new DB(connectionStr);

                return db.ExecuteDataSet("qMapTroopsMove_Outgoing", new object[] { bottomLeftX, bottomLeftY, regularMapSize, playerID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qMapTroopsMove_Outgoing", e);
                ex.AddAdditionalInformation("bottomLeftX", bottomLeftX);
                ex.AddAdditionalInformation("bottomLeftY", bottomLeftY);
                ex.AddAdditionalInformation("regularMapSize", regularMapSize);
                ex.AddAdditionalInformation("playerID", playerID);
                throw ex;
            }
        }

        //public static DataSet GetMapInfo(string connectionStr, int bottomLeftX, int bottomLeftY
        //    , int regularMapSize, int myClanID, int playerID, bool hasPF)
        //{
        //    Database db;

        //    try
        //    {
        //        db = new DB(connectionStr);

        //        return db.ExecuteDataSet("qMap", new object[] { bottomLeftX, bottomLeftY, regularMapSize, myClanID, playerID, hasPF });
        //    }
        //    catch (Exception e)
        //    {
        //        BaseApplicationException ex = new BaseApplicationException("Error while calling qMap", e);
        //        ex.AddAdditionalInformation("bottomLeftX", bottomLeftX);
        //        ex.AddAdditionalInformation("bottomLeftY", bottomLeftY);
        //        ex.AddAdditionalInformation("regularMapSize", regularMapSize);
        //        ex.AddAdditionalInformation("playerID", playerID);
        //        ex.AddAdditionalInformation("myClanID", myClanID);
        //        ex.AddAdditionalInformation("hasPF", hasPF);
        //        throw ex;
        //    }
        //}
        
        public static DataSet GetMapInfo(string connectionStr, int bottomLeftX, int bottomLeftY
            , int regularMapSizeX, int regularMapSizeY, int myClanID, int playerID, bool hasPF)
        {
            Database db;

            try
            {
                db = new DB(connectionStr);

                return db.ExecuteDataSet("qMap", new object[] { bottomLeftX, bottomLeftY, regularMapSizeX, regularMapSizeY, myClanID, playerID, hasPF });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qMap", e);
                ex.AddAdditionalInformation("bottomLeftX", bottomLeftX);
                ex.AddAdditionalInformation("bottomLeftY", bottomLeftY);
                ex.AddAdditionalInformation("regularMapSizeX", regularMapSizeX);
                ex.AddAdditionalInformation("regularMapSizeY", regularMapSizeY);
                ex.AddAdditionalInformation("playerID", playerID);
                ex.AddAdditionalInformation("myClanID", myClanID);
                ex.AddAdditionalInformation("hasPF", hasPF);
                throw ex;
            }
        }

        public static DataSet GetMapBySquares(string connectionStr, 
            int villageBottomLeftX, int villageBottomLeftY, int villageMapSizeX, int villageMapSizeY,
            int landmarkBottomLeftX, int landmarkBottomLeftY, int landmarkMapSizeX, int landmarkMapSizeY, 
            int myClanID, int playerID, bool hasPF, bool hasAllLandmarkTypes)
        {
            Database db;

            try
            {
                db = new DB(connectionStr);

                return db.ExecuteDataSet("qMapBySquares", new object[] {  
                    villageBottomLeftX,villageBottomLeftY,villageMapSizeX,villageMapSizeY,
                    landmarkBottomLeftX,landmarkBottomLeftY,landmarkMapSizeX,landmarkMapSizeY, 
                    myClanID, playerID, hasPF, hasAllLandmarkTypes });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qMapBySquares", e);
                ex.AddAdditionalInformation("villageMapBottomLeftX", villageBottomLeftX);
                ex.AddAdditionalInformation("villageMapBottomLeftY", villageBottomLeftY);
                ex.AddAdditionalInformation("villageMapSizeX", villageMapSizeX);
                ex.AddAdditionalInformation("villageMapSizeY", villageMapSizeY);
                ex.AddAdditionalInformation("landmarkMapBottomLeftX", landmarkBottomLeftX);
                ex.AddAdditionalInformation("landmarkMapBottomLeftY", landmarkBottomLeftY);
                ex.AddAdditionalInformation("landmarkMapSizeX", landmarkMapSizeX);
                ex.AddAdditionalInformation("landmarkMapSizeY", landmarkMapSizeY);
                ex.AddAdditionalInformation("playerID", playerID);
                ex.AddAdditionalInformation("myClanID", myClanID);
                ex.AddAdditionalInformation("hasPF", hasPF);
                ex.AddAdditionalInformation("hasAllLandmarkTypes", hasAllLandmarkTypes);
                throw ex;
            }
        }

        public static DataSet GetOverviewMapInfo(string connectionStr, int bottomLeftX_ov
            , int bottomLeftY_ov, int overviewMapSize, int myClanID, int playerID)
        {
            Database db;

            try
            {
                db = new DB(connectionStr);

                return db.ExecuteDataSet("qMapOverview", new object[] { bottomLeftX_ov, bottomLeftY_ov
                    , overviewMapSize, myClanID, playerID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qMapOverview", e);
                ex.AddAdditionalInformation("bottomLeftX_ov", bottomLeftX_ov);
                ex.AddAdditionalInformation("bottomLeftY_ov", bottomLeftY_ov);
                ex.AddAdditionalInformation("overviewMapSize", overviewMapSize);
                ex.AddAdditionalInformation("playerID", playerID);
                ex.AddAdditionalInformation("myClanID", myClanID);
                throw ex;
            }
        }
    }
}