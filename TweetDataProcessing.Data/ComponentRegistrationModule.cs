using Autofac;

using TweetDataProcessing.Common.Autofac;
using TweetDataProcessing.Contracts.Repositories;
using TweetDataProcessing.Data.Repositories;
using TweetDataProcessing.Repositories.Contracts;
using TweetDataProcessing.Repositories.Data;

namespace TweetDataProcessing.Data
{
    public class ComponentRegistrationModule : AutofacModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register((x, y) => new Logger())
                .As<ILogger>();

            builder.Register((x, y) => new GetTweetData())
                .As<IGetTweetData>();

            builder.Register((x, y) => new GetTimePerformance())
                .As<IGetTimePerformance>();
        }
    }
}
