namespace Model
{
    public class RamLoad : PerformanceMetric
    {
        #region Constructors
        public RamLoad(int executionId, double load, long available, DateTime date)
        {
            ExecutionId = executionId;
            Load = load;
            Date = date;
            Available = available;
        }
        #endregion Constructors

        #region Properties

        public long Available;
        #endregion Properties
        public override string ToString()
        {
            return $"{Date}: {Load:P} bytes";
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
