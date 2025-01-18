using Lab2_LibraryWebAPI.Data;
using Lab2_LibraryWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab2_LibraryWebAPI.Extensions
{
    public static class GenreControllerExtensions
    {
        public static async Task<Genre> GetGenreAsync(this string genre, BooksDbContext context)
        {
            var existingGenre = await context.Genres.FirstOrDefaultAsync(g => g.GenreName == genre);
            if (existingGenre != null)
                return existingGenre;
            else
                return new Genre { GenreName = genre };
        }
    }
}
