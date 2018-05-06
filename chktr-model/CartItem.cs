using System.ComponentModel.DataAnnotations;

namespace chktr.Model
{
    public class CartItem
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
    }
}