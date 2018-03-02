CREATE PROCEDURE [dbo].[spGetPolicies]
	@personId BIGINT
AS
	SELECT CONCAT(dbo.Policies.CountryOfIssuance, RIGHT(CONCAT('0000000000', ISNULL(3,'')), 10))
	FROM Policies 
	INNER JOIN dbo.PetsOwners ON dbo.Policies.PetId = dbo.PetsOwners.PetId
	WHERE dbo.PetsOwners.OwnerId = @personId
RETURN 0
