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
    /// <summary>
    /// Simple Injector's implementation of the <see cref="IContainer"/>
    /// </summary>
    /// <seealso cref="XecMe.Common.Injection.IContainer" />
    public class ScopedContainer : IContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScopedContainer"/> class.
        /// </summary>
        public ScopedContainer()
        {
            Container = new Container();
            Container.Options.DefaultScopedLifestyle = new ExecutionContextScopeLifestyle();
        }

        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <value>
        /// The container.
        /// </value>
        public Container Container { get; private set; }

        #region IContainer        
        /// <summary>
        /// Begins the scope of instances
        /// </summary>
        /// <returns></returns>
        public IDisposable BeginScope()
        {
            return Container.BeginExecutionContextScope();
        }

        /// <summary>
        /// Gets the collection of the type from the underlying container
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <returns></returns>
        public IEnumerable<TType> GetCollection<TType>() where TType : class
        {
            return Container.GetAllInstances<TType>();
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        public object GetInstance(Type serviceType)
        {
            return Container.GetInstance(serviceType);
        }

        /// <summary>
        /// Gets the instance of the type requested form the container
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <returns></returns>
        public TType GetInstance<TType>() where TType : class
        {
            return Container.GetInstance<TType>();
        }

        /// <summary>
        /// Registers the specified service type into the underlying container
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        public void Register(Type serviceType)
        {
            Container.Register(serviceType, serviceType, Lifestyle.Scoped);
        }

        /// <summary>
        /// Registers this instance.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        void IContainer.Register<TService, TImplementation>()
        {
            Container.Register<TService, TImplementation>(Lifestyle.Scoped);
        }
        #endregion
    }
}
