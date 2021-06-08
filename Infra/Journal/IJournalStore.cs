using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArangoDbPoc.Infra.Journal
{
    public interface IJournalStore
    {
        Task Open(string name);
        void Close();
        Task Add(EntryValue entry);
        Task<int> LastId();
        Task<EntryValue> GetEntry(int id);
        Task<IEnumerable<EntryValue>> GetStream(string stream);
    }
}
