 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'aspnet_Membership_UpdateUser')
	BEGIN
		DROP  Procedure  aspnet_Membership_UpdateUser
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].[aspnet_Membership_UpdateUser]
    @ApplicationName      nvarchar(256),
    @UserName             nvarchar(256),
    @Email                nvarchar(256),
    @Comment              ntext,
    @IsApproved           bit,
    @LastLoginDate        datetime,
    @LastActivityDate     datetime,
    @UniqueEmail          int,
    @CurrentTimeUtc       datetime
AS
BEGIN
    DECLARE @UserId uniqueidentifier
    DECLARE @ApplicationId uniqueidentifier
	DECLARE @OldLoweredEmail varchar(max)
    SELECT  @UserId = NULL
    SELECT  @UserId = u.UserId, @ApplicationId = a.ApplicationId, @OldLoweredEmail = LoweredEmail
    FROM    dbo.aspnet_Users u, dbo.aspnet_Applications a, dbo.aspnet_Membership m
    WHERE   LoweredUserName = LOWER(@UserName) AND
            u.ApplicationId = a.ApplicationId  AND
            LOWER(@ApplicationName) = a.LoweredApplicationName AND
            u.UserId = m.UserId

    IF (@UserId IS NULL)
        RETURN(1)

	-- when updating email, 
	SET @UniqueEmail = 0;

	-- Is this user a tactica account ?
	--	it is so, if username == email. 
	--	if so, we must change the username along with the password
	-- also insist the email is not used by anyone else
	DECLARE @isTacticaAccount bit 
	SET @isTacticaAccount = 0;
	IF (EXISTS (SELECT * FROM dbo.aspnet_Membership M join aspnet_Users U 
					ON U.UserID = M.UserID 
					WHERE M.ApplicationId = @ApplicationId  
						AND @UserId = M.UserId 
						AND LoweredEmail = LoweredUserName) )
	BEGIN 
		SET @isTacticaAccount = 1;
		SET @UniqueEmail = 1;
	END

 	-- Is this user a mobile-login account?
	--	it is so, insist the email is not used by anyone else
	IF (EXISTS (SELECT * FROM dbo.aspnet_Membership M join aspnet_Users U 
					ON U.UserID = M.UserID 
					WHERE M.ApplicationId = @ApplicationId  
						AND @UserId = M.UserId 
						AND LoweredEmail != LoweredUserName -- exclude tactica accounts
						AND len(username) > 20) -- exclude facebook accounts and kongregate accounts
						 )
	BEGIN 
		SET @UniqueEmail = 1;
	END

	--
	-- if unique email is required and  if we are changing the email, ensure it is unique
	--
    IF (@UniqueEmail = 1  AND @OldLoweredEmail <> LOWER(@Email))
    BEGIN
        IF (EXISTS (SELECT *
                    FROM  dbo.aspnet_Membership WITH (UPDLOCK, HOLDLOCK)
                    WHERE ApplicationId = @ApplicationId  AND @UserId <> UserId AND LoweredEmail = LOWER(@Email)))
        BEGIN
            RETURN(7)
        END
    END

	--
	-- for tactica accounts, if we are changing the email, we also need to make sure that no one has a username the same as this email 
	--
	IF (@isTacticaAccount = 1 AND @OldLoweredEmail <> LOWER(@Email))
    BEGIN
        IF (EXISTS (SELECT *
                    FROM  dbo.aspnet_Users WITH (UPDLOCK, HOLDLOCK)
                    WHERE ApplicationId = @ApplicationId  AND @UserId <> UserId AND LoweredUserName = LOWER(@Email)))
        BEGIN
            RETURN(7)
        END
    END

    DECLARE @TranStarted   bit
    SET @TranStarted = 0

    IF( @@TRANCOUNT = 0 )
    BEGIN
	    BEGIN TRANSACTION
	    SET @TranStarted = 1
    END
    ELSE
	SET @TranStarted = 0

    UPDATE dbo.aspnet_Users WITH (ROWLOCK)
    SET
         LastActivityDate = @LastActivityDate
    WHERE
       @UserId = UserId

    IF( @@ERROR <> 0 )
        GOTO Cleanup

    UPDATE dbo.aspnet_Membership WITH (ROWLOCK)
    SET
         Email            = @Email,
         LoweredEmail     = LOWER(@Email),
         Comment          = @Comment,
         IsApproved       = @IsApproved,
         LastLoginDate    = @LastLoginDate
    WHERE
       @UserId = UserId

    IF( @@ERROR <> 0 )
        GOTO Cleanup

	-- if tactica account, then also update the username to be the same as the email 
	IF (@isTacticaAccount = 1 AND @OldLoweredEmail <> LOWER(@Email))
    BEGIN
	    UPDATE dbo.aspnet_Users WITH (ROWLOCK)
		SET
			 UserName            = LOWER(@Email),
			 LoweredUserName     = LOWER(@Email)
		WHERE
		   @UserId = UserId

		IF( @@ERROR <> 0 )
			GOTO Cleanup
	END

    IF( @TranStarted = 1 )
    BEGIN
	SET @TranStarted = 0
	COMMIT TRANSACTION
    END

    RETURN 0

Cleanup:

    IF( @TranStarted = 1 )
    BEGIN
        SET @TranStarted = 0
    	ROLLBACK TRANSACTION
    END

    RETURN -1
END