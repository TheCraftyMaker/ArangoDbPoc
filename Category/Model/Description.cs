namespace ArangoDbPoc.Category.Model
{
    public class Description
    {
        public string Text { get; }

        private Description(string text)
        {
            Text = text;
        }

        public static Description Has(string text)
        {
            return new Description(text);
        }

        public override int GetHashCode()
        {
            return Text.GetHashCode();
        }

        public override bool Equals(object other)
        {
            if (other == null || other.GetType() != typeof(Description))
            {
                return false;
            }

            return Text.Equals(((Description)other).Text);
        }

        public override string ToString()
        {
            return $"Summary[Text={Text}]";
        }
    }
}
