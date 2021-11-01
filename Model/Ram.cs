namespace Model
{
    public class Ram
    {
        public Ram(int total)
        {
            Total = total;
        }

        public int Total { get; set; }
        public RamUsage Readings { get; set; }
    }
}
