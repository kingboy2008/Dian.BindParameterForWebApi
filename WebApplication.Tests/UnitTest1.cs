using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using Dian.NTest.WebApi;
using NUnit.Framework;
using WebApplication.Models;

namespace WebApplication.Tests
{
    [TestFixture]
    public class UnitTest1
    {
        private Dian.NTest.WebApi.TestTool _et;

        [SetUp]
        public void Setup()
        {
            _et = new TestTool();
            _et.InvokeEvent += Target;
        }

        private void Target(HttpConfiguration config)
        {
            WebApiConfig.Register(config);
        }

        [TearDown]
        public void TearDown()
        {
            _et = null;
        }

        [Test]
        public void UrlComplexListTest()
        {
            var acc = new List<ulong> {222, 11};
            var response = _et.InvokePostRequest("Url/Complex/List/1004", new UserAuthListVm {Account = acc});
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public void UrlComplexTest()
        {
            var acc = new List<ulong> {222, 11};
            var response = _et.InvokePostRequest("Url/Complex/1004", new UserAuthVm {Account = "ddd"});
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
    }
}