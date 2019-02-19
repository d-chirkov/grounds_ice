namespace GroundsIce.WebApi.Controllers.BorrowOrder
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Http;
    using GroundsIce.Model.Abstractions.Repositories;
    using GroundsIce.Model.Abstractions.Validators;
    using GroundsIce.Model.Entities;
    using GroundsIce.WebApi.Attributes;
    using GroundsIce.WebApi.DTO.Common;

    [AuthorizeApi]
    [RoutePrefix("api/borrow_order")]
    public class BorrowOrderController : ApiController
    {
        private readonly IBorrowOrderRepository borrowOrderRepository;
        private readonly IBorrowOrderValidator borrowOrderValidator;

        public BorrowOrderController(IBorrowOrderRepository borrowOrderRepository, IBorrowOrderValidator borrowOrderValidator)
        {
            this.borrowOrderRepository = borrowOrderRepository ?? throw new ArgumentNullException("borrowOrderRepository");
            this.borrowOrderValidator = borrowOrderValidator ?? throw new ArgumentNullException("borrowOrderValidator");
        }

        public enum ValueType
        {
            Success = 1000,
            BadData = 2000,
            BorrowOrderDoesNotExist = 3000
        }

        [Route("create")]
        [HttpPost]
        public Task<Value> CreateBorrowOrder(BorrowOrder borrowOrder)
        {
            throw new NotImplementedException();
        }

        [Route("delete")]
        [HttpPost]
        public Task<Value> DeleteBorrowOrder(long borrowOrderId)
        {
            throw new NotImplementedException();
        }

        [Route("get_all_by_user_id")]
        [HttpPost]
        public Task<Value<IList<BorrowOrder>>> GetBorrowOrders(long userId)
        {
            throw new NotImplementedException();
        }

        [Route("get_by_user_id")]
        [HttpPost]
        public Task<Value<BorrowOrder>> GetDetailedBorrowOrder(long borrowOrderId)
        {
            throw new NotImplementedException();
        }
    }
}
