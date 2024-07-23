var ROE;
(function (ROE) {
    var Village = (function () {
        function Village(id, name, points, villagetypeid, x, y) {
            this.id = id;
            this.name = name;
            this.points = points;
            this.villagetypeid = villagetypeid;
            this.x = x;
            this.y = y;
            this._loadingExtended_lastCallTimeStamp = new Date();
        }
        Object.defineProperty(Village.prototype, "loadingExtended_isLoading", {
            get: function () {
                ///<summary>Tells if you a load latest call is issues and still pending return. if so, do not issue new load calls, just wait for this one to come back </summary>
                // NOTE: this can expanded to also track how often extended data is loaded and perhaps do not load it again if loaded X seconds ago
                //  HOWEVER, if this is done, it must be done by adding another function, because users of this function, depend on the call to
                //  eventually return, and this call the necessary call backs, fire events etc. so we woudl need to add another function, something like
                //  "loadingExtended_isNewLoadRequiredByAgeOfLastLoad" or something like this
                var now;
                var timeDiff;

                if (this._loadingExtended_isLoading) {
                    // if loading, then check if the call is more than 10 seconds old,
                    //  if so, break the lock and say we are not loading (allow the call again)
                    now = new Date();
                    var timeDiff = Math.abs(now.getTime() - this._loadingExtended_lastCallTimeStamp.getTime());
                    if (timeDiff > 10000) {
                        this._loadingExtended_isLoading = false;
                        BDA.Console.log("ROE.Village", "loadingExtended_isLoading() - have to break lock");
                    }
                }
                return this._loadingExtended_isLoading;
            },
            enumerable: true,
            configurable: true
        });
        Village.prototype.loadingExtended_signalLoadInitiated = function () {
            ///<summary>Signal that the call to [re]load extended properties has been issuesd; necessary to make loadingExtended_isLoading work right</summary>
            BDA.Console.log("ROE.Village", "_loadingExtended_isLoading +");
            this._loadingExtended_isLoading = true;
            this._loadingExtended_lastCallTimeStamp = new Date();
        };

        Object.defineProperty(Village.prototype, "areExtendedPropertiesAvailable", {
            //END EXTENDED PROPERTIES
            get: function () {
                return this._TroopsDictionary !== undefined;
            },
            enumerable: true,
            configurable: true
        });

        Object.defineProperty(Village.prototype, "TroopList", {
            get: function () {
                return this._TroopsList;
            },
            enumerable: true,
            configurable: true
        });

        Village.prototype.TroopByID = function (unitTypeID) {
            return this._TroopsDictionary['id' + unitTypeID];
        };

        Village.prototype.setExtendedData = function (data) {
            ///<summary>returns troops in this village</summary>
            var newBuildingRecruitTimeFinishOn;
            var oldBuildingRecruitTimeFinishOn;
            var timeDifference;

            BDA.Console.verbose("ROE.Village", "_loadingExtended_isLoading -");
            this._loadingExtended_isLoading = false; // this signals that the API call to load extended data has completed

            //
            // compare what has changed and note it
            //
            this.changes = new VillageChanges();
            this.changes.coins = Math.abs(data.Village.coins - this.coins) > ((this.coinsPH / 3600) * 5); // if the change in coins is greater than a 5 seconds growth, than we consider it changed. we do this, because there is always a small mismatch between front end and back end calculation
            this.changes.troops = true; // TODO FINISH
            this.changes.points = this.points != data.Village.points;
            this.changes.popRem = (this.popRemaining != data.Village.popMax - data.Village.popCur);
            this.changes.type = this.villagetypeid != data.Village.type;
            this.changes.name = this.name != data.Village.name;
            this.changes.yey = data.Village.yey != this.yey;
            this.changes.areTranspAvail = this.areTranspAvail != data.Village.areTranspAvail;

            // compare the VOV data changes
            if (this.VOV) {
                var newBuilding;
                var oldBuilding;
                for (var i = 0; i < data.VOV.buildings.length; i++) {
                    newBuilding = data.VOV.buildings[i];
                    oldBuilding = this.VOV.getBuildingByID(newBuilding.id);
                    if (!oldBuilding) {
                        this.changes.buildings = true;
                        break;
                    } else {
                        // check if something about the building has changed
                        if (oldBuilding.built != newBuilding.built ||
                            oldBuilding.cnstr != newBuilding.cnstr ||
                            oldBuilding.image != newBuilding.image ||
                            oldBuilding.image_c != newBuilding.image_c ||
                            oldBuilding.level != newBuilding.level) {
                            this.changes.buildings = true;
                        }

                        // check if the recruitment Q [ building work timer ] changed
                        //  why dont we check if "buildcount" changed? because buildcount for one building will always be included in HQ's recruitcount (since HQ's work is building) so we only need to check recruitcount
                        newBuildingRecruitTimeFinishOn = newBuilding.recruitcount ? new Date(parseInt(newBuilding.recruitcount)) : new Date('jan 1 1970');
                        oldBuildingRecruitTimeFinishOn = oldBuilding.recruitFinishesOn ? new Date(parseInt(oldBuilding.recruitFinishesOn)) : new Date('jan 1 1970');
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
            var troops;
            for (var i = 0; i < data.Village.Units.length; i++) {
                troops = new VillageTroops(data.Village.Units[i].id, data.Village.Units[i].YourUnitsCurrentlyInVillageCount, data.Village.Units[i].CurrentlyRecruiting, data.Village.Units[i].SupportCount, data.Village.Units[i].YourUnitsTotalCount);

                this._TroopsList.push(troops);

                this._TroopsDictionary["id" + troops.id] = {
                    id: troops.id,
                    YourUnitsCurrentlyInVillageCount: troops.YourUnitsCurrentlyInVillageCount,
                    CurrentlyRecruiting: troops.CurrentlyRecruiting,
                    SupportCount: troops.SupportCount,
                    YourUnitsTotalCount: troops.YourUnitsTotalCount,
                    TotalNowInVillageCount: troops.TotalNowInVillageCount()
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
        };

        Object.defineProperty(Village.prototype, "coins", {
            get: function () {
                var coinsNow = this._coins_raw;
                if (coinsNow < this.coinsTresMax) {
                    var now = new Date();
                    var ts = now.getTime() - this.timeStampWhenExtendedPropertiesLastRetrieved.getTime();
                    if (ts >= 1000) {
                        var change;

                        change = Math.floor((this.coinsPH / (3600 * 1000)) * ts);

                        if (coinsNow + change <= this.coinsTresMax) {
                            coinsNow = coinsNow + change;
                        } else {
                            coinsNow = this.coinsTresMax;
                        }
                    }
                } else if (coinsNow > this.coinsTresMax) {
                    coinsNow = this.coinsTresMax;
                }
                return coinsNow;
            },
            enumerable: true,
            configurable: true
        });
        return Village;
    })();
    ROE.Village = Village;

    var VillageExtendedDataApiCallData = (function () {
        function VillageExtendedDataApiCallData() {
        }
        return VillageExtendedDataApiCallData;
    })();
    ROE.VillageExtendedDataApiCallData = VillageExtendedDataApiCallData;

    var VillageTroops = (function () {
        function VillageTroops(id, yourInVillage, recruiting, support, yourTotal) {
            this.id = id;
            this.YourUnitsCurrentlyInVillageCount = yourInVillage;
            this.CurrentlyRecruiting = recruiting;
            this.SupportCount = support;
            this.YourUnitsTotalCount = yourTotal;
        }
        VillageTroops.prototype.TotalNowInVillageCount = function () {
            return this.SupportCount + this.YourUnitsCurrentlyInVillageCount;
        };
        return VillageTroops;
    })();
    ROE.VillageTroops = VillageTroops;

    var VillageExtendedBasicInfoApiCallData = (function () {
        function VillageExtendedBasicInfoApiCallData() {
        }
        return VillageExtendedBasicInfoApiCallData;
    })();
    ROE.VillageExtendedBasicInfoApiCallData = VillageExtendedBasicInfoApiCallData;

    var VillageChanges = (function () {
        function VillageChanges() {
        }
        return VillageChanges;
    })();
    ROE.VillageChanges = VillageChanges;

    var VOVInfo = (function () {
        function VOVInfo(data) {
            this.animations = data.animations;
            this.bg = data.bg;
            this.trees = data.trees;
            this.isday = data.isday;

            this.buildings = new Array(data.buildings.length);
            for (var i = 0; i < data.buildings.length; i++) {
                this.buildings[i] = new VOVInfo_Building(data.buildings[i].id, data.buildings[i].level, data.buildings[i].image, data.buildings[i].images, data.buildings[i].image_c, data.buildings[i].built, data.buildings[i].cnstr, data.buildings[i].buildcount, data.buildings[i].recruitcount, data.buildings[i].areRequirementsSatisfied);
            }
        }
        VOVInfo.prototype.getBuildingByID = function (id) {
            for (var i = 0; i < this.buildings.length; i++) {
                if (this.buildings[i].id == id) {
                    return this.buildings[i];
                }
            }
            return undefined;
        };
        return VOVInfo;
    })();
    ROE.VOVInfo = VOVInfo;

    var VOVInfo_Building = (function () {
        function VOVInfo_Building(id, level, image, images, image_c, built, cnstr, buildcount, recruitcount, areRequirementsSatisfied) {
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
                this.recruitFinishesOn = this.recruitcount;
            }
        }
        return VOVInfo_Building;
    })();
    ROE.VOVInfo_Building = VOVInfo_Building;
})(ROE || (ROE = {}));
