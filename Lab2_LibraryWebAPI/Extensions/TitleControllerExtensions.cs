using Lab2_LibraryWebAPI.Data;
using Lab2_LibraryWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab2_LibraryWebAPI.Extensions
{
    public static class TitleControllerExtensions
    {
        public static async Task<Title> GetTitleAsync(this IQueryable<Title> titles, string title)
        {
            var existingTitle = await titles.FirstOrDefaultAsync(t => t.TitleName == title);
            if (existingTitle != null)
                return existingTitle;
            else
                return new Title { TitleName = title };
        }

        public static IQueryable<Title> GetTitles(this IQueryable<Title> titles, string title)
        {
            return titles.Where(t => t.TitleName == title);
        }

        public static IQueryable<Title> GetTitlesByIds(this IQueryable<Title> titles, List<int> titleIds)
        {
            return titles.Where(t => titleIds.Contains(t.Id));
        }

        public static bool TryGetTitleById(this IQueryable<Title> titles, int titleId, out Title title)
        {
            var titleExists = titles.Any(t => t.Id == titleId);

            title = titleExists ? titles.First(t => t.Id == titleId) : null!;

            return titleExists;
        }

        public static bool TryGetTitleByName(this IQueryable<Title> titles, string titleName, out Title title)
        {
            var titleExists = titles.Any(t => t.TitleName == titleName);

            title = titleExists ? titles.First(t => t.TitleName == titleName) : null!;

            return titleExists;
        }
    }
}
