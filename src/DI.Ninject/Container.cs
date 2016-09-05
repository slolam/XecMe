using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XecMe.Common.Injection;
using Ninject;

namespace XecMe.DI.Ninject
{
    public class Container : IContainer
    {
        public Container()
        {
            Kernel = new StandardKernel();
        }

        public StandardKernel Kernel { get; private set; }

        #region IContainer
        public IDisposable BeginScope()
        {
            return Kernel.BeginBlock();
        }

        public IEnumerable<TType> GetCollection<TType>() where TType : class
        {
            return Kernel.GetAll<TType>();
        }

        public object GetInstance(Type serviceType)
        {
            return Kernel.Get(serviceType);
        }

        public TType GetInstance<TType>() where TType : class
        {
            return Kernel.Get<TType>();
        }

        public void Register(Type serviceType)
        {
            Kernel.Bind(serviceType)
                .To(serviceType);
        }

        void IContainer.Register<TService, TImplementation>()
        {
            Kernel.Bind<TService>()
                .To<TImplementation>();
        }
        #endregion
    }
}
