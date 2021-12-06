using static Model.LogMessage;

namespace Model
{
    public class Log
    {
        #region Constructors
        public Log()
        {

        }
        #endregion Constructors

        #region Properties
        public List<LogMessage> Messages { get; set; } = new();
        public int InfoCount => Messages.Count(m => m.Type.HasFlag(LogMessageType.Info));
        public int WarnCount => Messages.Count(m => m.Type.HasFlag(LogMessageType.Warning));
        public int ErrorCount => Messages.Count(m => m.Type.HasFlag(LogMessageType.Error));
        public int FatalCount => Messages.Count(m => m.Type.HasFlag(LogMessageType.Fatal));
        public int ValidationCount => Messages.Count(m => m.Type.HasFlag(LogMessageType.Validation));
        public DateTime LastModified { get; set; } = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue; //Date of last modification.
        #endregion Properties
        public override int GetHashCode()
        {
            return HashCode.Combine(Messages.GetHashCode(),
                                    WarnCount,
                                    InfoCount,
                                    ErrorCount,
                                    FatalCount,
                                    ValidationCount);
        }
        public override bool Equals(object obj)
        {
            if (obj is not Log other)
                return false;

            if (Messages.Count == 0 && other.Messages.Count == 0)
                return true;
            return GetHashCode() == other.GetHashCode();

        }
    }
}
