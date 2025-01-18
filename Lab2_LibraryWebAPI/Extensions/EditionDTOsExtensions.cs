using Lab2_LibraryWebAPI.DTOs;
using Lab2_LibraryWebAPI.Models;

namespace Lab2_LibraryWebAPI.Extensions
{
    public static class EditionDTOsExtensions
    {
        public static Edition ToEdition(this EditionNameDTO editionDto) =>
            new Edition
            {
                Id = 0,
                EditionName = editionDto.EditionName
            };

        public static EditionNameDTO ToEditionDTO(this Edition edition) =>
            new EditionNameDTO
            {
                EditionName = edition.EditionName
            };
    }
}
