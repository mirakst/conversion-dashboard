namespace Model
{
    public class ValidationTest
    {
        #region Constructors
        public ValidationTest(DateTime date, string name, ValidationStatus status, string managerName, int? srcCount, int? dstCount, int? toolkitId, string srcSql, string dstSql)
        {
            Date = date;
            Name = name;
            Status = status;
            ManagerName = managerName;
            SrcCount = srcCount;
            DstCount = dstCount;
            ToolkitId = toolkitId;
            SrcSql = srcSql;
            DstSql = dstSql;
        }
        #endregion Constructors

        #region Enums
        public enum ValidationStatus
        {
            Failed, FailMismatch, Disabled, Ok
        }
        #endregion Enums

        #region Properties
        public ValidationStatus Status { get; } //From [AFSTEMRESULTAT] in [dbo].[AFSTEMNING]
        public string Name { get; } //From [DESCRIPTION] in [dbo].[AFSTEMNING]
        public DateTime Date { get; } //From [AFSTEMTDATO] in [dbo].[AFSTEMNING]
        public string ManagerName { get; } //From [MANAGER] in [dbo].[AFSTEMNING]
        public int? SrcCount { get; }
        public int? DstCount { get; }
        public int? ToolkitId { get; }
        public string SrcSql { get; }
        public string DstSql { get; }
        #endregion Properties
        
        public override string ToString()
        {
            return $"({Date}) {Name}: {Status}\n[src={SrcCount},dst={DstCount},toolkit={ToolkitId}]\nSrc sql: {SrcSql}\nDst sql: {DstSql}";
        }
    }
}
