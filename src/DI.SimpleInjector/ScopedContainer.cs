using SimpleInjector;
using SimpleInjector.Extensions.ExecutionContextScoping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XecMe.Common.Injection;

namespace XecMe.DI.SimpleInjector
{
    public class ScopedContainer : IContainer
    {
        public ScopedContainer()
        {
            Container = new Container();
            Container.Options.DefaultScopedLifestyle = new ExecutionContextScopeLifestyle();
        }

        public Container Container { get; private set; }

        #region IContainer
        public IDisposable BeginScope()
        {
            return Container.BeginExecutionContextScope();
        }

        public IEnumerable<TType> GetCollection<TType>() where TType : class
        {
            return Container.GetAllInstances<TType>();
        }

        public object GetInstance(Type serviceType)
        {
            return Container.GetInstance(serviceType);
        }

        public TType GetInstance<TType>() where TType : class
        {
            return Container.GetInstance<TType>();
        }

        public void Register(Type serviceType)
        {
            Container.Register(serviceType, serviceType, Lifestyle.Scoped);
        }

        void IContainer.Register<TService, TImplementation>()
        {
            Container.Register<TService, TImplementation>(Lifestyle.Scoped);
        }
        #endregion
    }
}
