SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[usp_InsertDoctorServiceAndSlots]
(
    @DoctorId INT,
    @ServiceId UNIQUEIDENTIFIER,
    @StartTime TIME,
    @EndTime TIME
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @UserServiceLinkId UNIQUEIDENTIFIER;
    DECLARE @MaxDurationMinutes INT;

    -- Get MaxDuration for this service
	SELECT @MaxDurationMinutes = DATEDIFF(MINUTE, '00:00:00', MaxDuration)
	FROM Services
	WHERE Id = @ServiceId;

    IF @MaxDurationMinutes IS NULL OR @MaxDurationMinutes <= 0
    BEGIN
        RAISERROR('Invalid MaxDuration for the service', 16, 1);
        RETURN;
    END

    -- Check if UserServiceLink already exists
    SELECT @UserServiceLinkId = Id
    FROM UserServiceLinks
    WHERE UserId = @DoctorId AND ServiceId = @ServiceId;

    -- If not exists, insert
    IF @UserServiceLinkId IS NULL
    BEGIN
        SET @UserServiceLinkId = NEWID();

        INSERT INTO UserServiceLinks (Id, UserId, ServiceId, IsActive, CreatedAt, CreatedBy)
        VALUES (@UserServiceLinkId, @DoctorId, @ServiceId, 1, GETUTCDATE(), NULL);
    END

    -- Generate slots
    DECLARE @CurrentStart TIME = @StartTime;
    DECLARE @CurrentEnd TIME;

    WHILE (@CurrentStart < @EndTime)
    BEGIN
        SET @CurrentEnd = DATEADD(MINUTE, @MaxDurationMinutes, CAST(@CurrentStart AS DATETIME));

        IF @CurrentEnd > @EndTime
            SET @CurrentEnd = @EndTime;

        -- Check if slot already exists
        IF NOT EXISTS (
            SELECT 1 FROM Slots
            WHERE UserServiceLinkId = @UserServiceLinkId
              AND StartTime = @CurrentStart
              AND EndTime = @CurrentEnd
        )
        BEGIN
            INSERT INTO Slots (Id, UserServiceLinkId, StartTime, EndTime, IsActive, CreatedAt, CreatedBy)
            VALUES (NEWID(), @UserServiceLinkId, @CurrentStart, @CurrentEnd, 1, GETUTCDATE(), NULL);
        END

        SET @CurrentStart = @CurrentEnd;
    END
END
GO


