using Library.Entities;
using Library.Models;
using Library.Services;
using Library.Utils;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.ComponentModel.Design;

var builder = new ConfigurationBuilder();
builder.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
IConfiguration configuration = builder.Build();
string? connectionString = configuration.GetConnectionString("Library");

var client = new MongoClient(connectionString);
var database = client.GetDatabase("Library");
var authorCollection = database.GetCollection<Author>("Authors");
var bookCollection = database.GetCollection<Book>("Books");

var authorService = new AuthorService();

var menu = new MenuGenerator();

var authorMenu = new List<MenuLibrary> {
    (new MenuLibrary {Option = "Create author",
        ExecuteCommandMenu = () =>
        authorService.CreateAuthor(authorCollection) }),
    (new MenuLibrary {Option = "List all authors",
        ExecuteCommandMenu = () =>
        authorService.GetAuthorList(authorCollection)}),
    (new MenuLibrary {Option = "Update author",
        ExecuteCommandMenu = () =>
        authorService.UpdateAuthor(authorCollection)}),
    (new MenuLibrary {Option = "Delete author",
        ExecuteCommandMenu = () =>
        authorService.DeleteAuthor(authorCollection)})
    };

menu.Menu("Author", authorMenu);