CREATE TABLE [dbo].[Person]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [Full Name] NVARCHAR(128) NOT NULL, 
    [Address1] NVARCHAR(128) NOT NULL, 
    [Address2] NVARCHAR(128) NULL, 
    [Address3] NVARCHAR(128) NULL, 
    [City] NVARCHAR(128) NOT NULL, 
    [State] NVARCHAR(128) NOT NULL, 
    [Zip] INT NOT NULL, 
    [ISOCountry] NVARCHAR(3) NOT NULL
)
