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
        private readonly IBorrowOrderValidator_OLD borrowOrderValidator;

        public BorrowOrderController(IBorrowOrderRepository borrowOrderRepository, IBorrowOrderValidator_OLD borrowOrderValidator)
        {
            this.borrowOrderRepository = borrowOrderRepository ?? throw new ArgumentNullException("borrowOrderRepository");
            this.borrowOrderValidator = borrowOrderValidator ?? throw new ArgumentNullException("borrowOrderValidator");
        }

        public enum ValueType
        {
            Success = 1000,
            BadData = 2000
        }

        [Route("create")]
        [HttpPost]
        public async Task<Value> CreateBorrowOrder(BorrowOrder borrowOrder)
        {
            if (borrowOrder == null)
            {
                throw new ArgumentNullException("borrowOrder");
            }

            long userId = this.GetUserIdFromRequest();
            return
                (await this.borrowOrderValidator.ValidateAsync(borrowOrder) &&
                await this.borrowOrderRepository.CreateBorrowOrder(userId, borrowOrder)) ?
                new Value((int)ValueType.Success) :
                new Value((int)ValueType.BadData);
        }

        [Route("delete")]
        [HttpPost]
        public async Task<Value> DeleteBorrowOrder(long borrowOrderId)
        {
            if (borrowOrderId < 0)
            {
                throw new ArgumentOutOfRangeException("borrowOrderId");
            }

            long userId = this.GetUserIdFromRequest();
            return await this.borrowOrderRepository.DeleteBorrowOrder(userId, borrowOrderId) ?
                new Value((int)ValueType.Success) :
                new Value((int)ValueType.BadData);
        }

        [Route("get_all_for_user")]
        [HttpPost]
        public async Task<Value<IList<BorrowOrder>>> GetBorrowOrders(long userId)
        {
            if (userId < 0)
            {
                throw new ArgumentOutOfRangeException("userId");
            }

            IList<BorrowOrder> borrowOrders = await this.borrowOrderRepository.GetBorrowOrders(userId);
            ValueType resultType = borrowOrders != null ? ValueType.Success : ValueType.BadData;
            return new Value<IList<BorrowOrder>>((int)resultType) { Payload = borrowOrders };
        }

        [Route("get_detailed")]
        [HttpPost]
        public Task<Value<BorrowOrder>> GetDetailedBorrowOrder(long borrowOrderId)
        {
            throw new NotImplementedException();
        }

        private long GetUserIdFromRequest()
        {
            return this.Request != null && this.Request.Properties.ContainsKey("USER_ID") ?
                (long)this.Request?.Properties["USER_ID"] :
                throw new ArgumentNullException("USER_ID");
        }
    }
}
