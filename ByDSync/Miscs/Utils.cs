using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ByDSync.Miscs
{
    public class Utils
    {
        public JArray parseJson(string jsonRaw)
        {
            JObject jsonData = JObject.Parse(jsonRaw);
            jsonData = JObject.Parse(jsonData["d"].ToString());
            JArray jsonEnumerableData = JArray.Parse(jsonData["results"].ToString());

            return jsonEnumerableData;
        }

    }
}