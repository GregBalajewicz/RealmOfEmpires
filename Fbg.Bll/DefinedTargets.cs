using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fbg.Bll.Items2;
using System.Data;

namespace Fbg.Bll
{

    public class DefinedTarget {
       

        public int DefinedTargetID { get; internal set; }
        public int TargetDefinitionOwnerPlayerID {get ; internal set; }
        public string TargetDefinitionOwnerName { get; internal set; }
        public int TargetDefinitionOwnerAvatarID { get; internal set; }
        public int TargetVillageID { get; internal set; }
        public int TargetVillageXCord { get; internal set; }
        public int TargetVillageYCord { get; internal set; }
        public string TargetVillageName { get; internal set; }
        public string TargetVillageOwnerPlayerName { get; internal set; }
        public int TargetVillageOwnerPlayerID { get; internal set; }
        public int TargetVillageOwnerPlayerClanID { get; internal set; }
        public DateTime TimeCreated { get; internal set; }
        public DateTime? SetTime { get; internal set; }
        public DateTime ExpiresOn { get; internal set; }
        public Int16 Type { get; internal set; }
        public string Note { get; internal set; }      
        public List<Response> Responses { get; internal set; }
        /// <summary>
        /// ID of player that this target is assigned to. May be null if not assigned to anyone
        /// </summary>
        public int? AssingedToPlayerID { get; internal set; }
        /// <summary>
        /// ID of player that this target is assigned to. May be empty if not assigned to anyone
        /// </summary>
        public string AssignedToPlayerName { get; internal set; }


        public class Response {
            public int PlayerID { get; internal set; }
            public string PlayerName { get; internal set; }
            public DateTime TimeLastUpdated { get; internal set; }
            public Int16 ResponseTypeID { get; internal set; }
            public string Message { get; internal set; }
            public int PlayerAvatarID { get; internal set; }

        }

    }


    public class DefinedTargets
    {
        Player _playerRef;
        public DefinedTargets(Fbg.Bll.Player playerRef)
        {
            _playerRef = playerRef;
        }


        public List<DefinedTarget> Add(int villageid, short typeID, DateTime? setTime, string note, out int definedTargetID, int expiresInXDays, string assignedTo)
        {
            DataSet ds = Fbg.DAL.Players.DefinedTargets_Add(this._playerRef.Realm.ConnectionStr, this._playerRef.ID, villageid, typeID, setTime, note, expiresInXDays, assignedTo);
            definedTargetID = (int)ds.Tables[2].Rows[0][0];
            return CreateDefinedTargetFromDataSet(ds);
        }

        public List<DefinedTarget> Edit(int definedTargetID, DateTime? setTime, string note, int expiresInXDays, string assignedTo)
        {
            DataSet ds = Fbg.DAL.Players.DefinedTargets_Edit(this._playerRef.Realm.ConnectionStr, this._playerRef.ID, definedTargetID, setTime, note, expiresInXDays,  assignedTo);
            return CreateDefinedTargetFromDataSet(ds);
        }

        private List<DefinedTarget> CreateDefinedTargetFromDataSet(DataSet ds)
        {
            DataTable dtDefeinedTargets = ds.Tables[0];
            DataTable dtResponses = ds.Tables[1];
            /*
                DefinedTargetID		,    
		        callerPlayerID			,
		        VillageID			,
		        Xcord				,
		        YCord				,
		        Vname		,           5
		        Pname		,
		        TimeCreated			,
		        SetTime,
		        DefinedTargetTypeID	,
		        Note,
		        callerName		        11
                callerAvatarID
                ExpiresOn
		        PID,	--village owner player ID
		        PClanID,	--village owner player clan ID  15
		        D.AssignedTo AssignedToPlayerID,
		        ATP.Name AssignedToPlayerName               17
            */
            List<DefinedTarget> list =
            (from dr in dtDefeinedTargets.AsEnumerable()
             select
             new DefinedTarget()
             {
                 DefinedTargetID = dr.Field<int>(0)
                 ,
                 TargetDefinitionOwnerPlayerID = dr.Field<int>(1)
                 ,
                 TargetVillageID = dr.Field<int>(2)
                 ,
                 TargetVillageXCord = dr.Field<int>(3)
                 ,
                 TargetVillageYCord = dr.Field<int>(4),
                 TargetVillageName = dr.Field<string>(5),
                 TargetVillageOwnerPlayerName = dr.Field<string>(6),
                 TimeCreated = dr.Field<DateTime>(7),
                 SetTime = dr.Field<DateTime>(8),//(DateTime?)(dr.Field<DateTime>(8) is DBNull ? null : (DateTime?) dr.Field<DateTime>(8)),
                 Type = dr.Field<Int16>(9),
                 Note = dr.Field<string>(10),
                 TargetDefinitionOwnerName = dr.Field<string>(11),
                 TargetDefinitionOwnerAvatarID = dr.Field<Int16>(12),
                 ExpiresOn = dr.Field<DateTime>(13),
                 TargetVillageOwnerPlayerID = dr.Field<int>(14),
                 TargetVillageOwnerPlayerClanID = dr.Field<int>(15),
                 AssingedToPlayerID = dr.Field<object>(16) == null ? null : (int?)dr.Field<int>(16),
                 AssignedToPlayerName = dr.Field<string>(17),
                 Responses = (from res in
                                (dtResponses.Select(string.Format("DefinedTargetID = {0}", dr.Field<int>(0))).AsEnumerable())
                              select new DefinedTarget.Response()
                              {
                                  /*
                                      R.DefinedTargetID	,    
                                      R.PlayerID			,
                                      Name				,
                                      ResponseTypeID		, 3
                                      Response,
                                      TimeLastUpdated	
                                      AvatarID
                                   */
                                  PlayerID = res.Field<int>(1),
                                  PlayerName = res.Field<string>(2),
                                  ResponseTypeID = res.Field<Int16>(3),
                                  Message = res.Field<string>(4),
                                  TimeLastUpdated = res.Field<DateTime>(5),
                                 PlayerAvatarID = res.Field<Int16>(6),
                              }).ToList()
             }).ToList();

            return list;
        }

        public List<DefinedTarget> Get() {
            DataSet ds = Fbg.DAL.Players.DefinedTargets_Get(this._playerRef.Realm.ConnectionStr, this._playerRef.ID);

            return CreateDefinedTargetFromDataSet(ds);
        }

        public object Delete(int definedTargetID)
        {
            DataSet ds = Fbg.DAL.Players.DefinedTargets_Delete(this._playerRef.Realm.ConnectionStr, this._playerRef.ID, definedTargetID);

            return CreateDefinedTargetFromDataSet(ds);
        }

        /// <summary>
        /// adding a response with Response being null or empty, effecitvely removes a response if there is one, and does not add one if there was none
        /// </summary>
        /// <param name="definedTargetID"></param>
        /// <param name="ResponseByPlayerID"></param>
        /// <param name="Response"></param>
        /// <returns></returns>
        public List<DefinedTarget> AddEditResonse(int definedTargetID, int responseByPlayerID, Int16 responseTypeID, string response)
        {

            DataSet ds = Fbg.DAL.Players.DefinedTargets_AddEditResonse(this._playerRef.Realm.ConnectionStr, definedTargetID, responseByPlayerID, responseTypeID, response);
            return CreateDefinedTargetFromDataSet(ds);
        }
    }
}
