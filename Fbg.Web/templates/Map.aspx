<%@ Page Language="C#" MasterPageFile="~/templates/masterPopupTemplate_m.master" AutoEventWireup="true" CodeFile="Map.aspx.cs" Inherits="templates_Map" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
    <style>
        #content
        {
            /*
            width: 100%; height: 100%;
            */
        }
        
        #map
        {
            height: 330px;
            /*
            width: 100%; height: 100%;
            position: relative;
            */
        }
        
        #mapwrap
        {
            overflow: hidden;
            position: absolute;
            height: 100%; width: 100%;
            left: 0px; top: 0px;
        }
        
        @-webkit-keyframes mapmove 
        {
            from { -webkit-transform: translate3d(674px, 220px, 0px) scale3d(1, 1, 1); }
            to { -webkit-transform: translate3d(574px, 120px, 0px) scale3d(1,1,1); }
        }
        
        #surface { position: absolute; left: 0px; top: 0px; z-index: 0; -webkit-animation-name: none; }
        #surface.anim { -webkit-animation: mapmove 3s ease; -webkit-animation-fill-mode: forwards; }
        #surface img { -webkit-user-select: none; padding: 0px; margin: 0px; position: absolute; }
        #surface canvas { -webkit-user-select: none; padding: 0px; margin: 0px; position: absolute; /*background: url(/static/bigtile_cloud.png);*/ }
        
        #redpoint
        {
            background-color: yellow;
            z-index: 1000;
            width: 10px; height: 10px;
            border-radius: 5px;
            -webkit-border-radius: 5px;
            opacity: 0.5;
            position: absolute;
            top: 50%; left: 50%;
            -webkit-transform: translate3d(0px, 0px, 1px);
        }
        
        .doact .ui_item{ white-space : nowrap; padding : 2px; }
        .doact .vilName,
        .doact .hdr {  background-color:rgb(41, 33, 22); color:#c8a488; font-weight:normal; }
        /*
        .doact .help { color:#c8a488; height:15px; text-align:center; }
        */
        .doact .help
        {
            display: none;
        }
        .doact a{  color : rgb(195, 144, 55); }
        .doact a { 
            white-space : nowrap;  
            color : rgb(195, 144, 55); 
            background-image : url('https://static.realmofempires.com/images/menuItemSeperator.png'); 
            text-decoration : none; 
            /*
            background-color : rgb(75, 61, 48); 
            */
            background-color: transparent;
            background-repeat:no-repeat;
            /*
            background-position: 2px center;
            */
            background-position: center center;
            display:table-cell;
            /*
            width:30px;
            height:30px;
            */
            width: 44px;
            height: 44px;
        }
        
        .doact .na
        { 
            white-space : nowrap;  
            color : rgb(195, 144, 55); 
            background-image : url('https://static.realmofempires.com/images/menuItemSeperator.png'); 
            text-decoration : none; 
            /*
            background-color : rgb(75, 61, 48); 
            */
            background-color: transparent;
            background-repeat:no-repeat;
            /*
            background-position: 2px center;
            */
            background-position: center center;
            display:table-cell;
            /*
            width:30px;
            height:30px;
            */
            width: 44px;
            height: 44px;
        }

        .doact a:hover { color : rgb(195, 144, 55); text-decoration : underline; text-decoration : none; }
        .doact a.attq{ background-image : url('https://static.realmofempires.com/images/icons/Q_1ClickAttack.png');  }
        .doact  .attq_na{ background-image : url('https://static.realmofempires.com/images/icons/Q_1ClickAttackGray.png');  }
        .doact a.attq:hover { background-image : url('https://static.realmofempires.com/images/icons/Q_1ClickAttackHover.png'); }
        .doact a.att{ background-image : url('https://static.realmofempires.com/images/icons/Q_Attack.png');  }
        .doact a.att:hover { background-image : url('https://static.realmofempires.com/images/icons/Q_AttackHover.png'); }
        .doact a.sup{ background-image : url('https://static.realmofempires.com/images/icons/Q_Support.png');  }
        .doact  .sup_na{ background-image : url('https://static.realmofempires.com/images/icons/Q_SupportGray.png');  }
        .doact a.sup:hover{ background-image : url('https://static.realmofempires.com/images/icons/Q_SupportHover.png');  }
        .doact a.supq{ background-image : url('https://static.realmofempires.com/images/icons/Q_1ClickSupport.png');  }
        .doact  .supq_na{ background-image : url('https://static.realmofempires.com/images/icons/Q_1ClickSupportGray.png');  }
        .doact a.supq:hover{ background-image : url('https://static.realmofempires.com/images/icons/Q_1ClickSupportHover.png');  }
        .doact a.supl{ background-image : url('https://static.realmofempires.com/images/icons/Q_SupportLookup.png');  }
        .doact  .supl_na{ background-image : url('https://static.realmofempires.com/images/icons/Q_SupportLookupGray.png');  }
        .doact a.supl:hover{ background-image : url('https://static.realmofempires.com/images/icons/Q_SupportLookupHover.png');  }
        .doact a.map{ background-image : url('https://static.realmofempires.com/images/icons/Q_CenterMap.png');  }
        .doact a.map:hover{ background-image : url('https://static.realmofempires.com/images/icons/Q_CenterMapHover.png');  }
        .doact a.sends{ background-image : url('https://static.realmofempires.com/images/icons/Q_Silver.png');  }
        .doact  .sends_na{ background-image : url('https://static.realmofempires.com/images/icons/Q_SilverGray.png');  }
        .doact a.sends:hover{ background-image : url('https://static.realmofempires.com/images/icons/Q_SilverHover.png');  }
        .doact a.myvov{ background-image : url('https://static.realmofempires.com/images/icons/Q_Vov2.png');}
        .doact a.myvov:hover{ background-image : url('https://static.realmofempires.com/images/icons/Q_Vov2Hover.png');}
        .doact a.othervov{ background-image : url('https://static.realmofempires.com/images/helpicon.png');}
        .doact a.hq{ background-image : url('https://static.realmofempires.com/images/icons/Q_Upgrade.png');}
        .doact  .hq_na{ background-image : url('https://static.realmofempires.com/images/icons/Q_UpgradeGray.png');}
        .doact a.hq:hover{ background-image : url('https://static.realmofempires.com/images/icons/Q_UpgradeHover.png');}
        .doact a.renV{ background-image : url('https://static.realmofempires.com/images/icons/Q_Rename.png');}
        .doact .renV_na{ background-image : url('https://static.realmofempires.com/images/icons/Q_RenameGray.png');}
        .doact a.renV:hover{ background-image : url('https://static.realmofempires.com/images/icons/Q_RenameHover.png');}
        .doact a.recr{ background-image : url('https://static.realmofempires.com/images/icons/Q_Recruit.png');}
        .doact .recr_na{ background-image : url('https://static.realmofempires.com/images/icons/Q_RecruitGray.png');}
        .doact a.recr:hover{ background-image : url('https://static.realmofempires.com/images/icons/Q_RecruitHover.png');}

        .doact .highlight { padding: 3px; font-size: 35px;text-align: center; cursor: pointer }

        .facebookMenu .vilName a { white-space : nowrap; color : rgb(195, 144, 55); text-decoration : none;  }
        .facebookMenu .vilName a { white-space : nowrap; color : rgb(195, 144, 55); text-decoration : none; }
        .facebookMenu .vilName a:hover { text-decoration : underline; }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="Server">
    <div id="map">
        <div id="mapwrap">
            <div id="surface"></div>
            <div id="redpoint">&nbsp;</div>
            
        </div>
        <!--<div class="getInOutDataChangedEvent"></div>-->
    </div>
</asp:Content>
