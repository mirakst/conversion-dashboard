using System.Diagnostics;

using static System.Formats.Asn1.AsnWriter;
using static Model.ValidationTest;

namespace Model
{
    public class ManagerScore
    {
        public double MaxPerformanceScore { get; private set; }

        /// <summary>
        /// Calculates the managers performance score
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public double GetPerformanceScore(Manager manager)
        {
            double score = CalculatePerformanceScore(manager);

            if (score > MaxPerformanceScore)
            {
                MaxPerformanceScore = score;
            }

            return PerformanceScoreToPercent(score);
        }

        /// <summary>
        /// Calculates the managers validation score
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public double GetValidationScore(Manager manager)
        {
            double OkCount = manager.Validations.Count(v => v.Status is ValidationStatus.Ok);
            double TotalCount = manager.Validations.Count;
            return TotalCount > 0 ? OkCount / TotalCount * 100.0d : 100.0d;
        }

        private double CalculatePerformanceScore(Manager manager)
        {
            if (manager.Runtime.HasValue && manager.RowsWritten.HasValue)
                return (double)(manager.RowsWritten) / manager.Runtime.Value.TotalSeconds;
            else
                return 0;
        }

        private double PerformanceScoreToPercent(double performanceScore)
        {
            return (performanceScore / MaxPerformanceScore) * 100;
        }
    }
}
