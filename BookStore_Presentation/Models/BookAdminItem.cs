using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore_Presentation.Models
{
    public class BookAdminItem
    {
        public string Isbn13 { get; set; } = "";
        public string Title { get; set; } = "";
        public string AuthorNameString { get; set; } = "";
        public string GenreName { get; set; } = "";
        public string PublisherName { get; set; } = "";
        public string? Language { get; set; }
        public decimal Price { get; set; }
        public DateOnly? PublicationDate { get; set; }
        public int? PageCount { get; set; }
        public int[] AuthorIds { get; set; } = [];
    }
}





