using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using BookStore_Domain;
using BookStore_Infrastrcuture.Data.Model;
using BookStore_Presentation.Command;
using BookStore_Presentation.Dialogs;
using BookStore_Presentation.Models;
using BookStore_Presentation.Services;
using Microsoft.EntityFrameworkCore;

namespace BookStore_Presentation.ViewModels
{
    public class BooksAdminViewModel : ViewModelBase
    {

        private readonly BookSelectionService _selectionService;

        private readonly AuthorService _authorService = new AuthorService(new BookStoreContext());

        private readonly BookStoreContext _context;
        public ObservableCollection<BookAdminItem> Books { get; }
        public ICommand DeleteBookFromInventoryCommand { get; }
        public ICommand CreateNewBookTitleCommand { get; }
        public ICommand EditNewBookTitleCommand { get; }
        public ICommand CreateNewAuthorCommand { get; }


        public BooksAdminViewModel(BookSelectionService selectionService, AuthorService authorService)
        {

            _authorService = authorService;
            _selectionService = selectionService;
            _context = new BookStoreContext();

            Books = new ObservableCollection<BookAdminItem>(LoadBooks());

            CreateNewBookTitleCommand = new DelegateCommand(_ => OpenAddBookDialog());

            CreateNewAuthorCommand = new DelegateCommand(_ => OpenAddNewAuthorDialog());


            //EditNewBookTitleCommand = new Delegate(_ => EditNewBookTitle(), _ => SelectedBook != null);

            DeleteBookFromInventoryCommand = new DelegateCommand(
               param =>
                {
                    if (param is BookAdminItem book)
                    {

                        var result = MessageBox.Show(
                          $"Are you sure you want to delete the book '{book.Title}'?",
                          "Confirm Delete",
                          MessageBoxButton.YesNo,
                          MessageBoxImage.Warning
                      );

                        if (result == MessageBoxResult.Yes)
                        {

                            DeleteBookFromInventory(book);


                            ReloadBooks();
                        }
                    }
                },
                  param => param is BookAdminItem //CanExecute: only if a book is selected
          );
        }

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
        
            var addNewTitleViewModel = new AddNewTitleViewModel();
            var dialog = new AddNewTitleDailog(addNewTitleViewModel); 

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
                .AsNoTracking()
           .Include(b => b.Genre)
           .Include(b => b.Publisher)
           .Include(b => b.BookAuthors)
               .ThenInclude(ba => ba.Author)
           .Select(b => new BookAdminItem
           {
               Isbn13 = b.Isbn13,
               Title = b.Title,

               AuthorNames = string.Join(", ",
               b.BookAuthors.Select(ba =>
               ba.Author.FirstName + " " + ba.Author.LastName)),

               GenreName = b.Genre != null ? b.Genre.GenreName : string.Empty,

               Language = b.Language,
               Price = b.Price ?? 0m,
               PublicationDate = b.PublicationDate,
               PageCount = b.PageCount,

               PublisherName = b.Publisher != null ? b.Publisher.PublisherName : "",

           })
           .ToList();
        }


        public List<string> LanguageOptions { get; } = new List<string>

        { "English", "Swedish", "Norwegian", "French", "Spanish", "Danish", "German" };


        public void CreateNewBookTitle
         (
            string isbn13, string title, int? genreId,
            string language, decimal price, DateOnly? publicationdate, int pagecount, int? publisherId, List<int> existingAuthorIds
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
                PublicationDate = publicationdate,
                PageCount = pagecount,
                PublisherId = publisherId,


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

            string publisherName = "";
            if (publisherId != null)
            {
                var publisher = _context.Publishers.Find(publisherId);
                if (publisher != null)
                {
                    publisherName = publisher.PublisherName;
                }
            }


            Books.Add(new BookAdminItem
            {
                Isbn13 = newBookTitle.Isbn13,
                Title = newBookTitle.Title,
                AuthorNames = string.Join(", ", authors.Select(a => $"{a.FirstName} {a.LastName}")),
                GenreName = genreName,
                Language = newBookTitle.Language,
                Price = newBookTitle.Price ?? 0m,
                PublicationDate = newBookTitle.PublicationDate,
                PageCount = newBookTitle.PageCount,
                PublisherName = publisherName

            });
            RaisePropertyChanged(nameof(Books));
        }

            private void EditNewBookTitle()
            {
                if (SelectedBook == null) return;

            }

            public void DeleteBookFromInventory(BookAdminItem bookItem)
            {
                if (bookItem == null) return;



            var book = _context.Books
                 .Include(b => b.BookAuthors)   //include related authors for EF
                .FirstOrDefault(b => b.Isbn13 == bookItem.Isbn13);

            if (book != null)

            {
                if (book.BookAuthors != null && book.BookAuthors.Any())
                {

                    _context.BookAuthors.RemoveRange(book.BookAuthors);

                }
                _context.Books.Remove(book);
                _context.SaveChanges();

                Books.Remove(bookItem);


                ReloadBooks();
                RaisePropertyChanged(nameof(Books));
            }
        }

            private void OpenAddNewAuthorDialog()
            {
                var dialog = new AddNewAuthorDialog
                {
                    Owner = Application.Current.MainWindow
                };

                if (dialog.ShowDialog() != true)
                    return;

                var dto = dialog.Author;
                if (dto == null) return;

                var newAuthor = _authorService.CreateAuthor(dto.FirstName, dto.LastName, dto.BirthDay);
            }
        }
}

























