CREATE PROCEDURE [dbo].[spTransferPets]
	@previousOwner BIGINT NOT NULL,
	@newOwner BIGINT NOT NULL
AS
	BEGIN TRANSACTION;  
		INSERT INTO dbo.PetsOwners([OwnerId], [PetId])
		SELECT dbo.Pets.Id AS "PetId" from dbo.Pets, @newOwner as "OwnerId"
		INNER JOIN dbo.PetsOwners ON
		dbo.Pets.Id = dbo.PetsOwners.PetId
		WHERE dbo.PetsOwners.OwnerId = @previousOwner

		DELETE FROM dbo.PetsOwners
		WHERE dbo.PetsOwners.OwnerId = @previousOwner
	COMMIT
RETURN 0
