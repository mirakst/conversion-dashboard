using System;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;

namespace Filestreaming_Program
{
    static class DBInfo
    {
        public static string SrcConnectionString { get; private set; }
        public static string DestConnectionString { get; private set; }
        public static DateTime convStartTime; //Time of the first database entry

        public static void Initialize()
        {
            using StreamReader reader = new(File.OpenRead("./hostinfo.meta"));
            string[] parts = reader.ReadLine().Split(';');

            if(parts.Length != 3) 
            {
                throw new ArgumentException("Invalid format in [hostinfo.meta] file. Expected [SERVER NAME;SOURCE DB;DESTINATION DB]");
            }

            string hostname = parts[0];
            string src = parts[1];
            string dest = parts[2];

            SrcConnectionString = $"Data Source={hostname};Initial Catalog={src};Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            DestConnectionString = $"Data Source={hostname};Initial Catalog={dest};Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
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