(function (obj) {
    BDA.Console.setCategoryDefaultView('BDA.Broadcast', false); // by default, do not display the performance category. this assumes BDA.Console is defined BEFORE this file

    var CONST = {
        // Prefix used for class
        cssPrefix: "EVENT-"
    };

    // Manage Events //

    /// Subscribe to an Event
    ///     jQueryContainer - jquery object, the DOM elem to add the binding to
    ///     eventName - string, the name of the event
    ///     callback - the function that gets called when event published. receives
    ///         jquery event obj and data (if available)
    ///
    /// Usage:
    /// BDA.Broadcast.subscribe(jQueryObject, "EventName", _myHandler);
    ///
    /// Example 1: callback func
    /// function _myHandler(event) {  }
    ///
    /// Example 2: callback func with data
    /// function _myHandler(event, data) {
    ///     console.log(data[2]);
    /// }
    ///
    var _subscribe = function _subscribe(jQueryContainer, eventName, callback) {
        BDA.Console.verbose('BDA.Broadcast', "subscribe eventName:%en%".format({ en: eventName}));
        // Event name cannot have spaces or other weird characters
        // as it needs to also be used as a css class name.
        if (/^ *$/.test(eventName)) {
            BDA.Console.error('BDA.Broadcast', "cannot subscribe, invalid eventName:%en%".format({ en: eventName }));
            return;
        }
        // We only bind if class doesn't exist.
        if (!jQueryContainer.hasClass(_getCssClass(eventName))) {
            jQueryContainer.addClass(_getCssClass(eventName));
            jQueryContainer.bind(eventName,

                function genericEventHandler(event, data) {
                    // we stop propagation of this event, then call the call back with just data
                    event.stopPropagation();
                    callback(data);
                }
            );
        } // else don't bind cause already exists (or should exist)
        else {
            BDA.Console.log('BDA.Broadcast', "cannot subscribe - object already has class for eventName:%en%".format({ en: eventName }));
        }
    }
    
    /// Send out the event to all listeners
    ///     eventName - string, the name of the event
    ///     data (optional) - send data as a payload, can be non-array if numeric 
    ///         or string, or an array of params.    
    ///
    /// Usage 1:
    /// BDA.Broadcast.publish("NewReports");
    ///
    /// Usage 2:
    /// BDA.Broadcast.publish("NewReports", myDataVariable);
    ///
    /// Usage 3:
    /// BDA.Broadcast.publish("NewReports", [variable1, variable]);
    ///
    var _publish = function _publish(eventName, data) {
        BDA.Console.verbose('BDA.Broadcast', "publish eventName:%en%, data:%d%".format({ en: eventName, d: data }));
        if (typeof (data) !== "undefined" && data !== null) {
            $(_getCssSelector(eventName)).trigger(eventName, data);
        } else {
            $(_getCssSelector(eventName)).trigger(eventName);
        }
    }

    /// Unsubscribe to an event
    ///     jQueryContainer - jquery object, the DOM elem to remove the binding from
    ///     eventName - string, the name of the event
    ///     callback - the function name that was getting called when event published
    var _unsubscribe = function _unsubscribe(jQueryContainer, eventName, callback) {
        jQueryContainer.unbind(eventName, callback).removeClass(_getCssClass(eventName));
    }
    
    /// Remove All subscribers for the specific event
    ///     eventName - string, the name of the event
    var _unsubscribeAll = function _unsubscribeAll(eventName) {
        $(_getCssSelector(eventName)).unbind().removeClass(_getCssClass(eventName));
    }
    
    // Helper Functions //

    /// Returns the formatted css class
    ///     eventName - string, the name of the event
    ///
    /// Also exposed externally so that the css class being used for this
    /// event can be retrieved if necessary. Should be used for read-only.
    ///
    /// WARNING: Removing the class from the element without unbinding
    /// may cause problems. Always use unsubscribe to remove an event.
    var _getCssClass = function _getCssClass(eventName) {
        return CONST.cssPrefix + eventName;
    }

    /// Returns the formatted css class for jquery selector
    ///     eventName - string, the name of the event
    var _getCssSelector = function _getCssSelector(eventName) {
        return "." + CONST.cssPrefix + eventName;
    }
    
    obj.subscribe   = _subscribe;
    obj.unsubscribe = _unsubscribe;
    obj.publish     = _publish;
    obj.cssClass    = _getCssClass;  

}(window.BDA.Broadcast = window.BDA.Broadcast || {}));