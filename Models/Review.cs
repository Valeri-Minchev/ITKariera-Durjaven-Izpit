namespace CampRating.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int UserId { get; set; }
        public int CampingSiteId { get; set; }
        public DateTime CreatedAt { get; set; }

        public User User { get; set; }
        public CampingSite CampingSite { get; set; }
    }
}
