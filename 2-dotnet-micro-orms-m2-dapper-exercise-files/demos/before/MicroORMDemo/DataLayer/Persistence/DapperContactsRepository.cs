using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicroOrmDemo.DomainModel;


namespace MicroOrmDemo.DataLayer.Persistence
{
    public class DapperContactsRepository : IContactsRepository
    {
        private IDbConnection dbConnection;

        public DapperContactsRepository()
        {
            this.dbConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["ContactsDB"].ConnectionString);
        }

        public Contact Add(Contact contact)
        {
            string insertStatement = @"
                Insert Into Contacts (FirstName, LastName, Email, Company, Title)
                Values (@FirstName, @LastName, @Email, @Company, @Title)
                Select Cast(SCOPE_IDENTITY() as Int)
            ";
            contact.Id = dbConnection.Query<int>(insertStatement, contact).Single();

            return contact;
        }

        public Contact Find(int id)
        {
            return dbConnection.Query<Contact>("Select * From Contacts where Id = @Id", new { Id = id }).SingleOrDefault();
        }

        public List<Contact> GetAll()
        {
            return this.dbConnection.Query<Contact>("Select * from Contacts").ToList();
        }

        public Contact GetFullContact(int id)
        {
            throw new NotImplementedException();
        }

        public void Remove(int id)
        {
            dbConnection.Execute("Delete from Contacts where Id = @Id", new { Id = id });
        }

        public void Save(Contact contact)
        {
            throw new NotImplementedException();
        }

        public Contact Update(Contact contact)
        {
            string updateSql = @"
                Update Contacts Set
                    FirstName = @FirstName,
                    LastName = @LastName,
                    Email = @Email,
                    Company = @Company,
                    Title = @Title
                Where Id = @Id";
            return dbConnection.Query<Contact>(updateSql, contact).SingleOrDefault();
        }
    }
}
