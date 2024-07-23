var BDA;
(function (BDA) {
    (function (Console) {
        Console.v = 1;
        Console.persistedLog = [];
        Console.persist = false;
        Console.logToConsole = true;
        Console.logHtmlObject = null;
        Console._showByType = [
            true, 
            true, 
            true
        ];
        Console._showByCategory = {
            _default: true
        };
        Console._showAllCategories = true;
        Console._types = [
            {
                'classname': ''
            }, 
            {
                'classname': 'error'
            }, 
            {
                'classname': 'verbose'
            }
        ];
        Console._handleOneEntry = function (logEntry) {
            if(Console.logHtmlObject) {
                Console.logHtmlObject.append('<div class="logentry ' + Console._types[logEntry.type].classname + '">' + padDigits(logEntry.now.getHours()) + ':' + padDigits(logEntry.now.getMinutes()) + ':' + padDigits(logEntry.now.getSeconds()) + ':' + padDigits(logEntry.now.getMilliseconds()) + '[' + logEntry.cat + ']' + ' : ' + logEntry.msg);
            }
            if(Console.logToConsole && window.console) {
                console.log(padDigits(logEntry.now.getHours()) + ':' + padDigits(logEntry.now.getMinutes()) + ':' + padDigits(logEntry.now.getSeconds()) + ':' + padDigits(logEntry.now.getMilliseconds()) + '{' + ([
                    'i', 
                    'e', 
                    'v'
                ][logEntry.type]) + '}' + '[' + logEntry.cat + ']', logEntry.msg);
            }
        };
        Console._log = function (cat, type, message) {
            if(Console._showByType[type]) {
                if(Console._showByCategory[cat] || (Console._showAllCategories && !(Console._showByCategory[cat] === false)) || type === 1) {
                    var logEntry = {
                        now: new Date(),
                        type: type,
                        msg: message,
                        cat: cat
                    };
                    if(Console.persist) {
                        Console.persistedLog.push(logEntry);
                    }
                    Console._handleOneEntry(logEntry);
                }
            }
        };
        Console.verbose = function (cat, message) {
            if(arguments.length == 1) {
                Console._log('_default', 2, cat);
            } else {
                Console._log(cat, 2, message);
            }
        };
        Console.log = function (cat, message) {
            if(arguments.length == 1) {
                Console._log('_default', 0, cat);
            } else {
                Console._log(cat, 0, message);
            }
        };
        Console.error = function (cat, message) {
            if(arguments.length == 1) {
                Console._log('_default', 1, cat);
            } else {
                Console._log(cat, 1, message);
            }
        };
        Console.showVerbose = function (value) {
            Console.showType[2] = value;
        };
        Console.showType = function (type, value) {
            Console._showByType[type] = value;
        };
        Console.showCategory = function (cat, value) {
            Console._showByCategory[cat] = value;
        };
        Console.setCategoryDefaultView = function (cat, value) {
            if(!(cat in Console._showByCategory)) {
                Console._showByCategory[cat] = value;
            }
        };
        Console.getShowAllCategories = function () {
            return Console._showAllCategories;
        };
        Console.setShowAllCategories = function (value) {
            Console._showAllCategories = value;
        };
        Console.forceShowAllCatgories = function () {
            Console._showAllCategories = true;
            Console._showByCategory = {
                _default: true
            };
        };
    })(BDA.Console || (BDA.Console = {}));
    var Console = BDA.Console;
})(BDA || (BDA = {}));
