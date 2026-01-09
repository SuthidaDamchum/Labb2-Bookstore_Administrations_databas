using System.Collections.ObjectModel;
using System.Linq;
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
    public class AuthorsAdminViewModel : ViewModelBase
    {
        private readonly BookStoreContext _context;

        private readonly AuthorService _authorService;

        private AuthorItem? _selectedAuthor;
        public AuthorItem? SelectedAuthor
        {
            get => _selectedAuthor;
            set
            {
                _selectedAuthor = value;
                RaisePropertyChanged();

                ((AsyncDelegateCommand)EditAuthorCommand).RaiseCanExecuteChanged();
                ((AsyncDelegateCommand)DeleteAuthorCommand).RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<AuthorItem> Authors { get; } = new();

        public ICommand AddAuthorCommand { get; }
        public ICommand EditAuthorCommand { get; }
        public ICommand DeleteAuthorCommand { get; }

        public AuthorsAdminViewModel(AuthorService authorService)
        {
            _authorService = authorService;
            _context = new BookStoreContext();

            LoadAuthorsAsycn();

            AddAuthorCommand = new AsyncDelegateCommand(_ => AddAuthorAsync());
            EditAuthorCommand = new AsyncDelegateCommand(_ => EditAuthorAsync(), _ => SelectedAuthor != null);
            DeleteAuthorCommand = new AsyncDelegateCommand(_ => DeleteAuthorAsync(), _ => SelectedAuthor != null);
        }

        public async Task LoadAuthorsAsycn()
        {
            Authors.Clear();

            var authors = await _context.Authors
                .Include(a => a.BookAuthors)
                .Select(a => new AuthorItem
                {
                    AuthorId = a.AuthorId,
                    FullName = a.FirstName + " " + a.LastName,
                    BirthDay = a.BirthDay,   // still stored
                    BooksCount = a.BookAuthors.Count
                })
                .ToListAsync();

            foreach (var a in authors)
                Authors.Add(a);
        }

        private async Task AddAuthorAsync()
        {
            var dialog = new AddNewAuthorDialog
            {
            };

            if (dialog.ShowDialog() != true)
                return;


            var dto = dialog.Author;
            if (dto == null)
                return;


            var newAuthor = await _authorService.CreateAuthorAsync(dto.FirstName, dto.LastName, dto.BirthDay);


            Authors.Add(new AuthorItem
            {
                AuthorId = newAuthor.AuthorId,
                FullName = newAuthor.FirstName + " " + newAuthor.LastName,
                BirthDay = newAuthor.BirthDay,
                BooksCount = 0
            });
        }

        private async Task EditAuthorAsync()
        {

            if (SelectedAuthor == null)
                return;


            var dialog = new EditNewAuthorDialog
            {
                DataContext = new AddNewAuthorViewModel
                {
                    FirstName = SelectedAuthor.FullName.Split(' ')[0],
                    LastName = SelectedAuthor.FullName.Split(' ').Length > 1 ? SelectedAuthor.FullName.Split(' ')[1] : "",
                    BirthDayText = SelectedAuthor.BirthDay?.ToString("yyyy-MM-dd")
                }
            };

   
            if (dialog.ShowDialog() != true)
                return;

       
            var dto = dialog.Author;
            if (dto == null)
                return;

            if (!_authorService.IsValidAuthor(dto.FirstName, dto.LastName, dto.BirthDay?.ToString(), out var error))
            {
                MessageBox.Show(error, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            var updateAuthor = await _authorService.UpdateAuthorAsync(
            SelectedAuthor.AuthorId,
                 dto.FirstName,
                 dto.LastName,
                 dto.BirthDay
            );

            SelectedAuthor.FullName = updateAuthor.FirstName + " " + updateAuthor.LastName;
            SelectedAuthor.BirthDay = updateAuthor.BirthDay;

        }


        private async Task DeleteAuthorAsync()
        {
            if (SelectedAuthor == null) return;

            var result = MessageBox.Show(
                $"Are you sure you want to delete {SelectedAuthor.FullName}?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (result != MessageBoxResult.Yes)
                return;

            try
            {

               await _authorService.DeleteAuthorAsync(SelectedAuthor.AuthorId);

                Authors.Remove(SelectedAuthor);

                SelectedAuthor = null;

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Cannot delete author", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}