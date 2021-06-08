using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDbPoc.Infra
{
    public abstract class DomainEvent : ISourceType
    {
        public long OccuredOn { get; }
        public int Eventversion { get; }

        public static DomainEvent NULL = new NullDomainEvent();

        protected DomainEvent()
            : this(1)
        {
        }

        protected DomainEvent(int eventVersion)
        {
            OccuredOn = DateTime.Now.Ticks;
            Eventversion = eventVersion;
        }

        public static List<DomainEvent> All(params DomainEvent[] domainEvents)
        {
            return All(new List<DomainEvent>(domainEvents));
        }

        public static List<DomainEvent> All(List<DomainEvent> domainEvents)
        {
            var all = new List<DomainEvent>(domainEvents.Count);
            foreach (var @event in domainEvents)
            {
                if (!@event.IsNull())
                {
                    all.Add(@event);
                }
            }

            return all;
        }

        public static List<DomainEvent> None()
        {
            return new List<DomainEvent>(0);
        }

        public virtual bool IsNull()
        {
            return false;
        }

        internal class NullDomainEvent : DomainEvent
        {
            public override bool IsNull()
            {
                return true;
            }
        }
    }
}
