/* WebSQL (v0.2) Paul Sayre */
(function (context) {
    var VERSION = '0.2';

    // Public object
    var pub = context.WebSQL = function (name, ver, desc, size, cb) {

        // Open database
        var db = context.openDatabase && context.openDatabase(name, ver || '1.0', desc || name, size || 5e6, cb),

        // Returned object
		ret = db && {
		    // Query the database in a transaction
		    query: function (sqls) {

		        // Query deferred
		        var df = pub.Deferred(),
					queries = isArray(sqls) ? sqls : arguments;

		        // Create transaction for all queries
		        ret.rawTx(function (tx) {
		            var dfSql = pub.Deferred(),
						sql, args, parts,
						i, iLen, j, jLen,
						succ, error = dfSql.reject;

		            // Loop through queries
		            for (i = 0, iLen = queries.length; i < iLen; i++) {
		                sql = queries[i];
		                args = queries[i + 1];

		                // Convert function into SQL
		                if (typeof sql === 'function') {
		                    sql = sql.toString();
		                    sql = sql.substr(sql.indexOf('/*!') + 3);
		                    sql = sql.substr(0, sql.lastIndexOf('*/'));
		                }

		                // Add ? for fields in insert
		                parts = /^\s*(?:INSERT|REPLACE)\s+INTO\s+\w+\s*\(([^\)]+)\)\s*$/i.exec(sql);
		                if (parts && parts[1]) {
		                    sql += ' VALUES (' + (new Array(parts[1].split(',').length)).join('?,') + '?)';
		                }

		                // If query has args
		                if (isArray(args)) {
		                    i += 1;

		                    // If args is actually array of args
		                    if (isArray(args[0])) {
		                        for (j = 0, jLen = args.length; j < jLen; j++) {
		                            if (i + 1 === iLen && j + 1 === jLen) {
		                                succ = dfSql.resolve;
		                            }
		                            tx.executeSql(sql, args[j], succ, error);
		                        }
		                    }

		                        // Run query with args
		                    else {
		                        if (i + 1 === iLen) {
		                            succ = dfSql.resolve;
		                        }

		                        tx.executeSql(sql, args, succ, error);
		                    }
		                }

		                    // Just run the query
		                else {
		                    if (i + 1 === iLen) {
		                        succ = dfSql.resolve;
		                    }
		                    tx.executeSql(sql, [], succ, error);
		                }
		            }

		            // Resolve the last set of results
		            dfSql.fail(df.reject).done(function (tx, res) {
		                var ret = null, i, rows;
		                if (res) {
		                    rows = res.rows;
		                    if (rows) {
		                        ret = [];
		                        for (i = 0; i < rows.length; i++) {
		                            ret[i] = rows.item(i);
		                        }
		                    }
		                    if (ret && ret.length === 0) {
		                        try {
		                            ret.insertId = res.insertId;
		                        } catch (e) {
		                            ret.insertId = null;
		                        }
		                    }
		                    else {
		                        ret.insertId = null;
		                    }
		                }
		                df.resolve(ret);
		            });
		        });

		        // Return a promise for queries
		        return df.promise();
		    },


		    // Runs a transaction manually on database
		    rawTx: function (fn) {
		        db.transaction(fn);
		    },


		    // Returns the names of the tables in the database
		    getTableNames: function () {
		        var df = $.Deferred();

		        ret.query('SELECT tbl_name FROM sqlite_master WHERE type = "table" AND tbl_name NOT REGEXP "^(__|sqlite_).*"')
					.fail(df.reject)
					.done(function (tables) {
					    var i, names = [];
					    for (i = 0; i < tables.length; i++) {
					        names[i] = tables[i].tbl_name;
					    }
					    df.resolve(names);
					});

		        return df.promise();
		    },


		    // Dump the database in various formats
		    dump: function (type, getData) {
		        var dfDump = pub.Deferred();

		        getData = getData !== false; // Defaults to true

		        switch (type) {
		            case 'json':
		            default:

		                ret.query('SELECT * FROM sqlite_master WHERE tbl_name NOT REGEXP "^(__|sqlite_).*" ORDER BY CASE type WHEN "table" THEN 1 WHEN "index" THEN 2 ELSE 3 END')
							.fail(dfDump.reject)
							.done(function (rows) {
							    var tables = {}, dfs = [], row, i;

							    for (i = 0; row = rows[i]; i++) {
							        if (!row.sql) continue;
							        switch (row.type) {

							            // Create table sql  
							            case 'table':
							                tables[row.tbl_name] = {
							                    schema: {
							                        table: row.sql,
							                        indexes: []
							                    }
							                };

							                // Pull data from table
							                if (getData) {
							                    (function (name) {
							                        var df = pub.Deferred();
							                        ret.query('SELECT * FROM ' + name)
														.fail(df.reject)
														.done(function (data) {
														    delete data.insertId;
														    tables[name].data = data;
														    df.resolve();
														});
							                        dfs.push(df);
							                    })(row.tbl_name);
							                }
							                break;

							                // Create index sql  
							            case 'index':
							                tables[row.tbl_name].schema.indexes.push(row.sql);
							                break;
							        }
							    }

							    // Wait for all data queries to come back before
							    pub.when.apply(pub, dfs)
									.fail(dfDump.reject)
									.done(function () {
									    dfDump.resolve(tables);
									});
							});
		                break;

		            case 'sql':
		                ret.dump('json', getData)
							.fail(dfDump.reject)
							.done(function (json) {
							    var sqls = [], table, row, i, field, fields, data, val;

							    for (var name in json) {
							        if (!hasOwn(name, json)) continue;
							        table = json[name];
							        sqls.push(table.schema.table);
							        sqls = sqls.concat(table.schema.indexes);
							        if (table.data && table.data.length > 0) {

							            // Get table fields
							            fields = [];
							            row = table.data[0];
							            for (field in row) {
							                if (!hasOwn(field, row)) continue;
							                fields.push(/\s/.test(field) ? '`' + field + '`' : field);
							            }
							            fields = fields.join(', ');

							            // Get data values
							            data = [];
							            for (i = 0; row = table.data[i]; i++) {
							                data[i] = [];
							                for (field in row) {
							                    if (!hasOwn(field, row)) continue;
							                    if (typeof row[field] === 'number') {
							                        val = row[field];
							                    }
							                    else if (row[field] === null || row[field] === void null) {
							                        val = 'NULL';
							                    }
							                    else {
							                        val = '"' + row[field] + '"';
							                    }
							                    data[i].push(val);
							                }
							                data[i] = '(' + data[i].join(', ') + ')';
							            }

							            // Add query
							            data = data.join(',\n\t');
							            sqls.push('INSERT INTO ' + name + ' (' + fields + ') VALUES\n\t' + data);
							        }
							    }

							    sqls = sqls.join(';\n') + ';';
							    dfDump.resolve(sqls);
							});
		        }

		        return dfDump.promise();
		    }


		};

        // Include the database version for reference
        //ret.version = db.version;
        //ret.changeVersion = db.changeVersion;

        return ret;

    };

    pub.VERSION = VERSION;


    // Test if an argument is an array
    var isArray = Array.isArray || function (arg) {
        return !!(arg && arg.constructor === Array);
    };


    var hasOwn = function (key, obj) {
        return Object.prototype.hasOwnProperty.call(obj, key);
    };


})(this);

(function ($, pub) {
    pub.when = $.when;
    pub.Deferred = $.Deferred;
    $.WebSQL = pub;
})(jQuery, WebSQL);


// code for reseting db when schema changed
$(function () {
    BDA.Console.setCategoryDefaultView('BDA.Database', false);

    // Let's not do this right away -- this is also called in roe-player
    //BDA.Database.ReCreateIfNeededByVersion();

    BDA.Database.Init();
});

