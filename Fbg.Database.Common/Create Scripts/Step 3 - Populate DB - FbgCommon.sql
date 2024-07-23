use FBGCommon
set nocount on 
--
-- ------------------------------------------------------------------
-- ------------------------------------------------------------------
-- delete first
-- ------------------------------------------------------------------
-- ------------------------------------------------------------------
--
delete PFEventTypes
delete InactivePlayersToBeWarned
delete StoriesPublished
delete invites
delete from PlayerSuspensions
delete from PlayerQ
delete from players 
delete from realms
delete from CreditPackages


delete from users where userid in ('00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000002') 
delete from aspnet_UsersInRoles where userid in ('00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000002') 
delete from aspnet_membership where userid in ('00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000002') 
delete from aspnet_users where userid in ('00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000002') 
delete from GiftsSent
delete from GiftsRealmAvailability
delete from Gifts
delete CreditPackages_Device 


--
-- ------------------------------------------------------------------
-- ------------------------------------------------------------------
-- Inserts
-- ------------------------------------------------------------------
-- ------------------------------------------------------------------ 
--

--insert into fbgcommon..realms values (300, '300', 'Ancient Glory',1,1,1, 'Data Source=localhost;Initial Catalog=fbg;Integrated Security=True;Max Pool Size=600;', 1000000,'2011-4-15 13:00:00', 0, 1, '', null )


insert into CreditPackages values (1, 30, 1.95)
insert into CreditPackages values (2, 100, 4.95)
insert into CreditPackages values (3, 200, 8.95)
insert into CreditPackages values (4, 500, 19.95)
insert into CreditPackages values (5, 1000, 29.95)
insert into CreditPackages values (6, 2000, 49.95)
insert into CreditPackages values (7, 4500, 99.95)
--
-- special packages, not available to all
--
insert into CreditPackages values (20, 100, 2.95)
insert into CreditPackages values (21, 200, 5.95)

--
-- subscriptions credit packages
--
insert into CreditPackages values (1000, 100, 3.95)
insert into CreditPackages values (1001, 200, 7.95)


--
-- insert special players
--
declare @appid uniqueidentifier
SELECT @AppID = ApplicationID from aspnet_applications where LoweredApplicationName = 'fbg'
IF @Appid is null BEGIN 
    exec aspnet_Applications_CreateApplication 'fbg', @appid output
END
insert into fbgcommon..aspnet_users values (@AppID ,'00000000-0000-0000-0000-000000000000', 0,0,null,0,getdate()  )
insert into fbgcommon..aspnet_users values (@AppID ,'00000000-0000-0000-0000-000000000001', -1,-1,null,0,getdate()  )
insert into fbgcommon..aspnet_users values (@AppID ,'00000000-0000-0000-0000-000000000002', -2,-2,null,0,getdate()  )



insert into fbgcommon..users (UserID, credits, timezone) values ('00000000-0000-0000-0000-000000000000',0,0)
insert into fbgcommon..users (UserID, credits, timezone) values ('00000000-0000-0000-0000-000000000001',0,0)
insert into fbgcommon..users (UserID, credits, timezone) values ('00000000-0000-0000-0000-000000000002',0,0)




insert into PFEventTypes values (1, 'Player Buys Servants-PayPal','')
insert into PFEventTypes values (4, 'Player is give servants','')
insert into PFEventTypes values (5, 'Servants for promo/reward','')
insert into PFEventTypes values (6, 'Servants used up','')
insert into PFEventTypes values (7, 'Servants Refund','')
insert into PFEventTypes values (11, 'Servants via OfferPal','')
insert into PFEventTypes values (12, 'Servants via AdParlor','')
insert into PFEventTypes values (15, 'Servants Refund-UpgradeToNP','')
insert into PFEventTypes values (16, 'Transfer servants to player','this player transfered servants to someone else')
insert into PFEventTypes values (17, 'Get servants via transfer','')
insert into PFEventTypes values (18, 'Credits taken away by admin','')
insert into PFEventTypes values (2, 'Player activates feature/package','')
insert into PFEventTypes values (3, 'Player extends feature/package','')
insert into PFEventTypes values (8, 'player cancels a feature/package','Q.7.2 Second table is for "(b) full prorated refund - this is for depreciated features only"')
insert into PFEventTypes values (9, 'player cancels a feature/package','Q.7.1.1 (a) partial prorateated refund - for all all active, not depreciated features. For a sitaiton where user get a full refund for an extension')
insert into PFEventTypes values (10, 'player cancels a feature/package','Q.7.1.1 (a) partial prorateated refund - for all all active, not depreciated features. For a sitaiton where user get a partial refund - package expires in less than its duration')
insert into PFEventTypes values (14, 'player cancels a feature/package(NP)','Q.7.3 Special refund when activating the Nobility package')
insert into PFEventTypes values (19, 'Servants via SuperRewards','')
insert into PFEventTypes values (20, 'Servants via gWallet','')
insert into PFEventTypes values (21, 'Servants via LinkEx','')
insert into PFEventTypes values (22, 'Servants via FBCredits','')
insert into PFEventTypes values (23, 'Servants via Quests','')
insert into PFEventTypes values (24, 'Servants used up to buy gift','')
insert into PFEventTypes values (25, 'Servants used for realm entry free','')
insert into PFEventTypes values (26, 'Servants via android PlayStore','')
insert into PFEventTypes values (27, 'Bonus Type Village','')
insert into PFEventTypes values (28, 'Reset','')
insert into PFEventTypes values (29, 'Servants via ios AppStore','')
insert into PFEventTypes values (30, 'Servants via Amazon','')
insert into PFEventTypes values (31, 'Servants via Kong','')
insert into PFEventTypes values (32, 'Servants via Farming','')
insert into PFEventTypes values (33, 'Loot Camp reshuffle','')
insert into PFEventTypes values (34, 'Buy Avatar','')
insert into PFEventTypes values (35, 'Loot Camp CatchUp','')

