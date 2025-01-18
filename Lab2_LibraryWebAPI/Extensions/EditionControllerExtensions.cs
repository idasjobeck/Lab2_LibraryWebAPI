using Lab2_LibraryWebAPI.Data;
using Lab2_LibraryWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab2_LibraryWebAPI.Extensions
{
    public static class EditionControllerExtensions
    {
        public static async Task<Edition> GetEditionAsync(this string edition, BooksDbContext context)
        {
            var existingEdition = await context.Editions.FirstOrDefaultAsync(e => e.EditionName == edition);
            if (existingEdition != null)
                return existingEdition;
            else
                return new Edition { EditionName = edition };
        }
    }
}
