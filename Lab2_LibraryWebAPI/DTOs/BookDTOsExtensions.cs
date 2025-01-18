﻿using Lab2_LibraryWebAPI.Extensions;
using Lab2_LibraryWebAPI.Models;

namespace Lab2_LibraryWebAPI.DTOs
{
    public static class BookDTOsExtensions
    {
        public static BookWithoutAuthorDTO toBookWithoutAuthorDTO(this CreateBookDTO createBookDto)
        {
            return new BookWithoutAuthorDTO
            {
                Title = createBookDto.Title,
                Series = createBookDto.Series,
                NumberInSeries = createBookDto.NumberInSeries,
                Genre = createBookDto.Genre,
                ISBN = createBookDto.ISBN,
                PublishedYear = createBookDto.PublishedYear,
                Publisher = createBookDto.Publisher,
                Edition = createBookDto.Edition,
                TotalQty = createBookDto.TotalQty,
                AvailableQty = createBookDto.AvailableQty
            };
        }

        public static BookWithoutAuthorDTO toBookWithoutAuthorDTO(this CreateBookWithAuthorIdDTO createBookWithAuthorIdDto)
        {
            return new BookWithoutAuthorDTO
            {
                Title = createBookWithAuthorIdDto.Title,
                Series = createBookWithAuthorIdDto.Series,
                NumberInSeries = createBookWithAuthorIdDto.NumberInSeries,
                Genre = createBookWithAuthorIdDto.Genre,
                ISBN = createBookWithAuthorIdDto.ISBN,
                PublishedYear = createBookWithAuthorIdDto.PublishedYear,
                Publisher = createBookWithAuthorIdDto.Publisher,
                Edition = createBookWithAuthorIdDto.Edition,
                TotalQty = createBookWithAuthorIdDto.TotalQty,
                AvailableQty = createBookWithAuthorIdDto.AvailableQty
            };
        }

        public static DisplayBookDTO ToDisplayBookDTO(this Book book)
        {
            return new DisplayBookDTO
            {
                Title = book.Title.ToTitleDTO(),
                Series = book.Series?.ToSeriesDTO(),
                NumberInSeries = book.NumberInSeries,
                Authors = book.Authors.ToAuthorNameDTOs(),
                Genre = book.Genre.ToGenreDTO(),
                ISBN = book.ISBN,
                PublishedYear = book.PublishedYear.Year,
                Publisher = book.Publisher.ToPublisherDTO(),
                Edition = book.Edition.ToEditionDTO(),
                TotalQty = book.TotalQty,
                AvailableQty = book.AvailableQty
            };
        }

        public static DisplayBookWithIdsDTO ToDisplayBookWithIdsDTO(this Book book)
        {
            return new DisplayBookWithIdsDTO
            {
                Title = book.Title,
                Series = book.Series,
                NumberInSeries = book.NumberInSeries,
                Authors = book.Authors.ToAuthorNameWithIdDTOs(),
                Genre = book.Genre,
                ISBN = book.ISBN,
                PublishedYear = book.PublishedYear.Year,
                Publisher = book.Publisher,
                Edition = book.Edition,
                TotalQty = book.TotalQty,
                AvailableQty = book.AvailableQty
            };
        }
    }
}
