using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BookStore_Domain;
using BookStore_Infrastrcuture.Data.Model;
using BookStore_Presentation.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BookStore_Presentation.Dialogs
{
    /// <summary>
    /// Interaction logic for AddNewTitle.xaml
    /// </summary>
    public partial class AddNewTitle : Window
    {
        private readonly BookStoreContext _context;

        public AddNewTitle()
        {

            InitializeComponent();
            _context = new BookStoreContext();

            DataContext = new BooksAdminViewModel();


            GenreComboBox.ItemsSource = _context.Genres.ToList();

            AuthorsListBox.ItemsSource = _context.Authors

                .Select(a => new AuthorItem
                {
                    AuthorId = a.AuthorId,
                    FullName = a.FirstName + " " + a.LastName
                })

                .ToList();


            AuthorsListBox.SelectionMode = SelectionMode.Multiple;
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
         
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text) || string.IsNullOrWhiteSpace(IsbnTextBox.Text))
            {
                MessageBox.Show("Title and ISBN are required.");
                return;
            }

        
            var selectedAuthors = AuthorsListBox.SelectedItems.Cast<AuthorItem>().ToList();
            if (!selectedAuthors.Any())
            {
                MessageBox.Show("Please select at least one author.");
                return;
            }


            var language = LanguageComboBox.SelectedItem as string;
            if (string.IsNullOrEmpty(language))
            {
                MessageBox.Show("Select a language.");
                return;
            }

 
            var newBook = new Book
            {
                Isbn13 = IsbnTextBox.Text,
                Title = TitleTextBox.Text,
                Language = language,   //here is where assign the selected language
                Price = decimal.TryParse(PriceTextBox.Text, out var p) ? p : null,
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

            MessageBox.Show("Book saved successfully!");
            DialogResult = true;
        }
    }
}