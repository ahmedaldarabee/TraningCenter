namespace TraningCenter.Entity {
    public partial class Course {
        public int CourseId { get; set; }
        public string Title { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string Level { get; set; } = null!;
        public int DurationHours { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
        public string Status { get; set; } = null!;

        //navigational properties section: to enable you to access enrollment table info from course
        //[ virtual ] upload data automaticily when you handle lazy loading
        public virtual ICollection<Enrollment> Enrollment { get; set; } = new List <Enrollment>();
        
        public int InstructorId { get; set; } //FK
        public virtual Instrcutor Instrcutor {get;set;} = null!;
    }
}
