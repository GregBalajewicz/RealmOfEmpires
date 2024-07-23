var page = {
    load: [],
    load_scripts: function(){
        $.each(this.load, function(i, n){ n(); });
    },
    fbinit: [],
    fbinit_call: function(){
        $.each(this.fbinit, function (i, n) { n(); });
    }
}

function ajax(url, param, suc, err, asynccall, timeoutInMS, type) {
    
    return $.ajax({
          url: url,
          dataType: 'json',
          data: param,
          success: function (json) {
              if (ROE.playerID && json.playerid && json.playerid != ROE.playerID) {
                  location.reload(); return;
              }
            
              if (json.success) {
                  suc(json.object);
              } else {
                  if (json.redirect_url) { window.location = json.redirect_url; return; }
                  if (!err && !json.message) { alert('command failed'); }
                  if (!err && json.message) {
                      if (window.ROE && window.ROE.Frame && window.ROE.Frame.infoBar) {
                          ROE.Frame.infoBar(json.message)
                      } else {
                          alert(json.message);
                      }
                  }
                  if (err) err(json, json.message);
              }
          },
          error : err,
          async : asynccall,
          timeout: timeoutInMS,
          type: type ? type : 'GET',
          cache: false
   });




//    $.getJSON(url, param, function(json) {

//        if (json.success) {
//            suc(json.object);
//        } else {
//            if (!err && !json.message) { alert('command failed'); }
//            if (!err && json.message) { alert(json.message); }
//            if (err) err(json, json.message);
//        }
//    });

}

// short hover
jQuery.fn.hoverClass = function(klass, inquery) {
    // if inquery, than you wish on hover of element, put over to several elements inside
    return this.hover(
        function() {
            var who = $(this);

            if (inquery) { who = $(this).find(inquery) }

            who.addClass(klass);
        },
        function() {
            var who = $(this);

            if (inquery) { who = $(this).find(inquery) }

            who.removeClass(klass);
        }
    );
}

/*
parseUri 1.2.1
(c) 2007 Steven Levithan <stevenlevithan.com>
MIT License
*/

function parseUri(str) {
    var o = parseUri.options,
                m = o.parser[o.strictMode ? "strict" : "loose"].exec(str),
                uri = {},
                i = 14;

    while (i--) uri[o.key[i]] = m[i] || "";

    uri[o.q.name] = {};
    uri[o.key[12]].replace(o.q.parser, function ($0, $1, $2) {
        if ($1) uri[o.q.name][$1] = $2;
    });

    return uri;
};

