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
    }
}
