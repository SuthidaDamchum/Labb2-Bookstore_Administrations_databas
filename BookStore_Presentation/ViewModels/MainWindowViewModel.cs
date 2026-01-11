using System.Windows.Input;
using BookStore_Infrastrcuture.Data.Model;
using BookStore_Presentation.Command;
using BookStore_Presentation.Services;

namespace BookStore_Presentation.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
 
        {
            // Current view bound to ContentControl
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

        public ICommand OpenInventoryByStoreCommand { get; }
        public ICommand OpenBookAdminCommand { get; }
        public ICommand OpenAllAuthorsCommand { get; }

        //Keep VM instances to avoid recreating them
        private readonly BooksAdminViewModel _booksVm;
        private readonly AuthorsAdminViewModel _authorsVm;
        private readonly InventoryByStoreViewModel _inventoryVm;


        public MainWindowViewModel(AuthorService authorService)
        {
      
            _booksVm = new BooksAdminViewModel(authorService);
            _authorsVm = new AuthorsAdminViewModel(authorService);
            _inventoryVm = new InventoryByStoreViewModel(_booksVm);

     
            OpenInventoryByStoreCommand = new DelegateCommand(_ =>
            {
                CurrentViewModel = _inventoryVm;
            });

            OpenBookAdminCommand = new DelegateCommand(_ =>
            {
                CurrentViewModel = _booksVm;
            });

            OpenAllAuthorsCommand = new AsyncDelegateCommand(async _ =>
              {
                  await _authorsVm.LoadAuthorsAsycn(); // Await the async load
                  CurrentViewModel = _authorsVm;
              });

          
            CurrentViewModel = _inventoryVm;
        }
    }
}