parseUri.options = {
    strictMode: false,
    key: ["source", "protocol", "authority", "userInfo", "user", "password", "host", "port", "relative", "path", "directory", "file", "query", "anchor"],
    q: {
        name: "queryKey",
        parser: /(?:^|&)([^&=]*)=?([^&]*)/g
    },
    parser: {
        strict: /^(?:([^:\/?#]+):)?(?:\/\/((?:(([^:@]*):?([^:@]*))?@)?([^:\/?#]*)(?::(\d*))?))?((((?:[^?#\/]*\/)*)([^?#]*))(?:\?([^#]*))?(?:#(.*))?)/,
        loose: /^(?:(?![^:@]+:[^:@\/]*@)([^:\/?#.]+):)?(?:\/\/)?((?:(([^:@]*):?([^:@]*))?@)?([^:\/?#]*)(?::(\d*))?)(((\/(?:[^?#](?![^?#\/]*\.[^?#\/.]+(?:[?#]|$)))*\/?)?([^?#\/]*))(?:\?([^#]*))?(?:#(.*))?)/
    }
};

﻿/* 
 * Url Manage
 * Version 0.1
 * Parses and recombine parts of URLs
 *
 * MIT License
 * 
 * Author: Igor Golodnitsky
 * Author email: webprogmaster@gmail.com
 * 
 *
 * Based on: uri parse by Steven Levithan
 * =====================================================
 * Examples:
 * =====================================================
 * Take any string where your url stayed
 *   var url = 'http://www.google.com'.url() // -> returns Url object
 *   
 *   url.attr('protocol', 'https'); // parts of url change
 *   url.param('id', 25); // query string change params
 *   url.param({id: 25, name: 'xuxel'}); // extend current params with object
 
*/


String.prototype.url = function() {
    return new Uri(parseUri(this));
}

var Uri = function(parsed) {
    this.parsed = parsed;
}

Uri.prototype = {
    attr: function(name, value) {
        return this.getset(this.parsed, name, value);
    },
    param: function(name, value) {
        return this.getset(this.parsed.queryKey, name, value);
    },
    getset: function(inner, name, value) {

        if (typeof (name) === 'object') {
            // like extend
            for (var key in name) {
                inner[key] = name[key];
            }
        }
        else {
            if (value) {
                // set
                inner[name] = value;
            }
            else {
                // get
                return inner[name];
            }
        }
        this.reload();
        return this;
    },
    reload: function() {
        this.parsed = parseUri(this.string());
    },
    string: function() {
        var o = this.parsed;

        var qs = '';
        for (var key in o.queryKey) {
            qs += key + '=' + o.queryKey[key] + '&';
        }
        qs = qs.slice(0, qs.length - 1);

        return o.protocol + '://' + o.host + (o.port ? ':' + o.port : '') + o.path + '?' + qs;
    }
}


function trim(s) {
  s = s.replace( /^\s+/g, '');
  return s.replace( /\s+$/g, '');
}


// turn clan entry boxes to suggestion boxes
$(function () {
	var cache = {},
	lastXhr;
	$(".jsclans").autocomplete({
	    minLength: 1,
	    source: function (request, response) {
	        var term = request.term;
	        if (term in cache) {
	            response(cache[term]);
	            return;
	        }

	        lastXhr = $.getJSON("NamesAjax.aspx?what=clans", request, function (data, status, xhr) {
	            cache[term] = data;
	            if (xhr === lastXhr) {
	                response(data);
	            }
	        });
	    }
	});
});



// turn player entry boxes to suggestion boxes
$(function () {
	var cache = {},
	lastXhr;
	$(".jsplayers").autocomplete({
	    minLength: 3,
	    source: function (request, response) {
	        var term = request.term;
	        if (term in cache) {
	            response(cache[term]);
	            return;
	        }

	        lastXhr = $.getJSON("NamesAjax.aspx?what=players", request, function (data, status, xhr) {
	            cache[term] = data;
	            if (xhr === lastXhr) {
	                response(data);
	            }
	        });
	    }
	});
});


//HACKY fix for case 30172 where the playername autofill thing broke whenever the content of the page was reserved.
//this was happening because $(".jsplayers") was only called once on page load, 
//and when the inner content was reserved the new elements didnt have the autocomplete functionality
//The fix basically delegates an onclick to re-autocomplete the proper elemnts, while being compatible with older stuff -Farhad Aug 2015
$(function () {
    $(document).ready(function () {
        $(".playerRanking").delegate(".TextBox.jsplayers", "click", function () {         
            $(function () {
                var cache = {},
                lastXhr;
                $(".jsplayers").autocomplete({
                    minLength: 3,
                    source: function (request, response) {
                        var term = request.term;
                        if (term in cache) {
                            response(cache[term]);
                            return;
                        }

                        lastXhr = $.getJSON("NamesAjax.aspx?what=players", request, function (data, status, xhr) {
                            cache[term] = data;
                            if (xhr === lastXhr) {
                                response(data);
                            }
                        });
                    }
                });
            });
        });
    });
});
	
function FixFFLinkIssue() 
{
    $(function () { 
        var isInIframe = (window.top != window); 
        if ($.browser.mozilla && isInIframe) { 
            $('a[href^="javascript:"]').each(function () { 
                var newOnClick = $(this).attr('href').replace(/^javascript:/i, ''); 
                var existingOnClick = this.getAttribute('onclick'); 
                existingOnClick = (existingOnClick ? (existingOnClick + ';') : '') + newOnClick; 
                this.setAttribute('onclick', existingOnClick); 
            }); 
        } 
    }); 

}

$.fn.timerpopup = function(fill) {

    var timer = -1;

    this.hover(
        function() {
            var tp = $(this);

            clearTimeout(timer);

            var hm = $('#hoverInfo');

            if (hm.length > 0) {
                hm.show();
            } else {
                $('body').append('<div id="hoverInfo" class="facebookMenu">Loading...</div>');

                hm = $('#hoverInfo');

                hm.hover(
                    function() {
                        clearTimeout(timer);
                    },
                    function() {
                        $(this).hide();
                    }
                );
            }
            fill(hm, tp);

            var pos = tp.position();
            pos.width = tp.width();
            pos.height = tp.height();

            hm.css({
                'top': parseInt(pos.top, 10) + 'px',
                'left': parseInt(pos.width, 10) + parseInt(pos.left, 10) + 'px'
            });
        },
        function() {
            timer = setTimeout("$('#hoverInfo').fadeOut('fast');", 1000);
        }
    );

}




$.fn.timerpopup2 = function(idOfContainerElement, classofContainerElement, fill) {
    if (typeof idOfContainerElement !== 'string') { throw 'idOfContainerElement not a string'; }

    var timer = -1;

    this.hover(
        function() {
            var tp = $(this);            

            clearTimeout(timer);

            var hm = $('#' + idOfContainerElement);

            if (hm.length > 0) {
                hm.show();
            } else {
                $('body').append('<div id="' + idOfContainerElement + '" class="' + classofContainerElement + '"></div>');

                hm = $('#' + classofContainerElement);

                hm.hover(
                    function() {
                        clearTimeout(timer);
                    },
                    function() {
                        $(this).hide();
                    }
                );
            }
            fill(hm, tp);

            var pos = tp.offset();
            pos.width = tp.width();
            pos.height = tp.height();

            hm.css({
                'top': parseInt(pos.top, 10) + 'px',
                'left': parseInt(pos.width, 10) + parseInt(pos.left, 10) + 'px'
            });
        },
        function() {
            timer = setTimeout("$('#" + idOfContainerElement + "').fadeOut('fast');", 1000);
        }
    );

}



Function.prototype.bind = function bind(context) {
    var slice = Array.prototype.slice;

    if (arguments.length < 2 && arguments[0] === undefined) return this;
    var __method = this, args = slice.call(arguments, 1);
    return function () {
        function update(array, args) {
            var arrayLength = array.length, length = args.length;
            while (length--) array[arrayLength + length] = args[length];
            return array;
        }

        function merge(array, args) {
            array = slice.call(array, 0);
            return update(array, args);
        }

        var a = merge(args, arguments);
        return __method.apply(context, a);
    }
}






