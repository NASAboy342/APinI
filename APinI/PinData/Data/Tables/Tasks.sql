﻿CREATE TABLE [dbo].[Tasks]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[Name] NVARCHAR(50) NOT NULL,
	[Description] NVARCHAR(50) NOT NULL,
	[Status] NVARCHAR(50) NOT NULL
)
