using Lab2_LibraryWebAPI.Data;
using Lab2_LibraryWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab2_LibraryWebAPI.Extensions
{
    public static class SeriesControllerExtensions
    {
        public static async Task<Series> GetSeriesAsync(this string series, BooksDbContext context)
        {
            var existingSeries = await context.Series.FirstOrDefaultAsync(s => s.SeriesName == series);
            if (existingSeries != null)
                return existingSeries;
            else
                return new Series { SeriesName = series };
        }
    }
}
