using System;

namespace ArangoDbPoc.Category.Model
{
    public class Id
    {
        public string Value { get; }

        private Id()
            : this(Guid.NewGuid().ToString())
        {
        }

        private Id(string referenceId)
        {
            Value = referenceId;
        }

        public static Id FromExisting(string referenceId)
        {
            return new Id(referenceId);
        }

        public static Id Unique()
        {
            return new Id();
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object other)
        {
            if (other == null || other.GetType() != typeof(Id))
            {
                return false;
            }

            var otherId = (Id)other;

            return this.Value.Equals(otherId.Value);
        }

        public override string ToString()
        {
            return $"Id[Value={Value}]";
        }
    }
}
