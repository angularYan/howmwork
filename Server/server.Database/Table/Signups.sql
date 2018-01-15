CREATE TABLE [dbo].[Signups]
(
    [FunEventId] INT NOT NULL,
    [Email] VARCHAR(30) NOT NULL, 
    [FirstName] VARCHAR(10) NOT NULL, 
    [LastName] VARCHAR(10) NOT NULL, 
    [Comments] VARCHAR(100) NULL, 
    [CreatedDatetime] DATETIME NOT NULL, 
    CONSTRAINT [PK_Signups] PRIMARY KEY ([FunEventId], [Email]), 
    CONSTRAINT [FK_Signups_FunEvents] FOREIGN KEY ([FunEventId]) REFERENCES [FunEvents]([FunEventId])
)
