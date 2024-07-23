(function (ROE) {
} (window.ROE = window.ROE || {}));

(function (obj) {
    var _storage;
    var _storageRawForCompare = "";



    function _init(storage) {
        if (_storageRawForCompare != storage) {
            _storage = jQuery.parseJSON(storage) || {};
            _storageRawForCompare = storage;
        }
    }


    function id () { return ROE.realmID + '_' + ROE.playerID; }
   
    function _get(key) {
        return _storage[key + '_' + id()];
    }
    function _set(key, value, callbackSuccess) {
        _storage[key + '_' + id()] = value;
        ROE.Api.call('setlocalserverstorage', {
            data: $.toJSON(_storage)
        }, callbackSuccess, undefined, true, undefined, 'POST');
        return value;
    }

    //like get/set above, but instead of limited to realm, they are user account wide "Global"
    function _getGlobal(key) {
        return _storage[key];
    }
    function _setGlobal(key, value, callbackSuccess) {
        _storage[key] = value;
        ROE.Api.call('setlocalserverstorage', {
            data: $.toJSON(_storage)
        }, callbackSuccess, undefined, true, undefined, 'POST');
        return value;
    }

    obj.init = _init;
    obj.get = _get;
    obj.set = _set;
    obj.getGlobal = _getGlobal;
    obj.setGlobal = _setGlobal;

   
} (window.ROE.LocalServerStorage = window.ROE.LocalServerStorage || {}));


