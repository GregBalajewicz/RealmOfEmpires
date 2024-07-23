using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Fbg.Bll
{
    /// <summary>
    /// Like an Avatar class, but with status info that comes from UsersUnlockedAvatars table
    /// </summary>
    public class UserAvatar
    {

        public UserAvatar(Avatar av, int status)
        {
            this.AvatarID = av.AvatarID;
            this.AvatarType = av.AvatarType;
            this.ImageUrlS = av.ImageUrlS;
            this.ImageUrlL = av.ImageUrlL;
            this.Info = av.Info;
            this.Cost = av.Cost;
            this.Status = status;
        }

        public int AvatarID;
        public int AvatarType;
        public string ImageUrlS;
        public string ImageUrlL;
        public string Info;
        public int Cost;
        public int Status;

    }
}
