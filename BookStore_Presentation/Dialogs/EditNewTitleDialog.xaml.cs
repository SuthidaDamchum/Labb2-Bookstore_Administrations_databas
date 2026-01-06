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
namespace BookStore_Presentation.Dialogs
{
    /// <summary>
    /// Interaction logic for EditNewTitleDialog.xaml
    /// </summary>
    public partial class EditNewTitleDialog : Window
    {

        public CreateNewTitleDto? Book { get; private set; }
        public EditNewTitleDialog()
        {
            InitializeComponent();
        }

    }
}

