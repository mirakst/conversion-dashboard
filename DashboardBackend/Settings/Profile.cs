using System.Data.Common;
using System.Text.Json.Serialization;

namespace DashboardBackend.Settings
{
    public delegate void ProfileChanged();

    public class Profile
    {
        public Profile(string name, string conversion, string dataSource, string database, int timeout)
        {
            Id = _nextId++;
            Name = name;
            Conversion = conversion;
            DataSource = dataSource;
            Database = database;
            Timeout = timeout;
        }

        [JsonConstructor]
        public Profile(int id, string name, string conversion, string dataSource, string database, int timeout)
            : this(name, conversion, dataSource, database, timeout)
        {
            Id = id;
            _nextId = id + 1;
        }

        public Profile() : this("", "", "", "", 5)
        {
        }

        private static int _nextId = 1;

        public int Id { get; }
        public string Name { get; set; }
        public string Conversion { get; set; }
        public string DataSource { get; set; }
        public string Database { get; set; }
        public int Timeout { get; set; } = 5;
        [JsonIgnore]
        public string ConnectionString { get; private set; }
        [JsonIgnore]
        public bool HasReceivedCredentials { get; set; }
        [JsonIgnore]
        public bool HasStartedMonitoring { get; set; }
        public event ProfileChanged ProfileChanged;

        /// <summary>
        /// Invokes the <see cref="ProfileChanged"/> event.
        /// </summary>
        public void OnProfileChange()
        {
            ProfileChanged?.Invoke();
        }

        /// <summary>
        /// Checks if the <see cref="ProfileChanged"/> event has any listeners.
        /// </summary>
        /// <returns>True, if it has listeners.</returns>
        public bool HasEventListeners()
        {
            return ProfileChanged != null;
        }

        /// <summary>
        /// Builds a connection string from the user settings and login.
        /// </summary>
        /// <param name="userId">The username for the connection.</param>
        /// <param name="password">The password for the connection.</param>
        public void BuildConnectionString(string userId, string password)
        {
            DbConnectionStringBuilder builder = new();
            builder.Add("Data Source", DataSource);
            builder.Add("Initial Catalog", Database);
            builder.Add("Connect Timeout", Timeout);
            builder.Add("Integrated Security", "True");
            builder.Add("User Id", userId);
            builder.Add("Password", password);
            // TODO: get provider type (Oracle/SQL) and use it to build the specific connection string
            ConnectionString = builder.ConnectionString;
        }

        public override bool Equals(object? obj)
        {
            return (obj as Profile)?.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}
