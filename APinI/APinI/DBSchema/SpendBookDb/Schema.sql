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
    [UsdDate] DATETIME2 NULL,
    [UtcModifiedOn] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UtcCreatedOn] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
);
GO

-- Transaction Table
CREATE TABLE [Transaction] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [AccountId] INT NOT NULL,
    [Amount] DECIMAL(18, 6) NOT NULL,
    [BalanceBefore] DECIMAL(18, 6) NOT NULL,
    [ReceiptUrl] NVARCHAR(MAX) NULL,
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
    [UtcModifiedOn] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO

-- Indexes for better performance
CREATE INDEX IX_Account_UserId ON Account(UserId);
CREATE INDEX IX_Transaction_AccountId ON [Transaction](AccountId);
CREATE INDEX IX_Transaction_TimeStamp ON [Transaction]([TimeStamp]);
GO
