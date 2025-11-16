using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Library.Entities;
internal class Author(
    string name,
    string country
    )
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; private set; } 
    public string Name { get; private set; } = name;
    public string Country { get; private set; } = country;

    [BsonRepresentation(BsonType.DateTime)]
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    public override string ToString()
    {
        return $"Id: {Id}\nName: {Name}\nCountry: {Country}";
    }
}
