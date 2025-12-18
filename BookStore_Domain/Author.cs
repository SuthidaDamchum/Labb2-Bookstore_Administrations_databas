using System;
using System.Collections.Generic;

namespace BookStore_Domain;

public partial class Author
{
    public int AuthorId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateOnly? BirthDate { get; set; }

    public virtual ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
}
