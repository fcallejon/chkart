using System;

namespace test_console
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new chktr.Client.ChktrClient("test", "A-KEY-GNERATED-BY-A-KEY-MANAGEMENT-SYSTEM");
            var cart = client.GetCart(new Guid("962b203d-c005-4ef9-ac8b-9e7b26f9216e")).Result;
            Console.WriteLine($"Got Cart: {cart != null}");
        }
    }
}
