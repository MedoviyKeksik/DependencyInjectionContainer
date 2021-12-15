using System;

namespace DependencyInjectionContainer
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class DependencyNameAttribute : Attribute
    {
        public Enum ImplementationName { get; }

        public DependencyNameAttribute(object implementationName)
        {
            ImplementationName = implementationName as Enum;
        }
    }
}