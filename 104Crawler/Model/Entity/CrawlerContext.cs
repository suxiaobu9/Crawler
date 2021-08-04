using System;
using _104Crawler.Model.Appsettings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;

#nullable disable

namespace _104Crawler.Model.Entity
{
    public partial class CrawlerContext : DbContext
    {
        private readonly DbConnections _dbConnections;
        public CrawlerContext(IOptions<DbConnections> dbConnextions)
        {
            _dbConnections = dbConnextions.Value;
        }

        public CrawlerContext(DbContextOptions<CrawlerContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<Vacancy> Vacancies { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_dbConnections.Default);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasKey(e => e.No)
                    .HasName("PK_Company_1");

                entity.ToTable("Company");

                entity.HasIndex(e => e.No, "IX_No_Company");

                entity.Property(e => e.No)
                    .HasMaxLength(100)
                    .HasComment("公司編號");

                entity.Property(e => e.LatestCheckDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("最後確認資料時間");

                entity.Property(e => e.Link)
                    .IsRequired()
                    .HasComment("公司連結");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasComment("公司名稱");

                entity.Property(e => e.SourceFrom)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasComment("資料來源");

                entity.Property(e => e.Welfare).HasComment("福利");
            });

            modelBuilder.Entity<Vacancy>(entity =>
            {
                entity.HasKey(e => e.No)
                    .HasName("PK_Vacancy_1");

                entity.ToTable("Vacancy");

                entity.HasIndex(e => new { e.IsDelete, e.CompanyNo }, "IX_CompId_Vacancy_1");

                entity.HasIndex(e => new { e.IsDelete, e.No }, "IX_No_Vacancy");

                entity.Property(e => e.No)
                    .HasMaxLength(100)
                    .HasComment("職缺編號");

                entity.Property(e => e.AppearDate)
                    .HasColumnType("datetime")
                    .HasComment("最後更新日期");

                entity.Property(e => e.CompanyNo)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasComment("公司Id");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("系統建立日期");

                entity.Property(e => e.Description).HasComment("職缺描述");

                entity.Property(e => e.IsDelete).HasComment("是否刪除");

                entity.Property(e => e.Link)
                    .IsRequired()
                    .HasComment("職缺連結");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasComment("職缺名稱");

                entity.Property(e => e.Required).HasComment("條件要求");

                entity.Property(e => e.SalaryMax).HasComment("高薪");

                entity.Property(e => e.SalaryMin).HasComment("低薪");

                entity.Property(e => e.Seniority).HasComment("資歷");

                entity.HasOne(d => d.CompanyNoNavigation)
                    .WithMany(p => p.Vacancies)
                    .HasForeignKey(d => d.CompanyNo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Vacancy_Company");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
