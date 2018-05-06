using System;
using chktr.Model;
using chktr.Client;

namespace test_console
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var client = new ChktrClient("test", "A-KEY-GNERATED-BY-A-KEY-MANAGEMENT-SYSTEM"))
            {
                try
                {
                    var cartId = client.Create(new Cart
                    {
                        Firstname = "First Test Name",
                        Lastname = "Last Test Name",
                        Items = new[] {
                        new CartItem {
                            Description = "Prod Test 1",
                            Quantity = 4,
                            UnitPrice = 7
                        }
                    }
                    }).Result;
                    Console.WriteLine($"Created new Cart: {cartId.ToString()}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
