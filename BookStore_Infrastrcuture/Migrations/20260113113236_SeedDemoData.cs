using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookStore_Infrastrcuture.Migrations
{
    /// <inheritdoc />
    public partial class SeedDemoData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    author_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    first_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    last_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    birth_date = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Authors__86516BCF744ECEB0", x => x.author_id);
                });

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    isbn13 = table.Column<string>(type: "char(13)", unicode: false, fixedLength: true, maxLength: 13, nullable: false),
                    title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    language = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    price = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    publication_date = table.Column<DateOnly>(type: "date", nullable: true),
                    page_count = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Books__AA00666DB9607A12", x => x.isbn13);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    customer_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    firstname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    lastname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    city = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    postal_code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Customer__CD65CB85B58CA293", x => x.customer_id);
                });

            migrationBuilder.CreateTable(
                name: "Stores",
                columns: table => new
                {
                    store_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    store_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    city = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    postal_code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Stores__A2F2A30C0791EF0A", x => x.store_id);
                });

            migrationBuilder.CreateTable(
                name: "BookAuthor",
                columns: table => new
                {
                    BookIsbn13 = table.Column<string>(type: "char(13)", unicode: false, fixedLength: true, maxLength: 13, nullable: false),
                    author_id = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: true, defaultValue: "Main Author")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookAuthor", x => new { x.BookIsbn13, x.author_id });
                    table.ForeignKey(
                        name: "FK_BookAuthor_Authors",
                        column: x => x.author_id,
                        principalTable: "Authors",
                        principalColumn: "author_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BookAuthor_Books",
                        column: x => x.BookIsbn13,
                        principalTable: "Books",
                        principalColumn: "isbn13");
                });

            migrationBuilder.CreateTable(
                name: "Inventory",
                columns: table => new
                {
                    store_id = table.Column<int>(type: "int", nullable: false),
                    isbn13 = table.Column<string>(type: "char(13)", unicode: false, fixedLength: true, maxLength: 13, nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Inventor__6852A56A8D19E2ED", x => new { x.store_id, x.isbn13 });
                    table.ForeignKey(
                        name: "FK_Inventory_Books",
                        column: x => x.isbn13,
                        principalTable: "Books",
                        principalColumn: "isbn13");
                    table.ForeignKey(
                        name: "FK_Inventory_Stores",
                        column: x => x.store_id,
                        principalTable: "Stores",
                        principalColumn: "store_id");
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    order_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    customer_id = table.Column<int>(type: "int", nullable: false),
                    store_id = table.Column<int>(type: "int", nullable: false),
                    order_datetime = table.Column<DateTime>(type: "smalldatetime", nullable: false, defaultValueSql: "(getdate())"),
                    order_status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Pending")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Orders__4659622910AD0AB2", x => x.order_id);
                    table.ForeignKey(
                        name: "FK_Orders_Customers",
                        column: x => x.customer_id,
                        principalTable: "Customers",
                        principalColumn: "customer_id");
                    table.ForeignKey(
                        name: "FK__Orders__store_id__4BAC3F29",
                        column: x => x.store_id,
                        principalTable: "Stores",
                        principalColumn: "store_id");
                });

            migrationBuilder.CreateTable(
                name: "Order_Items",
                columns: table => new
                {
                    order_id = table.Column<int>(type: "int", nullable: false),
                    isbn13 = table.Column<string>(type: "char(13)", unicode: false, fixedLength: true, maxLength: 13, nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    price = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Order_It__8CF9644F46863B4B", x => new { x.order_id, x.isbn13 });
                    table.ForeignKey(
                        name: "FK__Order_Ite__isbn1__3B40CD36",
                        column: x => x.isbn13,
                        principalTable: "Books",
                        principalColumn: "isbn13");
                    table.ForeignKey(
                        name: "FK__Order_Ite__order__3A4CA8FD",
                        column: x => x.order_id,
                        principalTable: "Orders",
                        principalColumn: "order_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Authors",
                columns: new[] { "author_id", "birth_date", "first_name", "last_name" },
                values: new object[,]
                {
                    { 26, new DateOnly(1980, 2, 1), "James", "Clear" },
                    { 27, new DateOnly(1982, 6, 10), "Cal", "Newport" }
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "isbn13", "language", "page_count", "price", "publication_date", "title" },
                values: new object[,]
                {
                    { "9781111000001", "English", 320, 250m, null, "Atomic Habits" },
                    { "9781111000002", "English", 304, 300m, null, "Deep Work" },
                    { "9781111000003", "English", 304, 280m, null, "Digital Minimalism" }
                });

            migrationBuilder.InsertData(
                table: "Stores",
                columns: new[] { "store_id", "address", "city", "country", "postal_code", "store_name" },
                values: new object[,]
                {
                    { 1, "123 Main St", "Springfield", "USA", "12345", "Green Leaf Books" },
                    { 2, "456 Oak Ave", "Rivertown", "USA", "67890", "Sunrise Reads" },
                    { 3, "789 Pine Rd", "Lakeside", "USA", "54321", "Riverstone Books" }
                });

            migrationBuilder.InsertData(
                table: "BookAuthor",
                columns: new[] { "author_id", "BookIsbn13" },
                values: new object[] { 26, "9781111000001" });

            migrationBuilder.InsertData(
                table: "Inventory",
                columns: new[] { "isbn13", "store_id", "quantity" },
                values: new object[,]
                {
                    { "9781111000002", 1, 10 },
                    { "9781111000001", 2, 5 },
                    { "9781111000003", 3, 15 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookAuthor_author_id",
                table: "BookAuthor",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "UQ_Customers_Email",
                table: "Customers",
                column: "email",
                unique: true,
                filter: "[email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UQ_Customers_Phone",
                table: "Customers",
                column: "phone",
                unique: true,
                filter: "[phone] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_isbn13",
                table: "Inventory",
                column: "isbn13");

            migrationBuilder.CreateIndex(
                name: "IX_Order_Items_isbn13",
                table: "Order_Items",
                column: "isbn13");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_customer_id",
                table: "Orders",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_store_id",
                table: "Orders",
                column: "store_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookAuthor");

            migrationBuilder.DropTable(
                name: "Inventory");

            migrationBuilder.DropTable(
                name: "Order_Items");

            migrationBuilder.DropTable(
                name: "Authors");

            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Stores");
        }
    }
}
