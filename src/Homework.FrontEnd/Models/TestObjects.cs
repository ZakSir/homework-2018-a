using System;
using System.Collections.Generic;

namespace Homework.FrontEnd.Models
{
    public class TestObjects
    {
        private const string Washington = nameof(Washington);
        private const string US = nameof(US);
        private const string Earth = nameof(Earth);

        private static readonly Address Work1 = new Address()
        {
            streetNumber = 23331,
            streetName = "Builder Street",
            City = "pwnville",
            State = Washington,
            Country = US,
            Planet = Earth
        };

        private static readonly Address Work2 = new Address()
        {
            streetNumber = 3829,
            streetName = "Finance Road",
            City = "pwnville",
            State = Washington,
            Country = US,
            Planet = Earth
        };

        private static readonly Address Work3 = new Address()
        {
            streetNumber = 99321,
            streetName = "Construction Court",
            City = "buildtown",
            State = Washington,
            Country = US,
            Planet = Earth
        };

        private static readonly Human OwnerA = new Human()
        {
            Name = "talos"
        };

        private static readonly Human OwnerB = new Human()
        {
            Name = "barbara"
        };

        private static readonly Human OwnerC = new Human()
        {
            Name = "sven"
        };

        private static readonly Human OwnerD = new Human()
        {
            Name = "jimbo"
        };

        private Lazy<Human> bob;
        private Lazy<Human> alice;
        private Lazy<Human> jim;
        private Lazy<Cat> zelda;
        private Lazy<Cat> roxy;
        private Lazy<Cat> peach;
        private Lazy<Cat> daisy;
        private Lazy<Cat> theAdmiral;
        private Lazy<Cat> lieutenant;
        private Lazy<Dog> bowser;
        private Lazy<Dog> captain;
        private Lazy<Insect> bug1;
        private Lazy<Insect> bug2;

        public TestObjects()
        {
            this.bob = new Lazy<Human>(() => new Human()
            {
                Age = 32,
                BirthFather = new Human() { Name = "Jack" },
                BirthMother = new Human() { Name = "Mary" },
                Name = nameof(bob),
                Home = new Address()
                {
                    streetNumber = 1234,
                    streetName = "elm street",
                    City = "footown",
                    State = Washington,
                    Country = US,
                    Planet = Earth

                },
                Sex = "Male",
                Weight = 201.455,
                Work = Work1
            });

            this.alice = new Lazy<Human>(() => new Human()
            {
                Age = 22,
                BirthFather = new Human() { Name = "Ralph" },
                BirthMother = new Human() { Name = "Shannon" },
                Home = new Address()
                {
                    streetNumber = 234,
                    address2 = "Apt 3332",
                    streetName = "epic drive",
                    City = "miami",
                    State = "Florida",
                    Country = US,
                    Planet = Earth
                },
                Name = nameof(this.alice),
                Sex = "Female",
                Weight = 321,
                Work = Work2
            });

            this.jim = new Lazy<Human>(() => new Human()
            {
                Age = 66,
                BirthFather = new Human() { Name = "Stewart", Age = 399 },
                BirthMother = new Human() { Name = "Ruth", Age = 9001 },
                Home = new Address()
                {
                    streetNumber = 22,
                    streetName = "floop floopian drive",
                    City = "Redmond",
                    State = Washington,
                    Country = US
                        ,
                    Planet = Earth
                },
                Name = nameof(this.jim),
                Sex = "Male",
                Weight = 111,
                Work = Work3
            });

            this.zelda = new Lazy<Cat>(() => new Cat()
            {
                Age = 7,
                Breed = "Unknown",
                IsInsured = false,
                Name = nameof(this.zelda),
                Owners = new Human[] { OwnerA, OwnerD }
            });

            this.roxy = new Lazy<Cat>(() => new Cat()
            {
                Age = 10,
                Breed = "Gray",
                IsInsured = false,
                Name = nameof(this.roxy),
                Sex = "female",
                Owners = new Human[] { OwnerB, OwnerC },
                Weight = 12.1,
            });

            this.peach = new Lazy<Cat>(() => new Cat()
            {
                Age = 2,
                Breed = "Calico",
                IsInsured = false,
                Name = nameof(this.peach),
                Owners = new Human[] { OwnerB, OwnerC },
                Sex = "female",
                Weight = 5
            });

            this.daisy = new Lazy<Cat>(() => new Cat()
            {
                Age = 2,
                Breed = "Calico",
                IsInsured = false,
                Name = nameof(this.daisy),
                Owners = new Human[] { OwnerB, OwnerC },
                Sex = "female",
                Weight = 5
            });

            this.theAdmiral = new Lazy<Cat>(() => new Cat()
            {
                Age = 4,
                Breed = "Angry Cat",
                IsInsured = false,
                Name = nameof(this.theAdmiral),
                Owners = new Human[] { OwnerB, OwnerC },
                Sex = "female",
                Weight = 16
            });

            this.lieutenant = new Lazy<Cat>(() => new Cat()
            {
                Age = 1.5,
                Breed = "Cute",
                IsInsured = false,
                Name = nameof(this.lieutenant),
                Owners = new Human[] { OwnerB, OwnerC },
                Sex = "female",
                Weight = 11
            });

            this.bowser = new Lazy<Dog>(() => new Dog()
            {
                Age = 1.3,
                BirthFather = new Dog() { Name = "Goliath" },
                BirthMother = new Dog() { Name = "Sapphire" },
                Breed = "Siberian Husky",
                IsInsured = false,
                IsServiceAnimal = true,
                Name = "Bowser",
                Owners = new Human[] { OwnerA },
                Sex = "Male",
                Weight = 76.3,
                Diet = "MEAT"
            });

            this.captain = new Lazy<Dog>(() => new Dog()
            {
                Age = 8,
                Breed = "Labrador",
                Diet = "Dog Foods",
                IsInsured = false,
                IsServiceAnimal = false,
                Name = nameof(this.captain),
                Owners = new Human[] { OwnerA, OwnerB, OwnerC, OwnerD },
                Sex = "Male",
                Weight = 65
            });

            this.bug1 = new Lazy<Insect>(() => new Insect() { Age = .02, Sex = "Asexual", Weight = 0.003 });
            this.bug2 = new Lazy<Insect>(() => new Insect() { Age = .02, Sex = "Asexual", Weight = 0.003 });
        }

