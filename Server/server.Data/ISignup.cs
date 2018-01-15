using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace server.Data
{
    [ServiceContract]
    public interface ISignup
    {
        [OperationContract]
        Task<SignupResult> Add(SignupInput signupInput);
        [OperationContract]
        Task<QueryResult> Query(QueryInput queryInput);
    }
}