insert into  Gifts values (1, 1)
insert into  Gifts values (2, 2)
insert into  Gifts values (3, 2)
insert into  Gifts values (4, 2)
insert into  Gifts values (5, 2)
insert into  Gifts values (6, 2)
insert into  Gifts values (7, 2)
insert into  Gifts values (8, 2)
insert into  Gifts values (9, 2)



delete CreditPackages 
go


insert into UserLoginTypeIDs values (0, 'unknown')
insert into UserLoginTypeIDs values (1, 'FB')
insert into UserLoginTypeIDs values (2, 'Tactica')
insert into UserLoginTypeIDs values (3, 'Kong')
insert into UserLoginTypeIDs values (4, 'Mobile_Android')
insert into UserLoginTypeIDs values (5, 'Mobile_iOS')
insert into UserLoginTypeIDs values (6, 'Mobile_Amazon')
insert into UserLoginTypeIDs values (7, 'Mobile_Unknown')
insert into UserLoginTypeIDs values (8, 'FB_inferred')
insert into UserLoginTypeIDs values (9, 'Kong_inferred')
insert into UserLoginTypeIDs values (10, 'Mobile_Android_inferred')
insert into UserLoginTypeIDs values (11, 'Mobile_iOS_inferred')
insert into UserLoginTypeIDs values (12, 'Mobile_Amazon_inferred')
insert into UserLoginTypeIDs values (13, 'ArmoredGames')






