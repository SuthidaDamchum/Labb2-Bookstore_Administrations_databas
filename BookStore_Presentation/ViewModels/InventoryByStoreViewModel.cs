using System.Collections.ObjectModel;
using System.Windows.Input;
using BookStore_Domain;
using BookStore_Infrastrcuture.Data.Model;
using BookStore_Presentation.Command;
using BookStore_Presentation.Models;
using BookStore_Presentation.Services;
using Microsoft.EntityFrameworkCore;

namespace BookStore_Presentation.ViewModels
{
    internal class InventoryByStoreViewModel : ViewModelBase
    {
        private readonly BookSelectionService _selectionService;
        private readonly BookStoreContext _context = new();
        private readonly BooksAdminViewModel _bookCatalog;
        public ObservableCollection<BookAdminItem> AvailableBooks { get; private set; }

        public ObservableCollection<BookAdminItem> Books => _bookCatalog.Books;

        public AsyncDelegateCommand IncreaseQuantityCommand { get; }
        public AsyncDelegateCommand DecreaseQuantityCommand { get; }
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
                LoadInventoryAsync();
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
                ((AsyncDelegateCommand)IncreaseQuantityCommand).RaiseCanExecuteChanged();
                ((AsyncDelegateCommand)DecreaseQuantityCommand).RaiseCanExecuteChanged();
                ((AsyncDelegateCommand)RemoveBookFromStoreCommand).RaiseCanExecuteChanged();
            }
        }
    
        public BookAdminItem? SelectedBook
        {
            get => _selectionService.SelectedBook;
            set
            {
                _selectionService.SelectedBook = value;
                RaisePropertyChanged();
                ((AsyncDelegateCommand)AddBookToStoreCommand).RaiseCanExecuteChanged();
            }
        }

        public InventoryByStoreViewModel(BookSelectionService selectionService, BooksAdminViewModel booksVm)
        {
            _selectionService = selectionService;
            _bookCatalog = booksVm;

            _ = LoadStoresAsync();

            IncreaseQuantityCommand = new AsyncDelegateCommand(
                async _ =>
                {
                    if (SelectedInventoryItem != null)
                        await UpdateQuantityInDatabaseAsync(SelectedInventoryItem, 1);
                },
                _ => SelectedInventoryItem != null
            );

            DecreaseQuantityCommand = new AsyncDelegateCommand(
                async _ =>
                {
                    if (SelectedInventoryItem != null && SelectedInventoryItem.Quantity > 0)
                        await UpdateQuantityInDatabaseAsync(SelectedInventoryItem, -1);
                },
                _ => SelectedInventoryItem != null && SelectedInventoryItem.Quantity > 0
            );

            AddBookToStoreCommand = new AsyncDelegateCommand(
                async _ =>
                {
                    if (SelectedBook != null)
                        await AddBookToStoreAsync(SelectedBook);
                },
                _ => SelectedBook != null && SelectedStore != null
            );


            AddBookToStoreCommand = new AsyncDelegateCommand(
               async _ =>
                {
                    if (SelectedBook != null)
                      await  AddBookToStoreAsync(SelectedBook);
                },
                _ => SelectedBook != null && SelectedStore != null
            );

            RemoveBookFromStoreCommand = new AsyncDelegateCommand(
              async _ =>
                {
                    if (SelectedInventoryItem != null)
                      await RemoveBookFromStoreAsync(SelectedInventoryItem);
                },
                _ => SelectedInventoryItem != null && SelectedStore != null
            );
        }

        private async Task AddBookToStoreAsync(BookAdminItem book)
        {
            if (SelectedStore == null || book == null) return;

            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.Isbn13 == book.Isbn13 && i.StoreId == SelectedStore.StoreId);

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

            await _context.SaveChangesAsync();
            await LoadInventoryAsync();
        }

        private async Task RemoveBookFromStoreAsync(InventoryItem item)
        {
            if (SelectedStore == null || item == null) return;

            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.Isbn13 == item.ISBN && i.StoreId == SelectedStore.StoreId);

            if (inventory != null)
            {
                _context.Inventories.Remove(inventory);
               await _context.SaveChangesAsync();
               await LoadInventoryAsync();
            }
        }

        private async Task UpdateQuantityInDatabaseAsync(InventoryItem item, int delta)
        {
            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.Isbn13 == item.ISBN && i.StoreId == item.StoreId);

            if (inventory != null)
            {
                inventory.Quantity += delta;
                _context.SaveChanges();
                item.Quantity += delta;
            }
        }

        private async Task LoadStoresAsync()
        {
            Stores = new ObservableCollection<Store>(await _context.Stores.ToListAsync());
            SelectedStore = Stores.FirstOrDefault();
            RaisePropertyChanged(nameof(Stores));
        }

        private async Task LoadInventoryAsync()
        {
            if (SelectedStore == null) return;

            var inventoryList = await _context.Inventories
                 .Include(i => i.Isbn13Navigation)
                    .Where(i => i.StoreId == SelectedStore.StoreId)
                    .Select(i => new InventoryItem
                    {
                        ISBN = i.Isbn13Navigation.Isbn13,
                        Title = i.Isbn13Navigation.Title,
                        Price = i.Isbn13Navigation.Price ?? 0m,
                        Quantity = i.Quantity,
                        StoreId = i.StoreId
                    }).ToListAsync();


            Inventory = new ObservableCollection<InventoryItem>(inventoryList);
                
            var inventoryIsbns = Inventory.Select(item => item.ISBN).ToHashSet();
            AvailableBooks = new ObservableCollection<BookAdminItem>(
                _bookCatalog.Books
                    .Where(b => !inventoryIsbns.Contains(b.Isbn13))
                    .ToList()
            );

            RaisePropertyChanged(nameof(AvailableBooks));
            RaisePropertyChanged(nameof(Inventory));
        }
    }
}
