
BDA.Templates =
{
    templateLocationPrefix: "templates/",
    _cache: {},
    _cacheJQObjects: {},



    _preload_helper: function (name, realmID, params, async) {
        var loadnames = [];

        $.each(name.split(','), function (i, n) {
            if (!BDA.Templates._cache[n + realmID]) { loadnames.push(n); }
        });

        if (loadnames.length != 0) {

            $.ajax({
                url: BDA.Templates.templateLocationPrefix + 'Bunch.aspx?tt=' + loadnames.join(',') + '&rid=' + realmID
                , data: params
                , dataType: 'json'
                , async: async
                , success: function (r) { BDA.Templates._reload_callback(r.object, realmID); }
                , error: BDA.Templates._reload_failureCallBack
            });
        }
    },


    _reload_failureCallBack: function (res, realmID) {
        if (res == undefined) {
            if (confirm("Critical asset failed to load, most likely due to network connection issues. We recommend you refresh or the game may not work properly. Refresh?")) {
                window.location.reload();
            }
        }
    },

    _reload_callback: function (res, realmID) {
        if (res == undefined) {
            if (confirm("Critical asset failed to load, most likely due to network connection issues. We recommend you refresh or the game may not work properly. Refresh?")) {
                window.location.reload();
            }
        }
        for (var n in res) {
            this._cache[n + realmID] = res[n];
        }
    },

    preload_asynch: function (name, realmID, params) {
        BDA.Templates._preload_helper(name, realmID, params, true);
    },

    preload: function (name, realmID, params) {
        BDA.Templates._preload_helper(name, realmID, params, false);
    },


    get: function (name, realmID, data, params) {
        this.preload(name, realmID, params);

        return this.populate(this._cache[name + realmID], data);
    },

    getRaw: function (name, realmID, data, params) {
        ///<Summary> like get but does not fill the template with date but returns it raw </summary>
        this.preload(name, realmID, params);

        return this._cache[name + realmID];
    },

    getRawJQObj: function (name, realmID, data, params) {
        ///<Summary> like getRaw but give you a JQuery object that represents this template. this is cached</summary>

        if (!BDA.Templates._cacheJQObjects[name + realmID])
        {
            this._cacheJQObjects[name + realmID] = $(BDA.Templates.getRaw(name, realmID, data, params));
        }
        return this._cacheJQObjects[name + realmID].clone();
    },


    populate: function (res, data) {
        try {
            return res.replace(/%([a-zA-Z0-9_.]*)%/g,
                        function (m, key) {
                            var ks = key.split('.');
                            var el = null;
                            $.each(ks, function (i, n) {
                                el = (!el ? data : el).hasOwnProperty(n) ? (!el ? data : el)[n] : "";
                            })
                            return el;
                        }
                   );
        } catch (e) {
            var roeex = new BDA.Exception("erorr in BDA.Templates.populate", e);
            roeex.data.add('res', res);
            roeex.data.add('data', data);
            BDA.latestException = roeex;
            BDA.Console.error(roeex);
            throw roeex;
        }
    }

    , getContextInfo: function getContextInfo() {
        return {
            '_cache': BDA.Templates._cache
            
        };
    }
};


