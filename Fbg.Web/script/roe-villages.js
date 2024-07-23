"use strict";

(function (obj) {
    BDA.Console.setCategoryDefaultView('ROE.Village', false); // by default, do not display the performance category. this assumes BDA.Console is defined BEFORE this file
    BDA.Console.setCategoryDefaultView('ROE.Villages', false); // by default, do not display the performance category. this assumes BDA.Console is defined BEFORE this file
    BDA.Console.setCategoryDefaultView('VillChanges', false); // by default, do not display the performance category. this assumes BDA.Console is defined BEFORE this file


    var CONST = {};
    CONST.Enum = {
        AttackType: { support: 0, attack: 1 }
    };


    obj.Enum = {}; // enums
    obj.Enum.ExtendedDataRetrieveOptions = {
        doNothing: 0
        , ensureExists: 1
        , ensureExistsAndTriggerGetLatest: 2
    };

 

    

    var _Villlist = []; // array of objects of type ROE.Village


    // Note that calls to any of the _ExtendedInfo_loadLatest*, could fail to call optionCallBack if an api call to 
    //  loaded extended data was already in progress, thus call to _ExtendedInfo_loadLatest* resulted in no actual call made; 
    //  via this variable, we remember the call back, and call it when extended info is loaded. 
    // WARNING! This has a possibility of memory leaks!!
    //  if a api call to loade extended data for a particular village fails, and thus never comes back, then the call backs stored here 
    //  will never be called, but worse, the reference to those call backs will remain! this keeping what ever is tied to those function in memory. 
    // We try to remedy this a bit, by doing :
    //      delete _listsOfCallBacks[village.id]; 
    //  before we make a new api call to get extended data. 
    //  why? because if we are making an API call, that means that there is no call currently pending, meaning that 
    //  anything in _listsOfCallBacks[village.id], must be from some old call that failed to return. 
    var _listsOfCallBacks = {};


    var _getVillages = function (cb) {
        
        if (_Villlist.length === 0 || _Villlist.length != ROE.playersNumVillages) {
            // readTransaction not supported on iPad people saying
            // http://stackoverflow.com/questions/3809229/html5-readtransaction-not-supported-on-ipad-ios-3-2
           
            // TODO: Test whether this will actually function the same or not.... and replace the current rawTx call
            // Needs to function properly for the new indexedDB setup with websql fallback / nothing fallback.

            BDA.Database.SelectAll("VillagesMy", "name", "asc").done(function (results) {
               if (results.length != ROE.playersNumVillages) {
                    ajax('api.aspx', { fn: 'VillageListA' }, function (data) {
                        _gotVillageListFromServer(function () {
                            _getVillages(cb);
                        }, data);
                    });
                } else {
                    // Need to update in order to work with a different format of results
                    _getVillages_HandleResult(cb, results);
                }
            });
            
           
            /*BDA.Database.GetDatabase().rawTx(function (tx) {
                tx.executeSql('SELECT * FROM VillagesMy_' + BDA.Database.id() + ' ORDER BY name asc', []
                    , function (tx, results) {
                        console.log("txresults");
                        console.log(results.rows);
                        if (results.rows.length != ROE.playersNumVillages) {
                            ajax('api.aspx', { fn: 'VillageListA' }, function (data) {
                                _gotVillageListFromServer(function () {
                                    _getVillages(cb);
                                }, data);
                            });
                        } else {
                            _getVillages_HandleResult(cb, results);
                        }
                    }
                );
            });*/
        } else {
            setTimeout(function _getVillages_timeout() { cb(_Villlist) }, 1);
        }
    }

    var _getVillage = function (villageID, callBack, extendedDataRetrieveOptions, doNotRefreshListWhenVillageNotFound) {
        ///<summary>
        ////   gives you the village object returned in the call back. If you call this with a villageID that is not yours, the village list be 
        ///    refreshed once to make ensure we got the latest village list; if you do not want the list to be refreshed, pass in true for doNotRefreshListWhenVillageNotFound
        ///</summary>
        ///<param name="callBack">call back function. signature : function (ROE.Village object). the object may be undefined if did not find villge with this villageID</param>
        ///<param name="extendedDataRetrieveOptions">
        /// optional param, of type ROE.Villages.Enum.ExtendedDataRetrieveOptions
        /// if ommited, same as passing in ROE.Villages.Enum.ExtendedDataRetrieveOptions.doNothing
        ///
        ///     ROE.Villages.Enum.ExtendedDataRetrieveOptions.doNothing
        ///         returns you the village object, but the extended properties / data may be undefined. 
        ///     ROE.Villages.Enum.ExtendedDataRetrieveOptions.ensureExists
        ///         returns you the village object, WITH the extended properties / data having validat values, althought those may be cached, and therefore old values.  
        ///     ROE.Villages.Enum.ExtendedDataRetrieveOptions.ensureExistsAndTriggerGetLatest
        ///         simillar to "ensureExists" except that, if we get the extended data / properties from cache, then a ROE.Villages.ExtendedInfo_loadLatest is triggered
        ///         for this village. Note that you will still get back the cached data, however, you can be assures that ROE.Villages.ExtendedInfo_loadLatest was triggered and hence
        ///         updated data will soon be available.
        ///
        ///         This is very simillar to doing this:
        ///
        ///             ROE.Villages.getVillage(x, callback, ROE.Villages.Enum.ExtendedDataRetrieveOptions.ensureExists);
        ///             ROE.Villages.ExtendedInfo_loadLatest(x)
        ///
        ///         Except that the code above MAY result in two api calls to be made, while doing this : 
        ///             ROE.Villages.getVillage(x, callback, ROE.Villages.Enum.ExtendedDataRetrieveOptions.ensureExistsAndTriggerGetLatest);
        ///
        ///         will ensure that only one api call is made; if we first retrieved the extended data via an api call, another api call will not be made
        ///</param>
        ///<param name="doNotRefreshListWhenVillageNotFound">Optional param; default false. see summary for how it is used </param>
        ///
        BDA.Val.required(villageID, "villageID must be specified and type int");
        BDA.Val.assertFUNCT(callBack, "callBack must be a function");
        _getVillages(function () { _getVillage_villageListReady(villageID, callBack, extendedDataRetrieveOptions, doNotRefreshListWhenVillageNotFound); });
    }

   
    var _getVillage_villageListReady = function (villageID, callBack, extendedDataRetrieveOptions, doNotRefreshListWhenVillageNotFound) {
        extendedDataRetrieveOptions = extendedDataRetrieveOptions || ROE.Villages.Enum.ExtendedDataRetrieveOptions.doNothing; // default value if not specified
        doNotRefreshListWhenVillageNotFound = doNotRefreshListWhenVillageNotFound == undefined ? false : doNotRefreshListWhenVillageNotFound; // default value if not specified
        if (callBack == undefined) {
            var roeex = new BDA.Exception("callBack in _getVillage_villageListReady undefined");
            roeex.data.add('villageID', villageID);
            BDA.latestException = roeex
            BDA.Console.error(roeex);
            throw roeex;
        }
        var village = _getVillageByID(villageID);

        if (village) {
            //
            // ok, we got a village, now, what do we do about extended data... 
            //
            if (extendedDataRetrieveOptions == ROE.Villages.Enum.ExtendedDataRetrieveOptions.doNothing) {
                //
                // do nothing special, so just give this village object
                callBack(village);
            } else if (extendedDataRetrieveOptions == ROE.Villages.Enum.ExtendedDataRetrieveOptions.ensureExists) {
                //
                // ensure that extended data is populated, and if not, get it. 
                if (village._TroopsList == undefined) {
                    _ExtendedInfo_loadLatest(village.id, callBack);
                } else {
                    callBack(village);
                }
            } else if (extendedDataRetrieveOptions == ROE.Villages.Enum.ExtendedDataRetrieveOptions.ensureExistsAndTriggerGetLatest) {
                //
                // ensure that extended data is populated, and trigger get latest if needed
                if (village._TroopsList == undefined) {
                    //
                    // extended data not available, so we must make a call to get it. 
                    //  however, since we already make an API call, do nto call loadlatest 
                    _ExtendedInfo_loadLatest(village.id, function handleCallBackFrom_ExtendedInfo_loadLatestFORensureExistsAndTriggerGetLatest(village) {
                        callBack(village);
                    });
                } else {
                    //
                    // extended data is available, hence it was cached, hence call loadlatest 
                    _ExtendedInfo_loadLatest(village.id);
                    callBack(village);
                }
            }


        } else {
            //
            // village not found. 
            //  now why would that be? Player passed in a wrong VID? possibly
            //  but perhaps because the village list is no longer valid; maybe we got a village in village list that is no longer player's 
            //  or a village is missing from the list. 
            //  SO, we try, once, to refresh the village list and we try again. 
            //
            if (doNotRefreshListWhenVillageNotFound) {
                // either user requested we dont try to refresh the village list, or we are here after refreshing the list, so we dont do it again
                callBack(undefined);
            } else {
                // refresh the list and try again. 
                BDA.Console.verbose('ROE.Villages', "Village not found, clearing village list cache and trying again, vid:" + villageID);
                _clearCache();
                _getVillage(
                    villageID
                    , callBack
                    , extendedDataRetrieveOptions
                    , true // this is important; this will ensure we do not try to refresh the village list again thus causing an infinite loop. 
                    );
            }
        }
    }


    // NOTE !! this function assumes that _Villlist is ready 
    var _getVillageByID = function (villageID) {

        for (var i = 0; i < _Villlist.length; i++) {
            if (_Villlist[i].id == villageID) {
                return _Villlist[i];
            }
        }
        return undefined;
    }


    var _gotVillageListFromServer = function (cb, data) {

        BDA.Database.Delete('VillagesMy');

        ///<summary> called back from ajax call when village list call completes</summary>
        var updata = [];
        for (var i in data) {
            var di = data[i];
                        
            if (typeof (di) != 'function') {
                updata.push(di);
            }
        }

        BDA.Database.Insert('VillagesMy', updata, cb);
    }

    var _getVillages_HandleResult = function (cb, results) {

        _Villlist = [];
        var village;
        //var len = results.rows.length;
        var len = results.length;
        //TODO:  IMPORTANT -- is this the most efficient way of ADDING to an array? ? ???? 
        for (var i = 0; i < len; i++) {
            //village = new ROE.Village(results.rows.item(i).id, results.rows.item(i).name, results.rows.item(i).points, results.rows.item(i).villagetypeid, results.rows.item(i).x, results.rows.item(i).y)
            // Not using SQL objects anymore but left line incase we need to revert.
            village = new ROE.Village(results[i].id, results[i].name, results[i].points, results[i].villagetypeid, results[i].x, results[i].y)

            _Villlist.push(village);

        }
        cb(_Villlist);
    }

    var _clearCache = function () {
        BDA.Console.log('ROE.Villages', "clearing the cached village list");
        _Villlist = [];
        BDA.Database.Delete('VillagesMy');
    }


    function _ExtendedInfo_loadLatestIFOlderThan(villageID, getLatestFromDate, optionalCallBack) {
        ///<summary>force load latest troop counts for this village. 
        ///     Exactly the same as _ExtendedInfo_loadLatest except that this, will check when was the village object's extended data retrieved last
        ///      and compare it to "getLatestFromDate". If getLatestFromDate is older than the last time the object was retrieved, then no need to 
        ///      retrieve data again and nothing is done, otherwise, we reload the extended data
        ///</summary>
        BDA.Val.required(villageID, "villageID must be specified and type int");
        BDA.Val.assertDATE(getLatestFromDate, "getLatestFromDate must be specified and type Date");
        if (optionalCallBack) {
            BDA.Val.assertFUNCT(optionalCallBack, "if optionalCallBack param is specified, it must be undefined or a function");
        }

        BDA.Console.verbose('ROE.Villages', "load extended info if older - vid:%vid%, getLatestFromDate:%d%".format({ vid: villageID, d: getLatestFromDate }));

        // we must get the village; without that, we cannot know when it was loaded last 
        _getVillage(villageID, function _ExtendedInfo_loadLatestIFOlderThan_callback(village) {
            if (village) {
                if (village.areExtendedPropertiesAvailable) {
                    // since extended properties are there, only reload, if the passed in date is further in the future than the last time 
                    //  the object was loaded (we do >= and not > on purpose)
                    if (getLatestFromDate >= village.timeStampWhenExtendedPropertiesLastRetrieved) {
                        BDA.Console.verbose('ROE.Villages', "load extended info if older - info old, loading new - vid:%vid%, getLatestFromDate:%d%, village.timeStampWhenExtendedPropertiesLastRetrieved:%d2%".format({ vid: villageID, d: getLatestFromDate, d2: village.timeStampWhenExtendedPropertiesLastRetrieved }));
                        _ExtendedInfo_loadLatest_Master(village, optionalCallBack)
                    } else {
                        BDA.Console.verbose('ROE.Villages', "load extended info if older - info newer, not reloading - vid:%vid%, getLatestFromDate:%d%, village.timeStampWhenExtendedPropertiesLastRetrieved:%d2%".format({ vid: villageID, d: getLatestFromDate, d2: village.timeStampWhenExtendedPropertiesLastRetrieved }));
                    }                    
                } else {
                    // since exended info is not at all loaded, then do NOT load it since it is definatelly not OUT OF DATE; 
                    //  not loading the info ensures that on refersh, not all villages with a record in cachetimestams are loaded
                    BDA.Console.verbose('ROE.Villages', "load extended info if older - no info , not loading- vid:%vid%".format({ vid: villageID }));
                    //_ExtendedInfo_loadLatest(villageID, optionalCallBack)
                }
            }
        });
    }

    function _ExtendedInfo_loadLatest(villageID, optionalCallBack) {
        ///<summary>force load latest troop counts for this village. 
        ///     note that this will BDA.broadcast event VillageExtendedInfoUpdated which tell you that extended info was updated;
        ///     if the extended info is set for the first time, then VillageExtendedInfoInitiallyPopulated will be broadcasted instead.
        ///</summary>
        ///<param name="villageID">village id to reload, must be interger </param>
        ///<param name="optionalCallBack">if you do not want to rely on the BDA.broadcast of VillageExtendedInfoUpdated event, you can pass an optional call back, that is basically
        /// very much simillar to the event; gets called with the village object updated
        /// NOTE that this call back, is guaranteed to be called every time we load latest datat, while VillageExtendedInfoUpdated MAYBE changed in the future to be triggered only if something actually changed
        ///</param>
        BDA.Val.required(villageID, "villageID must be specified and type int");
        if (optionalCallBack) {
            BDA.Val.assertFUNCT(optionalCallBack, "if optionalCallBack param is specified, it must be undefined or a function");
        }

        _getVillage(villageID, function _ExtendedInfo_loadLatest_getVillageCallBack(village) {
            if (village) {
                _ExtendedInfo_loadLatest_Master(village, optionalCallBack);
            } else {
                BDA.Console.info('ROE.Villages', "got null village when loading extended info for vid:" + villageID);
                if (optionalCallBack) { optionalCallBack(village) };
            }
        });
    } 

    /*
        There was an issue of Village List and Mass features, in large empires, calling too many API calls and choking the client.
        To fix this, we need to que up village load API calls, and execute them X amount at a time.
        We put all calls to _ExtendedInfo_loadLatest_Master in an array que, and process them n batches with a delay to allow other calls to go through.
    */
    var _loadLatestQue = [];
    var _processBatchIntervalTime = 2 * 1000; //seconds before every batch call
    var _processMaxBatchSize = 5; //Number of calls to make per delay
    var _processLoadLatestQueThrottle; //stores timeout
    var _loadQLatestInProcessCount = 0; //how many being processed atm

    //for new lists of calls coming in, to allow us to cluster together calls, but then put new individual calls ahead of the cluster
    var _tempQ = [];

    function _ExtendedInfo_loadLatest_Master(village, optionalCallBack) {
        _queUpLoadLatest(village, optionalCallBack);
    }


    function _queUpLoadLatest(village, optionalCallBack) {

        //console.log("_queUpLoadLatest", village.id);

        //find and delete previously qued up duplicates before placing new one
        var loadLatestCall;
        for (var i = 0, l = _loadLatestQue.length; i < l; i++) {
            loadLatestCall = _loadLatestQue[i];
            if (loadLatestCall && loadLatestCall.village.id == village.id && loadLatestCall.optionalCallBack == optionalCallBack) {
                _loadLatestQue[i] = null;
            }
        }


        _tempQ.push({
            village: village,
            optionalCallBack: optionalCallBack
        });
        
        //throttle
        clearTimeout(_processLoadLatestQueThrottle);
        _processLoadLatestQueThrottle = setTimeout(function () {
            _loadLatestQue = _tempQ.concat(_loadLatestQue);
            _tempQ = [];
            _processLoadLatestQue();
        }, 5);
     
        

    }

    function _processLoadLatestQue() {

        //clean the prcess Q
        var tempArr = new Array();
        for (var i = 0, l = _loadLatestQue.length; i < l; i++) {
            if (_loadLatestQue[i]) {
                tempArr.push(_loadLatestQue[i]);
            }
        }
        _loadLatestQue = tempArr;

        //if Q is empty, abort        
        if (_loadLatestQue.length < 1) {
            //console.log("_processLoadLatestQue EMPTY Q");
            return;
        }

        _processLoadLatestQueInternal();

        function _processLoadLatestQueInternal() {

            if (_loadLatestQue.length < 1) {
                return;
            }

            setTimeout(function () {
                _processLoadLatestQueInternal();
            }, _processBatchIntervalTime);

            //if too many calls out dont process any more
            if (_loadQLatestInProcessCount > _processMaxBatchSize) {
                //console.log("_processLoadLatestQueInternal process overflow, bounced.");
                return;
            }

            var processNowCount = Math.min(_processMaxBatchSize, _loadLatestQue.length);
            _loadQLatestInProcessCount += processNowCount;

            var loadLatestCall;
            for (var i = 0; i < processNowCount; i++) {

                loadLatestCall = _loadLatestQue.shift();
                if (loadLatestCall) {
                    //console.log("process vid", loadLatestCall.village.id);
                    _ExtendedInfo_loadLatest_Master_Execute(loadLatestCall.village, loadLatestCall.optionalCallBack);
                }
            }
        
            //console.log("_processLoadLatestQueInternal Q length:", _loadLatestQue.length, "in process:", _loadQLatestInProcessCount);

        }

    }



    function _ExtendedInfo_loadLatest_Master_Execute(village, optionalCallBack) {

        //console.log("_ExtendedInfo_loadLatest_Master_Execute", village.id);

        ///<summary>This is the private function that actually triggers load of the latest info
        ///</summary>
        ///<param name="villageID">village id to reload </param>
        ///<param name="optionalCallBack">if you do not want to rely on the BDA.broadcast of VillageExtendedInfoUpdated event, you can pass an optional call back, that is basically
        /// very much simillar to the event; gets called with the village object updated
        /// NOTE that this call back, is guaranteed to be called every time we load latest datat, while VillageExtendedInfoUpdated MAYBE changed in the future to be triggered only if something actually changed
        ///</param>

        if (!village.loadingExtended_isLoading) {
            BDA.Console.verbose('ROE.Villages', "loading extended info for vid:" + village.id);
            BDA.Console.verbose('VillChanges', "VID:%vid% - loading latest".format({ vid: village.id }));
            delete _listsOfCallBacks[village.id]; // IMPORTANT! we do this, because any callbacks that MAY be here, are orphaned. Read more above in comments by defintion of _listsOfCallBacks
            village.loadingExtended_signalLoadInitiated();
            ROE.Api.call("getvillage", { vid: village.id }, function (data) { _ExtendedInfo_loadLatestCallBack(data, optionalCallBack) });
        } else {
            _loadQLatestInProcessCount--;
            _loadQLatestInProcessCount = Math.max(_loadQLatestInProcessCount, 0);
            //
            // extended info is currently in the process of loading. 
            //
            BDA.Console.verbose('ROE.Villages', "loading extended info NOT!- load already in progress - for vid:" + village.id);
            BDA.Console.verbose('VillChanges', "VID:%vid% - loading latest - NOT! - load already in progress ".format({ vid: village.id }));
            if (optionalCallBack) {
                // because 
                BDA.Console.verbose('ROE.Villages', "remembering the optionalCallBack - for vid:" + village.id);
                _listsOfCallBacks[village.id] = _listsOfCallBacks[village.id] || [];
                _listsOfCallBacks[village.id].push(optionalCallBack);
            }
        }
    }



    function _ExtendedInfo_loadLatestCallBack(data, optionalCallBack) {
        _loadQLatestInProcessCount--;
        _loadQLatestInProcessCount = Math.max(_loadQLatestInProcessCount, 0);
        if (data) { // data may be null if trying to get troops for wrong villageid for example
            _getVillage(data.Village.id, function _ExtendedInfo_loadLatestCallBack_callback(village) {
                _populateExtendedData(data.Village.id, village, data, optionalCallBack)
            });
        }
    }


    function _populateExtendedData(vid, village, data, optionalCallBack) {
        if (village) {
            if (village.areExtendedPropertiesAvailable) {
                BDA.Console.verbose('ROE.Villages', "updating extended info - vid:%vid%".format({ vid: village.id }));
                BDA.Console.log('VillChanges', "VID:%vid% - updating extended info".format({ vid: village.id }));
                village.setExtendedData(data);
                BDA.Broadcast.publish("VillageExtendedInfoUpdated", village);
            } else {
                BDA.Console.verbose('ROE.Villages', "set extended info first time - vid:%vid%".format({ vid: village.id }));
                BDA.Console.log('VillChanges', "VID:%vid% - updating extended info - first time".format({ vid: village.id }));
                village.setExtendedData(data);
                BDA.Broadcast.publish("VillageExtendedInfoInitiallyPopulated", village);
            }
        }

        // NOTE! this call back shoudl be called EVERY TIME; even if troop count has not changed, even if initially populated, *even if village not found*! etc 
        if (optionalCallBack) {
            optionalCallBack(village);
        }
        // now step through any callbacks that were saved because call to _ExtendedInfo_loadLatest did not result in an api call, since one api call was already in progress. 
        //  we still want to call all these callbacks so anyone waiting for extendeddata to be loaded via a call back and not via the event, will be notified. 
        //  Why are we using data.Village.id and not village.id? Because it is possible that village is undefined / null
        var callbacksArray = _listsOfCallBacks[vid];
        if (callbacksArray) {
            delete _listsOfCallBacks[vid];
            for (var i = 0; i < callbacksArray.length; i++) {
                if (typeof (callbacksArray[i]) == "function") {
                    callbacksArray[i](village);
                }
            }
        }

    };
        
    



    obj.ExtendedInfo_loadLatest = _ExtendedInfo_loadLatest;
    obj.ExtendedInfo_loadLatestIFOlderThan = _ExtendedInfo_loadLatestIFOlderThan;
    obj.getVillages = _getVillages;
    obj.getVillage = _getVillage;
    obj.clearCache = _clearCache;
    obj.__populateExtendedData = _populateExtendedData;

} (window.ROE.Villages = window.ROE.Villages || {}));