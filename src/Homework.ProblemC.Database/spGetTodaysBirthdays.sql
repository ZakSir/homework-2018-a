CREATE PROCEDURE [dbo].[spGetTodaysBirthdays]
AS
	SELECT dbo.Pets.Name, dbo.Pets.Birthdate
	FROM dbo.Pets
	WHERE dbo.Pets.BirthDayOfYear = DATEPART(dayofyear,GETUTCDATE())
RETURN 0
