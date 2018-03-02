CREATE TABLE [dbo].[Animal]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [AnimalTypeShortName] NCHAR(10) NOT NULL UNIQUE, 
    [AnimalTypeDisplayName] NVARCHAR(128) NOT NULL
)
