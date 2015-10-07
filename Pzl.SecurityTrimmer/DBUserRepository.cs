using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Runtime.Caching;
using System.Threading;

namespace Pzl.SecurityTrimmer
{
    /// <summary>
    /// User repository with caching and database backing.
    /// </summary>
    public class DBUserRepository : IUserRepository
    {
        // Constants
        private const string SELECT_ONE =
            "SELECT TOP 1 Id from Users WHERE Sid = @Sid AND Domain = 'CONTOSO'";

        private const string ID = "Id";
        private const string SID = "Sid";
        private const string PARAM = "@Sid";

        private readonly string _connectionString;




        /// <summary>
        /// Initialize the database user repository
        /// </summary>
        /// <param name="properties">configuration settings for the user repository, must contain "connectionString"</param>
        public DBUserRepository(NameValueCollection properties)
        {
            if (properties.Get("connectionString") != null)
            {
                _connectionString = properties.Get("connectionString");
            }
            else
            {
                throw
                    new ArgumentException(
                        "Configuration settings for DBUserRepository must contain \"connectionString\"");
            }
        }

        /// <summary>
        /// Lookup external id based on CONTOSO sid
        /// </summary>
        /// <param name="key">sid</param>
        /// <returns>external id</returns>
        public string Lookup(string key)
        {
            string externalId = null;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(SELECT_ONE, connection))
                {
                    try
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            externalId = reader[ID].ToString();
                        }                        
                    }
                    catch (Exception)
                    {
                        // Log the event
                    }
                }
            }
            return externalId;
        }
    }
}