

(function (obj) {

    var _init = function (footerJQObj, village, refreshFunction) {
        footerJQObj.empty();
        var footer = footerJQObj;
        var _refreshFunction = refreshFunction;
        var _populate = function () {

            var template = $(BDA.Templates.getRaw("Footer", ROE.realmID));          
            template.find('.fVillName').html(village.name + " (" + village.x + "," + village.y + ")");
            template.find('.fVillFood').html(ROE.Utils.addThousandSeperator(village.popRemaining));
            template.find('.fVillSilver').html(ROE.Utils.addThousandSeperator(village.coins)).attr("data-vid", village.id);
            template.find('.transportSilverHere').attr('data-villageId', village.id);
            template = $(template.html());

            footer.addClass('fontGoldFrLClrg live').append(template);

            footer.find('.transportSilverHere').click(function () {
                ROE.Frame.showSilverTransportPopup(village.id.toString(), true);
            });
            
            footer.find('.goToBazzar').click(function () {
                ROE.Frame.popupGifts(village.id);
            });

            footer.find('.rewards').click(function () {
                ROE.Items2.showPopup(village.id);
            });
            
        };
        
        _populate();
        

        /*
        var _test = function () {
            console.log('TEST');
        };
        return {
            test: function () {
                _test();
            }
        };
        ///the widget test function is used like this:
        //var footerWidget = ROE.FooterWidget.init(footerContainer, DATA, "ROE.QuickRecruit.reInitContent();");
        //footerWidget.test();
        */
    }

obj.init = _init;

}(window.ROE.FooterWidget = window.ROE.FooterWidget || {}));


