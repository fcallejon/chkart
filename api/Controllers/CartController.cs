using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using chktr.Filters;
using chktr.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace chktr.Controllers
{
    [Route("api/[controller]")]
    public class CartController : Controller
    {
        private readonly CartService _service;

        public CartController(CartService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get a Cart by it's key
        /// </summary>
        /// <param name="key">Cart Key</param>
        /// <returns>A <see cref="Cart" /></returns>
        [HttpGet("{key:guid}")]
        [Produces(typeof(Cart))]
        [SwaggerResponse(404)]
        [SwaggerResponse(400)]
        [SwaggerResponse(500)]
        public async Task<IActionResult> Get(Guid key)
        {
            try
            {
                var cart = await _service.GetCart(key);
                if (cart == null)
                {
                    return NotFound();
                }
                return Ok(cart);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Create a new Cart
        /// </summary>
        /// <param name="newCart">The new <see cref="Cart" /></param>
        /// <returns>The cart key.</returns>
        [HttpPost]
        [Produces(typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: "Any Model Validation Error")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: "Any DB related error")]
        public async Task<IActionResult> Post([FromBody]Cart newCart)
        {
            try
            {
                if (newCart == null)
                {
                    return BadRequest("Missing Body");
                }

                var key = Guid.NewGuid();
                await _service.Create(key, newCart);

                return Ok(key);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Update a Cart
        /// </summary>
        /// <param name="key">The element Key</param>
        /// <param name="newDataCart">The new <see cref="Cart" /></param>
        /// <returns>The new cart key.</returns>
        [HttpPut("{key:Guid}")]
        [SwaggerResponse(404)]
        [SwaggerResponse(400)]
        public async Task<IActionResult> Put(Guid key, [FromBody]Cart newDataCart)
        {
            try
            {
                if (newDataCart == null)
                {
                    return BadRequest("Missing Body");
                }
                var cart = await _service.GetCart(key);
                if (cart == null)
                {
                    return NotFound();
                }
                await _service.Update(key, newDataCart);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Will delete the item with the <paramref name="key"/>.
        /// 
        /// It will <strong>NOT</strong> notify if the key doesn't exist.
        /// </summary>
        /// <param name="key">Element key</param>
        /// <returns>Either 200 or 500</returns>
        [HttpDelete("{key:Guid}")]
        [SwaggerResponse(500, description: "In case something unexpected happens at DB level.")]
        public async Task<IActionResult> Delete(Guid key)
        {
            try
            {
                await _service.Delete(key);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
