using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArangoDbPoc.Infra.Journal.Read;
using ArangoDbPoc.Infra.Journal.Write;

namespace ArangoDbPoc.Infra.Journal
{
    public class Journal
    {
        private static readonly Dictionary<string, Journal> Journals = new();

        private readonly IJournalStore _store;

        public string Name { get; set; }

        protected Journal(string name)
        {
            Name = name;
            _store = new ArangeDbJournalStore(name);
        }

        public static Journal Open(string name)
        {
            if (Journals.ContainsKey(name))
            {
                return Journals[name];
            }

            var eventJournal = new Journal(name);

            Journals.Add(name, eventJournal);

            return eventJournal;
        }

        public void Close()
        {
            _store.Close();
            Journals.Remove(Name);
        }

        public EntryStreamReader StreamReader()
        {
            return new EntryStreamReader(this);
        }

        public async Task Write(string streamName, int streamVersion, EntryBatch batch)
        {
            foreach (var entry in batch.Entries)
            {
                await _store.Add(new EntryValue(streamName, streamVersion, entry.Type, entry.Body, entry.Snapshot, DateTime.Now));
            }
        }

        internal async Task<int> GreatestId()
        {
            return await _store.LastId();
        }

        internal async Task<EntryStream> ReadStream(string streamName)
        {
            EntryValue latestSnapshotValue = null;

            var values = new List<EntryValue>();
            var storeValues = await _store.GetStream(streamName);
            foreach (var value in storeValues)
            {
                if (value.HasSnapshot())
                {
                    values.Clear();
                    latestSnapshotValue = value;
                }
                else
                {
                    values.Add(value);
                }
            }

            var snapshotVersion = latestSnapshotValue?.StreamVersion ?? 0;
            var streamVersion = values.Count == 0 ? snapshotVersion : values[^1].StreamVersion;

            return new EntryStream(streamName, streamVersion, values,
                latestSnapshotValue == null ? string.Empty : latestSnapshotValue.Snapshot);
        }
    }
}
