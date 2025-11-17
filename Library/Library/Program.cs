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
var bookService = new BookService();
var menu = new MenuGenerator();

MainMenu();

List<MenuLibrary> AuthorMenu()
{
	var menu = new List<MenuLibrary>
	{
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
			authorService.DeleteAuthor(authorCollection)}),
		(new MenuLibrary {Option = "Exit",
			ExecuteCommandMenu = () => MainMenu()
		})
	};

	return menu;
}

List<MenuLibrary> BookMenu()
{
	var bookMenu = new List<MenuLibrary> {
	(new MenuLibrary {Option = "Create book",
		ExecuteCommandMenu = () =>
		bookService.CreateBook(bookCollection, authorCollection)}),
	(new MenuLibrary {Option = "List all books",
		ExecuteCommandMenu = () =>
		bookService.GetBookList(bookCollection)}),
	(new MenuLibrary {Option = "Update book",
		ExecuteCommandMenu = () =>
		bookService.UpdateBook(bookCollection, authorCollection)}),
	(new MenuLibrary {Option = "Delete book",
		ExecuteCommandMenu = () =>
		bookService.DeleteBook(bookCollection)}),
	(new MenuLibrary {Option = "Exit",
		ExecuteCommandMenu = () => MainMenu()
		})
	};

	return bookMenu;
}

void MainMenu()
{
	var mainMenu = new List<MenuLibrary>
	{
		(new MenuLibrary {Option = "Author",
			ExecuteCommandMenu = () =>
			menu.Menu("Author", AuthorMenu())}),
		(new MenuLibrary {Option = "Book",
			ExecuteCommandMenu = () =>
			menu.Menu("Book", BookMenu())})
	};

	menu.Menu("Main Menu", mainMenu);
}
