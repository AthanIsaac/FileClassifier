namespace RLU.Classifier.Providers
{
    public class LibraryTag
    {
        public readonly int id;
        public readonly string name;

        public LibraryTag(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
        public override string ToString()
        {
            return name;
        }
    }
}