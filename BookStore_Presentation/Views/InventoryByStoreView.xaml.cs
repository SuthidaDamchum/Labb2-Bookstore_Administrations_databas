using System.Windows;
using System.Windows.Controls;
using BookStore_Presentation.ViewModels;

namespace BookStore_Presentation.Views
{
    /// <summary>
    /// Interaction logic for InventoryByStore.xaml
    /// </summary>
    public partial class InventoryByStoreView : UserControl
    {
        public InventoryByStoreView()
        {
            InitializeComponent();

        }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is InventoryByStoreViewModel vm)
            {
                await vm.InitializeAsync();
            }
        }
    }
}