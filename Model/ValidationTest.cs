using System;

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

        public ValidationStatus Status { get; } //From [AFSTEMRESULTAT] in [dbo].[AFSTEMNING]
        public string Name { get; } //From [DESCRIPTION] in [dbo].[AFSTEMNING]
        public DateTime Date { get; } //From [AFSTEMTDATO] in [dbo].[AFSTEMNING]
        public Manager Manager { get; } //From [MANAGER] in [dbo].[AFSTEMNING]
    }
}
