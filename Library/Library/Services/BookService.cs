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

		if (title == null)
		{
			Console.WriteLine("Ops... Title cannot be null");
			return;
		}

		Console.Write("Author id: ");
		var authorId = Console.ReadLine() ?? "";

		try
		{
			if(authorId.Length < 24 || authorId.Length > 24)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Ops... Author id is incorrect");
				Console.ResetColor();

				Console.WriteLine("\nPress ENTER to continue...");
				Console.ReadKey();

				return;
			}

			if (!await authorCollection.FindAsync(x => x.Id == authorId).Result.AnyAsync())
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Ops... Author id not found");
				Console.ResetColor();

				Console.WriteLine("\nPress ENTER to continue...");
				Console.ReadKey();
				return;
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


		Console.Write("Year: ");
		if (!int.TryParse(Console.ReadLine(), out var year) && (year < 0 || year > DateTime.Now.Year))
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Put a correctly year!");
			Console.ResetColor();

			Console.WriteLine("\nPress ENTER to continue...");
			Console.ReadKey();

			return;
		}

		try
		{
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

				Console.WriteLine("\nPress ENTER to continue...");
				Console.ReadKey();

				return;
			}
		}
		catch (MongoClientException ex)
		{
			Console.WriteLine("Client error: " + ex.Message);
			Console.WriteLine("\nPress ENTER to continue...");
			Console.ReadKey();
		}
		catch (MongoException ex)
		{
			Console.WriteLine("Error: " + ex.Message);
			Console.WriteLine("\nPress ENTER to continue...");
			Console.ReadKey();
		}

		var books = await bookCollection.FindAsync(x => true).Result.ToListAsync();

		foreach (var book in books.OrderBy(x => x.Title))
			Console.WriteLine($"{book}\n");

		Console.WriteLine("Press ENTER to continue...");
		Console.ReadKey();
	}

	public async Task UpdateBook(IMongoCollection<Book> bookCollection, IMongoCollection<Author> authorCollection)
	{
		Console.Clear();
		Console.Write("The author's id will be changed: ");
		var id = Console.ReadLine();

		var book = await bookCollection.FindAsync(x => x.Id == id).Result.FirstOrDefaultAsync();

		if (book == null)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Ops... Book doesn't exists!");
			Console.ResetColor();
			return;
		}

		Console.WriteLine(book);

		Console.WriteLine("\nIf you don't want to change, just press enter!");
		Console.Write("Title: ");
		var title = Console.ReadLine() ?? "";

		if (title == "")
			title = bookCollection.Find(x => x.Id == id).FirstOrDefault().Title;

		Console.Write("Author id: ");
		var authorId = Console.ReadLine() ?? "";

		if (authorId == "")
			authorId = bookCollection.Find(x => x.AuthorId == authorId).FirstOrDefault().AuthorId;

		if(!await authorCollection.FindAsync(x => x.Id == authorId).Result.AnyAsync())
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Ops... Author doesn't exists!");
			Console.ResetColor();
			return;
		}

		Console.Write("Year: ");
		var years = Console.ReadLine() ?? "";

		if (years == "")
			years = bookCollection.Find(x => x.Id == id).FirstOrDefault().Year.ToString();

		if (!int.TryParse(years, out var year) && (year < 0 || year > DateTime.Now.Year))
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Put a correctly year!");
			Console.ResetColor();

			Console.WriteLine("\nPress ENTER to continue...");
			Console.ReadKey();

			return;
		}

		var bookUpdate = Builders<Book>
							.Update
							.Set(x => x.Title, title)
							.Set(x => x.AuthorId, authorId)
							.Set(x => x.Year, year)
							.Set(x => x.UpdatedAt, DateTime.UtcNow);

		bookCollection.UpdateOne(x => x.Id == id, bookUpdate);

		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine("Succesful!");
		Console.ResetColor();
	}

	public void DeleteBook(IMongoCollection<Book> bookCollection)
	{
		Console.Clear();
		Console.Write("Digit book's id will be deleted: ");
		var id = Console.ReadLine() ?? "";

		if (!bookCollection.Find(x => x.Id == id).Any())
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("This book doesn't exists!");
			Console.ResetColor();
			return;
		}

		var book = bookCollection.Find(x => x.Id == id).FirstOrDefault();

		Console.WriteLine(book);

		Console.Write("Are you sure you want to delete this? (y/n) ");

		if((Console.ReadLine().ToLower() ?? "") != "y")
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

		Console.WriteLine("\nPress ENTER to continue...");
		Console.ReadKey();
	}
}
