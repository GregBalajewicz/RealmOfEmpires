declare module BDA.Utils {
    function convertCountDownStrToFinishOnDate(s: string): Date;
}


module ROE {

    
    export class Village {
        id: number;
        name: string;
        points: number;
        villagetypeid: number;
        x: number;
        y: number;
        changes: VillageChanges;

        // BEGIN CODE PREVENTING MULTIPLE CALLS TO REFRESH EXTENDED PROPERTIES
        // the following properties are used to ensure that multiple calls to load/reload extended properties are not 
        //  made while one of those is in progress. 
        _loadingExtended_isLoading: boolean  // tracks if we are making a call now; reset when call returns. 
        _loadingExtended_lastCallTimeStamp: Date // tracks when last call was made; if much time passed and call not returned, we'll assume call failed and allow issuing new ones

        get loadingExtended_isLoading(): boolean {
            ///<summary>Tells if you a load latest call is issues and still pending return. if so, do not issue new load calls, just wait for this one to come back </summary>

            // NOTE: this can expanded to also track how often extended data is loaded and perhaps do not load it again if loaded X seconds ago
            //  HOWEVER, if this is done, it must be done by adding another function, because users of this function, depend on the call to 
            //  eventually return, and this call the necessary call backs, fire events etc. so we woudl need to add another function, something like
            //  "loadingExtended_isNewLoadRequiredByAgeOfLastLoad" or something like this
            var now: Date;
            var timeDiff: number;
            
            if (this._loadingExtended_isLoading) {
                // if loading, then check if the call is more than 10 seconds old, 
                //  if so, break the lock and say we are not loading (allow the call again) 
                now = new Date();
                var timeDiff = Math.abs(now.getTime() - this._loadingExtended_lastCallTimeStamp.getTime()); //get diff in MS
                if (timeDiff > 10000) {// if nore than 10 seconds passed, break the lock                
                    this._loadingExtended_isLoading = false;
                    BDA.Console.log("ROE.Village", "loadingExtended_isLoading() - have to break lock");
                }
            }
            return this._loadingExtended_isLoading;
        }
        public loadingExtended_signalLoadInitiated() {
            ///<summary>Signal that the call to [re]load extended properties has been issuesd; necessary to make loadingExtended_isLoading work right</summary>
            BDA.Console.log("ROE.Village", "_loadingExtended_isLoading +");
            this._loadingExtended_isLoading = true;
            this._loadingExtended_lastCallTimeStamp = new Date();
        }
        // END CODE PREVENTING MULTIPLE CALLS TO REFRESH EXTENDED PROPERTIES

        //EXTENDED PROPERTIES. all of these may be undefined if setTroops was not called, via ROE.VillgeList.ExtendedBasicInfo_loadLatest
        _TroopsList: Array<VillageTroops>;
        _TroopsDictionary: any;
        private _coins_raw: number;
        coinsTresMax: number;
        coinsPH: number;
        popCur: number;
        popMax: number;
        popRemaining: number;
        yey: number;
        timeStampWhenExtendedPropertiesLastRetrieved: Date
        areTranspAvail: boolean;
        VOV: VOVInfo;
        upgrade: any;
        recruit: any;
        Buildings: any;
        //END EXTENDED PROPERTIES

        get areExtendedPropertiesAvailable(): boolean {
            return this._TroopsDictionary !== undefined;
        }

        get TroopList(): Array<VillageTroops> {
            return this._TroopsList;
        }
        
        constructor(id: number, name: string
            ,points: number
            ,villagetypeid: number
            ,x: number
            ,y: number) {
            this.id = id; 
            this.name = name
            this.points = points;
            this.villagetypeid = villagetypeid;
            this.x = x; 
            this.y = y;
            this._loadingExtended_lastCallTimeStamp = new Date();
        } 

        public TroopByID(unitTypeID: number): VillageTroops {
            return this._TroopsDictionary['id' + unitTypeID];
        }
       
