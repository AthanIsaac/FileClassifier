namespace RLU.Classifier.Service
{
    public interface IClassifierProvider
    {
        void AddDisplayNamesToTag(int tagId, string unparsedNames);
        bool CheckPath(string path);
        string[] GetDisplayNamesForTag(int tagId);
        LibraryTag[] GetTags(string nameSegment);
        int TagFiles(string path, string name);
    }
}