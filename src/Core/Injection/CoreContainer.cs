using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using XecMe.Common.Injection;

namespace XecMe.Core.Injection
{
    /// <summary>
    /// .NET Core implementation of the DI
    /// </summary>
    /// <seealso cref="XecMe.Common.Injection.IContainer" />
    public class CoreContainer : IContainer
    {
        /// <summary>
        /// The provider
        /// </summary>
        private IServiceProvider _provider;

        /// <summary>
        /// The scope
        /// </summary>
        private IServiceScope _scope;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreContainer"/> class.
        /// </summary>
        public CoreContainer()
        {
            ServiceCollection = new ServiceCollection();
            _provider = ServiceCollection.BuildServiceProvider();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreContainer"/> class.
        /// </summary>
        /// <param name="collection">The collection.</param>
        private CoreContainer(IServiceCollection collection, IServiceScope scope)
        {
            _scope = scope;
            _provider = _scope.ServiceProvider;
            ServiceCollection = collection;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreContainer"/> class.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="provider">The provider.</param>
        private CoreContainer(IServiceCollection collection, IServiceProvider provider)
        {
            _provider = provider;
            ServiceCollection = collection;
        }

        /// <summary>
        /// Gets the service collection.
        /// </summary>
        /// <value>
        /// The service collection.
        /// </value>
        public IServiceCollection ServiceCollection { get; }

        /// <summary>
        /// Begins the scope of instances
        /// </summary>
        /// <returns></returns>
        public IContainer BeginScope()
        {
            return new CoreContainer(ServiceCollection, _provider.CreateScope());
        }

        /// <summary>
        /// Gets the collection of the type from the underlying container
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <returns></returns>
        public IEnumerable<TType> GetCollection<TType>() where TType : class
        {
            return _provider.GetServices<TType>();
        }

        /// <summary>
        /// Gets the instance of the type requested form the container
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <returns></returns>
        public TType GetInstance<TType>() where TType : class
        {
            return _provider.GetRequiredService<TType>();
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        public object GetInstance(Type serviceType)
        {
            return _provider.GetRequiredService(serviceType);
        }


        /// <summary>
        /// Registers the specified service type into the underlying container
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="lifeStyle">The life style.</param>
        /// <exception cref="ArgumentOutOfRangeException">lifeStyle</exception>
        public void Register(Type serviceType, LifeStyle lifeStyle = LifeStyle.Scoped)
        {
            switch (lifeStyle)
            {
                case LifeStyle.Scoped:
                    ServiceCollection.AddScoped(serviceType);
                    break;
                case LifeStyle.Transcient:
                    ServiceCollection.AddTransient(serviceType);
                    break;
                case LifeStyle.Singleton:
                    ServiceCollection.AddSingleton(serviceType);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lifeStyle));
            }
        }

        /// <summary>
        /// Registers the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="factory">The factory.</param>
        /// <param name="lifeStyle">The life style.</param>
        /// <exception cref="ArgumentOutOfRangeException">lifeStyle</exception>
        public void Register(Type serviceType, Func<IContainer, object> factory, LifeStyle lifeStyle = LifeStyle.Scoped)
        {
            switch (lifeStyle)
            {
                case LifeStyle.Scoped:
                    ServiceCollection.AddScoped(serviceType, (provider) => factory(new CoreContainer(ServiceCollection, provider)));
                    break;
                case LifeStyle.Transcient:
                    ServiceCollection.AddTransient(serviceType, (provider) => factory(new CoreContainer(ServiceCollection, provider)));
                    break;
                case LifeStyle.Singleton:
                    ServiceCollection.AddSingleton(serviceType, (provider) => factory(new CoreContainer(ServiceCollection, provider)));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lifeStyle));
            }
        }

        


        /// <summary>
        /// Registers this instance.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="lifeStyle">The life style.</param>
        public void Register<TService, TImplementation>(LifeStyle lifeStyle = LifeStyle.Scoped)
            where TService : class
            where TImplementation : class, TService
        {
            switch (lifeStyle)
            {
                case LifeStyle.Scoped:
                    ServiceCollection.AddScoped<TService, TImplementation>();
                    break;
                case LifeStyle.Transcient:
                    ServiceCollection.AddTransient<TService, TImplementation>();
                    break;
                case LifeStyle.Singleton:
                    ServiceCollection.AddSingleton<TService, TImplementation>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lifeStyle));
            }
        }

        public void Register<TService, TImplementation>(Func<IContainer, TImplementation> factory, LifeStyle lifeStyle)
            where TService : class
            where TImplementation : class, TService
        {
            switch (lifeStyle)
            {
                case LifeStyle.Scoped:
                    ServiceCollection.AddScoped<TService, TImplementation>((provider) => factory(new CoreContainer(ServiceCollection, provider)));
                    break;
                case LifeStyle.Transcient:
                    ServiceCollection.AddTransient<TService, TImplementation>((provider) => factory(new CoreContainer(ServiceCollection, provider)));
                    break;
                case LifeStyle.Singleton:
                    ServiceCollection.AddSingleton<TService, TImplementation>((provider) => factory(new CoreContainer(ServiceCollection, provider)));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lifeStyle));
            }
        }

        #region IDisposable Support        
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _scope.Dispose();
        }
        #endregion
    }
}
