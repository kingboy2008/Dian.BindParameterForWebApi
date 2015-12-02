using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Dian.BindParameterForWebApi.Attribute;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class DefaultController : ApiController
    {
        [HttpPost, ComplexSingleParameter]
        [Route("Url/Complex/{Id}")]
        public IHttpActionResult UrlComplex(UserAuthVm user)
        {
            return Ok(user);
        }

        [HttpPost, ComplexSingleParameter]
        [Route("Url/Complex/List/{Id}")]
        public IHttpActionResult UrlComplex(UserAuthListVm user)
        {
            return Ok(user);
        }

        [HttpDelete, ComplexSingleParameter]
        [Route("Url/Complex/Delete/{Id}")]
        public IHttpActionResult UrlComplexDelete(UserAuthVm user)
        {
            return Ok(user);
        }

        [HttpPost, ComplexSingleParameter]
        [Route("Url/ComplexPar/{Id}")]
        public IHttpActionResult UrlComplex(int id)
        {
            return Ok(id);
        }

        [HttpPost, ComplexMultiParameters]
        [Route("Url/MultiComplex/{Id}")]
        public IHttpActionResult UrlMultiComplex(UserAuthVm user, UserAuthVm user1)
        {
            return Ok(new List<UserAuthVm>(2) { user, user1 });
        }

        [HttpPost, ComplexMultiParameters]
        [Route("Url/MultiComplexPar/{Id}")]
        public IHttpActionResult UrlMultiComplex(int id, UserAuthVm user1)
        {
            return Ok(new List<object>(2) { id, user1 });
        }
    }
}
