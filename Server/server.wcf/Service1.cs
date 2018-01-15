using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using server.Data;

namespace server.wcf
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall)]
    public class Service1 : ISignUpService
    {
        public async Task<SignupResult> Add(SignupInput signupInput)
        {
            throw new NotImplementedException();
        }

        public async Task<QueryResult> Query(QueryInput queryInput)
        {
            throw new NotImplementedException();
        }
    }
}
