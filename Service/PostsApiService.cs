using No_Forum.Models;
using System.Net.Http;
using System.Net.Http.Json;
using No_Forum.Models;

namespace No_Forum.Service
{
    public class PostsApiService
    {
        private readonly HttpClient _httpClient;

        public PostsApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> ImportPostsAsync(IEnumerable<Posts> posts)
        {
            var response = await _httpClient.PostAsJsonAsync("https://localhost:7022/Posts", posts);
            return response.IsSuccessStatusCode;
        }
    }
}
