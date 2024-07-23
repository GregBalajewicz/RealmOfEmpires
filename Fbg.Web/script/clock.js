var Clocks = new Array();
var lowestNotification = null;
var Months = { 'Jan' : 0,
               'Feb' : 1,
               'Mar' : 2,
               'Apr' : 3,
               'May' : 4,
               'Jun' : 5,
               'Jul' : 6,
               'Aug' : 7,
               'Sep' : 8,
               'Oct' : 9,
               'Nov' : 10,
               'Dec' : 11 };

var coins = {
  clockStart : null,
  Start : 0,
  Now : 0,
  Limit : 0,
  Production : 0,
  PreSecondProduction : 0,
  Count : true };


var lblCoins;
var lblProd;
var lblTres;

function UpdateStartingCoins()
{
    coins.clockStart = new Date();
    coins.Now = 0;
    
    //$("span[id$='_lblCoins']").length > 0)
    lblCoins = $('span[id$=ctrlVillageHeaderInfo_lblCoins]');
    lblProd = $('span[id$=ctrlVillageHeaderInfo_lblProduction]');
    lblTres = $('span[id$=ctrlVillageHeaderInfo_lblTrasury]');
    
    if ( (lblCoins.length > 0) && (lblCoins.length > 0)) {
        // if we can find this it means we're on a page with coins displayed
        // So we should enable the realtime silver counter.
        
        // Find our current coins, and our limit
        coins.Start = coins.Now = parseInt(lblCoins.text().replace(/,/g, ''), 10);
        coins.Limit = parseInt(lblTres.text().replace(/,/g, ''), 10);
        
        // Find our hourly productio
        prod = lblProd.text().split("/", 2);
        coins.Production = parseInt(prod[0].replace(/,/g, ''));
        coins.PreSecondProduction = (coins.Production / 60) / 60;
        
        //DumperAlert(coins);
        
        // this is in case treasury was full before
        if (!coins.Count) {
            coins.Count = true;
            lblCoins.removeClass('silverOverflow');
            lblTres.removeClass('silverOverflow');
        }
    }
}

function Tick ()
{
    $(Clocks).each(
        function ()
        {
            var now = new Date();
            
            var midnight = new Date();
            midnight.setUTCHours(23);
            midnight.setUTCMinutes(59);
            midnight.setUTCSeconds(59);
            
            var midnightTomorrow = new Date();
            midnightTomorrow.setUTCHours(47);
            midnightTomorrow.setUTCMinutes(59);
            midnightTomorrow.setUTCSeconds(59);
            
            
            var time = new Date();
            time.setUTCSeconds(time.getUTCSeconds() + (this.offset/1000));
            
            /*var text = padDigits(d.getUTCHours()) + ":" + 
                   padDigits(d.getUTCMinutes()) + ":" + 
                   padDigits(d.getUTCSeconds());*/
            //text = d.toUTCString();
            
            if ((time < now) || (time < midnight))
              // text = "Past";
              if (this.showToday)
                text = "today at " + padDigits(time.getUTCHours()) + ":" + padDigits(time.getUTCMinutes()) + ":" + padDigits(time.getUTCSeconds());
              else
                text = padDigits(time.getUTCHours()) + ":" + padDigits(time.getUTCMinutes()) + ":" + padDigits(time.getUTCSeconds());
//            else if (time < midnight)
//              text = "Today " + padDigits(time.getUTCHours()) + ":" + padDigits(time.getUTCMinutes()) + ":" + padDigits(time.getUTCSeconds());
            else if ( time < midnightTomorrow )
              text = "tomorow at " + padDigits(time.getUTCHours()) + ":" + padDigits(time.getUTCMinutes()) + ":" + padDigits(time.getUTCSeconds());
            else 
              text = time.toUTCString();
            
            //text = text + " = " + time.toUTCString() + " : " + midnight.toUTCString() + " : " + now.toUTCString();
            //text = midnightTomorrow.toUTCString();
            $('#' + this.name).text(text);
        }
    );
    
    if ( lblCoins) {

        // code for calculating real time silver.
        if (coins.Count) { // Check the script should be running.
            if (coins.Now >= coins.Limit) {
                lblCoins.addClass('silverOverflow');
                lblTres.addClass('silverOverflow');
                // whole <td> $('#ctl00_ctrlVillageHeaderInfo_lblCoins').parent().addClass('silverOverflow');
                $('#silverGetMoreLink').parent().remove();
                coins.Count = false;
                //alert('stoping');
            } else {
                now = new Date();
                Seconds = (now - coins.clockStart) / 1000;
                coins.Now = coins.Start + (Seconds * coins.PreSecondProduction);
            
                if (coins.Now >= coins.Limit)
                    coins.Now = coins.Limit;

            }
            lblCoins.text(addCommas( Math.floor(coins.Now).toFixed(0).toString()));
            if (lowestNotification != null)
                if (lowestNotification <= coins.Now)
                    windowReload();
        }
    }    
    //DumperAlert(coins);
    //return;
    
    setTimeout("Tick();", 500);
}

