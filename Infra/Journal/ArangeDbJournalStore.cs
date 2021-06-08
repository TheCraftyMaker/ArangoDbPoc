using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Arango;
using Core.Arango.Protocol;
using Core.Arango.Serialization.Newtonsoft;

namespace ArangoDbPoc.Infra.Journal
{
    public class ArangeDbJournalStore : IJournalStore
    {
        private IArangoContext _arango;
        private string _storeCollectionName;

        private readonly string _storeName;

        public ArangeDbJournalStore(string storeName)
        {
            _storeName = storeName;
        }

        public async Task Open(string name)
        {
            var serializer = new ArangoNewtonsoftSerializer(new ArangoNewtonsoftCamelCaseContractResolver());
            var configuration = new ArangoConfiguration { Serializer = serializer };

            _arango = new ArangoContext(
                "Server=http://localhost:8529;User=root;Password=openSesame;",
                configuration);

            //Create Database if it not exists
            //TODO: Add switch in configuration to disable automatic creation of databases
            var databaseExists = await _arango.Database.ExistAsync(_storeName);
            if (!databaseExists)
            {
                await _arango.Database.CreateAsync(_storeName);
            }

            //Create Collection if it not exists
            //TODO: Add switch in configuration to disable automatic creation of collection
            var collectionExists = await _arango.Collection.ExistAsync(_storeName, name);
            if (!collectionExists)
            {
                await _arango.Collection.CreateAsync(_storeName, new ArangoCollection
                {
                    Name = name,
                    Type = ArangoCollectionType.Document,
                    KeyOptions = new ArangoKeyOptions
                    {
                        Type = ArangoKeyType.Autoincrement,
                        AllowUserKeys = false,
                        Increment = 1
                    }
                });
            }
            _storeCollectionName = name;
        }

        public void Close()
        {
            _arango = null;
        }

        public async Task Add(EntryValue entry)
        {
            await _arango.Document.CreateAsync(_storeName, _storeCollectionName, entry);
        }

        public async Task<int> LastId()
        {
            FormattableString query = $"FOR e IN {_storeCollectionName:@} SORT e.timestamp ASC LIMIT 1 RETURN e._key";

            var result = await _arango.Query.ExecuteAsync<int>(_storeName, $"{query}");
            if (!result.Any())
            {
                throw new InvalidOperationException("No key found");
            }

            return result.First();
        }

        public async Task<EntryValue> GetEntry(int id)
        {
            FormattableString query = $"FOR e IN {_storeCollectionName:@} FILTER e._key == {id} RETURN e";

            var result = await _arango.Query.ExecuteAsync<EntryValue>(_storeName, $"{query}");
            if (!result.Any())
            {
                throw new InvalidOperationException($"No entry found for key: {id}");
            }

            return result.First();
        }

        public async Task<IEnumerable<EntryValue>> GetStream(string stream)
        {
            FormattableString query = $"FOR e IN {_storeCollectionName:@} FILTER e.streamname == {stream} RETURN e";

            var result = _arango.Query.ExecuteStreamAsync<EntryValue>(_storeName, $"{query}");
            return await result.ToListAsync();
        }
    }
}
