CREATE TABLE [dbo].[Pets]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [Name] NVARCHAR(128) NOT NULL, 
    [Birthdate] DATETIME NOT NULL, 
    [BreedId] BIGINT NOT NULL, 
    [ObjectCreatedDate] DATETIME NOT NULL, 
    [IsDeleted] BIT NOT NULL -- Never Delete
, 
    [BirthDayOfYear] TINYINT NOT NULL 
)
