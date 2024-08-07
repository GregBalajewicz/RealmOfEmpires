IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = '_PopulateDB_Map')
BEGIN
	DROP  Procedure  _PopulateDB_Map
END

GO


CREATE Procedure dbo._PopulateDB_Map
	@Theme int = 0 -- 0 europe, 1, desert					

	AS
set nocount on


delete Landmarks
delete LandmarkTypeParts
delete LandmarkTypes


	--
	--
	--
	-- LandMarks
	--
	--
	--
	insert into LandMarkTypes (LandmarkTypeID, Name, Chance, CheckPosition) 
	values(1,'Mountain1',10,0)

	insert into LandMarkTypes (LandmarkTypeID, Name, Chance, CheckPosition)
	values(3,'Forest',0,0)	

	insert into LandMarkTypes (LandmarkTypeID, Name, Chance, CheckPosition)
	values(4,'ForestA',30,0)

	insert into LandMarkTypes (LandmarkTypeID, Name, Chance, CheckPosition)
	values(5,'ForestB',40,0)

	insert into LandMarkTypes (LandmarkTypeID, Name, Chance, CheckPosition)
	values(6,'ForestC',40,0)

	insert into LandMarkTypes (LandmarkTypeID, Name, Chance, CheckPosition)
	values(7,'ForestD',30,0)

	insert into LandMarkTypes (LandmarkTypeID, Name, Chance, CheckPosition)
	values(8,'ForestE',5,0)

	insert into LandMarkTypes (LandmarkTypeID, Name, Chance, CheckPosition)
	values(9,'ForestF',10,0)

	insert into LandMarkTypes (LandmarkTypeID, Name, Chance, CheckPosition)
	values(10,'LakeA',20,0)

	insert into LandMarkTypes (LandmarkTypeID, Name, Chance, CheckPosition)
	values(11,'LakeB',20,0)

	insert into LandMarkTypes (LandmarkTypeID, Name, Chance, CheckPosition)
	values(12,'LakeC',20,0)

	insert into LandMarkTypes (LandmarkTypeID, Name, Chance, CheckPosition)
	values(13,'MountainA',20,0)

	insert into LandMarkTypes (LandmarkTypeID, Name, Chance, CheckPosition)
	values(14,'MountainB',20,0)

	insert into LandMarkTypes (LandmarkTypeID, Name, Chance, CheckPosition)
	values(15,'RidgeA',10,0)

	insert into LandMarkTypes (LandmarkTypeID, Name, Chance, CheckPosition)
	values(16,'ForestG',40,0)

	insert into LandMarkTypes (LandmarkTypeID, Name, Chance, CheckPosition)
	values(18,'ForestI',40,0)

	insert into LandMarkTypes (LandmarkTypeID, Name, Chance, CheckPosition)
	values(19,'ForestJ',20,0)

	insert into LandMarkTypes (LandmarkTypeID, Name, Chance, CheckPosition)
	values(20,'ForestH',30,0)

	insert into LandMarkTypes (LandmarkTypeID, Name, Chance, CheckPosition)
	values(21,'HillA',20,0)

	insert into LandMarkTypes (LandmarkTypeID, Name, Chance, CheckPosition)
	values(22,'HillB',15,0)
	insert into LandMarkTypes (LandmarkTypeID, Name, Chance, CheckPosition)
	values(23,'ForestK',15,0)
	insert into LandMarkTypes (LandmarkTypeID, Name, Chance, CheckPosition)
	values(24,'ForestL',15,0)
	insert into LandMarkTypes (LandmarkTypeID, Name, Chance, CheckPosition)
	values(25,'ForestM',15,0)
	insert into LandMarkTypes (LandmarkTypeID, Name, Chance, CheckPosition)
	values(26,'GorgeA',15,0)

	insert into LandMarkTypes (LandmarkTypeID, Name, Chance, CheckPosition)
	values(27,'MountainC',15,0)
	insert into LandMarkTypes (LandmarkTypeID, Name, Chance, CheckPosition)
	values(28,'MountainD',10,0)
	insert into LandMarkTypes (LandmarkTypeID, Name, Chance, CheckPosition)
	values(29,'MountainE',10,0)
	insert into LandMarkTypes (LandmarkTypeID, Name, Chance, CheckPosition)
	values(30,'MountainF',14,0)
	insert into LandMarkTypes (LandmarkTypeID, Name, Chance, CheckPosition)
	values(31,'MountainG',14,0)

	------------Mountain
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(1,'hills1BL.png',0,0)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(1,'hills1BR.png',1,0)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(1,'hills1TL.png',0,1)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(1,'hills1TR.png',1,1)


	------------Forest
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(3,'forest1.png',0,0)

	------------ForestA
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(4,'ForestA_1x1.png',0,0)


	------------ForestB
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(5,'ForestB_1x1.png',0,0)

	------------ForestC
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(6,'ForestC_1x1.png',0,0)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(6,'ForestC_2x1.png',1,0)

	------------ForestD
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(7,'ForestD_1x1.png',0,0)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(7,'ForestD_2x1.png',1,0)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(7,'ForestD_3x1.png',2,0)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(7,'ForestD_4x1.png',3,0)

	------------Forest E
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(8,'ForestE_1x1.png',0,0)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(8,'ForestE_1x2.png',0,-1)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(8,'ForestE_2x1.png',1,0)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(8,'ForestE_1x3.png',2,0)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(8,'ForestE_1x4.png',3,0)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(8,'ForestE_1x5.png',4,0)

	------------Forest F
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(9,'ForestF_1x2.png',0,0)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(9,'ForestF_2x2.png',1,0)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(9,'ForestF_3x2.png',2,0)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(9,'ForestF_4x2.png',3,0)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(9,'ForestF_5x2.png',4,0)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(9,'ForestF_5x1.png',4,1)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(9,'ForestF_4x1.png',3,1)


	------------ Lake A
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(10,'LakeA_1x1.png',0,0)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(10,'LakeA_2x1.png',1,0)

	------------ Lake B
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(11,'LakeB_1x1.png',0,0)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(11,'LakeB_2x1.png',1,0)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(11,'LakeB_3x1.png',2,0)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(11,'LakeB_4x1.png',3,0)

	------------ Lake c
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(12,'LakeC_1x1.png',0,0)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(12,'LakeC_2x1.png',1,0)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(12,'LakeC_2x2.png',1,-1)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(12,'LakeC_3x1.png',2,0)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(12,'LakeC_3x2.png',2,-1)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(12,'LakeC_4x2.png',3,-1)


	------------MountainA
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(13,'MountainA_1x1.png',0,0)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(13,'MountainA_1x2.png',0,-1)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(13,'MountainA_2x1.png',1,0)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(13,'MountainA_2x2.png',1,-1)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(13,'MountainA_3x1.png',2,0)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(13,'MountainA_3x2.png',2,-1)


	------------MountainB
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(14,'MountainB_1x2.png',0,-1)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(14,'MountainB_2x2.png',1,-1)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(14,'MountainB_3x1.png',2,0)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(14,'MountainB_3x2.png',2,-1)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(14,'MountainB_4x1.png',3,0)

	------------Ridge A
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(15,'RidgeA_1x1.png',0,0)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(15,'RidgeA_2x1.png',1,0)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(15,'RidgeA_3x1.png',2,0)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(15,'RidgeA_3x2.png',2,-1)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(15,'RidgeA_4x2.png',3,-1)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(15,'RidgeA_5x2.png',4,-1)



	------------ForestI
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(18,'ForestI.png',0,0)

	------------ForestJ
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(19,'ForestJ_1x1.png',0,0)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(19,'ForestJ_2x1.png',1,0)

	------------ForestH
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(20,'ForestH.png',0,0)

	------------ForestG
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(16,'ForestG_1x1.png',0,0)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(16,'ForestG_1x2.png',0,-1)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(16,'ForestG_2x1.png',1,0)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(16,'ForestG_2x2.png',1,-1)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(16,'ForestG_3x1.png',2,0)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(16,'ForestG_3x2.png',2,-1)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(16,'ForestG_4x1.png',3,0)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(16,'ForestG_4x2.png',3,-1)

	------------HillA
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(21,'HillA_1x1.png',0,0)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(21,'HillA_2x1.png',1,0)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(21,'HillA_3x1.png',2,0)

	------------HillB
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(22,'HillB_1x1.png',0,0)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(22,'HillB_1x2.png',0,-1)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(22,'HillB_2x1.png',1,0)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(22,'HillB_2x2.png',1,-1)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(22,'HillB_3x1.png',2,0)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(22,'HillB_3x2.png',2,-1)





	------------ForestK
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(23,'ForestK_1x1.png',0,0)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(23,'ForestK_2x1.png',1,0)

	------------ForestL
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(24,'ForestL_3x1.png',2,0)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(24,'ForestL_1x2.png',0,-1)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(24,'ForestL_2x2.png',1,-1)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(24,'ForestL_3x2.png',2,-1)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(24,'ForestL_1x3.png',0,-2)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(24,'ForestL_2x3.png',1,-2)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(24,'ForestL_3x3.png',2,-2)

	------------ForestM
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(25,'ForestM_1x1.png',0,0)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(25,'ForestM_2x1.png',1,0)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(25,'ForestM_3x1.png',2,0)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(25,'ForestM_1x2.png',0,-1)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(25,'ForestM_2x2.png',1,-1)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(25,'ForestM_3x2.png',2,-1)


	------------ GorgeA
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(26,'GorgeA_2x1.png',1,0)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(26,'GorgeA_3x1.png',2,0)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(26,'GorgeA_1x2.png',0,-1)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(26,'GorgeA_2x2.png',1,-1)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(26,'GorgeA_1x3.png',0,-2)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(26,'GorgeA_2x3.png',1,-2)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(26,'GorgeA_1x4.png',0,-3)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(26,'GorgeA_2x4.png',1,-3)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(26,'GorgeA_1x5.png',0,-4)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(26,'GorgeA_2x5.png',1,-4)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(26,'GorgeA_1x6.png',0,-5)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(26,'GorgeA_2x6.png',1,-5)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(26,'GorgeA_1x7.png',0,-6)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(26,'GorgeA_2x7.png',1,-6)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(26,'GorgeA_1x8.png',0,-7)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(26,'GorgeA_2x8.png',1,-7)


	------------MountainC
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(27,'MountainC_1x1.png',0,0)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(27,'MountainC_2x1.png',1,0)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(27,'MountainC_3x1.png',2,0)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(27,'MountainC_1x2.png',0,-1)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(27,'MountainC_2x2.png',1,-1)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(27,'MountainC_3x2.png',2,-1)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(27,'MountainC_1x3.png',0,-2)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(27,'MountainC_2x3.png',1,-2)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(27,'MountainC_3x3.png',2,-2)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(27,'MountainC_4x3.png',3,-2)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(27,'MountainC_1x4.png',0,-3)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(27,'MountainC_2x4.png',1,-3)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(27,'MountainC_3x4.png',2,-3)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(27,'MountainC_4x4.png',3,-3)

	------------MountainD
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(28,'MountainD_2x1.png',1,0)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(28,'MountainD_3x1.png',2,0)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(28,'MountainD_1x2.png',0,-1)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(28,'MountainD_2x2.png',1,-1)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(28,'MountainD_3x2.png',2,-1)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(28,'MountainD_2x3.png',1,-2)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(28,'MountainD_3x3.png',2,-2)

	------------MountainE
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(29,'MountainE_1x1.png',0,0)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(29,'MountainE_2x1.png',1,0)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(29,'MountainE_3x1.png',2,0)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(29,'MountainE_1x2.png',0,-1)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(29,'MountainE_2x2.png',1,-1)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(29,'MountainE_1x3.png',0,-2)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(29,'MountainE_2x3.png',1,-2)


	------------MountainF
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(30,'MountainF_2x1.png',1,0)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(30,'MountainF_3x1.png',2,0)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(30,'MountainF_4x1.png',3,0)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(30,'MountainF_1x2.png',0,-1)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(30,'MountainF_2x2.png',1,-1)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(30,'MountainF_3x2.png',2,-1)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(30,'MountainF_4x2.png',3,-1)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(30,'MountainF_1x3.png',0,-2)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(30,'MountainF_2x3.png',1,-2)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(30,'MountainF_3x3.png',2,-2)

	------------MountainG
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(31,'MountainG_2x1.png',1,0)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(31,'MountainG_3x1.png',2,0)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(31,'MountainG_4x1.png',3,0)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(31,'MountainG_1x2.png',0,-1)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(31,'MountainG_2x2.png',1,-1)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(31,'MountainG_3x2.png',2,-1)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(31,'MountainG_4x2.png',3,-1)

	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(31,'MountainG_1x3.png',0,-2)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(31,'MountainG_2x3.png',1,-2)
	insert into LandMarkTypeParts(LandMarkTypeID,ImageURL,XCord,YCord)
	values(31,'MountainG_3x3.png',2,-2)



	------------Calling SP to put LandMarks on MAP
	exec iLandMarks












