using System.ComponentModel;
using System.Globalization;

namespace Model
{
    public class Reconciliation : INotifyPropertyChanged
    {
        public Reconciliation(DateTime date, string name, ReconciliationStatus status, string managerName, int? srcCount, int? dstCount, int? toolkitId, string srcSql, string dstSql)
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

        public event PropertyChangedEventHandler PropertyChanged;

        public enum ReconciliationStatus
        {
            Failed, FailMismatch, Disabled, Ok
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected; 
            set
            {
                _isSelected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
            }
        }
        public ReconciliationStatus Status { get; } //From [AFSTEMRESULTAT] in [dbo].[AFSTEMNING]
        public string Name { get; } //From [DESCRIPTION] in [dbo].[AFSTEMNING]
        public DateTime Date { get; } //From [AFSTEMTDATO] in [dbo].[AFSTEMNING]
        public string ManagerName { get; } //From [MANAGER] in [dbo].[AFSTEMNING]
        public int? SrcCount { get; }
        public int? DstCount { get; }
        public int? ToolkitId { get; }
        public string SrcSql { get; }
        public string DstSql { get; }
                
        public override string ToString()
        {
            return $"({Date.ToString(new CultureInfo("da-DK"))}) {Name}: {Status}\n[src={SrcCount},dst={DstCount},toolkit={ToolkitId}]\nSrc sql: {SrcSql}\nDst sql: {DstSql}";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Date, Name, ManagerName, Status, SrcCount, DstCount);
        }

        public override bool Equals(object obj)
        {
            if (obj is not Reconciliation other)
            {
                return false;
            }

            return GetHashCode() == other.GetHashCode();
        }
    }
}
