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
    public async Task<IActionResult> GetBooksAsync()
    {
        var books = await _booksService.GetBooksAsync();
        return books.Data != null || books.Data!.Any()
            ? Ok(books)
            : NoContent();
    }
}
