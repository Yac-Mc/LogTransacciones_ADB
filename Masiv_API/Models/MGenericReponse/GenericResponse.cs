using Newtonsoft.Json;
using System.Net;

namespace Masiv_API.Models.MGenericReponse
{
    public class GenericResponse<T>
    {
        public bool IsSuccesful { get; set; } = true;
        public int StatusCode { get; set; } = (int)HttpStatusCode.OK;
        public string Message { get; set; }
        public T Result { get; set;}
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
