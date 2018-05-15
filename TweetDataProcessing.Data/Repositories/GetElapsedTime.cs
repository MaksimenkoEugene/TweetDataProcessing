using System;
using System.Diagnostics;

using TweetDataProcessing.Contracts.Repositories;

namespace TweetDataProcessing.Data.Repositories
{
    public class GetTimePerformance : IGetTimePerformance
    {
        Stopwatch stopWatch = new Stopwatch();

        public Stopwatch SetStartTime()
        {
            this.stopWatch.Start();
            return stopWatch;
        }

        public Stopwatch SetEndTime()
        {
            this.stopWatch.Stop();
            return stopWatch;
        }

        public string GetElapsedTime()
        {
            TimeSpan ts = this.stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);

            return elapsedTime;
        }

        public void Dispose()
        {
        }
    }
}
