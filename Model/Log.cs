namespace Model
{
    public class Log
    {
        public Log()
        {

        }

        public List<LogMessage> Messages { get; set; } = new();
        
        public override int GetHashCode()
        {
            return Messages.GetHashCode();
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
