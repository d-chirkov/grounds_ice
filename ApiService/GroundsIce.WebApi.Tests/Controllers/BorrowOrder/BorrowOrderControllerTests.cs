namespace GroundsIce.WebApi.Controllers.BorrowOrder.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using GroundsIce.Model.Abstractions.Repositories;
    using GroundsIce.Model.Abstractions.Validators;
    using GroundsIce.Model.Entities;
    using GroundsIce.WebApi.DTO.Common;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class BorrowOrderControllerTests
    {
        private Mock<IBorrowOrderRepository> mockBorrowOrderRepository;
        private Mock<IBorrowOrderValidator> mockBorrowOrderValidator;
        private long existUserId;
        private long notExistUserId;
        private long userIdThatHasNotBorrowOrders;
        private long existBorrowOrderId;
        private long notExistBorrowOrderId;
        private BorrowOrder validBorrowOrder;
        private BorrowOrder invalidBorrowOrder;
        private List<BorrowOrder> existBorrowOrders;
        private BorrowOrderController subject;

        [SetUp]
        public void SetUp()
        {
            this.existUserId = 0;
            this.notExistUserId = 1;
            this.userIdThatHasNotBorrowOrders = 2;
            this.existBorrowOrderId = 0;
            this.notExistBorrowOrderId = 1;
            this.existBorrowOrders = new List<BorrowOrder>()
            {
                new BorrowOrder() { Id = 0 },
                new BorrowOrder() { Id = 1 },
                new BorrowOrder() { Id = 2 }
            };
            this.validBorrowOrder = new BorrowOrder() { Id = 0 };
            this.invalidBorrowOrder = new BorrowOrder() { Id = 1 };
            this.mockBorrowOrderRepository = new Mock<IBorrowOrderRepository>();
            this.mockBorrowOrderRepository.Setup(c => c.CreateBorrowOrder(this.existUserId, It.IsAny<BorrowOrder>()))
                .ReturnsAsync(true);
            this.mockBorrowOrderRepository.Setup(c => c.CreateBorrowOrder(this.notExistUserId, It.IsAny<BorrowOrder>()))
                .ReturnsAsync(false);
            this.mockBorrowOrderRepository.Setup(c => c.DeleteBorrowOrder(this.existUserId, this.existBorrowOrderId))
                .ReturnsAsync(true);
            this.mockBorrowOrderRepository.Setup(c => c.DeleteBorrowOrder(this.existUserId, this.notExistBorrowOrderId))
                .ReturnsAsync(false);
            this.mockBorrowOrderRepository.Setup(c => c.DeleteBorrowOrder(this.notExistUserId, this.existBorrowOrderId))
                .ReturnsAsync(false);
            this.mockBorrowOrderRepository.Setup(c => c.DeleteBorrowOrder(this.notExistUserId, this.notExistBorrowOrderId))
                .ReturnsAsync(false);
            this.mockBorrowOrderRepository.Setup(c => c.GetBorrowOrders(this.notExistUserId))
                .ReturnsAsync((IList<BorrowOrder>)null);
            this.mockBorrowOrderRepository.Setup(c => c.GetBorrowOrders(this.existUserId))
                .ReturnsAsync(this.existBorrowOrders);
            this.mockBorrowOrderRepository.Setup(c => c.GetBorrowOrders(this.userIdThatHasNotBorrowOrders))
                .ReturnsAsync(new List<BorrowOrder>());
            this.mockBorrowOrderValidator = new Mock<IBorrowOrderValidator>();
            this.mockBorrowOrderValidator.Setup(c => c.ValidateAsync(this.validBorrowOrder)).ReturnsAsync(true);
            this.mockBorrowOrderValidator.Setup(c => c.ValidateAsync(this.invalidBorrowOrder)).ReturnsAsync(false);
            this.mockBorrowOrderValidator.Setup(c => c.ValidateAsync(null)).ThrowsAsync(new ArgumentNullException());
            this.subject = new BorrowOrderController(this.mockBorrowOrderRepository.Object, this.mockBorrowOrderValidator.Object);
        }

        public void SetUserIdToRequest(long userId)
        {
            this.subject.Request = new HttpRequestMessage();
            this.subject.Request.Properties["USER_ID"] = userId;
        }

        [Test]
        public void Ctor_ThrowArgumentNullException_When_PassingNullArgs()
        {
            Assert.Throws<ArgumentNullException>(() => new BorrowOrderController(null, null));
            Assert.Throws<ArgumentNullException>(() => new BorrowOrderController(
                null,
                this.mockBorrowOrderValidator.Object));
            Assert.Throws<ArgumentNullException>(() => new BorrowOrderController(
                this.mockBorrowOrderRepository.Object,
                null));
        }

        [Test]
        public void Ctor_DoesNotThrow_When_PassingNotNullArgs()
        {
            Assert.DoesNotThrow(() => new BorrowOrderController(
                this.mockBorrowOrderRepository.Object,
                this.mockBorrowOrderValidator.Object));
        }

        [Test]
        public void CreateBorrowOrder_ThrowArgumentNullException_When_PassingNullArgs()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => this.subject.CreateBorrowOrder(null));
        }

        [Test]
        public async Task CreateBorrowOrder_ReturnSuccessAndSavePassedOrder_When_PassingValidBorrowOrder()
        {
            this.SetUserIdToRequest(this.existUserId);
            Value result = await this.subject.CreateBorrowOrder(this.validBorrowOrder);
            this.mockBorrowOrderRepository.Verify(c => c.CreateBorrowOrder(this.existUserId, this.validBorrowOrder));
            Assert.AreEqual(result.Type, (int)BorrowOrderController.ValueType.Success);
        }

        [Test]
        public async Task CreateBorrowOrder_ReturnBadDataAndDoesNotSavePassedOrder_When_PassingInvalidBorrowOrder()
        {
            this.SetUserIdToRequest(this.existUserId);
            Value result = await this.subject.CreateBorrowOrder(this.invalidBorrowOrder);
            this.mockBorrowOrderRepository.VerifyNoOtherCalls();
            Assert.AreEqual(result.Type, (int)BorrowOrderController.ValueType.BadData);
        }

        [Test]
        public async Task CreateBorrowOrder_ValidateOrder_When_PassingValidBorrowOrder()
        {
            this.SetUserIdToRequest(this.existUserId);
            await this.subject.CreateBorrowOrder(this.validBorrowOrder);
            this.mockBorrowOrderValidator.Verify(c => c.ValidateAsync(this.validBorrowOrder));
        }

        [Test]
        public async Task CreateBorrowOrder_ValidateOrder_When_PassingInvalidBorrowOrder()
        {
            this.SetUserIdToRequest(this.existUserId);
            await this.subject.CreateBorrowOrder(this.invalidBorrowOrder);
            this.mockBorrowOrderValidator.Verify(c => c.ValidateAsync(this.invalidBorrowOrder));
        }

        [Test]
        public void DeleteBorrowOrder_ThrowArgumentOutOfRangeException_When_PassingNegativeBorrowOrderId()
        {
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => this.subject.DeleteBorrowOrder(-1));
        }

        public async Task DeleteBorrowOrder_ReturnBadDataAndDeleteOrder_When_Passing(
            long userId, long borrowOrderId, BorrowOrderController.ValueType expected)
        {
            this.SetUserIdToRequest(userId);
            Value result = await this.subject.DeleteBorrowOrder(borrowOrderId);
            this.mockBorrowOrderRepository.Verify(c => c.DeleteBorrowOrder(userId, borrowOrderId));
            Assert.AreEqual(result.Type, (int)expected);
        }

        [Test]
        public async Task DeleteBorrowOrder_ReturnSuccessAndDeleteOrder_When_PassingExistUserIdAndExistBorrowOrderId()
        {
            await this.DeleteBorrowOrder_ReturnBadDataAndDeleteOrder_When_Passing(
                this.existUserId, this.existBorrowOrderId, BorrowOrderController.ValueType.Success);
        }

        [Test]
        public async Task DeleteBorrowOrder_ReturnBadDataAndDeleteOrder_When_PassingExistUserIdAndNotExistBorrowOrderId()
        {
            await this.DeleteBorrowOrder_ReturnBadDataAndDeleteOrder_When_Passing(
                this.existUserId, this.notExistBorrowOrderId, BorrowOrderController.ValueType.BadData);
        }

        [Test]
        public async Task DeleteBorrowOrder_ReturnBadDataAndDeleteOrder_When_PassingNotExistUserIdAndExistBorrowId()
        {
            await this.DeleteBorrowOrder_ReturnBadDataAndDeleteOrder_When_Passing(
                this.notExistUserId, this.existBorrowOrderId, BorrowOrderController.ValueType.BadData);
        }

        [Test]
        public async Task DeleteBorrowOrder_ReturnBadDataAndDeleteOrder_When_PassingNotExistUserIdAndNotExistBorrowId()
        {
            await this.DeleteBorrowOrder_ReturnBadDataAndDeleteOrder_When_Passing(
                this.notExistUserId, this.notExistBorrowOrderId, BorrowOrderController.ValueType.BadData);
        }

        [Test]
        public void GetBorrowOrders_ThrowArgumentOutOfRangeException_When_PassingNegativeUserId()
        {
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => this.subject.GetBorrowOrders(-1));
        }

        [Test]
        public async Task GetBorrowOrders_ReturnBadDataWithNullPayload_When_PassingNotExistUserId()
        {
            this.SetUserIdToRequest(this.notExistUserId);
            Value<IList<BorrowOrder>> result = await this.subject.GetBorrowOrders(this.notExistUserId);
            this.mockBorrowOrderRepository.Verify(c => c.GetBorrowOrders(this.notExistUserId));
            Assert.AreEqual(result.Type, (int)BorrowOrderController.ValueType.BadData);
            Assert.AreEqual(result.Payload, null);
        }

        [Test]
        public async Task GetBorrowOrders_ReturnSuccessWithBorrowOrdersInPayload_When_PassingExistUserId()
        {
            this.SetUserIdToRequest(this.existUserId);
            Value<IList<BorrowOrder>> result = await this.subject.GetBorrowOrders(this.existUserId);
            this.mockBorrowOrderRepository.Verify(c => c.GetBorrowOrders(this.existUserId));
            Assert.AreEqual(result.Type, (int)BorrowOrderController.ValueType.Success);
            Assert.AreEqual(result.Payload, this.existBorrowOrders);
        }

        [Test]
        public async Task GetBorrowOrders_ReturnSuccessWithEmptyButNotNullPayload_When_PassingUserIdThatHasNotBorrowOrders()
        {
            this.SetUserIdToRequest(this.userIdThatHasNotBorrowOrders);
            Value<IList<BorrowOrder>> result = await this.subject.GetBorrowOrders(this.userIdThatHasNotBorrowOrders);
            this.mockBorrowOrderRepository.Verify(c => c.GetBorrowOrders(this.userIdThatHasNotBorrowOrders));
            Assert.AreEqual(result.Type, (int)BorrowOrderController.ValueType.Success);
            Assert.AreEqual(result.Payload.Count, 0);
        }
    }
}
