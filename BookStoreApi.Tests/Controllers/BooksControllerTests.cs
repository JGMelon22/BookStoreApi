using BookStoreApi.Controllers;
using BookStoreApi.DTOs;
using BookStoreApi.Interfaces;
using BookStoreApi.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Moq;

namespace BookStoreApi.Tests.Controllers;

public class BooksControllerTests
{
    [Fact]
    public async Task Should_ReturnSucess_When_NewBookIsValid()
    {
        // Arrange
        Mock<IBooksService> service = new();
        BookRequest newBook = new BookRequest(
                        BookName: "The Great Gatsby",
                        Price: 15.99m,
                        Category: "Fiction",
                        Author: "F. Scott Fitzgerald"
                    );
        BookResponse bookResponse = new()
        {

            Id = ObjectId.GenerateNewId().ToString(),
            BookName = newBook.BookName,
            Price = newBook.Price,
            Category = newBook.Category,
            Author = newBook.Author,
        };
        BooksController controller = new(service.Object);

        ServiceResponse<BookResponse> serviceResponse = new()
        {
            Data = bookResponse,
            Success = true,
            Message = string.Empty
        };

        service
            .Setup(x => x.AddBookAsync(It.IsAny<BookRequest>()))
            .ReturnsAsync(serviceResponse);

        // Act
        var result = await controller.AddBookAsync(newBook);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<OkObjectResult>();
        serviceResponse.Data.Should().Be(bookResponse);
    }

    [Fact]
    public async Task Should_ReturnSuccess_WhenBookCollectionIsNotEmpty()
    {
        // Arrange
        Mock<IBooksService> service = new();
        IEnumerable<BookResponse> books = new List<BookResponse>
        {
            new BookResponse
            {
                Id = ObjectId.GenerateNewId().ToString(),
                BookName = "The Great Gatsby",
                Price = 10.99m,
                Category = "Fiction",
                Author = "F. Scott Fitzgerald"
            },
            new BookResponse
            {
                Id = ObjectId.GenerateNewId().ToString(),
                BookName = "1984",
                Price = 9.99m,
                Category = "Dystopian",
                Author = "George Orwell"
            },
            new BookResponse
            {
                Id = ObjectId.GenerateNewId().ToString(),
                BookName = "To Kill a Mockingbird",
                Price = 12.99m,
                Category = "Fiction",
                Author = "Harper Lee"
            }
        };
        BooksController controller = new(service.Object);

        ServiceResponse<IEnumerable<BookResponse>> serviceResponse = new()
        {
            Data = books,
            Success = true,
            Message = string.Empty
        };

        service
            .Setup(x => x.GetBooksAsync())
                .ReturnsAsync(serviceResponse);

        // Act
        var result = await controller.GetBooksAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<OkObjectResult>();
        serviceResponse.Data.Count().Should().Be(3);

        service.Verify(x => x.GetBooksAsync(), Times.Once);
    }

    [Fact]
    public async Task Should_ReturnSuccess_When_BookWithProvidedIdIsFound()
    {
        // Arrange
        Mock<IBooksService> service = new();
        BookResponse bookResponse = new()
        {
            Id = "507f1f77bcf86cd799439011",
            BookName = "Mock Book Title",
            Price = 19.99m,
            Category = "Science Fiction",
            Author = "John Doe"
        };

        BooksController controller = new(service.Object);

        ServiceResponse<BookResponse> serviceResponse = new()
        {
            Data = bookResponse,
            Success = true,
            Message = string.Empty
        };

        service
            .Setup(x => x.GetBookByIdAsync(It.IsAny<String>()))
            .ReturnsAsync(serviceResponse);

        // Act
        var result = await controller.GetBookByIdAsync("507f1f77bcf86cd799439011");

        // Assert
        result.Should().NotBeNull();
        serviceResponse.Data.Should().Be(bookResponse);
        result.Should().BeOfType<OkObjectResult>();

        service.Verify(x => x.GetBookByIdAsync("507f1f77bcf86cd799439011"), Times.Once);
    }

    [Fact]
    public async Task Should_ReturnSuccess_When_BookToUpdateIsFoundAndValidInput()
    {
        // Arrange
        Mock<IBooksService> service = new();
        BookRequest updatedBook = new BookRequest(
                        BookName: "The Great Gatsby",
                        Price: 15.99m,
                        Category: "Fiction",
                        Author: "F. Scott Fitzgerald"
                    );
        BookResponse bookResponse = new()
        {
            Id = "507f1f77bcf86cd799439011",
            BookName = "Mock Book Title",
            Price = 19.99m,
            Category = "Science Fiction",
            Author = "John Doe"
        };

        BooksController controller = new(service.Object);

        ServiceResponse<BookResponse> serviceResponse = new()
        {
            Data = bookResponse,
            Success = true,
            Message = string.Empty
        };

        service
            .Setup(x => x.UpdateBookAsync(It.IsAny<String>(), It.IsAny<BookRequest>()))
            .ReturnsAsync(serviceResponse);

        // Act
        var result = await controller.UpdateBookAsync("507f1f77bcf86cd799439011", updatedBook);

        // Assert
        result.Should().NotBeNull();
        serviceResponse.Data.Should().Be(bookResponse);
        result.Should().BeOfType<OkObjectResult>();

        service.Verify(x => x.UpdateBookAsync("507f1f77bcf86cd799439011", updatedBook), Times.Once);
    }

    [Fact]
    public async Task Should_ReturnSuccess_When_BookToBeRemovedIsFound()
    {
        // Arrange
        Mock<IBooksService> service = new();
        BooksController controller = new(service.Object);

        ServiceResponse<bool> serviceResponse = new()
        {
            Success = true,
            Message = string.Empty
        };

        service
            .Setup(x => x.RemoveBookAsync(It.IsAny<String>()))
            .ReturnsAsync(serviceResponse);

        // Act
        var result = await controller.RemoveBookAsync("507f1f77bcf86cd799439011");

        // Assert
        result.Should().NotBeNull();
        serviceResponse.Success.Should().BeTrue();
        result.Should().BeOfType<NoContentResult>();

        service.Verify(x => x.RemoveBookAsync("507f1f77bcf86cd799439011"), Times.Once);
    }
}
