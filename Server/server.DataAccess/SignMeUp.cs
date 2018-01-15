using server.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.DataAccess
{
    public class SignMeUp : ISignup, IFunEvent
    {
        public async Task<SignupResult> Add(SignupInput signupInput)
        {
            SignupResult signupResult = new SignupResult() {  Status = OperationStatus.NoError, Errors = new List<Error>(), ResourceId = "" };

            if (signupInput == null)
            {
                signupResult.Status = OperationStatus.ErrorsWhenAdding;
                signupResult.Errors.Add( new Error {  ErrorType= ErrorType.EmptyEntry, DetailError="No Signu up Info Provided.", MembershipName = string.Empty});
                return signupResult;
            }

            if (string.IsNullOrWhiteSpace(signupInput.FirstName))
            {
                signupResult.Status = OperationStatus.ErrorsWhenAdding;
                signupResult.Errors.Add(new Error { ErrorType = ErrorType.DataInvalid, DetailError = "First Name is Required", MembershipName = "FirstName" });
            }

            if (string.IsNullOrWhiteSpace(signupInput.LastName))
            {
                signupResult.Status = OperationStatus.ErrorsWhenAdding;
                signupResult.Errors.Add(new Error { ErrorType = ErrorType.DataInvalid, DetailError = "Last Name is Required", MembershipName = "LastName" });
            }

            if (string.IsNullOrWhiteSpace(signupInput.Email))
            {
                signupResult.Status = OperationStatus.ErrorsWhenAdding;
                signupResult.Errors.Add(new Error { ErrorType = ErrorType.DataInvalid, DetailError = "Email is Required", MembershipName = "Email" });
            }

            if (signupInput.FunEventId <= 0)
            {
                signupResult.Status = OperationStatus.ErrorsWhenAdding;
                signupResult.Errors.Add(new Error { ErrorType = ErrorType.DataInvalid, DetailError = "Event is Required", MembershipName = "FunEventId" });
            }

            if (!string.IsNullOrWhiteSpace(signupInput.FirstName) && signupInput.FirstName.Length > 10)
            {
                signupResult.Status = OperationStatus.ErrorsWhenAdding;
                signupResult.Errors.Add(new Error { ErrorType = ErrorType.DataInvalid, DetailError = "First Name is more than 10 letters", MembershipName = "FirstName" });
            }

            if (!string.IsNullOrWhiteSpace(signupInput.LastName) && signupInput.LastName.Length > 10)
            {
                signupResult.Status = OperationStatus.ErrorsWhenAdding;
                signupResult.Errors.Add(new Error { ErrorType = ErrorType.DataInvalid, DetailError = "Last Name is more than 10 letters", MembershipName = "LastName" });
            }

            if (!string.IsNullOrWhiteSpace(signupInput.Email) && signupInput.Email.Length > 30)
            {
                signupResult.Status = OperationStatus.ErrorsWhenAdding;
                signupResult.Errors.Add(new Error { ErrorType = ErrorType.DataInvalid, DetailError = "Email is more than 30 letters", MembershipName = "Email" });
            }

            if (!string.IsNullOrWhiteSpace(signupInput.Comments) && signupInput.Comments.Length > 100)
            {
                signupResult.Status = OperationStatus.ErrorsWhenAdding;
                signupResult.Errors.Add(new Error { ErrorType = ErrorType.DataInvalid, DetailError = "Comments is more than 30 letters", MembershipName = "Comments" });
            }

            if (signupResult.Status != OperationStatus.NoError)
            {
                return signupResult;
            }

            try
            {
                using (HomeworkEntities context = new HomeworkEntities())
                {
                    using (var dbContextTransaction = context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                    {
                        string queryEvent = "SELECT FunEventId FROM FunEvents WITH (UPDLOCK) WHERE FunEventId = {0}";
                        var existingEvent = await context.Database.SqlQuery<int>(queryEvent, signupInput.FunEventId).SingleOrDefaultAsync();
                        if (existingEvent == 0)
                        {
                            signupResult.Status = OperationStatus.ErrorsWhenAdding;
                            signupResult.Errors.Add(new Error { ErrorType = ErrorType.DataInvalid, DetailError = "Event is not Existing", MembershipName = "FunEventId" });
                            return signupResult;
                        }

                        string query = "SELECT FunEventId, Email FROM Signups WITH (UPDLOCK) WHERE FunEventId = {0} AND Email = {1}";
                        object[] parameters = { signupInput.FunEventId, signupInput.Email };
                        var existingItem = await context.Database.SqlQuery<Row>(query, parameters).SingleOrDefaultAsync();
                        if (existingItem != null)
                        {
                            signupResult.Status = OperationStatus.DuplicatedWhenAdding;
                            signupResult.ResourceId = string.Format("{0}###{1}", existingItem.Email, existingItem.FunEventId);
                            return signupResult;
                        }

                        Signup signup = new Signup();
                        signup.FirstName = signupInput.FirstName;
                        signup.LastName = signupInput.LastName;
                        signup.Email = signupInput.Email;
                        signup.FunEventId = signupInput.FunEventId;
                        signup.Comments = signupInput.Comments;
                        signup.CreatedDatetime = DateTime.Now;
                        context.Signups.Add(signup);

                        try
                        {
                            await context.SaveChangesAsync();
                            dbContextTransaction.Commit();
                            signupResult.Status = OperationStatus.Added;
                            signupResult.ResourceId = string.Format("{0}###{1}", signupInput.Email, signupInput.FunEventId);
                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            signupResult = new SignupResult() { Errors = new List<Error> { }, Status = OperationStatus.DBError };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Permission issue..
                throw ex;
            }

            return signupResult;
        }

        public async Task<QueryResult> Query(QueryInput queryInput)
        {
            QueryResult queryResult = new QueryResult() {  Status = OperationStatus.NoError, Records = new List<SignupRecord>() };
            
            int PageSize = 20; // We defined this in here.

            bool skipFirstName = string.IsNullOrWhiteSpace(queryInput.FirstName);
            bool skipLastName = string.IsNullOrWhiteSpace(queryInput.LastName);
            bool AllEvents = queryInput.FunEvent == 0;
            try
            {
                using (HomeworkEntities context = new HomeworkEntities())
                {
                    var query = from sign in context.Signups
                                join funEvent in context.FunEvents on sign.FunEventId equals funEvent.FunEventId
                                where
                                    (skipFirstName || sign.FirstName.Contains(queryInput.FirstName)) &&
                                    (skipLastName || sign.LastName.Contains(queryInput.LastName)) &&
                                    (AllEvents || sign.FunEventId == queryInput.FunEvent)
                                orderby sign.CreatedDatetime ascending
                                select new
                                {
                                    funEvent.Name,
                                    funEvent.StartDateTime,
                                    funEvent.FunEventId,

                                    sign.FirstName,
                                    sign.LastName,
                                    sign.Email,
                                    sign.Comments
                                };

                    var data = await query.Skip(queryInput.CurrentPage * PageSize).Take(PageSize).ToListAsync();
                    if (data == null && data.Count == 0)
                    {
                        return queryResult;
                    }

                    foreach (var item in data)
                    {
                        SignupRecord r = new SignupRecord()
                        {
                            FirstName = item.FirstName,
                            LastName = item.LastName,
                            Email = item.Email,
                            Comments = item.Comments,
                            FunEventName = item.Name,
                            EventStartDate = item.StartDateTime,
                            FunEventId = item.FunEventId
                        };

                        queryResult.Records.Add(r);
                    }
                }

            }
            catch (Exception ex)
            {

            }

            return queryResult;
        }

        public async Task<List<FunnyEvent>> GetFutureEvents()
        {
            List<FunnyEvent> events = new List<FunnyEvent>();
            using (HomeworkEntities context = new HomeworkEntities())
            {
                var list = await context.FunEvents.ToListAsync();
                if(list == null || list.Count == 0)
                {
                    return events;
                }

                foreach(var l in list)
                {
                    events.Add(new FunnyEvent() {  FunEventId = l.FunEventId, EventStartDate = l.StartDateTime, FunEventName = l.Name });
                }
            }

            return events;
         }

        private class Row
        {
            public int FunEventId { get; set; }
            public string Email { get; set; }
        }

        public void TestingHelper()
        {
            using (HomeworkEntities context = new HomeworkEntities())
            {
                context.Database.ExecuteSqlCommand("Delete from Signups");
            }
        }
    }
}
