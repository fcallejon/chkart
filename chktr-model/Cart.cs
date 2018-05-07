using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace chktr.Model
{
    public class Cart
    {
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string Firstname { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string Lastname { get; set; }

        public IList<CartItem> Items { get; set; }

        public double? Total => Items?.Sum(i => i.Subtotal);
    }
}