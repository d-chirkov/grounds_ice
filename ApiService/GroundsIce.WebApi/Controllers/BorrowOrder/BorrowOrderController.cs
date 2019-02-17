namespace GroundsIce.WebApi.Controllers.BorrowOrder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using GroundsIce.WebApi.Attributes;

    [AuthorizeApi]
    [RoutePrefix("api/borrow-order")]
    public class BorrowOrderController : ApiController
    {
    }
}
