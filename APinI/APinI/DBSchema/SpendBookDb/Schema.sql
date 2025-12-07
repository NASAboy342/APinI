CREATE DATABASE [SpendBookDb]
GO
USE [SpendBookDb]
GO

-- User Table
CREATE TABLE [User] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [Username] NVARCHAR(255) NOT NULL UNIQUE,
    [Password] NVARCHAR(255) NOT NULL,
    [UtcCreatedOn] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UtcModifiedOn] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO

-- Account Table
CREATE TABLE Account (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [UserId] INT NOT NULL,
    [Name] NVARCHAR(255) NOT NULL,
    [Balance] DECIMAL(18, 6) NOT NULL DEFAULT 0,
    [Currency] NVARCHAR(10) NOT NULL,
    [UsdRate] DECIMAL(18, 6) NOT NULL DEFAULT 0,
    [UtcModifiedOn] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UtcCreatedOn] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UtcLastAccessedOn] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO

-- Transaction Table
CREATE TABLE [Transaction] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [AccountId] INT NOT NULL,
    [Amount] DECIMAL(18, 6) NOT NULL,
    [BalanceBefore] DECIMAL(18, 6) NOT NULL,
    [ReceiptUrl] NVARCHAR(MAX) NULL,
    [Remarks] NVARCHAR(MAX) NULL,
    [BalanceAfter] DECIMAL(18, 6) NOT NULL,
    [TimeStamp] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [TrackingTopicId] INT NULL,
);
GO

-- Payment Tracking Topic
CREATE TABLE PaymentTrackingTopic (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [Topic] NVARCHAR(255) NOT NULL,
    [UserId] INT NOT NULL,
    [UtcTargetDate] DATETIME2 NOT NULL,
    [TargetAmount] DECIMAL(18, 6) NOT NULL,
    [Currency] NVARCHAR(10) NOT NULL,
    [UtcCreatedOn] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UtcModifiedOn] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [Status] NVARCHAR(50) NOT NULL
);
GO

-- Indexes for better performance
CREATE INDEX IX_Account_UserId ON Account(UserId);
CREATE INDEX IX_Transaction_AccountId ON [Transaction](AccountId);
CREATE INDEX IX_Transaction_TimeStamp ON [Transaction]([TimeStamp]);
GO

CREATE PROCEDURE CreateUser
@username NVARCHAR(255),
@password NVARCHAR(255),
@utcCreateOn DATETIME2
AS
BEGIN
    INSERT INTO [User] (Username, Password, UtcCreatedOn, UtcModifiedOn)
    VALUES (@username, @password, @utcCreateOn, @utcCreateOn);
END
GO

CREATE PROCEDURE GetUserInfoByUsername
@username NVARCHAR(255)
AS
BEGIN
    SELECT [Id], [Username], [Password], [UtcCreatedOn], [UtcModifiedOn] FROM [User] WHERE Username = @username;
END
GO

CREATE PROCEDURE CreateAccount
@UserId INT,
@Name NVARCHAR(255),
@Balance DECIMAL(18, 6),
@Currency NVARCHAR(10),
@UsdRate DECIMAL(18, 6),
@UtcCreatedOn DATETIME2,
@UtcLastAccessedOn DATETIME2
AS
BEGIN
    INSERT INTO Account (UserId, [Name], Balance, Currency, UsdRate, UtcCreatedOn, UtcModifiedOn, UtcLastAccessedOn)
    VALUES (@UserId, @Name, @Balance, @Currency, @UsdRate, @UtcCreatedOn, @UtcCreatedOn, @UtcLastAccessedOn);
    
    SELECT 0 AS ErrorCode, 'Account created successfully' AS ErrorMessage;
END
GO

CREATE PROCEDURE GetAccountsByUsername
@username NVARCHAR(255)
AS
BEGIN
    SELECT 
        a.[Id], 
        a.[UserId],
        a.[Name], 
        a.[Balance], 
        a.[Currency], 
        a.[UsdRate], 
        a.[UtcCreatedOn], 
        a.[UtcModifiedOn], 
        a.[UtcLastAccessedOn]
    FROM Account a
    INNER JOIN [User] u ON a.UserId = u.Id
    WHERE u.Username = @username;
END
GO

