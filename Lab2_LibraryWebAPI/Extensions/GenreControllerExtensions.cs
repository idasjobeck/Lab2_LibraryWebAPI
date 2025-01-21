using Lab2_LibraryWebAPI.Data;
using Lab2_LibraryWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab2_LibraryWebAPI.Extensions
{
    public static class GenreControllerExtensions
    {
        public static async Task<Genre> GetGenreAsync(this IQueryable<Genre> genres, string genre)
        {
            var existingGenre = await genres.FirstOrDefaultAsync(g => g.GenreName == genre);
            if (existingGenre != null)
                return existingGenre;
            else
                return new Genre { GenreName = genre };
        }

        public static IQueryable<Genre> GetGenres(this IQueryable<Genre> genres, string genre)
        {
            return genres.Where(g => g.GenreName == genre);
        }

        public static IQueryable<Genre> GetGenresByIds(this IQueryable<Genre> genres, List<int> genreIds)
        {
            return genres.Where(g => genreIds.Contains(g.Id));
        }

        public static bool TryGetGenreById(this IQueryable<Genre> genres, int genreId, out Genre genre)
        {
            var genreExists = genres.Any(g => g.Id == genreId);

            genre = genreExists ? genres.First(g => g.Id == genreId) : null!;

            return genreExists;
        }

        public static bool TryGetGenreByName(this IQueryable<Genre> genres, string genreName, out Genre genre)
        {
            var genreExists = genres.Any(g => g.GenreName == genreName);

            genre = genreExists ? genres.First(g => g.GenreName == genreName) : null!;

            return genreExists;
        }
    }
}