insert into Avatars2 values (1,1,'https://static.realmofempires.com/images/Avatars/Av_FChar006.png','https://static.realmofempires.com/images/Avatars/M_BG_FChar006.jpg',null,null);
insert into Avatars2 values (2,1,'https://static.realmofempires.com/images/Avatars/Av_NChar001.png','https://static.realmofempires.com/images/Avatars/M_BG_NChar001.jpg',null,null);
insert into Avatars2 values (3,1,'https://static.realmofempires.com/images/Avatars/Av_FChar005.png','https://static.realmofempires.com/images/Avatars/M_BG_FChar005.jpg',null,null);
insert into Avatars2 values (4,1,'https://static.realmofempires.com/images/Avatars/Av_FChar002.png','https://static.realmofempires.com/images/Avatars/M_BG_FChar002.jpg',null,null);
insert into Avatars2 values (5,1,'https://static.realmofempires.com/images/Avatars/Av_MChar002.png','https://static.realmofempires.com/images/Avatars/M_BG_MChar002.jpg',null,null);
insert into Avatars2 values (6,1,'https://static.realmofempires.com/images/Avatars/Av_FChar001.png','https://static.realmofempires.com/images/Avatars/M_BG_FChar001.jpg',null,null);
insert into Avatars2 values (7,1,'https://static.realmofempires.com/images/Avatars/Av_MChar006.png','https://static.realmofempires.com/images/Avatars/M_BG_MChar006.jpg',null,null);
insert into Avatars2 values (8,1,'https://static.realmofempires.com/images/Avatars/Av_MChar003.png','https://static.realmofempires.com/images/Avatars/M_BG_MChar003.jpg',null,null);
insert into Avatars2 values (9,1,'https://static.realmofempires.com/images/Avatars/Av_FChar004.png','https://static.realmofempires.com/images/Avatars/M_BG_FChar004.jpg',null,null);
insert into Avatars2 values (10,1,'https://static.realmofempires.com/images/Avatars/Av_MChar004.png','https://static.realmofempires.com/images/Avatars/M_BG_MChar004.jpg',null,null);
insert into Avatars2 values (11,1,'https://static.realmofempires.com/images/Avatars/Av_MChar001.png','https://static.realmofempires.com/images/Avatars/M_BG_MChar001.jpg',null,null);
insert into Avatars2 values (12,1,'https://static.realmofempires.com/images/Avatars/Av_MChar005.png','https://static.realmofempires.com/images/Avatars/M_BG_MChar005.jpg',null,null);
insert into Avatars2 values (13,1,'https://static.realmofempires.com/images/Avatars/Av_MChar007.png','https://static.realmofempires.com/images/Avatars/M_BG_MChar007.jpg',null,null);
insert into Avatars2 values (14,1,'https://static.realmofempires.com/images/Avatars/Av_FChar003.png','https://static.realmofempires.com/images/Avatars/M_BG_FChar003.jpg',null,null);
insert into Avatars2 values (15,1,'https://static.realmofempires.com/images/Avatars/Av_NChar002.png','https://static.realmofempires.com/images/Avatars/M_BG_NChar002.jpg',null,null);
insert into Avatars2 values (16,1,'https://static.realmofempires.com/images/Avatars/Av_FChar007.png','https://static.realmofempires.com/images/Avatars/M_BG_FChar007.jpg',null,null);
insert into Avatars2 values (17,1,'https://static.realmofempires.com/images/Avatars/Av_FChar008.png','https://static.realmofempires.com/images/Avatars/M_BG_FChar008.jpg',null,null);
insert into Avatars2 values (18,1,'https://static.realmofempires.com/images/Avatars/Av_FChar009.png','https://static.realmofempires.com/images/Avatars/M_BG_FChar009.jpg',null,null);
insert into Avatars2 values (19,1,'https://static.realmofempires.com/images/Avatars/Av_MChar008.png','https://static.realmofempires.com/images/Avatars/M_BG_MChar008.jpg',null,null);
insert into Avatars2 values (20,1,'https://static.realmofempires.com/images/Avatars/Av_MChar009.png','https://static.realmofempires.com/images/Avatars/M_BG_MChar009.jpg',null,null);

insert into Avatars2 values (21,2,'https://static.realmofempires.com/images/Avatars/Av_PremChar001.png','https://static.realmofempires.com/images/Avatars/M_BG_PremChar001.jpg',null,null);
insert into Avatars2 values (22,2,'https://static.realmofempires.com/images/Avatars/Av_PremChar002.png','https://static.realmofempires.com/images/Avatars/M_BG_PremChar002.jpg',null,null);
insert into Avatars2 values (23,2,'https://static.realmofempires.com/images/Avatars/Av_PremChar003.png','https://static.realmofempires.com/images/Avatars/M_BG_PremChar003.jpg',null,null);
insert into Avatars2 values (24,2,'https://static.realmofempires.com/images/Avatars/Av_PremChar004.png','https://static.realmofempires.com/images/Avatars/M_BG_PremChar004.jpg',null,null);
insert into Avatars2 values (25,2,'https://static.realmofempires.com/images/Avatars/Av_PremChar005.png','https://static.realmofempires.com/images/Avatars/M_BG_PremChar005.jpg',null,null);
insert into Avatars2 values (26,2,'https://static.realmofempires.com/images/Avatars/Av_PremChar006.png','https://static.realmofempires.com/images/Avatars/M_BG_PremChar006.jpg',null,null);
insert into Avatars2 values (27,2,'https://static.realmofempires.com/images/Avatars/Av_PremChar007.png','https://static.realmofempires.com/images/Avatars/M_BG_PremChar007.jpg',null,null);
insert into Avatars2 values (28,2,'https://static.realmofempires.com/images/Avatars/Av_PremChar008.png','https://static.realmofempires.com/images/Avatars/M_BG_PremChar008.jpg',null,null);
insert into Avatars2 values (29,2,'https://static.realmofempires.com/images/Avatars/Av_PremChar009.png','https://static.realmofempires.com/images/Avatars/M_BG_PremChar009.jpg',null,null);
insert into Avatars2 values (30,2,'https://static.realmofempires.com/images/Avatars/Av_PremChar010.png','https://static.realmofempires.com/images/Avatars/M_BG_PremChar010.jpg',null,null);


/*
0 - english
2 - korean
*/
insert into GameAttributes values (24, 0, 'language') 

/*
0 - roe
1 - sw
*/
insert into GameAttributes values (25, 0, 'theme') 

