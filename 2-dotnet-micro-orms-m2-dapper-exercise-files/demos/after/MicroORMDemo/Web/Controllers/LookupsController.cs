using MicroOrmDemo.DataLayer;
using MicroOrmDemo.DataLayer.Dapper;
using System.Collections.Generic;
using System.Web.Http;

namespace MicroORMDemo.Web.Controllers
{
    public class LookupsController : ApiController
    {
        private LookupRepository repository = new LookupRepository();

        public List<State> GetStates()
        {
            return this.repository.GetStates();
        }
    }
}
