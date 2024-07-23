using System;
using System.Collections.Generic;
using System.Text;

namespace Fbg.Common
{
    public class Mail
    {
        public enum BlockPlayerResult : int
        {
            Success = 0,
            Blocked_Player_Not_Exist = 1,
            Player_Already_Blocked = 2,
            Cannot_Block_Yourself = 3
        }
    }
}
