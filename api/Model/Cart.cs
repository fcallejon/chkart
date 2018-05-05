using System.Collections.Generic;
using System.Linq;

namespace chktr.Model
{
    public class Cart
    {        
        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public IEnumerable<CartItem> Items { get; set; }

        public double Total => Items.Sum(i => i.Subtotal);
    }
}