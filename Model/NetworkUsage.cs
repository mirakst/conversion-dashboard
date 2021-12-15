namespace Model
{
    public class NetworkUsage
    {
        public NetworkUsage()
        {

        }

        public NetworkUsage(int executionId, long send, long sendDelta, long sendSpeed, long rcv, long rcvDelta, long rcvSpeed, DateTime date)
        {
            ExecutionId = executionId;
            BytesSend = send;
            BytesSendDelta = sendDelta;
            BytesSendSpeed = sendSpeed;
            BytesReceived = rcv;
            BytesReceivedDelta = rcvDelta;
            BytesReceivedSpeed = rcvSpeed;
            Date = date;
        }

        public int ExecutionId { get; set; } //From [EXECUTION_ID] in [dbo].[HEALTH_REPORT].
        public long? BytesSend { get; set; } // bytes //From [REPORT_NUMERIC_VALUE] in [dbo].[HEALTH_REPORT] where [REPORT_KEY] = 'Interface 0: Bytes Send'.
        public long? BytesSendDelta { get; set; } // bytes //From [REPORT_NUMERIC_VALUE] in [dbo].[HEALTH_REPORT] where [REPORT_KEY] = 'Interface 0: Bytes Send (Delta)'.
        public long? BytesSendSpeed { get; set; } // bps //From [REPORT_NUMERIC_VALUE] in [dbo].[HEALTH_REPORT] where [REPORT_KEY] = 'Interface 0: Bytes Send (Speed)'.
        public long? BytesReceived { get; set; } // bytes //From [REPORT_NUMERIC_VALUE] in [dbo].[HEALTH_REPORT] where [REPORT_KEY] = 'Interface 0: Bytes Received'.
        public long? BytesReceivedDelta { get; set; } // bytes //From [REPORT_NUMERIC_VALUE] in [dbo].[HEALTH_REPORT] where [REPORT_KEY] = 'Interface 0: Bytes Received (Delta)'.
        public long? BytesReceivedSpeed { get; set; } // bps //From [REPORT_NUMERIC_VALUE] in [dbo].[HEALTH_REPORT] where [REPORT_KEY] = 'Interface 0: Bytes Received (Speed)'.
        public DateTime Date { get; set; } //From [LOG_TIME] in [dbo].[HEALTH_REPORT].
                                           //The properties above can be gathered from the list of entries in [dbo].[HEALTH_REPORT], where [REPORT_TYPE] = 'NETWORK'.

        public override string ToString()
        {
            return $"BYTES SENT: {BytesSend} bytes\n" +
                   $"BYTES SENT (DELTA): {BytesSendDelta} bytes\n" +
                   $"BYTES SENT (SPEED): {BytesSendSpeed} bps\n" +
                   $"BYTES RECEIVED: {BytesReceived} bytes\n" +
                   $"BYTES RECEIVED (DELTA): {BytesReceivedDelta} bytes\n" +
                   $"BYTES RECEIVED (SPEED) {BytesReceivedSpeed} bps";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ExecutionId, Date, BytesSend, BytesReceived);
        }

        public override bool Equals(object obj)
        {
            if (obj is not NetworkUsage other)
            {
                return false;
            }

            return GetHashCode() == other.GetHashCode();
        }
    }
}
