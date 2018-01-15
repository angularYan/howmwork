using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace server.Data
{
    [ServiceContract]
    public interface IFunEvent
    {
        [OperationContract]
        Task<List<FunnyEvent>> GetFutureEvents();
    }

    [DataContract]
    public class FunnyEvent
    {
        [DataMember]
        public DateTime EventStartDate { get; set; }
        [DataMember]
        public string FunEventName { get; set; }
        [DataMember]
        public int FunEventId { get; set; }
    }
}
