 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'dPromotedVillage')
	BEGIN
		DROP  Procedure  dPromotedVillage
	END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].dPromotedVillage
		@PromotedVillageID int,
		@playerid int
AS


BEGIN TRAN 

	IF exists (select * from villages where villageid = @PromotedVillageID and ownerplayerid = @playerID) BEGIN

		delete villages_absorbed where PromotedVillageID = @PromotedVillageID

		delete villages_Promoted where PromotedVillageID = @PromotedVillageID

	END
COMMIT

	