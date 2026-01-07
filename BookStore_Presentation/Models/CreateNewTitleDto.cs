using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore_Presentation.Models
{
    public class CreateNewTitleDto
    {
        public string Isbn13 { get; set; } = null!;
        public string Title { get; set; } = null!;

        public string? Language { get; set; }

        public decimal? Price { get; set; }

        public DateOnly? PublicationDate { get; set; }

        public int? PageCount { get; set; }

        public int? GenreId { get; set; }

        public int? PublisherId { get; set; }

        public List<int> AuthorIds { get; set; } = new List<int>();

    }
}

