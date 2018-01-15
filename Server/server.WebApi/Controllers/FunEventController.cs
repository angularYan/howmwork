using server.WebApi.WcfServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace server.WebApi.Controllers
{
    // http://localhost:50029/api/FunEvent
    //[Authorize]
    public class FunEventController : ApiController
    {
        public async Task<IEnumerable<FunnyEvent>> Get()
        {
            SignUpServiceClient client = new SignUpServiceClient();
            return await client.GetFutureEventsAsync();
        }
    }
}
