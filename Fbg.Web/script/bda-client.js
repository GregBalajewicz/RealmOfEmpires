(function (obj) {
    BDA.Console.setCategoryDefaultView('BDA.Client', false); // by default, do not display the performance category. this assumes BDA.Console is defined BEFORE this file

    // Experimental Desktop Notification Feature

    ///
    /// Notify the user via a local push notification.
    /// On desktop this is a little popup in the bottom-right corner.
    /// Not available yet on other devices.
    ///
    var _pushLocalNotify = function _pushLocalNotify(title, msg, icon) {

        if (ROE.isD2) {// Only on desktop
            // Chrome 
            // and notifications are available           
            if ("Notification" in window) {
                // Do we have permission?                
                if (Notification.permission === "granted") {
                    _desktopNotify(title, msg, icon);
                } else if (Notification.permission !== 'denied') {
                    Notification.requestPermission(function (permission) {
                        // If the user is okay, let's create a notification
                        if (permission === "granted") {
                            console.log("permission granted");
                            _desktopNotify(title, msg, icon);
                        }
                    });
                }
            }
        }
    }

    /// You need to check permission before calling this:
    var _desktopNotify = function _desktopNotify(title, msg, preferredIcon) {
        // Default icon.
        var notifyIcon = "https://static.realmofempires.com/images/D2test/CircleBig.png";
        if (preferredIcon) { notifyIcon = preferredIcon; }

        var notification = new Notification(title, { body: msg, icon: notifyIcon });
    }

    obj.pushLocalNotify = _pushLocalNotify;

}(window.BDA.Client = window.BDA.Client || {}));