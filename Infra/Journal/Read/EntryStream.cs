using System.Collections.Generic;

namespace ArangoDbPoc.Infra.Journal.Read
{
    public class EntryStream
    {
        public string Snapshot { get; }
        public List<EntryValue> Stream { get; }
        public string StreamName { get; }
        public int StreamVersion { get; }

        internal EntryStream(string streamName, int streamVersion, List<EntryValue> stream, string snapshot)
        {
            StreamName = streamName;
            StreamVersion = streamVersion;
            Stream = stream;
            Snapshot = snapshot;
        }

        public bool HasSnapshot()
        {
            return !string.IsNullOrEmpty(Snapshot);
        }
    }
}
