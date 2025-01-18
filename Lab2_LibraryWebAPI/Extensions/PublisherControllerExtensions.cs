using Lab2_LibraryWebAPI.Data;
using Lab2_LibraryWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab2_LibraryWebAPI.Extensions
{
    public static class PublisherControllerExtensions
    {
        public static async Task<Publisher> GetPublisherAsync(this string publisher, BooksDbContext context)
        {
            var existingPublisher = await context.Publishers.FirstOrDefaultAsync(p => p.PublisherName == publisher);
            if (existingPublisher != null)
                return existingPublisher;
            else
                return new Publisher { PublisherName = publisher };
        }
    }
}
