using System;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Filestreaming_Program
{
    static class DBInfo
    {
        //Replace DesktopConn or LaptopConn with server name. These are stored for Mikkels computers
        public const string SrcConnectionString = "Data Source=" + ServerName + ";Initial Catalog=ANS_CUSTOM_2;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public const string DestConnectionString = "Data Source=" + ServerName + ";Initial Catalog=ACL_DEST;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public const string ServerName = MikkelLaptopConn;
        
        //A list of SQL servers the filestreaming application will run on
        #region SQL SERVERS
        public const string MikkelLaptopConn = "DESKTOP-7BJMDGL";
        public const string MikkelDesktopConn = "DESKTOP-6FI06VR";
        #endregion SQL SERVERS

        public static DateTime convStartTime; //Time of the first database entry
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