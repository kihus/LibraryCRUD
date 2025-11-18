using Library.Models;

namespace Library.Utils;
internal class MenuGenerator
{
	public void Menu(string title, List<MenuLibrary> options)
	{
		do
		{
			Console.Clear();

			Console.Write($"=-= ");
			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write($"{title}");
			Console.ResetColor();
			Console.WriteLine($" =-=");

			for (int i = 0; i < options.Count; i++)
			{
				Console.Write($"{i + 1}# ");
				Console.ForegroundColor = ConsoleColor.Blue;
				Console.WriteLine(options[i]);
				Console.ResetColor();
			}

			Console.WriteLine("\nChoose a correctly option:");
			Console.Write("-> ");

			if (!int.TryParse(Console.ReadLine(), out var option)
				&& (option < 0 || option > options.Count))
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Ops... Select a correctly option!");
				Console.ResetColor();
				Thread.Sleep(1000);
				return;
			}
			try
			{
				options[option - 1].ExecuteCommandMenu();
			}
			catch (ArgumentOutOfRangeException ex)
			{
				Console.WriteLine("Error: " + ex.Message);
				Console.WriteLine("\nPress ENTER to continue...");
				Console.ReadKey();
			}


		} while (true);
	}
}
