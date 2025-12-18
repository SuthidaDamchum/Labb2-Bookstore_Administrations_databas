using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using BookStore_Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore_Infrastrcuture.Data.Model;

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

    public virtual DbSet<CustomerOrderdetail> CustomerOrderdetails { get; set; }

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

        new AuthorEntityTypeConfiguration().Configure(modelBuilder.Entity<Author>());
        new BookEntityTypeConfiguration().Configure(modelBuilder.Entity<Book>());
        new BookAuthorEntityTypeConfiguration().Configure(modelBuilder.Entity<BookAuthor>());
        new GenreEntityTypeConfiguration().Configure(modelBuilder.Entity<Genre>());
        new InventoryEntityTypeConfiguration().Configure(modelBuilder.Entity<Inventory>());
        new OpenOrdersVsInventoryEntityTypeConfiguration().Configure(modelBuilder.Entity<OpenOrdersVsInventory>());
        new OrderEntityTypeConfiguration().Configure(modelBuilder.Entity<Order>());
        new OrderItemEntityTypeConfiguration().Configure(modelBuilder.Entity<OrderItem>());
        new PublisherEntityTypeConfiguration().Configure(modelBuilder.Entity<Publisher>());
        new TitlesPerAuthorEntityTypeConfiguration().Configure(modelBuilder.Entity<TitlesPerAuthor>());
        new StoreEntityTypeConfiguration().Configure(modelBuilder.Entity<Store>());
        new ReviewEntityTypeConfiguration().Configure(modelBuilder.Entity<Review>());


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}


public class AuthorEntityTypeConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
    
            builder.HasKey(e => e.AuthorId).HasName("PK__Authors__86516BCF744ECEB0");

            builder.Property(e => e.AuthorId).HasColumnName("author_id");
            builder.Property(e => e.BirthDate).HasColumnName("birth_date");
            builder.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("first_name");
            builder.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");
   
    }
}

public class BookEntityTypeConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
     
            builder.HasKey(e => e.Isbn13).HasName("PK__Books__AA00666DB9607A12");

            builder.Property(e => e.Isbn13)
                .HasMaxLength(13)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("isbn13");
            builder.Property(e => e.GenreId).HasColumnName("genre_id");
            builder.Property(e => e.Language)
                .HasMaxLength(20)
                .HasColumnName("language");
            builder.Property(e => e.PageCount).HasColumnName("page_count");
            builder.Property(e => e.Price)
                .HasColumnType("decimal(8, 2)")
                .HasColumnName("price");
            builder.Property(e => e.PublicationDate).HasColumnName("publication_date");
            builder.Property(e => e.PublisherId).HasColumnName("publisher_id");
            builder.Property(e => e.Title)
                .HasMaxLength(50)
                .HasColumnName("title");

            builder.HasOne(d => d.Genre).WithMany(p => p.Books)
                .HasForeignKey(d => d.GenreId)
                .HasConstraintName("FK_Books_Genres");

            builder.HasOne(d => d.Publisher).WithMany(p => p.Books)
                .HasForeignKey(d => d.PublisherId)
                .HasConstraintName("FK_Books_Publisher");
    }
}

public class BookAuthorEntityTypeConfiguration : IEntityTypeConfiguration<BookAuthor>
{
    public void Configure(EntityTypeBuilder<BookAuthor> builder)
    {

            builder.HasKey(e => new { e.BookIsbn13, e.AuthorId });

            builder.ToTable("BookAuthor");

            builder.Property(e => e.BookIsbn13)
                .HasMaxLength(13)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("BookISBN13");
            builder.Property(e => e.AuthorId).HasColumnName("author_id");
            builder.Property(e => e.Role)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Main Author")
                .IsFixedLength();

            builder.HasOne(d => d.Author).WithMany(p => p.BookAuthors)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookAuthor_Authors");

            builder.HasOne(d => d.BookIsbn13Navigation).WithMany(p => p.BookAuthors)
                .HasForeignKey(d => d.BookIsbn13)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookAuthor_Books");
    }
}

