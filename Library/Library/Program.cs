using Library.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

var builder = new ConfigurationBuilder();
builder.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
IConfiguration  configuration = builder.Build();
string? connectionString = configuration.GetConnectionString("Library");

var client = new MongoClient(connectionString);
var database = client.GetDatabase("Library");
var authorCollection = database.GetCollection<Author>("Authors");
var bookCollection = database.GetCollection<Book>("Books");