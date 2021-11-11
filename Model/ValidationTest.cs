namespace Model
{
    public class ValidationTest
    {
        #region Constructors
        public ValidationTest(DateTime date, string name, ValidationStatus status, string managerName)
        {
            Date = date;
            Name = name;
            Status = status;
            ManagerName = managerName;
        }
        #endregion Constructors

        #region Enums
        public enum ValidationStatus
        {
            OK, FAILED, FAIL_MISMATCH, DISABLED
        }
        #endregion Enums

        #region Properties
        public ValidationStatus Status { get; } //From [AFSTEMRESULTAT] in [dbo].[AFSTEMNING]
        public string Name { get; } //From [DESCRIPTION] in [dbo].[AFSTEMNING]
        public DateTime Date { get; } //From [AFSTEMTDATO] in [dbo].[AFSTEMNING]
        public string ManagerName { get; } //From [MANAGER] in [dbo].[AFSTEMNING]
        #endregion Properties

        public override string ToString()
        {
            return $"TEST NAME: {Name}\nTEST STATUS: {Status}\n";
        }
    }
}
