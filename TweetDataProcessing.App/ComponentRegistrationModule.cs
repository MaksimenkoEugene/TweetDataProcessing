using Autofac;
using TweetDataProcessing.Common.Autofac;
using TweetDataProcessing.Common.Common;

namespace TweetDataProcessing.App
{
    public class ComponentRegistrationModule : AutofacModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(AutofacService.Instance)
                .As<IDependencyResolver>()
                .SingleInstance();
        }
    }
}
