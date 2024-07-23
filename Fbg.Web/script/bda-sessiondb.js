
(function (obj) {

    BDA.Console.setCategoryDefaultView('BDA.SessionDB', false);
  
    var tables = {};
    var _selectAll = function (tablename, orderByField, direction) {
        BDA.Console.log('BDA.SessionDB', '_selectAll tbln:%tb%, orderByField:%orderByField%'.format({ tb: tablename, orderByField: orderByField }));

        var table = tables[_getTableName(tablename)];
        if (!table) {
            table = [];
        }

        var returnObj = {
            done: function (callback) {

                _done_helper(callback, table);
                //setTimeout(
                //    function () {
                //        if (callback) {
                //            callback(table)
                //        }
                //    }, 1);
            },
            fail: function () { }
        };

        return returnObj;
    }
    var _doesTableExist = function (tableName) {
        return tables[tableName] != null;
    }

    var _delete = function (tablename) {
        BDA.Console.log('BDA.SessionDB', '_delete tbln:%tb%'.format({ tb: tablename}));

        delete tables[_getTableName(tablename)];

        var returnObj = {
            done: function (callback) {
                _done_helper(callback);
                //setTimeout(
                //    function () {
                //        if (callback) {
                //            callback()
                //        }
                //    }, 1);
            },
            fail: function () { }
        };

        return returnObj;
    }

    var _insert = function (tablename, data, cb) {
        var table = _getTableOrCreateIfNotFound(tablename);
        var existingRow;
        var oneData;
        var idColumn = "id";
        BDA.Console.log('BDA.SessionDB', '_insert tbln:%tb%, data:%data%'.format({ tb: tablename, data: data.len }));

        // does a sort of INSERT OR UPDATE, on a hard-coded column "id" 

        //
        // update existing rows
        for (var j = 0, oneData; oneData = data[j]; ++j) {
            existingRow = _findRowInTable(table, idColumn, oneData[idColumn]);
            if (existingRow) {
                table[existingRow.rowIndex] = oneData;
                data[j] = null;
            }
        }

        // now add new rows
        for (var j = 0, oneData; oneData = data[j]; ++j) {
            if (oneData != null) {
                table.push(oneData)
            }
        }


        setTimeout(
          function () {
              if (cb) {
                  cb();
              }
          }, 1);
        return;
    }

    var _update = function (tablename, data, cb) {
        var table = tables[_getTableName(tablename)];

        if (data.fields.length > 1) {
            throw new Error("not implemented for more than one field");
        }

        var existingRow;
        var oneData;
        var idColumn = "id";
        BDA.Console.log('BDA.SessionDB', '_update tbln:%tb%, data:%data%'.format({ tb: tablename, data: data.len }));



        //hacky, we do something special for reports
        if (tablename == "Reports") {
            //(id, subject, time, type, viewed, forwarded, url, flag1, flag2, flag3, whatside, detailsjson, folderID, folderName)

            for (var j = 0, record; record = table[j]; ++j) {
                if (record[data.id] == data.fields[0][data.id]) {
                    table[j].viewed = data.fields[0].viewed;
                    var detailsjson = data.fields[0].detailsjson;
                    if (detailsjson) {
                        table[j].detailsjson = detailsjson;
                    }

                    break;
                }
            }

        } else {

            try {
                for (var j = 0, record; record = table[j]; ++j) {
                    if (record[data.id] == data.fields[0][data.id]) {
                        table[j] = data.fields[0];
                    }
                }

            } catch (e) {

                var roeex = new BDA.Exception("error in SessionDB._update", e);
                roeex.data.add('tablename', tablename);
                roeex.data.add('data', data);
                roeex.data.add('cb', cb);
                BDA.latestException = roeex;
                throw roeex;
            }

        }

        setTimeout(
          function () {
              if (cb) {
                  cb();
              }
          }, 1);
        return;
    }

        /// Match field against one or more values
    /// matchValues needs to be an array even if a single value
    ///  (if matchValues = ["~null"] then it looks for non-null of field) Note: -1 is considered NULL
    /// dir - note this is for indexdb only currently. websql does not use the dir key
    ///  (default for dir is "prev" which loads in desc order. other option is "next")
    var _find = function (tablename, field, matchValues, dir) {
        BDA.Console.log('BDA.SessionDB', 'Find tbln:%tb%, field:%field%, matchValues:%matchValues%,  dir:%dir%'.format({ tb: tablename, field: field, dir: dir }));
        
        var tempResults;
       
        var records = [];
        var record;
        var table = tables[_getTableName(tablename)];

        if (table) {
            if (matchValues[0] == "~null") {
                for (var j = 0; record = table[j]; ++j) {
                    if (record[field] != "null" && record[field] != null && record[field] != -1) {
                        records.push(record);
                    }
                }
            } else {
                for (var j = 0; record = table[j]; ++j) {
                    if ($.inArray(record[field], matchValues) != -1) {
                        records.push(record);
                    }
                }
            }
        }
        else  {
            table = [];
        }
        
        var returnObj = {

            done: function (callback) {
                _done_helper(callback, records);

                //this prevents an error (case30237) when chaining (...).done(...).fail(...)                
                var chainDummy = {
                    fail: function (callback) {
                        //however we may have to think a bit about actual fail callbacks
                        //right now because of chaining, this function would be called regardless of actual fail or not.
                    }
                }
                return chainDummy;

            },
            fail: function () { }
        }; 

        return returnObj;
    }

    var _findByFilter = function (tablename, field, searchTerm) {
        BDA.Console.log('BDA.SessionDB', '_findByFilter tbln:%tb%, field:%field%, searchTerm:%searchTerm%'.format({ tb: tablename, field: field, searchTerm: searchTerm }));

        var tempResults;

        var records = [];
        var record;
        var table = tables[_getTableName(tablename)];

        if (table) {

            for (var j = 0, oneData; record = table[j]; ++j) {
                if(record[field].search(new RegExp(searchTerm, "i")) > -1){ //faster and case insensitive search
                    records.push(record);
                }
            }
        }
        else {
            table = [];
        }

        var returnObj = {

            done: function (callback) {
                _done_helper(callback, records);

                //setTimeout(
                //    function () {
                //        callback(records)
                //    }, 1);
            },
            fail: function () { }
        };

        return returnObj;
    }

    var _findRange = function (tablename, quantity, offset, dir) {
        BDA.Console.log('BDA.SessionDB', 'FindRange tbln:%tb%, quantity:%quantity%, offset:%offset%'.format({ tb: tablename, quantity: quantity, offset: offset }));

        var table = tables[_getTableName(tablename)];
        if (table) {
            table = table.slice(offset, offset + quantity);
        }
        else {
            table = [];
        }

        var returnObj = {
            done: function (callback) {
                _done_helper(callback, table);

                //setTimeout(
                //    function () {
                //        callback(table)
                //    }, 1);
            },
            fail: function () { }
        };

        return returnObj;
    }

    // some limitations - only delete on interger rows is allowed. so it is assumed that column pointed to by ids.id is of type in, and ids.list are all ints
    // not very efficient implementation
    var _deleteRows = function (tablename, ids) {
        
        var tempResults;

        var records = [];
        var record;
        var table = tables[_getTableName(tablename)];

        if (table) {
            // delete all rows
            for (var j = 0, record; record = table[j]; ++j) {
                if ($.inArray(parseInt(record[ids.id],10), ids.list) != -1) {
                    table[j] = null
                }
            }

            //now remove deleted rows from array
            var indexOfUndefined;
            indexOfUndefined = table.indexOf(null);
            while (indexOfUndefined != -1) {
                table.remove(indexOfUndefined);
                indexOfUndefined = table.indexOf(null);
            }
            

        }

        var returnObj = {
            done: function (callback) {
                _done_helper(callback);
            },
            fail: function () { }
        };

        return returnObj;
    }
    
    var _getTableOrCreateIfNotFound = function _getTableOrCreateIfNotFound(tablename) {
        var table = tables[_getTableName(tablename)];
        if (!table) {
            tables[_getTableName(tablename)] = [];
            table = tables[_getTableName(tablename)]
        }
        return table;
    }

    var _getTableName = function (tableName) {
        return tableName + "_" + BDA.Database.id();
    }

    var _findRowInTable = function _findRowInTable(table, columnToSearch, valueToFind) {
        var row;
        for (var j = 0, row; row = table[j]; ++j) {
            if (!row[columnToSearch]) {
                return null; // no such column/property
            }
            if (row[columnToSearch] == valueToFind) {
                return { rowIndex: j, row: row };
            }
        }
        return null;
    }

    var _done_helper = function (callback, data) {
        setTimeout(
                    function () {
                        if (callback) {
                            callback(data)
                        }
                    }, 1);

    }


    obj.SelectAll = _selectAll;
    obj.Insert = _insert;
    obj.Find = _find;
    obj.FindRange = _findRange;
    obj.Delete = _delete;
    obj.DeleteRows = _deleteRows;
    obj.Update = _update;
    obj.FindByFilter = _findByFilter;
    obj.DoesTableExist = _doesTableExist;

}(window.BDA.SessionDB = window.ROE.SessionDB || {}));



