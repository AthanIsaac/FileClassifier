using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLU.Classifier.Contracts
{
    public class DatabaseLayer : IDatabaseLayer
    {
        private static readonly string connectionString = "Data Source=DESKTOP-I9QG8E6;Initial Catalog=ChurchLibraryDB;Integrated Security=True";
        private static readonly SqlConnection connection = new SqlConnection(connectionString);
        public void AddDisplayNameToTag(string name, int tagId)
        {
        
            SqlCommand cmd = new SqlCommand("spTagDisplayNames_AddToTag", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@TagDisplayName", SqlDbType.NVarChar).Value = name;
            cmd.Parameters.AddWithValue("@TagId", SqlDbType.Int).Value = tagId;
            
            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
        }

        public LibraryFile AddFile(byte[] content, string name, string type)
        {
            SqlCommand cmd = new SqlCommand("spFiles_AddFile", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Name", SqlDbType.NVarChar).Value = name;
            cmd.Parameters.AddWithValue("@Content", SqlDbType.VarBinary).Value = content;
            cmd.Parameters.AddWithValue("@Type", SqlDbType.VarChar).Value = type;
            connection.Open();
            var result = cmd.ExecuteScalar();
            connection.Close();

            if (result == null) throw new Exception("File not added properly");

            int id = (int)result;

            return new LibraryFile(id, name, type);
        }

        public void AddFileTag(int fileId, int tagId)
        {
            SqlCommand cmd = new SqlCommand("spFileTags_AddFileTag", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@FileId", SqlDbType.Int).Value = fileId;
            cmd.Parameters.AddWithValue("@TagId", SqlDbType.Int).Value = tagId;

            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
        }

        public int AddTag(string name)
        {
            SqlCommand cmd = new SqlCommand("spTags_AddTag", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@PrimaryName", SqlDbType.NVarChar).Value = name;

            connection.Open();
            var result = cmd.ExecuteScalar();
            connection.Close();

            if (result == null) throw new Exception("Tag not added properly");

            return (int)result;
        }

        public int FileExists(byte[] content)
        {
            SqlCommand cmd = new SqlCommand("spFiles_FileExists", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Content", SqlDbType.VarBinary).Value = content;
            
            connection.Open();
            var result = cmd.ExecuteScalar();
            connection.Close();
            if (result == null)
            {
                return -1;
            }
            return (int) result;
        }

        public LibraryTag[] GetTags(string nameSegment)
        {
            SqlCommand cmd = new SqlCommand("spTags_GetTags", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@NameSegment", SqlDbType.NVarChar).Value = nameSegment;

            DataTable dt = new DataTable();
            connection.Open();
            dt.Load(cmd.ExecuteReader());
            connection.Close();

            List<LibraryTag> tags = new List<LibraryTag>();
            foreach (DataRow row in dt.Rows)
            {
                // id, name
                tags.Add(new LibraryTag((int)row.ItemArray[0], (string)row.ItemArray[1]));
            }

            return tags.ToArray();
        }

        public string[] GetDisplayNames(int tagId)
        {
            SqlCommand cmd = new SqlCommand("spTagDisplayNames_GetDisplayNames", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@TagId", SqlDbType.Int).Value = tagId;

            DataTable dt = new DataTable();
            connection.Open();
            dt.Load(cmd.ExecuteReader());
            connection.Close();

            List<string> tags = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                // display name
                tags.Add(row[0].ToString());
            }

            return tags.ToArray();
        }

        public byte[] GetFileContentById(int fileId)
        {
            SqlCommand cmd = new SqlCommand("spFiles_GetContentById", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@FileId", SqlDbType.Int).Value = fileId;

            DataTable dt = new DataTable();
            connection.Open();
            dt.Load(cmd.ExecuteReader());
            connection.Close();

            return (byte[])dt.Rows[0][0];
        }

        public int TagExists(string name)
        {
            SqlCommand cmd = new SqlCommand("spTags_TagExists", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@PrimaryName", SqlDbType.NVarChar).Value = name;

           // var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.NVarChar);
           // returnParameter.Direction = ParameterDirection.ReturnValue;

            connection.Open();
            var result = cmd.ExecuteScalar();
           // var result = returnParameter.Value;
            connection.Close();
            if (result == null)
            {
                return -1;
            }
            return (int)result;
        }

       /* public static string[] GetTags(string nameSegment)
        {
            SqlCommand cmd = new SqlCommand("spTags_GetTagNames", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@NameSegment", SqlDbType.NVarChar).Value = nameSegment;

            DataTable dt = new DataTable();
            connection.Open();
            dt.Load(cmd.ExecuteReader());
            connection.Close();
            List<string> tags = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                foreach (DataColumn column in dt.Columns)
                {
                    tags.Add(row[column].ToString());
                }
            }
            return tags.ToArray();
        }
        */
    }
}
