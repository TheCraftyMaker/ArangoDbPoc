using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArangoDbPoc
{
    public class SimpleDocument : Flow
    {
        protected override async Task StartInternalFlow()
        {
            await NextAsync("Ready to create a document.", CreateDocument);
            await NextAsync("Ready to query a document.", QueryDocument);
            await NextAsync("Ready to update a document.", UpdateDocument);
            await NextAsync("Ready to query a document.", QueryDocument);
            await NextAsync("Ready to remove a document.", DeleteDocument);
        }

        private static async Task CreateDocument()
        {
            try
            {
                //Truncate. Make sure we only have 1 document at this stage.
                await Arango.Collection.TruncateAsync(DatabaseName, CollectionName);

                Console.WriteLine("Creating 'Ned Stark' document ...");

                var ned = new Character("Ned", "Stark", true, 41, new List<string> { "A", "H", "C", "N", "P" });

                await Arango.Document.CreateAsync(DatabaseName, CollectionName, ned);

                Console.WriteLine("Document created.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to create document. " + e);
            }
        }

        private static async Task QueryDocument()
        {
            try
            {
                Console.WriteLine("Querying 'Ned Stark' document ...");

                const string name = "Ned";

                FormattableString query = $"FOR c IN {CollectionName:@} FILTER c.name == {name} RETURN c";
                var result = await Arango.Query.ExecuteAsync<Character>(DatabaseName, $"{query}");

                foreach (var character in result)
                {
                    string json = JsonConvert.SerializeObject(character, Formatting.Indented);
                    Console.WriteLine(json);
                }

                Console.WriteLine("Document retrieved.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error occured while retrieving document. " + e);
            }
        }

        private static async Task UpdateDocument()
        {
            try
            {
                Console.WriteLine("Updating 'Ned Stark' document ...");

                const string name = "Ned";

                //Retrieve the key
                var key = await GetKey($"c.name == {name}", CollectionName);

                //Kill Ned Stark
                Console.WriteLine("Killing 'Ned Stark' ...");

                await Arango.Document.UpdateAsync(DatabaseName, CollectionName, new
                {
                    Key = key,
                    Alive = false
                });

                Console.WriteLine("'Ned Stark' document updated.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error occured while modifying document. " + e);
            }
        }

        private static async Task DeleteDocument()
        {
            try
            {
                Console.WriteLine("Deleting 'Ned Stark' document ...");

                const string name = "Ned";

                //Retrieve the key
                var key = await GetKey($"c.name == {name}", CollectionName);

                await Arango.Document.DeleteAsync<Character>(DatabaseName, CollectionName, key);

                Console.WriteLine("'Ned Stark' document removed.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error occured while removing document. " + e);
            }
        }
    }
}
