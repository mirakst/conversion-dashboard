using System;

namespace ConversionEngineSimulator
{
    public class Vote : ITimestampedDatabaseEntry, IComparable<Vote>
    {
        public string ID { get; set; }
        public DateTime AFSTEMTDATO { get; set; }
        public DateTime CREATED { get => AFSTEMTDATO; set => AFSTEMTDATO = value; }
        public string DESCRIPTION { get; set; }
        public string MANAGER { get; set; }
        public string CONTEXT { get; set; }
        public int? SRCANTAL { get; set; }
        public int? DSTANTAL { get; set; }
        public int? CUSTOMANTAL { get; set; }
        public string AFSTEMRESULTAT { get; set; }
        public bool RUN_JOB { get;set; }
        public int? TOOLKIT_ID { get; set; }
        public int? SRC_SQL_COST { get; set; }
        public int? DST_SQL_COST { get; set; }
        public int? CUSTOM_SQL_COST { get; set; }
        public string SRC_SQL { get; set; }
        public string DST_SQL { get; set; }
        public string CUSTOM_SQL {  get; set; }
        public int? SRC_SQL_TIME { get; set; }
        public int? DST_SQL_TIME { get; set; }
        public int? CUSTOM_SQL_TIME { get; set; }
        public DateTime START_TIME { get; set; }
        public DateTime? END_TIME { get; set; }
        public byte[] AFSTEMNINGSDATA { get; set; }

        public override string ToString()
        {
            return $"{"[Afstemning]",-15} {CREATED}: {MANAGER} {AFSTEMRESULTAT}";
        }
        public int CompareTo(Vote vote)
        {
            return DateTime.Compare(CREATED, vote.CREATED);
        }
    }
}
