using System;

namespace Model
{
    public class NetworkUsage
    {
        public NetworkUsage(int executionId, int send, int sendDelta, int sendSpeed, int rcv, int rcvDelta, int rcvSpeed, DateTime date)
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
        public long BytesSendSpeed { get; } // Bps //From [REPORT_NUMERIC_VALUE] in [dbo].[HEALTH_REPORT] where [REPORT_KEY] = 'Interface 0: Bytes Send (Speed)'.
        public long BytesReceived { get; } // bytes //From [REPORT_NUMERIC_VALUE] in [dbo].[HEALTH_REPORT] where [REPORT_KEY] = 'Interface 0: Bytes Received'.
        public long BytesReceivedDelta { get; } // bytes //From [REPORT_NUMERIC_VALUE] in [dbo].[HEALTH_REPORT] where [REPORT_KEY] = 'Interface 0: Bytes Received (Delta)'.
        public long BytesReceivedSpeed { get; } // Bps //From [REPORT_NUMERIC_VALUE] in [dbo].[HEALTH_REPORT] where [REPORT_KEY] = 'Interface 0: Bytes Received (Speed)'.
        public DateTime Date { get; } //From [LOG_TIME] in [dbo].[HEALTH_REPORT].
        //The properties above can be gathered from the list of entries in [dbo].[HEALTH_REPORT], where [REPORT_TYPE] = 'NETWORK'.
    }
}
