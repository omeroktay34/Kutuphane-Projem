using Microsoft.EntityFrameworkCore;

namespace kutuphane.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<UserModel> Kullanicilar { get; set; }
        public DbSet<BookPageModel> BookPages { get; set; }

        public DbSet<BookModel> Kitaplar { get; set; }
        public DbSet<CategoryModel> Kategoriler { get; set; }
       public DbSet<UserBookProgressModel>  UserLastReadPages { get; set; }
       

public DbSet<SavedBook> SavedBooks { get; set; }
public DbSet<FavoriteBook> FavoriteBooks { get; set; }





        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<BookModel>()
                .HasMany(b => b.Pages)
                .WithOne(p => p.BookModel)
                .HasForeignKey(p => p.BookModelId);




            modelBuilder.Entity<CategoryModel>()
                .HasMany(c => c.SubCategories)
                .WithOne(c => c.ParentCategory)
                .HasForeignKey(c => c.ParentCategoryId);

            base.OnModelCreating(modelBuilder);






        }




    }
}
