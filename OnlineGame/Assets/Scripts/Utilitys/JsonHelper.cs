using System.Text;
using Newtonsoft.Json;
using Server.Helper;

namespace Helper
{
    internal class JsonHelper
    {
        public static string ToJson(object obj)
        {
            string str = JsonConvert.SerializeObject(obj);
            return str;
        }

        public static T ToObject<T>(string str)
        {
            return JsonConvert.DeserializeObject<T>(str);
        }

        public static T ToObject<T>(byte[] bytes)
        {
            string str = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            return ToObject<T>(str);
        }
    }
}
