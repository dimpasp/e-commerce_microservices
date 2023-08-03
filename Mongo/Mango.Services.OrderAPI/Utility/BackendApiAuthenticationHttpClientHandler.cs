using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;

namespace Mango.Services.OrderAPI.Utility
{
    //DelegatingHandler is like dot net core middleware
    //however are on client side
    public class BackendApiAuthenticationHttpClientHandler:DelegatingHandler
    {
        //for cookies we need to inject IHttpContextAccessor
        private readonly IHttpContextAccessor _contextAccessor;
        public BackendApiAuthenticationHttpClientHandler(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        //override method to access the token and add to authorization header
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _contextAccessor.HttpContext.GetTokenAsync("access_token");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
