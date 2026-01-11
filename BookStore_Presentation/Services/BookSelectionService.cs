using BookStore_Domain;
using BookStore_Infrastrcuture.Data.Model;
using BookStore_Presentation.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BookStore_Presentation.Services
{
    public class BookService
    {

        private readonly BookStoreContext _context;

            public BookService(BookStoreContext context)
            {
                _context = context;
            }
                        public Book? UpdateBook(
                       string isbn13,
                       string title,
                       string? language,
                       decimal? price,
                       DateOnly? publicationDate,
                       int? pagecount,
                       int? genreID,
                       int? publisherId
                        )
                {

        
                    var book = _context.Books
                        .Include(b => b.Genre)
                        .Include(b => b.Publisher)
                        .Include(b => b.BookAuthors)
                               .ThenInclude(ba => ba.Author)
                               .FirstOrDefault(b => b.Isbn13 == isbn13);

      
                    _context.SaveChanges();
                    return book;
                }
            }
        }

