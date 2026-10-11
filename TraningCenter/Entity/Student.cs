namespace TraningCenter.Entity {
    public partial class Student {
        public int StudentId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateOnly DateOfBirth { get; set; }
        public DateTime RegisteredAt { get; set; }
        public string Status { get; set; } = null!;
        public string? PhoneNumber { get; set; }

        public virtual ICollection<Enrollment> Enrollment { get; set; } = new List<Enrollment>();
        
        //one to one relation
        public virtual StudentProfile? StudentProfile { get; set; }
    }
}
