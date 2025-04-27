namespace CampRating.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int RoleId { get; set; }

        public Role Role { get; set; }
        public List<Review> Reviews { get; set; }
    }
}
