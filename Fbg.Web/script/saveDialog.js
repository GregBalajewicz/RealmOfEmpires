

saveDialog = {
    CONSTS:
    {
        ListOfSaves: ".ListOfSaves",
        txtSaveName: "input.NewSaveName",
        btnSave: "input.save"
    },

    storageKeyName: "default",
    savedItems: [],
    containerElement: undefined,
    localStorageNoSupportedHtmlMessage: '<span class=Error>Your browser does not support the ability to save. Please upgrade your browser if you want this function</span>',
    getObjectForSave: function () { return ""; }, //sort of an abstrace implementation, expecting real call back to be passed in to init
    loadFromSavedObject: function (obj) { }, //sort of an abstrace implementation, expecting real call back to be passed in to init


    init: function (containerElement, name, getObjectForSave, loadFromSavedObject) {
        /// <summary>inits the save dialog</summary>
        /// <param name="containerElement" Type="HTMLelement" >required. some html element where the dialog should exist</param>
        /// <param name="name" type="String" >required. name of this dialoge, must be app unique, will be used as a key to save</param>
        /// <param name="getObjectForSave" type="function" >required. will be called when saving so that consumer may save its data and pass it as an object in the return value</param>
        /// <param name="loadFromSavedObject" type="function" >required. will be called when loading a saved object. function should accept one param, an object</param>
        if (typeof name !== 'string') { throw ('name param must be a non-empty string'); }
        if (typeof getObjectForSave !== 'function') { throw ('getObjectForSave required'); }
        if (typeof loadFromSavedObject !== 'function') { throw ('loadFromSavedObject required'); }

        this.storageKeyName = name;
        this.getObjectForSave = getObjectForSave;
        this.loadFromSavedObject = loadFromSavedObject;
        this.containerElement = $(containerElement);
        saveDialog.form();
    },

    savedItem_FindIndexOf: function (name) {
        var index = -1;
        for (var i = 0; i < this.savedItems.length; i++) {
            if (this.savedItems[i].Name === name) {
                index = i;
                break;
            }
        }
        return index;
    },
    savedItem_Find: function (name) {
        return this.savedItems[this.savedItem_FindIndexOf(name)];
    },

    loadSavedItem: function (divEl) {
        var savedItem = this.savedItem_Find($(divEl).find('span.saveItemName').text());
        if (savedItem) {
            this.containerElement.find(this.CONSTS.txtSaveName).val(savedItem.Name);
            this.loadFromSavedObject(savedItem.Data);
        }

    },

    // hanlde the onclick of the delete icon to delete a row. 
    deleteRow: function (imgEl) {
        // get containing row
        var parentDIV = $(imgEl).first().parent();
        var indexOfSavedItem = this.savedItem_FindIndexOf($(parentDIV).find('span.saveItemName').text());
        if (indexOfSavedItem > -1) {
            this.savedItems.pop(indexOfSavedItem);

            parentDIV.slideUp();
            this.persist();
        }
    },

    dialogHtml: '<div class="ListOfSaves"></div><input class="NewSaveName TextBox" /> <input type="button" class="save inputbutton" value="Save" onclick="saveDialog.save();">',

    item: '<div class="saveItem"  onclick="saveDialog.loadSavedItem(this);"> \
              <img class="cancel" onclick="saveDialog.deleteRow(this);return false;" src="https://static.realmofempires.com/images/cancel.png"/> \
              <span class="saveItemName"></span> \
              </div>',


    form: function () {

        if (window.localStorage) {
            var dlg = $(this.dialogHtml);
            this.containerElement.empty();
            this.containerElement.append(dlg);
            this.containerElement.show();

            saveDialog.load();
            saveDialog.fill();

        } else {
            this.containerElement.empty();
            this.containerElement.html(this.localStorageNoSupportedHtmlMessage);
            this.containerElement.show();
        }
    },
    addrow: function (fl) {


        return item;
    },
    fill: function () {
        var listOfSaves = this.containerElement.find(this.CONSTS.ListOfSaves);
        var item;
        listOfSaves.empty();
        for (var i = 0; i < this.savedItems.length; i++) {
            item = $(this.item);
            item.find('.saveItemName').text(this.savedItems[i].Name);
            listOfSaves.append(item);
        }

    },
    save: function () {
        saveDialog.flashMsg_Clear();
        var listOfSaves = this.containerElement.find(this.CONSTS.ListOfSaves);
        var saveNameTxt = this.containerElement.find(this.CONSTS.txtSaveName);
        var saveBtn = this.containerElement.find(this.CONSTS.btnSave);
        var cancelButton = this.containerElement.find('input.cancel'); // will only have the cancel button if confirming overwrite
        if (saveNameTxt.val().trim() === "") { this.flashMsg(true, 'Enter a name!'); return; }

        if (this.savedItem_FindIndexOf(saveNameTxt.val().trim()) !== -1
            && cancelButton.length === 0) {
            //
            // item already exists ( by such name) - overwrite
            saveBtn.val("Confirm overwrite");
            var cancelButton = $('<input type="button" class="cancel inputbutton" value="Cancel">');
            cancelButton.click(function () {
                $(this).fadeOut(function () { $(this).remove(); });
                $(this).parent().find(saveDialog.CONSTS.btnSave).val("Save");
            });
            saveBtn.before(cancelButton);
            cancelButton.fadeIn();
            saveBtn.fadeIn();

        }
        else {
            var data = this.getObjectForSave();
            var savedItem = this.savedItem_Find(saveNameTxt.val().trim())
            if (savedItem) {
                savedItem.Data = data;
            } else {
                this.savedItems.push({ Name: saveNameTxt.val().trim(), Data: data });
            }

            this.persist();

            this.fill();
            saveNameTxt.val('');
            this.flashMsg(false, 'Saved!');

            if (cancelButton.length !== 0) {
                // was confirming overwrite so change back to save 
                saveBtn.val("Save");
                cancelButton.fadeOut(function () { $(this).remove(); });
            }
        }
    },

    flashMsg: function (bIsError, sMsg) {
        this.containerElement.find('span.result').stop().remove();
        this.containerElement.find(this.CONSTS.btnSave).after($('<span class="result ' + (bIsError ? 'Error' : 'ConfirmationMsg') + '">' + sMsg + '</span>'));
        this.containerElement.find('span.result').fadeIn('fast', function () { $(this).fadeOut(6000); });
    },
    flashMsg_Clear: function () {
        this.containerElement.find('span.result').stop().remove();
    },

    persist: function () {
        var json = $.toJSON(this.savedItems);

        localStorage[this.storageKeyName] = json;
    },
    load: function () {

        var saved = localStorage[this.storageKeyName];
        if (saved) {
            this.savedItems = $.evalJSON(saved);
        }

    }


}
