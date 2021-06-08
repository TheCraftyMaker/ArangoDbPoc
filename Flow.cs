using Core.Arango;
using Core.Arango.Protocol;
using Core.Arango.Serialization.Newtonsoft;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ArangoDbPoc
{
    public abstract class Flow
    {
        protected static IArangoContext Arango;

        protected const string DatabaseName = "GameOfThrones";
        protected const string CollectionName = "Characters";

        public async Task StartFlow()
        {
            Console.WriteLine();

            Connect();

            await CreateDatabase();
            await CreateCollection();
            await StartInternalFlow();
        }

        protected abstract Task StartInternalFlow();

        protected static void Connect()
        {
            try
            {
                var serializer = new ArangoNewtonsoftSerializer(new ArangoNewtonsoftCamelCaseContractResolver());
                var configuration = new ArangoConfiguration { Serializer = serializer };

                Console.WriteLine("Connecting to Arango DB ...");

                Arango = new ArangoContext(
                    "Server=http://localhost:8529;User=root;Password=openSesame;",
                    configuration);

                Console.WriteLine("Connected.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to connect. " + e);
            }
            Console.WriteLine();
        }

        protected static async Task CreateDatabase()
        {
            try
            {
                var exists = await Arango.Database.ExistAsync(DatabaseName);
                if (exists)
                {
                    Console.WriteLine($"'{DatabaseName}' database already exists. No database created.");
                }
                else
                {
                    Console.WriteLine("Creating 'GameOfThrones' Database ...");

                    await Arango.Database.CreateAsync(DatabaseName);

                    Console.WriteLine("Database created.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to create database. " + e);
            }
            Console.WriteLine();
        }

        protected static async Task CreateCollection()
        {
            try
            {
                var exists = await Arango.Collection.ExistAsync(DatabaseName, CollectionName);
                if (exists)
                {
                    Console.WriteLine($"'{CollectionName}' collection already exists. No collection created.");
                }
                else
                {
                    Console.WriteLine($"Creating '{CollectionName}' collection ...");

                    await Arango.Collection.CreateAsync(DatabaseName, CollectionName, ArangoCollectionType.Document);

                    Console.WriteLine("Collection created.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to create collection. " + e);
            }
            Console.WriteLine();
        }

        protected static async Task<string> GetKey(FormattableString filterPart, string collectionName)
        {
            var idResult = await Arango.Query.ExecuteAsync<string>(
                DatabaseName, $"FOR c IN {collectionName:@} FILTER {filterPart} RETURN c._key");

            return idResult.Count switch
            {
                0 => throw new InvalidOperationException("Filter returned no result."),
                > 1 => throw new InvalidOperationException("Filter returned more than one result. " +
                                                           "Please refine filter. Result of query cannot " +
                                                           "contain more than one result."),
                _ => idResult.First()
            };
        }

        protected static void Next(string message, Action action)
        {
            DisplayMessage(message);

            var keyInfo = Console.ReadKey();
            if (keyInfo.Key == ConsoleKey.Enter)
            {
                action();
            }
            else
            {
                Console.WriteLine();
                Next("Invalid key. " + message, action);
            }
            Console.WriteLine();
        }

        protected static async Task NextAsync(string message, Func<Task> action)
        {
            DisplayMessage(message);

            var keyInfo = Console.ReadKey();
            if (keyInfo.Key == ConsoleKey.Enter)
            {
                await action();
            }
            else
            {
                Console.WriteLine();
                await NextAsync("Invalid key. " + message, action);
            }

            Console.WriteLine();
        }

        private static void DisplayMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;

            Console.WriteLine(message);

            Console.WriteLine("Press enter to continue ...");
            Console.WriteLine();

            Console.ResetColor();
        }
    }
}
