using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using server.Data;

namespace server.Wcf.App
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall)]
    public class SignUpService : ISignUpService
    {
        public async Task<SignupResult> Add(SignupInput signupInput)
        {
            var signup = Factory.CreateSignup();
            return await signup.Add(signupInput);
        }

        public async Task<List<FunnyEvent>> GetFutureEvents()
        {
            var events = Factory.CreateFunEvent();
            return await events.GetFutureEvents();
        }

        public async Task<QueryResult> Query(QueryInput queryInput)
        {
            var signup = Factory.CreateSignup();
            return await signup.Query(queryInput);
        }
    }
}
