using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using BookStore_Domain;
using BookStore_Infrastrcuture.Data.Model;
using BookStore_Presentation.Models;
using BookStore_Presentation.ViewModels;

namespace BookStore_Presentation.Dialogs
{
    /// <summary>
    /// Interaction logic for AddNewTitle.xaml
    /// </summary>
    public partial class AddNewTitle : Window
    {
        private readonly BookStoreContext _context;
        private readonly BooksAdminViewModel _viewModel;
        private ObservableCollection<AuthorItem> _authors;

        // Receive existing ViewModel
        public AddNewTitle(BooksAdminViewModel viewModel)
        {
            InitializeComponent();

            _context = new BookStoreContext();

            _viewModel = viewModel; // use the existing ViewModel

            DataContext = _viewModel;


            GenreComboBox.ItemsSource = _context.Genres.ToList();


            _authors = new ObservableCollection<AuthorItem>(
                _context.Authors.Select(a => new AuthorItem
                {
                    AuthorId = a.AuthorId,
                    FullName = a.FirstName + " " + a.LastName
                }));



            AuthorsComboBox.ItemsSource = _authors;
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                MessageBox.Show("Title is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var isbn = IsbnTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(isbn) || !System.Text.RegularExpressions.Regex.IsMatch(isbn, @"^[0-9A-Za-z]+$"))
            {
                MessageBox.Show("ISBN is required and must be alphanumeric.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            var selectedAuthors = _authors
                .Where(a => a.IsSelected)
                .ToList();

            if (!selectedAuthors.Any())
            {
                MessageBox.Show("Please select at least one author.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            var language = LanguageComboBox.SelectedItem as string;
            if (string.IsNullOrEmpty(language))
            {
                MessageBox.Show("Please select a language.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            if (string.IsNullOrWhiteSpace(PriceTextBox.Text)
                            || !decimal.TryParse(PriceTextBox.Text, out var p))
            {
                MessageBox.Show("Price is required and must be a valid number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal price = p; // now guaranteed to be valid

            var newBook = new Book
            {
                Isbn13 = IsbnTextBox.Text,
                Title = TitleTextBox.Text,
                Language = language,   //here is where assign the selected language
                Price = price,
                GenreId = (GenreComboBox.SelectedItem as Genre)?.GenreId,
                BookAuthors = selectedAuthors.Select(a => new BookAuthor
                {
                    AuthorId = a.AuthorId,
                    BookIsbn13 = IsbnTextBox.Text,
                    Role = "Main Author"
                }).ToList()
            };

            // 5️⃣ Save to database
            _context.Books.Add(newBook);
            _context.SaveChanges();

            MessageBox.Show("Book saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
            Close();
        }

        private void AuthorCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            // Update the ComboBox text to show all selected authors
            AuthorsComboBox.Text = SelectedAuthorsText;
        }

        public string SelectedAuthorsText
        {
            get
            {
                return string.Join(", ", _authors.Where(a => a.IsSelected).Select(a => a.FullName));
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
       


    