using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Fbg.Bll
{
    public class Avatar
    {


        DataRow dr;
        public Avatar(DataRow dr)
        {
            this.dr = dr;
        }


        public int AvatarID
        {
            get 
            {
                return Convert.ToInt32(dr[Avatars.CONSTS.ColIndx.Avatars.AvatarID]);
            }
        }

        /// <summary>
        /// type 1 - standard avatar, everyone has it, sees it, and can use it
        /// type 2 - everyone sees it, but must be owned to be used. It can be unlocked by doing something, or for some cost
        /// type 3 - only those who have been given it can see it and use it.
        /// </summary>
        public int AvatarType
        {
            get
            {
                return (int)dr[Avatars.CONSTS.ColIndx.Avatars.AvatarType];
            }
        }

        /// <summary>
        /// small image, used for avatar thumb 
        /// </summary>
        public string ImageUrlS
        {
            get
            {
                return (string)dr[Avatars.CONSTS.ColIndx.Avatars.ImageUrlS];
            }
        }

        /// <summary>
        /// larage full avatar illustration
        /// </summary>
        public string ImageUrlL
        {
            get
            {
                return (string)dr[Avatars.CONSTS.ColIndx.Avatars.ImageUrlL];
            }
        }

        /// <summary>
        /// Any info or description the avatar has. Can be null in DB
        /// </summary>
        public string Info
        {
            get
            {
                return dr[Avatars.CONSTS.ColIndx.Avatars.Info] == DBNull.Value ? string.Empty : (string)dr[Avatars.CONSTS.ColIndx.Avatars.Info];
            }
        }

        /// <summary>
        /// If avatar has cost, it is buyable. Can be null in DB
        /// </summary>
        public int Cost
        {
            get
            {
                return dr[Avatars.CONSTS.ColIndx.Avatars.Cost] == DBNull.Value ? 0 : (int)dr[Avatars.CONSTS.ColIndx.Avatars.Cost];
            }
        }

    }
}
