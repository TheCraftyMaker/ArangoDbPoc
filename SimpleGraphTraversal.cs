using Core.Arango.Protocol;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArangoDbPoc
{
    public class SimpleGraphTraversal : Flow
    {
        private const string ChildrenCollectionName = "ChildOf";
        private const string GraphName = "FamilyTree";

        protected override async Task StartInternalFlow()
        {
            await NextAsync("Ready to create multiple documents.", CreateDocuments);
            await NextAsync("Ready to create edge collection.", CreateEdgeCollection);
            await NextAsync("Ready to create edges.", CreatEdges);
            await NextAsync("Ready to query graph.", QueryGraph);
        }

        private static async Task CreateDocuments()
        {
            try
            {
                await Arango.Collection.TruncateAsync(DatabaseName, CollectionName);

                var data =
                    @"[{ ""name"": ""Ned"", ""surname"": ""Stark"", ""alive"": true, ""age"": 41, ""traits"": [""A"",""H"",""C"",""N"",""P""] },
                       { ""name"": ""Robert"", ""surname"": ""Baratheon"", ""alive"": false, ""traits"": [""A"",""H"",""C""] },
                       { ""name"": ""Jaime"", ""surname"": ""Lannister"", ""alive"": true, ""age"": 36, ""traits"": [""A"",""F"",""B""] },
                       { ""name"": ""Catelyn"", ""surname"": ""Stark"", ""alive"": false, ""age"": 40, ""traits"": [""D"",""H"",""C""] },
                       { ""name"": ""Cersei"", ""surname"": ""Lannister"", ""alive"": true, ""age"": 36, ""traits"": [""H"",""E"",""F""] },
                       { ""name"": ""Daenerys"", ""surname"": ""Targaryen"", ""alive"": true, ""age"": 16, ""traits"": [""D"",""H"",""C""] },
                       { ""name"": ""Jorah"", ""surname"": ""Mormont"", ""alive"": false, ""traits"": [""A"",""B"",""C"",""F""] },
                       { ""name"": ""Petyr"", ""surname"": ""Baelish"", ""alive"": false, ""traits"": [""E"",""G"",""F""] },
                       { ""name"": ""Viserys"", ""surname"": ""Targaryen"", ""alive"": false, ""traits"": [""O"",""L"",""N""] },
                       { ""name"": ""Jon"", ""surname"": ""Snow"", ""alive"": true, ""age"": 16, ""traits"": [""A"",""B"",""C"",""F""] },
                       { ""name"": ""Sansa"", ""surname"": ""Stark"", ""alive"": true, ""age"": 13, ""traits"": [""D"",""I"",""J""] },
                       { ""name"": ""Arya"", ""surname"": ""Stark"", ""alive"": true, ""age"": 11, ""traits"": [""C"",""K"",""L""] },
                       { ""name"": ""Robb"", ""surname"": ""Stark"", ""alive"": false, ""traits"": [""A"",""B"",""C"",""K""] },
                       { ""name"": ""Theon"", ""surname"": ""Greyjoy"", ""alive"": true, ""age"": 16, ""traits"": [""E"",""R"",""K""] },
                       { ""name"": ""Bran"", ""surname"": ""Stark"", ""alive"": true, ""age"": 10, ""traits"": [""L"",""J""] },
                       { ""name"": ""Joffrey"", ""surname"": ""Baratheon"", ""alive"": false, ""age"": 19, ""traits"": [""I"",""L"",""O""] },
                       { ""name"": ""Sandor"", ""surname"": ""Clegane"", ""alive"": true, ""traits"": [""A"",""P"",""K"",""F""] },
                       { ""name"": ""Tyrion"", ""surname"": ""Lannister"", ""alive"": true, ""age"": 32, ""traits"": [""F"",""K"",""M"",""N""] },
                       { ""name"": ""Khal"", ""surname"": ""Drogo"", ""alive"": false, ""traits"": [""A"",""C"",""O"",""P""] },
                       { ""name"": ""Tywin"", ""surname"": ""Lannister"", ""alive"": false, ""traits"": [""O"",""M"",""H"",""F""] },
                       { ""name"": ""Davos"", ""surname"": ""Seaworth"", ""alive"": true, ""age"": 49, ""traits"": [""C"",""K"",""P"",""F""] },
                       { ""name"": ""Samwell"", ""surname"": ""Tarly"", ""alive"": true, ""age"": 17, ""traits"": [""C"",""L"",""I""] },
                       { ""name"": ""Stannis"", ""surname"": ""Baratheon"", ""alive"": false, ""traits"": [""H"",""O"",""P"",""M""] },
                       { ""name"": ""Melisandre"", ""alive"": true, ""traits"": [""G"",""E"",""H""] },
                       { ""name"": ""Margaery"", ""surname"": ""Tyrell"", ""alive"": false, ""traits"": [""M"",""D"",""B""] },
                       { ""name"": ""Jeor"", ""surname"": ""Mormont"", ""alive"": false, ""traits"": [""C"",""H"",""M"",""P""] },
                       { ""name"": ""Bronn"", ""alive"": true, ""traits"": [""K"",""E"",""C""] },
                       { ""name"": ""Varys"", ""alive"": true, ""traits"": [""M"",""F"",""N"",""E""] },
                       { ""name"": ""Shae"", ""alive"": false, ""traits"": [""M"",""D"",""G""] },
                       { ""name"": ""Talisa"", ""surname"": ""Maegyr"", ""alive"": false, ""traits"": [""D"",""C"",""B""] },
                       { ""name"": ""Gendry"", ""alive"": false, ""traits"": [""K"",""C"",""A""] },
                       { ""name"": ""Ygritte"", ""alive"": false, ""traits"": [""A"",""P"",""K""] },
                       { ""name"": ""Tormund"", ""surname"": ""Giantsbane"", ""alive"": true, ""traits"": [""C"",""P"",""A"",""I""] },
                       { ""name"": ""Gilly"", ""alive"": true, ""traits"": [""L"",""J""] },
                       { ""name"": ""Brienne"", ""surname"": ""Tarth"", ""alive"": true, ""age"": 32, ""traits"": [""P"",""C"",""A"",""K""] },
                       { ""name"": ""Ramsay"", ""surname"": ""Bolton"", ""alive"": true, ""traits"": [""E"",""O"",""G"",""A""] },
                       { ""name"": ""Ellaria"", ""surname"": ""Sand"", ""alive"": true, ""traits"": [""P"",""O"",""A"",""E""] },
                       { ""name"": ""Daario"", ""surname"": ""Naharis"", ""alive"": true, ""traits"": [""K"",""P"",""A""] },
                       { ""name"": ""Missandei"", ""alive"": true, ""traits"": [""D"",""L"",""C"",""M""] },
                       { ""name"": ""Tommen"", ""surname"": ""Baratheon"", ""alive"": true, ""traits"": [""I"",""L"",""B""] },
                       { ""name"": ""Jaqen"", ""surname"": ""H'ghar"", ""alive"": true, ""traits"": [""H"",""F"",""K""] },
                       { ""name"": ""Roose"", ""surname"": ""Bolton"", ""alive"": true, ""traits"": [""H"",""E"",""F"",""A""] },
                       { ""name"": ""The High Sparrow"", ""alive"": true, ""traits"": [""H"",""M"",""F"",""O""] }]";

                var characters = JsonConvert.DeserializeObject<List<Character>>(data);

                Console.WriteLine("Creating documents ...");

                if (characters != null)
                {
                    foreach (var character in characters)
                    {
                        await Arango.Graph.Vertex.CreateAsync(DatabaseName, GraphName, CollectionName, character);
                    }

                    Console.WriteLine("Character documents created.");
                }
                else Console.WriteLine("No character documents created.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to create document. " + e);
            }
        }

        private static async Task CreateEdgeCollection()
        {
            try
            {
                var exists = await Arango.Collection.ExistAsync(DatabaseName, ChildrenCollectionName);
                if (exists)
                {
                    await Arango.Collection.DropAsync(DatabaseName, ChildrenCollectionName);
                }

                Console.WriteLine($"Creating '{ChildrenCollectionName}' edge collection ...");

                await Arango.Collection.CreateAsync(DatabaseName, ChildrenCollectionName, ArangoCollectionType.Edge);

                Console.WriteLine("Edge collection created.");

            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to create edge collection. " + e);
            }
        }

        private static async Task CreatEdges()
        {
            try
            {
                await Arango.Collection.TruncateAsync(DatabaseName, ChildrenCollectionName);
                await Arango.Graph.DropAsync(DatabaseName, GraphName);

                var data = new List<RelationShip>
                {
                    new("Ned", "Stark", "Robb", "Stark"),
                    new("Ned", "Stark", "Sansa", "Stark"),
                    new("Ned", "Stark", "Arya", "Stark"),
                    new("Ned", "Stark", "Bran", "Stark"),
                    new("Ned", "Stark", "Jon", "Snow"),
                    new("Catelyn", "Stark", "Robb", "Stark"),
                    new("Catelyn", "Stark", "Sansa", "Stark"),
                    new("Catelyn", "Stark", "Arya", "Stark"),
                    new("Catelyn", "Stark", "Bran", "Stark"),
                    new("Tywin", "Lannister", "Jaime", "Lannister"),
                    new("Tywin", "Lannister", "Cersei", "Lannister"),
                    new("Tywin", "Lannister", "Tyrion", "Lannister"),
                    new("Cersei", "Lannister", "Joffrey", "Baratheon"),
                    new("Jaime", "Lannister", "Joffrey", "Baratheon")
                };

                Console.WriteLine("Creating graph ...");

                await Arango.Graph.CreateAsync(DatabaseName, new ArangoGraph
                {
                    Name = GraphName,
                    EdgeDefinitions = new List<ArangoEdgeDefinition>
                    {
                        new()
                        {
                            Collection = ChildrenCollectionName,
                            From = new List<string> {CollectionName},
                            To = new List<string> {CollectionName}
                        }
                    }
                });

                Console.WriteLine("Creating edges ...");

                foreach (var relationShip in data)
                {
                    var parentKey = await GetKey($"c.name == {relationShip.ParentName} && c.surname == {relationShip.ParentSurname}", CollectionName);
                    var childKey = await GetKey($"c.name == {relationShip.ChildName} && c.surname == {relationShip.ChildSurname}", CollectionName);

                    await Arango.Graph.Edge.CreateAsync(DatabaseName, GraphName, ChildrenCollectionName, new
                    {
                        From = $"{CollectionName}/{childKey}",
                        To = $"{CollectionName}/{parentKey}",
                        Label = "Child"
                    });
                }

                Console.WriteLine("Edges created.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to create edges. " + e);
            }
        }

        private static async Task QueryGraph()
        {
            try
            {
                Console.WriteLine("Querying graph ...");
                Console.WriteLine("Fetching parents of Sansa Stark ...");

                FormattableString query =
                    $"FOR c IN {CollectionName:@} FILTER c.name == \"Sansa\" FOR v IN 1..1 OUTBOUND c {ChildrenCollectionName} RETURN v.name";

                var result = await Arango.Query.ExecuteAsync<object>(DatabaseName, $"{query}");

                foreach (var o in result)
                {
                    Console.WriteLine(o);
                }

                Console.WriteLine("Querying graph done.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error occured while querying graph. " + e);
            }
        }

        internal record RelationShip(string ParentName, string ParentSurname, string ChildName, string ChildSurname);
    }
}
