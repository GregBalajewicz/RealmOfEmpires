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

        public PlayerMorale Morale { get; private set; }

        public class PlayerMorale
        {
            public class Effect {
                public float  AttackMultiplier { get; internal set;}
                public float CarryMultiplier { get; internal set; }
                public float MoveSpeedMultiplier { get; internal set; }

            }


            public class CONSTS
            {
                internal class MoraleEffectsColIndexes {
                    public static  int Attack = 1;
                    public static int Carry = 3;
                    public static int MoveSpeed = 4;
                }

            }

            private DataTable dtMoraleEffectsRanges;
            private DataTable dtMoraleEffects;

            public bool IsActiveOnThisRealm { get; private set; }

            /// <summary>
            /// amount decreased when attacking regular players
            /// </summary>
            public int Decrease_normal { get; private set; }
            /// <summary>
            /// ammount decreased when attacking npc - rebels etc
            /// </summary>
            public int Decrease_npc { get; private set; }
            /// <summary>
            /// Max morale possible to have 
            /// </summary>
            public int MinMorale { get; private set; }
            /// <summary>
            /// min morale possible to have 
            /// </summary>
            public int MaxMorale { get; private set; }
            /// <summary>
            /// max "normal" morale - max morale before you get to the bonus area
            /// </summary>
            public int MaxMorale_Normal { get; private set; }
            /// <summary>
            /// min "normal" morale - min morale before you get to penalty area
            /// </summary>
            public int MinMorale_Normal { get; private set; }
            public double MoraleIncreasePerHour { get; private set; }

            public object jsSettings
            {
                get
                {
                    return new
                    {
                        EffectRangers =
                         from dr in dtMoraleEffectsRanges.AsEnumerable()
                         select
                         new
                         {
                             maxMorale = dr.Field<int>(0)
                             ,
                             minMorale = dr.Field<int>(1)
                                        ,
                             attack = dr.Field<float>(2)
                                        ,
                             desertion = dr.Field<float>(3)
                                        ,
                             carryCapacity = dr.Field<float>(4)
                                        ,
                             moveSpeed = dr.Field<float>(5)
                         }
                    
                                    
                        ,Effects =
                                    dtMoraleEffects.AsEnumerable().ToDictionary(
                                    r => r.Field<Int32>(0).ToString()
                                    , r => new
                                    {
                                        attack = r.Field<float>(1)
                                        , desertion = r.Field<float>(2)
                                        , carryCapacity = r.Field<float>(3)
                                        , moveSpeed = r.Field<float>(4)
                                    }
                                    ),
                        
                            isActiveOnThisRealm = IsActiveOnThisRealm,
                            decrease_normal = Decrease_normal,
                            decrease_npc = Decrease_npc,
                            minMorale = MinMorale,
                            maxMorale = MaxMorale,
                            maxMorale_Normal = MaxMorale_Normal,
                            minMorale_Normal = MinMorale_Normal,
                            increasePerHour = MoraleIncreasePerHour
                       
                    };

                }
            }
            public PlayerMorale(DataTable dtMoraleEffectsRanges, DataTable dtMoraleEffects, DataTable dtRealmAttributes)
            {
                this.dtMoraleEffects = dtMoraleEffects;
                this.dtMoraleEffectsRanges = dtMoraleEffectsRanges;
                
                IsActiveOnThisRealm = RealmAttributesGetHelper(false, dtRealmAttributes, Realm.CONSTS.RealmAttributeIDs.Morale_isActive);
                Decrease_normal = RealmAttributesGetHelper(0, dtRealmAttributes, Realm.CONSTS.RealmAttributeIDs.Morale_decrease_normal);
                Decrease_npc = RealmAttributesGetHelper(0, dtRealmAttributes, Realm.CONSTS.RealmAttributeIDs.Morale_decrease_npc);
                MinMorale = RealmAttributesGetHelper(0, dtRealmAttributes, Realm.CONSTS.RealmAttributeIDs.Morale_minMorale);
                MaxMorale = RealmAttributesGetHelper(0, dtRealmAttributes, Realm.CONSTS.RealmAttributeIDs.Morale_maxMorale);
                MaxMorale_Normal = RealmAttributesGetHelper(0, dtRealmAttributes, Realm.CONSTS.RealmAttributeIDs.Morale_maxMorale_Normal);
                MinMorale_Normal = RealmAttributesGetHelper(0, dtRealmAttributes, Realm.CONSTS.RealmAttributeIDs.Morale_minMorale_Normal);
                MoraleIncreasePerHour= RealmAttributesGetHelper(0.0f, dtRealmAttributes, Realm.CONSTS.RealmAttributeIDs.Morale_increasePerHour);
            }

            public Effect GetEffect(int morale)
            {
                Effect eff=null;
                DataRow[] drs = dtMoraleEffects.Select("Morale = " + morale);
                if (drs.Length == 1)
                {
                    eff = new Effect();
                    eff.AttackMultiplier = Convert.ToSingle (drs[0][CONSTS.MoraleEffectsColIndexes.Attack]);
                    eff.CarryMultiplier = Convert.ToSingle(drs[0][CONSTS.MoraleEffectsColIndexes.Carry]);
                    eff.MoveSpeedMultiplier = Convert.ToSingle(drs[0][CONSTS.MoraleEffectsColIndexes.MoveSpeed]);
                }
                return eff;
            }
        }

    }
}
