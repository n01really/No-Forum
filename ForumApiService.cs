using System.Collections.Generic; 
using System.Net.Http; 
using System.Net.Http.Json;
namespace No_Forum
{
    public class ForumApiService
    {
        private readonly HttpClient _httpClient;

        public ForumApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Example method
        public async Task<HttpResponseMessage> AddPostAsync(Post post)
        {
            return await _httpClient.PostAsJsonAsync("api/posts", post);
        }
        public async Task<HttpResponseMessage> AddCommentAsync(Comment comment)
        {
            return await _httpClient.PostAsJsonAsync("api/comments", comment);
        }

        public class Post
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Content { get; set; }
            public DateTime CreatedAt { get; set; }
            public string CreatedBy { get; set; }
            
        }
        public class Comment
        {
            public int Id { get; set; }
            
            public string Content { get; set; }
           
            public string CreatedBy { get; set; }
        }
    }
}
