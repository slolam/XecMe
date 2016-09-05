using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XecMe.Common.Injection
{
    /// <summary>
    /// DI container abstraction interface
    /// </summary>
    public interface IContainer
    {
        /// <summary>
        /// Begins the scope of instances
        /// </summary>
        /// <returns></returns>
        IDisposable BeginScope();

        /// <summary>
        /// Registers the specified service type into the underlying container
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        void Register(Type serviceType);

        /// <summary>
        /// Registers this instance.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        void Register<TService, TImplementation>()
            where TService : class
            where TImplementation: class, TService;

        /// <summary>
        /// Gets the instance of the type requested form the container
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <returns></returns>
        TType GetInstance<TType>()
            where TType: class;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        object GetInstance(Type serviceType);

        /// <summary>
        /// Gets the collection of the type from the underlying container
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <returns></returns>
        IEnumerable<TType> GetCollection<TType>()
            where TType : class;


    }
}
