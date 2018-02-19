using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebShop.Models;

namespace WebShop.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Image>()
                  .HasOne(p => p.Article)
                  .WithMany(b => b.Images)
                  .HasForeignKey(p => p.ArticleId)
                  .HasConstraintName("ForeignKey_Image_Article");

            builder.Entity<Article>().
                HasOne(p => p.Group)
                .WithMany(b => b.Articles)
                .HasForeignKey(p => p.GroupId)
                .HasConstraintName("ForeignKey_Article_Group");

            builder.Entity<Item>()
                .HasOne(p => p.Article)
                .WithMany(b => b.Items)
                .HasForeignKey(p => p.ArticleId)
                .HasConstraintName("ForeignKey_Item_Article");

            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        public DbSet<WebShop.Models.Article> Article { get; set; }
        public DbSet<WebShop.Models.Image> Image { get; set; }
        public DbSet<WebShop.Models.Group> Group { get; set; }
        public DbSet<WebShop.Models.Item> Item { get; set; }
    }
}
