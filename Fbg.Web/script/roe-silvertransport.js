

(function (obj) {

    var _cache = {};
    var _conatiner;
    var _targetVillageId;
    var _minAmount = 1000;
    var _xNumVillages = 10;
    var _village; //target village data
    var _isMine; //does target village belong to you?
    var _nearbyData; //data about nearby friendly villages

    var _init = function (container, villageId, isMine, x,y) {
        _conatiner = container.empty();
        _isMine = isMine;
        _targetVillageId = villageId;

        if (!_cache.template) {
            var temp = BDA.Templates.getRawJQObj("SilverTransportTempl", ROE.realmID);
            _cache['template'] = temp;
            _cache['villageRow'] = temp.find('.villageRow').remove();
        }
        
        ROE.Frame.busy('Gathering Village info...', 5000, _conatiner);
        if (_isMine) {

            //Get own village deep info, then get light info about nearby own vills
            ROE.Villages.getVillage(_targetVillageId, function _gotVillage(village) {
                _village = village;
                ROE.Frame.free(_conatiner);
                ROE.Frame.busy('Checking nearby villages...', 5000, _conatiner);
                ROE.Api.call_silvertransport_getnearestvillages(_targetVillageId, _minAmount, _xNumVillages, _populate);
            }, ROE.Villages.Enum.ExtendedDataRetrieveOptions.ensureExistsAndTriggerGetLatest);

        } else {
            
            //Get foreign village info, then get light info about nearby own vills
            ROE.Api.call_othervillageinfo(_targetVillageId, function _othervillageinfoReturn(village) {
                _village = village;
                ROE.Frame.free(_conatiner);
                ROE.Frame.busy('Checking nearby villages...', 5000, _conatiner);
                ROE.Api.call_silvertransport_getnearestvillagesforeign(_targetVillageId, _populateForeign);
            });

        }
    }

    //for sending silver to your own vills
    var _populate = function _populate(data) {
        ROE.Frame.free(_conatiner);
        _nearbyData = data;
       
        _conatiner.html(_cache.template.clone());
        _conatiner.find('.destinationName').html(_village.name);
        _conatiner.find('.destinationCords').html('(' + _village.x + ',' + _village.y + ')');
        
        var villageContainer = _conatiner.find('.villagesPanel').remove();
        var villageTreasurySpace = _village.coinsTresMax - _village.coins;
        var max = 0;
        var nearbyVill;
        var villageRow;
        
        if (_nearbyData.length) {
            villageContainer.empty();
            for (var v in _nearbyData) {
                nearbyVill = _nearbyData[v];
                if (nearbyVill.VillageID) {
                    max += nearbyVill.MinAmount;
                    nearbyVill.maxSend = Math.min(nearbyVill.MinAmount, villageTreasurySpace);
                    nearbyVill.maxSendFormatted = BDA.Utils.formatNum(nearbyVill.maxSend);
                    villageRow = $(BDA.Templates.populate(_cache['villageRow'][0].outerHTML, nearbyVill));
                    villageContainer.append(villageRow);
                }
            }
            villageContainer.append('<div class="paddinator2000"></div>');
        }
        
        if (max > villageTreasurySpace) {
            max = villageTreasurySpace;
        }

        //disable the maxall button
        if (max <= 0) {
            _conatiner.find('.getMaxAll').addClass('grayout').attr('onclick', '');
        }

        _conatiner.find('.getMaxAll').html('Get Max ' + BDA.Utils.formatNum(max));        
        _conatiner.find('.themeM-view.default').append(villageContainer);      
        
    };

    //for sending silver to other player's vills
    var _populateForeign = function _populate(data) {
        ROE.Frame.free(_conatiner);
        _nearbyData = data;

        _conatiner.html(_cache.template.clone());
        _conatiner.find('.destinationName').html(_village.name);
        _conatiner.find('.destinationCords').html('(' + _village.x + ',' + _village.y + ')');
        _conatiner.find('.getMaxAll').remove();
        _conatiner.find('.showFilters').remove();
        
        var villageContainer = _conatiner.find('.villagesPanel').addClass('foreign').remove();
        var nearbyVill;
        var villageRow;

        if (_nearbyData.length) {
            villageContainer.empty();
            for (var v in _nearbyData) {
                nearbyVill = _nearbyData[v];
                if (nearbyVill.VillageID) {
                    nearbyVill.maxSend = nearbyVill.MinAmount;
                    nearbyVill.maxSendFormatted = BDA.Utils.formatNum(nearbyVill.MinAmount);
                    villageRow = $(BDA.Templates.populate(_cache['villageRow'][0].outerHTML, nearbyVill));
                    villageContainer.append(villageRow);
                }
            }
        }
        
        _conatiner.find('.themeM-view.default').append(villageContainer);
        _conatiner.find('.noTransports').html('No transports availabale.');
    };

    //only works for friendly target vill
    var _getMaxAll = function _getMaxAll() {
        ROE.Frame.busy('Calling all transports...', 5000, _conatiner);
        ROE.Api.call_silvertransport_getmaxsilverfromnearestvillages(_targetVillageId, _minAmount, _xNumVillages, _getMaxAllReturn);
    }

    function _getMaxAllReturn(data) {
        //right now this can and should only be done for _isMine true anyway
        if (_isMine) {
            ROE.Frame.free(_conatiner);
            ROE.Frame.busy('Updating Village info...', 5000, _conatiner);
            ROE.Villages.ExtendedInfo_loadLatest(_targetVillageId, function _gotVillage(village) {
                _village = village;
                ROE.Frame.free(_conatiner);
                ROE.Frame.busy('Checking nearby villages...', 5000, _conatiner);
                ROE.Api.call_silvertransport_getnearestvillages(_targetVillageId, _minAmount, _xNumVillages, _populate);
            });
        }
    }

    var _getAmountFromVill = function _getAmountFromVill(vidFrom, amount) {
        ROE.Frame.busy('Transport initiating...', 5000, _conatiner);
        ROE.Api.call_silvertransport_sendamount(vidFrom, _targetVillageId, _village.x, _village.y, amount,_isMine, _getAmountFromVillReturn);
    }

    function _getAmountFromVillReturn(data) {
        ROE.Frame.free(_conatiner);

        if (!data.success) {
            ROE.Frame.errorBar('Transport failed.');
            return;
        }

        //simulate the deducted transport capability, without making a nearby call. Is it logical? -farhad
        if (_nearbyData.length) {
            for (var v in _nearbyData) {
                nearbyVill = _nearbyData[v];
                if (nearbyVill.VillageID && nearbyVill.VillageID == data.vFromId) {
                    nearbyVill.MinAmount -= data.amountToTransport;
                    if (nearbyVill.MinAmount <= 0) {
                        delete _nearbyData[v];
                    }
                    break;
                }
            }
        }

        //if target village is ours, get latest
        if (_isMine) {           
            ROE.Frame.busy('Updating Village info...', 5000, _conatiner);
            ROE.Villages.ExtendedInfo_loadLatest(_targetVillageId, function _gotVillage(village) {
                _village = village;
                ROE.Frame.free(_conatiner);
                _populate(_nearbyData);
            });
        } else {
            //maybe here hide the sender village row, strike it out from _nearbyData?
            _populateForeign(_nearbyData);
        }
         
    }

    ///UI functions
    function _activateInputForRow(btn) {
        btn.parent().addClass('inputActive');
        btn.parent().find('.sendAmountInput').focus();
    }

    function _deactivateInputForRow(input) {
        input.parent().removeClass('inputActive');
    }

    function _getTransportFromInputGo(btn, vid) {

        //cleanse and verify input
        var inputValue = btn.parent().find('.sendAmountInput').val();
        inputValue = parseInt(inputValue);
        if (isNaN(inputValue) || inputValue < 1) {
            btn.parent().find('.sendAmountInput').val(0);
            return;
        }

        //check against max possible transport
        var maxValue = parseInt(btn.attr('data-max'));
        if (isNaN(maxValue)) { maxValue = 0; }
        if (inputValue > maxValue) {
            btn.parent().find('.sendAmountInput').val(maxValue);
            return;
        }
        
        _getAmountFromVill(vid, inputValue);
    }

    function _showFilterOptions() {

        var pcontent = '<div class="minOption fontGoldFrLCmed"><div>Minimum transport</div><input class="minOptionAmount" type="number" pattern="\d*" value=""/></div>' +
            '<div class="villOption fontGoldFrLCmed"><div>Villages displayed</div><input class="villOptionAmount" type="number" pattern="\d*" value=""/></div>'+
            '<div class="setOptions BtnBSm2 fontSilverFrSClrg sfx2">Set</div>';

        ROE.Frame.simplePopopOverlay('https://static.realmofempires.com/images/icons/M_FilterB.png', 'Filter Options', pcontent, 'silverTransportFiltersPopup', _conatiner);

        var filterPopup = $('.silverTransportFiltersPopup', _conatiner);

        $('.silverTransportFiltersPopup .minOptionAmount').val(_minAmount);
        $('.silverTransportFiltersPopup .villOptionAmount').val(_xNumVillages);
        $('.silverTransportFiltersPopup .setOptions').click(function () {
            //cleanse and reject bad options
            var mA = parseInt($('.silverTransportFiltersPopup .minOptionAmount').val());
            if (isNaN(mA) || mA < 0) {
                mA = 0;
                $('.silverTransportFiltersPopup .minOptionAmount').val(mA);
                return;
            }
            var vA = parseInt($('.silverTransportFiltersPopup .villOptionAmount').val());
            if (isNaN(vA) || vA < 1) {               
                vA = 1;
                $('.silverTransportFiltersPopup .villOptionAmount').val(vA);
                return;
            }
            _minAmount = mA;
            _xNumVillages = vA;
            filterPopup.remove();

            ROE.Frame.busy('Checking nearby villages...', 5000, _conatiner);

            if (_isMine) {
                ROE.Api.call_silvertransport_getnearestvillages(_targetVillageId, _minAmount, _xNumVillages, _populate);
            } else {                
                //filters should have only been shown for your own vills
                //ROE.Api.call_silvertransport_getnearestvillagesforeign(_targetVillageId, _populateForeign);
            }

        });
        
        
    }

    function _viewTransports() {
        ROE.Frame.popGeneric("View Active Silver Transports", "", 600, 500);
        ROE.Frame.showIframeOpenDialog('#genericDialog', 'SilverTransports.aspx');
    }


    obj.init = _init;
    obj.getMaxAll = _getMaxAll;
    obj.getAmountFromVill = _getAmountFromVill;
    obj.activateInputForRow = _activateInputForRow;
    obj.deactivateInputForRow = _deactivateInputForRow;
    obj.getTransportFromInputGo = _getTransportFromInputGo;
    obj.showFilterOptions = _showFilterOptions;
    obj.viewTransports = _viewTransports;
    
}(window.ROE.SilverTransport = window.ROE.SilverTransport || {}));


