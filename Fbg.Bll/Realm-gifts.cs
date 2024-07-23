using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;

using Gmbc.Common.Diagnostics.ExceptionManagement;
using System.Globalization;

namespace Fbg.Bll
{
    public partial class Realm : ISerializableToNameValueCollection2
    {
        public List<Gift> Gifts
        {
            get
            {
                if (_gifts == null)
                {
                    _gifts = new List<Gift>(9);

                    if (this.RealmType == "X") 
                    {
                        #region FOR RXS - actually, for RSs since we dont expect this to effect short RXs  
                        // the gift progression (multiplier) was adjusted to fit the multiple consolidation realms a bit better
                        _gifts.Add(new Gift_HourlySilverProd(1, "A Sack of Silver", "https://static.realmofempires.com/images/gifts/Gift_sack_of_silver.png", "", null, "Send your friends a FREE gift of a sack of silver!", "Hey! Here is a gift of a SACK OF PURE SILVER!", 0.5)
                        {
                            MultipliersByTimeInRealm = new List<Gift.MultiplierByTimeInRealm> 
                        { 
                            new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 3, Multiplier=1.00}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 4, Multiplier=1.25}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 5, Multiplier=1.50}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 6, Multiplier=2.00}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 8, Multiplier=3.00}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 10, Multiplier=4.00}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 12, Multiplier=6.00}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 14, Multiplier=9.00}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 16, Multiplier=14.00}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 18, Multiplier=20.00}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 9999, Multiplier=25.00}  // silver gift is done slighly differently than all other gifts because silver gets obsolete faster so we want to make it a bigger bang for a buck
                        }
                        });

                        _gifts.Add(new Gift_Troops(4, "Infantry", "https://static.realmofempires.com/images/gifts/Gift_infantry.png", "https://static.realmofempires.com/images/gifts/Gift_infantry_locked.png", this.TitleByLevel(Fbg.Bll.CONSTS.TitleLevels.Merchant), "Send your friends a FREE gift of Mighty Infantry!", "Hey! Here is a gift of a MIGHTLY MEDIEVAL WARRIOR!", 1, this.GetUnitTypesByID(Fbg.Bll.CONSTS.UnitIDs.Infantry))
                        {
                            MultipliersByTimeInRealm = new List<Gift.MultiplierByTimeInRealm> 
                        { 
                            new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 3, Multiplier=1}
                           , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 4, Multiplier=5}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 5, Multiplier=10}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 6, Multiplier=20}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 8, Multiplier=30}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 10, Multiplier=45}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 12, Multiplier=60}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 14, Multiplier=75}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 9999, Multiplier=100}
                            
                        }
                        });
                        _gifts.Add(new Gift_Troops(5, "Light Cavalry", "https://static.realmofempires.com/images/gifts/Gift_cavalry.png", "https://static.realmofempires.com/images/gifts/Gift_cavalry_locked.png", this.TitleByLevel(Fbg.Bll.CONSTS.TitleLevels.Knight), "Send your friends a FREE gift of Light Cavalry!", "Hey! Here is a gift of a LIGHT CAVALRY!", 1, this.GetUnitTypesByID(Fbg.Bll.CONSTS.UnitIDs.LC))
                        {
                            MultipliersByTimeInRealm = new List<Gift.MultiplierByTimeInRealm> 
                        { 
                              new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 5, Multiplier=1}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 6, Multiplier=2}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 7, Multiplier=4}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 8, Multiplier=6}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 10, Multiplier=8}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 12, Multiplier=10}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 14, Multiplier=12}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 16, Multiplier=16}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 9999, Multiplier=20}
                        }
                        });
                        _gifts.Add(new Gift_Troops(6, "Knight", "https://static.realmofempires.com/images/gifts/Gift_knight.png", "https://static.realmofempires.com/images/gifts/Gift_knight_locked.png", this.TitleByLevel(Fbg.Bll.CONSTS.TitleLevels.Baron), "Send your friends a FREE gift of an Armor Clad Knight!", "Hey! Here is a gift of an ARMOR CLAD KNIGHT!", 1, this.GetUnitTypesByID(Fbg.Bll.CONSTS.UnitIDs.Knight))
                        {
                            MultipliersByTimeInRealm = new List<Gift.MultiplierByTimeInRealm>
                        {
                              new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 7, Multiplier=1}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 9, Multiplier=2}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 11, Multiplier=3}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 13, Multiplier=4}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 15, Multiplier=5}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 17, Multiplier=6}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 19, Multiplier=7}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 9999, Multiplier=8}
                        }
                        });
                        _gifts.Add(new Gift_Troops(7, "Spy", "https://static.realmofempires.com/images/gifts/Gift_spy.png", "https://static.realmofempires.com/images/gifts/Gift_spy_locked.png", this.TitleByLevel(Fbg.Bll.CONSTS.TitleLevels.KnightS), "Send your friends a FREE gift of a Spy", "Hey! Here is a gift of a SPY, a 007 of the medieval era!", 1, this.GetUnitTypesByID(Fbg.Bll.CONSTS.UnitIDs.Spy))
                        {
                            MultipliersByTimeInRealm = new List<Gift.MultiplierByTimeInRealm>
                        {
                              new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 5, Multiplier=1}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 6, Multiplier=2}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 7, Multiplier=4}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 8, Multiplier=6}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 10, Multiplier=8}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 12, Multiplier=10}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 14, Multiplier=14}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 16, Multiplier=18}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =9999, Multiplier=22}
                        }
                        });

                        _gifts.Add(new Gift_Troops(8, "Ram", "https://static.realmofempires.com/images/gifts/Gift_ram.png", "https://static.realmofempires.com/images/gifts/Gift_ram_locked.png", this.TitleByLevel(Fbg.Bll.CONSTS.TitleLevels.Lord), "Send your friends a FREE gift of Battering Ram", "Hey! Here is a gift of a gate crushing MIGHTY BATTERING RAM!", 1, this.GetUnitTypesByID(Fbg.Bll.CONSTS.UnitIDs.Ram))
                        {
                            MultipliersByTimeInRealm = new List<Gift.MultiplierByTimeInRealm>
                        {
                              new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 7, Multiplier=1}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 9, Multiplier=2}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 11, Multiplier=3}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 13, Multiplier=4}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 15, Multiplier=6}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 17, Multiplier=8}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 9999, Multiplier=10}
                        }
                        });

                        _gifts.Add(new Gift_Troops(9, "Trebuchet", "https://static.realmofempires.com/images/gifts/Gift_trebuchet.png", "https://static.realmofempires.com/images/gifts/Gift_trebuchet_locked.png", this.TitleByLevel(Fbg.Bll.CONSTS.TitleLevels.Viscount), "Send your friends a FREE gift of a Mightly Trebuchet!", "Hey! Here is a gift of wall crushing MIGHTY TREBUCHET!", 1, this.GetUnitTypesByID(Fbg.Bll.CONSTS.UnitIDs.Treb))
                        {
                            MultipliersByTimeInRealm = new List<Gift.MultiplierByTimeInRealm>
                        {     
                              new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 7, Multiplier=1}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 9, Multiplier=2}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 11, Multiplier=3}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 13, Multiplier=4}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 15, Multiplier=6}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 17, Multiplier=8}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm = 9999, Multiplier=10}
                        }
                        });
                        #endregion
                    } else
                    {
                        #region new regular realms, see case 27526
                        // the gift progression (multiplier) was adjusted to fit the multiple consolidation realms a bit better
                        _gifts.Add(new Gift_HourlySilverProd(1, "A Sack of Silver", "https://static.realmofempires.com/images/gifts/Gift_sack_of_silver.png", "", null, "Send your friends a FREE gift of a sack of silver!", "Hey! Here is a gift of a SACK OF PURE SILVER!", 0.5)
                        {
                            MultipliersByTimeInRealm = new List<Gift.MultiplierByTimeInRealm> 
                        { 
                            new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =10, Multiplier=1.00}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =20, Multiplier=1.25}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =30, Multiplier=1.50}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =40, Multiplier=2.00}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =50, Multiplier=3.00}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =60, Multiplier=4.00}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =70, Multiplier=6.00}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =80, Multiplier=9.00}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =90, Multiplier=14.00}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =120, Multiplier=20.00}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =9999, Multiplier=25.00}  // silver gift is done slighly differently than all other gifts because silver gets obsolete faster so we want to make it a bigger bang for a buck
                        }
                        });

                        _gifts.Add(new Gift_Troops(4, "Infantry", "https://static.realmofempires.com/images/gifts/Gift_infantry.png", "https://static.realmofempires.com/images/gifts/Gift_infantry_locked.png", this.TitleByLevel(Fbg.Bll.CONSTS.TitleLevels.Merchant), "Send your friends a FREE gift of Mighty Infantry!", "Hey! Here is a gift of a MIGHTLY MEDIEVAL WARRIOR!", 1, this.GetUnitTypesByID(Fbg.Bll.CONSTS.UnitIDs.Infantry))
                        {
                            MultipliersByTimeInRealm = new List<Gift.MultiplierByTimeInRealm> 
                        { 
                            new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =5, Multiplier=1}
                           , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =15, Multiplier=5}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =30, Multiplier=10}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =40, Multiplier=20}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =50, Multiplier=30}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =60, Multiplier=45}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =70, Multiplier=60}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =80, Multiplier=75}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =90, Multiplier=100}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =999, Multiplier=100}
                            
                        }
                        });
                        _gifts.Add(new Gift_Troops(5, "Light Cavalry", "https://static.realmofempires.com/images/gifts/Gift_cavalry.png", "https://static.realmofempires.com/images/gifts/Gift_cavalry_locked.png", this.TitleByLevel(Fbg.Bll.CONSTS.TitleLevels.Knight), "Send your friends a FREE gift of Light Cavalry!", "Hey! Here is a gift of a LIGHT CAVALRY!", 1, this.GetUnitTypesByID(Fbg.Bll.CONSTS.UnitIDs.LC))
                        {
                            MultipliersByTimeInRealm = new List<Gift.MultiplierByTimeInRealm> 
                        { 
                              new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =10, Multiplier=1}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =20, Multiplier=2}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =30, Multiplier=4}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =40, Multiplier=6}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =50, Multiplier=8}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =60, Multiplier=10}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =70, Multiplier=12}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =80, Multiplier=16}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =90, Multiplier=20}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =9999, Multiplier=20}
                        }
                        });
                        _gifts.Add(new Gift_Troops(6, "Knight", "https://static.realmofempires.com/images/gifts/Gift_knight.png", "https://static.realmofempires.com/images/gifts/Gift_knight_locked.png", this.TitleByLevel(Fbg.Bll.CONSTS.TitleLevels.Baron), "Send your friends a FREE gift of an Armor Clad Knight!", "Hey! Here is a gift of an ARMOR CLAD KNIGHT!", 1, this.GetUnitTypesByID(Fbg.Bll.CONSTS.UnitIDs.Knight))
                        {
                            MultipliersByTimeInRealm = new List<Gift.MultiplierByTimeInRealm>
                        {
                              new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =20, Multiplier=1}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =30, Multiplier=2}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =40, Multiplier=3}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =50, Multiplier=4}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =60, Multiplier=5}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =70, Multiplier=6}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =80, Multiplier=7}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =90, Multiplier=8}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =9999, Multiplier=8}
                        }
                        });
                        _gifts.Add(new Gift_Troops(7, "Spy", "https://static.realmofempires.com/images/gifts/Gift_spy.png", "https://static.realmofempires.com/images/gifts/Gift_spy_locked.png", this.TitleByLevel(Fbg.Bll.CONSTS.TitleLevels.KnightS), "Send your friends a FREE gift of a Spy", "Hey! Here is a gift of a SPY, a 007 of the medieval era!", 1, this.GetUnitTypesByID(Fbg.Bll.CONSTS.UnitIDs.Spy))
                        {
                            MultipliersByTimeInRealm = new List<Gift.MultiplierByTimeInRealm>
                        {
                              new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =10, Multiplier=1}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =20, Multiplier=2}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =30, Multiplier=3}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =40, Multiplier=4}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =50, Multiplier=5}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =60, Multiplier=6}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =70, Multiplier=7}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =80, Multiplier=8}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =90, Multiplier=10}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =9999, Multiplier=10}
                        }
                        });

                        _gifts.Add(new Gift_Troops(8, "Ram", "https://static.realmofempires.com/images/gifts/Gift_ram.png", "https://static.realmofempires.com/images/gifts/Gift_ram_locked.png", this.TitleByLevel(Fbg.Bll.CONSTS.TitleLevels.Lord), "Send your friends a FREE gift of Battering Ram", "Hey! Here is a gift of a gate crushing MIGHTY BATTERING RAM!", 1, this.GetUnitTypesByID(Fbg.Bll.CONSTS.UnitIDs.Ram))
                        {
                            MultipliersByTimeInRealm = new List<Gift.MultiplierByTimeInRealm>
                        {
                              new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =20, Multiplier=1}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =30, Multiplier=1}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =40, Multiplier=2}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =50, Multiplier=2}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =60, Multiplier=3}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =70, Multiplier=4}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =90, Multiplier=5}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =9999, Multiplier=5}
                        }
                        });

                        _gifts.Add(new Gift_Troops(9, "Trebuchet", "https://static.realmofempires.com/images/gifts/Gift_trebuchet.png", "https://static.realmofempires.com/images/gifts/Gift_trebuchet_locked.png", this.TitleByLevel(Fbg.Bll.CONSTS.TitleLevels.Viscount), "Send your friends a FREE gift of a Mightly Trebuchet!", "Hey! Here is a gift of wall crushing MIGHTY TREBUCHET!", 1, this.GetUnitTypesByID(Fbg.Bll.CONSTS.UnitIDs.Treb))
                        {
                            MultipliersByTimeInRealm = new List<Gift.MultiplierByTimeInRealm>
                        {     new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =20, Multiplier=1}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =30, Multiplier=1}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =40, Multiplier=2}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =50, Multiplier=2}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =60, Multiplier=3}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =70, Multiplier=4}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =90, Multiplier=5}
                            , new Gift.MultiplierByTimeInRealm(){ DaysOnRealm =9999, Multiplier=5}
                        }
                        });
                        #endregion
                    }
                }
                return _gifts;
            }
        }
    }
}
