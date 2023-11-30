using System.Net.Http.Headers;
using System.Net.Mail;

namespace Services
{
    public class HttpService
    {


        internal readonly string _loginUrl = "https://bootcomidentity.azurewebsites.net";

        internal HttpClient CreateClient()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            return httpClient;
        }

        public HttpService()
        {
        }

        private async Task<bool?> GetDateTime(string token)
        {
            try
            {
                var httpClient = CreateClient();

                var requestMessage = new HttpRequestMessage();
                requestMessage.RequestUri = new Uri($"{_loginUrl}/DateTime");
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
                requestMessage.Method = HttpMethod.Get;

                var responseMessage = await httpClient.SendAsync(requestMessage);

                return responseMessage.IsSuccessStatusCode;
            }
            catch
            {
                return null;
            }
        }

        private async Task<string?> GetUserId(string token)
        {
            try
            {
                var httpClient = CreateClient();

                var requestMessage = new HttpRequestMessage();
                requestMessage.RequestUri = new Uri($"{_loginUrl}/User/name");
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
                requestMessage.Method = HttpMethod.Get;

                var responseMessage = await httpClient.SendAsync(requestMessage);

                return await responseMessage.Content.ReadAsStringAsync();
            }
            catch
            {
                return null;
            }
        }

        public async Task<Dictionary<string, string>?> ValidateLogin(Guid deviceId, string token, string refreshToken)
        {

            var dateTimeCollection = await GetDateTime(token);

            if (dateTimeCollection.HasValue && dateTimeCollection.Value)
            {
                return null;
            }

            var httpClient = CreateClient();

            var requestMessage = new HttpRequestMessage();
            requestMessage.RequestUri = new Uri($"{_loginUrl}/RefreshToken");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
            requestMessage.Method = HttpMethod.Post;
            requestMessage.Headers.Add("RefreshToken", refreshToken);
            requestMessage.Headers.Add("DeviceId", deviceId.ToString());

            var responseMessage = await httpClient.SendAsync(requestMessage);

            if (!responseMessage.IsSuccessStatusCode)
            {
                throw new UnauthorizedAccessException();
            }

            var returnDictionary = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(await responseMessage.Content.ReadAsStreamAsync());
            return returnDictionary;
        }

        public async Task<Dictionary<string, string>?> CompleteLoginProcess(Guid deviceId, string accessCode)
        {
            var httpClient = CreateClient();

            var requestMessage = new HttpRequestMessage();
            requestMessage.RequestUri = new Uri($"{_loginUrl}/Authentication/{accessCode}");
            requestMessage.Method = HttpMethod.Post;
            requestMessage.Headers.Add("DeviceId", deviceId.ToString());

            var responseMessage = await httpClient.SendAsync(requestMessage);

            if (!responseMessage.IsSuccessStatusCode)
            {
                return null;
            }

            return System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(await responseMessage.Content.ReadAsStreamAsync());

        }

        public async Task<bool> PerformLoginRequest(Guid deviceId, string emailAddress)
        {
            var httpClient = CreateClient();
            
            var requestMessage = new HttpRequestMessage();
            requestMessage.RequestUri = new Uri($"{_loginUrl}/Authentication");
            requestMessage.Method = HttpMethod.Post;
            requestMessage.Headers.Add("EmailAddress", emailAddress);
            requestMessage.Headers.Add("DeviceId", deviceId.ToString());

            var responseMessage = await httpClient.SendAsync(requestMessage);

            return responseMessage.IsSuccessStatusCode;
        }

    }
}
