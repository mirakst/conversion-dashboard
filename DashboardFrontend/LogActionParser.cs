using Model;
using System.Diagnostics;

namespace DashboardFrontend
{
    public class LogActionParser
    {
        public LogActionParser(Conversion conversion)
        {
            Conversion = conversion;
        }

        public Conversion Conversion { get; }

        public void ParseMessage(LogMessage message)
        {

            if (message.Content.StartsWith("Starting manager"))
            {
                Manager? manager = Conversion.ActiveExecution.Managers.Find(m => m.ContextId == message.ContextId);
                if (manager is not null)
                {
                    //Trace.WriteLine(manager);
                }
            }
        }
    }
}