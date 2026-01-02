using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using BookStore_Domain;
using BookStore_Infrastrcuture.Data.Model;
using BookStore_Presentation.Command;
using BookStore_Presentation.Dialogs;
using BookStore_Presentation.Services;
using Microsoft.EntityFrameworkCore;
using static Microsoft.VisualStudio.Services.Graph.GraphResourceIds;


namespace BookStore_Presentation.ViewModels
{
    public class BooksAdminViewModel : ViewModelBase
    {

        private readonly BookSelectionService _selectionService;

        private readonly BookStoreContext _context;
        public ObservableCollection<BookAdminItem> Books { get; }
        public ICommand DeleteBookFromInventoryCommand { get; } 
        public ICommand CreateNewBookTitleCommand { get; }
        public BooksAdminViewModel(BookSelectionService selectionService)
        {
            _selectionService = selectionService;
            _context = new BookStoreContext();
            Books = new ObservableCollection<BookAdminItem>(LoadBooks());

            CreateNewBookTitleCommand = new DelegateCommand(_ => OpenAddBookDialog());

            DeleteBookFromInventoryCommand = new DelegateCommand(
             param =>
              {
              if (param is BookAdminItem book)
             {
              // Show confirmation dialog
              var result = MessageBox.Show(
                  $"Are you sure you want to delete the book '{book.Title}'?",
                  "Confirm Delete",
                  MessageBoxButton.YesNo,
                  MessageBoxImage.Warning
              );

              if (result == MessageBoxResult.Yes)
              {
                  // User confirmed, delete the book
                  DeleteBookFromInventory(book);

                  // Optionally reload the collection to reflect the database
                  ReloadBooks();
              }
          }
      },
                param => param is BookAdminItem // CanExecute: only if a book is selected
        );
   }



        public BooksAdminViewModel()
        {
        }

        // Shared SelectedBook property through service
        public BookAdminItem? SelectedBook
        {
            get => _selectionService.SelectedBook;
            set
            {
                _selectionService.SelectedBook = value;
                RaisePropertyChanged();
            }
        }



        private void OpenAddBookDialog()
        {
            var dialog = new AddNewTitle();
            if (dialog.ShowDialog() == true)
            {
                ReloadBooks();
            }
            
        }

        private void ReloadBooks()
        {
            Books.Clear();
            foreach (var book in LoadBooks())
            {
                Books.Add(book);
            }
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

        { "English", "Swedish", "Norwegian", "French", "Spanish", "Danish", "German" };
                         

        public void CreateNewBookTitle
         (
            string isbn13, string title, int? genreId,
            string language, decimal price, List<int> existingAuthorIds
         )

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
            RaisePropertyChanged(nameof(Books));
        }

        public void DeleteBookFromInventory(BookAdminItem bookItem)
        {
            if (bookItem == null) return;

            // Find the book in the database by ISBN

            var book = _context.Books
                 .Include(b => b.BookAuthors)   // include related authors for EF
                .FirstOrDefault(b => b.Isbn13 == bookItem.Isbn13);
            
            if ( book != null)
            // Remove related BookAuthors first to avoid FK constraint errors
            {
                if( book.BookAuthors != null && book.BookAuthors.Any())
                {

                     _context.BookAuthors.RemoveRange(book.BookAuthors);

                }
                _context.Books.Remove(book);
                _context.SaveChanges();

                Books.Remove(bookItem);

                // ✅ Replace Books.Remove with ReloadBooks to sync fully
                ReloadBooks();
                RaisePropertyChanged(nameof(Books));
            }
        }
    }
}    
        









