using System.Windows;
using BookStore_Infrastrcuture.Data.Model;
using BookStore_Presentation.Models;
using BookStore_Presentation.Services;
using BookStore_Presentation.ViewModels;

namespace BookStore_Presentation.Dialogs
{
    /// <summary>
    /// Interaction logic for AddNewAuthorDialog.xaml
    /// </summary>
    public partial class AddNewAuthorDialog : Window
    {
        public CreateNewAuthorDto Author { get; private set; }

        private readonly AuthorService _authorService;


        public AddNewAuthorDialog()
        {
            InitializeComponent();

            Author = new CreateNewAuthorDto();
            _authorService = new AuthorService(new BookStoreContext());

            DataContext = Author;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!_authorService.IsValidAuthor(Author.FirstName, Author.LastName, Author.BirthDay?.ToString(), out var error))
            {
                MessageBox.Show(error, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!string.IsNullOrWhiteSpace(Author.BirthDay?.ToString()))
            {
                if (!DateOnly.TryParse(Author.BirthDay?.ToString(), out var date))
                {
                    MessageBox.Show("Invalid birth date. Use YYYY-MM-DD.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Author.BirthDay = date;
            }

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
