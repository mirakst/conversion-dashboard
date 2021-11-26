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
        public DateTime LastModified { get; set; } = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue; //Date of last modification.

        private readonly List<ValidationTest> _validationTests = new();
        public List<ValidationTest> ValidationTests
        {
            get => _validationTests;
            set => _validationTests.AddRange(value);
        }
        #endregion Properties
    }
}