CREATE PROCEDURE GetPaymentTrackingTopicsByUsername
@username NVARCHAR(255)
AS
BEGIN
    SELECT 
        pt.[Id], 
        pt.[Topic], 
        pt.[UserId], 
        pt.[UtcTargetDate], 
        pt.[TargetAmount], 
        pt.[Currency], 
        pt.[UtcCreatedOn], 
        pt.[UtcModifiedOn], 
        pt.[Status]
    FROM PaymentTrackingTopic pt
    INNER JOIN [User] u ON pt.UserId = u.Id
    WHERE u.Username = @username;
END
GO

CREATE PROCEDURE GetTransactionsByAccountIdAndDateRange
@accountId INT,
@startDate DATETIME2,
@endDate DATETIME2
AS
BEGIN
    SELECT 
        [Id], 
        [AccountId], 
        [Amount], 
        [BalanceBefore], 
        [ReceiptUrl], 
        [Remarks], 
        [BalanceAfter], 
        [TimeStamp], 
        [TrackingTopicId]
    FROM [Transaction]
    WHERE [AccountId] = @accountId
        AND [TimeStamp] >= @startDate
        AND [TimeStamp] <= @endDate
    ORDER BY [TimeStamp] DESC;
END
GO

CREATE PROCEDURE StoreTransaction
@accountId INT,
@amount DECIMAL(18, 6),
@balanceBefore DECIMAL(18, 6),
@balanceAfter DECIMAL(18, 6),
@receiptUrl NVARCHAR(MAX),
@remarks NVARCHAR(MAX),
@timeStamp DATETIME2,
@trackingTopicId INT
AS
BEGIN
    INSERT INTO [Transaction] (AccountId, Amount, BalanceBefore, BalanceAfter, ReceiptUrl, Remarks, [TimeStamp], TrackingTopicId)
    VALUES (@accountId, @amount, @balanceBefore, @balanceAfter, @receiptUrl, @remarks, @timeStamp, @trackingTopicId);
    
    SELECT 0 AS ErrorCode, 'Transaction stored successfully' AS ErrorMessage;
END
GO

CREATE PROCEDURE UpdateBalance
@accountId INT,
@amount DECIMAL(18, 6)
AS
BEGIN
    UPDATE Account
    SET Balance = Balance + @amount,
        UtcModifiedOn = GETUTCDATE()
    WHERE Id = @accountId;
    
    SELECT 0 AS ErrorCode, 'Balance updated successfully' AS ErrorMessage;
END
GO

CREATE PROCEDURE CreateTrackingTopic
@Topic NVARCHAR(255),
@UserId INT,
@UtcTargetDate DATETIME2,
@TargetAmount DECIMAL(18, 6),
@Currency NVARCHAR(10),
@UtcCreatedOn DATETIME2,
@Status NVARCHAR(50)
AS
BEGIN
    INSERT INTO PaymentTrackingTopic (Topic, UserId, UtcTargetDate, TargetAmount, Currency, UtcCreatedOn, UtcModifiedOn, [Status])
    VALUES (@Topic, @UserId, @UtcTargetDate, @TargetAmount, @Currency, @UtcCreatedOn, @UtcCreatedOn, @Status);
    
    SELECT 0 AS ErrorCode, 'Tracking topic created successfully' AS ErrorMessage;
END
GO

CREATE PROCEDURE UpdateAccount
@accountId INT,
@newAccountName NVARCHAR(255),
@modifiedOn DATETIME2
AS
BEGIN
    UPDATE Account
    SET [Name] = @newAccountName,
        UtcModifiedOn = @modifiedOn,
        UtcLastAccessedOn = @modifiedOn
    WHERE Id = @accountId;
    
    SELECT 0 AS ErrorCode, 'Account updated successfully' AS ErrorMessage;
END
GO

CREATE PROCEDURE UpdateTrackingTopic
@topicId INT,
@newName NVARCHAR(255),
@status NVARCHAR(50),
@modifiedOn DATETIME2
AS
BEGIN
    UPDATE PaymentTrackingTopic
    SET Topic = @newName,
        [Status] = @status,
        UtcModifiedOn = @modifiedOn
    WHERE Id = @topicId;
    
    SELECT 0 AS ErrorCode, 'Tracking topic updated successfully' AS ErrorMessage;
END
GO

