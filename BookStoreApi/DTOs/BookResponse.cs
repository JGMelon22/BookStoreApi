using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookStoreApi.DTOs;

public record BookResponse
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; init; }

    [BsonElement("Name")]
    public string BookName { get; init; } = null!;

    public decimal Price { get; init; }
    public string Category { get; init; } = null!;
    public string Author { get; init; } = null!;
}
