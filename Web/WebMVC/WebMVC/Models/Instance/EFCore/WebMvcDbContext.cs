using Microsoft.EntityFrameworkCore;

namespace WebMVC.Models.Instance.EFCore
{
    public class WebMvcDbContext : DbContext
    {
        public WebMvcDbContext(DbContextOptions<WebMvcDbContext> options) : base(options) { }

        public DbSet<ContactInfoModel> ContactInfoModels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region ContactInfoModel
            modelBuilder.Entity<ContactInfoModel>()
                .Property(e => e.IsEnable)
                .HasDefaultValue(1);

            modelBuilder.Entity<ContactInfoModel>()
                .Property(e => e.CreateTime)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<ContactInfoModel>()
                .Property(e => e.RowVersion)
                .IsRowVersion();
            #endregion
        }
    }
}
