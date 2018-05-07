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
    public class CartItemController : Controller
    {
        private readonly CartService _service;

        public CartItemController(CartService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get a CartItem by it's index and <see cref="Cart"/>'s key
        /// </summary>
        /// <param name="key">Cart Key</param>
        /// <param name="itemIndex">Cart item index</param>
        /// <returns>A <see cref="Cart" /></returns>
        [HttpGet("{key:guid}/{itemIndex:range(0,9999)}")]
        [Produces(typeof(Cart))]
        [SwaggerResponse(404)]
        [SwaggerResponse(400)]
        [SwaggerResponse(500)]
        public async Task<IActionResult> Get(int itemIndex, Guid key)
        {
            try
            {
                var cart = await _service.GetCart(key);
                if (cart == null)
                {
                    return CartNotFound();
                }
                var item = cart.Items.ElementAtOrDefault(itemIndex);
                if (item == null)
                {
                    return NotFound();
                }
                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Create a new Cart
        /// </summary>
        /// <param name="newItem">The new <see cref="Cart" /></param>
        /// <returns>The cart key.</returns>
        [HttpPost("{id:guid}")]
        [Produces(typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: "Any Model Validation Error")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: "Any DB related error")]
        public async Task<IActionResult> Post(Guid id, [FromBody]CartItem newItem)
        {
            try
            {
                return await Do(id, newItem, (c, i) =>
                {
                    var items = (c.Items ?? new CartItem[0]);
                    if (items.Contains(i))
                    {
                        throw new InvalidOperationException("Can't insert a duplicated Item.");
                    }
                    c.Items = items.Append(i).ToArray();
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Update a Cart
        /// </summary>
        /// <param name="id">The element id</param>
        /// <param name="newDataCartItem">The new <see cref="Cart" /></param>
        /// <returns>The new cart key.</returns>
        [HttpPut("{id:guid}")]
        [SwaggerResponse(404)]
        [SwaggerResponse(400)]
        public async Task<IActionResult> Put(Guid id, [FromBody]CartItem newDataCartItem)
        {
            try
            {
                return await Do(id, newDataCartItem, (c, i) =>
                {
                    var items = (c.Items ?? new CartItem[0]);
                    if (!items.Any())
                    {
                        throw new IndexOutOfRangeException("Cart do not have any Items.");
                    }
                    var itemIndex = items.IndexOf(i);
                    if (itemIndex < 0)
                    {
                        throw new IndexOutOfRangeException("Cart do not have this Item.");
                    }
                    c.Items[itemIndex] = i;
                });
            }
            catch(IndexOutOfRangeException ior)
            {
                return NotFound(ior.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Will delete the item with the <paramref name="key"/>.
        /// 
        /// It will <strong>NOT</strong> notify if the key doesn't exist.
        /// </summary>
        /// <param name="key">Element key</param>
        /// <param name="itemIndex">0-based index</param>
        /// <returns>Either 200 or 500</returns>
        [HttpDelete("{id:guid}/{itemIndex:range(0,9999)}")]
        [SwaggerResponse(500, description: "In case something unexpected happens at DB level.")]
        public async Task<IActionResult> Delete(Guid id, int itemIndex)
        {
            try
            {
                return await Do(id, null, (c, i) =>
                {
                    if (c.Items == null || c.Items.ElementAtOrDefault(itemIndex) == null)
                    {
                        throw new IndexOutOfRangeException("Cart do not have any Items.");
                    }
                    c.Items.RemoveAt(itemIndex);
                }, true);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        private async Task<IActionResult> Do(Guid id, CartItem item, Action<Cart, CartItem> action, bool ignoreNullItem = false)
        {
            if (!ignoreNullItem && item == null)
            {
                return BadRequest("Missing Body");
            }
            var cart = await _service.GetCart(id);
            if (cart == null)
            {
                return CartNotFound();
            }
            action(cart, item);
            await _service.Update(id, cart);
            return Ok();
        }

        private ObjectResult CartNotFound()
        {
            return StatusCode((int)HttpStatusCode.NotFound, "Cart Not Found");
        }
    }
}
