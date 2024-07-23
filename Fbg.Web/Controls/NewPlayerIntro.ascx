<%@ Control Language="C#" AutoEventWireup="true" CodeFile="NewPlayerIntro.ascx.cs"
    Inherits="Controls_NewPlayerIntro" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<style>
.QIBody div {height:210px;}
</style>


<script type="text/javascript">
var imgPointer;
var vov;

$(
function () 
{
    imgPointer = $('img[id$=imgPointer]');
} 
);

 

function PointToShowQuests() 
{
    var lblProd = $('a[id$=linkQuests]');

    imgPointer.fadeIn('slow',function(){
      $(this).animate(
            {'top': lblProd.position().top  + 10
            , 'left': lblProd.position().left} 
            ,'5000');
    });
    $('#showme').hide();
}
	
</script>


        <asp:Panel ID="panelTutorial" runat="server" CssClass="QI">
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
            <table id="Table1" runat="server" cellpadding="0" cellspacing="0" width="100%">
                <tr>
                    <td class="QIBody">
                        <asp:Panel runat="server" ID="panelS1" Visible="false">
                            <img src='<%=RSic("farm")%>' style="float: left;
                                padding: 2px; height: 108px;" /><center>
                                <br /><br />
                                    <%= String.Format(RS("welcomeMsg"), Config.BaseUrl, _player.Realm.ID)%>
                                    <br />
                                    <SCRIPT language="javascript" src="http://www.sq2trk2.com/pixel.track?CID=162032&MerchantReferenceID="></SCRIPT>
                                    <NOSCRIPT><IMG src="http://www.sq2trk2.com/pixel.track?CID=162032&p=img&MerchantReferenceID=" width="1" height="1" border="0"></NOSCRIPT> 
                                </center>
                        </asp:Panel>
                        <asp:Panel runat="server" ID="panelS2" Visible="false">

                            <img src='<%=RSic("Gift_sack_of_silver")%>' style="float: left;
                                padding: 2px; height: 108px;" />
                            <center>
                            <br />
                                <%= RS("letsStartMsg")%>
                                
                                

                            </center>
                        </asp:Panel>
                        <asp:Panel runat="server" ID="panelS3" Visible="false">
                        
                            <img src='<%=RSic("Gift_sack_of_silver")%>' style="float: left;
                                padding: 2px; height: 105px;" />
                            <center>
                                <%= RS("clickThe") %>
                                <br />
                                <span style="color: #b6851f"><%= RS("showQuest")%></span>
                                <br />
                                <%= RS("linkOnRight")%>
                                <a id='showme' href="#" style="color:Black;" onclick="PointToShowQuests();return false;">
                                <img border="0" id="pointer" alt="->" src="https://static.realmofempires.com/images/misc/Arrow_pointer_East.gif" />
                                <BR />(<%= RS("showMe")%>)</a>
                                <br />
                                <br />
                                <%= RS("completeToGain")%>
                            </center>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td class="QIFooter">
                        <asp:Panel runat="server" ID="footer">
                        <center>
                            <asp:LinkButton CommandArgument="1" ID="linkNext" runat="server" OnCommand="linkNext_Click"><img border="0" src="https://static.realmofempires.com/images/misc/next_button.png"/></asp:LinkButton></li>
                                    </center>
                        </asp:Panel>
                    </td>
                </tr>
            </table>
    </ContentTemplate>
</asp:UpdatePanel>
        </asp:Panel>
<cc1:UpdatePanelAnimationExtender ID="UpdatePanelAnimationExtender1" runat="server"
    TargetControlID="UpdatePanel1" BehaviorID="animation" >
    <Animations>
            <OnUpdating>
                <Sequence>
                    <Parallel duration="1" Fps="30">
                        <FadeOut AnimationTarget="panelTutorial" minimumOpacity="0.2"></FadeOut>
                    </Parallel>
                </Sequence>

            </OnUpdating>
            <OnUpdated>
                <Sequence>
                    <Parallel duration="0.5" Fps="30">
                        <FadeIn AnimationTarget="panelTutorial" minimumOpacity="0.2"></FadeIn>
                    </Parallel>
                </Sequence>

            </OnUpdated>
           
    </Animations>
</cc1:UpdatePanelAnimationExtender>
