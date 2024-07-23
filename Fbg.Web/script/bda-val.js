/// <reference path="BDA.js" />
/// <reference path="BDA-exception.js" />
/// <reference path="BDA-utils.js" />


(function (obj) {

    var handleFailure = function handleFailure(paramToValidate, message, logMessageOnlyOnError, contextObjOrFunct) {
        // validation failed. log this 
        var roeex = new BDA.Exception(message);
        roeex.data.add('paramToValidate', paramToValidate);

        if (typeof (contextObjOrFunct) === 'function') {
            roeex.data.addContextInfoObj(contextObjOrFunct());
        } else {
            roeex.data.addContextInfoObj(contextObjOrFunct);
        }
        BDA.Console.error(roeex);

        // if log error only, then do not thow error; otherwise, throw it. 
        if (!logMessageOnlyOnError) {
            BDA.latestException = roeex
            throw roeex;
        }
    }



    obj.required = function validate(paramToValidate, message, logMessageOnlyOnError, contextObjOrFunct) {
        ///<summary>same as calling validate(..) where isValidFunction test if param == null</summary>

        return obj.validate(paramToValidate, message, logMessageOnlyOnError,
            function (param) {
                if (param == null) {
                    return false
                } else {
                    return true
                }
            }
         , contextObjOrFunct)
    }


    obj.assertINT = function validate(paramToValidate, message, logMessageOnlyOnError, contextObjOrFunct) {
        ///<summary>same as calling validate(..) where isValidFunction = function (param) { return typeof param === 'number' }</summary>

        return obj.validate(paramToValidate, message, logMessageOnlyOnError, function (param) { return typeof param === 'number' }, contextObjOrFunct)
    }
    obj.assertFUNCT = function validate(paramToValidate, message, logMessageOnlyOnError, contextObjOrFunct) {
        ///<summary>same as calling validate(..) where isValidFunction = function (param) { return typeof param === 'number' }</summary>

        return obj.validate(paramToValidate, message, logMessageOnlyOnError, function (param) { return typeof param === 'function' }, contextObjOrFunct)
    }
    obj.assertDATE = function validate(paramToValidate, message, logMessageOnlyOnError, contextObjOrFunct) {
        ///<summary>same as calling validate(..) where isValidFunction = function (param) { return typeof param === 'number' }</summary>

        return obj.validate(paramToValidate, message, logMessageOnlyOnError, function (param) { return Object.prototype.toString.call(paramToValidate) === '[object Date]' }, contextObjOrFunct)
    }

    obj.validate = function validate(paramToValidate, message, logMessageOnlyOnError, isValidFunction, contextObjOrFunct) {
        ///<summary>validate a parameter and handle failure in a consisten way</summary>
        ///<param name="paramToValidate">the paramter you are vaildating. will be passed in to the validation function</param>
        ///<param name="message">the message you want logged if validation fails</param>
        ///<param name="logMessageOnlyOnError">if true, error is logged but the function returns with FALSE return 
        ///     value and the program may continue. if false, an exception is throw and your code will not continue
        ///</param>
        ///<param name="isValidFunction">function use to validate the param. if (isValidFunction(paramToValidate) === false) then we know the param is not valid</param>
        ///<param name="contextObjOrFunct">additional context name-value type info to add to exception. 
        ///     If object is passed in, its own properties will be added to the exception.data collection. 
        ///     IF function is passed in, it will be called and its resulting object, its own properties will be added to the exception.data collection. 
        /// best to specify a function so that the object is created only if validation fails
        ///</param>

        message = message || "parameter not valid - msg not specified"; // default message is not specified
        isValidFunction = isValidFunction || function (param) { return param || false; }; // default validation function just to prevent failure if not specified
        logMessageOnlyOnError = logMessageOnlyOnError || false;


        // run the validation function to see what is says about the param
        if (!isValidFunction(paramToValidate)) {
            handleFailure(paramToValidate, message, logMessageOnlyOnError, contextObjOrFunct);
            return false;
        }

        return true;
    }


} (window.BDA.Val = window.BDA.Val || {}));