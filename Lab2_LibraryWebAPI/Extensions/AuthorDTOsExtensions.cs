using Lab2_LibraryWebAPI.DTOs;
using Lab2_LibraryWebAPI.Models;

namespace Lab2_LibraryWebAPI.Extensions
{
    public static class AuthorDTOsExtensions
    {
        public static Author ToAuthor(this AuthorNameDTO authorNameDto)
        {
            return new Author
            {
                FirstName = authorNameDto.FirstName,
                LastName = authorNameDto.LastName
            };
        }

        public static List<Author> ToAuthors(this List<AuthorNameDTO> authorNameDtos)
        {
            return authorNameDtos.Select(a => a.ToAuthor()).ToList();
        }

        public static AuthorNameDTO ToAuthorNameDTO(this Author author)
        {
            return new AuthorNameDTO
            {
                FirstName = author.FirstName,
                LastName = author.LastName
            };
        }

        public static List<AuthorNameDTO> ToAuthorNameDTOs(this List<Author> authors)
        {
            return authors.Select(a => a.ToAuthorNameDTO()).ToList();
        }

        public static AuthorNameWithIdDTO ToAuthorNameWithIdDTO(this Author author)
        {
            return new AuthorNameWithIdDTO()
            {
                Id = author.Id,
                FirstName = author.FirstName,
                LastName = author.LastName
            };
        }

        public static List<AuthorNameWithIdDTO> ToAuthorNameWithIdDTOs(this List<Author> authors)
        {
            return authors.Select(a => a.ToAuthorNameWithIdDTO()).ToList();
        }

        public static string ToAuthorsAsString(this List<Author> authors)
        {
            return string.Join(", ", authors.Select(a => $"{a.FirstName} {a.LastName}"));
        }
    }
}
