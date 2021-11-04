namespace Model
{
    public class Ram
    {
        public Ram(long total)
        {
            Total = total;
        }

        public long Total { get; set; } //bytes //From [REPORT_NUMERIC_VALUE] in [dbo].[HEALTH_REPORT], where [REPORT_KEY] = 'TOTAL'.
        //The property above can be gathered from the list of entries in [dbo].[HEALTH_REPORT], where [REPORT_TYPE] = 'CPU_INIT'.
        public RamUsage Readings { get; set; }

        public override string ToString()
        {
            return $"TOTAL MEMORY: {Total} bytes";
        }
    }
}
