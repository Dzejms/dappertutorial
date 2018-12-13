using System;
using System.Collections.Generic;
using MicroOrmDemo.DataLayer.Persistence;
using MicroOrmDemo.DomainModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Linq;

namespace MicroOrmDemo.DataLayer.Tests
{
    [TestClass]
    public class ContactsRepositoryTests
    {
        private IContactsRepository repository;
        private static int id; // gross

        [TestInitialize]
        public void SetUp()
        {
            repository = new DapperContactsRepository();
        }
        

        [TestMethod]
        public void Get_All_Should_Return_6_Results()
        {
            // Arrange

            // Act
            List<Contact> contacts = repository.GetAll();

            // Assert
            contacts.Should().NotBeNull();
            contacts.Count.Should().Be(6);
        }

        [TestMethod]
        public void Insert_Should_Assign_Identity_To_New_Entity()
        {
            // Arrange
            Contact contact = new Contact
            {
                FirstName = "James",
                LastName = "Curran",
                Email = "dzejms@gmail.com",
                Company = "Coda Software Solutions",
                Title = "Doer"
            };

            Address address = new Address
            {
                AddressType = "Home",
                StreetAddress = "123 Main Street",
                City = "Saint Petersburg",
                StateId = 1,
                PostalCode = "33701"
            };
            contact.Addresses.Add(address);

            // Act
            repository.Save(contact);

            // Assert
            contact.Id.Should().NotBe(0, "because identity should have been assigned by DB");
            id = contact.Id; // eww
        }

        [TestMethod]
        public void Find_Should_Retrieve_Existing_Entity()
        {
            // Arrange

            // Act
            Contact contact = repository.GetFullContact(id);

            // Assert
            contact.Should().NotBeNull();
            contact.Id.Should().Be(id);
            contact.Addresses.Count.Should().Be(1);
        }

        [TestMethod]
        public void Modify_Should_Update_Existing_Entity()
        {
            // Arrange
            Contact contact = repository.GetFullContact(id);

            // Act
            contact.Title = "CEO";
            contact.Addresses.First().PostalCode = "34219";

            repository.Save(contact);

            IContactsRepository repo2 = new DapperContactsRepository();
            Contact updatedContact = repo2.GetFullContact(id);

            // Assert
            updatedContact.Title.Should().Be("CEO");
            updatedContact.Addresses.First().PostalCode.Should().Be("34219");
        }

        [TestMethod]
        public void Delete_Should_Remove_Entity()
        {
            // Arrange


            // Act
            repository.Remove(id);
            IContactsRepository repo2 = new DapperContactsRepository();
            Contact deletedContact = repo2.Find(id);

            // Assert
            deletedContact.Should().BeNull();
        }
    }
}
