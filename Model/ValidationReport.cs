namespace Model
{
    public class ValidationReport
    {
        public ValidationReport()
        {

        }

        public DateTime LastModified { get; set; }
        public List<ValidationReport> ValidationTests { get; set; } = new();
    }
}
