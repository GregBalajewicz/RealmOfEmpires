using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fbg.Bll;

public partial class ClanMembersAjax : JsonPage
{
    public override object Result()
    {
        # region Player Part Of Clan


        if (Request.QueryString[CONSTS.QuerryString.PlayerID] != null && Request.QueryString[CONSTS.QuerryString.RoleID] != null && Request.QueryString[CONSTS.QuerryString.Action] != null)
        {
            int pid = Convert.ToInt32(Request.QueryString[CONSTS.QuerryString.PlayerID]);
            int rid = Convert.ToInt32(Request.QueryString[CONSTS.QuerryString.RoleID]);
            int Action = Convert.ToInt32(Request.QueryString[CONSTS.QuerryString.Action]);
            # region security for Owner and Admin
            if (FbgPlayer.Role.IsPlayerPartOfRole(Role.MemberRole.Owner))
            {
                if (Action == 1)//adding the role to specific player 
                {
                    if (!Role.AddPlayerRole(FbgPlayer, pid, rid))
                    {//Faild to add the player as the role alreday exist for this player
                        throw new Exception(RS("roleExits"));//"Role Already Exist for this player");
                    }

                }
                else
                {
                    if (!Role.RemovePlayerRole(FbgPlayer, pid, rid))
                    {
                        //Faild to delete the player as its the only Owner 
                        throw new Exception(RS("clanNeedsOneOwner"));//"This player is the only owner. Clan needs at least one owner");
                    }
                }
            }
            else if (FbgPlayer.Role.IsPlayerPartOfRole(Role.MemberRole.Administrator))
            {
                if (rid == (int)Role.MemberRole.Owner)
                {
                    throw new Exception(RS("onlyOwnersCan"));//"You can't make someone else an owner. Only owners can appoint other owners.");
                }
                else
                {
                    if (Action == 1)//adding the role to specific player
                    {
                        if (!Role.AddPlayerRole(FbgPlayer, pid, rid))
                        {//Faild to add the player as the role alreday exist for this player
                            throw new Exception(RS("roleExits"));//"Role already exist for this player");
                        }

                    }
                    else
                    {
                        if (!Role.RemovePlayerRole(FbgPlayer, pid, rid))
                        {
                            //Faild to delete the player as its the only Owner 
                            throw new Exception(RS("onlyOwner"));//"This player is the only owner you can't delete him");
                        }
                    }
                }
            }
            else
            {
                throw new Exception(RS("noPermission"));//"You don't have permssion to do that action");
            }
            #endregion
            
            return new { action = (Action == 0 ? 1 : 0) };
        }
        #endregion

        throw new Exception(RS("notEnough"));//"Not enough query-parameters");
    }
}
