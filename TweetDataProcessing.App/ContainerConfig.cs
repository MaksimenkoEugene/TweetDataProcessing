using Autofac;
using TweetDataProcessing.Common.Autofac;

namespace TweetDataProcessing.App
{
    public static class ContainerConfig
    {
        public static void Configure()
        {
            AutofacService.Instance.AddAssemblyWithModules(typeof(ComponentRegistrationModule).Assembly);
            AutofacService.Instance.AddAssemblyWithModules(typeof(TweetDataProcessing.Data.ComponentRegistrationModule).Assembly);
            AutofacService.Instance.Initialize();
        }
    }
}
