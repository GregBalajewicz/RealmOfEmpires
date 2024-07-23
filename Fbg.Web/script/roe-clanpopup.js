(function (ROE) {
}(window.ROE = window.ROE || {}));


(function (obj) {

    var CONST = {
        imgON: "https://static.realmofempires.com/images/misc/yesGreen.PNG",
        imgOFF: "https://static.realmofempires.com/images/icons/M_IcoCancel.png"
    }
    clanInfo = {};
    var ClanRoles = { 0: "Owner", 2: "Inviter", 3: "Admin", 4: "ForumAdmin", 5: "Diplomat" };
    var _forumPostID;
    var _membersListForMessageAll;
    
    _clanForumAdminButtonClick = function _clanForumAdminButtonClick() {
        var iframe = $("#clan_popup #clanForumAdminIframe");
        iframe.attr('src', iframe.attr('data-src')); //this loads the iframe, also works as a reload if clicked again.
    };

    _clanEmailMembersButtonClick = function _clanEmailMembersButtonClick() {
        var iframe = $("#clan_popup #clanEmailMembersiFrame");
      
        if (iframe.attr('src') != iframe.attr('data-src')) {
            iframe.attr('src', iframe.attr('data-src')); 
        }
    };
    _clanClaimsButtonClick = function _clanClaimsButtonClick() {
        var iframe = $("#clan_popup #clanClaimsiFrame");

        if (iframe.attr('src') != iframe.attr('data-src')) {
            iframe.attr('src', iframe.attr('data-src'));
        }
    };
    _clanSettingsButtonClick = function _clanSettingsButtonClick() {
        var iframe = $("#clan_popup #clanSettingsiFrame");

        if (iframe.attr('src') != iframe.attr('data-src')) {
            iframe.attr('src', iframe.attr('data-src'));
        }
    };


    _clanForumButtonClick = function _clanForumButtonClick() {
        var iframe = $("#clan_popup #clanForumIframe");

        if (_forumPostID != undefined) {
            iframe.attr('src', 'ShowThread.aspx?ID=%id%'.format({ id: _forumPostID })); //this loads the iframe, also works as a reload if clicked again.
            _forumPostID = undefined; // make sure we only autogoto forum once. 
        } else {
            iframe.attr('src', iframe.attr('data-src')); //this loads the iframe, also works as a reload if clicked again.
        }
    };

    obj.init = function (junk, forumPostID) {
        var iframe;
        _forumPostID = forumPostID;
        $("#" + ROE.Frame.CONSTS.popupNameIDPrefix + "clan .ClanPopup").append(BDA.Templates.getRawJQObj("ClanPopup", ROE.realmID));
       
        clanInfo.clanId = $(".ClanPopup").attr("data-clanid");       

        if (ROE.Player.Clan === null) {
            ROE.Player.Clan = {};
            ROE.Player.Clan.id = 0;
        }

        if (clanInfo.clanId != 0) {
            // Check first if player's clan is not null before
            // comparing the id properties
            if (ROE.Player.Clan && (clanInfo.clanId != ROE.Player.Clan.id)) {
            
                //other clan's view, hide other buttons
                $("#clanMain LI").not("#clanMembers").hide();
                $("#otherClanProfile .clanRequestInviteBtn").click(function wrapCallToClanRequestInvite() {                   
                    ROE.Frame.launchClanInviteRequestMessage(clanInfo.clanId);
                });
            }

                      
            $("#clan_popup #clanForumAdmin").hide(); //hide this button, show it based on role after _updateDefaultpage response
            $("#clan_popup #clanEmailMembers").hide(); //hide this button, show it based on role after _updateDefaultpage response

            $("#clan_popup li#clanForum").click(_clanForumButtonClick);
            $("#clan_popup li#clanForumAdmin").click(_clanForumAdminButtonClick);
            $("#clan_popup li#clanEmailMembers").click(_clanEmailMembersButtonClick);
            $("#clan_popup li#clanClaims").click(_clanClaimsButtonClick);
            $("#clan_popup li#clanSettings").click(_clanSettingsButtonClick);

           

            ROE.Api.call("getclanpublicprofile", { cid: clanInfo.clanId }, _updateDefaultpage);                        
            
            $("#clan_popup .clanInviteButton").click(_clanSendInvite);
           
        }
        else {//player not in any clan
            _notInClan();
        }
       
        $("#clan_popup .clanpages").hide();


        _autocomplete();

        // set up click events
        //
        $(".clanBackButton").click(_clanMainback);

        $("#clan_popup").delegate(".cmAdmin", "click", function (e) {

            $(".cmRow").removeClass("villageSelected");
            $(e.currentTarget).addClass("villageSelected");
            var plID = $(e.currentTarget).attr("data-info");
            var plname = $(e.currentTarget).attr("data-pname");
            var roles = $("#memberlist_" + plID + " SPAN[data-role]");
            var stew = $(e.currentTarget).attr("data-stew");

            //close any other open section
            $("#clan_popup .cmInfo:not(#cmInfo_" + plID + ")").removeClass("infoOpen");
            //delay for empty, as finsih first the close the infobar
            window.setTimeout(function () { $("#clan_popup .cmInfo:not(#cmInfo_" + plID + ")").empty(); }, 500);

            if ($("#clan_popup #cmInfo_" + plID).hasClass("infoOpen")) {

                $("#clan_popup #cmInfo_" + plID).removeClass("infoOpen");
                window.setTimeout(function () { $("#clan_popup #cmInfo_" + plID).empty(); }, 500);
            }
            else {
                _adminRoles(plID, plname, roles, stew);
            }
            
        });
        
        
        $("#clan_popup").delegate(".stewinfo", "click", function (e) {

            var TargetPlayer = $(e.currentTarget).text();
            ROE.Frame.popupPlayerProfile(TargetPlayer);
        });        

        $("#clan_popup").delegate(".dissmiss", "click", function (e) {
            var plID = $(e.currentTarget).attr("data-pid");
            var confirmtext = _phrases(33);
            ROE.Frame.Confirm(confirmtext, "Yes", "No", "rgba(33,33,33,0.8)", _doDismiss, plID, undefined, true);
        });
       
        $("#clan_popup").delegate(".RoleAdmin", "click", function (e) {

            //var plRoleOn = $(e.currentTarget).hasClass("RoleAdminON");
            e.stopPropagation();

            _doRoleChange(e);
        });
       
        $("#clan_popup").delegate(".deletedipl", "click", function (e) {
            
            var target = $(e.currentTarget).parent();
            var clanName = target.text();
            var dipl = target.parent().prev().attr("data-dipl");

            ROE.Frame.infoBar(_phrases(28));

            ROE.Api.call("deleteclandiplomacy", { clan: clanName, dip: dipl }, function () {

                $(target).remove();
            });

        });
        
        $("#clan_popup .clanSetRole_Inviter").click(_saveSettings);
        $("#clan_popup .addDipl").click(_addDiplomacy);
        $("#clanMain LI").click(_clanInfoPage);
        $("#clanMain #clanNameSubmit").click(_clanNewCreate);
        $("#clan_popup .leaveClanButton").click(_leaveClan);
        $("#clan_popup .renameClanButton").click(_renameClan);
        $("#clan_popup .disbandClanButton").click(_disbandClan);
        $("#clan_popup .clanEdit").click(_editProfile);

        $("#clan_popup .clanMembersTable").empty();
        
        if (!ROE.Player.Indicators.clanForum) {
            $(".newPostsOnForumIndicator").hide();
        }

        if (_forumPostID != undefined) {
            // if forumPostID, then go directly to the forum. 
            $("#clan_popup li#clanForum").click();
        }

        // handle message all members click
        $("#clan_popup .messageAllMembers").click(_handleMessageAllMembersClick);
         
    }
        
    function _handleMessageAllMembersClick() {
        ROE.Mail.sendmail(_membersListForMessageAll);

    }

    function _addDiplomacy(e) {

        e.stopPropagation();
        var diplType = $(e.currentTarget).attr("data-dipltyle");
        var TargetClan = $("#clan_popup #clanAddClanName").val();

        if (TargetClan != "") {

            ROE.Api.call('saveclandiplomacy', { clan: TargetClan, dip: diplType }, function (response) {

                $("#clan_popup #clanAddClanName").val("");

                if (response.rcode == 0) {
                    //on success update DiplTable
                    var diplomacylist = "<li><span>" + TargetClan+"</span>";
                    if (clanInfo.PlayerIsOnwer || clanInfo.PlayerIsAdmin || clanInfo.PlayerIsDiplomat) {
                        
                        diplomacylist += "<span class='deletedipl sfx2' ><img src='https://static.realmofempires.com/images/buttons/M_Btn_DotX.png'></span>";
                    }
                    diplomacylist += "</li>";

                    $($("#clan_popup .clanDiplTable>UL ")[diplType]).prepend(diplomacylist);
                    ROE.Frame.infoBar(_phrases(29));
                }
                else {
                    var errorcode = 34 + response.rcode;
                    ROE.Frame.infoBar(_phrases(errorcode));
                }
            });
        }
    }

    function _clanNewCreate() {
        var newName = $("#inputClanName").val();

        ROE.Api.call("createclan", { clanname: newName }, function (response) {

            if (response.newclandid == 0) {
                ROE.Frame.infoBar(_phrases(38));
            }
            else {
                //load new clan main info
                ROE.Frame.busy();

                //add to clan chat
                ROE.Chat2.joinOrCreateClanChat(response.newclandid);

                clanInfo.clanId = response.newclandid;
                if (ROE.Player.Clan === null) { ROE.Player.Clan = {}; }
                ROE.Player.Clan.id = response.newclandid;
                $("#clanMain .clanCreate").hide();

                ROE.Api.call("getclanpublicprofile", { cid: response.newclandid }, _updateDefaultpage);
            }
        });
    }


    function _notInClan() {

        $("#clanMain UL, #clan_popup .clanAction").hide();
        $("#inputClanName").val("");
        $("#clanMain .clanName").text(_phrases(2));
        $("#clanMain .clanCreate").prepend(_phrases(3)).show();
        $("#clanMain .clanEntry").append(_phrases(44));

        ROE.Api.call("getClanInvites", { pid: ROE.playerID }, _updateInvites);

    }


    function _updateDefaultpage(response) {
        //console.log("_updateDefaultpage: ",response);
        clanInfo.PlayerIsOnwer = response.PlayerIsOnwer;
        clanInfo.PlayerIsAdmin = response.PlayerIsAdmin;
        clanInfo.PlayerIsForumAdmin = response.PlayerIsForumAdmin;
        clanInfo.PlayerIsInviter = response.PlayerIsInviter;
        clanInfo.PlayerIsDiplomat = response.PlayerIsDiplomat;
        clanInfo.playerClanName = response.Name;

        $("#clanMain .clanName").text(clanInfo.playerClanName);
        $("#clanMain #clanMembers SPAN:first-child").html(response.NumOfMembers);
        
        //if not players clan, then display public profile on the main page
        if (ROE.Player.Clan && (clanInfo.clanId != ROE.Player.Clan.id)) {
        
            var PublicMessage = response.PublicMessageHtml.split("\n").join("<br />");
            var PublicMessage = PublicMessage.replace("onclick", "");

            $("#clanMain #otherClanProfile").show();
            $("#otherClanProfile .clanPoints SPAN").html(BDA.Utils.formatNum(response.Points));
            $("#otherClanProfile .clanRank SPAN").html(response.Rank);
            $("#otherClanProfile .clanMessage").html(PublicMessage);
            $("#clanMain #clanInvites").hide();
        }
        else {
            
            $("#clanMain #otherClanProfile").hide();
            $("#clanMain .clanCreate").hide();

            $("#clanMain UL, #clan_popup .clanAction").show();
            $("#clan_popup .clanAction .leaveClanButton").css("display","inline-block");

            //admin functions
            if (clanInfo.PlayerIsOnwer ) {
                $("#clan_popup .clanAction .renameClanButton").css("display", "inline-block");
                $("#clan_popup .clanAction .disbandClanButton").css("display", "inline-block");
            }  
            if (clanInfo.PlayerIsOnwer || clanInfo.PlayerIsAdmin) {
                $("#clanMain #clanSettings").show();
                $("#clan_popup .clanProfileEdits").show();
                $("#clan_popup #clanEmailMembers").show();
            }
            if (clanInfo.PlayerIsOnwer || clanInfo.PlayerIsAdmin || clanInfo.PlayerIsInviter) {
                $("#clan_popup .clanInvitePlayer").show();
                $("#clan_popup .claninviteLeft").show();
            }
            if (clanInfo.PlayerIsOnwer || clanInfo.PlayerIsAdmin || clanInfo.PlayerIsDiplomat) {
                $("#clan_popup .clanAddDimpl").show();
            }
            if (clanInfo.PlayerIsOnwer || clanInfo.PlayerIsAdmin || clanInfo.PlayerIsForumAdmin) {
                $("#clan_popup #clanForumAdmin").show();
            }

            

            ROE.Api.call("getClanInvites", { pid: ROE.playerID }, _updateInvites);            
        }

        ROE.Frame.free();
    }


    function _renameClan() {

        var savetext = _phrases(25);
        var clanName = $("#clanMain .clanName").text();

        var renametext = "<center><input type='text' id='inputClanName' value='" + clanName + "'>";
        renametext += "<BR><div id='clanRenameSubmit' class='customButtomBG' >"+savetext+"</div>";
        renametext += "<BR><BR>" + _phrases(44) + "<BR><BR></center>";

        ROE.Frame.popupInfo("Rename Clan", "280px", "center", "rgba(33,33,33,0.8)", renametext, false);

        $("#clanRenameSubmit").bind("click", function () {

            var newName = $("#inputClanName").val();
            var oldName = $("#clanMain .clanName").text();
            
            if (oldName != newName) {
                
                ROE.Api.call("renameclan", { newname: newName }, function (response) {

                    ROE.Frame.infoBar(_phrases(24));
                    $("#clanMain .clanName").text(response.newName);                    
                });
            }

            ROE.Frame.popupInfoClose($(this));
        });

    }


    function _disbandClan() {
        var confirmtext = _phrases(21);
        ROE.Frame.Confirm(confirmtext, "Yes", "No", "rgba(33,33,33,0.8)", _disbandClanConfirmed, undefined, undefined, true);
    }

    function _disbandClanConfirmed() {

        ROE.Frame.busy();

        ROE.Api.call("deleteclan", {}, function (response) {

            ROE.Frame.free();

            if (response.info == 1) {
                ROE.Chat2.deleteClanChat(clanInfo.clanId);
                clanInfo.clanId = 0;
                _notInClan();
            }
            else {//error
                if (response.info == 0) { ROE.Frame.infoBar(_phrases(36)); }
                if (response.info == 2) { ROE.Frame.infoBar(_phrases(48)); }
            }
        });
    }

    function _leaveClan() {        
        var confirmtext = _phrases(20);
        ROE.Frame.Confirm(confirmtext, "Yes", "No", "rgba(33,33,33,0.8)", _leaveClanConfirmed, undefined, undefined, true);
    }

    function _leaveClanConfirmed() {
        
        ROE.Frame.busy();

        ROE.Api.call("clanleave", { pid: ROE.playerID }, function (response) {            

            ROE.Frame.free();

            switch (response.result) {
                case "success":
                    ROE.Chat2.leaveClanChat(clanInfo.clanId);
                    clanInfo.clanId = 0;
                    _notInClan();
                    break;
                case "errorLeaving":
                    ROE.Frame.infoBar(_phrases(41));
                    break;
                case "errorLeaving_changesNoLongerAllowed":
                    ROE.Frame.infoBar(_phrases(53));
                    break;
            }
            
        });
    }
    
    function _joinClanConfirmed(clanId) {
          
        ROE.Frame.busy();

        ROE.Api.call("clanjoin", { cid: clanId }, function (response) {
            
            ROE.Frame.free();

            switch (response.result)
            {
                case 0:
                    //successfull join, so need refresh clan info
                    clanInfo.clanId = parseInt(clanId);
                    if (ROE.Player.Clan != null) { ROE.Chat2.leaveClanChat(ROE.Player.Clan.id) }
                    ROE.Chat2.joinOrCreateClanChat(clanId); //join the clan chat
            
                    if (ROE.Player.Clan === null) {  ROE.Player.Clan = {}; }
                    ROE.Player.Clan.id = parseInt(clanId);

                    $("#clan_popup .clanProfile .clanMessage").empty();
                    ROE.Api.call("getclanpublicprofile", { cid: clanInfo.clanId }, _updateDefaultpage);
                    break;
                case 1:
                    ROE.Frame.infoBar(_phrases(36));
                    break;
                case 2:
                    ROE.Frame.infoBar(_phrases(42));
                    break;
                case 3:
                    ROE.Frame.infoBar(_phrases(43));
                    break;
                case 4:
                    ROE.Frame.infoBar(_phrases(53));
                    break;
                case 9:
                    ROE.Frame.infoBar(_phrases(41));
                    break;
                default:
                    //code to be executed if n is different from case 1 and 2
            }
           
        });
        
    }


    function _updateInvites(response) {
        
        var inviterList = response.InviterList;
        var inviterTable = "<table>";
        var expiresOn = _phrases(9);
        var expiresDays = _phrases(10);
        var clanInfoTitle = _phrases(11);
        var clanInviteAcceptText = _phrases(17);
        var clanInviteRejectText = _phrases(18);

        if (inviterList.length >0 ) {
            
            for (var i = 0; i < inviterList.length; i++) {

                inviterTable += "<tr class=TRrow><td class='TDclanName sfx2' data-clanid='" + inviterList[i].ClanID + "' >" + inviterList[i].Name ;
                inviterTable += "<div >" + expiresOn + " " + inviterList[i].InvitedOn + " " + expiresDays + "</div></td>";
                inviterTable += "<td data-clanid='" + inviterList[i].ClanID +"' width=90px ><span class='clanInviteAccept sfx2'></span> <span class='clanInviteReject sfx2' ></span></td></tr>";
             }
        }
        else {
            inviterTable += "<tr class=TRrow><td >" + _phrases(1) + "</td></tr>";
        }

        inviterTable += "</table>";

        $("#clanMain .clanInvitelist").empty().html(inviterTable);

        //show invites from other clans
        $("#clanMain #clanInvites").show();

        $("#clan_popup .clanInviteReject").click(function (e) {

            var clanId = $(e.currentTarget).parent().attr("data-clanid");
            ROE.Frame.infoBar(_phrases(19));

            ROE.Api.call("cancelclaninvite", { cid: clanId, pid: ROE.playerID }, _updateInvites);
        });


        $("#clan_popup .clanInviteAccept").click(function (e) {
            
            var clanId = $(e.currentTarget).parent().attr("data-clanid");
            
            if (ROE.Player.Clan === null || ROE.Player.Clan.id === 0) { var confirmtext = _phrases(34); }
            else { var confirmtext = _phrases(22); }
            
            ROE.Frame.Confirm(confirmtext, "Yes", "No", "rgba(33,33,33,0.8)", _joinClanConfirmed, clanId, undefined, true);

        });
        
    }


    function _clanInfoPage() { //make info pages   
       
        ROE.UI.Sounds.clickActionOpen();

        var viewClanpage = $(this).attr("id");

        if (viewClanpage == "clanMembers") {
            $("#clan_popup .clanPageTitleLoading").removeClass("off");
            ROE.Api.call("getClanMemberList", { cid: clanInfo.clanId }, _updateMembers);
        }
        if (viewClanpage == "clanDiplomacy") {
            $("#clan_popup .clanDiplTitleLoading").removeClass("off");
            ROE.Api.call("getClanDiplomacy", { cid: clanInfo.clanId }, _updateDiplomacy);
        }
        if (viewClanpage == "clanProfile")      { ROE.Api.call("getclanpublicprofile", { cid: clanInfo.clanId }, _updateProfile); }
        if (viewClanpage == "clanInvitations")  { ROE.Api.call("getclanplayerinvites", { cid: clanInfo.clanId }, _updateInvitations); }
        //if (viewClanpage == "clanSettings")     { _updateSettings() }

        $("#clan_popup .clanpages").show();
        $("#clan_popup ." + viewClanpage).show();

        try {
            // Catch and hopefully bypass...
            BDA.UI.Transition.slideLeft($("#clan_popup .clanpages"), $("#clanMain"));
        } catch (ex) {
            var roeex = new BDA.Exception("slideLeft and or BDA.UI.Transition object unavailable in clanpopup", ex);
            BDA.latestException = roeex;
            //BDA.Console.error(roeex);            
            throw roeex;
        }
        
    }
    
    
    function _clanMainback() {     
        
        ROE.UI.Sounds.clickActionOpen();

        //update main page
        ROE.Api.call("getclanpublicprofile", { cid: clanInfo.clanId }, _updateDefaultpage);

        BDA.UI.Transition.slideRight($("#clanMain"), $("#clan_popup .clanpages"), function () { $("#clan_popup .clanDetail").hide(); $("#clan_popup .clanpages").hide(); });

        //close any opened memberinfo
        $("#clan_popup .cmInfo").removeClass("infoOpen");
        $("#clan_popup #clanAddClanName").val("");
    }


    function _clanSendInvite() {
        
        var TargetPlayer = $("#clan_popup #clanInvitePlayer").val();

        if (TargetPlayer != "") {

            ROE.Frame.infoBar(_phrases(16));

            ROE.Api.call('player_other_invite_to_clan', { pname: TargetPlayer }, function (response) {

                $("#clan_popup #clanInvitePlayer").val("");
                ROE.Frame.infoBar(response.message);

                if (response.code == 1) {//success
                    ROE.Api.call("getclanplayerinvites", { cid: clanInfo.clanId }, _updateInvitations);
                }
            });
        }
    }


    function _updateInvitations(response) {
        
        var inviterList = response.inviterList;
        var invitesLeft = response.invitesLeft;
        var moreInvitesOn = response.moreInvitesOn;
        
        var expiresOn = _phrases(9);
        var expiresDays = _phrases(10);
        var clanInfoTitle = _phrases(11);
        var inviteRevoke = _phrases(12);
        var inviterTable = "<table width=99%>";

        if ( typeof inviterList != "undefined" ) {

            inviterTable += "<tr class=TRrow><td colspan=3 >" + "<div class=separator2 ></td></tr>";

            for (var i = 0; i < inviterList.length; i++) {

                inviterTable += "<tr class='TRrow' id='revoke_" + inviterList[i].PlayerID + "'><td class='TDclanName2 sfx2'  >" + inviterList[i].Name + "</td>";
                inviterTable += "<td class=TDclanDays >" + inviterList[i].InvitedOn + " " + expiresDays + "</td>";
                if (clanInfo.PlayerIsOnwer || clanInfo.PlayerIsAdmin || clanInfo.PlayerIsInviter) {
                    inviterTable += "<td class='clanPlayerInvRevoke sfx2' data-playerid='" + inviterList[i].PlayerID + "' ><img src='https://static.realmofempires.com/images/buttons/M_Btn_DotX.png'></td>";
                    
                }
            }
        }
        else {
            inviterTable += "<tr class=TRrow><td >" + _phrases(13) + "</td></tr>";
        }

        inviterTable += "</table>";

        $("#clan_popup .clanInviteTable").empty().html(inviterTable);

        if (invitesLeft > 0) {
            $("#clan_popup .claninviteLeft").empty().append(_phrases(14));
            $("#clan_popup .claninviteLeft >SPAN").html(invitesLeft);
        }
        else {
            $("#clan_popup .claninviteLeft").empty().append(_phrases(15) + " " + moreInvitesOn);
        }

        $("#clan_popup .clanInviteTable").show();

        $("#clan_popup .TDclanName2").click(function (e) {

            var TargetPlayer = $(e.currentTarget).text();
            ROE.Frame.popupPlayerProfile(TargetPlayer);
        });
       

        $("#clan_popup .clanPlayerInvRevoke").click(function (e) {

            var playerId = $(e.currentTarget).attr("data-playerid");
            ROE.Frame.infoBar(_phrases(23));
            
            ROE.Api.call("clanrevoke", { pid: playerId }, function () {
                
                remove = response.revoked;
                $("#revoke_" + playerId).fadeOut();
            });
        });
    }
    

    function _updateProfile(response) {
        
        var PublicMessage = response.PublicMessageHtml.split("\n").join("<br />");
        var PublicMessage = PublicMessage.replace("onclick", "");

        $("#clan_popup .clanProfile .clanName").text(response.Name);
        $("#clan_popup .clanProfile .clanRank SPAN").html(response.Rank);
        $("#clan_popup .clanProfile .clanPoints SPAN").html(BDA.Utils.formatNum(response.Points));
        $("#clan_popup .clanProfile .clanMessage").empty().html(PublicMessage);
        $("#clan_popup #clanEditorMessage").text(response.PublicMessageBB);        
    }


    function _editProfile(e) {
            
            e.stopPropagation();
            ROE.UI.Sounds.click();

            var saveit = $("#clan_popup .clanEdit").hasClass("saveit");                
             
            if (saveit) {

                ROE.UI.Sounds.click();

                var profText = $("#clan_popup #clanEditorMessage").val();
                ROE.Api.call("saveclanprofile", { txt: profText }, function () {

                    ROE.Frame.infoBar(_phrases(27));

                    $("#clan_popup .clanEdit").html(_phrases(26)).removeClass("saveit");

                    $("#clan_popup .clanMessage").show();
                    $("#clan_popup .clanEditMessage").hide();

                    //update profile
                    ROE.Api.call("getclanpublicprofile", { cid: clanInfo.clanId }, _updateProfile);

                });
            }
            else {
                $("#clan_popup .clanEdit").html(_phrases(25)).addClass("saveit");
                $("#clan_popup .clanMessage").hide();
                $("#clan_popup .clanEditMessage").show();
            }           
        
    }

    
    function _saveSettings(e) {

        var plRoleOn = $(e.currentTarget).hasClass("RoleAdminON");
        var setting3 = !plRoleOn;//invert status

        ROE.Frame.infoBar(_phrases(30));

        ROE.Api.call("saveclansettings", { flag: setting3 }, function (setresp) {
                       
            if (setresp.inviter) { $("#clan_popup .clanSetRole_Inviter").removeClass('RoleAdminOFF').addClass('RoleAdminON'); }
            else { $("#clan_popup .clanSetRole_Inviter").removeClass('RoleAdminON').addClass('RoleAdminOFF'); }
        });
        
    }


    function _updateMembers(response) {   
        //console.log("_updateMembers: ", response);
        var serverTimeStamp = response.hdServerTime;
        var sleepDuration = response.sleepDuration;
        var mm = response.Members;
        var memberTable = "<table>"; 
        var mr = response.Roles;
        var MemberRoles = new Array;
        var cmStyle = "cmPlayer";
             

        if (clanInfo.PlayerIsOnwer || clanInfo.PlayerIsAdmin) {
            var cmStyle = "cmAdmin";

            //default role settings
            ROE.Api.call("getclansettings", {}, function (setresp) {
                
                if (setresp.inviter) { $("#clan_popup .clanSetRole_Inviter").removeClass('RoleAdminOFF').addClass('RoleAdminON'); }
                else { $("#clan_popup .clanSetRole_Inviter").removeClass('RoleAdminON').addClass('RoleAdminOFF'); }
            });
            $("#clan_popup .clanDefaultRole").show();
        }

        //create role array
        for (var i = 0; i < mr.length; i++) {

            MemberRoles[ mr[i].PlayerID] += "=" + mr[i].RoleID + "=";
        }        
        
        _membersListForMessageAll = "";
        for (var i = 0; i < mm.length; i++ ) {
            
            var currentStamp = new Date().getTime();
            var lastLoginStamp = new Date( mm[i].LastLoginTime ).getTime();            
            var sleepMode = mm[i].IsPlayerInSleepMode;
            if (typeof MemberRoles[mm[i].PlayerID] == "undefined") { MemberRoles[mm[i].PlayerID] = "=="; }

            _membersListForMessageAll += ((_membersListForMessageAll == "" ? "" : ",") + mm[i].Name); // create a comma deliminated list of members

            memberTable += "<tr ";

            if (clanInfo.PlayerIsOnwer || clanInfo.PlayerIsAdmin) {
                //admin onclick
            }
            else { memberTable += " onclick='ROE.Frame.popupPlayerProfile(\"" + mm[i].Name + "\")'"; }

            memberTable += "class='cmRow " + cmStyle + "' id='memberlist_" + mm[i].PlayerID + "' data-info='" + mm[i].PlayerID + "' data-pname='" + mm[i].Name + "' ";

            if (ROE.Player.Clan && (clanInfo.clanId == ROE.Player.Clan.id)) {
            
                memberTable += "data-stew='" +mm[i].StewardPlayerName+"' ";
            }

            memberTable += " ><td class=cmList >" + (i + 1) + "</td><td class='cmName sfx2' >";

            if (sleepMode) { memberTable += "<div class='sleepmode' ></div>"; }
            memberTable += " <span class='cmPName' >" + mm[i].Name + "</span> ";
            memberTable += "<span class='cmPoint' >[" + ROE.Utils.formatShortNum(mm[i].VillagePoints) + ", " + mm[i].VillagesCount + "v]</span>";
            
            //display Roles only for clan members
            if (ROE.Player.Clan && (clanInfo.clanId == ROE.Player.Clan.id)) {
                            
                memberTable += "<div class='cmRole' >";
                
                for (var index in ClanRoles) {
                    
                    var rON = "";
                    if (MemberRoles[mm[i].PlayerID].indexOf("=" + index + "=") > 0) { var rON = "ON"; }

                    memberTable += "<SPAN data-role="+index+" class='" + rON + "' >" + ClanRoles[index] + "</SPAN>";
                }
                
                memberTable += "</div>";
            }       
            memberTable += "</td>";
            
            if (clanInfo.PlayerIsOnwer || clanInfo.PlayerIsAdmin ) {

                var colorGreen=255;
                var colorRed=255;
                var colorGreenDays= 3;
                var colorYellowDays= 6

                var loginDiffDaysStamp = ((currentStamp - lastLoginStamp) / (24 * 3600 * 1000));
                var loginDiffDays = parseFloat(loginDiffDaysStamp).toFixed(1);
                if (loginDiffDays == "0.0") { loginDiffDays = 0;}


                if (loginDiffDays <= colorGreenDays) { colorRed = parseInt( (colorRed * loginDiffDays)/colorGreenDays, 10); }
                if (loginDiffDays <= colorYellowDays && loginDiffDays > colorGreenDays) { colorGreen = colorGreen - parseInt((colorGreen * loginDiffDays) / colorYellowDays, 10); }
                if (loginDiffDays > colorYellowDays) { colorGreen = 0;}

                memberTable += "<td class='cmMore sfx2' >";
                memberTable += "<svg ><circle cx=17 cy=17 r=16 style='fill:rgb(" + colorRed + "," + colorGreen + ",0)' /></svg>";
                memberTable += "<div class='clanMoreInfo' >" + loginDiffDays + "</div></td>";
            }
            
            memberTable += "</tr>";                
            
            //edit Roles if Admin
            
            if (clanInfo.PlayerIsOnwer || clanInfo.PlayerIsAdmin) {
                
                memberTable += "<tr><td colspan='3' ><div id='cmInfo_" + mm[i].PlayerID + "' class='cmInfo'  >";
                
                memberTable += "</div></td></tr>";
            }   
            
            memberTable += "<tr><td colspan=3 ><div class='separator1' ></div></tr>";
        }        
        
        memberTable += "</table>";

        $("#clan_popup .clanMembersTable").empty().append(memberTable);

        $("#clan_popup .clanPageTitleLoading").addClass("off");
    }


    function _adminRoles(pid, pName, roles, stew) {
        
        var content = "<a class='cmPlayer customButtomBG sfx2' onclick='ROE.Frame.popupPlayerProfile(\"" + pName + "\")' href=# data-pname='" + pName + "' >" + _phrases(51) + "</a> ";
        content += "<a class='dissmiss customButtomBG sfx2' href=# data-pid='" + pid + "' >" + _phrases(50) + "</a> ";

        if (stew != "null") { content += _phrases(52) + ": <a class='stewinfo ButtonTouch sfx2' href='#'>" + stew + "</a>"; }

        content += "<BR>";

        for (var i = 0; i < roles.length; i++) {

            var rON = "OFF";
            var index = $(roles[i]).attr("data-role");

            if ($(roles[i]).hasClass("ON")) { var rON = "ON"; }

            content += "<div class='cmRoleList' ><a class='RoleAdmin RoleAdmin" + rON + " sfx2' href=# data-pid='" + pid + "' data-role=" + index + " ></a><span>" + ClanRoles[index] + "</span></div>";
        }

        $("#clan_popup #cmInfo_" + pid).html(content).addClass("infoOpen");
        
    }


    function _doRoleChange(e) {
        
        var plID = $(e.currentTarget).attr("data-pid");
        var plRole = $(e.currentTarget).attr("data-role");
        var plRoleOn = $(e.currentTarget).hasClass("RoleAdminON");
        var roleText = $(e.currentTarget).find("SPAN");
        var action = 1;
        if (plRoleOn) { action = 0; }

        ajax('ClanMembersAjax.aspx', {
            'Action': action,
            'pid': plID,
            'roleid': plRole
        },
                function (response) {
                   
                    if (response.action == 0) {
                        $(e.currentTarget).removeClass('RoleAdminOFF').addClass('RoleAdminON');

                        $("#memberlist_" + plID + " SPAN[data-role='" + plRole + "']").addClass("ON");

                    }
                    else {
                        $(e.currentTarget).removeClass('RoleAdminON').addClass('RoleAdminOFF');

                        $("#memberlist_" + plID + " SPAN[data-role='" + plRole + "']").removeClass("ON");
                    }

                    ROE.Frame.infoBar(_phrases(32));
                }
            );
    }


    function _doDismiss(plID) {

        ROE.Api.call("clandissmiss", { pid: plID }, function (response) {

            switch (response.result) {
                case "success":
                    //kick player from chat
                    ROE.Chat2.dismissClanChat(plID, clanInfo.clanId);

                    //update memberlist
                    $("#memberlist_" + plID).fadeOut();
                    $("#cmInfo_" + plID).fadeOut();
                    ROE.Frame.infoBar(_phrases(47));
                    break;
                case "onlyOwnerNoDismiss":
                    ROE.Frame.infoBar(_phrases(46));
                    break;
                case "adminCannotDismiss":
                    ROE.Frame.infoBar(_phrases(45));
                    break;
            }   
        });
    }


    function _updateDiplomacy(response) {
              
        var allies = response.Allies;
        var enemies = response.Enemies;
        var nap = response.NAP;
        
        //allies
        var diplomacylist = "<div class='clanName' data-dipl='0' >" + _phrases(4) + "</div><ul>";

        
            for (var i = 0; i < allies.length; i++) {

                diplomacylist += "<li><span>" + allies[i].Name + "</span>";
                if (clanInfo.PlayerIsOnwer || clanInfo.PlayerIsAdmin || clanInfo.PlayerIsDiplomat) {
                    diplomacylist += "<span class='deletedipl sfx2' ><img src='https://static.realmofempires.com/images/buttons/M_Btn_DotX.png'></span>";
                }
                diplomacylist += "</li>";
            }
        
        //enemies
        diplomacylist += "</ul><div class='separator1' ></div>";
        diplomacylist += "<div class='clanName' data-dipl='1' >" + _phrases(5) + "</div><ul>";
        
            for (var i = 0; i < enemies.length; i++) {

                diplomacylist += "<li><span>" + enemies[i].Name+"</span>";
                if (clanInfo.PlayerIsOnwer || clanInfo.PlayerIsAdmin || clanInfo.PlayerIsDiplomat) {
                    diplomacylist += "<span class='deletedipl sfx2' ><img src='https://static.realmofempires.com/images/buttons/M_Btn_DotX.png'></span>";
                }
                diplomacylist += "</li>";
            }
        
        //NAP
        diplomacylist += "</ul><div class='separator1' ></div>";
        diplomacylist += "<div class='clanName' data-dipl='2' >" + _phrases(6) + "</div><ul>";
        
            for (var i = 0; i < nap.length; i++) {

                diplomacylist += "<li><span>" + nap[i].Name + "</span>";
                if (clanInfo.PlayerIsOnwer || clanInfo.PlayerIsAdmin || clanInfo.PlayerIsDiplomat) {
                    diplomacylist += "<span class='deletedipl sfx2' ><img src='https://static.realmofempires.com/images/buttons/M_Btn_DotX.png'></span>";
                }
                diplomacylist += "</li>";
            }
        
        diplomacylist += "</ul>";
        
        $("#clan_popup .clanDiplTable").empty().append(diplomacylist);        

        $("#clan_popup .clanDiplTitleLoading").addClass("off");
    }    
    

    function _autocomplete() {
        
        $("#clanInvitePlayer")
            .keyup(function (e) {
                
                var v = $(this).val();
                if (v.length < 3) { $(".clanInvitePlayer .autocomplete").empty(); return false; }
                
                $.getJSON("NamesAjax.aspx?what=players", { term: v }, nameResponse);
            })

        function nameResponse(r) {
            var ac = $(".clanInvitePlayer .autocomplete").empty();

            $.each(r, function (i, n) {
                ac.append($('<div v="' + n.value + '">' + n.value + '</div>').click(function autocompleteSelect() {
                    $("#clanInvitePlayer").val($(this).attr('v'));
                    $(".clanInvitePlayer .autocomplete").empty();
                }));
            });
        }


        $("#clanAddClanName")
            .keyup(function (e) {
                
                var v = $(this).val();
                if (v.length < 3) { $(".clanAddDimpl .autocomplete").empty(); return false; }

                $.getJSON("NamesAjax.aspx?what=clans", { term: v }, clanResponse);
            })

        function clanResponse(r) {
            var ac = $(".clanAddDimpl .autocomplete").empty();

            $.each(r, function (i, n) {
                ac.append($('<div v="' + n.value + '">' + n.value + '</div>').click(function autocompleteSelect() {
                    $("#clanAddClanName").val($(this).attr('v'));
                    $(".clanAddDimpl .autocomplete").empty();
                }));
            });
        }
    }


    function _phrases(id) {
        return $('#clan_popup .phrases [ph=' + id + ']').html();
    }

}(window.ROE.Clan = window.ROE.Clan || {}));