//add main clock
addClock();

function addClock() {
$(
    function ()
    {
        coins.clockStart = new Date();

        $('.Time').each(
            function (i)
            {
                clockname = "Clock" + i;
                //$(this).attr('id', name);
                /*
                f = new Date();
                
                if ($(this).text().match(/^\d:\d:\d$/)) {
                    // basic time
                    time = $(this).text().split(':', 3);
                    for(i=0;i<3;i++) { time[i] = parseInt(time[i], 10); }
                
                    f.setUTCHours(time[0]);
                    f.setUTCMinutes(time[1]);
                    f.setUTCSeconds(time[2]);
                } else if ($(this).text().match(/^(Apr)\s(\d{2)\s\d:\d:\d$/)) {
                
                }
                
                Clocks.push({ 'name' : name, 'offset' : (f - coins.clockStart) });*/
                showToday = false;
                //alert($(this).text());
                
                if (time = $(this).text().match(/^(\d+):(\d+):(\d+)$/i) ) {
                    //DumperAlert(time);
                    
                    f = new Date();
                    
                    f.setUTCHours(time[1]);
                    f.setUTCMinutes(time[2]);
                    f.setUTCSeconds(time[3]);
                    
                    showToday = false;
                } else if (time = $(this).text().match(/^(Today\s){0,1}[^0-9]*(\d+):(\d+):(\d+)$/i) ) {
                    //DumperAlert(time);
                    
                    f = new Date();
                    
                    f.setUTCHours(time[2]);
                    f.setUTCMinutes(time[3]);
                    f.setUTCSeconds(time[4]);
                    
                    if (time[1])
                        showToday = true;
                } else if (time = $(this).text().match(/^(Tomorrow)\s(\d+):(\d+):(\d+)$/) ) {
                    f = new Date();
                    
                    f.setUTCHours(parseInt(time[2]) + 24);
                    f.setUTCMinutes(time[3]);
                    f.setUTCSeconds(time[4]);
                } else if ( time = $(this).text().match(/^(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)\s(\d+)\s(\d+):(\d+):(\d+)$/) ) {
                    f = new Date();
                    
                    f.setUTCMonth(Months[ time[1] ]);
                    f.setUTCDate(time[2]);
                    f.setUTCHours(time[3]);
                    f.setUTCMinutes(time[4]);
                    f.setUTCSeconds(time[5]);
                } else if( Date.parse($(this).text()) ) {
                    f = new Date();
                    f.setTime(Date.parse($(this).text()));
                } else {
                    f = null;
                }
                
                if (f)
                {
                    //$(this).after('<td>' + f.toUTCString() + '</td><td id="' + name + '"></td>');
                    $(this).attr('id', clockname);
                    
                    //DumperAlert([name, f]);
                    
                    Clocks.push({ 'name' : clockname, 'offset' : (f - coins.clockStart), 'showToday' : showToday });
                }
            }
        );

        // this is now done on page load
       //UpdateStartingCoins();
        
        $('.jsSilver').each(
            function () 
            {
                n = parseInt($(this).text().replace(/,/g, ''), 10);
                if (!lowestNotification)
                    lowestNotification = n
                else
                    if (lowestNotification > n)
                        lowestNotification = n
            }
        );
        //alert('Lowest Notification:' + lowestNotification);
        
        setTimeout("Tick();", 500);
        
    }
);
}
function padDigits (num)
{
    var s = num.toString();
    if (s.length == 1) {
        return "0" + s;
    }
    return s;
}

// OLD - replaced with ROEUtils.addThousandSeperator
function addCommas(nStr)
{
  	nStr += '';
  	var x = nStr.split('.');
  	var x1 = x[0];
  	var x2 = x.length > 1 ? '.' + x[1] : '';
  	var rgx = /(\d+)(\d{3})/;
  	while (rgx.test(x1)) {
  		  x1 = x1.replace(rgx, '$1' + ',' + '$2');
  	}
  	return x1 + x2;
}
