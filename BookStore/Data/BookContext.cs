using System.Collections.Generic;
using BookStore.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Data;

public partial class BookContext : DbContext
{
    public BookContext()
    {
    }

    public BookContext(DbContextOptions<BookContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<Bookauthor> Bookauthors { get; set; }

    public virtual DbSet<Bookcondition> Bookconditions { get; set; }

    public virtual DbSet<Bookreview> Bookreviews { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<Permrole> Permroles { get; set; }

    public virtual DbSet<Publisher> Publishers { get; set; }

    public virtual DbSet<Purchaselog> Purchaselogs { get; set; }

    public virtual DbSet<Reviewer> Reviewers { get; set; }

    public virtual DbSet<Shoppingcart> Shoppingcarts { get; set; }

    public virtual DbSet<State> States { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.AuthorId).HasName("PK__author__8E2731D9E350936C");

            entity.ToTable("author");

            entity.Property(e => e.AuthorId)
                .ValueGeneratedNever()
                .HasColumnName("authorID");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Photo)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValueSql("(NULL)");
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Isbn).HasName("PK__book__447D36EB6BB6AF27");

            entity.ToTable("book");

            entity.HasIndex(e => e.Category, "IX_book_category");

            entity.HasIndex(e => e.PublisherId, "IX_book_publisherID");

            entity.Property(e => e.Isbn)
                .HasMaxLength(13)
                .IsUnicode(false)
                .HasColumnName("ISBN");
            entity.Property(e => e.Description).HasMaxLength(100);
            entity.Property(e => e.Edition)
                .HasMaxLength(30)
                                                .IsUnicode(false);
            entity.Property(e => e.PublisherId).HasColumnName("PublisherID");
            entity.Property(e => e.Title).HasMaxLength(70);

            entity.HasOne(d => d.CategoryNavigation).WithMany(p => p.Books)
                .HasForeignKey(d => d.Category)
                .HasConstraintName("FK_book_category");

