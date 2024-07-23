using System;
using System.Collections.Generic;
using System.Text;

namespace Fbg.Bll
{
    public class GiftAvailability
    {
        Gift _gift;
        Player _player;

        public Gift Gift
        {
            get { return _gift; }
        }
        bool _isAvailable;

        public bool IsAvailable
        {
            get { return _isAvailable; }
        }

        public String NotAvailableMessage
        {
            get
            {
                return _gift.NotAvailableMessage(_player.Sex);
            }
        }


        public GiftAvailability(Gift gift, Player player)
        {
            _gift = gift;
            _player = player;
            _isAvailable = _gift.CheckAvailability(player);
        }
    }
}
