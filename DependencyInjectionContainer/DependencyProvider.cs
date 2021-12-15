using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace DependencyInjectionContainer
{
    public class DependencyProvider : IDependencyProvider
    {
        private readonly IDependencyConfiguration _configuration;

        private readonly Dictionary<Type, List<object>> _singletons = new();
        private readonly Dictionary<Type, Dictionary<Enum, object>> _namedSingletons = new();

        public DependencyProvider(IDependencyConfiguration configuration)
        {
            _configuration = configuration;
            foreach (var singletonType in configuration.SingletonDependencies)
            {
                _singletons.Add(singletonType.Key, new List<object>());
            }
        }

        private readonly Mutex _mutex = new();

        public TDependency Resolve<TDependency>() where TDependency : class
        {
            var targetType = typeof(TDependency);
            return (TDependency)Resolve(targetType, new Stack<Type>());
        }

        public TDependency Resolve<TDependency>(Enum implementationName) where TDependency : class
        {
            var targetType = typeof(TDependency);
            return (TDependency)Resolve(targetType, new Stack<Type>(), implementationName);
        }

        private object Resolve(Type targetType, Stack<Type> stack, Enum implementationName = null)
        {
            if (_configuration.SingletonDependencies.ContainsKey(targetType))
            {
                if (implementationName != null
                    && _configuration.SingletonNamedDependencies.ContainsKey(targetType)
                    && _configuration.SingletonNamedDependencies[targetType].ContainsKey(implementationName))
                {
                    _mutex.WaitOne();

                    if (!_namedSingletons[targetType].ContainsKey(implementationName))
                    {
                        stack.Push(_configuration.SingletonNamedDependencies[targetType][implementationName]);
                        _namedSingletons[targetType].Add(implementationName, ConstructType(stack));
                    }

                    _mutex.ReleaseMutex();
                    return _namedSingletons[targetType][implementationName];
                }

                if (implementationName != null)
                {
                    return null;
                }

                _mutex.WaitOne();
                if (_singletons[targetType].Count == 0)
                {
                    stack.Push(_configuration.SingletonDependencies[targetType].First());
                    _singletons[targetType].Add(ConstructType(stack));
                }

                _mutex.ReleaseMutex();
                return _singletons[targetType].First();
            }

            if (_configuration.TransientDependencies.ContainsKey(targetType))
            {
                if (implementationName != null
                    && _configuration.TransientNamedDependencies.ContainsKey(targetType)
                    && _configuration.TransientNamedDependencies[targetType].ContainsKey(implementationName))
                {
                    stack.Push(_configuration.TransientNamedDependencies[targetType][implementationName]);
                    return ConstructType(stack);
                }

                if (implementationName != null)
                {
                    return null;
                }

                stack.Push(_configuration.TransientDependencies[targetType].First());
                return ConstructType(stack);
            }

            if (targetType.GenericTypeArguments.Length != 0
                && targetType == typeof(IEnumerable<>).MakeGenericType(targetType.GenericTypeArguments.First()))
            {
                targetType = targetType.GenericTypeArguments.First();
                if (_configuration.SingletonDependencies.ContainsKey(targetType)
                    || _configuration.TransientDependencies.ContainsKey(targetType))
                {
                    return GetAllImplementations(targetType);
                }
            }

            if (targetType.IsGenericType)
            {
                var genericType = targetType.GetGenericTypeDefinition();
                var genericArgs = targetType.GenericTypeArguments;

                if (_configuration.SingletonDependencies.ContainsKey(genericType))
                {
                    if (implementationName != null
                        && _configuration.SingletonNamedDependencies.ContainsKey(targetType)
                        && _configuration.SingletonNamedDependencies[targetType].ContainsKey(implementationName))
                    {
                        _mutex.WaitOne();

                        if (!_namedSingletons[targetType].ContainsKey(implementationName))
                        {
                            stack.Push(_configuration.SingletonNamedDependencies[targetType][implementationName].MakeGenericType(genericArgs));
                            _namedSingletons[targetType].Add(implementationName, ConstructType(stack));
                        }

                        _mutex.ReleaseMutex();
                        return _namedSingletons[targetType][implementationName];
                    }

                    if (implementationName != null)
                    {
                        return null;
                    }

                    _mutex.WaitOne();
                    if (_singletons[genericType].Count == 0)
                    {
                        stack.Push(_configuration.SingletonDependencies[genericType].First().MakeGenericType(genericArgs));
                        _singletons[genericType].Add(ConstructType(stack));
                    }
                    _mutex.ReleaseMutex();
                    return _singletons[genericType].First();
                }

                if (_configuration.TransientDependencies.ContainsKey(genericType))
                {
                    if (implementationName != null
                        && _configuration.SingletonNamedDependencies.ContainsKey(targetType)
                        && _configuration.SingletonNamedDependencies[targetType].ContainsKey(implementationName))
                    {
                        stack.Push(_configuration.TransientNamedDependencies[targetType][implementationName].MakeGenericType(genericArgs));
                        return ConstructType(stack);
                    }

                    if (implementationName != null)
                    {
                        return null;
                    }

                    stack.Push(_configuration.TransientDependencies[genericType].First().MakeGenericType(genericArgs));
                    return ConstructType(stack);
                }
            }

            return null;
        }

        private object GetAllImplementations(Type targetType)
        {
            var listType = typeof(List<>).MakeGenericType(targetType);
            var list = Activator.CreateInstance(listType);

            if (_configuration.SingletonDependencies.ContainsKey(targetType))
            {
                _mutex.WaitOne();
                foreach (var implementationType in _configuration.SingletonDependencies[targetType])
                {
                    if (_singletons[targetType].FirstOrDefault(s => s.GetType() == implementationType) == null)
                    {
                        var stack = new Stack<Type>();
                        stack.Push(implementationType);
                        _singletons[targetType].Add(ConstructType(stack));
                    }
                }

                foreach (var singleton in _singletons[targetType])
                {
                    var obj = new[] { singleton };
                    listType.GetMethod("Add")?.Invoke(list, obj);
                }
                _mutex.ReleaseMutex();
            }

            if (_configuration.TransientDependencies.ContainsKey(targetType))
            {
                foreach (var type in _configuration.TransientDependencies[targetType])
                {
                    var stack = new Stack<Type>();
                    stack.Push(type);
                    var obj = new[] { ConstructType(stack) };
                    listType.GetMethod("Add")?.Invoke(list, obj);
                }
            }

            return list;
        }

        private object ConstructType(Stack<Type> dependencyStack)
        {
            var targetType = dependencyStack.Peek();

            foreach (var target in dependencyStack.Skip(1))
            {
                if (target == targetType)
                {
                    return null;
                }
            }

            var constructors = targetType.GetConstructors().OrderByDescending(c =>
            {
                int lostArgsCount = 0;
                foreach (var argument in c.GetParameters())
                {
                    if (targetType.GenericTypeArguments.Length != 0
                        && targetType == typeof(IEnumerable<>).MakeGenericType(targetType.GenericTypeArguments.First()))
                    {
                        var genericArg = argument.ParameterType.GenericTypeArguments.First();
                        if (!_configuration.SingletonDependencies.ContainsKey(genericArg)
                            || !_configuration.TransientDependencies.ContainsKey(genericArg))
                        {
                            lostArgsCount++;
                        }
                    }
                    else if (!_configuration.SingletonDependencies.ContainsKey(argument.ParameterType)
                             || !_configuration.TransientDependencies.ContainsKey(argument.ParameterType))
                    {
                        lostArgsCount++;
                    }
                }
                return lostArgsCount;
            });

            object result = null;
            foreach (var constructor in constructors)
            {
                try
                {
                    var arguments = new List<object>();
                    foreach (var argument in constructor.GetParameters())
                    {
                        if (argument.ParameterType.IsValueType)
                        {
                            arguments.Add(CreateDefaultValue(argument.ParameterType));
                        }
                        else
                        {
                            Enum implementationName = null;
                            var dependencyNameAttribute = argument.GetCustomAttributes()
                                .FirstOrDefault(atr => atr.GetType() == typeof(DependencyNameAttribute));
                            if (dependencyNameAttribute != null)
                            {
                                implementationName = ((DependencyNameAttribute)dependencyNameAttribute)
                                    .ImplementationName;
                            }
                            arguments.Add(Resolve(argument.ParameterType, dependencyStack, implementationName));
                        }
                    }
                    result = constructor.Invoke(arguments.ToArray());
                }
                catch
                {
                    //Ignore
                }
            }

            return result;
        }

        private static object CreateDefaultValue(Type t)
        {
            return t.IsValueType
                ? Activator.CreateInstance(t)
                : null;
        }
    }
}
