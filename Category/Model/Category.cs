using ArangoDbPoc.Infra;
using System.Collections.Generic;

namespace ArangoDbPoc.Category.Model
{
    public class Category : SourcedEntity<DomainEvent>
    {
        public Id Id { get; private set; }
        public Description Description { get; private set; }

        private Category(Id id, Description description)
        {
            Apply(CategoryRegistered.Instance(id, description));
        }

        internal Category(IEnumerable<DomainEvent> stream, int streamVersion)
            : base(stream, streamVersion)
        {
        }

        public static Category Register(Id id, Description description)
        {
            return new Category(id, description);
        }

        public void When(CategoryRegistered categoryRegistered)
        {
            Id = Id.FromExisting(categoryRegistered.Id);
            Description = Description.Has(categoryRegistered.Description);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object other)
        {
            if (other == null || other.GetType() != typeof(Category))
            {
                return false;
            }

            var otherProposal = (Category)other;

            return Id.Equals(otherProposal.Id);
        }

        public override string ToString()
        {
            return $"Proposal[Id={Id} Description={Description}";
        }
    }
}
