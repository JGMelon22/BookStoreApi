using BookStoreApi.DTOs;
using BookStoreApi.Infrastructure.Data.Configuration;
using BookStoreApi.Infrastructure.Mapper;
using BookStoreApi.Interfaces;
using BookStoreApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BookStoreApi.Service;

public class BooksService : IBooksService
{
    private readonly IRedisCacheService _cacheService;
    private readonly IMongoCollection<Book> _booksCollection;
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(2);
    private readonly ILogger<BooksService> _logger;

    public BooksService(
        IOptions<BookStoreDatabaseSettings> bookStoreDatabaseSettings,
        IRedisCacheService cacheService,
        ILogger<BooksService> logger
        )
    {
        // Get MongoClient from the connection string in settings
        var mongoClient = new MongoClient(bookStoreDatabaseSettings.Value.ConnectionString);

        // Get the MongoDatabase from the client
        var mongoDatabase = mongoClient.GetDatabase(bookStoreDatabaseSettings.Value.DatabaseName);

        // Initialize the collection
        _booksCollection = mongoDatabase.GetCollection<Book>(bookStoreDatabaseSettings.Value.BooksCollectionName);

        _cacheService = cacheService;

        _logger = logger;
    }

    public async Task<ServiceResponse<BookResponse>> AddBookAsync(BookRequest newBook)
    {
        ServiceResponse<BookResponse> serviceResponse = new();

        try
        {
            var book = BookMapper.BookRequestToBook(newBook);
            await _booksCollection.InsertOneAsync(book);

            _logger.LogInformation("Book created successfully. Book: {0}", book);
        }

        catch (Exception ex)
        {
            _logger.LogError("Failed to create book. Error: {0}", ex.Message);

            serviceResponse.Success = false;
            serviceResponse.Message = ex.Message;
        }

        return serviceResponse;
    }

    public async Task<ServiceResponse<BookResponse>> GetBookByIdAsync(string id)
    {
        ServiceResponse<BookResponse> serviceResponse = new();
        string cacheKey = $"Book: {id}";
        var cachedBook = await _cacheService.GetCacheValueAsync<ServiceResponse<BookResponse>>(cacheKey);

        if (cachedBook != null)
        {
            _logger.LogInformation("Book retrieved from cache. Id: {0}", id);
            return cachedBook;
        }

        try
        {
            var book = await _booksCollection
                .Find(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (book == null)
            {
                _logger.LogWarning("Book not found. Id: {0}", id);

                serviceResponse.Success = false;
                serviceResponse.Message = $"Book with Id {id} was not found!";

                return serviceResponse;
            }

            serviceResponse.Data = BookMapper.BookToBookResponse(book);

            await _cacheService.SetCacheValueAsync(cacheKey, serviceResponse, CacheExpiration);

            _logger.LogInformation("Book retrieved successfully. Details: Id={0}, Name={1}, Category={2}, Author={3}, Price={4}",
                book.Id, book.BookName, book.Category, book.Author, book.Price);
        }

        catch (Exception ex)
        {
            _logger.LogError("Failed to retrieve book. Id: {0}. Error: {1}", id, ex.Message);

            serviceResponse.Success = false;
            serviceResponse.Message = ex.Message;
        }

        return serviceResponse;
    }

    public async Task<ServiceResponse<IEnumerable<BookResponse>>> GetBooksAsync()
    {
        ServiceResponse<IEnumerable<BookResponse>> serviceResponse = new();

        var cachedBooks = await _cacheService.GetCacheValueAsync<ServiceResponse<IEnumerable<BookResponse>>>("[books]");

        if (cachedBooks != null)
        {
            _logger.LogInformation("Retrieved {Count} books from cache", cachedBooks.Data!.Count());
            return cachedBooks;
        }

        try
        {
            var books = await _booksCollection
                .Find(_ => true)
                .ToListAsync();

            var booksResponse = books
                .Select(x => BookMapper.BookToBookResponse(x))
                .ToList();

            serviceResponse.Data = booksResponse;

            await _cacheService.SetCacheValueAsync("[books]", serviceResponse, CacheExpiration);

            _logger.LogInformation("Retrieved {0} books successfully", books.Count);
        }

        catch (Exception ex)
        {
            _logger.LogError("Failed to retrieve books. Error: {0}", ex.Message);

            serviceResponse.Success = false;
            serviceResponse.Message = ex.Message;
        }

        return serviceResponse;
    }

    public async Task<ServiceResponse<bool>> RemoveBookAsync(string id)
    {
        ServiceResponse<bool> serviceResponse = new();

        try
        {
            var book = await _booksCollection
                .DeleteOneAsync(x => x.Id == id);

            if (book.DeletedCount == 0)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Book with Id {id} not found!";
            }

            _logger.LogInformation("Book deleted successfully. Id: {0}", id);
        }

        catch (Exception ex)
        {
            _logger.LogError("Failed to delete book. Id: {0}. Error: {1}", id, ex.Message);

            serviceResponse.Success = false;
            serviceResponse.Message = ex.Message;
        }

        return serviceResponse;
    }

    public async Task<ServiceResponse<BookResponse>> UpdateBookAsync(string id, BookRequest updatedBook)
    {
        ServiceResponse<BookResponse> serviceResponse = new();

        try
        {
            var book = BookMapper.BookRequestToBook(updatedBook);
            book.Id = id;

            var result = await _booksCollection
                .ReplaceOneAsync(
                    x => x.Id == id,
                    book
                );

            if (result.MatchedCount == 0)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Book with Id {id} not found!";

                return serviceResponse;
            }

            var updatedDoc = await _booksCollection
                .Find(x => x.Id == id)
                .FirstOrDefaultAsync();

            serviceResponse.Data = BookMapper.BookToBookResponse(updatedDoc);

            _logger.LogInformation("Book updated successfully. Book Details: Id={0}, Name={1}, Category={2}, Author={3}, Price={4}",
                updatedDoc.Id, updatedDoc.BookName, updatedDoc.Category, updatedDoc.Author, updatedDoc.Price);
        }

        catch (Exception ex)
        {
            _logger.LogError("Failed to update book. Id: {0}. Error: {1}", id, ex.Message);

            serviceResponse.Success = false;
            serviceResponse.Message = ex.Message;
        }

        return serviceResponse;
    }
}
