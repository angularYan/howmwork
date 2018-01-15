using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using server.WebApi.Controllers;
using server.WebApi.WcfServiceReference;
using System.Web.Http.Results;
using System.Web.Http.ModelBinding;
using System.Collections.Generic;
using System.Web.Http;
using static server.WebApi.Models.Query;

namespace server.WebApi.Tests.Controllers
{
    [TestClass]
    public class SignupControllerTest
    {
        #region Post
        [TestMethod]
        public async Task TestMethod_Post_CreatedAtRoute()
        {
            var controller = new SignupController();
            Random r = new Random();

            SignupInput signupInput = new SignupInput()
            {
                Comments = "",
                Email = "Yan@web.api.test" + r.Next(1234, 123456789),
                FirstName = "Yan",
                LastName = "Wang",
                FunEventId = 101
            };
            string resourceId = signupInput.Email + "###" + signupInput.FunEventId;

            var actionResult = await controller.Post(signupInput);
            var result = actionResult as CreatedAtRouteNegotiatedContentResult<SignupResult>;
            Assert.IsNotNull(result);
            Assert.AreEqual("DefaultApi", result.RouteName);
            Assert.AreEqual(result.Content.ResourceId, resourceId);
            Assert.IsTrue(result.Content.Status == OperationStatus.Added);
        }

        [TestMethod]
        public async Task TestMethod_Post_Created_Duplicated_NegotiatedContentResult_HttpStatusCodeConflict()
        {
            // Created one
            var controller = new SignupController();
            Random r = new Random();

            SignupInput signupInput = new SignupInput()
            {
                Comments = "",
                Email = "Yan@web.api.test" + r.Next(1234, 123456789),
                FirstName = "Yan",
                LastName = "Wang",
                FunEventId = 101
            };
            string resourceId = signupInput.Email + "###" + signupInput.FunEventId;

            var actionResult = await controller.Post(signupInput);
            var result = actionResult as CreatedAtRouteNegotiatedContentResult<SignupResult>;
            Assert.IsNotNull(result);
            Assert.AreEqual("DefaultApi", result.RouteName);
            Assert.AreEqual(result.Content.ResourceId, resourceId);
            Assert.IsTrue(result.Content.Status == OperationStatus.Added);

            // Trying to create again
            var actionResultAgain = await controller.Post(signupInput);
            var createdResultAgain = actionResultAgain as NegotiatedContentResult<SignupResult>;
            Assert.IsNotNull(createdResultAgain); // Check Type
            Assert.IsTrue(createdResultAgain.StatusCode == System.Net.HttpStatusCode.Conflict);
            Assert.IsTrue(createdResultAgain.Content.ResourceId == resourceId);
            Assert.IsTrue(createdResultAgain.Content.Status == OperationStatus.DuplicatedWhenAdding);
        }

        [TestMethod]
        public async Task TestMethod_Post_InvalidModelStateResult_BadRequest()
        {
            var controller = new SignupController();

            SignupInput signupInput = new SignupInput()
            {
                Comments = "",
                Email = "",
                FirstName = "Looooooooooooooong",
                LastName = "",
                FunEventId = 0
            };

            var actionResult = await controller.Post(signupInput);

            var createdResult = actionResult as InvalidModelStateResult;
            Assert.IsNotNull(createdResult);
            Assert.IsNotNull(createdResult.ModelState);
            ModelStateDictionary info = createdResult.ModelState as ModelStateDictionary;
            Assert.IsTrue(info.Count == 4);

            // Todo: check Details
        }

        [TestMethod]
        public async Task TestMethod_Post_InvalidModelStateResult_BadRequest_Not_ExistingEvent()
        {
            var controller = new SignupController();

            SignupInput signupInput = new SignupInput()
            {
                Comments = "",
                Email = "good@test.om",
                FirstName = "ABC",
                LastName = "def",
                FunEventId = 9999
            };

            var actionResult = await controller.Post(signupInput);

            var createdResult = actionResult as InvalidModelStateResult;
            Assert.IsNotNull(createdResult);
            Assert.IsNotNull(createdResult.ModelState);
            ModelStateDictionary info = createdResult.ModelState as ModelStateDictionary;
            Assert.IsTrue(info.Count == 1);
        }


        #endregion

        #region Get
        [TestMethod]
        public async Task TestMethod_Get()
        {
            // Prepare data
            var controller = new SignupController();
            Random r = new Random();
            string searchKey = Guid.NewGuid().ToString().Substring(1, 10);
            SignupInput signupInput = new SignupInput()
            {
                Comments = "",
                Email = "Yan@web.api.test" + r.Next(1234, 123456789),
                FirstName = searchKey,
                LastName = "Wang",
            };

            string emailBackup = signupInput.Email;

            signupInput.FunEventId = 100;
            var task1 = controller.Post(signupInput);
            await Task.WhenAll(task1);

            signupInput.FunEventId = 101;
            var task2 = controller.Post(signupInput);
            await Task.WhenAll(task2);
            signupInput.FunEventId = 102;
            var task3 = controller.Post(signupInput);
            await Task.WhenAll(task3);

            // Noise
            signupInput.FunEventId = 100;
            signupInput.Email = "ThisisNoise@g"+ r.Next(123456789, 223456789);
            signupInput.FirstName = "NotMatched";
            var task4 = controller.Post(signupInput);
            await Task.WhenAll(task4);

            //
            // Query - By First Name
            //
            QueryInputFromClient queryInputFromClient = new QueryInputFromClient();
            queryInputFromClient.FirstName = searchKey;
            var queryResult = await controller.Get(queryInputFromClient);
            var list = queryResult.ToList();

            Assert.IsTrue(list.Count() == 3);
            // Same person
            Assert.IsTrue(list[0].Email == emailBackup);
            Assert.IsTrue(list[1].Email == emailBackup);
            Assert.IsTrue(list[2].Email == emailBackup);

            Assert.IsTrue(list[0].FirstName.Contains(searchKey));
            Assert.IsTrue(list[1].FirstName.Contains(searchKey));
            Assert.IsTrue(list[2].FirstName.Contains(searchKey));

            // Different events
            Assert.IsTrue(list[0].FunEventId != list[1].FunEventId);
            Assert.IsTrue(list[1].FunEventId != list[2].FunEventId);
            Assert.IsTrue(list[2].FunEventId != list[0].FunEventId);

            //
            // Query - By First Name & Eent
            //
            QueryInputFromClient queryInputFromClient2 = new QueryInputFromClient();
            queryInputFromClient2.FirstName = searchKey;
            queryInputFromClient2.FunEvent = 101;
            var queryResult2 = await controller.Get(queryInputFromClient2);
            var list2 = queryResult2.ToList();

            Assert.IsTrue(list2.Count() == 1);
            Assert.IsTrue(list2[0].Email == emailBackup);
            Assert.IsTrue(list2[0].FirstName.Contains(searchKey));
            Assert.IsTrue(list2[0].FunEventId == 101);
        }
        #endregion
    }
}
