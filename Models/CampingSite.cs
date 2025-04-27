namespace CampRating.Models
{
    public class CampingSite
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string PhotoPath { get; set; }

        public List<Review> Reviews { get; set; }
    }
}
