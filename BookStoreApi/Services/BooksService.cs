using BookStoreApi.DTOs;
using BookStoreApi.Interfaces;
using BookStoreApi.Models;
using MongoDB.Driver;

namespace BookStoreApi.Service;

public class BooksService : IBooksService
{
    private readonly IMongoCollection<Book> _booksCollection;

    public BooksService(IMongoCollection<Book> booksCollection)
    {
        _booksCollection = booksCollection;
    }

    public Task<ServiceResponse<BookResponse>> AddBookAsync(BookRequest newBook)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponse<BookResponse>> GetBookByIdAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponse<List<BookResponse>>> GetBooksAsync()
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponse<bool>> RemoveBookAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponse<BookResponse>> UpdateBookAsync(string id, BookRequest newBook)
    {
        throw new NotImplementedException();
    }
}
