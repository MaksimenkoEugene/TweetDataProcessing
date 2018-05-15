namespace TweetDataProcessing.Common.Common
{
    public interface IDependencyResolver
    {
        T Resolve<T>(params DependencyParameter[] parameters);

        T ResolveKeyed<T>(object key);

        T ResolveOptionalKeyed<T>(object key) where T : class;
    }
}
