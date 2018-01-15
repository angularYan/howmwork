using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace server.Data
{
    [DataContract]
    public class SignupResult : AbstractResult
    {
        [DataMember]
        public string ResourceId { get; set; }
    }
}