public class GenreEntityTypeConfiguration : IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> builder)
    {

            builder.HasKey(e => e.GenreId).HasName("PK__Genres__18428D427A7E6811");

            builder.Property(e => e.GenreId).HasColumnName("genre_id");
            builder.Property(e => e.GenreName)
                .HasMaxLength(30)
                .HasColumnName("genre_name");
    }
}

public class CustomerEntityTypeConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
     
            builder.HasKey(e => e.CustomerId).HasName("PK__Customer__CD65CB85B58CA293");

            builder.HasIndex(e => e.Email, "UQ_Customers_Email").IsUnique();

            builder.HasIndex(e => e.Phone, "UQ_Customers_Phone").IsUnique();

            builder.Property(e => e.CustomerId).HasColumnName("customer_id");
            builder.Property(e => e.Address)
                .HasMaxLength(100)
                .HasColumnName("address");
            builder.Property(e => e.City)
                .HasMaxLength(50)
                .HasColumnName("city");
            builder.Property(e => e.Country)
                .HasMaxLength(50)
                .HasColumnName("country");
            builder.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            builder.Property(e => e.Firstname)
                .HasMaxLength(50)
                .HasColumnName("firstname");
            builder.Property(e => e.Lastname)
                .HasMaxLength(50)
                .HasColumnName("lastname");
            builder.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            builder.Property(e => e.PostalCode)
                .HasMaxLength(20)
                .HasColumnName("postal_code");
    }
}

public class InventoryEntityTypeConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
            builder.HasKey(e => new { e.StoreId, e.Isbn13 }).HasName("PK__Inventor__6852A56A8D19E2ED");

            builder.ToTable("Inventory");

            builder.Property(e => e.StoreId).HasColumnName("store_id");
            builder.Property(e => e.Isbn13)
                .HasMaxLength(13)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("isbn13");
            builder.Property(e => e.Quantity).HasColumnName("quantity");

            builder.HasOne(d => d.Isbn13Navigation).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.Isbn13)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inventory_Books");

            builder.HasOne(d => d.Store).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.StoreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inventory_Stores");
    }
}

public class OpenOrdersVsInventoryEntityTypeConfiguration : IEntityTypeConfiguration<OpenOrdersVsInventory>
{
    public void Configure(EntityTypeBuilder<OpenOrdersVsInventory> builder)
    {
      
            builder
                .HasNoKey()
                .ToView("OpenOrdersVsInventory");

            builder.Property(e => e.InventoryQuantity).HasColumnName("inventory_quantity");
            builder.Property(e => e.Isbn13)
                .HasMaxLength(13)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("isbn13");
            builder.Property(e => e.OrderedNotDelivered).HasColumnName("ordered_not_delivered");
            builder.Property(e => e.RemainingAfterOrders).HasColumnName("remaining_after_orders");
            builder.Property(e => e.StoreName)
                .HasMaxLength(50)
                .HasColumnName("store_name");
            builder.Property(e => e.Title)
                .HasMaxLength(50)
                .HasColumnName("title");
    }
}

public class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
    
            builder.HasKey(e => e.OrderId).HasName("PK__Orders__4659622910AD0AB2");

            builder.Property(e => e.OrderId).HasColumnName("order_id");
            builder.Property(e => e.CustomerId).HasColumnName("customer_id");
            builder.Property(e => e.OrderDatetime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("smalldatetime")
                .HasColumnName("order_datetime");
            builder.Property(e => e.OrderStatus)
                .HasMaxLength(20)
                .HasDefaultValue("Pending")
                .HasColumnName("order_status");
            builder.Property(e => e.StoreId).HasColumnName("store_id");

            builder.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Customers");

            builder.HasOne(d => d.Store).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StoreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orders__store_id__4BAC3F29");
    }
}

