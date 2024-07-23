IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[qAccountRecoveryLogin]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[qAccountRecoveryLogin]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create Procedure [dbo].[qAccountRecoveryLogin]
	@Email nvarchar(256)
AS

select top 1 UID, 
	case when LEN(OldUserName) < 20 then 'fb' else 'mob' end as LT
	from AccountRecovery
	where Email = @Email and ([State] = 1 or [State] = 2 or [State] = 3)
	order by [State] asc

go
