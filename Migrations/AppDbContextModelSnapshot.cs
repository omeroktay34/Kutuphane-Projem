﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using kutuphane.Models;

#nullable disable

namespace kutuphane.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.5");

            modelBuilder.Entity("kutuphane.Models.BookModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("CategoryId")
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("CoverImage")
                        .HasColumnType("BLOB");

                    b.Property<int>("CreatedByUserId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PageCount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("CreatedByUserId");

                    b.ToTable("Kitaplar");
                });

            modelBuilder.Entity("kutuphane.Models.BookPageModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("BookModelId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("PageNumber")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("BookModelId");

                    b.ToTable("BookPages");
                });

            modelBuilder.Entity("kutuphane.Models.CategoryModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("ParentCategoryId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ParentCategoryId");

                    b.ToTable("Kategoriler");
                });

            modelBuilder.Entity("kutuphane.Models.FavoriteBook", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("BookId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("BookId");

                    b.HasIndex("UserId");

                    b.ToTable("FavoriteBooks");
                });

            modelBuilder.Entity("kutuphane.Models.SavedBook", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("BookId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("BookId");

                    b.HasIndex("UserId");

                    b.ToTable("SavedBooks");
                });

            modelBuilder.Entity("kutuphane.Models.UserBookProgressModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("BookId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PageNumber")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("BookId");

                    b.HasIndex("UserId");

                    b.ToTable("UserLastReadPages");
                });

            modelBuilder.Entity("kutuphane.Models.UserModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Bio")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("FontFamily")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("FontSize")
                        .HasColumnType("INTEGER");

                    b.Property<string>("FullName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Language")
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("ProfileImage")
                        .HasColumnType("BLOB");

                    b.Property<bool>("RememberMe")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Theme")
                        .HasColumnType("TEXT");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Kullanicilar");
                });

            modelBuilder.Entity("kutuphane.Models.BookModel", b =>
                {
                    b.HasOne("kutuphane.Models.CategoryModel", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("kutuphane.Models.UserModel", "CreatedByUser")
                        .WithMany()
                        .HasForeignKey("CreatedByUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("CreatedByUser");
                });

            modelBuilder.Entity("kutuphane.Models.BookPageModel", b =>
                {
                    b.HasOne("kutuphane.Models.BookModel", "BookModel")
                        .WithMany("Pages")
                        .HasForeignKey("BookModelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BookModel");
                });

            modelBuilder.Entity("kutuphane.Models.CategoryModel", b =>
                {
                    b.HasOne("kutuphane.Models.CategoryModel", "ParentCategory")
                        .WithMany("SubCategories")
                        .HasForeignKey("ParentCategoryId");

                    b.Navigation("ParentCategory");
                });

            modelBuilder.Entity("kutuphane.Models.FavoriteBook", b =>
                {
                    b.HasOne("kutuphane.Models.BookModel", "Book")
                        .WithMany()
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("kutuphane.Models.UserModel", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Book");

                    b.Navigation("User");
                });

            modelBuilder.Entity("kutuphane.Models.SavedBook", b =>
                {
                    b.HasOne("kutuphane.Models.BookModel", "Book")
                        .WithMany()
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("kutuphane.Models.UserModel", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Book");

                    b.Navigation("User");
                });

            modelBuilder.Entity("kutuphane.Models.UserBookProgressModel", b =>
                {
                    b.HasOne("kutuphane.Models.BookModel", "Book")
                        .WithMany()
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("kutuphane.Models.UserModel", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Book");

                    b.Navigation("User");
                });

            modelBuilder.Entity("kutuphane.Models.BookModel", b =>
                {
                    b.Navigation("Pages");
                });

            modelBuilder.Entity("kutuphane.Models.CategoryModel", b =>
                {
                    b.Navigation("SubCategories");
                });
#pragma warning restore 612, 618
        }
    }
}
