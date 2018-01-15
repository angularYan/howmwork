using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using server.DataAccess;
using server.Data;
using System.Collections.Generic;

namespace server.Business.Test
{
    [TestClass]
    public class UnitTest_Business_Integrated
    {
        #region prepare
        private static string FirstName = "FFF";
        private static string LastName = "LLL";

        private static List<SignupInput> ExpectedList { get; set; }

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            InitializeAsync().Wait();
        }

        private static async Task InitializeAsync()
        {
            ISignup signup1 = new SignMeUp();
            IFunEvent funEvent = new SignMeUp();
            ISignup signup = new SignupBusiness(signup1, funEvent); // <=========

            SignMeUp help = signup1 as SignMeUp;
            help.TestingHelper();

            ExpectedList = new List<SignupInput>();
            List<Task<SignupResult>> tasks = new List<Task<SignupResult>>();

            // Signup - Diff Persons sign up same events
            for (int i = 0; i <= 10; i++)
            {
                // Create one person.
                string random = Guid.NewGuid().ToString();
                SignupInput signupInput = new SignupInput
                {
                    FirstName = random.Substring(1, 3) + UnitTest_Business_Integrated.FirstName + random.Substring(3, 3),
                    LastName = random.Substring(6, 3) + UnitTest_Business_Integrated.LastName + random.Substring(10, 3),
                    Email = random.Substring(1, 10) + "@business.com",
                    Comments = ""
                };

                // Signup - Same person signs up 3 events.
                SignupInput signupInput1 = new SignupInput
                {
                    FirstName = signupInput.FirstName,
                    LastName = signupInput.LastName,
                    Email = signupInput.Email,
                    Comments = signupInput.Comments
                };
                ExpectedList.Add(signupInput1);
                signupInput1.FunEventId = 100;
                var task1 = signup.Add(signupInput1);
                tasks.Add(task1);

                SignupInput signupInput2 = new SignupInput
                {
                    FirstName = signupInput.FirstName,
                    LastName = signupInput.LastName,
                    Email = signupInput.Email,
                    Comments = signupInput.Comments
                };
                ExpectedList.Add(signupInput2);
                signupInput2.FunEventId = 101;
                var task2 = signup.Add(signupInput2);
                tasks.Add(task2);
            }

            await Task.WhenAll(tasks);
        }

        #endregion

        [TestMethod]
        public async Task TestMethod_Can_Signup_A_Person_Successfully()
        {
            ISignup signup = new SignMeUp();
            IFunEvent funEvent = new SignMeUp();
            ISignup business = new SignupBusiness(signup, funEvent);

            string random = Guid.NewGuid().ToString();
            SignupInput signupInput = new SignupInput { FirstName = "Business", LastName = "Test", Email = random.Substring(1, 10) + "@email.com", FunEventId = 100, Comments = "Created by Business" };

            var signupResult = await business.Add(signupInput);
            Assert.IsTrue(signupResult.Status == OperationStatus.Added);
            Assert.IsTrue(signupResult.Errors.Count == 0);
            Assert.IsTrue(signupResult.ResourceId == string.Format("{0}###{1}", signupInput.Email, signupInput.FunEventId));
        }

        [TestMethod]
        public async Task TestMethod_Can_Not_Signup_A_Person_With_Yahoo_Email()
        {
            ISignup signup = new SignMeUp();
            IFunEvent funEvent = new SignMeUp();
            ISignup business = new SignupBusiness(signup, funEvent);

            string random = Guid.NewGuid().ToString();
            SignupInput signupInput = new SignupInput { FirstName = "Business", LastName = "Test", Email = random.Substring(1, 10) + "@YaHoO.cOm", FunEventId = 100, Comments = "Created by Business" };

            var signupResult = await business.Add(signupInput);
            Assert.IsTrue(signupResult.Status == OperationStatus.ErrorsWhenAdding);
            Assert.IsTrue(signupResult.Errors.Count == 1);
            Assert.IsTrue(signupResult.Errors[0].ErrorType == ErrorType.DataInvalid);
            Assert.IsTrue(signupResult.Errors[0].MembershipName == "Email");
            Assert.IsTrue(signupResult.Errors[0].DetailError == "Email is not supported.");
        }

        [TestMethod]
        public async Task TestMethod_Query_By_FirstName_And_Event_101()
        {
            ISignup signup = new SignMeUp();
            IFunEvent funEvent = new SignMeUp();
            ISignup business = new SignupBusiness(signup, funEvent);

            QueryInput queryInput = new QueryInput() { FunEvent = 101, FirstName = ExpectedList[4].FirstName };
            queryInput.CurrentPage = 0;

            var r = await business.Query(queryInput);
            Assert.IsTrue(r.Records.Count == 1);

            // Same person
            Assert.IsTrue(r.Records[0].Email == ExpectedList[4].Email);

            // Diff Events
            Assert.IsTrue(r.Records[0].FunEventId == 101);
        }
    }
}
