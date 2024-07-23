using System;
using System.Collections.Generic;
using System.Text;

namespace Fbg.Bll
{
    /// <summary>
    /// Gift that gives you a silver production bonus based on your current hourly production
    /// </summary>
    public class Gift_HourlySilverProd :Gift
    {
        public Gift_HourlySilverProd(int id
            , string title
            , string availableImageUrl
            , string notAvailableImageUrl
            , Title requiredTitle
            , string fbRequestActionText
            , string fbRequestContentText
            , double productionRewardMultiplier): base(id, title, availableImageUrl, notAvailableImageUrl, requiredTitle, fbRequestActionText, fbRequestContentText)
        {
            _productionRewardMultiplier = productionRewardMultiplier;
        }

        double _productionRewardMultiplier;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="registeredOn">when player registered</param>
        /// <returns></returns>        
        public double ProductionRewardMultiplier(Realm realm) 
        {           
            return _productionRewardMultiplier * GetCorrectMultiplierBasedOnRegisterDate(realm.OpenOn);            
        }

        public double ProductionRewardMultiplierBase()
        {
            return _productionRewardMultiplier;
        }
    }

    /// <summary>
    /// gifts that give you X troops bonus
    /// </summary>
    public class Gift_Troops : Gift
    {
        public Gift_Troops(int id
            , string title
            , string availableImageUrl
            , string notAvailableImageUrl
            , Title requiredTitle
            , string fbRequestActionText
            , string fbRequestContentText
            , int numOfTroops
            , Fbg.Bll.UnitType unitType)
            : base(id, title, availableImageUrl, notAvailableImageUrl, requiredTitle, fbRequestActionText, fbRequestContentText)
        {
            _numOfTroops = numOfTroops;
            _unitType = unitType;
        }

        int _numOfTroops;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="registeredOn">when player registered</param>
        /// <returns></returns>
        public int numOfTroops(Realm realm) 
        {
            return _numOfTroops * Convert.ToInt32(GetCorrectMultiplierBasedOnRegisterDate(realm.OpenOn)); 
        }
        public int numOfTroopsBase()
        {
            return _numOfTroops;
        }
        UnitType _unitType;
        public UnitType unitType
        {
            get { return _unitType; }
        }

    }

    public abstract class Gift
    {
        public int CostInCredits { get { return 1; } }

        string _FBRequestActionText;
        public string FBRequestActionText
        {
            get { return _FBRequestActionText; }
        }
        string _FBRequestContentText;
        public string FBRequestContentText
        {
            get { return _FBRequestContentText; }
        }

        int _id;
        public int Id
        {
            get { return _id; }
        }

        String _title;
        public String Title
        {
            get { return _title; }
        }
 
        string _availableImageUrl;
        public string AvailableImageUrl
        {
            get { return _availableImageUrl; }
        }

        string _notAvailableImageUrl;
        public string NotAvailableImageUrl
        {
            get { return _notAvailableImageUrl; }
        }
        String _notAvailableMessageM;
        String _notAvailableMessageF;

        public String NotAvailableMessage(Fbg.Common.Sex sex)
        {
            if (sex == Fbg.Common.Sex.Female)
            {
                return _notAvailableMessageF; 
            }
            return _notAvailableMessageM;            
        }
        Title _requiredTitle=null;

        public Title RequiredTitle
        {
            get { return _requiredTitle; }
        }
        public List<MultiplierByTimeInRealm> MultipliersByTimeInRealm { get; set; }

        public struct MultiplierByTimeInRealm {
            public int DaysOnRealm { get; internal set; }
            public double Multiplier { get; internal set; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registeredOn">when player registered</param>
        /// <returns></returns>
        public double GetCorrectMultiplierBasedOnRegisterDate(DateTime registeredOn)
        {
            double playingForXDays = DateTime.Now.Subtract(registeredOn).TotalDays;
            double multiplier = 1;
            if (MultipliersByTimeInRealm != null)
            {
                foreach (MultiplierByTimeInRealm i in MultipliersByTimeInRealm)
                {
                    if (playingForXDays < i.DaysOnRealm)
                    {                     
                        multiplier = i.Multiplier;
                        break;
                    }

                }
            }
            return multiplier;
        }



        public Gift(int id
            , string title
            , string availableImageUrl
            , string notAvailableImageUrl
            , Title requiredTitle
            , string fbRequestActionText
            , string fbRequestContentText)
        {
            _id = id;
            _title = title;
            _availableImageUrl = availableImageUrl;
            _notAvailableImageUrl = notAvailableImageUrl;
            _requiredTitle = requiredTitle;
            _FBRequestActionText = fbRequestActionText;
            _FBRequestContentText = fbRequestContentText;

            if (_requiredTitle != null)
            {
                _notAvailableMessageM = "Unlock at " + _requiredTitle.TitleName_Male + " Title";
                _notAvailableMessageF = "Unlock at " + _requiredTitle.TitleName_Female + " Title";
            }
        }

        public bool CheckAvailability(Player player)
        {
            if (_requiredTitle != null)
            {
                if (player.Title < _requiredTitle)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
