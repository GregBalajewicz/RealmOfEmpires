


interface padDigitsInterface{
    (num:number):string;
}

declare var padDigits: padDigitsInterface;

module BDA.Console {
    export var v = 1;
    export var persistedLog = [];
    export var persist = false; // if true, each log will be stored in array - we will "persist" all logs
    export var logToConsole = true; // if true, we will attempt to log to console.log
    export var logHtmlObject = null; // give it a jquery div or spam or somehting that the log messages will be appended to
    export var _showByType = [true, true, true];
    export var _showByCategory = { _default: true };
    export var _showAllCategories= true // if true, shows all categories unless the category has been explicitly set to hide it. if false, show only categories explicitily set to show


    export var _types= [{ 'classname': '' }, { 'classname': 'error' }, { 'classname': 'verbose' }];

    export var _handleOneEntry = function (logEntry) { 
        if (logHtmlObject) {
            logHtmlObject.append('<div class="logentry ' + _types[logEntry.type].classname + '">'
            + padDigits(logEntry.now.getHours()) + ':'
            + padDigits(logEntry.now.getMinutes()) + ':'
            + padDigits(logEntry.now.getSeconds()) + ':'
            + padDigits(logEntry.now.getMilliseconds())
            + '[' + logEntry.cat + ']'
            + ' : ' + logEntry.msg);
        }
        if (logToConsole && window.console) {
            console.log(


            padDigits(logEntry.now.getHours()) + ':'
            + padDigits(logEntry.now.getMinutes()) + ':'
            + padDigits(logEntry.now.getSeconds()) + ':'
            + padDigits(logEntry.now.getMilliseconds())
            + '{' + (['i', 'e', 'v'][logEntry.type]) + '}'
            + '[' + logEntry.cat + ']'
                , logEntry.msg);
        }
    };
    
           
        
    export  var _log = function (cat, type, message) {
        if (_showByType[type]) // maybe we should still keep the even just not display it ?
        {
            if (_showByCategory[cat]
                || (_showAllCategories && !(_showByCategory[cat] === false))
                || type === 1)                 {
                var logEntry = { now: new Date(), type: type, msg: message, cat: cat };
                if (persist) {
                    persistedLog.push(logEntry)
                }
    
                _handleOneEntry(logEntry);
            }
        }
    }




    
    export var verbose = function (cat, message) {
        if (arguments.length == 1) {
            _log('_default', 2, cat);
        } else {
            _log(cat, 2, message);
        }
    }
    
    export var log = function (cat, message) {
        if (arguments.length == 1) {
            _log('_default', 0, cat);
        } else {
            _log(cat, 0, message);
        }
    }
    
    export var error = function (cat, message) {
        if (arguments.length == 1) {
            _log('_default', 1, cat);
        } else {
            _log(cat, 1, message);
        }
    
    }
    
    
        //var clearPersistedLog = function (message) {
        //    _persistenlog = [];
        //}
    
    
    export var showVerbose = function (value) {
        //BDA.Val.validate(value, 'only true/false', true, function (param) { return typeof param === 'boolean' }, value)
        showType[2] = value;
    }
    export var showType = function (type, value) {
        //BDA.Val.validate(type, 'invalid type', true, function (param) { return type in [0, 1, 2] }, type)
        //BDA.Val.validate(value, 'only true/false', true, function (param) { return typeof param === 'boolean' }, value)
        _showByType[type] = value;
    }
    export var showCategory = function (cat, value) {
        //BDA.Val.required(cat, 'cat is req', cat)
        //BDA.Val.validate(cat, 'only string for cat', true, function (param) { return typeof param === 'string' }, cat)
        //BDA.Val.validate(value, 'only true/false', true, function (param) { return typeof param === 'boolean' }, value)
        _showByCategory[cat] = value;
    }
    export var setCategoryDefaultView = function (cat, value) {
        ///<summary> set this category's display setting unless it has been set already. ie, this does not override an already set setting. to override, use showCategory</summary>
        //BDA.Val.required(cat, 'cat is req', cat)
        // BDA.Val.validate(cat, 'only string for cat', true, function (param) { return typeof param === 'string' }, cat)
        //BDA.Val.validate(value, 'only true/false', true, function (param) { return typeof param === 'boolean' }, value)
        if (!(cat in _showByCategory)) {
            _showByCategory[cat] = value;
        }
    }
    export var getShowAllCategories = function () {
        ///<summary> if true, shows all categories unless the category has been explicitly set to hide it. if false, show only categories explicitily set to show</summary>
        return _showAllCategories;
    }
    export var setShowAllCategories = function (value) {
        ///<summary> if true, shows all categories unless the category has been explicitly set to hide it. if false, show only categories explicitily set to show</summary>
        //BDA.Val.validate(value, 'only true/false', true, function (param) { return typeof param === 'boolean' }, value)
        _showAllCategories = value;
    }
    export var forceShowAllCatgories = function () {
        ///<summary> sets show all categories to true, and clear any settings set via showCategory</summary>
        _showAllCategories = true;
        _showByCategory = { _default: true };
    }


}
