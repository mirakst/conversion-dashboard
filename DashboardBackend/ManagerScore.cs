using Model;
using static Model.ValidationTest;

namespace DashboardBackend
{
    public class ManagerScore
    {
        public List<Manager> ManagerList { get; }
        public double AvrageRowsPerSecond { get; private set;  }

        public ManagerScore(List<Manager> managerList)
        {
            ManagerList = managerList;
            GetAvrageOfRowPerSecond(ManagerList);
        }

        /// <summary>
        /// Calculates a score for a manager, returns that score.
        /// </summary>
        /// <param name="validationReport"></param>
        /// <param name="manager"></param>
        /// <returns></returns>
        public double GetManagerScore(ValidationReport validationReport, Manager manager)
        {
            double score = GetRowPerSecond(manager) / AvrageRowsPerSecond;
                   score += GetValidationsScore(validationReport, manager) * 2;

            return score;
        }

        #region RowsPerSecond
        /// <summary>
        /// Calculates avrage rows per second for a <see cref="Manager"/>.
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        private static double GetRowPerSecond(Manager manager)
        {
            if (manager.RowsRead.HasValue && manager.RowsWritten.HasValue && manager.Runtime.HasValue)
            {
                return (manager.RowsRead.Value + manager.RowsWritten.Value) / manager.Runtime.Value.TotalSeconds;
            }
            else
            {
                return 0.0;
            }
        }

        /// <summary>
        /// Calculates the avrage of rows per second, for all <see cref="Manager"/>.
        /// </summary>
        /// <param name="managerList"></param>
        private void GetAvrageOfRowPerSecond(List<Manager> managerList)
        {
            AvrageRowsPerSecond = managerList.Select(m => GetRowPerSecond(m)).Sum() / managerList.Count;
        }
        #endregion
        #region ValidationsFailedPerOK
        /// <summary>
        /// Calculates a score for failed <see cref="ValidationReport.ValidationTests"/> out of total <see cref="ValidationReport.ValidationTests"/>.
        /// </summary>
        /// <param name="validationReport"></param>
        /// <param name="manager"></param>
        /// <returns></returns>
        private static double GetValidationsScore(ValidationReport validationReport, Manager manager)
        {
            int bad = validationReport.ValidationTests.Count(val => val.ManagerName == manager.Name
                                                                     && (val.Status == ValidationStatus.Failed
                                                                     || val.Status == ValidationStatus.FailMismatch));

            int total = validationReport.ValidationTests.Where(val => val.ManagerName == manager.Name)
                                                        .Where(val => val.Status != ValidationStatus.Disabled)
                                                        .Count();
            if (bad > 0 && total > 0)
            {
                return bad / total;
            }
            return 0;
        }
        #endregion
    }
}
