using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fbg.Bll
{
    public class UsersXP
    {

        public UsersXP(int xp)
        {
            XP = xp;
        }

        public int XP { get; set; }

        public int Level
        {
            get
            {
                return CalcLevel(XP);
            }
        }

        public static int CalcLevel(int xp)
        {
            return Convert.ToInt32(Math.Ceiling((decimal)xp / 1000));
        }
    }
}
