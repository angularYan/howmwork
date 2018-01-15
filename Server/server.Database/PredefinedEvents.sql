/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

INSERT INTO [FunEvents] ([Name] ,[StartDateTime])
     VALUES('Activity at High Park' ,'2018-02-01 09:00:00 AM');

INSERT INTO [FunEvents] ([Name] ,[StartDateTime])
     VALUES('Activity at Queens Park' ,'2018-02-11 01:00:00 PM');

INSERT INTO [FunEvents] ([Name] ,[StartDateTime])
     VALUES('Activity at Tech Room' ,'2018-02-18 03:00:00 PM');

GO