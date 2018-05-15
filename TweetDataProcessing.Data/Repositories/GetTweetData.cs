using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using TweetDataProcessing.Contracts.Repositories;
using TweetDataProcessing.DTO;

namespace TweetDataProcessing.Data.Repositories
{
    public class GetTweetData : IGetTweetData
    {
        public async Task<IEnumerable<TweetDTO>> GetTweets(HttpClient client, DateTime startDate, DateTime endDate)
        {
            if (client != null)
            {
                HttpResponseMessage response = client
                    .GetAsync(
                        String.Format("/api/v1/Tweets/?startDate={0}&endDate={1}",
                            startDate.ToString("o"),
                            endDate.ToString("o"))).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsAsync<List<TweetDTO>>().Result;

                    if (result.Count < 100)
                    {
                        return result;
                    }

                    var avgDate = new DateTime((long)((startDate.Ticks + endDate.Ticks) / (double)2));
                    var results = await Task.WhenAll(this.GetTweets(client, startDate, avgDate), this.GetTweets(client, avgDate, endDate));

                    return results.SelectMany(x => x);
                }
                else
                {
                    return Enumerable.Empty<TweetDTO>();
                }
            }
            else
            {
                return Enumerable.Empty<TweetDTO>();
            }
        }

        public void Dispose()
        {
        }
    }
}
