using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using server.WebApi;
using server.WebApi.Controllers;
using System.Threading.Tasks;

namespace server.WebApi.Tests.Controllers
{
    [TestClass]
    public class FunEventControllerTest
    {
        [TestMethod]
        public async Task TestMethod_Get()
        {
            var controller = new FunEventController();
            var result = await controller.Get();
            var list = result.ToList();

            Assert.IsTrue(list.Count() == 3);
            Assert.IsTrue(list[0].FunEventId == 100);
            Assert.IsTrue(list[0].FunEventName == "Activity at High Park");
            Assert.IsTrue(list[1].FunEventId == 101);
            Assert.IsTrue(list[1].FunEventName == "Activity at Queens Park");
            Assert.IsTrue(list[2].FunEventId == 102);
            Assert.IsTrue(list[2].FunEventName == "Activity at Tech Room");
        }
    }
}
