using Library.Entities;
using MongoDB.Driver;

namespace Library.Services;

internal class BookService
{
	public async void CreateBook(IMongoCollection<Book> bookCollection, IMongoCollection<Author> authorCollection)
	{
		Console.Clear();

		Console.Write("Title: ");
		var title = Console.ReadLine() ?? "";

		if (title is "")
		{
			Console.WriteLine("Ops... Title cannot be null");
			return;
		}

		Console.Write("Author id: ");
		var authorId = Console.ReadLine() ?? "";

		try
		{
			var author = VerifyAuthor(authorId, authorCollection);
			if (author is null)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Ops... Author id not found");
				Console.ResetColor();

				return;
			}

			Console.Write("Year: ");
			if (!int.TryParse(Console.ReadLine(), out var year) && (year < 0 || year > DateTime.Now.Year))
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Put a correctly year!");
				Console.ResetColor();

				return;
			}

			bookCollection.InsertOne(new Book(title, authorId, year));

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("Succeful!");
			Console.ResetColor();
		}
		catch (MongoAuthenticationException ex)
		{
			Console.WriteLine("Authentication error: " + ex.Message);
		}
		catch (MongoException ex)
		{
			Console.WriteLine("Error: " + ex.Message);
		}
		finally
		{
			Console.WriteLine("\nPress ENTER to continue...");
			Console.ReadKey();
		}

	}

	public async void GetBookList(IMongoCollection<Book> bookCollection)
	{
		Console.Clear();

		try
		{
			if (!await bookCollection.FindAsync(x => true).Result.AnyAsync())
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Ops... Book not found");
				Console.ResetColor();

				return;
			}

			var books = await bookCollection.FindAsync(x => true).Result.ToListAsync();

			foreach (var book in books.OrderBy(x => x.Title))
				Console.WriteLine($"{book}\n");
		}
		catch (MongoClientException ex)
		{
			Console.WriteLine("Client error: " + ex.Message);
		}
		catch (MongoException ex)
		{
			Console.WriteLine("Error: " + ex.Message);
		}
		finally
		{
			Console.WriteLine("\nPress ENTER to continue...");
			Console.ReadKey();
		}
	}

	public async Task UpdateBook(IMongoCollection<Book> bookCollection, IMongoCollection<Author> authorCollection)
	{
		Console.Clear();
		Console.Write("The book's id will be changed: ");
		var id = Console.ReadLine() ?? "";

		try
		{
			var book = VerifyBook(id, bookCollection);

			if (book == null)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Ops... Book doesn't exists!");
				Console.ResetColor();
				return;
			}

			Console.WriteLine(book);

			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("\nIf you don't want to change, just press enter!");
			Console.ResetColor();

			Console.Write("Title: ");
			var title = Console.ReadLine() ?? "";

			if (title is "")
				title = book.Result.Title;

			Console.Write("Author id: ");
			var authorId = Console.ReadLine() ?? "";

			if (authorId is "")
				authorId = book.Result.AuthorId;

			var author = VerifyAuthor(authorId, authorCollection);

			if (author is null)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Ops... Author doesn't exists!");
				Console.ResetColor();
				return;
			}

			Console.Write("Year: ");
			var years = Console.ReadLine() ?? "";

			if (years is "")
				years = book.Result.Year.ToString();

			if (!int.TryParse(years, out var year) && (year < 0 || year > DateTime.Now.Year))
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Put a correctly year!");
				Console.ResetColor();

				return;
			}

			var bookUpdate = Builders<Book>
								.Update
								.Set(x => x.Title, title)
								.Set(x => x.AuthorId, author.Id.ToString())
								.Set(x => x.Year, year)
								.Set(x => x.UpdatedAt, DateTime.UtcNow);

			bookCollection.UpdateOne(x => x.Id == id, bookUpdate);

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("Succesful!");
			Console.ResetColor();
		}
		catch (MongoClientException ex)
		{
			Console.WriteLine("Client error: " + ex.Message);
		}
		catch (MongoException ex)
		{
			Console.WriteLine("Error: " + ex.Message);
		}
		finally
		{
			Console.WriteLine("\nPress ENTER to continue...");
			Console.ReadKey();
		}
	}

	public async void DeleteBook(IMongoCollection<Book> bookCollection)
	{
		Console.Clear();
		Console.Write("Digit book's id will be deleted: ");
		var id = Console.ReadLine() ?? "";

		try
		{
			var book = VerifyBook(id, bookCollection);

			if (book is null)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("This book doesn't exists!");
				Console.ResetColor();
				return;
			}

			Console.WriteLine(book);

			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.Write("\nAre you sure you want to delete this? (y/n) ");
			Console.ResetColor();

			if ((Console.ReadLine().ToLower() ?? "") != "y")
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Operation canceled!");
				Console.ResetColor();

				Console.WriteLine("\nPress ENTER to continue...");
				Console.ReadKey();
				return;
			}

			bookCollection.DeleteOne(x => x.Id == id);

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("Succesful!");
			Console.ResetColor();
		}
		catch(MongoClientException ex)
		{
			Console.WriteLine("Error: " + ex.Message);
		}
		catch(MongoException ex)
		{
			Console.WriteLine("Error: " + ex.Message);
		}
		finally
		{
			Console.WriteLine("\nPress ENTER to continue...");
			Console.ReadKey();
		}
	}

	private async Task<Author> VerifyAuthor(string id, IMongoCollection<Author> authorCollection)
	{
		if (id.Length < 24 || id.Length > 24)
		{
			return null;
		}

		try
		{
			if (!await authorCollection.FindAsync(x => true).Result.AnyAsync())
			{
				return null;
			}
			var author = await authorCollection.FindAsync(x => x.Id == id).Result.FirstOrDefaultAsync();

			if (author != null)
			{
				return author;
			}
		}
		catch (MongoClientException ex)
		{
			Console.WriteLine("Client error: " + ex.Message);
		}
		catch (MongoException ex)
		{
			Console.WriteLine("Error: " + ex.Message);
		}

		return null;
	}
	private async Task<Book> VerifyBook(string id, IMongoCollection<Book> bookCollection)
	{
		if (id.Length < 24 || id.Length > 24)
		{
			return null;
		}

		try
		{
			if (!await bookCollection.FindAsync(x => true).Result.AnyAsync())
			{
				return null;
			}
			var book = await bookCollection.FindAsync(x => x.Id == id).Result.FirstOrDefaultAsync();

			if (book != null)
			{
				return book;
			}
		}
		catch (MongoClientException ex)
		{
			Console.WriteLine("Client error: " + ex.Message);
		}
		catch (MongoException ex)
		{
			Console.WriteLine("Error: " + ex.Message);
		}

		return null;
	}
}
