using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace RLU.Classifier.Contracts
{
    public class ClassifierProvider : IClassifierProvider
    {
        // Connection to my database. Change this connection string to plug into another database.
        private static SqlConnection connection = new SqlConnection("Data Source=DESKTOP-I9QG8E6;Initial Catalog=ChurchLibraryDB;Integrated Security=True");
        public IDatabaseLayer _databaseLayer;

        // Pass in databaselayer for dipendency injection
        public ClassifierProvider(IDatabaseLayer databaseLayer)
        {
            _databaseLayer = databaseLayer;
        }

        /// <summary>
        /// Checks if a given path is a path to an existing file or directory
        /// </summary>
        /// <param name="path"> the given path to check </param>
        /// <returns> true if the path is valid </returns>
        public bool CheckPath(string path)
        {

            return Directory.Exists(path) || File.Exists(path);
        }

        /// <summary>
        /// retieves the tags that contain a word that begins with the given name segment
        /// </summary>
        /// <param name="nameSegment"> the name segment to get tags based on </param>
        /// <returns> a list of LibraryTags that have name containing the given name segment </returns>
        public LibraryTag[] GetTags(string nameSegment)
        {
            return _databaseLayer.GetTags(nameSegment);
        }

        /// <summary>
        /// tags all the non-empty files in the given path with the given tag name.
        /// if there are no non-empty files, throws an IO exception.
        /// </summary>
        /// <param name="path"> the path if files to tag </param>
        /// <param name="name"> the name of the tag </param>
        /// <returns> the tag ID </returns>
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

            return tagId;
        }

        /// <summary>
        /// takes a tagId and a newline separated string of display names to add to the tag
        /// </summary>
        /// <param name="tagId"> the tag to add display names to </param>
        /// <param name="unparsedNames">the newline separated string of display names </param>
        public void AddDisplayNamesToTag(int tagId, string unparsedNames)
        {
            string[] names = unparsedNames.Trim().Split('\n');
            foreach (string name in names)
            {
                _databaseLayer.AddDisplayNameToTag(name.Trim(), tagId);
            }
        }

        /// <summary>
        /// retrieves the display names of a tag
        /// </summary>
        /// <param name="tagId"> the tag to retrive display names for </param>
        /// <returns> a string array of display names </returns>
        public string[] GetDisplayNamesForTag(int tagId)
        {
            return _databaseLayer.GetDisplayNames(tagId);
        }

        // takes a path and return a list of all the non-empty, non-hidden files in the directory
        private List<FileInfo> FilesInDirectory(string path)
        {
            if (!CheckPath(path)) throw new Exception("Invalid Path!");
            List<FileInfo> files = new List<FileInfo>();

            // base case if the path is a file, add it to the list and return.
            if (File.Exists(path))
            {
                FileInfo f = new FileInfo(path);
                if (!f.Attributes.HasFlag(FileAttributes.Hidden) && f.Length != 0)
                {
                    files.Add(new FileInfo(path));
                }
                return files;
            }

            // add all the files in the given directory
            foreach (String p in Directory.GetFiles(path))
            {
                FileInfo f = new FileInfo(p);
                if (!f.Attributes.HasFlag(FileAttributes.Hidden) && f.Length != 0)
                {
                    files.Add(f);
                }
            }

            // recursively add all the files from each subdirectory to the list
            foreach (String p in Directory.GetDirectories(path))
            {
                files = files.Concat(FilesInDirectory(p)).ToList();
            }
            return files;
        }
    }
}
