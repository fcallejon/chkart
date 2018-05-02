using System.Text;
using Newtonsoft.Json;

namespace chktr
{
    public static class StringExtensions
    {
        public static T To<T>(this string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }
    }
}