namespace No_Forum.Models
{
    public class Posts
    {
        public int Id { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public string Text { get; set; } = default!;
        public int ForumpageId { get; set; }

        
    }
}
