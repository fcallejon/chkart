namespace chktr.Model
{
    public class CartItem
    {
        public string Description { get; set; }

        public int Quantity { get; set; }

        public double UnitPrice { get; set; }

        public double Subtotal => Quantity * UnitPrice;
    }
}