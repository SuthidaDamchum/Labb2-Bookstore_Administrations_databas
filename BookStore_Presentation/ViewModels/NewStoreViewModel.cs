using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using BookStore_Domain;
using BookStore_Infrastrcuture.Data.Model;
using BookStore_Presentation.Command;
using BookStore_Presentation.Dialogs;
using BookStore_Presentation.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore_Presentation.ViewModels
{
    public class NewStoreViewModel : ViewModelBase
    {
        private readonly BookStoreContext _context;

        public string StoreName { get; set; } = "";
        public string Address { get; set; } = "";
        public string City { get; set; } = "";
        public string PostalCode { get; set; } = "";
        public string Country { get; set; } = "";

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        

        public NewStoreViewModel()
        {
            _context = new BookStoreContext();
            SaveCommand = new AsyncDelegateCommand(SaveAsync);
            CancelCommand = new DelegateCommand(Cancel);
        }

        private async Task SaveAsync(object? parameter)
        {
            if (string.IsNullOrWhiteSpace(StoreName) ||
                string.IsNullOrWhiteSpace(Address) ||
                string.IsNullOrWhiteSpace(City) ||
                string.IsNullOrWhiteSpace(PostalCode) ||
                string.IsNullOrWhiteSpace(Country))
            {
                MessageBox.Show("All fields are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var store = new Store
            {
                StoreName = StoreName,
                Address = Address,
                City = City,
                PostalCode = PostalCode,
                Country = Country
            };

            _context.Stores.Add(store);
                await _context.SaveChangesAsync();

            MessageBox.Show("Store created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            if (parameter is Window window)
                window.DialogResult = true;
        }

        public ObservableCollection<NewStore> Stores { get; } = new();
        public ICommand AddStoreCommand { get; }
        public ICommand DeleteStoreCommand { get; }

        private NewStore? _selectedStore;
        public NewStore? SelectedStore
        {
            get => _selectedStore;
            set
            {
                _selectedStore = value;
                RaisePropertyChanged();
                ((AsyncDelegateCommand)DeleteStoreCommand).RaiseCanExecuteChanged();
            }
        }


        private void Cancel(object? parameter)
        {
            if (parameter is Window window)
                window.DialogResult = false;
        }

        public async Task LoadStoreAsync()
        {
            Stores.Clear();
            var stores = await _context.Stores.ToListAsync();
            foreach (var store in stores)
            {
                Stores.Add(new NewStore
                {
                    StoreId = store.StoreId,
                    StoreName = store.StoreName,
                    Address = store.Address,
                    City = store.City,
                    PostalCode = store.PostalCode,
                    Country = store.Country
                });
            }
        }
        public void OpenNewStoreDialog()
        {
            var dialog = new NewStoreDialog();
            if (dialog.ShowDialog() == true)
            {
                LoadStoreAsync();
            }
        }
    }
}

