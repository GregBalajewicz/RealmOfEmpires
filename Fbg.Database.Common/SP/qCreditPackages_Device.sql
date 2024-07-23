IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[qCreditPackages_Android]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[qCreditPackages_Android]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[qCreditPackages_Device]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[qCreditPackages_Device]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[qCreditPackages_Device]
	@SaleType int,
	@DeviceType int
	AS
	
BEGIN TRY
	
	SELECT [ProductID]
		  ,[Credits]
		  ,[SaleType]
		  ,[Price]
	FROM [CreditPackages_Device]
	WHERE [SaleType] = @SaleType AND [DeviceType] = @DeviceType

END TRY

BEGIN CATCH
	DECLARE @ERROR_MSG AS VARCHAR(MAX)
	IF @@TRANCOUNT > 0 ROLLBACK
	SET @ERROR_MSG = 'qCreditPackages_android FAILED! ' +  + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	
END CATCH


GO


