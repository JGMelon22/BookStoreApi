using BookStoreApi.DTOs;
using BookStoreApi.Models;

namespace BookStoreApi.Infrastructure.Mapper;

public static class BookMapper
{
    public static BookResponse BookToBookResponse(this Book book)
    {
        return new BookResponse
        {
            Id = book.Id,
            BookName = book.BookName,
            Price = book.Price,
            Category = book.Category,
            Author = book.Author
        };
    }

    public static Book BookRequestToBook(this BookRequest book)
    {
        return new Book
        {
            BookName = book.BookName,
            Price = book.Price,
            Category = book.Category,
            Author = book.Author
        };
    }
}
