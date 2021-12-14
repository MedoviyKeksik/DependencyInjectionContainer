using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainer
{
    public class DependencyConfiguration : IDependencyConfiguration
    {
        private Dictionary<Type, List<Type>> _transientDependencies;
        private Dictionary<Type, List<Type>> _singletonDependencies;
        private Dictionary<Type, Dictionary<Enum, Type>> _transientNamedDependencies;
        private Dictionary<Type, Dictionary<Enum, Type>> _singletonNamedDependencies;

        public DependencyConfiguration()
        { 
        }
        public IDictionary<Type, IEnumerable<Type>> TransientDependencies => (IDictionary<Type, IEnumerable<Type>>)_transientDependencies;

        public IDictionary<Type, IEnumerable<Type>> SingletonDependencies => (IDictionary<Type, IEnumerable<Type>>)_singletonDependencies;

        public IDictionary<Type, IDictionary<Enum, Type>> TransientNamedDependencies => (IDictionary<Type, IDictionary<Enum, Type>>)_transientNamedDependencies;

        public IDictionary<Type, IDictionary<Enum, Type>> SingletonNamedDependencies => (IDictionary<Type, IDictionary<Enum, Type>>)_singletonNamedDependencies;

        public void AddSingleton<TDependency, TImplementation>() where TImplementation : TDependency
        {
            AddSingleton(typeof(TDependency), typeof(TImplementation));
        }

        public void AddSingleton<TDependency, TImplementation>(Enum name) where TImplementation : TDependency
        {
            AddSingleton(typeof(TDependency), typeof(TImplementation));
            if (_singletonNamedDependencies.ContainsKey(typeof(TDependency)))
            {
                _singletonNamedDependencies.Add(typeof(TDependency), new Dictionary<Enum, Type>());
            }
            _singletonNamedDependencies[typeof(TDependency)].Add(name, typeof(TImplementation));
        }

        public void AddSingleton(Type dependecy, Type implementation)
        {
            if (_singletonDependencies.ContainsKey(dependecy))
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
            if (_transientNamedDependencies.ContainsKey(typeof(TDependency)))
            {
                _transientNamedDependencies.Add(typeof(TDependency), new Dictionary<Enum, Type>());
            }
            _transientNamedDependencies[typeof(TDependency)].Add(name, typeof(TImplementation));
        }

        public void AddTransient(Type dependecy, Type implementation)
        {
            if (_transientDependencies.ContainsKey(dependecy))
            {
                _transientDependencies.Add(dependecy, new List<Type>());
            }
            _transientDependencies[dependecy].Add(implementation);
        }
    }
}
