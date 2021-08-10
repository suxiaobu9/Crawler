using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Model.StockCrawler.Entity
{
    public partial class StockCrawlerContext : DbContext
    {
        public StockCrawlerContext()
        {
        }

        public StockCrawlerContext(DbContextOptions<StockCrawlerContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<DailyTransaction> DailyTransactions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("server=localhost,1433;database=StockCrawler;user=Stock;password=123456;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Company>(entity =>
            {
                entity.ToTable("Company");

                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .HasComment("公司代號");

                entity.Property(e => e.ChairmanOfBoard)
                    .HasMaxLength(100)
                    .HasComment("董事長");

                entity.Property(e => e.Email).HasComment("電子郵件信箱");

                entity.Property(e => e.EnglishAddress).HasComment("英文通訊地址");

                entity.Property(e => e.EnglishNickname)
                    .HasMaxLength(200)
                    .HasComment("英文簡稱");

                entity.Property(e => e.EstablishmentDate)
                    .HasColumnType("datetime")
                    .HasComment("成立日期");

                entity.Property(e => e.ForeignCompanyRegistration)
                    .HasMaxLength(50)
                    .HasComment("外國企業註冊地國");

                entity.Property(e => e.GeneralManager)
                    .HasMaxLength(100)
                    .HasComment("總經理");

                entity.Property(e => e.Industry)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasComment("產業別");

                entity.Property(e => e.ListingDate)
                    .HasColumnType("datetime")
                    .HasComment("上市日期");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasComment("公司名稱");

                entity.Property(e => e.Nickname)
                    .HasMaxLength(100)
                    .HasComment("公司簡稱");

                entity.Property(e => e.Type).HasComment("上市或上櫃");

                entity.Property(e => e.UniformCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("統一編號");

                entity.Property(e => e.Url)
                    .HasColumnName("URL")
                    .HasComment("網址");
            });

            modelBuilder.Entity<DailyTransaction>(entity =>
            {
                entity.ToTable("DailyTransaction");

                entity.Property(e => e.Id).HasComment("Id");

                entity.Property(e => e.ClosingPrice)
                    .HasColumnType("decimal(5, 2)")
                    .HasComment("收盤價");

                entity.Property(e => e.CompanyId)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasComment("公司Id");

                entity.Property(e => e.HighestPrice)
                    .HasColumnType("decimal(5, 2)")
                    .HasComment("最高價");

                entity.Property(e => e.LowestPrice)
                    .HasColumnType("decimal(5, 2)")
                    .HasComment("最低價");

                entity.Property(e => e.OpeningPrice)
                    .HasColumnType("decimal(5, 2)")
                    .HasComment("開盤價");

                entity.Property(e => e.TotalAmount)
                    .HasColumnType("decimal(18, 2)")
                    .HasComment("總交易金額");

                entity.Property(e => e.TotalStock).HasComment("總交易股數");

                entity.Property(e => e.TransactionNumbers).HasComment("成交筆數");

                entity.Property(e => e.UpsDowns)
                    .HasColumnType("decimal(5, 2)")
                    .HasComment("漲跌");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.DailyTransactions)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DailyTransaction_Company");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
