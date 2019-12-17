using StackExchange.Redis;
using System;
using System.Configuration;

namespace UNIT
{
    public class RedisCache
    {
        #region  私有方法
        private int _dbNum { set; get; }//那个db
        private readonly ConnectionMultiplexer _conn;

        private static ConnectionMultiplexer _redisConn;
        private static readonly object Locker = new object();
        private static readonly string connString = ConfigurationManager.ConnectionStrings["redis"].ConnectionString;//连接字符串
        private static ConnectionMultiplexer _instance
        {
            get
            {

                return null;
                //if (_redisConn == null)
                //{
                //    lock (Locker)
                //    {
                //        if (_redisConn == null || !_redisConn.IsConnected)
                //        {
                //            _redisConn = ConnectionMultiplexer.Connect(connString);
                //        }
                //    }
                //}
                //return _redisConn;
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbNum">那个db，默认0</param>
        public RedisCache(int dbNum = 0)
        {
            _dbNum = dbNum;
            _conn = _instance;
        }
        public static RedisCache instance => new RedisCache(); //实例化
        #endregion

        public T Do<T>(Func<IDatabase, T> func)
        {
            return func(_conn.GetDatabase(_dbNum));
        }
    }
}