namespace TraningCenter.Entity {
    public partial class Instructor{
        public int InstructorId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateOnly HireDate { get; set; }
        public decimal Salary { get; set; }
        public int? ManagerId { get; set; }
        public bool IsActive { get; set; }

        //Self-Reference
        public virtual ICollection<Instructor> InvertManage { get; set; } = new List<Instructor>();
        public virtual Instructor Manager { get; set; } = null!;

        public virtual ICollection<Course> Course { get; set; } = new List<Course>();
    }
}
