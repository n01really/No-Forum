namespace No_Forum.Models
{
    public class Coments
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public string Text { get; set; } = default!;
        public int PostId { get; set; }
        public int ForumpageId { get; set; }
    }
}
