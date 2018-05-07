using System;
using System.Threading.Tasks;
using chktr.Model;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace chktr
{
    public class CartService
    {
        private readonly IDistributedCache _cache;

        public CartService(IDistributedCache cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// Get a Cart by it's id
        /// </summary>
        /// <param name="id">Cart Key</param>
        /// <returns>A <see cref="Cart" /> or null</returns>
        public virtual async Task<Cart> GetCart(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Cart Id is Empty.", nameof(id));
            }
            var cartString = await _cache.GetStringAsync(id.ToString());
            return cartString.To<Cart>();
        }

        /// <summary>
        /// Create a new Cart
        /// </summary>
        /// <param name="id">Cart Id</param>
        /// <param name="cart">The new <see cref="Cart" /> to save.</param>
        public virtual async Task Create(Guid id, Cart cart)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Cart Id is Empty.", nameof(id));
            }
            if (cart == null)
            {
                throw new ArgumentNullException("Cart is null.", nameof(cart));
            }
            try
            {
                await _cache
                    .SetStringAsync(
                        id.ToString(),
                        JsonConvert.SerializeObject(cart),
                        new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                        });
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Update a Cart
        /// </summary>
        /// <param name="key">The element Key</param>
        /// <param name="newDataCart">The new <see cref="Cart" /></param>
        public virtual async Task Update(Guid id, Cart cart)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Cart Id is Empty.", nameof(id));
            }
            if (cart == null)
            {
                throw new ArgumentNullException("Cart is null.", nameof(cart));
            }
            try
            {
                await _cache.RemoveAsync(id.ToString());
                await _cache
                    .SetStringAsync(
                        id.ToString(),
                        JsonConvert.SerializeObject(cart),
                        new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                        });
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Will delete the item with the <paramref name="key"/>.
        /// </summary>
        /// <param name="key">Element key</param>
        public virtual async Task Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Cart Id is Empty.", nameof(id));
            }
            await _cache.RemoveAsync(id.ToString());
        }
    }
}