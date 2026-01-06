using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookStore_Domain;
using BookStore_Infrastrcuture.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace BookStore_Presentation.Services
{
    public class AuthorService
    {
        private readonly BookStoreContext _context;

        public AuthorService(BookStoreContext context)
        {
            _context = context;
        }

        public Author CreateAuthor(string firstName, string lastName, DateOnly? birthDate)
        {
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("First and last name are required.");

            var author = new Author
            {
                FirstName = firstName,
                LastName = lastName,
                BirthDay = birthDate
            };

            _context.Authors.Add(author);
            _context.SaveChanges();

            return author;
        }

        public Author UpdateAuthor(int authorId, string firstName, string lastName, DateOnly? birthday)
        {
            var author = _context.Authors.Find(authorId);
            if (author == null)
                throw new Exception("Author not found.");


            author.FirstName = firstName;
            author.LastName = lastName;
            author.BirthDay = birthday;

            _context.SaveChanges();
            return author;
        }

        public void DeleteAuthor(int authorId)
        {
            var author = _context.Authors
                    
            .Include(a => a.BookAuthors)
            .FirstOrDefault(a => a.AuthorId == authorId);

            if (author == null)
                throw new Exception("Author not found.");

            if (author.BookAuthors != null && author.BookAuthors.Any())
                throw new Exception("Cannot delete author linked to books");

            _context.Authors.Remove(author);
            _context.SaveChanges();
        }
    }
}
