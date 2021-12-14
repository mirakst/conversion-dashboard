using System.Globalization;

namespace Model
{
    public class RamLoad : PerformanceMetric
    {
        public RamLoad()
        {

        }

        public RamLoad(int executionId, long available, DateTime date)
        {
            ExecutionId = executionId;
            Date = date;
            Available = available;
        }


        public long Available;
        
        public override string ToString()
        {
            return $"{Date.ToString(new CultureInfo("da-DK"))}: {Load:P} bytes";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ExecutionId, Load, Date, Available);
        }

        public override bool Equals(object obj)
        {
            if (obj is not RamLoad other)
            {
                return false;
            }

            return GetHashCode() == other.GetHashCode();
        }
    }
}
