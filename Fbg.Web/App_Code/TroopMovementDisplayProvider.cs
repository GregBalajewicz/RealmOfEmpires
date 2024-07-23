using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections.Generic;

/// <summary>
/// provides a reusable implementation  for ITroopMovementDisplay
/// </summary>
public class TroopMovementDisplayProvider : ITroopMovementDisplay
{
    public class ColIdx
    {
        public int fromPlayerID;
        public int fromPlayerName;
        public int toPlayerID;
        public int toPlayerName;
        public int fromVillageID;
        public int fromVillageName;
        public int fromVillageXCord;
        public int fromVillageYCord;
        public int toVillageID;
        public int toVillageName;
        public int toVillageXCord;
        public int toVillageYCord;
        public int visibleToTarget;

    }
    ColIdx _colIdx;
    DataTable _dt;

    List<Fbg.Common.DataStructs.Player> _toPlayers;
    List<Fbg.Common.DataStructs.Player> _fromPlayers;
    Dictionary<int, List<Fbg.Common.DataStructs.Village>> _fromVillages;
    Dictionary<int, List<Fbg.Common.DataStructs.Village>> _toVillages;

    public enum DirectionType
    {
        incoming,
        outgoing
    }
    DirectionType _type;
    Fbg.Bll.Player _currentPlayer;

    public TroopMovementDisplayProvider(Fbg.Bll.Player currentPlayer, ColIdx colDefinition, DataTable dt, DirectionType type)
    {
        if (colDefinition == null) throw new ArgumentNullException("colDefinition");
        if (dt == null) throw new ArgumentNullException("dt");

        _colIdx = colDefinition;
        _dt = dt;
        _type = type;
        _currentPlayer = currentPlayer;
    }


    #region ITroopMovementDisplay Members


    public bool CanSeeFrom(int originPlayerID, short visibleToTarget) //Object visibleToTarget, Object originPlayerID)
    {
        if (_currentPlayer.ID == originPlayerID)
        {
            return true;
        }
        else
        {
            if (visibleToTarget == Fbg.Bll.UnitMovements.CONSTS.IncomingTroops.VisibleToTargetColValues.AllVisible)
            {
                return true;
            }
        }
        return false;
    }

    public System.Collections.Generic.List<Fbg.Common.DataStructs.Player> GetToPlayers()
    {
        if (_toPlayers == null)
        {
            if (_type == DirectionType.incoming)
            {
                // empty list to signal that TO player is the logged in player
                _toPlayers = new List<Fbg.Common.DataStructs.Player>();
            }
            else
            {
                _toPlayers = new List<Fbg.Common.DataStructs.Player>();
                foreach (DataRow dr in _dt.Rows)
                {
                    if (!_toPlayers.Exists(delegate(Fbg.Common.DataStructs.Player p) { return p.ID == (int)dr[_colIdx.toPlayerID]; }))
                    {
                        _toPlayers.Add(
                            new Fbg.Common.DataStructs.Player()
                            {
                                ID = (int)dr[_colIdx.toPlayerID],
                                Name = (string)dr[_colIdx.toPlayerName]
                            });
                    }
                }
            }
        }

        return _toPlayers;
    }

    public System.Collections.Generic.List<Fbg.Common.DataStructs.Player> GetFromPlayers()
    {
        if (_fromPlayers == null)
        {
            if (_type == DirectionType.outgoing)
            {
                // empty list to signal that FROM player is the logged in player
                _fromPlayers = new List<Fbg.Common.DataStructs.Player>();
            }
            else
            {
                _fromPlayers = new List<Fbg.Common.DataStructs.Player>();
                foreach (DataRow dr in _dt.Rows)
                {
                    if (CanSeeFrom((int)dr[_colIdx.fromPlayerID], (short)dr[_colIdx.visibleToTarget])) // do not show the village if from payer is UNKNOWN
                    {
                        if (!_fromPlayers.Exists(delegate(Fbg.Common.DataStructs.Player p) { return p.ID == (int)dr[_colIdx.fromPlayerID]; }))
                        {
                            _fromPlayers.Add(
                                new Fbg.Common.DataStructs.Player()
                            {
                                ID = (int)dr[_colIdx.fromPlayerID],
                                Name = (string)dr[_colIdx.fromPlayerName]
                            });
                        }
                    }
                }
            }
        }
        return _fromPlayers;
    }

    /// <summary>
    /// </summary>
    /// <param name="playerID">send 0 if you just want all the villages</param>
    /// <returns></returns>
    public System.Collections.Generic.List<Fbg.Common.DataStructs.Village> GetToVillages(int playerID)
    {
        if (_toVillages == null)
        {
            _toVillages = new Dictionary<int, List<Fbg.Common.DataStructs.Village>>();
        }

        if (!_toVillages.ContainsKey(playerID))
        {
            List<Fbg.Common.DataStructs.Village> vills = new List<Fbg.Common.DataStructs.Village>();
            foreach (DataRow dr in _dt.Rows)
            {
                if (playerID == -1 || (int)dr[_colIdx.toPlayerID] == playerID)
                {
                    if (!vills.Exists(delegate(Fbg.Common.DataStructs.Village v) { return v.ID == (int)dr[_colIdx.toVillageID]; }))
                    {
                        vills.Add(new Fbg.Common.DataStructs.Village()
                        {
                            ID = (int)dr[_colIdx.toVillageID],
                            Name = (string)dr[_colIdx.toVillageName],
                            XCord = (int)dr[_colIdx.toVillageXCord],
                            YCord = (int)dr[_colIdx.toVillageYCord],
                        });
                    }
                }

            }
            _toVillages.Add(playerID, vills);
        }


        return _toVillages[playerID];
    }

    /// <summary>
    /// </summary>
    /// <param name="playerID">send 0 if you just want all the villages</param>
    /// <returns></returns>
    public System.Collections.Generic.List<Fbg.Common.DataStructs.Village> GetFromVillages(int playerID)
    {
        if (_fromVillages == null)
        {
            _fromVillages = new Dictionary<int, List<Fbg.Common.DataStructs.Village>>();
        }

        if (!_fromVillages.ContainsKey(playerID))
        {
            List<Fbg.Common.DataStructs.Village> vills = new List<Fbg.Common.DataStructs.Village>();
            foreach (DataRow dr in _dt.Rows)
            {
                if (playerID == -1 || (int)dr[_colIdx.fromPlayerID] == playerID)
                {
                    if (CanSeeFrom((int)dr[_colIdx.fromPlayerID], (short)dr[_colIdx.visibleToTarget])) // do not show the village if from payer is UNKNOWN
                    {
                        if (!vills.Exists(delegate(Fbg.Common.DataStructs.Village v) { return v.ID == (int)dr[_colIdx.fromVillageID]; }))
                        {
                            vills.Add(new Fbg.Common.DataStructs.Village()
                                {
                                    ID = (int)dr[_colIdx.fromVillageID],
                                    Name = (string)dr[_colIdx.fromVillageName],
                                    XCord = (int)dr[_colIdx.fromVillageXCord],
                                    YCord = (int)dr[_colIdx.fromVillageYCord],
                                });
                        }
                    }
                }

            }
            _fromVillages.Add(playerID, vills);
        }


        return _fromVillages[playerID];
    }

    #endregion
}
