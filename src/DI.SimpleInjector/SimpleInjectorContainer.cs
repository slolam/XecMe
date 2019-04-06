using SimpleInjector;
using SimpleInjector.Lifestyles;
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
    public class SimpleInjectorContainer : IContainer
    {

        Scope _scope;
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleInjectorContainer"/> class.
        /// </summary>
        public SimpleInjectorContainer()
        {
            Container = new Container();
            Container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
        }


        private SimpleInjectorContainer(Scope scope)
        {
            _scope = scope;
            Container = scope.Container;
        }


        private SimpleInjectorContainer(Container container)
        {
            Container = container;
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
        public IContainer BeginScope()
        {
            return new SimpleInjectorContainer(AsyncScopedLifestyle.BeginScope(Container));
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
        /// Registers the specified service type into the underlying container
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="lifeStyle">The life style.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void Register(Type serviceType, LifeStyle lifeStyle = LifeStyle.Scoped)
        {
            switch (lifeStyle)
            {
                case LifeStyle.Scoped:
                    Container.Register(serviceType, serviceType, Lifestyle.Scoped);
                    break;
                case LifeStyle.Transcient:
                    Container.Register(serviceType, serviceType, Lifestyle.Transient);
                    break;
                case LifeStyle.Singleton:
                    Container.Register(serviceType, serviceType, Lifestyle.Singleton);
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
                    Container.Register(serviceType, () => factory(new SimpleInjectorContainer(Container)), Lifestyle.Scoped);
                    break;
                case LifeStyle.Transcient:
                    Container.Register(serviceType, () => factory(new SimpleInjectorContainer(Container)), Lifestyle.Transient);
                    break;
                case LifeStyle.Singleton:
                    Container.Register(serviceType, () => factory(new SimpleInjectorContainer(Container)), Lifestyle.Singleton);
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
        /// <exception cref="ArgumentOutOfRangeException">lifeStyle</exception>
        public void Register<TService, TImplementation>(LifeStyle lifeStyle)
            where TService : class
            where TImplementation : class, TService
        {
            switch (lifeStyle)
            {
                case LifeStyle.Scoped:
                    Container.Register<TService, TImplementation>(Lifestyle.Scoped);
                    break;
                case LifeStyle.Transcient:
                    Container.Register<TService, TImplementation>(Lifestyle.Transient);
                    break;
                case LifeStyle.Singleton:
                    Container.Register<TService, TImplementation>(Lifestyle.Singleton);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lifeStyle));
            }
        }


        /// <summary>
        /// Registers the specified factory.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="factory">The factory.</param>
        /// <param name="lifeStyle">The life style.</param>
        /// <exception cref="NotSupportedException">This is not supported in SimpleInjector implementation</exception>
        public void Register<TService, TImplementation>(Func<IContainer, TImplementation> factory, LifeStyle lifeStyle)
            where TService : class
            where TImplementation : class, TService
        {
            switch (lifeStyle)
            {
                case LifeStyle.Scoped:
                    Container.Register(typeof(TService), () => factory(this), Lifestyle.Scoped);
                    break;
                case LifeStyle.Transcient:
                    Container.Register(typeof(TService), () => factory(this), Lifestyle.Transient);
                    break;
                case LifeStyle.Singleton:
                    Container.Register(typeof(TService), () => factory(this), Lifestyle.Singleton);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lifeStyle));
            }
        }
        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _scope?.Dispose();
        }
    }
}
