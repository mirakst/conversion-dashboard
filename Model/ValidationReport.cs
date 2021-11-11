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
                LastModified = DateTime.Now;
            }
        }
        #endregion Properties
    }
}