using No_Forum.Models;
using System.Net.Http;
using System.Net.Http.Json;
using No_Forum.Models;

namespace No_Forum.Service
{
    // Service class for handling API requests related to posts
    public class PostsApiService
    {
        private readonly HttpClient _httpClient; // HTTP client for making API calls

        // Constructor that injects the HttpClient dependency
        public PostsApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Sends a collection of posts to the API for import
        public async Task<bool> ImportPostsAsync(IEnumerable<Posts> posts)
        {
            // Sends a POST request with the posts as JSON to the specified API endpoint
            var response = await _httpClient.PostAsJsonAsync("https://noapi.azure-api.net/Posts/import-from-website-db", posts);
            // Returns true if the request was successful
            return response.IsSuccessStatusCode;
        }
    }
}
