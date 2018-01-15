using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace server.Data
{
    [DataContract]
    public abstract class AbstractResult
    {
        [DataMember]
        public OperationStatus Status { get; set; }
        [DataMember]
        public List<Error> Errors { get; set; }
    }

    public enum OperationStatus
    {
        Added = 0,
        DuplicatedWhenAdding,
        ErrorsWhenAdding,

        DBError,
        NoError,
    }

    public enum ErrorType
    {
        EmptyEntry = 0,
        DataInvalid,
        BusinessInvalid,
        DBError
    }

    [DataContract]
    public class Error
    {
        [DataMember]
        public ErrorType ErrorType { get; set; }
        [DataMember]
        public string MembershipName { get; set; }
        [DataMember]
        public string DetailError { get; set; }
    }
}
