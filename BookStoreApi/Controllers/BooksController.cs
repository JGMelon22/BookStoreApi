using BookStoreApi.DTOs;
using BookStoreApi.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBooksService _booksService;

    /// <summary>
    /// Initializes a new instance of the <see cref="BooksController"/> class.
    /// </summary>
    /// <param name="booksService">The service for handling book-related operations.</param>
    /// <remarks>
    /// The constructor takes an <paramref name="booksService"/> which is used to interact with the data layer to manage book operations.
    /// </remarks>
    public BooksController(IBooksService booksService)
    {
        _booksService = booksService;
    }

    /// <summary>
    /// Retrieves a list of all books.
    /// </summary>
    /// <returns>A list of books.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /api/Books
    ///     Returns all books available in the database. If no books exist, a 204 No Content response is returned.
    /// </remarks>
    /// <response code="200">Returns a list of books.</response>
    /// <response code="204">If no books are available.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetBooksAsync()
    {
        var books = await _booksService.GetBooksAsync();
        return books.Data != null || books.Data!.Any()
            ? Ok(books)
            : NoContent();
    }

    /// <summary>
    /// Retrieves a book by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the book.</param>
    /// <returns>The book with the specified id.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /api/Books/{id}
    ///     Returns the book that matches the specified <paramref name="id"/>. If no book is found, a 404 Not Found response is returned.
    /// </remarks>
    /// <response code="200">Returns the found book.</response>
    /// <response code="404">If no book with the specified id is found.</response>
    [HttpGet("{id:length(24)}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBookByIdAsync(string id)
    {
        var book = await _booksService.GetBookByIdAsync(id);
        return book.Data != null
            ? Ok(book)
            : NotFound(book);
    }

    /// <summary>
    /// Adds a new book to the store.
    /// </summary>
    /// <param name="newBook">The details of the new book to be added.</param>
    /// <returns>The added book.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/Books
    ///     {
    ///         "title": "The Great Adventure",
    ///         "author": "John Doe",
    ///         "price": 29.99
    ///     }
    ///     Adds a new book to the database. If the request body is properly formatted and valid, the book will be added.
    /// </remarks>
    /// <response code="200">Returns the added book.</response>
    /// <response code="400">If the book request is invalid or the format is incorrect.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddBookAsync([FromBody] BookRequest newBook)
    {
        var book = await _booksService.AddBookAsync(newBook);
        return book.Data != null
            ? Ok(book)
            : BadRequest(book);
    }

    /// <summary>
    /// Updates an existing book's details.
    /// </summary>
    /// <param name="id">The unique identifier of the book to update.</param>
    /// <param name="updatedBook">The updated book details.</param>
    /// <returns>The updated book.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     PATCH /api/Books/{id}
    ///     {
    ///         "title": "Updated Adventure",
    ///         "author": "Jane Doe",
    ///         "price": 24.99
    ///     }
    ///     This updates the details of the specified book. If the book does not exist, a 400 Bad Request response is returned.
    /// </remarks>
    /// <response code="200">Returns the updated book.</response>
    /// <response code="400">If the request is badly formatted or the book cannot be found.</response>
    [HttpPatch("{id:length(24)}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateBookAsync([FromRoute] string id, [FromBody] BookRequest updatedBook)
    {
        var book = await _booksService.UpdateBookAsync(id, updatedBook);
        return book.Data != null
            ? Ok(book)
            : BadRequest(book);
    }

    /// <summary>
    /// Removes a book from the store.
    /// </summary>
    /// <param name="id">The unique identifier of the book to remove.</param>
    /// <returns>A response indicating the result of the removal.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     DELETE /api/Books/{id}
    ///     Removes the book with the specified <paramref name="id"/>. If the book is successfully removed, a 204 No Content response is returned.
    /// </remarks>
    /// <response code="204">Successfully removed the book.</response>
    /// <response code="404">If no book is found with the specified id.</response>
    [HttpDelete("{id:length(24)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveBookAsync([FromRoute] string id)
    {
        var book = await _booksService.RemoveBookAsync(id);
        return book.Success != false
            ? NoContent()
            : NotFound(book);
    }
}
