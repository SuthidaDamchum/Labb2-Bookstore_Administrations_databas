using System.Windows;
using BookStore_Infrastrcuture.Data.Model;
using BookStore_Presentation.Services;
using BookStore_Presentation.ViewModels;

namespace BookStore_Presentation
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var bookSelectionService = new BookSelectionService();
            var authorService = new AuthorService(new BookStoreContext());

         
            var mainWindowViewModel = new MainWindowViewModel(bookSelectionService, authorService);

            var mainWindow = new MainWindow
            {
                DataContext = mainWindowViewModel
            };

        }
    }
}
