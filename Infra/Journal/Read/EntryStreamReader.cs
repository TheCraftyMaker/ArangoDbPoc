using System.Threading.Tasks;

namespace ArangoDbPoc.Infra.Journal.Read
{
    public class EntryStreamReader
    {
        private readonly Journal _journal;

        internal EntryStreamReader(Journal journal)
        {
            _journal = journal;
        }

        public async Task<EntryStream> StreamFor(string streamName)
        {
            return await _journal.ReadStream(streamName);
        }
    }
}
