using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace UNIT
{
    public class RuntimeCache
    {
        private string regionName = null;
        /// <summary>
        /// 当前应用程序的的缓存实例
        /// </summary>
        public MemoryCache Current
        {
            get
            {
                return MemoryCache.Default;
            }
        }

        /// <summary>
        /// 已缓存的缓存项数量
        /// </summary>
        public long Count
        {
            get
            {
                return Current.GetCount();
            }
        }

        /// <summary>
        /// 获取某个缓存项
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns></returns>
        public object Get(string key)
        {
            return Current.Get(key);
        }

        /// <summary>
        /// 获取某个缓存项
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            object obj = Current.Get(key);
            if (obj != null)
            {
                return (T)Convert.ChangeType(obj, typeof(T));
            }
            return default(T);
        }

        #region 移除清除
        /// <summary>
        /// 移除某个缓存项
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns></returns>
        public object Remove(string key)
        {
            return Current.Remove(key);
        }
        /// <summary>
        /// 清除所有缓存
        /// </summary>
        public void Clear()
        {
            IDictionary<string, object> c = Current.GetValues(regionName);
            foreach (var k in c.Keys)
            {
                c.Remove(k);
            }
        }
        #endregion

        #region Add
        /// <summary>
        /// 添加缓存项
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="data">缓存数据</param>
        public void Add(string key, object data)
        {
            CacheItemPolicy policy = new CacheItemPolicy();
            Current.Add(key, data, policy, regionName);
        }
        /// <summary>
        /// 添加具有一个过期时间的缓存项
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="data">缓存数据</param>
        /// <param name="absoluteExpiration">绝对的过期时间,到此时间后缓存过期</param>
        public void Add(string key, object data, DateTime absoluteExpiration)
        {
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = absoluteExpiration;
            Current.Add(key, data, policy, regionName);
        }
        /// <summary>
        /// 添加具有某些依赖项的缓存项
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="data">缓存数据</param>
        /// <param name="dependencies">依赖项</param>
        public void Add(string key, object data, ICollection<ChangeMonitor> changeMonitor)
        {
            CacheItemPolicy policy = new CacheItemPolicy();
            if (changeMonitor != null)
            {
                foreach (var cm in changeMonitor)
                {
                    policy.ChangeMonitors.Add(cm);
                }
            }
            Current.Add(key, data, policy, regionName);
        }
        /// <summary>
        /// 添加与某些文件关连的缓存项,当这些文件被修改时缓存过期
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="data">缓存数据</param>
        /// <param name="files">缓存依赖的文件地址，当这些文件有变更时缓存项将自动失效</param>
        public void Add(string key, object data, params string[] files)
        {
            CacheItemPolicy policy = new CacheItemPolicy();
            if (files != null)
            {
                policy.ChangeMonitors.Add(new HostFileChangeMonitor(files.ToList<string>()));
            }
            Current.Add(key, data, policy, regionName);
        }
        /// <summary>
        /// 添加具有一个过期时间并且与某些文件关连的缓存项,当过期时间已到或这些文件被修改时缓存过期
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="data">缓存数据</param>
        /// <param name="absoluteExpiration">绝对的过期时间,到此时间后缓存项自动过期</param>
        /// <param name="files">缓存依赖的文件地址，当这些文件有变更时缓存项将自动失效</param>
        public void Add(string key, object data, DateTime absoluteExpiration, params string[] files)
        {
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = absoluteExpiration;
            if (files != null)
            {
                policy.ChangeMonitors.Add(new HostFileChangeMonitor(files.ToList<string>()));
            }
            Current.Add(key, data, policy, regionName);

        }
        #endregion

        #region Put
        /// <summary>
        /// 添加缓存项
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="data">缓存数据</param>
        public void Put(string key, object data)
        {
            Remove(key);
            CacheItemPolicy policy = new CacheItemPolicy();
            Current.Add(key, data, policy, regionName);
        }
        /// <summary>
        /// 添加具有一个过期时间的缓存项
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="data">缓存数据</param>
        /// <param name="absoluteExpiration">绝对的过期时间,到此时间后缓存过期</param>
        public void Put(string key, object data, DateTime absoluteExpiration)
        {
            Remove(key);
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = absoluteExpiration;
            Current.Add(key, data, policy, regionName);
        }
        /// <summary>
        /// 添加具有某些依赖项的缓存项
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="data">缓存数据</param>
        /// <param name="dependencies">依赖项</param>
        public void Put(string key, object data, ICollection<ChangeMonitor> changeMonitor)
        {
            Remove(key);
            CacheItemPolicy policy = new CacheItemPolicy();
            if (changeMonitor != null)
            {
                foreach (var cm in changeMonitor)
                {
                    policy.ChangeMonitors.Add(cm);
                }
            }
            Current.Add(key, data, policy, regionName);
        }
        /// <summary>
        /// 添加与某些文件关连的缓存项,当这些文件被修改时缓存过期
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="data">缓存数据</param>
        /// <param name="files">缓存依赖的文件地址，当这些文件有变更时缓存项将自动失效</param>
        public void Put(string key, object data, params string[] files)
        {
            Remove(key);
            CacheItemPolicy policy = new CacheItemPolicy();
            if (files != null)
            {
                policy.ChangeMonitors.Add(new HostFileChangeMonitor(files.ToList<string>()));
            }
            Current.Add(key, data, policy, regionName);
        }
        /// <summary>
        /// 添加具有一个过期时间并且与某些文件关连的缓存项,当过期时间已到或这些文件被修改时缓存过期
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="data">缓存数据</param>
        /// <param name="absoluteExpiration">绝对的过期时间,到此时间后缓存项自动过期</param>
        /// <param name="files">缓存依赖的文件地址，当这些文件有变更时缓存项将自动失效</param>
        public void Put(string key, object data, DateTime absoluteExpiration, params string[] files)
        {
            Remove(key);
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = absoluteExpiration;
            if (files != null)
            {
                policy.ChangeMonitors.Add(new HostFileChangeMonitor(files.ToList<string>()));
            }
            Current.Add(key, data, policy, regionName);

        }
        #endregion

        #region GetOrAdd
        /// <summary>
        /// 返回某个缓存项,如果不存在则调用函数委托获取值,并将值存入缓存后返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">缓存项键值</param>
        /// <param name="handler">缓存项不存在时重新获取数据的函数委托</param>
        /// <returns></returns>
        public T GetOrAdd<T>(string key, Func<T> handler)
        {
            var value = Get(key);
            if (value == null)
            {
                T v = handler.Invoke();
                if (v != null)
                {
                    Add(key, v);
                }
                return v;
            }
            return (T)value;
        }
        /// <summary>
        /// 返回某个缓存项,如果不存在则调用函数委托获取值,并将值存入缓存后返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">缓存项键值</param>
        /// <param name="absoluteExpiration">绝对的过期时间，当时间超过此值时缓存项将自动失效</param>
        /// <param name="handler">缓存项不存在时重新获取数据的函数委托</param>
        /// <returns></returns>
        public T GetOrAdd<T>(string key, Func<T> handler, DateTime absoluteExpiration)
        {
            var value = Get(key);
            if (value == null)
            {
                T v = handler.Invoke();
                if (v != null)
                {
                    Add(key, v, absoluteExpiration);
                }
                return v;
            }
            return (T)value;
        }
        /// <summary>
        /// 返回某个缓存项,如果不存在则调用函数委托获取值,并将值存入缓存后返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">缓存项键值</param>
        /// <param name="dependencies">缓存依赖</param>
        /// <param name="handler">缓存项不存在时重新获取数据的函数委托</param>
        /// <returns></returns>
        public T GetOrAdd<T>(string key, Func<T> handler, ICollection<ChangeMonitor> changeMonitor)
        {
            var value = Get(key);
            if (value == null)
            {
                T v = handler.Invoke();
                if (v != null)
                {
                    Add(key, v, changeMonitor);
                }
                return v;
            }
            return (T)value;
        }
        /// <summary>
        /// 返回某个缓存项,如果不存在则调用函数委托获取值,并将值存入缓存后返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">缓存项键值</param>
        /// <param name="files">缓存依赖的文件地址，当这些文件有变更时缓存项将自动失效</param>
        /// <param name="handler">缓存项不存在时重新获取数据的函数委托</param>
        /// <returns></returns>
        public T GetOrAdd<T>(string key, Func<T> handler, params string[] files)
        {
            var value = Get(key);
            if (value == null)
            {
                T v = handler.Invoke();
                if (v != null)
                {
                    Add(key, v, files);
                }
                return v;
            }
            return (T)value;
        }
        /// <summary>
        /// 返回某个缓存项,如果不存在则调用函数委托获取值,并将值存入缓存后返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">缓存项键值</param>
        /// <param name="absoluteExpiration">绝对的过期时间，当时间超过此值时缓存项将自动失效</param>
        /// <param name="files">缓存依赖的文件地址，当这些文件有变更时缓存项将自动失效</param>
        /// <param name="handler">缓存项不存在时重新获取数据的函数委托</param>
        /// <returns></returns>
        public T GetOrAdd<T>(string key, Func<T> handler, DateTime absoluteExpiration, params string[] files)
        {
            var value = Get(key);
            if (value == null)
            {
                T v = handler.Invoke();
                if (v != null)
                {
                    Add(key, v, absoluteExpiration, files);
                }
                return v;
            }
            return (T)value;
        }
        #endregion
    }
}