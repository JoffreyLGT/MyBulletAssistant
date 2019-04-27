using Core.Data;
using Core.Models;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace ApiTester
{
    class Program
    {
        static int padRightNbChars;
        static readonly char padChar = ' ';
        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                throw new Exception("The first argument should be the URL of the API server.");
            }

            Stopwatch watch = new Stopwatch();
            watch.Start();
            // Configuration
            Uri apiUri = new Uri(args[0]);
            MbaApiClient manager = new MbaApiClient(apiUri);

            var displayServerAddress = $"The api server address is {apiUri}";
            padRightNbChars = displayServerAddress.Length;

            DisplayFullLine(" Start the tests ");
            Console.WriteLine(displayServerAddress);
            Console.WriteLine();


            Console.Write("Create a new user, should fail...".PadRight(padRightNbChars,padChar));
            LoginModel userLogin = new LoginModel { Email = $"j.lagut@live.fr", Password = "Hello world" };
            (UserModel user, string error) = await manager.CreateUser(userLogin);
            WriteResultInConsole(user != null && string.IsNullOrEmpty(error));

            Console.Write("Create a new user...".PadRight(padRightNbChars, padChar));
            string randomString = CreateRandomString(5);
            userLogin = new LoginModel { Email = $"j.lagut{randomString}@live.fr", Password = "Hello_world1" };
            (user, error) = await manager.CreateUser(userLogin);
            WriteResultInConsole(user == null || !string.IsNullOrEmpty(error));

            Console.Write("Loggin the API...".PadRight(padRightNbChars, padChar));
            bool isLogged = await manager.Login(userLogin);
            WriteResultInConsole(!isLogged);

            Console.Write("Get the user information without entries...".PadRight(padRightNbChars, padChar));
            (user, error) = await manager.GetUser();
            WriteResultInConsole(string.IsNullOrEmpty(user.Id)
                || string.IsNullOrEmpty(user.Email)
                || user.Entries.Count != 0
                || !string.IsNullOrEmpty(error));

            Console.Write("Update the user...".PadRight(padRightNbChars, padChar));
            user.Entries.Add(new EntryModel { Journal = "2020", Pages = "30-130", Title = "A new hope" });
            UserModel updatedUser;
            (updatedUser, error) = await manager.UpdateUser(user);
            WriteResultInConsole(string.IsNullOrEmpty(user.Id) || string.IsNullOrEmpty(user.Email) || user.Entries.Count == 0);

            Console.Write("Get the user with its entries...".PadRight(padRightNbChars, padChar));
            (user, error) = await manager.GetUser(true);
            WriteResultInConsole(string.IsNullOrEmpty(user.Id)
                || string.IsNullOrEmpty(user.Email)
                || user.Entries.Count != 1
                || !string.IsNullOrEmpty(error));

            Console.Write("Post a new entry for the user...".PadRight(padRightNbChars, padChar));
            EntryModel entryToAdd = new EntryModel { Journal = "2019", Title = "Hello entry", Pages = "3-33" };
            EntryModel entry;
            (entry, error) = await manager.CreateEntry(entryToAdd);
            WriteResultInConsole(entryToAdd.Journal != entry.Journal
                || entryToAdd.Title != entry.Title
                || entryToAdd.Pages != entry.Pages
                || entry.Id == 0
                || !string.IsNullOrEmpty(error));

            Console.Write("Get the newly created entry by its id...".PadRight(padRightNbChars, padChar));
            EntryModel newEntry;
            (newEntry, error) = await manager.GetEntry(entry.Id);
            WriteResultInConsole(newEntry.Journal != entry.Journal
                || newEntry.Title != entry.Title
                || newEntry.Pages != entry.Pages
                || newEntry.Id != entry.Id
                || !string.IsNullOrEmpty(error));

            Console.Write("Get all the user entries...".PadRight(padRightNbChars, padChar));
            EntryModel[] entries;
            (entries, error) = await manager.GetEntries();
            WriteResultInConsole(entries.GetLength(0) != 2 || !string.IsNullOrEmpty(error));

            Console.Write("Update the entry...".PadRight(padRightNbChars, padChar));
            EntryModel updatedEntry = new EntryModel { Journal = "Updated", Pages = "Updated", Title = "Updated" };
            (newEntry, error) = await manager.UpdateEntry(entry.Id, updatedEntry);
            WriteResultInConsole(newEntry.Journal != updatedEntry.Journal
                || newEntry.Title != updatedEntry.Title
                || newEntry.Pages != updatedEntry.Pages
                || !string.IsNullOrEmpty(error));

            Console.Write("Delete an entry...".PadRight(padRightNbChars, padChar));
            bool isDeleted;
            (isDeleted, error) = await manager.DeleteEntry(newEntry.Id);
            (entries, _) = await manager.GetEntries();
            WriteResultInConsole(!isDeleted || !string.IsNullOrEmpty(error) || entries.GetLength(0) != 1);

            Console.Write("Delete the user and its entries...".PadRight(padRightNbChars, padChar));
            (isDeleted, error) = await manager.DeleteUser();
            WriteResultInConsole(!isDeleted || !string.IsNullOrEmpty(error));
            watch.Stop();
            Console.WriteLine($"\nFinished in {watch.Elapsed.ToString(@"ss\,ff")} seconds.");
            DisplayFullLine(" Tests finished, press a key to quit ");
            Console.ReadKey();

        }

        private static void DisplayFullLine(string message)
        {
            int NbExtraCharsPerSide = (padRightNbChars - message.Length + 2) /2;
            string messageWithPadLeft = message.PadLeft(message.Length + NbExtraCharsPerSide, '-');
            string messageWithBothPads = messageWithPadLeft.PadRight(messageWithPadLeft.Length + NbExtraCharsPerSide, '-');

            Console.WriteLine(messageWithBothPads);
        }

        private static string CreateRandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * new Random((int)DateTime.Now.Ticks).NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        public static void WriteResultInConsole(bool result)
        {
            if (result)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine(" KO.");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(" OK.");
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
