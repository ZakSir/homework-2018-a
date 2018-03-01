CREATE PROCEDURE [dbo].[spGetAshyPets]
AS
	SELECT CONCAT(dbo.Policies.CountryOfIssuance, RIGHT(CONCAT('0000000000', ISNULL(dbo.Policies.Id,'')), 10))
	FROM Policies 
	WHERE dbo.Policies.IssueDate >= CONVERT(DATETIME, '2012-06-30 00:00:00') AND dbo.Policies.IssueDate <= CONVERT(DATETIME, '2012-06-30 23:59:59')
RETURN 0
