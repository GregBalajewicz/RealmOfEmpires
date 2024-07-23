<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/templates/masterPopupTemplate_m.master" CodeFile="VillagePopup.aspx.cs" Inherits="templates_VillagePopup" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">


    <div id="background">
        <img src="https://static.realmofempires.com/images/misc/M_BG_VillageList.jpg" class="stretch" alt="" />
    </div>
    
    <div id="village_popup" class="themeM-panel">

            
            <div class="info">
                
                <div class="separator" ></div>
                <div class="villagename" ><span></span><span class="coord"></span></div>
                <div class="separator" ></div>

                <div class="villageinfo" >
                    <div class="villageimage"><img ></img></div>
                    <div class="villageinfos">
                        <div class="villpoint"><span></span> <%=RS("InfoPoints") %></div>
                        <div class="villowner"><%=RS("InfoRuled") %> <div class="ButtonTouch sfx2"></div></div>
                        <div class="villclan"><%=RS("InfoClan") %> <div class="ButtonTouch sfx2"></div></div>
                    </div>
                </div>
                <div class="separator" ></div>
                
                                   

                <div class="actions fontSilverFrLCmed">
                    <div class="rebel" style="display: none">
                    <%=RS("RebelMessage") %>
                    </div>
                    <div class="abandoned" style="display: none">
                        <%=RS("AbandonedMessage") %>
                    </div> 

                    <div class="action attacktab" ><a class="attack sfx2 ButtonTouch"><%=RS("Attack") %></a></div>

                    <div class="action supporttab " ><a class="support faded"><%=RS("Support") %></a></div>

                    <div class="action sendtab" ><a class="sends faded"><%=RS("SendSilver") %></a></div>

                    <div class="action mapit"><a class="sfx2 ButtonTouch"><%=RS("MapIt") %></a></div>


                    <div class="action supportlookup sfx2 ButtonTouch"><%=RS("SupportLookup") %></div>

                    <div class="action qb sfx2 ButtonTouch"><%=RS("QB") %></div>
                    <div class="action qr sfx2 ButtonTouch"><%=RS("QR") %></div>
                    <div class="action items sfx2 ButtonTouch"><%=RS("items") %></div>
                    <div class="action incoming sfx2 ButtonTouch"><%=RS("incoming") %></div>
                    <div class="action outgoing sfx2 ButtonTouch"><%=RS("outgoing") %></div>

                </div>

                <div class="separator" ></div>

                <div class="note">                   
                    <div>
                        <div class="noteUpdate sfx2 ButtonTouch" onclick="ROE.UI.VillageOverview.notePopup_show();"><a><%=RS("EditNoteNoBrac") %></a></div>
                    </div>
                    
                    <div class="currentNote"><em><%=RS("NoNotes") %></em></div>
                   
                                      
                </div>
                 <div class="separator" ></div>

            </div>

        
        
    </div>

    <div class="template notePopup">
       
        <div class="NotesText" style="display: none;"></div>
        <div class="EditMessage">
            <textarea rows="10" id="EditorNotes" class="editNotesTextarea" autofocus="" onfocus="ROE.Frame.inputFocused();" onblur="ROE.Frame.inputBlured();"></textarea>
            <div class="noteButtonWrapper">
                 <div class="saveButton">
                    <a class="Edit customButtomBG" onclick="ROE.UI.VillageOverview.notePopup_save(); $(this).closest('.simplePopupOverlay').remove();"><%=RS("Save") %></a>
                </div>
                <div class="clearButton">
                    <a class="Edit customButtomBG" onclick="$('.village_notePopup .editNotesTextarea').val('');"><%=RS("Clear") %></a>
                </div>

            </div>
            <div class="paddinator2000"></div>
        </div>
       
    </div>
   
    <div id="VillagePopupPhrases" style="display:none;">
        <%= RSdiv("NoNotes") %>
    </div>

</asp:Content>