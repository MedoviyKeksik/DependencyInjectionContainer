using System;
using System.Collections.Generic;
using System.Linq;

namespace DependencyInjectionContainer
{
    public class DependencyProvider : IDependencyProvider
    {
        private IDependencyConfiguration _configuration;
        private Dictionary<Type, List<Object>> _singletons;
        private Dictionary<Type, Dictionary<Enum, object>> _namedSingletons;
        public DependencyProvider(IDependencyConfiguration configuration)
        {
            _configuration = configuration;
            _singletons = new Dictionary<Type, List<object>>();
            _namedSingletons = new Dictionary<Type, Dictionary<Enum, object>>();
        }

        public TDependency Resolve<TDependency>() where TDependency : class
        {
            throw new NotImplementedException();
        }

        public TDependency Resolve<TDependency>(Enum name) where TDependency : class
        {
            throw new NotImplementedException();
        }


        private bool CanCreate(Type type)
        {
            if (type.IsValueType) return true;
            return type.GetConstructors()
                .Any(c => c.GetParameters()
                    .All(p => GetImplementationTypes(p.ParameterType)
                        .Any(impl => CanCreate(impl))));
        }

        private List<Type> GetImplementationTypes(Type type, Enum name = null)
        {
            if (name != null)
            {
                if (_configuration.SingletonNamedDependencies.ContainsKey(type))
                {
                    if (_configuration.SingletonNamedDependencies[type].ContainsKey(name))
                        return new List<Type>() { _configuration.SingletonNamedDependencies[type][name] };
                    else return new List<Type>();
                }
                
                if (_configuration.TransientNamedDependencies.ContainsKey(type))
                {
                    if (_configuration.TransientNamedDependencies[type].ContainsKey(name))
                        return new List<Type>() { _configuration.TransientNamedDependencies[type][name] };
                    else return new List<Type>();
                }
                return new List<Type>();
            }
            if (_configuration.SingletonDependencies.ContainsKey(type)) new List<Type>(_configuration.SingletonDependencies[type]);
            if (_configuration.TransientDependencies.ContainsKey(type)) new List<Type>(_configuration.TransientDependencies[type]);
            return new List<Type>() { type };
        }
        
        private object GetInstance(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            var targetTypes = GetImplementationTypes(type);
            if (!targetTypes.Any(t => CanCreate(t))) throw new ArgumentException("Cannot create instance of type " + type.ToString());
            var targetType = targetTypes.First(t => CanCreate(t)); 
            var constructor = targetType.GetConstructors().Where(c => c.GetParameters().All(p => CanCreate(p.ParameterType))).First();
            var parameters = constructor.GetParameters().Select(p => GetInstance(p.ParameterType)).ToArray();
            return constructor.Invoke(parameters);
        }
    }
}
