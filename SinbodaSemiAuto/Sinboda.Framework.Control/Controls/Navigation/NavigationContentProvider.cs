using Sinboda.Framework.Common.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Sinboda.Framework.Control.Controls.Navigation
{
    /// <summary>
    /// 默认的<see cref="INavigationContentProvider"/>
    /// </summary>
    public class NavigationContentProvider : INavigationContentProvider
    {
        #region INavigationContentProvider
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="userCallback"></param>
        /// <param name="asyncState"></param>
        /// <returns></returns>
        public virtual IAsyncResult BeginLoad(object source, AsyncCallback userCallback, object asyncState)
        {
            NavigationContentProviderAsyncResult result = new NavigationContentProviderAsyncResult(source, asyncState);
            if (source == null)
                result.Exception = new ArgumentNullException("source");

            NavigationOperation op = asyncState as NavigationOperation;

            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(delegate
            {
                if (result.Exception != null)
                    result.IsCompleted = true;

                try
                {
                    result.Content = Load(result.Source);
                }
                catch (Exception exception)
                {
                    LogHelper.logSoftWare.Error("NavigationContentBeginLoad error", exception);
                    result.Exception = exception;
                }
                finally
                {
                    result.IsCompleted = true;
                    if (userCallback != null)
                        userCallback(result);
                }

            }));
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asyncResult"></param>
        public virtual void CancelLoad(IAsyncResult asyncResult)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public virtual bool CanLoad(object source)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asyncResult"></param>
        /// <returns></returns>
        public virtual LoadResult EndLoad(IAsyncResult asyncResult)
        {
            NavigationContentProviderAsyncResult navigationContentProviderAsyncResult = asyncResult as NavigationContentProviderAsyncResult;
            if (navigationContentProviderAsyncResult == null)
            {
                throw new InvalidOperationException();
            }
            if (navigationContentProviderAsyncResult.Exception != null)
            {
                throw navigationContentProviderAsyncResult.Exception;
            }
            return new LoadResult(navigationContentProviderAsyncResult.Content);
        }

        /// <summary>
        /// 加载并返回导航结果
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public virtual object Load(object source)
        {
            object obj = null;
            if (source is Uri)
                obj = Application.LoadComponent((Uri)source);
            else if (source is Type)
                obj = LoadByType((Type)source);
            else if (source is string)
                obj = LoadContent(source as string);

            if (obj == null)
                obj = source;

            return FilterContent(obj);
        }
        #endregion

        /// <summary>
        ///
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private object FilterContent(object source)
        {
            if (source is Page)
                return ((Page)source).Content;

            return source;
        }

        private object LoadContent(string source)
        {
            if (source == null)
                return null;

            Type typeByName = TypeProvider.Current.GetTypeByName(source);
            if (typeByName == null)
                return null;

            return LoadByType(typeByName);
        }

        /// <summary>
        /// 加载类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public object LoadByType(Type type)
        {
            return Activator.CreateInstance(type);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class TypeProvider
    {
        /// <summary>
        /// 
        /// </summary>
        public static TypeProvider Current = new TypeProvider();

        /// <summary>
        ///  类型缓存
        /// </summary>
        protected static readonly Dictionary<string, Type> TypesCache = new Dictionary<string, Type>();
        /// <summary>
        /// 
        /// </summary>
        public virtual IEnumerable<Assembly> Assemblys
        {
            get { return AppDomain.CurrentDomain.GetAssemblies(); }
        }

        /// <summary>
        /// 根据类型名称查询<see cref="Type"/>
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public virtual Type GetTypeByName(string typeName)
        {
            if (Assemblys == null)
                return null;

            Type type = null;
            // 缓存中存在直接返回
            if (TypesCache.TryGetValue(typeName, out type))
                return type;

            // typeName 在本程序集中
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            type = executingAssembly.GetTypes().FirstOrDefault(t => t.Name == typeName);
            if (type != null)
            {
                TypesCache[typeName] = type;
                return type;
            }

            // 查询全部以加载程序集
            foreach (Assembly current in Assemblys)
            {
                // 跳过当前程序集，上边已经找过了
                if (current == executingAssembly)
                    continue;

                try
                {
                    type = current.GetTypes().FirstOrDefault((Type t) => t.Name == typeName);
                    if (type != null)
                    {
                        TypesCache[typeName] = type;
                        return type;
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    // TODO：加载程序集错误
                    LogHelper.logSoftWare.Error("TypeProvider error", ex);
                }
            }
            return type;
        }
    }

    /// <summary>
    /// 异步导航结果
    /// </summary>
    public class NavigationContentProviderAsyncResult : IAsyncResult
    {
        internal object Source { get; set; }

        internal Exception Exception { get; set; }

        internal object Content { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public object AsyncState { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public WaitHandle AsyncWaitHandle { get { return null; } }
        /// <summary>
        /// 
        /// </summary>
        public bool CompletedSynchronously { get { return false; } }

        /// <summary>
        /// 返回导航是否完成
        /// </summary>
        public bool IsCompleted { get; internal set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="asyncState"></param>
        public NavigationContentProviderAsyncResult(object source, object asyncState)
        {
            AsyncState = asyncState;
            Source = source;
        }
    }
}
