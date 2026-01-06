using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BookStore_Presentation.ViewModels;

namespace BookStore_Presentation.Models
{
    public class AuthorItem : ViewModelBase
    {
        public int AuthorId { get; set; }

        private string _fullName = "";
        public string FullName
        {
            get => _fullName;
            set { _fullName = value; RaisePropertyChanged(); }
        }

        private DateOnly? _birthDay;
        public DateOnly? BirthDay
        {
            get => _birthDay;
            set { _birthDay = value; RaisePropertyChanged(); }
        }

        private int _booksCount;
        public int BooksCount
        {
            get => _booksCount;
            set { _booksCount = value; RaisePropertyChanged(); }
        }

        private bool _isSelected = false;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value; RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsSelected));
            }
        }
    }
}









