using System.Text;
using Newtonsoft.Json;

namespace chktr
{
    public static class ByteExtensions
    {
        public static T To<T>(this byte[] data)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(data));
        }
    }
}