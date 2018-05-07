using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Caching.Distributed;
using chktr.Controllers;
using Moq;
using chktr.Model;
using System;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using chktr;

namespace ApiTests
{
    [TestClass]
    public class CartControllerTests
    {
        private IDistributedCache _cache;

        [TestInitialize]
        public void Initialize()
        {
            _cache = new Mock<IDistributedCache>().Object;
        }

        [TestCleanup]
        public void Cleanup()
        {
            _cache = null;
        }

        [TestMethod]
        public void WhenGetCartThatExists_ShouldReturnValidInstance()
        {
            var cart = GetDummyCart();

            var mockedService = new Mock<CartService>(_cache);
            mockedService
                .Setup(c => c.GetCart(It.IsAny<Guid>()))
                .ReturnsAsync(cart);

            var result = GetController(mockedService.Object).Get(Guid.NewGuid()).GetAwaiter().GetResult();
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), "Should get the cart.");
        }

        [TestMethod]
        public void WhenGetCartThatDoNotExists_ShouldReturnNull()
        {
            var mockedService = new Mock<CartService>(_cache);
            mockedService
                .Setup(c => c.GetCart(It.IsAny<Guid>()))
                .ReturnsAsync((Cart)null);

            var controller = GetController(mockedService.Object);
            var cart = controller.Get(Guid.NewGuid()).GetAwaiter().GetResult();
            Assert.IsInstanceOfType(cart, typeof(NotFoundResult), "Should not get the cart.");
        }

        [TestMethod]
        public void WhenGetCartButServerThrows_ShouldReturn500()
        {
            var mockedService = new Mock<CartService>(_cache);
            mockedService
                .Setup(c => c.GetCart(It.IsAny<Guid>()))
                .ThrowsAsync(new Exception())
                .Verifiable();

            var controller = GetController(mockedService.Object);
            var result = controller.Get(Guid.NewGuid()).GetAwaiter().GetResult();

            mockedService.VerifyAll();

            Assert.IsInstanceOfType(result, typeof(ObjectResult), "Should not get the cart.");
            Assert.AreEqual(500, ((ObjectResult)result).StatusCode);
        }

        private Cart GetDummyCart()
        {
            return new Cart
            {
                Firstname = "1",
                Lastname = "2",
                Items = new[]
                    {
                        new CartItem{Description= "item", Quantity=1, UnitPrice =2}
                    }
            };
        }

        private CartController GetController(CartService service)
        {
            return new CartController(service);
        }
    }
}
