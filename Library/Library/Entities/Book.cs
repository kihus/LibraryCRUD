using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Library.Entities;
internal class Book(
    string title,
    string authorId,
    int year
    )
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; private set; }
    public string Title { get; private set; } = title;
    public string AuthorId { get; private set; } = authorId;
    public int Year { get; private set; } = year;

    [BsonRepresentation(BsonType.DateTime)]
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;


    public override string ToString()
    {
        return $"Id: {Id}\nTitle: {Title}\nAuthor id: {AuthorId}\nYear: {Year}";
    }
}
