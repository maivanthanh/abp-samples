using Microsoft.EntityFrameworkCore;
using CreditFeeManagement.Models;

namespace CreditFeeManagement.Data
{
    public class CreditFeeDbContext : DbContext
    {
        public CreditFeeDbContext(DbContextOptions<CreditFeeDbContext> options) : base(options)
        {
        }

        public DbSet<StudentType> StudentTypes { get; set; }
        public DbSet<ModulesType> ModulesTypes { get; set; }
        public DbSet<CreditFee> CreditFees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // StudentType configuration
            modelBuilder.Entity<StudentType>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Code).HasMaxLength(50);
                entity.HasIndex(e => e.Code).IsUnique();
            });

            // ModulesType configuration
            modelBuilder.Entity<ModulesType>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Code).HasMaxLength(50);
                entity.HasIndex(e => e.Code).IsUnique();
            });

            // CreditFee configuration
            modelBuilder.Entity<CreditFee>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FeePerCredit).HasColumnType("decimal(18,0)");
                
                // Composite unique index
                entity.HasIndex(e => new { e.StudentTypeId, e.ModulesTypeId, e.StudyType })
                      .IsUnique()
                      .HasDatabaseName("IX_CreditFee_Unique_Combination");

                // Foreign key relationships
                entity.HasOne(e => e.StudentType)
                      .WithMany()
                      .HasForeignKey(e => e.StudentTypeId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ModulesType)
                      .WithMany()
                      .HasForeignKey(e => e.ModulesTypeId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed StudentTypes
            modelBuilder.Entity<StudentType>().HasData(
                new StudentType { Id = 1, Name = "Sinh viên chính quy", Code = "REGULAR", Description = "Sinh viên học chương trình chính quy", IsActive = true },
                new StudentType { Id = 2, Name = "Sinh viên liên thông", Code = "TRANSFER", Description = "Sinh viên liên thông từ cao đẳng", IsActive = true },
                new StudentType { Id = 3, Name = "Sinh viên tại chức", Code = "INSERVICE", Description = "Sinh viên học tại chức", IsActive = true },
                new StudentType { Id = 4, Name = "Sinh viên quốc tế", Code = "INTERNATIONAL", Description = "Sinh viên quốc tế", IsActive = true }
            );

            // Seed ModulesTypes
            modelBuilder.Entity<ModulesType>().HasData(
                new ModulesType { Id = 1, Name = "Khối kiến thức cơ sở", Code = "BASIC", Description = "Các môn học cơ sở", IsActive = true },
                new ModulesType { Id = 2, Name = "Khối kiến thức chuyên ngành", Code = "MAJOR", Description = "Các môn học chuyên ngành", IsActive = true },
                new ModulesType { Id = 3, Name = "Khối kiến thức bổ trợ", Code = "SUPPORT", Description = "Các môn học bổ trợ", IsActive = true },
                new ModulesType { Id = 4, Name = "Thực tập tốt nghiệp", Code = "INTERNSHIP", Description = "Thực tập và đồ án tốt nghiệp", IsActive = true },
                new ModulesType { Id = 5, Name = "Ngoại ngữ", Code = "LANGUAGE", Description = "Các môn ngoại ngữ", IsActive = true }
            );

            // Seed some sample CreditFees
            modelBuilder.Entity<CreditFee>().HasData(
                // Regular students
                new CreditFee { Id = 1, StudentTypeId = 1, ModulesTypeId = 1, StudyType = StudyType.HocLan1, FeePerCredit = 500000, CreatedDate = DateTime.Now },
                new CreditFee { Id = 2, StudentTypeId = 1, ModulesTypeId = 1, StudyType = StudyType.HocLan2, FeePerCredit = 750000, CreatedDate = DateTime.Now },
                new CreditFee { Id = 3, StudentTypeId = 1, ModulesTypeId = 1, StudyType = StudyType.HocCaiThien, FeePerCredit = 600000, CreatedDate = DateTime.Now },
                
                new CreditFee { Id = 4, StudentTypeId = 1, ModulesTypeId = 2, StudyType = StudyType.HocLan1, FeePerCredit = 600000, CreatedDate = DateTime.Now },
                new CreditFee { Id = 5, StudentTypeId = 1, ModulesTypeId = 2, StudyType = StudyType.HocLan2, FeePerCredit = 900000, CreatedDate = DateTime.Now },
                new CreditFee { Id = 6, StudentTypeId = 1, ModulesTypeId = 2, StudyType = StudyType.HocCaiThien, FeePerCredit = 720000, CreatedDate = DateTime.Now },

                // Transfer students
                new CreditFee { Id = 7, StudentTypeId = 2, ModulesTypeId = 1, StudyType = StudyType.HocLan1, FeePerCredit = 450000, CreatedDate = DateTime.Now },
                new CreditFee { Id = 8, StudentTypeId = 2, ModulesTypeId = 2, StudyType = StudyType.HocLan1, FeePerCredit = 550000, CreatedDate = DateTime.Now }
            );
        }
    }
}