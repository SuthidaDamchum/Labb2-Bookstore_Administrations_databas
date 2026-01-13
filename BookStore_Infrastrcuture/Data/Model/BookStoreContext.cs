using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection.Metadata.Ecma335;
using BookStore_Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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


    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<Store> Stores { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var config = new ConfigurationBuilder().AddUserSecrets<BookStoreContext>().Build();
        var connectionString = config["connectionString"];
        optionsBuilder.UseSqlServer(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new CustomerEntityTypeConfiguration().Configure(modelBuilder.Entity<Customer>());
        new AuthorEntityTypeConfiguration().Configure(modelBuilder.Entity<Author>());
        new BookEntityTypeConfiguration().Configure(modelBuilder.Entity<Book>());
        new BookAuthorEntityTypeConfiguration().Configure(modelBuilder.Entity<BookAuthor>());
        new InventoryEntityTypeConfiguration().Configure(modelBuilder.Entity<Inventory>());
        new OrderEntityTypeConfiguration().Configure(modelBuilder.Entity<Order>());
        new OrderItemEntityTypeConfiguration().Configure(modelBuilder.Entity<OrderItem>());
        new StoreEntityTypeConfiguration().Configure(modelBuilder.Entity<Store>());

        modelBuilder.Entity<Store>().HasData(
            new Store { StoreId = 1, StoreName = "Green Leaf Books", Address = "123 Main St", City = "Springfield", PostalCode = "12345", Country = "USA" },
            new Store { StoreId = 2, StoreName = "Sunrise Reads", Address = "456 Oak Ave", City = "Rivertown", PostalCode = "67890", Country = "USA" },
            new Store { StoreId = 3, StoreName = "Riverstone Books", Address = "789 Pine Rd", City = "Lakeside", PostalCode = "54321", Country = "USA" }
            );


        modelBuilder.Entity<Author>().HasData(
           new Author
           {
               AuthorId = 26,
               FirstName = "James",
               LastName = "Clear",
               BirthDay = new DateOnly(1980, 02, 01)
           },
           new Author
           {
               AuthorId = 27,
               FirstName = "Cal",
               LastName = "Newport",
               BirthDay = new DateOnly(1982, 06, 10)
           }

       );

            modelBuilder.Entity<Book>().HasData(
        new Book
        {
            Isbn13 = "9781111000001",
            Title = "Atomic Habits",
            Language = "English",
            Price = 250,
            PageCount = 320
        },
           new Book
           {
               Isbn13 = "9781111000002",
               Title = "Deep Work",
               Language = "English",
               Price = 300,
               PageCount = 304
           },

            new Book
            {
                Isbn13 = "9781111000003",
                Title = "Digital Minimalism",
                Language = "English",
                Price = 280,
                PageCount = 304
            }


    );

        modelBuilder.Entity<BookAuthor>().HasData(
      new BookAuthor
      {
          BookIsbn13 = "9781111000001",
          AuthorId = 26
      }
     );

        modelBuilder.Entity<Inventory>().HasData(
            new Inventory
            {
                StoreId = 1,
                Isbn13 = "9781111000002", 
                Quantity = 10
            },


                  new Inventory
                  {
                      StoreId = 2,
                      Isbn13 = "9781111000001",
                      Quantity = 5
                  },

                   new Inventory
                   {
                       StoreId = 3,
                       Isbn13 = "9781111000003",
                       Quantity = 15
                   }

        );

        OnModelCreatingPartial(modelBuilder);
    }



    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}