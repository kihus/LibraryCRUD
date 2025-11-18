using Library.Entities;
using MongoDB.Driver;
using System.Net;
using System.Threading.Tasks;

namespace Library.Services;
internal class AuthorService
{
	public void CreateAuthor(IMongoCollection<Author> authorCollection)
	{
		Console.Clear();
		Console.Write("Name: ");
		var name = Console.ReadLine() ?? "";
		if (name.Length < 3)
		{
			Console.WriteLine("Invalid name.");
			return;
		}

		Console.Write("Country: ");
		var country = Console.ReadLine() ?? "";
		try
		{
			authorCollection.InsertOne(new Author(name, country));
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

	public async void GetAuthorList(IMongoCollection<Author> authorCollection)
	{
		Console.Clear();
		try
		{
			if (!await authorCollection.FindAsync(x => true).Result.AnyAsync())
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Don't have authors");
				Console.ResetColor();

				return;
			}

			var authors = await authorCollection.FindAsync(x => true).Result.ToListAsync();

			foreach (var author in authors.OrderBy(x => x.Name))
				Console.WriteLine($"{author}\n");
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
			Console.WriteLine("Press ENTER to continue...");
			Console.ReadKey();
		}
	}

	public async Task UpdateAuthor(IMongoCollection<Author> authorCollection)
	{
		Console.Clear();
		Console.Write("The author's id will be changed: ");
		var id = Console.ReadLine();



		try
		{


			var author = await authorCollection.FindAsync(x => x.Id == id).Result.FirstOrDefaultAsync();

			Console.WriteLine(author);

			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("\nIf you don't want to change, just press enter!");
			Console.ResetColor();

			Console.Write("Name: ");

			var name = Console.ReadLine() ?? "";

			if (name == "")
				name = author.Name;

			if (name.Length < 3)
			{
				Console.WriteLine("Invalid name.");

				Console.WriteLine("\nPress ENTER to continue...");
				Console.ReadKey();
				return;
			}

			Console.Write("Country: ");
			var country = Console.ReadLine() ?? "";

			if (country == "")
				country = author.Country;

			if (country.Length < 3)
			{
				Console.WriteLine("Invalid name.");

				Console.WriteLine("\nPress ENTER to continue...");
				Console.ReadKey();
				return;
			}

			var authorUpdate = Builders<Author>
								.Update
								.Set(x => x.Name, name)
								.Set(x => x.Country, country)
								.Set(x => x.UpdatedAt, DateTime.UtcNow);

			authorCollection.UpdateOne(x => x.Id == id, authorUpdate);
		}
		catch (MongoClientException ex)
		{
			Console.WriteLine("Client error: " + ex.Message);
		}
		catch (MongoException ex)
		{
			Console.WriteLine("Error: " + ex.Message);
		}

		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine("Succesful!");
		Console.ResetColor();

		Console.WriteLine("\nPress ENTER to continue...");
		Console.ReadKey();
	}

	public async void DeleteAuthor(IMongoCollection<Author> authorCollection)
	{
		Console.Clear();
		Console.Write("Digit author's id will be deleted: ");
		var id = Console.ReadLine() ?? "";

		try
		{
			var author = VerifyAuthor(id, authorCollection);

			if (author == null)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Ops... Author dont't exists");
				Console.ResetColor();

				Console.WriteLine("\nPress ENTER to continue...");
				Console.ReadKey();

				return;
			}

			authorCollection.DeleteOne(x => x.Id == id);
		}
		catch (MongoClientException ex)
		{
			Console.WriteLine("Client error: " + ex.Message);
		}
		catch (MongoException ex)
		{
			Console.WriteLine("Error: " + ex.Message);
		}

		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine("Succesful!");
		Console.ResetColor();

		Console.WriteLine("\nPress ENTER to continue...");
		Console.ReadKey();
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
}
