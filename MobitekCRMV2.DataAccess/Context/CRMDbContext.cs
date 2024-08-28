using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MobitekCRMV2.Entity.Entities;

namespace MobitekCRMV2.DataAccess.Context
{
    /// <summary>
    /// DbContext, AspnetIdentity kullanıldığı için IdentityDbContext class'ından miras alıyor
    /// IdentityDbContext'e user ve role tabloları için hangi modelleri kullanacağımızı generic olarak geçiyoruz.
    /// IdentityDbContext'in diğer overload'larına ihtiyacımız bu versiyon işimizi görür.
    /// </summary>
    public class CRMDbContext : IdentityDbContext<User, Role, string>
    {
        public CRMDbContext(DbContextOptions<CRMDbContext> options) : base(options)
        {
        }
        public DbSet<Keyword> Keywords { get; set; }
        public DbSet<KeywordResponse> KeywordResponses { get; set; }
        public DbSet<NewsSite> NewsSites { get; set; }
        public DbSet<Platform> Platforms { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Promotion> Customer { get; set; }
        public DbSet<Domain> Domains { get; set; }
        public DbSet<BackLink> BackLinks { get; set; }
        public DbSet<UserInfo> UserInfos { get; set; }
        public DbSet<KeywordInfo> KeywordInfos { get; set; }
        public DbSet<KeywordValue> KeywordValues { get; set; }
        public DbSet<LogMessage> LogMessages { get; set; }
        public DbSet<Todo> Todos { get; set; }
        public DbSet<TodoGroup> TodoGroups { get; set; }
        public DbSet<TemplateItem> TemplateItem { get; set; }
        public DbSet<SmPlatform> SmPlatforms { get; set; }
        public DbSet<ContentBudget> ContentBudgets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            ///Summary
            ///Tabloların arasındaki bazı ilişkilendirmeler aşağıda işlenmiştir.
            ///Summary

            ///SEOExpert (User) - SEOProjects (Project) 1-M



            ///NewsSite - Promotion 1-1
            modelBuilder.Entity<NewsSite>()
                .HasOne(a => a.Promotion)
                .WithMany(b => b.NewsSites);

            ///Project - (Expert) User M-1
            modelBuilder.Entity<Project>()
                .HasOne(a => a.Expert)
                .WithMany(b => b.ExpertProjects);

            ///Project - (Customer) User M-1
            modelBuilder.Entity<Project>()
                .HasOne(a => a.CustomerTypeUser)
                .WithMany(b => b.CustomerProjects);

            modelBuilder.Entity<Project>()
                .HasOne(a => a.Platform)
                .WithMany(b => b.Projects)
                .HasForeignKey(c => c.PlatformId);

            modelBuilder.Entity<KeywordValue>()
               .HasOne(a => a.Keyword)
               .WithMany(b => b.KeywordValues)
               .HasForeignKey(c => c.KeywordId);

            modelBuilder.Entity<KeywordValue>()
               .Property(e => e.CreatedDate)
               .HasColumnType("date");

            modelBuilder.Entity<KeywordValue>()
              .Property(e => e.UpdatedDate)
              .HasColumnType("date");

            modelBuilder.Entity<Domain>()
            .HasOne(b => b.Project)
            .WithOne(i => i.Domain)
            .HasForeignKey<Project>(b => b.DomainId);

            modelBuilder.Entity<NewsSite>()
             .HasIndex(u => u.Name)
             .IsUnique();

            modelBuilder.Entity<UserInfo>()
            .Property(u => u.Salary)
            .HasColumnType("decimal(18, 2)");


            base.OnModelCreating(modelBuilder);

        }
    }
}
