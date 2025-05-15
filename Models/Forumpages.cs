namespace No_Forum.Models
{
    public class Forumpages
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }

        public bool Political { get; set; }

        public bool NSFW { get; set; }
        public bool Roleplay { get; set; }
        public bool Discussion { get; set; }
        public bool Meme { get; set; }
        public bool Art { get; set; }
        public bool Technology { get; set; }
    }
}
