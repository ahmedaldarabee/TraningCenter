using Microsoft.EntityFrameworkCore;
using TraningCenter.Entity;

namespace TraningCenter.Data {
    public partial class AppDbContext :DbContext {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options){}

        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<Enrollment> Enrollments { get; set; }
        public virtual DbSet<Instrcutor> Instrcutors { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<StudentProfile> StudentProfiles { get; set; }

        //OnModelCreating: That configure table manually using fluent api, 
        //because not everything it will be [ doing as expected ] by conventions
        //where we want to define keys :PK,FK , relations,indexes,column type, default-values , constraints and more...
        //we will not define all entity fields, where just important constrains
        protected override void OnModelCreating(ModelBuilder modelBuilder) {

            modelBuilder.Entity<Course>(entity => {
                entity.HasIndex(e => e.InstructorId, "IX_Courses_InstructorId");
                entity.HasIndex(e => e.Status, "IX_Courses_Status");
                
                //the HasIndex can define both of IX [ Indexes ] and UQ [ Unique Values ]
                entity.HasIndex(e => e.Code, "UQ_Courses_Code").IsUnique();
                entity.Property(e => e.Code).HasMaxLength(30);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Level).HasMaxLength(30);

                entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.PublishedAt).HasColumnType("datetime");
                entity.Property(e => e.Status).HasMaxLength(20);
                entity.Property(e => e.Title).HasMaxLength(150);

                entity.HasOne(d => d.Instrcutor).WithMany(p => p.Course)
                      .HasForeignKey(d => d.InstructorId)
                      .OnDelete(DeleteBehavior.ClientSetNull) // that mean if Instructor is deleted the Course.InstructorId = null;
                      .HasConstraintName("FK_Courses_Instructors");
                
            });
            modelBuilder.Entity<Enrollment>(entity => {
                // Indexes for faster search
                entity.HasIndex(e => e.CourseId, "IX_Enrollments_CourseId");
                entity.HasIndex(e => e.Status, "IX_Enrollments_Status");
                entity.HasIndex(e => e.StudentId, "IX_Enrollments_StudentId");

                entity.HasIndex(e => new { e.StudentId, e.CourseId },"UQ_Enrollments_StudentId_CourseId").IsUnique();
                entity.Property(e => e.CompletionDate).HasColumnType("datetime");

                entity.Property(e => e.EnrollmentDate).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
                entity.Property(e => e.FinalGrade).HasColumnType("decimal(5, 2)");
                entity.Property(e => e.ProgressPercent).HasColumnType("decimal(5, 2)");
                entity.Property(e => e.Status).HasMaxLength(20);
                entity.HasOne(d => d.Course).WithMany(p => p.Enrollment).HasForeignKey(d => d.CourseId).HasConstraintName("FK_Enrollments_Courses");

                // Many enrollments belong to one student
                entity.HasOne(d => d.Student).WithMany(p => p.Enrollment).HasForeignKey(d => d.StudentId).HasConstraintName("FK_Enrollments_Students");
            });

            modelBuilder.Entity<Instrcutor>(entity =>{
                // Self-reference manager relationship
                entity.HasKey(e => e.InstructorId);
                entity.HasIndex(e => e.ManagerId, "IX_Instructors_ManagerId");
                entity.Property(e => e.FirstName).HasMaxLength(50);
                entity.Property(e => e.LastName).HasMaxLength(50);
                entity.HasIndex(e => e.Email, "UQ_Instructors_Email").IsUnique();
                entity.Property(e => e.Email).HasMaxLength(150);

                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.Salary).HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.Manager).WithMany(p => p.InvertManage).HasForeignKey(d => d.ManagerId).HasConstraintName("FK_Instructors_Manager");
            });

            modelBuilder.Entity<Student>(entity => {
                entity.HasIndex(e => e.Status, "IX_Students_Status");
                entity.HasIndex(e => e.Email, "UQ_Students_Email").IsUnique();
                entity.Property(e => e.Email).HasMaxLength(150);
                entity.Property(e => e.FirstName).HasMaxLength(50);
                entity.Property(e => e.LastName).HasMaxLength(50);
                entity.Property(e => e.PhoneNumber).HasMaxLength(30);
                entity.Property(e => e.Status).HasMaxLength(20);

                entity.Property(e => e.RegisteredAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
            });

            modelBuilder.Entity<StudentProfile>(entity =>{
                entity.HasKey(e => e.StudentId);//PK      
                entity.Property(e => e.StudentId).ValueGeneratedNever();// Not auto increment
                entity.Property(e => e.Address).HasMaxLength(200);
                entity.Property(e => e.Bio).HasMaxLength(500);
                entity.Property(e => e.City).HasMaxLength(100);
                entity.Property(e => e.Country).HasMaxLength(100);
                entity.Property(e => e.LinkedInUrl).HasMaxLength(200);

                // One Student has one Profile
                entity.HasOne(d => d.Student).WithOne(p => p.StudentProfile).HasForeignKey<StudentProfile>(d => d.StudentId).HasConstraintName("FK_StudentProfiles_Students");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
