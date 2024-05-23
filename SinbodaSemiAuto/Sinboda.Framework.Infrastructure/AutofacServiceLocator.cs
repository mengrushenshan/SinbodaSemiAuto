using Autofac;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Infrastructure
{
    public class AutofacServiceLocator : ServiceLocatorImplBase
    {
        private readonly IComponentContext _container;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="container"></param>
        public AutofacServiceLocator(IComponentContext container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            _container = container;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        protected override object DoGetInstance(Type serviceType, string key)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException("serviceType");
            }
            if (key == null)
            {
                return _container.Resolve(serviceType);
            }
            return _container.ResolveNamed(key, serviceType);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException("serviceType");
            }
            Type serviceType2 = typeof(IEnumerable).MakeGenericType(new Type[]
            {
                serviceType
            });
            object obj = _container.Resolve(serviceType2);
            return Enumerable.Cast<object>((IEnumerable)obj);
        }
    }
}
