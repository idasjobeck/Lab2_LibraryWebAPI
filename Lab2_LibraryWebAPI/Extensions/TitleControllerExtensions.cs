using Lab2_LibraryWebAPI.Data;
using Lab2_LibraryWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab2_LibraryWebAPI.Extensions
{
    public static class TitleControllerExtensions
    {
        public static async Task<Title> GetTitleAsync(this string title, BooksDbContext context)
        {
            var existingTitle = await context.Titles.FirstOrDefaultAsync(t => t.TitleName == title);
            if (existingTitle != null)
                return existingTitle;
            else
                return new Title { TitleName = title };
        }
    }
}
