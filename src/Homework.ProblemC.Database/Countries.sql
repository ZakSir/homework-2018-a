CREATE TABLE [dbo].[Countries]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [FullCountryName] NVARCHAR(128) NOT NULL, 
    [ISO] CHAR(3) NOT NULL
)
