using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using server.Wcf.App;
using server.Data;

namespace server.Wcf.Test
{
    [TestClass]
    public class UnitTest_Wcf
    {
        [TestMethod]
        public async Task TestMethod_Can_Signup_A_Person_Successfully()
        {
            ISignUpService wcf = new SignUpService();

            string random = Guid.NewGuid().ToString();
            SignupInput signupInput = new SignupInput { FirstName = "Wcf", LastName = "Test", Email = random.Substring(1, 10) + "@wcf.com", FunEventId = 100, Comments = "Created by Wcf" };

            var signupResult = await wcf.Add(signupInput);
            Assert.IsTrue(signupResult.Status == OperationStatus.Added);
            Assert.IsTrue(signupResult.Errors.Count == 0);
            Assert.IsTrue(signupResult.ResourceId == string.Format("{0}###{1}", signupInput.Email, signupInput.FunEventId));
        }
    }
}
