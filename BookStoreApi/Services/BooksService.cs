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

    public BooksService(
        IOptions<BookStoreDatabaseSettings> bookStoreDatabaseSettings,
        IRedisCacheService cacheService
        )
    {
        // Get MongoClient from the connection string in settings
        var mongoClient = new MongoClient(bookStoreDatabaseSettings.Value.ConnectionString);

        // Get the MongoDatabase from the client
        var mongoDatabase = mongoClient.GetDatabase(bookStoreDatabaseSettings.Value.DatabaseName);

        // Initialize the collection
        _booksCollection = mongoDatabase.GetCollection<Book>(bookStoreDatabaseSettings.Value.BooksCollectionName);

        _cacheService = cacheService;
    }

    public async Task<ServiceResponse<BookResponse>> AddBookAsync(BookRequest newBook)
    {
        ServiceResponse<BookResponse> serviceResponse = new();

        try
        {
            var book = BookMapper.BookRequestToBook(newBook);
            await _booksCollection.InsertOneAsync(book);
        }

        catch (Exception ex)
        {
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
            return cachedBook;

        try
        {
            var book = await _booksCollection
                .Find(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (book == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Book with Id {id} was not found!";

                return serviceResponse;
            }

            serviceResponse.Data = BookMapper.BookToBookResponse(book);

            await _cacheService.SetCacheValueAsync(cacheKey, serviceResponse, CacheExpiration);

        }

        catch (Exception ex)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = ex.Message;
        }

        return serviceResponse;
    }

    public async Task<ServiceResponse<IEnumerable<BookResponse>>> GetBooksAsync()
    {
        ServiceResponse<IEnumerable<BookResponse>> serviceResponse = new();
        // var cachedBook = await _cacheService.GetCacheValueAsync<ServiceResponse<BookResponse>>(cacheKey);
        var cachedBooks = _cacheService.GetCacheValueAsync<IEnumerable<ServiceResponse<BookResponse>>>("[controller]");
        try
        {
            var books = await _booksCollection
                .Find(_ => true)
                .ToListAsync();

            var booksResponse = books
                .Select(x => BookMapper.BookToBookResponse(x))
                .ToList();

            serviceResponse.Data = booksResponse;

            await _cacheService.SetCacheValueAsync("[controller]", serviceResponse, CacheExpiration);
        }

        catch (Exception ex)
        {
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
        }

        catch (Exception ex)
        {
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
        }

        catch (Exception ex)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = ex.Message;
        }

        return serviceResponse;
    }
}
