var windowReloading = false;
// was in countdown.js
function windowReload() {
    if (windowReloading)
        return;
    else
        windowReloading = true;

    url = window.location.toString();
    if (url.match(/\?/)) {
        window.location = window.location.toString() + "&refresh=1";
    } else {
        window.location = window.location.toString() + "?refresh=1";
    }
}

// was in countdown.js
function padDigits(num) {
    ///<summary> REPRECIATED. use ROE.Utils.padDigits();</summary>
    return ROE.Utils.padDigits();
}