        public Human Bob => this.bob.Value;
        public Human Alice => this.alice.Value;
        public Human Jim => this.jim.Value;
        public Cat Zelda => this.zelda.Value;
        public Cat Roxy => this.roxy.Value;
        public Cat Peach => this.peach.Value;
        public Cat Daisy => this.daisy.Value;
        public Cat TheAdmiral => this.theAdmiral.Value;
        public Cat Lieutenant => this.lieutenant.Value;
        public Dog Bowser => this.bowser.Value;
        public Dog Captain => this.captain.Value;
        public Insect Bug1 => this.bug1.Value;
        public Insect Bug2 => this.bug2.Value;
    }

    public class TestObject {
        public string Id { get; private set; } = Guid.NewGuid().ToString("N");
    }

    public class Computer : TestObject
    {
        public long OperationsPerSecond { get; set; }
        public bool IsDistributed { get; set; }
        public string Name { get; set; }
    }

    public class Company : TestObject
    {
        public string Name { get; set; }
        public bool IsPublic { get; set; }
    }

    public class Animal : TestObject
    {
        public double Age { get; set; }

        public string Sex { get; set; }

        public double Weight { get; set; }
    }

    public class Vertibrate : Animal
    {

    }

    public class Invertibrate : Animal
    {

    }

    public class Anthropod : Invertibrate
    {

    }

    public class Insect : Anthropod {
        bool IsFlying { get; set; }
    }

    public class Mammal : Vertibrate
    {
        public virtual Mammal BirthMother { get; set; }

        public virtual Mammal BirthFather { get; set; }
    }

    public class Primate : Mammal
    {
        public string TribalIdentifier { get; set; }
    }

    public class Human : Primate
    {
        public string Name { get; set; }

        public Address Home { get; set; }

        public Address Work { get; set; }

        public IEnumerable<Human> KnownAssociates { get; set; }
    }

    public interface IPet {
        bool IsInsured { get; set; }
        string Name { get; set; }
        double Age { get; set; }
        double Weight { get; set; }
        string Breed { get; set; }
        IEnumerable<Human> Owners { get; set;}
    }

    public class Cat : Mammal, IPet {
        public string Name { get; set; }
        public bool IsInsured { get; set; }
        public virtual string Breed { get; set; }
        public IEnumerable<Human> Owners { get; set; }

        public new Cat BirthMother { get; set; }

        public new Cat BirthFather { get; set; }
    }

    public class Dog : Mammal, IPet {
        public string Name { get; set; }
        public bool IsInsured { get; set; }
        public virtual string Breed { get; set; }
        public IEnumerable<Human> Owners { get; set; }
        public bool IsServiceAnimal { get; set; }
        public string Diet { get; set; }

        public new Dog BirthMother { get; set; }

        public new Dog BirthFather { get; set; }
    }


    public class Address : TestObject
    {
        public int streetNumber { get; set; }
        public string streetName { get; set; }
        public string address2 { get; set; }
        public string address3 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Planet { get; set; }
    }
}
