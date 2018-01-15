using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace server.Data
{
    [DataContract]
    public class SignupInput
    {
        [DataMember]
        public int FunEventId { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string Comments { get; set; }
    }
}
