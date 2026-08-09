using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SEEDONE.SERVICE.Interfaces.Service.Business;

namespace SEEDONE.SERVICE.Service.Business
{
    public class SerializerService : ISerializerService
    {
        public T DeserializeObject<T>(string s)
        {
            return JsonConvert.DeserializeObject<T>(s);
        }

        public object DeserializeObject(string s, Type type)
        {
            return JsonConvert.DeserializeObject(s, type);
        }

        public string SerializeObject(object o)
        {
            return JsonConvert.SerializeObject(o);
        }
    }
}
