namespace TraningCenter.Entity {
    public partial class Instrcutor {
        public int InstructorId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateOnly HireDate { get; set; }
        public decimal Salary { get; set; }
        public int? ManagerId { get; set; }
        public bool IsActive { get; set; }

        //Self-Reference
        public virtual ICollection<Instrcutor> InvertManage { get; set; } = new List<Instrcutor>();
        public virtual Instrcutor Manager { get; set; } = null!;

        public virtual ICollection<Course> Course { get; set; } = new List<Course>();
    }
}
