using Lab2_LibraryWebAPI.Data;
using Lab2_LibraryWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab2_LibraryWebAPI.Extensions
{
    public static class SeriesControllerExtensions
    {
        public static async Task<Series> GetSeriesAsync(this IQueryable<Series> series, string seriesName)
        {
            var existingSeries = await series.FirstOrDefaultAsync(s => s.SeriesName == seriesName);
            if (existingSeries != null)
                return existingSeries;
            else
                return new Series { SeriesName = seriesName };
        }

        public static IQueryable<Series> GetSeries(this IQueryable<Series> series, string seriesName)
        {
            return series.Where(s => s.SeriesName == seriesName);
        }

        public static IQueryable<Series> GetSeriesByIds(this IQueryable<Series> series, List<int> seriesIds)
        {
            return series.Where(s => seriesIds.Contains(s.Id));
        }

        public static bool TryGetSeriesById(this IQueryable<Series> series, int seriesId, out Series existingSeries)
        {
            var seriesExists = series.Any(s => s.Id == seriesId);
            existingSeries = seriesExists ? series.First(s => s.Id == seriesId) : null!;
            return seriesExists;
        }

        public static bool TryGetSeriesById(this IQueryable<Series> series, int? seriesId, out Series existingSeries)
        {
            if (seriesId == null)
            {
                existingSeries = null!;
                return false;
            }

            var seriesExists = series.Any(s => s.Id == seriesId);
            existingSeries = seriesExists ? series.First(s => s.Id == seriesId) : null!;
            return seriesExists;
        }

        public static bool TryGetSeriesByName(this IQueryable<Series> series, string seriesName, out Series existingSeries)
        {
            var seriesExists = series.Any(s => s.SeriesName == seriesName);
            existingSeries = seriesExists ? series.First(s => s.SeriesName == seriesName) : null!;
            return seriesExists;
        }
    }
}
