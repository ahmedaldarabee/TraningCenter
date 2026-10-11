namespace TraningCenter.Entity {
    public partial class StudentProfile {
        public int StudentId { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? Bio { get; set; }
        public string? LinkedInUrl { get; set; }
        
        //one to one relationship
        public virtual Student Student { get; set; } = null!;
    }
}
