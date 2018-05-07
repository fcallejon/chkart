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

namespace ApiTests
{
    [TestClass]
    public class CartControllerTests
    {
        private CartController GetController()
        {
            var mockedCache = new Mock<IDistributedCache>();
            return new CartController(mockedCache.Object);
        }
        
        private CartController GetController(IDistributedCache cache)
        {
            return new CartController(cache);
        }

        [TestMethod]
        public void WhenGetCartThatExists_ShouldReturnValidInstance()
        {
            var cart = GetDummyCart();
            var cartBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(cart));

            var mockedCache = new Mock<IDistributedCache>();
            mockedCache
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cartBytes);

            var result = GetController(mockedCache.Object).Get(Guid.NewGuid()).GetAwaiter().GetResult();
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), "Should get the cart.");
        }

        [TestMethod]
        public void WhenGetCartThatDoNotExists_ShouldReturnNull()
        {
            var mockedCache = new Mock<IDistributedCache>();
            mockedCache
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[])null);

            var controller = GetController(mockedCache.Object);
            var cart = controller.Get(Guid.NewGuid()).GetAwaiter().GetResult();
            Assert.IsInstanceOfType(cart, typeof(NotFoundResult), "Should not get the cart.");
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void WhenGetCartButServerThrows_ShouldThrowError()
        {
            var mockedCache = new Mock<IDistributedCache>();
            mockedCache
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception());

            var controller = GetController(mockedCache.Object);
            var result = controller.Get(Guid.NewGuid()).GetAwaiter().GetResult();
            Assert.IsInstanceOfType(result, typeof(StatusCodeResult), "Should not get the cart.");
            Assert.AreEqual(500, ((StatusCodeResult)result).StatusCode);
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
    }
}
