using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using BookStore_Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using static Microsoft.VisualStudio.Services.Graph.GraphResourceIds;

namespace BookStore_Presentation.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private BookStoreContext _context;

        public ObservableCollection<InventoryItem> Inventory { get; private set; } = new();
        public ObservableCollection<Store> Stores { get; private set; }

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

        public MainWindowViewModel()
        {
            _context = new BookStoreContext();
            LoadStore();
        }

        private void LoadStore()
        {
            Stores = new ObservableCollection<Store>(_context.Stores.ToList());
            SelectedStore = Stores.FirstOrDefault();
            RaisePropertyChanged(nameof(Stores));
        }

        private void LoadInventory()
        {
            if (SelectedStore == null) return;

            var inventoryList = _context.Inventories
             .Include(i => i.Isbn13Navigation)
            .Where(i => i.StoreId == SelectedStore.StoreId)
             .Select(i => new InventoryItem
         {
             ISBN = i.Isbn13Navigation.Isbn13,
             Title = i.Isbn13Navigation.Title,
                 Price = i.Isbn13Navigation.Price ?? 0m,
                 Quantity = i.Quantity
         })
         .ToList();


            Inventory = new ObservableCollection<InventoryItem>(inventoryList);
            RaisePropertyChanged(nameof(Inventory));
        }
    }
}