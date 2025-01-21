using Lab2_LibraryWebAPI.Data;
using Lab2_LibraryWebAPI.DTOs;
using Lab2_LibraryWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab2_LibraryWebAPI.Extensions
{
    public static class AuthorControllerExtensions
    {
        public static async Task<List<Author>> GetAuthorsAsync(this List<AuthorNameDTO> authorNameDtos, BooksDbContext context)
        {
            var existingAuthors = new List<Author>();
            foreach (var author in authorNameDtos)
            {
                var existingAuthor = await context.Authors.FirstOrDefaultAsync(a => a.FirstName == author.FirstName && a.LastName == author.LastName);
                if (existingAuthor != null)
                    existingAuthors.Add(existingAuthor);
                else
                    existingAuthors.Add(author.ToAuthor());
            }
            return existingAuthors;
        }

        public static  IQueryable<Author> GetAuthorsByIds(this IQueryable<Author> authors, List<int> authorIds)
        {
            return authors.Where(a => authorIds.Contains(a.Id));
        }

        public static bool AuthorsExists(this IQueryable<Author> authors, List<int> authorIds)
        {
            return authorIds.All(id => authors.Any(a => a.Id == id));
        }

        public static IQueryable<Author> GetAuthors(this IQueryable<Author> authors, string firstName, string lastName)
        {
            return authors.Where(a => a.FirstName == firstName && a.LastName == lastName);
        }

        public static bool TryGetAuthorById(this IQueryable<Author> authors, int authorId, out Author author)
        {
            var authorExists = authors.Any(a => a.Id == authorId);

            author = authorExists ? authors.First(a => a.Id == authorId) : null!;

            return authorExists;
        }

        public static bool TryGetAuthorByName(this IQueryable<Author> authors, string firstName, string lastName, out Author author)
        {
            var authorExists = authors.Any(a => a.FirstName == firstName && a.LastName == lastName);

            author = authorExists ? authors.First(a => a.FirstName == firstName && a.LastName == lastName) : null!;

            return authorExists;
        }
    }
}
