namespace No_Forum.Models
{
    public class Comments
    {
        public int Id { get; set; }
        public int PostsId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public string Text { get; set; } = default!;
        public int ForumpageId { get; set; }
        public bool Flagged { get; set; } = false;
    }
}
