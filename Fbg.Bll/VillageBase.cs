using System;
using System.Collections.Generic;
using System.Text;

using Gmbc.Common.Diagnostics.ExceptionManagement;


namespace Fbg.Bll
{
    public class VillageBase : ISerializableToNameValueCollection
    {
        protected Player _owner;
        public Player owner
        {
            get
            {
                return _owner;
            }
        }

        protected int _id;
        public int id
        {
            get
            {
                return _id;
            }
        }

        public VillageBase(Player owner
            , int id) 
        {
            this._owner = owner;
            this._id = id;
        }


        /// <summary>
        /// Adds the 'amount' of units of unit type 'unit' to this village.
        /// this does NOT check for space in the farm therefore you may end up with a negative farm space
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="amount"></param>
        virtual public void AddUnitsToVillage(UnitType unit, int amount)
        {
            DAL.Villages.AddUnits(owner.Realm.ConnectionStr, id, unit.ID, amount);
        }


        #region old code used for caching
        //
        // this code, along with Player.Village(id, list<villagebase>)
        //  was used to cache the village we retrieve. basically, we always had a list of VillageBase objects
        //  and when we 'upgrade' one to village or villgebasicb object, we would replace the VIllageBase object with this one
        //  in the list so that next time we could just get it from the list rathe than calling DB.
        //
        //  However, this is not very useful now so the code was remove in favour of simpler code. It is left here for future refrence
        //
        //public Village GetVillage()
        //{
        //    if (this is Village)
        //    {
        //        return (Village)this;
        //    }
        //    else
        //    {
        //        return new Village(owner, id);
        //    }
        //}
        //public VillageBasicB GetVillageBasicB()
        //{
        //    if (this is VillageBasicB)
        //    {
        //        return (VillageBasicB)this;
        //    }
        //    else
        //    {
        //        return new VillageBasicB(owner, id);
        //    }
        //}
        #endregion 



        #region ISerializableToNameValueCollection Members

        public void SerializeToNameValueCollection(System.Collections.Specialized.NameValueCollection col)
        {
            try
            {
                string pre = "Village[" + id.ToString() + "]";

                if (col == null)
                {
                    ExceptionManager.Publish("Error in VillageBase.SerializeToNameValueCollection. argument 'col' is null");
                }
                else
                {
                    BaseApplicationException.AddAdditionalInformation(col, pre + "owner", owner);
                }
            }
            catch (Exception e)
            {
                ExceptionManager.Publish("Error in VillageBase.SerializeToNameValueCollection. ExceptionManager.SerializeToString(e):" + ExceptionManager.SerializeToString(e));
            }
        }

        #endregion
    }
}
