using System.Net;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace Services
{
    public class HttpService
    {


        internal readonly string _loginUrl = "https://bootcomidentity.azurewebsites.net/AuthenticationV2";

        internal HttpClient CreateClient()
        {
            var httpClientHandler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            var httpClient = new HttpClient(httpClientHandler);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.AcceptEncoding.Clear();
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
                requestMessage.RequestUri = new Uri($"{_loginUrl}/verify-token-validity");
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


        public async Task<Dictionary<string, string>?> ValidateLogin(Guid deviceId, string token, string refreshToken)
        {

            var dateTimeCollection = await GetDateTime(token);

            if (dateTimeCollection.HasValue && dateTimeCollection.Value)
            {
                return null;
            }

            var httpClient = CreateClient();

            var requestMessage = new HttpRequestMessage();
            requestMessage.RequestUri = new Uri($"{_loginUrl}/refresh-token");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
            requestMessage.Method = HttpMethod.Post;

            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(deviceId.ToString(), Encoding.UTF8, MediaTypeNames.Text.Plain), "deviceId");
            formData.Add(new StringContent(refreshToken, Encoding.UTF8, MediaTypeNames.Text.Plain), "refreshToken");
            requestMessage.Content = formData;

            var responseMessage = await httpClient.SendAsync(requestMessage);

            if (!responseMessage.IsSuccessStatusCode)
            {
                throw new UnauthorizedAccessException();
            }

            var stream = await responseMessage.Content.ReadAsStreamAsync();
            var returnDictionary = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(stream);
            return returnDictionary;
        }

        public async Task<Dictionary<string, string>?> CompleteLoginProcess(Guid deviceId, string accessCode)
        {
            var httpClient = CreateClient();

            var requestMessage = new HttpRequestMessage();
            requestMessage.RequestUri = new Uri($"{_loginUrl}/verify-access-code");
            requestMessage.Method = HttpMethod.Post;

            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(accessCode, Encoding.UTF8, MediaTypeNames.Text.Plain), "accessCode");
            formData.Add(new StringContent(deviceId.ToString(), Encoding.UTF8, MediaTypeNames.Text.Plain), "deviceId");
            requestMessage.Content = formData;

            var responseMessage = await httpClient.SendAsync(requestMessage);

            if (!responseMessage.IsSuccessStatusCode)
            {
                return null;
            }

            var stream = await responseMessage.Content.ReadAsStreamAsync();
            return System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(stream);

        }

        public async Task<bool> PerformLoginRequest(Guid deviceId, string emailAddress)
        {
            var httpClient = CreateClient();

            var requestMessage = new HttpRequestMessage();
            requestMessage.RequestUri = new Uri($"{_loginUrl}/generate-access-code/BOOTCOM_HOME");
            requestMessage.Method = HttpMethod.Post;

            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(emailAddress, Encoding.UTF8, MediaTypeNames.Text.Plain), "emailAddress");
            formData.Add(new StringContent(deviceId.ToString(), Encoding.UTF8, MediaTypeNames.Text.Plain), "deviceId");

            requestMessage.Content = formData;

            var responseMessage = await httpClient.SendAsync(requestMessage);

            return responseMessage.IsSuccessStatusCode;
        }

    }
}
