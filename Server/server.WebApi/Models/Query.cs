using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace server.WebApi.Models
{
    public class Query
    {
        public class QueryInputFromClient
        {
            // Search conditons
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int? FunEvent { get; set; }

            // Data size control
            public int? PageSize { get; set; }
            public int? CurrentPage { get; set; }
        }
    }
}