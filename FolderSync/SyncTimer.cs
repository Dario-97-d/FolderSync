using System.Timers;

namespace FolderSync
{
    internal class SyncTimer : System.Timers.Timer
    {
        public int IntervalSeconds => (int)Interval / 1000;

        public SyncTimer()
        {
            Enabled = false;
        }

        public void Configure(int intervalSeconds, ElapsedEventHandler handler)
        {
            AutoReset = true;
            Elapsed += handler;
            Enabled = true;
            Interval = intervalSeconds * 1000;
        }
    }
}
