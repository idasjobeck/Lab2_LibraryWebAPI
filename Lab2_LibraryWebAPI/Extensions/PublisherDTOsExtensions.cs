using Lab2_LibraryWebAPI.DTOs;
using Lab2_LibraryWebAPI.Models;

namespace Lab2_LibraryWebAPI.Extensions
{
    public static class PublisherDTOsExtensions
    {
        public static Publisher ToPublisher(this PublisherNameDTO publisherDto) =>
            new Publisher
            {
                Id = 0,
                PublisherName = publisherDto.PublisherName
            };

        public static PublisherNameDTO ToPublisherDTO(this Publisher publisher) =>
            new PublisherNameDTO
            {
                PublisherName = publisher.PublisherName
            };
    }
}
