using BookStoreApi.DTOs;
using BookStoreApi.Models;

namespace BookStoreApi.Interfaces;

public interface IBooksService
{
    Task<ServiceResponse<List<BookResponse>>> GetBooksAsync();
    Task<ServiceResponse<BookResponse>> GetBookByIdAsync(string id);
    Task<ServiceResponse<BookResponse>> AddBookAsync(BookRequest newBook);
    Task<ServiceResponse<BookResponse>> UpdateBookAsync(string id, BookRequest newBook);
    Task<ServiceResponse<bool>> RemoveBookAsync(string id);
}