public class OrderItemEntityTypeConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
      
            builder.HasKey(e => new { e.OrderId, e.Isbn13 }).HasName("PK__Order_It__8CF9644F46863B4B");

            builder.ToTable("Order_Items");

            builder.Property(e => e.OrderId).HasColumnName("order_id");
            builder.Property(e => e.Isbn13)
                .HasMaxLength(13)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("isbn13");
            builder.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            builder.Property(e => e.Quantity).HasColumnName("quantity");

            builder.HasOne(d => d.Isbn13Navigation).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.Isbn13)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Order_Ite__isbn1__3B40CD36");

            builder.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__Order_Ite__order__3A4CA8FD");
    }
}

public class PublisherEntityTypeConfiguration : IEntityTypeConfiguration<Publisher>
{
    public void Configure(EntityTypeBuilder<Publisher> builder)
    {
            builder.HasKey(e => e.PublisherId).HasName("PK__Publishe__3263F29DE1B716F9");

            builder.ToTable("Publisher");

            builder.Property(e => e.PublisherId).HasColumnName("publisher_id");
            builder.Property(e => e.Address)
                .HasMaxLength(200)
                .HasColumnName("address");
            builder.Property(e => e.City)
                .HasMaxLength(50)
                .HasColumnName("city");
            builder.Property(e => e.Country)
                .HasMaxLength(50)
                .HasColumnName("country");
            builder.Property(e => e.PublisherName)
                .HasMaxLength(100)
                .HasColumnName("publisher_name");
    }
}

public class ReviewEntityTypeConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
            builder.HasKey(e => new { e.CustomerId, e.Isbn13 }).HasName("PK_review");

            builder.ToTable("Review");

            builder.Property(e => e.CustomerId).HasColumnName("customer_id");
            builder.Property(e => e.Isbn13)
                .HasMaxLength(13)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("isbn13");
            builder.Property(e => e.Comment)
                .HasMaxLength(500)
                .HasColumnName("comment");
            builder.Property(e => e.Rating).HasColumnName("rating");
            builder.Property(e => e.ReviewDatetime)
                .HasColumnType("smalldatetime")
                .HasColumnName("review_datetime");

            builder.HasOne(d => d.Customer).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Review__customer__2EDAF651");

            builder.HasOne(d => d.Isbn13Navigation).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.Isbn13)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Review__isbn13__2FCF1A8A");
    }
}

public class StoreEntityTypeConfiguration : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> builder)
    {
  
            builder.HasKey(e => e.StoreId).HasName("PK__Stores__A2F2A30C0791EF0A");

            builder.Property(e => e.StoreId).HasColumnName("store_id");
            builder.Property(e => e.Address)
                .HasMaxLength(100)
                .HasColumnName("address");
            builder.Property(e => e.City)
                .HasMaxLength(50)
                .HasColumnName("city");
            builder.Property(e => e.Country)
                .HasMaxLength(50)
                .HasColumnName("country");
            builder.Property(e => e.PostalCode)
                .HasMaxLength(20)
                .HasColumnName("postal_code");
            builder.Property(e => e.StoreName)
                .HasMaxLength(50)
                .HasColumnName("store_name");
    }
}

public class TitlesPerAuthorEntityTypeConfiguration : IEntityTypeConfiguration<TitlesPerAuthor>
{
    public void Configure(EntityTypeBuilder<TitlesPerAuthor> builder)
    {
      
            builder
                .HasNoKey()
                .ToView("TitlesPerAuthor");

            builder.Property(e => e.Age)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("age");
            builder.Property(e => e.InventoryValue)
                .HasMaxLength(44)
                .IsUnicode(false)
                .HasColumnName("inventory_value");
            builder.Property(e => e.Name)
                .HasMaxLength(101)
                .HasColumnName("name");
            builder.Property(e => e.Titles)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("titles");
 
    }
}
