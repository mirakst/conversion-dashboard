using System;

namespace ConversionEngineSimulator
{
    public interface ITimestampedDatabaseEntry //Interface for database entries that are streamed asynchronously
    {
        public abstract DateTime CREATED { get; set;  } //The timestamp of the entry

        public string ToString(); //A method to log the entry in the console
    }
}
