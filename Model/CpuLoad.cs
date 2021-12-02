namespace Model
{
    public class CpuLoad : PerformanceMetric
    {
        #region Constructors
        public CpuLoad(int executionId, double load, DateTime date)
        {
            ExecutionId = executionId;
            Load = load;
            Date = date;
        }
        #endregion Constructors

        public override string ToString()
        {
            return $"{Date.ToLongTimeString()}: {Load}%";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ExecutionId, Load, Date);
        }

        public override bool Equals(object obj)
        {
            if (obj is not CpuLoad other)
            {
                return false;
            }

            return GetHashCode() == other.GetHashCode();
        }
    }
}