            entity.HasOne(d => d.Publisher).WithMany(p => p.Books)
                .HasForeignKey(d => d.PublisherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_book_publisher");
        });

        modelBuilder.Entity<Bookauthor>(entity =>
        {
            entity.HasKey(e => new { e.Isbn, e.AuthorId }).HasName("PK__bookauth__1370992A4E95F0CB");

            entity.ToTable("bookauthor");

            entity.HasIndex(e => e.AuthorId, "IX_bookauthor_authorID");

            entity.Property(e => e.Isbn)
                .HasMaxLength(13)
                .IsUnicode(false)
                .HasColumnName("ISBN");
            entity.Property(e => e.AuthorId).HasColumnName("AuthorID");
            entity.Property(e => e.PrimaryAuthor)
                .HasMaxLength(1)
                .IsUnicode(false);

            entity.HasOne(d => d.Author).WithMany(p => p.Bookauthors)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_bookauthor_authorID");

            entity.HasOne(d => d.IsbnNavigation).WithMany(p => p.Bookauthors)
                .HasForeignKey(d => d.Isbn)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_bookauthor_ISBN");
        });

        modelBuilder.Entity<Bookcondition>(entity =>
        {
            entity.HasKey(e => e.Ranks).HasName("PK__bookcond__8784523ED4675200");

            entity.ToTable("bookcondition");

            entity.Property(e => e.Ranks).ValueGeneratedNever();
            entity.Property(e => e.Description).HasMaxLength(50);
            entity.Property(e => e.FullDescription).HasMaxLength(255);
            entity.Property(e => e.Price)
                .HasDefaultValue(30.00m)
                .HasColumnType("decimal(12, 2)");
        });

        modelBuilder.Entity<Bookreview>(entity =>
        {
            entity.HasKey(e => new { e.Isbn, e.ReviewerId }).HasName("PK__bookrevi__851C5A1096A472AE");

            entity.ToTable("bookreview");

            entity.HasIndex(e => e.ReviewerId, "IX_bookreview_ReviewerID");

            entity.Property(e => e.Isbn)
                .HasMaxLength(13)
                .IsUnicode(false)
                .HasColumnName("ISBN");
            entity.Property(e => e.ReviewerId).HasColumnName("ReviewerID");
            entity.Property(e => e.Comments).HasMaxLength(255);

            entity.HasOne(d => d.IsbnNavigation).WithMany(p => p.Bookreviews)
                .HasForeignKey(d => d.Isbn)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_bookreview_ISBN");

            entity.HasOne(d => d.Reviewer).WithMany(p => p.Bookreviews)
                .HasForeignKey(d => d.ReviewerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_bookreview_ReviewerID");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CatId).HasName("PK__category__6A1C8ADA1DDFD005");

            entity.ToTable("category");

            entity.Property(e => e.CatId)
                .ValueGeneratedNever()
                .HasColumnName("CatID");
            entity.Property(e => e.CatDescription).HasMaxLength(24);
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => e.InventoryId).HasName("PK__inventor__F5FDE6D3FCA30A74");

            entity.ToTable("inventory");

            entity.Property(e => e.InventoryId).HasColumnName("InventoryID");
            entity.Property(e => e.Isbn)
                .HasMaxLength(13)
                .IsUnicode(false)
                .HasColumnName("ISBN");
            entity.Property(e => e.Purchased).HasDefaultValue((byte)0);

            entity.HasOne(d => d.IsbnNavigation).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.Isbn)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("inventory_ISBN_fk");

            entity.HasOne(d => d.RanksNavigation).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.Ranks)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("inventory_rank_fk");
        });

        modelBuilder.Entity<Permrole>(entity =>
        {
            entity.HasKey(e => e.RoleNumber).HasName("PK__permrole__486BE7480F6C5901");

            entity.ToTable("permrole");

            entity.Property(e => e.RoleNumber).ValueGeneratedNever();
            entity.Property(e => e.PermRole1)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("PermRole");
        });

        modelBuilder.Entity<Publisher>(entity =>
        {
            entity.HasKey(e => e.PublisherId).HasName("PK__publishe__4C657E4B2E549AFA");

            entity.ToTable("publisher");

            entity.HasIndex(e => e.StateCode, "IX_publisher_statecode");

            entity.Property(e => e.PublisherId)
                .ValueGeneratedNever()
                .HasColumnName("PublisherID");
            entity.Property(e => e.City).HasMaxLength(30);
            entity.Property(e => e.Name).HasMaxLength(100);;
            entity.Property(e => e.StateCode)
                .HasMaxLength(2)
                .IsUnicode(false);

            entity.HasOne(d => d.StateCodeNavigation).WithMany(p => p.Publishers)
                .HasForeignKey(d => d.StateCode)
                .HasConstraintName("FK_publisher_statecode");
        });

        modelBuilder.Entity<Purchaselog>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.InventoryId }).HasName("PK__purchase__38D712C12EB63ED1");

            entity.ToTable("purchaselog");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.InventoryId).HasColumnName("InventoryID");

            entity.HasOne(d => d.Inventory).WithMany()
                .HasForeignKey(d => d.InventoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("purchaselog_inventory_fk");

            entity.HasOne(d => d.User).WithMany(p => p.Purchaselogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("purchaselog_user_fk");
        });

        modelBuilder.Entity<Reviewer>(entity =>
        {
            entity.HasKey(e => e.ReviewerId).HasName("PK__reviewer__1616CFBDE3FF28BF");

            entity.ToTable("reviewer");

            entity.Property(e => e.ReviewerId)
                .ValueGeneratedNever()
                .HasColumnName("ReviewerID");
            entity.Property(e => e.EmployedBy).HasMaxLength(30);
            entity.Property(e => e.Name).HasMaxLength(20);
        });

        modelBuilder.Entity<Shoppingcart>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.Isbn }).HasName("PK__shopping__B3CF1FC2E708D2BE");

            entity.ToTable("shoppingcart");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Isbn)
                .HasMaxLength(13)
                .IsUnicode(false)
                .HasColumnName("ISBN");

            entity.HasOne(d => d.IsbnNavigation).WithMany(p => p.Shoppingcarts)
                .HasForeignKey(d => d.Isbn)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("shoppingcart_user_fk");
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.HasKey(e => e.StateCode).HasName("PK__state__D515E98B05465646");

            entity.ToTable("state");

            entity.Property(e => e.StateCode)
                .HasMaxLength(2)
                .IsUnicode(false);
            entity.Property(e => e.StateName).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__user__1788CCAC7B392015");

            entity.ToTable("user");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.FirstName)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(14)
                .IsUnicode(false);
            entity.Property(e => e.RoleNumber).HasDefaultValue(1);
            entity.Property(e => e.UserName)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.HasOne(d => d.RoleNumberNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleNumber)
                .HasConstraintName("FK_user_RoleNumber");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
