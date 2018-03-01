CREATE PROCEDURE [dbo].[spSearchForBreed]
	@querystring NVARCHAR(255) NOT NULL
AS
	SELECT dbo.Breed.DisplayName
	FROM dbo.Breed
	WHERE FREETEXT(dbo.Breed.DisplayName, @querystring)
RETURN 0
