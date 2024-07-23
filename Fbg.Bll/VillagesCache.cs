using System;
using System.Collections.Generic;
using System.Text;

namespace Fbg.Bll
{


    public class VillagesCache
    {
        //List<Village> _villages = new List<Village>();
        SortedList<int, Village> _villages;
        Player _player;

        public VillagesCache(Player p)
        {
            _player = p;
            _villages = new SortedList<int, Village>();
        }

        public void InvalidateVillage(int villageID)
        {
            if (_villages.ContainsKey(villageID))
            {
                _villages[villageID] = null;
            }
        }

        public void InvalidateAllVillages()
        {
            _villages = new SortedList<int, Village>(_villages.Count);
        }

        public Village Village(int villageID)
        {
            Village v;

            
            if (!_villages.ContainsKey(villageID))
            {
                v = _player.Village(villageID);
                if (v != null)
                {
                    _villages.Add(v.id, v);
                }
            }
            v = _villages[villageID];
            if (v == null) 
            {
                v = _player.Village(villageID);
                if (v != null)
                {
                    _villages[villageID] = v;
                }
            }

            return v;
        }
    }
}
