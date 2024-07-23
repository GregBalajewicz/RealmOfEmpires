using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
namespace Fbg.Bll
{
    /// <summary>
    /// creates a class to describe the daily bonus 3x3 grid and the randomized daily bonus
    /// no matter what cell the user selects the prize will be DailyReward.Reward
    /// </summary>
    public class DailyReward
    {
        /// <summary>
        /// 
        /// </summary>
        public enum eDailyReward
        {
            SilverGift = 1,
            InfantryGift = 2,
            LightCalvaryGift = 3,
            KnightGift = 4,
            SpyGift = 5,
            Cornucopia = 6,
            Chests = 7,
            Servants = 8,
        }


        /// <summary>
        /// 
        /// </summary>
        private Player _player;
        /// <summary>
        /// the realm the user is requesting bonuses for
        /// </summary>
        private Realm _realm;
        /// <summary>
        /// the day from 1-14 - corresponds to consecutive days the reward has been accepted
        /// </summary>
        private int _level;
        /// <summary>
        /// private ctor
        /// </summary>
        private DailyReward() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="day">1-14</param>
        public DailyReward(Fbg.Bll.Player player, int level)
        {
            _player = player;
            _realm = player.Realm;
            _level = Math.Min(level, 14);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Gift GetGiftByID(int id)
        {
            Gift gift = null;
            foreach (Gift g in _realm.Gifts)
            {
                if (g.Id == id)
                {
                    gift = g;
                    break;
                }
            }
            return gift;
        }
        // Silver: ID=1
        // CM:     ID=2
        // 5CM:    ID=3
        // Inf:    ID=4
        // LC:     ID=5
        // Kn:     ID=6
        // Spy:    ID=7
        // Ram:    ID=8
        // Treb:   ID=9
        /// <summary>
        /// gets the rewards list based on number of days on realm and number of days active
        /// </summary>
        /// <returns></returns>
        public List<DailyRewardItem> GetRewards()
        {
            List<DailyRewardItem> items;

            //magnitude of rewards awarded, first index of this array is ignored because level min is 1
            int[] rewardValues;
            if (_realm.RealmType_isNoob)
            {
                rewardValues = new int[] { 0, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 14, 16, 18, 20 };
            }
            else {
                rewardValues = new int[] { 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 7, 9, 11, 13, 15 };
            }



            int rewardValue = 1;

            if (_level >= rewardValues.Length)
            {
                rewardValue = rewardValues[rewardValues.Length - 1];
            }
            else {
                rewardValue = rewardValues[_level];
            }

            if (_realm.RealmType_isNoob)
            {

                /*
                 in the noob realms, for all levels below 9, rewards will be silver gifts ranging from 1, to reward level value * 2
                 */
                if (_level < 9)
                {
                    Gift silverGift = GetGiftByID(1);
                    items = new List<DailyRewardItem>()
                    {
                        /*Silver Gift*/new DailyGiftRewardItem(_player, 1, eDailyReward.SilverGift, silverGift),
                        /*Silver Gift*/new DailyGiftRewardItem(_player, 1, eDailyReward.SilverGift, silverGift),
                        /*Silver Gift*/new DailyGiftRewardItem(_player, 1, eDailyReward.SilverGift, silverGift),
                        /*Silver Gift*/new DailyGiftRewardItem(_player, rewardValue, eDailyReward.SilverGift, silverGift),
                        /*Silver Gift*/new DailyGiftRewardItem(_player, rewardValue, eDailyReward.SilverGift, silverGift),
                        /*Silver Gift*/new DailyGiftRewardItem(_player, rewardValue, eDailyReward.SilverGift, silverGift),
                        /*Silver Gift*/new DailyGiftRewardItem(_player, rewardValue*2, eDailyReward.SilverGift, silverGift),
                        /*Silver Gift*/new DailyGiftRewardItem(_player, rewardValue*2, eDailyReward.SilverGift, silverGift),
                        /*Silver Gift*/new DailyGiftRewardItem(_player, rewardValue*2, eDailyReward.SilverGift, silverGift),
                    };
                }
                else
                {
                    //only include chests if playing for 7+ days
                    if (Math.Abs(_player.RegisteredOn.Subtract(DateTime.Now).Days) >= 7)
                    {
                        Gift silverGift = GetGiftByID(1);
                        Gift infGift = GetGiftByID(4);
                        Gift lcGift = GetGiftByID(5);
                        Gift knGift = GetGiftByID(6);
                        Gift spyGift = GetGiftByID(7);
                        Gift ramGift = GetGiftByID(8);
                        Gift trebGift = GetGiftByID(9);
                        items = new List<DailyRewardItem>()
                        {
                            /*Silver Gift*/new DailyGiftRewardItem(_player, rewardValue, eDailyReward.SilverGift, silverGift),
                            /*Silver Gift*/new DailyGiftRewardItem(_player, rewardValue, eDailyReward.SilverGift, silverGift),
                            /*Chests*/new DailyChestRewardItem(_player, _player.Villages.Count),
                            /*Infantry Gift*/new DailyGiftRewardItem(_player, rewardValue, eDailyReward.InfantryGift, infGift),
                            /*Spy Gift*/new DailyGiftRewardItem(_player, rewardValue, eDailyReward.SpyGift, spyGift),
                            /*Light Calvary Gift*/new DailyGiftRewardItem(_player, rewardValue, eDailyReward.LightCalvaryGift, lcGift),
                            /*Cornucopia*/new DailyCornucopiaRewardItem(_player, 4, knGift, infGift, ramGift, trebGift, spyGift),
                            /*Knight Gift*/new DailyGiftRewardItem(_player, rewardValue, eDailyReward.KnightGift, knGift),
                            /*Servants*/new DailyServantRewardItem(_player, 10)
                        };
                    }
                    else
                    {
                        Gift silverGift = GetGiftByID(1);
                        Gift infGift = GetGiftByID(4);
                        Gift lcGift = GetGiftByID(5);
                        Gift knGift = GetGiftByID(6);
                        Gift spyGift = GetGiftByID(7);
                        Gift ramGift = GetGiftByID(8);
                        Gift trebGift = GetGiftByID(9);
                        items = new List<DailyRewardItem>()
                    {
                        /*Silver Gift*/new DailyGiftRewardItem(_player, rewardValue, eDailyReward.SilverGift, silverGift),
                        /*Silver Gift*/new DailyGiftRewardItem(_player, rewardValue, eDailyReward.SilverGift, silverGift),
                        /*Light Calvary Gift*/new DailyGiftRewardItem(_player, rewardValue, eDailyReward.LightCalvaryGift, lcGift),
                        /*Infantry Gift*/new DailyGiftRewardItem(_player, rewardValue, eDailyReward.InfantryGift, infGift),
                        /*Spy Gift*/new DailyGiftRewardItem(_player, rewardValue, eDailyReward.SpyGift, spyGift),
                        /*Light Calvary Gift*/new DailyGiftRewardItem(_player, rewardValue, eDailyReward.LightCalvaryGift, lcGift),
                        /*Cornucopia*/new DailyCornucopiaRewardItem(_player, 3, knGift, infGift, ramGift, trebGift, spyGift),
                        /*Knight Gift*/new DailyGiftRewardItem(_player, rewardValue, eDailyReward.KnightGift, knGift),
                        /*Servants*/new DailyServantRewardItem(_player, 5)
                    };
                    }

                }



            }
            else //leaving non noob realms rewarding mostly untouched.
            {


                if (_level < 7)
                {
                    Gift silverGift = GetGiftByID(1);
                    Gift infGift = GetGiftByID(4);
                    Gift lcGift = GetGiftByID(5);
                    Gift knGift = GetGiftByID(6);
                    items = new List<DailyRewardItem>()
                    {
                        /*Silver Gift*/new DailyGiftRewardItem(_player, rewardValue, eDailyReward.SilverGift, silverGift),
                        /*Silver Gift*/new DailyGiftRewardItem(_player, rewardValue, eDailyReward.SilverGift, silverGift),
                        /*Silver Gift*/new DailyGiftRewardItem(_player, rewardValue, eDailyReward.SilverGift, silverGift),
                        /*Infantry Gift*/new DailyGiftRewardItem(_player, rewardValue, eDailyReward.InfantryGift, infGift),
                        /*Infantry Gift*/new DailyGiftRewardItem(_player, rewardValue, eDailyReward.InfantryGift, infGift),
                        /*Light Calvary Gift*/new DailyGiftRewardItem(_player, rewardValue, eDailyReward.LightCalvaryGift, lcGift),
                        /*Light Calvary Gift*/new DailyGiftRewardItem(_player, rewardValue, eDailyReward.LightCalvaryGift, lcGift),
                        /*Knight Gift*/new DailyGiftRewardItem(_player, rewardValue, eDailyReward.KnightGift, knGift),
                        /*Servants*/new DailyServantRewardItem(_player, 3)
                    };
                }
                else
                {

                    //only include chests if playing for 7+ days
                    if (Math.Abs(_player.RegisteredOn.Subtract(DateTime.Now).Days) >= 7)
                    {
                        Gift silverGift = GetGiftByID(1);
                        Gift infGift = GetGiftByID(4);
                        Gift lcGift = GetGiftByID(5);
                        Gift knGift = GetGiftByID(6);
                        Gift spyGift = GetGiftByID(7);
                        Gift ramGift = GetGiftByID(8);
                        Gift trebGift = GetGiftByID(9);
                        items = new List<DailyRewardItem>()
                        {
                            /*Silver Gift*/new DailyGiftRewardItem(_player, rewardValue, eDailyReward.SilverGift, silverGift),
                            /*Silver Gift*/new DailyGiftRewardItem(_player, rewardValue, eDailyReward.SilverGift, silverGift),
                            /*Chests*/new DailyChestRewardItem(_player, _player.Villages.Count),
                            /*Infantry Gift*/new DailyGiftRewardItem(_player, rewardValue, eDailyReward.InfantryGift, infGift),
                            /*Spy Gift*/new DailyGiftRewardItem(_player, rewardValue, eDailyReward.SpyGift, spyGift),
                            /*Light Calvary Gift*/new DailyGiftRewardItem(_player, rewardValue, eDailyReward.LightCalvaryGift, lcGift),
                            /*Cornucopia*/new DailyCornucopiaRewardItem(_player, 3, knGift, infGift, ramGift, trebGift, spyGift),
                            /*Knight Gift*/new DailyGiftRewardItem(_player, rewardValue, eDailyReward.KnightGift, knGift),
                            /*Servants*/new DailyServantRewardItem(_player, 10)
                        };
                    }
                    else
                    {
                        Gift silverGift = GetGiftByID(1);
                        Gift infGift = GetGiftByID(4);
                        Gift lcGift = GetGiftByID(5);
                        Gift knGift = GetGiftByID(6);
                        Gift spyGift = GetGiftByID(7);
                        Gift ramGift = GetGiftByID(8);
                        Gift trebGift = GetGiftByID(9);
                        items = new List<DailyRewardItem>()
                    {
                        /*Silver Gift*/new DailyGiftRewardItem(_player, rewardValue, eDailyReward.SilverGift, silverGift),
                        /*Silver Gift*/new DailyGiftRewardItem(_player, rewardValue, eDailyReward.SilverGift, silverGift),
                        /*Light Calvary Gift*/new DailyGiftRewardItem(_player, rewardValue, eDailyReward.LightCalvaryGift, lcGift),
                        /*Infantry Gift*/new DailyGiftRewardItem(_player, rewardValue, eDailyReward.InfantryGift, infGift),
                        /*Spy Gift*/new DailyGiftRewardItem(_player, rewardValue, eDailyReward.SpyGift, spyGift),
                        /*Light Calvary Gift*/new DailyGiftRewardItem(_player, rewardValue, eDailyReward.LightCalvaryGift, lcGift),
                        /*Cornucopia*/new DailyCornucopiaRewardItem(_player, 3, knGift, infGift, ramGift, trebGift, spyGift),
                        /*Knight Gift*/new DailyGiftRewardItem(_player, rewardValue, eDailyReward.KnightGift, knGift),
                        /*Servants*/new DailyServantRewardItem(_player, 10)
                    };
                    }

                }
            }

                
            


            return items;
        }
        /// <summary>
        /// returns the shuffled rewards
        /// </summary>
        /// <returns></returns>
        public List<DailyRewardItem> GetShuffledRewards()
        {
            List<DailyRewardItem> rewards = GetRewards();
            List<DailyRewardItem> shuffledRewards = ShuffleRewards(rewards);
            return shuffledRewards;
        }
        /// <summary>
        /// shuffles the rewards list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        static List<DailyRewardItem> ShuffleRewards(List<DailyRewardItem> rewards)
        {
            var provider = new RNGCryptoServiceProvider();
            int n = rewards.Count;
            while (n > 1)
            {
                var box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < n * (Byte.MaxValue / n)));
                var k = (box[0] % n);
                n--;
                var value = rewards[k];
                rewards[k] = rewards[n];
                rewards[n] = value;
            }
            return rewards;
        }
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>

