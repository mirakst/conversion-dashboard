using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashboardSettings
{
    public class Profile
    {
        public Profile(string name, string conversion, string dataSource, string database, int timeout)
        {
            Id = NextId++;
            Name = name;
            Conversion = conversion;
            DataSource = dataSource;
            Database = database;
            Timeout = timeout;
        }

        public Profile(int id, string name, string conversion, string dataSource, string database, int timeout)
            : this(name, conversion, dataSource, database, timeout)
        {
            Id = id;
            NextId = id + 1;
        }

        public Profile() : this("", "", "", "", 30)
        {
        }

        private static int NextId { get; set; } = 0;
        public int Id { get; }
        public string Name { get; set; }
        public string Conversion { get; set; }
        public string DataSource { get; set; }
        public string Database { get; set; }
        public int Timeout { get; set; } = 30;

        private string? _connectionString = null;
        public string ConnectionString(string userId, string password)
        {
            if (_connectionString is null)
            {
                DbConnectionStringBuilder builder = new();
                builder.Add("Data Source", DataSource);
                builder.Add("Initial Catalog", Database);
                builder.Add("Connect Timeout", Timeout);
                builder.Add("User Id", userId);
                builder.Add("Password", password);
                _connectionString = builder.ConnectionString;
            }
            return _connectionString;
        }
    }
}
