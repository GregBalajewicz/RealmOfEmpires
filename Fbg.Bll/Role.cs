using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Gmbc.Common.Diagnostics.ExceptionManagement;

namespace Fbg.Bll
{
    public class Role
        {
        internal class CONSTS
        {

            public class RoleColumnIndex
            {
                public static int RoleID = 0;

            }
        }
        public enum MemberRole : int
        {
            Owner = 0,
            Inviter = 2,
            Administrator = 3,
            ForumAdministrator = 4,
            Diplomat =5
        }
        public   DataTable  dtRoles;

        public Role(DataTable dt)
        {
            dtRoles = dt;
        }
        public  bool  IsPlayerPartOfRole(MemberRole Role)
        {
            foreach (DataRow dr in dtRoles.Rows )
            {
                if ((int)dr[CONSTS.RoleColumnIndex.RoleID]==(int)Role)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="Player"></param>
        /// <param name="PlayerID"></param>
        /// <param name="RoleID"></param>
        /// <returns>true if success false if failed </returns>
        public static bool  AddPlayerRole(Player Player, int PlayerID, int RoleID)
        {
            if (Player == null)
            {
                throw new ArgumentNullException("Player is null");

            }

            if (Player.Clan == null)
            {
                throw new ArgumentNullException("Clan is null");
            }

           return  DAL.Roles.AddPlayerRole(Player.Realm.ConnectionStr, Player.Clan.ID, PlayerID, RoleID);
        }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="Player"></param>
      /// <param name="PlayerID"></param>
      /// <param name="RoleID"></param>
        /// <returns>true if success false if failed </returns>
        public static bool  RemovePlayerRole(Player Player, int PlayerID, int RoleID)
        {
            if (Player == null)
            {
                throw new ArgumentNullException("Player is null");

            }

            if (Player.Clan == null)
            {
                throw new ArgumentNullException("Clan is null");
            }

           return  DAL.Roles.RemovePlayerRole (Player.Realm.ConnectionStr, Player.Clan.ID, PlayerID, RoleID);
        }
    }
}
