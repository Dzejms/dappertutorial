using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using FluentAssertions;

namespace MicroOrmDemo.DataLayer.Tests
{
    [TestClass]
    public class DapperSpecificTests
    {
        [TestMethod]
        public void Bulk_insert_should_insert_3_rows()
        {
            // arrange
            var repository = new Dapper.ContactRepository();
            var contacts = new List<Contact> {
                new Contact { FirstName = "Charles", LastName = "Barkley" },
                new Contact { FirstName = "Scottie", LastName = "Pippen" },
                new Contact { FirstName = "Tim", LastName = "Duncan" },
                new Contact { FirstName = "Patrick", LastName = "Ewing" }
            };

            // act
            var rowsAffected = repository.BulkInsertContacts(contacts);

            // assert
            rowsAffected.Should().Be(4);
        }

        [TestMethod]
        public void List_support_should_produce_correct_results()
        {
            // arrange
            var repository = new Dapper.ContactRepository();

            // act
            var contacts = repository.GetContactsById(1, 3, 4);

            // assert
            contacts.Count.Should().Be(3);
        }

        [TestMethod]
        public void Dynamic_support_should_produce_correct_results()
        {
            // arrange
            var repository = new Dapper.ContactRepository();

            // act
            var contacts = repository.GetDynamicById(1, 3, 4);

            // assert
            contacts.Count.Should().Be(3);
            Assert.AreEqual("Michael", contacts.First().FirstName);
            Assert.AreEqual("Jordan", contacts.First().LastName);
        }
    }
}
