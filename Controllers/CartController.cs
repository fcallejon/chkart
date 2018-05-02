﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chktr.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace chktr.Controllers
{
    [Route("api/[controller]")]
    public class CartController : Controller
    {
        private readonly IDistributedCache _cache;

        public CartController(IDistributedCache cache)
        {
            this._cache = cache;
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
        public async Task<IActionResult> Get(Guid key)
        {
            try
            {
                var cart = await _cache.GetStringAsync(key.ToString());
                if (cart == null){
                    return NotFound();
                }
                return Ok(cart.To<Cart>());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Create a new Cart
        /// </summary>
        /// <param name="newCart">The new <see cref="Cart" /></param>
        /// <returns>The cart key.</returns>
        [HttpPost]
        [Produces(typeof(string))]
        [SwaggerResponse(404)]
        public async Task<IActionResult> Post([FromBody]Cart newCart)
        {
            var key = Guid.NewGuid().ToString();
            await _cache.SetStringAsync(key,
                JsonConvert.SerializeObject(newCart),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                });
            return Ok(key);
        }

        /// <summary>
        /// Update a Cart
        /// </summary>
        /// <param name="key">The element Key</param>
        /// <param name="newCart">The new <see cref="Cart" /></param>
        /// <returns>The new cart key.</returns>
        [HttpPut("{key:Guid}")]
        [SwaggerResponse(404)]
        [SwaggerResponse(400)]
        public async Task<IActionResult> Put(Guid key, [FromBody]Cart newCart)
        {
            try
            {
                await _cache.RemoveAsync(key.ToString());
                await _cache.SetStringAsync(key.ToString(),
                    JsonConvert.SerializeObject(newCart),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                    });
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound(ex.GetType().Name);
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
            try{
            await _cache.RemoveAsync(key.ToString());
            return Ok();
            }
            catch(Exception ex){
                return StatusCode(500, ex.Message);
            }
        }
    }
}