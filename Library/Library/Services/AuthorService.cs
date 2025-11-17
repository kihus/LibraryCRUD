using Library.Entities;
using MongoDB.Driver;
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

        authorCollection.InsertOne(new Author(name, country));

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Succeful!");
        Console.ResetColor();

        Console.WriteLine("\nPress ENTER to continue...");
        Console.ReadKey();
    }

    public async void GetAuthorList(IMongoCollection<Author> authorCollection)
    {
        Console.Clear();

        if (!authorCollection.Find(x => true).Any())
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Don't have authors");
            Console.ResetColor();

            Console.WriteLine("\nPress ENTER to continue...");
            Console.ReadKey();
            return;
        }


        var authors = await authorCollection.FindAsync(x => true).Result.ToListAsync();
        
        foreach(var author in authors.OrderBy(x => x.Name))
            Console.WriteLine($"{author}\n");

        Console.WriteLine("Press ENTER to continue...");
        Console.ReadKey();
    }

    public async Task UpdateAuthor(IMongoCollection<Author> authorCollection)
    {
        Console.Clear();
        Console.Write("The author's id will be changed: ");
        var id = Console.ReadLine();

        var author = await authorCollection.FindAsync(x => x.Id == id).Result.FirstOrDefaultAsync();

        if (author == null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Ops... Author doesn't exists!");
            Console.ResetColor();
            return;
        }

        Console.WriteLine(author);

        Console.WriteLine("\nIf you don't want to change, just press enter!");
        Console.Write("Name: ");
        var name = Console.ReadLine() ?? "";

        if (name == "")
            name = authorCollection.Find(x => x.Id == id).FirstOrDefault().Name;

        if (name.Length < 3)
        {
            Console.WriteLine("Invalid name.");
            return;
        }

        Console.Write("Country: ");
        var country = Console.ReadLine() ?? "";

        if (country == "")
            country = authorCollection.Find(x => x.Id == id).FirstOrDefault().Country;

        if (country.Length < 3)
        {
            Console.WriteLine("Invalid name.");
            return;
        }

        var authorUpdate = Builders<Author>
                            .Update
                            .Set(x => x.Name, name)
                            .Set(x => x.Country, country)
                            .Set(x => x.UpdatedAt, DateTime.UtcNow);

        authorCollection.UpdateOne(x => x.Id == id, authorUpdate);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Succesful!");
        Console.ResetColor();
    }

    public void DeleteAuthor(IMongoCollection<Author> authorCollection)
    {
        Console.Clear();
        Console.Write("Digit author's id will be deleted: ");
        var id = Console.ReadLine() ?? "";

        if (!authorCollection.Find(x => x.Id == id).Any())
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Don't have authors");
            Console.ResetColor();
            return;
        }

        authorCollection.DeleteOne(x => x.Id == id);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Succesful!");
        Console.ResetColor();

        Console.WriteLine("\nPress ENTER to continue...");
        Console.ReadKey();
    }
}
