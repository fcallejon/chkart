using System.Text;
using Newtonsoft.Json;

namespace chktr
{
    public static class StringExtensions
    {
        public static T To<T>(this string data, T @default = default(T))
        {
            return string.IsNullOrEmpty(data) ? @default : JsonConvert.DeserializeObject<T>(data);
        }
    }
}