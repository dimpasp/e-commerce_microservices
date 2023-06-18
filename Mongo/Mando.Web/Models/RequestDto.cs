using static Mango.Web.Utility.SD;

namespace Mango.Web.Models
{
    public class RequestDto
    {
        /// <summary>
        /// we use enumerable to determine the http types
        /// </summary>
        public ApiType ApiType { get; set; } = ApiType.GET;
        /// <summary>
        /// 
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AccessToken { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public object Data { get; set;}
    }
   }