        public setExtendedData(data: ROE.VillageExtendedDataApiCallData) {
            ///<summary>returns troops in this village</summary>
            var newBuildingRecruitTimeFinishOn;
            var oldBuildingRecruitTimeFinishOn;
            var timeDifference: number;

            BDA.Console.verbose("ROE.Village", "_loadingExtended_isLoading -");
            this._loadingExtended_isLoading = false; // this signals that the API call to load extended data has completed
            //
            // compare what has changed and note it 
            //
            this.changes = new VillageChanges();
            this.changes.coins = Math.abs(data.Village.coins - this.coins) > ((this.coinsPH / 3600)*5);  // if the change in coins is greater than a 5 seconds growth, than we consider it changed. we do this, because there is always a small mismatch between front end and back end calculation
            this.changes.troops = true;  // TODO FINISH 
            this.changes.points = this.points != data.Village.points;
            this.changes.popRem = (this.popRemaining != data.Village.popMax - data.Village.popCur)
            this.changes.type = this.villagetypeid != data.Village.type;
            this.changes.name = this.name != data.Village.name;
            this.changes.yey = data.Village.yey != this.yey;
            this.changes.areTranspAvail = this.areTranspAvail != data.Village.areTranspAvail;

            // compare the VOV data changes
            if (this.VOV) {
                var newBuilding: VOVInfo_Building;
                var oldBuilding: VOVInfo_Building;
                for (var i = 0; i < data.VOV.buildings.length; i++) {
                    newBuilding = data.VOV.buildings[i];
                    oldBuilding = this.VOV.getBuildingByID(newBuilding.id);
                    if (!oldBuilding) {
                        this.changes.buildings = true;
                        break;
                    } else {
                        // check if something about the building has changed
                        if (oldBuilding.built != newBuilding.built
                            || oldBuilding.cnstr != newBuilding.cnstr
                            || oldBuilding.image != newBuilding.image
                            || oldBuilding.image_c != newBuilding.image_c
                            || oldBuilding.level != newBuilding.level
                            ) {
                            this.changes.buildings = true;
                        }

                        // check if the recruitment Q [ building work timer ] changed
                        //  why dont we check if "buildcount" changed? because buildcount for one building will always be included in HQ's recruitcount (since HQ's work is building) so we only need to check recruitcount
                        newBuildingRecruitTimeFinishOn = newBuilding.recruitcount ? BDA.Utils.convertCountDownStrToFinishOnDate(newBuilding.recruitcount) : new Date('jan 1 1970');
                        oldBuildingRecruitTimeFinishOn = oldBuilding.recruitFinishesOn ? oldBuilding.recruitFinishesOn : new Date('jan 1 1970');
                        timeDifference = (newBuildingRecruitTimeFinishOn.getTime() - oldBuildingRecruitTimeFinishOn.getTime());
                        if (Math.abs(timeDifference) > 1000) {
                            // we ignore chanaged smaller than 1 second, since those coudl be rounding issues, and 
                            //  we also know that time-when-finished is not 100% accurate and depends very much on when it was calculated. 
                            this.changes.buildingWork = true;
                        }

                        // if we found changes in one building, that's enough, no need to check every building
                        if (this.changes.buildingWork || this.changes.buildings) {
                            break; 
                        }
                    }
                }
            } else {
                // this.VOV undefined; ie, not loaded
                this.changes.buildings = true;
            }
            
            //
            //
            //
            this._TroopsList = [];
            this._TroopsDictionary = {};
            var troops: VillageTroops;
            for (var i = 0; i < data.Village.Units.length; i++) {

                troops = new VillageTroops(data.Village.Units[i].id
                    , data.Village.Units[i].YourUnitsCurrentlyInVillageCount
                    , data.Village.Units[i].CurrentlyRecruiting
                    , data.Village.Units[i].SupportCount
                    , data.Village.Units[i].YourUnitsTotalCount);

                this._TroopsList.push(troops);

                this._TroopsDictionary["id" + troops.id] =
                {
                    id : troops.id
                    , YourUnitsCurrentlyInVillageCount: troops.YourUnitsCurrentlyInVillageCount
                    , CurrentlyRecruiting: troops.CurrentlyRecruiting
                    , SupportCount: troops.SupportCount
                    , YourUnitsTotalCount: troops.YourUnitsTotalCount
                    , TotalNowInVillageCount: troops.TotalNowInVillageCount()
                };
            }
            this.points = data.Village.points;
            this._coins_raw = data.Village.coins;
            this.coinsTresMax = data.Village.coinsTresMax;
            this.coinsPH = data.Village.coinsPH;
            this.popCur = data.Village.popCur;
            this.popMax = data.Village.popMax;
            this.popRemaining = data.Village.popMax - data.Village.popCur;
            this.yey = data.Village.yey;
            this.name = data.Village.name;
            this.villagetypeid = data.Village.type;
            this.areTranspAvail = data.Village.areTranspAvail;
            this.timeStampWhenExtendedPropertiesLastRetrieved = new Date(data.timeStampWhenExtendedPropertiesLastRetrieved);

            this.VOV = new VOVInfo(data.VOV);
            this.upgrade = data.upgrade;
            this.recruit = data.recruit;
            this.Buildings = data.Buildings;


        } 


