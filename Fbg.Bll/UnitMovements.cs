using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Fbg.Bll
{
    public class UnitMovements
    {
        public class CONSTS
        {
            public class UnitMovementDetColIndex
            {
                public static int EventID = 0;
                public static int UnitTypeId = 1;
                public static int UnitCount = 2;
                public static int DestVillageName = 3;
                public static int DestVillageID = 4;
                public static int DestPlayerName = 5;
                public static int DestPlayerID = 6;
                public static int CommandType = 7;
                public static int TripDuration = 8;
                public static int EventTime = 9;
                public static int OriginVillageName = 10;
                public static int OriginVillageID = 11;
                public static int DestVillageX = 12;
                public static int DestVillageY = 13;
                public static int OriginVillageX = 14;
                public static int OriginVillageY = 15;
                public static int Loot = 16;
            }

            public class IncomingTroops
            {
                public class ColNames
                {
                    public static string EventID = "EventID";
                    public static string OriginVillageID = "OriginVillageID";
                    public static string DestinationVillageID = "DestinationVillageID";
                    public static string CommandType = "CommandType";
                    public static string EventTime = "EventTime";
                    public static string DestinationVillageName = "DestinationVillageName";
                    public static string OriginVillageName = "OriginVillageName";
                    public static string OriginPlayerName = "OriginPlayerName";
                    public static string OriginPlayerID = "OriginPlayerID";
                    public static string VisibleToTarget = "VisibleToTarget";
                    public static string DestVillageX = "DestVillageX";
                    public static string DestVillageY = "DestVillageY";
                    public static string OriginVillageX = "OriginVillageX";
                    public static string OriginVillageY = "OriginVillageY";
                }
                public class ColIdx
                {
                    public static int EventID = 0;
                    public static int OriginVillageID = 1;
                    public static int DestinationVillageID = 2;
                    public static int CommandType = 3;
                    public static int EventTime = 4;
                    public static int DestinationVillageName = 5;
                    public static int OriginVillageName = 6;
                    public static int OriginPlayerName = 7;
                    public static int OriginPlayerID = 8;
                    public static int VisibleToTarget = 9;
                    public static int DestVillageX = 10;
                    public static int DestVillageY = 11;
                    public static int OriginVillageX = 12;
                    public static int OriginVillageY = 13;
                }


                public class VisibleToTargetColValues
                {
                    public static short NotVisibleAtAll = 0;
                    public static short FromNotVisible = 1;
                    public static short AllVisible = 2;
                }
            }
            public class IncomingTroops2
            {
                public class TablesIdx
                {
                    public static int players = 0;
                    public static int villages = 1;
                    public static int commands= 2;
                    public static int cacheTimeStamp = 3;
                }
                public class PlayersColIdx
                {
                    public static int PID= 0;
                    public static int PNAME = 1;
                }
                public class VillagesColIdx
                {
                    public static int VID= 0;
                    public static int VName= 1;
                    public static int X= 2;
                    public static int Y= 3;
                }
              
                public class CommandsColIdx
                {
                    public static int EventID = 0;
                    public static int OriginVillageID = 1;
                    public static int DestinationVillageID = 2;
                    public static int CommandType = 3;
                    public static int EventTime = 4;
                    public static int OriginPlayerID = 5;
                    public static int VisibleToTarget = 6;
                    public static int Hidden= 7;
                    public static int Duration = 8;
                    
                }
            }
            public class OutgoingTroops
            {
                public class ColNames
                {
                    public static string EventID = "EventID";
                    public static string OriginVillageID = "OriginVillageID";
                    public static string DestinationVillageID = "DestinationVillageID";
                    public static string CommandType = "CommandType";
                    public static string EventTime = "EventTime";
                    public static string DestinationVillageName = "DestinationVillageName";
                    public static string DestinationPlayerName = "DestinationPlayerName";
                    public static string DestinationPlayerID = "DestinationPlayerID";
                    public static string OriginVillageName = "OriginVillageName";
                    public static string OriginPlayerName = "OriginPlayerName";
                    public static string OriginPlayerID = "OriginPlayerID";
                    public static string DestVillageX = "DestVillageX";
                    public static string DestVillageY = "DestVillageY";
                    public static string OriginVillageX = "OriginVillageX";
                    public static string OriginVillageY = "OriginVillageY";
                    public static string VisibleToTarget = "VisibleToTarget";
                }
                public class ColIdx
                {
                    public static int EventID = 0;
                    public static int OriginVillageID = 1;
                    public static int DestinationVillageID =2;
                    public static int CommandType = 3;
                    public static int EventTime = 4;
                    public static int DestinationVillageName = 5;
                    public static int DestinationPlayerName = 6;
                    public static int DestinationPlayerID = 7;
                    public static int OriginVillageName = 8;
                    public static int OriginPlayerName = 9;
                    public static int OriginPlayerID = 10;
                    public static int DestVillageX = 11;
                    public static int DestVillageY = 12;
                    public static int OriginVillageX = 13;
                    public static int OriginVillageY = 14;
                    public static int VisibleToTarget = 15;
                }

                public class VisibleToTargetColValues
                {
                    public static short NotVisibleAtAll = 0;
                    public static short FromNotVisible = 1;
                    public static short AllVisible = 2;
                }


            }
            public class OutgoingTroops2
            {
                public class TablesIdx
                {
                    public static int players = 0;
                    public static int villages = 1;
                    public static int commands = 2;
                    public static int cacheTimeStamp = 3;                    
                }
                public class PlayersColIdx
                {
                    public static int PID = 0;
                    public static int PNAME = 1;
                }
                public class VillagesColIdx
                {
                    public static int VID = 0;
                    public static int VName = 1;
                    public static int X = 2;
                    public static int Y = 3;
                }

                public class CommandsColIdx
                {
                    public static int EventID = 0;
                    public static int OriginVillageID = 1;
                    public static int DestinationVillageID = 2;
                    public static int CommandType = 3;
                    public static int EventTime = 4;
                    public static int DestinationPlayerID = 5;
                    public static int VisibleToTarget = 6;
                    public static int Hidden = 7;
                    public static int Duration = 8;
                    public static int isUnderBloodLust = 9;
                    public static int Morale = 10;
                }
            }         
        }


        /// <summary>
        /// class that handles the desertion of troops
        /// </summary>
        public class Desertion 
        {
            public double factor;

            /// <summary>
            /// best not to creat an instance of this class manually. use the 
            /// static "GetUnitDesertion" method to get this object instead
            /// </summary>
            /// <param name="factor"></param>
            public Desertion(double factor)
            {
                this.factor = factor;
            }

            public bool IsThereAnyDesertion
            {
                get
                {
                    return factor != 0;
                }
            }
            public int GetUnitsDeserted(int unitCount)
            {
                return Convert.ToInt32(Math.Round(unitCount * factor, MidpointRounding.AwayFromZero));
            }




            /// <summary>
            /// to get the number of troops deserted, do: Math.Round(GetUnitDesertionFactor(...) * [NUMBR OF UNITS],  MidpointRounding.AwayFromZero)
            /// </summary>
            /// <param name="distance"></param>
            /// <param name="unitsByPopulation"></param>
            /// <returns></returns>
            public static Desertion GetUnitDesertion(Fbg.Common.UnitCommand.CommandType commandType, double distance, int unitsByPopulation, bool isGovPresent, bool isSpecialPlayer, Realm realm)
            {
                if (commandType == Fbg.Common.UnitCommand.CommandType.Attack
                    && !isGovPresent
                    && distance > realm.UnitDesertionMinDistance
                    && unitsByPopulation <= realm.UnitDesertionMaxPopulation
                    && !isSpecialPlayer
                    && realm.UnitDesertionScalingFactor > 0)
                {
                    double a = Math.Pow(distance - realm.UnitDesertionMinDistance, 2) * realm.UnitDesertionScalingFactor;
                    double b = Math.Pow(unitsByPopulation, 2);
                    return new Desertion(1 - (1 / (a / b + 1)));
                }
                else
                {
                    return new Desertion(0);
                }
            }



            /// <summary>
            /// to get the number of troops deserted, do: Math.Round(GetUnitDesertionFactor(...) * [NUMBR OF UNITS],  MidpointRounding.AwayFromZero)
            /// </summary>
            /// <param name="distance"></param>
            /// <param name="unitsByPopulation"></param>
            /// <returns></returns>
            public static Desertion GetUnitDesertion(Fbg.Common.UnitCommand.CommandType commandType, double distance, int unitsByPopulation, bool isGovPresent, int targetPlayerID, Realm realm)
            {

                return Desertion.GetUnitDesertion(commandType, distance, unitsByPopulation, isGovPresent, Fbg.Bll.utils.IsSpecialPlayer(targetPlayerID, realm), realm);
            }

            /// <summary>
            /// to get the number of troops deserted, do: Math.Round(GetUnitDesertionFactor(...) * [NUMBR OF UNITS],  MidpointRounding.AwayFromZero)
            /// </summary>
            /// <param name="distance"></param>
            /// <param name="unitsByPopulation"></param>
            /// <returns></returns>
            public static Desertion GetUnitDesertion(Fbg.Common.UnitCommand cmd, Realm realm, VillageOther targetVillage, VillageBasicA originVillage)
            {
                if (cmd.command == Fbg.Common.UnitCommand.CommandType.Attack)
                {
                    double distance;
                    distance = Village.CalculateDistance(originVillage.Cordinates.X, originVillage.Cordinates.Y, targetVillage.Cordinates.X, targetVillage.Cordinates.Y);

                    return Desertion.GetUnitDesertion(cmd.command, distance, UnitsByPopulation(cmd, realm), IsGovPresent(cmd), targetVillage.OwnerPlayerID, realm);
                }
                return new Desertion(0);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="distance"></param>
            /// <param name="unitsByPopulation"></param>
            /// <returns></returns>
            public static Desertion GetUnitDesertion(Fbg.Common.UnitCommand.CommandType commandType
                , Realm realm
                , List<KeyValuePair<UnitType, int>> unitsToSend
                , VillageOther targetVillage
                , VillageBasicA originVillage)
            {
                if (commandType == Fbg.Common.UnitCommand.CommandType.Attack)
                {
                    double distance;
                    distance = Village.CalculateDistance(originVillage.Cordinates.X, originVillage.Cordinates.Y, targetVillage.Cordinates.X, targetVillage.Cordinates.Y);

                    return Desertion.GetUnitDesertion(commandType, distance, UnitsByPopulation(unitsToSend, realm), IsGovPresent(unitsToSend), targetVillage.OwnerPlayerID, realm);
                }
                return new Desertion(0);
            }
        }
        /// <summary>
        /// CONSTS.UnitMovementDetColIndex specifies the column indexes for the returned datatable
        /// </summary>
        /// <param name="eventID"></param>
        /// <returns></returns>
        public static  DataTable GetUnitMovementDetails(Player player, long eventID)
        {
            return DAL.Units.GetUnitMovementDetails(player.Realm.ConnectionStr, player.ID, eventID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player">Player that is canceling the event. The event has to belong to this player in order for cancel to work. </param>
        /// <param name="eventID"></param>
        /// <returns>true if event was successfully delete</returns>
        public static bool Cancel(Player player, long eventID)
        {

            DataTable tbl = DAL.Units.GetUnitMovementDetails(player.Realm.ConnectionStr, player.ID, eventID);

            if (tbl.Rows.Count > 0)
            {
                DateTime eventTime = (DateTime )tbl.Rows[0][CONSTS.UnitMovementDetColIndex.EventTime];
                long duration = (long)tbl.Rows[0][CONSTS.UnitMovementDetColIndex.TripDuration];
                DateTime now = DateTime.Now;

                if (now < eventTime) //sanity check. lets  not cancel events that are already in the past. 
                {
                    DateTime newEventTime = now + new TimeSpan(duration) - (eventTime - now);

                    // Note, security will be handled by the SP itself. it will ensure that only the owner of the movement is able to cancel it. 
                    DAL.Units.CancelUnitMovement(player.Realm.ConnectionStr, eventID,  newEventTime, player.ID);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// This control will show you outgoing troops from one or more villages of the loggedInPlayer
        /// 
        /// PASS -1 for destinationPlayerID, -1 for destinationVillageID, -1 for originVillageID and you get 
        ///     all attacks & supports traveling from the logged in player
        /// 
        /// * Passing in a value for destinationPlayerID, limits the result to only troops moving TO one of the villages
        ///     of this player
        /// * Passing in a valid village id for destinationVillageID limits the result to only troops moving towards 
        ///     that village
        /// * Passing in a a valid village id for originVillageID limits the resul to the troops moving out of this village
        ///     This village has to belong to the logged in player. 
        /// </summary>
        /// <param name="realm"></param>
        /// <param name="loggedInPlayer">Represent the logged in player, the player who will view these troop movements. 
        /// System will ensure that these troops are indeed visible to this player, ie, that he can see them. </param>
        /// <param name="destinationPlayerID">can pass -1</param>
        /// <param name="destVillageID">can pass -1 </param>
        /// <param name="originVillageID">can pass -1 </param>
        /// <returns>DataTable described by UnitMovements.CONSTS.OutgoingTroops</returns>
        public static DataTable GetOutgoingTroops(Realm realm
            , Player loggedInPlayer
            , int destinationPlayerID
            , int destVillageID
            , int originVillageID
            , bool showAttacks
            , bool showSupports
            , bool showHidden)
        {
            if (loggedInPlayer == null)
            {
                throw new ArgumentNullException("Player loggedInPlayer");
            }
            if (realm == null)
            {
                throw new ArgumentNullException("Realm realm");
            }
            if (destVillageID < -1)
            {
                throw new ArgumentException("Either pass valid village id for destVillageID or -1. Got:" + destVillageID.ToString());
            }
            if (originVillageID < -1)
            {
                throw new ArgumentNullException("Either pass valid village id for originVillageID or -1. Got:" + originVillageID.ToString());
            }
            if (destinationPlayerID < -1)
            {
                throw new ArgumentNullException("Either pass valid player id for destinationPlayerID or -1. Got:" + destinationPlayerID.ToString());
            }

            return DAL.Units.GetOutgoingTroops(realm.ConnectionStr
                , loggedInPlayer.ID
                , destinationPlayerID
                , destVillageID
                , originVillageID
                , showAttacks
                , showSupports
                , showHidden);

        }
        /// <summary>
        /// This control will show you outgoing troops from one or more villages of the loggedInPlayer
        /// 
        /// PASS -1 for destinationPlayerID, -1 for destinationVillageID, -1 for originVillageID and you get 
        ///     all attacks & supports traveling from the logged in player
        /// 
        /// * Passing in a value for destinationPlayerID, limits the result to only troops moving TO one of the villages
        ///     of this player
        /// * Passing in a valid village id for destinationVillageID limits the result to only troops moving towards 
        ///     that village
        /// * Passing in a a valid village id for originVillageID limits the resul to the troops moving out of this village
        ///     This village has to belong to the logged in player. 
        /// </summary>
        /// <param name="realm"></param>
        /// <param name="loggedInPlayer">Represent the logged in player, the player who will view these troop movements. 
        /// System will ensure that these troops are indeed visible to this player, ie, that he can see them. </param>
        /// <returns>DataTable described by UnitMovements.CONSTS.OutgoingTroops2</returns>
        public static DataSet GetOutgoingTroops2(Realm realm
            , Player loggedInPlayer)
        {
            if (loggedInPlayer == null)
            {
                throw new ArgumentNullException("Player loggedInPlayer");
            }
            if (realm == null)
            {
                throw new ArgumentNullException("Realm realm");
            }
            return DAL.Units.GetOutgoingTroops2(realm.ConnectionStr
                , loggedInPlayer.ID);

        }

        /// <summary>
        /// For toPlayer, pass the player for whom you want to see incoming troops; ie, troops shown will be troops incoming to this player
        /// Optionally, pass valid village ID to see troops coming to this village only. Othwerwise pass -1
        /// * Optionally, pass valid village ID for originVillageID to see troops coming FROM this village only. Othwerwise pass -1
        /// * Optionally, pass valid player ID for originPlayerID to see troops coming FROM this player's villages only. Othwerwise pass -1        
        /// </summary>
        /// <param name="realm"></param>
        /// <param name="toPlayer">required</param>
        /// <param name="destinationVillageID">can be -1</param>
        /// <returns>DataTable described by UnitMovements.CONSTS.IncomingTroops</returns>
        public static DataTable GetIncomingTroops(Realm realm
            , Player toPlayer
            , int destinationVillageID
            , int originVillageID
            , int originPlayerID
            , bool showAttacks
            , bool showSupports
            , bool showReturns
            , bool showHidden)
        {
            return DAL.Units.GetIncomingTroops(realm.ConnectionStr
                , toPlayer.ID
                , destinationVillageID
                , originVillageID
                , originPlayerID
                , showAttacks
                , showSupports
                , showReturns
                , showHidden);
        }

        /// <summary>
        /// For toPlayer, pass the player for whom you want to see incoming troops; ie, troops shown will be troops incoming to this player
        /// Optionally, pass valid village ID to see troops coming to this village only. Othwerwise pass -1
        /// * Optionally, pass valid village ID for originVillageID to see troops coming FROM this village only. Othwerwise pass -1
        /// * Optionally, pass valid player ID for originPlayerID to see troops coming FROM this player's villages only. Othwerwise pass -1        
        /// </summary>
        /// <param name="realm"></param>
        /// <param name="toPlayer">required</param>
        /// <param name="destinationVillageID">can be -1</param>
        /// <returns>DataTable described by UnitMovements.CONSTS.IncomingTroops2</returns>
        public static DataSet GetIncomingTroops2(Realm realm
            , Player toPlayer
            )
        {

            string sp = "qIncomingTroops2";
            if (realm.ID >= 99 || realm.ID == 13 || realm.ID == 21 || realm.ID == 0 || realm.ID < 0)
            {
                sp = "qIncomingTroops3";
            }
            return DAL.Units.GetIncomingTroops2(realm.ConnectionStr, toPlayer.ID, sp);
        }






        /// <summary>
        /// DEPRECIATED!! Use UnitMovements.Desertion.GetUnitDesertion instead
        /// to get the number of troops deserted, do: Math.Round(GetUnitDesertionFactor(...) * [NUMBR OF UNITS],  MidpointRounding.AwayFromZero)
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="unitsByPopulation"></param>
        /// <returns></returns>
        public static double GetUnitDesertionFactor(double distance, int unitsByPopulation, bool isGovPresent, bool isSpecialPlayer, Realm realm)
        {
            if ( !isGovPresent
                && distance > realm.UnitDesertionMinDistance
                && unitsByPopulation <= realm.UnitDesertionMaxPopulation
                && !isSpecialPlayer
                && realm.UnitDesertionScalingFactor > 0 )
            {
                double a = Math.Pow(distance - realm.UnitDesertionMinDistance, 2) * realm.UnitDesertionScalingFactor;
                double b = Math.Pow(unitsByPopulation, 2);
                return 1 - (1 / (a / b + 1));
            }
            else
            {
                return 0;
            }
        }



        /// <summary>
        /// DEPRECIATED!! Use UnitMovements.Desertion.GetUnitDesertion instead
        /// to get the number of troops deserted, do: Math.Round(GetUnitDesertionFactor(...) * [NUMBR OF UNITS],  MidpointRounding.AwayFromZero)
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="unitsByPopulation"></param>
        /// <returns></returns>
        public static double GetUnitDesertionFactor(double distance, int unitsByPopulation, bool isGovPresent, int targetPlayerID, Realm realm)
        {

            return GetUnitDesertionFactor(distance, unitsByPopulation, isGovPresent, Fbg.Bll.utils.IsSpecialPlayer(targetPlayerID, realm), realm);
        }

        /// <summary>
        /// DEPRECIATED!! Use UnitMovements.Desertion.GetUnitDesertion instead
        /// to get the number of troops deserted, do: Math.Round(GetUnitDesertionFactor(...) * [NUMBR OF UNITS],  MidpointRounding.AwayFromZero)
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="unitsByPopulation"></param>
        /// <returns></returns>
        public static double GetUnitDesertionFactor(Fbg.Common.UnitCommand cmd,  Realm realm, VillageOther targetVillage, VillageBasicA originVillage)
        {
            if (cmd.command == Fbg.Common.UnitCommand.CommandType.Attack)
            {
                double distance;
                distance = Village.CalculateDistance(originVillage.Cordinates.X, originVillage.Cordinates.Y, targetVillage.Cordinates.X, targetVillage.Cordinates.Y);

                return GetUnitDesertionFactor(distance, UnitsByPopulation(cmd, realm), IsGovPresent(cmd), targetVillage.OwnerPlayerID, realm);
            }
            return 0;
        }

        /// <summary>
        /// DEPREACIATED use UnitMovements.Desertion.GetUnitsDeserted
        /// once you get the desertion factor from GetUnitDesertionFactor(...), you can use this to calculate how many units desert
        /// </summary>
        /// <param name="desertionFactor"></param>
        /// <param name="unitCount"></param>
        /// <returns></returns>
        public static int GetUnitsDeserted(double desertionFactor, int unitCount)
        {
            return Convert.ToInt32(Math.Round(unitCount * desertionFactor, MidpointRounding.AwayFromZero));
        }


        public static int UnitsByPopulation(Fbg.Common.UnitCommand cmd, Realm realm)
        {
            int unitsByPop = 0;
            foreach (Fbg.Common.UnitCommand.Units u in cmd.unitsSent)
            {
                unitsByPop += realm.GetUnitTypesByID(u.utID).Pop * u.sendCount;

            }
            return unitsByPop;
        }

        public static bool IsGovPresent(Fbg.Common.UnitCommand cmd)
        {
            foreach (Fbg.Common.UnitCommand.Units u in cmd.unitsSent)
            {
                if (u.utID == Fbg.Bll.CONSTS.UnitIDs.Gov)
                {
                    return true;
                }

            }
            return false;
        }
        public static int UnitsByPopulation(List<KeyValuePair<UnitType, int>> unitsToSend, Realm realm)
        {
            int unitsByPop = 0;
            foreach (KeyValuePair<UnitType, int> u in unitsToSend)
            {
                unitsByPop += u.Key.Pop * u.Value;

            }
            return unitsByPop;
        }

        public static bool IsGovPresent(List<KeyValuePair<UnitType, int>> unitsToSend)
        {
            foreach (KeyValuePair<UnitType, int> u in unitsToSend)
            {
                if (u.Key.ID == Fbg.Bll.CONSTS.UnitIDs.Gov)
                {
                    return true;
                }

            }
            return false;
        }



        /// <summary>
        /// returns 0 if now the event is not hidden 
        /// returns 1 if now the event is hidden 
        /// </summary>
        /// <param name="FbgPlayer"></param>
        /// <param name="eventID"></param>
        /// <returns></returns>
        public static int ToggleHide(Player player, long eventID)
        {
            return Convert.ToInt32(DAL.Units.ToggleHideUnitMovement(player.Realm.ConnectionStr, eventID, player.ID));
        }
    }
}
