using System;

namespace ArangoDbPoc.Infra.Journal
{
    public class EntryValue
    {
        public static int NO_STREAM_VERSION = -1;

        public string Body { get; }
        public string Snapshot { get; }
        public string StreamName { get; }
        public int StreamVersion { get; }
        public string Type { get; }
        public DateTime Timestamp { get; }

        internal EntryValue(
            string streamName,
            int streamVersion,
            string type,
            string body,
            string snapshot,
            DateTime timestamp)
        {
            StreamName = streamName;
            StreamVersion = streamVersion;
            Type = type;
            Body = body;
            Snapshot = snapshot;
            Timestamp = timestamp;
        }

        public bool HasSnapshot()
        {
            return !string.IsNullOrEmpty(Snapshot);
        }

        public override int GetHashCode()
        {
            return StreamName.GetHashCode() + StreamVersion;
        }

        public override bool Equals(object other)
        {
            if (other == null || other.GetType() != typeof(EntryValue))
            {
                return false;
            }

            var otherEntryValue = (EntryValue)other;

            return StreamName.Equals(otherEntryValue.StreamName) &&
                   StreamVersion == otherEntryValue.StreamVersion &&
                   Type.Equals(otherEntryValue.Type) &&
                   Body.Equals(otherEntryValue.Body) &&
                   Snapshot.Equals(otherEntryValue.Snapshot) &&
                   Timestamp.Equals(otherEntryValue.Timestamp);
        }

        public override string ToString()
        {
            return $"EntryValue[streamName={StreamName} StreamVersion={StreamVersion} " +
                   $"type={Type} body={Body} snapshot={Snapshot} timestamp={Timestamp:dd/MM/yyyy}";
        }
    }
}
