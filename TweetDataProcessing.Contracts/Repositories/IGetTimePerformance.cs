using System;
using System.Diagnostics;

namespace TweetDataProcessing.Contracts.Repositories
{
    public interface IGetTimePerformance : IDisposable
    {
        Stopwatch SetStartTime();

        Stopwatch SetEndTime();

        string GetElapsedTime();
    }
}
