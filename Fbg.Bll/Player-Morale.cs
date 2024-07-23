using System;


namespace Fbg.Bll
{
    partial class Player
    {
        public PlayerMorale Morale { get; private set; }
        public void Morale_Set(int morale, DateTime moraleLastUpdated)
        {
            if (this.Morale.isDifferent(morale, moraleLastUpdated)) {
                this.Morale = new PlayerMorale(this, morale, moraleLastUpdated);
            }
        }
    }

    public class PlayerMorale
    {
        public class CONSTS {
           
        }

        private DateTime _moraleLastUpdated;
        private int _morale;
        Player _playerRef;
        public DateTime MoraleLastUpdatedOn
        {
            get
            {
                return _moraleLastUpdated;
            }
        }
        public int Morale {
            get
            {
                int change;
                DateTime now = DateTime.Now;
                TimeSpan ts = now.Subtract(_moraleLastUpdated);
                if (ts.TotalSeconds >= 1)
                {

                    _moraleLastUpdated = now;
                    change = Convert.ToInt32(Math.Floor((_playerRef.Realm.Morale.MoraleIncreasePerHour / 3600) * Convert.ToInt32(Math.Floor(ts.TotalSeconds))));
                    if (_morale + change <= _playerRef.Realm.Morale.MaxMorale)
                    {
                        _morale = _morale + change;
                    }
                    else
                    {
                        _morale = _playerRef.Realm.Morale.MaxMorale;
                    }
                }
                return _morale;
            }
        }


        public bool isDifferent(int morale, DateTime moraleLastUpdated)
        {
            return morale != this._morale || moraleLastUpdated != _moraleLastUpdated;
        }
        public PlayerMorale(Player playerRef, int morale, DateTime moraleLastUpdated)
        {
            _playerRef = playerRef;
            _morale = morale;
            _moraleLastUpdated = moraleLastUpdated;
        }

        public Realm.PlayerMorale.Effect Effect
        {
            get
            {
                return _playerRef.Realm.Morale.GetEffect(this.Morale);
            }
        }
    }
}
