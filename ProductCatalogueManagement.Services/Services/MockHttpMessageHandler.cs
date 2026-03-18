using System.Net.Http.Json;

namespace ProductCatalogueManagement.Services.Services
{
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                //To test fallback mechanism
                //To simulate a failure in 20% of the requests, we can use a random number generator to determine whether to return a successful response or an error response.
                if (new Random().Next(0, 5) == 0)
                {
                    return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
                }

                var response = new
                {
                    Price = 130.39m,
                    Stock = 50
                };

                return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(response)
                };
            }
            catch (Exception ex) {
                
                throw;
            }
        }
    }
}