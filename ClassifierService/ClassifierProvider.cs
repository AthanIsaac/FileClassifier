using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace RLU.Classifier.Service
{
    public class ClassifierProvider : IClassifierProvider
    {
        private static SqlConnection connection = new SqlConnection("Data Source=DESKTOP-I9QG8E6;Initial Catalog=ChurchLibraryDB;Integrated Security=True");
        public IDatabaseLayer _databaseLayer;

        public ClassifierProvider(IDatabaseLayer databaseLayer)
        {
            _databaseLayer = databaseLayer;
        }

        public bool CheckPath(string path)
        {

            return Directory.Exists(path) || File.Exists(path);
        }

        public LibraryTag[] GetTags(string nameSegment)
        {
            return _databaseLayer.GetTags(nameSegment);
        }

        public int TagFiles(string path, string name)
        {
            List<FileInfo> files = FilesInDirectory(path);
            if(files.Count == 0)
            {
                throw new IOException("This directory has no non-empty files");
            }
            int tagId = _databaseLayer.TagExists(name);
            bool exists = tagId != -1;
            if (!exists)
            {
                tagId = _databaseLayer.AddTag(name);
            }

            foreach (FileInfo f in files)
            {
                // check if file is in the db
                // if yes, tag it else add then tag
                byte[] content = File.ReadAllBytes(f.FullName);
                int fileId = _databaseLayer.FileExists(content);
                if (fileId == -1)
                {
                    fileId = _databaseLayer.AddFile(content, Path.GetFileNameWithoutExtension(f.Name), f.Extension).FileID;
                }

                _databaseLayer.AddFileTag(fileId, tagId);
            }
            return exists ? -1 : tagId;
        }

        public void AddDisplayNamesToTag(int tagId, string unparsedNames)
        {
            string[] names = unparsedNames.Trim().Split('\n');
            foreach (string name in names)
            {
                _databaseLayer.AddDisplayNameToTag(name.Trim(), tagId);
            }
        }

        public string[] GetDisplayNamesForTag(int tagId)
        {
            return _databaseLayer.GetDisplayNames(tagId);
        }

        private List<FileInfo> FilesInDirectory(string path)
        {
            if (!CheckPath(path)) throw new Exception("Invalid Path!");
            List<FileInfo> files = new List<FileInfo>();
            if (File.Exists(path))
            {
                FileInfo f = new FileInfo(path);
                if (!f.Attributes.HasFlag(FileAttributes.Hidden) && f.Length != 0)
                {
                    files.Add(new FileInfo(path));
                }
                return files;
            }

            foreach (String p in Directory.GetFiles(path))
            {
                FileInfo f = new FileInfo(p);
                if (!f.Attributes.HasFlag(FileAttributes.Hidden) && f.Length != 0)
                {
                    files.Add(f);
                }
            }
            foreach (String p in Directory.GetDirectories(path))
            {
                files = files.Concat(FilesInDirectory(p)).ToList();
            }
            return files;
        }
    }
}
