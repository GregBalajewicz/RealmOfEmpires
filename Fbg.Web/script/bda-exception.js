/// <reference path="BDA.js" />
/// <reference path="BDA-exception.js" />
/// <reference path="BDA-utils.js" />

BDA.latestException = {};
BDA.Exception = function (message, errorObject) {
    this.message = message;
    this.innerException = errorObject;
    try {
        this.stackTrace = printStackTrace();
    }   catch (ex2) {
        this.stackTrace = "error while calling printStackTrace"
    }        
    this.data = new BDA.Exception.Data();

}
BDA.Exception.prototype =
{
    message: ''
    , innerException: {}
    , data: {}
}

BDA.Exception.Data = function () { this._data = [] }
BDA.Exception.Data.prototype =
{
    add: function (name, value) {
        ///<summary>adds data. name must be string, value could be string or any other object</summary>
        this._data.push({ name: name, val: value });
    },
    addContextInfoObj: function (obj, namePrefix) {
        ///<summary>appends all properties of the object to data</summary>
        ///<param name="obj">object that typically results from the call to getContextInfo on some object</param>
        ///<param name="namePrefix">if you want to prefix all names found in the obj then specifiy this here. optional. set to "" if not specified</param>
        namePrefix = namePrefix || "";
        var item;
        for (item in obj) {
            if (obj.hasOwnProperty(item)) {
                this.add(namePrefix + item, obj[item]);
            }
        }
    }
}
