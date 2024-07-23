using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Gmbc.Common.Diagnostics.ExceptionManagement;


namespace Fbg.Bll
{
    public class Research
    {
        DataTable _dtResearchItemTypes;
        DataSet _dsRef;
        Realm _realmRef;
        List<ResearchItemType> _researchItemTypes;
        List<ResearchItem> _allResearchItems;
        Dictionary<ResearchItem, List<ResearchItem>> _dependentResearchItems;

        public Research(Realm realmRef, DataSet ds, DataTable dtResearchItemTypes) 
        {
            this._dtResearchItemTypes = dtResearchItemTypes;
            _dsRef = ds;
            _realmRef = realmRef;
        }
        public List<ResearchItem> ResearchItemsForBuildingType(BuildingType bt)
        {            
            return _allResearchItems.FindAll(delegate(ResearchItem ri) { return ri.Property_BuldingItEffects == bt; });
        }
        public List<ResearchItem> ResearchItemsByResearchItemPropertyID(int id)
        {
            return _allResearchItems.FindAll(delegate(ResearchItem ri) { return ri.ResearchPropertyTypeID == id; });
        }
        public ResearchItem ResearchItemByID(int ritid, int riid)
        {
            try
            {
                return ResearchItemTypeByID(ritid).ResearchItems.Find(delegate(ResearchItem ri) { return ri.ID == riid; });
            }
            catch (Exception e)
            {
                throw new Exception("Error trying to get riid:" + riid.ToString(), e);
            }
        }
        public ResearchItemType ResearchItemTypeByID(int ritid)
        {
            return ResearchItemTypes.Find(delegate(ResearchItemType rit) { return rit.ID == ritid; });
        }
        public List<ResearchItemType> ResearchItemTypes
        {
            get
            {
                
                
                if (_researchItemTypes == null)
                {
                    _researchItemTypes = new List<ResearchItemType>(_dtResearchItemTypes.Rows.Count);
                    foreach (DataRow dr in _dtResearchItemTypes.Rows)
                    {
                        _researchItemTypes.Add(new ResearchItemType(_realmRef, _dsRef, dr));
                    }
                }
                return _researchItemTypes;
            }
        }

        /// <summary>
        /// tells you if research is active on this realms
        /// </summary>
        public bool IsResearchActive
        {
            get
            {
                return AllResearchItems.Count > 0;
            }
        }
        public List<ResearchItem> AllResearchItems
        {
            get
            {                
                if (_allResearchItems == null)
                {
                    _allResearchItems = new List<ResearchItem>();

                    foreach (ResearchItemType rit in ResearchItemTypes)
                    {
                        _allResearchItems.AddRange(rit.ResearchItems);
                    }
                }
                return _allResearchItems;
            }
        }
         
        public List<ResearchItem> DependentResearchItems(ResearchItem ri)
        {
            if (_dependentResearchItems == null)
            {
                List<ResearchItem> depList;
                //ResearchItem i2;
                _dependentResearchItems = new Dictionary<ResearchItem, List<ResearchItem>>(AllResearchItems.Count);
                foreach (ResearchItem ri2 in AllResearchItems)
                {
                    depList = new List<ResearchItem>();
                    foreach (ResearchItem riDep in AllResearchItems)
                    {
                        if (riDep.Requirements.Exists(delegate(ResearchItem i) { return i == ri2; }))
                        {
                            depList.Add(riDep);
                        }
                    }
                    _dependentResearchItems.Add(ri2, depList);
                }

            }
            return _dependentResearchItems[ri];
        }

        /// <summary>
        /// should be called in a thread safe context to init itself
        /// </summary>
        public void Init() {
            //
            // THIS is running in thread SAFE context so init everything to ensure thread safety
            //
            // beware of changing the order of these or you get stack overflow!!
            try
            {
                List<ResearchItemType> researchItemTypes = this.ResearchItemTypes;
                foreach (ResearchItemType ri in researchItemTypes)
                {
                    ri.Initialize();
                }
                Object o = ResearchItemTypes;
                o = AllResearchItems;

                if (AllResearchItems.Count > 0) { o = DependentResearchItems(AllResearchItems[0]); }

                o = this.UniqueBuildingsEffectedByResearch;
                o = this.UniqueUnitsUnlockedByResearch;
                //
                // END THREAD SAFE INIT
                //
            }
            catch (Exception e)
            {
                BaseApplicationException bex = new BaseApplicationException("error in research init()", e);
                bex.AddAdditionalInformation("this._realmRef.ID", this._realmRef.ID);
                throw bex;
            }
        }



