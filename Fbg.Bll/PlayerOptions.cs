using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Fbg.Bll
{
    public class PlayerOptions
    {
        DataTable  _dtAnonymous;
        Player _player;
        
        internal class PlayerOptionsAnonymousCloumnIndex
        {
            public const int Anonymous = 0;

        }
        public PlayerOptions(DataTable dtAnonymous, Player player)
        {
            _dtAnonymous = dtAnonymous;
            _player = player;
        }


    }
}
