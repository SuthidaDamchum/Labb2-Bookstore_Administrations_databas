using System.Collections.ObjectModel;
using System.Windows.Input;
using BookStore_Domain;
using BookStore_Infrastrcuture.Data.Model;
using BookStore_Presentation.Command;
using BookStore_Presentation.Services;
using Microsoft.EntityFrameworkCore;

namespace BookStore_Presentation.ViewModels
{
    internal class InventoryByStoreViewModel : ViewModelBase
    {
        private readonly BookSelectionService _selectionService;
        private readonly BookStoreContext _context = new();
        private readonly BooksAdminViewModel _bookCatalog;

        public ObservableCollection<BookAdminItem> Books => _bookCatalog.Books;

        public DelegateCommand IncreaseQuantityCommand { get; }
        public DelegateCommand DecreaseQuantityCommand { get; }
        public ICommand RemoveBookFromStoreCommand { get; }
        public ICommand AddBookToStoreCommand { get; }

        public ObservableCollection<Store> Stores { get; private set; }
        public ObservableCollection<InventoryItem> Inventory { get; private set; } = new();

        private Store? _selectedStore;
        public Store? SelectedStore
        {
            get => _selectedStore;
            set
            {
                _selectedStore = value;
                LoadInventory();
                RaisePropertyChanged();
            }
        }

        private InventoryItem? _selectedInventoryItem;
        public InventoryItem? SelectedInventoryItem
        {
            get => _selectedInventoryItem;
            set
            {
                _selectedInventoryItem = value;
                RaisePropertyChanged();
                IncreaseQuantityCommand.RaiseCanExecuteChanged();
                DecreaseQuantityCommand.RaiseCanExecuteChanged();
                ((DelegateCommand)RemoveBookFromStoreCommand).RaiseCanExecuteChanged();
            }
        }

        // Use the shared service for SelectedBook
        public BookAdminItem? SelectedBook
        {
            get => _selectionService.SelectedBook;
            set
            {
                _selectionService.SelectedBook = value;
                RaisePropertyChanged();
                ((DelegateCommand)AddBookToStoreCommand).RaiseCanExecuteChanged();
            }
        }

        public InventoryByStoreViewModel(BookSelectionService selectionService, BooksAdminViewModel booksVm)
        {
            _selectionService = selectionService;  // store the shared service
            _bookCatalog = booksVm;                 // use the shared BooksAdminViewModel

            LoadStores();

            IncreaseQuantityCommand = new DelegateCommand(
                _ =>
                {
                    if (SelectedInventoryItem != null)
                        UpdateQuantityInDatabase(SelectedInventoryItem, 1);
                },
                _ => SelectedInventoryItem != null
            );

            DecreaseQuantityCommand = new DelegateCommand(
                _ =>
                {
                    if (SelectedInventoryItem != null && SelectedInventoryItem.Quantity > 0)
                        UpdateQuantityInDatabase(SelectedInventoryItem, -1);
                },
                _ => SelectedInventoryItem != null && SelectedInventoryItem.Quantity > 0
            );

            AddBookToStoreCommand = new DelegateCommand(
                _ =>
                {
                    if (SelectedBook != null)
                        AddBookToStore(SelectedBook);
                },
                _ => SelectedBook != null && SelectedStore != null
            );

            RemoveBookFromStoreCommand = new DelegateCommand(
                _ =>
                {
                    if (SelectedInventoryItem != null)
                        RemoveBookFromStore(SelectedInventoryItem);
                },
                _ => SelectedInventoryItem != null && SelectedStore != null
            );
        }

        private void AddBookToStore(BookAdminItem book)
        {
            if (SelectedStore == null) return;

            var inventory = _context.Inventories
                .FirstOrDefault(i => i.Isbn13 == book.Isbn13 && i.StoreId == SelectedStore.StoreId);

            if (inventory == null)
            {
                _context.Inventories.Add(new Inventory
                {
                    Isbn13 = book.Isbn13,
                    StoreId = SelectedStore.StoreId,
                    Quantity = 1
                });
            }
            else
            {
                inventory.Quantity++;
            }

            _context.SaveChanges();
            LoadInventory();
        }

        private void RemoveBookFromStore(InventoryItem item)
        {
            if (SelectedStore == null || item == null) return;

            var inventory = _context.Inventories
                .FirstOrDefault(i => i.Isbn13 == item.ISBN && i.StoreId == SelectedStore.StoreId);

            if (inventory != null)
            {
                _context.Inventories.Remove(inventory);
                _context.SaveChanges();
                LoadInventory();
            }
        }

        private void UpdateQuantityInDatabase(InventoryItem item, int delta)
        {
            var inventory = _context.Inventories
                .FirstOrDefault(i => i.Isbn13 == item.ISBN && i.StoreId == item.StoreId);

            if (inventory != null)
            {
                inventory.Quantity += delta;
                _context.SaveChanges();
                item.Quantity += delta;
            }
        }

        private void LoadStores()
        {
            Stores = new ObservableCollection<Store>(_context.Stores.ToList());
            SelectedStore = Stores.FirstOrDefault();
            RaisePropertyChanged(nameof(Stores));
        }

        private void LoadInventory()
        {
            if (SelectedStore == null) return;

            Inventory = new ObservableCollection<InventoryItem>(
                _context.Inventories
                    .Include(i => i.Isbn13Navigation)
                    .Where(i => i.StoreId == SelectedStore.StoreId)
                    .Select(i => new InventoryItem
                    {
                        ISBN = i.Isbn13Navigation.Isbn13,
                        Title = i.Isbn13Navigation.Title,
                        Price = i.Isbn13Navigation.Price ?? 0m,
                        Quantity = i.Quantity,
                        StoreId = i.StoreId
                    }).ToList()
            );
            RaisePropertyChanged(nameof(Inventory));
        }
    }
}
