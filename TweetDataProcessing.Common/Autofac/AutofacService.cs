using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using global::Autofac;
using TweetDataProcessing.Common.Common;

namespace TweetDataProcessing.Common.Autofac
{
    public class AutofacService : IDependencyResolver
    {
        private readonly Dictionary<string, Assembly> _assembliesWithModules;
        private volatile bool _initialized;

        static AutofacService()
        {
            new AutofacService();
        }

        private AutofacService()
        {
            Instance = this;
            this._assembliesWithModules = new Dictionary<string, Assembly>();
        }

        public static AutofacService Instance { get; private set; }

        public IContainer Container { get; private set; }

        public bool Initialized => this._initialized;

        /// <summary>
        /// At the registration step these assemblies will be processed after those which are found automatically.
        /// Be careful! AddAssemblyWithModules - register of ALL components from the specified assembly!
        /// If you don not need all then registrate only those you need in newly created AutofacModule and
        /// add it in the executable assembly.
        /// </summary>
        /// <param name="asm">The assembly to discover registration modules.</param>
        public void AddAssemblyWithModules(Assembly asm)
        {
            lock (this._assembliesWithModules)
            {
                if (!this._assembliesWithModules.ContainsKey(asm.FullName))
                {
                    lock (this._assembliesWithModules)
                    {
                        if (!this._assembliesWithModules.ContainsKey(asm.FullName))
                        {
                            this._assembliesWithModules.Add(asm.FullName, asm);
                        }
                    }
                }
            }
        }

        public void Initialize()
        {
            if (!this._initialized)
            {
                this._initialized = true;

                var builder = new ContainerBuilder();

                this.RegisterInternalModules(builder);

                this.Container = builder.Build();
            }
        }

        public T Resolve<T>(params DependencyParameter[] parameters)
        {
            return (T)Instance?.Container?.Resolve(typeof(T), parameters.Select(x => new NamedParameter(x.Name, x.Value)));
        }

        public T ResolveKeyed<T>(object key)
        {
            return (T)Instance?.Container?.ResolveKeyed(key, typeof(T));
        }

        public T ResolveOptionalKeyed<T>(object key) where T : class
        {
            return Instance?.Container?.ResolveOptionalKeyed<T>(key);
        }

        private void RegisterInternalModules(ContainerBuilder builder)
        {
            var domainAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            var dotPos = assemblyName.IndexOf(".", StringComparison.Ordinal);
            var rootNamespace = dotPos == -1 ? assemblyName : assemblyName.Substring(0, dotPos);
            var internalAssemblies = new List<Assembly>();

            foreach (var assembly in domainAssemblies)
            {
                if (assembly.GetName().Name.StartsWith(rootNamespace))
                {
                    if (!this._assembliesWithModules.ContainsKey(assembly.FullName))
                    {
                        internalAssemblies.Add(assembly);
                    }
                }
            }

            Array.ForEach(this._assembliesWithModules.Values.ToArray(), internalAssemblies.Add);

            builder.RegisterAssemblyModules<AutofacModule>(internalAssemblies.ToArray());
        }
    }
}
