using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Gmbc.Common.Diagnostics.ExceptionManagement;
using System.Collections;

namespace Fbg.Bll
{
    public class Avatars : IEnumerable
    {

        public class CONSTS
        {
            internal class ColIndx
            {
                internal class Avatars
                {
                    public static int AvatarID = 0;
                    public static int AvatarType = 1;
                    public static int ImageUrlS = 2;
                    public static int ImageUrlL = 3;
                    public static int Info = 4;
                    public static int Cost = 5;
                }
            }
        }

        public Avatar GetAvatar(int id)
        {
            return _avatars.Find(delegate(Avatar a) { return a.AvatarID == id; });
        }

        private List<Avatar> _avatars
        {
            get;
            set;
        }

        public List<Avatar> listAll {
            get
            {
                return _avatars;
            }
        }

        internal Avatars(DataTable avatars)
        {
            _avatars = new List<Avatar>();
            foreach (DataRow dr in avatars.Rows)
            {
                _avatars.Add(new Avatar(dr));
            }
        }
        
        public IEnumerator GetEnumerator()
        {
            return _avatars.GetEnumerator();
        }


        public enum unlockUserAvatarByTag_AvatarTags
        {
            r100_reward_title_knight,
            moraleRealmSpecial,
            donaldSeries
        }


        //TODO - review
        /// <summary>
        /// given a string tag, unlocks a specific avatr for a User
        /// </summary>
        /// <param name="tag"></param>
        public void unlockUserAvatarByTag(Fbg.Bll.User user, unlockUserAvatarByTag_AvatarTags tag)
        {
            switch (tag)
            {
                case unlockUserAvatarByTag_AvatarTags.r100_reward_title_knight:
                    unlockUserAvatarByAvatarID(user, 33);
                    break;
                case unlockUserAvatarByTag_AvatarTags.moraleRealmSpecial:
                    unlockUserAvatarByAvatarID(user, 34);
                    unlockUserAvatarByAvatarID(user, 35);
                    break;
                case unlockUserAvatarByTag_AvatarTags.donaldSeries:
                    unlockUserAvatarByAvatarID(user, 37);
                    unlockUserAvatarByAvatarID(user, 38);
                    break;
            }
        }

        /// <summary>
        /// given an avatarID, unlocks the avatar for the User 
        /// </summary>
        /// <param name="avatarID"></param>
        public void unlockUserAvatarByAvatarID(Fbg.Bll.User user, int avatarID)
        {
            Fbg.DAL.User.UnlockUserAvatarByAvatarID(user.ID, avatarID);
        }

    }
}
