using Lab2_LibraryWebAPI.DTOs;
using Lab2_LibraryWebAPI.Models;

namespace Lab2_LibraryWebAPI.Extensions
{
    public static class SeriesDTOsExtensions
    {
        public static Series ToSeries(this SeriesNameDTO seriesDto) =>
            new Series
            {
                Id = 0,
                SeriesName = seriesDto.SeriesName
            };

        public static SeriesNameDTO ToSeriesDTO(this Series series) =>
            new SeriesNameDTO
            {
                SeriesName = series.SeriesName
            };
    }
}
