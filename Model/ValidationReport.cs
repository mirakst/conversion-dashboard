namespace Model
{
    public class ValidationReport
    {
        #region Constructors
        public ValidationReport()
        {

        }
        #endregion Constructors

        #region Properties
        public DateTime LastModified { get; private set; }

        private List<ValidationTest> _validationTests = new();
        public List<ValidationTest> ValidationTests
        {
            get => _validationTests;
            set
            {
                _validationTests.AddRange(value);
                OnModified(); // Raises event when property is set. Possible parameters include = List content, list count etc.
                LastModified = DateTime.Now;
            }
        }
        #endregion Properties

        #region Event handling
        public delegate void ValidationReportModifiedHandler(ValidationReport source, DateTime timestamp);

        public event ValidationReportModifiedHandler Modified;

        protected virtual void OnModified()
        {
            if (Modified != null)
            {
                Modified(this, DateTime.Now);
            }
        }
        #endregion Event handling
    }
}