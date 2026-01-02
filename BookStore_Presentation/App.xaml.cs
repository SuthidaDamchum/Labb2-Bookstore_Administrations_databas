using System.Configuration;
using System.Data;
using System.Runtime.Serialization.DataContracts;
using System.Windows;
using BookStore_Presentation.Services;
using BookStore_Presentation.ViewModels;

namespace BookStore_Presentation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
          protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // ✅ Create the shared service
            var bookSelectionService = new BookSelectionService();

            // ✅ Create the ViewModels with the service

            var booksVm = new BooksAdminViewModel(bookSelectionService);
            var inventoryVm = new InventoryByStoreViewModel(bookSelectionService, booksVm); // pass shared instance
            // ✅ Create the main window and assign DataContexts

            var mainWindow = new MainWindow
                {
                   DataContext = inventoryVm  // or booksVm depending on your window
                };

         

        }
    }
}
