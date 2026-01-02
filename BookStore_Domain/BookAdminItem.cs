using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore_Domain
{
    public class BookAdminItem
    {
        public string Isbn13 { get; set; } = "";
        public string Title { get; set; } = "";
        public string AuthorNames { get; set; } = "";
        public string Genre { get; set; } = "";
        public string? Language { get; set; }
        public decimal Price { get; set; }
    }
}
