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

        public int ExecutionId { get; }
        public int BytesSend { get; } // bytes
        public int BytesSendDelta { get; } // bytes
        public int BytesSendSpeed { get; } // Bps
        public int BytesReceived { get; } // bytes
        public int BytesReceivedDelta { get; } // bytes
        public int BytesReceivedSpeed { get; } // Bps
        public DateTime Date { get; }
    }
}
