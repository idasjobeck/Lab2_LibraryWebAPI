using Lab2_LibraryWebAPI.Data;
using Lab2_LibraryWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab2_LibraryWebAPI.Extensions
{
    public static class EditionControllerExtensions
    {
        public static async Task<Edition> GetEditionAsync(this IQueryable<Edition> editions, string edition)
        {
            var existingEdition = await editions.FirstOrDefaultAsync(e => e.EditionName == edition);
            if (existingEdition != null)
                return existingEdition;
            else
                return new Edition { EditionName = edition };
        }

        public static IQueryable<Edition> GetEditions(this IQueryable<Edition> editions, string edition)
        {
            return editions.Where(e => e.EditionName == edition);
        }

        public static IQueryable<Edition> GetEditionsByIds(this IQueryable<Edition> editions, List<int> editionIds)
        {
            return editions.Where(e => editionIds.Contains(e.Id));
        }

        public static bool TryGetEditionById(this IQueryable<Edition> editions, int editionId, out Edition edition)
        {
            var editionExists = editions.Any(e => e.Id == editionId);

            edition = editionExists ? editions.First(e => e.Id == editionId) : null!;

            return editionExists;
        }

        public static bool TryGetEditionByName(this IQueryable<Edition> editions, string editionName, out Edition edition)
        {
            var editionExists = editions.Any(e => e.EditionName == editionName);

            edition = editionExists ? editions.First(e => e.EditionName == editionName) : null!;

            return editionExists;
        }
    }
}
