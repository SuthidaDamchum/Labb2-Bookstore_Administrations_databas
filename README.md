# Develop a Relational Database App with Entity Framework 

# 📚Bookstore Administration

Bookstore Administration is a WPF application that helps users manage inventory and data for several bookstores. The app uses Entity Framework Core, async methods, and a SQL Server database. Sensitive database connection information is stored in a JSON secrets file for security.

## Features
* View inventory for all stores
* Increase or decrease book quantity in each store
* Add new books to a store from the stock
* Remove books from a store’s inventory
* View all books, add new books, edit, or remove books
* View all authors, add new authors, edit, or remove authors
* Authors can only be deleted if they are not linked to any book
* Three main views: Inventory by Store, Books Admin, Authors Admin
* All database operations are asynchronous for a smooth user experience
* Uses service classes to handle business logic and data access
  
## Setup Instructions

1. **Clone the repository:**
  *• Go to GitHub and copy the repository URL.
  *• Open Visual Studio 2022.
  *• Select "Clone a repository" and paste the URL.

2. **Configure the database connection:**
   *• The database connection string is stored in a `secrets.json` file for security.
   *• In Visual Studio, right-click the project and select "Manage User Secrets".
   *• Add your SQL Server connection string in the JSON file like this:
     ```json
     {
       "ConnectionStrings": {
         "DefaultConnection": "Your SQL Server connection string here"
       }
     }
     ```

3. **Apply database migrations:**
   *• Open the Package Manager Console.
   *• Run: `Update-Database` to create the database.

4. **Build and run the app:**
   *• Press F5 or click "Start" in Visual Studio.

## Usage

*• When you start the app, the first page shows the inventory by store. Here you can see and adjust inventory, add new books, delete books, and change quantities.
*• The second view is the Books Admin. Here you can see all books, add new books, edit, or remove them.
*• The third view is the Authors Admin. Here you can see all authors, add, edit, or remove them. You can only delete an author if they are not linked to any book. To delete an author with books, you must first    delete those books.

## Requirements

• .NET 8 SDK
• SQL Server (LocalDB or full version)
• Visual Studio 2022

## How Relationships Are Used:

*🗃️ •	Inventory by Store:
Shows all books available in the selected store, using the relationship between Stores, Inventory, and Books.
*📖 •	Add Book to Store:
Lets you select a book from the catalog and add it to a store’s inventory.
*⚙️ •	Book Management:
When adding or editing a book, you select an author from the list.
*✍ •	Author Management:
You can only delete an author if they have no books. If an author has books, you must delete those books first.
