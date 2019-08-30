using System;
using System.ComponentModel;

namespace RLU.Classifier.Service
{
    [ImmutableObject(true)]
    public class LibraryFile
    {
        private readonly int fileID;
        private readonly string fileName;
        private readonly string fileType;

        private byte[] content;

        public LibraryFile(int fileID, string fileName, string fileType)
        {
            this.fileID = fileID;
            this.fileName = fileName;
            this.fileType = fileType;
        }

        public int FileID
        {
            get { return fileID; }
        }

        public string FileName
        {
            get { return fileName; }
        }

        public string FileType
        {
            get { return fileType; }
        }

        public byte[] Content
        {
            get
            {
                if (content == null)
                    LoadContent();

                return content;
            }
        }

        private void LoadContent()
        {
            // content = DatabaseLayer.GetFileContentById(fileID);
            throw new NotImplementedException();
        }
    }
}
