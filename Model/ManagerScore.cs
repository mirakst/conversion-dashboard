using static Model.ValidationTest;

namespace Model
{
    public static class ManagerScore
    {
        public static List<double> MaxPerformanceScore { get; private set; }

        /// <summary>
        /// Calculates the managers performance score
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public static double GetPerformanceScore(Manager manager)
        {
            double score = RuntimeHasValue(manager);
            IsNewMaxScore(score, 0 /* manager.ExecutionId */);
            return PerformanceScoreToPercent(score, 0/* manager.ExecutionId */);
        }

        /// <summary>
        /// Calculates the managers validation score
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public static double GetValidationScore(Manager manager)
        {
            double OkCount = manager.Validations.Count(v => v.Status is ValidationStatus.Ok);
            double TotalCount = manager.Validations.Count(v => v.Status is not ValidationStatus.Disabled);
            return TotalCount > 0 ? OkCount / TotalCount * 100.0d : 100.0d;
        }

        private static double RuntimeHasValue(Manager manager)
        {
            if (manager.Runtime.HasValue)
                return (double)(manager.RowsRead + manager.RowsWritten) / manager.Runtime.Value.Seconds;
            else
                return 0;
        }
        
        private static void IsNewMaxScore(double score, int execution)
        {
            if (score > MaxPerformanceScore[execution])
                MaxPerformanceScore[execution] = score;
        }

        private static double PerformanceScoreToPercent(double performanceScore, int execution)
        {
            return (performanceScore / MaxPerformanceScore[execution]) * 100;
        }
    }
}
