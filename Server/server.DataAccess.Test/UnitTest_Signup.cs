using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using server.Data;

namespace server.DataAccess.Test
{
    /// <summary>
    /// Summary description for UnitTest_Signup
    /// </summary>
    [TestClass]
    public class UnitTest_Signup
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

        [TestInitialize()]
        public void TestInitialize()
        {
            // Waits for the System.Threading.Tasks.Task to complete execution.
        }

        private static async Task InitializeAsync()
        {
            ISignup signup = new SignMeUp();
            SignMeUp help = signup as SignMeUp;
            help.TestingHelper();

            ExpectedList = new List<SignupInput>();
            List<Task<SignupResult>> tasks = new List<Task<SignupResult>>();

            // Signup - Diff Persons sign up same events
            for (int i = 0; i <= 50; i++)
            {
                // Create one person.
                string random = Guid.NewGuid().ToString();
                SignupInput signupInput = new SignupInput
                {
                    FirstName = random.Substring(1, 3) + UnitTest_Signup.FirstName + random.Substring(3, 3),
                    LastName = random.Substring(6, 3) + UnitTest_Signup.LastName + random.Substring(10, 3),
                    Email = random.Substring(1, 10) + "@email.com",
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

                SignupInput signupInput3 = new SignupInput
                {
                    FirstName = signupInput.FirstName,
                    LastName = signupInput.LastName,
                    Email = signupInput.Email,
                    Comments = signupInput.Comments
                };
                ExpectedList.Add(signupInput3);
                signupInput3.FunEventId = 102;
                var task3 = signup.Add(signupInput3);
                tasks.Add(task3);
            }

            await Task.WhenAll(tasks);
        }

        #endregion

        #region Add
        [TestMethod]
        public async Task TestMethod_Can_Signup_A_Person_Successfully()
        {
            ISignup signup = new SignMeUp();
            string random = Guid.NewGuid().ToString();
            SignupInput signupInput = new SignupInput { FirstName = "Yan", LastName = "Wang", Email = random.Substring(1, 10) + "@email.com", FunEventId = 100, Comments = "I will bring my two kids." };
            var signupResult = await signup.Add(signupInput);
            Assert.IsTrue(signupResult.Status == OperationStatus.Added);
            Assert.IsTrue(signupResult.Errors.Count == 0);
            Assert.IsTrue(signupResult.ResourceId == string.Format("{0}###{1}", signupInput.Email, signupInput.FunEventId));
        }

        [TestMethod]
        public async Task TestMethod_Can_Not_SignUp_Same_Email_Twice_Under_Same_Event()
        {
            ISignup signup = new SignMeUp();
            string random = Guid.NewGuid().ToString();
            SignupInput signupInput = new SignupInput { FirstName = "Yan", LastName = "Wang", Email = random.Substring(1, 10) + "@email.com", FunEventId = 100 };
            var signupResult = await signup.Add(signupInput);
            Assert.IsTrue(signupResult.Status == OperationStatus.Added);
            Assert.IsTrue(signupResult.Errors.Count == 0);
            Assert.IsTrue(signupResult.ResourceId == string.Format("{0}###{1}", signupInput.Email, signupInput.FunEventId));

            // Try again
            var signupResultAgain = await signup.Add(signupInput);
            Assert.IsTrue(signupResultAgain.Status == OperationStatus.DuplicatedWhenAdding);
            Assert.IsTrue(signupResultAgain.Errors.Count == 0);
            Assert.IsTrue(signupResultAgain.ResourceId == string.Format("{0}###{1}", signupInput.Email, signupInput.FunEventId));
        }

        [TestMethod]
        public async Task TestMethod_Can_SignUp_Same_Email_Twice_Under_Different_Events()
        {
            ISignup signup = new SignMeUp();
            string random = Guid.NewGuid().ToString();
            SignupInput signupInput = new SignupInput { FirstName = "Yan", LastName = "Wang", Email = random.Substring(1, 10) + "@email.com", FunEventId = 100 };
            var signupResult = await signup.Add(signupInput);
            Assert.IsTrue(signupResult.Status == OperationStatus.Added);
            Assert.IsTrue(signupResult.Errors.Count == 0);
            Assert.IsTrue(signupResult.ResourceId == string.Format("{0}###{1}", signupInput.Email, signupInput.FunEventId));

            // Try again with other event
            signupInput.FunEventId = 101;
            var signupResultAgain = await signup.Add(signupInput);
            Assert.IsTrue(signupResultAgain.Status == OperationStatus.Added);
            Assert.IsTrue(signupResultAgain.Errors.Count == 0);
            Assert.IsTrue(signupResultAgain.ResourceId == string.Format("{0}###{1}", signupInput.Email, signupInput.FunEventId));
        }

        [TestMethod]
        public async Task TestMethod_Can_Not_Signup_Person_With_Not_Existing_Event()
        {
            int notExistingEvent = int.MaxValue - 1;
            ISignup signup = new SignMeUp();
            string random = Guid.NewGuid().ToString();
            SignupInput signupInput = new SignupInput { FirstName = "Yan", LastName = "Wang", Email = random.Substring(1, 10) + "@email.com", FunEventId = notExistingEvent };
            var signupResult = await signup.Add(signupInput);
            Assert.IsTrue(signupResult.Status == OperationStatus.ErrorsWhenAdding);
            Assert.IsTrue(signupResult.Errors.Count == 1);
            Assert.IsTrue(signupResult.Errors[0].ErrorType == ErrorType.DataInvalid);
            Assert.IsTrue(signupResult.Errors[0].MembershipName == "FunEventId");
            Assert.IsTrue(signupResult.Errors[0].DetailError == "Event is not Existing");
        }

        [TestMethod]
        public async Task TestMethod_Can_Not_Signup_A_Person_With_Invalid_Data_Empty()
        {
            ISignup signup = new SignMeUp();
            string random = Guid.NewGuid().ToString();
            SignupInput signupInput = new SignupInput { FirstName = "", LastName = "  ", Email = null, FunEventId = 0, Comments = "I will bring my two kids." };
            var signupResult = await signup.Add(signupInput);
            Assert.IsTrue(signupResult.Status == OperationStatus.ErrorsWhenAdding);
            Assert.IsTrue(signupResult.Errors.Count == 4);

            Assert.IsTrue(signupResult.Errors[0].ErrorType == ErrorType.DataInvalid);
            Assert.IsTrue(signupResult.Errors[0].DetailError == "First Name is Required");
            Assert.IsTrue(signupResult.Errors[0].MembershipName == "FirstName");

            Assert.IsTrue(signupResult.Errors[1].ErrorType == ErrorType.DataInvalid);
            Assert.IsTrue(signupResult.Errors[1].DetailError == "Last Name is Required");
            Assert.IsTrue(signupResult.Errors[1].MembershipName == "LastName");

            Assert.IsTrue(signupResult.Errors[2].ErrorType == ErrorType.DataInvalid);
            Assert.IsTrue(signupResult.Errors[2].DetailError == "Email is Required");
            Assert.IsTrue(signupResult.Errors[2].MembershipName == "Email");

            Assert.IsTrue(signupResult.Errors[3].ErrorType == ErrorType.DataInvalid);
            Assert.IsTrue(signupResult.Errors[3].DetailError == "Event is Required");
            Assert.IsTrue(signupResult.Errors[3].MembershipName == "FunEventId");
        }

        [TestMethod]
        public async Task TestMethod_Can_Not_Signup_A_Person_With_Too_Long()
        {
            ISignup signup = new SignMeUp();
            string random = Guid.NewGuid().ToString();
            SignupInput signupInput = new SignupInput
            {
                FirstName = "FirstNameFirstName",
                LastName = "LastNameLastNameLastName  ",
                Email = "EmailEmailEmailEmailEmailEmailEmailEmailEmailEmailEmail",
                FunEventId = 1,
                Comments =
                "I will bring my two kids.I will bring my two kids.I will bring my two kids.I will bring my two kids.I will bring my two kids.I will bring my two kids.I will bring my two kids.I will bring my two kids.I will bring my two kids.I will bring my two kids.I will bring my two kids."
            };
            var signupResult = await signup.Add(signupInput);
            Assert.IsTrue(signupResult.Status == OperationStatus.ErrorsWhenAdding);
            Assert.IsTrue(signupResult.Errors.Count == 4);

            Assert.IsTrue(signupResult.Errors[0].ErrorType == ErrorType.DataInvalid);
            Assert.IsTrue(signupResult.Errors[0].DetailError == "First Name is more than 10 letters");
            Assert.IsTrue(signupResult.Errors[0].MembershipName == "FirstName");

            Assert.IsTrue(signupResult.Errors[1].ErrorType == ErrorType.DataInvalid);
            Assert.IsTrue(signupResult.Errors[1].DetailError == "Last Name is more than 10 letters");
            Assert.IsTrue(signupResult.Errors[1].MembershipName == "LastName");

            Assert.IsTrue(signupResult.Errors[2].ErrorType == ErrorType.DataInvalid);
            Assert.IsTrue(signupResult.Errors[2].DetailError == "Email is more than 30 letters");
            Assert.IsTrue(signupResult.Errors[2].MembershipName == "Email");

            Assert.IsTrue(signupResult.Errors[3].ErrorType == ErrorType.DataInvalid);
            Assert.IsTrue(signupResult.Errors[3].DetailError == "Comments is more than 30 letters");
            Assert.IsTrue(signupResult.Errors[3].MembershipName == "Comments");
        }

        [TestMethod]
        public async Task TestMethod_Can_Not_Signup_With_Empty_Input()
        {
            ISignup signup = new SignMeUp();
            var signupResult = await signup.Add(null);
            Assert.IsTrue(signupResult.Status == OperationStatus.ErrorsWhenAdding);
            Assert.IsTrue(signupResult.Errors.Count == 1);
            Assert.IsTrue(signupResult.Errors[0].ErrorType == ErrorType.EmptyEntry);
            Assert.IsTrue(signupResult.Errors[0].DetailError == "No Signu up Info Provided.");
        }

        [TestMethod]
        public async Task TestMethod_Can_Signup_A_Person_Successfully_OnlyOnce_WhenTryingToRepeat()
        {
            ISignup signup = new SignMeUp();
            List<Task<SignupResult>> tasks = new List<Task<SignupResult>>();
            string random = Guid.NewGuid().ToString();
            SignupInput signupInput = new SignupInput { FirstName = "ONLY", LastName = "ONE", Email = random.Substring(1, 10) + "@email.com", FunEventId = 100, Comments = "ONly ONe" };
            for (int i = 0; i < 100; i++)
            {
                var task = signup.Add(signupInput);
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);

            int total = 0;
            int dup = 0;
            foreach(var t in tasks)
            {
                if (t.Result.Status == OperationStatus.Added)
                {
                    total++;
                }
                else if (t.Result.Status == OperationStatus.DuplicatedWhenAdding)
                {
                    dup++;
                }
                else
                {
                }
            }

            Assert.IsTrue(total == 1);
            Assert.IsTrue(dup == 99);
        }
        #endregion

        #region Query
        // [TestMethod]
        public async Task TestMethod_Query_ALL()
        {
            ISignup signup = new SignMeUp();
            List<Task<QueryResult>> tasks = new List<Task<QueryResult>>();

            QueryInput queryInput1 = new QueryInput() { FunEvent = 0 };
            QueryInput queryInput2 = new QueryInput() { FunEvent = 0 };
            QueryInput queryInput3 = new QueryInput() { FunEvent = 0 };
            QueryInput queryInput4 = new QueryInput() { FunEvent = 0 };
            QueryInput queryInput5 = new QueryInput() { FunEvent = 0 };
            QueryInput queryInput6 = new QueryInput() { FunEvent = 0 };
            QueryInput queryInput7 = new QueryInput() { FunEvent = 0 };
            QueryInput queryInput8 = new QueryInput() { FunEvent = 0 };
            QueryInput queryInput9 = new QueryInput() { FunEvent = 0 };

            queryInput1.CurrentPage = 0;
            var task1 = signup.Query(queryInput1);
            tasks.Add(task1);

            queryInput2.CurrentPage = 1;
            var task2 = signup.Query(queryInput2);
            tasks.Add(task2);

            queryInput3.CurrentPage = 2;
            var task3 = signup.Query(queryInput3);
            tasks.Add(task3);

            queryInput4.CurrentPage = 3;
            var task4 = signup.Query(queryInput4);
            tasks.Add(task4);

            queryInput5.CurrentPage = 4;
            var task5 = signup.Query(queryInput5);
            tasks.Add(task5);

            queryInput6.CurrentPage = 5;
            var task6 = signup.Query(queryInput6);
            tasks.Add(task6);

            queryInput7.CurrentPage = 6;
            var task7 = signup.Query(queryInput7);
            tasks.Add(task7);

            queryInput8.CurrentPage = 7;
            var task8 = signup.Query(queryInput8);
            tasks.Add(task8);

            queryInput9.CurrentPage = 8;
            var task9 = signup.Query(queryInput9);
            tasks.Add(task9);

            await Task.WhenAll(tasks);

            Assert.IsTrue(task1.Result.Records.Count == 20);
            Assert.IsTrue(task2.Result.Records.Count == 20);
            Assert.IsTrue(task3.Result.Records.Count == 20);
            Assert.IsTrue(task4.Result.Records.Count == 20);
            Assert.IsTrue(task5.Result.Records.Count == 20);
            Assert.IsTrue(task6.Result.Records.Count == 20);
            Assert.IsTrue(task7.Result.Records.Count == 20);
            Assert.IsTrue(task8.Result.Records.Count < 20);
            Assert.IsTrue(task9.Result.Records.Count == 0);

            TotallyDiff(task1.Result.Records, task3.Result.Records);
            TotallyDiff(task1.Result.Records, task4.Result.Records);
            TotallyDiff(task1.Result.Records, task5.Result.Records);
            TotallyDiff(task1.Result.Records, task6.Result.Records);
            TotallyDiff(task1.Result.Records, task7.Result.Records);
            TotallyDiff(task1.Result.Records, task8.Result.Records);

            TotallyDiff(task2.Result.Records, task4.Result.Records);
            TotallyDiff(task2.Result.Records, task5.Result.Records);
            TotallyDiff(task2.Result.Records, task6.Result.Records);
            TotallyDiff(task2.Result.Records, task7.Result.Records);
            TotallyDiff(task2.Result.Records, task8.Result.Records);

            TotallyDiff(task3.Result.Records, task5.Result.Records);
            TotallyDiff(task3.Result.Records, task6.Result.Records);
            TotallyDiff(task3.Result.Records, task7.Result.Records);
            TotallyDiff(task3.Result.Records, task8.Result.Records);

            TotallyDiff(task4.Result.Records, task6.Result.Records);
            TotallyDiff(task4.Result.Records, task7.Result.Records);
            TotallyDiff(task4.Result.Records, task8.Result.Records);

            TotallyDiff(task5.Result.Records, task7.Result.Records);
            TotallyDiff(task5.Result.Records, task8.Result.Records);

            TotallyDiff(task6.Result.Records, task8.Result.Records);

            TotallyDiff(task7.Result.Records, task1.Result.Records);

            TotallyDiff(task8.Result.Records, task2.Result.Records);

            AllQueryShouldBeInExpectedList(task1.Result.Records, ExpectedList);
            AllQueryShouldBeInExpectedList(task2.Result.Records, ExpectedList);
            AllQueryShouldBeInExpectedList(task3.Result.Records, ExpectedList);
            AllQueryShouldBeInExpectedList(task4.Result.Records, ExpectedList);
            AllQueryShouldBeInExpectedList(task5.Result.Records, ExpectedList);
            AllQueryShouldBeInExpectedList(task6.Result.Records, ExpectedList);
            AllQueryShouldBeInExpectedList(task7.Result.Records, ExpectedList);
            AllQueryShouldBeInExpectedList(task8.Result.Records, ExpectedList);
        }

        [TestMethod]
        public async Task TestMethod_QueryByEvent_Specific_Event_102()
        {
            ISignup signup = new SignMeUp();
            List<Task<QueryResult>> tasks = new List<Task<QueryResult>>();

            QueryInput queryInput1 = new QueryInput() { FunEvent = 102 };
            QueryInput queryInput2 = new QueryInput() { FunEvent = 102 };
            QueryInput queryInput3 = new QueryInput() { FunEvent = 102 };

            queryInput1.CurrentPage = 0;
            var task1 = signup.Query(queryInput1);
            tasks.Add(task1);

            queryInput2.CurrentPage = 1;
            var task2 = signup.Query(queryInput2);
            tasks.Add(task2);

            queryInput3.CurrentPage = 2;
            var task3 = signup.Query(queryInput3);
            tasks.Add(task3);

            await Task.WhenAll(tasks);

            Assert.IsTrue(task1.Result.Records.Count == 20);
            Assert.IsTrue(task2.Result.Records.Count == 20);
            Assert.IsTrue(task3.Result.Records.Count == 11);

            TotallyDiff(task1.Result.Records, task2.Result.Records);
            TotallyDiff(task2.Result.Records, task3.Result.Records);
            TotallyDiff(task3.Result.Records, task1.Result.Records);

            AllQueryShouldBeInExpectedList(task1.Result.Records, ExpectedList);
            AllQueryShouldBeInExpectedList(task2.Result.Records, ExpectedList);
            AllQueryShouldBeInExpectedList(task3.Result.Records, ExpectedList);
        }

        [TestMethod]
        public async Task TestMethod_Query_By_FirstName()
        {
            ISignup signup = new SignMeUp();
            QueryInput queryInput = new QueryInput() { FunEvent = 0, FirstName = ExpectedList[10].FirstName };
            queryInput.CurrentPage = 0;

            var r =  await signup.Query(queryInput);
            Assert.IsTrue(r.Records.Count == 3);

            // Same person
            Assert.IsTrue(r.Records[0].Email == ExpectedList[10].Email);
            Assert.IsTrue(r.Records[1].Email == ExpectedList[10].Email);
            Assert.IsTrue(r.Records[2].Email == ExpectedList[10].Email);

            // Diff Events
            Assert.IsTrue(r.Records[0].FunEventId != r.Records[1].FunEventId);
            Assert.IsTrue(r.Records[1].FunEventId != r.Records[2].FunEventId);
            Assert.IsTrue(r.Records[2].FunEventId != r.Records[0].FunEventId);
        }

        [TestMethod]
        public async Task TestMethod_Query_By_FirstName_And_Event_102()
        {
            ISignup signup = new SignMeUp();
            QueryInput queryInput = new QueryInput() { FunEvent = 102, FirstName = ExpectedList[10].FirstName };
            queryInput.CurrentPage = 0;

            var r = await signup.Query(queryInput);
            Assert.IsTrue(r.Records.Count == 1);

            // Same person
            Assert.IsTrue(r.Records[0].Email == ExpectedList[10].Email);

            // Diff Events
            Assert.IsTrue(r.Records[0].FunEventId == 102);
        }

        [TestMethod]
        public async Task TestMethod_Query_By_LastName()
        {
            ISignup signup = new SignMeUp();
            QueryInput queryInput = new QueryInput() { FunEvent = 0, LastName = ExpectedList[36].LastName };
            queryInput.CurrentPage = 0;

            var r = await signup.Query(queryInput);
            Assert.IsTrue(r.Records.Count == 3);

            // Same person
            Assert.IsTrue(r.Records[0].Email == ExpectedList[36].Email);
            Assert.IsTrue(r.Records[1].Email == ExpectedList[36].Email);
            Assert.IsTrue(r.Records[2].Email == ExpectedList[36].Email);

            // Diff Events
            Assert.IsTrue(r.Records[0].FunEventId != r.Records[1].FunEventId);
            Assert.IsTrue(r.Records[1].FunEventId != r.Records[2].FunEventId);
            Assert.IsTrue(r.Records[2].FunEventId != r.Records[0].FunEventId);
        }

        [TestMethod]
        public async Task TestMethod_Query_By_LastName_NoMatch()
        {
            ISignup signup = new SignMeUp();
            QueryInput queryInput = new QueryInput() { FunEvent = 0, LastName = "NO FOUND" };
            queryInput.CurrentPage = 0;

            var r = await signup.Query(queryInput);
            Assert.IsTrue(r.Records.Count == 0);
        }

        [TestMethod]
        public async Task TestMethod_Query_By_LastName_FirstName()
        {
            ISignup signup = new SignMeUp();
            QueryInput queryInput = new QueryInput() { FunEvent = 0, LastName = ExpectedList[51].LastName, FirstName = LastName = ExpectedList[51].FirstName };
            queryInput.CurrentPage = 0;

            var r = await signup.Query(queryInput);
            Assert.IsTrue(r.Records.Count == 3);

            // Same person
            Assert.IsTrue(r.Records[0].Email == ExpectedList[51].Email);
            Assert.IsTrue(r.Records[1].Email == ExpectedList[51].Email);
            Assert.IsTrue(r.Records[2].Email == ExpectedList[51].Email);

            // Diff Events
            Assert.IsTrue(r.Records[0].FunEventId != r.Records[1].FunEventId);
            Assert.IsTrue(r.Records[1].FunEventId != r.Records[2].FunEventId);
            Assert.IsTrue(r.Records[2].FunEventId != r.Records[0].FunEventId);
        }

        [TestMethod]
        public async Task TestMethod_GetAllEvents()
        {
            IFunEvent signup = new SignMeUp();
            var r =  await signup.GetFutureEvents();

            Assert.IsTrue(r.Count == 3);
            Assert.IsTrue(r[0].FunEventId == 100);
            Assert.IsTrue(r[0].FunEventName == "Activity at High Park");

            Assert.IsTrue(r[1].FunEventId == 101);
            Assert.IsTrue(r[2].FunEventId == 102);
        }

        #endregion

        #region Help
        private void TotallyDiff(List<SignupRecord> left, List<SignupRecord> right)
        {
            if (left == null || right == null)
            {
                Assert.Fail("Invalid Left or Right");
            }

            int matched = 0;
            foreach (var l in left)
            {
                foreach (var r in right)
                {
                    if(l.Email == r.Email)
                    {
                        matched++;
                    }
                }
            }

            foreach (var r in right)
            {
                foreach (var l in left)
                {
                    if (l.Email == r.Email)
                    {
                        matched++;
                    }
                }
            }

            Assert.IsTrue(matched == 0);
        }

        private void AllQueryShouldBeInExpectedList(List<SignupRecord> list, List<SignupInput> expectedList)
        {
            foreach (var r in list)
            {
                bool found = false;
                foreach (var e in expectedList)
                {
                    if (r.Email == e.Email)
                    {
                        found = true;
                    }
                }
                Assert.IsTrue(found);
            }
        }

        #endregion
    }
}
