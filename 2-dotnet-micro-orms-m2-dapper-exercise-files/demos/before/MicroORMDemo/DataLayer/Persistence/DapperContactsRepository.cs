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
using System.Transactions;

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
            //string selectStatement =
            //@"
            //    Select * from Contacts Where Id = @Id;
            //    Select * from Addresses Where ContactId = @Id;
            //";

            //using (var multipleResults = this.dbConnection.QueryMultiple(selectStatement, new { Id = id }))
            using (var multipleResults = this.dbConnection.QueryMultiple("GetContact", new { Id = id }, commandType: CommandType.StoredProcedure))
            {
                Contact contact = multipleResults.Read<Contact>().SingleOrDefault();
                List<Address> addresses = multipleResults.Read<Address>().ToList();

                if (contact != null & addresses != null)
                {
                    contact.Addresses.AddRange(addresses);
                }

                return contact;
            }
        }

        public void Remove(int id)
        {
            dbConnection.Execute("Delete from Contacts where Id = @Id", new { Id = id });
        }

        public void Save(Contact contact)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                //if (contact.IsNew)
                //{
                //    Add(contact);
                //}
                //else
                //{
                //    Update(contact);
                //}

                DynamicParameters saveContactParams = new DynamicParameters();
                saveContactParams.Add("@Id", value: contact.Id, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                saveContactParams.Add("@FirstName", contact.FirstName);
                saveContactParams.Add("@LastName", contact.LastName);
                saveContactParams.Add("@Company", contact.Company);
                saveContactParams.Add("@Title", contact.Title);
                saveContactParams.Add("@Email", contact.Email);
                this.dbConnection.Execute("SaveContact", saveContactParams, commandType: CommandType.StoredProcedure);
                contact.Id = saveContactParams.Get<int>("@Id");

                foreach (Address address in contact.Addresses.Where(ad => !ad.IsDeleted))
                {
                    address.ContactId = contact.Id;

                    //if (address.IsNew)
                    //{
                    //    Add(address);
                    //}
                    //else
                    //{
                    //    Update(address);
                    //}

                    DynamicParameters saveAddressParams = new DynamicParameters(new
                    {
                        address.ContactId,
                        address.AddressType,
                        address.StreetAddress,
                        address.City,
                        address.StateId,
                        address.PostalCode
                    });
                    saveAddressParams.Add("@Id", value: address.Id, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

                    this.dbConnection.Execute("SaveAddress", saveAddressParams, commandType: CommandType.StoredProcedure);
                    address.Id = saveAddressParams.Get<int>("@Id");
                }

                //if (contact.Addresses.Any(ad => ad.IsDeleted))
                //{
                //    string deleteStatement = "Delete from Addresses where Id in @Ids";
                //    IEnumerable<int> ids = contact.Addresses.Where(ad => ad.IsDeleted).Select(ad => ad.Id);
                //    dbConnection.Execute(deleteStatement, new { Ids = ids });
                //}

                foreach (var address in contact.Addresses.Where(addr => addr.IsDeleted))
                {
                    this.dbConnection.Execute("DeleteAddress", new { Id = address.Id }, commandType: CommandType.StoredProcedure);
                }

                scope.Complete();
            }
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

        public Address Add(Address address)
        {
            string insertStatement = @"
                Insert Into Addresses (ContactId, AddressType, StreetAddress, City, StateId, PostalCode)
                Values (@ContactId, @AddressType, @StreetAddress, @City, @StateId, @PostalCode)
                Select Cast(Scope_Identity() as int)
            ";

            address.Id = dbConnection.ExecuteScalar<int>(insertStatement, address);
            return address;
        }

        public Address Update(Address address)
        {
            string updateStatement = @"
                Update Addresses Set
                    ContactId = @ContactId,
                    AddressType = @AddressType,
                    StreetAddress = @StreetAddress,
                    City = @City,
                    StateId = @StateId,
                    PostalCode = @PostalCode
                Where Id = @Id
            ";

            dbConnection.Execute(updateStatement, address);
            return address;
        }
    }
}
