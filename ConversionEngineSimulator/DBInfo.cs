using System;
using System.Data.SqlClient;

namespace ConversionEngineSimulator
{
    static class DBInfo
    {
        public static string SrcConnectionString { get; private set; }
        public static string DestConnectionString { get; private set; }
        public static string DestDatabase { get; private set; }
        public static string SrcDatabase { get; private set; }
        public static string Servername { get; private set; }
        public static DateTime convStartTime; //Time of the first database entry

        public static void Initialize(string server, string srcDb, string dstDb)
        {
            SrcConnectionString = $"Data Source={server};Initial Catalog={srcDb};Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            DestConnectionString = $"Data Source={server};Initial Catalog={dstDb};Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            Console.WriteLine(SrcConnectionString);
            Console.WriteLine(DestConnectionString);
        }

        public static SqlConnection ConnectToSrcDB() //Returns a connection to the source DB
        {
            return new SqlConnection(SrcConnectionString);
        } 
        public static SqlConnection ConnectToDestDB() //Returns a conection to the destination DB
        {
            return new SqlConnection(DestConnectionString);
        }
    }
}