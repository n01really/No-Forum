namespace No_Forum.Models
{
    public class Friends
    {
        public int Id { get; set; }
        public string UserId { get; set; }         
        public string FriendUserId { get; set; }   
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
