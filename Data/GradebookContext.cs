using Microsoft.EntityFrameworkCore;
using GradebookApp.Models;
using dotenv.net;


namespace GradebookApp.Data
{
    public class GradebookContext : DbContext
    {
        public DbSet<GradeRecord> GradeRecords { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
			DotEnv.Load();
          
            var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
            var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
            var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "gradebook";
            var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "postgres";
            var dbPass = Environment.GetEnvironmentVariable("DB_PASS") ?? "postgres";

            var connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPass}";

            optionsBuilder.UseNpgsql(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<GradeRecord>(entity =>
            {
                entity.ToTable("grade_records");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.StudentName).HasColumnName("student_name");
                entity.Property(e => e.SubjectName).HasColumnName("subject_name");
                entity.Property(e => e.Mark).HasColumnName("mark").IsRequired();
    
                entity.HasIndex(e => new { e.StudentName, e.SubjectName });
                entity.HasIndex(e => e.SubjectName);
            });
        }
    }
}
