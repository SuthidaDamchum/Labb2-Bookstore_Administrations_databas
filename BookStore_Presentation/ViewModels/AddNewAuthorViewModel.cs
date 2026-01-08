    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;
    using BookStore_Presentation.Models;


namespace BookStore_Presentation.ViewModels
    {
        public class AddNewAuthorViewModel : ViewModelBase
    {
            private string _firstName = "";
            private string _lastName = "";
            private string? _birthDayText;

            public string FirstName
            {
                get => _firstName;
                set { _firstName = value; RaisePropertyChanged(); }
            }

            public string LastName
            {
                get => _lastName;
                set { _lastName = value; RaisePropertyChanged(); }
            }

            public string? BirthDayText
            {
                get => _birthDayText;
                set { _birthDayText = value; RaisePropertyChanged(); }
            }
            public DateOnly? BirthDay
            {
                get
                {
                    if (DateOnly.TryParse(BirthDayText, out var date))
                        return date;
                    return null;
                }
            }




            
        }
    }
