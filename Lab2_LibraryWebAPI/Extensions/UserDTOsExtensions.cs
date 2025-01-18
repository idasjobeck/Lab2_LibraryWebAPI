using Lab2_LibraryWebAPI.DTOs;
using Lab2_LibraryWebAPI.Models;

namespace Lab2_LibraryWebAPI.Extensions
{
    public static class UserDTOsExtensions
    {
        public static DisplayUserDTO ToDisplayUserDTO(this User user) =>
            new DisplayUserDTO
            {
                CardNumber = user.CardNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };

        public static DisplayUserWithIdDTO ToDisplayUserWithIdDTO(this User user) =>
            new DisplayUserWithIdDTO
            {
                Id = user.Id,
                CardNumber = user.CardNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };
    }
}
