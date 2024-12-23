using MongoDB.Bson.Serialization.Attributes;

namespace BookStoreApi.DTOs
{
    public record BookRequest(
        [property: BsonElement("Name")]
        string BookName,

        decimal Price,
        string Category,
        string Author
    );
}
