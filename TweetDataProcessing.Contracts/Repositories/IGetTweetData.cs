using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using TweetDataProcessing.DTO;

namespace TweetDataProcessing.Contracts.Repositories
{
    public interface IGetTweetData : IDisposable
    {
        Task<IEnumerable<TweetDTO>> GetTweets(HttpClient client, DateTime startDate, DateTime endDate);
    }
}
