using Microsoft.EntityFrameworkCore;
using TraningCenter.Entity;

namespace TraningCenter.Data {
    public partial class AppDbContext :DbContext {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options){}

        public virtual DbSet<Course> Course { get; set; }
        public virtual DbSet<Instrcutor> Instrcutor { get; set; }
        public virtual DbSet<Enrollment> Enrollment { get; set; }
        public virtual DbSet<Student> Student { get; set; }
        public virtual DbSet<StudentProfile> StudentProfile { get; set; }

        //OnModelCreating: That configure table manually using fluent api, 
        //because not everything it will be [ doing as expected ] by conventions
        // where we want to define keys, relations,indexes,column type, default-values , constraints
        protected override void OnModelCreating(ModelBuilder modelBuilder) {

            modelBuilder.Entity<Course>(entity => {
                entity.HasIndex(e => e.InstructorId, "IX_Courses_InstructorId");
                entity.HasIndex(e => e.Status, "IX_Courses_Status");
                
                //the HasIndex can define both of IX [ Indexes ] and UQ [ Unique Values ]
                entity.HasIndex(e => e.Code, "UQ_Courses_Code").IsUnique();

            });
            modelBuilder.Entity<Instrcutor>(entity => { });
            modelBuilder.Entity<Enrollment>(entity => { });
            modelBuilder.Entity<Student>(entity => { });
            modelBuilder.Entity<StudentProfile>(entity => { });



            base.OnModelCreating(modelBuilder);
        }
    }
}
