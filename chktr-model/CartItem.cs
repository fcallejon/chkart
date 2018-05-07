using System;
using System.ComponentModel.DataAnnotations;

namespace chktr.Model
{
    public class CartItem : IEquatable<CartItem>
    {
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string Description { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public double UnitPrice { get; set; }

        public double Subtotal => Quantity * UnitPrice;

        public bool Equals(CartItem other)
        {
            if (other == null) return false;
            return Description == other.Description;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as CartItem);
        }

        public override int GetHashCode()
        {
            return (13 * 397) ^ Description.GetHashCode();
        }
    }
}