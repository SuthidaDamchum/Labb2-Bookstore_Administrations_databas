using System.Collections.ObjectModel;
using System.Runtime.Serialization.DataContracts;
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

        private readonly BookService _bookService;

        private readonly AuthorService _authorService = new AuthorService(new BookStoreContext());

        private readonly BookStoreContext _context;
        public ObservableCollection<BookAdminItem> Books { get; }
        public ICommand DeleteBookFromInventoryCommand { get; }
        public ICommand CreateNewBookTitleCommand { get; }
        public ICommand EditNewBookTitleCommand { get; }
        public ICommand CreateNewAuthorCommand { get; }

        public BooksAdminViewModel(AuthorService authorService)
        {


            _authorService = authorService;
            _context = new BookStoreContext();
            _bookService = new BookService(_context);


            Books = new ObservableCollection<BookAdminItem>(LoadBooks());

            CreateNewBookTitleCommand = new AsyncDelegateCommand(async _=> await OpenAddBookDialog());

            CreateNewAuthorCommand = new DelegateCommand(async _=> await OpenAddNewAuthorDialogAsync());

            EditNewBookTitleCommand = new AsyncDelegateCommand(
              async  _ => await EditNewBookTitleAsync(),
                _ => SelectedBookInBookAdmin != null);


            DeleteBookFromInventoryCommand = new AsyncDelegateCommand(
             async param =>
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

                            await DeleteBookFromInventoryAsync(book);

                            await ReloadBooksAsyns();
                        }
                    }
                },
                  param => param is BookAdminItem
          );
        }

        private BookAdminItem? _selectedBookInBookAdmin;
        public BookAdminItem? SelectedBookInBookAdmin
        {
            get => _selectedBookInBookAdmin;
            set
            {
                _selectedBookInBookAdmin = value;
                RaisePropertyChanged();
                ((AsyncDelegateCommand)EditNewBookTitleCommand).RaiseCanExecuteChanged();
                ((AsyncDelegateCommand)DeleteBookFromInventoryCommand).RaiseCanExecuteChanged();
            }
        }

        private async Task OpenAddBookDialog()
        {

            var addNewTitleViewModel = new AddNewTitleViewModel();
            var dialog = new AddNewTitleDialog(addNewTitleViewModel);

            if (dialog.ShowDialog() == true)
            {
               await ReloadBooksAsyns();
            }
        }

        private async Task ReloadBooksAsyns()
        {
            Books.Clear();

            var books = await _context.Books
                .AsNoTracking()
                .Include(b => b.BookAuthors)
                     .ThenInclude(ba => ba.Author)
                .ToListAsync();

            foreach (var b in books)
            {
                Books.Add(new BookAdminItem

                {
                    Isbn13 = b.Isbn13,
                    Title = b.Title,
                    Language = b.Language,
                    Price = b.Price ?? 0m,
                    PublicationDate = b.PublicationDate,
                    AuthorIds = b.BookAuthors.Select(ba => ba.AuthorId).ToArray(),
                    AuthorNameString = string.Join(" ,", b.BookAuthors.Select(ba => ba.Author.FirstName + " " + ba.Author.LastName ))

                });
            }
            RaisePropertyChanged(nameof(Books));
        }

        private List<BookAdminItem> LoadBooks()
        {
            return _context.Books
                .AsNoTracking()
           .Include(b => b.BookAuthors)
               .ThenInclude(ba => ba.Author)
           .Select(b => new BookAdminItem
           {
               Isbn13 = b.Isbn13,
               Title = b.Title,

               AuthorNameString = string.Join(", ",
               b.BookAuthors
               .Where(ba => ba.Author != null)
               .Select(ba => ba.Author.FirstName + " " + ba.Author.LastName)),


               Language = b.Language,
               Price = b.Price ?? 0m,
               PublicationDate = b.PublicationDate,
               PageCount = b.PageCount,
               AuthorIds = b.BookAuthors.Select(b => b.AuthorId).ToArray()
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

         
            var newBookTitle = new Book

            {
                Isbn13 = isbn13,
                Title = title,
                Language = language,
                Price = price,
                PublicationDate = publicationdate,
                PageCount = pagecount,
        

                BookAuthors = authors.Select(a => new BookAuthor
                {
                    AuthorId = a.AuthorId
                }).ToList()
            };

            _context.Books.Add(newBookTitle);
            _context.SaveChanges();


            Books.Add(new BookAdminItem
            {
                Isbn13 = newBookTitle.Isbn13,
                Title = newBookTitle.Title,
                AuthorNameString = string.Join(", ", authors.Select(a => $"{a.FirstName} {a.LastName}")),
                Language = newBookTitle.Language,
                Price = newBookTitle.Price ?? 0m,
                PublicationDate = newBookTitle.PublicationDate,
                PageCount = newBookTitle.PageCount,
                AuthorIds = authors.Select(a => a.AuthorId).ToArray()

            });
            RaisePropertyChanged(nameof(Books));
        }

        private async Task EditNewBookTitleAsync()
        {
            if (SelectedBookInBookAdmin == null)
                return;

            var viewModel = new AddNewTitleViewModel();
            viewModel.LoadFromBook(SelectedBookInBookAdmin);

            var dialog = new EditNewTitleDialog
            {
                DataContext = viewModel,
            };  

            if (dialog.ShowDialog() != true)
            {
                SelectedBookInBookAdmin = null; 
                return;
            }

            using var context = new BookStoreContext();

            var updatedBook = await context.Books
                .AsNoTracking()
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .FirstOrDefaultAsync(b => b.Isbn13 == SelectedBookInBookAdmin.Isbn13);

            if (updatedBook == null)
            {
                MessageBox.Show("Could not reload updated book.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


       

            SelectedBookInBookAdmin.Title = updatedBook.Title;
            SelectedBookInBookAdmin.Language = updatedBook.Language;
            SelectedBookInBookAdmin.Price = updatedBook.Price ?? 0m;
            SelectedBookInBookAdmin.PublicationDate = updatedBook.PublicationDate;
            SelectedBookInBookAdmin.PageCount = updatedBook.PageCount;
            SelectedBookInBookAdmin.AuthorIds = updatedBook.BookAuthors
                .Select(ba => ba.AuthorId)
                .ToArray();

            SelectedBookInBookAdmin.AuthorNameString = string.Join(", ",
                updatedBook.BookAuthors.Select(ba =>
                    ba.Author.FirstName + " " + ba.Author.LastName));

            SelectedBookInBookAdmin.RaisePropertyChanged(nameof(SelectedBookInBookAdmin.Title));
            SelectedBookInBookAdmin.RaisePropertyChanged(nameof(SelectedBookInBookAdmin.Language));
            SelectedBookInBookAdmin.RaisePropertyChanged(nameof(SelectedBookInBookAdmin.Price));
            SelectedBookInBookAdmin.RaisePropertyChanged(nameof(SelectedBookInBookAdmin.PublicationDate));
            SelectedBookInBookAdmin.RaisePropertyChanged(nameof(SelectedBookInBookAdmin.PageCount));
            SelectedBookInBookAdmin.RaisePropertyChanged(nameof(SelectedBookInBookAdmin.AuthorNameString));
        }

        public async Task DeleteBookFromInventoryAsync(BookAdminItem bookItem)
        {
            if (bookItem == null) return;


            var book = await _context.Books
                .Include(b => b.BookAuthors)
                .FirstOrDefaultAsync(b => b.Isbn13 == bookItem.Isbn13);

            if (book != null)
            {
                if (book.BookAuthors.Any())
                {
                    _context.BookAuthors.RemoveRange(book.BookAuthors);
                }

                var inventories = _context.Inventories.Where(i => i.Isbn13 == book.Isbn13);
                _context.Inventories.RemoveRange(inventories);


                _context.Books.Remove(book);

                await _context.SaveChangesAsync();

                Books.Remove(bookItem);
                RaisePropertyChanged(nameof(Books));
            }
        }

        private async Task OpenAddNewAuthorDialogAsync()
        {
            var dialog = new AddNewAuthorDialog
            {
                Owner = Application.Current.MainWindow
            };

            if (dialog.ShowDialog() != true)
                return;

            var dto = dialog.Author;
            if (dto == null) return;

            var newAuthor = await _authorService.CreateAuthorAsync(dto.FirstName, dto.LastName, dto.BirthDay);

            if (SelectedBookInBookAdmin != null && newAuthor != null)
            {
                var bookAuthor = new BookAuthor
                {
                    AuthorId = newAuthor.AuthorId,
                    BookIsbn13 = SelectedBookInBookAdmin.Isbn13,
                    Role = "Main Author"
                };
                _context.BookAuthors.Add(bookAuthor);
                await _context.SaveChangesAsync();
            }

        }
    }
}
