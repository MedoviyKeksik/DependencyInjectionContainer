using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainer
{
    public interface IDependencyProvider
    {
        public TDependency Resolve<TDependency>() where TDependency : class;
        public TDependency Resolve<TDependency>(Enum name) where TDependency : class;
    }
}