--
-- Map village icons 
--


	/*

	ORIGINAL

	
	insert into Map values ('https://static.realmofempires.com/images/map/village1a.png', 'hut', 200,0)
	insert into Map values ('https://static.realmofempires.com/images/map/village2a.png', 'hut', 800,0)
	insert into Map values ('https://static.realmofempires.com/images/map/village3a.png', 'hut', 2000,0)
	insert into Map values ('https://static.realmofempires.com/images/map/village4a.png', 'hut', 5000,0)
	insert into Map values ('https://static.realmofempires.com/images/map/village5a.png', 'hut', 10000,0)
	insert into Map values ('https://static.realmofempires.com/images/map/village6a.png', 'hut', 999999,0)

	*/


	insert into Map values ('https://static.realmofempires.com/images/map/VillVar1_1a.png', 'hut', 100,0)
	insert into Map values ('https://static.realmofempires.com/images/map/VillVar1_2a.png', 'village', 200,0)
	insert into Map values ('https://static.realmofempires.com/images/map/VillVar1_3a.png', 'town', 400,0)
	insert into Map values ('https://static.realmofempires.com/images/map/VillVar1_4a.png', 'mega town', 700,0)
	insert into Map values ('https://static.realmofempires.com/images/map/VillVar1_5a.png', 'metropolis', 1200,0)
	insert into Map values ('https://static.realmofempires.com/images/map/VillVar1_6a.png', 'metropolis', 2400,0)
	insert into Map values ('https://static.realmofempires.com/images/map/VillVar1_7a.png', 'metropolis', 4000,0)
	insert into Map values ('https://static.realmofempires.com/images/map/VillVar1_8a.png', 'metropolis', 6000,0)
	insert into Map values ('https://static.realmofempires.com/images/map/VillVar1_9a.png', 'metropolis', 8000,0)
	insert into Map values ('https://static.realmofempires.com/images/map/VillVar1_10a.png', 'metropolis', 9500,0)
	insert into Map values ('https://static.realmofempires.com/images/map/VillVar1_11a.png', 'metropolis', 11000,0)
	insert into Map values ('https://static.realmofempires.com/images/map/VillVar1_12a.png', 'metropolis', 13000,0)
	insert into Map values ('https://static.realmofempires.com/images/map/VillVar1_13a.png', 'metropolis', 17000,0)
	insert into Map values ('https://static.realmofempires.com/images/map/VillVar1_14a.png', 'metropolis', 50000,0)
	insert into Map values ('https://static.realmofempires.com/images/map/VillVar1_15a.png', 'metropolis', 99999999,0)

	-- silver bonus
	insert into Map values ('https://static.realmofempires.com/images/map/VillBsilver1a.png', 'hut', 100,1)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBsilver2a.png', 'village', 600,1)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBsilver3a.png', 'town', 1200,1)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBsilver4a.png', 'mega town', 2400,1)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBsilver5a.png', 'metropolis', 4800,1)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBsilver6a.png', 'metropolis', 7200,1)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBsilver7a.png', 'metropolis', 9000,1)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBsilver8a.png', 'metropolis', 12000,1)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBsilver9a.png', 'metropolis', 17000,1)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBsilver10a.png', 'metropolis', 99999999,1)

	--Barracks vill
	insert into Map values ('https://static.realmofempires.com/images/map/VillBbarracks1a.png', 'hut', 100,2)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBbarracks2a.png', 'village', 600,2)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBbarracks3a.png', 'town', 1200,2)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBbarracks4a.png', 'mega town', 2400,2)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBbarracks5a.png', 'metropolis', 4800,2)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBbarracks6a.png', 'metropolis', 7200,2)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBbarracks7a.png', 'metropolis', 9000,2)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBbarracks8a.png', 'metropolis', 12000,2)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBbarracks9a.png', 'metropolis', 17000,2)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBbarracks10a.png', 'metropolis', 99999999,2)

	-- all recruit bonus   
	insert into Map values ('https://static.realmofempires.com/images/map/VillBrecruitall1a.png', 'hut', 100,3)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBrecruitall2a.png', 'village', 600,3)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBrecruitall3a.png', 'town', 1200,3)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBrecruitall4a.png', 'mega town', 2400,3)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBrecruitall5a.png', 'metropolis', 4800,3)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBrecruitall6a.png', 'metropolis', 7200,3)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBrecruitall7a.png', 'metropolis', 9000,3)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBrecruitall8a.png', 'metropolis', 12000,3)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBrecruitall9a.png', 'metropolis', 17000,3)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBrecruitall10a.png', 'metropolis', 99999999,3)

	-- farm bonus   
	insert into Map values ('https://static.realmofempires.com/images/map/VillBfarm1a.png', 'hut', 100,4)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBfarm2a.png', 'village', 600,4)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBfarm3a.png', 'town', 1200,4)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBfarm4a.png', 'mega town', 2400,4)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBfarm5a.png', 'metropolis', 4800,4)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBfarm6a.png', 'metropolis', 7200,4)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBfarm7a.png', 'metropolis', 9000,4)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBfarm8a.png', 'metropolis', 12000,4)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBfarm9a.png', 'metropolis', 17000,4)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBfarm10a.png', 'metropolis', 99999999,4)

	-- def bonus   
	insert into Map values ('https://static.realmofempires.com/images/map/VillBwall1a.png', 'hut', 100,5)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBwall2a.png', 'village', 600,5)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBwall3a.png', 'town', 1200,5)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBwall4a.png', 'mega town', 2400,5)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBwall5a.png', 'metropolis', 4800,5)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBwall6a.png', 'metropolis', 7200,5)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBwall7a.png', 'metropolis', 9000,5)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBwall8a.png', 'metropolis', 12000,5)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBwall9a.png', 'metropolis', 17000,5)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBwall10a.png', 'metropolis', 99999999,5)

	-- stable recruit  bonus   
	insert into Map values ('https://static.realmofempires.com/images/map/VillBstable1a.png', 'hut', 100,6)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBstable2a.png', 'village', 600,6)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBstable3a.png', 'town', 1200,6)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBstable4a.png', 'mega town', 2400,6)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBstable5a.png', 'metropolis', 4800,6)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBstable6a.png', 'metropolis', 7200,6)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBstable7a.png', 'metropolis', 9000,6)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBstable8a.png', 'metropolis', 12000,6)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBstable9a.png', 'metropolis', 17000,6)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBstable10a.png', 'metropolis', 99999999,6)

	-- tavern recruit  bonus   
	insert into Map values ('https://static.realmofempires.com/images/map/VillBtav1a.png', 'hut', 100,7)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBtav2a.png', 'village', 600,7)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBtav3a.png', 'town', 1200,7)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBtav4a.png', 'mega town', 2400,7)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBtav5a.png', 'metropolis', 4800,7)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBtav6a.png', 'metropolis', 7200,7)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBtav7a.png', 'metropolis', 9000,7)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBtav8a.png', 'metropolis', 12000,7)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBtav9a.png', 'metropolis', 17000,7)
	insert into Map values ('https://static.realmofempires.com/images/map/VillBtav10a.png', 'metropolis', 99999999,7)


	------LVs

	-- Silver Hall
	insert into Map values ('https://static.realmofempires.com/images/map/LV_05_01.png', 'hut', 100,8)
	insert into Map values ('https://static.realmofempires.com/images/map/LV_05_02.png', 'village', 600,8)
	insert into Map values ('https://static.realmofempires.com/images/map/LV_05_03.png', 'town', 1200,8)
	insert into Map values ('https://static.realmofempires.com/images/map/LV_05_04.png', 'mega town', 2400,8)
	insert into Map values ('https://static.realmofempires.com/images/map/LV_05_05.png', 'metropolis', 4800,8)
	insert into Map values ('https://static.realmofempires.com/images/map/LV_05_06.png', 'metropolis', 7200,8)
	insert into Map values ('https://static.realmofempires.com/images/map/LV_05_07.png', 'metropolis', 9000,8)
	insert into Map values ('https://static.realmofempires.com/images/map/LV_05_08.png', 'metropolis', 12000,8)
	insert into Map values ('https://static.realmofempires.com/images/map/LV_05_09.png', 'metropolis', 17000,8)
	insert into Map values ('https://static.realmofempires.com/images/map/LV_05_10.png', 'metropolis', 99999999,8)

	-- House of Whispers   
	insert into Map values ('https://static.realmofempires.com/images/map/LV_HOW_1.png', 'hut', 100,9)
	insert into Map values ('https://static.realmofempires.com/images/map/LV_HOW_2.png', 'village', 600,9)
	insert into Map values ('https://static.realmofempires.com/images/map/LV_HOW_3.png', 'town', 1200,9)
	insert into Map values ('https://static.realmofempires.com/images/map/LV_HOW_4.png', 'mega town', 2400,9)
	insert into Map values ('https://static.realmofempires.com/images/map/LV_HOW_5.png', 'metropolis', 4800,9)
	insert into Map values ('https://static.realmofempires.com/images/map/LV_HOW_6.png', 'metropolis', 7200,9)
	insert into Map values ('https://static.realmofempires.com/images/map/LV_HOW_7.png', 'metropolis', 9000,9)
	insert into Map values ('https://static.realmofempires.com/images/map/LV_HOW_8.png', 'metropolis', 12000,9)
	insert into Map values ('https://static.realmofempires.com/images/map/LV_HOW_9.png', 'metropolis', 17000,9)
	insert into Map values ('https://static.realmofempires.com/images/map/LV_HOW_10.png', 'metropolis', 99999999,9)

	-- Bloodlance   
	insert into Map values ('https://static.realmofempires.com/images/map/LV_06_01.png', 'hut', 100,10)
	insert into Map values ('https://static.realmofempires.com/images/map/LV_06_02.png', 'village', 600,10)
	insert into Map values ('https://static.realmofempires.com/images/map/LV_06_03.png', 'town', 1200,10)
	insert into Map values ('https://static.realmofempires.com/images/map/LV_06_04.png', 'mega town', 2400,10)
	insert into Map values ('https://static.realmofempires.com/images/map/LV_06_05.png', 'metropolis', 4800,10)
	insert into Map values ('https://static.realmofempires.com/images/map/LV_06_06.png', 'metropolis', 7200,10)
	insert into Map values ('https://static.realmofempires.com/images/map/LV_06_07.png', 'metropolis', 9000,10)
	insert into Map values ('https://static.realmofempires.com/images/map/LV_06_08.png', 'metropolis', 12000,10)
	insert into Map values ('https://static.realmofempires.com/images/map/LV_06_09.png', 'metropolis', 17000,10)
	insert into Map values ('https://static.realmofempires.com/images/map/LV_06_10.png', 'metropolis', 99999999,10)

	-- Ironhand Mercs
	insert into Map values ('https://static.realmofempires.com/images/map/LV_03_01.png', 'hut', 100,11)
	insert into Map values ('https://static.realmofempires.com/images/map/LV_03_02.png', 'village', 600,11)
	insert into Map values ('https://static.realmofempires.com/images/map/LV_03_03.png', 'town', 1200,11)
	insert into Map values ('https://static.realmofempires.com/images/map/LV_03_04.png', 'mega town', 2400,11)
	insert into Map values ('https://static.realmofempires.com/images/map/LV_03_05.png', 'metropolis', 4800,11)
	insert into Map values ('https://static.realmofempires.com/images/map/LV_03_06.png', 'metropolis', 7200,11)
	insert into Map values ('https://static.realmofempires.com/images/map/LV_03_07.png', 'metropolis', 9000,11)
	insert into Map values ('https://static.realmofempires.com/images/map/LV_03_08.png', 'metropolis', 12000,11)
	insert into Map values ('https://static.realmofempires.com/images/map/LV_03_09.png', 'metropolis', 17000,11)
	insert into Map values ('https://static.realmofempires.com/images/map/LV_03_10.png', 'metropolis', 99999999,11)

	-- Endless Fields  
	insert into Map values ('https://static.realmofempires.com/images/map/LV_04_01.png', 'hut', 100,12)
	insert into Map values ('https://static.realmofempires.com/images/map/LV_04_02.png', 'village', 600,12)
	insert into Map values ('https://static.realmofempires.com/images/map/LV_04_03.png', 'town', 1200,12)
	insert into Map values ('https://static.realmofempires.com/images/map/LV_04_04.png', 'mega town', 2400,12)
	insert into Map values ('https://static.realmofempires.com/images/map/LV_04_05.png', 'metropolis', 4800,12)
	insert into Map values ('https://static.realmofempires.com/images/map/LV_04_06.png', 'metropolis', 7200,12)
	insert into Map values ('https://static.realmofempires.com/images/map/LV_04_07.png', 'metropolis', 9000,12)
	insert into Map values ('https://static.realmofempires.com/images/map/LV_04_08.png', 'metropolis', 12000,12)
	insert into Map values ('https://static.realmofempires.com/images/map/LV_04_09.png', 'metropolis', 17000,12)
	insert into Map values ('https://static.realmofempires.com/images/map/LV_04_10.png', 'metropolis', 99999999,12)


