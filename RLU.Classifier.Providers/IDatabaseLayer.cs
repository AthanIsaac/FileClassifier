namespace RLU.Classifier.Providers
{
    public interface IDatabaseLayer
    {
        void AddDisplayNameToTag(string name, int tagId);
        LibraryFile AddFile(byte[] content, string name, string type);
        void AddFileTag(int fileId, int tagId);
        int AddTag(string name);
        int FileExists(byte[] content);
        string[] GetDisplayNames(int tagId);
        byte[] GetFileContentById(int fileId);
        LibraryTag[] GetTags(string nameSegment);
        int TagExists(string name);
    }
}