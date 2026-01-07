using System.Windows;
using BookStore_Infrastrcuture.Data.Model;
using BookStore_Presentation.Services;
using BookStore_Presentation.ViewModels;

namespace BookStore_Presentation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            var context = new BookStoreContext();
            var bookSelectionService = new BookSelectionService(context);
            var authorService = new AuthorService(context);

            // Pass them to MainWindowViewModel
            DataContext = new MainWindowViewModel(bookSelectionService, authorService);

        }
    }
}
