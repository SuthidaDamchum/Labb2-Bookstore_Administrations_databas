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
using BookStore_Presentation.Services;
using Microsoft.EntityFrameworkCore;

namespace BookStore_Presentation.ViewModels
{
    public class AddNewTitleViewModel : ViewModelBase
    {
        private readonly BookStoreContext _context;

        public AddNewTitleViewModel()
        {
            _context = new BookStoreContext();

            // Load data

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
            {
                author.PropertyChanged += OnAuthorSelectionChanged;
            }


            LanguageOptions = new List<string> { "English", "Swedish", "Norwegian", "French", "Spanish", "Danish", "German" };

            // Commands
            SaveCommand = new DelegateCommand(Save);
            EditCommand = new DelegateCommand(Edit);
            CancelCommand = new DelegateCommand(Cancel);
        }

        #region Properties

        private string _title = "";
        private string _isbn = "";
        private string? _language;
        private string? _priceText;
        private string? _publicationDateText;
        private string? _pageCountText;

        private Genre? _selectedGenre;
        private Publisher? _selectedPublisher;

        public string Title
        {
            get => _title;
            set { _title = value; RaisePropertyChanged(); }
        }

        public string ISBN
        {
            get => _isbn;
            set { _isbn = value; RaisePropertyChanged(); }
        }

        public string? Language
        {
            get => _language;
            set { _language = value; RaisePropertyChanged(); }
        }

        public string? PriceText
        {
            get => _priceText;
            set { _priceText = value; RaisePropertyChanged(); }
        }

        public decimal? Price
        {
            get
            {
                if (decimal.TryParse
                    (_priceText, 
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture, 
                    out var result))
                    return result;
                return null;
            }
        }

        public string? PublicationDateText
        {
            get => _publicationDateText;
            set { _publicationDateText = value; RaisePropertyChanged(); }
        }

        public DateOnly? PublicationDate
        {
            get
            {
                if (DateOnly.TryParse(_publicationDateText, out var date))
                    return date;
                return null;
            }
        }

        public string? PageCountText
        {
            get => _pageCountText;
            set { _pageCountText = value; RaisePropertyChanged(); }
        }

        public int? PageCount
        {
            get
            {
                if (int.TryParse(_pageCountText, out var result))
                    return result;
                return null;
            }
        }

        public Genre? SelectedGenre
        {
            get => _selectedGenre;
            set { _selectedGenre = value; RaisePropertyChanged(); }
        }

        public Publisher? SelectedPublisher
        {
            get => _selectedPublisher;
            set { _selectedPublisher = value; RaisePropertyChanged(); }
        }


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

        private void Save(object? parameter)
        {
            ValidateInput();

            if (!decimal.TryParse(PriceText, NumberStyles.Currency, CultureInfo.InvariantCulture, out var price))
            {
                MessageBox.Show("Invalid price.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateOnly? publicationDate = null;
            if (!string.IsNullOrWhiteSpace(PublicationDateText))
            {
                if (DateOnly.TryParseExact(PublicationDateText, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out var date))
                    publicationDate = date;
                else
                {
                    MessageBox.Show("Invalid publication date. Use YYYY-MM-DD.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            int? pageCount = null;
            if (!string.IsNullOrWhiteSpace(PageCountText) && int.TryParse(PageCountText, out var pages))
                pageCount = pages;

            // Create Book
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

         
            if (parameter is Window window) window.DialogResult = true;
        }

        private void Cancel(object? parameter)
        {
            if (parameter is Window window) window.DialogResult = false;
        }

        private void OnAuthorSelectionChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AuthorItem.IsSelected))
            {
                RaisePropertyChanged(nameof(SelectedAuthors));
                RaisePropertyChanged(nameof(SelectedAuthorsText));
            }
        }

        #endregion
    }
}