BDA.Database = {
    id: function () { return (ROE.realmID < 0 ? ('minus' + Math.abs(ROE.realmID)) : ROE.realmID) + '_' + ROE.playerID; },
    rid: null,
    pid: null,
    db: null,
    dbx: null,
    tableList: [],
    usingIndexedDB: null,
    usingSessionDB: false
    , Init: function () {

        //FORCE INDEX DB
        /*
        BDA.Database.usingIndexedDB = true;
        BDA.Database.usingSessionDB = false;
        return;
        */

        //FORCE SESSION DB
        BDA.Database.usingIndexedDB = false;
        BDA.Database.usingSessionDB = true;
        return;

        //if RX force SessionDB for snappy experience, especially on Map
        if (ROE.rt == "X") {
            BDA.Console.verbose('BDA.Database', "Using Session DB for RX");

            BDA.Database.usingIndexedDB = false;
            BDA.Database.usingSessionDB = true;

        } else if (window.openDatabase && !BDA.Database.usingIndexedDB) {
            BDA.Console.verbose('BDA.Database', "Using webSQL");

            BDA.Database.usingIndexedDB = false;
            BDA.Database.usingSessionDB = false;

            // this test is for safari which in iFrame happily claims window.openDatabase is not null/falsifieable 
            try {
                var test = WebSQL('ROE', '1.0', 'ROE', 5e6); // advance db interface
            } catch (ee) {
                BDA.Database.usingIndexedDB = false;
                BDA.Database.usingSessionDB = true;
            }


        } else if (window.indexedDB) {

            // FF in private mode check 
            // https://github.com/ocombe/angular-localForage/commit/04f55e6f71e543a7ae123a77f42cb396c2ba7d29
            var is_firefox = /firefox/i.test(navigator.userAgent);
            if (is_firefox) {
                BDA.Console.verbose('BDA.Database', "not using indexDB in FF. Using Session");

                BDA.Database.usingIndexedDB = false;
                BDA.Database.usingSessionDB = true;
            } else {
                BDA.Console.verbose('BDA.Database', "using Index DB");

                BDA.Database.usingIndexedDB = true;
                BDA.Database.usingSessionDB = false;
            }
        } else {
            BDA.Console.verbose('BDA.Database', "No WebSql, no indexedDB, using sessionDB ");

            BDA.Database.usingIndexedDB = false;
            BDA.Database.usingSessionDB = true;
        }

    }
    , GetDatabase: function (dbReadyCallback, tableName) {
        // Passing the table name will do a check to see if it exists in the database

        // A callback "dbReadyCallback" is used for indexeddb --> when the db has been opened this will get called,
        // it is at that point when you should do the transactions on the db. The callback MUST be supplied for indexedDB      

        // Assigning locally to ensure scope for nested functions
        var dbcb = dbReadyCallback;

        //var forceIndexedDB = false;
        //if (BetaFeatures && (BetaFeatures.status('UseIndexedDB') == 'ON'))
        //    forceIndexedDB = true;

        // We default to websql
        if (window.openDatabase && !BDA.Database.usingIndexedDB) {

            try {
                // if not created, create 
                // Test if realm id or player id don't match
                var dbexists = false;
                if (BDA.Database.db) dbexists = true;

                // Some debug data.
                var testingStr = "realmID: roe=" + ROE.realmID + " db=" + BDA.Database.rid + ", playerID: roe=" + ROE.playerID + " db=" + BDA.Database.db + ", db var not null?=" + dbexists;
                BDA.Console.verbose('BDA.Database', "GetDatabase initial state: " + testingStr);

                var rebuildNecessary = false;
                var db;

                // Check to see if we have the right rid and pid, and if there is even a db object
                if (ROE.realmID != BDA.Database.rid || ROE.playerID != BDA.Database.pid || !BDA.Database.db) {
                    BDA.Console.verbose('BDA.Database', "Database ids or db var needs refresh: creating tables if necessary.");
                    db = BDA.Database.db = WebSQL('ROE', '1.0', 'ROE', 5e6); // advance db interface

                    // Set the realm and player id internally to this database
                    // we'll use it for checking
                    BDA.Database.rid = ROE.realmID;
                    BDA.Database.pid = ROE.playerID;

                    rebuildNecessary = true;
                } else {
                    db = BDA.Database.db;
                    if (tableName) {
                        if (!BDA.Database.DoesTableExist(tableName)) {
                            BDA.Console.verbose('BDA.Database', "Table " + tableName + " does not exist so flag a rebuild.");
                            rebuildNecessary = true;
                        }
                    }
                }


                if (rebuildNecessary) {
                    // We try and create the tables if they don't exist. We can't tell that they exist or not so we query anyways
                    // ensure the right tables are there.
                    // It could be the case that the IDs have changed but the table exists (i.e. switching to another realm)
                    // so we only do if not exists.
                    db.query(
                        "CREATE TABLE IF NOT EXISTS Villages_" + BDA.Database.id() + " (id unique, name text, created timestamp default (datetime('now')), x number, y number, area number, areacoord text, flag text, friend text, note text, pid number, points number, tags text,claimedStatus number,claimedStatus_otherClan number, mine number, type number)",
                        "CREATE TABLE IF NOT EXISTS VillagesMy_" + BDA.Database.id() + " (id unique, name text, x number, y number, points number, villagetypeid number)",
                        "CREATE TABLE IF NOT EXISTS Landmarks_" + BDA.Database.id() + " (x number, y number, image number, area text)",
                        "CREATE TABLE IF NOT EXISTS Reports_" + BDA.Database.id() + " (id number, subject text, time text, type text, viewed number, forwarded text, url text, flag1 number, flag2 number, flag3 number, whatside text, detailsjson text, folderID number, folderName text)",
                        "CREATE TABLE IF NOT EXISTS Mail_" + BDA.Database.id() + " (id number, subject text, timesent text, sender text, viewed number, folderID number, folderName text)",
                        "CREATE TABLE IF NOT EXISTS SentMail_" + BDA.Database.id() + " (id number, subject text, timesent text, sender text, receiver text, viewed number, folderID number, folderName text)",
                        "SELECT name, type, sql FROM sqlite_master WHERE type in ('table') AND name NOT LIKE '?_?_%' ESCAPE '?'"
                    ).done(function (listOfTablesResult) {

                        // Rebuild table list
                        // We'll use this to check whether a table exists.
                        BDA.Database.tableList = [];
                        for (var i = 0; i < listOfTablesResult.length; i++) {
                            BDA.Database.tableList.push(listOfTablesResult[i].name);
                        }
                        // Was a table name provided to do a check against?
                        if (tableName) {
                            if (!BDA.Database.DoesTableExist(tableName)) {
                                // Should exist at this point so throw an exception
                                var roeex = new BDA.Exception("BDA.Database.GetDatabase tableName doesn't exist!");
                                roeex.data.add('rid', BDA.Database.rid);
                                roeex.data.add('pid', BDA.Database.pid);
                                roeex.data.add('tableName', tableName);
                                roeex.data.add('tableList', BDA.Database.tableList.join(","));
                                BDA.latestException = roeex;
                                BDA.Console.error(roeex);
                                throw roeex;
                            }
                        }

                        // The database was created successfully
                        BDA.Console.verbose('BDA.Database', "Database tables are ready.");
                        BDA.Database.db = db;
                        // The database was created successfully, call callback. Return db object.
                        if (dbcb) {
                            dbcb(BDA.Database.db);
                        }
                    }).fail(function (t, e) {
                        BDA.Console.error('BDA.Database', "Table creation failed.");
                        var roeex = new BDA.Exception("BDA.Database.GetDatabase SQL Fail: " + e.message);
                        roeex.data.add('rid', BDA.Database.rid);
                        roeex.data.add('pid', BDA.Database.pid);
                        BDA.latestException = roeex;
                        BDA.Console.error(roeex);
                        throw roeex;
                    });
                } else {


                    BDA.Console.verbose('BDA.Database', "Database and vars exist and are correct, just do callback (if supplied).");
                    if (dbcb) {
                        dbcb(BDA.Database.db);
                    }
                }

                // Cannot return anything because we don't
                // know if db was created or ready. Use callback instead.

            } catch (e) {
                BDA.Console.error('BDA.Database', 'Error using WebSQL e=' + e);

            }

        } else if ( BDA.Database.IsUsingIndexedDB()) {

            try {
                // If the database is null or the player id or realm id done match with this database object,
                // then we have to create and or open.
                //console.log("Check DB and vars: id = " + this.id() + "  r = " + this.rid + "  p = " + this.pid + "  dbx = " + BDA.Database.dbx);
                if (ROE.realmID != BDA.Database.rid || ROE.playerID != BDA.Database.pid || !BDA.Database.dbx) {
                    //console.log("Open/create the db");

                    BDA.Database.rid = ROE.realmID;
                    BDA.Database.pid = ROE.playerID;
                    var dbName = "ROEDB_" + BDA.Database.id();
                    var dbver = 2;// ROE.localDBVersion || 1; // Get the server db version

                    // First, try and open the database with the given name
                    var vCheckRequest = indexedDB.open(dbName);
                    vCheckRequest.onsuccess = function (e) {
                        var dblocal = e.target.result;
                        var version = dblocal.version;
                        dblocal.close();

                        // If they versions don't match, just destroy and rebuild the 
                        // database (it doesn't matter if its higher or lower)
                        if (dbver != version) {
                            BDA.Console.verbose("BDA.Database", "Local database version out of sync with server. Refreshing...");
                            var req = indexedDB.deleteDatabase(dbName);
                            req.onsuccess = function () {
                                BDA.Console.verbose('BDA.Database', 'Local database successfully removed.');
                                openIndexedDB();
                            };
                            req.onerror = function () {
                                BDA.Console.error('BDA.Database', 'Error trying to delete local database.');
                            };
                            req.onblocked = function () {
                                BDA.Console.error('BDA.Database', 'Cannot delete local database, it is blocked.');
                            };
                        } else {
                            openIndexedDB();
                        }
                    };
                    // Handle errors when opening the datastore.
                    vCheckRequest.onupgradeneeded = function (e) { BDA.Console.verbose('BDA.Database', 'Only gets called if DB does not exist already. Will set to version 1.'); /* Only gets called if DB does not exist already. Will set to version 1. */ };
                    vCheckRequest.onerror = function (e) { BDA.Console.verbose('BDA.Database', 'Probably db doesnt exist'); /* Probably doesnt exist */ };
                    vCheckRequest.onblocked = function (e) { BDA.Console.verbose('BDA.Database', 'Blocked and cant close'); /* Blocked and can't close */ };

                    function openIndexedDB() {
                        BDA.Console.verbose("BDA.Database", "Open connection to local database.");
                        var request = indexedDB.open(dbName, dbver);

                        request.onupgradeneeded = function (e) {
                            BDA.Console.verbose("BDA.Database", "Rebuilding database.");
                            //console.log("Open DB - Upgrade needed");
                            var dbx = e.target.result;
                            dbx.onerror = function (e) { BDA.Console.error('BDA.Database', 'Error in onupgradeneeded e=' + e); };

                            //console.log(dbx);
                            e.target.transaction.onerror = function (e) { BDA.Console.error('BDA.Database', 'Transaction error e=' + e); };

                            // Check for the object store "report"
                            if (dbx.objectStoreNames.contains("Reports")) {
                                //console.log("contains store: " + "Reports");
                                // If we find it, delete it so we can create a fresh new one
                                dbx.deleteObjectStore("Reports");
                            }
                            // Create the object store. this would be like a "table"
                            // keyPath defines the property under which the items will be stored under
                            dbx.createObjectStore("Reports", { keyPath: "id" });

                            if (dbx.objectStoreNames.contains("Villages")) { dbx.deleteObjectStore("Villages"); }
                            var objectStore = dbx.createObjectStore("Villages", { keyPath: "id" });
                            objectStore.createIndex("id", "id", { unique: true });
                            objectStore.createIndex("areacoord", "areacoord", { unique: false });

                            if (dbx.objectStoreNames.contains("VillagesMy")) { dbx.deleteObjectStore("VillagesMy"); }
                            var objectStore = dbx.createObjectStore("VillagesMy", { keyPath: "id" });
                            objectStore.createIndex("id", "id", { unique: true });

                            if (dbx.objectStoreNames.contains("Landmarks")) { dbx.deleteObjectStore("Landmarks"); }
                            var objectStore = dbx.createObjectStore("Landmarks", { keyPath: "id" });
                            objectStore.createIndex("id", "id", { unique: true });
                            objectStore.createIndex("area", "area", { unique: false});

                            if (dbx.objectStoreNames.contains("Mail")) { dbx.deleteObjectStore("Mail"); }
                            dbx.createObjectStore("Mail", { keyPath: "id" });

                            if (dbx.objectStoreNames.contains("SentMail")) { dbx.deleteObjectStore("SentMail"); }
                            dbx.createObjectStore("SentMail", { keyPath: "id" });

                            // We select on the name index for a situation, so lets add that index here.
                            var objStore = e.target.transaction.objectStore('VillagesMy');
                            objStore.createIndex('name', 'name');
                        };

                        // Handle successful datastore access.
                        request.onsuccess = function (e) {
                            BDA.Console.verbose('BDA.Database', 'Database opened successfully.');
                            // Get a reference to the DB.
                            BDA.Database.dbx = e.target.result;

                            if (dbcb)
                                dbcb(BDA.Database.dbx);
                            //df.resolve(BDA.Database.dbx);
                        };

                        // Handle errors when opening the datastore.
                        request.onerror = function (e) { BDA.Console.error('BDA.Database', 'Open IndexedDB Failed e=' + e); };

                        request.onblocked = function (e) { BDA.Console.error('BDA.Database', 'Open IndexedDB Failed - Blocked e=' + e); };
                    }
                } else {
                    if (dbcb)
                        dbcb(BDA.Database.dbx);

                }

                //return df.promise();
            } catch (e) {
                BDA.Console.error('BDA.Database', 'Error using indexedDB e=' + e);
            }

        } else {
            //
            // no local dbs, use session DB instead
            //
            if (self && self.indexedDB)
                BDA.Console.verbose('self has indexedDB');

            BDA.Database.usingIndexedDB = false;
            BDA.Database.usingSessionDB = true;

            BDA.Console.verbose('BDA.Database', 'WebSQL and IndexedDB not supported');
        }
    }
    // Checks if the given table name exists in the cached table list.
    , DoesTableExist: function (tableName) {
        if (BDA.Database.whichDatabaseToUse() == "session") {
            return BDA.SessionDB.DoesTableExist(tableName);
        }
        else {
            return ($.inArray(tableName, BDA.Database.tableList || []) != -1);
        }
    }

    , LocalGet: function (key) {
        return localStorage[key + '_' + BDA.Database.id()];
    },
    LocalGetByPrefix: function (prefix) {
        var res = {};
        var re = new RegExp('^' + prefix + '[a-zA-Z]+_' + BDA.Database.id() + '$');
        for (key in localStorage) {
            if (key.match(re)) {
                res[key] = localStorage[key];
            }
        }
        return res;
    }
    , LocalSet: function (key, value) {
        try {
            localStorage[key + '_' + BDA.Database.id()] = value;
            return value;
        } catch (e) {
            //probably LS was full
            BDA.Database.LocalCleanSelective();
        }
    }

    //we clenase certain LS data because we are hitting the cap issue
    , LocalCleanSelective: function () {

        //to prevent spam that would go into HAMMER clear right away
        if (window.localCleanseActive) {
            return;
        }

        window.localCleanseActive = true;

        //we put a set timeout here to allow whatever set data loop there was to finish executing
        window.setTimeout(function () {

            var cleanseCount = window.localStorage.getItem('localCleansedCount');

            //to prevent a deadlock of reload loop
            //if cleansed once before, try a FULL clenase
            if (cleanseCount && cleanseCount === '1') {
                localStorage.clear();
                window.localStorage.setItem('localCleansedCount', '2');
                alert('Issue with storing data locally. CLEARING LOCAL STORAGE. Restarting.');
                window.location.reload();
                return;
            } else if (cleanseCount && cleanseCount === '2') { //if already super cleansed and still got here...
                alert('Local data storage still failed. Please contact support.');
            }

            window.localStorage.setItem('localCleansedCount', '1');

            for (var key in window.localStorage) {
                if (window.localStorage.hasOwnProperty(key)) {
                    if (key.indexOf('MapLandTypes') > -1 ||
                        key.indexOf('MapLandArea') > -1 ||
                        key.indexOf('MapVillArea') > -1) {
                        localStorage.removeItem(key);
                    }
                }
            }

            alert('Issue with storing data locally. Cleaning up some space. Restarting.');
            window.location.reload();

        }, 500);

    }

    , LocalClear: function () {
        var id = BDA.Database.id();
        for (var i = 0; i < localStorage.length;) {
            var k = localStorage.key(i); if (k.length >= id.length && k.substr(k.length - id.length) == id) { localStorage.removeItem(k); } else { i++; }
        }
    }
    // increments +1
    , LocalInc: function (key) {
        return BDA.Database.LocalSet(key, parseInt(BDA.Database.LocalGet(key)) + 1);
    }
    , LocalDel: function (key) {
        localStorage.removeItem(key + '_' + this.id());
    }
    , ReCreate: function (cb) {

        if (BDA.Database.IsUsingIndexedDB()) {
            // Only do the work once we know we have the db
            BDA.Database.GetDatabase(function (db) {
                //console.log("GetDB callback in recreate");

                // Close database first, before deleting
                db.close(); // returns void

                BDA.Database.db = null; // not used by indexeddb, but clear anyways jic
                BDA.Database.dbx = null;
                BDA.Database.tableList = [];

                // Reference BDA.Database literally, as "this" no longer exists here
                var request = window.indexedDB.deleteDatabase("ROEDB_" + BDA.Database.id());
                request.onsuccess = function () {
                    if (cb) cb();
                };
                request.onblocked = function () {
                    BDA.Console.error('BDA.Database', "Error deleting database: not deleted, it is still open. DBX: ROEDB_" + BDA.Database.id());
                };

                //if (cb) cb();
            });
        } else {
            // WebSQL
            BDA.Database.GetDatabase(function (db) {
                // Whether this database existed or not, it will have
                // been created when this function is called.
                // When a query is called after this, the database will be
                // rebuilt
                BDA.Console.verbose('BDA.Database', "Tables' id to be dropped info " + BDA.Database.id());

                db.query('DROP TABLE Villages_' + BDA.Database.id(),
                         'DROP TABLE Landmarks_' + BDA.Database.id(),
                         'DROP TABLE VillagesMy_' + BDA.Database.id(),
                         'DROP TABLE Reports_' + BDA.Database.id(),
                         'DROP TABLE Mail_' + BDA.Database.id(),
                         'DROP TABLE SentMail_' + BDA.Database.id()).done(function () {
                             BDA.Database.db = null; // Force a recreate.
                             BDA.Database.tableList = []; // clear table list
                             cb();
                         }).fail(function (t, e) {
                             BDA.Console.error('BDA.Database', "WebSQL Error Table Drop. Error: " + e.message);
                         });
            });
        }
    }
    , Delete: function (tablename) {
        BDA.Console.log('BDA.Database', 'Delete tbln:%tb%'.format({ tb: tablename }));

        //console.log("Delete");
        if (BDA.Database.whichDatabaseToUse() == "session") {
            return BDA.SessionDB.Delete(tablename);
        }
        else if (BDA.Database.IsUsingIndexedDB()) {
            // if (true) {
            var df = $.Deferred();
            var promises = [];
            var transaction, objStore, request;

            if (Array.isArray(tablename)) {
                BDA.Database.GetDatabase(function (db) {
                    transaction = db.transaction(tablename, 'readwrite');
                    $.each(tablename, function (i, t) {
                        var tempDf = $.Deferred();
                        objStore = transaction.objectStore(t);
                        request = objStore.clear();
                        request.onsuccess = function () { tempDf.resolve(); }
                        request.onerror = function () { tempDf.reject(); }
                        promises.push(tempDf);
                    });
                });
                return $.when(promises);
            } else {
                BDA.Database.GetDatabase(function (db) {
                    transaction = db.transaction([tablename], 'readwrite');
                    objStore = transaction.objectStore(tablename);
                    request = objStore.clear();
                    request.onsuccess = function () { df.resolve(); }
                    request.onerror = function () { df.reject(); }
                });
                return df.promise();
            }

        } else {
            var qs = [];
            if (Array.isArray(tablename)) {
                $.each(tablename, function (i, t) { qs.push('Delete from ' + t + '_' + BDA.Database.id()) });
            } else {
                qs.push('Delete from ' + tablename + '_' + BDA.Database.id());
            }

            var df = $.Deferred();
            BDA.Database.GetDatabase(function (db) {
                db.query(qs).done(function () {
                    df.resolve();
                }).fail(function () {
                    df.reject();
                });
            });

            return df.promise();
        }
    }
    , DeleteRows: function (tablename, ids) {
        BDA.Console.log('BDA.Database', 'DeleteRows tbln:%tb%, ids:%ids%'.format({ tb: tablename, ids: ids }));


        //  { id: 'id', list: [1,2,3] } 
        //console.log("DeleteRows");
        if (BDA.Database.whichDatabaseToUse() == "session") {
            return BDA.SessionDB.DeleteRows(tablename, ids);
        }
        else if (BDA.Database.whichDatabaseToUse() == "session") {
            return BDA.SessionDB.DeleteRows(tablename, ids);
        }
        else if (BDA.Database.IsUsingIndexedDB()) {
            var df = $.Deferred();
            var db = BDA.Database.GetDatabase();
            BDA.Database.GetDatabase(function (db) {
                var transaction = db.transaction([tablename], 'readwrite');
                var objStore = transaction.objectStore(tablename);
                // var cursorRequest = objStore.openCursor(IDBKeyRange.only(ids.list));
                // Since the key were are looking for isn't the primary, we must get
                // all and then check each for the key given and what its value is
                var cursorRequest = objStore.openCursor();

                transaction.oncomplete = function (e) {
                    // Execute the callback function.
                    df.resolve();
                };

                var key = ids.id;
                var matchesArr = ids.list;

                // This is triggered FOR EACH item returned in the range
                cursorRequest.onsuccess = function (e) {
                    var cursor = e.target.result;
                    if (!!cursor == false) { return; }
                    // Note: indexOf is not available in IE <= 8

                    if ($.inArray(cursor.value[key], matchesArr) != -1)
                        cursor.delete(cursor.primaryKey);

                    // Moves the cursor onto the next item...
                    cursor.continue();
                };

                // again we send the errors to our generic error func
                cursorRequest.onerror = function (e) { BDA.Console.error('BDA.Database', 'IndexedDB DeleteRows cursorRequest e=' + e); };;
            });
            return df.promise();
        } else {

            //return BDA.Database.GetDatabase().query(
            //    'delete from ' + tablename + '_' + BDA.Database.id() +
            //    ' where ' + ids.id + ' in (' + ids.list.join(',') + ')');

            var df = $.Deferred();
            var sql = 'delete from ' + tablename + '_' + BDA.Database.id() + ' where ' + ids.id + ' in (' + ids.list.join(',') + ')';
            BDA.Database.GetDatabase(function (db) {
                db.query([sql]).done(function () {
                    df.resolve();
                }).fail(function () {
                    df.reject();
                });
            }, tablename + '_' + BDA.Database.id());

            return df.promise();
        }
    }
    , Insert: function (tablename, data, cb) {
        if (data.length == 0) { return; }

        BDA.Console.log('BDA.Database', 'Insert tbln:%tb%, data:%data%'.format({ tb: tablename, data: data.len }));

        if (BDA.Database.whichDatabaseToUse() == "session") {
            return BDA.SessionDB.Insert(tablename, data, cb);
        }
        else if (BDA.Database.IsUsingIndexedDB()) {
            // *** Assumed that the primary key is id and that "id" in data
            // is in fact id. At the time of writing this was always the
            // case in the project and the usage of Database.Update() ***

            // Mimics an INSERT OR REPLACE

            //var db = this.GetDatabase();
            BDA.Database.GetDatabase(function (db) {
                // Specify multiple stores for the transaction in the []
                var transaction = db.transaction([tablename], 'readwrite');
                var objStore = transaction.objectStore(tablename);

                var inputArr = data.slice(0); // clone array
                var resultsArr = []; // To be returned upon completion

                // Grab all the id keys we're using
                // These ids need to be the primary keys
                var idkeys = [];
                //for(var d in data) {
                for (var i = 0; i < inputArr.length; i++) {
                    //console.log("Adding key: " + inputArr[i].id);
                    idkeys.push(inputArr[i].id);
                }

                // When everything has finished, this will be called.
                transaction.oncomplete = function (e) {
                    // Execute the callback function. 
                    //console.log("Insert complete");
                    if (cb)
                        cb(resultsArr);
                };


                var cursorRequest = objStore.openCursor();

                cursorRequest.onsuccess = function (event) {
                    var cursor = event.target.result;
                    //console.log("cursor");
                    if (!!cursor == false) {

                        // All done existing, add anything left in inputArr

                        var len = inputArr.length;
                        //console.log(len);

                        //for (var j = 0, item; item = data[j]; j++) {
                        for (var j = 0; j < len && j < 1000; j++) {
                            var o = inputArr[j];

                            //console.log(tablename + ' '+j + ' adding %s', JSON.stringify(o));                          
                            objStore.add(o);

                            resultsArr.push(o);
                        }
                        return;
                    } else {

                        // Gab the current object from the store
                        var record = cursor.value;
                        // Determine if we're updating it or not
                        var index = $.inArray(cursor.primaryKey, idkeys);
                        if (index > -1) {
                            // Manually update each field in the object as replacing
                            // it may delete some existing fields.                            
                            $.each(inputArr[index], function (k, v) {
                                record[k] = v;
                            });
                            //console.log(tablename + ' ' + j + ' updating %s', JSON.stringify(record));
                            objStore.put(record);
                            resultsArr.push(record);
                            // Remove this item from the inputArr so we
                            // don't insert it again. Also remove it from
                            // the keys so both arrays remain in sync
                            inputArr.splice(index, 1);
                            idkeys.splice(index, 1);
                        }
                        // Next object...
                        cursor.continue();
                    }
                };

                // again we send the errors to our generic error func
                cursorRequest.onerror = function (e) { BDA.Console.error('BDA.Database', 'IndexedDB Insert cursorRequest e=' + e); };
            });

        } else {

            function insertNames(arr) {
                var str = ''; for (var i in arr) { str += i + ','; }
                return str.substr(0, str.length - 1);
            }
            function insertQuests(arr) {
                var str = ''; for (var i in arr) { str += '?,'; }
                return str.substr(0, str.length - 1);
            }
            function insertValues(data, end) {
                var ar = []; for (var i in data) { ar.push(data[i]); }; if (end) ar.push(end); return ar;
            }

            var sqls = [];

            for (var i = 0, item; item = data[i]; i++) {
                sqls.push('insert or replace into ' + tablename + '_' + BDA.Database.id() + ' (' + insertNames(item) + ') values (' + insertQuests(item) + ')');
                sqls.push(insertValues(item));
            }

            BDA.Database.GetDatabase(function (db) {
                db.query(sqls).done(cb).fail(function (t, e) {
                    BDA.Console.error('BDA.Database', 'WebSQL Error SQL: ' + sqls + ", Error: " + e.message);
                });
            }, tablename + '_' + BDA.Database.id());

        }
    }
    , Update: function (tablename, data, cb) {
        //console.log("Update");
        // data { id: 'id', fields: [{ 'id': 10, 'viewed': true }] }
        BDA.Console.log('BDA.Database', 'update tbln:%tb%, data:%data%'.format({ tb: tablename, data: data.len }));

        if (!data.id || !data.fields) { return; }

        if (BDA.Database.whichDatabaseToUse() == "session") {
            return BDA.SessionDB.Update(tablename, data, cb);
        }
        else if (BDA.Database.IsUsingIndexedDB()) {
            // *** Assumed that the primary key is id and that "id" in data
            // is in fact id. At the time of writing this was always the
            // case in the project and the usage of Database.Update() ***

            //var db = this.GetDatabase();
            BDA.Database.GetDatabase(function (db) {
                // Specify multiple stores for the transaction in the []
                var transaction = db.transaction([tablename], 'readwrite');
                var objStore = transaction.objectStore(tablename);

                var inputArr = data.fields;
                var resultsArr = []; // To be returned upon completion

                // Grab all the id keys we're using
                // These ids need to be the primary keys
                var idkeys = [];
                for (var i = 0; i < inputArr.length; i++) {
                    idkeys.push(inputArr[i].id);
                }

                // When everything has finished, this will be called.
                transaction.oncomplete = function (e) {
                    // Execute the callback function.
                    //console.log("all done");
                    if (cb)
                        cb(resultsArr);
                };

                var cursorRequest = objStore.openCursor().onsuccess = function (event) {
                    var cursor = event.target.result;

                    if (!!cursor == false) {
                        // Since there are no objects in the store to match
                        // we just return
                        /*for (var j = 0; j < inputArr.length; j++) {
                            //console.log('Inserting %s', JSON.stringify(inputArr[j]));
                            objStore.add(inputArr[j]);
                        }*/
                        return;
                    } else {
                        // Gab the current object from the store
                        var record = cursor.value;
                        // Determine if we're updating it or not
                        var index = $.inArray(cursor.primaryKey, idkeys);
                        if (index > -1) {
                            // Manually update each field in the object as replacing
                            // it may delete some existing fields.
                            //console.log('Updating %s', JSON.stringify(inputArr[index]));
                            $.each(inputArr[index], function (k, v) {
                                record[k] = v;
                            });
                            objStore.put(record);
                            resultsArr.push(record);
                            // Remove this item from the inputArr so we
                            // don't insert it again. Also remove it from
                            // the keys so both arrays remain in sync
                            inputArr.splice(index, 1);
                            idkeys.splice(index, 1);
                        }
                        // Next object...
                        cursor.continue();
                    }
                };

                // again we send the errors to our generic error func
                cursorRequest.onerror = function (e) { BDA.Console.error('BDA.Database', 'IndexedDB Update cursorRequest e=' + e); };
            });
        } else {
            //console.log(data);
            function insertSet(item) {
                var str = '';
                // for (var i in item) { str += i + " = '" + item[i] + "',"; };
                for (var i in item) { str += i + " = ?,"; }; // Don't quote the question ? mark
                return str.substr(0, str.length - 1);
            }

            function insertValues(item) {
                var arr = [];
                for (var i in item) { arr.push(item[i]); }
                return arr;
            }

            var sqls = [];

            for (var i = 0, item; item = data.fields[i]; i++) {
                //sqls.push("update " + tablename + "_" + this.id() + " set " + insertSet(item) + " where " + data.id + "=" + item[data.id]);
                sqls.push("update " + tablename + "_" + BDA.Database.id() + " set " + insertSet(item) +
                          " where " + data.id + "=" + item[data.id]);
                sqls.push(insertValues(item));
            }

            //BDA.Database.GetDatabase().query(sqls).done(cb).fail(function (t, e) { BDA.Console.error('BDA.Database', 'WebSQL Error SQL: ' + sql + ", Error: " + e.message); });

            BDA.Database.GetDatabase(function (db) {
                db.query(sqls).done(cb).fail(function (t, e) {
                    BDA.Console.error('BDA.Database', 'WebSQL Error SQL: ' + sqls + ", Error: " + e.message);
                });
            }, tablename + '_' + BDA.Database.id());
        }
    }
    // same as above but doesnt update id (in case of constraint)
    , UpdateNoId: function (tablename, data, cb) {
        //console.log("UpdateNoId");
        // data { id: 'id', fields: [{ 'id': 10, 'viewed': true }] }
        if (!data.id || !data.fields) { return; }
        BDA.Console.log('BDA.Database', 'UpdateNoId tbln:%tb%, data:%data%'.format({ tb: tablename, data: data }));

        if (BDA.Database.whichDatabaseToUse() == "session") {
            return BDA.SessionDB.Update(tablename, data, cb);
        }
        else if (BDA.Database.IsUsingIndexedDB()) {
            // *** Assumed that the primary key is id and that "id" in data
            // is in fact id. At the time of writing this was always the
            // case in the project and the usage of Database.Update() ***

            //var db = this.GetDatabase();
            BDA.Database.GetDatabase(function (db) {
                // Specify multiple stores for the transaction in the []
                var transaction = db.transaction([tablename], 'readwrite');
                var objStore = transaction.objectStore(tablename);

                var inputArr = data.fields;
                var resultsArr = [];

                // Grab all the id keys we're using
                // These ids need to be the primary keys
                var idkeys = [];
                for (var i = 0; i < inputArr.length; i++) {
                    idkeys.push(inputArr[i].id);
                }

                // When everything has finished, this will be called.
                transaction.oncomplete = function (e) {
                    // Execute the callback function.
                    //console.log("all done");
                };

                var cursorRequest = objStore.openCursor().onsuccess = function (event) {
                    var cursor = event.target.result;

                    if (!!cursor == false) {
                        // Since there are no objects in the store to match
                        // we just return
                        /*for (var j = 0; j < inputArr.length; j++) {
                            //console.log('Inserting %s', JSON.stringify(inputArr[j]));
                            objStore.add(inputArr[j]);
                        }*/
                        return;
                    } else {
                        // Gab the current object from the store
                        var record = cursor.value;
                        // Determine if we're updating it or not
                        var index = $.inArray(cursor.primaryKey, idkeys);
                        if (index > -1) {
                            // Manually update each field in the object as replacing
                            // it may delete some existing fields.
                            //console.log('Updating %s', JSON.stringify(inputArr[index]));
                            $.each(inputArr[index], function (k, v) {
                                if (k != "id")
                                    record[k] = v;
                            });
                            objStore.put(record);
                            resultsArr.push(record);
                            // Remove this item from the inputArr so we
                            // don't insert it again. Also remove it from
                            // the keys so both arrays remain in sync
                            inputArr.splice(index, 1);
                            idkeys.splice(index, 1);
                        }
                        // Next object...
                        cursor.continue();
                    }
                };

                // again we send the errors to our generic error func
                cursorRequest.onerror = function (e) { BDA.Console.error('BDA.Database', 'IndexedDB UpdateNoId cursorRequest e=' + e); };
            });
        } else {


            function insertSetNoId(id, item) {
                var str = '';
                for (var i in item) {

                    if (i != id) {
                        str += i + " = ?,";
                    }
                };
                return str.substr(0, str.length - 1);
            }

            function insertValues(id, item) {
                var arr = [];
                for (var i in item) {
                    if (i != id) {
                        arr.push(item[i]);
                    }
                }
                return arr;
            }

            var sqls = [];

            for (var i = 0, item; item = data.fields[i]; i++) {
                sqls.push('update ' + tablename + '_' + BDA.Database.id() + ' set ' + insertSetNoId(data.id, item) +
                            ' where ' + data.id + '=' + item[data.id]);
                sqls.push(insertValues(data.id, item));
            }

            //BDA.Database.GetDatabase().query(sqls).done(cb).fail(function (t, e) { BDA.Console.error('BDA.Database', 'WebSQL Error SQL: ' + sql + ", Error: " + e.message); });

            BDA.Database.GetDatabase(function (db) {
                db.query(sqls).done(cb).fail(function (t, e) {
                    BDA.Console.error('BDA.Database', 'WebSQL Error SQL: ' + sqls + ", Error: " + e.message);
                });
            }, tablename + '_' + BDA.Database.id());
        }
    }
    /// Match within the content of a field (can be partial)
    /// For matching exact field content, use Find() instead
    , FindByFilter: function (tablename, field, searchTerm, dir) {
        //console.log("FindByFilter");
        // e.g field="My cat is cuddly" therefore if searchTerm="cat" then it would find cat within the field and return that record(s)
        BDA.Console.log('BDA.Database', 'FindByFilter tbln:%tb%, field:%field%, searchTerm:%searchTerm%'.format({ tb: tablename, field: field, searchTerm: searchTerm }));

        if (BDA.Database.whichDatabaseToUse() == "session") {
            return BDA.SessionDB.FindByFilter(tablename, field, searchTerm);
        }
        else if (BDA.Database.IsUsingIndexedDB()) {
            var df = $.Deferred();

            //var db = this.GetDatabase();
            BDA.Database.GetDatabase(function (db) {
                // Specify multiple stores for the transaction in the []
                var transaction = db.transaction([tablename], 'readwrite');
                var objStore = transaction.objectStore(tablename);

                // A cursor is used to iterate through all the items in the range.
                if (!dir)
                    dir = "prev"; // reverse
                var cursorRequest = objStore.openCursor(IDBKeyRange.lowerBound(0), dir);

                // An array to store the items in from the results
                var records = [];

                transaction.oncomplete = function (e) {
                    // Execute the callback function.
                    df.resolve(records);
                };

                // This is triggered FOR EACH item returned in the range
                cursorRequest.onsuccess = function (e) {
                    var cursor = e.target.result;

                    // Check to see if the result contains an item and
                    // if so add it to the array we defined earlier
                    if (!!cursor == false) {
                        return;
                    }

                    var record = cursor.value;
                    if (record[field].indexOf(searchTerm) > -1)
                        records.push(record);

                    // Moves the cursor onto the next item...
                    cursor.continue();
                };

                // again we send the errors to our generic error func
                cursorRequest.onerror = function (e) { BDA.Console.error('BDA.Database', 'IndexedDB FindByFilter cursorRequest e=' + e); df.reject(); };
            });
            return df.promise();

        } else {

            var sqls = [];

            var sql = "select * from " + tablename + '_' + BDA.Database.id() + " where " + field + " like ?";
            sqls.push(sql);
            sqls.push(['%' + searchTerm + '%']);

            //return BDA.Database.GetDatabase().query(sqls).fail(function (t, e) { BDA.Console.error('BDA.Database', 'WebSQL Error SQL: ' + sql + ", Error: " + e.message); });

            var df = $.Deferred();
            BDA.Database.GetDatabase(function (db) {
                db.query(sqls).done(function (r) {
                    df.resolve(r);
                }).fail(function (t, e) {
                    BDA.Console.error('BDA.Database', 'WebSQL Error SQL: ' + sqls + ", Error: " + e.message);
                    df.reject(e);
                });
            }, tablename + '_' + BDA.Database.id());
            return df.promise();

        }

    }
    /// Match field against one or more values
    /// matchValues needs to be an array even if a single value
    ///  (if matchValues = ["~null"] then it looks for non-null of field) Note: -1 is considered NULL
    /// dir - note this is for indexdb only currently. websql does not use the dir key
    ///  (default for dir is "prev" which loads in desc order. other option is "next")
    , Find: function (tablename, field, matchValues, dir) {
        //console.log("Find");
        // matchValues= e.g. [123] ...  ["myContent"] ...  [123, 987, 43] ...  ["what?", "huh?"]
        BDA.Console.log('BDA.Database', 'Find tbln:%tb%, field:%field%, matchValues:%matchValues%,  dir:%dir%'.format({ tb: tablename, field: field, dir: dir }));

        var notNull = (matchValues[0] == "~null");

        if (BDA.Database.whichDatabaseToUse() == "session") {
            return BDA.SessionDB.Find(tablename, field, matchValues);
        }
        else if (BDA.Database.IsUsingIndexedDB()) {
            var df = $.Deferred();

            //var db = this.GetDatabase();
            BDA.Database.GetDatabase(function (db) {

                // Specify multiple stores for the transaction in the []
                var transaction = db.transaction([tablename]);
                var objStore = transaction.objectStore(tablename);


                if (!dir) { dir = "prev"; }// reverse
                // A cursor is used to iterate through all the items in the range.
                var cursorRequest = objStore.openCursor(IDBKeyRange.lowerBound(0), dir);



                // An array to store the items in from the results
                var records = [];

                transaction.oncomplete = function (e) {

                    // Execute the callback function.
                    df.resolve(records);

                };



                // This is triggered FOR EACH item returned in the range
                cursorRequest.onsuccess = function (e) {
                    var cursor = e.target.result;

                    // Check to see if the result contains an item and
                    // if so add it to the array we defined earlier
                    if (!!cursor == false) {
                        return;
                    }

                    var record = cursor.value;
                    if (notNull) {
                        if (record[field] != "null" && record[field] != null && record[field] != -1) {
                            records.push(record);
                        }
                    } else {
                        if ($.inArray(record[field], matchValues) != -1) {
                            records.push(record);
                        }
                    }
                    // Moves the cursor onto the next item...
                    cursor.continue();
                };

                // again we send the errors to our generic error func
                cursorRequest.onerror = function (e) { BDA.Console.error('BDA.Database', 'IndexedDB Find cursorRequest e=' + e); df.reject(); };

            });

            return df.promise();

        } else {


            function getQuestionMarks(arr) {

                var str = '';
                var len = arr.length;
                for (var i = 0; i < len; i++) {
                    str += '?,';
                }

                return str.substr(0, str.length - 1);
            }

            var sqls = [];
            var sql = "";
            if (notNull) { // Only select those that are not null
                sql = "select * from " + tablename + '_' + BDA.Database.id() + " where " + field + " is not null and " + field + " <> -1";
                sqls.push(sql);
            } else {
                sql = "select * from " + tablename + '_' + BDA.Database.id() + " where " + field + " in (" + getQuestionMarks(matchValues) + ")";
                sqls.push(sql);
                sqls.push(matchValues);
            }

            //return BDA.Database.GetDatabase().query(sqls).fail(function (t, e) { BDA.Console.error('BDA.Database', 'WebSQL Error SQL: ' + sql + ", Error: " + e.message); });

            var df = $.Deferred();
            BDA.Database.GetDatabase(function (db) {
                db.query(sqls).done(function (r) {
                    df.resolve(r);
                }).fail(function (t, e) {
                    BDA.Console.error('BDA.Database', 'WebSQL Error SQL: ' + sqls + ", Error: " + e.message);
                    df.reject(e);
                });
            }, tablename + '_' + BDA.Database.id());
            return df.promise();


        }
    }

    //indexxed version of find
    , Find2: function (tablename, field, matchValues, dir) {
        //console.log("Find");
        // matchValues= e.g. [123] ...  ["myContent"] ...  [123, 987, 43] ...  ["what?", "huh?"]
        BDA.Console.log('BDA.Database', 'Find tbln:%tb%, field:%field%, matchValues:%matchValues%,  dir:%dir%'.format({ tb: tablename, field: field, dir: dir }));

        var notNull = (matchValues[0] == "~null");

        if (BDA.Database.whichDatabaseToUse() == "session") {
            return BDA.SessionDB.Find(tablename, field, matchValues);
        }
        else if (BDA.Database.IsUsingIndexedDB()) {
            
            var df = $.Deferred();

            BDA.Database.GetDatabase(function (db) {

                var t0 = performance.now();

                var transaction = db.transaction([tablename]);
                var objStore = transaction.objectStore(tablename);

                //output table row count
                /*
                var count = objStore.count();
                count.onsuccess = function () { console.log(tablename, ' count:', count.result); }
                */
                //debug

                var records = [];
                var index = objStore.index(field);

                if (!dir) { dir = "prev"; }// reverse

                for (var matchValueIndex = 0; matchValueIndex < matchValues.length; matchValueIndex++) {
                    var keyRange = IDBKeyRange.only(matchValues[matchValueIndex]);
                    index.openCursor(keyRange, dir).onsuccess = function (event) {
                        var cursor = event.target.result;
                        //console.log(tablename, field, cursor);
                        if (cursor) {
                            var record = cursor.value;
                            records.push(record);
                            cursor.continue();
                        }
                    };
                }

                transaction.oncomplete = function (e) {
                    var t1 = performance.now();
                    console.log(tablename, field, 'transaction.oncomplete:', (t1 - t0).toFixed(0) + 'ms');
                    df.resolve(records);
                };
                
            });

            return df.promise();

        } else {


            function getQuestionMarks(arr) {

                var str = '';
                var len = arr.length;
                for (var i = 0; i < len; i++) {
                    str += '?,';
                }

                return str.substr(0, str.length - 1);
            }

            var sqls = [];
            var sql = "";
            if (notNull) { // Only select those that are not null
                sql = "select * from " + tablename + '_' + BDA.Database.id() + " where " + field + " is not null and " + field + " <> -1";
                sqls.push(sql);
            } else {
                sql = "select * from " + tablename + '_' + BDA.Database.id() + " where " + field + " in (" + getQuestionMarks(matchValues) + ")";
                sqls.push(sql);
                sqls.push(matchValues);
            }

            //return BDA.Database.GetDatabase().query(sqls).fail(function (t, e) { BDA.Console.error('BDA.Database', 'WebSQL Error SQL: ' + sql + ", Error: " + e.message); });

            var df = $.Deferred();
            BDA.Database.GetDatabase(function (db) {
                db.query(sqls).done(function (r) {
                    df.resolve(r);
                }).fail(function (t, e) {
                    BDA.Console.error('BDA.Database', 'WebSQL Error SQL: ' + sqls + ", Error: " + e.message);
                    df.reject(e);
                });
            }, tablename + '_' + BDA.Database.id());
            return df.promise();


        }
    }

    /// Selects all records from a table, with optional ordering
    /// direction should be lowercase: asc or desc\
    /// For orderByField can only be name for VillagesMy (Because
    /// when using indexeddb we have to set the index in the db create
    , SelectAll: function (tablename, orderByField, direction) {
        //console.log("SelectAll");
        BDA.Console.log('BDA.Database', 'SelectAll tbln:%tb%, orderByField:%orderByField%, direction:%direction%'.format({ tb: tablename, orderByField: orderByField, direction: direction }));

        if (BDA.Database.whichDatabaseToUse() == "session") {
            return BDA.SessionDB.SelectAll(tablename, orderByField, direction);
        }
        else if (BDA.Database.IsUsingIndexedDB()) {
            var df = $.Deferred();

            // var db = this.GetDatabase();
            BDA.Database.GetDatabase(function (db) {
                // Specify multiple stores for the transaction in the []
                var transaction = db.transaction([tablename], 'readwrite');
                var objStore = transaction.objectStore(tablename);

                // A cursor is used to iterate through all the items in the range.

                var cursorRequest;
                if (!direction)
                    direction = "asc";
                var dir = (direction == "asc" ? "next" : "prev");

                if (orderByField && tablename == "VillagesMy") {
                    var index = objStore.index('name');
                    cursorRequest = index.openCursor(IDBKeyRange.lowerBound(0), dir);
                } else {

                    cursorRequest = objStore.openCursor(IDBKeyRange.lowerBound(0), dir);
                }

                // An array to store the items in from the results
                var records = [];

                transaction.oncomplete = function (e) {
                    //console.log("SelectAll complete");
                    // Execute the callback function.

                    if (orderByField && (orderByField == "name")) {
                        records.sort(function (a, b) {
                            var aName = a.name.toLowerCase(),
                                bName = b.name.toLowerCase();
                            if (aName < bName) return -1;
                            if (aName > bName) return 1;
                            return 0;
                        });
                    }
                    df.resolve(records);
                };

                // This is triggered FOR EACH item returned in the range
                cursorRequest.onsuccess = function (e) {
                    var cursor = e.target.result;

                    // Check to see if the result contains an item and
                    // if so add it to the array we defined earlier
                    if (!!cursor == false) {
                        return;
                    }

                    var record = cursor.value;
                    //if (record[field].indexOf(searchTerm) > -1)
                    records.push(record);

                    // Moves the cursor onto the next item...
                    cursor.continue();
                };

                // again we send the errors to our generic error func
                cursorRequest.onerror = function (e) { BDA.Console.error('BDA.Database', 'IndexedDB SelectAll cursorRequest e=' + e); df.reject(); };
            });
            return df.promise();

        } else {

            var sql = "select * from " + tablename + '_' + BDA.Database.id();
            if (orderByField) {
                sql += " order by " + orderByField + " COLLATE NOCASE";
                if (direction)
                    sql += " " + direction;
            }

            var df = $.Deferred();
            BDA.Database.GetDatabase(function (db) {
                db.query([sql]).done(function (r) {
                    df.resolve(r);
                }).fail(function (t, e) {

                    var DB_null = true;
                    if (db) {
                        DB_null = false;
                    }
                    var bdaDB_null = true;
                    if (BDA.Database.db) {
                        bdaDB_null = false;
                    }

                    var roeex = new BDA.Exception("BDA.Database.SelectAll SQL Fail: " + e.message);
                    var debugTableList = [];
                    roeex.data.add('sql', sql);
                    roeex.data.add('BDA.Database.rid', BDA.Database.rid);
                    roeex.data.add('BDA.Database.pid', BDA.Database.pid);
                    roeex.data.add('DB_null', DB_null);
                    roeex.data.add('bdaDB_null', bdaDB_null);

                    if (db) {
                        db.query(["SELECT name, type, sql FROM sqlite_master WHERE type in ('table') AND name NOT LIKE '?_?_%' ESCAPE '?'"]).done(function (r) {

                            for (var i = 0; i < r.length; i++) {
                                debugTableList.push(r[i].name);
                            }

                            roeex.data.add('tableList', debugTableList.join(","));
                            BDA.latestException = roeex;
                            BDA.Console.error(roeex);
                            throw roeex;

                        }).fail(function (t, e2) {
                            roeex.data.add('tableQueryFailed', e2.message);
                            BDA.latestException = roeex;
                            BDA.Console.error(roeex);
                            throw roeex;
                        });
                    } else {
                        roeex.data.add('noTableQueryDBNull', 'true');
                        BDA.latestException = roeex;
                        BDA.Console.error(roeex);
                        throw roeex;
                    }




                    df.reject(e);
                });
            }, tablename + '_' + BDA.Database.id());
            return df.promise();
        }
    }

    /// Selects based on field filter values.
    /// fields length and filters length must be equal as they are treated like keys and values
    /// respectively. e.g. fields[1] corresponds to filters[1]
    /// Each filter is treated as an "AND" so all fields must match their corresponding filters. 
    /// TODO: Test this function is working as expected on both IndexedDB and WebSQL.
    , SelectWithFilters: function (tablename, fields, filters) {
        // Drop a little warning incase someone tries to use this feature -- its not fully tested yet.
        console.log("BDA.Database.SelectWithFilters() is in beta and needs to be tested. Use at own risk.");
        var dir = "prev";
        if (BDA.Database.IsUsingIndexedDB()) {
            /// TODO: Get the indexeddb doing the same thing as WebSQL
            var df = $.Deferred();

            //var db = this.GetDatabase();
            BDA.Database.GetDatabase(function (db) {
                // Specify multiple stores for the transaction in the []
                var transaction = db.transaction([tablename], 'readwrite');
                var objStore = transaction.objectStore(tablename);

                // A cursor is used to iterate through all the items in the range.
                if (!dir)
                    dir = "prev"; // reverse 
                var cursorRequest = objStore.openCursor(IDBKeyRange.lowerBound(0), dir);

                // An array to store the items in from the results
                var records = [];

                transaction.oncomplete = function (e) {
                    // Execute the callback function.
                    df.resolve(records);
                };
                var fieldslen = fields.length;
                // This is triggered FOR EACH item returned in the range
                cursorRequest.onsuccess = function (e) {
                    var cursor = e.target.result;

                    // Check to see if the result contains an item and
                    // if so add it to the array we defined earlier
                    if (!!cursor == false) {
                        return;
                    }
                    // TODO: Verify this works in Firefox.
                    var record = cursor.value;
                    var addRecord = false;
                    // Cycle through all the fields...
                    for (var i = 0; i < fieldslen; i++) {

                        var field = fields[i];
                        if (filter[i] == "~null") {
                            // Handle looking for non null case.
                            if (record[field] != "null" && record[field] != null && record[field] != -1) {
                                addRecord = true;
                            } else {
                                addRecord = false;
                            }
                        } else if (filters[i].indexOf("%%") != -1) {
                            // Handle looking for LIKE case.
                            if (record[field].indexOf(filters[i].substr(2)) != -1) {
                                addRecord = true;
                            } else {
                                addRecord = false;
                            }
                        } else if (record[field] == filters[i]) {
                            // Handle straight up matching case.
                            addRecord = true;
                        } else {
                            addRecord = false;
                        }
                    }

                    if (addRecord) {
                        records.push(record);
                    }

                    // Moves the cursor onto the next item...
                    cursor.continue();
                };

                // again we send the errors to our generic error func
                cursorRequest.onerror = function (e) { BDA.Console.error('BDA.Database', 'IndexedDB SelectWithFilter cursorRequest e=' + e); df.reject(); };
            });
            return df.promise();

        } else {

            // setup the SQL so all the fields are good.
            var sqlfilter = [];
            var len = filters.length;
            var likeIndex = -1;
            for (var i = 0; i < len; i++) {
                if (filters[i] == "~null") {
                    //" + field + " is not null and " + field + " <> -1";
                    // Checking for non-null values of this field.
                    sqlfilter.push(fields[i] + " is not null and " + fields[i] + " <> -1");
                    filters.splice(i, 1); // don't need the filter value.
                } else if (filters[i].indexOf("%%") != -1) {
                    // If found double %%, then the user wants something "like"
                    // TODO: Test to confirm this works...
                    sqlfilter.push(fields[i] + " like ?");
                    filters[i] = "%" + filters[i].substr(2) + "%";
                } else {
                    sqlfilter.push(fields[i] + " = ?");
                }
                console.log(sqlfilter[i]);
                console.log(filters[i]);
            }

            var sqls = [];
            var sql = "select * from " + tablename + '_' + BDA.Database.id();
            if (fields.length > 0) {
                sql += " where " + sqlfilter.join(" and ");
            }
            /* if (orderByField) {
                 sql += " order by " + orderByField + " COLLATE NOCASE";
                 if (direction)
                     sql += " " + direction;
             }*/
            console.log(sql);
            sqls.push(sql);
            if (filters.length > 0)
                sqls.push(filters);

            var df = $.Deferred();
            BDA.Database.GetDatabase(function (db) {
                db.query(sqls).done(function (r) {
                    df.resolve(r);
                }).fail(function (t, e) {
                    var roeex = new BDA.Exception("BDA.Database.SelectAll SQL Fail: " + e.message);
                    roeex.data.add('sql', sql);
                    roeex.data.add('rid', BDA.Database.rid);
                    roeex.data.add('pid', BDA.Database.pid);
                    BDA.latestException = roeex;
                    BDA.Console.error(roeex);
                    throw roeex;
                    df.reject(e);
                });
            }, tablename + '_' + BDA.Database.id());
            return df.promise();
        }
    }

    /// Get a set of records starting at the offset
    , FindRange: function (tablename, quantity, offset, dir) {
        //console.log("FindRange");
        // first record = offset + 1
        // e.g. quantity = 10, offset = 10 therefore you'd get records 11 - 20
        BDA.Console.log('BDA.Database', 'FindRange tbln:%tb%, quantity:%quantity%, offset:%offset%'.format({ tb: tablename, quantity: quantity, offset: offset }));


        if (BDA.Database.whichDatabaseToUse() == "session") {
            return BDA.SessionDB.FindRange(tablename, quantity, offset, dir);
        }
        else if (BDA.Database.IsUsingIndexedDB()) {
            var df = $.Deferred();

            //var db = this.GetDatabase();
            BDA.Database.GetDatabase(function (db) {

                // console.log("quantity = " + quantity + "  offset = " + offset);
                //console.log(tablename + " " + quantity + " " + this.id());
                // console.log(tablename + " " + quantity);
                // Specify multiple stores for the transaction in the []
                var transaction = db.transaction([tablename], 'readwrite');
                var objStore = transaction.objectStore(tablename);

                // A cursor is used to iterate through all the items in the range.
                //var cursorRequest = objStore.openCursor(IDBKeyRange.bound(offset, quantity));           
                // ^^^^^ WRONG !!!! bound is the actual key 1... 4, but problematic if the
                // objects are 1, 3, 10, 11, 12, 56 for example where there is gaps.

                if (!dir)
                    dir = "prev"; // reverse 
                var cursorRequest = objStore.openCursor(IDBKeyRange.lowerBound(0), dir);

                // An array to store the items in from the results
                var records = [];

                transaction.oncomplete = function (e) {
                    //console.log("FindRange complete");
                    // Execute the callback function.
                    df.resolve(records);
                };

                // This is triggered FOR EACH item returned in the range
                var advance = true;
                if (offset == 0)
                    advance = false;
                var count = 0;
                cursorRequest.onsuccess = function (e) {
                    var cursor = e.target.result;
                    //console.log(cursor);
                    //console.log("count = " + count);
                    /// console.log(cursor);
                    // Check to see if the result contains an item and
                    // if so add it to the array we defined earlier
                    if (!!cursor == false) {
                        return;
                    } else if (advance) {
                        // console.log("advance = " + offset);
                        // calling advance moves the cursor HOWEVER
                        // the cursor is still at the current item, not
                        // the advanced item. DO NOT call continue. Instead,
                        // let the function finish, no return. It will
                        // call onsuccess again with the advanced cursor position.
                        cursor.advance(offset);
                        //console.log(cursor);
                        advance = false;
                    } else {
                        //console.log("regular push and continue");
                        //console.log(cursor.source);
                        records.push(cursor.value);

                        count++;
                        if (count == quantity) {

                            return;
                        }
                        cursor.continue();
                    }
                    // Moves the cursor onto the next item...
                    //cursor.continue();
                };

                // again we send the errors to our generic error func
                cursorRequest.onerror = function (e) { BDA.Console.error('BDA.Database', 'IndexedDB FindRange cursorRequest e=' + e); df.reject(); };
            });
            return df.promise();
        } else {

            var sql = "select * from " + tablename + '_' + BDA.Database.id() + " limit " + quantity + " offset " + offset;
            //return BDA.Database.GetDatabase().query([sql]).fail(function (t, e) { BDA.Console.error('BDA.Database', 'WebSQL Error SQL: ' + sql + ", Error: " + e.message); });

            var df = $.Deferred();
            BDA.Database.GetDatabase(function (db) {
                db.query([sql]).done(function (r) {
                    df.resolve(r);
                }).fail(function (t, e) {
                    BDA.Console.error('BDA.Database', 'WebSQL Error SQL: ' + sql + ", Error: " + e.message);
                    df.reject(e);
                });
            }, tablename + '_' + BDA.Database.id());
            return df.promise();
        }
    }

    // This method needs to be async because recreate is async so calling
    // recreate may not finish immeditely even though this function exits.
    // Therefore we make it deferred and the caller needs to handle done().
    // Returns true if recreate needed (i.e. the db has been cleared due to version inconsistencies).
    // The DB tables are dropped if true. The next call to GetDatabase will actually build it.
    , ReCreateIfNeededByVersion: function ReCreateIfNeededByVersion() {

        var df = $.Deferred();
        // When a change is made to the Database schema, increment
        // this version number:
        ROE.localDBVersion = ROE.localDBVersion || 1;
        if (!BDA.Database.LocalGet('Version') || BDA.Database.LocalGet('Version') != ROE.localDBVersion) {

            // Version does not exist so create it
            // and assume db out of date.
            BDA.Database.ReCreate(function () {
                BDA.Console.verbose('BDA.Database', "Database tables have been dropped. Next call will created them.");
                // Since it may take a moment to rebuild, may have to wait till callback before any further db calls.

                BDA.Database.LocalClear();
                BDA.Database.LocalSet('Version', ROE.localDBVersion);

                BDA.Console.verbose('BDA.Database', "Versions are different, recreating database.");
                BDA.Console.verbose('BDA.Database', "Looking for tables for id " + BDA.Database.id());
                df.resolve(true);
            });

        } else {
            BDA.Console.verbose('BDA.Database', "No recreate needed.");
            df.resolve(false);
        }

        return df.promise();
    }



    , whichDatabaseToUse: function whichDatabaseToUse() {

        if (BDA.Database.usingSessionDB == true) {
            return "session";
        } else {
            if (BDA.Database.IsUsingIndexedDB()) {
                return "indexdb";
            } else {
                return "websql";
            }
        }

        return BDA.Database.usingIndexedDB;
    }

    , IsUsingIndexedDB: function IsUsingIndexedDB() {
        /* better code is not in "init"
        if (BDA.Database.usingIndexedDB == null) {
            if (window.openDatabase) {
                BDA.Database.usingIndexedDB = false;
            } else if (window.indexedDB) {
                BDA.Database.usingIndexedDB = true;
            } else {
                BDA.Console.error('BDA.Database', "Could not determine database type.");
            }
        }*/

        return BDA.Database.usingIndexedDB;
    }

    , QueryDebug: function QueryDebug(sql) {

        BDA.Database.GetDatabase(function (db) {
            db.query([sql]).done(function (r) {
                console.log(r);
            }).fail(function (t, e) {
                console.log(e.message);
            });
        });
    }


}


