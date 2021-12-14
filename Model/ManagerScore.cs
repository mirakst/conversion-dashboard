using System.Diagnostics;

using static System.Formats.Asn1.AsnWriter;
using static Model.ValidationTest;

namespace Model
{
    public class ManagerScore
    {
        public event EventHandler<Manager> MaxManagerScoreUpdated;

        public static List<double> MaxPerformanceScore { get; private set; } = new();

        /// <summary>
        /// Calculates the managers performance score
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public double GetPerformanceScore(Manager manager)
        {
            MaxManagerScoreUpdated += manager.OnManagerScoreUpdated;
            double score = CalculatePerformanceScore(manager);

            if (score > MaxPerformanceScore[0/* manager.ExecutionId*/])
            {
                MaxPerformanceScore[0/* manager.ExecutionId*/] = score;
                MaxManagerScoreUpdated?.Invoke(this, manager);
            }

            return PerformanceScoreToPercent(score, 0 /* manager.ExecutionId */);
        }

        /// <summary>
        /// Calculates the managers validation score
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public double GetValidationScore(Manager manager)
        {
            double OkCount = manager.Validations.Count(v => v.Status is ValidationStatus.Ok);
            double TotalCount = manager.Validations.Count(v => v.Status is not ValidationStatus.Disabled);
            return TotalCount > 0 ? OkCount / TotalCount * 100.0d : 100.0d;
        }

        private double CalculatePerformanceScore(Manager manager)
        {
            if (manager.Runtime.HasValue)
                return (double)(manager.RowsRead + manager.RowsWritten) / manager.Runtime.Value.Seconds;
            else
                return 0;
        }

        private double PerformanceScoreToPercent(double performanceScore, int execution)
        {
            return (performanceScore / MaxPerformanceScore[execution]) * 100;
        }
    }
}
