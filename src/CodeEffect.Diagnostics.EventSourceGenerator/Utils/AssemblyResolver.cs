using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FG.Diagnostics.AutoLogger.Generator.Utils
{
    public class AssemblyResolver
    {
        private readonly Assembly _dynamicAssembly;
        private readonly IEnumerable<string> _references;
        private readonly IList<Assembly> _loadedAssemblies;

        private readonly bool _onlyReferentialLoad;

        private readonly Assembly[] _appDomainLoadedAssemblies;

        public AssemblyResolver(IEnumerable<string> references)
        {
            _references = references;
            _onlyReferentialLoad = false;
            _loadedAssemblies = new List<Assembly>();

            _appDomainLoadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToArray();
        }

        public AssemblyResolver(Assembly dynamicAssembly, IEnumerable<string> referenceAssembliesPath)
            : this(referenceAssembliesPath)
        {
            _dynamicAssembly = dynamicAssembly;
            _onlyReferentialLoad = true;
        }

        private bool ShouldLoad(Assembly requestingAssembly, string assemblyName)
        {
            if (!_onlyReferentialLoad) return true;

            return (requestingAssembly == _dynamicAssembly) || _loadedAssemblies.Contains(requestingAssembly);
        }

        public Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (!ShouldLoad(args.RequestingAssembly, args.Name)) return null;

            var appDomainLoadedAssembly = _appDomainLoadedAssemblies.FirstOrDefault(a => a.FullName == args.Name);
            if (appDomainLoadedAssembly != null)
            {
                return appDomainLoadedAssembly;
            }

            var alreadyLoadedAssembly = _loadedAssemblies.FirstOrDefault(a => a.FullName == args.Name);
            if (alreadyLoadedAssembly != null)
            {
                return alreadyLoadedAssembly;
            }

            var assemblyName = args.Name.Split(',')[0];
            var reference = _references.FirstOrDefault(a => System.IO.Path.GetFileNameWithoutExtension(a) == assemblyName);
            if (reference != null)
            {
                var loadedAssembly = Assembly.LoadFile(reference);
                if (loadedAssembly != null)
                {
                    _loadedAssemblies.Add(loadedAssembly);
                }
                return loadedAssembly;
            }
            return null;
        }

    }
}