using Model;
using static Model.ValidationTest;

namespace DashboardBackend
{
    public class ManagerScore
    {
        public ValidationReport ValidationReport { get; }
        public List<Manager> ManagerList { get; }
        public double AvrageRowsPerSecond { get; private set;  }

        public ManagerScore(ValidationReport validationReport, List<Manager> managerList)
        {
            ValidationReport = validationReport;
            ManagerList = managerList;

            GetAvrageOfRowPerSecond(ManagerList);

            foreach (var manager in ManagerList)
            {
                manager.Score = GetManagerScore(manager);
            }
        }

        /// <summary>
        /// Calculates a score for a manager, returns that score.
        /// </summary>
        /// <param name="validationReport"></param>
        /// <param name="manager"></param>
        /// <returns></returns>
        public double GetManagerScore(Manager manager)
        {
            double score = GetRowPerSecond(manager) / AvrageRowsPerSecond;
                   score += GetValidationsScore(manager) * 2;

            return score;
        }

        /// <summary>
        /// Calculates avrage rows per second for a <see cref="Manager"/>.
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        private static double GetRowPerSecond(Manager manager)
        {
            return (manager.RowsRead + manager.RowsWritten) / manager.Runtime.TotalSeconds;
        }

        /// <summary>
        /// Calculates the avrage of rows per second, for all <see cref="Manager"/>.
        /// </summary>
        /// <param name="managerList"></param>
        private void GetAvrageOfRowPerSecond(List<Manager> managerList)
        {
            AvrageRowsPerSecond = managerList.Select(m => GetRowPerSecond(m)).Sum() / managerList.Count;
        }

        /// <summary>
        /// Calculates a score for failed <see cref="ValidationReport.ValidationTests"/> out of total <see cref="ValidationReport.ValidationTests"/>.
        /// </summary>
        /// <param name="validationReport"></param>
        /// <param name="manager"></param>
        /// <returns></returns>
        private double GetValidationsScore(Manager manager)
        {
            int bad = ValidationReport.ValidationTests.Count(val => val.ManagerName == manager.Name
                                                                     && (val.Status == ValidationStatus.Failed
                                                                     || val.Status == ValidationStatus.FailMismatch));

            int total = ValidationReport.ValidationTests.Where(val => val.ManagerName == manager.Name)
                                                        .Where(val => val.Status != ValidationStatus.Disabled)
                                                        .Count();
            if (bad > 0 && total > 0)
            {
                return bad / total;
            }
            return 0;
        }
    }
}
