using System;
using System.Text;
using System.Text.Json;
using Course.Ui.Exceptions;
using Course.Ui.Resources;
using Microsoft.Net.Http.Headers;

namespace Course.Ui.Service
{
    public class CrudService : ICrudService
    {
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;
      
        private const string baseUrl = "https://localhost:7064/api/";

        public CrudService(IHttpContextAccessor httpContextAccessor)
        {
            _client = new HttpClient();
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task Create<TRequest>(TRequest data, string url)
        {
            var jsonData = JsonSerializer.Serialize(data);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);

            using (var response = await _client.PostAsync(baseUrl + url, content))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpException(response.StatusCode);
                }
            }
        }
        public async Task<PaginatedResponseResource<TResponse>> GetAllPaginated<TResponse>(string path, int page)
        {
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);

            using (var response = await _client.GetAsync(baseUrl + path + "?page=" + page))
            {
                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<PaginatedResponseResource<TResponse>>(await response.Content.ReadAsStringAsync(), options);
                }
                else throw new HttpException(response.StatusCode);
            }
        }
       
        public async Task<TResponse> Get<TResponse>(string path)
        {
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);

            using (var response = await _client.GetAsync(baseUrl + path))
            {
                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<TResponse>(await response.Content.ReadAsStringAsync(), options);
                }
                else { throw new HttpException(response.StatusCode); }
            }
        }
        public async Task Delete(string url)
        {
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);

            using (var response = await _client.DeleteAsync(baseUrl + url))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpException(response.StatusCode);
                }
            }
        }

    }
}

