using System.Collections.Generic;

namespace ConversionEngineSimulator
{
    public interface IDatabaseTable //Interface for database tables. On top of this, they include Entries<T> to store their entries
    {
        public string ColumnNames { get; } //Comma-separated list of column names
        public string OutputColumnNames { get; } //Comma-separated list of column names preceded by @. Example: "@ID, @TIME"
        public string TableName { get; } //Name of the database table
    }
}