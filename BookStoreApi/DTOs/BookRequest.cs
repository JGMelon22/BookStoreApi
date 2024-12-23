using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace BookStoreApi.DTOs
{
    public record BookRequest(
        [property: BsonElement("Name")]
        [Required(ErrorMessage = "Book Name is a required field!")]
        [Length(1, 100, ErrorMessage = "Book Name must be between 1 and 100 characters long!")]
        string BookName,

        [Required(ErrorMessage = "Book price is a required field!")]
        [Range(1, 9999.99, ErrorMessage = $"Book Price must be between $1.0 and $9999.99")]
        decimal Price,

        [Required(ErrorMessage = "Book Category is a required field!")]
        [Length(1, 100, ErrorMessage = "Book Category must be between 1 and 100 characters long!")]
        string Category,

        [Required(ErrorMessage = "Book Author Name is a required field!")]
        [Length(1, 100, ErrorMessage = "Book Author Name must be between 1 and 100 characters long!")]
        string Author
    );
}
