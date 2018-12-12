using MicroOrmDemo.DataLayer;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MvcApplication1.Controllers
{
    public class ContactsController : ApiController
    {
        private IContactRepository repository = new MicroOrmDemo.DataLayer.Dapper.ContactRepository();

        public List<Contact> Get()
        {
            return this.repository.GetAll();
        }

        public Contact Get(int id)
        {
            return this.repository.GetFullContact(id);
        }

        public Contact Post(Contact contact)
        {
            this.repository.Save(contact);
            return contact;
        }

        public Contact Put(int id, Contact contact)
        {
            this.repository.Save(contact);
            return contact;
        }

        public HttpResponseMessage Delete(int id)
        {
            this.repository.Remove(id);

            var response = this.Request.CreateResponse(HttpStatusCode.NoContent);
            return response;
        }
    }
}
