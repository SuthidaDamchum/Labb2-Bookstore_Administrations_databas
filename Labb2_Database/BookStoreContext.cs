using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BookStore_Domain;

public partial class BookStoreContext : DbContext
{
    public BookStoreContext()
    {
    }

    public BookStoreContext(DbContextOptions<BookStoreContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<BookAuthor> BookAuthors { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<OpenOrdersVsInventory> OpenOrdersVsInventories { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<Publisher> Publishers { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Store> Stores { get; set; }

    public virtual DbSet<TitlesPerAuthor> TitlesPerAuthors { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Initial Catalog=LABB1_Bokhandel.Testing;Integrated Security = True;TrustServerCertificate=True;Server SPN=locaohost");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.AuthorId).HasName("PK__Authors__86516BCF744ECEB0");

            entity.Property(e => e.AuthorId).HasColumnName("author_id");
            entity.Property(e => e.BirthDate).HasColumnName("birth_date");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Isbn13).HasName("PK__Books__AA00666DB9607A12");

            entity.Property(e => e.Isbn13)
                .HasMaxLength(13)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("isbn13");
            entity.Property(e => e.GenreId).HasColumnName("genre_id");
            entity.Property(e => e.Language)
                .HasMaxLength(20)
                .HasColumnName("language");
            entity.Property(e => e.PageCount).HasColumnName("page_count");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(8, 2)")
                .HasColumnName("price");
            entity.Property(e => e.PublicationDate).HasColumnName("publication_date");
            entity.Property(e => e.PublisherId).HasColumnName("publisher_id");
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .HasColumnName("title");

            entity.HasOne(d => d.Genre).WithMany(p => p.Books)
                .HasForeignKey(d => d.GenreId)
                .HasConstraintName("FK_Books_Genres");

            entity.HasOne(d => d.Publisher).WithMany(p => p.Books)
                .HasForeignKey(d => d.PublisherId)
                .HasConstraintName("FK_Books_Publisher");
        });

        modelBuilder.Entity<BookAuthor>(entity =>
        {
            entity.HasKey(e => new { e.BookIsbn13, e.AuthorId });

            entity.ToTable("BookAuthor");

            entity.Property(e => e.BookIsbn13)
                .HasMaxLength(13)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("BookISBN13");
            entity.Property(e => e.AuthorId).HasColumnName("author_id");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Main Author")
                .IsFixedLength();

            entity.HasOne(d => d.Author).WithMany(p => p.BookAuthors)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookAuthor_Authors");

            entity.HasOne(d => d.BookIsbn13Navigation).WithMany(p => p.BookAuthors)
                .HasForeignKey(d => d.BookIsbn13)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookAuthor_Books");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__CD65CB85B58CA293");

            entity.HasIndex(e => e.Email, "UQ_Customers_Email").IsUnique();

            entity.HasIndex(e => e.Phone, "UQ_Customers_Phone").IsUnique();

            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.Address)
                .HasMaxLength(100)
                .HasColumnName("address");
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .HasColumnName("city");
            entity.Property(e => e.Country)
                .HasMaxLength(50)
                .HasColumnName("country");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.Firstname)
                .HasMaxLength(50)
                .HasColumnName("firstname");
            entity.Property(e => e.Lastname)
                .HasMaxLength(50)
                .HasColumnName("lastname");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.PostalCode)
                .HasMaxLength(20)
                .HasColumnName("postal_code");
        });

      
        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.GenreId).HasName("PK__Genres__18428D427A7E6811");

            entity.Property(e => e.GenreId).HasColumnName("genre_id");
            entity.Property(e => e.GenreName)
                .HasMaxLength(30)
                .HasColumnName("genre_name");
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => new { e.StoreId, e.Isbn13 }).HasName("PK__Inventor__6852A56A8D19E2ED");

            entity.ToTable("Inventory");

            entity.Property(e => e.StoreId).HasColumnName("store_id");
            entity.Property(e => e.Isbn13)
                .HasMaxLength(13)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("isbn13");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Isbn13Navigation).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.Isbn13)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inventory_Books");

            entity.HasOne(d => d.Store).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.StoreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inventory_Stores");
        });

        modelBuilder.Entity<OpenOrdersVsInventory>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("OpenOrdersVsInventory");

            entity.Property(e => e.InventoryQuantity).HasColumnName("inventory_quantity");
            entity.Property(e => e.Isbn13)
                .HasMaxLength(13)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("isbn13");
            entity.Property(e => e.OrderedNotDelivered).HasColumnName("ordered_not_delivered");
            entity.Property(e => e.RemainingAfterOrders).HasColumnName("remaining_after_orders");
            entity.Property(e => e.StoreName)
                .HasMaxLength(50)
                .HasColumnName("store_name");
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .HasColumnName("title");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__4659622910AD0AB2");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.OrderDatetime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("smalldatetime")
                .HasColumnName("order_datetime");
            entity.Property(e => e.OrderStatus)
                .HasMaxLength(20)
                .HasDefaultValue("Pending")
                .HasColumnName("order_status");
            entity.Property(e => e.StoreId).HasColumnName("store_id");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Customers");

            entity.HasOne(d => d.Store).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StoreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orders__store_id__4BAC3F29");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => new { e.OrderId, e.Isbn13 }).HasName("PK__Order_It__8CF9644F46863B4B");

            entity.ToTable("Order_Items");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Isbn13)
                .HasMaxLength(13)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("isbn13");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Isbn13Navigation).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.Isbn13)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Order_Ite__isbn1__3B40CD36");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__Order_Ite__order__3A4CA8FD");
        });

        modelBuilder.Entity<Publisher>(entity =>
        {
            entity.HasKey(e => e.PublisherId).HasName("PK__Publishe__3263F29DE1B716F9");

            entity.ToTable("Publisher");

            entity.Property(e => e.PublisherId).HasColumnName("publisher_id");
            entity.Property(e => e.Address)
                .HasMaxLength(200)
                .HasColumnName("address");
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .HasColumnName("city");
            entity.Property(e => e.Country)
                .HasMaxLength(50)
                .HasColumnName("country");
            entity.Property(e => e.PublisherName)
                .HasMaxLength(100)
                .HasColumnName("publisher_name");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => new { e.CustomerId, e.Isbn13 }).HasName("PK_review");

            entity.ToTable("Review");

            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.Isbn13)
                .HasMaxLength(13)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("isbn13");
            entity.Property(e => e.Comment)
                .HasMaxLength(500)
                .HasColumnName("comment");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.ReviewDatetime)
                .HasColumnType("smalldatetime")
                .HasColumnName("review_datetime");

            entity.HasOne(d => d.Customer).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Review__customer__2EDAF651");

            entity.HasOne(d => d.Isbn13Navigation).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.Isbn13)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Review__isbn13__2FCF1A8A");
        });

        modelBuilder.Entity<Store>(entity =>
        {
            entity.HasKey(e => e.StoreId).HasName("PK__Stores__A2F2A30C0791EF0A");

            entity.Property(e => e.StoreId).HasColumnName("store_id");
            entity.Property(e => e.Address)
                .HasMaxLength(100)
                .HasColumnName("address");
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .HasColumnName("city");
            entity.Property(e => e.Country)
                .HasMaxLength(50)
                .HasColumnName("country");
            entity.Property(e => e.PostalCode)
                .HasMaxLength(20)
                .HasColumnName("postal_code");
            entity.Property(e => e.StoreName)
                .HasMaxLength(50)
                .HasColumnName("store_name");
        });

        modelBuilder.Entity<TitlesPerAuthor>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("TitlesPerAuthor");

            entity.Property(e => e.Age)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("age");
            entity.Property(e => e.InventoryValue)
                .HasMaxLength(44)
                .IsUnicode(false)
                .HasColumnName("inventory_value");
            entity.Property(e => e.Name)
                .HasMaxLength(101)
                .HasColumnName("name");
            entity.Property(e => e.Titles)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("titles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
