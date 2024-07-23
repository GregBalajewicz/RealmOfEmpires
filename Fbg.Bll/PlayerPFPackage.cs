using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Fbg.Bll
{
    public class PlayerPFPackage
    {
        DataRow _dr;
        Player _player;

        public PlayerPFPackage(DataRow dr, Player p)
        {
            this._dr = dr;
            _player = p;
        }


        public PFPackage Package
        {
            get
            {
                return _player.Realm.PFPackage((int)_dr[Player.CONSTS.PlayerPFPackageColIndex.PFPackageID]);
            }
        }

        public DateTime ExpiresOn
        {
            get
            {
                return (DateTime)_dr[Player.CONSTS.PlayerPFPackageColIndex.ExpiresOn];
            }
        }

         public bool IsActive
        {
            get
            {
                return ExpiresOn <= DateTime.Now;
            }
        }

    }
}
