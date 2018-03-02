BEGIN TRANSACTION;

INSERT INTO dbo.Animal(dbo.Animal.AnimalTypeDisplayName, dbo.Animal.AnimalTypeShortName)
VALUES('Dog', 'DOG');

INSERT INTO dbo.Animal(dbo.Animal.AnimalTypeDisplayName, dbo.Animal.AnimalTypeShortName)
VALUES('Cat', 'CAT');

INSERT INTO dbo.Breed(dbo.Breed.DisplayName, dbo.Breed.[Link], dbo.Breed.AnimalTypeId)
SELECT 'Siberian Husky' AS "DisplayName", 'http://www.akc.org/dog-breeds/siberian-husky/' AS "AKCLink", dbo.Animal.Id
FROM dbo.Animal 
WHERE dbo.Animal.AnimalTypeShortName = 'DOG'

INSERT INTO dbo.Breed(dbo.Breed.DisplayName, dbo.Breed.[Link])
SELECT 'Shiba Inu' = "DisplayName", 'http://www.akc.org/dog-breeds/shiba-inu/' = "AKCLink", dbo.Animal.Id
FROM dbo.Animal 
WHERE dbo.Animal.AnimalTypeShortName = 'DOG'

INSERT INTO dbo.Breed(dbo.Breed.DisplayName, dbo.Breed.[Link])
SELECT 'German Shepherd Dog' AS "DisplayName", 'http://www.akc.org/dog-breeds/german-shepherd-dog/' = "AKCLink", dbo.Animal.Id
FROM dbo.Animal 
WHERE dbo.Animal.AnimalTypeShortName = 'DOG'

INSERT INTO dbo.Breed(dbo.Breed.DisplayName, dbo.Breed.[Link])
SELECT 'Exotic Shorthair' AS "DisplayName", 'http://www.catbreedslist.com/all-cat-breeds/exotic-shorthair.html#.WpjauOjwaUk' = "AKCLink", dbo.Animal.Id
FROM dbo.Animal 
WHERE dbo.Animal.AnimalTypeShortName = 'CAT'


INSERT INTO dbo.Breed(dbo.Breed.DisplayName, dbo.Breed.[Link])
SELECT 'Persian' AS "DisplayName", 'http://www.catbreedslist.com/all-cat-breeds/persian-cat.html#.WpjauejwaUk' = "AKCLink", dbo.Animal.Id
FROM dbo.Animal 
WHERE dbo.Animal.AnimalTypeShortName = 'CAT'


INSERT INTO dbo.Breed(dbo.Breed.DisplayName, dbo.Breed.[Link])
SELECT 'Maine Coon' AS "DisplayName", 'http://www.catbreedslist.com/all-cat-breeds/maine-coon.html#.WpjauujwaUk' = "AKCLink", dbo.Animal.Id
FROM dbo.Animal 
WHERE dbo.Animal.AnimalTypeShortName = 'CAT'

COMMIT