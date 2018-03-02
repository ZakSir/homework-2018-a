CREATE TABLE [dbo].[Breed]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [DisplayName] NVARCHAR(128) NOT NULL, 
    [Link]	 NVARCHAR(512) NOT NULL, 
    [AnimalTypeId] INT NOT NULL
)

GO


CREATE UNIQUE INDEX [IX_Breed_Displayname] ON [dbo].[Breed] ([DisplayName])

GO

CREATE FULLTEXT CATALOG ft AS DEFAULT;  

GO

CREATE FULLTEXT INDEX ON [dbo].[Breed] ([DisplayName])
   KEY INDEX [IX_Breed_Displayname]   
   WITH STOPLIST = SYSTEM;  
GO  

