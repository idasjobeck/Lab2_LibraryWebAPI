using Lab2_LibraryWebAPI.DTOs;
using Lab2_LibraryWebAPI.Models;

namespace Lab2_LibraryWebAPI.Extensions
{
    public static class TitleDTOsExtensions
    {
        public static Title ToTitle(this TitleNameDTO titleDto) =>
            new Title
            {
                Id = 0,
                TitleName = titleDto.TitleName
            };

        public static TitleNameDTO ToTitleDTO(this Title title) =>
            new TitleNameDTO
            {
                TitleName = title.TitleName
            };
    }
}
