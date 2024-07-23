using System;
using System.Collections.Generic;
using System.Text;

namespace Fbg.Bll
{
    public class Quests
    {
        //public enum Quests
        //{
        //    CompleteTutorial = 0
        //}

        Player _player;
        public Quests(Player player)
        {
            _player = player;
        }

        public bool AdvisorAccessed
        {
            get
            {
                return _player.HasFlag(Player.Flags.Quests_AdvisorAccessed) != null;
            }
            set
            {
                if (_player.HasFlag(Player.Flags.Quests_AdvisorAccessed) == null)
                {
                    _player.SetFlag(Player.Flags.Quests_AdvisorAccessed);
                }

            }
        }
        public bool InviteButtonClicked
        {
            get
            {
                return _player.HasFlag(Player.Flags.Quests_InviteClicked) != null;
            }
            set
            {
                if (_player.HasFlag(Player.Flags.Quests_InviteClicked) == null)
                {
                    _player.SetFlag(Player.Flags.Quests_InviteClicked);
                }
            }
        }

        public bool HasPlayerMadeAForumPost
        {
            get
            {
                return Fbg.DAL.Quests.HasPlayerMadeAForumPost(_player.Realm.ConnectionStr, _player.Name);
            }
        }

    }
}
