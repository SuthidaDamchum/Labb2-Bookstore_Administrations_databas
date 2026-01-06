using BookStore_Domain;
using BookStore_Infrastrcuture.Data.Model;
using BookStore_Presentation.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore_Presentation.Services
{
    public class BookSelectionService
    {
        public BookAdminItem? SelectedBook { get; set; }


        private readonly BookStoreContext _context;

        public BookSelectionService(BookStoreContext context)
        {
            _context = context;
        }
        public Book UpdateBook(
            string isbn13, string title, string? language, decimal? price, DateOnly? publicationDate, int? pagecount, int? genreID, int? publisherId)
        {
            var book = _context.Books.FirstOrDefault(b => b.Isbn13 == isbn13);

            if (book == null)
                throw new Exception("Book not found");


            book.Title = title;
            book.Language = language;
            book.Price = price;
            book.PublicationDate = publicationDate;
            book.PageCount = pagecount;
            book.GenreId = genreID;
            book.PublisherId = publisherId;

            _context.SaveChanges();
            return book;

        }
    }
}

