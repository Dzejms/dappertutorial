using MicroOrmDemo.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroOrmDemo.DataLayer.Persistence
{
    public interface IContactsRepository
    {
        Contact Find(int id);
        List<Contact> GetAll();
        Contact Add(Contact contact);
        Contact Update(Contact contact);
        void Remove(int id);
        Contact GetFullContact(int id);
        void Save(Contact contact);
    }
}