        Dictionary<BuildingType, decimal> _bonuses = new Dictionary<BuildingType, decimal>();
        Dictionary<int, decimal> _bonuses_miscResearchGroups = new Dictionary<int, decimal>();
        /// <summary>
        /// Max bonus for building available from research. 0.1 means 10% bonus
        /// </summary>
        public float MaxBonus(BuildingType bt)
        {            
            decimal bonus;

            if (!_bonuses.TryGetValue(bt, out bonus))
            {
                bonus = 0;
                List<ResearchItem> ris = ResearchItemsForBuildingType(bt);

                foreach (ResearchItem ri in ris)
                {
                    bonus += Convert.ToDecimal(ri.PropertyAsFloat);
                }

                _bonuses.Add(bt, bonus);
            }

            return Convert.ToSingle(bonus);
        }
        /// <summary>
        /// Max bonus for building available from research. 0.1 means 10% bonus
        /// </summary>
        public float MaxBonus(int miscResearchGroupID)
        {
            decimal bonus;

            if (!_bonuses_miscResearchGroups.TryGetValue(miscResearchGroupID, out bonus))
            {
                bonus = 0;
                List<ResearchItem> ris = ResearchItemsByResearchItemPropertyID(miscResearchGroupID);

                foreach (ResearchItem ri in ris)
                {
                    bonus += Convert.ToDecimal(ri.PropertyAsFloat);
                }

                _bonuses_miscResearchGroups.Add(miscResearchGroupID, bonus);
            }

            return Convert.ToSingle(bonus);
        }




        /// <summary>
        /// Max wall & towers defence bonus available from research. 0.1 means 10% bonus
        /// </summary>
        public float MaxBonus_DefenceFactor()
        {
            return MaxBonus(_realmRef.BuildingType(Fbg.Bll.CONSTS.BuildingIDs.DefenseTower)) + MaxBonus(_realmRef.BuildingType(Fbg.Bll.CONSTS.BuildingIDs.Wall));
        }

        /// <summary>
        /// Max village defence bonus available from research. 0.1 means 10% bonus
        /// </summary>
        public float MaxBonus_VillageDefenceFactor()
        {
            return MaxBonus(12);
        }


        /// <summary>
        /// Max village defence bonus available from research. 0.1 means 10% bonus
        /// </summary>
        public float MaxBonus_AttackBonus()
        {
            return MaxBonus(13);
        }


        List<int> miscResearchGroups;
        public List<int> MiscResearchGroups
        {
            get
            {
                if (miscResearchGroups == null)
                {
                    miscResearchGroups = new List<int>();
                    foreach (ResearchItem ri in AllResearchItems)
                    {
                        if (ri.TypesOfResearchItem == ResearchItem.TypesOfResearchItems.MiscEffect)
                        {
                            if (!miscResearchGroups.Exists(delegate(int id) { return id == ri.ResearchPropertyTypeID; }))
                            {
                                miscResearchGroups.Add(ri.ResearchPropertyTypeID);
                            }
                        }                      
                    }
                }
                return miscResearchGroups;
            }
        }

        List<BuildingType> bts;
        public List<BuildingType> UniqueBuildingsEffectedByResearch
        {
            get
            {
                if (bts == null)
                {
                    bts = new List<BuildingType>();
                    foreach (ResearchItem ri in AllResearchItems)
                    {
                        if (ri.Property_BuldingItEffects != null)
                        {
                            if (!bts.Exists(delegate(BuildingType bt) { return bt == ri.Property_BuldingItEffects; }))
                            {
                                bts.Add(ri.Property_BuldingItEffects);
                            }
                        }
                    }
                }
                return bts;
            }
        }


        List<Fbg.Bll.UnitType> uts;
        public List<Fbg.Bll.UnitType> UniqueUnitsUnlockedByResearch
        {
            get
            {
                if (uts == null)
                {
                    uts = new List<Fbg.Bll.UnitType>();
                    foreach (ResearchItem ri in AllResearchItems)
                    {                       
                        if (ri.UnlocksUnitTye != null)
                        {
                            if (!uts.Exists(delegate(Fbg.Bll.UnitType ut) { return ut == ri.UnlocksUnitTye; }))
                            {
                                uts.Add(ri.UnlocksUnitTye);
                            }
                        }
                    }
                }
                return uts;
            }
        }
    }
}
