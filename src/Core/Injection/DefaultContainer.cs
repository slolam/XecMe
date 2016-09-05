using System;
using System.Collections.Generic;
using XecMe.Common;
using XecMe.Common.Injection;

namespace XecMe.Core.Injection
{
    public class DefaultContainer : IContainer
    {
        public IDisposable BeginScope()
        {
            return null;
        }

        public IEnumerable<TType> GetCollection<TType>() where TType : class
        {
            throw new NotImplementedException();
        }

        public TType GetInstance<TType>() where TType : class
        {
            return Reflection.CreateInstance<TType>();
        }

        public object GetInstance(Type serviceType)
        {
            return Reflection.CreateInstance(serviceType);
        }

        public void Register(Type serviceType)
        {
            throw new NotImplementedException();
        }

        void IContainer.Register<TService, TImplementation>()
        {
            throw new NotImplementedException();
        }
    }
}
