using server.WebApi.WcfServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;
using static server.WebApi.Models.Query;

namespace server.WebApi.Controllers
{
    public class SignupController : ApiController
    {
        // POST api/Signup
        [ResponseType(typeof(SignupInput))]
        public async Task<IHttpActionResult> Post([FromBody] SignupInput signupInput)
        {
            SignUpServiceClient client = new SignUpServiceClient();
            var result = await client.AddAsync(signupInput);

            if (result.Status == OperationStatus.Added)
            {
                return CreatedAtRoute<SignupResult>("DefaultApi", new { id = result.ResourceId }, result);
            }

            if (result.Status == OperationStatus.DuplicatedWhenAdding)
            {
                return Content<SignupResult>(System.Net.HttpStatusCode.Conflict, result);
            }

            if (result.Status == OperationStatus.ErrorsWhenAdding)
            {
                ModelStateDictionary model = GetError(result);
                return BadRequest(model);
            }

            if (result.Status == OperationStatus.DBError)
            {
                // {"Message":"An error has occurred.","ExceptionMessage":"DB Internal Error","ExceptionType":"System.Exception","StackTrace":null}
                return InternalServerError(new Exception("DB Internal Error"));
            }

            return InternalServerError(new Exception("Not handled request: " + result.Status.ToString()));
        }

        // GET api/Signup?fn=&ln=
        public async Task<IEnumerable<SignupRecord>> Get([FromUri] QueryInputFromClient queryInputFromClient)
        {
            QueryInput queryInput = new QueryInput()
            {
                 CurrentPage = queryInputFromClient.CurrentPage.HasValue? queryInputFromClient.CurrentPage.Value : 0,
                 FirstName = queryInputFromClient.FirstName,
                 LastName = queryInputFromClient.LastName,
                 FunEvent = queryInputFromClient.FunEvent.HasValue ? queryInputFromClient.FunEvent.Value : 0
            };

            SignUpServiceClient client = new SignUpServiceClient();
            var result = await client.QueryAsync(queryInput);
            return result.Records;
        }

        private ModelStateDictionary GetError(AbstractResult result)
        {
            ModelStateDictionary model = new ModelStateDictionary { };
            if (result.Errors == null || result.Errors.Count() == 0)
            {
                return model;
            }

            foreach (var error in result.Errors)
            {
                model.AddModelError(error.MembershipName, error.DetailError);
            }

            return model;
        }
    }
}

/*
 * 
 POST http://localhost:50029/api/Signup
 [{"key":"Content-Type","value":"application/json","description":""}]

 { 
   "FunEventId":"102", 
   "Email":"Yan@tom", 
   "FirstName":"Wei", 
   "LastName":"Wang",
   "Comments":"YourOwnPassword" 
}

    http://localhost:50029/api/Signup?CurrentPage=3
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int FunEvent { get; set; }
    public int CurrentPage { get; set; }
*/
