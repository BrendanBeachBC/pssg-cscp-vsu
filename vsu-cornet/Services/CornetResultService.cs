using Gov.Cscp.Victims.Public.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System;

namespace Gov.Cscp.Victims.Public.Services
{
    public interface ICornetResultService
    {
        Task<DynamicsResult> Get(string endpointUrl);
        Task<DynamicsResult> Post(string endpointUrl, string requestJson);
    }

    public class CornetResultService : ICornetResultService
    {
        private HttpClient _client;
        private IConfiguration _configuration;

        public CornetResultService(IConfiguration configuration, HttpClient httpClient)
        {
            _client = httpClient;
            _configuration = configuration;
        }

        public async Task<DynamicsResult> Get(string endpointUrl)
        {
            DynamicsResult blob = await DynamicsResultAsync(HttpMethod.Get, endpointUrl, "");
            return blob;
        }

        public async Task<DynamicsResult> Post(string endpointUrl, string modelJson)
        {
            DynamicsResult blob = await DynamicsResultAsync(HttpMethod.Post, endpointUrl, modelJson);
            return blob;
        }

        private async Task<DynamicsResult> DynamicsResultAsync(HttpMethod method, string endpointUrl, string requestJson)
        {
            endpointUrl = _configuration["CORNET_URI"] + endpointUrl;
            requestJson = requestJson.Replace("fortunecookie", "@odata.");

            Console.WriteLine(endpointUrl);
            Console.WriteLine(requestJson);

            HttpRequestMessage _httpRequest = new HttpRequestMessage(method, endpointUrl);
            _httpRequest.Headers.Add("username", "test");
            _httpRequest.Headers.Add("fullname", "test");
            _httpRequest.Headers.Add("client", "test");
            _httpRequest.Content = new StringContent(requestJson, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage _httpResponse = await _client.SendAsync(_httpRequest);
            HttpStatusCode _statusCode = _httpResponse.StatusCode;

            string _responseContent = await _httpResponse.Content.ReadAsStringAsync();

            DynamicsResult result = new DynamicsResult();
            result.statusCode = _statusCode;
            result.responseMessage = _httpResponse;
            string clean = _responseContent.Replace("@odata.", "fortunecookie");
            result.result = Newtonsoft.Json.Linq.JObject.Parse(clean);

            Console.WriteLine(result.result);

            return result;
        }
    }
}