namespace Model
{
    public class NetworkUsage
    {
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

        public int ExecutionId { get; } //From [EXECUTION_ID] in [dbo].[HEALTH_REPORT].
        public long BytesSend { get; } // bytes //From [REPORT_NUMERIC_VALUE] in [dbo].[HEALTH_REPORT] where [REPORT_KEY] = 'Interface 0: Bytes Send'.
        public long BytesSendDelta { get; } // bytes //From [REPORT_NUMERIC_VALUE] in [dbo].[HEALTH_REPORT] where [REPORT_KEY] = 'Interface 0: Bytes Send (Delta)'.
        public long BytesSendSpeed { get; } // bps //From [REPORT_NUMERIC_VALUE] in [dbo].[HEALTH_REPORT] where [REPORT_KEY] = 'Interface 0: Bytes Send (Speed)'.
        public long BytesReceived { get; } // bytes //From [REPORT_NUMERIC_VALUE] in [dbo].[HEALTH_REPORT] where [REPORT_KEY] = 'Interface 0: Bytes Received'.
        public long BytesReceivedDelta { get; } // bytes //From [REPORT_NUMERIC_VALUE] in [dbo].[HEALTH_REPORT] where [REPORT_KEY] = 'Interface 0: Bytes Received (Delta)'.
        public long BytesReceivedSpeed { get; } // bps //From [REPORT_NUMERIC_VALUE] in [dbo].[HEALTH_REPORT] where [REPORT_KEY] = 'Interface 0: Bytes Received (Speed)'.
        public DateTime Date { get; } //From [LOG_TIME] in [dbo].[HEALTH_REPORT].
        //The properties above can be gathered from the list of entries in [dbo].[HEALTH_REPORT], where [REPORT_TYPE] = 'NETWORK'.

        public override string ToString()
        {
            return $"BYTES SENT: {BytesSend} bytes\nBYTES SENT (DELTA): {BytesSendDelta} bytes\nBYTES SENT (SPEED): {BytesSendSpeed} bps\n" +
                   $"BYTES RECEIVED: {BytesReceived} bytes\nBYTES RECEIVED (DELTA): {BytesReceivedDelta} bytes\nBYTES RECEIVED (SPEED) {BytesReceivedSpeed} bps";
        }
    }
}
