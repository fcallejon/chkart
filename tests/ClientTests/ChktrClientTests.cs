using Microsoft.VisualStudio.TestTools.UnitTesting;
using chktr.Client;
using chktr.Model;
using Moq;
using RestSharp;
using System.Net;
using System.Threading.Tasks;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace chktr.ClientTests
{
    [TestClass]
    public class ChktrClientTests
    {
        private Mock<IRestClient> _restSharpClient;
        private IList<Parameter> _defaultParameters;

        [TestInitialize]
        public void InitializeTest()
        {
            _defaultParameters = new List<Parameter>();
            _restSharpClient = new Mock<IRestClient>();
            _restSharpClient
                .Setup(c => c.DefaultParameters)
                .Returns(_defaultParameters);
        }

        [TestCleanup]
        public void TestFinalize()
        {
            _defaultParameters.Clear();
            _defaultParameters = null;
            _restSharpClient = null;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WhenEmptyAppName_ShouldThrowError()
        {
            var client = new ChktrClient("", "test", "http://test.com");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WhenEmptyAppKey_ShouldThrowError()
        {
            var client = new ChktrClient("test", "", "http://test.com");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WhenEmptyHost_ShouldThrowError()
        {
            var client = new ChktrClient("test", "Test", "");
        }

        [TestMethod]
        [ExpectedException(typeof(UriFormatException))]
        public void WhenInvalidHost_ShouldThrowError()
        {
            var client = new ChktrClient("test", "test", "hp://!£$$%^test.com");
        }

        [TestMethod]
        public void WhenGetCartThatExists_ShouldReturnValidInstance()
        {
            _restSharpClient
                .Setup(c => c.ExecuteGetTaskAsync<Cart>(It.IsAny<IRestRequest>()))
                .ReturnsAsync(new RestResponse<Cart>
                {
                    StatusCode = HttpStatusCode.OK,
                    Data = GetDummyCart()
                });

            using (var client = new ChktrClient("test", "test", "http://test.com", _restSharpClient.Object))
            {
                var cart = client.Get(Guid.NewGuid()).GetAwaiter().GetResult();
                Assert.IsNotNull(cart, "Should get a Cart not matter what I pass as ID.");
            }
        }

        [TestMethod]
        public void WhenGetCartThatDoNotExists_ShouldReturnNull()
        {
            _restSharpClient
                .Setup(c => c.ExecuteGetTaskAsync<Cart>(It.IsAny<IRestRequest>()))
                .ReturnsAsync(new RestResponse<Cart> { StatusCode = HttpStatusCode.NotFound });

            using (var client = new ChktrClient("test", "test", "http://test.com", _restSharpClient.Object))
            {
                var cart = client.Get(Guid.NewGuid()).GetAwaiter().GetResult();
                Assert.IsNull(cart, "Should not get a Cart when it doesn't exists.");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void WhenGetCartIdEmpty_ShouldThrow()
        {
            using (var client = new ChktrClient("test", "test", "http://test.com", _restSharpClient.Object))
            {
                var cart = client.Get(Guid.Empty).GetAwaiter().GetResult();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void WhenGetCartButServerThrows_ShouldThrowError()
        {
            _restSharpClient
                .Setup(c => c.ExecuteGetTaskAsync<Cart>(It.IsAny<IRestRequest>()))
                .ThrowsAsync(new ApplicationException("SOMETHING WRONG IN THE SERVER"));

            using (var client = new ChktrClient("test", "test", "http://test.com", _restSharpClient.Object))
            {
                var cart = client.Get(Guid.NewGuid()).GetAwaiter().GetResult();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void WhenDeleteCartButServerThrows_ShouldThrowError()
        {
            _restSharpClient
                .Setup(c => c.ExecuteTaskAsync<bool>(It.IsAny<IRestRequest>()))
                .ThrowsAsync(new ApplicationException("SOMETHING WRONG IN THE SERVER"));

            using (var client = new ChktrClient("test", "test", "http://test.com", _restSharpClient.Object))
            {
                var cart = client.Delete(Guid.NewGuid()).GetAwaiter().GetResult();
            }
        }

        [TestMethod]
        public void WhenDeleteCartButDoNotExists_ShouldReturnFalse()
        {
            _restSharpClient
                .Setup(c => c.ExecuteTaskAsync<bool>(It.IsAny<IRestRequest>()))
                .ReturnsAsync(new RestResponse<bool> { StatusCode = HttpStatusCode.NotFound });

            using (var client = new ChktrClient("test", "test", "http://test.com", _restSharpClient.Object))
            {
                Assert.IsFalse(client.Delete(Guid.NewGuid()).GetAwaiter().GetResult(), "Should return false as cart do not exist.");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void WhenDeleteCartIsEmpty_ShouldThrow()
        {
            using (var client = new ChktrClient("test", "test", "http://test.com", _restSharpClient.Object))
            {
                Assert.IsFalse(client.Delete(Guid.Empty).GetAwaiter().GetResult(), "Should return false as cart do not exist.");
            }
        }

        [TestMethod]
        public void WhenDeleteAndCartExists_ShouldReturnTrue()
        {
            _restSharpClient
                .Setup(c => c.ExecuteTaskAsync<bool>(It.IsAny<IRestRequest>()))
                .ReturnsAsync(new RestResponse<bool> { StatusCode = HttpStatusCode.OK, Data = true });

            using (var client = new ChktrClient("test", "test", "http://test.com", _restSharpClient.Object))
            {
                Assert.IsTrue(client.Delete(Guid.NewGuid()).GetAwaiter().GetResult(), "Should return true as cart do exist.");
            }
        }

        [TestMethod]
        public void WhenCreateSuccess_ShouldReturnGuid()
        {
            var expectedGuid = Guid.NewGuid();
            _restSharpClient
                .Setup(c => c.ExecutePostTaskAsync<Guid>(It.IsAny<IRestRequest>()))
                .ReturnsAsync(new RestResponse<Guid>
                {
                    StatusCode = HttpStatusCode.OK,
                    Data = expectedGuid
                });

            using (var client = new ChktrClient("test", "test", "http://test.com", _restSharpClient.Object))
            {
                var createdId = client.Create(GetDummyCart()).GetAwaiter().GetResult();
                Assert.AreEqual(expectedGuid, createdId, "Just created Cart should have the expected id.");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WhenCreateEmptyCart_ShouldThrowException()
        {
            using (var client = new ChktrClient("test", "test", "http://test.com", _restSharpClient.Object))
            {
                var cartId = client.Create(null).GetAwaiter().GetResult();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void WhenCreateEmptyFirstnameCart_ShouldReturnThrowException()
        {
            var cart = GetDummyCart();
            cart.Firstname = string.Empty;

            _restSharpClient
                .Setup(c => c.ExecutePostTaskAsync<Guid>(
                    It.Is<IRestRequest>(r =>
                        r.Parameters
                            .Any(p => string.IsNullOrEmpty(JsonConvert.DeserializeObject<Cart>(p.Value.ToString()).Firstname))
                    )))
                .ReturnsAsync(new RestResponse<Guid>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Data = Guid.Empty
                });

            using (var client = new ChktrClient("test", "test", "http://test.com", _restSharpClient.Object))
            {
                var task = client.Create(cart);
                _restSharpClient.VerifyAll();
                var cartId = task.GetAwaiter().GetResult();
            }
        }

        [TestMethod]
        public void WhenUpdateSuccess_ShouldReturnTrue()
        {
            var expectedGuid = Guid.NewGuid();
            _restSharpClient
                .Setup(c => c.ExecuteTaskAsync<Cart>(It.IsAny<IRestRequest>()))
                .ReturnsAsync(new RestResponse<Cart>
                {
                    StatusCode = HttpStatusCode.OK,
                    Data = null
                });

            using (var client = new ChktrClient("test", "test", "http://test.com", _restSharpClient.Object))
            {
                var updated = client.Update(expectedGuid, GetDummyCart()).GetAwaiter().GetResult();
                Assert.IsTrue(updated, "Just created Cart should have the expected id.");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WhenUpdateEmptyCart_ShouldReturnThrowException()
        {
            var cartId = Guid.NewGuid();

            using (var client = new ChktrClient("test", "test", "http://test.com", _restSharpClient.Object))
            {
                var created = client.Update(cartId, null).GetAwaiter().GetResult();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void WhenUpdateEmptyFirstnameCart_ShouldReturnThrowException()
        {
            var cart = GetDummyCart();
            cart.Firstname = string.Empty;

            _restSharpClient
                .Setup(c => c.ExecuteTaskAsync<Cart>(
                    It.IsAny<IRestRequest>()))
                .ReturnsAsync(new RestResponse<Cart>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    StatusDescription = "WhenUpdateEmptyFirstnameCart_ShouldReturnThrowException",
                    Data = null
                })
                .Verifiable();

            using (var client = new ChktrClient("test", "test", "http://test.com", _restSharpClient.Object))
            {
                var updated = client.Update(Guid.NewGuid(), cart).GetAwaiter().GetResult();
            }
            _restSharpClient.VerifyAll();
        }

        [TestMethod]
        public void WhenUpdateCartDoNotExists_ShouldReturnThrowException()
        {
            _restSharpClient
                .Setup(c => c.ExecuteTaskAsync<Cart>(
                    It.IsAny<IRestRequest>()))
                .ReturnsAsync(new RestResponse<Cart>
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Data = null
                })
                .Verifiable();

            using (var client = new ChktrClient("test", "test", "http://test.com", _restSharpClient.Object))
            {
                var updated = client.Update(Guid.NewGuid(), GetDummyCart()).GetAwaiter().GetResult();
                Assert.IsFalse(updated, "Should return false as it wasn't found.");
            }
            _restSharpClient.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void WhenUpdateEmptyCartId_ShouldReturnThrowException()
        {
            using (var client = new ChktrClient("test", "test", "http://test.com", _restSharpClient.Object))
            {
                var updated = client.Update(Guid.Empty, GetDummyCart()).GetAwaiter().GetResult();
            }
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