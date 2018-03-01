CREATE TABLE [dbo].[Policies]
(
	[Id] BIGINT NOT NULL  IDENTITY(1,1) PRIMARY KEY,
	[CountryOfIssuance] CHAR(3) NOT NULL ,
	[PolicyId] CHAR(13),
	[IssueDate] DATETIME NOT NULL,
	[StartDate] DATETIME NOT NULL,
	[EndDate] DATETIME NOT NULL,
	[PetId] BIGINT NOT NULL, 
	[PolicyLegalText] NVARCHAR(MAX) NOT NULL
)

GO

CREATE INDEX [IX_POLICIES_ISSUEDATE] ON [dbo].[Policies] ([IssueDate])

GO

CREATE INDEX [IX_POLICIES_StartDate] ON [dbo].[Policies] ([StartDate])

GO