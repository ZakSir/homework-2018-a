CREATE PROCEDURE [dbo].[spAddPolicy]
	@petId BIGINT,
	@startDate DATETIME,
	@endDate DATETIME,
	@legalText NVARCHAR(MAX)
AS
	IF ( NOT EXISTS ( SELECT 1 FROM dbo.Pets WHERE dbo.Pets.Id = @petId))
		THROW 50000, 'Unsupport Country Code', 1;
	ELSE
		-- date time checks
		INSERT INTO dbo.Policies(dbo.Policies.CountryOfIssuance, dbo.Policies.IssueDate, dbo.Policies.StartDate, dbo.Policies.EndDate, dbo.Policies.PetId, dbo.Policies.PolicyLegalText)
		SELECT dbo.Person.ISOCountry, GETUTCDATE() AS "IssueDate", @startDate AS "StartDate", @endDate AS "EndDate", dbo.Pets.Id AS "PetId", @legalText AS "PolicyLegalText"
		FROM dbo.Pets
		INNER JOIN dbo.PetsOwners ON dbo.PetsOwners.PetId = dbo.Pets.Id
		INNER JOIN dbo.Person ON dbo.Person.Id = dbo.PetsOwners.OwnerId
		WHERE dbo.Pets.Id = @petId
RETURN 0
