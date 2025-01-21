using Lab2_LibraryWebAPI.Data;
using Lab2_LibraryWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab2_LibraryWebAPI.Extensions
{
    public static class PublisherControllerExtensions
    {
        public static async Task<Publisher> GetPublisherAsync(this IQueryable<Publisher> publishers, string publisher)
        {
            var existingPublisher = await publishers.FirstOrDefaultAsync(p => p.PublisherName == publisher);
            if (existingPublisher != null)
                return existingPublisher;
            else
                return new Publisher { PublisherName = publisher };
        }

        public static IQueryable<Publisher> GetPublishers(this IQueryable<Publisher> publishers, string publisher)
        {
            return publishers.Where(p => p.PublisherName == publisher);
        }

        public static IQueryable<Publisher> GetPublishersByIds(this IQueryable<Publisher> publishers, List<int> publisherIds)
        {
            return publishers.Where(p => publisherIds.Contains(p.Id));
        }

        public static bool TryGetPublisherById(this IQueryable<Publisher> publishers, int publisherId, out Publisher publisher)
        {
            var genreExists = publishers.Any(p => p.Id == publisherId);

            publisher = genreExists ? publishers.First(p => p.Id == publisherId) : null!;

            return genreExists;
        }

        public static bool TryGetPublisherByName(this IQueryable<Publisher> publishers, string publisherName, out Publisher publisher)
        {
            var genreExists = publishers.Any(p => p.PublisherName == publisherName);

            publisher = genreExists ? publishers.First(p => p.PublisherName == publisherName) : null!;

            return genreExists;
        }
    }
}
