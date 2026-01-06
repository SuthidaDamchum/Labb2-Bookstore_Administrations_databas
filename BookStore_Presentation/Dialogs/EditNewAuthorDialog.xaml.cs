using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BookStore_Presentation.Models;
using BookStore_Presentation.ViewModels;


namespace BookStore_Presentation.Dialogs
{
    /// <summary>
    /// Interaction logic for EditNewAuthorDialog.xaml
    /// </summary>
    public partial class EditNewAuthorDialog : Window
    {
        public CreateNewAuthorDto? Author { get; private set; }
        public EditNewAuthorDialog()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is AddNewAuthorViewModel vm)
            {
                // Parse the BirthDayText safely
                DateOnly? birthDate = null;
                if (!string.IsNullOrWhiteSpace(vm.BirthDayText) &&
                    DateOnly.TryParse(vm.BirthDayText, out var d))
                {
                    birthDate = d;
                }

                // Create the DTO to return
                Author = new CreateNewAuthorDto
                {
                    FirstName = vm.FirstName,
                    LastName = vm.LastName,
                    BirthDay = birthDate
                };

                DialogResult = true; // Close dialog with "OK"
                Close();
            }
        }
        private void CancelButton_Click_1(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
