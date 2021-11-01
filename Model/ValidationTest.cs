namespace Model
{
    public class ValidationTest
    {
        public ValidationTest(DateTime date, string name, ValidationStatus status, Manager manager)
        {
            Date = date;
            Name = name;
            Status = status;
            Manager = manager;
        }

        public enum ValidationStatus
        {
            OK, FAILED, FAIL_MISMATCH, DISABLED
        }

        public ValidationStatus Status { get; }
        public string Name { get; }
        public DateTime Date { get; }
        public Manager Manager { get; }
    }
}
