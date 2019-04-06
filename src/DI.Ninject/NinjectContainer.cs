using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XecMe.Common.Injection;
using Ninject;
using Ninject.Activation.Blocks;
using System.Threading;

namespace XecMe.DI.Ninject
{
    /// <summary>
    /// Ninject implementation of <see cref="IContainer"/> 
    /// </summary>
    /// <seealso cref="IContainer" />
    public class NinjectContainer : IContainer
    {
        /// <summary>
        /// The scope
        /// </summary>
        IActivationBlock _scope;

        /// <summary>
        /// Initializes a new instance of the <see cref="NinjectContainer"/> class.
        /// </summary>
        public NinjectContainer()
        {
            Kernel = new StandardKernel();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NinjectContainer"/> class.
        /// </summary>
        /// <param name="kernal">The kernal.</param>
        /// <param name="scope">The scope.</param>
        private NinjectContainer(IKernel kernal, IActivationBlock scope)
        {
            Kernel = kernal;
            _scope = scope;
        }
        /// <summary>
        /// Gets the Ninject kernel.
        /// </summary>
        /// <value>
        /// The kernel.
        /// </value>
        public IKernel Kernel { get; private set; }

        #region IContainer        
        /// <summary>
        /// Begins the new scope of object creation instances
        /// </summary>
        /// <returns></returns>
        public IContainer BeginScope()
        {
            return new NinjectContainer(Kernel, Kernel.BeginBlock());
        }



        /// <summary>
        /// Gets the collection of the type from the underlying container
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <returns>Collection of <typeparam name="TType>type</typeparam></returns>
        public IEnumerable<TType> GetCollection<TType>() where TType : class
        {
            return _scope.GetAll<TType>();
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        public object GetInstance(Type serviceType)
        {
            return _scope?.Get(serviceType);
        }

        /// <summary>
        /// Gets the instance of the type requested form the container
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <returns>Object of the <typeparam name="TType>type</typeparam></returns>
        public TType GetInstance<TType>() where TType : class
        {
            return _scope?.Get<TType>();
        }

        /// <summary>
        /// Registers the specified service type into the underlying container
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="lifeStyle">The life style.</param>
        /// <exception cref="ArgumentOutOfRangeException">lifeStyle</exception>
        public void Register(Type serviceType, LifeStyle lifeStyle = LifeStyle.Scoped)
        {
            var scope = Kernel.Bind(serviceType)
                .ToSelf();

            switch (lifeStyle)
            {
                case LifeStyle.Scoped:
                    scope.InScope((IContext) => this);
                    break;
                case LifeStyle.Transcient:
                    scope.InTransientScope();
                    break;
                case LifeStyle.Singleton:
                    scope.InSingletonScope();
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
            var scope = Kernel.Bind(serviceType)
                .ToMethod((context) => factory(this));

            switch (lifeStyle)
            {
                case LifeStyle.Scoped:
                    scope.InScope((IContext) => this);
                    break;
                case LifeStyle.Transcient:
                    scope.InTransientScope();
                    break;
                case LifeStyle.Singleton:
                    scope.InSingletonScope();
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
            var scope = Kernel.Bind<TService>()
                .To<TImplementation>();

            switch (lifeStyle)
            {
                case LifeStyle.Scoped:
                    scope.InScope((IContext) => this);
                    break;
                case LifeStyle.Transcient:
                    scope.InTransientScope();
                    break;
                case LifeStyle.Singleton:
                    scope.InSingletonScope();
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
        /// <exception cref="ArgumentOutOfRangeException">lifeStyle</exception>
        public void Register<TService, TImplementation>(Func<IContainer, TImplementation> factory, LifeStyle lifeStyle)
             where TService : class
            where TImplementation : class, TService
        {
            var scope = Kernel.Bind<TService>()
                .ToMethod<TImplementation>((context) => factory(this));

            switch (lifeStyle)
            {
                case LifeStyle.Scoped:
                    scope.InScope((IContext) => this);
                    break;
                case LifeStyle.Transcient:
                    scope.InTransientScope();
                    break;
                case LifeStyle.Singleton:
                    scope.InSingletonScope();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lifeStyle));
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _scope?.Dispose();

        }
        #endregion
    }
}
