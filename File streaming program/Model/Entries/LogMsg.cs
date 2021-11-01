using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filestreaming_Program
{
    public class LogMsg : ITimestampedDatabaseEntry, IComparable<LogMsg>
    {
        public DateTime CREATED { get; set;  }
        public string LOG_MESSAGE { get; set; }
        public string LOG_LEVEL { get; set; }
        public int EXECUTION_ID { get; set; }
        public int CONTEXT_ID { get; set; }

        public int CompareTo(LogMsg msg)
        {
            return DateTime.Compare(CREATED, msg.CREATED);
        }

        public override string ToString()
        {
            return $"{"[Log]",-15} {CREATED}: {LOG_MESSAGE} {LOG_LEVEL}";
        }
    }
}
