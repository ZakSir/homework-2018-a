CREATE PROCEDURE [dbo].[spAddPerson]
	@fullName NVARCHAR(128),
	@address1 NVARCHAR(128),
	@address2 NVARCHAR(128) = NULL,
	@address3 NVARCHAR(128) = NULL,
	@city NVARCHAR(128),
	@state NVARCHAR(128),
	@zip INT,
	@ISOCountryCode CHAR(3)
AS
	IF ( NOT EXISTS ( SELECT 1 FROM dbo.Countries WHERE dbo.Countries.ISO = @ISOCountryCode ))
		THROW 50000, 'Unsupport Country Code', 1;
	ELSE
		INSERT INTO dbo.Person(dbo.Person.FullName, dbo.Person.Address1, dbo.Person.Address2, dbo.Person.Address3, dbo.Person.City, dbo.Person.State, dbo.Person.Zip, dbo.Person.ISOCountry)
		VALUES (@fullName, @address1, @address2, @address3, @city, @state, @zip, @ISOCountryCode)
		
RETURN 0
