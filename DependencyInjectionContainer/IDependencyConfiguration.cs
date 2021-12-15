using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainer
{
    public interface IDependencyConfiguration
    {
        Dictionary<Type, List<Type>> TransientDependencies { get; }
        Dictionary<Type, List<Type>> SingletonDependencies { get; }
        Dictionary<Type, Dictionary<Enum, Type>> TransientNamedDependencies { get; }
        Dictionary<Type, Dictionary<Enum, Type>> SingletonNamedDependencies { get; }

        public void AddTransient<TDependency, TImplementation>() where TImplementation : TDependency;

        public void AddTransient<TDependency, TImplementation>(Enum name) where TImplementation : TDependency;

        public void AddTransient(Type dependecy, Type implementation);

        public void AddSingleton<TDependency, TImplementation>() where TImplementation : TDependency;

        public void AddSingleton<TDependency, TImplementation>(Enum name) where TImplementation : TDependency;

        public void AddSingleton(Type dependecy, Type implementation);
    }
}
