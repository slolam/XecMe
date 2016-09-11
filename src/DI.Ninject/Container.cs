using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XecMe.Common.Injection;
using Ninject;

namespace XecMe.DI.Ninject
{
    /// <summary>
    /// Ninject implementation of <see cref="IContainer"/> 
    /// </summary>
    /// <seealso cref="IContainer" />
    public class Container : IContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Container"/> class.
        /// </summary>
        public Container()
        {
            Kernel = new StandardKernel();
        }

        /// <summary>
        /// Gets the Ninject kernel.
        /// </summary>
        /// <value>
        /// The kernel.
        /// </value>
        public StandardKernel Kernel { get; private set; }

        #region IContainer        
        /// <summary>
        /// Begins the new scope of object creation instances
        /// </summary>
        /// <returns></returns>
        public IDisposable BeginScope()
        {
            return Kernel.BeginBlock();
        }

        /// <summary>
        /// Gets the collection of the type from the underlying container
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <returns></returns>
        public IEnumerable<TType> GetCollection<TType>() where TType : class
        {
            return Kernel.GetAll<TType>();
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        public object GetInstance(Type serviceType)
        {
            return Kernel.Get(serviceType);
        }

        /// <summary>
        /// Gets the instance of the type requested form the container
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <returns></returns>
        public TType GetInstance<TType>() where TType : class
        {
            return Kernel.Get<TType>();
        }

        /// <summary>
        /// Registers the specified service type into the underlying container
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        public void Register(Type serviceType)
        {
            Kernel.Bind(serviceType)
                .To(serviceType);
        }

        /// <summary>
        /// Registers this instance.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        void IContainer.Register<TService, TImplementation>()
        {
            Kernel.Bind<TService>()
                .To<TImplementation>();
        }
        #endregion
    }
}
