using System.Windows;
using BookStore_Infrastrcuture.Data.Model;
using BookStore_Presentation.Services;
using BookStore_Presentation.ViewModels;

namespace BookStore_Presentation
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var context = new BookStoreContext();
            var authorService = new AuthorService(context);

            DataContext = new MainWindowViewModel(authorService);
        }
    }
}
