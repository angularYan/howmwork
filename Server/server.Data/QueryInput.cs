using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace server.Data
{
    [DataContract]
    public class QueryInput
    {
        // Search conditons
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public int FunEvent { get; set; }

        // Data size control
        [DataMember]
        public int PageSize { get; set; }
        [DataMember]
        public int CurrentPage { get; set; }
    }
}
