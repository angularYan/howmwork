using server.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.Business
{
    public class SignupBusiness : ISignup, IFunEvent
    {
        private readonly ISignup _iSignup;
        private readonly IFunEvent _iFunEvent;
        public SignupBusiness(ISignup iSignup, IFunEvent iFunEvent)
        {
            this._iSignup = iSignup;
            this._iFunEvent = iFunEvent;
        }

        public async Task<SignupResult> Add(SignupInput signupInput)
        {
            SignupResult signupResult = new SignupResult() { Status = OperationStatus.NoError, Errors = new List<Error>(), ResourceId = "" };

            if (signupInput == null)
            {
                signupResult.Status = OperationStatus.ErrorsWhenAdding;
                signupResult.Errors.Add(new Error { ErrorType = ErrorType.EmptyEntry, DetailError = "No Signu up Info Provided.", MembershipName = string.Empty });
                return signupResult;
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(signupInput.Email))
                {
                    // Todo:
                    // Email rule check -  at least have @ and .
                }

                if ( !string.IsNullOrWhiteSpace(signupInput.Email) && signupInput.Email.ToLowerInvariant().Contains("@yahoo.com") )
                {
                    // Not support yahoo
                    signupResult.Status = OperationStatus.ErrorsWhenAdding;
                    signupResult.Errors.Add(new Error { ErrorType = ErrorType.DataInvalid, DetailError = "Email is not supported.", MembershipName = "Email" });
                    return signupResult;
                }
            }

            
            var result = await this._iSignup.Add(signupInput);
            return result;
        }

        public async Task<List<FunnyEvent>> GetFutureEvents()
        {
            return await this._iFunEvent.GetFutureEvents();
        }

        public async Task<QueryResult> Query(QueryInput queryInput)
        {
            var result = await this._iSignup.Query(queryInput);
            return result;
        }
    }
}
