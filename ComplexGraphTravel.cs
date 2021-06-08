using Core.Arango.Protocol;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArangoDbPoc
{
    public class ComplexGraphTraversal : Flow
    {
        private const string WeaponsCollectionName = "Weapons";
        private const string EdgeCollectionName = "ChildOf";
        private const string GraphName = "FamilyTree";

        protected override async Task StartInternalFlow()
        {
            await NextAsync("Ready to create weapons collection.", CreateWeaponsCollection);
            await NextAsync("Ready to create character documents.", CreateCharacterDocuments);
            await NextAsync("Ready to create weapon documents.", CreateWeaponDocuments);
            await NextAsync("Ready to create edge collections.", CreateEdgeCollection);
            await NextAsync("Ready to create character edges.", CreateEdges);
            await NextAsync("Ready to query graph.", QueryGraph);
        }

        protected static async Task CreateWeaponsCollection()
        {
            try
            {
                var exists = await Arango.Collection.ExistAsync(DatabaseName, WeaponsCollectionName);
                if (exists)
                {
                    Console.WriteLine($"'{WeaponsCollectionName}' collection already exists. No collection created.");
                }
                else
                {
                    Console.WriteLine($"Creating '{WeaponsCollectionName}' collection ...");

                    await Arango.Collection.CreateAsync(DatabaseName, WeaponsCollectionName, ArangoCollectionType.Document);

                    Console.WriteLine("Collection created.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to create weapon collection. " + e);
            }
            Console.WriteLine();
        }

        private static async Task CreateCharacterDocuments()
        {
            try
            {
                await Arango.Collection.TruncateAsync(DatabaseName, CollectionName);

                var data =
                    @"[{ ""name"": ""Ned"", ""surname"": ""Stark"", ""alive"": true, ""age"": 41, ""traits"": [""A"",""H"",""C"",""N"",""P""] },
                       { ""name"": ""Rickard"", ""surname"": ""Stark"", ""alive"": false, ""traits"": [""A"",""H"",""C"",""N"",""P""] },
                       { ""name"": ""Jaime"", ""surname"": ""Lannister"", ""alive"": true, ""age"": 36, ""traits"": [""A"",""F"",""B""] },
                       { ""name"": ""Catelyn"", ""surname"": ""Stark"", ""alive"": false, ""age"": 40, ""traits"": [""D"",""H"",""C""] },
                       { ""name"": ""Cersei"", ""surname"": ""Lannister"", ""alive"": true, ""age"": 36, ""traits"": [""H"",""E"",""F""] },
                       { ""name"": ""Jon"", ""surname"": ""Snow"", ""alive"": true, ""age"": 16, ""traits"": [""A"",""B"",""C"",""F""] },
                       { ""name"": ""Sansa"", ""surname"": ""Stark"", ""alive"": true, ""age"": 13, ""traits"": [""D"",""I"",""J""] },
                       { ""name"": ""Arya"", ""surname"": ""Stark"", ""alive"": true, ""age"": 11, ""traits"": [""C"",""K"",""L""] },
                       { ""name"": ""Robb"", ""surname"": ""Stark"", ""alive"": false, ""traits"": [""A"",""B"",""C"",""K""] },
                       { ""name"": ""Bran"", ""surname"": ""Stark"", ""alive"": true, ""age"": 10, ""traits"": [""L"",""J""] },
                       { ""name"": ""Tyrion"", ""surname"": ""Lannister"", ""alive"": true, ""age"": 32, ""traits"": [""F"",""K"",""M"",""N""] },
                       { ""name"": ""Tytos"", ""surname"": ""Lannister"", ""alive"": false, ""traits"": [""F"",""K"",""M"",""N""] },
                       { ""name"": ""Tywin"", ""surname"": ""Lannister"", ""alive"": false, ""traits"": [""O"",""M"",""H"",""F""] }]";

                var characters = JsonConvert.DeserializeObject<List<Character>>(data);

                Console.WriteLine("Creating character documents ...");

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
                Console.WriteLine("Failed to create character documents. " + e);
            }
        }

        private static async Task CreateWeaponDocuments()
        {
            try
            {
                await Arango.Collection.TruncateAsync(DatabaseName, WeaponsCollectionName);

                var data =
                    @"[{ ""type"": ""Melee"", ""name"": ""sword"" },
                       { ""type"": ""Melee"", ""name"": ""axe"" },
                       { ""type"": ""Melee"", ""name"": ""knife"" },
                       { ""type"": ""Melee"", ""name"": ""spear"" },
                       { ""type"": ""Ranged"", ""name"": ""crossbow"" },
                       { ""type"": ""Ranged"", ""name"": ""javelin"" },
                       { ""type"": ""Ranged"", ""name"": ""longbow"" }]";

                var weapons = JsonConvert.DeserializeObject<List<Weapon>>(data);

                Console.WriteLine("Creating weapon documents ...");

                if (weapons != null)
                {
                    foreach (var weapon in weapons)
                    {
                        await Arango.Graph.Vertex.CreateAsync(DatabaseName, GraphName, WeaponsCollectionName, weapon);
                    }

                    Console.WriteLine("Weapon documents created.");
                }
                else Console.WriteLine("No weapon documents created.");

                Console.WriteLine("Weapon documents created.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to create weapon documents. " + e);
            }
        }

        private static async Task CreateEdgeCollection()
        {
            try
            {
                var exists = await Arango.Collection.ExistAsync(DatabaseName, EdgeCollectionName);
                if (exists)
                {
                    await Arango.Collection.DropAsync(DatabaseName, EdgeCollectionName);
                }

                Console.WriteLine($"Creating '{EdgeCollectionName}' edge collection ...");

                await Arango.Collection.CreateAsync(DatabaseName, EdgeCollectionName, ArangoCollectionType.Edge);

                Console.WriteLine("Edge collection created.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to create edge collection. " + e);
            }
        }

        private static async Task CreateEdges()
        {
            try
            {
                await Arango.Collection.TruncateAsync(DatabaseName, EdgeCollectionName);
                await Arango.Graph.DropAsync(DatabaseName, GraphName);

                var characterRelationships = new List<CharacterRelationShip>
                {
                    new("Rickard", "Stark", "Ned", "Stark"),
                    new("Ned", "Stark", "Robb", "Stark"),
                    new("Ned", "Stark", "Sansa", "Stark"),
                    new("Ned", "Stark", "Arya", "Stark"),
                    new("Ned", "Stark", "Bran", "Stark"),
                    new("Ned", "Stark", "Jon", "Snow"),
                    new("Catelyn", "Stark", "Robb", "Stark"),
                    new("Catelyn", "Stark", "Sansa", "Stark"),
                    new("Catelyn", "Stark", "Arya", "Stark"),
                    new("Catelyn", "Stark", "Bran", "Stark"),
                    new("Tytos", "Lannister", "Tywin", "Lannister"),
                    new("Tywin", "Lannister", "Jaime", "Lannister"),
                    new("Tywin", "Lannister", "Cersei", "Lannister"),
                    new("Tywin", "Lannister", "Tyrion", "Lannister")
                };

                var weaponRelationships = new List<WeaponRelationShip>
                {
                    new("Rickard", "Stark", "sword"),
                    new("Ned", "Stark", "sword"),
                    new("Robb", "Stark", "spear"),
                    new("Sansa", "Stark", "longbow"),
                    new("Arya", "Stark", "sword"),
                    new("Jon", "Snow", "sword"),
                    new("Catelyn", "Stark", "javelin"),
                    new("Bran", "Stark", "crossbow"),
                    new("Tywin", "Lannister", "longbow"),
                    new("Tytos", "Lannister", "crossbow"),
                    new("Jaime", "Lannister", "sword"),
                    new("Cersei", "Lannister", "knife"),
                    new("Tyrion", "Lannister", "axe")
                };

                Console.WriteLine("Creating graph ...");

                await Arango.Graph.CreateAsync(DatabaseName, new ArangoGraph
                {
                    Name = GraphName,
                    EdgeDefinitions = new List<ArangoEdgeDefinition>
                    {
                        new()
                        {
                            Collection = EdgeCollectionName,
                            From = new List<string> {CollectionName},
                            To = new List<string> { CollectionName, WeaponsCollectionName }
                        }
                    }
                });

                Console.WriteLine("Creating character edges ...");

                foreach (var relationShip in characterRelationships)
                {
                    var parentKey = await GetKey($"c.name == {relationShip.ParentName} && c.surname == {relationShip.ParentSurname}", CollectionName);
                    var childKey = await GetKey($"c.name == {relationShip.ChildName} && c.surname == {relationShip.ChildSurname}", CollectionName);

                    await Arango.Graph.Edge.CreateAsync(DatabaseName, GraphName, EdgeCollectionName, new
                    {
                        From = $"{CollectionName}/{childKey}",
                        To = $"{CollectionName}/{parentKey}",
                        Label = "Child"
                    });
                }

                foreach (var relationShip in weaponRelationships)
                {
                    var weaponKey = await GetKey($"c.name == {relationShip.WeaponName}", WeaponsCollectionName);
                    var characterKey = await GetKey($"c.name == {relationShip.CharacterName} && c.surname == {relationShip.CharacterSurname}", CollectionName);

                    await Arango.Graph.Edge.CreateAsync(DatabaseName, GraphName, EdgeCollectionName, new
                    {
                        From = $"{CollectionName}/{characterKey}",
                        To = $"{WeaponsCollectionName}/{weaponKey}",
                        Label = "Weapon"
                    });
                }

                Console.WriteLine("Edges created.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to create Edges. " + e);
            }
        }

        private static async Task QueryGraph()
        {
            try
            {
                Console.WriteLine("Querying graph ...");
                Console.WriteLine("Fetching weapons of Sansa bloodline ...");

                var sansaKey = await GetKey($"c.name == \"Sansa\"", CollectionName);
                string start = $"{CollectionName}/{sansaKey}";

                FormattableString query = $"FOR v IN 1..3 OUTBOUND {start} GRAPH 'FamilyTree' FILTER v._id LIKE 'Weapon%' RETURN v";

                var result = await Arango.Query.ExecuteAsync<object>(DatabaseName, query);

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

        internal record Weapon(string Type, string Name);
        internal record CharacterRelationShip(string ParentName, string ParentSurname, string ChildName, string ChildSurname);
        internal record WeaponRelationShip(string CharacterName, string CharacterSurname, string WeaponName);
    }
}
