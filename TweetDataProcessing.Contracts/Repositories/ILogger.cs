using System;

namespace TweetDataProcessing.Repositories.Contracts
{
    public interface ILogger : IDisposable
    {
        void WriteLog(string text);
    }
}
