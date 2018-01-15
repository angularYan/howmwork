using server.Business;
using server.Data;
using server.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace server.Wcf.App
{
    public class Factory
    {
        public static ISignup CreateSignup()
        {
            SignMeUp smu = new SignMeUp();
            ISignup su = smu;
            IFunEvent fe = smu;
            SignupBusiness sb = new SignupBusiness(su, fe);
            return sb;
        }

        public static IFunEvent CreateFunEvent()
        {
            SignMeUp smu = new SignMeUp();
            ISignup su = smu;
            IFunEvent fe = smu;
            SignupBusiness sb = new SignupBusiness(su, fe);
            return sb;
        }
    }
}