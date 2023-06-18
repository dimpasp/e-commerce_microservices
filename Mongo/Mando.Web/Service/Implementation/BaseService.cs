using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Models;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using static Mango.Web.Utility.SD;

namespace Mango.Web.Service.Implementation
{
    public class BaseService : IBaseService
    {
        //todo comment to this
        private readonly IHttpClientFactory _httpClientFactory;
        public BaseService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        //todo make it simple 
        public async Task<ResponseDto?> SendAsync(RequestDto requestDto)
        {
            //todo comment on this
            HttpClient httpClient = _httpClientFactory.CreateClient("MangoAPI");
            HttpRequestMessage httpRequestMessage = new();
            HttpResponseMessage? httpResponse = null;

            httpRequestMessage.Headers.Add("Accept", "application/json");

            //token

            httpRequestMessage.RequestUri = new Uri(requestDto.Url);
            if (requestDto.Data != null)
            {
                httpRequestMessage.Content = new StringContent(
                    JsonConvert.SerializeObject(requestDto.Data),
                    Encoding.UTF8,
                    "application/json");
            }

            //todo another switch to look
            switch (requestDto.ApiType)
            {

                case ApiType.POST:
                    httpRequestMessage.Method = HttpMethod.Post;
                    break;
                case ApiType.PUT:
                    httpRequestMessage.Method = HttpMethod.Put;
                    break;
                case ApiType.DELETE:
                    httpRequestMessage.Method = HttpMethod.Delete;
                    break;
                default:
                    httpRequestMessage.Method = HttpMethod.Get;
                    break;

            }

            httpResponse = await httpClient.SendAsync(httpRequestMessage);

            try
            {
                switch (httpResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return new() { IsSuccess = false, Message = "Not found" };

                    case HttpStatusCode.BadRequest:
                        return new() { IsSuccess = false, Message = "Bad Request" };
                    case HttpStatusCode.Forbidden:
                        return new() { IsSuccess = false, Message = "Forbidden" };
                    case HttpStatusCode.Unauthorized:
                        return new() { IsSuccess = false, Message = "Unauthorized" };
                    case HttpStatusCode.InternalServerError:
                        return new() { IsSuccess = false, Message = "InternalServerError" };
                    default:
                        var apiContent = await httpResponse.Content.ReadAsStringAsync();
                        var apiResponseDto = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
                        return apiResponseDto;

                }
            }
            catch (Exception ex)
            {

                var dto = new ResponseDto
                {
                    Message = ex.Message.ToString(),
                    IsSuccess = false
                };
                return dto;
            }
        }
    }
}
