/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.6.4-vsdoc.js"/>
/// <reference path="bda-ui-scrolling.js"/>
/// <reference path="bda-ui-transition.js"/>
/// <reference path="roe-api.js"/>
/// <reference path="roe-player.js" />
/// <reference path="countdown.js"/>
/// <reference path="roe-vov.js" />
/// <reference path="roe-ui.sounds.js" />
(function (ROE) {
}(window.ROE = window.ROE || {}));

(function (obj) {
    // holds the dom for reports template
    var _container;
    BDA.Console.setCategoryDefaultView('ROE.Reports', false); // by default, do not display the performance category. this assumes BDA.Console is defined BEFORE this file


    var CACHE = {};

    var CONST = {
        pageLength: 20,
        tableName: 'Reports',
        forwardLookupThreshold: 500,
        starred: "STARRED"
    }

    // Names for each of the report templates
    CONST.ReportTemplateName = {
        misc: "Report_MiscDetails",
        attack: "Report_AttackDetails",
        support: "Report_SupportAttacked"
    };

    // Report types
    CONST.ReportType = {
        attack: "Attack",
        supportAttacked: "Support Attacked",
        misc: "Misc." // Catch all report type
    };

    CONST.Selector = {
        reportDetailsContainer: ".reportDetailsView .reportDetailContainer", 
        reportDetailsView: ".reportDetailsView", 
        reportList: ".list",
        miscReportPanel: ".miscReport", 
        attackReportPanel: ".attackReport",
        attackingUnitTitle: ".attackingUnitTitle",
        defendingUnitTitle: ".defendingUnitTitle",
        attackerUnitsTableSection: ".attackerUnits",
        defenderUnitsTableSection: ".defenderUnits",
        supportAttackedReportPanel: ".supportAttackedReport",
        deployedPrefix: ".deployed-",
        lostPrefix: ".lost-",
        remainingPrefix: ".remaining-",
        zeroStyle: ".ZeroUnitCount",
        forwardedInfo: ".forwardedInfo",
        reportItemsList: "#reports_popup .list .items",
        moreButton: "#reports_popup .more",
        templates: {
            attackReport: ".attackReport.template",
            supportAttackedReport: ".supportAttackedReport.template"
        }
    };
    
    CONST.reportDetailPhrases = {
        Plunder: "",
        SilverInTreasury: "",
        WasAttackedAndDamaged: "",
        ToLevel: "",
        Silver: "",
        Unknown: "",
        TitleVictory: "",
        TitleSuccess: "",
        TitleDefeat: "",
        TitleWarning: "",
        TitleVillageCaptured: "",
        TitleReport: ""
    };

    CONST.maxNameLength = 14;

    var Api = window.ROE.Api,
        Frame = window.ROE.Frame,
        UnitTypesLookup = window.ROE.Entities.UnitTypes;

    CACHE.reportPopupTemplate=null;
    CACHE.reportDetailsTemplate = null;
    CACHE.unitsReportTable = null;
    CACHE.headingsRowTemplate = null;
    CACHE.unitsRowTemplate = null;
    CACHE.dividerRowTemplate = null;
    CACHE.versusTemplate = null;
    CACHE.attackReportTemplate = null;
    CACHE.supportAttackedReportTemplate = null;
    CACHE.damagedBuildingTemplate = null;
    CACHE.forwardedTemplate = null;
    CACHE.forwardReportTemplate = null;
    CACHE.notePopupTemplate = null;
    CACHE.reportData = {};

    var _advancedReportsListOn = false;

    CACHE.supportAttackedInfoTemplate = null; // The chunk for each supporting village

    // Store the last selected report information
    // so that it is available between functions
    var _itemTemplate;
    var _selectedReportID;
    var _selectedReportItem;
    var _reportItems; // A jQuery list of item elements in the main report list
    var _reportItemIndex = -1;
    var _filterOn = false;
    var _starredOn = false;
    var _filterGovAttack = false;
    var _filterSpyOnly = false;
    var _filterSuccess = false;
    var _filterDefeat = false;
    var _numItemsInFilter = 0;
    var _selectedFromList = true;
    var _pendingNextReport = false;
    var _lastReportsDeleted = [];
    var _totalChecked = 0; // num of checked reports

    // Confirm deleting
    var _confirmDeleteTimer;
    var _waitingForDeleteConfirm = false;
    var _waitingForDeleteSelectedConfirm = false;
    var _confirmDeleteSelectedTimer;
    var _alreadyLoaded = false;

    

    obj.init = function (container, thereIsNewReports, searchString) {

        // Force a reload if reports table doesn't exist
        var reportsTableDirty = false;
        try {
            // note, for situations where websql and indexeddb does not exist, this will result in no table, and will force the reload of reports 
            //  on first load and is necessary for reports to work properly
            if (!BDA.Database.DoesTableExist("Reports_" + BDA.Database.id()) || !BDA.Database.LocalGet('ReportsListLoaded')) {
                // The table doesn't so need to rebuild
                reportsTableDirty = true;
                BDA.Database.LocalDel('ReportsListLoaded');
            }
        } catch (e) {
            // Ignore, database is probably undefined
            reportsTableDirty = true;
        }

        // Note: on mobile views are destroyed by the system so we have no choice but to rebuild.
        if (ROE.Player.Indicators.report || ROE.isMobile || !_alreadyLoaded /*|| thereIsNewReports */ || reportsTableDirty) {
            _container = $(container);

            if (!ROE.isMobile) { // Only for d2
                // Listen for the dialog resize event so we can adjust layout items if necessary
                $('#reportsDialog').on("dialogresize", _handleDialogResize);
            }

            // Reset any vars
            _filterOn = false;
            _starredOn = false;
            _filterGovAttack = false;
            _filterSpyOnly = false;
            _filterSuccess = false;
            _filterDefeat = false;
            _numItemsInFilter = 0;
            _selectedFromList = true;

            CONST.tableFullName = CONST.tableName + '_' + BDA.Database.id();


            Frame.busy(undefined, undefined, _container);

            var temp1;
            if (ROE.isMobile) {
                temp1 = BDA.Templates.getRawJQObj("ReportsPopup", ROE.realmID);
            } else {
                temp1 = BDA.Templates.getRawJQObj("ReportsPopup_d2", ROE.realmID);
            }


            Frame.free(_container);
            CACHE.reportPopupTemplate = temp1;

            container.empty().append(CACHE.reportPopupTemplate);
            _itemTemplate = $('.template.item', container).removeClass('template').remove();

            $(CONST.Selector.moreButton, container).click(_clickedLoadMore);

            // Buttons that appear on the bottom control bar of a report detail
            $('.slideBackToReportList', container).click(_slideBackToDefaultPage);
            $('.btnDeleteReport', container).click(_reportDelete);
            $('.btnStar', container).click(function () { _clickStarByReportID(_selectedReportID); });          
            $('.forward-search .selected', container).keyup(_reportForwardSearch);
            $('.forward-search .send', container).click(_reportForwardSend);
          
            if (!_container.hasClass('delegated')) {
                //setting up report item delegates
                _container.delegate(".item .hotspot", "click", _clickedReportItem);
                _container.delegate(".item .btnCheckboxToggle", "click", _clickedCheckbox);
                _container.delegate(".item .btnStarToggle", "click", _clickedStar);               
                //flag the container to avoid further rebinds as long as it exists
                _container.addClass('delegated');
            }


            // get the report forwarding template
            CACHE.forwardReportTemplate = $('.template.forwardPopup', CACHE.reportPopupTemplate).clone().removeClass('template');

            Frame.busy(undefined, undefined, _container);

            var temp;
            if (ROE.isMobile) {
                temp = BDA.Templates.getRawJQObj("ReportDetailsTempl", ROE.realmID);
            } else {
                temp = BDA.Templates.getRawJQObj("ReportDetailsTempl_d2", ROE.realmID);
            }

            Frame.free(_container);
            CACHE.reportDetailsTemplate = temp;

            // Add the phrases to ReportsPopup as we'll need to use them
            // via the ROE.Utils.phrases func where phrases for details won't
            // be the same scope.
            container.append($('#ReportDetailPhrases', CACHE.reportDetailsTemplate));
            CONST.reportDetailPhrases.Plunder = ROE.Utils.phrase('ReportDetailPhrases', 'Plunder');
            CONST.reportDetailPhrases.SilverInTreasury = ROE.Utils.phrase('ReportDetailPhrases', 'SilverInTreasury');
            CONST.reportDetailPhrases.WasAttackedAndDamaged = ROE.Utils.phrase('ReportDetailPhrases', 'WasAttackedAndDamaged');
            CONST.reportDetailPhrases.ToLevel = ROE.Utils.phrase('ReportDetailPhrases', 'ToLevel');
            CONST.reportDetailPhrases.Silver = ROE.Utils.phrase('ReportDetailPhrases', 'Silver');
            CONST.reportDetailPhrases.Unknown = ROE.Utils.phrase('ReportDetailPhrases', 'Unknown');

            CONST.reportDetailPhrases.TitleVictory = ROE.Utils.phrase('ReportDetailPhrases', 'TitleVictory');
            CONST.reportDetailPhrases.TitleSuccess = ROE.Utils.phrase('ReportDetailPhrases', 'TitleSuccess');
            CONST.reportDetailPhrases.TitleDefeat = ROE.Utils.phrase('ReportDetailPhrases', 'TitleDefeat');
            CONST.reportDetailPhrases.TitleWarning = ROE.Utils.phrase('ReportDetailPhrases', 'TitleWarning');
            CONST.reportDetailPhrases.TitleVillageCaptured = ROE.Utils.phrase('ReportDetailPhrases', 'TitleVillageCaptured');
            CONST.reportDetailPhrases.TitleReport = ROE.Utils.phrase('ReportDetailPhrases', 'TitleReport');


            CACHE.headingsRowTemplate = $('.template.headingsRow', CACHE.reportDetailsTemplate).clone().removeClass('template');
            CACHE.unitsRowTemplate = $('.template.unitsRow', CACHE.reportDetailsTemplate).clone().removeClass('template');
            CACHE.dividerRowTemplate = $('.template.dividerRow', CACHE.reportDetailsTemplate).clone().removeClass('template');
            CACHE.buildingLevelTemplate = $('.template.buildingLevel2', CACHE.reportDetailsTemplate).clone().removeClass('template');
            CACHE.versusTemplate = $('.template.versus', CACHE.reportDetailsTemplate).clone().removeClass('template');
            CACHE.attackReportTemplate = $(CONST.Selector.templates.attackReport, CACHE.reportDetailsTemplate).clone().removeClass('template');
            CACHE.supportAttackedReportTemplate = $(CONST.Selector.templates.supportAttackedReport, CACHE.reportDetailsTemplate).clone().removeClass('template');
            CACHE.damagedBuildingTemplate = $('.template.damagedBuilding', CACHE.reportDetailsTemplate).clone().removeClass('template');
            CACHE.forwardedTemplate = $('.template.reportForwarded', CACHE.reportDetailsTemplate).clone().removeClass('template');

            // Add in (instead of overriding) these extra classes
            $('#popup_reports .IFrameDivTitle').css({ 'background-color': 'rgba(0, 0, 0, 0.55)' });
            $('#popup_reports .popupBody').css({
                'position': 'absolute',
                'left': '0px',
                'right': '0px',
                'top': '0px',
                'bottom': '0px',
                'height': 'auto !important',
                'margin-top': '40px'
            });

            if (!ROE.isMobile) { // d2 only
                // View offset problem work around...
                // First we clear the width and height
                // Then we'll re-set it after populate
                $('.themeM-view', container).css('width', 0);
                $('.themeM-view', container).css('height', 0);
            }

            BDA.Broadcast.subscribe(_container, "NewReports", _handleNewReportsBroadcast);

            // we are in reload from opening screen, not button
            fromAjaxLoadedQuickerThanFromCache = 1;

            _loadCache(0).done(_initLoadFromCache);

            if (BetaFeatures.status('AdvancedReportList') == 'ON') {
                _advancedReportsListOn = true;
            } else {
                _advancedReportsListOn = false;
            }

            // Don't show clear at the start (only when filtering)
            $('.filterReports .clearFilterBtn').hide();
            $('.filterReports .hiddenDuringFilter').show();
            $('.filterReports .filterTerm').hide();

            if (ROE.isMobile) {
                $('.hideToggleBtn').click(_toggleSelectBtns);
            }

            // If the user presses enter within the filter input,
            // then do the same action as hitting the filter button.
            $('#reportFilterInput').keypress(function _keyPressFilterReportsInput(e) {
                if (!e) e = window.event;
                var keyCode = e.keyCode || e.which;
                if (keyCode == '13') {
                    // Enter pressed
                    _filterReports();
                    return false;
                }
            });

            // case 33009
            if (searchString) {
                $('#reportFilterInput').val(searchString);
                _filterReports();
            } else {
                _reload();
            }
        } else {

            if (thereIsNewReports) {
                _showLatestReports();
            }

            if (searchString) {
                $('#reportFilterInput').val(searchString);
                _filterReports();
            }
        }

        _toggleFilterIcons(true);
        _alreadyLoaded = true;

      
    }

    var fromAjaxLoadedQuickerThanFromCache = 0; // 0 means do nothing

    var _reload = function _reload() {
        BDA.Console.log('ROE.Reports', "in _reload.");
        $('.list .loading', _container).show();
        Frame.busy(undefined, undefined, _container);

        _clearTotalChecked();
       
        ROE.Api.call('report', {}, _list_onAjaxDataSuccess);

        $(CONST.Selector.moreButton, _container).attr('offset', 0);
    }

    function _initLoadFromCache(r) {      
        if (fromAjaxLoadedQuickerThanFromCache == 2) {           
            fromAjaxLoadedQuickerThanFromCache = 0; return;
        }
        _populate(r);
    }

    function _clickedLoadMore(e) {
        e.preventDefault();
        _loadMore($(e.currentTarget));
    }

    function _loadMore(moreBtnElement) {
        var offset = moreBtnElement.attr('offset');
        offset = parseInt(offset) + CONST.pageLength;
        moreBtnElement.attr('offset', offset);
             
        _loadCache(offset).done(_appendRows);

        if (parseInt(BDA.Database.LocalGet('ReportsListLoaded')) - offset <= CONST.pageLength) {
            $(CONST.Selector.moreButton, _container).hide();
        }
    }

    // Refreshes the list after delete.
    function _refreshList(quantity) {
     
        // If we are filtering, we don't need to append anything
        // because currently filter and starred show all (no "more" button)
        if (_filterOn || _starredOn || _filterGovAttack || _filterSpyOnly) {
            _numItemsInFilter--;
            if (_numItemsInFilter <= 0) {

            } else {
                _updateNumResultsForFilter(_numItemsInFilter);
            }
            return;
        }

        if (!ROE.isMobile) {
            var offset = parseInt($(CONST.Selector.moreButton, _container).attr('offset'));
        
       
            if (quantity) {
                _loadQuantityCache(offset, quantity).done(_appendRows);
            } else {
                _loadOneCache(offset).done(_appendRows);
            }
        

            if (parseInt(BDA.Database.LocalGet('ReportsListLoaded')) - offset <= CONST.pageLength) {
                $(CONST.Selector.moreButton, _container).hide();
            }
        }
    }

    function _handleNewReportsBroadcast(event) {       
        $('#newReportsIndicator').addClass('newToast').show(); 
        $('#reports_popup .reload').addClass('effect-pulse newIndicator-round');
    }

    function _showLatestReports() {
        $('#newReportsIndicator').removeClass('newToast').hide();
        $('#reports_popup .reload').removeClass('effect-pulse newIndicator-round');
        if (ROE.isD2) {
            ROE.Frame.checkedReports(); // Let frame know we looked at the new reports.         
        }
        
        // Reset list scroll-load-more variables
        $(CONST.Selector.reportItemsList).scrollTop(0);

        _clearReportsFilterValues();
        _selectedFromList = true;

        // Now reload the list
        _reload();
    }

    function _handleDialogResize(event, ui) {
        if (ui.size.width > 800) {
            $('#reportsDialog').addClass('width800');
        } else {
            $('#reportsDialog').removeClass('width800');
        }
    }

    function _toggleSelectBtns(e) {
        e.preventDefault();
        var toggle = $(e.currentTarget);
        toggle.toggleClass("open");
        $('#reports_popup .listToolsSelect').toggleClass('hide');
        $('#reports_popup .items').toggleClass('toolsOpen');
    }

    /// Get database rows which contain a filter match inside subject
    function _loadFilteredCache(sFilter) {      
        return BDA.Database.FindByFilter(CONST.tableName, "subject", sFilter);       
    }

    /// Called when the num of filtered reports changes
    function _updateNumResultsForFilter(num) {        
        if (_filterOn || _starredOn || _filterGovAttack || _filterSpyOnly || _filterSuccess || _filterDefeat) {
            $('#numReportsFound').html(num);
        } 
    }
    
    /// An exposed func that grabs a filter to apply to the list
    function _filterReports() {

        if ($(CONST.Selector.reportDetailsView, _container).hasClass('slideLeftTo')) {
            _slideBackToDefaultPage();
        }
        
        _clearTotalChecked();
        var filterPhrase = $('#reportFilterInput').val();
        // Has to be an actual phrase to continue
        if (typeof (filterPhrase) === "undefined" || filterPhrase === null || filterPhrase === "")
            return;

        $('#reportFilterInput').blur();
        _toggleFilterIcons(false);
        _filterOn = true;       
        $('#filterApplied').html(filterPhrase);
        $('.filterReports .clearFilterBtn').show();
        $('.filterReports .filterTerm').show();
        $('.filterReports .hiddenDuringFilter').hide();
        _loadFilteredCache(filterPhrase).done(_populate);
    }

    /// An exposed func that clears the filter (and re-builds the list)
    function _clearReportsFilter() {
        _clearTotalChecked();
        // Separate the variables so we can call them
        // without having to call load cache
        _clearReportsFilterValues();
        $(CONST.Selector.reportItemsList).scrollTop(0);
        _loadCache(0).done(_populate);
    }
    function _clearReportsFilterValues() {
        _filterOn = false;
        _starredOn = false;
        _filterGovAttack = false;
        _filterSpyOnly = false;
        _filterSuccess = false;
        _filterDefeat = false;
        _toggleFilterIcons(true);
        $('.filterReports .clearFilterBtn').hide();
        $('.filterReports .hiddenDuringFilter').show();
        $('.filterReports .filterTerm').hide();
        _numItemsInFilter = 0;
        $('#numReportsFound').html("");
        $('#filterApplied').html("");
        $('#reportFilterInput').val("");
        // Reset more button offset.
        $(CONST.Selector.moreButton, _container).attr('offset', 0);
        if (parseInt(BDA.Database.LocalGet('ReportsListLoaded')) <= CONST.pageLength) {
            $(CONST.Selector.moreButton, _container).hide();
        }
    }

    function _loadStarredReports() {
        return BDA.Database.Find(CONST.tableName, "folderID", ["~null"]);
    }

    function _loadGovReports() {
        return BDA.Database.Find(CONST.tableName, "flag3", ["~null"]);
    }

    function _loadSpyReports() {
        return BDA.Database.Find(CONST.tableName, "flag2", ["~null"]);
    }

    function _loadSuccessReports() {
        return BDA.Database.Find(CONST.tableName, "flag1", ["Green"]);
    }

    function _loadDefeatReports() {
        return BDA.Database.Find(CONST.tableName, "flag1", ["Red"]);
    }

    function _nukeAllBtnClick(btn) {

        btn = $(btn);

        clearTimeout(btn.data('sureTimeOut'))

        btn.data('sureTimeOut', window.setTimeout(function () {
            btn.removeClass('areyousure areyousure2');
            $('.listToolIcon', btn).empty();
        }, 2500));

        if (!btn.hasClass('areyousure')) {
            $('.listToolIcon',btn).html('Sure?');
            btn.addClass('areyousure');
            return;
        }

        /* turned off 2nd are you sure, in favor of an ALERT
        if (!btn.hasClass('areyousure2')) {
            $('.listToolIcon', btn).html('100% Sure?');
            btn.addClass('areyousure2');
            return;
        }
        */

        btn.removeClass('areyousure areyousure2');
        $('.listToolIcon', btn).empty();

        if (window.confirm('This will remove ALL UNSTARRED reports.\nAre You Sure?')) {
            _nukeAll();
        }

    }

    function _nukeAll() {
        ROE.Frame.busy('Nuking All Reports...',15000,_container);
        ROE.Api.call('report_nuke',null, _nukeAllCB);
    }

    function _nukeAllCB(data) {
        ROE.Frame.free(_container);
        _reload();
    }

    // Detele report items selecting in the list
    function _deleteSelected(deleteBtn) {
        var listOfSelectedItems = $("#reports_popup .btnCheckboxToggle[data-selected='true']").closest('.item');

        if (deleteBtn.parent().hasClass('active'))
            ROE.UI.Sounds.click();

        if(listOfSelectedItems.length != 0) {

            if (_waitingForDeleteSelectedConfirm) {
                clearTimeout(_confirmDeleteSelectedTimer);
                _waitingForDeleteSelectedConfirm = false;
                deleteBtn.html("");
                _confirmDeleteSelected(listOfSelectedItems);
            } else {
                deleteBtn.html("Sure?");
                _waitingForDeleteSelectedConfirm = true;
                _confirmDeleteSelectedTimer = window.setTimeout(function () {
                    deleteBtn.html("");
                    _waitingForDeleteSelectedConfirm = false;
                }, 2000);
            }

           
        }
    }

    function _confirmDeleteSelected(listOfSelectedItems) {
        _reportDeleteMultiple(listOfSelectedItems);
    }

  
    function _forwardSelected() {

        if ($('.forwardSelectedReports').hasClass('active'))
            ROE.UI.Sounds.click();

        // First, check if anything was selected...
        var selectedItemsArr = $("#reports_popup .btnCheckboxToggle[data-selected='true']").closest('.item');
        if (selectedItemsArr.length > 0) {
            _forwardReportPopup(true, selectedItemsArr);
        }
    }

    function _selectAllReports() {       
        $('#reports_popup .item:not(.template) .btnCheckboxToggle').each(function (i, elem) {
            var toggle = $(elem);
            var checkmark = $('.checkmark', toggle);
            if (checkmark.hasClass('off')) {          
                checkmark.removeClass('off').addClass('on');
                toggle.attr('data-selected', 'true');
            }
            _updateTotalChecked(1);
        });
    }
    function _deselectAllReports() {
        $('#reports_popup .item:not(.template) .btnCheckboxToggle').not('.template').each(function (i, elem) {
            var toggle = $(elem);
            var checkmark = $('.checkmark', toggle);
            if (checkmark.hasClass('on')) {
                 checkmark.removeClass('on').addClass('off');
                 toggle.attr('data-selected', 'false');
            }            
        });
        _clearTotalChecked();
    }
   
    // Check mark stuff
    function _clickedCheckbox(e) {
        ROE.UI.Sounds.click();
        var toggle = $(e.currentTarget);
        var checkmark = $('.checkmark', toggle);
        if (checkmark.hasClass('on')) {
            checkmark.removeClass('on').addClass('off');
            toggle.attr('data-selected', 'false');
            _updateTotalChecked(-1);
        } else {
            checkmark.removeClass('off').addClass('on');
            toggle.attr('data-selected', 'true');
            _updateTotalChecked(1);
        }
    }

    function _clearTotalChecked() {
        _totalChecked = 0;
        $(".listTools .deleteSelectedBtn, .listTools .forwardSelectedReports").toggleClass("active", false);
    }

    function _updateTotalChecked(n) {       
        _totalChecked += n;
        if (_totalChecked <= 0) {
            _totalChecked = 0;
            
            $(".listTools .forwardSelectedReports, .listTools .deleteSelectedBtn").toggleClass("active", false);
        } else {
            $(" .listTools .forwardSelectedReports, .listTools .deleteSelectedBtn").toggleClass("active", true);
        }        
    }
    
    // TODO: use phrases for text
    function _editReports(editBtn) {      
        if (editBtn.hasClass('on')) {
            editBtn.removeClass('on').addClass('off');
            //$('#reports_popup .btnCheckboxToggle').hide();
            $('#reports_popup .editable').hide();
            editBtn.text("Edit");
            // We don't want filtering on anymore (in case it was).
            _clearReportsFilter();
        } else {
            editBtn.removeClass('off').addClass('on');
            //$('#reports_popup .btnCheckboxToggle').show();
            $('#reports_popup .editable').show();
            editBtn.text("Done");
        }
    }

    // Retuns true if any filter is active.
    function _isFilterActive() {
        return _filterOn || _starredOn || _filterGovAttack || _filterSpyOnly || _filterSuccess || _filterDefeat;
    }

    function _toggleFilterIcons(state) {
        // makes the filter buttons look inactive if any filter is active
        if (state) {
            $('.filterSpyOnlyBtn, .filterGovAttacksOnlyBtn, .filterSuccessOnlyBtn, .filterDefeatOnlyBtn', _container).toggleClass('active', true);
        } else {
            $('.filterSpyOnlyBtn, .filterGovAttacksOnlyBtn, .filterSuccessOnlyBtn, .filterDefeatOnlyBtn', _container).toggleClass('active', false);
        }
    }


    // Star stuff
    function _showStarredOnly() {
        // Do nothing if another/current filter active
        if (_isFilterActive())
            return;

        _clearTotalChecked();
        _toggleFilterIcons(false);
        _starredOn = true;
        $('#reportFilterInput').val(""); // incase they were searching for something
        // Piggy back off the filter feature
        $('#filterApplied').html(CONST.starred);
        $('.filterReports .clearFilterBtn').show();
        $('.filterReports .hiddenDuringFilter').hide();
        $('.filterReports .filterTerm').show();
        _loadStarredReports().done(_populate);
    }

    function _clickedStar(e) {
        ROE.UI.Sounds.click();
        var toggle = $(e.currentTarget);
        var reportItem = $(toggle).closest('.item');        
        if (toggle.hasClass('on')) {
            toggle.removeClass('on').addClass('off');
            ROE.Api.call('unstar_report', {
                rids: reportItem.attr('rid')
            }, _starredReportAPISuccessCallback);                
        } else {
            toggle.removeClass('off').addClass('on');
            ROE.Api.call('star_report', {
                rids: reportItem.attr('rid')
            }, _starredReportAPISuccessCallback);                       
        }
    }

    //bit of a hack, to be called by another function but still use existing _clickedStar(e)
    function _clickStarByReportID(reportID) {

        var reportListItem = $('#reports_popup .item[rid="' + reportID + '"]');
        var itemStarToggle = $('.btnStarToggle', reportListItem);
            
        if (itemStarToggle.hasClass('on')) {
            itemStarToggle.removeClass('on').addClass('off');
            $('#reportDetails_section .btnStar').removeClass('on').addClass('off');
            ROE.Api.call('unstar_report', {
                rids: reportID
            }, _starredReportAPISuccessCallback);
        } else {
            itemStarToggle.removeClass('off').addClass('on');
            $('#reportDetails_section .btnStar').removeClass('off').addClass('on');
            ROE.Api.call('star_report', {
                rids: reportID
            }, _starredReportAPISuccessCallback);
        }

    }

    function _starredReportAPISuccessCallback(r) { 
        BDA.Database.Update(CONST.tableName, { id: 'id', fields: r });
    } // end star stuff

    // Gov attacks only stuff
    function _showGovAttacksOnly() {
        // Do nothing if another/current filter active
        if (_isFilterActive())
            return;

        _clearTotalChecked();
        _toggleFilterIcons(false);
        _filterGovAttack = true;
        $('#reportFilterInput').val(""); // incase they were searching for something
        // Piggy back off the filter feature
        $('#filterApplied').html("Gov Attacks Only");
        $('.filterReports .clearFilterBtn').show();
        $('.filterReports .hiddenDuringFilter').hide();
        $('.filterReports .filterTerm').show();
        _loadGovReports().done(_populate);
    }

    // Spy only stuff
    function _showSpyOnly() {
        // Do nothing if another/current filter active
        if (_isFilterActive())
            return;

        _clearTotalChecked();
        _toggleFilterIcons(false);
        _filterSpyOnly = true;
        $('#reportFilterInput').val(""); // incase they were searching for something
        // Piggy back off the filter feature
        $('#filterApplied').html("Spy Ops Only");
        $('.filterReports .clearFilterBtn').show();
        $('.filterReports .hiddenDuringFilter').hide();
        $('.filterReports .filterTerm').show();
        _loadSpyReports().done(_populate);
    }


    function _showSuccessReportsOnly() {
        // Do nothing if another/current filter active
        if (_isFilterActive())
            return;

        _clearTotalChecked();
        _toggleFilterIcons(false);
        _filterSuccess = true;
        $('#reportFilterInput').val(""); // incase they were searching for something
        // Piggy back off the filter feature
        $('#filterApplied').html("Successful Only");
        $('.filterReports .clearFilterBtn').show();
        $('.filterReports .hiddenDuringFilter').hide();
        $('.filterReports .filterTerm').show();
        _loadSuccessReports().done(_populate);
    }

    function _showDefeatReportsOnly() {
        // Do nothing if another/current filter active
        if (_isFilterActive())
            return;

        _clearTotalChecked();
        _toggleFilterIcons(false);
        _filterDefeat = true;
        $('#reportFilterInput').val(""); // incase they were searching for something
        // Piggy back off the filter feature
        $('#filterApplied').html("Defeat Only");
        $('.filterReports .clearFilterBtn').show();
        $('.filterReports .hiddenDuringFilter').hide();
        $('.filterReports .filterTerm').show();
        _loadDefeatReports().done(_populate);
    }


    function _list_onAjaxDataSuccess(r) {     
        BDA.Database.Delete(CONST.tableName).done(_afterCacheRemoved.apply(r));
    }

    function _afterCacheRemoved() {      
        BDA.Database.Insert(CONST.tableName, this, _afterCacheUpdated.apply(this));       
    }

    function _afterCacheUpdated() {
        BDA.Database.LocalSet('ReportsListLoaded', this.length);
        var numReports = this.length;


        if (fromAjaxLoadedQuickerThanFromCache == 1) fromAjaxLoadedQuickerThanFromCache = 2;
        
       if (ROE.isMobile) {
            // Load it all on mobile, we don't use the more button...
           //     _populate(this);
           _populate($.grep(this, function (n, i) { return (i < numReports); }));
       } else {
            _populate($.grep(this, function (n, i) { return (i < CONST.pageLength); }));
        }

        Frame.free(_container);

        $('.list .loading', _container).hide();
    }

    // Useful for regular operation
    function _loadCache(offset) {
        if (ROE.isMobile) {
            // Since using smart loader we want to get the whole list
            return BDA.Database.SelectAll(CONST.tableName);
        } else {
            return _loadQuantityCache(offset, CONST.pageLength);
        }
    }

    // Useful for a single delete
    function _loadOneCache(offset) {
        return _loadQuantityCache(offset, 1);
    }

    // Handle getting various amounts of reports
    function _loadQuantityCache(offset, quantity) {
        var finalOffset = offset + (CONST.pageLength - quantity);
        return BDA.Database.FindRange(CONST.tableName, quantity, finalOffset);
    }   

    function _appendRows(r) {   
        _populateWithItems(r);
        // Check for any pending reports to show.
        _delayedShowNextReport();
    }

    function _populate(r) {
        BDA.Console.log('ROE.Reports', "in _populate. r.length=" + r.length);

        _numItemsInFilter = r.length;
        _updateNumResultsForFilter(r.length);

        // Clears all the existing items
        $('#reports_popup .item').not('.template').remove();

        _populateWithItems(r);
        _gotoList();

    }

    function _gotoList(doSlide) {
        if (doSlide) BDA.UI.Transition.slideRight($('.list', _container), $('.detail', _container));

        if (_filterOn || _starredOn || _filterGovAttack || _filterSuccess || _filterDefeat || _filterSpyOnly || BDA.Database.LocalGet('ReportsListLoaded') <= CONST.pageLength ||
            BDA.Database.LocalGet('ReportsListLoaded') == $('#reports_popup .items .item', _container).length || BDA.Database.LocalGet('ReportsListLoaded') == 0) {
            $(CONST.Selector.moreButton, _container).hide();
        } else {
            $(CONST.Selector.moreButton, _container).show();
            $('.empty', _container).hide();
        }

        if (BDA.Database.LocalGet('ReportsListLoaded') == 0) {
            $('.empty', _container).show();
        } else {
            $('.empty', _container).hide();
        }

        var fs = $('.forward-search', _container).hide();
        $('.selected', fs).val('');
        $('.result, .message', fs).empty();
    }


    function _populateWithItems(itemList) {
        /* 
        var start = new Date().getTime();
       */

        if (ROE.isMobile) {
            $('.list .items').empty();
        }

        var l = itemList.length;
        var staticurl = 'https://static.realmofempires.com/images/';
        var n; // the current item
        var itemData = {}; //item data
        var items = ""; //collection item html for single appending

         // for each item in list
         for (var i = 0; i < l; i++) {
             
             n = itemList[i];

             // prevents failure from corrupted rows. if report has no time, it is corrupted and should be ignored. 
             if (!n.time) {
                 continue;
             }

             itemData = {};
             itemData.subject = n.subject;
             try {
                 itemData.time = BDA.Utils.formatEventTime(BDA.Utils.fromMsJsonDate(n.time), true).capitalizeFirst();
             } catch (e) {
                 var roeex = new BDA.Exception("Error when formatting time", e);
                 roeex.data.add('n', n);
                 roeex.data.add('itemList', itemList);
                 BDA.latestException = roeex;
                 BDA.Console.error(roeex);
                 throw roeex;
             }



             if (n.viewed == 'false' || !n.viewed) {
                 itemData.new = 'new';
                 if (n.flag1 == "Red") {
                     itemData.sound = 'defeatSound';
                 } else if (n.flag1 == "Green") {
                     itemData.sound = 'victorySound';
                 }
             } else {
                 itemData.sound = 'openSound';
             }
        
             itemData.iconSrc = '';

             if (n.whatside != 'NoSide' && n.flag1 != null) {
                 itemData.iconSrc = staticurl + 'icons/M_' + n.whatside + n.flag1 + '.png';
             } else if (n.flag1 == null && n.type == 'Attack') {
                 if (n.flag2 == null) {
                     itemData.iconSrc = staticurl + 'icons/M_SpyFail.png';
                 } else if (n.flag2 == 'Check') {
                     itemData.iconSrc = staticurl + 'icons/M_Spy_Attacker_Success.png';
                 } else if (n.flag2 == 'Exclamation') {
                     itemData.iconSrc = staticurl + 'icons/M_Defender_Warning.png';
                 }
                 itemData.sound = 'openSound';
             } else if (n.type.indexOf('Support') != -1) {
                 itemData.iconSrc = staticurl + 'icons/M_Support.png';
                 itemData.sound = 'openSound';
             } else if (n.type.indexOf('Silver') != -1) {
                 itemData.iconSrc = staticurl + 'icons/M_SilverTransport.png';
                 itemData.sound = 'openSound';
             } else {
                 itemData.iconSrc = staticurl + 'icons/M_Misc.png';
                 itemData.sound = 'openSound';
             }
          
             itemData.forwarddisp = n.forwarded == '0' ? 'hide' : 'show';

             if (n.flag1 != null && n.flag2 != null) {
                 if (!ROE.isMobile) {
                     itemData.flag2src = staticurl + 'icons/M_' + (n.flag2 == 'Check' ? 'Spy_Attacker_Success' : 'Defender_Warning') + '.png';
                 } else {
                     itemData.flag2src = staticurl + 'icons/M_Defender_' + (n.flag2 == 'Check' ? 'success' : 'warning') + '_sml.png';
                 }
             } else {
                 itemData.flag2src = '';
             }

            itemData.flag3src = n.flag3 == null ? '' : 'https://static.realmofempires.com/images/units/Governor_M.png'; //flag3 'gov icon'display             
            itemData.starstate = n.folderID == null ? 'off' : 'on'; //if has folder, its starred
            itemData.id = n.id; //report ID
            itemData.type = n.type; //decides which template to use when opening report

            items += BDA.Templates.populate(_itemTemplate[0].outerHTML, itemData);

         }

         if (ROE.isD2) {
             $('.list .items .more-wrap', _container).before(items);
         } else {
             $('.list .items').append(items);
         }

         //recache items
         _reportItems = $('#reports_popup .items .item');

         if (!ROE.isMobile) { // d2 only
             // Need to reset the width and height of themeM-view for d2
             // or its offset in the dialog box. Not sure why but this seems
             // to fix it (set, unset and set again "refreshes" it to show properly)
             $('#reports_popup .themeM-view').css({'width': '100%','height': '100%'});
         }

         // Use the super fine loader on M
         if (ROE.isMobile) {
             $('#reports_popup .items').unbind("scroll");
             ROE.Frame.smartLoadInit({
                 containerSelector: '#reports_popup .items',
                 itemSelector: '.item',
                 loadSize: 10
             });
         }

        /*
         var end = new Date().getTime();
         var time = end - start;
         alert('Execution: ' + time+'ms');
         */
     }
     
    function _clickedReportItem(e) {
         // Record the method of accessing this report so we know which transition to use.
        _selectedFromList = true;
        e.preventDefault();
        var hotspot = $(e.currentTarget);
        if (hotspot.hasClass("victorySound")) {
            ROE.UI.Sounds.clickVictory();
        } else if (hotspot.hasClass("defeatSound")) {
            ROE.UI.Sounds.clickDefeat();
        } else {
            ROE.UI.Sounds.clickActionOpen();
        }
        _chooseReport(hotspot.parent());       
    }


    function _chooseReport(itemElement, adjustScrollManually) {
        /// Opens the requested report
        /// Based on the html .item jQuery element passed
       
        // Highlight the selected report item
        $('.list .item', _container).removeClass('selected');
        _selectedReportItem = itemElement.addClass('selected');
        _selectedReportItem.toggleClass('new', false);

        if (adjustScrollManually) {
            // Scroll the items window manually so
            // if the user returns they'll see item
            var offset = _selectedReportItem.offset();
            if (offset.top > 275) {
                $('#reports_popup .items').scrollTop(offset.top);
            } else if(offset.top < 86) {
                var scrollPos = $('#reports_popup .items').scrollTop();
                $('#reports_popup .items').scrollTop(scrollPos - 86);
            }
        }

        // Store the id of the report to be loaded.
        // This is needed globally to load the report.
        _selectedReportID = itemElement.attr('rid');

        //set the star btn inside the details section based on the star in the list item
        if ($('.btnStarToggle', itemElement).hasClass('on')) {
            $('#reportDetails_section .btnStar').removeClass('off').addClass('on');
        } else {
            $('#reportDetails_section .btnStar').removeClass('on').addClass('off');
        }
        
        var reportType = itemElement.attr('data-type');
        if (reportType == CONST.ReportType.attack) {
            _attack_loadReport();
        } else if (reportType == CONST.ReportType.supportAttacked) {
            _supportAttacked_loadReport();
        } else {
            // Non specific so we'll use misc report
            _misc_loadReport();
        }     
    }

    // Used to trigger next once the data has been loaded.
    // This is the case when it needs to load more upon next click.
    function _showNextReport(btn) {
        if(btn && btn.hasClass('btnDisabled'))
            return false;

        var reports = _selectedReportItem.nextAll('.item');
        _pendingNextReport = false;

        if (reports.length == 0) {            
            // Next should not get triggered unless there is actually
            // more which should have been determined in _checkNextPrevBtnVisibility
            if ($("#reports_popup .more").length && $("#reports_popup .more").css("display") != "none") {
                // We can grab more reports, so lets do that.
                // Fake a load more...
                Frame.busy(undefined, undefined, _container);
                _pendingNextReport = true;
                _loadMore($('#reports_popup .more'));
            }
        } else {
            // Clear the container, do not transition
            $(CONST.Selector.reportDetailsContainer, _container).empty();
            _selectedFromList = false;
            _chooseReport(reports.first(), true);
        }
    }

    /// Call this once the reports have been appended.
    function _delayedShowNextReport() {
       
        if (_pendingNextReport) {
            Frame.free(_container);
            _pendingNextReport = false;
            _showNextReport();
        }
    }



    function _showPrevReport(btn) {
        if (btn && btn.hasClass('btnDisabled'))
            return false;

        var reports = _selectedReportItem.prevAll('.item');
       
        if (reports.length > 0) {          
            // Clear the container, do not transition
            _selectedFromList = false;          
            _chooseReport(reports.first(), true);
        }
    }

    function _checkNextPrevBtnVisibility() {
        /// Check to see if we should hide any of the next and
        /// prev buttons.
        /// Call everywhere the reports are setup
        $('.btnNextReport').removeClass('btnDisabled');
        $('.btnPrevReport').removeClass('btnDisabled');

        var index = _reportItems.index(_selectedReportItem);

        // Is first item?
        if (index == 0)
            $('.btnPrevReport').addClass('btnDisabled');

        // Is last?
        if (index == (_reportItems.length - 1)) {
            if ($("#reports_popup .more").css("display") == "none") {
                $('.btnNextReport').addClass('btnDisabled');
            }
        } 
    }

    // MISC REPORTS
    function _misc_loadReport() {  
        BDA.Database.Find(CONST.tableName, "id", [_selectedReportID])
            .done(_misc_onLoadFromDBSuccess);
        // .fail(_misc_onLoadFromDBFail);   // see case 29598    
    }

    function _misc_onLoadFromDBSuccess(r) {
        // length = 0, there was nothing found (error?)
        if (!r || r.length == 0) {            
            _misc_loadFromApi();      
        } else if (!r[0].detailsjson) {           
            _misc_loadFromApi();
        } else {
            BDA.Database.Update(CONST.tableName, { id: 'id', fields: [{ 'id': _selectedReportID, 'viewed': 'true' }] }, _db_updateSuccess);
            _populateMiscReport(JSON.parse(r[0].detailsjson));
        }
    }

    // see case 29598
    //function _misc_onLoadFromDBFail() {
    //    _misc_loadFromApi()
    //}

    function _misc_loadFromApi() {
        Frame.busy(undefined, undefined, _container); // Gonna take a bit, throw up the busy sign
        Api.call_report_miscreport(_selectedReportID, _misc_onApiDataSyncSuccess);
    }

    function _misc_onApiDataSyncSuccess(data) {
        // The assumption is that it already exists
        BDA.Database.Update(CONST.tableName, { id: 'id', fields: [{ 'id': _selectedReportID, 'viewed': 'true', 'detailsjson': JSON.stringify(data) }] }, _db_updateSuccess);

        _populateMiscReport(data);
        Frame.free(_container);
    }

    function _populateMiscReport(reportData) {
        // Use the template and id that was last stored to display this report
        var data;

        data = {};        
        data.subject = reportData.Subject;
        data.reporttime = BDA.Utils.formatEventTime(BDA.Utils.fromMsJsonDate(reportData.ReportTime), true).capitalizeFirst();
        data.description = reportData.Description;
        if (reportData.ForwardedBy) {
            data.forwardedPlayerName = reportData.ForwardedBy.ForwardedPlayerName;
            data.forwardedOn = BDA.Utils.formatEventTime(BDA.Utils.fromMsJsonDate(reportData.ForwardedBy.ForwardedOn), true).capitalizeFirst();
            // Note: We will turn on the forwarded section after the template is populated and returned.
        }

        if (_selectedReportItem) {
            data.reportTitle = ($(_selectedReportItem).attr('data-type')).toUpperCase();
            if (data.reportTitle == "MISC.")
                data.reportTitle = CONST.reportDetailPhrases.TitleReport;
        } else {
            data.reportTitle = "";
        }

        var content = BDA.Templates.populate($(CONST.Selector.miscReportPanel, CACHE.reportDetailsTemplate)[0].outerHTML, data);
        content = $(content);

        

        // Display the forwarded section if we had
        // forward content.
        if (reportData.ForwardedBy) {
            var forwardedObj = CACHE.forwardedTemplate.clone();
            var forwardData = {};
            if (ROE.isSpecialPlayer(reportData.ForwardedBy.ForwardedPlayerID)) {
                forwardData.forwardedByAvatar = "";
                $('.forward-name', forwardedObj).removeClass('player-name').removeAttr('target').attr('href', '#');
                $('.forwardedPlayerAvatar', forwardedObj).css('background-image', 'none');
            } else {
                if (ROE.Avatar.list[reportData.ForwardedBy.ForwardedPlayerAvatarID]) {
                    forwardData.forwardedByAvatar = ROE.Avatar.list[reportData.ForwardedBy.ForwardedPlayerAvatarID].imageUrlS;
                } else {
                    forwardData.forwardedByAvatar = "";
                    $('.forwardedPlayerAvatar', forwardedObj).css('background-image', 'none');
                    BDA.Console.error("Forward avatars unavailable. Using blank avatar.");
                }
            }
            forwardData.forwardedByID = reportData.ForwardedBy.ForwardedPlayerID;
            forwardData.forwardedByName = reportData.ForwardedBy.ForwardedPlayerName;
            forwardData.forwardedTime = BDA.Utils.formatEventTime(BDA.Utils.fromMsJsonDate(reportData.ForwardedBy.ForwardedOn), true);
            var forwardedHTML = BDA.Templates.populate(forwardedObj[0].outerHTML, forwardData);
            $('.reportDetailTop', content).prepend(forwardedHTML);
        }


        $(CONST.Selector.reportDetailsContainer, _container).empty().append(content);
        $(CONST.Selector.reportDetailsView, _container).show();

        // Hook in player profile popups
        $('.reportDetailsView .player-name', _container).click(function (e) {
            e.preventDefault(); ROE.Frame.popupPlayerProfile($(e.currentTarget).html());
        });

        // Hook in the village profile popups
        $('.miscDescription', _container).find('a').each(function () {

            var href = $(this).attr('href');
            
            if (href.indexOf("Village") != -1) {
                var ovid = href.substring(href.indexOf("ovid") + 5);
                $(this).attr('data-vid', ovid);
                $(this).click(function (e) {
                    e.preventDefault();
                    ROE.Frame.popupVillageProfile($(e.currentTarget).attr('data-vid'));
                });
            } else if (href.indexOf("player") != -1) {
                var pname = href.substring(href.indexOf("pname") + 6);
                $(this).attr('data-pname', pname);
                $(this).click(function (e) {
                    e.preventDefault();
                    ROE.Frame.popupPlayerProfile($(e.currentTarget).attr('data-pname'));

                });
            } else {
                $(this).replaceWith($(this).text());
            }
        });

        var clanInvitesAreas = $('.miscDescription', _container).find('.jsReportType-ClanInvite');
        if (clanInvitesAreas.length > 0) {

            // Hook in the clan popups
            if (ROE.isD2) {
                clanInvitesAreas.replaceWith('<a href=# onClick="ROE.Frame.popupClan(0, 0, \'overview\')" >Click to see all your invites</a>'
                         + '<BR><a href=#  onClick="ROE.Frame.popupClan(' + $(clanInvitesAreas[0]).attr('data-clanid') + ')" >Click to learn about this clan</a> ');
            } else {
                clanInvitesAreas.replaceWith('<a href=# onClick="ROE.Frame.popupClan()" >TAP to see all your invites</a>'
                         + '<BR><a href=#  onClick="ROE.Frame.popupClan(' + $(clanInvitesAreas[0]).attr('data-clanid') + ')" >TAP to learn about this clan</a> ');
            }
        }


        // Call here once everything has been appended.
        _handleBlockForwardedPlayer(_container);
        _checkNextPrevBtnVisibility();

        if(_selectedFromList)
            BDA.UI.Transition.slideLeft($(CONST.Selector.reportDetailsView, _container), $('.default', _container));
    }



    // SUPPORT ATTACKED REPORT
    function _supportAttacked_loadReport() {
        BDA.Database.Find(CONST.tableName, "id", [_selectedReportID])
            .done(_supportAttacked_onLoadFromDBSuccess)
            .fail(_supportAttacked_onLoadFromDBFail);
    }

    function _supportAttacked_onLoadFromDBSuccess(r) {
       
        if (r.length == 0) { 
            _supportAttacked_loadFromApi();   
        } else if (!r[0].detailsjson) {
            _supportAttacked_loadFromApi();
        } else {
            BDA.Database.Update(CONST.tableName, { id: 'id', fields: [{ 'id': _selectedReportID, 'viewed': 'true' }] }, _db_updateSuccess);
            _populateSupportAttackedReport(JSON.parse(r[0].detailsjson));
        }
    }

    function _supportAttacked_onLoadFromDBFail() {       
        _supportAttacked_loadFromApi()
    }

    function _supportAttacked_loadFromApi() {
        Frame.busy(undefined, undefined, _container); // Gonna take a bit, throw up the busy sign
        Api.call_report_supportattackedreport(_selectedReportID, _supportAttacked_onApiDataSyncSuccess);
    }

    function _supportAttacked_onApiDataSyncSuccess(data) {       
        BDA.Database.Update(CONST.tableName, { id: 'id', fields: [{ 'id': _selectedReportID, 'viewed': 'true', 'detailsjson': JSON.stringify(data) }] }, _db_updateSuccess);
        _populateSupportAttackedReport(data);
        Frame.free(_container);
    }

    function _populateSupportAttackedReport(reportData) {

        // Use the template and id that was last stored to display this report
        var data;
        var supportAttackedReportPanelObj = CACHE.supportAttackedReportTemplate.clone();

        data = {};
        data.subject = reportData.Subject;
        data.reporttime = BDA.Utils.formatEventTime(BDA.Utils.fromMsJsonDate(reportData.ReportTime), true).capitalizeFirst();

        data.supportedPlayerID = reportData.SupportedPlayerID;
       
        if (ROE.isSpecialPlayer(reportData.SupportedPlayerID)) {          
            data.supportedPlayerAvatar = "";
            $('.support-name.nameOfPlayer', supportAttackedReportPanelObj).removeClass('player-name').removeAttr('target').attr('href', '#').addClass('doNotShow');
            $('.supportedAvatar ', supportAttackedReportPanelObj).removeClass('player-name').addClass("noPointer");
            $('.supportedView .playerIconWrapper', supportAttackedReportPanelObj).addClass('dull');

            if (ROE.isSpecialPlayer_Rebel(reportData.SupportedPlayerID)) {
                data.supportedPlayerAvatar = "https://static.realmofempires.com/images/npc/Av_Rebel.png";
            } else if (ROE.isSpecialPlayer_Abandoned(reportData.SupportedPlayerID)) {
                data.supportedPlayerAvatar = "https://static.realmofempires.com/images/npc/Av_Abandoned.png";
            } else {
                $('.supportedAvatar', supportAttackedReportPanelObj).css('background-image', 'none').addClass('doNotShow');
            }

        } else {
           
            if (ROE.Avatar.list[reportData.SupportedAvatarID]) {                
                data.supportedPlayerAvatar = ROE.Avatar.list[reportData.SupportedAvatarID].imageUrlS;
            } else {
               
                // Could be an unkown player, so don't send error
                $('.support-name.nameOfPlayer', supportAttackedReportPanelObj).removeClass('player-name').removeAttr('target').attr('href', '#').addClass('doNotShow');
                $('.supportedAvatar ', supportAttackedReportPanelObj).removeClass('player-name');
                $('.supportedView .playerIconWrapper', supportAttackedReportPanelObj).addClass('dull');

                data.supportedPlayerAvatar = "https://static.realmofempires.com/images/npc/Av_Unknown.png";              
            }
        }

        data.supportedPlayerName = reportData.SupportedPlayerName;
        data.supportedVillageID = reportData.SupportedVillageID;
        data.supportedVillageImage = BDA.Utils.GetMapVillageIconUrl(reportData.SupportedVillagePoints, reportData.SupportedVillageTypeID);
        data.supportedVillageName = reportData.SupportedVillageName;
        data.supportedVillageTypeID = reportData.SupportedVillageTypeID;
        data.supportedVillagePoints = reportData.SupportedVillagePoints;
        data.supportedVillageX = reportData.SupportedVillageX;
        data.supportedVillageY = reportData.SupportedVillageY;

        if (reportData.ForwardedBy) {
            data.forwardedPlayerName = reportData.ForwardedBy.ForwardedPlayerName;
            data.forwardedOn = BDA.Utils.formatEventTime(BDA.Utils.fromMsJsonDate(reportData.ForwardedBy.ForwardedOn), true);
            // Note: We will turn on the forwarded section after the template is populated and returned.
        }

        if (_selectedReportItem) {
            data.reportTitle = ($(_selectedReportItem).attr('data-type')).toUpperCase();
        } else {
            data.reportTitle = "";
        }

        var content = BDA.Templates.populate(supportAttackedReportPanelObj[0].outerHTML, data);

        content = $(content);

        var supportVillageRows = "";
        $.each(reportData.VillagesSupporting, function (i, support) {
            var supportData = {};
            supportData.villageName = support.name;
            supportData.villageImage = 'https://static.realmofempires.com/images/map/VillBrecruitall8a.png';
            supportData.villageID = support.id;
            supportData.villageX = support.x;
            supportData.villageY = support.y;

            var supportingVillageHTML = BDA.Templates.populate(CACHE.versusTemplate[0].outerHTML, supportData);
            supportVillageRows += supportingVillageHTML + _getUnitCountTable([], support.units, true) + "<div class='sectionDivider'></div>";

        });

        $('.reportTables', content).empty().append(supportVillageRows);

        // Display the forwarded section if we had
        // forward content.
        if (reportData.ForwardedBy) {
            var forwardedObj = CACHE.forwardedTemplate.clone();
            var forwardData = {};
            if (ROE.isSpecialPlayer(reportData.ForwardedBy.ForwardedPlayerID)) {
                forwardData.forwardedByAvatar = "";
                $('.forward-name', forwardedObj).removeClass('player-name').removeAttr('target').attr('href', '#');
                $('.forwardedPlayerAvatar', forwardedObj).css('background-image', 'none');
            } else {
                if (ROE.Avatar.list[reportData.ForwardedBy.ForwardedPlayerAvatarID]) {
                    forwardData.forwardedByAvatar = ROE.Avatar.list[reportData.ForwardedBy.ForwardedPlayerAvatarID].imageUrlS;
                } else {
                    forwardData.forwardedByAvatar = "";
                    $('.forwardedPlayerAvatar', supportAttackedReportPanelObj).css('background-image', 'none');
                    BDA.Console.error("Forward avatars unavailable. Using blank avatar.");
                }
            }
            forwardData.forwardedByID = reportData.ForwardedBy.ForwardedPlayerID;
            forwardData.forwardedByName = reportData.ForwardedBy.ForwardedPlayerName;
            forwardData.forwardedTime = BDA.Utils.formatEventTime(BDA.Utils.fromMsJsonDate(reportData.ForwardedBy.ForwardedOn), true);
            var forwardedHTML = BDA.Templates.populate(forwardedObj[0].outerHTML, forwardData);
            $('.reportDetailTop', content).prepend(forwardedHTML);
        }


        $(CONST.Selector.reportDetailsContainer, _container).empty().append(content);
        $(CONST.Selector.reportDetailsView, _container).show();

        // Hook in player profiles
        $('.reportDetailsView .player-name', _container).click(function (e) {
            ROE.UI.Sounds.click();
            e.preventDefault(); ROE.Frame.popupPlayerProfile($(e.currentTarget).attr('data-pn'));
        });
        // Hook in village profiles
        $('.reportDetailsView .village-name', _container).click(function (e) {
            ROE.UI.Sounds.click();

        });

        // Call here once everything has been appended.
        _handleBlockForwardedPlayer(_container);
        _checkNextPrevBtnVisibility();

        if (_selectedFromList)
            BDA.UI.Transition.slideLeft($(CONST.Selector.reportDetailsView, _container), $('.default', _container));
    }



    // ATTACK REPORT
    function _attack_loadReport() {
        // First, let's see if its in the database (i.e. already loaded)
        BDA.Database.Find(CONST.tableName, "id", [parseInt(_selectedReportID)])
            .done(_attack_onLoadFromDBSuccess)
            .fail(_attack_onLoadFromDBFail);
    }

    function _attack_onLoadFromDBSuccess(r) {
        if (r.length == 0) {     
            _attack_loadFromApi();      
        } else if (!r[0].detailsjson) {
            _attack_loadFromApi();
        } else {
            BDA.Database.Update(CONST.tableName, { id: 'id', fields: [{ 'id': _selectedReportID, 'viewed': 'true' }] }, _db_updateSuccess);
            _populateAttackReport(JSON.parse(r[0].detailsjson));
        }
    }

    function _attack_onLoadFromDBFail() {
        _attack_loadFromApi()
    }

    function _attack_loadFromApi() {
        Frame.busy(undefined, undefined, _container); // Gonna take a bit, throw up the busy sign
        Api.call_report_battlereport(_selectedReportID, _attack_onApiDataSyncSuccess);
    }

    function _attack_onApiDataSyncSuccess(data) {      
        BDA.Database.Update(CONST.tableName, { id: 'id', fields: [{ 'id': _selectedReportID, 'viewed': 'true', 'detailsjson': JSON.stringify(data) }] }, _db_updateSuccess);
        _populateAttackReport(data);
        Frame.free(_container);
    }

    function _populateAttackReport(reportData) {

        // Use the template and id that was last stored to display this report
        var data;
        var attackReportPanelObj = CACHE.attackReportTemplate.clone();
        

        data = {};
        data.reportId = _selectedReportID; // needed for BattleSim link
        // Title and time
        data.subject = reportData.Subject;
        data.reporttime = BDA.Utils.formatEventTime(BDA.Utils.fromMsJsonDate(reportData.ReportTime), true).capitalizeFirst();

        // Players and villages
        data.attackerVillageName = reportData.attackerVillageName;
        data.attackerVillageID = reportData.attackerVillageID;
        data.attackerVillageXCord = reportData.attackerVillageXCord;
        data.attackerVillageYCord = reportData.attackerVillageYCord;
        data.attackerVillageIcon = BDA.Utils.GetMapVillageIconUrl(reportData.AttackerVillagePoints, reportData.AttackerVillageTypeID); //"https://static.realmofempires.com/images/map/VillBwall10a.png";
        data.attackerVillageNote = reportData.AttackerVillageNote;
        data.attackerPlayerName = reportData.AttackerPlayerName; 
        data.attackerPlayerID = reportData.attackerPlayerID;
        
        if (ROE.isSpecialPlayer(reportData.attackerPlayerID)) {            
            data.attackerPlayerAvatar = "";
            $('.attacker-name.nameOfPlayer', attackReportPanelObj).removeClass('player-name').removeAttr('target').attr('href', '#').addClass('doNotShow');
            $('.brAfterAttacker', attackReportPanelObj).addClass('doNotShow');
            $('.attackerAvatar ', attackReportPanelObj).removeClass('player-name').addClass("noPointer");
            $('.leftSide .playerIconWrapper', attackReportPanelObj).addClass('dull');
           
            if (ROE.isSpecialPlayer_Rebel(reportData.attackerPlayerID)) {
                data.attackerPlayerAvatar = "https://static.realmofempires.com/images/npc/Av_Rebel.png";
            } else if(ROE.isSpecialPlayer_Abandoned(reportData.attackerPlayerID)) {
                data.attackerPlayerAvatar = "https://static.realmofempires.com/images/npc/Av_Abandoned.png";                    
            } else {
                $('.attackerAvatar', attackReportPanelObj).css('background-image', 'none').addClass('doNotShow');
            }

        } else {
           
            if (ROE.Avatar.list[reportData.AttackerAvatarID]) {
                data.attackerPlayerAvatar = ROE.Avatar.list[reportData.AttackerAvatarID].imageUrlS;
            } else {               
                // Could be an unkown player, so don't send error
                $('.attacker-name.nameOfPlayer', attackReportPanelObj).removeClass('player-name').removeAttr('target').attr('href', '#').addClass('doNotShow');
                $('.brAfterAttacker', attackReportPanelObj).addClass('doNotShow');
                $('.attackerAvatar ', attackReportPanelObj).removeClass('player-name').addClass("noPointer");
                $('.attackerVillage ', attackReportPanelObj).removeClass('village-name').addClass("noPointer").removeClass('jsV');
                $('.leftSide .playerIconWrapper, .leftSide .villageIconShiner', attackReportPanelObj).addClass('dull');

                data.attackerPlayerAvatar = "https://static.realmofempires.com/images/npc/Av_Unknown.png";                
            }
        }
      
        data.attackerPlayerNote = reportData.AttackerPlayerNote;

        data.defenderVillageName = reportData.DefenderVillageName;
        data.defenderVillageID = reportData.DefenderVillageID;
        data.defenderVillageXCord = reportData.DefenderVillageXCord;
        data.defenderVillageYCord = reportData.DefenderVillageYCord;
        data.defenderVillageIcon = BDA.Utils.GetMapVillageIconUrl(reportData.DefenderVillagePoints, reportData.DefenderVillageTypeID); // "https://static.realmofempires.com/images/map/VillBsilver9a.png";
        data.defenderVillageNote = reportData.DefenderVillageNote;
        data.defenderPlayerName = reportData.DefenderPlayerName;
        data.defenderPlayerID = reportData.DefenderPlayerID;
        if (ROE.isSpecialPlayer(reportData.DefenderPlayerID)) {
            data.defenderPlayerAvatar = "";
            $('.defender-name.nameOfPlayer', attackReportPanelObj).removeClass('player-name').removeClass('player-name').removeAttr('target').attr('href', '#').addClass('doNotShow');
            $('.defenderAvatar ', attackReportPanelObj).removeClass('player-name').addClass("noPointer");
            $('.brAfterDefender', attackReportPanelObj).addClass('doNotShow');
            $('.rightSide .playerIconWrapper', attackReportPanelObj).addClass('dull');

            if (ROE.isSpecialPlayer_Rebel(reportData.DefenderPlayerID)) {
                data.defenderPlayerAvatar = "https://static.realmofempires.com/images/npc/Av_Rebel.png";
            } else if(ROE.isSpecialPlayer_Abandoned(reportData.DefenderPlayerID)) {
                data.defenderPlayerAvatar = "https://static.realmofempires.com/images/npc/Av_Abandoned.png";                    
            } else {
                $('.defenderAvatar', attackReportPanelObj).css('background-image', 'none').addClass('doNotShow');
            }

        } else {
            if (ROE.Avatar.list[reportData.DefenderAvatarID]) {
                data.defenderPlayerAvatar = ROE.Avatar.list[reportData.DefenderAvatarID].imageUrlS;
            } else {
                
                $('.defender-name.nameOfPlayer', attackReportPanelObj).removeClass('player-name').removeAttr('target').attr('href', '#').addClass('doNotShow');
                $('.defenderAvatar ', attackReportPanelObj).removeClass('player-name').addClass("noPointer");
                $('.brAfterDefender', attackReportPanelObj).addClass('doNotShow');
                $('.defenderVillage ', attackReportPanelObj).removeClass('village-name').addClass("noPointer").removeClass('jsV');
                $('.rightSide .playerIconWrapper, .rightSide .villageIconShiner', attackReportPanelObj).addClass('dull');

                data.defenderPlayerAvatar = "https://static.realmofempires.com/images/npc/Av_Unknown.png";
            }
        }
    
        data.defenderPlayerNote = reportData.DefenderPlayerNote;
        
        // Determine the icon to show in the versus matchup
        var staticurl = 'https://static.realmofempires.com/images/';
        var colourArray = ["Green", "Yellow", "Red"];
        data.vsIconImageUrl = "";
        if (reportData.Flag1 >= 0) {            
            if(reportData.IsAttacker) {
                data.vsIconImageUrl = staticurl + 'icons/M_Attack' + colourArray[reportData.Flag1] + '.png';
            } else {
                data.vsIconImageUrl = staticurl + 'icons/M_Defend' + colourArray[reportData.Flag1] + '.png';
            }
        } else {
           
            if (reportData.Flag2 < 0 && reportData.IsAttacker) {
                data.vsIconImageUrl = staticurl + 'icons/M_SpyFail.png';
            } else if (reportData.Flag2 == 0) {
                data.vsIconImageUrl = staticurl + 'icons/M_Spy_Attacker_Success.png';

            } else  if (reportData.Flag2 == 1) {
                data.vsIconImageUrl = staticurl + 'icons/M_Defender_Warning.png';                 
            } 
        } 

        if (data.vsIconImageUrl == "")
            $('vsIcon', attackReportPanelObj).hide();

        // Handle notes
        // On click events are included on the html elements in the template.

        // Need to show both if even one has note to keep layout even and symmetrical
        if (data.attackerVillageNote == "" && data.defenderVillageNote == "") {
            $('.note.attackerVillageNote', attackReportPanelObj).addClass('hideNote');
            $('.attacker.villageInlineNote', attackReportPanelObj).hide();
            $('.note.defenderVillageNote', attackReportPanelObj).addClass('hideNote');
            $('.defender.villageInlineNote', attackReportPanelObj).hide();
        }
        // However, we do need to make the empty note not clickable (i.e. don't show a hover state)
        if (data.attackerVillageNote == "") {
            $('.attacker.villageInlineNote', attackReportPanelObj).addClass('notClickable');
        }
        if (data.defenderVillageNote == "") {
            $('.defender.villageInlineNote', attackReportPanelObj).addClass('notClickable');
        }

        // Do the same as village for player notes.
        if (data.attackerPlayerNote == "" && data.defenderPlayerNote == "") {
            $('.note.attackerPlayerNote', attackReportPanelObj).addClass('hideNote');
            $('.attacker.playerInlineNote', attackReportPanelObj).hide();
            $('.note.defenderPlayerNote', attackReportPanelObj).addClass('hideNote');
            $('.defender.playerInlineNote', attackReportPanelObj).hide();
        }

        if (data.attackerPlayerNote == "") {
            $('.attacker.playerInlineNote', attackReportPanelObj).addClass('notClickable');
        }
        if (data.defenderPlayerNote == "") {
            $('.defender.playerInlineNote', attackReportPanelObj).addClass('notClickable');
        }

        // Turned off the beta feature, will be for all now...
        // Do not show the inline notes unless the beta feature is on as
        // it is not ready yet
        //if (BetaFeatures.status('AdvancedReportList') != 'ON') {
        //    $('.playerInlineNote, .villageInlineNote', attackReportPanelObj).hide();
        //}


        // Default title (in case all else fails)
        data.reportTitle = CONST.reportDetailPhrases.TitleReport;

        if (reportData.Flag2 == 0) {
            $('.titleBanner', attackReportPanelObj).addClass('successBanner');
            data.reportTitle = CONST.reportDetailPhrases.TitleSuccess;
        } else {

            if (reportData.Flag1 == 0) { // Green
                $('.titleBanner', attackReportPanelObj).addClass('victoryBanner');
                data.reportTitle = CONST.reportDetailPhrases.TitleVictory;
            } else if (reportData.Flag1 == 1) { // Yellow
                $('.titleBanner', attackReportPanelObj).addClass('victoryBanner');
                data.reportTitle = CONST.reportDetailPhrases.TitleVictory;
            } else if (reportData.Flag1 == 2) { // Red
                $('.titleBanner', attackReportPanelObj).addClass('defeatBanner');
                data.reportTitle = CONST.reportDetailPhrases.TitleDefeat;
            }            
        }

        // Some additional special cases:
        if (reportData.Flag1 < 0) {
            if (reportData.Flag2 < 0) {
                if (reportData.SpyOnlyAttack) {
                    if (reportData.IsAttacker) {
                        // Your spies were unsuccessful
                        $('.titleBanner', attackReportPanelObj).addClass('defeatBanner');
                        data.reportTitle = CONST.reportDetailPhrases.TitleDefeat;
                    } else {
                        //  You were spied upon BUT you stopped the spy attack
                        $('.titleBanner', attackReportPanelObj).addClass('successBanner');
                        data.reportTitle = CONST.reportDetailPhrases.TitleSuccess;
                    }
                }
            } else if (reportData.Flag2 == 1) {                
                if (reportData.SpyOnlyAttack && !reportData.IsAttacker) {
                    // You were spied upon (and identity probably unknown?)
                    $('.titleBanner', attackReportPanelObj).addClass('defeatBanner');
                    data.reportTitle = CONST.reportDetailPhrases.TitleWarning;
                }
            }
        }

        // Loyalty Change
        if (reportData.LoyaltyBeforeAttack) { // not null

            var newLoyalty = (reportData.LoyaltyBeforeAttack - reportData.LoyaltyChange);

            data.afterYeaPercentage = newLoyalty + "%";
            data.beforeYeaPercentage = reportData.LoyaltyBeforeAttack + "%";

            data.barColorR = 66;
            data.barColorG = 66;
            data.barColorB = 66;

            var diff = (newLoyalty - 40);
            if (diff < 0)
                diff = 0;
            
            if (diff < 30) {
                // work on red
                data.barColorR = 235;
                data.barColorG = 235 - Math.floor((1 - diff / 30) * 213);                
            } else {
                // work on green
                data.barColorR = 22 + Math.floor((1 - (diff - 30) / 30) * 213);
                data.barColorG = 235;
            }
            $('.loyalty', attackReportPanelObj).show();

        }

        // Units table
        // Populate the unit table
        var unknownRight = false;
        var unknownLeft = false;
        // Defender:
        if (!reportData.IsAttacker) {
            // this player is the defender
            // their troops will be on the right.
            unknownRight = false; // player can always see their own troops
            unknownLeft = false;
        } else {
            // this player is the attacker
            // their troops will be on the left
            unknownRight = !reportData.CanAttackerSeeDefendingTroops; // 
            unknownLeft = false; // player can always see their own troops
        }

        $('.reportTables', attackReportPanelObj).empty().append(_getUnitCountTable(reportData.AttackerUnits, reportData.DefenderUnits, unknownLeft, unknownRight));

        if (reportData.IsAttacker && !reportData.CanAttackerSeeDefendingTroops) {
            if (!reportData.SpyOnlyAttack) {
                $('.cannotSeeNoSurvivingTroops', attackReportPanelObj).show();
            } 
        }

        if (reportData.Morale > -32768) {
            $('.moraleInfo', attackReportPanelObj).html('Troop Morale: ' + reportData.Morale ).show();
        }

        // Battle Briefing  
        var briefingObj = $('.briefingText', attackReportPanelObj);

        if (reportData.Plunder > 0) {
            $('.plunder', attackReportPanelObj).empty().append("<p class='fontTanFrLCmed'>" + CONST.reportDetailPhrases.Plunder + "<br /><img src='https://static.realmofempires.com/images/icons/Q_Silver.png'><span  class='fontWhiteNumbersSm'>" + BDA.Utils.formatNum(reportData.Plunder) + "</span></p>");
        }
        
        if (reportData.BuildingsAttacked !== null) {
            var damagedBuildingsHTML = "";
            $.each(reportData.BuildingsAttacked, function (k, building) {

                var damagedBuildingData = {};
                damagedBuildingData.damagedBuildingIcon = ROE.Entities.BuildingTypes[building.BuildingID].IconUrl_ThemeM;
                damagedBuildingData.damangedBuildingName = ROE.Entities.BuildingTypes[building.BuildingID].Name;
                damagedBuildingData.damagedOldLevel = building.BeforeAttackLevel;
                damagedBuildingData.damagedNewLevel = building.AfterAttackLevel;
                damagedBuildingsHTML += BDA.Templates.populate(CACHE.damagedBuildingTemplate[0].outerHTML, damagedBuildingData);
            });

            $('.damagedBuildings', attackReportPanelObj).empty().append(damagedBuildingsHTML);
        }

        if (reportData.SpyOutcome != -1) { // There were spies in attack
            // Spy Report //
            var levelColumnObj = $('.spyReport .levelColumnContainer', attackReportPanelObj);
            var spyReportObj = $('.spyReport', attackReportPanelObj);
            if (reportData.SpyOutcome == 1) { // Spies are successful

                // Show silver in treasury of opponent
                if (reportData.DefendersCoins >= 0) {
                    $('.rightSide', briefingObj).empty().append("<p class='fontTanFrLCmed'>" + CONST.reportDetailPhrases.SilverInTreasury + "<br /><span  class='fontWhiteNumbersMed'>" + BDA.Utils.formatNum(reportData.DefendersCoins) + "</span> " + CONST.reportDetailPhrases.Silver + "</p>");

                    if (reportData.Plunder > 0) {
                        $('.leftSide', briefingObj).empty().append("<p class='fontTanFrLCmed'>" + CONST.reportDetailPhrases.Plunder + "<br /><span  class='fontWhiteNumbersMed'>" + BDA.Utils.formatNum(reportData.Plunder) + "</span> " + CONST.reportDetailPhrases.Silver + "</p>");
                        $('.plunder', attackReportPanelObj).hide();
                    }
                    briefingObj.show();
                }

                $('.spiedVillageImage', attackReportPanelObj).css('background-image', 'url(' + data.defenderVillageIcon + ')');

                var html = "";
                var buildingLevelHTML = CACHE.buildingLevelTemplate[0].outerHTML;
                $.each(reportData.BuildingIntel, function (k, v) {
                    var building = ROE.Entities.BuildingTypes[k];
                    if (reportData.BuildingIntel[building.ID]) { //undefined if not
                        html += BDA.Templates.populate(buildingLevelHTML, { buildingIcon: building.IconUrl_ThemeM, buildingName: building.Name,  levelValue: v });
                    } else {
                        html += BDA.Templates.populate(buildingLevelHTML, { buildingIcon: building.IconUrl_ThemeM, buildingName: building.Name, levelValue: "0" });
                    }
                });
                
                $(levelColumnObj).append(html);
                spyReportObj.show();

            } else {
                // Spies failed
                // Don't show building panel
                spyReportObj.hide();
            }

            if (!reportData.IsAttacker) {
                // Point of view of the defender

                if (!reportData.DoesDefenderKnownsAttackersIdentity) {
                    data.attackerPlayerName = CONST.reportDetailPhrases.Unknown;
                    data.attackerPlayerID = 0;
                    data.attackerPlayerAvatar = "https://static.realmofempires.com/images/npc/Av_Unknown.png";
                    data.attackerVillageName = CONST.reportDetailPhrases.Unknown;
                    data.attackerVillageID = 0;
                    data.attackerVillageXCord = "?";
                    data.attackerVillageYCord = "?";
                    $('.leftSide .village-name', attackReportPanelObj).removeClass('village-name').removeAttr('onclick');
                    $('.leftSide .player-name', attackReportPanelObj).removeClass('player-name').removeAttr('onclick');
                    $('.cannotSeeSpyNotSuccessful', attackReportPanelObj).show();
                    // TODO: Publish to Facebook here?
                    
                } else {
                    // TODO: Publish to Facebook here?
                }

                if (reportData.SpyOutcome == 1) {
                    $('.youGotSpiedSuccess', attackReportPanelObj).show();
                } else {
                    $('.youGotSpiedFailed', attackReportPanelObj).show();
                }

            } else {
                // Point of view of the attacker
                if (reportData.SpyOnlyAttack) {
                    if (reportData.SpyOutcome == 1) { // Spies successful
                        if (reportData.DoesDefenderKnownsAttackersIdentity) {
                            $('.spiesSuccessIdentityKnown', attackReportPanelObj).show();
                        } else {
                            $('.spiesSuccessIdentityUnknown', attackReportPanelObj).show();
                        }
                    } else { // Spies unsuccessful
                        if (reportData.DoesDefenderKnownsAttackersIdentity) {
                            $('.spiesFailedIdentityKnown', attackReportPanelObj).show();
                        } else {
                            $('.spiesFailedIdentityUnknown', attackReportPanelObj).show();
                        }
                    }
                } else {
                    // Not only spies were attacking
                    if (reportData.SpyOutcome == 1) { // Spies successful                        
                        $('.spiesSuccessfulWithInfo', attackReportPanelObj).show();
                    } else {
                        $('.spiesUnsuccessfulWithNoInfo', attackReportPanelObj).show();
                    }
                }

                // Handle bonus villages
                if (reportData.SpyOutcome == 1) {
                    if (reportData.DefenderVillageTypeID != 0) {
                        $('.bonusVillageName', attackReportPanelObj).empty().append(ROE.Entities.VillageTypes[reportData.DefenderVillageTypeID].Name);
                        $('.bonusVillage', attackReportPanelObj).show();
                    }
                }
            }

            $('.spyStatusMessage', attackReportPanelObj).show();

        } else {
            // No spies in attack
        }

        if (reportData.ForwardedBy) {
            var forwardedObj = CACHE.forwardedTemplate.clone();
            var forwardData = {};
            if (ROE.isSpecialPlayer(reportData.ForwardedBy.ForwardedPlayerID)) {
                forwardData.forwardedByAvatar = "";
                $('.forward-name', forwardedObj).removeClass('player-name').removeAttr('target').attr('href', '#');
                $('.forwardedPlayerAvatar', forwardedObj).css('background-image', 'none');
            } else {
                if (ROE.Avatar.list[reportData.ForwardedBy.ForwardedPlayerAvatarID]) {
                    forwardData.forwardedByAvatar = ROE.Avatar.list[reportData.ForwardedBy.ForwardedPlayerAvatarID].imageUrlS;
                } else {
                    forwardData.forwardedByAvatar = "";
                    $('.forwardedPlayerAvatar', attackReportPanelObj).css('background-image', 'none');
                    BDA.Console.error("Forward avatars unavailable. Using blank avatar.");
                }
            }
            forwardData.forwardedByID = reportData.ForwardedBy.ForwardedPlayerID;
            forwardData.forwardedByName = reportData.ForwardedBy.ForwardedPlayerName;
            forwardData.forwardedTime = BDA.Utils.formatEventTime(BDA.Utils.fromMsJsonDate(reportData.ForwardedBy.ForwardedOn), true);
            var forwardedHTML = BDA.Templates.populate(forwardedObj[0].outerHTML, forwardData);
            $('.reportDetailTop', attackReportPanelObj).prepend(forwardedHTML);
            _handleBlockForwardedPlayer(attackReportPanelObj);
        }



        // Finally populate the remaining report data
        var content = BDA.Templates.populate(attackReportPanelObj[0].outerHTML, data);
        content = $(content);

        $(CONST.Selector.reportDetailsContainer, _container).empty().append(content);
        $(CONST.Selector.reportDetailsView, _container).show();


        // Hook in player profile popup
        $('.reportDetailsView .player-name', _container).click(function (e) {
            ROE.UI.Sounds.click();
            e.preventDefault(); ROE.Frame.popupPlayerProfile($(e.currentTarget).attr('data-pn'));
        });

        // Hook in village propfile popup
        $('.reportDetailsView .village-name', _container).click(function (e) {
            ROE.UI.Sounds.click();
           
        });
        
        // Call here once everything has been appended.
        _handleBlockForwardedPlayer(_container);
        _checkNextPrevBtnVisibility();
        
        if (_selectedFromList)
            BDA.UI.Transition.slideLeft($(CONST.Selector.reportDetailsView, _container), $('.default', _container));
    }

    function _showNotePopup(note) {
        if (note.hasClass("hideNote") || note.text() == "") {
            return false;
        }

        ROE.Frame.simplePopopOverlay('https://static.realmofempires.com/images/icons/M_Notes.png', "Note", "<p class='fontDarkGoldFrLClg noShadow'>" + note.attr('data-note') + "</p>", 'reportDetails_showNotePopup', $("#reportsDialog").parent());
        return true;
    }


    function _showBattleSim(reportId, as, who) {
        if (!ROE.NoPopupBrowser) {
            event.preventDefault();
            return !popupModalIFrame2('battlesimulator.aspx?recID=' + reportId + '&as=' + as + '&who=' + who, 'BattleSimulator', 'Battle Simulator', 'https://static.realmofempires.com/images/icons/M_BattleSim.png');
        }
    }

    // Forward popup  
    function _forwardReportPopup(multi, selectedItemsArr) {

        ROE.Frame.simplePopopOverlay('https://static.realmofempires.com/images/icons/M_FwdReport.png', "Forward", CACHE.forwardReportTemplate[0].outerHTML, 'reportDetails_forwardReportPopup', $("#reportsDialog").parent());

        if (multi) {            
          
            if(selectedItemsArr) // pass the item array to the forward func. only of data was provided.
                $('.reportDetails_forwardReportPopup .forwardSendBtn').on("click", { items: selectedItemsArr }, _forwardSendSelected);
        } else {            
            $('.reportDetails_forwardReportPopup .forwardSendBtn').click(_forwardSend);
        }
        $('.reportDetails_forwardReportPopup .forwardTo').keyup(_handleForwardToKeyUp);
        $('.reportDetails_forwardReportPopup .loadingSearchList').hide();

        return true;
    }    

    function _handleForwardToKeyUp(e) {
        _waitToSearchPlayerNames($(e.currentTarget).val(), _handleGetPlayerNamesSuccessful, _clearForwardSearchList);
    }

    function _handleGetPlayerNamesSuccessful(r) {
       
        var result = $('.reportDetails_forwardReportPopup .forwardSearchList').empty();

        $.each(r, function (i, n) {
            var sp = $('<li>' + n.value + '</li>').click(_clearForwardSearchList, _selectPlayerNameFromForwardSearchList);
            result.append(sp);
        });

        $('.reportDetails_forwardReportPopup .loadingSearchList').hide();
    }

    function _selectPlayerNameFromForwardSearchList(e) {
     
        _clearForwardSearchList();
        var playerName = $(e.currentTarget).html();       
        $('.reportDetails_forwardReportPopup .forwardTo').val(playerName);
    }

    function _clearForwardTo() {
        $('.reportDetails_forwardReportPopup .forwardTo').val("");
    }

    function _clearForwardSearchList() {
     
        $('.reportDetails_forwardReportPopup .forwardSearchList').empty();
    }

    // Same as _searchPlayerNames except has a timer before it makes
    // the call.
    var _forwardLookupTimer;
    var _nameToLookup;
    function _waitToSearchPlayerNames(partialName, cbSuccess, cbFail) {
        _nameToLookup = partialName;
        if (_forwardLookupTimer)
            clearTimeout(_forwardLookupTimer);
        
        // Time out look up.
        if (_nameToLookup.length > 2) {            
            _forwardLookupTimer = setTimeout(_handleSearchPlayerNamesOnTimeout, CONST.forwardLookupThreshold);
        } else {
            _clearForwardSearchList();
        }
    }

    function _handleSearchPlayerNamesOnTimeout() {
        clearTimeout(_forwardLookupTimer);       
        $('.reportDetails_forwardReportPopup .loadingSearchList').show();
        _clearForwardSearchList();
        ROE.Api.call('player_search', {
            term: _nameToLookup
        }, _handleGetPlayerNamesSuccessful);
    }
    
    function _forwardSend() {
        // don't want any late look ups.
        if (_forwardLookupTimer)
            clearTimeout(_forwardLookupTimer);

        var playerName = $('.reportDetails_forwardReportPopup .forwardTo').val();
        if (playerName && playerName != '') {
            ROE.Frame.busy(undefined, undefined, $('.reportDetails_forwardReportPopup'));
            
            ROE.Api.call_report_forward(~~$(_selectedReportItem).attr('rid'), playerName, _forwardSend_APISuccess);
            
        }
    }

    function _forwardSend_APISuccess(r) {
 
        $('.reportDetails_forwardReportPopup .forwardSearchList').html("<li>" + $('#ReportPhrases [ph=' + r + ']', _container).html() + "</li>");
        ROE.Frame.free($('.reportDetails_forwardReportPopup'));
    }

    function _forwardSendSelected(event) {
        var itemList = event.data.items;        
        var listOfIds = new Array();
        itemList.each(function (i, item) {
            listOfIds.push(~~$(item).attr('rid'));
        });

        // don't want any late look ups.
        if (_forwardLookupTimer)
            clearTimeout(_forwardLookupTimer);

        var playerName = $('.reportDetails_forwardReportPopup .forwardTo').val();
        if (listOfIds.length > 0 && playerName && playerName != '') {
            
            ROE.Frame.busy(undefined, undefined, $('.reportDetails_forwardReportPopup'));           
            ROE.Api.call_report_forward(listOfIds.join(" "), playerName, _forwardSendSelected_APISuccess);
        }
    }

    function _forwardSendSelected_APISuccess(r) {

        $('.reportDetails_forwardReportPopup .forwardSearchList').html("<li>" + $('#ReportPhrases [ph=' + r + ']', _container).html() + "</li>");
        ROE.Frame.free($('.reportDetails_forwardReportPopup'));
    } // end forward system

    // Called when Update database is successful
    function _db_updateSuccess() { /* Super! Do nothing. */ }


    function _handleBlockForwardedPlayer(container) {
        $('.blockPlayerBtn', container).attr('href', '#').click(function (e) {
            e.preventDefault();
            ajax("ChatBlockAjax.aspx", { block: $('.forwardedByWho .player-name', container).html() }, function (obj) {
                if (obj == "ok") {
                    ROE.Frame.infoBar($('#ReportPhrases [ph=PlayerBlocked]', _container).html());
                }
            });
        });
    }


    function _getUnitCountTable(leftDataArray, rightDataArray, unknownLeft, unknownRight) {
        /// Returns a <table> populated with unit data
        /// where leftDataArray is the attacking player, and
        /// rightDataArray is the defending player.

        // Create a new table object.
        var table = $('<table class="unitsReportTable"></table>');

        var allRowsHTML = "";

        var unitTypesList = ROE.Entities.UnitTypes.SortedList;
        var rowData = {};
        var unitsRowHTML = "";

        var len = unitTypesList.length;
        var lasti = len - 1;
        // Add the headings
        table.append(CACHE.headingsRowTemplate[0].outerHTML + CACHE.dividerRowTemplate[0].outerHTML);

        for (var i = 0; i < len; i++) {
            if (unknownLeft) {
                rowData.leftDeployed = "<span class='zeroUnits'>?</span>";
                rowData.leftLost = "<span class='zeroUnits'>?</span>";
                rowData.leftRemaining = "<span class='zeroUnits'>?</span>";
            } else {
                rowData.leftDeployed = "<span class='zeroUnits'>0</span>";
                rowData.leftLost = "<span class='zeroUnits'>0</span>";
                rowData.leftRemaining = "<span class='zeroUnits'>0</span>";
            }

            rowData.unitName = ROE.Entities.UnitTypes[unitTypesList[i]].Name;
            rowData.unitImage = ROE.Entities.UnitTypes[unitTypesList[i]].IconUrl_ThemeM;

            if (unknownRight) {
                rowData.rightDeployed = "<span class='zeroUnits'>?</span>";
                rowData.rightLost = "<span class='zeroUnits'>?</span>";
                rowData.rightRemaining = "<span class='zeroUnits'>?</span>";
            } else {
                rowData.rightDeployed = "<span class='zeroUnits'>0</span>";
                rowData.rightLost = "<span class='zeroUnits'>0</span>";
                rowData.rightRemaining = "<span class='zeroUnits'>0</span>";
            }

            // Left side units
            if (!unknownLeft && (leftDataArray !== null)) {
                var lu = $.grep(leftDataArray, function (o) { return o.UnitTypeID == unitTypesList[i]; });
                if (lu.length == 1) {
                    rowData.leftDeployed = ((lu[0].DeployedUnitCount == 0) ? "<span class='zeroUnits'>0</span>" : BDA.Utils.formatNum(lu[0].DeployedUnitCount));
                    rowData.leftLost = ((lu[0].KilledUnitCount == 0) ? "<span class='zeroUnits'>0</span>" : "-" + BDA.Utils.formatNum(lu[0].KilledUnitCount));
                    rowData.leftRemaining = ((lu[0].ReaminingUnitCount == 0) ? "<span class='zeroUnits'>0</span>" : BDA.Utils.formatNum(lu[0].ReaminingUnitCount));
                }
            }

            // Right side units
            if (!unknownRight && (rightDataArray !== null)) {
                var ru = $.grep(rightDataArray, function (o) { return o.UnitTypeID == unitTypesList[i]; });
                if (ru.length == 1) {
                    rowData.rightDeployed = ((ru[0].DeployedUnitCount == 0) ? "<span class='zeroUnits'>0</span>" : BDA.Utils.formatNum(ru[0].DeployedUnitCount));
                    rowData.rightLost = ((ru[0].KilledUnitCount == 0) ? "<span class='zeroUnits'>0</span>" : "-" + BDA.Utils.formatNum(ru[0].KilledUnitCount));
                    rowData.rightRemaining = ((ru[0].ReaminingUnitCount == 0) ? "<span class='zeroUnits'>0</span>" : BDA.Utils.formatNum(ru[0].ReaminingUnitCount));
                }
            }

            var unitsRowHTML = BDA.Templates.populate(CACHE.unitsRowTemplate[0].outerHTML, rowData);
            if (i == lasti) {
                table.append(unitsRowHTML); // No trailing divider
            } else {
                table.append(unitsRowHTML + CACHE.dividerRowTemplate[0].outerHTML);
            }
        }

        return table[0].outerHTML;

    }


    

    function _showVillageProfile(villageAnchor) {
        if (!villageAnchor.hasClass('village-name'))
            return false;

        var link = villageAnchor; // the anchor tag as a jquery object       
       
        ROE.Frame.popupVillageProfile(link.attr('data-vid'));
        return false;
    }



    function _slideBackToDefaultPage() {

        // close any open dialogues:
        $('.forward-search', _container).toggle(false);

        ROE.UI.Sounds.click();
        //$('.default',_container).removeClass('slideLeftFrom').addClass('slideLeftTo');
        // $(CONST.Selector.reportDetailsView).removeClass('slideRightTo').addClass('slideRightFrom');

        //sliding left here is a bit weird but it avoids the sldieright rare bug
        BDA.UI.Transition.slideLeft($('.default', _container), $(CONST.Selector.reportDetailsView, _container)); 
              
        $(_selectedReportItem).removeClass('new');
        $('.hotspot', _selectedReportItem).removeClass('defeatSound victorySound').addClass("openSound");
    }

    // Need to confirm first, so add a Sure? counter
    function _reportDelete(e) {
        ROE.UI.Sounds.click();
        var deleteBtn = $(e.currentTarget);
        if (_waitingForDeleteConfirm) {            
            clearTimeout(_confirmDeleteTimer);
            _waitingForDeleteConfirm = false;
            deleteBtn.html("");
            _reportDeleteConfirmed();
        } else {           
            deleteBtn.html("Sure?");
            _waitingForDeleteConfirm = true;
            _confirmDeleteTimer = window.setTimeout(function () {
                deleteBtn.html("");
                _waitingForDeleteConfirm = false;               
            }, 2000);
        }
    }


    function _reportDeleteConfirmed() {

        var selectedReportElement = $(_selectedReportItem); //save report element before going to next

        ROE.Api.call('report_remove', { rids: JSON.stringify([~~selectedReportElement.attr('rid')]) }, _reportDelete_AjaxSuccess);
        BDA.Database.DeleteRows(CONST.tableName, { id: 'id', list: [selectedReportElement.attr('rid')] });
        BDA.Database.LocalSet('ReportsListLoaded', BDA.Database.LocalGet('ReportsListLoaded') - 1);

        //really ugly way of doing previous or next, but its the way reports are done... -farhad
        _selectedFromList = false; //stops it from doing a slide when going up or down
        var nextItemDown = selectedReportElement.next('.item');
        var prevItemUp = selectedReportElement.prev('.item');
        if (nextItemDown.length) { //try to go next item, which is down
            _chooseReport(nextItemDown);
        } else if (prevItemUp.length) { //then try to previous item, which is up the down
            _chooseReport(prevItemUp);
        } else { //if no items, go back to list
            _gotoList();
            _slideBackToDefaultPage();
        }

        selectedReportElement.remove();

    }

    function _reportDelete_AjaxSuccess() {
        //empty delete return
    }

    function _reportDeleteMultiple(itemList) {
        if (itemList.length == 0)
            return;
        
        ROE.Frame.busy(undefined, undefined, _container);

        _lastReportsDeleted = itemList;
        var listOfIds = new Array();
        _lastReportsDeleted.each(function (i, item) {
            listOfIds.push(~~$(item).attr('rid'));
        });

        ROE.Api.call('report_remove', { rids: JSON.stringify(listOfIds) }, _reportDeleteMultiple_AjaxSuccess);
    }

    function _reportDeleteMultiple_AjaxSuccess() {       
        var listOfIds = $(_lastReportsDeleted).map(function () { return ~~($(this).attr("rid")); }).get();
        BDA.Database.DeleteRows(CONST.tableName, { id: 'id', list: listOfIds });
      
        BDA.Database.LocalSet('ReportsListLoaded', BDA.Database.LocalGet('ReportsListLoaded') - _lastReportsDeleted.length);

        _lastReportsDeleted.each(function (i, item) {
            $(this).fadeOut('slow', function () {});
        }).promise().done(function () {           
            var l = _lastReportsDeleted.length;
            _lastReportsDeleted.remove();
            _refreshList(l);
        });

        // Make sure the buttons hide again
        _clearTotalChecked();

        ROE.Frame.free(_container);
    }

      
    function _reportForward() {
        $('.forward-search', _container).toggle();      
    }

    function _reportForwardSearch(e) {
        if ($(this).val().length > 2) {
            ROE.Api.call('player_search', {
                term: $('.forward-search .selected', _container).val()
            }, _reportForwardSearch_AjaxSuccess);
        }
    }

    function _reportForwardSearch_AjaxSuccess(r) {
        var result = $('.forward-search .result', _container).empty();
        $.each(r, function (i, n) {
            var sp = $('<span>' + n.value + '</span></br>').click(_clearReportForwardSearch, _reportForwardSelect);
            result.append(sp);
        });      
    }

    function _reportForwardSelect() {
        $('.forward-search .selected', _container).val($(this).html());
    }
    function _clearReportForwardSearch() {
        $('.forward-search .result', _container).empty();      
    }


    function _reportForwardSend() {

        if ($('.forward-search .selected', _container).val() != '') {
            ROE.Api.call('report_forward', {
                record: ~~$(_selectedReportItem).attr('rid'),
                player: $('.forward-search .selected', _container).val()
            }, _reportForwardSend_AjaxSuccess);
        }
    }

    function _reportForwardSend_AjaxSuccess(r) {
        $('.forward-search .message', _container).html($('#ReportPhrases [ph=' + r + ']', _container).html());    
    }

    function _breakLongText(str, maxLength) {
        /// Returns a string with line breaks defined by
        /// max length.

        if (maxLength < 1)
            return str;

        if (str.length > maxLength) {
            // the string is longer than max length so break up and recursively look for more
            // break points.
            return str.substring(0, maxLength).trim() + "<br />" + _breakLongText(str.substring(maxLength).trim(), maxLength);
        } else {
            // str is as long or less than max length, so 
            // just return
            return str;
        }
    }

    
    // Expose the note popup function
    obj.showNotePopup = _showNotePopup;
    obj.forwardReportPopup = _forwardReportPopup;   
    obj.showVillageProfile = _showVillageProfile;
    obj.breakLongText = _breakLongText;

    // Inside report details next/prev 
    obj.showNextReport = _showNextReport;
    obj.showPrevReport = _showPrevReport;
    obj.showBattleSim = _showBattleSim;

    // Refreshing
    obj.showLatestReports = _showLatestReports;
    obj.handleNewReportsBroadcast = _handleNewReportsBroadcast;

    // Filtering
    obj.filterReports = _filterReports;
    obj.clearReportsFilter = _clearReportsFilter;
    obj.showStarredOnly = _showStarredOnly;
    obj.showGovAttacksOnly = _showGovAttacksOnly;
    obj.showSpyOnly = _showSpyOnly;
    obj.showSuccessReportsOnly = _showSuccessReportsOnly;
    obj.showDefeatReportsOnly = _showDefeatReportsOnly;

    // Selected reports
    obj.deleteSelected = _deleteSelected;
    obj.forwardSelected = _forwardSelected;
    obj.selectAllReports = _selectAllReports;
    obj.deselectAllReports = _deselectAllReports;
    obj.editReports = _editReports;

    obj.nukeAllBtnClick = _nukeAllBtnClick;

}(window.ROE.Reports = window.ROE.Reports || {}));