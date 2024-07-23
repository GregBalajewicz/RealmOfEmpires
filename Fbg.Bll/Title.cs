using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Fbg.Bll
{
    public class Title
    {
        DataRow _dr;
        Realm _realm;

        public static bool operator ==(Title x, Title y)
        {
            if ((object)x == null && (object)y == null) return true;
            if ((object)x != null && (object)y != null)
            {
                if (x.ID == y.ID) return true;
            }
            return false;
        }

        public static bool operator !=(Title x, Title y)
        {
            if ((object)x == null && (object)y == null) return false;
            if ((object)x != null && (object)y != null)
            {
                if (x.ID != y.ID) return true;
            }
            else
            {
                return true;
            }
            return false;
        }
        public static bool operator <(Title x, Title y)
        {
            if (x.ID < y.ID) return true;
            return false;
        }
        public static bool operator >(Title x, Title y)
        {
            if (x.ID > y.ID) return true;
            return false;
        }
        public static bool operator <=(Title x, Title y)
        {
            if (x.ID <= y.ID) return true;
            return false;
        }
        public static bool operator >=(Title x, Title y)
        {
            if (x.ID >= y.ID) return true;
            return false;
        }


        //// Inequality operator. Returns dbNull if either operand is
        //// dbNull, otherwise returns dbTrue or dbFalse:
        //public static DBBool operator !=(DBBool x, DBBool y)
        //{
        //    if (x.value == 0 || y.value == 0) return dbNull;
        //    return x.value != y.value ? dbTrue : dbFalse;
        //}


        
        internal class PlayerOptionsAnonymousCloumnIndex
        {
            public const int Anonymous = 0;
        }

        public Title(DataRow dr, Realm realm )
        {
            _dr = dr;
            _realm = realm;
        }

        public int ID
        {
            get
            {
                return (int)_dr[Realm.CONSTS.TitlesColIndex.TitleID];
            }
        }
        public int MaxPoints
        {
            get
            {
                return (int)_dr[Realm.CONSTS.TitlesColIndex.MaxPoints];
            }
        }
        public int XP
        {
            get
            {
                return (int)_dr[Realm.CONSTS.TitlesColIndex.XP];
            }
        }
        public int XP_Cumulative
        {
            get
            {
                return (int)_dr[Realm.CONSTS.TitlesColIndex.XP_Cumulative];
            }
        }
        public string TitleName_Male
        {
            get
            {
                return (string)_dr[Realm.CONSTS.TitlesColIndex.Title_Male];
            }
        }
        public string TitleName_Female
        {
            get
            {
                return (string)_dr[Realm.CONSTS.TitlesColIndex.Title_Female];
            }
        }

        public string TitleName(Fbg.Common.Sex sex)
        {
            switch (sex)
            {
                case Fbg.Common.Sex.Female:
                    return TitleName_Female;
                default:
                    return TitleName_Male;
            }

        }

        public long Level 
        {
            get
            {
                return (long)_dr[Realm.CONSTS.TitlesColIndex.Level];
            }
        }

        public string Desc
        {
            get
            {
                return (string)_dr[Realm.CONSTS.TitlesColIndex.Desc];
            }
        }
        
        /// <summary>
        /// may return NULL if there is no prev title, ie this is the first title
        /// </summary>
        public Title PreviousTitle
        {
            get
            {
                return _realm.TitleByLevel(this.Level - 1);
            }
        }
        /// <summary>
        /// may return NULL if there is no next title, ie this is the last title
        /// </summary>
        public Title NextTitle
        {
            get
            {
                return _realm.TitleByLevel(this.Level + 1);
            }
        }

        /// <summary>
        /// may return NULL which means that no more titles are available. 
        /// Otherwise returns the maximum possible title that the player qualifies for.
        /// If title returned == player.Title then it means the player already has the highest possible title
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public Title MyMaxTitle(Player player, int myPoints)
        {
            return Title.GetMaxTitle(myPoints, player.Title);
        }

        public static Title GetMaxTitle(int points, Title title)
        {
            if (title != null)
            {
                if (points > title.MaxPoints)
                {
                    return GetMaxTitle(points, title.NextTitle);
                }
                else
                {
                    return title;
                }
            }
            return null;
        }

    }
}
