using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplication.Models;

namespace WebApplication.Tests
{
    [TestClass]
    public class UnitTest1 : BaseWebApiUnitTest
    {

        protected override void Target(HttpConfiguration config)
        {
            WebApiConfig.Register(config);
        }

        [TestMethod]
        public void UrlComplexListTest()
        {
            var acc = new List<ulong> {222, 11};
            var response = InvokePostRequest("Url/Complex/List/1004", new UserAuthListVm() { Account = acc });
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public void UrlComplexTest()
        {
            var acc = new List<ulong> { 222, 11 };
            var response = InvokePostRequest("Url/Complex/1004", new UserAuthVm() { Account = "ddd" });
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
