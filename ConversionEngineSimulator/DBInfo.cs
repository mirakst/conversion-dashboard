using System;
using System.Data.SqlClient;

namespace ConversionEngineSimulator
{
    static class DBInfo
    {
        public static string SrcConnectionString { get; private set; }
        public static string DestConnectionString { get; private set; }
        public static DateTime convStartTime; //Time of the first database entry

        /// <summary>
        /// Creates and assigns the <see cref="SrcConnectionString"/> and <see cref="DestConnectionString"/> properties from the specified inputs.
        /// </summary>
        /// <param name="server">The server hosting the database, e.g. '(local)\SQLEXPRESS' or an IP address.</param>
        /// <param name="srcDb">The name of the source database (to stream from).</param>
        /// <param name="dstDb">The name of the destination database (to stream to).</param>
        public static void Initialize(string server, string srcDb, string dstDb)
        {
            SrcConnectionString = $"Data Source={server};Initial Catalog={srcDb};Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            DestConnectionString = $"Data Source={server};Initial Catalog={dstDb};Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        }

        /// <summary>
        /// Creates an <see cref="SqlConnection"/> to the source database.
        /// </summary>
        /// <returns>The connection object.</returns>
        public static SqlConnection ConnectToSrcDB() //Returns a connection to the source DB
        {
            return new SqlConnection(SrcConnectionString);
        }

        /// <summary>
        /// Creates an <see cref="SqlConnection"/> to the destination database.
        /// </summary>
        /// <returns>The connection object.</returns>
        public static SqlConnection ConnectToDestDB() //Returns a conection to the destination DB
        {
            return new SqlConnection(DestConnectionString);
        }
    }
}