using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using BookStore_Domain;
using BookStore_Infrastrcuture.Data.Model;
using BookStore_Presentation.Command;
using BookStore_Presentation.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore_Presentation.ViewModels
{
    public class AddNewTitleViewModel : ViewModelBase
    {
        private readonly BookStoreContext _context;

        public AddNewTitleViewModel()
        {
            _context = new BookStoreContext();

            // Load reference data
            Genres = _context.Genres.ToList();
            Publishers = _context.Publishers.ToList();
            Authors = new ObservableCollection<AuthorItem>(
                _context.Authors
                    .Where(a => !string.IsNullOrWhiteSpace(a.FirstName) && !string.IsNullOrWhiteSpace(a.LastName))
                    .Select(a => new AuthorItem
                    {
                        AuthorId = a.AuthorId,
                        FullName = $"{a.FirstName} {a.LastName}",
                        BirthDay = a.BirthDay
                    })
            );

            foreach (var author in Authors)
                author.PropertyChanged += OnAuthorSelectionChanged;

            LanguageOptions = new List<string> { "English", "Swedish", "Norwegian", "French", "Spanish", "Danish", "German" };

            // Commands
            SaveCommand = new DelegateCommand(Save);
            EditCommand = new DelegateCommand(Edit);
            CancelCommand = new DelegateCommand(Cancel);
        }

            #region Properties

            public string Title { get; set; } = "";
            public string ISBN { get; set; } = "";
            public string? Language { get; set; }
            public string? PriceText { get; set; }
            public string? PublicationDateText { get; set; }
            public string? PageCountText { get; set; }

            public Genre? SelectedGenre { get; set; }
            public Publisher? SelectedPublisher { get; set; }

            public List<string> LanguageOptions { get; set; }
            public List<Genre> Genres { get; set; }
            public List<Publisher> Publishers { get; set; }

            private ObservableCollection<AuthorItem> _authors = new();
            public ObservableCollection<AuthorItem> Authors
            {
                get => _authors;
                set { _authors = value; RaisePropertyChanged(); }
            }

            public List<AuthorItem> SelectedAuthors => Authors.Where(a => a.IsSelected).ToList();
            public string SelectedAuthorsText => string.Join(", ", SelectedAuthors.Select(a => a.FullName));

            public ICommand SaveCommand { get; }
            public ICommand EditCommand { get; }
            public ICommand CancelCommand { get; }

            #endregion

            #region Methods

            /// <summary>
            /// Load data from existing book (for edit)
            /// </summary>
        public void LoadFromBook(BookAdminItem book)
        {
            if (book == null) return;

            Title = book.Title;
            ISBN = book.Isbn13;
            Language = book.Language;
            PriceText = book.Price.ToString("0.##", CultureInfo.InvariantCulture);
            PublicationDateText = book.PublicationDate?.ToString("yyyy-MM-dd");
            PageCountText = book.PageCount?.ToString();

            SelectedGenre = Genres.FirstOrDefault(g => g.GenreName == book.GenreName);
            SelectedPublisher = Publishers.FirstOrDefault(p => p.PublisherName == book.PublisherName);

            // Mark authors as selected
            foreach (var author in Authors)
                author.IsSelected = book.AuthorIds.Contains(author.AuthorId);
        }

        private void Save(object? parameter)
        {
            if (!ValidateInput()) return;

            if (!decimal.TryParse(PriceText, NumberStyles.Number, CultureInfo.InvariantCulture, out var price))
            {
                MessageBox.Show("Invalid price.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateOnly? publicationDate = null;
            if (!string.IsNullOrWhiteSpace(PublicationDateText))
            {
                if (DateOnly.TryParseExact(PublicationDateText, "yyyy-MM-dd", CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var d))
                    publicationDate = d;
                else
                {
                    MessageBox.Show("Invalid publication date. Use YYYY-MM-DD.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            int? pageCount = int.TryParse(PageCountText, out var p) ? p : null;

            var newBook = new Book
            {
                Title = Title,
                Isbn13 = ISBN,
                Language = Language,
                Price = price,
                GenreId = SelectedGenre?.GenreId,
                PublisherId = SelectedPublisher?.PublisherId,
                PublicationDate = publicationDate,
                PageCount = pageCount,
                BookAuthors = SelectedAuthors.Select(a => new BookAuthor
                {
                    AuthorId = a.AuthorId,
                    BookIsbn13 = ISBN,
                    Role = "Main Author"
                }).ToList()
            };

            _context.Books.Add(newBook);
            _context.SaveChanges();

            MessageBox.Show("Book saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            if (parameter is Window window)
                window.DialogResult = true;
        }

        private void Edit(object? parameter)
        {
            if (!ValidateInput()) return;

            var book = _context.Books.Include(b => b.BookAuthors).FirstOrDefault(b => b.Isbn13 == ISBN);
            if (book == null)
            {
                MessageBox.Show("Book not found.");
                return;
            }

            if (!decimal.TryParse(PriceText, NumberStyles.Number, CultureInfo.InvariantCulture, out var price))
            {
                MessageBox.Show("Invalid price.");
                return;
            }

            DateOnly? publicationDate = null;
            if (!string.IsNullOrWhiteSpace(PublicationDateText))
            {
                if (DateOnly.TryParseExact(PublicationDateText, "yyyy-MM-dd", CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var d))
                    publicationDate = d;
                else
                    publicationDate = null;
            }

            int? pageCount = int.TryParse(PageCountText, out var p) ? p : null;

            book.Title = Title;
            book.Language = Language;
            book.Price = price;
            book.GenreId = SelectedGenre?.GenreId;
            book.PublisherId = SelectedPublisher?.PublisherId;
            book.PublicationDate = publicationDate;
            book.PageCount = pageCount;

            // Sync authors
            var selectedIds = SelectedAuthors.Select(a => a.AuthorId).ToList();
            var toRemove = book.BookAuthors.Where(ba => !selectedIds.Contains(ba.AuthorId)).ToList();
            foreach (var ba in toRemove) book.BookAuthors.Remove(ba);
            foreach (var id in selectedIds.Where(i => !book.BookAuthors.Any(ba => ba.AuthorId == i)))
                book.BookAuthors.Add(new BookAuthor { AuthorId = id, BookIsbn13 = book.Isbn13, Role = "Main Author" });

            _context.SaveChanges();

            if (parameter is Window window)
                window.DialogResult = true;
        }

                private void Cancel(object? parameter)
                {
                    if (parameter is Window window)
                        window.DialogResult = false;
                }

                private void OnAuthorSelectionChanged(object? sender, PropertyChangedEventArgs e)
                {
                    if (e.PropertyName == nameof(AuthorItem.IsSelected))
                        RaisePropertyChanged(nameof(SelectedAuthorsText));
                }

                private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                MessageBox.Show("Title is required.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(ISBN) || ISBN.Length != 13 || !ISBN.All(char.IsDigit))
            {
                MessageBox.Show("ISBN-13 must be exactly 13 digits.");
                return false;
            }
            if (SelectedAuthors.Count == 0)
            {
                MessageBox.Show("Select at least one author.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(Language))
            {
                MessageBox.Show("Language is required.");
                return false;
            }
            return true;
        }

        #endregion
    }
}
