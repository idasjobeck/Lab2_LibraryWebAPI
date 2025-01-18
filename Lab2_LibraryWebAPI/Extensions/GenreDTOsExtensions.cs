using Lab2_LibraryWebAPI.DTOs;
using Lab2_LibraryWebAPI.Models;

namespace Lab2_LibraryWebAPI.Extensions
{
    public static class GenreDTOsExtensions
    {
        public static Genre ToGenre(this GenreNameDTO genre) =>
            new Genre
            {
                Id = 0,
                GenreName = genre.GenreName
            };

        public static GenreNameDTO ToGenreDTO(this Genre genre) =>
            new GenreNameDTO
            {
                GenreName = genre.GenreName
            };
    }
}