        /// <summary>
        /// 
        /// </summary>
        public abstract class DailyRewardItem
        {
            protected Fbg.Bll.Player _player;
            public eDailyReward Type { get; protected set; }
            public int Number { get; protected set; }
            public string Title { get; protected set; }
            public string Desc { get; protected set; }
            public string ImageUrl { get; protected set; }
            protected abstract bool _UpdatePlayer();
            public abstract string GetLongDesc();
            private void UpdatePlayerFlag()
            {
                string newData = string.Format("{0}", _player.GetDailyRewardLevel() + 1);
                _player.SetFlag(Player.Flags.Misc_GiftAccepted, newData);
            }
            public bool UpdatePlayer()
            {
                bool updated = _UpdatePlayer();
                if (updated)
                {
                    UpdatePlayerFlag();
                }
                return updated;
            }
            public DailyRewardItem(Fbg.Bll.Player player, eDailyReward type, int number, string title, string imageUrl)
            {
                _player = player;
                Type = type;
                Title = title;
                ImageUrl = imageUrl;
                Number = number;
            }

        }
        /// <summary>
        /// Reward of a type of Gift
        /// </summary>
        public class DailyGiftRewardItem : DailyRewardItem
        {
            private int _giftID;
            public DailyGiftRewardItem(Fbg.Bll.Player player, int numGifts, eDailyReward type, Gift gift)
                : base(player, type, numGifts, gift.Title, gift.AvailableImageUrl)
            {
                _giftID = gift.Id;
                Desc = string.Format("{0}x", Number);
            }
            protected override bool _UpdatePlayer()
            {
                return _player.RewardGifts(_giftID, Number);
            }
            public override string GetLongDesc()
            {                
                return string.Format("{0}x  {1}", Number, Title);
            }
        }
        /// <summary>
        /// Reward of Chests
        /// </summary>
        public class DailyChestRewardItem : DailyRewardItem
        {
            public DailyChestRewardItem(Fbg.Bll.Player player, int numChests)
                : base(player, eDailyReward.Chests, numChests, "Chests", "http://static.realmofempires.com/images/icons/M_PF_Silver.png")
            {
                Desc = string.Format("{0}", Number, Title);
            }
            protected override bool _UpdatePlayer()
            {
                return _player.RewardChests(Number); 
            }
            public override string GetLongDesc()
            {
                return Desc;
            }
        }
        /// <summary>
        /// reward of Servants
        /// </summary>
        public class DailyServantRewardItem : DailyRewardItem
        {
            public DailyServantRewardItem(Fbg.Bll.Player player, int numServants)
                : base(player, eDailyReward.Servants, numServants, "Servants", "http://static.realmofempires.com/images/icons/M_FreeServants.png")
            {
                Desc = string.Format("{0}", Number, Title);
            }
            protected override bool _UpdatePlayer()
            {
                return _player.RewardServants(Number);
            }
            public override string GetLongDesc()
            {
                return Desc;
            }
        }
        /// <summary>
        /// reward of 5 types of gifts and some servants: 3 Knight, 3 Infantry, 3 Ram, 3 Treb, 3 Spy, 3 Servants		
        /// </summary>
        public class DailyCornucopiaRewardItem : DailyRewardItem
        {
            protected int _knightID;
            protected string _knightTitle;
            protected int _infantryID;
            protected string _infantryTitle;
            protected int _ramID;
            protected string _ramTitle;
            protected int _trebID;
            protected string _trebTitle;
            protected int _spyID;
            protected string _spyTitle;
            public DailyCornucopiaRewardItem(Fbg.Bll.Player player, int number, Gift knightGift, Gift infantryGift, Gift ramGift, Gift trebGift, Gift spyGift)
                : base(player, eDailyReward.Cornucopia, number, "Cornucopia", "https://static.realmofempires.com/images/icons/M_PF_NP.png")
            {
                _knightID = knightGift.Id;
                _knightTitle = knightGift.Title;
                _infantryID = infantryGift.Id;
                _infantryTitle = infantryGift.Title;
                _ramID = ramGift.Id;
                _ramTitle = ramGift.Title;
                _trebID = trebGift.Id;
                _trebTitle = trebGift.Title;
                _spyID = spyGift.Id;
                _spyTitle = spyGift.Title;
                Desc = "";
            }
            protected override bool _UpdatePlayer()
            {
                bool updated = true;
                updated &= _player.RewardGifts(_knightID, Number);
                updated &= _player.RewardGifts(_infantryID, Number);
                updated &= _player.RewardGifts(_ramID, Number);
                updated &= _player.RewardGifts(_trebID, Number);
                updated &= _player.RewardGifts(_spyID, Number);
                updated &= _player.RewardServants(Number);
                return updated;
            }
            public override string GetLongDesc()
            {
                return string.Format("{0}: {1}x {2}s, {1}x {3}s, {1}x {4}s, {1}x {5}s, {1}x {6}s and {1} Servants", Title, Number, _knightTitle,
                    _infantryTitle, _ramTitle, _trebTitle, _spyTitle);
            }
        }
    }
}
