using BookStoreApi.DTOs;
using BookStoreApi.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBooksService _booksService;

    public BooksController(IBooksService booksService)
    {
        _booksService = booksService;
    }

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

    [HttpDelete("{id:length(24)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateBookAsync([FromRoute] string id)
    {
        var book = await _booksService.RemoveBookAsync(id);
        return book.Success != false
            ? NoContent()
            : NotFound(book);
    }
}
