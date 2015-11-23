using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Dian.BindParameterForWebApi
{
    public class Tool
    {
        /// <summary>
        /// 尝试转为JSON对象
        /// </summary>
        /// <param name="jsonString"></param>
        /// <param name="jsonObject"></param>
        /// <returns></returns>
        public static bool TryParseJson<T>(string jsonString, out T jsonObject)
        {
            try
            {
                jsonObject = JsonConvert.DeserializeObject<T>(jsonString);
                return true;
            }
            catch (Exception)
            {
                jsonObject = default(T);
                return false;
            }
        }
    }
}
