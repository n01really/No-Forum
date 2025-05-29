namespace No_Forum.Models
{
    public class DM
    {
        public int Id { get; set; }

        public string SenderId { get; set; }

        public string ReciverId { get; set; }

        public string? Message { get; set; }

        public DateTime CreatedAt { get; set; }
        public string? SenderName { get; set; }
    }
}
