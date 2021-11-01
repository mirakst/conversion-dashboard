using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filestreaming_Program
{
    public class Execution : ITimestampedDatabaseEntry, IComparable<Execution>
    {
        public int EXECUTION_ID { get; set; }
        public string EXECUTION_UUID {  get; set; }
        public DateTime CREATED { get; set;  }

        public override string ToString()
        {
            return $"{"[Execution]",-15} {CREATED}: {EXECUTION_ID} {EXECUTION_UUID}";
        }
        public int CompareTo(Execution execution)
        {
            return DateTime.Compare(CREATED, execution.CREATED);
        }
    }
}
