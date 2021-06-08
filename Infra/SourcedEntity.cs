using System.Collections.Generic;

namespace ArangoDbPoc.Infra
{
    public abstract class SourcedEntity<TSource>
    {
        private readonly List<TSource> _applied;
        private readonly int _currentVersion;

        public List<TSource> Applied => _applied;
        public int NextVersion => _currentVersion + 1;
        public int CurrentVersion => _currentVersion;

        protected SourcedEntity()
        {
            _applied = new List<TSource>();
            _currentVersion = 0;
        }

        protected SourcedEntity(IEnumerable<TSource> stream, int streamVersion)
            : this()
        {
            foreach (var source in stream)
            {
                DispatchWhen(source);
            }

            _currentVersion = streamVersion;
        }

        protected void Apply(TSource source)
        {
            _applied.Add(source);
            DispatchWhen(source);
        }

        protected void DispatchWhen(TSource source)
        {
            ((dynamic)this).When((dynamic)source);
        }
    }
}
