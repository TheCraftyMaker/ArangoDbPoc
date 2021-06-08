using ArangoDbPoc.Infra;

namespace ArangoDbPoc.Category.Model
{
    public class CategoryRegistered : DomainEvent
    {
        public string Id { get; private init; }
        public string Description { get; private init; }
        
        public static CategoryRegistered Instance(Id id, Description description)
        {
            var categoryRegistered = new CategoryRegistered
            {
                Id = id.Value,
                Description = description.Text
            };
            return categoryRegistered;
        }
    }
}
