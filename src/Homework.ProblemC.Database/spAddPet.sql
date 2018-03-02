CREATE PROCEDURE [dbo].[spAddPet]
	@ownerId BIGINT,
	@name NVARCHAR(128),
	@birthdate datetime,
	@breedId BIGINT
AS
	BEGIN TRANSACTION
		DECLARE @createdPet TABLE (Id BIGINT);
		DECLARE @createdPetId BIGINT;

		IF ( NOT EXISTS ( SELECT 1 FROM dbo.Breed WHERE dbo.Breed.Id = @ownerId ))
			ROLLBACK TRANSACTION
		ELSE
		BEGIN
			IF ( NOT EXISTS ( SELECT 1 FROM dbo.Person WHERE dbo.Person.Id = @ownerId ))
				ROLLBACK TRANSACTION 
			ELSE
				BEGIN
					INSERT INTO dbo.Pets(dbo.Pets.Name, dbo.Pets.Birthdate, dbo.Pets.BreedId, dbo.Pets.ObjectCreatedDate, dbo.Pets.IsDeleted, dbo.Pets.BirthDayOfYear)
					OUTPUT inserted.Id INTO @createdPet
					VALUES (@name, @birthdate, @breedId, GETUTCDATE(), 0, DATEPART(DAYOFYEAR, @birthdate));

					SELECT @createdPetId = "Id"
					FROM @createdPet

					INSERT INTO dbo.PetsOwners(dbo.PetsOwners.OwnerId, dbo.PetsOwners.PetId)
					VALUES(@ownerId, @createdPetId)

					COMMIT TRANSACTION
				END
		END	
RETURN 0
