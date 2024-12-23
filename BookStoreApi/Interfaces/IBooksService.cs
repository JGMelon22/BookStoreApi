using BookStoreApi.DTOs;
using BookStoreApi.Models;

namespace BookStoreApi.Interfaces;

public interface IBooksService
{
    Task<ServiceResponse<IEnumerable<BookResponse>>> GetBooksAsync();
    Task<ServiceResponse<BookResponse>> GetBookByIdAsync(string id);
    Task<ServiceResponse<BookResponse>> AddBookAsync(BookRequest newBook);
    Task<ServiceResponse<BookResponse>> UpdateBookAsync(string id, BookRequest updatedBook);
    Task<ServiceResponse<bool>> RemoveBookAsync(string id);
}