        get coins() {
            var coinsNow: number = this._coins_raw;
            if (coinsNow < this.coinsTresMax) {
                var now: Date = new Date();
                var ts: number = now.getTime() - this.timeStampWhenExtendedPropertiesLastRetrieved.getTime();
                if (ts >= 1000) {
                    var change: number;

                    change = Math.floor((this.coinsPH / (3600*1000)) * ts);

                    if (coinsNow + change <= this.coinsTresMax) {
                        coinsNow = coinsNow + change;
                    }
                    else {
                        coinsNow = this.coinsTresMax;
                    }
                }
            }
            else if (coinsNow > this.coinsTresMax) {
                coinsNow = this.coinsTresMax;
            }
            return coinsNow;
        }

        
    }   

    
    export class VillageExtendedDataApiCallData {        
        Village: VillageExtendedBasicInfoApiCallData;
        timeStampWhenExtendedPropertiesLastRetrieved: string
        VOV: any;
        upgrade: any;
        recruit: any;
        Buildings: any;
    }

    export class VillageTroops {
        id: number;
        YourUnitsCurrentlyInVillageCount : number;
        CurrentlyRecruiting : number;
        SupportCount: number;
        YourUnitsTotalCount: number;

        constructor(id :number, yourInVillage : number, recruiting : number, support : number, yourTotal:number  ) {
            this.id = id;
            this.YourUnitsCurrentlyInVillageCount = yourInVillage;
            this.CurrentlyRecruiting = recruiting;
            this.SupportCount = support;
            this.YourUnitsTotalCount = yourTotal;
        }

        TotalNowInVillageCount() : number {
            return this.SupportCount + this.YourUnitsCurrentlyInVillageCount;
        }
        
    }

    export class VillageExtendedBasicInfoApiCallData {
        coins: number;
        coinsPH: number;
        coinsTresMax: number;
        popCur: number;
        popMax: number;
        yey: number;
        points: number;
        name: string;
        type: number;
        Units: Array<ROE.VillageTroops>;
        areTranspAvail: boolean;
    }


    export class VillageChanges {
        troops: boolean;
        coins: boolean;
        popRem: boolean;
        points: boolean;
        name: boolean;
        type: boolean
        yey: boolean;
        areTranspAvail: boolean;
        buildings: boolean;
        buildingWork: boolean; // so this is the Q of the building's work. for HQ, this is total build Q time, for recruitment buildings, this is recruitment timer
    }


    export class VOVInfo {
        animations: any;
        bg: string;
        buildings: Array<VOVInfo_Building>
        trees: any;
        isday: number;

        constructor(data: VOVInfo) {
            this.animations = data.animations;
            this.bg = data.bg            
            this.trees = data.trees;
            this.isday = data.isday;

            this.buildings = new Array(data.buildings.length);
            for (var i = 0; i < data.buildings.length; i++) {

                this.buildings[i] = new VOVInfo_Building(                    
                    data.buildings[i].id,
                    data.buildings[i].level,
                    data.buildings[i].image,
                    data.buildings[i].images,
                    data.buildings[i].image_c,
                    data.buildings[i].built,
                    data.buildings[i].cnstr,
                    data.buildings[i].buildcount,
                    data.buildings[i].recruitcount,
                    data.buildings[i].areRequirementsSatisfied
                    );
            }
        } 

        public getBuildingByID(id): VOVInfo_Building {

            for (var i = 0; i < this.buildings.length; i++) {
                if (this.buildings[i].id == id) {
                    return this.buildings[i];
                }
            }
            return undefined;
        }
    }

    export class VOVInfo_Building {
        id: number;
        level: number;
        image: string;
        images: Array < string>;
        image_c: string;
        built: boolean;
        cnstr: boolean;
        buildcount: string;
        recruitcount: string;
        areRequirementsSatisfied: boolean;
        recruitFinishesOn: Date; // maybe undefined if no recruit timer

        constructor(id: number,
        level: number,
        image: string,
        images: Array < string>,
        image_c: string,
        built: boolean,
        cnstr: boolean,
        buildcount: string,
        recruitcount: string,
        areRequirementsSatisfied: boolean) {
            this.id = id;
            this.level = level;
            this.image = image;
            this.image_c = image_c;
            this.images = images;
            this.built = built;
            this.cnstr = cnstr;
            this.buildcount = buildcount;
            this.recruitcount = recruitcount;
            this.areRequirementsSatisfied = areRequirementsSatisfied;
            if (this.recruitcount) {
                this.recruitFinishesOn = BDA.Utils.convertCountDownStrToFinishOnDate(this.recruitcount);
            }

        } 




    }




}



