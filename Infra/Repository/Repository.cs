using System;
using System.Collections.Generic;
using ArangoDbPoc.Infra.Journal;
using ArangoDbPoc.Infra.Journal.Write;

namespace ArangoDbPoc.Infra.Repository
{
    public abstract class Repository
    {
        protected EntryBatch ToBatch<T>(List<T> sources)
        {
            var batch = new EntryBatch(sources.Count);
            foreach (var source in sources)
            {
                var eventBody = Serialization.Serialize(source);
                batch.AddEntry(source.GetType().AssemblyQualifiedName, eventBody);
            }

            return batch;
        }

        protected List<T> ToSourceStream<T>(List<EntryValue> stream)
        {
            var sourceStream = new List<T>(stream.Count);
            foreach (var value in stream)
            {
                var sourceType = Type.GetType(value.Type);
                var source = (T)Serialization.Deserialize(value.Body, sourceType);
                sourceStream.Add(source);
            }

            return sourceStream;
        }

    }
}
