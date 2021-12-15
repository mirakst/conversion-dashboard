using DashboardBackend.Database.Models;
using Model;
using static Model.ValidationTest;

namespace DashboardBackend.Parsers
{
    public class ReconciliationParser : IDataParser<AfstemningEntry, List<ValidationTest>>
    {
        public List<ValidationTest> Parse(List<AfstemningEntry> data)
        {
            return (from item in data
                    let date = item.Afstemtdato
                    let name = item.Description
                    let managerName = item.Manager
                    let status = GetValidationStatus(item)
                    let srcCount = item.Srcantal
                    let dstCount = item.Dstantal
                    let toolkitId = item.ToolkitId
                    let srcSql = item.SrcSql
                    let dstSql = item.DstSql
                    select new ValidationTest(date, name, status, managerName, srcCount, dstCount, toolkitId, srcSql, dstSql))
                    .ToList();
        }

        /// <summary>
        /// Returns the status of the validation test parameter 'entry'.
        /// </summary>
        /// <param name="entry">A single entry from the [AFSTEMNING] table in the state database.</param>
        /// <returns>A validation status based on the enum in the validation class.</returns>
        /// <exception cref="ArgumentException">Thrown if the parameter passed is not a legal validation status.</exception>
        private ValidationStatus GetValidationStatus(AfstemningEntry entry)
        {
            return entry.Afstemresultat switch
            {
                "OK" => ValidationStatus.Ok,
                "DISABLED" => ValidationStatus.Disabled,
                "FAILED" => ValidationStatus.Failed,
                "FAIL MISMATCH" => ValidationStatus.FailMismatch,
                _ => throw new ArgumentException(nameof(entry) + " is not a known validation test result.")
            };
        }
    }
}
