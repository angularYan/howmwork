﻿CREATE TABLE [dbo].[FunEvents]
(
	[FunEventId] INT NOT NULL PRIMARY KEY IDENTITY(100, 1), 
    [Name] VARCHAR(30) NOT NULL, 
    [StartDateTime] DATETIME NOT NULL
)
