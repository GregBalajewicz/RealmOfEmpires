using System;
using System.Collections.Generic;
using System.Text;

namespace Fbg.Common
{
    public class Clan
    {
        public enum DissmissFromClanResult : int
        {
            Success = 0,
            TryingToDismissLastOwner = -1,
            AdminTryingToDismissOwner = -2,
        }

    }
}
