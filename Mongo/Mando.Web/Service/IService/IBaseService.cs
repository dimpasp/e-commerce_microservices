using Mando.Web.Models;
using Mango.Web.Models;

namespace Mando.Web.Service.IService
{
    public interface IBaseService
    {
        Task<ResponseDto?> SendAsync(RequestDto requestDto);
    }
}
