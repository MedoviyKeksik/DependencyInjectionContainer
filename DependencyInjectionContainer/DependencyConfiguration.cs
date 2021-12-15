using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainer
{
    public class DependencyConfiguration : IDependencyConfiguration
    {
        private Dictionary<Type, List<Type>> _transientDependencies = new Dictionary<Type, List<Type>>();
        private Dictionary<Type, List<Type>> _singletonDependencies = new Dictionary<Type, List<Type>>();
        private Dictionary<Type, Dictionary<Enum, Type>> _transientNamedDependencies = new Dictionary<Type, Dictionary<Enum, Type>>();
        private Dictionary<Type, Dictionary<Enum, Type>> _singletonNamedDependencies = new Dictionary<Type, Dictionary<Enum, Type>>();

        public DependencyConfiguration()
        { }

        public Dictionary<Type, List<Type>> TransientDependencies => _transientDependencies;

        public Dictionary<Type, List<Type>> SingletonDependencies => _singletonDependencies;

        public Dictionary<Type, Dictionary<Enum, Type>> TransientNamedDependencies => _transientNamedDependencies;

        public Dictionary<Type, Dictionary<Enum, Type>> SingletonNamedDependencies => _singletonNamedDependencies;

        public void AddSingleton<TDependency, TImplementation>() where TImplementation : TDependency
        {
            AddSingleton(typeof(TDependency), typeof(TImplementation));
        }

        public void AddSingleton<TDependency, TImplementation>(Enum name) where TImplementation : TDependency
        {
            AddSingleton(typeof(TDependency), typeof(TImplementation));
            if (!_singletonNamedDependencies.ContainsKey(typeof(TDependency)))
            {
                _singletonNamedDependencies.Add(typeof(TDependency), new Dictionary<Enum, Type>());
            }
            _singletonNamedDependencies[typeof(TDependency)].Add(name, typeof(TImplementation));
        }

        public void AddSingleton(Type dependecy, Type implementation)
        {
            if (!_singletonDependencies.ContainsKey(dependecy))
            {
                _singletonDependencies.Add(dependecy, new List<Type>());
            }
            _singletonDependencies[dependecy].Add(implementation);
        }

        public void AddTransient<TDependency, TImplementation>() where TImplementation : TDependency
        {
            AddTransient(typeof(TDependency), typeof(TImplementation));
        }

        public void AddTransient<TDependency, TImplementation>(Enum name) where TImplementation : TDependency
        {
            AddTransient(typeof(TDependency), typeof(TImplementation));
            if (!_transientNamedDependencies.ContainsKey(typeof(TDependency)))
            {
                _transientNamedDependencies.Add(typeof(TDependency), new Dictionary<Enum, Type>());
            }
            _transientNamedDependencies[typeof(TDependency)].Add(name, typeof(TImplementation));
        }

        public void AddTransient(Type dependency, Type implementation)
        {
            if (!_transientDependencies.ContainsKey(dependency))
            {
                _transientDependencies.Add(dependency, new List<Type>());
            }
            _transientDependencies[dependency].Add(implementation);
        }
    }
}
