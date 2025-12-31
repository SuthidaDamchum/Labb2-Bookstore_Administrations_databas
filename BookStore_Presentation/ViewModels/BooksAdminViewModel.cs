using System.Collections.ObjectModel;
using BookStore_Infrastrcuture.Data.Model;
using Microsoft.EntityFrameworkCore;
using BookStore_Domain;
using System.Drawing.Printing;
using System.Windows.Input;
using BookStore_Presentation.Dialogs;
using BookStore_Presentation.Command;




namespace BookStore_Presentation.ViewModels
{
    public class BooksAdminViewModel : ViewModelBase
    {
        private readonly BookStoreContext _context;

        public ObservableCollection<BookAdminItem> Books { get; }

        public ICommand CreateNewTitleCommand { get; }


        public BooksAdminViewModel()
        {
            _context = new BookStoreContext();
            Books = new ObservableCollection<BookAdminItem>(LoadBooks());

            CreateNewTitleCommand = new DelegateCommand(_ => OpenAddBookDialog());


        }

        private void OpenAddBookDialog()
        {
            var dialog = new AddNewTitle();
            dialog.ShowDialog();
        }



        private List<BookAdminItem> LoadBooks()
        {
            return _context.Books
                .Include(b => b.Genre)
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .Select(b => new BookAdminItem
                {
                    Isbn13 = b.Isbn13,
                    Title = b.Title,
                    AuthorNames = string.Join(", ",
                    b.BookAuthors.Select(ba =>
                    ba.Author.FirstName + " " + ba.Author.LastName)),
                    Genre = b.Genre != null ? b.Genre.GenreName : "",
                    Language = b.Language,
                    Price = b.Price ?? 0m
                })
                .ToList();
        }

        public List<string> LanguageOptions { get; } = new List<string>
                 {
                   "English", "Swedish", "Norwegian", "French", "Spanish", "Danish", "German"
                 };
        public void CreateNewTitle(string isbn13, string title, int? genreId,
            string language, decimal price, List<int> existingAuthorIds)

        {
            var authors = _context.Authors
            .Where(a => existingAuthorIds.Contains(a.AuthorId))
            .ToList();

            if (!authors.Any())
                throw new Exception("Could not find any authors with the specified IDs.");

            var newBookTitle = new Book
            {
                Isbn13 = isbn13,
                Title = title,
                GenreId = genreId,
                Language = language,
                Price = price,
                BookAuthors = authors.Select(a => new BookAuthor
                {
                    AuthorId = a.AuthorId
                }).ToList()
            };

            _context.Books.Add(newBookTitle);
            _context.SaveChanges();



            string genreName = "";

            if (genreId != null)
            {
                var genre = _context.Genres.Find(genreId);
                if (genre != null)
                {
                    genreName = genre.GenreName;
                }
            }

            //var genreName = genreId != null
            //    ? _context.Genres.Find(genreId)?.GenreName ?? ""
            //    : "";


            

            Books.Add(new BookAdminItem

            {

                Isbn13 = newBookTitle.Isbn13,
                Title = newBookTitle.Title,
                AuthorNames = string.Join(", ", authors.Select(a => $"{a.FirstName} {a.LastName}")),
                Genre = newBookTitle.Genre != null ? newBookTitle.Genre.GenreName : "",
                Language = newBookTitle.Language,
                Price = newBookTitle.Price ?? 0m
            });

        }
    }
}

