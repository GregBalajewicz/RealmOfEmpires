 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qPromotedVillages')
	BEGIN
		DROP  Procedure  qPromotedVillages
	END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].qPromotedVillages	
	@playerid int
AS

select villageid, name, xcord, ycord, villages_promoted.promotedvillageid, absorbedvillageid from villages left join villages_promoted on promotedvillageid = villageid
 left join villages_absorbed on absorbedvillageid = villageid where ownerplayerid = @playerid
