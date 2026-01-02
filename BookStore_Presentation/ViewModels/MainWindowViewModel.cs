using System.ComponentModel.Design;
using System.Windows.Input;
using BookStore_Domain;
using BookStore_Infrastrcuture.Data.Model;
using BookStore_Presentation.Command;
using Microsoft.EntityFrameworkCore;
using BookStore_Presentation.Command;
using BookStore_Presentation.Services;

namespace BookStore_Presentation.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private readonly BookStoreContext _context;

     

        public ICommand OpenInventoryByStoreCommand { get; }
        public ICommand OpenBookAdminCommand { get; }

        private ViewModelBase? _currentViewModel;
        public ViewModelBase? CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                RaisePropertyChanged();
            }
        }

      

        public MainWindowViewModel(BookSelectionService bookSelectionService)

        {
            var selectionService = new BookSelectionService();
            
            // Create shared BooksAdminViewModel
            var booksVm = new BooksAdminViewModel(selectionService);

            _context = new BookStoreContext();

            OpenInventoryByStoreCommand = new DelegateCommand(_ =>
            {
                CurrentViewModel = new InventoryByStoreViewModel(selectionService, booksVm);
            });
            OpenBookAdminCommand = new DelegateCommand(_ =>
            {
                CurrentViewModel = booksVm;
            });

            
        }

    }
}

