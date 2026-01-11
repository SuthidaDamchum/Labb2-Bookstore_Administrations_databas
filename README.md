# Develop a Relational Database App with Entity Framework 🛠️ 
# 🏬 Bookstore Administration

Bookstore Administration is a WPF application designed to manage inventory and data across multiple bookstores. It is built using Entity Framework Core, async/await, and a SQL Server database, and follows the MVVM pattern to ensure clean architecture and long-term maintainability. The application delivers a responsive user experience by utilizing ICommand-based UI interactions and asynchronous database operations.

## 📱✨ Features

* Manage book titles, authors, genres, publishers, and inventory
* Create, edit, and delete books and authors
* Asynchronous database operations using async/await
* MVVM architecture for clean UI–logic separation
* Secure storage of sensitive connection strings using JSON secrets
* WPF dialogs for adding and editing data
* Automatic UI updates via data binding and property notifications

## 💻 Setup Instructions

1. **Clone the repository:**
  * Go to GitHub and copy the repository URL.
  * Open Visual Studio 2022.
  * Select "Clone a repository" and paste the URL.

2. **Configure the database connection:**
   * The database connection string is stored in a secrets.json file for security.
   * In Visual Studio, right-click the project "BookStore_Infrastrcuture" and select "Manage User Secrets".
   *Note if there's no "Manage User Secrets" option, try to install "Windows Communication Foundation" package from the Visual studio installer:*
   <img width="795" height="281" alt="image" src="https://github.com/user-attachments/assets/9c332992-4ef9-4edc-b580-f411567cc617" />

   * Add your SQL Server connection string in the JSON file like this:
   *Note that the database doesn't need to exist, but will be created during step 3*
     ```json
     {
         "connectionString": ": "Initial Catalog=<your database name>;Database=<your database name>;Data Source=<your database server>;TrustServerCertificate=True;Integrated Security=True"
     }
     ```

3. **Apply database migrations:**
   * Open the Package Manager Console.
   * Set "BookStore_Presentation" as Start up project and set default project to "BookStore_Infrastrcuture"
   * Run: `Update-Database` to create the database.

5. **Build and run the app:**
   * Press F5 or click "Start" in Visual Studio.
 
## 📜 Usage

* When you start the app, the first page shows the inventory by store. Here you can see and adjust inventory, add new books, delete books, and change quantities.
* The second view is the Books Admin. Here you can see all books, add new books, edit, or remove them.
* The third view is the Authors Admin. Here you can see all authors, add, edit, or remove them. You can only delete an author if they are not linked to any book. To delete an author with books, you must first    delete those books.

## ⛓ How Relationships Are Used:

* Inventory by Store: 🗃️
Shows all books available in the selected store, using the relationship between Stores, Inventory, and Books.
* Add Book to Store: 📖
Lets you select a book from the catalog and add it to a store’s inventory.
* Book Management: ⚙️
When adding or editing a book, you select an author from the list.
* Author Management: ✍
You can only delete an author if they have no books. If an author has books, you must delete those books first.

## 🖼️ Screenshots
### Inventory by Store
![Bookstore Administration Screenshot](BookStore_Presentation/Assets/inventory.png)

### Books Administration
![Bookstore Administration Screenshot](BookStore_Presentation/Assets/books-admin.png)

### Authors Administration
![Bookstore Administration Screenshot](BookStore_Presentation/Assets/author-admin.png)

### Add / Edit Book Dialog
![Bookstore Administration Screenshot](BookStore_Presentation/Assets/add-edit-book.png).
