using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DinkLabs.ClaimsAuth.Web.Controllers
{
    public class ApplicationApiController : ApiController
    {
        // GET: api/ApplicationApi
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/ApplicationApi/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/ApplicationApi
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/ApplicationApi/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ApplicationApi/5
        public void Delete(int id)
        {
        }
    }
}
